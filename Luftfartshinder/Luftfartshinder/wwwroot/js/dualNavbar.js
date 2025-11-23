window.addEventListener('resize', () => {
    var windowWidth = window.innerWidth;

    if (windowWidth <= 1024) {
        document.getElementById("navbarPC").style.display = "none";
        document.getElementById("navbarIPAD").style.display = "block";
    }

    else {
        document.getElementById("navbarPC").style.display = "flex";
        document.getElementById("navbarIPAD").style.display = "none";
    }
});