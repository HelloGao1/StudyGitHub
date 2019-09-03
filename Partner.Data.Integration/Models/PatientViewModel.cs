using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Hl7.Fhir.Model;

namespace Partner.Data.Integration.Models
{
    public class PatientViewModel
    {
        #region Demography
        public int Id { get; set; }

        [Required]
        [Display(Name = "Family Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Given Name")]
        public string FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "DOB")]
        public DateTime DOB { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "MRN")]
        public string MRN { get; set; }
        public string CDRId { get {
                return this.Id.ToString();
            } }

        [Display(Name = "Name")]
        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        public string DOBFormatted
        {
            get
            {
                return this.DOB.ToString("yyyy-MM-dd");
            }
        }

        [Display(Name = "Age")]
        public string Age {
            get
            {
                DateTime now = DateTime.Now;
                int age = now.Year - this.DOB.Year;
                if(this.DOB.Month > now.Month || (this.DOB.Month == now.Month && this.DOB.Day > now.Day))
                {
                    age--;
                }
                if (age < 0)
                    age = 0;

                return age.ToString();
            }
        }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Payor")]
        public string Payor { get; set; }
        #endregion

        [Display(Name = "Encounter")]
        public List<Encounter> PatientEncounters { get; set; }

        [Display(Name = "Observation")]
        public List<Observation> PatientObservations { get; set; }

        [Display(Name = "AllergyIntolerance")]
        public List<AllergyIntolerance> PatientAllergyIntolerances { get; set; }

        [Display(Name = "Coverage")]
        public List<Coverage> PatientCoverages { get; set; }

        [Display(Name = "Medication")]
        public List<MedicationRequest> PatientMedications { get; set; }
    }
}