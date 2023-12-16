using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
    public interface IVicinityService
	{
        void CreateVicinity(Vicinity vicinity);
        void UpdateVicinity(Vicinity model);
        void UpdateVicinityGeography(Vicinity model);
        void SetEnabled(IEnumerable<long> vicinityIDs, bool enabled);
		void SetCanContainPropertyRecords(IEnumerable<long> vicinityIDs, bool canContainPropertyRecords);
		void RemoveVicinities(IEnumerable<long> vicinityIDs);
        void MoveVicinities(long[] vicinityIDs, long? targetVicinityId);
	}
}