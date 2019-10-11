using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADUC2.Models
{
    public class AppException : Exception
    {
        public AppException(string message)
            : base(message) { }

        public AppException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}