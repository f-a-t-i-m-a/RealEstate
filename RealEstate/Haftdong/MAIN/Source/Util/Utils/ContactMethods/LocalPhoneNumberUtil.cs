using System;
using System.Text.RegularExpressions;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Enums;
using MongoDB.Bson;
using PhoneNumbers;

namespace JahanJooy.RealEstateAgency.Util.Utils.ContactMethods
{
    [Contract]
    [Component]
    public class LocalPhoneNumberUtil
    {
        #region Injected dependencies

        private const string NationalLocaleSymbol = "IR";
        private const string DefaultCityPrefix = "21";

        private readonly string phoneCodePattern = @"^0[0-9]{1,3}$";
        private readonly string mobileCodePattern = @"^09[0-3][0-9]$";

        public Regex PhoneCodeRegex => new Regex(phoneCodePattern);
        public Regex MobileCodeRegex => new Regex(mobileCodePattern);

        #endregion

        #region Action methods

        public ValidationResult PreparePhoneInfo(PhoneInfo phone, bool isVerified, bool isDeleted, bool isActive)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            phone.IsVerifiable = true;
            phone.IsVerified = phone.ID != ObjectId.Empty ? phone.IsVerified : isVerified;
            phone.IsDeleted = phone.ID != ObjectId.Empty ? phone.IsDeleted : isDeleted;
            phone.IsActive = phone.ID != ObjectId.Empty ? phone.IsActive : isActive;
            phone.UserContactMethodVerification = phone.ID != ObjectId.Empty
                ? phone.UserContactMethodVerification
                : new UserContactMethodVerification();
            phone.ID = phone.ID != ObjectId.Empty ? phone.ID : ObjectId.GenerateNewId();

            var phoneNumber = NormalizePhoneNumber(phone);

            var formattedNumber = Format(phoneNumber);
            if (!string.IsNullOrEmpty(formattedNumber))
            {
                phone.NormalizedValue = formattedNumber;
                phone.CanReceiveSms = phoneNumberUtil.GetNumberType(phoneNumber) == PhoneNumberType.MOBILE;
                return ValidationResult.Success;
            }

            phone.NormalizedValue = "";
            phone.CanReceiveSms = false;
            return ValidationResult.Failure("Phone", GeneralValidationErrors.NotValid);
        }

        private PhoneNumber NormalizePhoneNumber(PhoneInfo phone)
        {
            try
            {
                string locale;
                var phoneNumber = phone.Value;
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();

                var phoneAsPhoneNumber = phoneNumberUtil.Parse(phone.Value, NationalLocaleSymbol);
                if (phoneNumberUtil.IsValidNumber(phoneAsPhoneNumber))
                {
                    locale = phoneNumberUtil.GetRegionCodeForNumber(phoneAsPhoneNumber);
                    phone.CountryCode = phoneNumberUtil.GetCountryCodeForRegion(locale).ToString();
                    var nationalSignificantNumber = phoneNumberUtil.GetNationalSignificantNumber(phoneAsPhoneNumber);
                    int nationalDestinationCodeLength =
                        phoneNumberUtil.GetLengthOfNationalDestinationCode(phoneAsPhoneNumber);
                    if (nationalDestinationCodeLength > 0)
                    {
                        phone.AreaCode = nationalSignificantNumber.Substring(0, nationalDestinationCodeLength);
                    }
                }
                else
                {
                    phoneAsPhoneNumber = phoneNumberUtil.Parse(DefaultCityPrefix + phone.Value, NationalLocaleSymbol);
                    if (phoneNumberUtil.IsValidNumber(phoneAsPhoneNumber))
                    {
                        phoneNumber = DefaultCityPrefix + phoneNumber;
                        phone.AreaCode = DefaultCityPrefix;
                        locale = phoneNumberUtil.GetRegionCodeForNumber(phoneAsPhoneNumber);
                        phone.CountryCode = phoneNumberUtil.GetCountryCodeForRegion(locale).ToString();
                    }
                    else
                    {
                        return null;
                    }
                }

                var originalResult = phoneNumberUtil.Parse(phoneNumber, locale);
                if (phoneNumberUtil.IsValidNumber(originalResult))
                    return originalResult;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void NormalizePhoneNumber(string phone, out PhoneNumber normalizedValue, out int countryCode,
            out string areaCode)
        {
            normalizedValue = null;
            countryCode = 0;
            areaCode = null;
            string locale = null;
            var phoneNumber = phone;
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            var phoneAsPhoneNumber = phoneNumberUtil.Parse(phone, NationalLocaleSymbol);
            if (phoneNumberUtil.IsValidNumber(phoneAsPhoneNumber))
            {
                locale = phoneNumberUtil.GetRegionCodeForNumber(phoneAsPhoneNumber);
                countryCode = phoneNumberUtil.GetCountryCodeForRegion(locale);
                var nationalSignificantNumber = phoneNumberUtil.GetNationalSignificantNumber(phoneAsPhoneNumber);
                int nationalDestinationCodeLength =
                    phoneNumberUtil.GetLengthOfNationalDestinationCode(phoneAsPhoneNumber);
                if (nationalDestinationCodeLength > 0)
                {
                    areaCode = nationalSignificantNumber.Substring(0, nationalDestinationCodeLength);
                }
            }
            else
            {
                phoneAsPhoneNumber = phoneNumberUtil.Parse(DefaultCityPrefix + phone, NationalLocaleSymbol);
                if (phoneNumberUtil.IsValidNumber(phoneAsPhoneNumber))
                {
                    phoneNumber = DefaultCityPrefix + phoneNumber;
                    areaCode = DefaultCityPrefix;
                    locale = phoneNumberUtil.GetRegionCodeForNumber(phoneAsPhoneNumber);
                    countryCode = phoneNumberUtil.GetCountryCodeForRegion(locale);
                }
            }

            var originalResult = phoneNumberUtil.Parse(phoneNumber, locale);
            if (phoneNumberUtil.IsValidNumber(originalResult))
                normalizedValue = originalResult;
        }

        public string Format(PhoneNumber input)
        {
            if (input == null)
                return null;

            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            return phoneNumberUtil.Format(input, PhoneNumberFormat.INTERNATIONAL);
        }

        #endregion
    }
}