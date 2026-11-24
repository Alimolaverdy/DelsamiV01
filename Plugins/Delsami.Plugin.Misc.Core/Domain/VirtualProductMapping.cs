using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Domain
{
    public class VirtualProductMapping : BaseEntity
    {
        public int VirtualProductId { get; set; }

        public int? MainProductId { get; set; }

        public bool Deleted { get; set; }
    }
}
