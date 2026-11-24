using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Services
{
    public class CustomerProfileValidationService : ICustomerProfileValidationService
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;

        public CustomerProfileValidationService(
            ICustomerService customerService,
            ICustomerAttributeValidationService customerAttributeValidationService)
        {
            _customerService = customerService;
            _customerAttributeValidationService = customerAttributeValidationService;
        }

        public async Task<bool> IsCheckoutCompletedAsync(Customer customer)
        {
            if (customer == null)
                return false;

            if (await _customerService.IsGuestAsync(customer))
                return true;

            List<CustomerAttribute> invalidStateCustomerAttributes = new();
            List<CustomerAttribute> nullOrEmptyCustomerAttributes = new();

            if (!string.IsNullOrWhiteSpace(customer.CustomCustomerAttributesXML))
            {
                invalidStateCustomerAttributes =
                    await _customerAttributeValidationService.GetInvalidCustomerAttributesAsync(customer.CustomCustomerAttributesXML);

                nullOrEmptyCustomerAttributes =
                    await _customerAttributeValidationService.GetNullOrEmptyRequiredCustomerAttributesAsync(customer.CustomCustomerAttributesXML);
            }

            invalidStateCustomerAttributes.AddRange(nullOrEmptyCustomerAttributes);

            if (invalidStateCustomerAttributes.Any() ||
                string.IsNullOrWhiteSpace(customer.FirstName) ||
                string.IsNullOrWhiteSpace(customer.LastName))
            {
                return false;
            }

            return true;
        }

    }

}
