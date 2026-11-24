using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using SevenSpikes.Nop.Services.Catalog;
using SevenSpikes.Nop.Services.Catalog.DTO;
using SevenSpikes.Nop.Services.Helpers;

namespace SevenSpikes.Nop.Plugins.AjaxFilters.Helpers;

public class SpecificationFilterOptionsHelper : ISpecificationFilterOptionsHelper
{
    private bool _shouldRebuildSpecificationOptionsDictionary;

    public ISpecificationAttributeService7Spikes SpecificationAttributeService7Spikes { get; set; }

    private Dictionary<int, HashSet<int>> AvailableSpecificationAttributeOptionIds { get; set; }

    private bool NoSpecificationFiltersSelected { get; set; }

    private IDictionary<int, IList<int>> SpecificationOptionsDictionary { get; set; }

    public Dictionary<int, List<Product>> PotentiallyAvailableSpecificationOptionIds { get; private set; }

    public SpecificationFilterOptionsHelper(ISpecificationAttributeService7Spikes specificationAttributeService7Spikes)
    {
        SpecificationAttributeService7Spikes = specificationAttributeService7Spikes;
        AvailableSpecificationAttributeOptionIds = new Dictionary<int, HashSet<int>>();
        PotentiallyAvailableSpecificationOptionIds = new Dictionary<int, List<Product>>();
    }

    public async Task<IQueryable<Product>> GetProductsForSpecificationFiltersAndDetermineAvailableSpecificationOptionsForLaterRerievalAsync(IQueryable<Product> query, SpecificationFilterModelDTO specifiationFilterModelDTO)
    {
        IList<int> productIdsToRemove = new List<int>();
        if (specifiationFilterModelDTO != null && specifiationFilterModelDTO.SpecificationFilterDTOs != null && specifiationFilterModelDTO.SpecificationFilterDTOs.Count > 0)
        {
            NoSpecificationFiltersSelected = false;
            IDictionary<int, IList<int>> productAttributeSpecificationOptionsDictionary = await SpecificationAttributeService7Spikes.GetSpecificationAttributeOptionsDictionaryForProductsAsync(query);
            _shouldRebuildSpecificationOptionsDictionary = true;
            foreach (Product product in query)
            {
                if (!(await DetermineWhetherProductsMeetsSpecificationFiltersAsync(product, productAttributeSpecificationOptionsDictionary, specifiationFilterModelDTO.SpecificationFilterDTOs)))
                {
                    productIdsToRemove.Add(product.Id);
                }
            }
            query = query.Where((Expression<Func<Product, bool>>)((Product p) => !productIdsToRemove.Contains(p.Id)));
        }
        else
        {
            NoSpecificationFiltersSelected = true;
        }
        return query;
    }

    private async Task<bool> DetermineWhetherProductsMeetsSpecificationFiltersAsync(Product product, IDictionary<int, IList<int>> productSpecificationAttributeOptionsDictionary, IList<SpecificationFilterDTO> specificationFilterDtos)
    {
        if (specificationFilterDtos == null || specificationFilterDtos.Count == 0)
        {
            return true;
        }
        IList<SpecificationFilterDTO> specificationFilterDtosLocal = specificationFilterDtos.ToList();
        IList<SpecificationFilterDTO> potentiallyOkGroups = new List<SpecificationFilterDTO>();
        List<int> potentiallyOkOptionIds = new List<int>();
        if (specificationFilterDtosLocal.Count > 0 && productSpecificationAttributeOptionsDictionary.ContainsKey(product.Id))
        {
            foreach (int num in productSpecificationAttributeOptionsDictionary[product.Id])
            {
                int specificationAttributeOptionIdLocal = num;
                SpecificationFilterDTO productSpecificationOptionsGroup = ((IEnumerable<SpecificationFilterDTO>)specificationFilterDtosLocal).FirstOrDefault((Func<SpecificationFilterDTO, bool>)((SpecificationFilterDTO x) => x.SelectedFilterIds.Contains(specificationAttributeOptionIdLocal)));
                if (productSpecificationOptionsGroup != null)
                {
                    specificationFilterDtosLocal.Remove(productSpecificationOptionsGroup);
                }
                else
                {
                    foreach (SpecificationFilterDTO specificationFilterDto in specificationFilterDtosLocal)
                    {
                        SpecificationFilterDTO dto = specificationFilterDto;
                        if (dto.IsMain)
                        {
                            return false;
                        }
                        IEnumerable<int> selectedFilterIds = dto.SelectedFilterIds;
                        if (selectedFilterIds.Intersect((await GetSpecificationOptionsDictionaryAsync())[specificationAttributeOptionIdLocal]).Any())
                        {
                            potentiallyOkOptionIds.Add(specificationAttributeOptionIdLocal);
                            productSpecificationOptionsGroup = dto;
                            break;
                        }
                    }
                    if (productSpecificationOptionsGroup != null)
                    {
                        potentiallyOkGroups.Add(productSpecificationOptionsGroup);
                    }
                }
                if (specificationFilterDtosLocal.FirstOrDefault() == null)
                {
                    break;
                }
            }
        }
        bool specificationFiltersAsync;
        if (specificationFilterDtosLocal.FirstOrDefault() == null)
        {
            specificationFiltersAsync = true;
            productSpecificationAttributeOptionsDictionary.TryGetValue(product.Id, out var intList);
            if (intList != null)
            {
                foreach (int specificationOptionId in intList)
                {
                    AddAvailableSpecificationOptionId(specificationOptionId, product.Id);
                }
            }
        }
        else if (specificationFilterDtosLocal.Count == 1 && potentiallyOkGroups.Contains(specificationFilterDtosLocal[0]))
        {
            specificationFiltersAsync = false;
            foreach (int specificationOptionId2 in potentiallyOkOptionIds)
            {
                AddPotentialSpecificationOptionId(specificationOptionId2, product);
            }
        }
        else
        {
            specificationFiltersAsync = false;
        }
        return specificationFiltersAsync;
    }

    private async Task<IDictionary<int, IList<int>>> GetSpecificationOptionsDictionaryAsync()
    {
        if (_shouldRebuildSpecificationOptionsDictionary)
        {
            SpecificationOptionsDictionary = await SpecificationAttributeService7Spikes.GetSpecificationAttributeOptionsDictionaryAsync();
            _shouldRebuildSpecificationOptionsDictionary = false;
        }
        return SpecificationOptionsDictionary;
    }

    private void AddAvailableSpecificationOptionId(int specificationOptionId, int productId)
    {
        if (!AvailableSpecificationAttributeOptionIds.TryGetValue(specificationOptionId, out var productIds))
        {
            productIds = new HashSet<int>();
            AvailableSpecificationAttributeOptionIds.Add(specificationOptionId, productIds);
        }

        productIds.Add(productId);
    }

    private void AddPotentialSpecificationOptionId(int specificationOptionId, Product product)
    {
        if (!PotentiallyAvailableSpecificationOptionIds.ContainsKey(specificationOptionId))
        {
            List<Product> productList = new List<Product> { product };
            PotentiallyAvailableSpecificationOptionIds.Add(specificationOptionId, productList);
            return;
        }
        IList<Product> specificationOptionId2 = PotentiallyAvailableSpecificationOptionIds[specificationOptionId];
        if (!specificationOptionId2.Contains(product))
        {
            specificationOptionId2.Add(product);
        }
    }

    public async Task<IList<int>> GetAvailableSpecificationAttributeOptionsIdsForFilteredProductsAsync(IList<Product> products)
    {
        List<int> specificationAttributeOptionIds = new List<int>();
        specificationAttributeOptionIds.AddRange(PotentiallyAvailableSpecificationOptionIds.Keys);
        var productIds = products.Select(product => product.Id).Distinct().OrderBy(id => id).ToList();
        var filteredProductIds = new HashSet<int>(productIds);

        if (NoSpecificationFiltersSelected && productIds.Count > 0)
        {
            var knownOptionIds = new HashSet<int>(specificationAttributeOptionIds);
            var availableOptions = new Dictionary<int, SpecificationAttributeOption>();

            foreach (var productIdBatch in productIds.Chunk(1000))
            {
                var options = await SpecificationAttributeService7Spikes
                    .GetSpecificationAttributeOptionsByProductIdsAsync(productIdBatch.ToList(), false);

                if (options == null)
                    continue;

                foreach (var option in options)
                {
                    if (!knownOptionIds.Contains(option.Id))
                        availableOptions.TryAdd(option.Id, option);
                }
            }

            foreach (var option in availableOptions.Values.OrderBy(option => option.DisplayOrder))
            {
                if (knownOptionIds.Add(option.Id))
                    specificationAttributeOptionIds.Add(option.Id);
            }
        }
        else
        {
            foreach (var entry in AvailableSpecificationAttributeOptionIds)
            {
                if (!specificationAttributeOptionIds.Contains(entry.Key)
                    && entry.Value.Any(filteredProductIds.Contains))
                {
                    specificationAttributeOptionIds.Add(entry.Key);
                }
            }
        }
        return specificationAttributeOptionIds;
    }
}
