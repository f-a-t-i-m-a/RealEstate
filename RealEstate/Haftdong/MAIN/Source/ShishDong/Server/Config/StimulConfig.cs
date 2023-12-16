﻿using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.Stimulsoft.Common;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Config
{
    public static class StimulConfig
    {
        public static void Configure()
        {
            StiCommonUtils.RegisterCustomFunctions();
            StiRealEstateUtils.RegisterCustomFunctions();
        }
    }
}