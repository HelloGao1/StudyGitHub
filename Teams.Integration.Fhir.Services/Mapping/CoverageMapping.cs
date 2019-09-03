using Hl7.Fhir.Model;
using System;
using System.Globalization;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class CoverageMapping : BaseMapping
    {
        public static Bundle MapFromCDRToFHirModelBundle(XmlDocument xml, string uri)
        {
            Bundle bundle = new Bundle();

            var resources = xml.SelectNodes("//IN1_Insurance");
            foreach (XmlNode item in resources)
            {
                var start = GetElementToString(item, "PlanEffectiveDate");
                var end = GetElementToString(item, "PlanExpirationDate");

                if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
                {
                    Coverage coverage = MapFromCDRToFHirModel(item);
                    //bundle.AddResourceEntry(coverage, $"{uri}/{coverage.Id}");
                    bundle.AddResourceEntry(coverage, string.Format("{0}/{1}", uri, coverage.Id ));
                }
            }

            return bundle;
        }

        public static Coverage MapFromCDRToFHirModel(XmlNode xml)
        {
            var orgName = xml.SelectNodes("InsuranceCompanyName")[0];
            var tempName = orgName.SelectNodes("ExtendedCompositeNameandIdentificationNumberforOrganizations")[0];
            var planName = tempName.SelectSingleNode("OrganizationName");

            var start = GetElementToString(xml, "PlanEffectiveDate");
            var end = GetElementToString(xml, "PlanExpirationDate");

            var startDate = DateTime.ParseExact(start, "yyyyMMdd", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(end, "yyyyMMdd", CultureInfo.InvariantCulture);

            Coverage cover = new Coverage
            {
                Period = new Period
                {
                    Start = start,//startDate.ToShortDateString(),
                    End = end,//endDate.ToShortDateString(),
                },

                Grouping = new Coverage.GroupComponent
                {
                    GroupDisplay = GetElementToString(xml, "InsurancePlanIdIdentifierId"), // IN1-8  Modified, added suffix Id
                    PlanDisplay = planName.InnerText, // IN1-35
                    //Plan = planName.InnerText

                },
                SubscriberId = GetElementToString(xml, "CDR_id"),
                Subscriber = new ResourceReference
                {
                    //Url = new Uri("http://teams-fhir-api.dapasoft.com/Patient/" + GetElementToString(xml, "CDR_id")),
                    Identifier = new Identifier
                    {
                        Value = GetElementToString(xml, "CDR_id")
                    }
                }
            };

            return cover;
        }
    }
}
