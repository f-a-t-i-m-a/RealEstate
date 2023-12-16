using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using FluentScheduler;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Utils;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.HaftDong.Controllers
{
    [Contract]
    [Component]
    public class ScheduledTaskRegistry : Registry, IEagerComponent
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ScheduledTaskRegistry));
        private const int NumberOfIndexingItems = 10;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        public ScheduledTaskRegistry()
        {
            DefaultAllTasksAsNonReentrant();
        }

        [OnCompositionComplete]
        public void OnCompositionComplete()
        {
            Schedule(() =>
            {
                try
                {
                    ReindexProperty();
                    ReindexSupplies();
                    ReindexCustomer();
                    ReindexRequests();
                    ReindexContract();
                    ReindexUsers();
                }
                catch (Exception e)
                {
                    Log.Error("Unhandled exception in Scheduled Task Registry", e);
                }
            }).ToRunEvery(5).Seconds();

            if (ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Production)
            {
                Schedule(() =>
                {
                    try
                    {
                        PropertyUtil.RetrieveFromKhoonat();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Unhandled exception in Scheduled Task Registry", e);
                    }
                }).ToRunEvery(20).Seconds();
            }

            TaskManager.Initialize(this);
        }

        private void ReindexUsers()
        {
            var userFilter = Builders<ApplicationUser>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<ApplicationUser> users =
                    DbManager.ApplicationUser.Find(userFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (users.Count > 0)
                {
                    var logText = "Starting indexing " + users.Count + " user(s) with ID ";
                    users.ForEach(u =>
                    {
                        logText += u.Id + ", ";
                    });
                    Log.Info(logText);
                }

                users.ForEach(u =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<ApplicationUserIE>(ur => ur
                            .Index(ElasticManager.Index)
                            .Type(Types.UserType)
                            .Query(q =>
                            {
                                QueryContainer query = q.Term(urr => urr.ID, u.Id);
                                return query;
                            })
                            );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed users, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<ApplicationUser, ApplicationUserIE, SearchIE>(Types.UserType, u, u.Id.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<ApplicationUser, ApplicationUserIE, SearchIE>(Types.UserType, u);
                        }

                        var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(u.Id));
                        var update = Builders<ApplicationUser>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.ApplicationUser.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing User with ID {0}, exception: {1}", u.Id, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in User Reindexing", e);
            }
        }

        private void ReindexContract()
        {
            var contractFilter = Builders<Contract>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<Contract> contracts =
                    DbManager.Contract.Find(contractFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (contracts.Count > 0)
                {
                    var logText = "Starting indexing " + contracts.Count + " contract(s) with ID ";
                    contracts.ForEach(c =>
                    {
                        logText += c.ID + ", ";
                    });
                    Log.Info(logText);
                }

                contracts.ForEach(c =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<ContractIE>(cu => cu
                            .Index(ElasticManager.Index)
                            .Type(Types.ContractType)
                            .Query(q =>
                            {
                                QueryContainer query = q.Term(cie => cie.ID, c.ID);
                                return query;
                            })
                            );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed contracts, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<Contract, ContractIE, SearchIE>(Types.ContractType, c, c.ID.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<Contract, ContractIE, SearchIE>(Types.ContractType, c);
                        }

                        var filter = Builders<Contract>.Filter.Eq("ID", c.ID);
                        var update = Builders<Contract>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.Contract.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing Contract with ID {0}, exception: {1}", c.ID, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in Contract Reindexing", e);
            }
        }

        private void ReindexRequests()
        {
            var requestFilter = Builders<Request>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<Request> requests =
                    DbManager.Request.Find(requestFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (requests.Count > 0)
                {
                    var logText = "Starting indexing " + requests.Count + " request(s) with ID ";
                    requests.ForEach(r =>
                    {
                        logText += r.ID + ", ";
                    });
                    Log.Info(logText);
                }

                requests.ForEach(r =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<RequestIE>(re => re
                            .Index(ElasticManager.Index)
                            .Type(Types.RequestType)
                            .Query(q =>
                            {
                                QueryContainer query = q.Term(rie => rie.ID, r.ID);
                                return query;
                            })
                            );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed requests, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<Request, RequestIE, SearchIE>(Types.RequestType, r, r.ID.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<Request, RequestIE, SearchIE>(Types.RequestType, r);
                        }


                        var filter = Builders<Request>.Filter.Eq("ID", r.ID);
                        var update = Builders<Request>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.Request.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing Request with ID {0}, exception: {1}", r.ID, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in Request Reindexing", e);
            }
        }

        private void ReindexCustomer()
        {
            var customerFilter = Builders<Customer>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<Customer> customers =
                    DbManager.Customer.Find(customerFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (customers.Count > 0)
                {
                    var logText = "Starting indexing " + customers.Count + " customer(s) with ID ";
                    customers.ForEach(c =>
                    {
                        logText += c.ID + ", ";
                    });
                    Log.Info(logText);
                }

                customers.ForEach(c =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<CustomerIE>(cu => cu
                        .Index(ElasticManager.Index)
                        .Type(Types.CustomerType)
                        .Query(q =>
                        {
                            QueryContainer query = q.Term(cie => cie.ID, c.ID);
                            return query;
                        })
                        );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed customers, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<Customer, CustomerIE, SearchIE>(Types.CustomerType, c, c.ID.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<Customer, CustomerIE, SearchIE>(Types.CustomerType, c);
                        }

                        var filter = Builders<Customer>.Filter.Eq("ID", c.ID);
                        var update = Builders<Customer>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.Customer.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing Customer with ID {0}, Exception: {1}", c.ID, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in Customer Reindexing", e);
            }
        }

        private void ReindexSupplies()
        {
            var supplyFilter = Builders<Supply>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<Supply> supplies =
                    DbManager.Supply.Find(supplyFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (supplies.Count > 0)
                {
                    var logText = "Starting indexing " + supplies.Count + " supply(ies) with ID ";
                    supplies.ForEach(s =>
                    {
                        logText += s.ID + ", ";
                    });
                    Log.Info(logText);
                }

                supplies.ForEach(s =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<SupplyIE>(si => si
                            .Index(ElasticManager.Index)
                            .Type(Types.SupplyType)
                            .Query(q =>
                            {
                                QueryContainer query = q.Term(prr => prr.ID, s.ID);
                                return query;
                            })
                            );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed supplies, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<Supply, SupplyIE, SearchIE>(Types.SupplyType, s, s.ID.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<Supply, SupplyIE, SearchIE>(Types.SupplyType, s);
                        }

                        var filter = Builders<Supply>.Filter.Eq("ID", s.ID);
                        var update = Builders<Supply>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.Supply.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing Supply with ID {0}, exception: {1}", s.ID, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in Supply Reindexing", e);
            }
        }

        private void ReindexProperty()
        {
            var propertFilter = Builders<Property>.Filter.Eq("LastIndexingTime", BsonNull.Value);
            try
            {
                List<Property> properties =
                    DbManager.Property.Find(propertFilter).Limit(NumberOfIndexingItems).ToListAsync().Result;
                if (properties.Count > 0)
                {
                    var logText = "Starting indexing " + properties.Count + " property(ies) with ID ";
                    properties.ForEach(p =>
                    {
                        logText += p.ID + ", ";
                    });
                    Log.Info(logText);
                }

                properties.ForEach(p =>
                {
                    try
                    {
                        var response = ElasticManager.Client.Search<PropertyIE>(pr => pr
                            .Index(ElasticManager.Index)
                            .Type(Types.PropertyType)
                            .Query(q =>
                            {
                                QueryContainer query = q.Term(prr => prr.ID, p.ID);
                                return query;
                            })
                            );

                        if (!response.IsValid)
                        {
                            ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching existing indexed properties, debug information: {0}",
                                response.DebugInformation);
                        }

                        if (response.Documents.Any())
                        {
                            ElasticManager.ReIndexEntity<Property, PropertyIE, SearchIE>(Types.PropertyType, p, p.ID.ToString());
                        }
                        else
                        {
                            ElasticManager.IndexEntity<Property, PropertyIE, SearchIE>(Types.PropertyType, p);
                        }

                        var filter = Builders<Property>.Filter.Eq("ID", p.ID);
                        var update = Builders<Property>.Update.Set("LastIndexingTime", DateTime.Now);
                        DbManager.Property.UpdateOneAsync(filter, update);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat("Unhandled exception in Reindexing Property with ID {0}, exception: {1}", p.ID, e);
                    }
                });
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in Property Reindexing", e);
            }
        }
    }
}