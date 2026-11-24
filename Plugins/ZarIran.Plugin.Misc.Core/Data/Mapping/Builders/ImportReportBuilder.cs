using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Domain;

namespace ZarIran.Plugin.Misc.Core.Data.Mapping.Builders
{
    public class ImportReportBuilder : NopEntityBuilder<ImportReport>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ImportReport.CustomerId)).AsInt32().NotNullable()
                .WithColumn(nameof(ImportReport.ImportType)).AsInt32().NotNullable()
                .WithColumn(nameof(ImportReport.CreatedOn)).AsDateTime2().NotNullable()
                .WithColumn(nameof(ImportReport.OrginalFile)).AsBinary().NotNullable();
        }
    }
}
