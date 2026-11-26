using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.SMS.Exceptions
{
    public class KavehNegarException : Exception
    {
        public KavehNegarException(string message) : base(message) { }
    }
}
