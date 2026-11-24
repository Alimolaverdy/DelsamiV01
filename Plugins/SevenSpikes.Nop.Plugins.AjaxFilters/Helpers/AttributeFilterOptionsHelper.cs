using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public class AttributeFilterOptionsHelper : IAttributeFilterOptionsHelper
{
    private IProductAttributeService7Spikes ProductAttributeService7Spikes { get; set; }

    private Dictionary<int, HashSet<int>> AvailableAttributeOptionIds { get; set; }

    private bool NoAttributeFiltersSelected { get; set; }

    public Dictionary<int, List<Product>> PotentiallyAvailableAttributeOptionIds { get; private set; }

    public AttributeFilterOptionsHelper(IProductAttributeService7Spikes productAttributeService7Spikes)
    {
        ProductAttributeService7Spikes = productAttributeService7Spikes;
        AvailableAttributeOptionIds = new Dictionary<int, HashSet<int>>();
        PotentiallyAvailableAttributeOptionIds = new Dictionary<int, List<Product>>();
    }

    public async Task<IQueryable<Product>> GetProductsForAttributeFiltersAndDetermineAvailableAttributeOptionsForLaterRerievalAsync(
        IQueryable<Product> query,
        AttributeFilterModelDTO attributeFilterModelDTO)
    {
        var attributeFilters = attributeFilterModelDTO?.AttributeFilterDTOs;
        if (attributeFilters == null || attributeFilters.Count == 0)
        {
            NoAttributeFiltersSelected = true;
            return query;
        }

        NoAttributeFiltersSelected = false;

        // Materialize the candidate products once, then fetch their attribute mappings in
        // bounded batches. The previous implementation issued one database query per product.
        var products = query.ToList();
        if (products.Count == 0)
            return query;

        var productIds = products.Select(product => product.Id).Distinct().ToList();
        var attributeMappings = new List<ProductAttributeMapping>();

        // Stay below SQL Server's 2,100-parameter limit when the provider expands Contains().
        foreach (var productIdBatch in productIds.Chunk(1000))
        {
            var batchMappings = await ProductAttributeService7Spikes
                .GetAllProductVariantAttributesWhichHaveValuesByProductIdsAsync(productIdBatch.ToList(), false);

            if (batchMappings != null)
                attributeMappings.AddRange(batchMappings);
        }

        var mappingsByProductId = attributeMappings
            .GroupBy(mapping => mapping.ProductId)
            .ToDictionary(
                group => group.Key,
                group => (IList<ProductAttributeMapping>)group.ToList());

        var productIdsToRemove = new List<int>();
        foreach (var product in products)
        {
            IList<ProductAttributeMapping> productMappings =
                mappingsByProductId.TryGetValue(product.Id, out var availableMappings)
                    ? availableMappings
                    : Array.Empty<ProductAttributeMapping>();

            if (!DetermineWhetherProductMeetsAttributeFilters(
                    product,
                    productMappings,
                    attributeFilters))
            {
                productIdsToRemove.Add(product.Id);
            }
        }

        return query.Where(product => !productIdsToRemove.Contains(product.Id));
    }

    private bool DetermineWhetherProductMeetsAttributeFilters(
        Product product,
        IList<ProductAttributeMapping> productAttributeMappings,
        IList<AttributeFilterDTO> attributeFilterDTOs)
    {
        if (attributeFilterDTOs == null || attributeFilterDTOs.Count == 0)
            return true;

        var remainingAttributeFilters = attributeFilterDTOs.ToList();
        var potentiallyMatchingGroups = new List<AttributeFilterDTO>();
        var potentiallyMatchingAttributeIds = new List<int>();

        foreach (var attributeMapping in productAttributeMappings)
        {
            var selectedFilter = remainingAttributeFilters.FirstOrDefault(filter =>
                filter.SelectedProductVariantIds.Contains(attributeMapping.Id));

            if (selectedFilter != null)
            {
                remainingAttributeFilters.Remove(selectedFilter);
            }
            else
            {
                var possibleFilter = remainingAttributeFilters.FirstOrDefault(filter =>
                    filter.AllProductVariantIds.Contains(attributeMapping.Id));

                if (possibleFilter != null)
                {
                    potentiallyMatchingGroups.Add(possibleFilter);
                    potentiallyMatchingAttributeIds.Add(attributeMapping.Id);
                }
            }

            if (remainingAttributeFilters.Count == 0)
                break;
        }

        if (remainingAttributeFilters.Count == 0)
        {
            foreach (var attributeMapping in productAttributeMappings)
                AddAvailableAttributeOptionId(attributeMapping.Id, product.Id);

            return true;
        }

        if (remainingAttributeFilters.Count == 1
            && potentiallyMatchingGroups.Contains(remainingAttributeFilters[0]))
        {
            foreach (var attributeId in potentiallyMatchingAttributeIds)
                AddPotentialAttributeOptionId(attributeId, product);
        }

        return false;
    }

    public async Task<bool> DetermineWhetherPotentialProductMeetsAttributeFiltersAsync(Product product, AttributeFilterModelDTO attributeFilterModelDTO)
    {
        if (attributeFilterModelDTO == null || attributeFilterModelDTO.AttributeFilterDTOs.Count == 0)
        {
            return true;
        }
        IList<AttributeFilterDTO> attributeFilterDtosLocal = attributeFilterModelDTO.AttributeFilterDTOs.ToList();
        foreach (ProductAttributeMapping attributeMapping in await ProductAttributeService7Spikes.GetAllProductVariantAttributesWhichHaveValuesByProductIdAsync(product.Id, false))
        {
            int productVariantAttributeId = attributeMapping.Id;
            AttributeFilterDTO attributeFilterDto = ((IEnumerable<AttributeFilterDTO>)attributeFilterDtosLocal).FirstOrDefault((Func<AttributeFilterDTO, bool>)((AttributeFilterDTO x) => x.SelectedProductVariantIds.Contains(productVariantAttributeId)));
            if (attributeFilterDto != null)
            {
                attributeFilterDtosLocal.Remove(attributeFilterDto);
            }
            if (attributeFilterDtosLocal.FirstOrDefault() == null)
            {
                break;
            }
        }
        return attributeFilterDtosLocal.FirstOrDefault() == null;
    }

    private void AddAvailableAttributeOptionId(int attributeOptionId, int productId)
    {
        if (!AvailableAttributeOptionIds.TryGetValue(attributeOptionId, out var productIds))
        {
            productIds = new HashSet<int>();
            AvailableAttributeOptionIds.Add(attributeOptionId, productIds);
        }

        productIds.Add(productId);
    }

    private void AddPotentialAttributeOptionId(int attributeOptionId, Product product)
    {
        if (!PotentiallyAvailableAttributeOptionIds.ContainsKey(attributeOptionId))
        {
            List<Product> productList = new List<Product> { product };
            PotentiallyAvailableAttributeOptionIds.Add(attributeOptionId, productList);
            return;
        }
        IList<Product> attributeOptionId2 = PotentiallyAvailableAttributeOptionIds[attributeOptionId];
        if (!attributeOptionId2.Contains(product))
        {
            attributeOptionId2.Add(product);
        }
    }

    public async Task<IList<int>> GetAvailableProductVariantAttributeIdsForFilteredProductsAsync(IList<Product> products)
    {
        List<int> productVariantAttributeIds = new List<int>();
        productVariantAttributeIds.AddRange(PotentiallyAvailableAttributeOptionIds.Keys);
        var filteredProductIdList = products.Select(product => product.Id).Distinct().OrderBy(id => id).ToList();
        var filteredProductIds = new HashSet<int>(filteredProductIdList);

        if (NoAttributeFiltersSelected && filteredProductIdList.Count > 0)
        {
            foreach (var productIdBatch in filteredProductIdList.Chunk(1000))
            {
                var productAttributeMappings = await ProductAttributeService7Spikes
                    .GetAllProductVariantAttributesWhichHaveValuesByProductIdsAsync(productIdBatch.ToList(), false);

                if (productAttributeMappings != null)
                    productVariantAttributeIds.AddRange(productAttributeMappings.Select(mapping => mapping.Id));
            }
        }
        else
        {
            var availableAttributeIds = new HashSet<int>(productVariantAttributeIds);
            foreach (var entry in AvailableAttributeOptionIds)
            {
                if (!availableAttributeIds.Contains(entry.Key)
                    && entry.Value.Any(filteredProductIds.Contains))
                {
                    productVariantAttributeIds.Add(entry.Key);
                    availableAttributeIds.Add(entry.Key);
                }
            }
        }
        return productVariantAttributeIds;
    }
}
