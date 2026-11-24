using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.SMS.Model;

namespace ZarIran.Plugin.Misc.SMS.Data
{

    [NopMigration("2025/10/20 13:00:00", "ZarIran.Plugin.Misc.SMS base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<QueuedSMS>();
        }
    }
}
