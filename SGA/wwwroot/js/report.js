$("#ApplicationId").select2({
    placeholder: "Selecione uma aplicação",
    minimumInputLength: 0,
    allowClear: true,
    language: "pt-BR",
    width: "100%",
    height: "100%"
});

function SetModalApplicationDescription(header, description) {

    var modal = document.querySelector(".modal-body");
    var modalDescription = document.querySelector(".modal-description");

    description = description.replace(/(\r\n|\n|\r)/gm, '<br />');

    modalDescription.textContent = header;
    modal.innerHTML = description;
}


function GetGroupPermission(GroupName, ApplicationId) {
    var modal = document.querySelector(".modal-body");
    var modalDescription = document.querySelector(".modal-description");
    modalDescription.textContent = "Detalhes do grupo " + GroupName;
    modal.innerHTML = "";

    (async () => {
        await new Promise(() => {
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/Report/GetGroupDetails");
            xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            xhr.send('GroupName=' + GroupName.trim() + '&ApplicationId=' + ApplicationId);
            var modalContent = "";

            xhr.addEventListener("load", function () {
                if (xhr.status == 200 || xhr.status == 204) {

                    if (xhr.status == 200) {
                        var permissions = JSON.parse(xhr.responseText);
                        permissions.forEach(function (permission) {
                            if (permission.length === 0) {
                                modalContent = "Não existem informações detalhadas para este grupo."
                            }
                            else {
                                modalContent += '<span onmouseover = "this.style.fontWeight=\'bold\'" onmouseleave = "this.style.fontWeight=\'normal\'" >' + permission + '</span><br />'
                            }
                            console.log(permission);
                        });
                        
                    }
                    if (modalContent == "") {
                        modalContent = "Não existem informações detalhadas para este grupo."
                    }

                    modal.innerHTML = modalContent;
                }
                else {
                    console.log("Status: " + xhr.status);
                    console.log(xhr.status);
                    console.log(xhr.responseText);
                }
            });

        })
    })()
}




function GetUserListFromGroup(GroupName, ApplicationId) {
    var modal = document.querySelector(".modal-body");
    var modalDescription = document.querySelector(".modal-description");
    modalDescription.textContent = "Detalhes do grupo " + GroupName;
    var modalContent = '';

    (async () => {
        await new Promise(() => {
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/Report/GetUserListFromGroup");
            xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            xhr.send('GroupName=' + GroupName + '&ApplicationId=' + ApplicationId);

            xhr.addEventListener("load", function () {
                if (xhr.status == 200 || xhr.status == 204) {

                    if (xhr.status == 200) {
                        var users = JSON.parse(xhr.responseText);
                        var modalContent = '';

                        console.log(users);

                        users.forEach(function (user) {
                            modalContent += '<span onmouseover = "this.style.fontWeight=\'bold\'" onmouseleave = "this.style.fontWeight=\'normal\'" >' + user + '</span><br />'

                            
                        });
                    }

                    else {
                        modalContent = "Não existem informações detalhadas para este grupo."
                    }

                    modal.innerHTML = modalContent;
                }
                else {
                    console.log(xhr.status);
                    console.log(xhr.responseText);
                }
            });

        })
    })()
}