using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HKAdBus.Others
{
    public class QueryStringConverter
    {
        public static Dictionary<string, object> GetQueries(NameValueCollection queryString)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in queryString.AllKeys)
                result.Add(key, queryString[key]);
            return result;
        }
    }
}