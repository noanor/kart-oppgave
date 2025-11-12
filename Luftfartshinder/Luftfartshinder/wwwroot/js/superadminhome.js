setTimeout(function () {
    var alertEl = document.getElementById('registerAlert');
    if(alertEl)
    {
        var alert = new bootstrap.Alert(alertEl);
        alert.close();
    }
}, 3500); 