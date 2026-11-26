using Delsami.Plugin.Misc.ProductManagement.Domain;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.ProductManagement.Services
{
    public class GeneratedVirtualProductService : IGeneratedVirtualProductService
    {
        #region Fields

        private readonly IRepository<GeneratedVirtualProduct> _generatedVirtualProductRepo;

        #endregion

        #region Ctor

        public GeneratedVirtualProductService(IRepository<GeneratedVirtualProduct> generatedVirtualProductRepo)
        {
            _generatedVirtualProductRepo = generatedVirtualProductRepo;
        }

        #endregion

        #region Methods

        public async Task InsertAsync(GeneratedVirtualProduct generatedVirtualProduct)
        {
            if (generatedVirtualProduct is null)
                throw new ArgumentNullException(nameof(generatedVirtualProduct));

            await _generatedVirtualProductRepo.InsertAsync(generatedVirtualProduct);
        }

        public async Task<IList<GeneratedVirtualProduct>> GetAllGeneratedVirtualProductsAsync(bool? publish = null)
        {
            var query = _generatedVirtualProductRepo.Table.AsQueryable();

            if (publish.HasValue)
                query = query.Where(x => x.Published == publish);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(GeneratedVirtualProduct generatedVirtualProduct)
        {
            if (generatedVirtualProduct is null)
                throw new ArgumentNullException(nameof(generatedVirtualProduct));

            await _generatedVirtualProductRepo.UpdateAsync(generatedVirtualProduct);
        }

        public async Task<bool> IsExistMainParentProductIdAsync(int mainParentProductId)
        {
            return await _generatedVirtualProductRepo.Table.AnyAsync(a => a.MainParentProductId == mainParentProductId);
        }

        #endregion
    }
}
