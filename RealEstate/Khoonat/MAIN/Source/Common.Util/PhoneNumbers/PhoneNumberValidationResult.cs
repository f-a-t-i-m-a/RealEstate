namespace JahanJooy.Common.Util.PhoneNumbers
{
    /**
     * Possible outcomes when testing if a PhoneNumber is possible.
     */
    public enum PhoneNumberValidationResult
    {
        IS_POSSIBLE,
        INVALID_COUNTRY_CODE,
        TOO_SHORT,
        TOO_LONG,
    }
}