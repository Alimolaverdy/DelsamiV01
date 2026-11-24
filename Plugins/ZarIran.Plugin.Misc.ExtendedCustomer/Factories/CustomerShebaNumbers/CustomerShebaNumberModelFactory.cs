using ZarIran.Plugin.Misc.ExtendedCustomer.Domain;
using ZarIran.Plugin.Misc.ExtendedCustomer.Models.CustomerShebaNumbers;
using ZarIran.Plugin.Misc.ExtendedCustomer.Services;
using Nop.Core;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.ExtendedCustomer.Factories.CustomerShebaNumbers
{
    public class CustomerShebaNumberModelFactory : ICustomerShebaNumberModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ICustomerShebaNumberService _customerShebaNumberService;

        #endregion

        #region Ctor

        public CustomerShebaNumberModelFactory(IWorkContext workContext,
                                               ICustomerShebaNumberService customerShebaNumberService)
        {
            _workContext = workContext;
            _customerShebaNumberService = customerShebaNumberService;
        }

        #endregion

        #region Methods

        public async Task<CustomerShebaNumberListModel> PrepareCustomerShebaNumberModelListAsync(int? customerId = null)
        {
            if (!customerId.HasValue)
                customerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            var customerShebaNumbers = await _customerShebaNumberService.GetCustomerShebaNumbersAsync(customerId.Value);

            var customerShebaNumberListModel = new CustomerShebaNumberListModel();

            foreach (var customerShebaNumber in customerShebaNumbers)
            {
                var customerShebaNumberModel = customerShebaNumber.ToModel<CustomerShebaNumberModel>();

                customerShebaNumberListModel.CustomerShebaNumbers.Add(customerShebaNumberModel);

            }
            return customerShebaNumberListModel;
        }

        public virtual async Task<CustomerShebaNumberSearchModel> PrepareCustomerShebaNumberSearchModelAsync(CustomerShebaNumberSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);

        }

        public virtual async Task<CustomerShebaNumberListModel> PrepareShebaNumberListModelAsync(CustomerShebaNumberSearchModel searchModel, int? customerId = null)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customerShebaNumbers = await _customerShebaNumberService.GetCustomerShebaNumbersAsync(customerId: (await _workContext.GetCurrentCustomerAsync()).Id,
                                                                                              pageIndex: searchModel.Page - 1,
                                                                                              pageSize: searchModel.PageSize);
            var customerShebaNumberList = new CustomerShebaNumberListModel();

            foreach (var customerShebaNumber in customerShebaNumbers)
                customerShebaNumberList.CustomerShebaNumbers.Add(customerShebaNumber.ToModel<CustomerShebaNumberModel>());

            return customerShebaNumberList;
        }

        public CustomerShebaNumberModel PrepareCustomerShebaNumberModel(CustomerShebaNumberModel customerShebaNumberModel, CustomerShebaNumber customerShebaNumber)
        {
            if (customerShebaNumberModel == null)
            {
                customerShebaNumberModel = new CustomerShebaNumberModel();

                if (customerShebaNumber != null)
                    customerShebaNumberModel = customerShebaNumber.ToModel<CustomerShebaNumberModel>();
            }

            return customerShebaNumberModel;
        }

        #endregion
    }
}
