var username = "";
var fullname = "";
var password = "";


$('#modal-password').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    username = button.data('username');
    fullname = button.data('full-name');
    document.getElementById("modal-body").innerHTML = "Alterar a senha de <strong>" + fullname + "</strong >?"
    document.getElementById("buttonCancel").textContent = "Cancelar";
    
    buttonOK.hidden = false;
})


function ChangePassword() {

    var divMessage = document.getElementById("modal-body");
    var buttonOK = document.getElementById("buttonOK");
    ChangePasswordInternal(username);

    console.log(password);

    divMessage.innerHTML = "A senha de <strong> " + fullname + "</strong> foi alterada para <strong> " + password + "</strong>.";
    document.getElementById("buttonCancel").textContent = "Fechar";
    buttonOK.hidden = true;
}

function ChangePasswordInternal(username) {
    var url = "/Report/ChangePassword?Username=" + username;
    $.ajax({
        url: url,
        dataType: 'text',
        async: false,
        success: function (data) {
            password = data;
        }
    });
}