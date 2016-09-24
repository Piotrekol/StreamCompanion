using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osu_StreamCompanion.Code.Helpers
{
    public class NonLoggableException : Exception
    {
        public Exception Exception { get; }
        public string CustomMessage { get; set; }
        public NonLoggableException(Exception ex, string message)
        {
            this.Exception = ex;
            this.CustomMessage = message;
        }

    }
}
