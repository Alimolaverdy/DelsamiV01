using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Infrastructure.Enums;

namespace Delsami.Plugin.Misc.SMS.Exceptions
{
    public class ApiException : KavehNegarException
    {
        readonly MetaCode _result;
        public ApiException(string message, int code) : base(message)
        {

        }

        public MetaCode Code
        {
            get
            {
                return _result;
            }
        }
    }
}
