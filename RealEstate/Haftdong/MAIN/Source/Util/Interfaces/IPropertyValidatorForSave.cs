using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Util.Enums;

namespace JahanJooy.RealEstateAgency.Util.Interfaces
{
    [Contract]
    public interface IPropertyValidatorForSave
    {
        ValidationResult Validate(Property property);
    }

    [Component]
    public class PropertyValidatorForLand : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Land)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForGarden : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Garden)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForTenement : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Tenement)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForComplex : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Complex)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForTower : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Tower)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForGardenTower : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.GardenTower)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForHouse : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.House)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForVilla : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Villa)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForOldHouse : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.OldHouse)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForOffice : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Office)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForOfficialResidency : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.OfficialResidency)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForApartment : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Apartment)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForPenthouse : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Penthouse)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForSuite : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Suite)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForShop : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Shop)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForCommercial : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Commercial)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForCommercialResidency : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.CommercialResidency)
            {
                if (!property.UnitArea.HasValue)
                {
                    return ValidationResult.Failure("Property.UnitArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.NumberOfRooms.HasValue)
                {
                    return ValidationResult.Failure("Property.NumberOfRooms",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForShed : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Shed)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForAgriculturalLand : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.AgriculturalLand)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForFactory : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Factory)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForWorkShop : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.WorkShop)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForRepairShop : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.RepairShop)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForStoreHouse : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.StoreHouse)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForParking : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Parking)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForGym : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.Gym)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }

    [Component]
    public class PropertyValidatorForCityService : IPropertyValidatorForSave
    {
        public ValidationResult Validate(Property property)
        {
            if (property.PropertyType == PropertyType.CityService)
            {
                if (!property.EstateArea.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateArea",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateDirection.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateDirection",
                        GeneralValidationErrors.ValueNotSpecified);
                }
                if (!property.EstateVoucherType.HasValue)
                {
                    return ValidationResult.Failure("Property.EstateVoucherType",
                        GeneralValidationErrors.ValueNotSpecified);
                }
            }
            return ValidationResult.Success;
        }
    }
}
    