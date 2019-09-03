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
    public class ObservationService : BaseService, IFhirResourceServiceSTU3
    {
        private ObservationData dataAccess;
        public ObservationService()
        {
            dataAccess = new ObservationData();
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
            throw new NotImplementedException();
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
            XmlDocument xml = dataAccess.GetXmlForFihr_Observation(parameters);

            // Map the fields from the CDR xml to Fhir model
            Bundle observationBundle = ObservationMapping.MapFromCDRToFHirModelBundle(xml, uri);

            return observationBundle;
        }

        public Bundle TypeHistory(DateTimeOffset? since, DateTimeOffset? Till, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }
    }
}
