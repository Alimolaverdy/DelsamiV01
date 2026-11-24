using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Services
{
    public class CustomCustomerAttributeParser : CustomerAttributeParser, ICustomerAttributeParser
    {
        #region Fields

        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomCustomerAttributeParser(
            ICustomerAttributeService customerAttributeService,
            ILocalizationService localizationService) : base(customerAttributeService, localizationService)
        {
            _customerAttributeService = customerAttributeService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public override async Task<IList<string>> GetAttributeWarningsAsync(string attributesXml)
        {
            var warnings = new List<string>();

            var customerAttributeValidationService = EngineContext.Current.Resolve<ICustomerAttributeValidationService>();

            var nullOrEmptyCustomerAttributes = await customerAttributeValidationService.GetNullOrEmptyRequiredCustomerAttributesAsync(attributesXml);

            foreach (var nullOrEmptyCustomerAttribute in nullOrEmptyCustomerAttributes)
            {
                //if not found
                var notFoundWarning = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(nullOrEmptyCustomerAttribute, a => a.Name));

                warnings.Add(notFoundWarning);
            }

            var invalidStateCustomerAttributes = await customerAttributeValidationService.GetInvalidCustomerAttributesAsync(attributesXml);

            foreach (var invalidStateCustomerAttribute in invalidStateCustomerAttributes)
            {

                if (invalidStateCustomerAttribute.Name == "NationalCodeDuplicate")
                {
                    var nationalWarningMessage = string.Format(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.DuplicateNationalCode"), await _localizationService.GetLocalizedAsync(invalidStateCustomerAttribute, a => a.Name));
                    warnings.Add(nationalWarningMessage);
                }
                else
                {
                    var warningMessage = string.Format(await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CustomerAttributeValidator.CustomerAttributeValidation.CustomerAttribute.InvalidState"), await _localizationService.GetLocalizedAsync(invalidStateCustomerAttribute, a => a.Name));
                    warnings.Add(warningMessage);
                }
            }

            return warnings;
        }

        #endregion
    }
}
