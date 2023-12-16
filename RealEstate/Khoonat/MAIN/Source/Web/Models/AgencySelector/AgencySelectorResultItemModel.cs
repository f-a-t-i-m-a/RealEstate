using System.Linq;
using AutoMapper;
using Compositional.Composer.Web;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Util;
using JahanJooy.RealEstate.Util.Presentation;
using JahanJooy.RealEstate.Web.Application.DomainModel;

namespace JahanJooy.RealEstate.Web.Models.AgencySelector
{
    public class AgencySelectorResultItemModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string ManagerName { get; set; }
        public string Code { get; set; }
		public string VicinityText { get; set; }

        public static void ConfigureMapper()
        {
            Mapper.CreateMap<Domain.Directory.Agency, AgencySelectorResultItemModel>()
                .BeforeMap((a, m) => a.GetContent())
                .ForMember(m => m.Name, opt => opt.MapFrom(a => a.Content.Name))
                .ForMember(m => m.ManagerName, opt => opt.MapFrom(a => a.GetContent().ManagerName))
                .ForMember(m => m.Code, opt => opt.MapFrom(a => a.GetContent().Code))
				.ForMember(m => m.VicinityText, opt => opt.ResolveUsing(BuildVicinityText));

        }

		private static string BuildVicinityText(Domain.Directory.Agency agency)
		{
			if (agency == null || agency.AgencyBranches == null)
				return string.Empty;

			var mainBranch = agency.AgencyBranches.FirstOrDefault(b => b.IsMainBranch);
			if (mainBranch == null)
				return string.Empty;

			var vicinity = ComposerWebUtil.ComponentContext.GetComponent<IVicinityCache>()[mainBranch.GetContent().VicinityID];
			return vicinity == null ? string.Empty : VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(vicinity, true, false, true, false));
		}
    }
}