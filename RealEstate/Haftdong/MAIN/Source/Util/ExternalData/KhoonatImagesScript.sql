select plp.ID as PhotoID, plp.StoreItemID, plp.Title, plp.Description, plp.CreationTime, plp.DeleteTime
from PropertyListingPhoto as plp
where plp.Approved = 1 and plp.PropertyListingID = @PropertyListingID
