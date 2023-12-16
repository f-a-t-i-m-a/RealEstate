namespace JahanJooy.RealEstate.Core.Impl.Notification.Sms
{
    internal enum FarapayamakSenderLineStatus : byte
    {
        NotConfigured = 0,
        Disabled = 1,
        NotWorking = 4,
        Blocked = 6,
        HasTransmissionIssues = 8,
        Unknown = 10,
        HasDelaysInTransmission = 12,
        HasHighTransmissionTraffic = 14,
        HasDeliveryReportIssues = 16,
        HasDelaysInDeliveryReport = 18,
        Healthy = 20
    }
}