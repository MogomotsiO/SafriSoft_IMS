﻿function SafriSoftGetRequest(url, callback) {
    $.ajax({
        url: url,
        method: 'GET'
    }).done(function (data) {

        callback(data);
    });
}

function SafriSoftPostJsonRequest(url, data, callback) {
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'json',
        data: {
            JsonString: JSON.stringify(data)
        },
        contextType: "application/json; charset=utf-8",
        traditional: true
    }).done(function (data) {
        callback(data);
    });
}

function SafriSoftPostRequest(url, data, callback) {
    $.ajax({
        url: url,
        method: 'POST',
        dataType: 'json',
        data: data,
        contextType: "application/json; charset=utf-8",
        traditional: true
    }).done(function (data) {
        callback(data);
    });
}