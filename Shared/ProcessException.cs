using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ProcessException : Exception
    {
        public ProcessException(string? message) : base(message)
        {
        }
    }
}
