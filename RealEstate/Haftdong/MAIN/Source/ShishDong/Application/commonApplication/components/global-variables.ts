module JahanJooy.ShishDong {
    import PropertyType = RealEstateAgency.Domain.Enums.Property.PropertyType;
    import UsageType = RealEstateAgency.Domain.Enums.Property.UsageType;
    import IntentionOfOwner = RealEstateAgency.Domain.Enums.Property.IntentionOfOwner;
    import SalePriceSpecificationType = RealEstateAgency.Domain.Enums.Property.SalePriceSpecificationType;

    var globalModule = angular.module('realEstateGlobal', []);

    globalModule.constant('mapApiKey', "AIzaSyDlE2V9O9FzosTGXcX6SPQatAukHsQwP6U");

    globalModule.constant('allUsagesPerProperty', [
        {
            propertyType: PropertyType.Land,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Shop,
                UsageType.Industrial,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Tenement,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Shop,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Shed,
            usages: [
                UsageType.Other,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Garden,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Agricultural,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Complex,
            usages: [
                UsageType.Residency,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Tower,
            usages: [
                UsageType.Residency,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.GardenTower,
            usages: [
                UsageType.Residency,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.House,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Villa,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.OldHouse,
            usages: [
                UsageType.Residency,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Office,
            usages: [
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.OfficialResidency,
            usages: [
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Apartment,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Suite,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Penthouse,
            usages: [
                UsageType.Residency,
                UsageType.Office,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Shop,
            usages: [
                UsageType.Shop,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Commercial,
            usages: [
                UsageType.Shop,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.CommercialResidency,
            usages: [
                UsageType.Shop,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.AgriculturalLand,
            usages: [
                UsageType.Agricultural,
                UsageType.Mixed,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Factory,
            usages: [
                UsageType.Industrial,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.WorkShop,
            usages: [
                UsageType.Industrial,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.RepairShop,
            usages: [
                UsageType.Industrial,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.StoreHouse,
            usages: [
                UsageType.Other,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Gym,
            usages: [
                UsageType.Other,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.Parking,
            usages: [
                UsageType.Other,
                UsageType.Unknown
            ]
        },
        {
            propertyType: PropertyType.CityService,
            usages: [
                UsageType.Other,
                UsageType.Unknown
            ]
        }
    ]);

    globalModule.constant('allPriceSpecificationTypePerProperty', [
        {
            propertyType: PropertyType.Land,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Garden,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Tenement,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Shed,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Complex,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Tower,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.GardenTower,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.House,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.OldHouse,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Office,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.OfficialResidency,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Villa,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Apartment,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Penthouse,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Suite,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Commercial,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.CommercialResidency,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.AgriculturalLand,
            priceTypes: [
                SalePriceSpecificationType.PerEstateArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Shop,
            priceTypes: [
                SalePriceSpecificationType.PerUnitArea,
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Factory,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.WorkShop,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.RepairShop,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.StoreHouse,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Parking,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.Gym,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        },
        {
            propertyType: PropertyType.CityService,
            priceTypes: [
                SalePriceSpecificationType.Total
            ]
        }
    ]);

    globalModule.constant('allPropertiesPerUsage', [
        {
            usage: UsageType.Residency,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse
            ]
        },
        {
            usage: UsageType.Agricultural,
            properties: [
                PropertyType.Garden,
                PropertyType.AgriculturalLand
            ]
        },
        {
            usage: UsageType.Office,
            properties: [
                PropertyType.Tenement,
                PropertyType.Land,
                PropertyType.Office,
                PropertyType.OfficialResidency
            ]
        },
        {
            usage: UsageType.Shop,
            properties: [
                PropertyType.Land,
                PropertyType.Shop,
                PropertyType.Tenement,
                PropertyType.Commercial,
                PropertyType.CommercialResidency
            ]
        },
        {
            usage: UsageType.Industrial,
            properties: [
                PropertyType.Land,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop
            ]
        },
        {
            usage: UsageType.Mixed,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ]
        },
        {
            usage: UsageType.Other,
            properties: [
                PropertyType.Shed,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ]
        },
        {
            usage: UsageType.Unknown,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ]
        }
    ]);


    globalModule.constant('allPropertiesPerIntention', [
        {
            intention: IntentionOfOwner.ForCooperation,
            properties: [
                PropertyType.Land,
                PropertyType.OldHouse,
                PropertyType.Apartment
            ],
            usages: []
        },
        {
            intention: IntentionOfOwner.ForRent,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ],
            usages: []
        },
        {
            intention: IntentionOfOwner.ForSale,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ],
            usages: []
        }, {
            intention: IntentionOfOwner.ForFullMortgage,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ],
            usages: []
        }, {
            intention: IntentionOfOwner.ForDailyRent,
            properties: [
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.StoreHouse,
                PropertyType.Parking
            ],
            usages: []
        }, {
            intention: IntentionOfOwner.ForSwap,
            properties: [
                PropertyType.Land,
                PropertyType.Garden,
                PropertyType.House,
                PropertyType.Villa,
                PropertyType.Apartment,
                PropertyType.Penthouse,
                PropertyType.Suite,
                PropertyType.Tenement,
                PropertyType.Complex,
                PropertyType.Tower,
                PropertyType.GardenTower,
                PropertyType.OldHouse,
                PropertyType.Office,
                PropertyType.OfficialResidency,
                PropertyType.Shop,
                PropertyType.Commercial,
                PropertyType.CommercialResidency,
                PropertyType.Shed,
                PropertyType.AgriculturalLand,
                PropertyType.Factory,
                PropertyType.WorkShop,
                PropertyType.RepairShop,
                PropertyType.Gym,
                PropertyType.StoreHouse,
                PropertyType.CityService,
                PropertyType.Parking
            ],
            usages: []
        }
    ]);


    globalModule.constant('allEstateAreaRanges', [
        {
            id: 1,
            text: "کمتر از 100 متر",
            min: null,
            max: 100
        },
        {
            id: 2,
            text: "100-300 متر",
            min: 100,
            max: 300
        },
        {
            id: 3,
            text: "300-500 متر",
            min: 300,
            max: 500
        },
        {
            id: 4,
            text: "500-1000 متر",
            min: 500,
            max: 1000
        },
        {
            id: 5,
            text: "بیشتر از 1000 متر",
            min: 1000,
            max: null
        }
    ]);

    globalModule.constant('allUnitAreaRanges', [
        {
            id: 1,
            text: "کمتر از 50 متر",
            min: null,
            max: 50
        },
        {
            id: 2,
            text: "50-100 متر",
            min: 50,
            max: 100
        },
        {
            id: 3,
            text: "100-200 متر",
            min: 100,
            max: 200
        },
        {
            id: 4,
            text: "200-400 متر",
            min: 200,
            max: 400
        },
        {
            id: 5,
            text: "بیشتر از 400 متر",
            min: 400,
            max: null
        }
    ]);

    globalModule.constant('allNumberOfRoomRanges', [
        {
            id: 1,
            text: "1 یا هیچ",
            min: null,
            max: 1
        },
        {
            id: 2,
            text: "2",
            min: 2,
            max: 2
        },
        {
            id: 3,
            text: "3",
            min: 3,
            max: 3
        },
        {
            id: 4,
            text: "4",
            min: 4,
            max: 4
        },
        {
            id: 5,
            text: "5+",
            min: 5,
            max: null
        }
    ]);

    globalModule.constant('allMortgageRanges', [
        {
            id: 1,
            text: "کمتر از 10,000,000 تومان",
            min: null,
            max: 10000000
        },
        {
            id: 2,
            text: "10,000,000-30,000,000 تومان",
            min: 10000000,
            max: 30000000
        },
        {
            id: 3,
            text: "30,000,000-50,000,000 تومان",
            min: 30000000,
            max: 50000000
        },
        {
            id: 4,
            text: "50,000,000-100,000,000 تومان",
            min: 50000000,
            max: 100000000
        },
        {
            id: 5,
            text: "بیشتر از 100,000,000 تومان",
            min: 100000000,
            max: null
        }
    ]);

    globalModule.constant('allRentRanges', [
        {
            id: 1,
            text: "کمتر از 200,000 تومان",
            min: null,
            max: 200000
        },
        {
            id: 2,
            text: "200,000-500,000 تومان",
            min: 200000,
            max: 500000
        },
        {
            id: 3,
            text: "500,000-1,000,000 تومان",
            min: 500000,
            max: 1000000
        },
        {
            id: 4,
            text: "1,000,000-1,500,000 تومان",
            min: 1000000,
            max: 1500000
        },
        {
            id: 5,
            text: "بیشتر از 1,500,000 تومان",
            min: 1500000,
            max: null
        }
    ]);

    globalModule.constant('allDailyRentRanges', [
        {
            id: 1,
            text: "کمتر از 50,000 تومان",
            min: null,
            max: 50000
        },
        {
            id: 2,
            text: "50,000-100,000 تومان",
            min: 50000,
            max: 100000
        },
        {
            id: 3,
            text: "100,000-200,000 تومان",
            min: 100000,
            max: 200000
        },
        {
            id: 4,
            text: "200,000-300,000 تومان",
            min: 200000,
            max: 300000
        },
        {
            id: 5,
            text: "بیشتر از 300,000 تومان",
            min: 300000,
            max: null
        }
    ]);

    globalModule.constant('allPriceRanges', [
        {
            id: 1,
            text: "کمتر از 100,000,000 تومان",
            min: null,
            max: 100000000
        },
        {
            id: 2,
            text: "100,000,000-300,000,000 تومان",
            min: 100000000,
            max: 300000000
        },
        {
            id: 3,
            text: "300,000,000-600,000,000 تومان",
            min: 300000000,
            max: 600000000
        },
        {
            id: 4,
            text: "600,000,000-1,000,000,000 تومان",
            min: 600000000,
            max: 1000000000
        },
        {
            id: 5,
            text: "بیشتر از 1,000,000,000 تومان",
            min: 1000000000,
            max: null
        }
    ]);

    globalModule.constant('allPricePerEstateAreaRanges', [
        {
            id: 1,
            text: "کمتر از 3,000,000 تومان",
            min: null,
            max: 3000000
        },
        {
            id: 2,
            text: "3,000,000-5,000,000 تومان",
            min: 3000000,
            max: 5000000
        },
        {
            id: 3,
            text: "5,000,000-10,000,000 تومان",
            min: 5000000,
            max: 10000000
        },
        {
            id: 4,
            text: "بیشتر از 10,000,000 تومان",
            min: 10000000,
            max: null
        }
    ]);

    globalModule.constant('allPricePerUnitAreaRanges', [
        {
            id: 1,
            text: "کمتر از 2,000,000 تومان",
            min: null,
            max: 2000000
        },
        {
            id: 2,
            text: "2,000,000-4,000,000 تومان",
            min: 2000000,
            max: 4000000
        },
        {
            id: 3,
            text: "4,000,000-8,000,000 تومان",
            min: 4000000,
            max: 8000000
        },
        {
            id: 4,
            text: "8,000,000-10,000,000 تومان",
            min: 8000000,
            max: 10000000
        },
        {
            id: 5,
            text: "بیشتر از 10,000,000 تومان",
            min: 10000000,
            max: null
        }
    ]);
}