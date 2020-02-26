
function executa() {
    var resultado = document.querySelector("#Resultado");
    resultado.textContent = "Executando...";
    $.getJSON("/Procedures/ImportDataService", function (data) {
        resultado.textContent = data.toString();
    });
}