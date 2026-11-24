using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Domain;

namespace ZarIran.Plugin.Misc.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(City), $"ZI_C_{nameof(City)}"},
            { typeof(OTP), $"ZI_C_{nameof(OTP)}"},
            { typeof(VirtualProductMapping), $"ZI_C_{nameof(VirtualProductMapping)}"},
            { typeof(ImportReport), $"ZI_C_{nameof(ImportReport)}s"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
