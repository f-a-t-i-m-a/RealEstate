using System;
using System.Collections;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class CustomerSearchIndexMapper : ISearchIndexMapper<Customer, SearchIE>
    {
        public SearchIE SearchMap(Customer entity)
        {
            var indexText = entity.DisplayName
                            + " " + entity.Description
                            + " " + entity.NameOfFather
                            + " " + entity.Deputy?.DisplayName
                            + " " + entity.Deputy?.NameOfFather
                            + " " + entity.Deputy?.Address
                            + " " + entity.Deputy?.PhoneNumber
                            + " " + entity.Deputy?.Email;

            entity.Contact?.Phones?.ForEach(p => { indexText += " " + p; });

            entity.Contact?.Emails?.ForEach(e => { indexText += " " + e; });

            entity.Contact?.Addresses?.ForEach(a => { indexText += " " + a; });

            var title = entity.DisplayName;

            var phoneList = new ArrayList();
            if (entity.Contact?.Phones != null)
                phoneList.AddRange(entity.Contact.Phones.Select(p => p.NormalizedValue).ToList());

            var displayText = "تلفن های تماس: " + string.Join(",", phoneList.ToArray())
                              + Environment.NewLine
                              + "آدرس ایمیل: " +
                              (entity.Contact?.Emails != null && entity.Contact?.Emails.Count != 0
                                  ? entity.Contact.Emails.First().NormalizedValue
                                  : "")
                              + Environment.NewLine
                              + "آدرس محل سکونت: " +
                              (entity.Contact?.Addresses != null && entity.Contact?.Addresses.Count != 0
                                  ? entity.Contact.Addresses.First().NormalizedValue
                                  : "");

            return new SearchIE
            {
                ID = entity.ID.ToString(),
                DataType = EntityType.Customer.ToString(),
                IndexedText = indexText,
                Title = title,
                DisplayText = displayText,
                CreatedByID = entity.CreatedByID.ToString(),
                CreationTime = entity.CreationTime.Ticks,
                ModificationTime = entity.LastModificationTime?.Ticks
            };
        }
    }
}