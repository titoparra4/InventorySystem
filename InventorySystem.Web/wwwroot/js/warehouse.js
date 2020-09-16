var datatable;

$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Warehouse/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "description", "width": "40%" },
            {
                "data": "status",
                "render": function (data) {
                    if (data == true) {
                        return "Active";
                    }
                    else {
                        return "Inactive";
                    }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Admin/Warehouse/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                            <i class="fas fa-edit"></i>
                        </a>
                        <a onclick=Delete("/Admin/Warehouse/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                            <i class="fas fa-trash"></i>
                        </a>
                    </div>`;
                }, "width": "20%"

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