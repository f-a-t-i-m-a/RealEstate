using JahanJooy.RealEstateAgency.Domain.Base;

namespace JahanJooy.RealEstateAgency.Api.External.Models.File
{
    public class ExternalNewPhotoInput
    {
        public long PropertyExternalID{ get; set; }
        public PhotoInfo Photo { get; set; }
        public byte[] PhotoBinary { get; set; }
    }
}
