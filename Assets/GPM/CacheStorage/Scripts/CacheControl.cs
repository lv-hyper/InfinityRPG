using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gpm.CacheStorage
{
    [Serializable]
    public class CacheControl
    {
        private static readonly char[] separatorCommas = new char[] { ',' };
        private static readonly char[] separatorEqual = new char[] { '=' };
        public StringToValue<int> maxAge;

        public bool noCache;
        public bool noStore;
        public bool mustRevalidate;

        public CacheControl()
        {

        }

        public static Dictionary<string, string> GetElements(string cacheControl)
        {
            Dictionary<string, string> dicElements = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(cacheControl) == false)
            {
                string[] elements = cacheControl.Split(separatorCommas, StringSplitOptions.RemoveEmptyEntries);
                foreach (string element in elements)
                {
                    string[] split = element.Trim().Split(separatorEqual, StringSplitOptions.RemoveEmptyEntries);

                    string key = split[0].ToLower();
                    string value = string.Empty;
                    if (split.Length > 1)
                    {
                        value = split[1];
                    }
                    dicElements.Add(key, value);
                }
            }

            return dicElements;
        }

        public CacheControl(Dictionary<string, string> elements)
        {
            string getValue;
            if (elements.TryGetValue("max-age", out getValue) == true)
            {
                maxAge = getValue;
            }
            else
            {
                maxAge = null;
            }
            noStore = elements.ContainsKey("no-store") == true;
            noCache = elements.ContainsKey("no-cache") == true;
            mustRevalidate = elements.ContainsKey("must-revalidate");
        }
    }
}
