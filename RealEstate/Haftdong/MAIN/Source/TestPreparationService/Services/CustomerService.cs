using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Services
{
    [Contract]
    [Component]
    public class CustomerService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }
        public bool SaveCustomer(Customer input)
        {
            DbManager.Customer.InsertOneAsync(input).Wait();
            return true;
        }

        public Customer GetCustomer(string id)
        {
            var filter = Builders<Customer>.Filter.Eq("ID", ObjectId.Parse(id));
            var customer =
                DbManager.Customer.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            return customer;
        }

    }
}