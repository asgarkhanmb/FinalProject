/* ==========================================================
   Cake Shop — səhifə modulları (body[data-page] ilə seçilir)
   Asılılıq: js/api.js, js/layout.js
   ========================================================== */
'use strict';

const Pages = {};

const FALLBACK = {
    hero: 'img/hero/hero-1.jpg',
    about: 'img/about-video.jpg',
    avatar: 'img/testimonial/ta-1.jpg'
};

function logError(where, err) { console.error(`[${where}]`, err); }

/* ---------------- Paylaşılan bölmələr ---------------- */

async function renderProductsInto(el, products) {
    if (!products.length) {
        el.innerHTML = UI.empty('fa-search', 'Tort tapılmadı', 'Başqa kateqoriya və ya axtarış sözü yoxlayın.',
            '<a href="shop.html" class="btn btn--dark">Bütün məhsullara bax</a>');
        return;
    }
    el.innerHTML = products.map(UI.productCard).join('');
    markWishlisted(el);
}

async function renderCategoryCards(el) {
    if (!el) return;
    try {
        const [cats, products] = await Promise.all([Store.categories(), Store.products().catch(() => [])]);
        el.innerHTML = cats.map(c => {
            const count = products.filter(p => String(p.categoryName).toLowerCase() === c.name.toLowerCase()).length;
            return `<a class="cat-card" href="shop.html?category=${encodeURIComponent(c.name)}">
                        <img src="${esc(imageUrl(c.icon))}" alt="" loading="lazy">
                        <h3>${esc(c.name.toLowerCase())}</h3>
                        <small>${count} məhsul</small>
                    </a>`;
        }).join('');
    } catch (err) { logError('categories', err); el.innerHTML = ''; }
}

async function renderAboutBlock(root) {
    if (!root) return;
    try {
        const about = asArray(await Api.abouts())[0];
        if (!about) return;
        qs('[data-about-title]', root).textContent = about.title;
        qs('[data-about-desc]', root).textContent = about.description;
        if (about.image) qs('[data-about-img]', root).src = imageUrl(about.image);
    } catch (err) { logError('about', err); }
}

function socialIcon(name) {
    const n = String(name || '').toLowerCase();
    if (n.includes('youtube')) return 'fa-youtube-play';
    const known = ['facebook', 'twitter', 'instagram', 'linkedin'].find(s => n.includes(s));
    return known ? `fa-${known}` : 'fa-link';
}

async function renderTeam(el) {
    if (!el) return;
    try {
        const teams = asArray(await Api.teams());
        el.innerHTML = teams.map(t => {
            const socials = (t.socials || []).map(s => {
                const href = /^https?:\/\//i.test(s.url) ? s.url : `https://${s.url}`;
                return `<a href="${esc(href)}" target="_blank" rel="noopener" aria-label="${esc(s.name)}"><i class="fa ${socialIcon(s.name)}"></i></a>`;
            }).join('');
            return `<div class="team-card">
                        <div class="team-card__img">
                            <img src="${esc(imageUrl(t.image))}" alt="${esc(t.fullName)}" loading="lazy">
                            ${socials ? `<div class="team-card__social">${socials}</div>` : ''}
                        </div>
                        <h3>${esc(t.fullName)}</h3>
                        <span>${esc(t.position)}</span>
                    </div>`;
        }).join('');
    } catch (err) { logError('team', err); }
}

async function renderTestimonials(el) {
    if (!el) return;
    try {
        const items = asArray(await Api.testimonials());
        el.innerHTML = items.map(t => `
            <figure class="quote">
                <div class="stars" aria-label="5 ulduz">★★★★★</div>
                <p>“${esc(String(t.description).replace(/\s+/g, ' ').trim())}”</p>
                <figcaption class="quote__author">
                    <img src="${esc(imageUrl(t.image, FALLBACK.avatar))}" alt="" loading="lazy">
                    <div><strong>${esc(String(t.fullName).toLowerCase())}</strong><small>${esc(t.city)}</small></div>
                </figcaption>
            </figure>`).join('');
    } catch (err) { logError('testimonials', err); }
}

/* ==========================================================
   HOME
   ========================================================== */
Pages.home = async function () {
    // Hero slider
    const hero = qs('[data-hero]');
    try {
        const slides = asArray(await Api.sliders());
        if (slides.length) {
            const title = qs('[data-hero-title]', hero);
            const desc = qs('[data-hero-desc]', hero);
            const img = qs('[data-hero-img]', hero);
            const dots = qs('[data-hero-dots]', hero);
            let i = 0, timer;
            slides.forEach(s => { const pre = new Image(); pre.src = imageUrl(s.image, FALLBACK.hero); });

            const show = (n) => {
                i = (n + slides.length) % slides.length;
                const s = slides[i];
                title.innerHTML = esc(s.title).replace(/(\S+)$/, '<em>$1</em>');
                desc.textContent = s.description;
                img.style.backgroundImage = `url("${imageUrl(s.image, FALLBACK.hero)}")`;
                [title, desc].forEach(el => { el.classList.remove('hero__fade'); void el.offsetWidth; el.classList.add('hero__fade'); });
                qsa('button', dots).forEach((d, k) => d.classList.toggle('is-active', k === i));
            };
            const restart = () => { clearInterval(timer); if (slides.length > 1) timer = setInterval(() => show(i + 1), 6000); };

            dots.innerHTML = slides.length > 1
                ? slides.map((_, k) => `<button aria-label="Slayd ${k + 1}"></button>`).join('') : '';
            qsa('button', dots).forEach((d, k) => d.addEventListener('click', () => { show(k); restart(); }));
            show(0);
            restart();
        }
    } catch (err) { logError('slider', err); }

    renderCategoryCards(qs('[data-categories]'));

    const grid = qs('[data-featured]');
    grid.innerHTML = UI.skeletons(4);
    Store.products()
        .then(list => renderProductsInto(grid, list.slice(0, 8)))
        .catch(err => { grid.innerHTML = UI.empty('fa-plug', 'Məhsullar yüklənmədi', esc(err.message)); });

    renderAboutBlock(qs('[data-about]'));
    renderTeam(qs('[data-team]'));
    renderTestimonials(qs('[data-testimonials]'));

    // Instagram
    try {
        const insta = asArray(await Api.instagrams())[0];
        const box = qs('[data-instagram]');
        if (insta && box) {
            const handle = String(insta.socialName || '').replace(/^@/, '');
            const href = `https://instagram.com/${encodeURIComponent(handle)}`;
            qs('[data-insta-title]').textContent = insta.title;
            const link = qs('[data-insta-handle]');
            link.textContent = '@' + handle;
            link.href = href;
            box.innerHTML = (insta.images || []).slice(0, 6).map(img =>
                `<a href="${href}" target="_blank" rel="noopener" aria-label="Instagram-da aç"><img src="${esc(imageUrl(img))}" alt="" loading="lazy"></a>`).join('');
        }
    } catch (err) { logError('instagram', err); }
};

/* ==========================================================
   ABOUT
   ========================================================== */
Pages.about = function () {
    renderAboutBlock(qs('[data-about]'));
    renderTeam(qs('[data-team]'));
    renderTestimonials(qs('[data-testimonials]'));
};

/* ==========================================================
   SHOP
   ========================================================== */
Pages.shop = async function () {
    const take = 8;
    const params = new URLSearchParams(location.search);
    const state = { category: params.get('category') || '', search: params.get('search') || '', sort: '', page: 1 };

    const grid = qs('[data-products]');
    const chips = qs('[data-chips]');
    const searchInput = qs('#shopSearch');
    const sortSelect = qs('#shopSort');
    const count = qs('[data-count]');
    const pager = qs('[data-pagination]');
    searchInput.value = state.search;

    try {
        const cats = await Store.categories();
        chips.innerHTML = `<button class="chip-btn" data-cat="">Hamısı</button>` +
            cats.map(c => `<button class="chip-btn" data-cat="${esc(c.name)}">${esc(c.name.toLowerCase())}</button>`).join('');
    } catch (err) { logError('shop categories', err); }

    const syncChips = () => qsa('[data-cat]', chips).forEach(b =>
        b.classList.toggle('is-active', b.dataset.cat.toLowerCase() === state.category.toLowerCase()));

    const updateUrl = () => {
        const p = new URLSearchParams();
        if (state.category) p.set('category', state.category);
        if (state.search) p.set('search', state.search);
        history.replaceState(null, '', 'shop.html' + (p.toString() ? '?' + p : ''));
    };

    function clientSort(list) {
        if (!state.sort) return list;
        const [key, dir] = state.sort.split('|');
        const sorted = [...list].sort((a, b) => key === 'price' ? a.price - b.price : String(a.name).localeCompare(String(b.name)));
        return dir === 'desc' ? sorted.reverse() : sorted;
    }

    function renderPager(total) {
        if (total <= 1) { pager.innerHTML = ''; return; }
        let html = '';
        if (state.page > 1) html += `<button data-page="${state.page - 1}" aria-label="Əvvəlki"><i class="fa fa-angle-left"></i></button>`;
        for (let n = 1; n <= total; n++) html += `<button data-page="${n}" class="${n === state.page ? 'is-active' : ''}">${n}</button>`;
        if (state.page < total) html += `<button data-page="${state.page + 1}" aria-label="Növbəti"><i class="fa fa-angle-right"></i></button>`;
        pager.innerHTML = html;
    }

    async function load() {
        syncChips();
        updateUrl();
        grid.innerHTML = UI.skeletons(take);
        try {
            let items, totalPage, total;
            if (!state.category && !state.search && !state.sort) {
                // Filtr yoxdursa — backend səhifələməsi
                const res = await Api.productsPaged(state.page, take);
                items = asArray(res.datas);
                totalPage = res.totalPage;
                total = (await Store.products()).length;
            } else {
                let list;
                if (state.search) {
                    try { list = asArray(await Api.searchProducts(state.search)); }
                    catch (err) { if (err.status === 404) list = []; else throw err; }
                } else if (state.category) {
                    list = asArray(await Api.filterProducts(state.category));
                } else {
                    const [key, dir] = state.sort.split('|');
                    list = asArray(await Api.sortProducts(key, dir === 'desc'));
                }
                if (state.search && state.category) {
                    list = list.filter(p => String(p.categoryName).toLowerCase() === state.category.toLowerCase());
                }
                if (state.search || state.category) list = clientSort(list);
                total = list.length;
                totalPage = Math.ceil(total / take);
                items = list.slice((state.page - 1) * take, state.page * take);
            }
            count.textContent = `${total} məhsul` +
                (state.search ? ` — “${state.search}” üzrə` : '') +
                (state.category ? ` — ${state.category.toLowerCase()} kateqoriyasında` : '');
            renderProductsInto(grid, items);
            renderPager(totalPage);
        } catch (err) {
            logError('shop', err);
            grid.innerHTML = UI.empty('fa-plug', 'Məhsullar yüklənmədi', esc(err.message));
            pager.innerHTML = '';
            count.textContent = '';
        }
    }

    chips.addEventListener('click', e => {
        const b = e.target.closest('[data-cat]');
        if (!b) return;
        state.category = b.dataset.cat;
        state.page = 1;
        load();
    });
    sortSelect.addEventListener('change', () => { state.sort = sortSelect.value; state.page = 1; load(); });
    qs('[data-shop-search]').addEventListener('submit', e => {
        e.preventDefault();
        state.search = searchInput.value.trim();
        state.page = 1;
        load();
    });
    pager.addEventListener('click', e => {
        const b = e.target.closest('[data-page]');
        if (!b) return;
        state.page = Number(b.dataset.page);
        load();
        window.scrollTo({ top: grid.getBoundingClientRect().top + window.scrollY - 140, behavior: 'smooth' });
    });

    load();
};

/* ==========================================================
   PRODUCT DETAIL
   ========================================================== */
Pages.product = async function () {
    const id = Number(new URLSearchParams(location.search).get('id'));
    const root = qs('[data-product]');
    if (!id) { root.innerHTML = UI.empty('fa-question-circle', 'Məhsul tapılmadı', 'Belə məhsul yoxdur.', '<a href="shop.html" class="btn btn--dark">Mağazaya qayıt</a>'); return; }

    try {
        const p = await Api.product(id);
        document.title = `${p.name} | Cake Shop`;
        qs('[data-crumb]').textContent = p.name;
        const images = (p.images && p.images.length ? p.images : [null]).map(i => imageUrl(i));
        let qty = 1;

        root.innerHTML = `
            <div class="gallery">
                <div class="gallery__main"><img src="${esc(images[0])}" alt="${esc(p.name)}" data-main-img></div>
                ${images.length > 1 ? `<div class="gallery__thumbs">${images.map((src, k) =>
                    `<button class="${k === 0 ? 'is-active' : ''}" data-thumb="${esc(src)}" aria-label="Şəkil ${k + 1}"><img src="${esc(src)}" alt=""></button>`).join('')}</div>` : ''}
            </div>
            <div class="product-info">
                <a href="shop.html?category=${encodeURIComponent(p.categoryName || '')}" class="chip">${esc(p.categoryName)}</a>
                <h1>${esc(p.name)}</h1>
                <div class="price">${money(p.price)}</div>
                <p class="lead">Sifarişlə, mətbəximizdə təbii kərə yağı, Belçika şokoladı və mövsümi meyvələrlə təzə bişirilir. Ad günləri, bayramlar və ya sadəcə özünüzü əzizləmək üçün idealdır.</p>
                <div class="buy-row">
                    <div class="qty" aria-label="Miqdar">
                        <button type="button" data-qty="-1" aria-label="Azalt">−</button>
                        <span data-qty-value>1</span>
                        <button type="button" data-qty="1" aria-label="Artır">+</button>
                    </div>
                    <button class="btn btn--primary" data-buy><i class="fa fa-shopping-bag"></i> Səbətə at</button>
                    <button class="wish-btn" style="position:static;width:50px;height:50px;border:1px solid var(--line)" data-wish="${p.id}" aria-label="Seçilmişlərə əlavə et"><i class="fa fa-heart-o"></i></button>
                </div>
                <ul class="perks">
                    <li><i class="fa fa-truck"></i><span>Saat 14:00-dək sifarişlərə eyni gün çatdırılma</span></li>
                    <li><i class="fa fa-leaf"></i><span>Təbii inqrediyentlər, süni konservantsız</span></li>
                    <li><i class="fa fa-lock"></i><span>Stripe ilə təhlükəsiz kart ödənişi</span></li>
                </ul>
            </div>`;

        root.addEventListener('click', async e => {
            const thumb = e.target.closest('[data-thumb]');
            if (thumb) {
                qs('[data-main-img]').src = thumb.dataset.thumb;
                qsa('[data-thumb]').forEach(t => t.classList.toggle('is-active', t === thumb));
            }
            const q = e.target.closest('[data-qty]');
            if (q) {
                qty = Math.min(20, Math.max(1, qty + Number(q.dataset.qty)));
                qs('[data-qty-value]').textContent = qty;
            }
            const buy = e.target.closest('[data-buy]');
            if (buy) {
                buy.disabled = true;
                await Store.addToCart(p.id, qty);
                buy.disabled = false;
            }
        });
        markWishlisted(root);

        // Oxşar məhsullar
        const related = (await Store.products()).filter(x => x.categoryName === p.categoryName && x.id !== p.id).slice(0, 4);
        const relWrap = qs('[data-related-wrap]');
        if (related.length) {
            relWrap.hidden = false;
            renderProductsInto(qs('[data-related]'), related);
        }
    } catch (err) {
        logError('product', err);
        root.innerHTML = UI.empty('fa-question-circle', 'Məhsul tapılmadı', esc(err.message), '<a href="shop.html" class="btn btn--dark">Mağazaya qayıt</a>');
    }
};

/* ==========================================================
   CART
   ========================================================== */
Pages.cart = async function () {
    const list = qs('[data-cart-list]');
    const summary = qs('[data-summary]');
    const user = Auth.user();

    if (!user) {
        qs('[data-cart-layout]').innerHTML = UI.empty('fa-shopping-bag', 'Səbətinizi görmək üçün daxil olun',
            'Səbətiniz hesabınızda saxlanılır — istədiyiniz vaxt davam edə bilərsiniz.',
            '<a href="login.html?returnUrl=shoping-cart.html" class="btn btn--dark">Daxil ol</a>');
        return;
    }

    let busy = false;

    async function load() {
        try {
            const [items, products] = await Promise.all([Store.basketItems(), Store.products()]);
            const byId = new Map(products.map(p => [p.id, p]));
            if (!items.length) {
                qs('[data-cart-layout]').innerHTML = UI.empty('fa-shopping-bag', 'Səbətiniz boşdur',
                    'Hələ heç bir şirniyyat əlavə etməmisiniz.', '<a href="shop.html" class="btn btn--primary">Alış-verişə başla</a>');
                return;
            }
            let subtotal = 0, count = 0;
            list.innerHTML = items.map(i => {
                const p = byId.get(i.productId) || { name: 'Məhsul #' + i.productId, price: 0, images: [] };
                const line = p.price * i.quantity;
                subtotal += line;
                count += i.quantity;
                return `
                    <div class="cart-item" data-id="${i.productId}">
                        <a href="product.html?id=${i.productId}"><img src="${esc(imageUrl(p.images && p.images[0]))}" alt=""></a>
                        <div>
                            <h3><a href="product.html?id=${i.productId}">${esc(p.name)}</a></h3>
                            <small>${money(p.price)} / ədəd</small><br>
                            <button class="remove-btn" data-remove><i class="fa fa-trash-o"></i> Sil</button>
                        </div>
                        <div class="qty">
                            <button data-dec aria-label="Azalt">−</button>
                            <span>${i.quantity}</span>
                            <button data-inc aria-label="Artır">+</button>
                        </div>
                        <div class="line-total">${money(line)}</div>
                    </div>`;
            }).join('');
            summary.innerHTML = `
                <h3>Sifariş xülasəsi</h3>
                <div class="summary-row"><span>Məhsullar (${count})</span><span>${money(subtotal)}</span></div>
                <div class="summary-row"><span>Çatdırılma</span><span>Pulsuz</span></div>
                <div class="summary-row summary-row--total"><span>Cəmi</span><span>${money(subtotal)}</span></div>
                <a href="checkout.html" class="btn btn--primary btn--block">Sifarişi rəsmiləşdir <i class="fa fa-arrow-right"></i></a>
                <a href="shop.html" class="btn btn--ghost btn--block" style="margin-top:10px">Alış-verişə davam et</a>
                <div class="secure-note"><i class="fa fa-lock"></i> Stripe ilə təhlükəsiz ödəniş</div>`;
        } catch (err) {
            logError('cart', err);
            list.innerHTML = UI.empty('fa-plug', 'Səbət yüklənmədi', esc(err.message));
        }
    }

    list.addEventListener('click', async e => {
        const row = e.target.closest('[data-id]');
        if (!row || busy) return;
        const id = Number(row.dataset.id);
        let action;
        if (e.target.closest('[data-inc]')) action = () => Api.increaseQty(user.id, id);
        else if (e.target.closest('[data-dec]')) action = () => Api.decreaseQty(user.id, id);
        else if (e.target.closest('[data-remove]')) action = () => Api.removeFromBasket(user.id, id);
        if (!action) return;
        busy = true;
        row.style.opacity = '.55';
        try {
            await action();
            await load();
            Store.refreshCartBadge();
        } catch (err) {
            toast(err.message, 'error');
            row.style.opacity = '';
        } finally {
            busy = false;
        }
    });

    load();
};

/* ==========================================================
   CHECKOUT (Stripe)
   ========================================================== */
Pages.checkout = async function () {
    if (!Auth.requireLogin('checkout.html')) return;
    const user = Auth.user();
    const form = qs('[data-checkout-form]');
    const alertBox = qs('[data-alert]');
    const summary = qs('[data-summary]');

    if (new URLSearchParams(location.search).get('canceled')) {
        alertBox.className = 'alert alert--warning';
        alertBox.textContent = 'Ödəniş ləğv edildi. Səbətiniz yerindədir — yenidən cəhd edə bilərsiniz.';
    }

    qs('#coName').value = user.fullName || '';

    let total = 0;
    try {
        const [items, products] = await Promise.all([Store.basketItems(), Store.products()]);
        if (!items.length) {
            qs('[data-cart-layout]').innerHTML = UI.empty('fa-shopping-bag', 'Səbətiniz boşdur',
                'Rəsmiləşdirmədən əvvəl səbətə nəsə şirin əlavə edin.', '<a href="shop.html" class="btn btn--primary">Mağazaya keç</a>');
            return;
        }
        const byId = new Map(products.map(p => [p.id, p]));
        const rows = items.map(i => {
            const p = byId.get(i.productId) || { name: 'Məhsul', price: 0, images: [] };
            total += p.price * i.quantity;
            return `<div class="mini-item">
                        <img src="${esc(imageUrl(p.images && p.images[0]))}" alt="">
                        <div><strong>${esc(p.name)}</strong><small>Say: ${i.quantity}</small></div>
                        <span>${money(p.price * i.quantity)}</span>
                    </div>`;
        }).join('');
        summary.innerHTML = `
            <h3>Sifarişiniz</h3>
            <div class="mini-items">${rows}</div>
            <div class="summary-row"><span>Aralıq cəm</span><span>${money(total)}</span></div>
            <div class="summary-row"><span>Çatdırılma</span><span>Pulsuz</span></div>
            <div class="summary-row summary-row--total"><span>Cəmi</span><span>${money(total)}</span></div>
            <div class="alert alert--info" style="margin-top:16px"><i class="fa fa-credit-card"></i><span>Test rejimi: <strong>4242 4242 4242 4242</strong> kartını, istənilən gələcək tarixi və istənilən CVC-ni istifadə edin.</span></div>`;
    } catch (err) {
        logError('checkout', err);
        summary.innerHTML = `<div class="alert alert--error">${esc(err.message)}</div>`;
    }

    const fields = {
        fullName: { el: qs('#coName'), check: v => v.length >= 2 || 'Ad soyadınızı yazın.' },
        phone: { el: qs('#coPhone'), check: v => /^[+\d][\d\s()-]{6,}$/.test(v) || 'Düzgün telefon nömrəsi yazın.' },
        address: { el: qs('#coAddress'), check: v => v.length >= 5 || 'Çatdırılma ünvanını yazın.' }
    };
    const validate = (key) => {
        const f = fields[key];
        const result = f.check(f.el.value.trim());
        const wrap = f.el.closest('.field');
        wrap.classList.toggle('has-error', result !== true);
        qs('.field-error', wrap).textContent = result === true ? '' : result;
        return result === true;
    };
    Object.keys(fields).forEach(k => fields[k].el.addEventListener('blur', () => validate(k)));

    form.addEventListener('submit', async e => {
        e.preventDefault();
        const ok = Object.keys(fields).map(validate).every(Boolean);
        if (!ok) return;
        const btn = qs('button[type="submit"]', form);
        btn.disabled = true;
        btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Stripe-a yönləndirilir...';
        alertBox.className = 'alert';
        alertBox.textContent = '';
        try {
            const res = await Api.checkout({
                fullName: fields.fullName.el.value.trim(),
                phone: fields.phone.el.value.trim(),
                address: fields.address.el.value.trim(),
                note: qs('#coNote').value.trim() || null
            });
            location.href = res.url;
        } catch (err) {
            alertBox.className = 'alert alert--error';
            alertBox.textContent = err.message;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-lock"></i> Təhlükəsiz ödə';
        }
    });
};

/* ==========================================================
   ORDER SUCCESS — Stripe-dan qayıdış
   ========================================================== */
Pages.orderSuccess = async function () {
    const box = qs('[data-result]');
    const sessionId = new URLSearchParams(location.search).get('session_id');
    if (!Auth.requireLogin('order-success.html' + location.search)) return;
    if (!sessionId) {
        box.innerHTML = statusBlock('error', 'fa-times', 'Ödəniş məlumatı tapılmadı', 'Ödəniş sessiyanız tapılmadı.', '<a href="orders.html" class="btn btn--dark">Sifarişlərim</a>');
        return;
    }
    try {
        const order = await Api.confirmPayment(sessionId);
        Store.refreshCartBadge();
        const paid = order.status !== 'Pending';
        box.innerHTML = paid
            ? statusBlock('success', 'fa-check', 'Sifarişiniz üçün təşəkkürlər!',
                `Ödəniş qəbul edildi. <strong>#${order.id}</strong> nömrəli sifarişiniz təsdiqləndi və qənnadçılarımız artıq işə başladı.`,
                `${orderSummary(order)}<div style="display:flex;gap:10px;justify-content:center;flex-wrap:wrap;margin-top:24px">
                    <a href="orders.html" class="btn btn--dark">Sifarişlərimə bax</a>
                    <a href="shop.html" class="btn btn--ghost">Alış-verişə davam et</a></div>`)
            : statusBlock('warning', 'fa-hourglass-half', 'Ödəniş hələ tamamlanmayıb',
                `#${order.id} nömrəli sifariş ödəniş gözləyir. Ödəniş etmisinizsə, bir azdan səhifəni yeniləyin.`,
                '<a href="checkout.html" class="btn btn--dark">Rəsmiləşdirməyə qayıt</a>');
    } catch (err) {
        box.innerHTML = statusBlock('error', 'fa-times', 'Ödəniş yoxlanıla bilmədi', esc(err.message), '<a href="orders.html" class="btn btn--dark">Sifarişlərim</a>');
    }
};

function statusBlock(kind, icon, title, text, extra = '') {
    return `<div class="card status-card">
                <div class="status-icon status-icon--${kind}"><i class="fa ${icon}"></i></div>
                <h1 style="font-size:34px">${title}</h1>
                <p class="muted">${text}</p>
                ${extra}
            </div>`;
}

function orderSummary(o) {
    return `<div style="text-align:left;margin-top:20px">
                <div class="mini-items">${o.items.map(i => `
                    <div class="mini-item">
                        <img src="${esc(imageUrl(i.image))}" alt="">
                        <div><strong>${esc(i.productName)}</strong><small>Say: ${i.quantity}</small></div>
                        <span>${money(i.price * i.quantity)}</span>
                    </div>`).join('')}</div>
                <div class="summary-row summary-row--total"><span>Ödənilən məbləğ</span><span>${money(o.totalPrice)}</span></div>
            </div>`;
}

/* ==========================================================
   MY ORDERS
   ========================================================== */
Pages.orders = async function () {
    if (!Auth.requireLogin('orders.html')) return;
    const box = qs('[data-orders]');
    try {
        const orders = asArray(await Api.myOrders());
        if (!orders.length) {
            box.innerHTML = UI.empty('fa-file-text-o', 'Hələ sifarişiniz yoxdur', 'Sifariş verdikdən sonra burada görünəcək.',
                '<a href="shop.html" class="btn btn--primary">Tortlara bax</a>');
            return;
        }
        box.innerHTML = orders.map(o => `
            <article class="order-card">
                <div class="order-card__head">
                    <div><h3>Sifariş #${o.id}</h3><small>${formatDate(o.createdDate)} · ${o.items.reduce((s, i) => s + i.quantity, 0)} məhsul</small></div>
                    <span class="status status--${esc(o.status)}">${esc(statusLabel(o.status))}</span>
                </div>
                <div class="order-card__items">${o.items.map(i =>
                    `<img src="${esc(imageUrl(i.image))}" alt="${esc(i.productName)}" title="${esc(i.productName)} × ${i.quantity}">`).join('')}</div>
                <div class="order-card__foot">
                    <span class="muted"><i class="fa fa-map-marker"></i> ${esc(o.address)}</span>
                    <strong>${money(o.totalPrice)}</strong>
                </div>
            </article>`).join('');
    } catch (err) {
        box.innerHTML = UI.empty('fa-plug', 'Sifarişlər yüklənmədi', esc(err.message));
    }
};

/* ==========================================================
   WISHLIST
   ========================================================== */
Pages.wishlist = async function () {
    const grid = qs('[data-wishlist]');
    const user = Auth.user();
    if (!user) {
        grid.innerHTML = UI.empty('fa-heart-o', 'Seçilmişlərinizi görmək üçün daxil olun', 'Sevdiyiniz tortları saxlayın və istədiyiniz vaxt onlara qayıdın.',
            '<a href="login.html?returnUrl=wisslist.html" class="btn btn--dark">Daxil ol</a>');
        return;
    }

    async function load() {
        grid.innerHTML = UI.skeletons(4);
        try {
            const [items, products] = await Promise.all([Store.wishlistItems(), Store.products().catch(() => [])]);
            if (!items.length) {
                grid.innerHTML = UI.empty('fa-heart-o', 'Seçilmişləriniz boşdur', 'Tortu bura əlavə etmək üçün üzərindəki ürəyə toxunun.',
                    '<a href="shop.html" class="btn btn--primary">Mağazaya bax</a>');
                return;
            }
            const byId = new Map(products.map(p => [p.id, p]));
            grid.innerHTML = items.map(w => {
                const p = byId.get(w.productId);
                return `
                    <article class="product-card">
                        <a href="product.html?id=${w.productId}" class="product-card__media">
                            <img src="${esc(imageUrl(w.productImage))}" alt="${esc(w.productName)}" loading="lazy">
                            ${p ? `<span class="chip">${esc(p.categoryName)}</span>` : ''}
                        </a>
                        <button class="wish-btn is-active" data-unwish="${w.productId}" aria-label="Seçilmişlərdən sil"><i class="fa fa-trash-o"></i></button>
                        <div class="product-card__body">
                            <h3 class="product-card__title"><a href="product.html?id=${w.productId}">${esc(w.productName)}</a></h3>
                            <div class="product-card__foot">
                                <span class="price">${money(w.productPrice)}</span>
                                <button class="add-btn" data-add-cart="${w.productId}" aria-label="Səbətə at"><i class="fa fa-shopping-bag"></i></button>
                            </div>
                        </div>
                    </article>`;
            }).join('');
        } catch (err) {
            grid.innerHTML = UI.empty('fa-plug', 'Seçilmişlər yüklənmədi', esc(err.message));
        }
    }

    grid.addEventListener('click', async e => {
        const b = e.target.closest('[data-unwish]');
        if (!b) return;
        b.disabled = true;
        try {
            await Api.removeFromWishlist(user.id, Number(b.dataset.unwish));
            toast('Seçilmişlərdən silindi');
            load();
        } catch (err) {
            toast(err.message, 'error');
            b.disabled = false;
        }
    });

    load();
};

/* ==========================================================
   BLOG
   ========================================================== */
Pages.blog = async function () {
    const grid = qs('[data-blogs]');
    const modal = qs('[data-modal]');
    grid.innerHTML = UI.skeletons(3);
    let blogs = [];
    try {
        blogs = asArray(await Api.blogs());
        grid.innerHTML = blogs.length ? blogs.map((b, k) => `
            <article class="blog-card">
                <div class="blog-card__img" style="background-image:url('${esc(imageUrl(b.image))}')"></div>
                <div class="blog-card__body">
                    <h3>${esc(b.title)}</h3>
                    <p>${esc(b.description)}</p>
                    <button class="link-arrow" data-read="${k}">Ətraflı oxu <i class="fa fa-arrow-right"></i></button>
                </div>
            </article>`).join('')
            : UI.empty('fa-pencil', 'Hələ yazı yoxdur', 'Tezliklə mətbəximizdən reseptlər və hekayələr burada olacaq.');
    } catch (err) {
        grid.innerHTML = UI.empty('fa-plug', 'Yazılar yüklənmədi', esc(err.message));
    }

    const close = () => modal.classList.remove('is-open');
    grid.addEventListener('click', e => {
        const b = e.target.closest('[data-read]');
        if (!b) return;
        const post = blogs[Number(b.dataset.read)];
        qs('[data-modal-img]', modal).src = imageUrl(post.image);
        qs('[data-modal-title]', modal).textContent = post.title;
        qs('[data-modal-text]', modal).textContent = post.description;
        modal.classList.add('is-open');
        qs('.modal__close', modal).focus();
    });
    modal.addEventListener('click', e => { if (e.target === modal || e.target.closest('.modal__close')) close(); });
    document.addEventListener('keydown', e => { if (e.key === 'Escape') close(); });
};

/* ==========================================================
   CONTACT
   ========================================================== */
Pages.contact = async function () {
    Store.settings().then(s => {
        if (s && s.phone) {
            const phone = s.phone.trim();
            const el = qs('[data-phone]');
            el.textContent = phone;
            el.href = 'tel:' + phone.replace(/\s+/g, '');
        }
    });
    Api.contactSettings().then(d => {
        const cs = asArray(d)[0];
        if (cs && cs.title) qs('[data-contact-title]').textContent = cs.title;
    }).catch(() => {});

    const form = qs('[data-contact-form]');
    const alertBox = qs('[data-alert]', form);
    form.addEventListener('submit', async e => {
        e.preventDefault();
        const name = qs('#ctName').value.trim();
        const email = qs('#ctEmail').value.trim();
        const message = qs('#ctMessage').value.trim();
        if (!name || !email || !message) {
            alertBox.className = 'alert alert--error';
            alertBox.textContent = 'Bütün sahələri doldurun.';
            return;
        }
        if (!/^\S+@\S+\.\S+$/.test(email)) {
            alertBox.className = 'alert alert--error';
            alertBox.textContent = 'Düzgün e-poçt ünvanı yazın.';
            return;
        }
        const btn = qs('button[type="submit"]', form);
        btn.disabled = true;
        try {
            await Api.sendContact(name, email, message);
            form.reset();
            alertBox.className = 'alert alert--success';
            alertBox.textContent = 'Təşəkkürlər! Mesajınız göndərildi — tezliklə cavab verəcəyik.';
        } catch (err) {
            alertBox.className = 'alert alert--error';
            alertBox.textContent = err.message;
        } finally {
            btn.disabled = false;
        }
    });
};

/* ---------------- Başlanğıc ---------------- */
const pageInit = {
    home: Pages.home, about: Pages.about, shop: Pages.shop, product: Pages.product,
    cart: Pages.cart, checkout: Pages.checkout, 'order-success': Pages.orderSuccess,
    orders: Pages.orders, wishlist: Pages.wishlist, blog: Pages.blog, contact: Pages.contact
}[PAGE];
if (pageInit) pageInit();
