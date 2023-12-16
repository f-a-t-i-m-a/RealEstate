using System;
using System.Collections.Generic;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class Building
	{
		public long ID { get; set; }

		//
		// Units arrangement

		public int? TotalNumberOfUnits { get; set; }
		public int? NumberOfUnitsPerFloor { get; set; }
		public int? NumberFloorsAboveGround { get; set; }
		public int? NumberOfParkingFloors { get; set; }
		public bool? HasJanitorUnit { get; set; }
		public bool? HasEntranceLobby { get; set; }

		//
		// Building properties

		public BuildingFaceType? FaceType { get; set; }
		public BuildingStructureType? StructureType { get; set; }
		public BuildingRoofCoverType? RoofCoverType { get; set; }
		public int? BuildingAgeYears { get; set; }
		public bool? HasEndOfConstructionCertificate { get; set; }
		public DateTime? PermitDate { get; set; }
		public DateTime? ConstructionCompletionDate { get; set; }
		public DateTime? EndOfConstructionCertificateDate { get; set; }

		//
		// Standard features

		public bool? HasElevator { get; set; }
		public bool? HasFurnitureElevator { get; set; }
		public bool? HasCentralTelevisionAntenna { get; set; }
		public bool? HasCentralInternetConnection { get; set; }
		public bool? HasGuestParking { get; set; }
		public bool? HasTrashChute { get; set; }
		public bool? HasCentralBoiler { get; set; }
		public bool? HasCentralChiller { get; set; }
		public bool? HasCentralVacuum { get; set; }
		public bool? HasGatheringHall { get; set; }
		public string OtherFeatures { get; set; }

		//
		// Welfare features

		public bool? HasAutomaticParkingDoor { get; set; }
		public bool? HasVideoEyePhone { get; set; }
		public bool? HasCarWash { get; set; }
		public bool? HasBackupWaterTank { get; set; }
		public bool? HasAutomaticStairwayLights { get; set; }
		public bool? HasAccessibilityFeatures { get; set; }
		public string OtherWelfareFeatures { get; set; }

		//
		// Recreation features

		public bool? HasGreenGarden { get; set; }
		public bool? HasBowerInGarden { get; set; }
		public bool? HasBowerOnRoof { get; set; }
		public bool? HasSwimmingPool { get; set; }
		public bool? HasSauna { get; set; }
		public bool? HasJacuzzi { get; set; }
		public bool? HasGym { get; set; }
		public string OtherRecreationFeatures { get; set; }

		//
		// Safety features

		public bool? HasFireAlarm { get; set; }
		public bool? HasEmergencyEscape { get; set; }
		public bool? HasClosedCircuitCamera { get; set; }
		public bool? HasGuardian { get; set; }
		public bool? HasQuakeInsurance { get; set; }
		public bool? HasFireInsurance { get; set; }
		public bool? HasLightningArrester { get; set; }
		public string OtherSafetyFeatures { get; set; }

		//
		// Related entities

		public long EstateID { get; set; }
		public Estate Estate { get; set; } // TODO: Remove this relationship

		public ICollection<Unit> Units { get; set; } // TODO: Remove this relationship
	}
}