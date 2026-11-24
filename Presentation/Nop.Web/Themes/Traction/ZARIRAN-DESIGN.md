# Zar Iran customer storefront

This is a presentation-only adaptation of the supplied `Prototype_Source` for the existing nopCommerce 4.60 Traction theme. No Admin views, controllers, price calculations, payment/shipping providers, database settings, migrations or plugin binaries are changed.

## Apply and review

1. Check out `codex/zariran-storefront-design` and build/publish Nop.Web normally.
2. Deploy the **entire** `Themes/Traction` directory, including `Content/zariran`, the new `_ZarBrand.cshtml` partial, and the changed Razor views. Do not deploy the zip's React/Next.js application.
3. Restart the web application and hard-refresh the browser. The CSS and JS links use content hashes for cache invalidation.
4. Review the homepage, category/search filters, simple/grouped product pages, add-to-cart and wishlist, cart quantity/removal/coupons, checkout addresses/shipping/payment, login, account addresses/orders, footer newsletter, and desktop/mobile menus against a test account before deploying to production.

Only the existing **Traction** theme is restyled. No theme-selection setting is changed. Rolling back the commit restores the old storefront.

## CMS and SevenSpikes integration

- Anywhere Sliders continues rendering `home_page_main_slider`. The supplied hero artwork is used only if that widget returns no content.
- `theme_header_menu` continues using the existing Mega Menu; nop's TopMenu is the fallback.
- The homepage filter form stays in `home_page_search_widget`. Existing Kendo instances and search handlers are preserved when the form is moved into its visual slot.
- All 13 original named homepage widget zones are retained. Existing JCarousel and HtmlWidgets content is rendered from the database, not replaced by the prototype's example products.
- The product slot immediately below categories renders `home_page_before_products`. Assign the `new` JCarousel to this supported zone, set its display Title, and enable ShowTitle. Its own title/products replace the former fixed heading and native homepage product grid. Category, bestseller, footer certificate and body/header widget zones remain available.
- Optional HtmlWidgets slots `zariran_home_editorial` and `zariran_home_gift` override the two supplied editorial/packaging sections when configured. Otherwise, the uploaded prototype artwork and copy are used.
- Original widget-zone names containing automotive terms are kept for compatibility. Update their configured banners/text in the CMS when ready; this commit does not modify database content.
- Catalog/product/cart/account data and permissions remain supplied by nop and the installed ZarIran plugins. Wholesale roles use the same existing permission/pricing logic. The prototype's sample account, prices, product counts, gift fees, order dates and standalone wholesale dashboards are not inserted as fake live data or new backend features.
- The gold-rate strip reads the existing CoreSettings manual rate and adjustment, formats it with the working currency, and is omitted when the rate is non-positive. It does not invent an update timestamp or change the gold-price service.

## Assets and identity

`Content/zariran` includes the supplied hero, editorial, packaging, product collage and original logo-reference image. `_ZarBrand.cshtml` reproduces the supplied source's ZarLogo vector wordmark and sprout symbol; the original logo reference is retained as an asset. Stroke icons are a local SVG sprite, with no third-party CDN dependency. The existing IRANSansX WOFF2 font is reused. Real catalog images always come from the store.

The 4X PNG archive exceeded the attachment tool's 32 MiB transfer limit, so the supplied source and its original image assets were used as the design reference.

## Validation and limits

- Nop.Web builds using .NET SDK 7.0.410. Build verifies Razor views as well as code.
- A separate static browser fixture checks the final theme skin together with the legacy Traction CSS at desktop/tablet/mobile widths, including horizontal overflow, header icon rendering, search focus/Escape, mobile navigation and JavaScript exceptions. This fixture contains sample data solely for visual verification and is **not** shipped.
- Widget-zone preservation and product partial/widget calls are checked against the original templates.
- Full checkout/payment and CMS-populated plugin rendering require the connected application's database and provider configuration. They cannot be exercised by the static fixture; validate these against your own test environment.
