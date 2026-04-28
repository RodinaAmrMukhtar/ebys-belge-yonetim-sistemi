(() => {
    const app = document.getElementById("ebysApp");
    const backdrop = document.getElementById("ebysBackdrop");

    const btnCollapse = document.getElementById("btnSbCollapse");
    const btnOpen = document.getElementById("btnSbOpen");
    const btnMockRefresh = document.getElementById("btnMockRefresh");

    const crumbStep = document.getElementById("crumbStep");
    const crumbHint = document.getElementById("crumbHint");
    const dashStatus = document.getElementById("dashStatus");
    const dashClock = document.getElementById("dashClock");
    const dashTitle = document.getElementById("dashTitle");

    // Public helpers
    window.EBYS_DASH = window.EBYS_DASH || {};
    window.EBYS_DASH.rebuild = () => rebuildMock();
    window.EBYS_DASH.toast = (msg) => toast(msg);

    // -------------------------
    // Sidebar (collapse + mobile open)
    // -------------------------
    const LS_COLLAPSED = "ebys_sb_collapsed_v3";

    function setCollapsed(on) {
        app.classList.toggle("is-collapsed", !!on);
        localStorage.setItem(LS_COLLAPSED, String(!!on));
    }

    function setMobileOpen(on) {
        app.classList.toggle("is-sbopen", !!on);
    }

    btnCollapse?.addEventListener("click", () => {
        const now = app.classList.contains("is-collapsed");
        setCollapsed(!now);
    });

    btnOpen?.addEventListener("click", () => setMobileOpen(true));
    backdrop?.addEventListener("click", () => setMobileOpen(false));

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") setMobileOpen(false);
    });

    // restore collapse
    setCollapsed(localStorage.getItem(LS_COLLAPSED) === "true");
    setMobileOpen(false);

    // -------------------------
    // Optional: refresh badges from /Dashboard/Counts (safe if endpoint exists)
    // -------------------------
    async function refreshBadges() {
        try {
            const res = await fetch("/Dashboard/Counts", { headers: { "X-Requested-With": "XMLHttpRequest" } });
            if (!res.ok) return;
            const data = await res.json();

            const bi = document.getElementById("b-inbox");
            const bd = document.getElementById("b-drafts");   // ✅ new
            const bo = document.getElementById("b-outbox");
            const ba = document.getElementById("b-archive");

            if (bi) bi.textContent = data.inbox ?? 0;
            if (bd) bd.textContent = data.drafts ?? 0;        // ✅ new
            if (bo) bo.textContent = data.outbox ?? 0;
            if (ba) ba.textContent = data.archive ?? 0;
        } catch {
            // ignore
        }
    }
    refreshBadges();
    setInterval(refreshBadges, 9000);

    // -------------------------
    // AJAX nav
    // -------------------------
    function skeletonHtml() {
        return `
      <div class="p-2">
        <div style="height:16px;width:45%;border-radius:10px;background:rgba(15,23,42,.08)" class="mb-2"></div>
        <div style="height:16px;width:70%;border-radius:10px;background:rgba(15,23,42,.08)" class="mb-2"></div>
        <div style="height:16px;width:60%;border-radius:10px;background:rgba(15,23,42,.08)" class="mb-2"></div>
        <div style="height:16px;width:30%;border-radius:10px;background:rgba(15,23,42,.08)" class="mb-2"></div>
      </div>
    `;
    }

    function wireAjaxLinks() {
        document.querySelectorAll(".ajax").forEach(a => {
            a.addEventListener("click", async (e) => {
                e.preventDefault();

                const url = a.dataset.url;
                const title = a.dataset.title || "Sayfa";

                if (dashTitle) dashTitle.textContent = title;  // ✅ update top title too
                if (crumbStep) crumbStep.textContent = title;
                if (crumbHint) crumbHint.textContent = "İçerik";

                document.querySelectorAll(".ebys-navlink").forEach(x => x.classList.remove("active"));
                a.classList.add("active");

                const content = document.getElementById("content");
                if (!content) return;

                content.classList.add("is-loading");
                content.innerHTML = skeletonHtml();

                try {
                    const res = await fetch(url, { headers: { "X-Requested-With": "XMLHttpRequest" } });
                    const html = await res.text();
                    content.innerHTML = html;

                    refreshBadges();
                    setMobileOpen(false);
                } catch {
                    content.innerHTML = `<div class="alert alert-danger">Sayfa yüklenemedi.</div>`;
                } finally {
                    content.classList.remove("is-loading");
                }
            });
        });
    }
    wireAjaxLinks();

    // -------------------------
    // Clock + status line
    // -------------------------
    function tick() {
        const now = new Date();
        if (dashClock) dashClock.textContent = now.toLocaleString("tr-TR");
    }
    tick();
    setInterval(tick, 20000);

    function setStatus(text) {
        if (!dashStatus) return;
        dashStatus.innerHTML = text;
    }

    // -------------------------
    // Mock data + mini charts
    // -------------------------
    const mock = {
        seed: Date.now(),
        inbox: [],
        outbox: [],
        archive: [],
        weeklyIn: [],
        weeklyOut: [],
        recentDocs: [],
        notifs: [],
        pending: []
    };

    function rand() {
        // xorshift32
        let x = (mock.seed | 0) || 123456789;
        x ^= x << 13; x ^= x >>> 17; x ^= x << 5;
        mock.seed = x;
        return (x >>> 0) / 4294967296;
    }

    function pick(arr) { return arr[Math.floor(rand() * arr.length)]; }

    function genSeries(n, base, swing) {
        let v = base;
        const a = [];
        for (let i = 0; i < n; i++) {
            v += (rand() - 0.5) * swing;
            v = Math.max(0, v);
            a.push(Math.round(v));
        }
        return a;
    }

    function rowHtml(icon, title, sub, status) {
        return `
      <div class="ebys-row">
        <div class="left"><i class="bi ${icon}"></i></div>
        <div class="mid">
          <div class="t">${escapeHtml(title)}</div>
          <div class="s">${escapeHtml(sub)}</div>
        </div>
        <div class="right">
          <span class="ebys-status ${status?.cls || ""}">${escapeHtml(status?.t || "—")}</span>
        </div>
      </div>
    `;
    }

    function renderSparkline(container, data) {
        if (!container) return;

        const w = container.clientWidth || 320;
        const h = container.clientHeight || 44;

        const pad = 6;
        const min = Math.min(...data);
        const max = Math.max(...data);
        const span = (max - min) || 1;

        const step = (w - pad * 2) / (data.length - 1);
        let d = "";

        data.forEach((v, i) => {
            const x = pad + i * step;
            const y = pad + (h - pad * 2) * (1 - (v - min) / span);
            d += (i === 0 ? "M" : "L") + x.toFixed(1) + " " + y.toFixed(1) + " ";
        });

        const svg = `
      <svg viewBox="0 0 ${w} ${h}" width="${w}" height="${h}" preserveAspectRatio="none">
        <defs>
          <linearGradient id="g" x1="0" x2="1">
            <stop offset="0" stop-color="rgba(13,110,253,.85)"/>
            <stop offset="1" stop-color="rgba(22,163,74,.75)"/>
          </linearGradient>
        </defs>
        <path d="${d}" fill="none" stroke="url(#g)" stroke-width="2.4" stroke-linecap="round"/>
      </svg>
    `;
        container.innerHTML = svg;
    }

    function renderWeekly(el, a, b) {
        if (!el) return;

        const w = el.clientWidth || 420;
        const h = el.clientHeight || 180;
        const pad = 10;
        const n = Math.min(a.length, b.length);

        const max = Math.max(...a, ...b, 1);
        const bw = (w - pad * 2) / n;
        const gap = Math.max(6, bw * 0.18);

        let bars = "";
        for (let i = 0; i < n; i++) {
            const x = pad + i * bw;
            const ah = (h - pad * 2) * (a[i] / max);
            const bh = (h - pad * 2) * (b[i] / max);

            bars += `
        <rect x="${x + gap}" y="${h - pad - ah}" width="${(bw - gap * 2) / 2}" height="${ah}"
              rx="8" fill="rgba(13,110,253,.65)"/>
        <rect x="${x + gap + (bw - gap * 2) / 2 + 6}" y="${h - pad - bh}" width="${(bw - gap * 2) / 2}" height="${bh}"
              rx="8" fill="rgba(22,163,74,.60)"/>
      `;
        }

        el.innerHTML = `
      <svg viewBox="0 0 ${w} ${h}" width="${w}" height="${h}" preserveAspectRatio="none">
        ${bars}
      </svg>
    `;
    }

    function renderAll() {
        const recent = document.getElementById("recentDocsList");
        const pending = document.getElementById("pendingSignList");
        const notifs = document.getElementById("notifList");

        if (recent) recent.innerHTML = mock.recentDocs.map(d => rowHtml(d.status.icon, d.title, d.sub, d.status)).join("");
        if (pending) pending.innerHTML = mock.pending.map(d => rowHtml(d.status.icon, d.title, d.sub, d.status)).join("");
        if (notifs) notifs.innerHTML = mock.notifs.map(n => `
      <div class="ebys-note">
        <div class="t"><i class="bi bi-dot me-1"></i>${escapeHtml(n.title)}</div>
        <div class="s">${escapeHtml(n.sub)}</div>
      </div>
    `).join("");

        document.querySelectorAll("[data-sparkline]").forEach(el => {
            const key = el.getAttribute("data-sparkline");
            const series = mock[key] || mock.inbox;
            renderSparkline(el, series);
        });

        const weekly = document.getElementById("weeklyChart");
        if (weekly) renderWeekly(weekly, mock.weeklyIn, mock.weeklyOut);
    }

    function rebuildMock() {
        const statuses = [
            { t: "Onaylandı", cls: "ok", icon: "bi-check2-circle" },
            { t: "Bekliyor", cls: "warn", icon: "bi-hourglass-split" },
            { t: "İade", cls: "bad", icon: "bi-arrow-return-left" }
        ];

        const topics = ["Görevlendirme", "Dilekçe", "Yazı", "Talep", "Bilgilendirme", "Tutanak"];
        const units = ["Fakülte", "Dekanlık", "Bölüm", "Öğrenci İşleri", "Rektörlük", "Enstitü"];
        const prio = ["Düşük", "Orta", "Yüksek"];

        mock.inbox = genSeries(18, 12 + rand() * 8, 6);
        mock.outbox = genSeries(18, 10 + rand() * 10, 7);
        mock.archive = genSeries(18, 6 + rand() * 6, 4);

        mock.weeklyIn = genSeries(7, 20 + rand() * 20, 16);
        mock.weeklyOut = genSeries(7, 16 + rand() * 18, 14);

        mock.recentDocs = Array.from({ length: 6 }).map(() => {
            const st = pick(statuses);
            return {
                title: `${pick(topics)} • ${pick(units)} (${Math.floor(rand() * 90) + 10})`,
                sub: `Konu: ${pick(["Ders", "Staj", "İzin", "Burs", "Yaz okulu", "Sınav"])}`,
                status: st
            };
        });

        mock.pending = Array.from({ length: 5 }).map(() => ({
            title: `İmza Bekliyor • ${pick(units)}`,
            sub: `Öncelik: ${pick(prio)} • Süre: ${Math.floor(rand() * 7) + 1} gün`,
            status: pick(statuses)
        }));

        mock.notifs = Array.from({ length: 5 }).map(() => {
            const kind = pick(["Onay", "İade", "Arşiv", "Hatırlatma"]);
            return {
                title: `${kind} bildirimi`,
                sub: pick([
                    "Belge işlem gördü.",
                    "İmzacı yorum ekledi.",
                    "Arşive taşındı.",
                    "Eksik alan uyarısı."
                ])
            };
        });

        renderAll();
        refreshBadges();
        setStatus(`✔ Mock yenilendi <span class="muted">(az önce)</span>`);
    }

    document.addEventListener("click", (e) => {
        const btn = e.target.closest('[data-action="refresh"]');
        if (!btn) return;
        rebuildMock();
    });

    btnMockRefresh?.addEventListener("click", () => rebuildMock());

    document.getElementById("btnDemoToast")?.addEventListener("click", () => {
        toast("Yeni bildirim (demo).");
    });

    setStatus("—");
    rebuildMock();

    function toast(text) {
        let host = document.getElementById("ebysToastHost");
        if (!host) {
            host = document.createElement("div");
            host.id = "ebysToastHost";
            host.style.position = "fixed";
            host.style.right = "16px";
            host.style.bottom = "16px";
            host.style.zIndex = "9999";
            host.style.display = "grid";
            host.style.gap = "10px";
            document.body.appendChild(host);
        }

        const el = document.createElement("div");
        el.style.padding = "10px 12px";
        el.style.borderRadius = "14px";
        el.style.background = "rgba(255,255,255,.92)";
        el.style.border = "1px solid rgba(15,23,42,.14)";
        el.style.boxShadow = "0 18px 50px rgba(0,0,0,.18)";
        el.style.backdropFilter = "blur(12px)";
        el.style.fontWeight = "800";
        el.innerHTML = `<i class="bi bi-bell me-2"></i>${escapeHtml(text)}`;

        host.appendChild(el);
        setTimeout(() => el.remove(), 2600);
    }

    function escapeHtml(s) {
        return String(s ?? "").replace(/[&<>"']/g, m => ({
            "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#039;"
        }[m]));
    }
})();
