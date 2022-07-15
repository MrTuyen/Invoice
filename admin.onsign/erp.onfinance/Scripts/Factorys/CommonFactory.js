app.factory('CommonFactory', ['$timeout', function ($timeout) {
    var service = {};

    service.PostDataAjax = function (url, data, callBack, timeout) {
        if (!timeout)
            timeout = 60000;
        var tokenString = "";
        var uuid = "";
        var storeID = 0;

        $.ajax({
            url: url,
            type: "POST",

            headers: {
                "X-Requested-With": "SABApp",
                "Token-String": tokenString,
                "Device-UUID": uuid,
                "StoreID": storeID,
                "GUID": storeID
            },

            timeout: timeout,
            cache: true,
            crossDomain: true,
            contentType: "application/json; charset=utf-8;",
            dataType: "json",
            data: data,
            processData: true,
            beforeSend: function () { },
            async: true,
            tryCount: 0,
            retryLimit: 3,

            success: function (response) {
                if (response) {
                    $timeout(function () {
                        callBack(response);
                    }, 10);
                } else {
                    $timeout(callBack, 10);
                }
            },

            error: function (error) {
                $timeout(callBack, 10);
            }
        });
    };


    return service;
}]);
