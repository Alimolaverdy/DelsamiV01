using Nop.Data;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class AdvRedirectHistoryService : IAdvRedirectHistoryService
    {

        #region Fields

        private readonly IRepository<AdvRedirectHistory> _advRedirectHistoryRepository;

        #endregion

        #region Ctor

        public AdvRedirectHistoryService(IRepository<AdvRedirectHistory> advRedirectHistoryRepository)
        {
            _advRedirectHistoryRepository = advRedirectHistoryRepository;
        }

        #endregion

        #region  Methods

        public async Task InsertAsync(AdvRedirectHistory advRedirectHistory)
        {
            if (advRedirectHistory == null)
                throw new ArgumentNullException(nameof(advRedirectHistory));

            await _advRedirectHistoryRepository.InsertAsync(advRedirectHistory);
        }

        #endregion

    }
}
