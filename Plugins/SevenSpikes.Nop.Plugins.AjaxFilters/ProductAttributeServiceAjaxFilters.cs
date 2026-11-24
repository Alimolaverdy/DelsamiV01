using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using SevenSpikes.Nop.Plugins.AjaxFilters.Infrastructure.Cache;
using SevenSpikes.Nop.Plugins.AjaxFilters.Models.AttributeFilter;
using SevenSpikes.Nop.Plugins.AjaxFilters.Services;

public class ProductAttributeServiceAjaxFilters : IProductAttributeServiceAjaxFilters
{
    private readonly IRepository<PredefinedProductAttributeValue> _predefinedAttributeValueRepository;
    private readonly IStaticCacheManager _staticCacheManager;

    public ProductAttributeServiceAjaxFilters(
        IRepository<PredefinedProductAttributeValue> predefinedAttributeValueRepository,
        IStaticCacheManager staticCacheManager)
    {
        _predefinedAttributeValueRepository = predefinedAttributeValueRepository;
        _staticCacheManager = staticCacheManager;
    }

    public async Task<IList<AttributeFilterItem>> GetSortedAttributeValuesBasedOnTheirPredefinedDisplayOrderAsync(
        IEnumerable<AttributeFilterItem> attributeValues)
    {
        var predefinedValues = await _staticCacheManager.GetAsync(
            NopAjaxFiltersModelCacheEventConsumer.NOP_AJAX_FILTERS_PREDEFINED_ATTRIBUTE_VALUES_KEY,
            () => AsyncIQueryableExtensions.ToListAsync(_predefinedAttributeValueRepository.Table));

        var query = from attributeValue in attributeValues
                    join predefinedValue in predefinedValues
                        on new
                        {
                            attributeValue.AttributeId,
                            AttributeValue = attributeValue.Name
                        }
                        equals new
                        {
                            AttributeId = predefinedValue.ProductAttributeId,
                            AttributeValue = predefinedValue.Name
                        }
                        into matchingPredefinedValues
                    from predefinedValue in matchingPredefinedValues.DefaultIfEmpty(
                        new PredefinedProductAttributeValue { DisplayOrder = int.MaxValue })
                    orderby predefinedValue.DisplayOrder
                    select attributeValue;

        return query.ToList();
    }
}
