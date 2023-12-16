using System;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class PhoneMetaDataCollection // TODO : Externalizable
    {
        public static PhoneMetaDataCollectionBuilder newBuilder()
        {
            return new PhoneMetaDataCollectionBuilder();
        }

        private IList<PhoneMetaData> metadata_ = new List<PhoneMetaData>();

        public IList<PhoneMetaData> getMetadataList()
        {
            return metadata_;
        }

        public int getMetadataCount
        {
            get { return metadata_.Count; }
        }

        public PhoneMetaDataCollection addMetadata(PhoneMetaData value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            metadata_.Add(value);
            return this;
        }

//        public void writeExternal(ObjectOutput objectOutput) TODO
//        {
//            int size = getMetadataCount();
//            objectOutput.writeInt(size);
//            for (int i = 0; i < size; i++)
//            {
//                metadata_[i].writeExternal(objectOutput);
//            }
//        }
//
//        public void readExternal(ObjectInput objectInput) TODO
//        {
//            int size = objectInput.readInt();
//            for (int i = 0; i < size; i++)
//            {
//                PhoneMetaData metadata = new PhoneMetaData();
//                metadata.readExternal(objectInput);
//                metadata_.Add(metadata);
//            }
//        }

        public PhoneMetaDataCollection clear()
        {
            metadata_.Clear();
            return this;
        }

    }
}