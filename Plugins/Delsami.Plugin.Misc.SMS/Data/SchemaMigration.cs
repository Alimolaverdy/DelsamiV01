using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Data
{

    [NopMigration("2026/11/26 13:00:00", "Delsami.Plugin.Misc.SMS base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<QueuedSMS>();
        }
    }
}
