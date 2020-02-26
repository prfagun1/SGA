$("#ApplicationId").select2({
    placeholder: "Selecione uma aplicação",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});

$("#ApplicationTypeId").select2({
    placeholder: "Selecione um tipo de grupo",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});


$("#RestType").select2({
    placeholder: "Selecione um tipo conexão",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});


$("#MD5").select2({
    placeholder: "Selecione sim ou não",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});

$('#ApplicationTypeId').on("select2:select", function (e) {
    verifyConectionType($(this).val());
});


var typeDiv = document.querySelector("#TypeSelected");
typeDiv.style.display = "none";

function verifyConectionType(type) {
    var typeDiv = document.querySelector("#TypeSelected");
    if (type == 6 || type == 5) {
        typeDiv.style.display = "block";
    }
    else {
        typeDiv.style.display = "none";
    }

}