using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JahanJooy.RealEstate.Web.Models.Agency
{
    public class AgencyEditAgencyLogo
    {
        public long? AgencyID { get; set; }

        public Guid? LogoStoreItemID { get; set; }

        public string AgencyName { get; set; }
    }
}