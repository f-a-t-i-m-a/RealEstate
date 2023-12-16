using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Components
{
    [Contract]
    public interface ISponsorshipCalculatorComponent
    {
		List<TEntity> SelectSponsoredEntities<TEntity>(IEnumerable<TEntity> sponsorships, int numberOfImpressions)
			where TEntity : class, ISponsoredEntityRelated;

		List<Tuple<TEntity, SponsoredEntityImpression>> CalculateImpressions<TEntity>(
			IEnumerable<TEntity> sponsorships, List<TEntity> selectedSponsorships, SponsorshipTarrif tarrif)
			where TEntity : class, ISponsoredEntityRelated;

		List<Tuple<TEntity, SponsoredEntityImpression>> SelectAndCalculateImpressions<TEntity>(
			IEnumerable<TEntity> sponsorships, SponsorshipTarrif tarrif, int numberOfImpressions)
		    where TEntity : class, ISponsoredEntityRelated;
    }
}