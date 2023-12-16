using System;
using System.Text;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    /*
     * Copyright (C) 2010 The Libphonenumber Authors
     *
     * Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     *
     * http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /**
     * Definition of the class representing international telephone numbers. This class is hand-created
     * based on the class file compiled from phonenumber.proto. Please refer to that file for detailed
     * descriptions of the meaning of each field.
     */

    [Serializable]
    public sealed class PhoneNumber
    {
        public int CountryCode { get; private set; }
        public bool HasCountryCode { get; private set; }

        private bool _hasNationalNumber;
        private long _nationalNumber;

        private bool _hasExtension;
        private string _extension = "";

        private bool _hasItalianLeadingZero;
        private bool _italianLeadingZero;

        private bool _hasRawInput;
        private String _rawInput = "";

        private bool _hasCountryCodeSource;
        private CountryCodeSource _countryCodeSource;

        private bool _hasPreferredDomesticCarrierCode;
        private string _preferredDomesticCarrierCode = "";

        public PhoneNumber()
        {
            CountryCode = 0;
            _countryCodeSource = CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN;
        }

        public PhoneNumber SetCountryCode(int value)
        {
            HasCountryCode = true;
            CountryCode = value;
            return this;
        }

        public PhoneNumber ClearCountryCode()
        {
            HasCountryCode = false;
            CountryCode = 0;
            return this;
        }

        public bool HasNationalNumber
        {
            get { return _hasNationalNumber; }
        }

        public long NationalNumber
        {
            get { return _nationalNumber; }
        }

        public PhoneNumber SetNationalNumber(long value)
        {
            _hasNationalNumber = true;
            _nationalNumber = value;
            return this;
        }

        public PhoneNumber ClearNationalNumber()
        {
            _hasNationalNumber = false;
            _nationalNumber = 0L;
            return this;
        }

        public bool HasExtension
        {
            get { return _hasExtension; }
        }

        public string Extension
        {
            get { return _extension; }
        }

        public PhoneNumber SetExtension(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            _hasExtension = true;
            _extension = value;
            return this;
        }

        public PhoneNumber ClearExtension()
        {
            _hasExtension = false;
            _extension = "";
            return this;
        }

        public bool HasItalianLeadingZero
        {
            get { return _hasItalianLeadingZero; }
        }

        public bool ItalianLeadingZero
        {
            get { return _italianLeadingZero; }
        }

        public PhoneNumber setItalianLeadingZero(bool value)
        {
            _hasItalianLeadingZero = true;
            _italianLeadingZero = value;
            return this;
        }

        public PhoneNumber ClearItalianLeadingZero()
        {
            _hasItalianLeadingZero = false;
            _italianLeadingZero = false;
            return this;
        }

        public bool HasRawInput
        {
            get { return _hasRawInput; }
        }

        public string RawInput
        {
            get { return _rawInput; }
        }

        public PhoneNumber SetRawInput(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            _hasRawInput = true;
            _rawInput = value;
            return this;
        }

        public PhoneNumber ClearRawInput()
        {
            _hasRawInput = false;
            _rawInput = "";
            return this;
        }

        public bool HasCountryCodeSource
        {
            get { return _hasCountryCodeSource; }
        }

        public CountryCodeSource CountryCodeSource
        {
            get { return _countryCodeSource; }
        }

        public PhoneNumber SetCountryCodeSource(CountryCodeSource? value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            _hasCountryCodeSource = true;
            _countryCodeSource = value.Value;
            return this;
        }

        public PhoneNumber ClearCountryCodeSource()
        {
            _hasCountryCodeSource = false;
            _countryCodeSource = CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN;
            return this;
        }

        public bool HasPreferredDomesticCarrierCode
        {
            get { return _hasPreferredDomesticCarrierCode; }
        }

        public string PreferredDomesticCarrierCode
        {
            get { return _preferredDomesticCarrierCode; }
        }

        public PhoneNumber SetPreferredDomesticCarrierCode(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            _hasPreferredDomesticCarrierCode = true;
            _preferredDomesticCarrierCode = value;
            return this;
        }

        public PhoneNumber ClearPreferredDomesticCarrierCode()
        {
            _hasPreferredDomesticCarrierCode = false;
            _preferredDomesticCarrierCode = "";
            return this;
        }

        public PhoneNumber Clear()
        {
            CountryCode = 0;
            ClearCountryCode();
            ClearNationalNumber();
            ClearExtension();
            ClearItalianLeadingZero();
            ClearRawInput();
            ClearCountryCodeSource();
            ClearPreferredDomesticCarrierCode();
            return this;
        }

        public PhoneNumber MergeFrom(PhoneNumber other)
        {
            if (other.HasCountryCode)
            {
                SetCountryCode(other.CountryCode);
            }
            if (other.HasNationalNumber)
            {
                SetNationalNumber(other.NationalNumber);
            }
            if (other.HasExtension)
            {
                SetExtension(other.Extension);
            }
            if (other.HasItalianLeadingZero)
            {
                setItalianLeadingZero(other.ItalianLeadingZero);
            }
            if (other.HasRawInput)
            {
                SetRawInput(other.RawInput);
            }
            if (other.HasCountryCodeSource)
            {
                SetCountryCodeSource(other.CountryCodeSource);
            }
            if (other.HasPreferredDomesticCarrierCode)
            {
                SetPreferredDomesticCarrierCode(other.PreferredDomesticCarrierCode);
            }
            return this;
        }

        public bool exactlySameAs(PhoneNumber other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return (CountryCode == other.CountryCode && _nationalNumber == other._nationalNumber &&
                    _extension == other._extension && _italianLeadingZero == other._italianLeadingZero &&
                    _rawInput == other._rawInput && _countryCodeSource == other._countryCodeSource &&
                    _preferredDomesticCarrierCode == other._preferredDomesticCarrierCode &&
                    HasPreferredDomesticCarrierCode == other.HasPreferredDomesticCarrierCode);
        }

        #region Overrides

        public override bool Equals(Object that)
        {
            return (that is PhoneNumber) && exactlySameAs((PhoneNumber) that);
        }

        public override int GetHashCode()
        {
            // Simplified rendition of the hashCode function automatically generated from the proto
            // compiler with java_generate_equals_and_hash set to true. We are happy with unset values to
            // be considered equal to their explicitly-set equivalents, so don't check if any value is
            // unknown. The only exception to this is the preferred domestic carrier code.
            int hash = 41;
            hash = (53*hash) + CountryCode;
            hash = (53*hash) + NationalNumber.GetHashCode();
            hash = (53*hash) + Extension.GetHashCode();
            hash = (53*hash) + (ItalianLeadingZero ? 1231 : 1237);
            hash = (53*hash) + RawInput.GetHashCode();
            hash = (53*hash) + CountryCodeSource.GetHashCode();
            hash = (53*hash) + PreferredDomesticCarrierCode.GetHashCode();
            hash = (53*hash) + (HasPreferredDomesticCarrierCode ? 1231 : 1237);
            return hash;
        }

        public override string ToString()
        {
            var outputString = new StringBuilder();
            outputString.Append("Country Code: ").Append(CountryCode);
            outputString.Append(" National Number: ").Append(NationalNumber);
            if (HasItalianLeadingZero && ItalianLeadingZero)
            {
                outputString.Append(" Leading Zero: true");
            }
            if (HasExtension)
            {
                outputString.Append(" Extension: ").Append(_extension);
            }
            if (_hasCountryCodeSource)
            {
                outputString.Append(" Country Code Source: ").Append(_countryCodeSource);
            }
            if (_hasPreferredDomesticCarrierCode)
            {
                outputString.Append(" Preferred Domestic Carrier Code: ").
                    Append(_preferredDomesticCarrierCode);
            }
            return outputString.ToString();
        }

        #endregion
    }

}