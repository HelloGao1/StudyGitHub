using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Partner.Data.Integration.Models
{
    public class PatientSearchModel
    {
        [Display(Name = "Family Name")]
        public string Family { get; set; }
        [Display(Name = "Given Name")]
        public string Given { get; set; }
        [Required]
        [Display(Name = "DOB")]
        public string DOB { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "MRN")]
        public string MRN { get; set; }
        [Required]
        [Display(Name = "ID")]
        public string Id { get; set; }
        public List<PatientViewModel> PatientSearchResult { get; set; }

        public SelectList GenderList {
            get{ 
                List<KeyValuePair<string, string>> genderList = new List<KeyValuePair<string, string>>();
                genderList.Add(new KeyValuePair<string, string>("F", "Female"));
                genderList.Add(new KeyValuePair<string, string>("M", "Male"));
                genderList.Add(new KeyValuePair<string, string>("O", "Other"));
                genderList.Add(new KeyValuePair<string, string>("U", "Unknown"));

                return new SelectList(genderList, "Key", "Value");
            }
        }
    }

}