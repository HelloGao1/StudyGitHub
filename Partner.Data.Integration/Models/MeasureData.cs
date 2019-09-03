using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Partner.Data.Integration.Models
{
    public class MeasureData
    {
        public MeasureData(string name, string format, string unit, double value)
        {
            this.Name = name;
            this.Format = format;
            this.Unit = unit;
            this.Value = value;
            this.TimeStamp = DateTime.UtcNow.Ticks;
        }
        public string Name { get; set; }
        public string Format { get; set; }
        public string Unit { get; set; }
        public double Value { get; set; }
        /// <summary>
        ///  UTC datetime's Ticks.
        /// </summary>
        public long TimeStamp { get; set; }
    }
}