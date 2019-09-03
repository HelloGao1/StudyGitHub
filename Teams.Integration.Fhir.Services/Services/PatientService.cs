using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.WebApi;
using System;
using System.Collections.Generic;
using System.Xml;
using Teams.Integration.Fhir.Services.Data;
using Teams.Integration.Fhir.Services.Mapping;

namespace Teams.Integration.Fhir.Services
{
    public class PatientService : BaseService, IFhirResourceServiceSTU3
    {
        private PatientData dataAccess;
        public PatientService()
        {
            dataAccess = new PatientData();
        }

        public ModelBaseInputs RequestDetails { get; set; }

        public string ResourceName { get; set; }

        public Resource Create(Resource resource, string ifMatch, string ifNoneExist, DateTimeOffset? ifModifiedSince)
        {
            throw new NotImplementedException();
        }

        public string Delete(string id, string ifMatch)
        {
            throw new NotImplementedException();
        }

        public Resource Get(string resourceId, string VersionId, SummaryType summary)
        {
            // Creating params to GetXmlForFihr_Patient
            var parameters = new[] {
              new KeyValuePair<string,string>("_id", resourceId)
            };

            // Get XML from CDR
            XmlDocument xml = dataAccess.GetXmlForFihr_Patient(parameters, 1);

            // Mapping one patient resource 
            Patient patient = PatientMapping.MapFromCDRToFHirModel(xml);

            if (patient == null) 
                throw new Exception(string.Format("Patient {0}, v{1} not found",resourceId, VersionId));
                //throw new Exception($"Patient {resourceId}, v{VersionId} not found");

            return patient;
        }

        public CapabilityStatement.ResourceComponent GetRestResourceComponent()
        {
            throw new NotImplementedException();
        }

        public Bundle InstanceHistory(string ResourceId, DateTimeOffset? since, DateTimeOffset? Till, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }

        public Resource PerformOperation(string id, string operation, Parameters operationParameters, SummaryType summary)
        {
            throw new NotImplementedException();
        }

        public Resource PerformOperation(string operation, Parameters operationParameters, SummaryType summary)
        {
            throw new NotImplementedException();
        }

        public Bundle Search(IEnumerable<KeyValuePair<string, string>> parameters, int? Count, SummaryType summary)
        {
            var uri = GetUrl(RequestDetails.RequestUri.AbsoluteUri);

            // Get XML from CDR
            XmlDocument xml = dataAccess.GetXmlForFihr_Patient(parameters, (int)Count);

            // Map the fields from the CDR xml to Fhir model
            Bundle patientBundle = PatientMapping.MapFromCDRToFHirModelBundle(xml, uri);

            return patientBundle;
        }

        public Bundle TypeHistory(DateTimeOffset? since, DateTimeOffset? Till, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }
    }
}
