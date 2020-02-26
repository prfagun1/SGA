
$("#Username").select2({
    placeholder: "Selecione um usuário",
    minimumInputLength: 3,
    allowClear: true,
    language: "pt-BR",
    width: "100%",
    ajax: {
        url: '/Procedures/GetUserDetails',
        dataType: 'json',
        type: "GET",
        quietMillis: 250,
        create_option: true,
        data: function (term) {
            var query = {
                term: term,
                type: 'Disable'
            }
            return {
                query
            };
        },
        processResults: function (data) {
            var myResults = [];
            $.each(data, function (index, item) {
                console.log(data);
                myResults.push({
                    'id': item.username,
                    'text': GetDescription(item)
                });

            });
            return {
                results: myResults
            };
        }
    }
});

function GetDescription(item) {
    var description = item.username
    description += item.fullName ? " - " + item.fullName : "";
    description += item.department ? " - " + item.department : "";
    return description;
}