using Hl7.Fhir.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class ObservationMapping : BaseMapping
    {
        public static ConcurrentDictionary<string, Tuple<string, string>> _CategoryDict;
        public static void InitCategoryDict()
        {
            if(_CategoryDict == null || _CategoryDict.Count ==0)
            {
                if (_CategoryDict == null)
                    _CategoryDict = new ConcurrentDictionary<string, Tuple<string, string>>();

                _CategoryDict.TryAdd("E.BPD", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.BPS", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.PULSE", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.PULSE.R", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.GCS", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.O2SAT", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.O2SATSRC", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.TEMP", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.TEMP.S", new Tuple<string, string>("vital-signs", "Vital Signs"));
                _CategoryDict.TryAdd("E.RESP", new Tuple<string, string>("vital-signs", "Vital Signs"));
            }

        }
        public static Bundle MapFromCDRToFHirModelBundle(XmlDocument xml, string uri)
        {
            //init the category dictionary.
            InitCategoryDict();
            Bundle bundle = new Bundle();

            var resources = xml.SelectNodes("//OBX_ObservationResult");
            foreach (XmlNode item in resources)
            {
                Observation observation = MapFromCDRToFHirModel(item);
                //bundle.AddResourceEntry(observation, $"{uri}/{observation.Id}");
                bundle.AddResourceEntry(observation, string.Format("{0}/{1}", uri, observation.Id ));
            }

            return bundle;
        }

        public static Observation MapFromCDRToFHirModel(XmlNode xml)
        {
            //if OBX.5 can be converted to numeric, return valueQuantity, otherwise valueString
            Element valElement;
            var numericOBX5 = GetElementToDecimal(xml.Clone(), "//ObservationValue/CodedElement/IdentifierId"); //Modified, Added suffix Id
            if(numericOBX5 != null)
            {
                valElement = new Quantity()
                {
                    Value =numericOBX5,
                    Unit = GetElementToString(xml, "UnitsIdentifierId")//Modified, Added suffix Id
                };
            }
            else
            {
                valElement = new FhirString()
                {
                    Value = GetElementToString(xml.Clone(), "//ObservationValue/CodedElement/IdentifierId"), //Modified, Added suffix Id
                };
            }
            
            List<Coding> obxCoding = new List<Coding>
            {
                new Coding
                {
                    Code = GetElementToString(xml, "ObservationIdentifierIdentifierId"),//Modified, Added suffix Id
                    Display = GetElementToString(xml, "ObservationIdentifierText")                    
                },                
            };

            string categoryCode = "LAB";
            string categoryDisplay = "LAB";
            if (obxCoding != null && obxCoding.Count > 0 && _CategoryDict.Keys.Contains(obxCoding[0].Code))
            {
                Tuple<string, string> categoryInfo ;
                if(_CategoryDict.TryGetValue(obxCoding[0].Code, out categoryInfo))
                {
                    categoryCode = categoryInfo.Item1;
                    categoryDisplay = categoryInfo.Item2;
                }
            }

            Observation obx = new Observation
            {
                //Subject = new ResourceReference
                //{
                //    Url = new System.Uri("http://teams-fhir-api.dapasoft.com/Patient/6")
                //},

                Code = new CodeableConcept
                {
                    Coding = obxCoding,                    
                    
                    Text = GetElementToString(xml, "ObservationIdentifierText")
                },

                Category = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System="http://hl7.org/fhir/observation-category",
                                Display = categoryDisplay,
                                Code = categoryCode,

                            },

                        },
                        //Text = "Vital Sign"
                    }
                },

                Effective = new FhirDateTime
                {
                    Value = GetElementToString(xml,"DateTimeOfTheObservationTime")
                },

                Value = valElement,
                
            };

            return obx;
        }
    }
}
