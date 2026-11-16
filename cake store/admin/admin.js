/* ==========================================================
   Cake Shop — Admin panel (hash router + konfiqurasiya əsaslı CRUD)
   Backend: /api/admin/{Controller}/{Action}  (Admin rolu tələb olunur)
   Asılılıq: ../js/api.js
   ========================================================== */
'use strict';

(function () {
    const app = document.getElementById('app');
    const IMG_FALLBACK = '../img/shop/product-1.jpg';
    const img = (f) => imageUrl(f, IMG_FALLBACK);

    /* ---------------- Giriş yoxlaması ---------------- */
    const user = Auth.user();
    if (!user) {
        location.href = '../login.html?returnUrl=' + encodeURIComponent('admin/index.html');
        return;
    }
    if (!user.isAdmin) {
        app.innerHTML = `
            <div class="gate"><div class="card status-card">
                <div class="status-icon status-icon--error"><i class="fa fa-lock"></i></div>
                <h1 style="font-size:30px">Yalnız adminlər üçün</h1>
                <p class="muted"><strong>${esc(user.username)}</strong> hesabının admin icazəsi yoxdur.</p>
                <div style="display:flex;gap:10px;justify-content:center;flex-wrap:wrap">
                    <a href="../index.html" class="btn btn--dark">Mağazaya qayıt</a>
                    <button class="btn btn--ghost" id="switchUser">Admin kimi daxil ol</button>
                </div>
            </div></div>`;
        document.getElementById('switchUser').onclick = () => Auth.logout(true, '../login.html?returnUrl=' + encodeURIComponent('admin/index.html'));
        return;
    }

    /* ---------------- Admin API ---------------- */
    const A = {
        get: (ctrl, action = 'GetAll') => api(`/api/admin/${ctrl}/${action}`, { auth: true }),
        create: (ctrl, form) => api(`/api/admin/${ctrl}/Create`, { method: 'POST', form, auth: true }),
        edit: (ctrl, id, form) => api(`/api/admin/${ctrl}/Edit/${encodeURIComponent(id)}`, { method: 'PUT', form, auth: true }),
        remove: (ctrl, id) => api(`/api/admin/${ctrl}/Delete`, { method: 'DELETE', query: { id }, auth: true })
    };

    const thumb = (src, contain) => `<img class="thumb${contain ? ' thumb--contain' : ''}" src="${esc(src)}" alt="" loading="lazy">`;
    const clip = (t) => `<div class="clip" title="${esc(t)}">${esc(t)}</div>`;

    /* ---------------- Resurs konfiqurasiyası ---------------- */
    const categoryOptions = async () => asArray(await A.get('Category')).map(c => ({ value: c.id, label: c.name }));
    const teamOptions = async () => asArray(await A.get('Team')).map(t => ({ value: t.id, label: t.fullName }));
    const imageField = (required = true, label = 'Şəkil') => ({ name: 'UploadImage', label, type: 'file', requiredOnCreate: required, help: 'JPG/PNG/WebP, maks. 1 MB' });

    const RESOURCES = {
        products: {
            title: 'Məhsullar', icon: 'fa-birthday-cake', ctrl: 'Product', noun: 'məhsul',
            columns: [
                { label: '', render: r => thumb(img(r.images && r.images[0])) },
                { label: 'Ad', render: r => `<strong>${esc(r.name)}</strong>` },
                { label: 'Kateqoriya', render: r => esc(r.categoryName) },
                { label: 'Qiymət', render: r => money(r.price) }
            ],
            fields: [
                { name: 'Name', label: 'Ad', type: 'text', required: true, max: 50 },
                { name: 'Price', label: 'Qiymət (₼)', type: 'number', required: true, step: '0.01', min: '0' },
                { name: 'CategoryId', label: 'Kateqoriya', type: 'select', required: true, options: categoryOptions },
                { name: 'UploadImages', label: 'Şəkillər', type: 'files', requiredOnCreate: true, help: 'Birinci şəkil əsas şəkildir. Yeni şəkil yükləsəniz köhnələr əvəz olunur. Hər biri maks. 1 MB.' }
            ],
            toForm: (r, opts) => ({
                Name: r.name, Price: r.price,
                CategoryId: (opts.CategoryId.find(o => o.label === r.categoryName) || {}).value
            }),
            images: r => r.images || []
        },
        categories: {
            title: 'Kateqoriyalar', icon: 'fa-th-large', ctrl: 'Category', noun: 'kateqoriya',
            columns: [
                { label: '', render: r => thumb(img(r.icon), true) },
                { label: 'Ad', render: r => `<strong>${esc(r.name)}</strong>` }
            ],
            fields: [
                { name: 'Name', label: 'Ad', type: 'text', required: true },
                imageField(true, 'İkon')
            ],
            toForm: r => ({ Name: r.name }),
            images: r => [r.icon]
        },
        sliders: {
            title: 'Slayder', icon: 'fa-picture-o', ctrl: 'Slider', noun: 'slayd',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` },
                { label: 'Təsvir', render: r => clip(r.description) }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true, max: 50 },
                { name: 'Description', label: 'Təsvir', type: 'textarea', required: true, max: 100 },
                imageField()
            ],
            toForm: r => ({ Title: r.title, Description: r.description }),
            images: r => [r.image]
        },
        about: {
            title: 'Haqqımızda bölməsi', icon: 'fa-info-circle', ctrl: 'About', noun: 'haqqımızda yazısı',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` },
                { label: 'Təsvir', render: r => clip(r.description) }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true },
                { name: 'Description', label: 'Təsvir', type: 'textarea', required: true },
                imageField()
            ],
            toForm: r => ({ Title: r.title, Description: r.description }),
            images: r => [r.image]
        },
        blogs: {
            title: 'Bloq yazıları', icon: 'fa-pencil', ctrl: 'Blog', noun: 'yazı',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` },
                { label: 'Mətn', render: r => clip(r.description) }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true },
                { name: 'Description', label: 'Mətn', type: 'textarea', required: true, rows: 8 },
                imageField()
            ],
            toForm: r => ({ Title: r.title, Description: r.description }),
            images: r => [r.image]
        },
        team: {
            title: 'Komanda', icon: 'fa-users', ctrl: 'Team', noun: 'komanda üzvü',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Ad', render: r => `<strong>${esc(r.fullName)}</strong>` },
                { label: 'Vəzifə', render: r => esc(r.position) },
                { label: 'Sosial şəbəkələr', render: r => esc((r.socialNames || []).join(', ')) || '<span class="muted">—</span>' }
            ],
            fields: [
                { name: 'FullName', label: 'Ad soyad', type: 'text', required: true },
                { name: 'Position', label: 'Vəzifə', type: 'text', required: true },
                { name: 'Title', label: 'Bölmə başlığı', type: 'text', required: true, help: 'Məs. “Sweet Baker”' },
                imageField(true, 'Foto')
            ],
            toForm: r => ({ FullName: r.fullName, Position: r.position, Title: r.title }),
            images: r => [r.image]
        },
        socials: {
            title: 'Komanda sosial linkləri', icon: 'fa-share-alt', ctrl: 'Social', noun: 'sosial link',
            columns: [
                { label: 'Şəbəkə', render: r => `<strong>${esc(r.name)}</strong>` },
                { label: 'URL', render: r => clip(r.url) }
            ],
            fields: [
                { name: 'Name', label: 'Şəbəkə', type: 'text', required: true, help: 'Facebook, Twitter, Instagram, Youtube...' },
                { name: 'Url', label: 'URL', type: 'text', required: true },
                { name: 'TeamId', label: 'Komanda üzvü', type: 'select', required: true, createOnly: true, options: teamOptions }
            ],
            toForm: r => ({ Name: r.name, Url: r.url })
        },
        testimonials: {
            title: 'Rəylər', icon: 'fa-quote-left', ctrl: 'Testimonial', noun: 'rəy',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Müştəri', render: r => `<strong>${esc(r.fullName)}</strong><br><small class="muted">${esc(r.city)}</small>` },
                { label: 'Rəy', render: r => clip(r.description) }
            ],
            fields: [
                { name: 'FullName', label: 'Müştərinin adı', type: 'text', required: true },
                { name: 'City', label: 'Şəhər', type: 'text', required: true },
                { name: 'Title', label: 'Bölmə başlığı', type: 'text', required: true },
                { name: 'Description', label: 'Rəy', type: 'textarea', required: true },
                imageField(true, 'Foto')
            ],
            toForm: r => ({ FullName: r.fullName, City: r.city, Title: r.title, Description: r.description }),
            images: r => [r.image]
        },
        instagram: {
            title: 'Instagram', icon: 'fa-instagram', ctrl: 'Instagram', noun: 'Instagram bloku',
            columns: [
                { label: 'Şəkillər', render: r => (r.images || []).slice(0, 4).map(i => thumb(img(i))).join(' ') },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` },
                { label: 'Hesab', render: r => esc(r.socialName) }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true },
                { name: 'SocialName', label: 'Hesab', type: 'text', required: true, help: 'Məs. @sweetcake' },
                { name: 'UploadImages', label: 'Şəkillər', type: 'files', requiredOnCreate: true, help: '6 şəkil ən yaxşı görünür. Hər biri maks. 1 MB.' }
            ],
            toForm: r => ({ Title: r.title, SocialName: r.socialName }),
            images: r => r.images || []
        },
        settings: {
            title: 'Sayt tənzimləmələri', icon: 'fa-cog', ctrl: 'Setting', noun: 'tənzimləmə',
            columns: [
                { label: 'Loqo', render: r => thumb(img(r.logo), true) },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` },
                { label: 'Telefon', render: r => esc(r.phone) }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true },
                { name: 'Phone', label: 'Telefon', type: 'text', required: true },
                imageField(true, 'Loqo')
            ],
            toForm: r => ({ Title: r.title, Phone: r.phone }),
            images: r => [r.logo]
        },
        contactSettings: {
            title: 'Əlaqə səhifəsi', icon: 'fa-envelope-o', ctrl: 'ContactSetting', noun: 'əlaqə tənzimləməsi',
            columns: [
                { label: '', render: r => thumb(img(r.image)) },
                { label: 'Başlıq', render: r => `<strong>${esc(r.title)}</strong>` }
            ],
            fields: [
                { name: 'Title', label: 'Başlıq', type: 'text', required: true },
                imageField()
            ],
            toForm: r => ({ Title: r.title }),
            images: r => [r.image]
        },
        messages: {
            title: 'Mesajlar', icon: 'fa-comments-o', ctrl: 'Contact', noun: 'mesaj', noCreate: true, noEdit: true, view: true,
            columns: [
                { label: 'Göndərən', render: r => `<strong>${esc(r.name)}</strong><br><a class="muted" href="mailto:${esc(r.email)}">${esc(r.email)}</a>` },
                { label: 'Mesaj', render: r => clip(r.message) }
            ],
            viewBody: r => `<dl class="detail-grid">
                    <div><dt>Ad</dt><dd>${esc(r.name)}</dd></div>
                    <div><dt>E-poçt</dt><dd><a href="mailto:${esc(r.email)}">${esc(r.email)}</a></dd></div>
                </dl>
                <div><dt class="help" style="font-weight:700;text-transform:uppercase">Mesaj</dt><p style="white-space:pre-wrap;margin-top:6px">${esc(r.message)}</p></div>`
        },
        subscribers: {
            title: 'Abunəçilər', icon: 'fa-paper-plane', ctrl: 'Subscribe', noun: 'abunəçi', noCreate: true, noEdit: true, noDelete: true,
            columns: [
                { label: '#', render: (r, i) => i + 1 },
                { label: 'E-poçt', render: r => `<a href="mailto:${esc(r.email)}">${esc(r.email)}</a>` }
            ]
        }
    };

    /* ---------------- Naviqasiya ---------------- */
    const NAV = [
        { group: 'Ümumi', items: [['dashboard', 'İdarə paneli', 'fa-dashboard'], ['orders', 'Sifarişlər', 'fa-shopping-bag']] },
        { group: 'Kataloq', items: [['products', 'Məhsullar', 'fa-birthday-cake'], ['categories', 'Kateqoriyalar', 'fa-th-large']] },
        { group: 'Məzmun', items: [['sliders', 'Slayder', 'fa-picture-o'], ['about', 'Haqqımızda', 'fa-info-circle'], ['blogs', 'Bloq', 'fa-pencil'],
            ['team', 'Komanda', 'fa-users'], ['socials', 'Komanda sosial linkləri', 'fa-share-alt'], ['testimonials', 'Rəylər', 'fa-quote-left'], ['instagram', 'Instagram', 'fa-instagram']] },
        { group: 'Müştərilər', items: [['users', 'İstifadəçilər', 'fa-user'], ['messages', 'Mesajlar', 'fa-comments-o'], ['subscribers', 'Abunəçilər', 'fa-paper-plane']] },
        { group: 'Tənzimləmələr', items: [['settings', 'Sayt tənzimləmələri', 'fa-cog'], ['contactSettings', 'Əlaqə səhifəsi', 'fa-envelope-o']] }
    ];

    app.innerHTML = `
        <div class="admin-layout">
            <aside class="sidebar">
                <a href="#dashboard" class="logo"><span class="logo__mark"><i class="fa fa-birthday-cake"></i></span><span>Cake<em>.</em><small>Admin panel</small></span></a>
                ${NAV.map(g => `<div class="side-group"><h6>${g.group}</h6>${g.items.map(([id, label, icon]) =>
                    `<a href="#${id}" class="side-link" data-route="${id}"><i class="fa ${icon}"></i>${label}</a>`).join('')}</div>`).join('')}
                <div class="sidebar__foot">
                    <a href="../index.html" class="side-link"><i class="fa fa-external-link"></i>Sayta bax</a>
                    <button class="side-link" style="border:none;background:none;width:100%;text-align:left" id="logoutBtn"><i class="fa fa-sign-out"></i>Çıxış</button>
                </div>
            </aside>
            <div class="side-backdrop" id="sideBackdrop"></div>
            <div class="admin-main">
                <header class="topbar">
                    <button class="icon-btn menu-btn" id="menuBtn" aria-label="Menyu"><i class="fa fa-bars"></i></button>
                    <h1 id="pageTitle">İdarə paneli</h1>
                    <div class="topbar__right">
                        <a href="../index.html" class="btn btn--ghost btn--sm"><i class="fa fa-external-link"></i> Sayta bax</a>
                        <span class="avatar" title="${esc(user.fullName)}">${esc(user.fullName.split(/\s+/).map(s => s[0]).join('').slice(0, 2).toUpperCase())}</span>
                    </div>
                </header>
                <main class="admin-content" id="content"></main>
            </div>
        </div>
        <div class="a-modal" id="modal" role="dialog" aria-modal="true"><div class="a-modal__box" id="modalBox"></div></div>`;

    let content = document.getElementById('content');
    // Hər görünüş təmiz element alır ki, köhnə event listener-lər təkrarlanmasın
    function resetContent() {
        const fresh = content.cloneNode(false);
        content.replaceWith(fresh);
        content = fresh;
    }
    const pageTitle = document.getElementById('pageTitle');
    const modal = document.getElementById('modal');
    const modalBox = document.getElementById('modalBox');

    document.getElementById('logoutBtn').onclick = () => Auth.logout(true, '../login.html');
    document.getElementById('menuBtn').onclick = () => document.body.classList.add('side-open');
    document.getElementById('sideBackdrop').onclick = () => document.body.classList.remove('side-open');

    /* ---------------- Modal köməkçiləri ---------------- */
    function openModal(html, wide = false) {
        modalBox.className = 'a-modal__box' + (wide ? ' a-modal__box--wide' : '');
        modalBox.innerHTML = html;
        modal.classList.add('is-open');
        qsa('[data-close]', modalBox).forEach(b => b.onclick = closeModal);
        const first = modalBox.querySelector('input, select, textarea, button');
        if (first) setTimeout(() => first.focus(), 30);
    }
    function closeModal() { modal.classList.remove('is-open'); modalBox.innerHTML = ''; }
    modal.addEventListener('mousedown', e => { if (e.target === modal) closeModal(); });
    document.addEventListener('keydown', e => { if (e.key === 'Escape' && modal.classList.contains('is-open')) closeModal(); });

    function confirmDialog(title, text, okLabel = 'Sil') {
        return new Promise(resolve => {
            openModal(`
                <div class="a-modal__head"><h3>${title}</h3><button class="icon-btn" data-close aria-label="Bağla"><i class="fa fa-times"></i></button></div>
                <div class="a-modal__body"><p class="muted" style="margin:0">${text}</p></div>
                <div class="a-modal__foot">
                    <button class="btn btn--ghost btn--sm" data-close>Ləğv et</button>
                    <button class="btn btn--primary btn--sm" id="confirmOk" style="background:var(--danger)">${okLabel}</button>
                </div>`);
            const ok = document.getElementById('confirmOk');
            ok.focus();
            ok.onclick = () => { closeModal(); resolve(true); };
            qsa('[data-close]', modalBox).forEach(b => b.addEventListener('click', () => resolve(false)));
        });
    }

    function panel(title, toolbar, body) {
        return `<section class="panel">
                    <div class="panel__head"><h2>${title}</h2><span class="spacer"></span>${toolbar || ''}</div>
                    ${body}
                </section>`;
    }

    function table(columns, rows, actions) {
        if (!rows.length) return '<div class="table-empty"><i class="fa fa-inbox" style="font-size:28px"></i><p>Hələ heç nə yoxdur.</p></div>';
        return `<div class="table-wrap"><table class="table">
                    <thead><tr>${columns.map(c => `<th>${c.label}</th>`).join('')}${actions ? '<th></th>' : ''}</tr></thead>
                    <tbody>${rows.map((r, i) => `<tr>${columns.map(c => `<td>${c.render(r, i)}</td>`).join('')}${actions ? `<td class="actions">${actions(r)}</td>` : ''}</tr>`).join('')}</tbody>
                </table></div>`;
    }

    function loading() {
        resetContent();
        content.innerHTML = '<div class="skeleton" style="height:320px"></div>';
    }
    function failed(err) {
        content.innerHTML = `<div class="alert alert--error"><i class="fa fa-exclamation-circle"></i><span>${esc(err.message)}</span></div>`;
    }

    /* ==========================================================
       DASHBOARD
       ========================================================== */
    async function viewDashboard() {
        loading();
        try {
            const s = await api('/api/admin/Order/GetStats', { auth: true });
            const stat = (icon, color, label, value) => `
                <div class="stat"><div class="stat__icon stat__icon--${color}">${icon === 'manat' ? '<span class="manat-icon">₼</span>' : `<i class="fa ${icon}"></i>`}</div>
                <div><small>${label}</small><strong>${value}</strong></div></div>`;
            content.innerHTML = `
                <div class="stats">
                    ${stat('manat', 'green', 'Gəlir', money(s.revenue))}
                    ${stat('fa-shopping-bag', 'rose', 'Sifarişlər', `${s.totalOrders} <small style="font-size:13px;color:var(--warning)">${s.pendingOrders} gözləyir</small>`)}
                    ${stat('fa-birthday-cake', 'amber', 'Məhsullar', s.products)}
                    ${stat('fa-user', 'blue', 'İstifadəçilər', s.users)}
                </div>
                ${panel('Son sifarişlər', '<a href="#orders" class="btn btn--ghost btn--sm">Bütün sifarişlər</a>', table(ORDER_COLUMNS, s.recentOrders,
                    o => `<button class="act-btn" data-view-order="${o.id}" title="Bax"><i class="fa fa-eye"></i></button>`))}
                <div class="stats" style="margin-top:24px;grid-template-columns:repeat(3,1fr)">
                    ${stat('fa-th-large', 'amber', 'Kateqoriyalar', s.categories)}
                    ${stat('fa-comments-o', 'rose', 'Mesajlar', s.messages)}
                    ${stat('fa-paper-plane', 'blue', 'Abunəçilər', s.subscribers)}
                </div>`;
            bindOrderView(s.recentOrders);
        } catch (err) { failed(err); }
    }

    /* ==========================================================
       ORDERS
       ========================================================== */
    const STATUSES = ['Pending', 'Paid', 'Preparing', 'Delivered', 'Cancelled'];
    const ORDER_COLUMNS = [
        { label: 'Sifariş', render: o => `<strong>#${o.id}</strong>` },
        { label: 'Müştəri', render: o => `${esc(o.fullName)}<br><small class="muted">${esc(o.email || o.userName || '')}</small>` },
        { label: 'Tarix', render: o => formatDate(o.createdDate) },
        { label: 'Məhsul sayı', render: o => o.items.reduce((s, i) => s + i.quantity, 0) },
        { label: 'Cəmi', render: o => `<strong>${money(o.totalPrice)}</strong>` },
        { label: 'Status', render: o => `<span class="status status--${esc(o.status)}">${esc(statusLabel(o.status))}</span>` }
    ];

    function orderDetail(o) {
        return `
            <div class="a-modal__head"><h3>Sifariş #${o.id}</h3><button class="icon-btn" data-close aria-label="Bağla"><i class="fa fa-times"></i></button></div>
            <div class="a-modal__body">
                <dl class="detail-grid">
                    <div><dt>Müştəri</dt><dd>${esc(o.fullName)}</dd></div>
                    <div><dt>Hesab</dt><dd>${esc(o.userName || '')} · ${esc(o.email || '')}</dd></div>
                    <div><dt>Telefon</dt><dd><a href="tel:${esc(o.phone)}">${esc(o.phone)}</a></dd></div>
                    <div><dt>Tarix</dt><dd>${formatDate(o.createdDate)}</dd></div>
                    <div style="grid-column:1/-1"><dt>Ünvan</dt><dd>${esc(o.address)}</dd></div>
                    ${o.note ? `<div style="grid-column:1/-1"><dt>Qeyd</dt><dd>${esc(o.note)}</dd></div>` : ''}
                    <div><dt>Ödəniş tarixi</dt><dd>${o.paidDate ? formatDate(o.paidDate) : '<span class="muted">Ödənilməyib</span>'}</dd></div>
                    <div><dt>Status</dt><dd><span class="status status--${esc(o.status)}">${esc(statusLabel(o.status))}</span></dd></div>
                </dl>
                <div class="mini-items">${o.items.map(i => `
                    <div class="mini-item"><img src="${esc(img(i.image))}" alt="">
                        <div><strong>${esc(i.productName)}</strong><small>${money(i.price)} × ${i.quantity}</small></div>
                        <span>${money(i.price * i.quantity)}</span></div>`).join('')}</div>
                <div class="summary-row summary-row--total"><span>Cəmi</span><span>${money(o.totalPrice)}</span></div>
            </div>
            <div class="a-modal__foot"><button class="btn btn--dark btn--sm" data-close>Bağla</button></div>`;
    }

    function bindOrderView(orders) {
        content.addEventListener('click', e => {
            const b = e.target.closest('[data-view-order]');
            if (!b) return;
            const o = orders.find(x => x.id === Number(b.dataset.viewOrder));
            if (o) openModal(orderDetail(o), true);
        });
    }

    async function viewOrders() {
        loading();
        let orders;
        try { orders = asArray(await A.get('Order')); } catch (err) { return failed(err); }
        const state = { status: '', q: '' };

        content.innerHTML = panel(`Sifarişlər <span class="muted" style="font-size:15px">(${orders.length})</span>`, `
            <div class="panel__search"><i class="fa fa-search"></i><input id="q" type="search" placeholder="Ad, e-poçt və ya #id axtar"></div>
            <select class="select" id="statusFilter"><option value="">Bütün statuslar</option>${STATUSES.map(s => `<option value="${s}">${statusLabel(s)}</option>`).join('')}</select>`,
            '<div id="tbl"></div>');

        const draw = () => {
            const q = state.q.toLowerCase();
            const rows = orders.filter(o => (!state.status || o.status === state.status) &&
                (!q || `#${o.id} ${o.fullName} ${o.email} ${o.userName} ${o.phone}`.toLowerCase().includes(q)));
            const cols = ORDER_COLUMNS.slice(0, 5).concat([{
                label: 'Status',
                render: o => `<select class="status-select" data-status="${o.id}" aria-label="Status">${STATUSES.map(s => `<option value="${s}" ${s === o.status ? 'selected' : ''}>${statusLabel(s)}</option>`).join('')}</select>`
            }]);
            document.getElementById('tbl').innerHTML = table(cols, rows,
                o => `<button class="act-btn" data-view-order="${o.id}" title="Bax"><i class="fa fa-eye"></i></button>`);
        };
        draw();

        document.getElementById('q').oninput = e => { state.q = e.target.value; draw(); };
        document.getElementById('statusFilter').onchange = e => { state.status = e.target.value; draw(); };
        content.addEventListener('change', async e => {
            const sel = e.target.closest('[data-status]');
            if (!sel) return;
            const id = Number(sel.dataset.status);
            sel.disabled = true;
            try {
                await api(`/api/admin/Order/ChangeStatus/${id}`, { method: 'PUT', query: { status: sel.value }, auth: true });
                orders.find(o => o.id === id).status = sel.value;
                toast(`Sifariş #${id} → ${statusLabel(sel.value)}`);
            } catch (err) {
                toast(err.message, 'error');
                sel.value = orders.find(o => o.id === id).status;
            } finally {
                sel.disabled = false;
            }
        });
        bindOrderView(orders);
    }

    /* ==========================================================
       USERS
       ========================================================== */
    async function viewUsers() {
        loading();
        let users;
        try { users = asArray(await api('/api/admin/Account/GetAllUsers', { auth: true })); } catch (err) { return failed(err); }
        let q = '';
        content.innerHTML = panel(`İstifadəçilər <span class="muted" style="font-size:15px">(${users.length})</span>`,
            '<div class="panel__search"><i class="fa fa-search"></i><input id="q" type="search" placeholder="İstifadəçi axtar"></div>', '<div id="tbl"></div>');

        const draw = () => {
            const rows = users.filter(u => !q || `${u.fullName} ${u.username} ${u.email}`.toLowerCase().includes(q.toLowerCase()));
            document.getElementById('tbl').innerHTML = table([
                { label: 'Ad', render: u => `<span class="avatar" style="margin-right:10px;vertical-align:middle">${esc(String(u.fullName || u.username).split(/\s+/).map(s => s[0]).join('').slice(0, 2).toUpperCase())}</span><strong>${esc(u.fullName)}</strong>` },
                { label: 'İstifadəçi adı', render: u => '@' + esc(u.username) },
                { label: 'E-poçt', render: u => `<a href="mailto:${esc(u.email)}">${esc(u.email)}</a>` },
                { label: 'Rollar', render: u => (u.roles || []).map(r => `<span class="role-chip ${r === 'Admin' ? 'role-chip--admin' : ''}">${esc(r === 'Member' ? 'Üzv' : r)}</span>`).join('') }
            ], rows, u => (u.roles || []).includes('Admin') ? ''
                : `<button class="btn btn--soft btn--sm" data-make-admin="${esc(u.username)}"><i class="fa fa-shield"></i> Admin et</button>`);
        };
        draw();
        document.getElementById('q').oninput = e => { q = e.target.value; draw(); };

        content.addEventListener('click', async e => {
            const b = e.target.closest('[data-make-admin]');
            if (!b) return;
            const username = b.dataset.makeAdmin;
            if (!await confirmDialog('Admin icazəsi verilsin?', `<strong>@${esc(username)}</strong> məhsulları, sifarişləri və bütün sayt məzmununu idarə edə biləcək.`, 'Admin et')) return;
            try {
                await api('/api/admin/Account/AddRole', { method: 'POST', query: { Username: username, RoleName: 'Admin' }, auth: true });
                users.find(u => u.username === username).roles.push('Admin');
                toast(`@${username} artıq admindir`);
                draw();
            } catch (err) { toast(err.message, 'error'); }
        });
    }

    /* ==========================================================
       GENERIC CRUD
       ========================================================== */
    async function viewResource(key) {
        const R = RESOURCES[key];
        loading();
        let rows;
        try { rows = asArray(await A.get(R.ctrl)); } catch (err) { return failed(err); }
        let q = '';

        const toolbar = `
            <div class="panel__search"><i class="fa fa-search"></i><input id="q" type="search" placeholder="Axtar..."></div>
            ${R.noCreate ? '' : `<button class="btn btn--primary btn--sm" id="addBtn"><i class="fa fa-plus"></i> Yeni ${esc(R.noun)}</button>`}`;
        content.innerHTML = panel(`${esc(R.title)} <span class="muted" style="font-size:15px">(${rows.length})</span>`, toolbar, '<div id="tbl"></div>');

        const draw = () => {
            const filtered = rows.filter(r => !q || JSON.stringify(r).toLowerCase().includes(q.toLowerCase()));
            document.getElementById('tbl').innerHTML = table(R.columns, filtered, r =>
                (R.view ? `<button class="act-btn" data-view="${esc(r.id)}" title="Bax"><i class="fa fa-eye"></i></button>` : '') +
                (R.noEdit ? '' : `<button class="act-btn" data-edit="${esc(r.id)}" title="Redaktə et"><i class="fa fa-pencil"></i></button>`) +
                (R.noDelete ? '' : `<button class="act-btn act-btn--danger" data-del="${esc(r.id)}" title="Sil"><i class="fa fa-trash-o"></i></button>`));
        };
        draw();

        const reload = () => viewResource(key);
        document.getElementById('q').oninput = e => { q = e.target.value; draw(); };
        const addBtn = document.getElementById('addBtn');
        if (addBtn) addBtn.onclick = () => openForm(R, null, reload);

        content.addEventListener('click', async e => {
            const find = id => rows.find(r => String(r.id) === String(id));
            const edit = e.target.closest('[data-edit]');
            if (edit) return openForm(R, find(edit.dataset.edit), reload);

            const view = e.target.closest('[data-view]');
            if (view) {
                const r = find(view.dataset.view);
                return openModal(`
                    <div class="a-modal__head"><h3>${esc(R.noun[0].toUpperCase() + R.noun.slice(1))}</h3><button class="icon-btn" data-close aria-label="Bağla"><i class="fa fa-times"></i></button></div>
                    <div class="a-modal__body">${R.viewBody(r)}</div>
                    <div class="a-modal__foot"><button class="btn btn--dark btn--sm" data-close>Bağla</button></div>`);
            }

            const del = e.target.closest('[data-del]');
            if (del) {
                if (!await confirmDialog(`${R.noun[0].toUpperCase() + R.noun.slice(1)} silinsin?`, 'Bu əməliyyatı geri qaytarmaq mümkün deyil.')) return;
                try {
                    await A.remove(R.ctrl, del.dataset.del);
                    toast(`${R.noun[0].toUpperCase() + R.noun.slice(1)} silindi`);
                    reload();
                } catch (err) { toast(err.message, 'error'); }
            }
        });
    }

    async function openForm(R, row, onSaved) {
        const isEdit = !!row;
        const fields = R.fields.filter(f => !(isEdit && f.createOnly));

        // Select seçimlərini əvvəlcədən yükləyirik
        const opts = {};
        try {
            for (const f of fields) if (f.type === 'select') opts[f.name] = await f.options();
        } catch (err) { return toast(err.message, 'error'); }
        const values = isEdit && R.toForm ? R.toForm(row, opts) : {};
        const currentImages = isEdit && R.images ? R.images(row).filter(Boolean) : [];

        const fieldHtml = f => {
            const id = `f_${f.name}`;
            const req = f.required || (f.requiredOnCreate && !isEdit);
            const label = `<label for="${id}">${esc(f.label)}${req ? ' <span class="req">*</span>' : ''}</label>`;
            const v = values[f.name] ?? '';
            let input;
            if (f.type === 'textarea') {
                input = `<textarea id="${id}" name="${f.name}" class="textarea" rows="${f.rows || 4}" ${f.max ? `maxlength="${f.max}"` : ''}>${esc(v)}</textarea>`;
            } else if (f.type === 'select') {
                input = `<select id="${id}" name="${f.name}" class="input select" style="border-radius:14px">
                            <option value="">Seçin...</option>
                            ${opts[f.name].map(o => `<option value="${esc(o.value)}" ${String(o.value) === String(v) ? 'selected' : ''}>${esc(o.label)}</option>`).join('')}
                         </select>`;
            } else if (f.type === 'file' || f.type === 'files') {
                input = `<div class="file-drop">
                            <input id="${id}" name="${f.name}" type="file" accept="image/*" ${f.type === 'files' ? 'multiple' : ''}>
                            <div class="previews" data-previews="${f.name}">${currentImages.map(src => `<img src="${esc(img(src))}" alt="">`).join('')}</div>
                         </div>`;
                if (isEdit) f = { ...f, help: (f.help ? f.help + ' ' : '') + 'Mövcud şəkil(lər)i saxlamaq üçün boş buraxın.' };
            } else {
                input = `<input id="${id}" name="${f.name}" class="input" type="${f.type}" value="${esc(v)}" ${f.max ? `maxlength="${f.max}"` : ''} ${f.step ? `step="${f.step}"` : ''} ${f.min ? `min="${f.min}"` : ''}>`;
            }
            return `<div class="field">${label}${input}${f.help ? `<small class="help">${esc(f.help)}</small>` : ''}<div class="field-error"></div></div>`;
        };

        openModal(`
            <form id="crudForm" novalidate>
                <div class="a-modal__head">
                    <h3>${isEdit ? 'Redaktə:' : 'Yeni'} ${esc(R.noun)}</h3>
                    <button type="button" class="icon-btn" data-close aria-label="Bağla"><i class="fa fa-times"></i></button>
                </div>
                <div class="a-modal__body">
                    <div class="alert" id="formAlert" role="alert"></div>
                    ${fields.map(fieldHtml).join('')}
                </div>
                <div class="a-modal__foot">
                    <button type="button" class="btn btn--ghost btn--sm" data-close>Ləğv et</button>
                    <button type="submit" class="btn btn--primary btn--sm"><i class="fa fa-check"></i> ${isEdit ? 'Yadda saxla' : 'Yarat'}</button>
                </div>
            </form>`);

        const form = document.getElementById('crudForm');

        // Seçilən şəkillərin önizləməsi
        qsa('input[type="file"]', form).forEach(input => {
            input.addEventListener('change', () => {
                const box = form.querySelector(`[data-previews="${input.name}"]`);
                box.innerHTML = '';
                Array.from(input.files).forEach(file => {
                    const im = document.createElement('img');
                    im.src = URL.createObjectURL(file);
                    box.appendChild(im);
                });
            });
        });

        form.addEventListener('submit', async e => {
            e.preventDefault();
            const alertBox = document.getElementById('formAlert');
            alertBox.className = 'alert';
            alertBox.textContent = '';
            const fd = new FormData();
            let valid = true;

            for (const f of fields) {
                const el = form.elements[f.name];
                const wrap = el.closest('.field');
                const err = wrap.querySelector('.field-error');
                let message = '';

                if (f.type === 'file' || f.type === 'files') {
                    const files = Array.from(el.files);
                    if (!files.length && f.requiredOnCreate && !isEdit) message = 'Şəkil seçin.';
                    const tooBig = files.find(x => x.size > 1024 * 1024);
                    if (tooBig) message = `"${tooBig.name}" 1 MB-dan böyükdür.`;
                    files.forEach(file => fd.append(f.name, file));
                } else {
                    const value = el.value.trim();
                    if (f.required && !value) message = 'Bu sahə mütləqdir.';
                    if (f.type === 'number' && value && (isNaN(Number(value)) || Number(value) < 0)) message = 'Düzgün müsbət ədəd yazın.';
                    fd.append(f.name, value);
                }
                wrap.classList.toggle('has-error', !!message);
                err.textContent = message;
                if (message) valid = false;
            }
            if (!valid) return;

            const btn = form.querySelector('button[type="submit"]');
            btn.disabled = true;
            btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Saving...';
            try {
                if (isEdit) await A.edit(R.ctrl, row.id, fd);
                else await A.create(R.ctrl, fd);
                closeModal();
                toast(isEdit ? 'Dəyişikliklər yadda saxlanıldı' : `${R.noun[0].toUpperCase() + R.noun.slice(1)} yaradıldı`);
                onSaved();
            } catch (err) {
                alertBox.className = 'alert alert--error';
                alertBox.textContent = err.message;
                btn.disabled = false;
                btn.innerHTML = `<i class="fa fa-check"></i> ${isEdit ? 'Yadda saxla' : 'Yarat'}`;
            }
        });
    }

    /* ---------------- Router ---------------- */
    const TITLES = { dashboard: 'İdarə paneli', orders: 'Sifarişlər', users: 'İstifadəçilər' };
    function route() {
        const key = (location.hash || '#dashboard').slice(1);
        const known = key in RESOURCES || key in TITLES;
        const current = known ? key : 'dashboard';
        qsa('[data-route]').forEach(a => a.classList.toggle('is-active', a.dataset.route === current));
        pageTitle.textContent = TITLES[current] || RESOURCES[current].title;
        document.title = `${pageTitle.textContent} | Cake Shop Admin`;
        document.body.classList.remove('side-open');
        closeModal();

        if (current === 'dashboard') viewDashboard();
        else if (current === 'orders') viewOrders();
        else if (current === 'users') viewUsers();
        else viewResource(current);
    }

    window.addEventListener('hashchange', route);
    route();
})();
