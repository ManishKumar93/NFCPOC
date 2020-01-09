using System;
using System.Collections.Generic;
using System.Text;

namespace NFCPOC.Helpers
{
    public class LoggerHelper
    {
        public static void LogException(Exception exc)
        {
            string message = exc.Message;
        }
    }
}
