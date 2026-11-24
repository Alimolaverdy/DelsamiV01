

function conv2EnNum(str) {

    // تبدیل اعداد فارسی و عربی به انگلیسی
    return str.replace(/[\u0660-\u0669]/g, d => d.charCodeAt(0) - 0x0660)
        .replace(/[\u06F0-\u06F9]/g, d => d.charCodeAt(0) - 0x06F0);


}


jQuery(document).ready(function () {

    /* Home Page */
    document.addEventListener('DOMContentLoaded', function () {
        let promotionBarDomReadyInterval = setInterval(function () {
            let promoBar = document.querySelector('.promotion-bar-container');
            let headerLinks = document.querySelector('.header-links-wrapper');

            if (!promoBar || !headerLinks) return; // جلوگیری از خطا اگر پیدا نشدن
            clearInterval(promotionBarDomReadyInterval);

            let promoHeight = promoBar.offsetHeight;

            window.addEventListener('scroll', function () {
                if (window.scrollY > promoHeight) {
                    headerLinks.style.top = '0px';
                } else {
                    headerLinks.style.top = promoHeight + 'px';
                }
            });

        }, 100);

    });


    let promotionBarDomReadyInterval = setInterval(function () {
        var promoBar = jQuery('.promotion-bar-container img');
        var headerLinks = jQuery('.header-links-wrapper');

        if (!promoBar || !headerLinks) return; // جلوگیری از خطا اگر پیدا نشدن

        clearInterval(promotionBarDomReadyInterval);
        var promoHeight = promoBar.outerHeight();

        jQuery(window).on('resize', setMasterWrapperMarginTop);
        jQuery(window).on('resize scroll', stickyHeaderLinks);

        setMasterWrapperMarginTop();
        stickyHeaderLinks();

        function setMasterWrapperMarginTop() {
            promoHeight = promoBar.outerHeight();
            jQuery('.master-wrapper-page').css('marginTop', promoHeight + 'px');
        }
        function stickyHeaderLinks() {

            if (window.innerWidth > 1200) {
                headerLinks.css('top', '0px');
                return;
            }

            if (window.scrollY > promoHeight) {
                headerLinks.css('top', '0px');
            } else {
                headerLinks.css('top', promoHeight + 'px');
            }
        }

    }, 100);



    /* Home Page */

    /* FAQ Page */
    jQuery('.page-body .faq-page .collapsible > h2').next().slideUp();
    jQuery('.page-body .faq-page .collapsible > h2').click(function () {
        jQuery(this).next().slideToggle();
        jQuery(this).toggleClass('open');
    });
    /* FAQ Page */


    /* Orders Page */
    (function () {
        $('.timer-wrap').each(function (i, e) {
            var orderInitialTime = $(e).attr('data-initialtime');

            if (orderInitialTime == undefined) return;

            var time = 60 * 20 - calculateTimeDifferenceMoment(orderInitialTime);/* how long the timer will run (seconds) */

            // عناصر DOM
            var progress = jQuery(e).find('#progress')[0];
            var secondsText = jQuery(e).find('#secondsText')[0];

            // دایره: محیط (circumference)
            var R = 25;
            var C = 2 * Math.PI * R;

            // مقداردهی اولیه stroke-dasharray تا کامل پنهان/نمایان شود
            progress.style.strokeDasharray = `${C} ${C}`;

            var rafId = null;
            var totalDuration = 0; // میلی‌ثانیه
            var endTime = 0;

            function clamp(v, a, b) { return Math.max(a, Math.min(b, v)); }

            // بروزرسانی UI — اجرا با requestAnimationFrame برای smooth بودن
            function formatTime(ms) {
                const totalSec = Math.ceil(ms / 1000);
                const m = Math.floor(totalSec / 60);
                const s = totalSec % 60;
                return `${m}:${s.toString().padStart(2, '0')}`;
            }

            function tick() {
                const now = Date.now();
                const remainingMs = Math.max(0, endTime - now);
                const frac = clamp(remainingMs / totalDuration, 0, 1);

                // بروزرسانی متن (فرمت دقیقه:ثانیه)
                secondsText.textContent = formatTime(remainingMs);

                // بروزرسانی دایره
                const visible = frac * C;
                const offset = C - visible;
                progress.style.strokeDashoffset = offset;

                if (remainingMs > 0) {
                    rafId = requestAnimationFrame(tick);
                }
            }

            // شروع تایمر با "کل مدت" و "زمانی که گذشته"
            function startCountdown(totalMinutes, elapsedMinutes) {
                totalDuration = totalMinutes * 1000;
                const elapsed = elapsedMinutes * 1000;
                const remaining = Math.max(0, totalDuration - elapsed);
                endTime = Date.now() + remaining;

                if (rafId) cancelAnimationFrame(rafId);
                requestAnimationFrame(tick);
            }

            // در دسترس عمومی
            window.svgCountdown = { startCountdown };

            svgCountdown.startCountdown(20 * 60, 20 * 60 - time);

            function calculateTimeDifferenceMoment(dateString) {
                const inputDate = moment.from(dateString, 'fa', 'YYYY/MM/DD HH:mm:ss').locale('en').toString('YYYY/MM/DD HH:mm:ss');

                return moment().diff(inputDate, 'seconds');
            }
        });
    })();
    /* Orders Page */

    /* Change Password Page */
    jQuery('.password-container button.toggle-password').click(function () {
        if (jQuery(this).hasClass('show')) {
            jQuery(this).find('img').attr('src', "/icons/nobparts-icon-pack/Eye-Close.svg");
            jQuery(this).parent().find('input[type="password"').attr('type', 'input');
            jQuery(this).removeClass('show');
        } else {
            jQuery(this).find('img').attr('src', "/icons/nobparts-icon-pack/Eye-Open.svg");
            jQuery(this).parent().find('input[type="input"').attr('type', 'password');
            jQuery(this).addClass('show');
        }
    });
    /* Change Password Page */

    /* Filter Page */

    jQuery('.filtersSearchContainer input').on('keyup', function () {
        let filteredValue = conv2EnNum(jQuery(this).val().toString().toLowerCase());
        if (!filteredValue) {
            jQuery(this).closest('.filter-block').find('ul.checkbox-list li').css('display', 'flex');

            if (jQuery(this).closest('.filter-block').find('ul.checkbox-list li[style*="display: flex"]').length == 0) {
                jQuery(this).closest('.filter-block').find('.no-data').removeClass('d-none');
            } else {
                jQuery(this).closest('.filter-block').find('.no-data').addClass('d-none');
            }

            return;
        }


        jQuery(this).closest('.filter-block').find('ul.checkbox-list li').css('display', 'flex');
        jQuery(this).closest('.filter-block').find('ul.checkbox-list li').filter(function () {
            return jQuery(this).text().toString().toLowerCase().indexOf(filteredValue) === -1;
        }).css('display', 'none');

        if (jQuery(this).closest('.filter-block').find('ul.checkbox-list li[style*="display: flex"]').length == 0) {
            jQuery(this).closest('.filter-block').find('.no-data').removeClass('d-none');
        } else {
            jQuery(this).closest('.filter-block').find('.no-data').addClass('d-none');
        }

    });

    /* Filter Page */


    /* All Profile Pages */

    function toggleProfileMenu() {
        jQuery('.side-2 .block-account-navigation .listbox').toggleClass('open');
    }

    if (jQuery('.page.account-page').length > 0) {
        jQuery('.page.account-page').first().prepend(
            `<div class="toggle-profile-menu">
                <img src="/icons/nobparts-icon-pack/Hamburger-Menu.svg" />
                منوی کاربری
                </div>
            `
        );

        let profileMenuClickable = setInterval(function () {
            if (jQuery('.header-cart-mobile .header-cart-mobile-item').length > 0) {
                jQuery('.header-cart-mobile .header-cart-mobile-item').last().click(function (e) {
                    e.preventDefault();
                    toggleProfileMenu();
                });

                clearInterval(profileMenuClickable);
            }
        }, 100);

        jQuery('.toggle-profile-menu').click(function () {
            toggleProfileMenu();
        });

        jQuery('.side-2').click(function () {
            toggleProfileMenu();
        });

    }
    /* All Profile Pages */

    /* Checkout Page */

    //Sticky
    $(function () {
        var $sticky = $('.order-info-confirm');
        if (!$sticky.length) return;

        var $parent = $sticky.parent();
        var marginTop = 5 * 16; // فاصله از بالای صفحه
        var stickyTop = $sticky.offset().top - marginTop;
        var parentBottom, stickyHeight;
        var windowWidth = $(window).width();
        function updateSticky() {
            windowWidth = $(window).width();
            if (windowWidth < 768) {
                $sticky.css({
                    position: 'relative',
                    top: 'auto',
                    width: 'auto'
                });
                return;
            };

            stickyHeight = $sticky.outerHeight();
            parentBottom = $parent.offset().top + $parent.outerHeight() - stickyHeight;
            var scrollTop = $(window).scrollTop();

            if (scrollTop > stickyTop && scrollTop < parentBottom) {
                $sticky.css({
                    position: 'fixed',
                    top: marginTop + 'px',
                    width: $parent.width()
                });
            } else if (scrollTop >= parentBottom) {
                $sticky.css({
                    position: 'absolute',
                    top: (parentBottom - $parent.offset().top) + 'px',
                    width: $parent.width()
                });
            } else {
                $sticky.css({
                    position: 'static',
                    width: 'auto',
                    top: 'auto'
                });
            }
        }

        $(window).on('scroll resize', updateSticky);
        updateSticky(); // اجرا در لود اولیه
    });

    //Stepping Checkout
    jQuery(function () {

        if (document.body.classList.contains('zar-retail')) return;

        // کنترل دستی جلوگیری از اجرای popstate برای pushState
        let manualStateChange = false;

        // مرحله‌های checkout
        const steps = {
            billing: {
                hash: '#opc-billing',
                show: function () {
                    jQuery('ol#checkout-steps > li#opc-shipping').show();
                    jQuery('ol#checkout-steps > li:not(#opc-shipping)').hide();

                    jQuery('.confirm-order-next-step-button').hide();
                    jQuery('.fake-checkout-button').hide();

                    jQuery('.next-step-button.continue-to-shpping-methods').show();
                    jQuery('.next-step-button:not(.continue-to-shpping-methods)').hide();
                }
            },
            shippingMethod: {
                hash: '#choose-shpping-method',
                show: function () {
                    jQuery('ol#checkout-steps > li:not(#opc-shipping_method)').hide();
                    jQuery('ol#checkout-steps li#opc-shipping_method').show();

                    jQuery('.confirm-order-next-step-button').hide();
                    jQuery('.fake-checkout-button').hide();

                    jQuery('.continue-to-shpping-methods').hide();
                    jQuery('.continue-to-payment-methods').show();

                    // نمایش آدرس انتخاب شده
                    let choosedAddress = jQuery('#opc-shipping div.select-shipping-address label:has(input[type="radio"]:checked)').clone();
                    jQuery(choosedAddress).find('input[type="radio"]').remove();
                    jQuery("ol#checkout-steps li#opc-shipping_method div.choosed-address").html(choosedAddress);
                }
            },
            paymentMethod: {
                hash: '#choose-payment-method',
                show: function () {
                    jQuery('ol#checkout-steps > li:not(#opc-payment_method)').hide();
                    jQuery('ol#checkout-steps li#opc-payment_method').show();

                    jQuery('.continue-to-payment-methods').hide();
                    jQuery('.confirm-order-next-step-button').show();
                    jQuery('.fake-checkout-button').show();
                }
            }
        };

        // ----------------------------
        // تغییر مرحله با pushState
        // ----------------------------
        function goToStep(stepName) {
            const step = steps[stepName];
            if (!step) return;

            manualStateChange = true;
            window.history.pushState(
                { step: stepName },
                '',
                '?step=' + stepName + step.hash
            );
            manualStateChange = false;

            step.show();
        }

        // ----------------------------
        // بک / فوروارد مرورگر
        // ----------------------------
        window.addEventListener('popstate', function (e) {
            if (manualStateChange) return;

            // اگر state اطلاعات مرحله را دارد
            let stepName = e.state?.step;

            if (stepName && steps[stepName]) {
                steps[stepName].show();
                return;
            }

            // اگر state خالی بود، از hash بخوان
            switch (location.hash) {
                case steps.shippingMethod.hash:
                    steps.shippingMethod.show();
                    break;

                case steps.paymentMethod.hash:
                    steps.paymentMethod.show();
                    break;

                default:
                case steps.billing.hash:
                    steps.billing.show();
                    break;
            }
        });

        // ----------------------------
        // کلیک‌ها
        // ----------------------------
        jQuery('.continue-to-shpping-methods').click(function () {
            goToStep('shippingMethod');
        });

        jQuery('.continue-to-payment-methods').click(function () {

            if ($(".myRadioClass:checked").length > 0) {
                console.log("انتخاب شده");
            }


            if (jQuery('input[name="shippingoption"].shipping-method-22:checked').length > 0 || jQuery('input[name="shippingoption"].shipping-method-27:checked').length > 0) {
                if (jQuery('input[name="deliveryDateTimesRadioGroup"]:checked').length == 0) {
                    alert('زمان ارسال انتخاب نشده است');
                    return;
                }
            }
            goToStep('paymentMethod');
        });

        // ----------------------------
        // مرحله اولیه هنگام ورود به صفحه
        // ----------------------------
        if (!location.hash) {
            if (location.pathname !== '/onepagecheckout') return;//lock this part on one page checkout

            manualStateChange = true;
            history.replaceState({ step: 'billing' }, '', '?step=billing#opc-billing');
            manualStateChange = false;
            steps.billing.show();
        } else {
            if (location.pathname !== '/onepagecheckout') return;//lock this part on one page checkout

            // اگر هش موجود است، همان مرحله را نمایش بده
            switch (location.hash) {
                case steps.shippingMethod.hash:
                    steps.shippingMethod.show();
                    break;

                case steps.paymentMethod.hash:
                    steps.paymentMethod.show();
                    break;

                default:
                    steps.billing.show();
            }
        }

    });


    /* Checkout Page */

});

function filterOrders(filter, el) {
    var group = jQuery(el).closest('.filters');
    group.find('.orderstatus').removeClass('active').attr('aria-pressed', 'false');
    jQuery(el).addClass('active').attr('aria-pressed', 'true');

    var page = jQuery(el).closest('.order-list-page');
    if (!page.length) page = jQuery(document);
    var shown = 0;
    page.find('.order-list .order-item').each(function () {
        var matches = !filter || jQuery(this).attr('data-orderstatus') === filter;
        jQuery(this).toggle(matches);
        if (matches) shown++;
    });
    page.find('.order-filter-empty').prop('hidden', shown > 0);
}

/*
  Usage:
  - Include this script near the end of <body> or after DOMContentLoaded.
  - Make sure you have a valid manifest.json and Service Worker registered (recommended).
*/

(function () {
    const installBtn = document.getElementsByClassName('pwa-install-btn')[0];
    const installBtn2 = document.getElementsByClassName('pwa-install-btn')[1];
    const iosHint = document.getElementById('ios-install-hint');

    let deferredPrompt = null;

    // Utility: show/hide

    // Detect iOS (WebApp capable) — because iOS doesn't fire beforeinstallprompt
    const isIos = () => {
        const ua = window.navigator.userAgent;
        return /iP(ad|hone|od)/.test(ua) && !window.MSStream;
    };

    const isInStandaloneMode = () =>
        ('standalone' in window.navigator && window.navigator.standalone) ||
        window.matchMedia('(display-mode: standalone)').matches;

    // Listen for beforeinstallprompt (Chrome, Edge, Opera, Android browsers that support it)
    window.addEventListener('beforeinstallprompt', (e) => {
        // Prevent the mini-infobar from appearing on mobile
        e.preventDefault();
        // Save the event for later prompt
        deferredPrompt = e;
    });


    // Button click -> show prompt
    if (installBtn) {
        installBtn.addEventListener('click', async () => {
            // If we have the saved event, use it.
            if (deferredPrompt) {
                deferredPrompt.prompt();
                // Wait for the user to respond to the prompt
                const { outcome } = await deferredPrompt.userChoice;
                if (outcome === 'accepted') {
                    console.log('[PWA] User accepted the install prompt');
                } else {
                    console.log('[PWA] User dismissed the install prompt');
                }
                // Clear the saved prompt since it can only be used once.
                deferredPrompt = null;
            }
        });
    }

    if (installBtn2) {
        installBtn2.addEventListener('click', async () => {
            // If we have the saved event, use it.
            if (deferredPrompt) {
                deferredPrompt.prompt();
                // Wait for the user to respond to the prompt
                const { outcome } = await deferredPrompt.userChoice;
                if (outcome === 'accepted') {
                    console.log('[PWA] User accepted the install prompt');
                } else {
                    console.log('[PWA] User dismissed the install prompt');
                }
                // Clear the saved prompt since it can only be used once.
                deferredPrompt = null;
            }
        });
    }

    // Listen for successful installation (some browsers fire this)
    window.addEventListener('appinstalled', (evt) => {
        console.log('[PWA] appinstalled event fired:', evt);
        // Optional: give feedback to user
        if (iosHint) iosHint.classList.add('hidden');
        // you can send analytics here
    });


})();

