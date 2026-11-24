# Gold wallet (nopCommerce 4.60)

The existing `/gold-purchase` and `/gold-sell` routes render the same dashboard. The second route opens the sell tab, including after sale validation errors. Forms still post to their existing server actions. Browser estimates never supply the authoritative price or wallet balance.

## Portfolio accounting

All amounts follow the plugin's existing **Toman** convention. Wallet spot price comes only from `CoreSettings.ManualGoldPrice`. Wallet trades do not use the product/jewelry formula, Product.Weight, making fees, profit, AdditionalAmount, tax settings or GoldPriceAdjustment. GoldPurchaseSettings controls activation, amount limits, payment method and buy/sell commissions.

The customer can edit either grams or the whole-Toman purchase amount and choose how much to pay from the monetary wallet. The last edited field determines the quote. For a Toman input, the server keeps the amount and truncates the derived grams to eight decimal places. The server computes `ceil(grams × spot × (1 + BuyCommission/100))` for a purchase and `floor(grams × spot × (1 - SellCommission/100))` for a sale. Round the final money amount, not each gram. Preserve the exact entered grams (up to eight decimals); do not derive them back from the rounded money amount. The server recalculates the derived field at the current rate.
- Total paid is the sum of successful purchase ledger amounts, not pending orders.
- Remaining cost uses moving-average inventory accounting, ordered by UTC timestamp and ledger ID. Sales and administrative deductions remove a proportional share of the remaining cost. Rebuying adds the new actual purchase cost.
- Current value = current grams × market quote, before sell commission.
- Unrealized profit/loss = current value − remaining cost. Sale proceeds are displayed separately; selling gold is not itself treated as an investment loss.
- Administrative credits have no customer payment and therefore zero purchase cost; the dashboard discloses this when relevant.
- An incomplete/inconsistent ledger suppresses the profit/loss figure rather than inventing a cost basis.
- Calculation reads the complete ledger, independently of the paginated admin history.

## Monetary wallet and withdrawals

Gold purchases can use any valid wallet amount up to the purchase total and available balance. Leaving the wallet amount blank applies as much as possible. A zero remainder completes in nopCommerce, and a positive remainder goes to the configured redirect gateway. Cancellation of an unpaid order restores its wallet debit.

A gold sale commits its gold debit, sale ledger entry and exact wallet credit together. The wallet account and the sale tab link to the withdrawal form. The form uses the customer's saved ExtendedCustomer Shebas and validates the Iranian IBAN checksum on new entries. Only unspent gold-sale credit is eligible for withdrawal. Registering a request reserves the amount immediately and snapshots the destination Sheba.

The **درخواست برداشت وجه** submenu requires `ManageOrders`. Its open and completed tabs show the payout amount and customer. The detail page shows the name, phone, customer ID and Sheba. Once the bank transfer has been made, staff confirm it on the detail page. Completion records the acting administrator and time.

## Administration

Menu: **خرید و فروش طلا → مدیریت**. Registered customers can be filtered by email/name. Open a customer for paginated history and signed gram adjustments. The existing customer-edit wallet card uses the same component and adjustment endpoint.

`ManageCustomers` is required on the list, history, wallet component and adjustment endpoint. The controller also requires admin authorization and antiforgery validation. History retains the signed change, resulting balance, timestamp, explanation and acting administrator. Explanations are required. Inputs support Persian/Arabic digits, decimal points/commas and negative values, with at most eight decimal places. Negative adjustments cannot overdraw the wallet. Balance changes and the administrative audit row commit in a single serializable transaction; a failed ledger insert rolls back the change. Existing history is not edited/deleted.

## Deployment

`GoldPurchase` 1.16 depends on `ZarIran.Misc.Core`, `ZarIran.Misc.ExtendedCustomer` and `NopShop.Wallet`. Deploy the **entire GoldPurchase output directory**, including `Views`, `Style`, `Scripts` and DLL, then restart nopCommerce and apply the plugin upgrade. The upgrade creates the withdrawal and order-wallet settlement tables. Earlier sales retain their original settlement state. Verify that the deployed storefront serves `/Plugins/ZarIran.Misc.GoldPurchase/Scripts/gold-wallet-v116.js`. In a customer session, open `/gold-purchase` and confirm that changing the Toman amount updates the weight.

For local testing, stop the running Nop.Web instance, build the GoldPurchase project, then start Nop.Web from the same checkout. The buy/sell dashboard uses `Presentation/Nop.Web/Plugins/ZarIran.Misc.GoldPurchase/Views/GoldTrade.cshtml` in the running web project. The rendered amount field should have `name="purchaseAmount"` and be editable.

## Checks

From the repository root, with .NET 7:

```sh
dotnet build Plugins/ZarIran.Plugin.Misc.Core/ZarIran.Plugin.Misc.Core.csproj -c Release
dotnet build Plugins/ZarIran.Plugin.Misc.ProductManagements/ZarIran.Plugin.Misc.ProductManagements.csproj -c Release
dotnet build Plugins/ZarIran.Plugin.Misc.GoldPurchase/ZarIran.Plugin.Misc.GoldPurchase.csproj -c Release
dotnet run --project Plugins/ZarIran.Plugin.Misc.GoldPurchase/Tests/PortfolioChecks.csproj -c Release
dotnet run --project Plugins/ZarIran.Plugin.Misc.GoldPurchase/Tests/ServerChecks/ServerChecks.csproj -c Release
```

Portfolio checks cover gains/losses, zero balance, partial/full sales, rebuying, administrative adjustments, incomplete history, more than 100 rows, commission and precision. Server checks cover authorization, acting administrator identity, Persian/decimal input, validation and request filters. They do not substitute for SQL Server integration tests.

Before production, verify on the target database: a paid purchase credits once; a sale uses the latest server quote and credits the wallet; an administrative credit/debit produces one history row; an overdraw is rejected; simultaneous deductions cannot create negative balances; an injected ledger-insert failure rolls back the administrative balance change.

## Wallet orders and normal product pricing

Normal products retain the original ProductManagements formula and Product.Weight behavior. Only the configured GoldProductId is handled by GoldCartQuoteProvider: it preserves the spot purchase total through cart and order processing instead of replacing it with a product-formula price. The provider validates the customer, activation, limits and spot rate. No one-gram product-formula quote is used for wallet trading or valuation.

GoldPurchase checks the calculated unit price and gross cart total against the confirmed spot quote, then checks the persisted order total against the gateway remainder. The order stores the exact grams, effective spot-plus-fee quote and total as a snapshot; successful payment credits those grams without recalculating at a later rate. Order details and invoices continue reading persisted order amounts. Existing orders and ledger rows are not rewritten.

Pricing checks compile the actual normal cart service/formula source and verify that the wallet exception leaves ordinary formula pricing intact:

```sh
dotnet run --project Plugins/ZarIran.Plugin.Misc.GoldPurchase/Tests/PricingChecks/PricingChecks.csproj -c Release
```
