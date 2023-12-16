namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.BillingAdminHome
{
    public class BillingAdminHomeStatisticsModel
    {
        public long AdministrativeChangesQueueLength { get; set; }
        public long WireTransferPaymentsQueueLength { get; set; }
        public long ElectronicPaymentsQueueLenth { get; set; }
        public long RefundRequestQueueLength { get; set; }

        public long TotalQueueLength
        {
            get
            {
                return AdministrativeChangesQueueLength +
                       WireTransferPaymentsQueueLength +
                       ElectronicPaymentsQueueLenth +
                       RefundRequestQueueLength;
            }
        }
    }
}