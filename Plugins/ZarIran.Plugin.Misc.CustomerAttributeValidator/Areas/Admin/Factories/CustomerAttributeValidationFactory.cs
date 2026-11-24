using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Models;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Domain;
using ZarIran.Plugin.Misc.CustomerAttributeValidator.Services;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Misc.CustomerAttributeValidator.Areas.Admin.Factories
{
    public class CustomerAttributeValidationFactory : ICustomerAttributeValidationFactory
    {
        #region Fields

        private readonly ICustomerAttributeValidationService _customerAttributeValidationService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomerAttributeValidationFactory(ICustomerAttributeValidationService customerAttributeValidationService,
                                                  ICustomerAttributeService customerAttributeService,
                                                  ILocalizationService localizationService)
        {
            _customerAttributeValidationService = customerAttributeValidationService;
            _customerAttributeService = customerAttributeService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<CustomerAttributeValidationSearchModel> PrepareCustomerAttributeValidationSearchModelAsync(CustomerAttributeValidationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return await Task.FromResult(searchModel);
        }

        public async Task<CustomerAttributeValidationListModel> PrepareCustomerAttributeValidationListModelAsync(CustomerAttributeValidationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customerAttributeValidations = await _customerAttributeValidationService.GetCustomerAttributeValidationsAsync(
                                                                                                                 pageIndex: searchModel.Page - 1,
                                                                                                                 pageSize: searchModel.PageSize);

            var customerAttributes = await _customerAttributeService.GetAllCustomerAttributesAsync();

            //prepare grid model
            var model = new CustomerAttributeValidationListModel().PrepareToGrid(searchModel, customerAttributeValidations, () =>
            {
                return customerAttributeValidations.Select(customerAttributeValidation =>
                {
                    var currentCustomerAttribute = customerAttributes.FirstOrDefault(w => w.Id == customerAttributeValidation.CustomerAttributeId);

                    if (currentCustomerAttribute == null)
                        throw new ArgumentNullException(nameof(currentCustomerAttribute));

                    var customerAttributeValidationModel = customerAttributeValidation.ToModel<CustomerAttributeValidationModel>();

                    //priceInquiryModel.CustomerMobilePhoneNumber = customer.MobilePhoneNumber;
                    customerAttributeValidationModel.CustomerAttributeName = currentCustomerAttribute.Name;

                    return customerAttributeValidationModel;
                });
            });

            return model;
        }

        public async Task<CustomerAttributeValidationModel> PrepareCustomerAttributeValidationModelAsync(CustomerAttributeValidationModel model, CustomerAttributeValidation customerAttributeValidation)
        {

            if (customerAttributeValidation != null)
            {
                //fill in model values from the entity
                if (model == null)
                    model = customerAttributeValidation.ToModel<CustomerAttributeValidationModel>();

            }
            var customerAttributes = (await _customerAttributeService.GetAllCustomerAttributesAsync())
                                                                                        .Where(w => w.AttributeControlType == AttributeControlType.TextBox)
                                                                                        .ToList();

            await PrepareCustomerAttributeAsync(model.AvailableCustomerAttributes, customerAttributes);

            return model;
        }

        #endregion

        #region Utilities

        private async Task PrepareCustomerAttributeAsync(List<SelectListItem> availableCustomerAttributes, IList<CustomerAttribute> customerAttributes)
        {
            if (customerAttributes == null)
                return;

            availableCustomerAttributes.AddRange(await customerAttributes.SelectAwait(async s => new SelectListItem
            {
                Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                Value = s.Id.ToString(),

            }).ToListAsync());
        }

        #endregion
    }
}