using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using MongoDB.Bson;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Utils.ContactMethods
{
    [Contract]
    [Component]
    public class LocalContactMethodUtil
    {
        #region Injected dependencies

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalEmailUtil LocalEmailUtil { get; set; }

        [ComponentPlug]
        public LocalAddressUtil LocalAddressUtil { get; set; }

        #endregion

        #region Action methods

        public ContactMethodCollection MapContactMethods(List<NewContactInfoInput> contacts, string contactName,
            string organizationName = "")
        {
            var contactMethod = new ContactMethodCollection
            {
                ContactName = contactName,
                OrganizationName = organizationName,
                Emails = new List<EmailInfo>(),
                Phones = new List<PhoneInfo>(),
                Addresses = new List<AddressInfo>()
            };

            contacts?.ForEach(c =>
            {
                if (!c.Value.IsNullOrEmpty())
                {
                    switch (c.Type)
                    {
                        case ContactMethodType.Phone:
                            var phone = Mapper.Map<PhoneInfo>(c);
                            LocalPhoneNumberUtil.PreparePhoneInfo(phone, false, false, true);
                            contactMethod.Phones.Add(phone);
                            break;
                        case ContactMethodType.Email:
                            var email = Mapper.Map<EmailInfo>(c);
                            LocalEmailUtil.PrepareEmailInfo(email, false, false, true);
                            contactMethod.Emails.Add(email);
                            break;
                        case ContactMethodType.Address:
                            var address = Mapper.Map<AddressInfo>(c);
                            LocalAddressUtil.PrepareAddressInfo(address, false, false, true);
                            contactMethod.Addresses.Add(address);
                            break;
                    }
                }
            });

            return contactMethod;
        }

        public ContactInfo MapAndAddContactMethod(ContactMethodCollection contactMethod,
            NewContactInfoInput contact)
        {
            if (!contact.Value.IsNullOrEmpty())
            {
                switch (contact.Type)
                {
                    case ContactMethodType.Phone:
                        var phone = Mapper.Map<PhoneInfo>(contact);
                        contactMethod.Phones.Add(phone);
                        return phone;
                    case ContactMethodType.Email:
                        var email = Mapper.Map<EmailInfo>(contact);
                        contactMethod.Emails.Add(email);
                        return email;
                    case ContactMethodType.Address:
                        var address = Mapper.Map<AddressInfo>(contact);
                        contactMethod.Addresses.Add(address);
                        return address;
                }
            }
            return null;
        }

        public ValidationResult PrepareContactMethods(ContactMethodCollection contact, bool isVerified, bool isDeleted,
            bool isActive)
        {
            var errors = new List<ValidationError>();
            ValidationResult result;
            contact.Phones?.ForEach(p =>
            {
                if (!p.Value.IsNullOrEmpty())
                {
                    result = LocalPhoneNumberUtil.PreparePhoneInfo(p, isVerified, isDeleted, isActive);
                    if (!result.IsValid)
                        errors.AddRange(result.Errors);
                }
            });

            contact.Emails?.ForEach(e =>
            {
                if (!e.Value.IsNullOrEmpty())
                {
                    result = LocalEmailUtil.PrepareEmailInfo(e, isVerified, isDeleted, isActive);
                    if (!result.IsValid)
                        errors.AddRange(result.Errors);
                }
            });

            contact.Addresses?.ForEach(a =>
            {
                if (!a.Value.IsNullOrEmpty())
                {
                    result = LocalAddressUtil.PrepareAddressInfo(a, isVerified, isDeleted, isActive);
                    if (!result.IsValid)
                        errors.AddRange(result.Errors);
                }
            });

            if (errors.Any())
            {
                return new ValidationResult {Errors = errors};
            }
            return ValidationResult.Success;
        }
        public ValidationResult PrepareContactMethod(ContactInfo contact, bool isVerified, bool isDeleted, bool isActive)
        {
            var phone = contact as PhoneInfo;
            if (phone != null)
                return LocalPhoneNumberUtil.PreparePhoneInfo(phone, isVerified, isDeleted, isActive);

            var email = contact as EmailInfo;
            if (email != null)
                return LocalEmailUtil.PrepareEmailInfo(email, isVerified, isDeleted, isActive);

            var address = contact as AddressInfo;
            if (address != null)
                return LocalAddressUtil.PrepareAddressInfo(address, isVerified, isDeleted, isActive);

            return ValidationResult.Failure("UnsupportedContactInfoType");
        }

        public ContactInfo AddVerificationToContactInfo(ContactMethodCollection contact, ObjectId contactInfoId,
            ContactMethodType type, UserContactMethodVerification userContactMethodVerification)
        {
            ContactInfo contactInfo;

            switch (type)
            {
                case ContactMethodType.Phone:
                    contactInfo = contact.Phones?.FirstOrDefault(p => p.ID == contactInfoId);
                    if (contactInfo != null)
                    {
                        contactInfo.UserContactMethodVerification = userContactMethodVerification;
                    }
                    return contactInfo;
                case ContactMethodType.Email:
                    contactInfo = contact.Emails?.FirstOrDefault(p => p.ID == contactInfoId);
                    if (contactInfo != null)
                    {
                        contactInfo.UserContactMethodVerification = userContactMethodVerification;
                    }
                    return contactInfo;
                case ContactMethodType.Address:
                    contactInfo = contact.Addresses?.FirstOrDefault(p => p.ID == contactInfoId);
                    if (contactInfo != null)
                    {
                        contactInfo.UserContactMethodVerification = userContactMethodVerification;
                    }
                    return contactInfo;
                default:
                    return null;
            }
        }

        public ContactInfo GetContactInfo(ContactMethodCollection contact, ObjectId id)
        {
            ContactInfo contactInfo = contact.Phones?.FirstOrDefault(p => p.ID == id);
            if (contactInfo != null)
                return contactInfo;

            contactInfo = contact.Emails?.FirstOrDefault(p => p.ID == id);
            if (contactInfo != null)
                return contactInfo;

            contactInfo = contact.Addresses?.FirstOrDefault(p => p.ID == id);
            return contactInfo;
        }

        public void UpdateContactInfo(ContactMethodCollection contact, ContactInfo contactInfo)
        {
            var phone = contact.Phones?.SingleOrDefault(p => p.ID == contactInfo.ID);
            if (phone != null)
            {
                contact.Phones.Remove(phone);
                contact.Phones.Add((PhoneInfo) contactInfo);
            }

            var email = contact.Emails?.SingleOrDefault(e => e.ID == contactInfo.ID);
            if (email != null)
            {
                contact.Emails.Remove(email);
                contact.Emails.Add((EmailInfo) contactInfo);
            }

            var address = contact.Addresses?.SingleOrDefault(a => a.ID == contactInfo.ID);
            if (address != null)
            {
                contact.Addresses.Remove(address);
                contact.Addresses.Add((AddressInfo) contactInfo);
            }
        }

        public bool HasVerifiedContactMethod(ContactMethodCollection contact)
        {
            return contact.Phones != null && contact.Phones.Any(p => p.IsVerified);
        }

        public ContactMethodType GetTypeOfContactInfo(ContactInfo contactInfo)
        {
            if (contactInfo is EmailInfo)
                return ContactMethodType.Email;
            if (contactInfo is PhoneInfo)
                return ContactMethodType.Phone;
            return ContactMethodType.Address;
        }

        #endregion
    }
}