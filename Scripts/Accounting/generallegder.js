﻿$(document).ready(function () {
    //var today = new Date();

    //document.getElementById("period").defaultValue = today.getFullYear() + "-" + ("0" + (today.getMonth() + 1)).slice(-2);
});

var generalLedgerTable = $("#generalLedgerTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[-1], ["All"]],
    //"order": [[2, "asc"]],
    //ajax: {
    //    url: '/api/Accounting/GetGeneralLedgerAccounts/0/0',
    //    method: 'GET',
    //    "dataSrc": "Obj"
    //},
    'columns': [
        {
            'data': 'DateStr',
            'searchable': true,
            'render': function (data, type, full, meta) {
                return '<td id="' + data + '" >' + data + '</td>';
            }
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
            'data': 'AccountDescription',
            'searchable': true
        },
        //{
        //    'data': 'id',
        //    'searchable': true,
        //    'className': 'indexStyle',
        //    render: function (data, type, full, meta) {
        //        return '<span>' + full.Month + '/' + full.Year + '</span>'
        //    }
        //},
        {
            'data': 'Debit',
            'searchable': true,
            'className': 'indexStyle',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            'data': 'Credit',
            'searchable': true,
            'className': 'indexStyle',
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(data) + '</a>';
                else
                    return data;
            }
        },
        //{
        //    'data': 'Id',
        //    'searchable': true,
        //    'className': 'buttonWidth',
        //    'render': function (data) {
        //        return '<td class="text-right py-0 align-middle"> <div class="btn-group btn-group-sm"> <a id="' + data + '" class="btn btn-light" href="#" onclick="linkGlAccounts(this.id)"><i class="fas fa-link text-info"></i></a> </div> </td>'
        //    }
        //}
    ]
});

$('#ImportGeneralLedger').on('click', function () {
    $('#upload-excel').modal('show');

    $('#dropzone').fileupload({
        dropZone: $('#dropzone'),
        dataType: "application/json",
        url: '/api/Accounting/UploadGlExcelData/' + "1"
    }).on('fileuploadadd', function (e, data) {
        data.submit();
    }).on('fileuploadalways', function (e, data) {

        //generalLedgerTable.ajax.reload();
        $('#upload-excel').modal('hide');

        var responseJson = $.parseJSON(data.jqXHR.responseText);

        generalLedgerResultsDT.clear();
        generalLedgerResultsDT.rows.add(responseJson.Obj);
        generalLedgerResultsDT.draw();

        $('#generalLedgerResultsModal').modal('show');
        var dropZone = $('#dropzone');
        dropZone.removeClass('hover');
    });

    $(document).bind('dragover', function (e) {
        var dropZone = $('#dropzone');
        dropZone.addClass('hover');
    });

    $(document).bind('dragleave', function (e) {
        var dropZone = $('#dropzone');
        dropZone.removeClass('hover');
    });

    $(document).bind('drop dragover', function (e) {
        e.preventDefault();
    });

});

var generalLedgerResultsDT = $("#generalLedgerResultsTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
    data: [],
    columns: [
        {
            data: "Message"
        },
        {
            data: "Success",
            className: 'indexStyle',
            render: function (data) {
                console.log(data);
                if (data == true)
                    return '<span><i class="fa fa-check text-success"></i></span>';
                else
                    return '<span><i class="fas fa-times text-danger"></i></span>';
            }
        }
    ]
});


function refreshGeneralLedgerDT() {
    var start = $("#start").val();
    var end = $("#end").val();

    var vm = {
        'Start': start,
        'End': end
    };

    $.ajax({
        url: '/api/Accounting/GetGeneralLedgerAccounts',
        method: 'POST',
        dataType: 'json',
        data: vm,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            generalLedgerTable.clear();
            generalLedgerTable.rows.add(data.Obj);
            generalLedgerTable.draw();
        } else {
            toastr.error(data.Message);
        }
    });

    //if (period != null && period != '') {

    //    var month = period[1];
    //    var year = period[0];

    //    generalLedgerTable.ajax.url('/api/Accounting/GetGeneralLedgerAccounts/' + month + '/' + year).load();
    //} else {
    //    toastr.error("Please select a period");
    //}
}