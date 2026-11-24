using Nop.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Domain;

namespace Delsami.Plugin.Misc.Core.Data.Mapping
{
    public class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(City), $"D_C_{nameof(City)}"},
            { typeof(OTP), $"D_C_{nameof(OTP)}"},
            { typeof(VirtualProductMapping), $"D_C_{nameof(VirtualProductMapping)}"},
            { typeof(ImportReport), $"D_C_{nameof(ImportReport)}s"},
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string> { };
    }
}
