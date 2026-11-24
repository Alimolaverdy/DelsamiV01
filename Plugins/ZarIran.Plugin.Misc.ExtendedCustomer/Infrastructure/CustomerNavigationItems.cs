using ZarIran.Plugin.Misc.Core.Bases;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Models.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZarIran.Plugin.Misc.Core.Enums;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Infrastructure
{
    public class CustomerNavigationItems : ICustomerNavigationItems
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Utilities

        public CustomerNavigationItems(ILocalizationService localizationService,
                                       ICustomerService customerService,
                                       IWorkContext workContext)
        {
            _localizationService = localizationService;
            _customerService = customerService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<List<CustomerNavigationItemModel>> PrepareCustomerNavigationItemsAsync()
        {
            List<CustomerNavigationItemModel> customerNavigationItems = new();

            var customer = await _workContext.GetCurrentCustomerAsync();

            //if (await _customerService.IsInCustomerRoleAsync(customer, Core.Infrastrcuture.Constants.CASH_REPAIR_SHOP) || await _customerService.IsInCustomerRoleAsync(customer, Core.Infrastrcuture.Constants.CREDIT_REPAIR_SHOP))
            //{
            customerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerShebaNumberList",
                Title = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.ExtendedCustomer.Account.CustomerShebaNumbers"),
                Tab = (int)ZarIranCustomerNavigationEnum.CustomerShebaNumbers,
                ItemClass = "sheba-numbers",
            });
            //}

            return customerNavigationItems;
        }

        #endregion
    }

}
