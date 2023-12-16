using System.Data.Entity.Spatial;
using System.Web.Mvc;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.VicinityAdmin
{
	[Bind(Exclude = "Vicinity,ParentVicinity")]
    public class VicinityAdminEditGeographyModel
    {
		public Vicinity Vicinity { get; set; }
		public Vicinity ParentVicinity { get; set; }

        public long? ID { get; set; }

        public DbGeography CenterPoint { get; set; }
        public DbGeography Boundary { get; set; }
        public long? ParentID { get; set; }

        public string CenterPointWkt
        {
            get { return CenterPoint.ToWkt(); }
            set { CenterPoint = DbGeographyUtil.CreatePoint(value); }
        }

        public string BoundaryWkt
        {
            get { return Boundary.ToWkt(); }
            set { Boundary = DbGeographyUtil.CreatePolygon(value); }
        }

        #region Convert to/from domain

        public static VicinityAdminEditGeographyModel FromDomain(Vicinity vicinity, Vicinity parentVicinity)
        {
            var result = new VicinityAdminEditGeographyModel
            {
				Vicinity = vicinity,
				ParentVicinity = parentVicinity,

                ID = vicinity.ID,
                CenterPoint = vicinity.CenterPoint,
                Boundary = vicinity.Boundary,
                ParentID = vicinity.ParentID
            };

            return result;
        }

        public void UpdateDomain(Vicinity domain)
        {
            domain.CenterPoint = CenterPoint;
            domain.Boundary = Boundary;
        }

        #endregion
    }
}