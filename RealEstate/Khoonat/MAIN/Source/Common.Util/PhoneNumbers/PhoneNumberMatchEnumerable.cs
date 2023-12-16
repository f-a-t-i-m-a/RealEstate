using System.Collections;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class PhoneNumberMatchEnumerable : IEnumerable<PhoneNumberMatch>
    {
        private readonly PhoneNumberUtil _util;
        private readonly string _text;
        private readonly string _country;
        private readonly Leniency _leniency;
        private readonly long _maxTries;

        public PhoneNumberMatchEnumerable(PhoneNumberUtil util, string text, string country, Leniency leniency, long maxTries)
        {
            _util = util;
            _text = text;
            _country = country;
            _leniency = leniency;
            _maxTries = maxTries;
        }

        #region Implementation of IEnumerable

        public IEnumerator<PhoneNumberMatch> GetEnumerator()
        {
            return new PhoneNumberMatchEnumerator(_util, _text, _country, _leniency, _maxTries);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}