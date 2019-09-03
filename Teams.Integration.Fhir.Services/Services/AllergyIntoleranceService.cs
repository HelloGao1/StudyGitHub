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
    public class AllergyIntoleranceService : BaseService, IFhirResourceServiceSTU3
    {
        private AllergyIntoleranceData dataAccess;
        public AllergyIntoleranceService()
        {
            dataAccess = new AllergyIntoleranceData();
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

            XmlDocument xml = dataAccess.GetXmlForFihr_AllergyIntolerance(parameters, (int)Count);

            // Map the fields from the stored procedure to Fhir model
            Bundle allergyIntoleranceBundle = AllergyIntoleranceMapping.MapFromCDRToFHirModelBundle(xml, uri);

            return allergyIntoleranceBundle;
        }

        public Bundle TypeHistory(DateTimeOffset? since, DateTimeOffset? Till, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }
    }
}
