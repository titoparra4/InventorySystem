var datatable;

$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "userName", "width": "10%" },
            { "data": "name", "width": "10%" },
            { "data": "lastName", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {

                    var today = new Date().getTime();
                    var block = new Date(data.lockoutEnd).getTime();

                    if (block > today) {
                        return `
                    <div class="text-center">
                        <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:150px">
                            <i class="fas fa-lock-open"></i> Unlock
                        </a>
                    </div>`;

                    }
                    else {
                        return `
                    <div class="text-center">
                        <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:150px">
                            <i class="fas fa-lock"></i> Lock
                        </a>
                    </div>`;
                    }

                }, "width": "30%"

            }
        ]
    });
}

function LockUnlock(id) {
            $.ajax({
                type: "POST",
                url: '/Admin/User/LockUnlock',
                data: JSON.stringify(id),
                contentType: "application/json",
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