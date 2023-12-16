using System;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class PhoneMetaData // TODO : Externalizable
    {
        public static PhoneMetaDataBuilder newBuilder()
        {
            return new PhoneMetaDataBuilder();
        }

        private bool hasGeneralDesc;
        private PhoneNumberDesc generalDesc_ = null;

        public bool HasGeneralDesc
        {
            get { return hasGeneralDesc; }
        }

        public PhoneNumberDesc GeneralDesc
        {
            get { return generalDesc_; }
        }

        public PhoneMetaData setGeneralDesc(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasGeneralDesc = true;
            generalDesc_ = value;
            return this;
        }

        // required PhoneNumberDesc fixed_line = 2;
        private bool hasFixedLine;
        private PhoneNumberDesc fixedLine_ = null;

        public bool HasFixedLine
        {
            get { return hasFixedLine; }
        }

        public PhoneNumberDesc FixedLine
        {
            get { return fixedLine_; }
        }

        public PhoneMetaData setFixedLine(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasFixedLine = true;
            fixedLine_ = value;
            return this;
        }

        // required PhoneNumberDesc mobile = 3;
        private bool hasMobile;
        private PhoneNumberDesc mobile_ = null;

        public bool HasMobile
        {
            get { return hasMobile; }
        }

        public PhoneNumberDesc Mobile
        {
            get { return mobile_; }
        }

        public PhoneMetaData setMobile(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasMobile = true;
            mobile_ = value;
            return this;
        }

        // required PhoneNumberDesc toll_free = 4;
        private bool hasTollFree;
        private PhoneNumberDesc tollFree_ = null;

        public bool HasTollFree
        {
            get { return hasTollFree; }
        }

        public PhoneNumberDesc TollFree
        {
            get { return tollFree_; }
        }

        public PhoneMetaData setTollFree(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasTollFree = true;
            tollFree_ = value;
            return this;
        }

        // required PhoneNumberDesc premium_rate = 5;
        private bool hasPremiumRate;
        private PhoneNumberDesc premiumRate_ = null;

        public bool HasPremiumRate
        {
            get { return hasPremiumRate; }
        }

        public PhoneNumberDesc PremiumRate
        {
            get { return premiumRate_; }
        }

        public PhoneMetaData setPremiumRate(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasPremiumRate = true;
            premiumRate_ = value;
            return this;
        }

        // required PhoneNumberDesc shared_cost = 6;
        private bool hasSharedCost;
        private PhoneNumberDesc sharedCost_ = null;

        public bool HasSharedCost
        {
            get { return hasSharedCost; }
        }

        public PhoneNumberDesc SharedCost
        {
            get { return sharedCost_; }
        }

        public PhoneMetaData setSharedCost(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasSharedCost = true;
            sharedCost_ = value;
            return this;
        }

        // required PhoneNumberDesc personal_number = 7;
        private bool hasPersonalNumber;
        private PhoneNumberDesc personalNumber_ = null;

        public bool HasPersonalNumber
        {
            get { return hasPersonalNumber; }
        }

        public PhoneNumberDesc PersonalNumber
        {
            get { return personalNumber_; }
        }

        public PhoneMetaData setPersonalNumber(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasPersonalNumber = true;
            personalNumber_ = value;
            return this;
        }

        // required PhoneNumberDesc voip = 8;
        private bool hasVoip;
        private PhoneNumberDesc voip_ = null;

        public bool HasVoip
        {
            get { return hasVoip; }
        }

        public PhoneNumberDesc Voip
        {
            get { return voip_; }
        }

        public PhoneMetaData setVoip(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasVoip = true;
            voip_ = value;
            return this;
        }

        // required PhoneNumberDesc pager = 21;
        private bool hasPager;
        private PhoneNumberDesc pager_ = null;

        public bool HasPager
        {
            get { return hasPager; }
        }

        public PhoneNumberDesc Pager
        {
            get { return pager_; }
        }

        public PhoneMetaData setPager(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasPager = true;
            pager_ = value;
            return this;
        }

        private bool hasUan;
        private PhoneNumberDesc uan_ = null;

        public bool HasUan
        {
            get { return hasUan; }
        }

        public PhoneNumberDesc Uan
        {
            get { return uan_; }
        }

        public PhoneMetaData setUan(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasUan = true;
            uan_ = value;
            return this;
        }

        private bool hasVoicemail;
        private PhoneNumberDesc voicemail_ = null;

        public bool HasVoicemail
        {
            get { return hasVoicemail; }
        }

        public PhoneNumberDesc Voicemail
        {
            get { return voicemail_; }
        }

        public PhoneMetaData setVoicemail(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasVoicemail = true;
            voicemail_ = value;
            return this;
        }

        // required PhoneNumberDesc emergency = 27;
        private bool hasEmergency;
        private PhoneNumberDesc emergency_ = null;

        public bool HasEmergency
        {
            get { return hasEmergency; }
        }

        public PhoneNumberDesc Emergency
        {
            get { return emergency_; }
        }

        public PhoneMetaData setEmergency(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasEmergency = true;
            emergency_ = value;
            return this;
        }

        // required PhoneNumberDesc noInternationalDialling = 24;
        private bool hasNoInternationalDialling;
        private PhoneNumberDesc noInternationalDialling_ = null;

        public bool HasNoInternationalDialling
        {
            get { return hasNoInternationalDialling; }
        }

        public PhoneNumberDesc NoInternationalDialling
        {
            get { return noInternationalDialling_; }
        }

        public PhoneMetaData setNoInternationalDialling(PhoneNumberDesc value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            hasNoInternationalDialling = true;
            noInternationalDialling_ = value;
            return this;
        }

        private bool hasId;
        private string id_ = "";

        public bool HasId
        {
            get { return hasId; }
        }

        public string ID
        {
            get { return id_; }
        }

        public PhoneMetaData setId(string value)
        {
            hasId = true;
            id_ = value;
            return this;
        }

        private bool hasCountryCode;
        private int countryCode_ = 0;

        public bool HasCountryCode
        {
            get { return hasCountryCode; }
        }

        public int CountryCode
        {
            get { return countryCode_; }
        }

        public PhoneMetaData setCountryCode(int value)
        {
            hasCountryCode = true;
            countryCode_ = value;
            return this;
        }

        private bool hasInternationalPrefix;
        private string internationalPrefix_ = "";

        public bool HasInternationalPrefix
        {
            get { return hasInternationalPrefix; }
        }

        public string InternationalPrefix
        {
            get { return internationalPrefix_; }
        }

        public PhoneMetaData setInternationalPrefix(string value)
        {
            hasInternationalPrefix = true;
            internationalPrefix_ = value;
            return this;
        }

        private bool hasPreferredInternationalPrefix;
        private string preferredInternationalPrefix_ = "";

        public bool HasPreferredInternationalPrefix
        {
            get { return hasPreferredInternationalPrefix; }
        }

        public string PreferredInternationalPrefix
        {
            get { return preferredInternationalPrefix_; }
        }

        public PhoneMetaData setPreferredInternationalPrefix(string value)
        {
            hasPreferredInternationalPrefix = true;
            preferredInternationalPrefix_ = value;
            return this;
        }

        private bool hasNationalPrefix;
        private string nationalPrefix_ = "";

        public bool HasNationalPrefix
        {
            get { return hasNationalPrefix; }
        }

        public string NationalPrefix
        {
            get { return nationalPrefix_; }
        }

        public PhoneMetaData setNationalPrefix(string value)
        {
            hasNationalPrefix = true;
            nationalPrefix_ = value;
            return this;
        }

        // optional string preferred_extn_prefix = 13;
        private bool hasPreferredExtnPrefix;
        private string preferredExtnPrefix_ = "";

        public bool HasPreferredExtnPrefix
        {
            get { return hasPreferredExtnPrefix; }
        }

        public string PreferredExtnPrefix
        {
            get { return preferredExtnPrefix_; }
        }

        public PhoneMetaData setPreferredExtnPrefix(string value)
        {
            hasPreferredExtnPrefix = true;
            preferredExtnPrefix_ = value;
            return this;
        }

        // optional string national_prefix_for_parsing = 15;
        private bool hasNationalPrefixForParsing;
        private string nationalPrefixForParsing_ = "";

        public bool HasNationalPrefixForParsing
        {
            get { return hasNationalPrefixForParsing; }
        }

        public string NationalPrefixForParsing
        {
            get { return nationalPrefixForParsing_; }
        }

        public PhoneMetaData setNationalPrefixForParsing(string value)
        {
            hasNationalPrefixForParsing = true;
            nationalPrefixForParsing_ = value;
            return this;
        }

        // optional string national_prefix_transform_rule = 16;
        private bool hasNationalPrefixTransformRule;
        private string nationalPrefixTransformRule_ = "";

        public bool HasNationalPrefixTransformRule
        {
            get { return hasNationalPrefixTransformRule; }
        }

        public string NationalPrefixTransformRule
        {
            get { return nationalPrefixTransformRule_; }
        }

        public PhoneMetaData setNationalPrefixTransformRule(string value)
        {
            hasNationalPrefixTransformRule = true;
            nationalPrefixTransformRule_ = value;
            return this;
        }

        // optional bool same_mobile_and_fixed_line_pattern = 18 [default = false];
        private bool hasSameMobileAndFixedLinePattern;
        private bool sameMobileAndFixedLinePattern_ = false;

        public bool HasSameMobileAndFixedLinePattern
        {
            get { return hasSameMobileAndFixedLinePattern; }
        }

        public bool SameMobileAndFixedLinePattern
        {
            get { return sameMobileAndFixedLinePattern_; }
        }

        public PhoneMetaData setSameMobileAndFixedLinePattern(bool value)
        {
            hasSameMobileAndFixedLinePattern = true;
            sameMobileAndFixedLinePattern_ = value;
            return this;
        }

        // repeated NumberFormat number_format = 19;
        private IList<PhoneMetaDataNumberFormat> numberFormat_ = new List<PhoneMetaDataNumberFormat>();

        public IList<PhoneMetaDataNumberFormat> numberFormats()
        {
            return numberFormat_;
        }

        public int numberFormatSize
        {
            get { return numberFormat_.Count; }
        }

        public PhoneMetaDataNumberFormat getNumberFormat(int index)
        {
            return numberFormat_[index];
        }

        public PhoneMetaData addNumberFormat(PhoneMetaDataNumberFormat value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            numberFormat_.Add(value);
            return this;
        }

        // repeated NumberFormat intl_number_format = 20;
        private IList<PhoneMetaDataNumberFormat> intlNumberFormat_ =
            new List<PhoneMetaDataNumberFormat>();

        public IList<PhoneMetaDataNumberFormat> intlNumberFormats()
        {
            return intlNumberFormat_;
        }

        public int intlNumberFormatSize
        {
            get { return intlNumberFormat_.Count; }
        }

        public PhoneMetaDataNumberFormat getIntlNumberFormat(int index)
        {
            return intlNumberFormat_[index];
        }

        public PhoneMetaData addIntlNumberFormat(PhoneMetaDataNumberFormat value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            intlNumberFormat_.Add(value);
            return this;
        }

        public PhoneMetaData clearIntlNumberFormat()
        {
            intlNumberFormat_.Clear();
            return this;
        }

        // optional bool main_country_for_code = 22 [default = false];
        private bool hasMainCountryForCode;
        private bool mainCountryForCode_ = false;

        public bool HasMainCountryForCode
        {
            get { return hasMainCountryForCode; }
        }

        public bool MainCountryForCode
        {
            get { return mainCountryForCode_; }
        }

        // Method that lets this class have the same interface as the one generated by Protocol Buffers
        // which is used by C++ build tools.
        public bool getMainCountryForCode()
        {
            return mainCountryForCode_;
        }

        public PhoneMetaData setMainCountryForCode(bool value)
        {
            hasMainCountryForCode = true;
            mainCountryForCode_ = value;
            return this;
        }

        // optional string leading_digits = 23;
        private bool hasLeadingDigits;
        private string leadingDigits_ = "";

        public bool HasLeadingDigits
        {
            get { return hasLeadingDigits; }
        }

        public string LeadingDigits
        {
            get { return leadingDigits_; }
        }

        public PhoneMetaData setLeadingDigits(string value)
        {
            hasLeadingDigits = true;
            leadingDigits_ = value;
            return this;
        }

        // optional bool leading_zero_possible = 26 [default = false];
        private bool hasLeadingZeroPossible;
        private bool leadingZeroPossible_ = false;

        public bool HasLeadingZeroPossible
        {
            get { return hasLeadingZeroPossible; }
        }

        public bool LeadingZeroPossible
        {
            get { return leadingZeroPossible_; }
        }

        public PhoneMetaData setLeadingZeroPossible(bool value)
        {
            hasLeadingZeroPossible = true;
            leadingZeroPossible_ = value;
            return this;
        }

//        public void writeExternal(ObjectOutput objectOutput) TODO
//        {
//            objectOutput.writebool(hasGeneralDesc);
//            if (hasGeneralDesc)
//            {
//                generalDesc_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasFixedLine);
//            if (hasFixedLine)
//            {
//                fixedLine_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasMobile);
//            if (hasMobile)
//            {
//                mobile_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasTollFree);
//            if (hasTollFree)
//            {
//                tollFree_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasPremiumRate);
//            if (hasPremiumRate)
//            {
//                premiumRate_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasSharedCost);
//            if (hasSharedCost)
//            {
//                sharedCost_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasPersonalNumber);
//            if (hasPersonalNumber)
//            {
//                personalNumber_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasVoip);
//            if (hasVoip)
//            {
//                voip_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasPager);
//            if (hasPager)
//            {
//                pager_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasUan);
//            if (hasUan)
//            {
//                uan_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasVoicemail);
//            if (hasVoicemail)
//            {
//                voicemail_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasEmergency);
//            if (hasEmergency)
//            {
//                emergency_.writeExternal(objectOutput);
//            }
//            objectOutput.writebool(hasNoInternationalDialling);
//            if (hasNoInternationalDialling)
//            {
//                noInternationalDialling_.writeExternal(objectOutput);
//            }
//
//            objectOutput.writeUTF(id_);
//            objectOutput.writeInt(countryCode_);
//            objectOutput.writeUTF(internationalPrefix_);
//
//            objectOutput.writebool(hasPreferredInternationalPrefix);
//            if (hasPreferredInternationalPrefix)
//            {
//                objectOutput.writeUTF(preferredInternationalPrefix_);
//            }
//
//            objectOutput.writebool(hasNationalPrefix);
//            if (hasNationalPrefix)
//            {
//                objectOutput.writeUTF(nationalPrefix_);
//            }
//
//            objectOutput.writebool(hasPreferredExtnPrefix);
//            if (hasPreferredExtnPrefix)
//            {
//                objectOutput.writeUTF(preferredExtnPrefix_);
//            }
//
//            objectOutput.writebool(hasNationalPrefixForParsing);
//            if (hasNationalPrefixForParsing)
//            {
//                objectOutput.writeUTF(nationalPrefixForParsing_);
//            }
//
//            objectOutput.writebool(hasNationalPrefixTransformRule);
//            if (hasNationalPrefixTransformRule)
//            {
//                objectOutput.writeUTF(nationalPrefixTransformRule_);
//            }
//
//            objectOutput.writebool(sameMobileAndFixedLinePattern_);
//
//            int numberFormatSize = numberFormatSize();
//            objectOutput.writeInt(numberFormatSize);
//            for (int i = 0; i < numberFormatSize; i++)
//            {
//                numberFormat_.get(i).writeExternal(objectOutput);
//            }
//
//            int intlNumberFormatSize = intlNumberFormatSize();
//            objectOutput.writeInt(intlNumberFormatSize);
//            for (int i = 0; i < intlNumberFormatSize; i++)
//            {
//                intlNumberFormat_.get(i).writeExternal(objectOutput);
//            }
//
//            objectOutput.writebool(mainCountryForCode_);
//
//            objectOutput.writebool(hasLeadingDigits);
//            if (hasLeadingDigits)
//            {
//                objectOutput.writeUTF(leadingDigits_);
//            }
//
//            objectOutput.writebool(leadingZeroPossible_);
//        }

//        public void readExternal(ObjectInput objectInput) TODO
//        {
//            bool hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setGeneralDesc(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setFixedLine(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setMobile(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setTollFree(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setPremiumRate(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setSharedCost(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setPersonalNumber(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setVoip(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setPager(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setUan(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setVoicemail(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setEmergency(desc);
//            }
//            hasDesc = objectInput.readbool();
//            if (hasDesc)
//            {
//                PhoneNumberDesc desc = new PhoneNumberDesc();
//                desc.readExternal(objectInput);
//                setNoInternationalDialling(desc);
//            }
//
//            setId(objectInput.readUTF());
//            setCountryCode(objectInput.readInt());
//            setInternationalPrefix(objectInput.readUTF());
//
//            bool hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setPreferredInternationalPrefix(objectInput.readUTF());
//            }
//
//            hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setNationalPrefix(objectInput.readUTF());
//            }
//
//            hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setPreferredExtnPrefix(objectInput.readUTF());
//            }
//
//            hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setNationalPrefixForParsing(objectInput.readUTF());
//            }
//
//            hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setNationalPrefixTransformRule(objectInput.readUTF());
//            }
//
//            setSameMobileAndFixedLinePattern(objectInput.readbool());
//
//            int nationalFormatSize = objectInput.readInt();
//            for (int i = 0; i < nationalFormatSize; i++)
//            {
//                NumberFormat numFormat = new NumberFormat();
//                numFormat.readExternal(objectInput);
//                numberFormat_.add(numFormat);
//            }
//
//            int intlNumberFormatSize = objectInput.readInt();
//            for (int i = 0; i < intlNumberFormatSize; i++)
//            {
//                NumberFormat numFormat = new NumberFormat();
//                numFormat.readExternal(objectInput);
//                intlNumberFormat_.add(numFormat);
//            }
//
//            setMainCountryForCode(objectInput.readbool());
//
//            hasstring = objectInput.readbool();
//            if (hasstring)
//            {
//                setLeadingDigits(objectInput.readUTF());
//            }
//
//            setLeadingZeroPossible(objectInput.readbool());
//        }

    }
}