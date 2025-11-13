setTimeout(function () {
    var alertEl = document.getElementById('registerAlert');
    if (alertEl) {
        alertEl.classList.remove('show');
        setTimeout(function() {
            alertEl.remove(); 
        }, 500); 
    }
}, 3500);