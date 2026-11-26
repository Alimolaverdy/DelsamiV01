using Nop.Core;
using System;

namespace Delsami.Plugin.Shipping.IRPost.Domain
{
    public class PriceRange : BaseEntity
    {
        public int ModifierCustomerId { get; set; }

        public decimal MinWieght { get; set; }

        public bool ToUpperWeight { get; set; }

        public decimal? MaxWeight { get; set; }

        public decimal NeighbouringPrice { get; set; }

        public decimal NonNeighbouringPrice { get; set; }

        public bool Deleted { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime? EndDateUtc { get; set; }
    }
}
