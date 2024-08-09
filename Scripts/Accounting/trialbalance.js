$(document).ready(function () {
    $("#linkGlAccountsModal").on('hide.bs.modal', function () {
        $("#" + currTbId).closest("tr").removeClass("bg-info");

    });

    $("#linkedGlAccountsModal").on('hide.bs.modal', function () {

        $("#" + currTbId).closest("tr").removeClass("bg-info");
    });
});
var pageScrollPos;
var trialBalanceTable = $("#trialBalanceTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[10], ["10"]],
    //"order": [[3, "asc"]],
    ajax: {
        url: '/api/Accounting/GetTrialBalanceAccounts/',
        method: 'GET',
        "dataSrc": "Obj"
    },
    'columns': [
        {
            'searchable': true,
            'className': 'buttonWidth',
            'render': function (data, type, full, meta) {
                return '<td class="text-right py-0 align-middle" style="height:20px !important;"> <div class="btn-group btn-group-sm"> <a id="' + full.Id + '" class="btn btn-light" href="#" onclick="linkedGlAccounts(this.id)"><i class="fas fa-eye text-info"></i></a> </div> </td>';
            }
        },
        {
            'data': 'AccountNumber',
            'searchable': true
        },
        {
            'data': 'AccountName',
            'searchable': true
        },
        {
            'data': 'Index',
            'searchable': true,
            'className': 'indexStyle'
        },
        {
            'data': 'Id',
            'searchable': true,
            'className': 'buttonWidth',
            'render': function (data) {
                return '<td class="text-right py-0 align-middle" style="height:20px !important;"> <div class="btn-group btn-group-sm"> <a id="' + data + '" class="btn btn-light" href="#" onclick="moveUp(this.id)"><i class="fas fa-arrow-up text-info"></i></a> <a id="' + data +'" class="btn btn-light" href="#" onclick="moveDown(this.id)"><i class="fas fa-arrow-down text-info"></i></a> </div> </td>'
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

function openCreateTBModal() {
    $("#createTrialBalanceModal").modal('show');
}

function createTBAccount() {

    var accountName = $("#accountName").val();

    if (accountName == '') {
        toastr.error("Missing account name!");
        return;
    }

    var tbAccount = {
        'AccountName': accountName
    };

    $.ajax({
        url: '/api/Accounting/CreateUpdateTBAccount',
        method: 'POST',
        dataType: 'json',
        data: tbAccount,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            $('#createTrialBalanceModal').modal('hide');
            trialBalanceTable.ajax.reload();
        } else {
            toastr.error(data.Message);
        }
    });
}

function moveUp(tbId) {

    if (tbId == '') {
        toastr.error("Missing account id, contact support!");
        return;
    }

    var tbAccount = {
        'Id': tbId
    };

    $.ajax({
        url: '/api/Accounting/MoveUp',
        method: 'POST',
        dataType: 'json',
        data: tbAccount,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            trialBalanceTable.ajax.reload(function (res) {
                console.log(res);
            }, false);
            setTimeout(function () {
                $("#" + tbId).closest("tr").addClass("bg-info");
            }, 50);
        } else {
            toastr.error(data.Message);
        }
    });
}

function moveDown(tbId) {
    if (tbId == '') {
        toastr.error("Missing account id, contact support!");
        return;
    }

    var tbAccount = {
        'Id': tbId
    };

    $.ajax({
        url: '/api/Accounting/MoveDown',
        method: 'POST',
        dataType: 'json',
        data: tbAccount,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            trialBalanceTable.ajax.reload(function (res) {
                console.log(res);
            }, false);
            setTimeout(function () {
                $("#" + tbId).closest("tr").addClass("bg-info");
            }, 50);
            
        } else {
            toastr.error(data.Message);
        }
    });
}


$('#ImportTrialBalance').on('click', function () {
    $('#upload-excel').modal('show');

    $('#dropzone').fileupload({
        dropZone: $('#dropzone'),
        dataType: "application/json",
        url: '/api/Accounting/UploadExcelData/' + "1"
    }).on('fileuploadadd', function (e, data) {
        data.submit();
    }).on('fileuploadalways', function (e, data) {

        trialBalanceTable.ajax.reload();
        $('#upload-excel').modal('hide');

        var responseJson = $.parseJSON(data.jqXHR.responseText);

        trialBalanceResultsDT.clear();
        trialBalanceResultsDT.rows.add(responseJson.Obj);
        trialBalanceResultsDT.draw();

        $('#trialBalanceResultsModal').modal('show');
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

var trialBalanceResultsDT = $("#trialBalanceResultsTable").DataTable({
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

var glAccountsDT = $("#glAccountsTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
    data: [],
    columns: [
        {
            data: "DateStr"
        },
        {
            data: "AccountNumber"
        },
        {
            data: "AccountName",
        },
        {
            data: "Debit",
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            data: "Credit",
            'render': function (data, type, full, meta) {
                if (type === 'display')
                    return '<a href="#">' + new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(data) + '</a>';
                else
                    return data;
            }
        },
        {
            data: "Id",
            render: function (data) {
                return '<span><div class="btn-group btn-group-sm"> <a id="' + data + '" class="btn btn-light" href="#" onclick="linkGlAccount(this.id)"><i class="fas fa-link text-info"></i></a> </div></span>';
            }
        }
    ]
});

var linkedGlAccountsDT = $("#linkedGlAccountsTable").DataTable({
    "responsive": true,
    "autoWidth": false,
    "info": true,
    dom: '<"bottom row"<"center-col col-md-6"B><"right-col col-md-6"f>>rtip',
    "buttons": [{ extend: 'csv', exportOptions: { orthogonal: { display: ':null' } } }, { extend: 'excel', exportOptions: { orthogonal: { display: ':null' } } }],
    lengthMenu: [[10, 20, 30, -1], [10, 20, 30, "All"]],
    data: [],
    columns: [
        {
            data: "AccountNumber"
        },
        {
            data: "AccountName",
        },
        //{
        //    data: "Id",
        //    render: function (data) {
        //        return '<span><div class="btn-group btn-group-sm"> <a id="' + data + '" class="btn btn-light" href="#" onclick="unlinkGlAccount(this.id)"><i class="fas fa-unlink text-info"></i></a> </div></span>';
        //    }
        //}
    ]
});

var currTbId = 0;

function linkGlAccounts(tbId) {
    $("#linkGlAccountsModal").modal("show");
    loadUnMappedGlAccounts();
    currTbId = tbId;
}

function linkGlAccount(glId) {

    var vm = {
        'GeneralLedgerId': glId,
        'TrialBalanceAccountId': currTbId
    };

    $.ajax({
        url: '/api/Accounting/MapGeneralLedger',
        method: 'POST',
        dataType: 'json',
        data: vm,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            loadUnMappedGlAccounts();
            setTimeout(function () {
                $("#" + currTbId).closest("tr").addClass("bg-info");
            }, 50);
        } else {
            toastr.error(data.Message);
        }
    });
}

function loadUnMappedGlAccounts() {

    $.ajax({
        url: '/api/Accounting/GetUnmappedGlAccounts',
        method: 'GET',
        dataType: 'json',
    }).done(function (data) {
        if (data.Success) {
            glAccountsDT.clear();
            glAccountsDT.rows.add(data.Obj);
            glAccountsDT.draw();
            setTimeout(function () {
                $("#" + currTbId).closest("tr").addClass("bg-info");
            }, 50);
        } else {
            toastr.error(data.Message);
        }
    });
}

function linkedGlAccounts(tbId) {

    currTbId = tbId;

    $.ajax({
        url: '/api/Accounting/GetMappedGlAccounts/' + tbId,
        method: 'GET',
    }).done(function (data) {
        if (data.Success) {
            $("#linkedGlAccountsModal").modal('show');
            linkedGlAccountsDT.clear();
            linkedGlAccountsDT.rows.add(data.Obj);
            linkedGlAccountsDT.draw();
            setTimeout(function () {
                $("#" + tbId).closest("tr").addClass("bg-info");
            }, 50);
        } else {
            toastr.error(data.Message);
        }
    });
}

function unlinkGlAccount(glId) {

    var vm = {
        'GeneralLedgerId': glId,
        'TrialBalanceAccountId': currTbId
    };

    $.ajax({
        url: '/api/Accounting/UnmapGeneralLedger',
        method: 'POST',
        dataType: 'json',
        data: vm,
        contextType: "application/json",
        traditional: true
    }).done(function (data) {
        if (data.Success) {
            linkedGlAccounts(currTbId);
        } else {
            toastr.error(data.Message);
        }
    });
}