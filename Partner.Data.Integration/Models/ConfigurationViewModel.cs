using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Partner.Data.Integration.Models
{
    public class ConfigurationViewModel
    {
        [Required]
        [Display(Name = "URL")]
        public string URL { get; set; }
        [Required]
        [Display(Name = "Tab Name")]
        public string TabName { get; set; }
    }
}