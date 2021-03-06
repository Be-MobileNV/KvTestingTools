﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Spookfiles.Testing.Testrunners
{
    public class HttpValidationTests
    {
        public static bool IsValidJson(WebHeaderCollection headers)
        {
            try
            {
                return headers["Content-Type"].ToLowerInvariant().Contains("application/json");
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// According to HTTP specs you can use content-type or charset to validate the text encoding.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static bool IsValidCharsetUtf8(WebHeaderCollection headers)
        {
            try
            {
                return headers["Content-Type"].ToLowerInvariant().Contains("charset=utf-8") ||
       headers["Charset"].ToLowerInvariant().Contains("utf-8");
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
