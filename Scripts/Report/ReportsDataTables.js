﻿$("#customer-reports-table").DataTable({
    "responsive": true, "lengthChange": false, "autoWidth": false,
    dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    //lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
    ajax: {
        url: '/api/Report/GetCustomers/',
        method: 'GET',
        "dataSrc": ""
    },
    'columns': [
        {
            'data': 'CustomerName',
            'searchable': true,
            'render': function (data, type, full, meta) {
                return '<td id="' + data + '" >' + data + '</td>';
            }
        },
        {
            'data': 'CustomerEmail',
            'searchable': true
        },
        {
            'data': 'CustomerCell',
            'searchable': true
        },
        {
            'data': 'CustomerAddress',
            'searchable': true
        },
        {
            'data': 'DateCustomerCreated',
            'searchable': true
        },
        {
            'data': 'NumberOfOrders',
            'searchable': true,
            'render': function (data, type, full, meta) {
                return '<td class="text-right py-0 align-middle"> <div class="btn-group btn-group-sm"> <a class="btn btn-white" href="#">' + data + '</a> </div> </td>';
            }
        }
    ]
});

$("#products-reports-table").DataTable({
    "responsive": true, "lengthChange": false, "autoWidth": false,
    dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    ajax: {
        url: '/api/Report/GetProducts/',
        method: 'GET',
        "dataSrc": ""
    },
    'columns': [
        {
            'data': 'ProductCode',
            'searchable': true
        },
        {
            'data': 'ProductName',
            'searchable': true
        },
        {
            'data': 'ProductCategory',
            'searchable': true
        },
        
        {
            'data': 'ProductReference',
            'searchable': true
        },
        
        {
            'data': 'ItemsSold',
            'searchable': true
        },
        {
            'data': 'ItemsAvailable',
            'searchable': true
        },
        {
            'data': 'Cost',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'SellingPrice',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'TotalItems',
            'searchable': true
        },
        {
            'data': 'TotalItemsCost',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },        
        {
            'data': 'ExpectedProfit',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'CurrProfit',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        }

    ]
});

$("#orders-reports-table").DataTable({
    "responsive": true, "lengthChange": false, "autoWidth": false,
    dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    ajax: {
        url: '/api/Report/GetOrders/',
        method: 'GET',
        "dataSrc": ""
    },
    'columns': [
        {
            'data': 'OrderId',
            'searchable': true
        },
        {
            'data': 'ProductName',
            'searchable': true
        },
        {
            'data': 'NumberOfItems',
            'searchable': true
        },
        {
            'data': 'CustomerName',
            'searchable': true
        },
        {
            'data': 'DateOrderCreated',
            'searchable': true
        },
        {
            'data': 'ExpectedDeliveryDate',
            'searchable': true
        },
        {
            'data': 'OrderWorth',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'OrderStatus',
            'searchable': true,
            'render': function (data, type, full, meta) {
                if (data == "Processed") {
                    return '<span class="badge bg-danger">' + data + '</span>'
                }
                if (data == "Packaged") {
                    return '<span class="badge bg-warning">' + data + '</span>'
                }
                if (data == "InTransit") {
                    return '<span class="badge bg-info">' + data + '</span>'
                }
                if (data == "Delivered") {
                    return '<span class="badge bg-success">' + data + '</span>'
                }
            }
        },
        {
            'data': 'OrderProgress',
            'searchable': true,
            'render': function (data, type, full, meta) {
                if (full.OrderStatus == "Processed") {
                    return '<span class="badge bg-danger">' + data + '%</span>'
                }
                if (full.OrderStatus == "Packaged") {
                    return '<span class="badge bg-warning">' + data + '%</span>'
                }
                if (full.OrderStatus == "InTransit") {
                    return '<span class="badge bg-info">' + data + '%</span>'
                }
                if (full.OrderStatus == "Delivered") {
                    return '<span class="badge bg-success">' + data + '%</span>'
                }

            }
        }
    ]
});


$("#system-reports-table").DataTable({
    "responsive": true, "lengthChange": false, "autoWidth": false,
    dom: '<"top"<"left-col"B><"center-col"l><"right-col"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    //lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
    ajax: {
        url: '/api/Report/GetSystemNotifications/',
        method: 'GET',
        "dataSrc": ""
    },
    'columns': [
        {
            'data': 'OrderId',
            'searchable': true
        },
        {
            'data': 'ProductName',
            'searchable': true
        },
        {
            'data': 'NumberOfItems',
            'searchable': true
        },
        {
            'data': 'CustomerName',
            'searchable': true
        },
        {
            'data': 'OrderStatus',
            'searchable': true,
            'render': function (data, type, full, meta) {
                if (data == "Processed") {
                    return '<span class="badge bg-danger">' + data + '</span>'
                }
                if (data == "Packaged") {
                    return '<span class="badge bg-warning">' + data + '</span>'
                }
                if (data == "InTransit") {
                    return '<span class="badge bg-info">' + data + '</span>'
                }
                if (data == "Delivered") {
                    return '<span class="badge bg-success">' + data + '</span>'
                }
            }
        },
        {
            'data': 'OrderWorth',
            'searchable': true,
            'className': 'text-right',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat(currencyIsoName, { style: 'currency', currency: currency }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'DateOrderCreated',
            'searchable': true
        },
        {
            'data': 'ExpectedDeliveryDate',
            'searchable': true,
            'render': function (data, type, full, meta) {
                return '<span class="badge bg-danger">' + data + '</span>'
            }
        }
    ]
});