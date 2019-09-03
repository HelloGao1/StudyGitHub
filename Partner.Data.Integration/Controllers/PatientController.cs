using Partner.Data.Integration.Business;
using Partner.Data.Integration.Models;
using Partner.Data.Integration.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Partner.Data.Integration.Controllers
{
    public class PatientController : Controller
    {
        private PatientService patientService = new PatientService();

        /// <summary>
        /// Get Patient List from local file
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string filePath = string.Empty;
            if (!AppSettings.StoredPatientInAzure)
                filePath = Server.MapPath("~/TempPatientData");

            List<PatientViewModel> patientList = patientService.GetPatientList(filePath);

            return View(patientList);
        }

        /// <summary>
        /// Get patient list
        /// </summary>
        /// <returns></returns>
        public JsonResult List()
        {
            string filePath = string.Empty;
            if (!AppSettings.StoredPatientInAzure)
                filePath = Server.MapPath("~/TempPatientData");
            List<PatientViewModel> patientList = patientService.GetPatientList(filePath);
            return Json(patientList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Patient search partial view
        /// </summary>
        /// <returns></returns>
        public PartialViewResult SearchPatient()
        {
            PatientSearchModel searchModel = new PatientSearchModel();

            return PartialView(searchModel);
        }
        /// <summary>
        /// Search Patient  from Fhir Service
        /// </summary>
        /// <param name="id"></param>
        /// <param name="familyName"></param>
        /// <param name="givenName"></param>
        /// <param name="dob"></param>
        /// <param name="gender"></param>
        /// <param name="mrn"></param>
        /// <returns></returns>
        public JsonResult SearchPatientList(string id, string familyName, string givenName, string dob, string gender, string mrn)
        {
            List<PatientViewModel> patientList = new List<PatientViewModel>();
            PatientSearchModel search = new PatientSearchModel()
            {
                Id = id == "0" ? null : id,
                Family = familyName,
                Given = givenName,
                DOB = dob,
                Gender = gender,
                MRN = mrn
            };

            if (!string.IsNullOrEmpty(search.Id) ||
                !string.IsNullOrEmpty(search.Family) ||
                !string.IsNullOrEmpty(search.Given) ||
                !string.IsNullOrEmpty(search.DOB) ||
                !string.IsNullOrEmpty(search.Gender) ||
                !string.IsNullOrEmpty(search.MRN))
                patientList = patientService.SearchPatients(search.Id, search.Family, search.Given, search.DOB, search.Gender, search.MRN);

            return Json(patientList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add Patient to List
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult AddPatientToList(string id)
        {
            string filePath = string.Empty;
            if (!AppSettings.StoredPatientInAzure)
                filePath = Server.MapPath("~/TempPatientData");

            patientService.GetPatient(filePath, id);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// remove patient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult RemovePatientFromList(string id)
        {
            try
            {
                string filePath = string.Empty;
                if (!AppSettings.StoredPatientInAzure)
                    filePath = Server.MapPath("~/TempPatientData");

                patientService.RemovePatient(filePath, id);
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// display patient details page (include LiveData/DI Image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            else
            {
                string filePath = string.Empty;
                if (!AppSettings.StoredPatientInAzure)
                    filePath = Server.MapPath("~/TempPatientData");

                PatientViewModel patient = patientService.GetPatientDetails(filePath, id);

                return View(patient);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Configure()
        {
            ConfigurationViewModel model = new ConfigurationViewModel()
            {
                URL = "https://partner-data-integration.azurewebsites.net/",
                TabName = "Patient (Demo)"
            };
            return View(model);
        }
    }
}