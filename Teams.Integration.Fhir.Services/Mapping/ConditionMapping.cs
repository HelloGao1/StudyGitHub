using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class ConditionMapping : BaseMapping
    {
        public static Bundle MapFromCDRToFHirModelBundle(string uri)
        {
            Bundle bundle = new Bundle();

            Condition condition = MapFromCDRToFHirModel();
            //bundle.AddResourceEntry(condition, $"{uri}/{condition.Id}");
            bundle.AddResourceEntry(condition, string.Format("{0}/{1}", uri, condition.Id));

            return bundle;
        }

        public static Condition MapFromCDRToFHirModel()
        {
            Random gen = new Random();
            int range = 5 * 365; //5 years          
            DateTime randomDate = DateTime.Today.AddDays(-gen.Next(range));

            int dice = gen.Next(1, 6);

            string strCode = string.Empty;
            string strDisplay = string.Empty;

            switch (dice)
            {
                case 1:
                    strCode = "1023001";
                    strDisplay = "Apnea";
                    break;
                case 2:
                    strCode = "698296002";
                    strDisplay = "Acute exacerbation of chronic congestive heart failure";
                    break;
                case 3:
                    strCode = "82272006";
                    strDisplay = "Acute nasopharyngitis";
                    break;
                case 4:
                    strCode = "56038003";
                    strDisplay = "Staphylococcal infectious disease";
                    break;
                case 5:
                    strCode = "282981007";
                    strDisplay = "Unable to kneel";
                    break;
                case 6:
                    strCode = "1023001";
                    strDisplay = "Arthropathy";
                    break;
                default:
                    break;
            }

            Condition condition = new Condition
            {
                Code = new CodeableConcept()
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            Code = strCode,
                            Display = strDisplay,
                            System = "http://snomed.info/sct"
                        }
                    },

                    Text = strDisplay
                },

                AssertedDate = randomDate.ToString(),
                ClinicalStatus = Condition.ConditionClinicalStatusCodes.Active
            };

            return condition;
        }
    }
}
