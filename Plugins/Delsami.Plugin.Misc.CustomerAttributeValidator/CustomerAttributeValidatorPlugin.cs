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

namespace Delsami.Plugin.Misc.CustomerAttributeValidator
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
                ["Delsami.Plugin.BaseName"] = "Delsami",
                ["Delsami.Plugin.SystemName"] = "Delsami",
                ["Delsami.Plugin.Settings.Saved"] = "Settings saved successfully",
                ["Delsami.Plugin.Settings.Error"] = "Error saving settings",
                ["Delsami.Plugin.Error"] = "Error saving settings",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Name"] = "Customer attr validator",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.SystemName"] = "CustomerAttributeValidator",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Configuration"] = "Configuration",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Configuration.SystemName"] = "CustomerAttributeValidatorConfiguration",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Added"] = "The new customer attribute validation added successfully.",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.Updated"] = "The new customer attribute validation updated successfully.",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List.SystemName"] = "CustomerAttributeValdationList",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List"] = "Customer attr validations",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId"] = "Customer attribute",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern"] = "Regex pattern",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.IsNationalCode"] = "Is national code",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.PreventFromShoppingInInvalidState"] = "Prevent from shopping",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.Active"] = "Active",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.AddNew"] = "Add a new customer attribute validation",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.BackToList"] = "Back to customer attribute validation list",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Edit"] = "Edit customer attribute validation",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.CustomerAttributeId.IsRequired"] = "Customer attribute is required",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.IsRequired"] = "Regex pattern is required",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.Admin.CustomerAttributeValidation.CustomerAttributeDuplicated"] = "Selected customer attribute is duplicated",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.Fields.RegexPattern.Invalid"] = "Regex pattern invalid",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidState"] = "The {0} is not valid",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidStateInCustomerInfo"] = "The {0} is not valid in customer info section.Please correct it to continue shopping",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.HasNoValueInCustomerInfo"] = "The {0} has no value in customer info section.Please fill it to continue shopping",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerCheckoutInfoStep"] = "The {0} has no value in customer info section.Please fill it to continue shopping",
                ["Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.DuplicateNationalCode"] = "The National Code Is Duplicate",

                //["Delsami.Plugin.Misc.CarInfo.Configuration.SelectedSpecificationAttributeIdForModel"] = "Specification attribute used for 'Model'",
                //["Delsami.Plugin.Misc.CarInfo.Configuration.SelectedSpecificationAttributeIdForYear"] = "Specification attribute used for 'Year'",
                //["Delsami.Plugin.Misc.CarInfo.Account.NoCustomerCars"] = "No cars",
                //["Delsami.Plugin.Misc.CarInfo.Account.CustomerCars"] = "Cars",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ManufacturerId"] = "Manufacturer",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ModelSpecificationAttributeOptionId"] = "Model",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.YearSpecificationAttributeOptionId"] = "Year",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.SelectDefaultOptionText"] = "Select Manufacturer",
                //["Delsami.Plugin.Misc.CarInfo.Model.SelectDefaultOptionText"] = "Select Model",
                //["Delsami.Plugin.Misc.CarInfo.Year.SelectDefaultOptionText"] = "Select Year",
                //["Delsami.Plugin.Misc.CarInfo.CustomerModel.Fields.ManufacturerId.Required"] = "Manufacturer is required",
                //["Delsami.Plugin.Misc.CarInfo.CustomerModel.Fields.ModelSpecificationAttributeOptionId.Required"] = "Model is required",
                //["Delsami.Plugin.Misc.CarInfo.Account.Car.AddNew"] = "Add new car",
                //["Delsami.Plugin.Misc.CarInfo.Account.Car.Edit"] = "Edit car",
                //["Delsami.Plugin.Misc.CarInfo.AlphabetLicencePlate.SelectDefaultOptionText"] = "Select alphabet",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.AlphabetLicencePlate"] = "Alphabet of licence plate",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.TwoLeftNumberLicencePlate"] = "Two digits on the left side of the license plate",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.TwoLeftNumberLicencePlate.ValidationError"] = "The two digits on the left side of the license plate are not entered",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ThreeRighNumberLicencePlate"] = "Three digits on the right side of the license plate",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ThreeRighNumberLicencePlate.ValidationError"] = "The three digits on the right side of the license plate are not entered",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.IranNumberLicencePlate"] = "Iran number of licence plate",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.IranNumberLicencePlate.ValidationError"] = "The iran number of the license plate are not entered",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ThirdPartyInsurancePolicyNumber"] = "Third party insurance policy number",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.ThirdPartyInsurancePolicyExpirationDate"] = "Third party insurance policy expiration date",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.BodyInsurancePolicyNumber"] = "Body insurance policy number",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.TechnicalVisitExpirationDate"] = "Expiry date of technical visit of the car",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["Delsami.Plugin.Misc.CarInfo.Account.CarServiceReminders"] = "Car service reminders",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.VinNumber"] = "VIN number",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.CustomerCarId"] = "Car",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.CarServiceReminderType"] = "Type",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.Message"] = "Message",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.DueDate"] = "Due Date",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.CarServiceReminderStatus"] = "Status",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.ManufacturerModelSpecificationOptions.Hint"] = "In this section You can choose related models from specification attribute options to this manufacturer.",
                //["Delsami.plugin.Misc.CarInfo.ManufacturerModelSpecificationOption"] = "Related models",
                //["Delsami.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOptionModel.Fields.ModelSpecificationOptionName"] = "Model (Specificaiotn attribute option name)",
                //["Delsami.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOption.AddNew"] = "Add new model",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.ManufacturerModelSpecificationOptions.SaveBeforeEdit"] = "You need to save the manufacturer before you can add models for this manufacturer page.",
                //["Delsami.Plugin.Misc.CarInfo.AddSpecificationModelSearchModel.Fields.SearchSpecificationAttributeOptionName"] = "Name",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Save"] = "The manufacturer type has been saved",
                //["Delsami.Plugin.Misc.CarInfo.ManufacturerModelSpecificationOptionSearch.IsDomestic"] = "Domestic",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Error"] = "Error in saving process",
                //["Delsami.Plugin.Misc.CarInfo.Configuration.AllProductsCategoryId"] = "All products category",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.LicencePlate"] = "Licence plate",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.LicencePlate.ValidationError"] = "All values related to the license plate must have a value",
                //["Delsami.Plugin.Misc.CarInfo.Account.NoCarServiceReminders"] = "There are no car service reminder",
                //["Delsami.Plugin.Misc.CarInfo.CarServiceReminder.Message.LengthInvalid"] = "Message length invalid",
                //["Delsami.Plugin.Misc.CarInfo.Manufacturer.ManufacturerType.Saved"] = "Manufacturer type saved",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.BodyInsurancePolicyExpirationDate"] = "Body insurance policy expiration date",
                //["Delsami.Plugin.Misc.CarInfo.CustomerCarModel.TechnicalVisitExpirationDate"] = "Technical visit expiration date",

            });

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            {
                return;
            }

            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");
            var pluginSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.SystemName");
            var pluginName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Name");
            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.Configuration.SystemName");
            var CustomerAttributeValidationListName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List");
            var CustomerAttributeValidationListSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.List.SystemName");

            var DelsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (DelsamiSiteMapNode == null)
                DelsamiSiteMapNode = new SiteMapNode
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

            DelsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = pluginName,
                SystemName = pluginSystemName,
                Visible = true,
                IconClass = "far fa-file-alt",
                ChildNodes = pluginChildNodes
            });


            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(DelsamiSiteMapNode);

            await Task.CompletedTask;
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<CustomerAttributeValidatorSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.Misc.CarInfo");

            await base.UninstallAsync();
        }

        #endregion
    }
}
