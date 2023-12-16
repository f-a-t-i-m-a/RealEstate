using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.Models.Report;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;

namespace JahanJooy.RealEstateAgency.Util.Report
{
    [Contract]
    [Component]
    public class GeneralReport
    {

        public void PopulateVariables(StiReport report, List<ReportTemplateParameter> templateParameters,
            ParametersOutput parametersOutput)
        {
            templateParameters.ForEach(param =>
            {
                var paramValue = parametersOutput.Parameteres.FirstOrDefault(p => p.ParameterName == param.ParameterName);
                if (paramValue != null)
                {
                    object value;
                    switch (paramValue.ParameterType)
                    {
                        case ParameterType.String:
                            value = paramValue.Value;
                            break;
                        case ParameterType.DateTime:
                            value = DateTime.Parse(paramValue.Value);
                            break;
                        default:
                            value = "";
                            break;
                    }

                    if (!report.Dictionary.Variables.Contains(param.ParameterName))
                    {
                        switch (paramValue.ParameterType)
                        {
                            case ParameterType.String:
                                report.Dictionary.Variables.Add(new StiVariable
                                {
                                    Name = param.ParameterName,
                                    Type = typeof (string),
                                    ValueObject = value
                                });
                                break;
                            case ParameterType.DateTime:
                                report.Dictionary.Variables.Add(new StiVariable
                                {
                                    Name = param.ParameterName,
                                    Type = typeof (DateTime),
                                    ValueObject = value
                                });
                                break;
                        }
                        //                         report.Dictionary.Variables.Add(param.ParameterName, value);
                    }
                    else
                    {
                        report.Dictionary.Variables[param.ParameterName].ValueObject = value;
                    }
                }
            });
        }
    }
}