using Microsoft.AspNetCore.Mvc.Rendering;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Models.PaymentMethodDiscounts;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Domain;
using ZarIran.Plugin.Payments.ManagePaymentMethod.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZarIran.Plugin.Payments.ManagePaymentMethod.Areas.Admin.Factories
{
    public class PaymentMethodDiscountModelFactory : IPaymentMethodDiscountModelFactory
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentMethodDiscountService _paymentMethodDiscountService;

        #endregion

        #region Ctor

        public PaymentMethodDiscountModelFactory(ISpecificationAttributeService specificationAttributeService,
                                            ILocalizationService localizationService,
                                            IPaymentMethodDiscountService paymentMethodDiscountService)
        {
            _specificationAttributeService = specificationAttributeService;
            _localizationService = localizationService;
            _paymentMethodDiscountService = paymentMethodDiscountService;
        }

        #endregion

        #region Methods

        public virtual async Task<PaymentMethodDiscountSearchModel> PreparePaymentMethodDiscountSearchModelAsync(PaymentMethodDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //var carBrandSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.CarBrandSpecificationAttributeId);
            //var modelSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.ModelSpecificationAttributeId);

            //searchModel.AvailableCarBrandSpecificationAttributeOptions.Add(new SelectListItem
            //{
            //    Value = "0",
            //    Text = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.PaymentMethodDiscount.List.SearchCarBrandSpecificationAttributeOption.All")
            //});

            //foreach (var specificationAttributeOption in carBrandSpecificationAttributeOptions)
            //    searchModel.AvailableCarBrandSpecificationAttributeOptions.Add(new SelectListItem
            //    {
            //        Value = specificationAttributeOption.Id.ToString(),
            //        Text = specificationAttributeOption.Name
            //    });


            //searchModel.AvailableModelSpecificationAttributeOptions.Add(new SelectListItem
            //{
            //    Value = "0",
            //    Text = await _localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.PaymentMethodDiscount.List.SearchModelSpecificationAttributeOption.All")
            //});

            //foreach (var specificationAttributeOption in modelSpecificationAttributeOptions)
            //    searchModel.AvailableModelSpecificationAttributeOptions.Add(new SelectListItem
            //    {
            //        Value = specificationAttributeOption.Id.ToString(),
            //        Text = specificationAttributeOption.Name
            //    });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual async Task<PaymentMethodDiscountModel> PreparePaymentMethodDiscountModelAsync(PaymentMethodDiscountModel model, PaymentMethodDiscount paymentMethodDiscount)
        {
            if (model == null)
            {
                model = new PaymentMethodDiscountModel();

                if (paymentMethodDiscount != null)
                    model = paymentMethodDiscount.ToModel<PaymentMethodDiscountModel>();
            }

            //var carBrandSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.CarBrandSpecificationAttributeId);
            //var modelSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.ModelSpecificationAttributeId);

            //model.AvailableCarBrandSpecificationAttributeOptions = carBrandSpecificationAttributeOptions.Select(carBrandSpecificationAttributeOption => new SelectListItem
            //{
            //    Text = carBrandSpecificationAttributeOption.Name,
            //    Value = carBrandSpecificationAttributeOption.Id.ToString()
            //}).ToList();

            //model.AvailableModelSpecificationAttributeOptions = modelSpecificationAttributeOptions.Select(modelSpecificationAttributeOption => new SelectListItem
            //{
            //    Text = modelSpecificationAttributeOption.Name,
            //    Value = modelSpecificationAttributeOption.Id.ToString()
            //}).ToList();

            return model;
        }

        public virtual async Task<PaymentMethodDiscountListModel> PreparePaymentMethodDiscountListModelAsync(PaymentMethodDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var paymentMethodDiscounts = await _paymentMethodDiscountService.GetPaymentMethodDiscountsAsync(pageIndex: searchModel.Page - 1,
                                                                                                  pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new PaymentMethodDiscountListModel().PrepareToGridAsync(searchModel, paymentMethodDiscounts, () =>
            {
                return paymentMethodDiscounts.SelectAwait(async paymentMethodDiscount =>
                {
                    //fill in model values from the entity
                    var paymentMethodDiscountModel = paymentMethodDiscount.ToModel<PaymentMethodDiscountModel>();
                    //paymentMethodDiscountModel.ModelSpecificationAttributeOptionName = (await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(paymentMethodDiscountModel.ModelSpecificationAttributeOptionId)).Name;
                    //paymentMethodDiscountModel.CarBrandSpecificationAttributeOptionName = (await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(paymentMethodDiscountModel.CarBrandSpecificationAttributeOptionId)).Name;

                    //var carBrandSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.CarBrandSpecificationAttributeId);
                    //var modelSpecificationAttributeOptions = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(_carInfoSettings.ModelSpecificationAttributeId);

                    //paymentMethodDiscountModel.AvailableCarBrandSpecificationAttributeOptions = carBrandSpecificationAttributeOptions.Select(carBrandSpecificationAttributeOption => new SelectListItem
                    //{
                    //    Text = carBrandSpecificationAttributeOption.Name,
                    //    Value = carBrandSpecificationAttributeOption.Id.ToString()
                    //}).ToList();

                    //paymentMethodDiscountModel.AvailableModelSpecificationAttributeOptions = modelSpecificationAttributeOptions.Select(modelSpecificationAttributeOption => new SelectListItem
                    //{
                    //    Text = modelSpecificationAttributeOption.Name,
                    //    Value = modelSpecificationAttributeOption.Id.ToString()
                    //}).ToList();

                    return paymentMethodDiscountModel;
                });
            });

            return model;
        }

        #endregion
    }
}
