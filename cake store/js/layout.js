/* ==========================================================
   Cake Shop — ümumi layout: header, footer, mobil menyu, axtarış,
   istifadəçi menyusu, səbət sayğacı və məhsul kartı.
   Asılılıq: js/api.js
   ========================================================== */
'use strict';

const NAV = [
    { id: 'home', href: 'index.html', label: 'Ana səhifə' },
    { id: 'shop', href: 'shop.html', label: 'Mağaza' },
    { id: 'about', href: 'about.html', label: 'Haqqımızda' },
    { id: 'blog', href: 'blog.html', label: 'Bloq' },
    { id: 'contact', href: 'contact.html', label: 'Əlaqə' }
];

const PAGE = document.body.dataset.page || '';

/* ---------------- Ümumi məlumat keşi və səbət ---------------- */

const Store = {
    _products: null,
    _categories: null,
    _settings: null,

    products() {
        if (!this._products) {
            this._products = Api.products().then(asArray).catch(err => { this._products = null; throw err; });
        }
        return this._products;
    },
    categories() {
        if (!this._categories) {
            this._categories = Api.categories().then(asArray).catch(err => { this._categories = null; throw err; });
        }
        return this._categories;
    },
    settings() {
        if (!this._settings) {
            this._settings = Api.settings().then(d => asArray(d)[0] || null).catch(() => null);
        }
        return this._settings;
    },

    // Səbət hələ yaradılmayıbsa backend 404 qaytarır — boş səbət kimi qəbul edirik
    async basketItems() {
        const user = Auth.user();
        if (!user) return [];
        try {
            const basket = await Api.getBasket(user.id);
            return (basket && basket.basketProducts) || [];
        } catch (err) {
            if (err.status === 404) return [];
            throw err;
        }
    },
    async wishlistItems() {
        const user = Auth.user();
        if (!user) return [];
        try {
            const w = await Api.getWishlist(user.id);
            return (w && w.products) || [];
        } catch (err) {
            if (err.status === 404) return [];
            throw err;
        }
    },

    async refreshCartBadge() {
        const badges = qsa('[data-cart-count]');
        try {
            const items = await this.basketItems();
            const count = items.reduce((s, i) => s + i.quantity, 0);
            badges.forEach(b => { b.textContent = count || ''; b.dataset.count = count; });
        } catch { /* sayğac vacib deyil */ }
    },

    async addToCart(productId, qty = 1) {
        if (!Auth.requireLogin()) return false;
        const user = Auth.user();
        try {
            await Api.addToBasket(user.id, productId);
            for (let i = 1; i < qty; i++) await Api.increaseQty(user.id, productId);
            toast('Səbətə əlavə olundu');
            this.refreshCartBadge();
            document.dispatchEvent(new CustomEvent('cart:changed'));
            return true;
        } catch (err) {
            toast(err.message, 'error');
            return false;
        }
    },

    async addToWishlist(productId, btn) {
        if (!Auth.requireLogin()) return false;
        try {
            await Api.addToWishlist(Auth.user().id, productId);
            if (btn) setWishActive(btn, true);
            toast('Seçilmişlərə əlavə olundu');
            return true;
        } catch (err) {
            if (/already|artıq/i.test(err.message)) {
                if (btn) setWishActive(btn, true);
                toast('Artıq seçilmişlərinizdədir', 'info');
            } else {
                toast(err.message, 'error');
            }
            return false;
        }
    }
};

function setWishActive(btn, active) {
    btn.classList.toggle('is-active', active);
    const icon = btn.querySelector('.fa');
    if (icon) { icon.classList.toggle('fa-heart', active); icon.classList.toggle('fa-heart-o', !active); }
}

/* ---------------- Ümumi UI parçaları ---------------- */

const UI = {
    productCard(p) {
        const img = imageUrl(p.images && p.images[0]);
        return `
            <article class="product-card">
                <a href="product.html?id=${p.id}" class="product-card__media" aria-label="${esc(p.name)}">
                    <img src="${esc(img)}" alt="${esc(p.name)}" loading="lazy">
                    ${p.categoryName ? `<span class="chip">${esc(p.categoryName)}</span>` : ''}
                </a>
                <button class="wish-btn" data-wish="${p.id}" aria-label="Seçilmişlərə əlavə et"><i class="fa fa-heart-o"></i></button>
                <div class="product-card__body">
                    <h3 class="product-card__title"><a href="product.html?id=${p.id}">${esc(p.name)}</a></h3>
                    <div class="product-card__foot">
                        <span class="price">${money(p.price)}</span>
                        <button class="add-btn" data-add-cart="${p.id}" aria-label="${esc(p.name)} — səbətə at"><i class="fa fa-shopping-bag"></i></button>
                    </div>
                </div>
            </article>`;
    },
    skeletons(n) {
        return Array.from({ length: n }, () => '<div class="skeleton skeleton--card"></div>').join('');
    },
    empty(icon, title, text, action) {
        return `<div class="empty-state">
                    <i class="fa ${icon}" aria-hidden="true"></i>
                    <h3>${title}</h3>
                    <p>${text}</p>
                    ${action || ''}
                </div>`;
    },
    initials(name) {
        return String(name || '?').split(/\s+/).filter(Boolean).slice(0, 2).map(s => s[0].toUpperCase()).join('');
    }
};

/* ---------------- Header ---------------- */

function renderHeader() {
    const user = Auth.user();
    const links = NAV.map(n => `<a href="${n.href}" class="${n.id === PAGE ? 'is-active' : ''}">${n.label}</a>`).join('');

    const account = user
        ? `<div class="user-menu" data-user-menu>
               <button class="user-menu__btn" aria-haspopup="true" aria-expanded="false">
                   <span class="avatar">${esc(UI.initials(user.fullName))}</span>
                   <span>${esc(String(user.fullName).split(' ')[0])}</span>
                   <i class="fa fa-angle-down" aria-hidden="true"></i>
               </button>
               <div class="user-menu__list" role="menu">
                   <div class="user-menu__head"><strong>${esc(user.fullName)}</strong><small>@${esc(user.username)}</small></div>
                   ${user.isAdmin ? '<a href="admin/index.html" role="menuitem"><i class="fa fa-dashboard"></i> Admin panel</a>' : ''}
                   <a href="orders.html" role="menuitem"><i class="fa fa-file-text-o"></i> Sifarişlərim</a>
                   <a href="wisslist.html" role="menuitem"><i class="fa fa-heart-o"></i> Seçilmişlər</a>
                   <a href="shoping-cart.html" role="menuitem"><i class="fa fa-shopping-bag"></i> Səbət</a>
                   <button data-logout role="menuitem"><i class="fa fa-sign-out"></i> Çıxış</button>
               </div>
           </div>`
        : `<a href="login.html" class="btn btn--dark btn--sm signin-link">Daxil ol</a>`;

    const drawerAuth = user
        ? `${user.isAdmin ? '<a href="admin/index.html" class="btn btn--soft btn--block">Admin panel</a>' : ''}
           <a href="orders.html" class="btn btn--ghost btn--block">Sifarişlərim</a>
           <button class="btn btn--dark btn--block" data-logout>Çıxış</button>`
        : `<a href="login.html" class="btn btn--dark btn--block">Daxil ol</a>
           <a href="register.html" class="btn btn--ghost btn--block">Hesab yarat</a>`;

    const header = document.createElement('div');
    header.innerHTML = `
        <header class="site-header" id="siteHeader">
            <div class="container header-inner">
                <a href="index.html" class="logo" aria-label="Cake Shop ana səhifə">
                    <span class="logo__mark"><i class="fa fa-birthday-cake" aria-hidden="true"></i></span>
                    <span>Cake<em>.</em></span>
                </a>
                <nav class="main-nav" aria-label="Əsas menyu">${links}</nav>
                <div class="header-actions">
                    <button class="icon-btn" data-open-search aria-label="Axtarış"><i class="fa fa-search"></i></button>
                    <a class="icon-btn" href="wisslist.html" aria-label="Seçilmişlər"><i class="fa fa-heart-o"></i></a>
                    <a class="icon-btn" href="shoping-cart.html" aria-label="Səbət">
                        <i class="fa fa-shopping-bag"></i><span class="badge" data-cart-count></span>
                    </a>
                    ${account}
                    <button class="icon-btn menu-toggle" data-open-drawer aria-label="Menyunu aç"><i class="fa fa-bars"></i></button>
                </div>
            </div>
        </header>
        <div class="drawer-backdrop" data-close-drawer></div>
        <aside class="drawer" aria-label="Mobil menyu">
            <div class="drawer__head">
                <a href="index.html" class="logo"><span class="logo__mark"><i class="fa fa-birthday-cake"></i></span><span>Cake<em>.</em></span></a>
                <button class="icon-btn" data-close-drawer aria-label="Menyunu bağla"><i class="fa fa-times"></i></button>
            </div>
            ${user ? `<p class="muted">Salam, <strong>${esc(user.fullName)}</strong></p>` : ''}
            <nav>${links}</nav>
            <div class="drawer__auth">${drawerAuth}</div>
        </aside>
        <div class="search-overlay" role="dialog" aria-label="Məhsul axtar">
            <button class="icon-btn search-overlay__close" data-close-search aria-label="Axtarışı bağla"><i class="fa fa-times"></i></button>
            <form data-search-form>
                <label for="globalSearch" class="sr-only">Məhsul axtar</label>
                <input id="globalSearch" type="search" placeholder="Tort axtar..." autocomplete="off">
                <p class="hint">Axtarmaq üçün Enter, bağlamaq üçün Esc</p>
            </form>
        </div>`;
    document.body.prepend(...header.childNodes);
}

/* ---------------- Footer ---------------- */

function renderFooter() {
    const footer = document.createElement('footer');
    footer.className = 'site-footer';
    footer.innerHTML = `
        <div class="container">
            <div class="footer-grid">
                <div>
                    <a href="index.html" class="logo"><span class="logo__mark"><i class="fa fa-birthday-cake"></i></span><span>Cake<em>.</em></span></a>
                    <p>Hər səhər təbii kərə yağı, kənd yumurtası və bol sevgi ilə təzə bişirilən əl işi tortlar, kekslər və şirniyyatlar.</p>
                    <div class="footer-social" data-footer-social></div>
                </div>
                <div>
                    <h4>Mağaza</h4>
                    <div class="footer-links" data-footer-categories><a href="shop.html">Bütün məhsullar</a></div>
                </div>
                <div>
                    <h4>Hesab</h4>
                    <div class="footer-links">
                        <a href="orders.html">Sifarişlərim</a>
                        <a href="wisslist.html">Seçilmişlər</a>
                        <a href="shoping-cart.html">Səbət</a>
                        <a href="contact.html">Bizimlə əlaqə</a>
                    </div>
                </div>
                <div>
                    <h4>Abunəlik</h4>
                    <p>Yeni dadlar, mövsümi təkliflər və endirimlər — birbaşa e-poçtunuza.</p>
                    <form class="footer-news" data-subscribe-form>
                        <label for="footerEmail" class="sr-only">E-poçt</label>
                        <input id="footerEmail" type="email" placeholder="E-poçtunuz" required>
                        <button type="submit" aria-label="Abunə ol"><i class="fa fa-paper-plane"></i></button>
                    </form>
                </div>
            </div>
            <div class="footer-bottom">
                <span>© ${new Date().getFullYear()} Cake Shop. Bütün hüquqlar qorunur.</span>
                <span data-footer-phone></span>
            </div>
        </div>`;
    document.body.appendChild(footer);

    Store.categories().then(cats => {
        const box = qs('[data-footer-categories]');
        if (box) box.innerHTML = '<a href="shop.html">Bütün məhsullar</a>' +
            cats.slice(0, 5).map(c => `<a href="shop.html?category=${encodeURIComponent(c.name)}">${esc(c.name.charAt(0).toUpperCase() + c.name.slice(1).toLowerCase())}</a>`).join('');
    }).catch(() => {});

    Store.settings().then(s => {
        if (s && s.phone) {
            const phone = s.phone.trim();
            qs('[data-footer-phone]').innerHTML = `<i class="fa fa-phone"></i> <a href="tel:${esc(phone.replace(/\s+/g, ''))}">${esc(phone)}</a>`;
        }
    });

    Api.instagrams().then(d => {
        const insta = asArray(d)[0];
        const box = qs('[data-footer-social]');
        if (insta && box) {
            const handle = String(insta.socialName || '').replace(/^@/, '');
            box.innerHTML = `<a href="https://instagram.com/${encodeURIComponent(handle)}" target="_blank" rel="noopener" aria-label="Instagram"><i class="fa fa-instagram"></i></a>`;
        }
    }).catch(() => {});
}

/* ---------------- Davranışlar ---------------- */

function bindLayout() {
    const header = qs('#siteHeader');
    const onScroll = () => header && header.classList.toggle('is-scrolled', window.scrollY > 8);
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();

    // Mobil menyu
    qsa('[data-open-drawer]').forEach(b => b.addEventListener('click', () => document.body.classList.add('drawer-open')));
    qsa('[data-close-drawer]').forEach(b => b.addEventListener('click', () => document.body.classList.remove('drawer-open')));

    // Axtarış
    const overlay = qs('.search-overlay');
    const input = qs('#globalSearch');
    const openSearch = () => { overlay.classList.add('is-open'); setTimeout(() => input.focus(), 50); };
    const closeSearch = () => overlay.classList.remove('is-open');
    qsa('[data-open-search]').forEach(b => b.addEventListener('click', openSearch));
    qsa('[data-close-search]').forEach(b => b.addEventListener('click', closeSearch));
    qs('[data-search-form]').addEventListener('submit', e => {
        e.preventDefault();
        const q = input.value.trim();
        if (q) location.href = `shop.html?search=${encodeURIComponent(q)}`;
    });
    document.addEventListener('keydown', e => {
        if (e.key === 'Escape') { closeSearch(); document.body.classList.remove('drawer-open'); closeUserMenu(); }
    });

    // İstifadəçi menyusu
    const menu = qs('[data-user-menu]');
    function closeUserMenu() {
        if (!menu) return;
        menu.classList.remove('is-open');
        menu.querySelector('.user-menu__btn').setAttribute('aria-expanded', 'false');
    }
    if (menu) {
        const btn = menu.querySelector('.user-menu__btn');
        btn.addEventListener('click', e => {
            e.stopPropagation();
            const open = menu.classList.toggle('is-open');
            btn.setAttribute('aria-expanded', String(open));
        });
        document.addEventListener('click', e => { if (!menu.contains(e.target)) closeUserMenu(); });
    }

    // Çıxış, səbətə əlavə, istək siyahısı — bütün səhifələrdə
    document.addEventListener('click', async e => {
        const logout = e.target.closest('[data-logout]');
        if (logout) { e.preventDefault(); Auth.logout(); return; }

        const add = e.target.closest('[data-add-cart]');
        if (add) {
            e.preventDefault();
            add.disabled = true;
            await Store.addToCart(Number(add.dataset.addCart));
            add.disabled = false;
            return;
        }

        const wish = e.target.closest('[data-wish]');
        if (wish) {
            e.preventDefault();
            Store.addToWishlist(Number(wish.dataset.wish), wish);
        }
    });

    // Abunəlik formaları (footer + ana səhifə)
    document.addEventListener('submit', async e => {
        const form = e.target.closest('[data-subscribe-form]');
        if (!form) return;
        e.preventDefault();
        const input = form.querySelector('input[type="email"]');
        const email = input.value.trim();
        if (!/^\S+@\S+\.\S+$/.test(email)) return toast('Düzgün e-poçt ünvanı yazın.', 'error');
        const btn = form.querySelector('button');
        btn.disabled = true;
        try {
            await Api.subscribe(email);
            toast('Abunə olduğunuz üçün təşəkkürlər!');
            input.value = '';
        } catch (err) {
            toast(err.message, 'error');
        } finally {
            btn.disabled = false;
        }
    });
}

// Wishlist-dəki məhsulların ürək ikonlarını aktiv göstərir
async function markWishlisted(root = document) {
    if (!Auth.isLoggedIn()) return;
    try {
        const ids = new Set((await Store.wishlistItems()).map(p => p.productId));
        qsa('[data-wish]', root).forEach(b => setWishActive(b, ids.has(Number(b.dataset.wish))));
    } catch { /* vacib deyil */ }
}

renderHeader();
renderFooter();
bindLayout();
Store.refreshCartBadge();
