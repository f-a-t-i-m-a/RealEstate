using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;

namespace JahanJooy.RealEstate.Core.Services.Billing
{
    [Contract]
    public interface ITarrifService
    {
        Tarrif GetTarrif(long? userId, DateTime? asOf = null);
    }
}