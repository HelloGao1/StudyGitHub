using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Data
{
    public class AllergyIntoleranceData : BaseData
    {
        public XmlDocument GetXmlForFihr_AllergyIntolerance(IEnumerable<KeyValuePair<string, string>> parameters, int Count)
        {
            var patientId = parameters.Where(p => p.Key.Equals("patient")).FirstOrDefault().Value;

            // Create a xmldocument to map the fields
            XmlDocument xml = new XmlDocument();

            // Open connection with CDR and query Resource
            using (var con = new SqlConnection(GetConnectionString()))
            {
                // Call the procedure to the specific resource
                var cmd = new SqlCommand("sp_GetXmlForFihr_AllergyIntolerance", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // add procedure parameters
                cmd.Parameters.AddWithValue("@patientId", Convert.ToInt64(patientId));

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
