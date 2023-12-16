using System.Collections.Generic;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    public class ParametersOutput
    {
        public List<ParameterValue> Parameteres { get; set; }

        public ParametersOutput(List<ParameterValue> parameteres)
        {
            Parameteres = parameteres;
        }
    }
}