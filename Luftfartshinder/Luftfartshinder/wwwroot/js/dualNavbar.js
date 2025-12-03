// ===== DUAL NAVBAR SYSTEM =====
// Handles display of PC-navbar vs iPad-navbar based on screen size
// and layout-type for the page

// Function to update navbar display based on screen size and layout-type
function updateNavbar() {
    const navbarPC = document.getElementById("navbarPC");
    const navbarIPAD = document.getElementById("navbarIPAD");
    
    if (!navbarPC || !navbarIPAD) return;
    
    // Get layout-type from body element (set by _Layout.cshtml)
    const layoutType = document.body.getAttribute('data-layout-type') || 'auto';
    const windowWidth = window.innerWidth;
    
    // If layout-type is set to 'pc', always show PC-navbar
    if (layoutType === 'pc') {
        navbarPC.style.display = "flex";
        navbarIPAD.style.display = "none";
        return;
    }
    
    // If layout-type is set to 'ipad', always show iPad-navbar
    if (layoutType === 'ipad') {
        navbarPC.style.display = "none";
        navbarIPAD.style.display = "block";
        return;
    }
    
    // Auto mode: switch based on screen size (1024px is breakpoint)
    if (windowWidth <= 1024) {
        navbarPC.style.display = "none";
        navbarIPAD.style.display = "block";
    } else {
        navbarPC.style.display = "flex";
        navbarIPAD.style.display = "none";
    }
}

// Update on resize (with debounce for better performance)
let resizeTimeout;
window.addEventListener('resize', () => {
    clearTimeout(resizeTimeout);
    resizeTimeout = setTimeout(updateNavbar, 100);
});

// Run on initialization
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', updateNavbar);
} else {
    // DOM is already loaded
    updateNavbar();
}