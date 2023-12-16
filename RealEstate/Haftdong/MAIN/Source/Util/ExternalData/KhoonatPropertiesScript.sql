select top (@numberOfItems) pl.ID as PropertyId, pl.PropertyType, pl.IntentionOfOwner, pl.IsAgencyListing,
pl.IsAgencyActivityAllowed, pl.VicinityID,
e.Address, e.Area, e.Direction, e.VoucherType, e.PassageEdgeLength,
b.TotalNumberOfUnits, b.NumberOfUnitsPerFloor, b.NumberFloorsAboveGround, b.FaceType,
b.BuildingAgeYears, b.HasElevator, b.HasGatheringHall, b.HasAutomaticParkingDoor,
b.HasVideoEyePhone, b.HasSwimmingPool, b.HasSauna, b.HasJacuzzi,
u.FloorNumber, u.NumberOfRooms, u.NumberOfParkings, u.UsageType, u.Area, u.StorageRoomArea,
u.IsDuplex, u.HasIranianLavatory, u.HasForeignLavatory, u.HasPrivatePatio, u.KitchenCabinetTopType,
u.KitchenCabinetBodyType, u.MainDaylightDirection, u.LivingRoomFloor,
u.HasBeenReconstructed, u.NumberOfMasterBedrooms,
sc.PriceSpecificationType, sc.Price, sc.PricePerEstateArea, sc.PricePerUnitArea,
sc.HasTransferableLoan, sc.TransferableLoanAmount,
rc.Mortgage, rc.Rent, rc.MortgageAndRentConvertible, rc.MinimumMortgage, rc.MinimumRent,
plci.AgencyName, plci.AgencyAddress, plci.ContactName, plci.ContactPhone1, plci.ContactPhone2,
plci.ContactEmail, plci.OwnerCanBeContacted, plci.OwnerName, plci.OwnerPhone1, plci.OwnerPhone2,
plci.OwnerEmail,
pl.PropertyStatus, pl.CreationDate as PropertyCreationTime, pl.ModificationDate, pl.PublishEndDate,
pl.GeographicLocation,
pl.GeographicLocationType
from PropertyListing as pl 
left join Estate as e on pl.EstateID = e.ID
left join Building as b on pl.BuildingID = b.ID
left join Unit as u on pl.UnitID = u.ID
left join SaleConditions as sc on pl.SaleConditionsID = sc.ID
left join RentConditions as rc on pl.RentConditionsID = rc.ID
left join PropertyListingContactInfo as plci on pl.ContactInfoID = plci.ID
where pl.Approved = 1 and pl.PublishDate is not null and pl.PublishEndDate > GETDATE()
and pl.ModificationDate > '@lastFetchTime'
Order by pl.ModificationDate ASC
