var ieMessage = document.getElementById("IEMessage");
ieMessage.hidden = true;

msieversion();

function msieversion() {
    if (window.document.documentMode) {
        var acessar = document.getElementById("Acessar");
        acessar.hidden = true;
        ieMessage.hidden = false;
    }
}