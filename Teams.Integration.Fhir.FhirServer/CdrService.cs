using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teams.Integration.Fhir.Services;

namespace Teams.Integration.Fhir.FhirServer
{
    public class CdrService : IFhirSystemServiceSTU3
    {
        public CapabilityStatement GetConformance(ModelBaseInputs request, SummaryType summary)
        {
            CapabilityStatement con = new CapabilityStatement
            {
                Url = request.BaseUri + "metadata",
                Description = new Markdown("CDR based FHIR server"),
                DateElement = new Hl7.Fhir.Model.FhirDateTime("2017-04-30"),
                Version = "1.0.0.0",
                Name = "CDR FHIR API",
                Experimental = true,
                Status = PublicationStatus.Active,
                FhirVersion = Hl7.Fhir.Model.ModelInfo.Version,
                AcceptUnknown = CapabilityStatement.UnknownContentCode.Extensions,
                Format = new string[] { "xml", "json" },
                Kind = CapabilityStatement.CapabilityStatementKind.Instance,
                Meta = new Meta(),
                Rest = new List<CapabilityStatement.RestComponent>()
                {
                    new CapabilityStatement.RestComponent
                    {
                        Security = new CapabilityStatement.SecurityComponent
                        {
                            Extension = new List<Extension>()
                            {
                                new Extension()
                                {
                                    Url = "http://fhir-registry.smarthealthit.org/StructureDefinition/oauth-uris",
                                    Extension = new List<Extension>()
                                    {
                                        new Extension
                                        {
                                            Url = "token",
                                            Value = new FhirString("http://teams-fhir-auth.dapasoft.com/connect/token")
                                        },
                                        new Extension
                                        {
                                            Url = "authorize",
                                            Value = new FhirString("http://teams-fhir-auth.dapasoft.com/connect/authorize")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            };
            con.Meta.LastUpdatedElement = Instant.Now();

            con.Rest = new List<CapabilityStatement.RestComponent>
            {
                new CapabilityStatement.RestComponent()
                {
                    Operation = new List<CapabilityStatement.OperationComponent>()
                }
            };
            con.Rest[0].Mode = CapabilityStatement.RestfulCapabilityMode.Server;
            con.Rest[0].Resource = new List<CapabilityStatement.ResourceComponent>();

            //foreach (var model in ModelFactory.GetAllModels(GetInputs(buri)))
            //{
            //    con.Rest[0].Resource.Add(model.GetRestResourceComponent());
            //}

            return con;
        }

        public IFhirResourceServiceSTU3 GetResourceService(ModelBaseInputs request, string resourceName)
        {
            switch (resourceName.ToUpper())
            {
                case "PATIENT":
                    return new PatientService() { RequestDetails = request, ResourceName = resourceName };
                case "OBSERVATION":
                    return new ObservationService() { RequestDetails = request, ResourceName = resourceName };
                case "ALLERGYINTOLERANCE":
                    return new AllergyIntoleranceService() { RequestDetails = request, ResourceName = resourceName };
                case "ENCOUNTER":
                    return new EncounterService() { RequestDetails = request, ResourceName = resourceName };
                case "COVERAGE":
                    return new CoverageService() { RequestDetails = request, ResourceName = resourceName };
                case "CONDITION":
                    return new ConditionService() { RequestDetails = request, ResourceName = resourceName };
                case "MEDICATIONREQUEST":
                    return new MedicationRequestService() { RequestDetails = request, ResourceName = resourceName };
            }

            throw new Exception("Resource not found");
        }

        public Resource PerformOperation(ModelBaseInputs request, string operation, Parameters operationParameters, SummaryType summary)
        {
            throw new NotImplementedException();
        }

        public Bundle ProcessBatch(ModelBaseInputs request, Bundle bundle)
        {
            throw new NotImplementedException();
        }

        public Bundle Search(ModelBaseInputs request, IEnumerable<KeyValuePair<string, string>> parameters, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }

        public Bundle SystemHistory(ModelBaseInputs request, DateTimeOffset? since, DateTimeOffset? Till, int? Count, SummaryType summary)
        {
            throw new NotImplementedException();
        }
    }
}
