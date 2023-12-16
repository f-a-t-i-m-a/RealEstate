using System;
using AutoMapper;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiOutputUserModel
	{
		public long ID { get; set; }
		public long Code { get; set; }
		public bool IsOperator { get; set; }
		public string DisplayName { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime? LastLogin { get; set; }
		public bool IsVerified { get; set; }
		public int ReputationScore { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<User, ApiOutputUserModel>()
				.ForMember(m => m.CreationTime, opt => opt.MapFrom(u => u.CreationDate));
		}
	}
}