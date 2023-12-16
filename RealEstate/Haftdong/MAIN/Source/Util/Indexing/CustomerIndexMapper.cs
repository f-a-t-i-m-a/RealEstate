using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Customers;

namespace JahanJooy.RealEstateAgency.Util.Indexing
{
    [Component]
    public class CustomerIndexMapper : IIndexMapper<Customer, CustomerIE>
    {
        public CustomerIE Map(Customer entity)
        {
            var numbers = "";
            var emails = "";

            if (entity.Contact?.Phones != null)
            {
                foreach (var phone in entity.Contact.Phones)
                {
                    if (phone != null)
                    {
                        var length = 2;
                        var value = phone.Value.Replace(" ", "");
                        while (length <= value.Length)
                        {
                            for (var i = 0; i <= value.Length - length; i++)
                            {
                                numbers += value.Substring(i, length) + " ";
                            }
                            length++;
                        }
                    }
                }
            }

            if (entity.Contact?.Emails != null)
            {
                foreach (var email in entity.Contact.Emails)
                {
                    if (email != null)
                    {
                        emails += email + " ";
                    }
                }
            }

            return new CustomerIE
            {
                ID = entity.ID.ToString(),
                DisplayName = entity.DisplayName,
                Email = emails,
                Numbers = numbers,
                RequestCount = entity.RequestCount,
                PropertyCount = entity.PropertyCount,
                LastVisitTime = entity.LastVisitTime?.Ticks ?? 0,
                DeletionTime = entity.DeletionTime?.Ticks ?? 0,
                CreatedByID = entity.CreatedByID.ToString(),
                IsArchived = entity.IsArchived,
            };
        }
    }
}