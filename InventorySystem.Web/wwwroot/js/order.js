﻿var datatable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pending")) {
        loadDataTable("GetOrderList?status=pending");
    }
    else {
        if (url.includes("approved")) {
            loadDataTable("GetOrderList?status=approved");
        }
        else {
            if (url.includes("completed")) {
                loadDataTable("GetOrderList?status=completed");
            }
            else {
                if (url.includes("rejected")) {
                    loadDataTable("GetOrderList?status=rejected");
                }
                else {
                    loadDataTable("GetOrderList?status=all");
                }
            }
        }
    }
});

function loadDataTable(url) {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/order/"+url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "nameClient", "width": "15%" },
            { "data": "phone", "width": "15%" },
            { "data": "userApplication.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                        <a href="/Admin/Order/Detail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                            <i class="fas fa-list-ul"></i>
                        </a>
                    </div>`;
                }, "width": "5%"

            }
        ]
    });
}
