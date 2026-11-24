using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopShop.Plugin.Misc.Wallet.Services;

public class ZarIranRewardPointService : RewardPointService, IRewardPointService
{

	public ZarIranRewardPointService(IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, IRepository<RewardPointsHistory> rewardPointsHistoryRepository,
										RewardPointsSettings rewardPointsSettings) : base(dateTimeHelper, localizationService, rewardPointsHistoryRepository, rewardPointsSettings)
	{
	}


	public async override Task<int> AddRewardPointsHistoryEntryAsync(Customer customer,
															int points,
															int storeId,
															string message = "",
															Order usedWithOrder = null,
															decimal usedAmount = 0,
															DateTime? activatingDate = null,
															DateTime? endDate = null)
	{
		if (customer == null)
			throw new ArgumentNullException(nameof(customer));

		if (storeId == 0)
			throw new ArgumentException("Store ID should be valid");

		if (points < 0 && endDate.HasValue)
			throw new ArgumentException("End date is available only for positive points amount");

		points =(int) Math.Round((double)points / 10, 0, MidpointRounding.AwayFromZero) * 10; //Round to nearest number for toman currency

		//insert new history entry
		var newHistoryEntry = new RewardPointsHistory
		{
			CustomerId = customer.Id,
			StoreId = storeId,
			Points = points,
			PointsBalance = activatingDate.HasValue ? null : (int?)(await GetRewardPointsBalanceAsync(customer.Id, storeId) + points),
			UsedAmount = usedAmount,
			Message = message,
			CreatedOnUtc = activatingDate ?? DateTime.UtcNow,
			EndDateUtc = endDate,
			ValidPoints = points > 0 ? (int?)points : null,
			UsedWithOrder = usedWithOrder?.OrderGuid
		};

		await InsertRewardPointsHistoryEntryAsync(newHistoryEntry);

		//reduce valid points of previous entries
		if (points >= 0)
			return newHistoryEntry.Id;

		var withValidPoints = (await GetRewardPointsQueryAsync(customer.Id, storeId))
			.Where(historyEntry => historyEntry.ValidPoints > 0)
			.OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();
		foreach (var historyEntry in withValidPoints)
		{
			points += historyEntry.ValidPoints.Value;
			historyEntry.ValidPoints = Math.Max(points, 0);
			await UpdateRewardPointsHistoryEntryAsync(historyEntry);

			if (points >= 0)
				break;
		}

		return newHistoryEntry.Id;
	}
}
