using NobParts.Plugin.Misc.CustomPdfGenerator.Model;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace NobParts.Plugin.Misc.CustomPdfGenerator.Templates
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
                       page.Size(PageSizes.A5.Landscape());
                       page.Margin(0.5F, Unit.Centimetre);
                       page.ContentFromRightToLeft();
                       page.DefaultTextStyle(x => x.FontFamily("IRANSansX").FontSize(8));


                       page.Content().Border(0).Table(table =>
                       {
                           table
                           .ColumnsDefinition(columns =>
                           {
                               columns.ConstantColumn(6.6F, Unit.Centimetre);
                               columns.ConstantColumn(6.6F, Unit.Centimetre);
                               columns.ConstantColumn(6.6F, Unit.Centimetre);
                           });

                           #region Row 1

                           if (ShipmentSource.LogoStream != null)
                           {
                               table
                                   .Cell()
                                   .Row(1)
                                   .ColumnSpan(3)
                                   .Element(elm => elm
                                        .Border(1)
                                        .ShowOnce()
                                        .AlignCenter()
                                        .AlignMiddle())
                                   .Width(4, Unit.Centimetre)
                                   .Height(1, Unit.Centimetre)
                                   .Image(ShipmentSource.LogoStream, ImageScaling.FitArea);
                           }
                           else
                           {
                               table
                                   .Cell()
                                   .Row(1)
                                   .ColumnSpan(3)
                                   .Element(elm => elm
                                        .Border(1)
                                        .ShowOnce()
                                        .AlignCenter()
                                        .AlignMiddle())
                                   .Width(4, Unit.Centimetre)
                                   .Height(1, Unit.Centimetre);
                           }

                           #endregion

                           #region Row 2

                           table
                                .Cell()
                                .Row(2)
                                .ColumnSpan(3)
                                .Element(elm => elm
                                     .Border(1)
                                     .ShowOnce()
                                     .AlignCenter()
                                     .AlignMiddle())
                                .Width(4, Unit.Centimetre)
                                .Height(1, Unit.Centimetre)
                                .Text(t => t.GetSpans($"شماره تماس: {ShipmentSource.NobPartsPhoneNumber}"));

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
                               .Text("شماره محموله")
                               .FontSize(10);

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
                            .Text("شماره سفارش")
                            .FontSize(10);

                           table
                             .Cell()
                             .Row(3)
                             .Column(3)
                             .Element(elm => elm
                                  .Border(1)
                                  .ShowOnce()
                                  .Height(1, Unit.Centimetre)
                                  .AlignCenter()
                                  .AlignMiddle())
                             .Text("نام گیرنده")
                             .FontSize(10);

                           #endregion

                           #region Row 4

                           table
                               .Cell()
                               .Row(4)
                               .Column(1)
                               .Element(elm => elm
                                    .Border(1)
                                    .ShowOnce()
                                    .Height(1.5F, Unit.Centimetre)
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Text(t => t.GetSpans(ShipmentSource.ShipmentNumberText));


                           table
                            .Cell()
                            .Row(4)
                            .Column(2)
                            .Element(elm => elm
                                 .Border(1)
                                 .ShowOnce()
                                 .Height(1.5F, Unit.Centimetre)
                                 .AlignCenter()
                                 .AlignMiddle())
                            .Text(t => t.GetSpans(ShipmentSource.OrderNumberText));

                           table
                             .Cell()
                             .Row(4)
                             .Column(3)
                             .Element(elm => elm
                                  .Border(1)
                                  .ShowOnce()
                                  .Height(1.5F, Unit.Centimetre)
                                  .AlignCenter()
                                  .AlignMiddle())
                             .Text(ShipmentSource.CustomerFullName);

                           #endregion


                           #region Row 5

                           table
                              .Cell()
                              .Row(5)
                              .ColumnSpan(3)
                              .Element(elm => elm
                                   .Border(1)
                                   .ShowOnce()
                                   .Height(2, Unit.Centimetre)
                                   .AlignRight())
                              .Text(t => t.GetSpans($"آدرس: {ShipmentSource.Address.Address}"));

                           #endregion

                           #region Row 6

                           table
                              .Cell()
                              .Row(6)
                              .ColumnSpan(3)
                              .Element(elm => elm
                                   .Border(1)
                                   .ShowOnce()
                                   .Height(1.5F, Unit.Centimetre)
                              .AlignRight())
                              .Text(t => t.GetSpans($"نحوه ارسال: {ShipmentSource.Address.ShippingMethod}"));

                           #endregion

                           #region Row 7


                           table
                               .Cell()
                               .Row(7)
                               .ColumnSpan(3)
                               .Element(elm => elm
                                    .Border(1)
                                    .ShowOnce()
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Width(12, Unit.Centimetre)
                               .Height(2, Unit.Centimetre)
                               .Image(ShipmentSource.BracodeStream);

                           #endregion

                           #region Row 8


                           table
                               .Cell()
                               .Row(8)
                               .ColumnSpan(3)
                               .Element(elm => elm
                                    .Border(1)
                                    .ShowOnce()
                                    .AlignCenter()
                                    .AlignMiddle())
                               .Text(t => t.GetSpans(ShipmentSource.BracodeContent));

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
