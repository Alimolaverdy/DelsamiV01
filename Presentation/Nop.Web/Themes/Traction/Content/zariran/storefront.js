/* Presentation only: no price calculations, form submissions or checkout overrides. */
(function () {
    'use strict';
    function init() {
        if (!document.body.classList.contains('zar-retail')) return;
        var menuButton = document.querySelector('.zar-menu-toggle');
        var searchButton = document.querySelector('.zar-search-toggle');
        var menu = document.getElementById('zar-navigation');
        var search = document.getElementById('zar-search-panel');
        function toggleMenu(open) {
            if (!menu || !menuButton) return;
            menu.classList.toggle('zar-nav-open', open);
            menuButton.setAttribute('aria-expanded', String(open));
        }
        function toggleSearch(open) {
            if (!search || !searchButton) return;
            search.hidden = !open;
            searchButton.setAttribute('aria-expanded', String(open));
            if (open) {
                var input = search.querySelector('input[type="text"], input[type="search"]');
                if (input) input.focus();
            }
        }
        if (menuButton) menuButton.addEventListener('click', function () {
            var open = menuButton.getAttribute('aria-expanded') !== 'true';
            toggleMenu(open);
            if (open) toggleSearch(false);
        });
        if (searchButton) searchButton.addEventListener('click', function () {
            var open = searchButton.getAttribute('aria-expanded') !== 'true';
            toggleSearch(open);
            if (open) toggleMenu(false);
        });
        document.addEventListener('keydown', function (event) {
            if (event.key !== 'Escape') return;
            if (searchButton && searchButton.getAttribute('aria-expanded') === 'true') {
                toggleSearch(false);
                searchButton.focus();
            } else if (menuButton && menuButton.getAttribute('aria-expanded') === 'true') {
                toggleMenu(false);
                menuButton.focus();
            }
        });
        // Leave nop's localized link text in the accessibility tree and retain all AJAX IDs.
        var icons = { 'ico-account': 'user', 'ico-login': 'user', 'ico-wishlist': 'heart', 'ico-cart': 'bag' };
        var skinScript = document.querySelector('script[src*="zariran/storefront.js"]');
        var iconBase = skinScript ? new URL('icons.svg', skinScript.src).href : '';
        Object.keys(icons).forEach(function (className) {
            document.querySelectorAll('.zar-header .header-links .' + className).forEach(function (link) {
                if (link.querySelector('.zar-icon')) return;
                link.setAttribute('aria-label', link.textContent.trim());
                var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
                svg.setAttribute('class', 'zar-icon');
                svg.setAttribute('aria-hidden', 'true');
                var use = document.createElementNS('http://www.w3.org/2000/svg', 'use');
                use.setAttribute('href', iconBase + '#' + icons[className]);
                svg.appendChild(use);
                link.appendChild(svg);
            });
        });
        // This checkout presents address, shipping and payment together. Keep its
        // existing confirm handler; hide only the obsolete intermediate shortcuts.
        var checkoutActions = document.querySelector('.order-info-confirm #confirm-order-buttons-container');
        if (checkoutActions) {
            checkoutActions.querySelectorAll('.confirm-order-next-step-button').forEach(function (button) {
                button.hidden = false;
                button.style.removeProperty('display');
            });
        }
        // Plugin partials can be returned by AJAX from an older deployed copy.
        // Repair presentation there too, without changing values or save handlers.
        function repairCheckoutPresentation() {
            var checkout = document.querySelector('.page.checkout-page');
            if (!checkout) return;
            checkout.querySelectorAll('.shipping-addresses a[href*="addressadd"]').forEach(function (link) {
                if (link.querySelector('img')) link.textContent = '+ افزودن آدرس جدید';
            });
            checkout.querySelectorAll('.shipping-addresses a[href*="addressedit"]').forEach(function (link) {
                if (link.querySelector('img')) link.textContent = 'ویرایش آدرس';
            });
            checkout.querySelectorAll('.shipping-method .method-fee').forEach(function (fee) {
                if (/^(shipping\.shippingfee\.sorent|پس‌کرایه)$/.test(fee.textContent.trim())) fee.remove();
            });
        }
        repairCheckoutPresentation();
        var checkoutSteps = document.querySelector('.checkout-page #checkout-steps');
        if (checkoutSteps) new MutationObserver(repairCheckoutPresentation).observe(checkoutSteps, { childList: true, subtree: true });
        // The legacy filter widget repositions its form. Keep it in the designed slot
        // without rebuilding it, so Kendo instances, tokens and change handlers survive.
        var filter = document.getElementById('AjaxFiltersProForm');
        var slot = document.querySelector('.zar-home-search');
        function placeFilters() {
            if (filter && slot && filter.parentElement !== slot) slot.appendChild(filter);
        }
        placeFilters();
        window.addEventListener('resize', placeFilters);
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
    else init();
})();
