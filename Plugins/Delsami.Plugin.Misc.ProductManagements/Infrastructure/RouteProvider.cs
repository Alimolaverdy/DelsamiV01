using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ZarIran.Plugin.Misc.ProductManagements.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => int.MaxValue;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("ProductIndexExtendedVendor",
                                                    $"/Admin/Product/List",
                                                    new { controller = "CustomProduct", action = "List", area = "Admin" });


            //endpointRouteBuilder.MapControllerRoute(name: "ProductIndexExtendedVendor",
            //                                        pattern: $"Admin/Product/Index",
            //                                        defaults: new { area = "Admin", controller = "CustomProduct", action = "NewIndex" }
            //                                        );

            //endpointRouteBuilder.MapControllerRoute(name: "ProductListExtendedVendor",
            //                             pattern: $"Admin/Product/List",
            //                             defaults: new { area = "Admin", controller = "CustomProduct", action = "NewList" }
            //                             );


            endpointRouteBuilder.MapControllerRoute(name: "ProductListExtendedVendor",
                                         pattern: $"Admin/Product/List",
                                         defaults: new { area = "Admin", controller = "CustomProduct", action = "NewList" }
                                         );

            endpointRouteBuilder.MapControllerRoute(name: "ExportAllProductListAsExcell",
                                         pattern: $"Admin/Product/ExportToExcel",
                                         defaults: new { area = "Admin", controller = "CustomProduct", action = "ExportToExcel" }
                                         );

            endpointRouteBuilder.MapControllerRoute(name: "ExportAllProductListAsXML",
                                         pattern: $"Admin/Product/ExportToXml",
                                         defaults: new { area = "Admin", controller = "CustomProduct", action = "ExportToXml" }
                                         );

            endpointRouteBuilder.MapControllerRoute(name: "PostProductListExtendedVendor",
                                                     pattern: $"Admin/Product/ProductList",
                                                     defaults: new { area = "Admin", controller = "CustomProduct", action = "NewProductList" }
                                                     );


            endpointRouteBuilder.MapControllerRoute(name: "CreateProductExtendedVendor",
                                                    pattern: $"Admin/Product/Create",
                                                    defaults: new { area = "Admin", controller = "CustomProduct", action = "Create" }
                                                    );

            endpointRouteBuilder.MapControllerRoute(name: "EditProductExtendedVendor",
                                        pattern: $"Admin/Product/Edit/{{id?}}",
                                        defaults: new { area = "Admin", controller = "CustomProduct", action = "Edit" }
                                        );

            endpointRouteBuilder.MapControllerRoute(name: "CustomAssociatedProductDelete",
                                        pattern: $"Admin/Product/AssociatedProductDelete",
                                        defaults: new { area = "Admin", controller = "CustomProduct", action = "AssociatedProductDelete" }
                                        );

            endpointRouteBuilder.MapControllerRoute(name: "CustomAssociatedProductAddPopup",
                                        pattern: $"Admin/Product/AssociatedProductAddPopup",
                                        defaults: new { area = "Admin", controller = "CustomProduct", action = "AssociatedProductAddPopup" }
                                        );

            endpointRouteBuilder.MapControllerRoute(name: "CustomProductSpecificationAttributeAdd",
                            pattern: $"Admin/Product/ProductSpecificationAttributeAdd",
                            defaults: new { area = "Admin", controller = "CustomProduct", action = "ProductSpecificationAttributeAdd" }
                            );

            endpointRouteBuilder.MapControllerRoute(name: "CustomProductSpecAttrUpdate",
                            pattern: $"Admin/Product/ProductSpecAttrUpdate",
                            defaults: new { area = "Admin", controller = "CustomProduct", action = "ProductSpecAttrUpdate" }
                            );

            endpointRouteBuilder.MapControllerRoute(name: "CustomProductSpecAttrDelete",
                            pattern: $"Admin/Product/ProductSpecAttrDelete",
                            defaults: new { area = "Admin", controller = "CustomProduct", action = "ProductSpecAttrDelete" }
                            );

            endpointRouteBuilder.MapControllerRoute(
                                    name: "ZarIranProductDetails",
                                    pattern: $"Product/ProductDetails/{{productId?}}",
                                    defaults: new { controller = "ZarIranProduct", action = "ProductDetails" });


            endpointRouteBuilder.MapControllerRoute(name: "ZarIranAddShipment",
                                        pattern: $"Admin/Order/AddShipment/{{id}}",
                                        defaults: new { area = "Admin", controller = "ZarIranOrder", action = "AddShipment" }
                                        );

        }
    }
}
