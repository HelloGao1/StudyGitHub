using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Teams.Integration.Fhir.Services.Mapping;

namespace Teams.Integration.Fhir.Services.Data
{
    public class PatientData : BaseData
    {
        public XmlDocument GetXmlForFihr_Patient(IEnumerable<KeyValuePair<string, string>> parameters, int Count)
        {
            var id = parameters.Where(p => p.Key.Equals("_id")).FirstOrDefault().Value;
            var family = parameters.Where(p => p.Key.Equals("family")).FirstOrDefault().Value;
            var given = parameters.Where(p => p.Key.Equals("given")).FirstOrDefault().Value;
            var birthdate = parameters.Where(p => p.Key.Equals("birthday")).FirstOrDefault().Value;
            var gender = parameters.Where(p => p.Key.Equals("gender")).FirstOrDefault().Value;
            var identifier = parameters.Where(p => p.Key.Equals("identifier")).FirstOrDefault().Value;

            // Create a xmldocument to map the fields
            XmlDocument xml = new XmlDocument();

            // Open connection with CDR and query Resource
            using (var con = new SqlConnection(GetConnectionString()))
            {
                // Call the procedure to the specific resource
                var cmd = new SqlCommand("sp_GetXmlForFihr_Patient", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // add procedure parameters
                if (!string.IsNullOrEmpty(id)) cmd.Parameters.AddWithValue("@id", Convert.ToInt64(id));
                if (!string.IsNullOrEmpty(family)) cmd.Parameters.AddWithValue("@family", family);
                if (!string.IsNullOrEmpty(given)) cmd.Parameters.AddWithValue("@given", given);
                if (!string.IsNullOrEmpty(birthdate)) cmd.Parameters.AddWithValue("@birthdate", birthdate);
                if (!string.IsNullOrEmpty(gender)) cmd.Parameters.AddWithValue("@gender", PatientMapping.GetHl7GenderFromAdministrativeGender(gender));
                if (!string.IsNullOrEmpty(identifier)) cmd.Parameters.AddWithValue("@identifier", identifier);
                cmd.Parameters.AddWithValue("@count", Convert.ToInt64(Count));

                // open the connection with DB Server
                con.Open();

                // read the XML returned from the procedure
                XmlReader reader = cmd.ExecuteXmlReader();
                reader.Read();

                xml.Load(reader);

                // close connection with DB Server
                con.Close();
            }

            return xml;
        }
    }
}
