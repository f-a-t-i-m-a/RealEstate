using JahanJooy.RealEstateAgency.Domain.MasterData;

namespace JahanJooy.RealEstateAgency.Util.Models.Report
{
    public class ParameterValue
    {
        public string ParameterName { get; private set; }
        public ParameterType ParameterType { get; set; }
        public string Value { get; private set; }

        public ParameterValue(string parameterName, ParameterType parameterType, string value)
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            Value = value;
        }
    }
}