using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class EncounterMapping : BaseMapping
    {
        public static Bundle MapFromCDRToFHirModelBundle(XmlDocument xml, string uri)
        {
            Bundle bundle = new Bundle();

            var resources = xml.SelectNodes("//PV1_PatientVisit");
            foreach (XmlNode item in resources)
            {
                Encounter encounter = MapFromCDRToFHirModel(item);
                //bundle.AddResourceEntry(encounter, $"{uri}/{encounter.Id}");
                bundle.AddResourceEntry(encounter, string.Format("{0}/{1}", uri, encounter.Id ));
            }

            return bundle;
        }

        public static Encounter MapFromCDRToFHirModel(XmlNode xml)
        {
            // TODO : map this resource

            List<Encounter.LocationComponent> locationList = new List<Encounter.LocationComponent>
            {
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Display = GetElementToString(xml, "AssignedPatientLocationPointOfCare"),
                    }
                },
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Display = GetElementToString(xml, "PriorPatientLocationPointOfCare"),
                    }
                },
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Display = GetElementToString(xml, "TemporaryLocationPointOfCare"),
                    }
                },
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Display = GetElementToString(xml, "PendingLocationPointOfCare"),
                    }
                },
                new Encounter.LocationComponent
                {
                    Location = new ResourceReference
                    {
                        Display = GetElementToString(xml, "PriorTemporaryLocationPointOfCare"),
                    }
                }
            };

            // remove empty entries
            var locationListFiltered = locationList.Where(l => !l.Location.Display.Equals("")).ToList();

            Encounter encounter = new Encounter
            {
                Subject = new ResourceReference
                {
                    Display = GetElementToString(xml, "CDR_id") // PID-3
                },
                Location = locationListFiltered // PV1-3 / PV1-6 / PV1-11 / PV1-42 / PV1-43
            };

            return encounter;
        }
    }
}
