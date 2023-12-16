
 
 

 


declare module AdminHome {
	interface AdminHomeIndexModel {
		AbuseFlagsQueueSize: number;
		PropertyListingQueueSize: number;
		PropertyListingPhotosQueueSize: number;
		IndexesWithErrors: string[];
		IndexesNotCommitting: string[];
	}
}


