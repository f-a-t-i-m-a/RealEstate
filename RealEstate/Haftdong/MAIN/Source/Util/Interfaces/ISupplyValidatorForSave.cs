using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Util.Enums;

namespace JahanJooy.RealEstateAgency.Util.Interfaces
{
    [Contract]
    public interface ISupplyValidatorForSave
    {
        ValidationResult Validate(Supply supply, Property property);
    }

    [Component]
    public class SupplyValidatorForSale : ISupplyValidatorForSave
    {
        public ValidationResult Validate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == IntentionOfOwner.ForSale)
            {
                if (!supply.PriceSpecificationType.HasValue)
                {
                    errors.Add(new ValidationError("Supply.PriceSpecificationType",
                        GeneralValidationErrors.ValueNotSpecified));
                }
                if (!supply.TotalPrice.HasValue && !supply.PricePerEstateArea.HasValue &&
                    !supply.PricePerUnitArea.HasValue)
                {
                    errors.Add(new ValidationError("Supply.Price",
                        GeneralValidationErrors.ValueNotSpecified));
                }
            }

            if(errors.Any())
                return new ValidationResult { Errors = errors };

            return ValidationResult.Success;
        }
    }

    [Component]
    public class SupplyValidatorForRent : ISupplyValidatorForSave
    {
        public ValidationResult Validate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == IntentionOfOwner.ForRent)
            {
                if (!supply.Mortgage.HasValue)
                {
                    errors.Add(new ValidationError("Supply.Mortgage",
                        GeneralValidationErrors.ValueNotSpecified));
                }
                if (!supply.Rent.HasValue)
                {
                    errors.Add(new ValidationError("Supply.Rent",
                        GeneralValidationErrors.ValueNotSpecified));
                }
            }

            if (errors.Any())
                return new ValidationResult { Errors = errors };

            return ValidationResult.Success;
        }
    }

    [Component]
    public class SupplyValidatorForFullMortgage : ISupplyValidatorForSave
    {
        public ValidationResult Validate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == IntentionOfOwner.ForFullMortgage)
            {
                if (!supply.Mortgage.HasValue)
                {
                    errors.Add(new ValidationError("Supply.Mortgage",
                        GeneralValidationErrors.ValueNotSpecified));
                }
            }

            if (errors.Any())
                return new ValidationResult { Errors = errors };

            return ValidationResult.Success;
        }
    }

    [Component]
    public class SupplyValidatorForDailyRent : ISupplyValidatorForSave
    {
        public ValidationResult Validate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == IntentionOfOwner.ForDailyRent)
            {
                if (!supply.Rent.HasValue)
                {
                    errors.Add(new ValidationError("Supply.Rent",
                        GeneralValidationErrors.ValueNotSpecified));
                }
            }

            if (errors.Any())
                return new ValidationResult { Errors = errors };

            return ValidationResult.Success;
        }
    }

    [Component]
    public class SupplyValidatorForCooperation : ISupplyValidatorForSave
    {
        public ValidationResult Validate(Supply supply, Property property)
        {
            var validatedPropertyTypes = new List<PropertyType>
            {
                PropertyType.Land,
                PropertyType.OldHouse,
                PropertyType.Apartment
            };
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == IntentionOfOwner.ForCooperation)
            {
                if (!validatedPropertyTypes.Contains(property.PropertyType))
                {
                    errors.Add(new ValidationError("Property.PropertyType",
                        GeneralValidationErrors.NotValid));
                }
            }

            if (errors.Any())
                return new ValidationResult { Errors = errors };

            return ValidationResult.Success;
        }
    }
}
    