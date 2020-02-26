$("#EnvironmentId").select2({
    placeholder: "Selecione um ambiente",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});

$("#DatabaseTypeId").select2({
    placeholder: "Selecione um tipo de banco de dados",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%"
});

$('#DatabaseTypeId').on("select2:select", function (e) {
    verifyODBC($(this).val());
});

var odbcDiv = document.querySelector("#ConnectionStringODBC");
odbcDiv.style.display = "none";

function verifyODBC(databaseTypeId) {

    var odbcDiv = document.querySelector("#ConnectionStringODBC");
    if (databaseTypeId == 6) {
        odbcDiv.style.display = "block";
    }
    else {
        odbcDiv.style.display = "none";
    }

}

var databaseId = $('#DatabaseTypeId').val();
if (databaseId != null) {
    verifyODBC(databaseId)
}
