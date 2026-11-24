using ZarIran.Plugin.Misc.CustomPdfGenerator.Model;
using Nop.Core.Domain.Customers;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace ZarIran.Plugin.Misc.CustomPdfGenerator.Templates
{
    internal class PackagingSlipTemplate : IDocument
    {
        public CustomShipmentSource ShipmentSource { get; }

        public PackagingSlipTemplate(CustomShipmentSource shipmentSource)
        {
            ShipmentSource = shipmentSource;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {

            container
                   .Page(page =>
                   {
                       page.Size(9.6F, 7.85F, Unit.Centimetre);
                       page.Margin(0.2F, Unit.Centimetre);
                       page.ContentFromRightToLeft();
                       page.DefaultTextStyle(x => x.FontFamily("IRANSansX").FontSize(7));


                       page.Content().Border(0).Table(table =>
                       {
                           table
                           .ColumnsDefinition(columns =>
                           {
                               columns.ConstantColumn(2.01F, Unit.Centimetre);
                               columns.ConstantColumn(2.01F, Unit.Centimetre);
                               columns.ConstantColumn(5.01F, Unit.Centimetre);
                           });


                           #region Row 1

                           if (ShipmentSource.LogoStream != null)
                           {
                               table
                                   .Cell()
                                   .Row(1)
                                   .ColumnSpan(3)
                                   .Element(elm => elm
                                        .BorderBottom(1)
                                        .ShowOnce()
                                        .AlignCenter()
                                        .AlignMiddle())
                                   .Width(1.80F, Unit.Centimetre)
                                   .Height(1, Unit.Centimetre)
                                   .Image(ShipmentSource.LogoStream, ImageScaling.FitArea);

                               table
                                    .Cell()
                                    .Row(1)
                                    .Column(3)
                                    .Element(elm => elm
                                         .BorderBottom(1)
                                         .ShowOnce()
                                         .AlignLeft()
                                         .AlignBottom())
                                    .Text(t => t.GetSpans($"شماره تماس: {ShipmentSource.ZarIranPhoneNumber}"));
                           }
                           else
                           {
                               table
                                 .Cell()
                                 .Row(1)
                                 .Column(3)
                                 .Element(elm => elm
                                      .BorderBottom(1)
                                      .ShowOnce()
                                      .AlignCenter()
                                      .AlignMiddle())
                                 .Width(1.8F, Unit.Centimetre)
                                 .Height(1, Unit.Centimetre);

                               table
                                 .Cell()
                                 .Row(1)
                                 .ColumnSpan(3)
                                 .Element(elm => elm
                                      .BorderBottom(1)
                                      .ShowOnce()
                                      .AlignLeft()
                                      .AlignBottom())
                                 .Text(t => t.GetSpans($"شماره تماس: {ShipmentSource.ZarIranPhoneNumber}"));
                           }

                           #endregion

                           #region Row 2

                           table
                               .Cell()
                               .Row(2)
                               .Column(1)
                               .Element(elm => elm
                                    .Border(1)
                                    .ShowOnce()
                                    .Height(0.8F, Unit.Centimetre)
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Text("شماره مرسوله");

                           table
                            .Cell()
                            .Row(2)
                            .Column(2)
                            .Element(elm => elm
                                 .Border(1)
                                 .ShowOnce()
                                 .Height(0.8F, Unit.Centimetre)
                                 .AlignCenter()
                                 .AlignMiddle())
                            .Text("شماره سفارش");

                           table
                             .Cell()
                             .Row(2)
                             .Column(3)
                             .Element(elm => elm
                                  .Border(1)
                                  .ShowOnce()
                                  .Height(0.8F, Unit.Centimetre)
                                  .AlignCenter()
                                  .AlignMiddle())
                             .Text("نحوه ارسال");

                           #endregion

                           #region Row 3

                           table
                               .Cell()
                               .Row(3)
                               .Column(1)
                               .Element(elm => elm
                                    .Border(1)
                                    .ShowOnce()
                                    .Height(1, Unit.Centimetre)
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Text(t => t.GetSpans(ShipmentSource.ShipmentNumberText));


                           table
                            .Cell()
                            .Row(3)
                            .Column(2)
                            .Element(elm => elm
                                 .Border(1)
                                 .ShowOnce()
                                 .Height(1, Unit.Centimetre)
                                 .AlignCenter()
                                 .AlignMiddle())
                            .Text(t => t.GetSpans(ShipmentSource.OrderNumberText));

                           table
                             .Cell()
                             .Row(3)
                             .Column(3)
                             .Element(elm => elm
                                  .Border(1)
                                  .ContentFromLeftToRight()
                                  .ShowOnce()
                                  .Height(1, Unit.Centimetre))
                             .AlignCenter()
                             .AlignMiddle()
                             .Text(t => t.GetSpans($"{ShipmentSource.Address.ShippingMethod}{ShipmentSource.DeliveryTime}"));


                           #endregion

                           #region Row 4

                           table
                              .Cell()
                              .Row(4)
                              .ColumnSpan(3)
                              .Element(elm => elm
                                   .Border(1)
                                   .ShowOnce()
                                   .Height(3, Unit.Centimetre)
                                   .AlignRight())
                                   .Padding(4)
                              .Text(t =>
                              {
                                  var reciverCustomer = $"نام گیرنده: {ShipmentSource.ReciverCustomerFullName}\r\n";
                                  t.GetSpans(reciverCustomer, fontSize: 8);

                                  if (ShipmentSource.IsDifferentCustomer)
                                  {
                                      var orderCustomer = $"نام سفارش دهنده: {ShipmentSource.OrderCustomerFullName}\r\n";
                                      t.GetSpans(orderCustomer, fontSize: 8);
                                  }

                                  t.GetSpans($"استان: {ShipmentSource.StateProvinceName}   شهر: {ShipmentSource.CityName}  \r\n", fontSize: 8);

                                  var postalCode = ShipmentSource.PostalCode is null ? "" : $"- کد پستی: {ShipmentSource.PostalCode}";
                                  t.GetSpans($"آدرس: {ShipmentSource.Address.Address} {ShipmentSource.Address.Address2} {postalCode}", fontSize: 8);


                              });


                           #endregion

                           #region Row 5

                           table
                               .Cell()
                               .Row(5)
                               .ColumnSpan(3)
                               .Element(elm => elm
                                    .Border(1)
                                    .BorderBottom(0)
                                    .ShowOnce()
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Width(8, Unit.Centimetre)
                               .Height(0.88F, Unit.Centimetre)
                               .Image(ShipmentSource.BracodeStream);

                           #endregion

                           #region Row 6

                           table
                               .Cell()
                               .Row(6)
                               .ColumnSpan(3)
                               .Element(elm => elm
                                    .Border(1)
                                    .BorderTop(0)
                                    .ShowOnce()
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Text(ShipmentSource.BracodeContent)
                               .FontSize(9);

                           #endregion
                       });
                   });
            //FontManager.RegisterFont(File.OpenRead("App_Data/pdf/IRANSansX-Regular.ttf")); // use file name

            //container.Page(page =>
            //{
            //    page.Size(PageSizes.A4);
            //    page.Margin(2, Unit.Centimetre);
            //    page.PageColor(Colors.White);
            //    page.DefaultTextStyle(x => x.FontSize(20));

            //    page.Content()
            //        .PaddingVertical(1, Unit.Centimetre)
            //        .Image(ShipmentSource.BracodeStream);

            //    //page.Footer()
            //    //    .AlignCenter()
            //    //    .Text(x =>
            //    //    {
            //    //        x.Span("Page ");
            //    //        x.CurrentPageNumber();
            //    //    });
            //});

        }
    }

}
