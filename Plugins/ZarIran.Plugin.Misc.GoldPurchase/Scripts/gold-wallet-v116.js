(function () {
    'use strict';
    function init() {
        var page = document.querySelector('.gold-wallet-page');
        if (!page || !page.querySelector('#gold-purchase-form')) return;
        var tabs = Array.from(page.querySelectorAll('[role="tab"]'));
        function activate(tab) {
            tabs.forEach(function (item) {
                var selected = item === tab;
                item.setAttribute('aria-selected', String(selected));
                item.tabIndex = selected ? 0 : -1;
                document.getElementById(item.getAttribute('aria-controls')).hidden = !selected;
            });
            page.querySelector('#gold-sell-payout').hidden = tab.id !== 'gold-sell-tab';
        }
        tabs.forEach(function (tab, index) {
            tab.addEventListener('click', function (e) { e.preventDefault(); activate(tab); });
            tab.addEventListener('keydown', function (e) {
                if (!['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(e.key)) return;
                e.preventDefault();
                var next = e.key === 'Home' ? 0 : e.key === 'End' ? tabs.length - 1 : (index + (e.key === 'ArrowLeft' ? 1 : -1) + tabs.length) % tabs.length;
                activate(tabs[next]); tabs[next].focus();
            });
        });
        var sellPrice = Number(page.dataset.sellPrice), balance = Number(page.dataset.balance);
        var amount = page.querySelector('#gold-buy-amount'), buyWeight = page.querySelector('#gold-buy-weight'), sellWeight = page.querySelector('#gold-sell-weight');
        var quoteBy = page.querySelector('#gold-quote-by');
        var purchaseForm = page.querySelector('#gold-purchase-form'), buyError = page.querySelector('#gold-buy-error');
        var saleAmount = page.querySelector('#gold-sell-amount'), sellQuoteBy = page.querySelector('#gold-sell-quote-by');
        var cashBalance = Number(page.dataset.cashBalance);
        var useCash = page.querySelector('#gold-use-cash'), cashInput = page.querySelector('#gold-cash-amount');
        var money = new Intl.NumberFormat('fa-IR', { maximumFractionDigits: 0 });
        var grams = new Intl.NumberFormat('fa-IR', { maximumFractionDigits: 8 });
        function amountDigits(value) {
            return value.replace(/[۰-۹]/g, function (digit) { return String(digit.charCodeAt(0) - 0x06f0); })
                .replace(/[٠-٩]/g, function (digit) { return String(digit.charCodeAt(0) - 0x0660); })
                .replace(/[\s,٬]/g, '');
        }
        function updateBuy() {
            buyError.hidden = true;
            var digits = amountDigits(amount.value);
            var a = /^[0-9]+$/.test(digits) ? Number(digits) : NaN, w = Number(buyWeight.value);
            var validAmount = a > 0 && a >= Number(amount.dataset.min) && a <= Number(amount.dataset.max);
            amount.setCustomValidity(!amount.value || validAmount ? '' : 'مبلغ خرید را به تومان و در بازه مجاز وارد کنید.');
            var walletShare = 0;
            if (cashInput) {
                var max = Math.floor(Math.min(cashBalance, a || 0));
                cashInput.disabled = !useCash.checked;
                cashInput.max = max;
                if (useCash.checked) walletShare = cashInput.value === '' ? max : Number(cashInput.value);
            }

            var valid = validAmount && walletShare >= 0 && walletShare <= a
                && (!cashInput || cashInput.validity.valid);
            page.querySelector('#gold-buy-preview').textContent = valid ? money.format(a - walletShare) + ' تومان' : '—';
            page.querySelector('#gold-cash-preview').textContent = valid ? money.format(walletShare) + ' تومان' : '—';
            page.querySelector('#gold-buy-grams-preview').textContent = w > 0 && Number.isFinite(w) ? grams.format(w) + ' گرم' : '—';
        }
        // Decimal arithmetic for grams × spot quote; round only the final Toman amount.
        function fraction(text) {
            if (!/^\d*(\.\d*)?$/.test(text) || text === '' || text === '.') return null;
            var parts = text.split('.'), decimals = parts[1] || '';
            return { n: BigInt((parts[0] || '0') + decimals), d: 10n ** BigInt(decimals.length) };
        }
        function quoteAmount(weight, rate, roundUp) {
            var w = fraction(weight), r = fraction(rate);
            if (!w || !r || r.n === 0n) return '';
            var numerator = w.n * r.n, denominator = w.d * r.d;
            return ((numerator + (roundUp ? denominator - 1n : 0n)) / denominator).toString();
        }
        function updateBuyQuote() {
            amount.value = quoteAmount(buyWeight.value, page.dataset.buyPrice, true);
            updateBuy();
        }
        function weightFromAmount(value, rateText, roundUp) {
            var digits = amountDigits(value);
            var budget = /^[0-9]+$/.test(digits) ? fraction(digits) : null, rate = fraction(rateText);
            if (!budget || !rate || rate.n === 0n) return '';
            var numerator = budget.n * rate.d * 100000000n, denominator = budget.d * rate.n;
            var units = (numerator + (roundUp ? denominator - 1n : 0n)) / denominator;
            var text = units.toString().padStart(9, '0');
            return text.slice(0, -8) + '.' + text.slice(-8);
        }
        function updateBuyWeight() {
            buyWeight.value = weightFromAmount(amount.value, page.dataset.buyPrice, false);
            updateBuy();
        }
        buyWeight.addEventListener('input', function () {
            quoteBy.value = 'weight';
            updateBuyQuote();
        });
        amount.addEventListener('input', function () {
            quoteBy.value = 'amount';
            updateBuyWeight();
        });
        if (useCash) {
            useCash.addEventListener('change', updateBuy);
            cashInput.addEventListener('input', updateBuy);
        }
        function updateSell() {
            var w = Number(sellWeight.value), value = Number(quoteAmount(sellWeight.value, page.dataset.sellPrice, false));
            page.querySelector('#gold-sell-preview').textContent = w > 0 && w <= balance && Number.isFinite(value) && value > 0
                ? money.format(value) + ' تومان' : '—';
        }
        function sellByWeight() {
            sellQuoteBy.value = 'weight';
            saleAmount.setCustomValidity('');
            saleAmount.value = quoteAmount(sellWeight.value, page.dataset.sellPrice, false);
            updateSell();
        }
        sellWeight.addEventListener('input', function () {
            if (Number(sellWeight.value) > balance) sellWeight.value = page.dataset.balance;
            sellByWeight();
        });
        saleAmount.addEventListener('input', function () {
            sellQuoteBy.value = 'amount';
            var digits = amountDigits(saleAmount.value);
            if (/^[0-9]+$/.test(digits) && BigInt(digits) > BigInt(saleAmount.dataset.max)) {
                saleAmount.value = saleAmount.dataset.max;
            }
            sellWeight.value = weightFromAmount(saleAmount.value, page.dataset.sellPrice, true);
            var proposed = Number(sellWeight.value);
            saleAmount.setCustomValidity(proposed > 0 && proposed <= balance ? '' :
                'مبلغ دریافتی باید مثبت و در محدوده موجودی طلا باشد.');
            updateSell();
        });
        page.querySelector('#gold-sell-all').addEventListener('click', function () {
            sellWeight.value = page.dataset.balance;
            sellByWeight();
        });
        page.querySelector('#gold-sell-form').addEventListener('submit', function (e) {
            var w = Number(sellWeight.value);
            if (!Number.isFinite(w) || w <= 0 || w > balance || sellPrice <= 0 || Number(quoteAmount(sellWeight.value, page.dataset.sellPrice, false)) <= 0) {
                e.preventDefault(); alert('مقدار فروش معتبر نیست یا موجودی کافی نیست.'); return;
            }
            if (!confirm('فروش ' + grams.format(w) + ' گرم طلا به مبلغ تقریبی ' + money.format(Number(quoteAmount(sellWeight.value, page.dataset.sellPrice, false))) + ' تومان را تأیید می‌کنید؟')) {
                e.preventDefault();
                return;
            }
            e.currentTarget.querySelector('button[type="submit"]').disabled = true;
        });
        purchaseForm.noValidate = true;
        purchaseForm.addEventListener('submit', function (e) {
            var fields = quoteBy.value === 'amount' ? [amount] : [buyWeight, amount];
            if (cashInput && useCash.checked) fields.push(cashInput);
            var invalid = fields.find(function (field) { return !field.validity.valid; });
            if (invalid) {
                e.preventDefault();
                buyError.textContent = invalid.validationMessage;
                buyError.hidden = false;
                invalid.focus();
                return;
            }
            e.currentTarget.querySelector('button[type="submit"]').disabled = true;
        });
        var errors = page.querySelector('#gold-server-errors');
        if (errors && errors.classList.contains('validation-summary-errors')) errors.focus();
        updateBuy(); updateSell();
    }
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
})();
