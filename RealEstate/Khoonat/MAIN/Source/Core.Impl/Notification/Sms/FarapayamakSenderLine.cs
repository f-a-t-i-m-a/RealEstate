namespace JahanJooy.RealEstate.Core.Impl.Notification.Sms
{
    internal class FarapayamakSenderLine
    {
        public string LineNumber { get; set; }
        public int LineDefaultPriority { get; set; }
        public int LinePriorityForIrancell { get; set; }
        public int LinePriorityForRightel { get; set; }
        public FarapayamakSenderLineStatus Status { get; set; }
        public bool IsFlashCapable { get; set; }
    }
}