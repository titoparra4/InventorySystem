var datatable;

$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Inventory/Inventory/GetRecord"
        },
        "columns": [
            {
                "data": "initialDate", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            {
                "data": "finalDate", "width": "15%",
                "render": function (data) {
                    var d = new Date(data);
                    return d.toLocaleString();
                }
            },
            { "data": "warehouse.name", "width": "15%" },
            {
                "data": function nameUser(data) {
                    return data.userApplication.name + " " + data.userApplication.lastName;
                }, "width":"20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Inventory/Inventory/HistoryDetail/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                            Detail
                        </a>
                        
                    </div>`;
                }, "width": "10%"

            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure to delete the record?",
        text: "This record cannot be recovered",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((erase) => {
        if (erase) {
            $.ajax({
                type: "Delete",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });

}