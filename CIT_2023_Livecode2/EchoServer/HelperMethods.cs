using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    internal class HelperMethods
    {
        public static bool HasProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static bool IsValidJson(string input)
        {
            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (Exception)
            {
                // Failed to parse, not a valid JSON string
                return false;
            }
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
