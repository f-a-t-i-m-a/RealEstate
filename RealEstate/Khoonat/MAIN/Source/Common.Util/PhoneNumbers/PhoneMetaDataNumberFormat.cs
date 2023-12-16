using System;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class PhoneMetaDataNumberFormat // TODO : Externalizable
    {
        public static PhoneMetaDataNumberFormatBuilder newBuilder()
        {
            return new PhoneMetaDataNumberFormatBuilder();
        }

        private bool hasPattern;
        private string pattern_ = "";

        public bool HasPattern
        {
            get { return hasPattern; }
        }

        public string Pattern
        {
            get { return pattern_; }
        }

        public PhoneMetaDataNumberFormat SetPattern(string value)
        {
            hasPattern = true;
            pattern_ = value;
            return this;
        }

        private bool hasFormat;
        private string format_ = "";

        public bool HasFormat
        {
            get { return hasFormat; }
        }

        public string Format
        {
            get { return format_; }
        }

        public PhoneMetaDataNumberFormat SetFormat(string value)
        {
            hasFormat = true;
            format_ = value;
            return this;
        }

        private IList<string> leadingDigitsPattern_ = new List<string>();

        public IList<string> LeadingDigitsPattern
        {
            get { return leadingDigitsPattern_; }
        }

        public int LeadingDigitsPatternSize
        {
            get { return leadingDigitsPattern_.Count; }
        }

        public string getLeadingDigitsPattern(int index)
        {
            return leadingDigitsPattern_[index];
        }

        public PhoneMetaDataNumberFormat addLeadingDigitsPattern(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            leadingDigitsPattern_.Add(value);
            return this;
        }

        private bool hasNationalPrefixFormattingRule;
        private string nationalPrefixFormattingRule_ = "";

        public bool HasNationalPrefixFormattingRule
        {
            get { return hasNationalPrefixFormattingRule; }
        }

        public string NationalPrefixFormattingRule
        {
            get { return nationalPrefixFormattingRule_; }
        }

        public PhoneMetaDataNumberFormat setNationalPrefixFormattingRule(string value)
        {
            hasNationalPrefixFormattingRule = true;
            nationalPrefixFormattingRule_ = value;
            return this;
        }

        public PhoneMetaDataNumberFormat clearNationalPrefixFormattingRule()
        {
            hasNationalPrefixFormattingRule = false;
            nationalPrefixFormattingRule_ = "";
            return this;
        }

        private bool hasNationalPrefixOptionalWhenFormatting;
        private bool nationalPrefixOptionalWhenFormatting_ = false;

        public bool HasNationalPrefixOptionalWhenFormatting
        {
            get { return hasNationalPrefixOptionalWhenFormatting; }
        }

        public bool NationalPrefixOptionalWhenFormatting
        {
            get { return nationalPrefixOptionalWhenFormatting_; }
        }

        public PhoneMetaDataNumberFormat setNationalPrefixOptionalWhenFormatting(bool value)
        {
            hasNationalPrefixOptionalWhenFormatting = true;
            nationalPrefixOptionalWhenFormatting_ = value;
            return this;
        }

        private bool hasDomesticCarrierCodeFormattingRule;
        private string domesticCarrierCodeFormattingRule_ = "";

        public bool HasDomesticCarrierCodeFormattingRule
        {
            get { return hasDomesticCarrierCodeFormattingRule; }
        }

        public string DomesticCarrierCodeFormattingRule
        {
            get { return domesticCarrierCodeFormattingRule_; }
        }

        public PhoneMetaDataNumberFormat setDomesticCarrierCodeFormattingRule(string value)
        {
            hasDomesticCarrierCodeFormattingRule = true;
            domesticCarrierCodeFormattingRule_ = value;
            return this;
        }

        public PhoneMetaDataNumberFormat mergeFrom(PhoneMetaDataNumberFormat other)
        {
            if (other.HasPattern)
            {
                SetPattern(other.Pattern);
            }
            if (other.HasFormat)
            {
                SetFormat(other.Format);
            }
            int leadingDigitsPatternSize = other.LeadingDigitsPatternSize;
            for (int i = 0; i < leadingDigitsPatternSize; i++)
            {
                addLeadingDigitsPattern(other.getLeadingDigitsPattern(i));
            }
            if (other.HasNationalPrefixFormattingRule)
            {
                setNationalPrefixFormattingRule(other.NationalPrefixFormattingRule);
            }
            if (other.HasDomesticCarrierCodeFormattingRule)
            {
                setDomesticCarrierCodeFormattingRule(other.DomesticCarrierCodeFormattingRule);
            }
            setNationalPrefixOptionalWhenFormatting(other.NationalPrefixOptionalWhenFormatting);
            return this;
        }

//        TODO public void writeExternal(ObjectOutput objectOutput)
//        {
//            objectOutput.writeUTF(pattern_);
//            objectOutput.writeUTF(format_);
//            int leadingDigitsPatternSize = leadingDigitsPatternSize();
//            objectOutput.writeInt(leadingDigitsPatternSize);
//            for (int i = 0; i < leadingDigitsPatternSize; i++)
//            {
//                objectOutput.writeUTF(leadingDigitsPattern_.get(i));
//            }
//
//            objectOutput.writeBoolean(hasNationalPrefixFormattingRule);
//            if (hasNationalPrefixFormattingRule)
//            {
//                objectOutput.writeUTF(nationalPrefixFormattingRule_);
//            }
//            objectOutput.writeBoolean(hasDomesticCarrierCodeFormattingRule);
//            if (hasDomesticCarrierCodeFormattingRule)
//            {
//                objectOutput.writeUTF(domesticCarrierCodeFormattingRule_);
//            }
//            objectOutput.writeBoolean(nationalPrefixOptionalWhenFormatting_);
//        }
//
//        TODO public void readExternal(ObjectInput objectInput)
//        {
//            setPattern(objectInput.readUTF());
//            setFormat(objectInput.readUTF());
//            int leadingDigitsPatternSize = objectInput.readInt();
//            for (int i = 0; i < leadingDigitsPatternSize; i++)
//            {
//                leadingDigitsPattern_.add(objectInput.readUTF());
//            }
//            if (objectInput.readBoolean())
//            {
//                setNationalPrefixFormattingRule(objectInput.readUTF());
//            }
//            if (objectInput.readBoolean())
//            {
//                setDomesticCarrierCodeFormattingRule(objectInput.readUTF());
//            }
//            setNationalPrefixOptionalWhenFormatting(objectInput.readBoolean());
//        }
    }
}