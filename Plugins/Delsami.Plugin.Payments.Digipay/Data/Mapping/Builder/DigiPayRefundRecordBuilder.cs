using FluentMigrator.Builders.Create.Table;
using ZarIran.Plugin.Payments.Digipay.Domain;
using Nop.Data.Mapping.Builders;

namespace ZarIran.Plugin.Payments.Digipay.Data.Mapping.Builder
{
	public class DigiPayRefundRecordBuilder : NopEntityBuilder<DigiPayRefundRecord>
	{
		public override void MapEntity(CreateTableExpressionBuilder table)
		{
			table
				.WithColumn(nameof(DigiPayRefundRecord.RefundTrackingCode)).AsString().Nullable()
				.WithColumn(nameof(DigiPayRefundRecord.PaymentTrackingCode)).AsString().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.Amount)).AsString().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.Message)).AsString().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.RefundDateTimeOnUtc)).AsDateTime2().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.Customer)).AsString().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.PaymentType)).AsInt32().NotNullable()
				.WithColumn(nameof(DigiPayRefundRecord.Status)).AsBoolean().NotNullable();
		}
	}
}
