using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Models.VicinityAdmin
{
    public class VicinityAdminNewVicinityModel
	{
        [Required]
        public string Name { get; set; }

        [Required]
        public VicinityType? Type { get; set; }

		public bool ShowTypeInTitle { get; set; }
		public string AlternativeNames { get; set; }
		public string AdditionalSearchText { get; set; }

		[Required]
		public VicinityType? WellKnownScope { get; set; }

		public bool ShowInSummary { get; set; }
		public bool CanContainPropertyRecords { get; set; }

        public long? CurrentVicinityID { get; set; }
	    public IEnumerable<VicinityType> AllowedVicinityTypes { get; set; }
	}
}