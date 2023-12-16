namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class PhoneNumberDesc // TODO : Externalizable
    {
        public static PhoneNumberDescBuilder newBuilder()
        {
            return new PhoneNumberDescBuilder();
        }

        private bool hasNationalNumberPattern;
        private string nationalNumberPattern_ = "";

        public bool HasNationalNumberPattern
        {
            get { return hasNationalNumberPattern; }
        }

        public string NationalNumberPattern
        {
            get { return nationalNumberPattern_; }
        }

        public PhoneNumberDesc setNationalNumberPattern(string value)
        {
            hasNationalNumberPattern = true;
            nationalNumberPattern_ = value;
            return this;
        }

        private bool hasPossibleNumberPattern;
        private string possibleNumberPattern_ = "";

        public bool HasPossibleNumberPattern
        {
            get { return hasPossibleNumberPattern; }
        }

        public string PossibleNumberPattern
        {
            get { return possibleNumberPattern_; }
        }

        public PhoneNumberDesc setPossibleNumberPattern(string value)
        {
            hasPossibleNumberPattern = true;
            possibleNumberPattern_ = value;
            return this;
        }

        // optional string example_number = 6;
        private bool hasExampleNumber;
        private string exampleNumber_ = "";

        public bool HasExampleNumber
        {
            get { return hasExampleNumber; }
        }

        public string ExampleNumber
        {
            get { return exampleNumber_; }
        }

        public PhoneNumberDesc setExampleNumber(string value)
        {
            hasExampleNumber = true;
            exampleNumber_ = value;
            return this;
        }

        public PhoneNumberDesc mergeFrom(PhoneNumberDesc other)
        {
            if (other.HasNationalNumberPattern)
            {
                setNationalNumberPattern(other.NationalNumberPattern);
            }
            if (other.HasPossibleNumberPattern)
            {
                setPossibleNumberPattern(other.PossibleNumberPattern);
            }
            if (other.HasExampleNumber)
            {
                setExampleNumber(other.ExampleNumber);
            }
            return this;
        }

        public bool exactlySameAs(PhoneNumberDesc other)
        {
            return nationalNumberPattern_.Equals(other.nationalNumberPattern_) &&
                   possibleNumberPattern_.Equals(other.possibleNumberPattern_) &&
                   exampleNumber_.Equals(other.exampleNumber_);
        }

//        public void writeExternal(ObjectOutput objectOutput) TODO
//        {
//            objectOutput.writebool(hasNationalNumberPattern);
//            if (hasNationalNumberPattern)
//            {
//                objectOutput.writeUTF(nationalNumberPattern_);
//            }
//
//            objectOutput.writebool(hasPossibleNumberPattern);
//            if (hasPossibleNumberPattern)
//            {
//                objectOutput.writeUTF(possibleNumberPattern_);
//            }
//
//            objectOutput.writebool(hasExampleNumber);
//            if (hasExampleNumber)
//            {
//                objectOutput.writeUTF(exampleNumber_);
//            }
//        }
//
//        public void readExternal(ObjectInput objectInput) TODO
//        {
//            if (objectInput.readbool())
//            {
//                setNationalNumberPattern(objectInput.readUTF());
//            }
//
//            if (objectInput.readbool())
//            {
//                setPossibleNumberPattern(objectInput.readUTF());
//            }
//
//            if (objectInput.readbool())
//            {
//                setExampleNumber(objectInput.readUTF());
//            }
//        }
    }
}