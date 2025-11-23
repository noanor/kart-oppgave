// ===== DUAL NAVBAR SYSTEM =====
// Håndterer visning av PC-navbar vs iPad-navbar basert på skjermstørrelse
// og layout-type for siden

// Funksjon for å oppdatere navbar-visning basert på skjermstørrelse og layout-type
function updateNavbar() {
    const navbarPC = document.getElementById("navbarPC");
    const navbarIPAD = document.getElementById("navbarIPAD");
    
    if (!navbarPC || !navbarIPAD) return;
    
    // Henter layout-type fra body-elementet (satt av _Layout.cshtml)
    const layoutType = document.body.getAttribute('data-layout-type') || 'auto';
    const windowWidth = window.innerWidth;
    
    // Hvis layout-type er satt til 'pc', vis alltid PC-navbar
    if (layoutType === 'pc') {
        navbarPC.style.display = "flex";
        navbarIPAD.style.display = "none";
        return;
    }
    
    // Hvis layout-type er satt til 'ipad', vis alltid iPad-navbar
    if (layoutType === 'ipad') {
        navbarPC.style.display = "none";
        navbarIPAD.style.display = "block";
        return;
    }
    
    // Auto-modus: bytt basert på skjermstørrelse (1024px er breakpoint)
    if (windowWidth <= 1024) {
        navbarPC.style.display = "none";
        navbarIPAD.style.display = "block";
    } else {
        navbarPC.style.display = "flex";
        navbarIPAD.style.display = "none";
    }
}

// Oppdater ved resize (med debounce for bedre ytelse)
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(updateNavbar, 100);
});

// Kjør ved initialisering
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', updateNavbar);
} else {
    // DOM er allerede lastet
    updateNavbar();
}