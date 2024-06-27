$(document).ready(function () {
    var today = new Date();
    var monthEnd = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    var startDay = '01';
    var endDay = monthEnd.getDate();

    document.getElementById("start").defaultValue = today.getFullYear() + "-" + ("0" + (today.getMonth() + 1)).slice(-2) + "-" + startDay;
    document.getElementById("end").defaultValue = today.getFullYear() + "-" + ("0" + (today.getMonth() + 1)).slice(-2) + "-" + endDay;

    refreshTrialBalanceReportDT();
});

let exportFormatter = {
    format: {
        body: function (data, row, column, node) {
            // Strip $ from salary column to make it numeric
            return column === 4 ? data.replace(/[R]/g, '') : data;
        }
    }
};

var trialBalanceReportTable = $("#trialBalanceReportTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    
    
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    lengthMenu: [[-1], ["All"]],
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    "order": [],
    //ajax: {
    //    url: '/api/Accounting/GetTrialBalanceReport/0/0',
    //    method: 'GET',
    //    "dataSrc": "Obj"
    //},
    'columns': [
        {
            'data': 'Period',
            'searchable': true,
            sortable: false,
            'className': 'indexStyle',
        },
        {
            'data': 'AccountNumber',
            'searchable': true,
            'render': function (data, type, full, meta) {
                if (data != 0) {
                    return '<td id="' + data + '" >' + data + '</td>';
                } else {
                    return '<td id="' + data + '" ></td>';
                }
                
            }
        },
        {
            'data': 'AccountName',
            'searchable': true
        },
        {
            className: "dt-control text-right",
            'data': 'Total',
            'searchable': true,
            'render': function (data, type, full, meta) {

                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(data) + '</a>';
                else
                    return data;
            }
        },
    ]
});



var trialBalanceItemsTable = $("#trialBalanceItemsTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[20, 30, -1], [20, 30, "All"]],
    "order": [[0, "desc"]],
    'columns': [
        {
            'data': 'DateStr',
            'searchable': true,
            'className': 'indexStyle',
        },
        {
            'data': 'AccountNumber',
            'searchable': true,
            'render': function (data, type, full, meta) {
                return '<td id="' + data + '" >' + data + '</td>';
            }
        },
        {
            'data': 'AccountName',
            'searchable': true
        },
        {
            'searchable': true,
            className: "text-right",
            render: function (data, type, full, meta) {
                if (type == 'display') {
                    return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(full.Debit);
                } else {
                    return full.Debit;
                }
                
            }
        },
        {
            'searchable': true,
            className: "text-right",
            render: function (data, type, full, meta) {
                if (type == 'display') {
                    return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(full.Credit);
                } else {
                    return full.Credit;
                }
            }
        }
    ]
});

function format(d) {
    console.log(d);
    var data = "";
    for (var i = 0; i < d.TrialBalanceItems.length; i++) {
        data += '<tr><td>' + d.TrialBalanceItems[i].DateStr + '</td><td>' + d.TrialBalanceItems[i].AccountNumber + '</td><td> ' + d.TrialBalanceItems[i].AccountName + '</td><td> ' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(d.TrialBalanceItems[i].Debit) + '</td><td> ' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(d.TrialBalanceItems[i].Credit) + '</td></tr>';
    }
    // `d` is the original data object for the row
    return (
        '<table class="table-bordered"><tbody>' + data + '</tbody></table>'
    );
}

// Add event listener for opening and closing details
trialBalanceReportTable.on('click', 'td.dt-control', function (e) {
    let tr = e.target.closest('tr');
    let row = trialBalanceReportTable.row(tr);

    trialBalanceItemsTable.clear();
    trialBalanceItemsTable.rows.add(row.data().TrialBalanceItems);
    trialBalanceItemsTable.draw();

    $("#trialBalanceItems").modal('show');
    //if (row.child.isShown()) {
    //    // This row is already open - close it
    //    row.child.hide();
    //}
    //else {
    //    // Open this row
    //    row.child(format(row.data())).show();
    //}
});


function refreshTrialBalanceReportDT() {

    var start = $("#start").val();
    var end = $("#end").val();

    var vm = {
        'Start': start,
        'End': end
    };

    var url = '/api/Accounting/GetTrialBalanceReport';

    SafriSoftPostRequest(url, vm, (data) => {
        if (data.Success) {
            trialBalanceReportTable.clear();
            trialBalanceReportTable.rows.add(data.Obj);
            trialBalanceReportTable.draw();
        } else {
            toastr.error(data.Message);
        }
    });

    //var period = $("#period").val().split("-");

    //if (period != null && period != '') {

    //    var month = period[1];
    //    var year = period[0];

    //    trialBalanceReportTable.ajax.url('/api/Accounting/GetTrialBalanceReport/' + month + '/' + year).load();
    //} else {
    //    toastr.error("Please select a period");
    //}
}