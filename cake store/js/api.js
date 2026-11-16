/* ==========================================================
   Cake Shop — Backend (ASP.NET Core API) ilə əlaqə qatı
   Bütün səhifələr (sayt + admin panel) bu faylı ilk yükləyir.
   ========================================================== */
'use strict';

const CONFIG = {
    // Backend ünvanı (FinalProject/Properties/launchSettings.json -> "FinalProject" profili)
    API_URL: 'https://localhost:7077',
    TOKEN_KEY: 'cakestore_token'
};

/* ---------------- Köməkçilər ---------------- */

function imageUrl(fileName, fallback = 'img/shop/product-1.jpg') {
    if (!fileName) return fallback;
    if (/^https?:\/\//i.test(fileName)) return fileName;
    return `${CONFIG.API_URL}/images/${encodeURIComponent(fileName)}`;
}

// API-dən gələn mətni HTML-ə təhlükəsiz yerləşdirmək üçün
function esc(value) {
    return String(value ?? '')
        .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
}

function money(value) {
    return Number(value || 0).toFixed(2) + ' ₼';
}

function asArray(data) {
    if (Array.isArray(data)) return data;
    return data == null ? [] : [data];
}

function formatDate(value) {
    if (!value) return '';
    const d = new Date(value);
    return isNaN(d) ? '' : d.toLocaleDateString('az-Latn-AZ', { day: 'numeric', month: 'short', year: 'numeric' });
}

// Backend sifariş statuslarını ingiliscə saxlayır — ekranda Azərbaycan dilində göstəririk
const STATUS_LABELS = {
    Pending: 'Gözləyir',
    Paid: 'Ödənilib',
    Preparing: 'Hazırlanır',
    Delivered: 'Çatdırılıb',
    Cancelled: 'Ləğv edilib'
};
function statusLabel(status) {
    return STATUS_LABELS[status] || status;
}

function qs(sel, root = document) { return root.querySelector(sel); }
function qsa(sel, root = document) { return Array.from(root.querySelectorAll(sel)); }

/* ---------------- HTTP ---------------- */

class ApiError extends Error {
    constructor(message, status, body) {
        super(message);
        this.status = status;
        this.body = body;
    }
}

function extractError(body, status) {
    if (!body) return `Xəta baş verdi (${status})`;
    if (typeof body === 'string') return body;
    const msg = body.message || body.Message;
    if (msg) return msg;
    const rm = body.responseMessage || body.ResponseMessage;
    if (Array.isArray(rm) && rm.length) return rm.join('\n');
    if (typeof rm === 'string' && rm) return rm;
    if (body.errors) {
        const list = Object.values(body.errors).flat();
        if (list.length) return list.join('\n');
    }
    if (body.title) return body.title;
    return `Xəta baş verdi (${status})`;
}

/**
 * api(path, { method, query, json, form, auth })
 *  - query: URL parametrləri
 *  - json:  JSON body ([FromBody])
 *  - form:  FormData ([FromForm], şəkil yükləmə)
 */
async function api(path, { method = 'GET', query, json, form, auth = false } = {}) {
    const url = new URL(CONFIG.API_URL + path);
    if (query) {
        Object.entries(query).forEach(([k, v]) => {
            if (v !== undefined && v !== null && v !== '') url.searchParams.append(k, v);
        });
    }

    const headers = { Accept: 'application/json' };
    let body;
    if (json !== undefined) {
        headers['Content-Type'] = 'application/json';
        body = JSON.stringify(json);
    } else if (form) {
        body = form; // brauzer multipart boundary-ni özü qoyur
    }

    if (auth) {
        const token = Auth.getToken();
        if (!token) throw new ApiError('Əvvəlcə daxil olun.', 401);
        headers.Authorization = `Bearer ${token}`;
    }

    let response;
    try {
        response = await fetch(url, { method, headers, body });
    } catch {
        throw new ApiError('Serverə qoşulmaq mümkün olmadı. Backend işləyirmi?', 0);
    }

    const text = await response.text();
    let data = text;
    try { data = text ? JSON.parse(text) : null; } catch { /* düz mətn */ }

    if (!response.ok) {
        if (response.status === 401 && auth) Auth.logout(false);
        if (response.status === 403 && auth && !data) data = 'Bu əməliyyat üçün icazəniz yoxdur.';
        throw new ApiError(extractError(data, response.status), response.status, data);
    }
    return data;
}

/* ---------------- Auth (JWT) ---------------- */

const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

const Auth = {
    getToken() {
        try { return localStorage.getItem(CONFIG.TOKEN_KEY); } catch { return null; }
    },
    setToken(token) {
        try { localStorage.setItem(CONFIG.TOKEN_KEY, token); } catch { /* ignore */ }
    },
    payload() {
        const token = this.getToken();
        if (!token) return null;
        try {
            const part = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
            const json = decodeURIComponent(atob(part).split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join(''));
            return JSON.parse(json);
        } catch {
            return null;
        }
    },
    user() {
        const p = this.payload();
        if (!p) return null;
        if ((p.exp && p.exp * 1000 < Date.now()) || !p.uid) {
            this.logout(false);
            return null;
        }
        const roles = asArray(p[ROLE_CLAIM] || p.role);
        return { id: p.uid, username: p.sub, fullName: p.fullName || p.sub, roles, isAdmin: roles.includes('Admin') };
    },
    isLoggedIn() { return !!this.user(); },
    isAdmin() { const u = this.user(); return !!(u && u.isAdmin); },
    logout(redirect = true, to = 'index.html') {
        try { localStorage.removeItem(CONFIG.TOKEN_KEY); } catch { /* ignore */ }
        if (redirect) window.location.href = to;
    },
    // Daxil olmayıbsa login səhifəsinə göndərir və geri qayıdış ünvanını saxlayır
    requireLogin(back) {
        if (this.isLoggedIn()) return true;
        const target = back || (location.pathname.split('/').pop() + location.search);
        window.location.href = `login.html?returnUrl=${encodeURIComponent(target)}`;
        return false;
    }
};

/* ---------------- Endpoint-lər (sayt) ---------------- */

const Api = {
    sliders: () => api('/api/ui/Slider/GetAll'),
    abouts: () => api('/api/ui/About/GetAll'),
    categories: () => api('/api/ui/Category/GetAll'),
    teams: () => api('/api/ui/Team/GetAll'),
    testimonials: () => api('/api/ui/Testimonial/GetAll'),
    instagrams: () => api('/api/ui/Instagram/GetAll'),
    blogs: () => api('/api/ui/Blog/GetAll'),
    settings: () => api('/api/ui/Setting/GetAll'),
    contactSettings: () => api('/api/ui/ContactSetting/GetAll'),

    products: () => api('/api/ui/Product/GetAll'),
    product: (id) => api(`/api/ui/Product/GetById/${encodeURIComponent(id)}`),
    productsPaged: (page, take) => api('/api/ui/Product/GetPaginateDatas', { query: { page, take } }),
    searchProducts: (name) => api('/api/ui/Product/Search', { query: { name } }),
    filterProducts: (categoryName) => api('/api/ui/Product/Filter', { query: { categoryName } }),
    sortProducts: (sortKey, isDescending) => api('/api/ui/Product/SortBy', { query: { sortKey, isDescending } }),

    sendContact: (name, email, message) =>
        api('/api/ui/Contact/Create', { method: 'POST', query: { Name: name, Email: email, Message: message } }),
    subscribe: (email) =>
        api('/api/ui/Subscribe/AddSubscribe', { method: 'POST', query: { Email: email } }),

    signIn: (emailOrUsername, password) =>
        api('/api/ui/Account/SignIn', { method: 'POST', query: { EmailOrUsername: emailOrUsername, Password: password } }),
    signUp: (data) => api('/api/ui/Account/SignUp', { method: 'POST', query: data }),
    verifyEmail: (email, token) =>
        api('/api/ui/Account/VerifyEmail', { method: 'POST', query: { verifyEmail: email, token } }),
    forgetPassword: (email) =>
        api('/api/ui/Account/ForgetPassword', { method: 'POST', query: { email } }),
    resetPassword: (data) => api('/api/ui/Account/ResetPassword', { method: 'POST', query: data }),

    getBasket: (userId) => api(`/api/ui/Basket/GetBasketByUserId/${encodeURIComponent(userId)}`, { auth: true }),
    addToBasket: (userId, productId) =>
        api('/api/ui/Basket/AddBasket', { method: 'POST', auth: true, query: { UserId: userId, ProductId: productId } }),
    increaseQty: (userId, productId) =>
        api('/api/ui/Basket/IncreaseQuantity', { method: 'POST', auth: true, query: { UserId: userId, ProductId: productId } }),
    decreaseQty: (userId, productId) =>
        api('/api/ui/Basket/DecreaseQuantity', { method: 'POST', auth: true, query: { UserId: userId, ProductId: productId } }),
    removeFromBasket: (userId, productId) =>
        api('/api/ui/Basket/DeleteProductFromBasket', { method: 'DELETE', auth: true, query: { productId, userId } }),

    getWishlist: (userId) => api(`/api/ui/Wishlist/GetWishlistByUserId/${encodeURIComponent(userId)}`, { auth: true }),
    addToWishlist: (userId, productId) =>
        api('/api/ui/Wishlist/AddWishlist', { method: 'POST', auth: true, query: { AppUserId: userId, ProductId: productId } }),
    removeFromWishlist: (userId, productId) =>
        api('/api/ui/Wishlist/DeleteProductFromWishlist', { method: 'DELETE', auth: true, query: { userId, productId } }),

    checkout: (data) => api('/api/ui/Order/Checkout', { method: 'POST', auth: true, json: data }),
    confirmPayment: (sessionId) => api('/api/ui/Order/Confirm', { method: 'POST', auth: true, query: { sessionId } }),
    myOrders: () => api('/api/ui/Order/MyOrders', { auth: true })
};

/* ---------------- Toast ---------------- */

function toast(message, type = 'success') {
    let box = document.getElementById('toasts');
    if (!box) {
        box = document.createElement('div');
        box.id = 'toasts';
        box.setAttribute('role', 'status');
        box.setAttribute('aria-live', 'polite');
        document.body.appendChild(box);
    }
    const icons = { success: 'fa-check-circle', error: 'fa-exclamation-circle', info: 'fa-info-circle' };
    const el = document.createElement('div');
    el.className = `toast toast--${type}`;
    el.innerHTML = `<i class="fa ${icons[type] || icons.info}" aria-hidden="true"></i><span></span>`;
    el.querySelector('span').textContent = message;
    box.appendChild(el);
    setTimeout(() => el.classList.add('is-hiding'), 3200);
    setTimeout(() => el.remove(), 3600);
}
