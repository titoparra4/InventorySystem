var datatable;

$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Inventory/Inventory/GetAll"
        },
        "columns": [
            { "data": "warehouse.name", "width": "20%" },
            { "data": "product.description", "width": "30%" },
            { "data": "product.cost", "width": "10%", "className": "text-right" },
            { "data": "quantity", "width": "10%", "className": "text-right" }
        ]
    });
}
