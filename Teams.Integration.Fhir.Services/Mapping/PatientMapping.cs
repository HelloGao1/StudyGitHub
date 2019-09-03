using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class PatientMapping : BaseMapping
    {
        private static readonly string xmlRootPath = "//PID_PatientIdentification";

        public static Patient MapFromCDRToFHirModel(XmlDocument xml)
        {
            try
            {
                XmlNode node = xml.SelectNodes(xmlRootPath)[0];
                return MapFromCDRToFHirModel(node);
            }
            catch
            {
                return null;
            }
        }

        public static Bundle MapFromCDRToFHirModelBundle(XmlDocument xml, string uri)
        {
            Bundle bundle = new Bundle();

            var resources = xml.SelectNodes(xmlRootPath);
            foreach (XmlNode item in resources)
            {
                Patient patient = MapFromCDRToFHirModel(item);
                //bundle.AddResourceEntry(patient, $"{uri}/{patient.Id}");
                bundle.AddResourceEntry(patient, string.Format("{0}/{1}", uri, patient.Id ));
            }

            return bundle;
        }

        private static Patient MapFromCDRToFHirModel(XmlNode xml)
        {
            xml = xml.Clone();
            // return null if there is no ID information
            if (string.IsNullOrEmpty(GetElementToString(xml, "CDR_id"))) return null;

            List<Identifier> lstPatientIdentifier = new List<Identifier>();
            List<HumanName> lstHumanName = new List<HumanName>();
            List<ContactPoint> lstPhones = new List<ContactPoint>();
            List<ResourceReference> lstGeneralPractitioner = new List<ResourceReference>();

            // Identifier => PID-3 (query only the TypeCode = 'MR'. Desconsider others)
            var PatientIdentifierListNodes = xml.SelectNodes(@"//PatientIdentifierList/ExtendedCompositeIDwithCheckDigit[IdentifierTypeCode='MR']");
            foreach (XmlNode item in PatientIdentifierListNodes)
            {
                lstPatientIdentifier.Add(
                    new Identifier
                    {
                        ElementId = GetElementToString(item, "Id"),
                        Value = GetElementToString(item, "IdNumber"),
                        
                        Type = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    Code = "MR"
                                }
                            },
                            //Text = GetElementToString(item, "IdentifierTypeCode"),
                            Text = "Medical record number"
                        }
                    });
            }

            // GeneralPractitioner => PID-4
            var GeneralPractitionerNodes = xml.SelectNodes(@"//PD1_PatientAdditionalDemographic_Set/PD1_PatientAdditionalDemographic/PatientPrimaryCareProviderNameIdNo/ExtendedCompositeIDNumberandNameforPersons");
            foreach (XmlNode item in GeneralPractitionerNodes)
            {
                var name = "Dr. " + GetElementToString(item, "GivenName") + " " + GetElementToString(item, "FamilyNameSurname");

                lstGeneralPractitioner.Add(
                    new ResourceReference
                    {
                        Display = name
                    });
            }

            // Name => PID-5, PID-9 (PatientName, PatientAlias)
            var PatientNameNodes = xml.SelectNodes(@"//PatientName/ExtendedPersonName");
            foreach (XmlNode item in PatientNameNodes)
            {
                lstHumanName.Add(
                   new HumanName
                   {
                       Family = GetElementToString(item, "FamilyNameSurname"),
                       Given = new List<string> {
                            GetElementToString(item, "GivenName"),
                            GetElementToString(item, "SecondAndFurtherGivenNamesOrInitialsThereof"),
                       },
                       Suffix = new List<string>
                       {
                            GetElementToString(item, "SuffixEGJrOrIii"),
                       },
                       Prefix = new List<string>
                       {
                            GetElementToString(item, "PrefixEGDr"),
                       },
                   });
            }

            // Telecom => PID-13, PID-14, PID-40
            var PhoneNumberHomeNodes = xml.SelectNodes(@"//PhoneNumberHome/ExtendedTelecommunicationNumber");
            foreach (XmlNode item in PhoneNumberHomeNodes)
            {
                lstPhones.Add(
                    new ContactPoint
                    {
                        System = ContactPoint.ContactPointSystem.Phone,
                        Value = GetElementToString(item, "TelephoneNumber"),
                    });

                lstPhones.Add(
                    new ContactPoint
                    {
                        System = ContactPoint.ContactPointSystem.Email,
                        Value = GetElementToString(item, "EmailAddress"),
                    });
            }

            // initialized a new Patient
            var patient = new Patient
            {
                Identifier = lstPatientIdentifier,
                Name = lstHumanName,
                Telecom = lstPhones,
                Id = GetElementToString(xml, "CDR_id"),
                Active = GetElementToBool(xml, "CDR_Patient/Active"),
                Gender = GetGenderFromHl7(GetElementToString(xml, "AdministrativeSex")),
                //BirthDate = Convert.ToDateTime(GetElementToString(xml, "DateTimeOfBirthTime")).ToShortDateString(),
                BirthDate = Convert.ToDateTime(GetElementToString(xml, "DateTimeOfBirthTime")).ToString("s"),
                VersionId = "1", // version must be filled to avoid runtime erros regarding the META field to populate the Http header
                GeneralPractitioner = lstGeneralPractitioner,
            };

            return patient;
        }

        // Private method to return the Fhir gender considering the Hl7 returned in the stored procedure
        private static AdministrativeGender? GetGenderFromHl7(string gender)
        {
            switch (gender)
            {
                case "F":
                    return AdministrativeGender.Female;
                case "M":
                    return AdministrativeGender.Male;
                case "O":
                    return AdministrativeGender.Other;
                case "U":
                    return AdministrativeGender.Unknown;
            }

            return null;
        }

        public static string GetHl7GenderFromAdministrativeGender(string gender)
        {
            // 0 = Male
            // 1 = Female
            // 2 = Other
            // 3 = Unknown
            switch (gender)
            {
                case "1":
                    return "F";
                case "0":
                    return "M";
                case "2":
                    return "O";
                case "3":
                    return "U";
                case "female":
                    return "F";
                case "male":
                    return "M";
            }

            return "";
        }
    }
}
