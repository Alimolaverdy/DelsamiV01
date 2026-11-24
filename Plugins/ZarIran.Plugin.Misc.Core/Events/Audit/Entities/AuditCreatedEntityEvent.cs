using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Events.Audit.Base;

namespace ZarIran.Plugin.Misc.Core.Events.Audit.Entities
{
    public class AuditCreatedEntityEvent<T> : AuditEntityBaseEvent<T>
    {

        public AuditCreatedEntityEvent(T Entity, dynamic parentEntity = default, string descriptions = null)
        {
            CurrentEntity = Entity;
            Descriptions = descriptions;
            ParentEntity = parentEntity;

        }
    }
}