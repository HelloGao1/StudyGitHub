using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Partner.Data.Integration.Models;
using System.IO;

using FhirModel = Hl7.Fhir.Model;
using FhirBundleModel = Hl7.Fhir.Model.Bundle;
using Hl7.Fhir.Serialization;
using Partner.Data.Integration.Utils;

using Newtonsoft.Json;
using Hl7.Fhir.Model;
using System.Xml.Linq;
using System.Xml;

namespace Partner.Data.Integration.Business
{
    public class PatientService
    {
        private const string BearerToken = "Bearer BF54VOR3AJ33WKYEXEAYFFAEPJVR726V";

        /// <summary>
        /// Get patient list
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<PatientViewModel> GetPatientList(string filePath)
        {
            List<PatientViewModel> patientList = new List<PatientViewModel>();
            FhirXmlParser fjp = new FhirXmlParser();
            if (AppSettings.StoredPatientInAzure)
            {
                //get the patient information from Azure Storage
                List<KeyValuePair<string, string>> azurePatientList = AzureStorageHelper.ListAllBlobFromAzure("", ".xml");
                foreach (KeyValuePair<string, string> item in azurePatientList)
                {
                    var fhirPatient = fjp.Parse<Hl7.Fhir.Model.Patient>(item.Value);
                    if (fhirPatient != null)
                    {
                        patientList.Add(ConvertFhirToViewModel(fhirPatient));
                    }
                }
            }
            else
            {
                //get the patient information from local folder
                string[] jsonFiles = Directory.GetFiles(filePath, "*.xml");
                if (jsonFiles != null && jsonFiles.Length > 0)
                {
                    foreach (string fileName in jsonFiles)
                    {
                        var fhirPatient = fjp.Parse<Hl7.Fhir.Model.Patient>(File.ReadAllText(fileName));
                        if (fhirPatient != null)
                        {
                            patientList.Add(ConvertFhirToViewModel(fhirPatient));
                        }
                    }
                }
            }

            return patientList.OrderBy(p => p.FullName).ToList();
        }
        /// <summary>
        /// Search pathent from API
        /// </summary>
        /// <param name="family"></param>
        /// <param name="given"></param>
        /// <param name="dob"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public List<PatientViewModel> SearchPatients(string _id, string family, string given, string birthdate, string gender, string MRN)
        {
            List<PatientViewModel> results = new List<PatientViewModel>();
            string searchPatientUrl = AppSettings.FhirAPIBaseUrl + "Patient?";
            if (!string.IsNullOrEmpty(_id))
                searchPatientUrl += string.Format("_id={0}", _id);
            else
            {
                if (!string.IsNullOrEmpty(family))
                    searchPatientUrl += string.Format("family={0}", family);
                if (!string.IsNullOrEmpty(given))
                    searchPatientUrl += "&" + string.Format("given={0}", given);
                if (!string.IsNullOrEmpty(birthdate))
                    searchPatientUrl += "&" + string.Format("birthdate={0}", birthdate);
                if (!string.IsNullOrEmpty(gender))
                    searchPatientUrl += "&" + string.Format("gender={0}", GetGenderFromHl7(gender).ToString().ToLower());
                if (!string.IsNullOrEmpty(MRN))
                    searchPatientUrl += "&" + string.Format("identifier={0}", MRN);

                searchPatientUrl = searchPatientUrl.Replace("?&", "?");
            }

            //call API to get patient list
            string response = APIHelper.CallFhirApi(BearerToken, searchPatientUrl);

            if (CheckIfNotEmptyBundle(response))
            {
                FhirXmlParser fxp = new FhirXmlParser();

                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                results = ExtraFhirResourceFromBundle(bundle, "Patient").Select(r => ConvertFhirToViewModel((Patient)r)).ToList();
                //ConvertFhirBundleToViewModel(bundle);
            }
            return results.OrderBy(p => p.FullName).ToList();
        }

        /// <summary>
        /// get patient details information. (include demographic/encounter/observations...)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PatientViewModel GetPatientDetails(string filePath, string id)
        {
            PatientViewModel patient = GetPatient(filePath, id, false);

            //Get  encounter information
            string apiUrl = AppSettings.FhirAPIBaseUrl + string.Format("Encounter?patient={0}", id);
            //call API to get encounter
            string response = APIHelper.CallFhirApi(BearerToken, apiUrl);

            FhirXmlParser fxp = new FhirXmlParser();
            if (CheckIfNotEmptyBundle(response))
            {
                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                patient.PatientEncounters = ExtraFhirResourceFromBundle(bundle, "Encounter").Select(o => o as Encounter).ToList();
                patient.PatientEncounters.ForEach(item =>
                {
                    if (item.Location == null)
                        item.Location = new List<Encounter.LocationComponent>();
                });
            }
            else
            {
                patient.PatientEncounters = new List<Encounter>();
            }
            //set patient.location
            if (patient.PatientEncounters.Count > 0 &&
                patient.PatientEncounters[0].Location != null &&
                patient.PatientEncounters[0].Location.Count > 0 &&
                patient.PatientEncounters[0].Location[0] != null &&
                patient.PatientEncounters[0].Location[0].Location != null &&
                !string.IsNullOrEmpty(patient.PatientEncounters[0].Location[0].Location.Display))
                patient.Location = patient.PatientEncounters[0].Location[0].Location.Display;

            //Get observation information
            apiUrl = AppSettings.FhirAPIBaseUrl + string.Format("Observation?patient={0}", id);
            response = APIHelper.CallFhirApi(BearerToken, apiUrl);

            if (CheckIfNotEmptyBundle(response))
            {
                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                patient.PatientObservations = ExtraFhirResourceFromBundle(bundle, "Observation").Select(o => o as Observation).ToList();
            }
            else
            {
                patient.PatientObservations = new List<Observation>();
            }

            //Get Coverage 
            apiUrl = AppSettings.FhirAPIBaseUrl + string.Format("Coverage?patient={0}", id);
            response = APIHelper.CallFhirApi(BearerToken, apiUrl);

            if (CheckIfNotEmptyBundle(response))
            {
                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                patient.PatientCoverages = ExtraFhirResourceFromBundle(bundle, "Coverage").Select(o => o as Coverage).ToList();
            }
            else
            {
                patient.PatientCoverages = new List<Coverage>();
            }
            if (patient.PatientCoverages.Count > 0 &&
               patient.PatientCoverages[0].Grouping != null &&
               !string.IsNullOrEmpty(patient.PatientCoverages[0].Grouping.PlanDisplay))
                patient.Payor = patient.PatientCoverages[0].Grouping.PlanDisplay;
            //get AllergyIntolerance
            apiUrl = AppSettings.FhirAPIBaseUrl + string.Format("AllergyIntolerance?patient={0}", id);
            response = APIHelper.CallFhirApi(BearerToken, apiUrl);

            if (CheckIfNotEmptyBundle(response))
            {
                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                patient.PatientAllergyIntolerances = ExtraFhirResourceFromBundle(bundle, "AllergyIntolerance").Select(o => o as AllergyIntolerance).ToList();
                patient.PatientAllergyIntolerances.ForEach(item =>
                {
                    if (item.Reaction == null || item.Reaction.Count == 0)
                        item.Reaction = new List<AllergyIntolerance.ReactionComponent>()
                        {
                            new AllergyIntolerance.ReactionComponent()
                            {
                                 Substance = new CodeableConcept(){Text = ""},
                                 Manifestation = new List<CodeableConcept>(){
                                     new CodeableConcept(){Text = ""} 
                                 }
                            }
                        };

                    if (item.Reaction[0].Substance == null)
                    {
                        item.Reaction[0].Substance = new CodeableConcept() { Text = "" };
                    }

                    if (item.Reaction[0].Manifestation == null || item.Reaction[0].Manifestation.Count == 0)
                    {
                        item.Reaction[0].Manifestation = new List<CodeableConcept>(){
                                     new CodeableConcept(){Text = ""} 
                                 };
                    }
                });
            }
            else
            {
                patient.PatientAllergyIntolerances = new List<AllergyIntolerance>();
            }

            //get Medicatioin
            apiUrl = AppSettings.FhirAPIBaseUrl + string.Format("MedicationRequest?patient={0}", id);
            response = APIHelper.CallFhirApi(BearerToken, apiUrl);

            if (CheckIfNotEmptyBundle(response))
            {
                var bundle = fxp.Parse<Hl7.Fhir.Model.Bundle>(RemoveAllEmptyNode(response));

                patient.PatientMedications = ExtraFhirResourceFromBundle(bundle, "MedicationRequest").Select(o => o as MedicationRequest).ToList();
            }
            else
            {
                patient.PatientMedications = new List<MedicationRequest>();
            }

            return patient;
        }

        /// <summary>
        /// save patient demography to file/Azure Storage
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PatientViewModel GetPatient(string savePath, string id, bool saveToFile = true)
        {
            string getPatientUrl = AppSettings.FhirAPIBaseUrl + string.Format("Patient/{0}", id);

            //call API to get patient list
            string response = APIHelper.CallFhirApi(BearerToken, getPatientUrl);

            string fhirXml = RemoveAllEmptyNode(response);
            if (saveToFile)
            {
                if (AppSettings.StoredPatientInAzure)
                {
                    //stored the patient information into azure storage to demo
                    MemoryStream stream = new MemoryStream();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(fhirXml);
                    writer.Flush();
                    stream.Position = 0;

                    AzureStorageHelper.UploadBlobToAzure("", string.Format("{0}.xml", id), stream);
                }
                else
                {
                    //stored the patient information into local folder to demo
                    string saveFilePath = Path.Combine(savePath, string.Format("{0}.xml", id));
                    File.WriteAllText(saveFilePath, fhirXml);
                }
            }

            FhirXmlParser fxp = new FhirXmlParser();
            var patient = fxp.Parse<Hl7.Fhir.Model.Patient>(fhirXml);
            return ConvertFhirToViewModel(patient);
        }

        /// <summary>
        /// remove the patient from list
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="id"></param>
        public void RemovePatient(string savePath, string id)
        {
            if (AppSettings.StoredPatientInAzure)
            {
                AzureStorageHelper.DeleteBlob("", id + ".xml");
            }
            else
            {
                //stored the patient information into local folder to demo
                string saveFilePath = Path.Combine(savePath, string.Format("{0}.xml", id));
                if(File.Exists(saveFilePath))
                    File.Delete(saveFilePath);
            }
        }

        /// <summary>
        /// convert Fhir patient response to Patient view model to display
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        private PatientViewModel ConvertFhirToViewModel(FhirModel.Patient patient)
        {
            return new PatientViewModel()
            {
                Id = Int32.Parse(patient.Id),
                LastName = patient.Name[0].Family,
                FirstName = patient.Name[0].Given.ToList()[0],
                DOB = DateTime.Parse(patient.BirthDate),
                Gender = patient.Gender.Value.ToString(),
                MRN = patient.Identifier.Count > 0 ? patient.Identifier.First(i => i.Type.Coding[0].Code == "MR").Value : ""
            };
        }
        
        /// <summary>
        /// Extra Fhir resource list from fhir bundle.entry.
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private List<Resource> ExtraFhirResourceFromBundle(FhirModel.Bundle bundle, string resourceName)
        {
            List<Resource> fhirResourceList = new List<Resource>();

            if (bundle.Entry != null && bundle.Entry.Count > 0)
            {
                bundle.Entry.ForEach(e =>
                {
                    if (e.Resource.ResourceType.ToString() == resourceName)
                        fhirResourceList.Add(e.Resource);
                });
            }

            return fhirResourceList;
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

        /// <summary>
        /// remove all empty attribute/node from fhir api response
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        private string RemoveAllEmptyNode(string xmlString)
        {
            XElement rootElement = XElement.Parse(xmlString);

            rootElement.Descendants().Where(e => e.Attribute("value") != null && string.IsNullOrEmpty(e.Attribute("value").Value)).Remove();

            for (int i = 0; i < 5; i++)
            {
                rootElement.Descendants().Where(e => !e.HasElements && !e.HasAttributes).Remove();
            }

            return rootElement.ToString();
        }

        /// <summary>
        /// check if the response is an empty fhir bundle from API
        /// </summary>
        /// <param name="fhirXml"></param>
        /// <returns></returns>
        private bool CheckIfNotEmptyBundle(string fhirXml)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(fhirXml);

            return xDoc.DocumentElement.HasChildNodes;
        }
    }
}