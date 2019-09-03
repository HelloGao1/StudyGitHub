using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class MedicationRequestMapping : BaseMapping
    {
        public static Bundle MapFromCDRToFHirModelBundle(string uri)
        {
            Bundle bundle = new Bundle();

            MedicationRequest medicationStatement = MapFromCDRToFHirModel();

            //bundle.AddResourceEntry(medicationStatement, $"{uri}/{medicationStatement.Id}");
            bundle.AddResourceEntry(medicationStatement, string.Format("{0}/{1}", uri, medicationStatement.Id ));

            return bundle;
        }

        public static MedicationRequest MapFromCDRToFHirModel()
        {
            Random gen = new Random();
            int range = 5 * 365; //5 years          
            DateTime randomDate = DateTime.Today.AddDays(-gen.Next(range));

            MedicationRequest medicationRequest = new MedicationRequest
            {
                Medication = new CodeableConcept
                {

                    Coding = new List<Coding>
                    {
                       new Coding
                       {
                           Code = "197884",
                           Display = "Lisinopril 40 MG Oral Tablet",
                           System = "http://www.nlm.nih.gov/research/umls/rxnorm",
                           
                       },
                       
                       
                    },
                    Text = "Lisinopril 40 MG Oral Tablet"

                },
                AuthoredOn = new FhirDateTime("2017-08-01").ToString(),
                DosageInstruction = new List<Dosage>()
                {
                    new Dosage
                    {
                        Text = "Take Lisinopril 40mg tablet once a day for high blood pressure.",
                        Timing = new Timing()
                        {

                            Code = new CodeableConcept()
                            {

                            },
                            Repeat = new Timing.RepeatComponent() {
                                Count = 0,
                                Period = 1,
                                PeriodUnit = Timing.UnitsOfTime.D,
                                Frequency = 1,
                                Bounds = new Period
                                {
                                    Start = "2017-08-01",
                                    End = "2018-06-30",
                                }

                            }
                        },
                        AsNeeded = new FhirBoolean(false),
                        Route = new CodeableConcept
                        {
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "http://snomed.info/sct",
                                    Display = "Oral route",
                                    Code = "26643006"
                                }
                            }
                        },
                        Rate = new Quantity
                        {
                            Value = 40,
                            Unit = "mG"
                        }
                    }, 
                },
                Requester = new MedicationRequest.RequesterComponent
                {
                    Agent = new ResourceReference
                    {
                        Reference = "Practitioner/cc-prac-carlson-john",
                        Display = "Dr. John Carlson, MD"
                    }
                },
                Subject = new ResourceReference
                {
                    Reference = "Patient/6",
                    Display = "Betsy"
                },
                ReasonReference = new List<ResourceReference>
                {
                    new ResourceReference
                    {
                        Reference = "Condition/cc-cond-betsy-hypertension"
                    }
                },
                
            };

            return medicationRequest;
        }
    }
}
