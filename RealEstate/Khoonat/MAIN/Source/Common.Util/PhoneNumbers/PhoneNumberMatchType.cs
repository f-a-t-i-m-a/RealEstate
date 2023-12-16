namespace JahanJooy.Common.Util.PhoneNumbers
{
    /**
     * Types of phone number matches. See detailed description beside the isNumberMatch() method.
     */
    public enum PhoneNumberMatchType
    {
        NOT_A_NUMBER,
        NO_MATCH,
        SHORT_NSN_MATCH,
        NSN_MATCH,
        EXACT_MATCH,
    }
}