using ZarIran.Plugin.Misc.CustomerAttributeValidator.Services;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.EventConsumers
{
    public class CustomerAttributeDeletionHandler : IConsumer<EntityDeletedEvent<CustomerAttribute>>
    {
        #region Fields

        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;

        #endregion

        #region Ctor

        public CustomerAttributeDeletionHandler(ICustomerAttributeValidationService customerAttributeValidationService)
        {
            _customerAttributeValidationService = customerAttributeValidationService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(EntityDeletedEvent<CustomerAttribute> eventMessage)
        {
            var customerAttributeValidation = await _customerAttributeValidationService.GetCustomerAttributeValidationByCustomerAttributeIdAsync(eventMessage.Entity.Id);

            if (customerAttributeValidation != null)
                await _customerAttributeValidationService.DeleteAsync(customerAttributeValidation);
        }

        #endregion
    }
}
