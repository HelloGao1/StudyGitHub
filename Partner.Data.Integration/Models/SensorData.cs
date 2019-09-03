using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Partner.Data.Integration.Models
{
    public class SensorData
    {
        /// <summary>
        ///  Current patient's uniqueidentify.
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        ///  Shimmer device name.
        /// </summary>
        public string SensorDevice { get; set; }

        public IList<MeasureData> Measures { get; set; }
    }
}