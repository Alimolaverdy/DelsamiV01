using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Helpers;
using ZarIran.Plugin.Misc.CustomPdfGenerator.Model;
using Nop.Core.Infrastructure;
using Nop.Services.Common.Pdf;
using Nop.Services.Localization;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Templates;

public class OrderTemplate : IDocument
{
	public CustomOrderSource InvoiceSource { get; }
	public int ZarIranVendorId { get; set; }
	private string _shippingName { get; set; }
	public OrderTemplate(CustomOrderSource invoiceSource, int zarIranVendorId)
	{
		InvoiceSource = invoiceSource;
		ZarIranVendorId = zarIranVendorId;
		var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
		_shippingName = localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomPdfGenerator.OrderTemplate.ShippingName").GetAwaiter().GetResult();

	}

	public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

	public void Compose(IDocumentContainer container)
	{
		FontManager.RegisterFont(File.OpenRead("App_Data/pdf/IRANSansX-Regular.ttf")); // use file name

		#region فاکتور فروش

		if (InvoiceSource.ZarIranProducts != null && InvoiceSource.ZarIranProducts.Any() && !InvoiceSource.IsProformaInvoice)
		{
			container
				.Page(page =>
				{
					SetupPage(page);

					page.Content().Border(0).Table(table =>
					{
						table
						.ColumnsDefinition(columns =>
						{
							columns.ConstantColumn(2, Unit.Centimetre);
							columns.ConstantColumn(22, Unit.Centimetre);
							columns.ConstantColumn(4.7F, Unit.Centimetre);
						});


						PrepareTitle(table, "صورت حساب الکترونیکی فروش");

						PrepareSellerInfo(table, InvoiceSource, "فروشنده", false);

						PrepareCustomerInfo(table, InvoiceSource);

						#region Product Rows

						table
							.Cell()
							.Row(4)
							.ColumnSpan(3)
							.Element(elm => elm
										   .Border(1)
										   .ShowOnce()
										   .AlignCenter()
										   .AlignMiddle())
							.Table(innerTable =>
							{
								innerTable.ColumnsDefinition(columns =>
								{
									columns.ConstantColumn(2, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.ConstantColumn(1.3f, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
								});



								uint rowNumber = GenerateProductRows(innerTable, InvoiceSource.ZarIranProducts, false, new List<uint> { 3 });

								#endregion

								PrepareTotalRow(innerTable, rowNumber, 5, InvoiceSource.ZarIranTotals, InvoiceSource.ZarIranProductsQuantity, false);

								PrepareSigniture(innerTable, InvoiceSource.ZarIranTotals.OrderTotal, rowNumber);

							});
					});
				});
		}

		#endregion

		#region پیش فاکتور

		if (InvoiceSource.ZarIranProducts != null && InvoiceSource.ZarIranProducts.Any() && InvoiceSource.IsProformaInvoice)
		{
			container
				.Page(page =>
				{
					SetupPage(page);

					page.Content().Border(0).Table(table =>
					{
						table
						.ColumnsDefinition(columns =>
						{
							columns.ConstantColumn(2, Unit.Centimetre);
							columns.ConstantColumn(22, Unit.Centimetre);
							columns.ConstantColumn(4.7F, Unit.Centimetre);
						});


						PrepareTitle(table, "پیش‌فاکتور فروش");

						PrepareSellerInfo(table, InvoiceSource, "فروشنده", true);

						PrepareCustomerInfo(table, InvoiceSource);

						#region Product Rows

						table
							.Cell()
							.Row(4)
							.ColumnSpan(3)
							.Element(elm => elm
										   .Border(1)
										   .ShowOnce()
										   .AlignCenter()
										   .AlignMiddle())
							.Table(innerTable =>
							{
								innerTable.ColumnsDefinition(columns =>
								{
									columns.ConstantColumn(2, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.ConstantColumn(1.3f, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
								});

								uint rowNumber = GenerateProductRows(innerTable, InvoiceSource.ZarIranProducts, false, new List<uint> { 5 });

								#endregion

								PrepareTotalRow(innerTable, rowNumber, 7, InvoiceSource.ZarIranTotals, InvoiceSource.ZarIranProductsQuantity, true);

								PrepareSigniture(innerTable, InvoiceSource.ZarIranTotals.OrderTotal, rowNumber);

							});
					});
				});
		}

		#endregion

		#region فاکتور فروش حق العمل کار

		if (InvoiceSource.VendorsProducts != null && InvoiceSource.VendorsProducts.Any() && !InvoiceSource.IsProformaInvoice)
		{
			container
				.Page(page =>
		 {
			 SetupPage(page);

			 page.Content().Border(0).Table(table =>
			 {
				 table
				 .ColumnsDefinition(columns =>
				 {
					 columns.ConstantColumn(2, Unit.Centimetre);
					 columns.ConstantColumn(22, Unit.Centimetre);
					 columns.ConstantColumn(4.7F, Unit.Centimetre);
				 });


				 PrepareTitle(table, "صورت حساب الکترونیکی فروش نیابتی");

				 PrepareSellerInfo(table, InvoiceSource, "حق‌العمل کار", false);

				 PrepareCustomerInfo(table, InvoiceSource);

				 #region Product Rows

				 table
					 .Cell()
					 .Row(4)
					 .ColumnSpan(3)
					 .Element(elm => elm
									.Border(1)
									.ShowOnce()
									.AlignCenter()
									.AlignMiddle())
					 .Table(innerTable =>
					 {
						 innerTable.ColumnsDefinition(columns =>
						 {
							 columns.ConstantColumn(2, Unit.Centimetre);
							 columns.RelativeColumn();
							 columns.RelativeColumn();
							 columns.RelativeColumn();
							 columns.RelativeColumn();
							 columns.ConstantColumn(1.3f, Unit.Centimetre);
							 columns.RelativeColumn();
							 columns.RelativeColumn();
							 columns.RelativeColumn();
							 columns.RelativeColumn();
						 });

						 uint rowNumber = GenerateProductRows(innerTable, InvoiceSource.VendorsProducts, true, new List<uint> { 2 });

						 #endregion

						 PrepareTotalRow(innerTable, rowNumber, 5, InvoiceSource.VendorsTotals, InvoiceSource.VendorsProductsQuantity, false);

						 PrepareSigniture(innerTable, InvoiceSource.VendorsTotals.OrderTotal, rowNumber);

					 });
			 });
		 });
		}

		#endregion

		#region پیش فاکتور حق العمل کار

		if (InvoiceSource.VendorsProducts != null && InvoiceSource.VendorsProducts.Any() && InvoiceSource.IsProformaInvoice)
		{
			container
				.Page(page =>
	{
		SetupPage(page);

		page.Content().Border(0).Table(table =>
		{
			table
			.ColumnsDefinition(columns =>
			{
				columns.ConstantColumn(2, Unit.Centimetre);
				columns.ConstantColumn(22, Unit.Centimetre);
				columns.ConstantColumn(4.7F, Unit.Centimetre);
			});

			PrepareTitle(table, "پیش‌فاکتور فروش نیابتی");

			PrepareSellerInfo(table, InvoiceSource, "حق‌العمل کار", true);

			PrepareCustomerInfo(table, InvoiceSource);

			#region Product Rows

			table
				.Cell()
				.Row(4)
				.ColumnSpan(3)
				.Element(elm => elm
							   .Border(1)
							   .ShowOnce()
							   .AlignCenter()
							   .AlignMiddle())
				.Table(innerTable =>
				{
					innerTable.ColumnsDefinition(columns =>
					{
						columns.ConstantColumn(2, Unit.Centimetre);
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.ConstantColumn(1.3f, Unit.Centimetre);
						columns.RelativeColumn();
						columns.RelativeColumn();
						columns.RelativeColumn();
					});

					uint rowNumber = GenerateProductRows(innerTable, InvoiceSource.VendorsProducts, true, new List<uint> { 3, 2 });

					#endregion

					PrepareTotalRow(innerTable, rowNumber, 7, InvoiceSource.VendorsTotals, InvoiceSource.VendorsProductsQuantity, true);

					PrepareSigniture(innerTable, InvoiceSource.VendorsTotals.OrderTotal, rowNumber);

				});
		});
	});
		}

		#endregion

		#region فاکتور ارسال

		if (!string.IsNullOrEmpty(InvoiceSource.ShippingPrice))
		{
			container
				.Page(page =>
				{
					SetupPage(page);

					page.Content().Border(0).Table(table =>
					{
						table
						.ColumnsDefinition(columns =>
						{
							columns.ConstantColumn(2, Unit.Centimetre);
							columns.ConstantColumn(22, Unit.Centimetre);
							columns.ConstantColumn(4.7F, Unit.Centimetre);
						});

						PrepareTitle(table, InvoiceSource.IsProformaInvoice ? "پیش‌فاکتور فروش" : "صورت حساب الکترونیکی فروش");

						PrepareSellerInfo(table, InvoiceSource, "فروشنده", InvoiceSource.IsProformaInvoice);

						PrepareCustomerInfo(table, InvoiceSource);


						#region Product Rows

						table
							.Cell()
							.Row(4)
							.ColumnSpan(3)
							.Element(elm => elm
										   .Border(1)
										   .ShowOnce()
										   .AlignCenter()
										   .AlignMiddle())
							.Table(innerTable =>
							{
								innerTable.ColumnsDefinition(columns =>
								{
									columns.ConstantColumn(2, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.ConstantColumn(1.3f, Unit.Centimetre);
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
									columns.RelativeColumn();
								});

								innerTable.Cell().Row(1).Column(1).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("ردیف");
								});

								innerTable.Cell().Row(1).Column(2).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("شناسه کالا یا خدمت");
								});

								innerTable.Cell().Row(1).Column(3).ColumnSpan(3).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("شرح کالا یا خدمت");
								});

								innerTable.Cell().Row(1).Column(6).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("تعداد");
								});

								innerTable.Cell().Row(1).Column(7).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("مبلغ واحد");
								});

								innerTable.Cell().Row(1).Column(8).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("تخفیف");
								});

								innerTable.Cell().Row(1).Column(9).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("مالیات");
								});

								innerTable.Cell().Row(1).Column(10).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
								{
									t.Span("مبلغ کل");
								});

								innerTable.Cell().Row(2).Column(1).Height(30).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("1");
								});

								innerTable.Cell().Row(2).Column(2).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									//t.GetSpans("شماره کالا sku");
								});

								innerTable.Cell().Row(2).Column(3).ColumnSpan(3).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans(_shippingName);
								});

								innerTable.Cell().Row(2).Column(6).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("1");
								});

								innerTable.Cell().Row(2).Column(7).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans(InvoiceSource.ShippingPrice, true);
								});

								innerTable.Cell().Row(2).Column(8).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("", true);
								});

								innerTable.Cell().Row(2).Column(9).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("", true);
								});

								innerTable.Cell().Row(2).Column(10).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans(InvoiceSource.ShippingPrice, true);
								});

								#endregion

								#region TotalPrice row

								innerTable.Cell().Row(3).ColumnSpan(5).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("جمع کل");
								});

								innerTable.Cell().Row(3).Column(6).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("1");
								});

								innerTable.Cell().Row(3).Column(7).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans(InvoiceSource.ShippingPrice, true);
								});

								innerTable.Cell().Row(3).Column(8).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("", true);
								});

								innerTable.Cell().Row(3).Column(9).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans("", true);
								});

								innerTable.Cell().Row(3).Column(10).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
								{
									t.GetSpans(InvoiceSource.ShippingPrice, true);
								});

								#endregion

								#region End row

								PrepareSigniture(innerTable, InvoiceSource.ShippingPrice, 3);

								#endregion
							});
					});
				});
		}

		#endregion

		#region Utilities

		void PrepareTitle(TableDescriptor table, string title)
		{
			table
						  .Cell()
						  .Row(1)
						  .ColumnSpan(3)
						  .Element(elm => elm
							   .Border(0)
							   .ShowOnce()
							   .Height(1, Unit.Centimetre)
							   .AlignCenter()
							   .AlignMiddle())
						  .Text(title)
						  .FontSize(12);
		}

		void PrepareSellerInfo(TableDescriptor table, CustomOrderSource invoiceSource, string title, bool isProformaInvoice)
		{
			table
				 .Cell()
				 .Row(2)
				 .Column(1)
				 .Element(elm => elm.Border(1)
									.Background(Colors.Grey.Lighten3)
									.ShowOnce()
									.Height(3, Unit.Centimetre)
									.AlignCenter()
									.AlignMiddle()
									.RotateLeft())
				 .Text(title);

			table
				.Cell()
				.Row(2)
				.Column(2)
				.Element(elm => elm.Border(1)
							   .ShowOnce()
							   .Height(3, Unit.Centimetre)
							   .AlignCenter()
							   .AlignMiddle())
				.Table(innerTable =>
				{

					innerTable.ColumnsDefinition(columns =>
					{
						columns.ConstantColumn(5, Unit.Centimetre);
						columns.ConstantColumn(4.5F, Unit.Centimetre);
						columns.ConstantColumn(4.5F, Unit.Centimetre);
						columns.ConstantColumn(6F, Unit.Centimetre);
					});

					innerTable.Cell().Row(1).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyNameTitle}: {invoiceSource.CompanyName}"));

					innerTable.Cell().Row(1).Column(2).ColumnSpan(2).Padding(2, Unit.Millimetre).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyNationalCodeTitle}: {invoiceSource.CompanyNationalCode}"));

					innerTable.Cell().Row(1).Column(4).Padding(2, Unit.Millimetre).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyRegisterNumberTitle}: {invoiceSource.CompanyRegisterNumber}"));

					innerTable.Cell().Row(2).ColumnSpan(2).Padding(2, Unit.Millimetre).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyAddressTitle}: {invoiceSource.CompanyAddress}"));

					innerTable.Cell().Row(2).Column(3).Padding(2, Unit.Millimetre).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyPostalCodeTitle}: {invoiceSource.CompanyPostalCode}"));

					innerTable.Cell().Row(2).Column(4).Element(Block).Text(t => t.GetSpans($"{invoiceSource.CompanyPhoneNumberTitle}: {invoiceSource.CompanyPhoneNumber}"));

				});

			table
				.Cell()
				.Row(2)
				.Column(3)
				.Element(elm => elm
							   .Border(1)
							   .ShowOnce()
							   .AlignCenter()
							   .AlignMiddle())
				.Table(innerTable =>
				{
					innerTable.ColumnsDefinition(columns =>
					{
						columns.ConstantColumn(4.5F, Unit.Centimetre);
					});
					if (!isProformaInvoice)
					{

						innerTable.Cell().Padding(2, Unit.Millimetre).Element(Block).Text(t =>
						{
							t.GetSpans("شماره فاکتور: ");
							t.GetSpans(invoiceSource.InvoiceId);
						});
					}

					innerTable.Cell().Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					{
						t.Span("تاریخ: ");
						t.Span(TextDescriptorExtension.ConvertDigitsToPersian(DateTimeHelper.GetPersianStrFromDateTime(invoiceSource.OrderDateUser))).DirectionFromLeftToRight();
					});

					innerTable.Cell().Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					{
						t.Span("پیگیری: ");
						t.GetSpans(invoiceSource.OrderNumberText);
					});

				});

		}

		void PrepareCustomerInfo(TableDescriptor table, CustomOrderSource invoiceSource)
		{

			table.Cell()
				 .Row(3)
				 .Column(1)
				 .Element(elm => elm.Border(1)
									.Background(Colors.Grey.Lighten3)
									.ShowOnce()
									.Height(3, Unit.Centimetre)
									.AlignCenter()
									.AlignMiddle()
									.RotateLeft())
				 .Text("خریدار");


			table.Cell()
				 .Row(3)
				 .Column(2)
				 .Element(elm => elm.Border(1)
								.ShowOnce()
								.Height(3, Unit.Centimetre)
								.AlignCenter()
								.AlignMiddle())
				 .Table(innerTable =>
				 {
					 innerTable.ColumnsDefinition(columns =>
					 {
                        columns.ConstantColumn(7, Unit.Centimetre);
                        columns.ConstantColumn(6.5F, Unit.Centimetre);
                        columns.ConstantColumn(6.5F, Unit.Centimetre);
					 });

					 innerTable.Cell().Row(1).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					 {
						 t.Span("خریدار: ");
						 t.GetSpans(invoiceSource.ShippingAddress.Name);
					 });

					 innerTable.Cell().Row(1).Column(2).Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					 {
						 t.Span("شماره اقتصادی/کد ملی: ");
						 t.Span(invoiceSource.CustomerNationalCode).DirectionFromLeftToRight();
					 });

					 innerTable.Cell().Row(1).Column(3).Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					 {
						 t.Span("شناسه ملی: ");
						 t.Span(invoiceSource.CustomerNationalCode).DirectionFromLeftToRight();
					 });

					 innerTable.Cell().Row(2).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text(t =>
					 {
						 t.Span("شماره ثبت: ");
						 t.Span("").DirectionFromLeftToRight();
					 });

					 innerTable.Cell().Row(2).Column(2).Element(Block).Text(t => t.GetSpans($"شماره تماس: {invoiceSource.ShippingAddress.Phone}"));

					 innerTable.Cell().Row(2).Column(3).Element(Block).Text(t => t.GetSpans($"کد پستی: {invoiceSource.CustomerZipPostalCode}"));

					 //innerTable.Cell().Row(3).ColumnSpan(3).Padding(2, Unit.Millimetre).AlignRight().ShowOnce().Text(t =>
					 //{
						// t.GetSpans($"آدرس: {invoiceSource.ShippingAddress.Address}");
					 //});
					 innerTable.Cell().Row(3).ColumnSpan(3).Element(Block).PaddingRight(2, Unit.Millimetre).Text(t => t.GetSpans($"آدرس: {invoiceSource.ShippingAddress.Address}"));


				 });


			table.Cell()
				 .Row(3)
				 .Column(3)
				 .Element(elm => elm
								.Border(1)
								.ShowOnce()
								.Height(3, Unit.Centimetre)
								.AlignCenter()
								.AlignMiddle())
				 .Table(innerTable =>
				 {
					 innerTable.ColumnsDefinition(columns =>
					 {
						 columns.ConstantColumn(4.5F, Unit.Centimetre);
					 });

					 innerTable.Cell().Row(1).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text("شماره مالیاتی: ");
					 innerTable.Cell().Row(2).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text("سریال حافظه مالیاتی: ");
					 innerTable.Cell().Row(3).Column(1).Padding(2, Unit.Millimetre).Element(Block).Text("سریال پایانه فروشگاهی: ");

				 });
		}

		void SetupPage(PageDescriptor page)
		{
			page.Size(PageSizes.A4.Landscape());
			page.Margin(0.5F, Unit.Centimetre);
			page.ContentFromRightToLeft();
			page.DefaultTextStyle(x => x.FontFamily("IRANSansX").FontSize(8));
		}

		void PrepareTotalRow(TableDescriptor innerTable, uint rowNumber, uint startColNumber, CustomInvoiceTotals invoiceTotals, int quantity, bool isProformaInvoice)
		{
			innerTable.Cell().Row(rowNumber).ColumnSpan(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
			{
				t.GetSpans("جمع کل");
			});

			startColNumber++;
			innerTable.Cell().Row(rowNumber).Column(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
			{
				t.GetSpans(quantity.ToString());

			});

			startColNumber++;
			innerTable.Cell().Row(rowNumber).Column(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
			{
				t.GetSpans(invoiceTotals.SubTotal, true);

			});
			if (!isProformaInvoice)
			{

				startColNumber++;
				innerTable.Cell().Row(rowNumber).Column(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(invoiceTotals.Discount, true);

				});

				startColNumber++;
				innerTable.Cell().Row(rowNumber).Column(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(invoiceTotals.Tax, true);

				});

			}
			startColNumber++;
			innerTable.Cell().Row(rowNumber).Column(startColNumber).Height(0.7f, Unit.Centimetre).Border(1).Background(Colors.Grey.Lighten3).Element(ProductItemBlock).Text(t =>
			{
				t.GetSpans(invoiceTotals.OrderTotalWithoutShipping, true);
			});
		}

		void PrepareSigniture(TableDescriptor innerTable, string totalPrice, uint rowNumber)
		{
			innerTable.Cell().Row(rowNumber + 1).ColumnSpan(10).Element(ProductItemBlock).MinHeight(5).Text(t =>
			{
			});

			innerTable.Cell().Row(rowNumber + 2).Column(1).ColumnSpan(2).Element(ProductItemBlock).Text(t =>
			{
				//t.GetSpans($"هزینه ارسال: {InvoiceSource.ShippingPrice}", true);
			});

			innerTable.Cell().Row(rowNumber + 2).Column(3).ColumnSpan(2).Element(ProductItemBlock).Text(t =>
			{
				t.DefaultTextStyle(x => x.Bold());
				t.GetSpans($"مبلغ پرداختی: {totalPrice}", true, 12);
			});

			innerTable.Cell().Row(rowNumber + 2).Column(5).ColumnSpan(2).Element(ProductItemBlock).Image("wwwroot/Images/ViraSign.png");

			innerTable.Cell().Row(rowNumber + 2).Column(7).ColumnSpan(4).Element(ProductItemBlock).Text(t =>
			{
			});
		}

		uint GenerateProductRows(TableDescriptor innerTable, List<CustomProductItem> products, bool isVendor, List<uint> colSpan)
		{
			uint i = 1;
			uint j = 1;
			var secondColSpan = colSpan.Count == 2 ? colSpan[1] : 1;

			innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("ردیف");
			});

			j++;
			innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("شناسه کالا یا خدمت");
			});

			j++;
			innerTable.Cell().Row(1).Column(j).ColumnSpan(colSpan[0]).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("شرح کالا یا خدمت");
			});

			j += colSpan[0];
			if (isVendor)
			{
				innerTable.Cell().Row(1).Column(j).ColumnSpan(secondColSpan).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
				{
					t.Span("آمر");
				});

				j += secondColSpan;
			}

			innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("تعداد");
			});

			j++;
			innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("مبلغ واحد");
			});

			if (!InvoiceSource.IsProformaInvoice)
			{
				j++;
				innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
				 {
					 t.Span("تخفیف");
				 });

				j++;
				innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
				{
					t.Span("مالیات");
				});
			}

			j++;
			innerTable.Cell().Row(1).Column(j).Border(1).Background(Colors.Grey.Lighten3).Element(HeaderBlock).Text(t =>
			{
				t.Span("مبلغ کل");
			});

			foreach (var product in products)
			{
				j = 1;
				i++;

				innerTable.Cell().Row(i).Column(j).Height(30).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans((i - 1).ToString());
				});

				j++;
				innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(product.Sku);
				});

				j++;
				innerTable.Cell().Row(i).Column(j).ColumnSpan(colSpan[0]).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(product.Name);
				});

				j += colSpan[0];
				if (isVendor)
				{
					innerTable.Cell().Row(i).Column(j).ColumnSpan(secondColSpan).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
							 {
								 t.GetSpans(product.VendorName);
							 });

					j += secondColSpan;

				}

				innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(product.Quantity);
				});

				j++;
				innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(product.UnitPriceIncludeTax, true);
				});
				if (!InvoiceSource.IsProformaInvoice)
				{
					j++;
					innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
					{
						t.GetSpans(product.DiscountAmountExcludeTax, true);
					});

					j++;
					innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
					{
						t.GetSpans(product.Tax, true);
					});
				}

				j++;
				innerTable.Cell().Row(i).Column(j).Border(0.1F, Unit.Millimetre).Element(ProductItemBlock).Text(t =>
				{
					t.GetSpans(product.PriceIncludeTax, true);
				});

			}

			i++;
			return i;
		}
		static IContainer Block(IContainer container)
		{
			return container.ShowOnce()
							.AlignRight()
							.AlignMiddle();

		}

		static IContainer ProductItemBlock(IContainer container)
		{
			return container.ShowOnce()
							.AlignCenter()
							.AlignMiddle();

		}

		static IContainer HeaderBlock(IContainer container)
		{
			return container
									   .ShowOnce()
									   .AlignCenter()
									   .AlignMiddle()
									   .Padding(3, Unit.Millimetre);

		}

		#endregion
	}
}
