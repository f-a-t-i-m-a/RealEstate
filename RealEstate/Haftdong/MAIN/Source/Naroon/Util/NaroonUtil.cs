using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Naroon.Models;
using JahanJooy.RealEstateAgency.Naroon.Server.Controller;
using JahanJooy.RealEstateAgency.Util.Resources;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using log4net;
using MongoDB.Driver;
using ServiceStack;
using ServiceStack.Text;

// ReSharper disable PossibleNullReferenceException

namespace JahanJooy.RealEstateAgency.Naroon.Util
{
    [Contract]
    [Component]
    public class NaroonUtil : ExtendedApiController
    {
        private const string ConnectionStringName = "MongoConnection";
        private static readonly ILog Log = LogManager.GetLogger(typeof(NaroonUtil));
        public readonly int BlockSize = 50;
        public readonly string SourceBaseAddress = "http://www.naroon.com/";
        public string DestinationBaseAddress;
        public string OathToken = "";

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        public void RetrieveFromNaroon(string fromDate, string toDate)
        {
            Log.Debug("Retrieving data from Naroon starts");

            XmlDocument dataXmlDoc;
            XmlDocument subDataXmlDoc;

            var loginResult = LoginHaftDong();
            if (!loginResult)
            {
                Log.Error("Could not log in Haftdong!");
            }

            var getXmlResult = GetXmlFromNaroon(fromDate, toDate, out dataXmlDoc, out subDataXmlDoc);
            if (!getXmlResult)
            {
                Log.Error("Could not get Naroon files!");
                return;
            }

            var saveResult = SaveXmlFiles(fromDate, toDate, dataXmlDoc, subDataXmlDoc);
            if (!saveResult)
            {
                Log.Error("Could not save Naroon files!");
            }

            var files = ReadXml(dataXmlDoc, subDataXmlDoc);

            if (loginResult)
            {
                SaveFiles(files);
            }

            Log.InfoFormat("Retrieving data from Naroon ends, read {0} files.", files.Count);
        }

        private bool SaveXmlFiles(string fromDate, string toDate, XmlDocument dataXmlDoc, XmlDocument subDataXmlDoc)
        {
            try
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];

                var connectionString = connectionStringSettings?.ConnectionString;
                if (connectionString == null)
                {
                    Log.Error("Could not found database");
                    return false;
                }

                var mongoUrl = MongoUrl.Create(connectionString);
                var databaseName = mongoUrl.DatabaseName;
                var client = new MongoClient(mongoUrl);
                var database = client.GetDatabase(databaseName);
                var xmlDocCollection = database.GetCollection<XmlDoc>(typeof(XmlDoc).Name);

                xmlDocCollection.InsertOneAsync(new XmlDoc
                {
                    FetchDateFrom = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FetchDateTo = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    DataContent = dataXmlDoc.InnerXml,
                    SubDataContent = subDataXmlDoc.InnerXml
                })
                    .Wait();
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("An error occured during saving XML file, exception: {0}", e);
                return false;
            }
        }

        private void SaveFiles(List<ExternalNewFileInput> files)
        {
            for (var i = 0; i <= (files.Count/BlockSize); i++)
            {
                var input = new ExternalNewFilesInput
                {
                    Files = files.GetRange(i*BlockSize, Math.Min(BlockSize, files.Count - (i*BlockSize)))
                };
                var saveResult = SendPostRequest("/api/external/externalfiles/save/batch", input);
                if (!saveResult.IsSuccessStatusCode)
                {
                    Log.Error("There was an error during saving files in Haftdong");
                }
            }
        }

        private bool LoginHaftDong()
        {
            var userName = ConfigurationManager.AppSettings["username"];
            if (userName == null)
                return false;

            var password = ConfigurationManager.AppSettings["password"];
            if (password == null)
                return false;

            DestinationBaseAddress = ConfigurationManager.AppSettings["destination"];
            if (DestinationBaseAddress == null)
                return false;

            var client = new HttpClient {BaseAddress = new Uri(DestinationBaseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent("grant_type=password&username=" + userName + "&password=" + password,
                Encoding.UTF8, "application/json");
            var response = client.PostAsync("/token", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var jsonBody = JsonObject.Parse(body);
                OathToken = jsonBody["access_token"];
                return true;
            }

            return false;
        }

        private bool GetXmlFromNaroon(string fromDate, string toDate, out XmlDocument dataXmlDoc,
            out XmlDocument subDataXmlDoc)
        {
            dataXmlDoc = new XmlDocument();
            subDataXmlDoc = new XmlDocument();
            try
            {
//                dataXmlDoc.Load("D:\\test\\data.xml");
//                subDataXmlDoc.Load("D:\\test\\subdata.xml");

                var fileName = Guid.NewGuid();
                var createResponse = SendGetRequest(
                    "?content=webservice&filename=" + fileName +
                    "&user=jahanjooy&password=BlackDiamond!&datefrom=" + fromDate + "&dateto=" + toDate);

                if (!createResponse.IsSuccessStatusCode)
                {
                    Log.Error("Could not create XML files from Naroon");
                    return false;
                }

                var dataResponse = SendGetRequest("http://www.naroon.com/download/jahanjooy_" + fileName + "_data.xml");
                var subDataResponse =
                    SendGetRequest("http://www.naroon.com/download/jahanjooy_" + fileName + "_subdata.xml");

                if (dataResponse.IsSuccessStatusCode)
                {
                    var dataBody = dataResponse.Content.ReadAsStringAsync().Result;
                    var dataBodyChar = dataBody.ToCharArray();
                    NormalizerUtil.NormalizeToPersian(dataBodyChar, dataBody.Length);
                    var dataBodyStr = new string(dataBodyChar);
                    dataXmlDoc.LoadXml(dataBodyStr);
                }

                if (subDataResponse.IsSuccessStatusCode)
                {
                    var subDataBody = subDataResponse.Content.ReadAsStringAsync().Result;
                    var subDataBodyChar = subDataBody.ToCharArray();
                    NormalizerUtil.NormalizeToPersian(subDataBodyChar, subDataBody.Length);
                    var subDataBodyStr = new string(subDataBodyChar);
                    subDataXmlDoc.LoadXml(subDataBodyStr);
                }

                var finishResponse = SendGetRequest("?content=webservice&user=jahanjooy&execute=finished");
                if (!finishResponse.IsSuccessStatusCode)
                {
                    Log.Error("Could not delete XML files from Naroon");
                }
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Could not get XML files from Naroon, exception: {0}", e);
                return false;
            }
        }

        private List<ExternalNewFileInput> ReadXml(XmlDocument dataXmlDoc, XmlDocument subDataXmlDoc)
        {
            var files = new List<ExternalNewFileInput>();

            try
            {
                var dataNodes = dataXmlDoc.SelectNodes("/DocumentElement/data");
                var subDataNodes = subDataXmlDoc.SelectNodes("/DocumentElement/subdata");

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (XmlNode node in dataNodes)
                {
                    var file = ReadData(node);
                    files.Add(file);
                }

                foreach (XmlNode node in subDataNodes)
                {
                    var result = ReadSubData(node, files);
                    if (!result)
                    {
                        Log.ErrorFormat("Subdata could not saved, Inner XML: {0}", node.InnerXml);
                    }
                }

                foreach (var file in files)
                {
                    if (file.Property.ConversionWarning == null || file.Property.ConversionWarning == false)
                        file.Property.ExternalDetails = "";
                }
                var finalResult = files.GroupBy(f => f.Property.CorrelationID);
                finalResult.ForEach(r =>
                {
                    if (r.Count() > 1)
                    {
                        var firstFile =
                            files.SingleOrDefault(f => f.Property.IsMasterBuiling && f.Property.CorrelationID == r.Key);
                        var fileJson = firstFile.ToJson();
                        var masterFile = fileJson.FromJson<ExternalNewFileInput>();

                        masterFile.Property.PropertyType = PropertyType.Tenement;
                        masterFile.Property.UnitArea = r.Sum(f => f.Property.UnitArea);
                        masterFile.Property.StorageRoomArea = r.Sum(f => f.Property.StorageRoomArea);
                        masterFile.Property.NumberOfRooms = r.Sum(f => f.Property.NumberOfRooms);
                        masterFile.Property.NumberOfParkings = r.Sum(f => f.Property.NumberOfParkings);

                        files.Add(masterFile);

                        firstFile.Property.ExternalID = firstFile.Property.ExternalID + "-" + 1;
                        firstFile.Supply.ExternalID = firstFile.Supply.ExternalID + "-" + 1;
                        firstFile.Property.IsMasterBuiling = false;
                    }
                });
            }
            catch (Exception e)
            {
                Log.ErrorFormat("An error occured during reading XML data, exception: {0}", e);
            }

            return files;
        }

        private ExternalNewFileInput ReadData(XmlNode node)
        {
            var file = new ExternalNewFileInput
            {
                Property = new Property(),
                Supply = new Supply()
            };

            try
            {
                //Property
                file.Property.ExternalDetails = node.InnerXml;
                file.Property.ExternalID =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Code")?.InnerText)
                        ? node.SelectSingleNode("Code").InnerText
                        : "";
                file.Property.CreationTime =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Date_")?.InnerText)
                        ? DateTime.Parse(node.SelectSingleNode("Date_").InnerText)
                        : DateTime.Now;
                file.Property.Address = node.SelectSingleNode("Address")?.InnerText;
                file.Property.MainDaylightDirection = GetMainDaylightDirection(node);
                file.Property.UnitFloorNumber =
                    !string.IsNullOrEmpty(node.SelectSingleNode("FloorNum")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("FloorNum").InnerText)
                        : 0;
                file.Property.NumberOfUnitsPerFloor =
                    !string.IsNullOrEmpty(node.SelectSingleNode("UnitNum")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("UnitNum").InnerText)
                        : 0;
                file.Property.AdditionalSpecialFeatures = GetAdditionalComments(node);
                file.Property.HasElevator =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Lift")?.InnerText)
                        ? GetBooleanValue(node, "Lift")
                        : null;
                file.Property.HasSwimmingPool =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Pool")?.InnerText)
                        ? GetBooleanValue(node, "Pool")
                        : null;
                file.Property.EstateArea =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Area")?.InnerText)
                        ? long.Parse(node.SelectSingleNode("Area").InnerText)
                        : 0;
                file.Property.PassageEdgeLength =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Front")?.InnerText)
                        ? long.Parse(node.SelectSingleNode("Front").InnerText)
                        : 0;
                file.Property.HasPrivatePatio =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Patio")?.InnerText)
                        ? GetBooleanValue(node, "Patio")
                        : null;
                file.Property.PropertyStatus = GetPropertyStatus(node);
                file.Property.FaceType = GetFaceType(node);
                file.Property.PropertyType = GetPropertyType(node, file.Property);
                file.Property.BuildingAgeYears =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Age")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("Age").InnerText)
                        : 0;
                file.Property.HasSauna = GetBooleanValue(node, "Sona");
                file.Property.HasJacuzzi = GetBooleanValue(node, "Jakoozi");

                file.Property.LastModificationTime =
                    !string.IsNullOrEmpty(node.SelectSingleNode("Pdate")?.InnerText)
                        ? DateTime.Parse(node.SelectSingleNode("Pdate").InnerText)
                        : DateTime.Now;

                file.Property.UsageType = GetUsageType(node, file.Property);
                file.Property.State = PropertyState.New;
                file.Property.SourceType = SourceType.Other;
                file.Property.Vicinity = GetVicinity(node);
                file.Property.CorrelationID = Guid.NewGuid();
                file.Property.IsMasterBuiling = true;

                //Supply
                var intentionOfOwner = GetIntentionOfOwner(node, file.Property);
                file.Supply.ExternalID = !string.IsNullOrEmpty(node.SelectSingleNode("Code")?.InnerText)
                    ? node.SelectSingleNode("Code").InnerText
                    : "";

                file.Supply.OwnerCanBeContacted = true;
                var ownerPhoneNumbers = new List<PhoneInfo>();

                if (!node.SelectSingleNode("Tel1").InnerText.IsNullOrEmpty())
                    ownerPhoneNumbers.Add(new PhoneInfo
                    {
                        Value = node.SelectSingleNode("Tel1").InnerText
                    });

                if (!node.SelectSingleNode("Tel2").InnerText.IsNullOrEmpty())
                    ownerPhoneNumbers.Add(new PhoneInfo
                    {
                        Value = node.SelectSingleNode("Tel2").InnerText
                    });

                if (!node.SelectSingleNode("Tel3").InnerText.IsNullOrEmpty())
                    ownerPhoneNumbers.Add(new PhoneInfo
                    {
                        Value = node.SelectSingleNode("Tel3").InnerText
                    });

                file.Supply.OwnerContact = new ContactMethodCollection
                {
                    ContactName = node.SelectSingleNode("Owner")?.InnerText,
                    Phones = ownerPhoneNumbers
                };

                file.Supply.IntentionOfOwner = intentionOfOwner;
                file.Supply.PriceSpecificationType =
                    !string.IsNullOrEmpty(node.SelectSingleNode("TotalPrice")?.InnerText)
                        ? SalePriceSpecificationType.Total
                        : SalePriceSpecificationType.PerEstateArea;
                file.Supply.PricePerEstateArea = (intentionOfOwner == IntentionOfOwner.ForSale) ? GetUnitPrice(node) : 0;
                file.Supply.PricePerUnitArea = 0;
                file.Supply.Rent = (intentionOfOwner == IntentionOfOwner.ForRent) ? GetUnitPrice(node) : 0;
                file.Supply.Mortgage = ((intentionOfOwner == IntentionOfOwner.ForRent) &&
                                        (!string.IsNullOrEmpty(node.SelectSingleNode("TotalPrice")?.InnerText)))
                    ? decimal.Parse(node.SelectSingleNode("TotalPrice").InnerText)
                    : 0;
                file.Supply.TotalPrice = (intentionOfOwner == IntentionOfOwner.ForSale) ? CalculateTotalPrice(node) : 0;
                file.Supply.AdditionalRentalComments =
                    !string.IsNullOrEmpty(node.SelectSingleNode("PriceComment")?.InnerText)
                        ? node.SelectSingleNode("PriceComment").InnerText
                        : "";
                file.Supply.LastModificationTime = !string.IsNullOrEmpty(node.SelectSingleNode("Pdate")?.InnerText)
                    ? DateTime.Parse(node.SelectSingleNode("Pdate").InnerText)
                    : DateTime.Now;
                file.Supply.CreationTime = !string.IsNullOrEmpty(node.SelectSingleNode("Date_")?.InnerText)
                    ? DateTime.Parse(node.SelectSingleNode("Date_").InnerText)
                    : DateTime.Now;
                file.Supply.State = SupplyState.New;
            }
            catch (Exception e)
            {
                var message = Environment.NewLine + e.Message + Environment.NewLine + e.InnerException;
                SetConversionWarning(file.Property, message, true);
                Log.ErrorFormat("An error occured during reading node data, exception: {0}, Inner XML: {1}", e,
                    node.InnerXml);
            }
            return file;
        }

        private bool ReadSubData(XmlNode node, List<ExternalNewFileInput> files)
        {
            var externalID =
                !string.IsNullOrEmpty(node.SelectSingleNode("code")?.InnerText)
                    ? node.SelectSingleNode("code").InnerText
                    : "";
            var file = files.SingleOrDefault(f => f.Property.ExternalID == externalID && f.Property.IsMasterBuiling);

            if (file == null)
            {
                Log.ErrorFormat("In during reading sub data, file with External ID {0} could not found", externalID);
                return false;
            }

            try
            {
                file.Property.ExternalDetails += node.InnerXml;
                var row = !string.IsNullOrEmpty(node.SelectSingleNode("Row")?.InnerText)
                    ? int.Parse(node.SelectSingleNode("Row").InnerText)
                    : 1;


                if (row == 1)
                {
                    file.Property.UnitFloorNumber = GetFloorNumber(node);
                    file.Property.NumberOfRooms = !string.IsNullOrEmpty(node.SelectSingleNode("Bedroom")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("Bedroom").InnerText)
                        : 0;
                    file.Property.UnitArea = !string.IsNullOrEmpty(node.SelectSingleNode("UsedArea")?.InnerText)
                        ? decimal.Parse(node.SelectSingleNode("UsedArea").InnerText)
                        : 0;
                    file.Property.HasIranianLavatory = HasIranianLavatory(node);
                    file.Property.HasForeignLavatory = HasForeignLavatory(node);
                    file.Property.LivingRoomFloor = GetFloorCoverType(node);
                    file.Property.NumberOfParkings = (!string.IsNullOrEmpty(node.SelectSingleNode("Garage")?.InnerText)
                                                      &&
                                                      node.SelectSingleNode("Garage")
                                                          .InnerText.ToLower()
                                                          .Equals("true"))
                        ? 1
                        : 0;
                    file.Property.StorageRoomArea = (!string.IsNullOrEmpty(node.SelectSingleNode("Anbari")?.InnerText)
                                                     &&
                                                     node.SelectSingleNode("Anbari").InnerText.ToLower().Equals("true"))
                        ? 3
                        : 0; //Smallest storageroom is about 3 meters

                    if (file.Property.UnitArea != 0)
                    {
                        file.Supply.PriceSpecificationType = SalePriceSpecificationType.PerUnitArea;
                        file.Supply.PricePerUnitArea = file.Supply.PricePerEstateArea;
                        file.Supply.PricePerEstateArea = 0;
                        file.Supply.TotalPrice = file.Property.UnitArea*file.Supply.PricePerUnitArea;
                    }
                }
                else
                {
                    var fileJson = file.ToJson();
                    var newFile = fileJson.FromJson<ExternalNewFileInput>();

                    newFile.Property.ExternalID = newFile.Property.ExternalID + "-" + row;

                    newFile.Property.UnitFloorNumber = GetFloorNumber(node);
                    newFile.Property.NumberOfRooms = !string.IsNullOrEmpty(node.SelectSingleNode("Bedroom")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("Bedroom").InnerText)
                        : 0;
                    newFile.Property.UnitArea = !string.IsNullOrEmpty(node.SelectSingleNode("UsedArea")?.InnerText)
                        ? decimal.Parse(node.SelectSingleNode("UsedArea").InnerText)
                        : 0;
                    newFile.Property.HasIranianLavatory = HasIranianLavatory(node);
                    newFile.Property.HasForeignLavatory = HasForeignLavatory(node);
                    newFile.Property.LivingRoomFloor = GetFloorCoverType(node);
                    newFile.Property.NumberOfParkings = (!string.IsNullOrEmpty(
                        node.SelectSingleNode("Garage")?.InnerText)
                                                         &&
                                                         node.SelectSingleNode("Garage")
                                                             .InnerText.ToLower()
                                                             .Equals("true"))
                        ? 1
                        : 0;
                    newFile.Property.StorageRoomArea = (!string.IsNullOrEmpty(node.SelectSingleNode("Anbari")?.InnerText)
                                                        &&
                                                        node.SelectSingleNode("Anbari")
                                                            .InnerText.ToLower()
                                                            .Equals("true"))
                        ? 3
                        : 0; //Smallest storageroom is about 3 meters
                    if (newFile.Property.UnitArea != 0)
                    {
                        newFile.Supply.PriceSpecificationType = SalePriceSpecificationType.PerUnitArea;
                        newFile.Supply.PricePerUnitArea = newFile.Supply.PricePerEstateArea;
                        newFile.Supply.PricePerEstateArea = 0;
                        newFile.Supply.TotalPrice = newFile.Property.UnitArea*newFile.Supply.PricePerUnitArea;
                    }

                    newFile.Supply.ExternalID = newFile.Supply.ExternalID + "-" + row;
                    newFile.Property.CorrelationID = file.Property.CorrelationID;
                    newFile.Property.IsMasterBuiling = false;

                    files.Add(newFile);
                }
                return true;
            }
            catch (Exception e)
            {
                var message = Environment.NewLine + e.Message + Environment.NewLine + e.InnerException;
                SetConversionWarning(file.Property, message, true);
                Log.ErrorFormat("An error occured during reading node sub data, exception: {0}, Inner XML: {1}", e,
                    node.InnerXml);
            }
            return false;
        }

        private static void SetConversionWarning(Property property, string message, bool setHidden)
        {
            property.IsHidden = setHidden;
            property.ConversionWarning = true;
            property.ExternalDetails += message;
        }

        private int? GetFloorNumber(XmlNode node)
        {
            if (string.IsNullOrEmpty(node.SelectSingleNode("Floor")?.InnerText))
            {
                return 0;
            }

            return NumberUtil.FullyTextualOrdinalNumber(node.SelectSingleNode("Floor").InnerText);
        }

        private VicinityReference GetVicinity(XmlNode node)
        {
            var vicinity = new VicinityReference
            {
                Region =
                    !string.IsNullOrEmpty(node.SelectSingleNode("NewReg")?.InnerText)
                        ? int.Parse(node.SelectSingleNode("NewReg").InnerText)
                        : 0
            };
            return vicinity;
        }

        private FloorCoverType? GetFloorCoverType(XmlNode node)
        {
            if (string.IsNullOrEmpty(node.SelectSingleNode("FloorKind")?.InnerText))
            {
                return null;
            }

            var coverType = EnumTranslatorUtil.GetByteOfEnum(typeof(FloorCoverType),
                node.SelectSingleNode("FloorKind").InnerText);
            if (coverType != 0)
                return (FloorCoverType) coverType;

            return FloorCoverType.Other;
        }

        private bool? HasIranianLavatory(XmlNode node)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Wc")?.InnerText)
                && node.SelectSingleNode("Wc").InnerText.Contains("ایرانی"))
                return true;
            return null;
        }

        private bool? HasForeignLavatory(XmlNode node)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Wc")?.InnerText)
                && node.SelectSingleNode("Wc").InnerText.Contains("فرنگی"))
                return true;
            return null;
        }

        private UsageType? GetUsageType(XmlNode node, Property property)
        {
            if (string.IsNullOrEmpty(node.SelectSingleNode("DucKind")?.InnerText))
            {
                switch (property.PropertyType)
                {
                    case PropertyType.Apartment:
                    case PropertyType.Villa:
                    case PropertyType.House:
                    case PropertyType.Tenement:
                    case PropertyType.OldHouse:
                    case PropertyType.Suite:
                        return UsageType.Residency;
                    case PropertyType.Office:
                        return UsageType.Office;
                    case PropertyType.Shop:
                        return UsageType.Shop;
                    default:
                        var innerMessage = Environment.NewLine + "Unit usage type has error.";
                        SetConversionWarning(property, innerMessage, false);
                        return UsageType.Unknown;
                }
            }

            switch (node.SelectSingleNode("DucKind").InnerText)
            {
                case "مسکونی":
                    return UsageType.Residency;
                case "م-اداری":
                    return UsageType.Office;
                case "تجاری":
                    return UsageType.Shop;
                case "سند اداری":
                    return UsageType.Office;
                case "شخصی":
                    switch (property.PropertyType)
                    {
                        case PropertyType.Apartment:
                        case PropertyType.Villa:
                        case PropertyType.House:
                        case PropertyType.Tenement:
                        case PropertyType.OldHouse:
                        case PropertyType.Suite:
                            return UsageType.Residency;
                        case PropertyType.Office:
                            return UsageType.Office;
                        case PropertyType.Shop:
                            return UsageType.Shop;
                        default:
                            var innerMessage = Environment.NewLine + "Unit usage type has error.";
                            SetConversionWarning(property, innerMessage, false);
                            return UsageType.Unknown;
                    }
                default:
                    var outerMessage = Environment.NewLine + "Unit usage type has error.";
                    SetConversionWarning(property, outerMessage, false);
                    return UsageType.Unknown;
            }
        }

        private PropertyType GetPropertyType(XmlNode node, Property property)
        {
            string message;
            if (string.IsNullOrEmpty(node.SelectSingleNode("CaseKind")?.InnerText))
            {
                message = Environment.NewLine + "Property type has error1.";
                SetConversionWarning(property, message, true);
                return PropertyType.Apartment;
            }

            if (node.SelectSingleNode("CaseKind").InnerText.Equals("دفتر کار"))
            {
                return PropertyType.Office;
            }

            if (node.SelectSingleNode("CaseKind").InnerText.Contains("مستغلات"))
            {
                return PropertyType.Tenement;
            }

            var propertyType = EnumTranslatorUtil.GetByteOfEnum(typeof(PropertyType),
                node.SelectSingleNode("CaseKind").InnerText);
            if (propertyType != 0)
                return (PropertyType) propertyType;

            message = Environment.NewLine + "Property type has error2.";
            SetConversionWarning(property, message, true);
            return PropertyType.Apartment;
        }

        private string GetAdditionalComments(XmlNode node)
        {
            var comment = "";
            if (!string.IsNullOrEmpty(node.SelectSingleNode("UnitComment")?.InnerText))
            {
                comment += node.SelectSingleNode("UnitComment").InnerText + " ";
            }

            if (!string.IsNullOrEmpty(node.SelectSingleNode("Comment")?.InnerText))
            {
                comment += node.SelectSingleNode("Comment").InnerText;
            }

            return comment;
        }

        private BuildingFaceType? GetFaceType(XmlNode node)
        {
            var faceType = node.SelectSingleNode("FrontKind")?.InnerText;
            if (string.IsNullOrEmpty(faceType))
                return null;

            var faceTypeByte = EnumTranslatorUtil.GetByteOfEnum(typeof(BuildingFaceType), faceType);
            if (faceTypeByte != 0)
                return (BuildingFaceType) faceTypeByte;

            return BuildingFaceType.Other;
        }

        private PropertyStatus? GetPropertyStatus(XmlNode node)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Maskoni")?.InnerText) &&
                node.SelectSingleNode("Maskoni").InnerText.ToLower().Equals("true"))
            {
                return PropertyStatus.NoOccupantYet;
            }

            if (!string.IsNullOrEmpty(node.SelectSingleNode("Empty")?.InnerText) &&
                node.SelectSingleNode("Empty").InnerText.ToLower().Equals("true"))
            {
                return PropertyStatus.Emptied;
            }

            if (!string.IsNullOrEmpty(node.SelectSingleNode("Rented")?.InnerText) &&
                node.SelectSingleNode("Rented").InnerText.ToLower().Equals("true"))
            {
                return PropertyStatus.OccupiedByRenter;
            }

            return null;
        }

        private bool? GetBooleanValue(XmlNode node, string tagName)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode(tagName)?.InnerText) &&
                node.SelectSingleNode(tagName).InnerText.ToLower().Equals("true"))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(node.SelectSingleNode(tagName)?.InnerText) &&
                node.SelectSingleNode(tagName).InnerText.ToLower().Equals("false"))
            {
                return false;
            }
            return null;
        }

        private decimal? CalculateTotalPrice(XmlNode node)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("TotalPrice")?.InnerText) &&
                !node.SelectSingleNode("TotalPrice").InnerText.Equals("0"))
            {
                return decimal.Parse(node.SelectSingleNode("TotalPrice").InnerText);
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Area")?.InnerText) &&
                !string.IsNullOrEmpty(node.SelectSingleNode("UnitPrice")?.InnerText))
            {
                return decimal.Parse(node.SelectSingleNode("Area").InnerText)*
                       decimal.Parse(node.SelectSingleNode("UnitPrice").InnerText);
            }
            return 0;
        }

        private static decimal GetUnitPrice(XmlNode node)
        {
            return !string.IsNullOrEmpty(node.SelectSingleNode("UnitPrice")?.InnerText)
                ? decimal.Parse(node.SelectSingleNode("UnitPrice").InnerText)
                : 0;
        }

        private static IntentionOfOwner GetIntentionOfOwner(XmlNode node, Property property)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Sale")?.InnerText) &&
                node.SelectSingleNode("Sale").InnerText.ToLower().Equals("true"))
            {
                return IntentionOfOwner.ForSale;
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Rent")?.InnerText) &&
                node.SelectSingleNode("Rent").InnerText.ToLower().Equals("true"))
            {
                return IntentionOfOwner.ForRent;
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("Rahn")?.InnerText) &&
                node.SelectSingleNode("Rahn").InnerText.ToLower().Equals("true"))
            {
                return IntentionOfOwner.ForFullMortgage;
            }

            var message = Environment.NewLine + "Intention of owner has error.";
            SetConversionWarning(property, message, true);
            return IntentionOfOwner.ForSale;
        }

        private static DaylightDirection? GetMainDaylightDirection(XmlNode node)
        {
            if (!string.IsNullOrEmpty(node.SelectSingleNode("North")?.InnerText) &&
                node.SelectSingleNode("North").InnerText.ToLower().Equals("true"))
            {
                return DaylightDirection.North;
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("South")?.InnerText) &&
                node.SelectSingleNode("South").InnerText.ToLower().Equals("true"))
            {
                return DaylightDirection.South;
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("East")?.InnerText) &&
                node.SelectSingleNode("East").InnerText.ToLower().Equals("true"))
            {
                return DaylightDirection.East;
            }
            if (!string.IsNullOrEmpty(node.SelectSingleNode("West")?.InnerText) &&
                node.SelectSingleNode("West").InnerText.ToLower().Equals("true"))
            {
                return DaylightDirection.West;
            }

            return null;
        }

        private HttpResponseMessage SendGetRequest(string url)
        {
            var client = new HttpClient {BaseAddress = new Uri(SourceBaseAddress)};
            return client.GetAsync(url).Result;
        }

        private HttpResponseMessage SendPostRequest(string url, object contentObject)
        {
            var client = new HttpClient {BaseAddress = new Uri(DestinationBaseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OathToken);

            var content = new StringContent(contentObject.ToJson(), Encoding.UTF8, "application/json");
            return client.PostAsync(url, content).Result;
        }
    }
}