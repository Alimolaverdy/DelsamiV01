using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Enums;

namespace ZarIran.Plugin.Misc.Core.Domain
{
    public class ImportReport : BaseEntity
    {
        public int CustomerId { get; set; }
        public ImportTypes ImportType { get; set; }
        public byte[] OrginalFile { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
