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

$(document).ajaxStart(function () {
    console.log('Ajax started');
    const loader = document.querySelector(".loader");

    loader.classList.remove("loader--hidden");
});

$(document).ajaxComplete(function () {
    console.log('Ajax call completed');

    const loader = document.querySelector(".loader");

    loader.classList.add("loader--hidden");
});

$(function () {
    const loader = document.querySelector(".loader");

    loader.classList.add("loader--hidden");
});