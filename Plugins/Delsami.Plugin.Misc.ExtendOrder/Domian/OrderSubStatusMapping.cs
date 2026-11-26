using Nop.Core;
using System;

namespace Delsami.Plugin.Misc.ExtendedOrder.Domian
{
    public class OrderSubStatusMapping : BaseEntity
    {
        public int OrderId { get; set; }
        public int SubStatusId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
