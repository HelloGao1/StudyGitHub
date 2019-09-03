using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class AllergyIntoleranceMapping : BaseMapping
    {
        public static Bundle MapFromCDRToFHirModelBundle(XmlDocument xml, string uri)
        {
            Bundle bundle = new Bundle();

            var resourcesAl1 = xml.SelectNodes("//AL1_PatientAllergyInformation");
            foreach (XmlNode item in resourcesAl1)
            {
                AllergyIntolerance allergy = MapFromCDRToFHirModel(item);
                //bundle.AddResourceEntry(allergy, $"{uri}/{allergy.Id}");
                bundle.AddResourceEntry(allergy, string.Format("{0}/{1}", uri, allergy.Id ));
            }

            var resourcesIAM = xml.SelectNodes("//IAM_PatientAdverseReactionInformation");
            foreach (XmlNode item in resourcesIAM)
            {
                AllergyIntolerance allergy = MapFromCDRToFHirModel(item);
                //bundle.AddResourceEntry(allergy, $"{uri}/{allergy.Id}");
                bundle.AddResourceEntry(allergy, string.Format("{0}/{1}", uri, allergy.Id ));
            }

            return bundle;
        }

        public static AllergyIntolerance MapFromCDRToFHirModel(XmlNode xml)
        {
            // TODO : map this resource

            List<Annotation> annotationList = new List<Annotation>
            {
                new Annotation
                {
                    Text = GetElementToString(xml, "AllergyReactionCode") // TODO : what's the mapping ????? 
                }
            };

            List<CodeableConcept> manifestationList = new List<CodeableConcept>
            {
                new CodeableConcept
                {
                    Text = GetElementToString(xml, "AllergyReactionCode"),
                }
            };

            List<AllergyIntolerance.ReactionComponent> reactionList = new List<AllergyIntolerance.ReactionComponent>
            {
                new AllergyIntolerance.ReactionComponent
                {
                    Severity = GetSeverityFromHl7(GetElementToString(xml, "AllergySeverityCodeIdentifierId")), //Modified, add suffix Id
                    Manifestation = manifestationList,
                    
                    Substance = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                Code = GetElementToString(xml,"AllergenCodeMnemonicDescriptionIdentifierId"), //Modified, add suffix Id
                                Display = GetElementToString(xml,"AllergenCodeMnemonicDescriptionText"),
                                System = GetElementToString(xml,"AllergenCodeMnemonicDescriptionNameOfCodingSystem")
                            } 
                        },
                        Text = GetElementToString(xml, "AllergenCodeMnemonicDescriptionText")
                    }
                }
            };


            var assertedDate = GetElementToString(xml, "StatusedAtDateTimeTime"); // IAM
            if(string.IsNullOrEmpty(assertedDate)) assertedDate = GetElementToString(xml, "IdentificationDate"); // AL1

            AllergyIntolerance allergy = new AllergyIntolerance
            {
                AssertedDate = assertedDate, // IAM-20
                Note = annotationList,
                Reaction = reactionList,
                //Code = new CodeableConcept
                //{
                //    Text = GetElementToString(xml, "AllergenCodeMnemonicDescriptionText"),
                //    Coding = new List<Coding>
                //    {
                //        new Coding
                //        {
                //            Display = GetElementToString(xml, "AllergenCodeMnemonicDescriptionText")
                //        }
                //    }
                //}
            };

            return allergy;
        }

        private static AllergyIntolerance.AllergyIntoleranceSeverity? GetSeverityFromHl7(string severity)
        {
            switch (severity.ToUpper())
            {
                case "MI":
                    return AllergyIntolerance.AllergyIntoleranceSeverity.Mild;
                case "MO":
                    return AllergyIntolerance.AllergyIntoleranceSeverity.Moderate;
                case "SV":
                    return AllergyIntolerance.AllergyIntoleranceSeverity.Severe;
                case "U":
                    return null;
                default:
                    return null;
            }
            
        }
    }
}
