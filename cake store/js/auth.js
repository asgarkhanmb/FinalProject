/* ==========================================================
   Cake Shop — Login / Register / Verify / Forgot / Reset
   Canlı validasiya, şifrəni göstər/gizlət, şifrə gücü göstəricisi.
   Asılılıq: js/api.js, js/layout.js
   ========================================================== */
'use strict';

(function () {
    const params = new URLSearchParams(location.search);

    // Yalnız saytın öz səhifələrinə qayıtmağa icazə (open redirect olmasın)
    function returnUrl() {
        const url = params.get('returnUrl');
        return url && /^(admin\/)?[\w\-]+\.html(\?[^#]*)?$/.test(url) ? url : 'index.html';
    }

    function setAlert(box, text, type = 'error') {
        if (!box) return;
        box.className = `alert alert--${type}`;
        box.textContent = text;
    }

    function setBusy(btn, busy, text) {
        if (!btn.dataset.label) btn.dataset.label = btn.innerHTML;
        btn.disabled = busy;
        btn.innerHTML = busy ? `<i class="fa fa-spinner fa-spin"></i> ${text}` : btn.dataset.label;
    }

    // Sahə üçün validasiya: rule(value) true və ya xəta mətni qaytarır
    function makeField(input, rule) {
        const wrap = input.closest('.field');
        const errorEl = wrap.querySelector('.field-error');
        let touched = false;
        const run = (show = touched) => {
            const result = rule(input.value);
            const ok = result === true;
            if (show) {
                wrap.classList.toggle('has-error', !ok);
                wrap.classList.toggle('is-valid', ok && input.value !== '');
                errorEl.textContent = ok ? '' : result;
            }
            return ok;
        };
        input.addEventListener('blur', () => { touched = true; run(); });
        input.addEventListener('input', () => { if (touched) run(); });
        return { validate: () => { touched = true; return run(true); }, input };
    }

    // Şifrəni göstər/gizlət düymələri
    document.querySelectorAll('[data-toggle-pass]').forEach(btn => {
        btn.addEventListener('click', () => {
            const input = btn.parentElement.querySelector('input');
            const show = input.type === 'password';
            input.type = show ? 'text' : 'password';
            btn.querySelector('.fa').className = `fa ${show ? 'fa-eye-slash' : 'fa-eye'}`;
            btn.setAttribute('aria-label', show ? 'Şifrəni gizlət' : 'Şifrəni göstər');
        });
    });

    const emailRule = v => /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(v.trim()) || 'Düzgün e-poçt ünvanı yazın.';

    // Backend-in Identity qaydaları ilə eyni
    const PASS_RULES = [
        { key: 'len', test: v => v.length >= 6, text: 'Ən azı 6 simvol' },
        { key: 'upper', test: v => /[A-Z]/.test(v), text: 'Böyük hərf' },
        { key: 'lower', test: v => /[a-z]/.test(v), text: 'Kiçik hərf' },
        { key: 'digit', test: v => /\d/.test(v), text: 'Rəqəm' },
        { key: 'symbol', test: v => /[^A-Za-z0-9]/.test(v), text: 'Simvol (məs. !@#)' }
    ];
    const passwordRule = v => PASS_RULES.every(r => r.test(v)) || 'Şifrə aşağıdakı tələblərə cavab vermir.';

    function bindStrength(input) {
        const meter = document.querySelector('[data-strength]');
        const rules = document.querySelector('[data-rules]');
        if (!meter || !rules) return;
        rules.innerHTML = PASS_RULES.map(r => `<li data-rule="${r.key}">${r.text}</li>`).join('');
        const update = () => {
            const v = input.value;
            const passed = PASS_RULES.filter(r => r.test(v)).length;
            meter.dataset.score = v ? Math.max(1, Math.round(passed * 4 / PASS_RULES.length)) : 0;
            PASS_RULES.forEach(r => rules.querySelector(`[data-rule="${r.key}"]`).classList.toggle('ok', r.test(v)));
        };
        input.addEventListener('input', update);
        update();
    }

    /* ---------------- LOGIN ---------------- */
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        if (Auth.isLoggedIn()) location.replace(returnUrl());
        const alertBox = loginForm.querySelector('[data-alert]');
        const id = makeField(loginForm.querySelector('#loginId'), v => v.trim().length > 0 || 'İstifadəçi adı və ya e-poçt yazın.');
        const pass = makeField(loginForm.querySelector('#loginPass'), v => v.length > 0 || 'Şifrənizi yazın.');

        if (params.get('registered')) setAlert(alertBox, 'Hesab yaradıldı! Daxil olun.', 'success');
        if (params.get('returnUrl')) setAlert(alertBox, 'Davam etmək üçün daxil olun.', 'info');

        loginForm.addEventListener('submit', async e => {
            e.preventDefault();
            if (![id.validate(), pass.validate()].every(Boolean)) return;
            const btn = loginForm.querySelector('button[type="submit"]');
            setBusy(btn, true, 'Daxil olunur...');
            try {
                const res = await Api.signIn(id.input.value.trim(), pass.input.value);
                if (!res || !res.token) throw new Error((res && res.error) || 'Daxil olmaq alınmadı.');
                Auth.setToken(res.token);
                setAlert(alertBox, 'Xoş gəldiniz! Yönləndirilir...', 'success');
                location.href = returnUrl();
            } catch (err) {
                setAlert(alertBox, err.message);
                setBusy(btn, false);
            }
        });
    }

    /* ---------------- REGISTER ---------------- */
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        if (Auth.isLoggedIn()) location.replace('index.html');
        const alertBox = registerForm.querySelector('[data-alert]');
        const passInput = registerForm.querySelector('#regPass');
        const f = {
            name: makeField(registerForm.querySelector('#regName'), v => v.trim().length >= 2 || 'Ad soyadınızı yazın.'),
            user: makeField(registerForm.querySelector('#regUser'), v =>
                /^[A-Za-z0-9._-]{3,30}$/.test(v.trim()) || '3–30 simvol: hərf, rəqəm, . _ -'),
            email: makeField(registerForm.querySelector('#regEmail'), emailRule),
            pass: makeField(passInput, passwordRule),
            pass2: makeField(registerForm.querySelector('#regPass2'), v => (v && v === passInput.value) || 'Şifrələr eyni deyil.')
        };
        bindStrength(passInput);

        registerForm.addEventListener('submit', async e => {
            e.preventDefault();
            if (!Object.values(f).map(x => x.validate()).every(Boolean)) return;
            const btn = registerForm.querySelector('button[type="submit"]');
            setBusy(btn, true, 'Hesab yaradılır...');
            const data = {
                FullName: f.name.input.value.trim(),
                Username: f.user.input.value.trim(),
                Email: f.email.input.value.trim(),
                Password: f.pass.input.value
            };
            try {
                const res = await Api.signUp(data);
                if (res && res.requiresEmailConfirmation) {
                    registerForm.reset();
                    setAlert(alertBox, `Hesab yaradıldı! ${data.Email} ünvanına təsdiq linki göndərdik. E-poçtunuzu təsdiqləyin, sonra daxil olun.`, 'success');
                    setBusy(btn, false);
                    return;
                }
                // E-poçt təsdiqi tələb olunmursa avtomatik daxil oluruq
                const login = await Api.signIn(data.Username, data.Password);
                Auth.setToken(login.token);
                toast('Cake Shop-a xoş gəldiniz!');
                location.href = returnUrl();
            } catch (err) {
                setAlert(alertBox, err.message);
                setBusy(btn, false);
            }
        });
    }

    /* ---------------- VERIFY EMAIL ---------------- */
    const verifyBox = document.getElementById('verifyBox');
    if (verifyBox) {
        const email = params.get('email');
        const token = params.get('token');
        const render = (kind, icon, title, text, action = '') => {
            verifyBox.innerHTML = `<div class="status-icon status-icon--${kind}"><i class="fa ${icon}"></i></div>
                                   <h1 style="font-size:32px">${title}</h1><p class="muted">${text}</p>${action}`;
        };
        if (!email || !token) {
            render('error', 'fa-times', 'Etibarsız link', 'Bu təsdiq linki etibarsızdır və ya natamamdır.', '<a href="index.html" class="btn btn--dark">Ana səhifəyə qayıt</a>');
        } else {
            render('info', 'fa-spinner fa-spin', 'E-poçtunuz təsdiqlənir...', 'Bir az gözləyin.');
            Api.verifyEmail(email, token)
                .then(() => render('success', 'fa-check', 'E-poçt təsdiqləndi!', 'Hesabınız hazırdır. İndi daxil ola bilərsiniz.', '<a href="login.html" class="btn btn--primary">Daxil ol</a>'))
                .catch(err => render('error', 'fa-times', 'Təsdiqləmə alınmadı', esc(err.message), '<a href="index.html" class="btn btn--dark">Ana səhifəyə qayıt</a>'));
        }
    }

    /* ---------------- FORGOT PASSWORD ---------------- */
    const forgotForm = document.getElementById('forgotForm');
    if (forgotForm) {
        const alertBox = forgotForm.querySelector('[data-alert]');
        const email = makeField(forgotForm.querySelector('#fpEmail'), emailRule);
        forgotForm.addEventListener('submit', async e => {
            e.preventDefault();
            if (!email.validate()) return;
            const btn = forgotForm.querySelector('button[type="submit"]');
            setBusy(btn, true, 'Göndərilir...');
            try {
                await Api.forgetPassword(email.input.value.trim());
                setAlert(alertBox, 'Bu e-poçt qeydiyyatdadırsa, şifrə yeniləmə linki göndərildi. Poçt qutunuzu yoxlayın.', 'success');
            } catch (err) {
                setAlert(alertBox, err.message);
            } finally {
                setBusy(btn, false);
            }
        });
    }

    /* ---------------- RESET PASSWORD ---------------- */
    const resetForm = document.getElementById('resetForm');
    if (resetForm) {
        const alertBox = resetForm.querySelector('[data-alert]');
        const email = params.get('email');
        const token = params.get('token');
        if (!email || !token) setAlert(alertBox, 'Bu link etibarsızdır və ya natamamdır. Yeni link istəyin.');

        const passInput = resetForm.querySelector('#rpPass');
        const pass = makeField(passInput, passwordRule);
        const pass2 = makeField(resetForm.querySelector('#rpPass2'), v => (v && v === passInput.value) || 'Şifrələr eyni deyil.');
        bindStrength(passInput);

        resetForm.addEventListener('submit', async e => {
            e.preventDefault();
            if (![pass.validate(), pass2.validate()].every(Boolean)) return;
            const btn = resetForm.querySelector('button[type="submit"]');
            setBusy(btn, true, 'Yadda saxlanılır...');
            try {
                await Api.resetPassword({ Email: email, Token: token, Password: pass.input.value, ConfirmPassword: pass2.input.value });
                setAlert(alertBox, 'Şifrəniz yeniləndi. Giriş səhifəsinə yönləndirilir...', 'success');
                setTimeout(() => { location.href = 'login.html'; }, 1500);
            } catch (err) {
                setAlert(alertBox, err.message);
                setBusy(btn, false);
            }
        });
    }
})();
