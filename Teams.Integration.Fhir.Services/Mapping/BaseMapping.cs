using System;
using System.Xml;

namespace Teams.Integration.Fhir.Services.Mapping
{
    public class BaseMapping
    {
        public static string GetElementToString(XmlDocument xml, string xpath)
        {
            try
            {
                return xml.SelectSingleNode(xpath).InnerText;
            }
            catch
            {
                return "";
            }
        }

        public static string GetElementToString(XmlNode node, string xpath)
        {
            try
            {
                return node.SelectSingleNode(xpath).InnerText;
            }
            catch
            {
                return "";
            }
        }

        public static bool? GetElementToBool(XmlDocument xml, string xpath)
        {
            try
            {
                var value = xml.SelectSingleNode(xpath).InnerText;
                return Convert.ToBoolean(Convert.ToInt16(value));
            }
            catch
            {
                return null;
            }
        }

        public static bool? GetElementToBool(XmlNode node, string xpath)
        {
            try
            {
                var value = node.SelectSingleNode(xpath).InnerText;
                return Convert.ToBoolean(Convert.ToInt16(value));
            }
            catch
            {
                return null;
            }
        }

        public static decimal? GetElementToDecimal(XmlDocument xml, string xpath)
        {
            try
            {
                var value = xml.SelectSingleNode(xpath).InnerText;
                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? GetElementToDecimal(XmlNode node, string xpath)
        {
            try
            {
                var value = node.SelectSingleNode(xpath).InnerText;
                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }

        public static int? GetElementToInt(XmlDocument xml, string xpath)
        {
            try
            {
                var value = xml.SelectSingleNode(xpath).InnerText;
                return Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        public static Int64? GetElementToBigInt(XmlDocument xml, string xpath)
        {
            try
            {
                var value = xml.SelectSingleNode(xpath).InnerText;
                return Convert.ToInt64(value);
            }
            catch
            {
                return null;
            }
        }

        public static decimal? ConvertStringToDecimal(string value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return null;
            }
        }
    }
}
