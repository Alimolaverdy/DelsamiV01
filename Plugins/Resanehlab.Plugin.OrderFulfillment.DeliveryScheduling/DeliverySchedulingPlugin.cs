using LinqToDB.DataProvider;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Components;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Components;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Helpers;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ZarIran.Plugin.OrderFulfillment.DeliveryScheduling
{
    public class DeliverySchedulingPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ILocalizationService _localizationService;
        private readonly INopDataProvider _dataProvider;

        public bool HideInWidgetList => false;

        #endregion

        #region Ctor

        public DeliverySchedulingPlugin(ISettingService settingService,
            WidgetSettings widgetSettings,
            ILocalizationService localizationService,
            IRLSqlProvider sqlProvider,
            INopDataProvider dataProvider)
        {
            _settingService = settingService;
            _widgetSettings = widgetSettings;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl() => "/Admin/Plugin/DeliveryScheduling/Configure";

        /// <summary>
        /// Install plugin
        /// </summary>
        public async override Task InstallAsync()
        {
            #region Stored procedures

            if (!await StoredProcedureExistsAsync(Constants.LoadAvailableDeliveryDateTimesStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.LoadAvailableDeliveryDateTimesSqlBody);

            if (!await StoredProcedureExistsAsync(Constants.GetPlacedOrdersCountStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.GetPlacedOrdersCountMsSqlBody);

            if (!await StoredProcedureExistsAsync(Constants.GetOrderShippingMethodCapacitiesStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.GetOrderShippingMethodCapacitiesMsSqlBody);

            #endregion

            #region Resources 

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling"] = "Delivery scheduling",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"] = "Settings",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"] = "Manage delivery time ranges",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"] = "Manage delivery date time capacities",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"] = "Manage exception dates",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"] = "Select",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.SameDateError"] = "Exception with the same date already exists",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time"] = "Time",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time.Hint"] = "Time.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DisplayOrder"] = "Display order",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DisplayOrder.Hint"] = "Display order.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.EnablePlugin"] = "Enabled",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.EnablePlugin.Hint"] = "If checked, the delivery scheduling plugin will be enabled.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.NumberOfDisplayDays"] = "Number of display days",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.NumberOfDisplayDays.Hint"] = "This filed specifies the number of available days that customers are able to choose from.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartDisplayDaysOffset"] = "Start display days offset",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartDisplayDaysOffset.Hint"] = "This filed specifies the interval between the customer's order day and the first day which customers can select. For example, if the value of this field is “3”, customers that are submitting an order on 2021/01/01, the first available delivery day for them will be 2021/01/04.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title"] = "Title",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title.Hint"] = "Exception date title.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc"] = "Date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc.Hint"] = "Exception date.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.SelectedShippingMethod"] = "Shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.SelectedShippingMethod.Hint"] = "Select a shipping method for defining capacities.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc.Required"] = "Date field is required",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.ConfirmButton"] = "Confirm existing delivery time ranges",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.AddButton"] = "Add delivery time range",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteButton"] = "Delete delivery time ranges",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.AddNew"] = "Add new delivery time range",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.BackToList"] = "Back to delivery time ranges list",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.EditDetails"] = "Edit delivery time range details",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.List"] = "Delivery time ranges list",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Info"] = "Info",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.MustBeSelectedAtLeastOne"] = "At least one delivery date time must be selected",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.NoLongerAvailable"] = "Selected date time is no longer available",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate"] = "Delivery date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate.Hint"] = "Order delivery date.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime"] = "Delivery time",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime.Hint"] = "Order delivery time.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName"] = "Shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName.Hint"] = "Order delivery shipping method.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Fields.ShippingMethod"] = "Shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Fields.ShippingMethod.Hint"] = "Select a shipping method for defining capacities.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.DaysOfWeek"] = "Days of week",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Saturday"] = "Saturday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Sunday"] = "Sunday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Monday"] = "Monday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Tuesday"] = "Tuesday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Wednesday"] = "Wednesday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Thursday"] = "Thursday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Friday"] = "Friday",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.HasNotConfimedTimeRange"] = "There are no confirmed time ranges",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"] = "Settings",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable"] = "Time table",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Info"] = "Info",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.TimeTable"] = "Time table",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.TimeTable.SaveBeforeEdit"] = "You need to save the exception date before you can add time table for this page.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.AddButton"] = "Add exception date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.AddNew"] = "Add new exception date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.BackToList"] = "Back to exception dates list",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.EditDetails"] = "Edit exception date details",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.List"] = "Exception dates list",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeRange.Fields.Capacity.Required"] = "Capacity field is required",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"] = "Action completed successfully",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable"] = "Current available date times",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable.AvailableCapacities"] = "Available capacities",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTableText"] = "Click here to see current available date times states",
                ["ZarIran.Plugin.Orderfulfillment.Deliveryscheduling.Availabledeliverytimerange.NotconfirmedText"] = "Not Confirmed",
                ["ZarIran.Plugin.Orderfulfillment.Deliveryscheduling.Availabledeliverytimerange.ConfirmedText"] = "Confirmed",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteOnce"] = "Delete",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmText"] = "Are you sure you want to delete?",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.BookedUp"] = "Booked up",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.NoDefinedDeliveryDateTimes"] = "There is no defined delivery date times for this shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteModal.Header"] = "Select a expiration date for current time ranges",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.LastOrderDateInTimeRangesText"] = "Last order creation date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.Date"] = "Expiration date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.Submit"] = "Submit",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"] = "Order delivery date times list",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.NoData"] = "No data",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.FromDate"] = "From date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.FromDate.Hint"] = "From date for the search.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.ToDate"] = "To date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.ToDate.Hint"] = "To date for the search.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.SelectedShippingMethod"] = "Shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.SelectedShippingMethod.Hint"] = "Search by a shipping method.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.Store"] = "Store",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.Store.Hint"] = "Search by store.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.OrderId"] = "Order details link",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.OrderDetails"] = "Order details",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.DeliveryDateTime"] = "Delivery date time",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.ShippingMethodName"] = "Shipping method",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.OverloadingOrder"] = "The number of placed orders in these date times have exceeded from the current capacity: {0}",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ErrorNotification"] = "Error happened!",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation"] = "Are you sure you want to delete delivery time ranges?",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Capacities"] = "Delivery date time capacities",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Hint"] = "<li><strong>Define exceptions for special days.</strong></li><li><strong>Define appropriate capacity for every defined time range of the day in Time table tab.</strong></li>",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities.Hint"] = "<li><strong>Select a shipping method.</strong></li><li><strong>Define capacities for pre-defined time ranges of every week day (per shipping method).</strong></li>",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable.Hint"] = "<strong>This table has been provided to generate an exports, according to real dates, time ranges and remaining capacities of upcoming days, based on primary capacities and considering the effect of exception dates and calculation of the number of submitted orders for defined capacities.</strong>",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.StoreName"] = "Store",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Unknown"] = "Unknown",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryDate"] = "Delivery date",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryTime"] = "Delivery time",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery"] = "Print PDF orders delivery",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery.All"] = "Print PDF orders delivery (all found)",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery.Selected"] = "Print PDF orders delivery (selected)",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.IsConfirmed"] = "Confirmed",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DisabledShippingMethodIds"] = "Excluded shipping methods",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DisabledShippingMethodIds.Hint"] = "Selected shipping methods will be excluded from delivery scheduling process.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.HideUnselectableDays"] = "Hide unselectable days",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.HideUnselectableDays.Hint"] = "Hide days that have no available delivery time for selecting.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Orders.DeliveryScheduling"] = "Delivery scheduling",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.ShippingMethods.NoShippingMethodsAvailable"] = "No shipping methods available.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DeliveryDateTimeFormatString"] = "Delivery date time format string",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DeliveryDateTimeFormatString.Hint"] = "A standard date and time format string for defining the text representation of a date and time value. The delivery date time will be displayed in this format in the store. You can check this 'https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings' link for more details.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.FormatDeliveryDateTimesBasedOnCustomerLanguage"] = "Format delivery date times based on customer language",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.FormatDeliveryDateTimesBasedOnCustomerLanguage.Hint"] = "If checked, the delivery date time values in the store will be formatted based on current customer language.",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.Category"] = "Category",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.StartDisplayDaysOffset"] = "Start display days offset",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.OverriddenStartDisplayDaysOffsetsByCategories"] = "Overridden start display days offsets by categories",
                ["ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.OverriddenStartDisplayDaysOffsetsByCategories.Hint"] = "You can specify the start display days offset value for every category. These offsets will override the base offset setting in some cases. For example, if a customer has a product from category ‘a’ in his cart and this category has offset ‘x’, the offset will be ‘x’. If multiple categories exist, the largest offset will be considered. If products in the cart have no categories or any of the existing categories have no defined offset, the base offset setting will be considered."
            });

            #endregion

            #region Settings

            DeliverySchedulingSetting deliverySchedulingSetting = new DeliverySchedulingSetting()
            {
                EnablePlugin = true,
                NumberOfDisplayDays = 7,
                DeliveryDateTimeFormatString = Constants.DefaultDeliveryDateTimeFormatString
            };
            await _settingService.SaveSettingAsync(deliverySchedulingSetting);

            _widgetSettings.ActiveWidgetSystemNames.Add(Constants.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            var paymentSettings = await _settingService.LoadSettingAsync<PaymentSettings>();

            paymentSettings.BypassPaymentMethodSelectionIfOnlyOne = false;
            await _settingService.SaveSettingAsync<PaymentSettings>(paymentSettings);

            #endregion

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public async override Task UninstallAsync()
        {
            #region Resources 

            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling");

            #endregion

            #region Settings

            await _settingService.DeleteSettingAsync<DeliverySchedulingSetting>();

            _widgetSettings.ActiveWidgetSystemNames.Remove(Constants.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            #endregion

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            var widgetList = new List<string>();

            widgetList.AddRange(new List<string>
                {
                    PublicWidgetZones.CheckoutShippingMethodBottom,
                    PublicWidgetZones.OpCheckoutShippingMethodBottom,
                    PublicWidgetZones.OrderDetailsPageOverview,
                    PublicWidgetZones.OrderSummaryContentBefore,
                    AdminWidgetZones.OrderDetailsBlock
                });

            return Task.FromResult<IList<string>>(widgetList);
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.OrderDetailsPageOverview)
                return typeof(LoadOrderDetailsDeliveryDateTime);
            else if (widgetZone == PublicWidgetZones.OrderSummaryContentBefore)
                return typeof(LoadOrderSummaryDeliveryDateTime);
            else if (widgetZone == AdminWidgetZones.OrderDetailsBlock)
                return typeof(OrderDeliveryDateTime);
            else
                return typeof(LoadShippingMethodContent);
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {

            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");

            var pluginName = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Name");
            var pluginSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.SystemName");

            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.SystemName");

            //var receiverPhoneNumberName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.OrderSMSManagement.ReceiverPhoneNumber.List");
            //var receiverPhoneNumberSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.OrderSMSManagement.ReceiverPhoneNumber.List.SystemName");

            var zarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (zarIranSiteMapNode == null)
                zarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var childNodes = new List<SiteMapNode>
            {
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"),
                    ControllerName="DeliverySchedulingAdmin",
                    ActionName="Configure",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Resanehlab.DeliveryScheduling.Configuration"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"),
                    ControllerName = "AvailableDeliveryTimeRangeAdmin",
                    ActionName = "List",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "ZarIran.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"),
                    ControllerName = "DeliverySchedulingAdmin",
                    ActionName = "TimeTable",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "ZarIran.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"),
                    ControllerName = "ExceptionDateAdmin",
                    ActionName = "List",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "ZarIran.DeliveryScheduling.ManageExceptionDates"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"),
                    ControllerName = "DeliverySchedulingAdmin",
                    ActionName = "OrderDeliveryList",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "ZarIran.DeliveryScheduling.OrderDelivery"
                }
            };



            zarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = pluginName,
                SystemName = pluginSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = childNodes
            });

            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/TimeTable"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "ZarIran.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"
            //});
            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/List"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "ZarIran.DeliveryScheduling.ManageExceptionDates"
            //});
            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/OrderDeliveryList"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "ZarIran.DeliveryScheduling.OrderDelivery"
            //});

            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(zarIranSiteMapNode);

            await Task.CompletedTask;
        }

        #endregion

        public async Task<bool> StoredProcedureExistsAsync(string storedProcedureName)
        {
            return (await _dataProvider.QueryAsync<IntegerQueryResultModel>(
                string.Format("SELECT 1 AS [Result] FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC')",
                storedProcedureName))).FirstOrDefault() != null;
        }
    }
}
