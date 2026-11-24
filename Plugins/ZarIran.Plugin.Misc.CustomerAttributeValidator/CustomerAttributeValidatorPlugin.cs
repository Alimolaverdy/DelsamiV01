using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator
{
    public class CustomerAttributeValidatorPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public CustomerAttributeValidatorPlugin(IWebHelper webHelper,
                             ISettingService settingService,
                             ILocalizationService localizationService,
                             IMessageTemplateService messageTemplateService,
                             EmailAccountSettings emailAccountSettings,
                             IScheduleTaskService scheduleTaskService,
                             IPermissionService permissionService)
        {
            _webHelper = webHelper;
            _settingService = settingService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _emailAccountSettings = emailAccountSettings;
            _scheduleTaskService = scheduleTaskService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/CustomerAttributeValidator/Configure";
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["ZarIran.Plugin.BaseName"] = "ZarIran",
                ["ZarIran.Plugin.SystemName"] = "ZarIran",
                ["ZarIran.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["ZarIran.Plugin.Settings.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Error"] = "Error saving settings",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Name"] = "Customer attr validator",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.SystemName"] = "CustomerAttributeValidator",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Configuration"] = "Configuration",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Configuration.SystemName"] = "CustomerAttributeValidatorConfiguration",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Added"] = "The new customer attribute validation added successfully.",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Updated"] = "The new customer attribute validation updated successfully.",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List.SystemName"] = "CustomerAttributeValdationList",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List"] = "Customer attr validations",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId"] = "Customer attribute",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern"] = "Regex pattern",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.IsNationalCode"] = "Is national code",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.PreventFromShoppingInInvalidState"] = "Prevent from shopping",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.Active"] = "Active",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.AddNew"] = "Add a new customer attribute validation",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.BackToList"] = "Back to customer attribute validation list",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Edit"] = "Edit customer attribute validation",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId.IsRequired"] = "Customer attribute is required",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.IsRequired"] = "Regex pattern is required",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.CustomerAttributeDuplicated"] = "Selected customer attribute is duplicated",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.Invalid"] = "Regex pattern invalid",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidState"] = "The {0} is not valid",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidStateInCustomerInfo"] = "The {0} is not valid in customer info section.Please correct it to continue shopping",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.HasNoValueInCustomerInfo"] = "The {0} has no value in customer info section.Please fill it to continue shopping",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerCheckoutInfoStep"] = "The {0} has no value in customer info section.Please fill it to continue shopping",
                ["ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.DuplicateNationalCode"] = "The National Code Is Duplicate",

                //["ZarIran.Plugin.Misc.CarInfo.Configuration.SelectedSpecificationAttributeIdForModel"] = "Specification attribute used for 'Model'",
                //["ZarIran.Plugin.Misc.CarInfo.Configuration.SelectedSpecificationAttributeIdForYear"] = "Specification attribute used for 'Year'",
                //["ZarIran.Plugin.Misc.CarInfo.Account.NoCustomerCars"] = "No cars",
                //["ZarIran.Plugin.Misc.CarInfo.Account.CustomerCars"] = "Cars",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ManufacturerId"] = "Manufacturer",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ModelSpecificationAttributeOptionId"] = "Model",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.YearSpecificationAttributeOptionId"] = "Year",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.SelectDefaultOptionText"] = "Select Manufacturer",
                //["ZarIran.Plugin.Misc.CarInfo.Model.SelectDefaultOptionText"] = "Select Model",
                //["ZarIran.Plugin.Misc.CarInfo.Year.SelectDefaultOptionText"] = "Select Year",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerModel.Fields.ManufacturerId.Required"] = "Manufacturer is required",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerModel.Fields.ModelSpecificationAttributeOptionId.Required"] = "Model is required",
                //["ZarIran.Plugin.Misc.CarInfo.Account.Car.AddNew"] = "Add new car",
                //["ZarIran.Plugin.Misc.CarInfo.Account.Car.Edit"] = "Edit car",
                //["ZarIran.Plugin.Misc.CarInfo.AlphabetLicencePlate.SelectDefaultOptionText"] = "Select alphabet",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.AlphabetLicencePlate"] = "Alphabet of licence plate",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.TwoLeftNumberLicencePlate"] = "Two digits on the left side of the license plate",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.TwoLeftNumberLicencePlate.ValidationError"] = "The two digits on the left side of the license plate are not entered",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ThreeRighNumberLicencePlate"] = "Three digits on the right side of the license plate",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ThreeRighNumberLicencePlate.ValidationError"] = "The three digits on the right side of the license plate are not entered",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.IranNumberLicencePlate"] = "Iran number of licence plate",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.IranNumberLicencePlate.ValidationError"] = "The iran number of the license plate are not entered",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ThirdPartyInsurancePolicyNumber"] = "Third party insurance policy number",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.ThirdPartyInsurancePolicyExpirationDate"] = "Third party insurance policy expiration date",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.BodyInsurancePolicyNumber"] = "Body insurance policy number",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.TechnicalVisitExpirationDate"] = "Expiry date of technical visit of the car",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["ZarIran.Plugin.Misc.CarInfo.Account.CarServiceReminders"] = "Car service reminders",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.CustomerCarId"] = "Car",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.CarServiceReminderType"] = "Type",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.Message"] = "Message",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.DueDate"] = "Due Date",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.CarServiceReminderStatus"] = "Status",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.ManufacturerModelSpecificationOptions.Hint"] = "In this section You can choose related models from specification attribute options to this manufacturer.",
                //["ZarIran.plugin.Misc.CarInfo.ManufacturerModelSpecificationOption"] = "Related models",
                //["ZarIran.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOptionModel.Fields.ModelSpecificationOptionName"] = "Model (Specificaiotn attribute option name)",
                //["ZarIran.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOption.AddNew"] = "Add new model",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.ManufacturerModelSpecificationOptions.SaveBeforeEdit"] = "You need to save the manufacturer before you can add models for this manufacturer page.",
                //["ZarIran.Plugin.Misc.CarInfo.AddSpecificationModelSearchModel.Fields.SearchSpecificationAttributeOptionName"] = "Name",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Save"] = "The manufacturer type has been saved",
                //["ZarIran.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOptionSearch.IsDomestic"] = "Domestic",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Error"] = "Error in saving process",
                //["ZarIran.Plugin.Misc.CarInfo.Configuration.AllProductsCategoryId"] = "All products category",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.LicencePlate"] = "Licence plate",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.LicencePlate.ValidationError"] = "All values related to the license plate must have a value",
                //["ZarIran.Plugin.Misc.CarInfo.Account.NoCarServiceReminders"] = "There are no car service reminder",
                //["ZarIran.Plugin.Misc.CarInfo.CarServiceReminder.Message.LengthInvalid"] = "Message length invalid",
                //["ZarIran.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Saved"] = "Manufacturer type saved",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.BodyInsurancePolicyExpirationDate"] = "Body insurance policy expiration date",
                //["ZarIran.Plugin.Misc.CarInfo.CustomerCarModel.TechnicalVisitExpirationDate"] = "Technical visit expiration date",

            });

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }

            var baseName = await _localizationService.GetResourceAsync("ZarIran.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.SystemName");
            var pluginSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.SystemName");
            var pluginName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.Name");
            var configurationName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.Configuration.SystemName");
            var CustomerAttributeValidationListName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List");
            var CustomerAttributeValidationListSystemName = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List.SystemName");

            var ZarIranSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (ZarIranSiteMapNode == null)
                ZarIranSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var pluginChildNodes = new List<SiteMapNode>
            {
                new SiteMapNode
                {
                    Title = CustomerAttributeValidationListName,
                    ControllerName = "CustomerAttributeValidation",
                    ActionName = "List",
                    IconClass="far fa-dot-circle",
                    SystemName=CustomerAttributeValidationListSystemName,
                    Visible = true,
                    RouteValues=new Microsoft.AspNetCore.Routing.RouteValueDictionary{ {"Area","Admin"} }
                },
            };

            ZarIranSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = pluginName,
                SystemName = pluginSystemName,
                Visible = true,
                IconClass = "far fa-file-alt",
                ChildNodes = pluginChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(ZarIranSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<CustomerAttributeValidatorSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("ZarIran.Plugin.Misc.CarInfo");

            await base.UninstallAsync();
        }

        #endregion
    }
}
