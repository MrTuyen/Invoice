var app = angular.module('novaon', ['ngRoute', 'angular-loading-bar', 'ngSanitize']);

app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
    $routeProvider
        .when('/tong-quan', {
            templateUrl: '/Home/Home'
        })
        .when('/quan-ly-hoa-don', {
            templateUrl: '/Invoice/Index',
        })
        .when('/quan-ly-khach-hang', {
            templateUrl: '/Customer/Index'
        })
        .when('/chi-tiet-khach-hang', {
            templateUrl: '/Customer/Detail',
            controller: 'CustomerController'
        })
        .when('/quan-ly-san-pham', {
            templateUrl: '/Product/Index',
            controller: 'ProductController'
        })
        .when('/quan-ly-hang-hoa-dich-vu', {
            templateUrl: '/Category/Index',
            controller: 'CategoryController'
        })
        .when('/cai-dat', {
            templateUrl: '/Settings/Index',
        })
        .when('/thong-tin-tai-khoan', {
            templateUrl: '/Settings/UserSettings',
        })
        .when('/dai-hoa-don-cho', {
            templateUrl: '/Invoice/InvoiceWait',
        })
        .when('/cai-dat-tai-khoan', {
            templateUrl: '/Settings/AccountSetting',
        })
        .when('/thong-bao-phat-hanh', {
            templateUrl: '/Settings/Announcement'
        })
        .when('/quan-ly-nguoi-dung', {
            templateUrl: '/User/Index'
        })
        .when('/cong-cu', {
            templateUrl: '/Settings/Tool',
            controller: 'SettingsController'
        })
        .when('/danh-muc', {
            templateUrl: '/Settings/AttachementFile',
            controller: 'SettingsController'
        })
        .when('/bao-cao-tinh-hinh-su-dung-hoa-don', {
            templateUrl: '/Statistic/UsingInvoice',
            controller: 'StatisticController'
        })
        .when('/bang-ke-hoa-don-dau-ra', {
            templateUrl: '/Statistic/OutputInvoice',
            controller: 'StatisticController'
        })
        .when('/lich-su-gui-mail', {
            templateUrl: '/Statistic/MailHistory',
            controller: 'StatisticController'
        })
        .when('/bao-cao-lich-su-hoat-dong', {
            templateUrl: '/Statistic/ActionHistory',
            controller: 'StatisticController'
        })
        .when('/mau-hoa-don/:id', {
            templateUrl: '/TempInvoice/Index',
            controller: 'TempInvoiceController'
        })
        .when('/settings-template/:id', {
            templateUrl: '/SettingTemplate/Index',
            controller: 'SettingTemplateController'
        })
        .when('/mau-hoa-don', {
            templateUrl: '/TempInvoice/Index',
            controller: 'TempInvoiceController'
        })
        .when('/bien-lai-thu-phi-le-phi', {
            templateUrl: '/Receipt/Index',
            controller: 'ReceiptController'
        })
        .when('/quan-ly-cong-to', {
            templateUrl: '/Meter/Index',
            controller: 'MeterController'
        })
        .when('/nhat-ky-truy-cap', {
            templateUrl: '/Log/Index',
            controller: 'LogController'
        })
        .when('/tien-thanh-toan', {
            templateUrl: '/CurrencyUnit/Index',
            controller: 'CurrencyUnitController'
        })
        .when('/hinh-thuc-thanh-toan', {
            templateUrl: '/PaymentMethod/Index',
            controller: 'PaymentMethodController'
        })
        .when('/don-vi-tinh', {
            templateUrl: '/QuantityUnit/Index',
            controller: 'QuantityUnitController'
        })
        .when('/cai-dat-he-thong', {
            templateUrl: '/Settings/Parameters',
            controller:'SettingsController'
        })
        .otherwise({
            redirectTo: '/tong-quan'
        });
    $locationProvider
        .html5Mode(false)
        .hashPrefix('');
}]);

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
                "X-Requested-With": "Novaon-DS",
                "Token-String": tokenString,
                "Device-UUID": uuid,
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

app.factory('rootVar', function () {
    var mytemp = null;//the object to hold our data
    return {
        getTemp: function () {
            return mytemp;
        },
        setTemp: function (value) {
            mytemp = value;
        }
    }

});

app.factory('persistObject', function () {

    var persistObject = [];

    function set(objectName, data) {
        persistObject[objectName] = data;
    }
    function get(objectName) {
        return persistObject[objectName];
    }
    return {
        set: set,
        get: get
    }
});

app.factory('permissions', function ($rootScope) {
    var permissionList;
    return {
        setPermissions: function (permissions) {
            permissionList = permissions;
            $rootScope.$broadcast('permissionsChanged');
        },
        hasPermission: function (permissions) {
            var hasPermission = false;
            if (!permissionList) {
                if (localStorage.roles && typeof (localStorage.roles).toLowerCase() == 'string')
                    permissionList = [localStorage.roles];
                else
                    permissionList = localStorage.roles;
            }
            if (!permissionList) return false;
            angular.forEach(permissions, function (permission, index) {
                if (!hasPermission) {
                    permission = permission.trim();
                    return angular.forEach(permissionList, function (item) {
                        if (item) {
                            hasPermission = hasPermission || item.trim() === permission;
                        }
                    });
                }
            });
            return hasPermission;
        }
    };
});

//app.directive('modal', function () {
//    return {
//        template: '<div class="modal fade">' +
//            '<div class="modal-dialog">' +
//            '<div class="modal-content">' +
//            '<div class="modal-header">' +
//            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
//            '<h4 class="modal-title">{{ title }}</h4>' +
//            '</div>' +
//            '<div class="modal-body" ng-transclude></div>' +
//            '</div>' +
//            '</div>' +
//            '</div>',
//        restrict: 'E',
//        transclude: true,
//        replace: true,
//        scope: true,
//        link: function postLink(scope, element, attrs) {
//            scope.title = attrs.title;

//            scope.$watch(attrs.visible, function (value) {
//                if (value == true)
//                    $(element).modal('show');
//                else
//                    $(element).modal('hide');
//            });

//            $(element).on('shown.bs.modal', function () {
//                scope.$apply(function () {
//                    scope.$parent[attrs.visible] = true;
//                });
//            });

//            $(element).on('hidden.bs.modal', function () {
//                scope.$apply(function () {
//                    scope.$parent[attrs.visible] = false;
//                });
//            });
//        }
//    };
//});

app.directive('goClick', function ($location) {
    return function (scope, element, attrs) {
        var path;

        attrs.$observe('goClick', function (val) {
            path = val;
        });

        element.bind('click', function () {
            scope.$apply(function () {
                $location.path(path);
            });
        });
    };
});

app.directive('autoComplete', function ($timeout) {
    return function (scope, iElement, iAttrs) {
        iElement.autocomplete({
            source: scope[iAttrs.uiItems],
            select: function () {
                $timeout(function () {
                    iElement.trigger('input');
                }, 0);
            }
        });
    };
});

//app.directive('draggable', function () {
//    return {
//        // A = attribute, E = Element, C = Class and M = HTML Comment
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//        }
//    }
//});

//app.directive('droppable', function ($compile) {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//            //element.droppable({
//            //    accept: ".dashboard-review-box",
//            //    hoverClass: "drop-hover",
//            //    drop: function (event, ui) { }
//            //})
//        }
//    }
//});

app.directive('sortable', function () {
    return {
        restrict: 'A',
        link: function (scope, elt, attrs) {
            return elt.sortable({
                connectToSortable: $("#iframe_templateViewInvoice").contents().find("#sortable").sortable({
                    //items: "> div",
                    revert: true,
                    iframeFix: true
                }),
                revert: true,
                iframeFix: true
                //revert: true,
                //stop: function (evt, ui) {
                //    return scope.apply(function () {
                //        /* code goes here */
                //    });
                //}
            });
        }
    };
});

"use strict";

var dyn_functions = [];
var callbackExec = function (str) { };

function CreatePagerSubmit(currPage, totalPage, $ctr, frmSearch) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    //$ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        if (frmSearch == null)
            frmSearch = "frmSearch";
        var pIndex = parseInt($(this).text());

        $("#" + frmSearch + " #hdPageIndex").val(pIndex);
        $("#" + frmSearch).submit();

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function CreatePagerAjax(currPage, totalPage, $ctr, functionName) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        //CreatePagerAjax(parseInt($(this).text()), totalPage, $ctr, functionName);
        var pIndex = parseInt($(this).text());
        dyn_functions[functionName](pIndex);

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function CreateDivPagerAjax(currPage, totalPage, $ctr, model, action, view, functionName, inputName) {
    var pager = GenHtmlPager(currPage, totalPage);
    $ctr.html(pager);
    $ctr.find('span').click(function () {
        if ($(this).text() == "...") {
            return;
        }
        var pIndex = parseInt($(this).text());
        dyn_functions[functionName](model, action, view, pIndex, inputName);

        $ctr.css('opactity', '0.5');
        $ctr.css('cursor', 'progress');
    });
}

function GenHtmlPager(currPage, totalPage) {
    var pager = '';
    var numButton = 8;
    if (totalPage < 1 || currPage < 1) {
        return;
    }
    if (totalPage < numButton) {
        for (var i = 1; i <= totalPage; i++) {
            if (i != currPage) {
                pager += '<span class="btn pagerbtn">';
                pager += i;
                pager += '</span>'
            }
            else {
                pager += '<span class="btn active pagerbtn">';
                pager += i;
                pager += '</span>'
            }
        }
    }
    else {
        var center = Math.floor((numButton - 2) / 2);
        var left = currPage - center;
        if (left < 1)
            left = 1;
        var right = currPage + center;
        if (right > totalPage)
            right = totalPage;
        if (right - left <= 2 * center) {
            right = left + 2 * center;
        }
        if (currPage - center > 1) {
            pager += '<span class="btn pagerbtn">';
            pager += 1;
            pager += '</span>';
            if (currPage - center > 2) {
                pager += '<span class="btn pagerbtn">...</span>';
            }
        }
        else {
            right++;
        }

        if (right >= totalPage) {
            right = totalPage;
            left = right - (2 * center);
        }
        var temp = '';
        if (currPage + center < totalPage) {
            if (currPage + center < totalPage - 1) {
                temp += '<span class="btn pagerbtn">...</span>';
            }
            temp += '<span class="btn pagerbtn">';
            temp += totalPage;
            temp += '</span>';
        }
        else {
            left--;
        }
        for (var i = left; i <= right; i++) {
            if (i != currPage) {
                pager += '<span class="btn pagerbtn" page="' + i + '">';
                pager += i;
                pager += '</span>'
            }
            else {
                pager += '<span class="btn active pagerbtn" page="' + i + '">';
                pager += i;
                pager += '</span>'
            }
        }
        pager += temp;
    }
    return pager;
}

function printDiv(divName) {
    var printContents = document.getElementById(divName).innerHTML;
    var originalContents = document.body.innerHTML;

    document.body.innerHTML = printContents;

    window.print();

    document.body.innerHTML = originalContents;
}

function CheckAll(chkAll, chkSelect) {
    if ($(chkAll).attr('checked')) {
        $('input:checkbox[id=' + chkSelect + ']').attr('checked', true);
    }
    else {
        $('input:checkbox[id=' + chkSelect + ']').attr('checked', false);
    }
}

function ShowLoading(value) {
    document.getElementById('loading-page').style.display = value ? 'block' : 'none';
}


//Cookie's owned
function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = document.cookie;
    try {
        decodedCookie = decodeURIComponent(document.cookie);
    } catch (error) {
        console.log(error);
    }

    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
}
function checkCookie(cookie_name) {

}

var validation = {
    isEmailAddress: function (str) {
        var pattern = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return pattern.test(str);  // returns a boolean
    },
    isNotEmpty: function (str) {
        var pattern = /\S+/;
        return pattern.test(str);  // returns a boolean
    },
    isNumber: function (str) {
        var pattern = /^\d+$/;
        return pattern.test(str);  // returns a boolean
    },
    isSame: function (str1, str2) {
        return str1 === str2;
    }
};

app.directive('clearabletextbox', function ($timeout) {
    return {
        restrict: "E",

        scope: {
            iconposition: "@",
            textboxtype: "@",
            textboxplaceholder: "@",
            textboxmodel: "=",
            textboxmaxlength: "@",
            resultchange: "&",
            resultchangedelay: "@",
            resultchangeminlength: "@",
            submitonenter: "&",
            cleartext: "&",
            blur: "&",
            disable: "=",
            textalignment: "@",
            isnumberformat: "=",
            isnumberformat3digit: "=",
            islargetext: "=",
            disablenumber: "=",
            allowonlynumber: "=",
            nfocus: "=",
            isinteger: "=",
            focustextbox: "=",
            textview: "=?"
        },

        transclude: true,
        templateUrl: "/Directives/ClearableTextBox.html?v=2",

        link: function (scope, element, attrs) {

            //#region Variable

            var submissionHandler = null;
            scope.textboxmodel = scope.textview + "";   //Parse to string
            
            //#endregion

            //#region Function

            //#region Support

            var formatNumber = function () {
                if ((typeof scope.textboxmodel === "string"))
                    scope.textboxmodel = scope.textboxmodel.trim();

                if (scope.textboxmodel.length > 1) {
                    while (scope.textboxmodel.charAt(0) == "0" && scope.textboxmodel.charAt(1) != ".") {
                        scope.textboxmodel = scope.textboxmodel.substr(1);
                    }

                    var field = scope.textboxmodel.replace(/[^\d.\','] /g, "");
                    var point = field.indexOf(".");

                    if (point >= 0) {
                        field = field.slice(0, point + 3);
                    }

                    var decimalSplit = field.split(".");
                    var intPart = decimalSplit[0];
                    var decPart = decimalSplit[1];

                    intPart = intPart.replace(/[^\d]/g, "");

                    if (intPart.length > 3) {
                        var intDiv = Math.floor(intPart.length / 3);

                        while (intDiv > 0) {
                            var lastComma = intPart.indexOf(",");

                            if (lastComma < 0) {
                                lastComma = intPart.length;
                            }

                            if (lastComma - 3 > 0) {
                                intPart = intPart.substr(0, lastComma - 3) + "," + intPart.substr(lastComma - 3);//intPart.splice(lastComma - 3, 0, ",");
                            }

                            --intDiv;
                        }
                    }

                    if (decPart === undefined) {
                        decPart = "";
                    } else {
                        decPart = "." + decPart;
                    }

                    var res = intPart + decPart;

                    scope.textboxmodel = res;
                    try {
                        var inputelement = element[0].children[0].children[0];
                        // Cache references
                        var $el = $(inputelement),
                            el = inputelement;

                        // Only focus if input isn't already
                        if (!$el.is(":focus")) {
                            $el.focus();
                        }

                        // If this function exists... (IE 9+)
                        if (el.setSelectionRange) {

                            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                            var len = $el.val().length * 2;

                            // Timeout seems to be required for Blink
                            setTimeout(function () {
                                el.setSelectionRange(len, len);
                            }, 1);

                        } else {
                            // As a fallback, replace the contents with itself
                            // Doesn't work in Chrome, but Chrome supports setSelectionRange
                            $el.val($el.val());

                        }

                        // Scroll to the bottom, in case we're in a tall textarea
                        // (Necessary for Firefox and Chrome)
                        this.scrollTop = 999999;


                    } catch (e) {

                    }

                }
            }

            var formatNumber3Digit = function () {
                if ((typeof scope.textboxmodel === "string"))
                    scope.textboxmodel = scope.textboxmodel.trim();

                if (scope.textboxmodel.length > 1) {
                    while (scope.textboxmodel.charAt(0) == "0" && scope.textboxmodel.charAt(1) != ".") {
                        scope.textboxmodel = scope.textboxmodel.substr(1);
                    }

                    var field = scope.textboxmodel.replace(/[^\d.\','] /g, "");
                    var point = field.indexOf(".");

                    if (point >= 0) {
                        // configure the number after the dot
                        //field = field.slice(0, point + 4);
                        field = field.slice(0, point + 10);
                    }

                    var decimalSplit = field.split(".");
                    var intPart = decimalSplit[0];
                    var decPart = decimalSplit[1];

                    intPart = intPart.replace(/[^\d]/g, "");

                    if (intPart.length > 3) {
                        var intDiv = Math.floor(intPart.length / 3);

                        while (intDiv > 0) {
                            var lastComma = intPart.indexOf(",");

                            if (lastComma < 0) {
                                lastComma = intPart.length;
                            }

                            if (lastComma - 3 > 0) {
                                intPart = intPart.substr(0, lastComma - 3) + "," + intPart.substr(lastComma - 3);//intPart.splice(lastComma - 3, 0, ",");
                            }

                            --intDiv;
                        }
                    }

                    if (decPart === undefined) {
                        decPart = "";
                    } else {
                        decPart = "." + decPart;
                    }

                    var res = intPart + decPart;

                    scope.textboxmodel = res;
                    //scope.textview = res;
                    try {
                        var inputelement = element[0].children[0].children[0];
                        // Cache references
                        var $el = $(inputelement),
                            el = inputelement;

                        // Only focus if input isn't already
                        if (!$el.is(":focus")) {
                            $el.focus();
                        }

                        // If this function exists... (IE 9+)
                        if (el.setSelectionRange) {

                            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                            var len = $el.val().length * 2;

                            // Timeout seems to be required for Blink
                            setTimeout(function () {
                                el.setSelectionRange(len, len);
                            }, 1);

                        } else {
                            // As a fallback, replace the contents with itself
                            // Doesn't work in Chrome, but Chrome supports setSelectionRange
                            $el.val($el.val());

                        }

                        // Scroll to the bottom, in case we're in a tall textarea
                        // (Necessary for Firefox and Chrome)
                        this.scrollTop = 999999;


                    } catch (e) {

                    }

                }
            }

            var submit = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                if (!scope.resultchangeminlength || (scope.textboxmodel && scope.textboxmodel.trim().length >= scope.resultchangeminlength)) {
                    submissionHandler = $timeout(function () {
                        scope.resultchange();
                    }, scope.resultchangedelay);
                }
            };

            //#endregion

            //#region Verify

            //#endregion

            //#region Logic

            scope.Change = function () {
                if (scope.textboxmodel == null) {
                    scope.textboxmodel = "";
                }

                if (scope.isnumberformat) {
                    if (isNaN(parseFloat(scope.textboxmodel))) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }
                if (scope.isnumberformat3digit) {
                    if (isNaN(parseFloat(scope.textboxmodel))) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber3Digit();
                    }
                }
                if (scope.isinteger) {
                    var transformedInput = scope.textboxmodel.replace(/[^0-9]/g, '');
                    scope.textboxmodel = scope.textboxmodel.replace(',', '');
                    // transformedInput.replace(' ', '');
                    if (transformedInput !== scope.textboxmodel) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }

                if (scope.resultchangedelay) {
                    submit();
                } else if (scope.resultchange) {
                    $timeout(scope.resultchange);
                }
            }

            //Gọi lại hàm khi cần cho trường hợp xem lại có sẵn dữ liệu
            scope.Change();

            scope.KeyPress = function (event) {
                if (scope.submitonenter && (event.keyCode == 13 || event.which == 13)) {
                    $timeout(scope.submitonenter);
                }
            }

            scope.KeyDown = function (event) {
                if (scope.isnumberformat || scope.isnumberformat3digit || scope.textboxtype == "tel") {
                    //if ((!event.ctrlKey && (event.keyCode < 48 || event.keyCode > 57) &&
                    //    event.keyCode != 46 && event.keyCode != 13 && event.keyCode != 8 &&
                    //    event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 46 && event.keyCode != 8 &&
                    //    (event.keyCode != 190 || scope.textboxmodel.indexOf(".") > -1)) ||
                    //    event.shiftKey) {
                    //    event.preventDefault();
                    //}
                    var allow = ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 39 || event.keyCode == 37
                        || event.keyCode == 35 || event.keyCode == 36 || event.keyCode == 190 || event.keyCode == 110 || event.keyCode == 9 || event.keyCode == 33 || event.keyCode == 34
                        || event.keyCode == 8 || event.keyCode == 46 || event.ctrlKey);

                    if (!allow) {
                        event.preventDefault();
                    }
                }

                if (scope.disablenumber && event.keyCode > 48 && event.keyCode < 57) {
                    event.preventDefault();
                }

                if (scope.textboxmodel && scope.textboxmodel.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8) {
                    event.preventDefault();
                }
            }

            scope.Clear = function () {
                scope.textboxmodel = "";

                if (scope.cleartext) {
                    $timeout(scope.cleartext);
                }
            }

            scope.Blur = function () {
                if (scope.blur) {
                    $timeout(scope.blur);
                }

            }

            //#endregion

            //#endregion

            //#region Init

            var initialise = function () {
                if (scope.isnumberformat && scope.textboxtype != 'number') {
                    scope.type = "tel";
                } else {
                    scope.type = scope.textboxtype;
                }

                scope.textStyle = "padding-";
                scope.iconStyle = "position:absolute;font-size:20px;line-height:40px;height:40px;width:34px;color:black;";

                if (scope.iconposition == "left") {
                    scope.textStyle = scope.textStyle + "left:0px;text-align:right";
                    scope.iconStyle = scope.iconStyle + "left";
                } else {
                    scope.textStyle = scope.textStyle + "right:0px;text-align:left";
                    scope.iconStyle = scope.iconStyle + "right";
                }

                scope.iconStyle = scope.iconStyle + ":0";

                if (scope.islargetext) {
                    scope.textStyle = scope.textStyle + ";font-size:20px";
                }

                scope.maxLength = 100;
                if (scope.nfocus) {
                    scope.focus = scope.nfocus;
                }
                if (scope.textboxmaxlength) {
                    scope.maxLength = scope.textboxmaxlength;
                }

                if (scope.resultchangedelay) {
                    if (scope.resultchangeminlength) {
                        scope.resultchangeminlength = parseInt(scope.resultchangeminlength);
                    } else {
                        scope.resultchangeminlength = 0;
                    }
                }

                if (!scope.iconposition) {
                    scope.iconposition = "right";
                }
            }

            initialise();

            //#endregion
        },
    }
});
app.directive('formatnumberinput', function ($timeout) {
    return {
        restrict: "E",

        scope: {
            iconposition: "@",
            textboxtype: "@",
            textboxplaceholder: "@",
            textboxmodel: "=",
            textboxmaxlength: "@",
            resultchange: "&",
            resultchangedelay: "@",
            resultchangeminlength: "@",
            submitonenter: "&",
            cleartext: "&",
            blur: "&",
            disable: "=",
            textalignment: "@",
            isnumberformat: "=",
            isnumberformat3digit: "=",
            islargetext: "=",
            disablenumber: "=",
            allowonlynumber: "=",
            nfocus: "=",
            isinteger: "=",
            focustextbox: "=",
            textview: "=?"
        },

        transclude: true,
        templateUrl: "/Directives/ClearableTextBox.html?v=2",

        link: function (scope, element, attrs) {

            //#region Variable

            var submissionHandler = null;
            scope.textboxmodel = scope.textview + "";   //Parse to string

            //#endregion

            //#region Function

            //#region Support

            var formatNumber = function () {
                var vVal = scope.textboxmodel;
                if (vVal === "string")
                    vVal = scope.textboxmodel = scope.textboxmodel.trim();

                res = ParseVietnameseNumber(vVal);
                if (res === false) res = vVal;
                res = parseInt(res.toString().replace(/\./g, ""));
                var vValue = formatCurrency(res);
                scope.textboxmodel = vValue;
                try {
                    var inputelement = element[0].children[0].children[0];
                    // Cache references
                    var $el = $(inputelement),
                        el = inputelement;

                    // Only focus if input isn't already
                    if (!$el.is(":focus")) {
                        $el.focus();
                    }

                    // If this function exists... (IE 9+)
                    if (el.setSelectionRange) {

                        // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                        var len = $el.val().length * 2;

                        // Timeout seems to be required for Blink
                        setTimeout(function () {
                            el.setSelectionRange(len, len);
                        }, 1);

                    } else {
                        // As a fallback, replace the contents with itself
                        // Doesn't work in Chrome, but Chrome supports setSelectionRange
                        $el.val($el.val());

                    }

                    // Scroll to the bottom, in case we're in a tall textarea
                    // (Necessary for Firefox and Chrome)
                    this.scrollTop = 999999;
                } catch (e) { }
            }

            var submit = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                if (!scope.resultchangeminlength || (scope.textboxmodel && scope.textboxmodel.trim().length >= scope.resultchangeminlength)) {
                    submissionHandler = $timeout(function () {
                        scope.resultchange();
                    }, scope.resultchangedelay);
                }
            };

            //#endregion

            //#region Verify

            //#endregion

            //#region Logic

            scope.Change = function () {
                if (scope.textboxmodel == null) {
                    scope.textboxmodel = "";
                }

                formatNumber();

                if (scope.resultchangedelay) {
                    submit();
                } else if (scope.resultchange) {
                    $timeout(scope.resultchange);
                }
            }

            //Gọi lại hàm khi cần cho trường hợp xem lại có sẵn dữ liệu
            scope.Change();

            scope.KeyPress = function (event) {
                if (scope.submitonenter && (event.keyCode == 13 || event.which == 13)) {
                    $timeout(scope.submitonenter);
                }
            }

            scope.KeyDown = function (event) {
                if (scope.isnumberformat || scope.isnumberformat3digit || scope.textboxtype == "tel") {
                    var allow = ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 39 || event.keyCode == 37
                        || event.keyCode == 35 || event.keyCode == 36 || event.keyCode == 190 || event.keyCode == 110 || event.keyCode == 9 || event.keyCode == 33 || event.keyCode == 34
                        || event.keyCode == 8 || event.keyCode == 46 || event.ctrlKey);

                    if (!allow) {
                        event.preventDefault();
                    }
                }

                if (scope.disablenumber && event.keyCode > 48 && event.keyCode < 57) {
                    event.preventDefault();
                }

                if (scope.textboxmodel && scope.textboxmodel.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8) {
                    event.preventDefault();
                }
            }

            scope.Clear = function () {
                scope.textboxmodel = "";

                if (scope.cleartext) {
                    $timeout(scope.cleartext);
                }
            }

            scope.Blur = function () {
                if (scope.blur) {
                    $timeout(scope.blur);
                }

            }

            //#endregion

            //#endregion

            //#region Init

            var initialise = function () {
                if (scope.isnumberformat && scope.textboxtype != 'number') {
                    scope.type = "tel";
                } else {
                    scope.type = scope.textboxtype;
                }

                scope.textStyle = "padding-";
                scope.iconStyle = "position:absolute;font-size:20px;line-height:40px;height:40px;width:34px;color:black;";

                if (scope.iconposition == "left") {
                    scope.textStyle = scope.textStyle + "left:40px;text-align:right";
                    scope.iconStyle = scope.iconStyle + "left";
                } else {
                    scope.textStyle = scope.textStyle + "right:0;text-align:right";
                    scope.iconStyle = scope.iconStyle + "right";
                }

                scope.iconStyle = scope.iconStyle + ":0";

                if (scope.islargetext) {
                    scope.textStyle = scope.textStyle + ";font-size:20px";
                }

                scope.maxLength = 100;
                if (scope.nfocus) {
                    scope.focus = scope.nfocus;
                }
                if (scope.textboxmaxlength) {
                    scope.maxLength = scope.textboxmaxlength;
                }

                if (scope.resultchangedelay) {
                    if (scope.resultchangeminlength) {
                        scope.resultchangeminlength = parseInt(scope.resultchangeminlength);
                    } else {
                        scope.resultchangeminlength = 0;
                    }
                }

                if (!scope.iconposition) {
                    scope.iconposition = "right";
                }
            }

            initialise();

            //#endregion
        },
    }
});

app.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;


            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });


            ctrl.$parsers.unshift(function (viewValue) {
                var plainNumber = viewValue.replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber));
                return plainNumber;
            });
        }
    };
}]);

app.directive('hasPermission', ['permissions', function (permissions) {
    return {
        link: function (scope, element, attrs) { element.ready(function () {
                var value = attrs.hasPermission.trim();
                var notPermissionFlag = value[0] === '!';
                if (notPermissionFlag) {
                    value = value.slice(1).trim();
                }

                function toggleVisibilityBasedOnPermission() {
                    var hasPermission = permissions.hasPermission([value]);
                    if (hasPermission && !notPermissionFlag || !hasPermission && notPermissionFlag) {
                        // Do nothing
                    }
                    else {
                        element.addClass("disabled-element");
                    }
                }
                toggleVisibilityBasedOnPermission();
                scope.$on('permissionsChanged', toggleVisibilityBasedOnPermission);
            });

        }
    };
}]);

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
}

function UpdateFormat(id, value) {
    if (value === undefined) {
        value = ParseVietnameseNumber($(id).val());
        if (value === false) value = $(id).val();
    }
    value = FormatVietnameseNumber(value);

    if (value !== false) $(id).val(value);

    //ReadNumber(ParseVietnameseNumber($(id).val()));
}

function ParseVietnameseNumber(value) {
    if (!ValidateVietnameseNumber(value)) return false;

    var valueS = value.toString().replace(/\./g, "").replace(/\,/g, ".");
    value = parseFloat(valueS);
    if (isNaN(value)) return false;

    return value;
}

function ValidateVietnameseNumber(value) {
    if (value === undefined) return false;

    value = value.toString();

    var commaParts = value.split(",");
    if (commaParts.length > 2) return false; // có nhiều hơn 1 dấu phẩy là sai

    if (commaParts.length == 2 && commaParts[1].indexOf('.') >= 0) return false; // có dấu . đằng sau dấu , là sai

    for (i = 0; i < commaParts.length; i++) {
        if (commaParts[i].length == 0) return false; // có dấu phẩy đứng đầu hoặc cuối là sai
    }

    var pointParts = value.split("."); // tách thành các phần phân cách nhau bởi dấu chấm để kiểm tra từng phần

    if (pointParts.length == 1) return !isNaN(pointParts[0].replace(/\,/g, ".")); //không có dấu chấm (có hoặc không có dấu phẩy): trả về đúng nếu là số

    for (i = 0; i < pointParts.length; i++) {
        if (pointParts[i].length == 0) return false; // có dấu chấm đứng đầu hoặc cuối là sai

        var type = 'first';
        if (i > 0 && i < pointParts.length - 1) type = 'middle';
        else if (i > 0 && i == pointParts.length - 1) type = 'last';

        if (!CheckPartVietnameseNumber(type, pointParts[i])) return false;
    }
    return true;
}

function CheckPartVietnameseNumber(type, p) {
    var ic = p.indexOf(',');
    if (type == 'first') {
        if (p.length > 3) return false;
        if (ic >= 0) return false;
    } else if (type == 'middle') {
        if (p.length != 3) return false;
        if (ic >= 0) return false;
    } else if (type == 'last') {
        var commaParts = p.split(",");
        if (commaParts[0].length != 3) return false;
        p = p.replace(",", "."); //chuyển về dạng số mà code hiểu
    } else return false;

    return !isNaN(p); //trả về đúng nếu là số
}

function Comma(Num) {
    Num += '';
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    x = Num.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    return x1 + x2;
}

function replaceAll(varb, replaceThis, replaceBy) {
    var newvarbarray = varb.toString().split(replaceThis);
    var newvarb = newvarbarray.join(replaceBy);
    return newvarb;
}

function formatCurrency(num) {
    var le = num % 1;
    if (le > 0)
        return num.formatMoney(2, ',', '.');
}

function formatCurrency(num, n) {
    var le = num % 1;
    if (le > 0)
        return num.formatMoney(n, ',', '.');
    else return formatMoneynum.formatMoney(0, ',', '.');
}

function FormatVietnameseNumber(value) { // format value ở dạng số mà code hiểu sang dạng Việt Nam
    if (isNaN(value)) return false;
    var pointParts = value.toString().split(".");

    value = "";
    var first = pointParts[0];
    while (first.length > 0) {
        var i = first.length - 3;
        if (i < 0) i = 0;
        if (value.length > 0) value = "." + value;
        value = first.substring(i) + value;
        first = first.substring(0, i);
    }
    if (pointParts.length > 1) value = value + "," + pointParts[1];
    console.log(value)
    return value;
}

function Beep() {
    var snd = new Audio("data:audio/wav;base64,//uQRAAAAWMSLwUIYAAsYkXgoQwAEaYLWfkWgAI0wWs/ItAAAGDgYtAgAyN+QWaAAihwMWm4G8QQRDiMcCBcH3Cc+CDv/7xA4Tvh9Rz/y8QADBwMWgQAZG/ILNAARQ4GLTcDeIIIhxGOBAuD7hOfBB3/94gcJ3w+o5/5eIAIAAAVwWgQAVQ2ORaIQwEMAJiDg95G4nQL7mQVWI6GwRcfsZAcsKkJvxgxEjzFUgfHoSQ9Qq7KNwqHwuB13MA4a1q/DmBrHgPcmjiGoh//EwC5nGPEmS4RcfkVKOhJf+WOgoxJclFz3kgn//dBA+ya1GhurNn8zb//9NNutNuhz31f////9vt///z+IdAEAAAK4LQIAKobHItEIYCGAExBwe8jcToF9zIKrEdDYIuP2MgOWFSE34wYiR5iqQPj0JIeoVdlG4VD4XA67mAcNa1fhzA1jwHuTRxDUQ//iYBczjHiTJcIuPyKlHQkv/LHQUYkuSi57yQT//uggfZNajQ3Vmz+Zt//+mm3Wm3Q576v////+32///5/EOgAAADVghQAAAAA//uQZAUAB1WI0PZugAAAAAoQwAAAEk3nRd2qAAAAACiDgAAAAAAABCqEEQRLCgwpBGMlJkIz8jKhGvj4k6jzRnqasNKIeoh5gI7BJaC1A1AoNBjJgbyApVS4IDlZgDU5WUAxEKDNmmALHzZp0Fkz1FMTmGFl1FMEyodIavcCAUHDWrKAIA4aa2oCgILEBupZgHvAhEBcZ6joQBxS76AgccrFlczBvKLC0QI2cBoCFvfTDAo7eoOQInqDPBtvrDEZBNYN5xwNwxQRfw8ZQ5wQVLvO8OYU+mHvFLlDh05Mdg7BT6YrRPpCBznMB2r//xKJjyyOh+cImr2/4doscwD6neZjuZR4AgAABYAAAABy1xcdQtxYBYYZdifkUDgzzXaXn98Z0oi9ILU5mBjFANmRwlVJ3/6jYDAmxaiDG3/6xjQQCCKkRb/6kg/wW+kSJ5//rLobkLSiKmqP/0ikJuDaSaSf/6JiLYLEYnW/+kXg1WRVJL/9EmQ1YZIsv/6Qzwy5qk7/+tEU0nkls3/zIUMPKNX/6yZLf+kFgAfgGyLFAUwY//uQZAUABcd5UiNPVXAAAApAAAAAE0VZQKw9ISAAACgAAAAAVQIygIElVrFkBS+Jhi+EAuu+lKAkYUEIsmEAEoMeDmCETMvfSHTGkF5RWH7kz/ESHWPAq/kcCRhqBtMdokPdM7vil7RG98A2sc7zO6ZvTdM7pmOUAZTnJW+NXxqmd41dqJ6mLTXxrPpnV8avaIf5SvL7pndPvPpndJR9Kuu8fePvuiuhorgWjp7Mf/PRjxcFCPDkW31srioCExivv9lcwKEaHsf/7ow2Fl1T/9RkXgEhYElAoCLFtMArxwivDJJ+bR1HTKJdlEoTELCIqgEwVGSQ+hIm0NbK8WXcTEI0UPoa2NbG4y2K00JEWbZavJXkYaqo9CRHS55FcZTjKEk3NKoCYUnSQ0rWxrZbFKbKIhOKPZe1cJKzZSaQrIyULHDZmV5K4xySsDRKWOruanGtjLJXFEmwaIbDLX0hIPBUQPVFVkQkDoUNfSoDgQGKPekoxeGzA4DUvnn4bxzcZrtJyipKfPNy5w+9lnXwgqsiyHNeSVpemw4bWb9psYeq//uQZBoABQt4yMVxYAIAAAkQoAAAHvYpL5m6AAgAACXDAAAAD59jblTirQe9upFsmZbpMudy7Lz1X1DYsxOOSWpfPqNX2WqktK0DMvuGwlbNj44TleLPQ+Gsfb+GOWOKJoIrWb3cIMeeON6lz2umTqMXV8Mj30yWPpjoSa9ujK8SyeJP5y5mOW1D6hvLepeveEAEDo0mgCRClOEgANv3B9a6fikgUSu/DmAMATrGx7nng5p5iimPNZsfQLYB2sDLIkzRKZOHGAaUyDcpFBSLG9MCQALgAIgQs2YunOszLSAyQYPVC2YdGGeHD2dTdJk1pAHGAWDjnkcLKFymS3RQZTInzySoBwMG0QueC3gMsCEYxUqlrcxK6k1LQQcsmyYeQPdC2YfuGPASCBkcVMQQqpVJshui1tkXQJQV0OXGAZMXSOEEBRirXbVRQW7ugq7IM7rPWSZyDlM3IuNEkxzCOJ0ny2ThNkyRai1b6ev//3dzNGzNb//4uAvHT5sURcZCFcuKLhOFs8mLAAEAt4UWAAIABAAAAAB4qbHo0tIjVkUU//uQZAwABfSFz3ZqQAAAAAngwAAAE1HjMp2qAAAAACZDgAAAD5UkTE1UgZEUExqYynN1qZvqIOREEFmBcJQkwdxiFtw0qEOkGYfRDifBui9MQg4QAHAqWtAWHoCxu1Yf4VfWLPIM2mHDFsbQEVGwyqQoQcwnfHeIkNt9YnkiaS1oizycqJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU=");
    snd.play();
}

app.directive('suggestproduct', function ($timeout, $rootScope) {
    return {
        restrict: "E",
        //require: "?ngModel",
        scope: {
            textboxplaceholder: "=?",
            pagesize: "=?",
            selectsearchresult: "&",
            keypress: "&",
            //selectresultmodel: "=?",//Trả về ProductModel
            selectresultid: "=?",//Trả về ProductID
            selectbarcoderesult: "&",
            autoclear: "=?",
            isbasequantityunit: "=?", //Khi bắn barcode sẽ luôn luôn trả ra mã sản phẩm có đơn vị tính cơ sở
            delayresearch: "=?",
            textboxmaxlength: "=?",
            textview: "=?",
            disabled: "=?",
            userkeycode: "=?",
            fucusme: "=?"
        },
        transclude: true,
        templateUrl: "/Directives/SuggestProduct.html?v=12",
        link: function (scope, element, attrs) {
            //if (!ngModel) return;

            var submissionHandler = null;
            //#region Variable
            //Cờ show list product
            scope.isShowlist = true;
            scope.isMousOverFocus = false;
            scope.delayresearch = scope.delayresearch || 500;
            scope.textboxmaxlength = scope.textboxmaxlength || 1000;
            scope.keyword = scope.textview || '';
            scope.fucusme = scope.fucusme || true;
            //#endregion

            scope.clearTextSearch = function () {
                scope.lstObject = null;
                scope.keyword = null;
            }

            scope.KeyDown = function (event) {
                if (event.keyCode == 40 || event.keyCode == 38) {  //Down UP arrow
                    if (scope.lstObject != null && scope.lstObject.length > 0) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //index =-1 có nghĩa là chưa có item được chọn, index > -1 có nghĩa là đang được chọn
                        //Tăng lên 1 để chọn cái tiếp theo
                        //Giảm đi 1 để chọn cái bên trên
                        index += (event.keyCode == 40 ? 1 : -1);

                        //Nếu tăng quá mức thì quay về đầu tiên index = 0
                        if (index >= scope.lstObject.length)
                            index = 0;
                        else if (index < 0)
                            index = scope.lstObject.length - 1;

                        //Clear hết trước khi chọn lại
                        scope.lstObject.forEach(function (obj) {
                            obj.isSelected = false;
                        });
                        //Chọn cái mới
                        scope.lstObject[index].isSelected = true;
                    } else {
                        //Gọi hàm thêm dòng, nếu được khai báo
                        //ngModel.$setViewValue(event.keyCode);
                        scope.userkeycode = event.keyCode;
                        scope.keypress();
                    }
                } else if (event.keyCode == 13) {
                    //Enter
                    if (scope.lstObject != null && scope.lstObject.length > 0 && scope.isShowlist) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //Nếu chưa có cái nào được chọn thì chọn cái đầu tiên
                        index = index == -1 ? 0 : index;
                        scope.redirect(scope.lstObject[index]);
                    }

                    //Gọi hàm thêm dòng, nếu được khai báo
                    //ngModel.$setViewValue(event.keyCode);
                    scope.userkeycode = event.keyCode;
                    scope.keypress();

                } else if (scope.keyword && scope.keyword.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8)
                    event.preventDefault();
            }

            scope.LossFocus = function () {
                if (!scope.isMousOverFocus) {
                    scope.isShowlist = false;
                }
            }

            scope.MousOverFocus = function (isOver) {
                scope.isMousOverFocus = isOver;
                if (isOver) {
                    //Clear selection
                    for (var i = 0; i < scope.lstObject.length; i++) {
                        if (scope.lstObject[i].isSelected) {
                            scope.lstObject[i].isSelected = false;
                            break;
                        }
                    }
                }
            }

            scope.search = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                submissionHandler = $timeout(function () {
                    if (scope.keyword.length >= 2) {
                        scope.isSearching = true;
                        scope.errorMessage = null;

                        $.ajax({
                            url: "/Product/SuggestByObject",
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json;charset=utf-8;',
                            async: false,
                            data: JSON.stringify({ strKeyword: scope.keyword, intPageSize: scope.pagesize }),
                            success: function (result) {
                                $timeout(function () {
                                    if (result) {
                                        if (result.rs) {
                                            //Thêm điều kiện keyword.length > 0 để tránh trường hợp người dùng chọn rồi mà server chưa phản hồi danh sách
                                            if (scope.keyword.length > 0) {
                                                scope.lstObject = result.listResult;
                                            }
                                        } else {
                                            alert(result.msg);
                                        }
                                    }
                                    //Set biến không phải quét từ mã vạch
                                    scope.isRedirect = false;
                                    scope.isSearching = false;
                                });
                            }
                        });
                    } else {
                        //Set biến không phải quét từ mã vạch
                        scope.isRedirect = false;
                        scope.lstObject = null;
                    }

                    scope.textview = scope.keyword;
                }, scope.delayresearch);
            }

            scope.redirect = function (obj) {
                $timeout.cancel(submissionHandler);
                scope.isSearching = false;
                scope.lstObject = null;
                //Gán giá trị đã chọn cho biến toàn cục
                $rootScope.selectedResultobj = obj;
                //Gọi hàm xử lý
                $timeout(scope.selectsearchresult, 50);
                //Gán lại vào input
                scope.keyword = obj.PRODUCTNAME;
            }
        }
    };
});

app.directive('suggestcustomer', function ($timeout, $rootScope) {
    return {
        restrict: "E",
        //require: "?ngModel",
        scope: {
            textboxplaceholder: "=?",
            pagesize: "=?",
            selectsearchresult: "&",
            keypress: "&",
            //selectresultmodel: "=?",//Trả về ProductModel
            selectresultid: "=?",//Trả về ProductID
            selectbarcoderesult: "&",
            autoclear: "=?",
            isbasequantityunit: "=?", //Khi bắn barcode sẽ luôn luôn trả ra mã sản phẩm có đơn vị tính cơ sở
            delayresearch: "=?",
            textboxmaxlength: "=?",
            textview: "=?",
            disabled: "=?",
            userkeycode: "=?",
            fucusme: "=?",
            keyword: "=?",
            url: "=?",
        },
        transclude: true,
        templateUrl: "/Directives/SuggestCustomer.html?v=2",
        link: function (scope, element, attrs) {
            //if (!ngModel) return;

            var submissionHandler = null;
            //#region Variable
            //Cờ show list product
            scope.isShowlist = true;
            scope.isMousOverFocus = false;
            scope.delayresearch = scope.delayresearch || 500;
            scope.textboxmaxlength = scope.textboxmaxlength || 1000;
            //scope.keyword = scope.textview || '';
            scope.fucusme = scope.fucusme || true;
            //console.log(scope.keyword);
            //#endregion

            scope.clearTextSearch = function () {
                scope.lstObject = null;
                scope.keyword = null;
            }

            scope.KeyDown = function (event) {
                if (event.keyCode == 40 || event.keyCode == 38) {  //Down UP arrow
                    if (scope.lstObject != null && scope.lstObject.length > 0) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //index =-1 có nghĩa là chưa có item được chọn, index > -1 có nghĩa là đang được chọn
                        //Tăng lên 1 để chọn cái tiếp theo
                        //Giảm đi 1 để chọn cái bên trên
                        index += (event.keyCode == 40 ? 1 : -1);

                        //Nếu tăng quá mức thì quay về đầu tiên index = 0
                        if (index >= scope.lstObject.length)
                            index = 0;
                        else if (index < 0)
                            index = scope.lstObject.length - 1;

                        //Clear hết trước khi chọn lại
                        scope.lstObject.forEach(function (obj) {
                            obj.isSelected = false;
                        });
                        //Chọn cái mới
                        scope.lstObject[index].isSelected = true;
                    } else {
                        //Gọi hàm thêm dòng, nếu được khai báo
                        //ngModel.$setViewValue(event.keyCode);
                        scope.userkeycode = event.keyCode;
                        scope.keypress();
                    }
                } else if (event.keyCode == 13) {
                    //Enter
                    if (scope.lstObject != null && scope.lstObject.length > 0 && scope.isShowlist) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //Nếu chưa có cái nào được chọn thì chọn cái đầu tiên
                        index = index == -1 ? 0 : index;
                        scope.redirect(scope.lstObject[index]);
                    }

                    //Gọi hàm thêm dòng, nếu được khai báo
                    //ngModel.$setViewValue(event.keyCode);
                    scope.userkeycode = event.keyCode;
                    scope.keypress();

                } else if (scope.keyword && scope.keyword.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8)
                    event.preventDefault();
            }

            scope.LossFocus = function () {
                if (!scope.isMousOverFocus) {
                    scope.isShowlist = false;
                }
            }

            scope.MousOverFocus = function (isOver) {
                scope.isMousOverFocus = isOver;
                if (isOver) {
                    //Clear selection
                    for (var i = 0; i < scope.lstObject.length; i++) {
                        if (scope.lstObject[i].isSelected) {
                            scope.lstObject[i].isSelected = false;
                            break;
                        }
                    }
                }
            }

            scope.search = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                submissionHandler = $timeout(function () {
                    if (scope.keyword.length >= 2) {
                        scope.isSearching = true;
                        scope.errorMessage = null;

                        $.ajax({
                            url: scope.url,
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json;charset=utf-8;',
                            async: false,
                            data: JSON.stringify({ strKeyword: scope.keyword, intPageSize: scope.pagesize }),
                            success: function (result) {
                                $timeout(function () {
                                    if (result) {
                                        if (result.rs) {
                                            //Thêm điều kiện keyword.length > 0 để tránh trường hợp người dùng chọn rồi mà server chưa phản hồi danh sách
                                            if (scope.keyword.length > 0) {
                                                scope.lstObject = result.listResult;
                                            }
                                        } else {
                                            alert(result.msg);
                                        }
                                    }
                                    //Set biến không phải quét từ mã vạch
                                    scope.isRedirect = false;
                                    scope.isSearching = false;
                                });
                            }
                        });
                    } else {
                        //Set biến không phải quét từ mã vạch
                        scope.isRedirect = false;
                        scope.lstObject = null;
                    }

                    //scope.textview = scope.keyword;
                }, scope.delayresearch);
            }

            scope.redirect = function (obj) {
                $timeout.cancel(submissionHandler);
                scope.isSearching = false;
                scope.lstObject = null;
                //Gán giá trị đã chọn cho biến toàn cục
                $rootScope.selectedCustomer = obj;
                //Gọi hàm xử lý
                $timeout(scope.selectsearchresult, 50);
                //Gán lại vào input
                scope.keyword = obj.CUSNAME;
            }
        }
    };
});

app.directive('suggestemployee', function ($timeout, $rootScope) {
    return {
        restrict: "E",
        //require: "?ngModel",
        scope: {
            textboxplaceholder: "=?",
            pagesize: "=?",
            selectsearchresult: "&",
            keypress: "&",
            //selectresultmodel: "=?",//Trả về ProductModel
            selectresultid: "=?",//Trả về ProductID
            selectbarcoderesult: "&",
            autoclear: "=?",
            isbasequantityunit: "=?", //Khi bắn barcode sẽ luôn luôn trả ra mã sản phẩm có đơn vị tính cơ sở
            delayresearch: "=?",
            textboxmaxlength: "=?",
            textview: "=?",
            disabled: "=?",
            userkeycode: "=?",
            fucusme: "=?",
            keyword: "=?"
        },
        transclude: true,
        templateUrl: "/Directives/SuggestEmployee.html?v=2",
        link: function (scope, element, attrs) {
            //if (!ngModel) return;

            var submissionHandler = null;
            //#region Variable
            //Cờ show list product
            scope.isShowlist = true;
            scope.isMousOverFocus = false;
            scope.delayresearch = scope.delayresearch || 500;
            scope.textboxmaxlength = scope.textboxmaxlength || 1000;
            //scope.keyword = scope.textview || '';
            scope.fucusme = scope.fucusme || true;
            //console.log(scope.keyword);
            //#endregion

            scope.clearTextSearch = function () {
                scope.lstObject = null;
                scope.keyword = null;
            }

            scope.KeyDown = function (event) {
                if (event.keyCode == 40 || event.keyCode == 38) {  //Down UP arrow
                    if (scope.lstObject != null && scope.lstObject.length > 0) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //index =-1 có nghĩa là chưa có item được chọn, index > -1 có nghĩa là đang được chọn
                        //Tăng lên 1 để chọn cái tiếp theo
                        //Giảm đi 1 để chọn cái bên trên
                        index += (event.keyCode == 40 ? 1 : -1);

                        //Nếu tăng quá mức thì quay về đầu tiên index = 0
                        if (index >= scope.lstObject.length)
                            index = 0;
                        else if (index < 0)
                            index = scope.lstObject.length - 1;

                        //Clear hết trước khi chọn lại
                        scope.lstObject.forEach(function (obj) {
                            obj.isSelected = false;
                        });
                        //Chọn cái mới
                        scope.lstObject[index].isSelected = true;
                    } else {
                        //Gọi hàm thêm dòng, nếu được khai báo
                        //ngModel.$setViewValue(event.keyCode);
                        scope.userkeycode = event.keyCode;
                        scope.keypress();
                    }
                } else if (event.keyCode == 13) {
                    //Enter
                    if (scope.lstObject != null && scope.lstObject.length > 0 && scope.isShowlist) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //Nếu chưa có cái nào được chọn thì chọn cái đầu tiên
                        index = index == -1 ? 0 : index;
                        scope.redirect(scope.lstObject[index]);
                    }

                    //Gọi hàm thêm dòng, nếu được khai báo
                    //ngModel.$setViewValue(event.keyCode);
                    scope.userkeycode = event.keyCode;
                    scope.keypress();

                } else if (scope.keyword && scope.keyword.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8)
                    event.preventDefault();
            }

            scope.LossFocus = function () {
                if (!scope.isMousOverFocus) {
                    scope.isShowlist = false;
                }
            }

            scope.MousOverFocus = function (isOver) {
                scope.isMousOverFocus = isOver;
                if (isOver) {
                    //Clear selection
                    for (var i = 0; i < scope.lstObject.length; i++) {
                        if (scope.lstObject[i].isSelected) {
                            scope.lstObject[i].isSelected = false;
                            break;
                        }
                    }
                }
            }

            scope.search = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                submissionHandler = $timeout(function () {
                    if (scope.keyword.length >= 2) {
                        scope.isSearching = true;
                        scope.errorMessage = null;

                        $.ajax({
                            url: "/Customer/SuggestByObject",
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json;charset=utf-8;',
                            async: false,
                            data: JSON.stringify({ strKeyword: scope.keyword, intPageSize: scope.pagesize }),
                            success: function (result) {
                                $timeout(function () {
                                    if (result) {
                                        if (result.rs) {
                                            //Thêm điều kiện keyword.length > 0 để tránh trường hợp người dùng chọn rồi mà server chưa phản hồi danh sách
                                            if (scope.keyword.length > 0) {
                                                scope.lstObject = result.listResult;
                                            }
                                        } else {
                                            alert(result.msg);
                                        }
                                    }
                                    //Set biến không phải quét từ mã vạch
                                    scope.isRedirect = false;
                                    scope.isSearching = false;
                                });
                            }
                        });
                    } else {
                        //Set biến không phải quét từ mã vạch
                        scope.isRedirect = false;
                        scope.lstObject = null;
                    }

                    //scope.textview = scope.keyword;
                }, scope.delayresearch);
            }

            scope.redirect = function (obj) {
                $timeout.cancel(submissionHandler);
                scope.isSearching = false;
                scope.lstObject = null;
                //Gán giá trị đã chọn cho biến toàn cục
                $rootScope.selectedCustomer = obj;
                //Gọi hàm xử lý
                $timeout(scope.selectsearchresult, 50);
                //Gán lại vào input
                scope.keyword = obj.CUSBUYER;
            }
        }
    };
});

app.directive('suggestuser', function ($timeout, $rootScope) {
    return {
        restrict: "E",
        //require: "?ngModel",
        scope: {
            textboxplaceholder: "=?",
            pagesize: "=?",
            selectsearchresult: "&",
            keypress: "&",
            //selectresultmodel: "=?",//Trả về ProductModel
            selectresultid: "=?",//Trả về ProductID
            selectbarcoderesult: "&",
            autoclear: "=?",
            isbasequantityunit: "=?", //Khi bắn barcode sẽ luôn luôn trả ra mã sản phẩm có đơn vị tính cơ sở
            delayresearch: "=?",
            textboxmaxlength: "=?",
            textview: "=?",
            disabled: "=?",
            userkeycode: "=?",
            fucusme: "=?",
            keyword: "=?",
            url: "=?",
        },
        transclude: true,
        templateUrl: "/Directives/SuggestUser.html?v=2",
        link: function (scope, element, attrs) {
            //if (!ngModel) return;

            var submissionHandler = null;
            //#region Variable
            //Cờ show list product
            scope.isShowlist = true;
            scope.isMousOverFocus = false;
            scope.delayresearch = scope.delayresearch || 500;
            scope.textboxmaxlength = scope.textboxmaxlength || 1000;
            //scope.keyword = scope.textview || '';
            scope.fucusme = scope.fucusme || true;
            //console.log(scope.keyword);
            //#endregion

            scope.clearTextSearch = function () {
                scope.lstObject = null;
                scope.keyword = null;
            }

            scope.KeyDown = function (event) {
                if (event.keyCode == 40 || event.keyCode == 38) {  //Down UP arrow
                    if (scope.lstObject != null && scope.lstObject.length > 0) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //index =-1 có nghĩa là chưa có item được chọn, index > -1 có nghĩa là đang được chọn
                        //Tăng lên 1 để chọn cái tiếp theo
                        //Giảm đi 1 để chọn cái bên trên
                        index += (event.keyCode == 40 ? 1 : -1);

                        //Nếu tăng quá mức thì quay về đầu tiên index = 0
                        if (index >= scope.lstObject.length)
                            index = 0;
                        else if (index < 0)
                            index = scope.lstObject.length - 1;

                        //Clear hết trước khi chọn lại
                        scope.lstObject.forEach(function (obj) {
                            obj.isSelected = false;
                        });
                        //Chọn cái mới
                        scope.lstObject[index].isSelected = true;
                    } else {
                        //Gọi hàm thêm dòng, nếu được khai báo
                        //ngModel.$setViewValue(event.keyCode);
                        scope.userkeycode = event.keyCode;
                        scope.keypress();
                    }
                } else if (event.keyCode == 13) {
                    //Enter
                    if (scope.lstObject != null && scope.lstObject.length > 0 && scope.isShowlist) {
                        var index = -1;
                        //Tìm item đang được chọn
                        for (var i = 0; i < scope.lstObject.length; i++) {
                            if (scope.lstObject[i].isSelected) {
                                index = i;
                                break;
                            }
                        }
                        //Nếu chưa có cái nào được chọn thì chọn cái đầu tiên
                        index = index == -1 ? 0 : index;
                        scope.redirect(scope.lstObject[index]);
                    }

                    //Gọi hàm thêm dòng, nếu được khai báo
                    //ngModel.$setViewValue(event.keyCode);
                    scope.userkeycode = event.keyCode;
                    scope.keypress();

                } else if (scope.keyword && scope.keyword.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8)
                    event.preventDefault();
            }

            scope.LossFocus = function () {
                if (!scope.isMousOverFocus) {
                    scope.isShowlist = false;
                }
            }

            scope.MousOverFocus = function (isOver) {
                scope.isMousOverFocus = isOver;
                if (isOver) {
                    //Clear selection
                    for (var i = 0; i < scope.lstObject.length; i++) {
                        if (scope.lstObject[i].isSelected) {
                            scope.lstObject[i].isSelected = false;
                            break;
                        }
                    }
                }
            }

            scope.search = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                submissionHandler = $timeout(function () {
                    if (scope.keyword.length >= 2) {
                        scope.isSearching = true;
                        scope.errorMessage = null;

                        $.ajax({
                            url: scope.url,
                            type: "POST",
                            dataType: 'json',
                            contentType: 'application/json;charset=utf-8;',
                            async: false,
                            data: JSON.stringify({ strKeyword: scope.keyword, intPageSize: scope.pagesize }),
                            success: function (result) {
                                $timeout(function () {
                                    if (result) {
                                        if (result.rs) {
                                            //Thêm điều kiện keyword.length > 0 để tránh trường hợp người dùng chọn rồi mà server chưa phản hồi danh sách
                                            if (scope.keyword.length > 0) {
                                                scope.lstObject = result.listResult;
                                            }
                                        } else {
                                            alert(result.msg);
                                        }
                                    }
                                    //Set biến không phải quét từ mã vạch
                                    scope.isRedirect = false;
                                    scope.isSearching = false;
                                });
                            }
                        });
                    } else {
                        //Set biến không phải quét từ mã vạch
                        scope.isRedirect = false;
                        scope.lstObject = null;
                    }

                    //scope.textview = scope.keyword;
                }, scope.delayresearch);
            }

            scope.redirect = function (obj) {
                $timeout.cancel(submissionHandler);
                scope.isSearching = false;
                scope.lstObject = null;
                //Gán giá trị đã chọn cho biến toàn cục
                $rootScope.SelectedUser = obj;
                //Gọi hàm xử lý
                $timeout(scope.selectsearchresult, 50);
                //Gán lại vào input
                scope.keyword = obj.EMAIL;
            }
        }
    };
});
app.directive("onlyNumber", function () {
    return {
        restrict: "A",
        link: function (scope, element, attr) {
            element.bind('input', function () {
                var position = this.selectionStart - 1;

                //remove all but number and .
                var fixed = this.value.replace(/[^0-9\.]/g, '');
                if (fixed.charAt(0) === '.')                  //can't start with .
                    fixed = fixed.slice(1);

                var pos = fixed.indexOf(".") + 1;
                if (pos >= 0)               //avoid more than one .
                    fixed = fixed.substr(0, pos) + fixed.slice(pos).replace('.', '');

                if (this.value != fixed) {
                    this.value = fixed;
                    this.selectionStart = position;
                    this.selectionEnd = position;
                }
            });
        }
    };
});

app.directive('ngMin', function ($rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            scope.$watch(attr.ngMin, function () {
                ctrl.$setViewValue(ctrl.$viewValue);
            });
            var minValidator = function (value) {
                var min = scope.$eval(attr.ngMin) || 0;
                if (!$rootScope.isNullOrEmpty(value) && value < min) {
                    ctrl.$setValidity('ngMin', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMin', true);
                    return value;
                }
            };

            ctrl.$parsers.push(minValidator);
            ctrl.$formatters.push(minValidator);
        }
    };
});

app.directive('ngMax', function ($rootScope) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            elem.bind('input', function () {
                var val = this.value.replace(/[^0-9\.]/g, '');
                var max = parseFloat(scope.$eval(attr.ngMax)) || Infinity;

                var intVal = parseFloat(val);

                if (val != null && val != '' && intVal > max) {
                    while (intVal > max) {
                        val = val.slice(0, -1);
                        intVal = parseFloat(val);
                    }
                    this.value = val;
                }
            });
        }
    };
});

app.directive('focusOn', ['$timeout',
    function ($timeout) {
        var checkDirectivePrerequisites = function (attrs) {
            if (!attrs.focusOn && attrs.focusOn != "") {
                throw "FocusOnCondition missing attribute to evaluate";
            }
        }

        return {
            restrict: "A",
            link: function (scope, element, attrs, ctrls) {
                checkDirectivePrerequisites(attrs);

                scope.$watch(attrs.focusOn, function (currentValue, lastValue) {
                    if (currentValue == true) {
                        $timeout(function () {
                            element.focus();
                            //element.selectionStart = element.selectionEnd = element.value.length;
                            //element.selectionStart = element.selectionEnd = 10000;
                            //var range = element.createTextRange();
                            //range.collapse(false);
                            //range.select();
                        });
                    }
                });
            }
        };
    }
]);

app.directive('capitalize', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            var capitalize = function (inputValue) {
                if (inputValue == undefined) inputValue = '';
                var capitalized = inputValue.toUpperCase();
                if (capitalized !== inputValue) {
                    var selection = element[0].selectionStart;
                    modelCtrl.$setViewValue(capitalized);
                    modelCtrl.$render();
                    element[0].selectionStart = selection;
                    element[0].selectionEnd = selection;
                }
                return capitalized;
            }
            modelCtrl.$parsers.push(capitalize);
            capitalize(scope[attrs.ngModel]); // capitalize initial value
        }
    };
});

app.directive('selectOnClick', ['$window', function ($window) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function () {
                if (!$window.getSelection().toString()) {
                    // Required for mobile Safari
                    this.setSelectionRange(0, this.value.length)
                }
            });
        }
    };
}]);

app.directive('customOnChange', ['$timeout', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $timeout(function () {
                var onChangeFunc = scope.$eval(attrs.customOnChange);
                element.bind('change', onChangeFunc);
            }, 100);
        }
    };
}]);

app.directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return parseInt(val, 10);
            });
            ngModel.$formatters.push(function (val) {
                return '' + val;
            });
        }
    };
});

app.directive('format', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;

            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.format)(ctrl.$modelValue)
            });

            elem.bind('blur', function (event) {
                var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                elem.val($filter(attrs.format)(plainNumber));
            });
        }
    };
}]);

app.directive('myEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.myEnter);
                });
                event.preventDefault();
            }
        });
    };
});

app.directive('ckEditor', function () {
    return {
        require: '?ngModel',
        link: function (scope, elm, attr, ngModel) {
            var ck = CKEDITOR.replace(elm[0]);

            if (!ngModel) return;

            ck.on('pasteState', function () {
                scope.$apply(function () {
                    ngModel.$setViewValue(ck.getData());
                });
            });

            ngModel.$render = function (value) {
                ck.setData(ngModel.$viewValue);
            };
        }
    };
});

app.filter('numberFormat', function () {
    return function (n, decPlaces, thouSeparator, decSeparator, trimZero, emptyWhenZero) {
        try {
            if (isNaN(n)) {
                return 0;
            }

            if (n == 0) {
                if (emptyWhenZero == "true") {
                    return "";
                } else if (trimZero == "true") {
                    return 0;
                }
            }

            var decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
                decSeparator = decSeparator == undefined ? "." : decSeparator,
                thouSeparator = thouSeparator == undefined ? "," : thouSeparator,
                sign = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(decPlaces)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return sign + (j ? i.substr(0, j) + thouSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thouSeparator) + (decPlaces ? decSeparator + Math.abs(n - i).toFixed(decPlaces).slice(2) : "");
        } catch (e) {
            return n;
        }
    }
});

app.filter('dateTimeFormat', function ($filter) {
    function getCurrentDate() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = "0" + dd;
        }

        if (mm < 10) {
            mm = "0" + mm;
        }

        today = dd + "/" + mm + "/" + yyyy;
        return today;
    };

    return function (input) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var date;
        var resultDate;
        if (input.indexOf("Date") > -1) {
            resultDate = new Date(+temp);
            date = $filter("date")(resultDate, "dd/MM/yyyy");
            var utc = resultDate.getTime() + (resultDate.getTimezoneOffset() * 60000);
            // create new Date object for different city
            // using supplied offset
            resultDate = new Date(utc + (3600000 * 7));
            if (getCurrentDate() === date) {
                return $filter("date")(resultDate, "HH:mm") + " Hôm nay";
            } else {
                return $filter("date")(resultDate, "HH:mm ") + " " + $filter("date")(resultDate, "dd/MM/yyyy");
            }
        } else {

            date = $filter("date")(new Date(temp), "dd/MM/yyyy");

            if (getCurrentDate() === date) {
                return "Hôm nay";
            } else {
                var utc = date.getTime() + (date.getTimezoneOffset() * 60000);

                // create new Date object for different city
                // using supplied offset
                resultDate = new Date(utc + (3600000 * 7));
                return $filter("date")(resultDate, "dd/MM/yyyy");
            }
        }
    };
});

app.filter('dateFormat', function ($filter) {
    return function (input, format) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var resultDate = new Date(+temp);
        return $filter("date")(resultDate, format);
    };
});

app.filter('sumTotal', function () {
    return function (input, property) {
        var i = input instanceof Array ? input.length : 0;
        if (typeof property === 'undefined' || i === 0) {
            return i;
        } else if (isNaN(input[0][property])) {
            throw 'filter total can count only numeric values';
        } else {
            var total = 0;
            while (i--)
                total += parseFloat(input[i][property]);
            return total;
        }
    };
});

app.filter('totalSumPriceQty', function () {
    return function (data, key1, key2) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum = sum + (parseFloat(value[key1], 10) * parseFloat(value[key2], 10));
        });
        return sum;
    }
});

app.filter('totalWithOperation', function () {
    return function (data, key1, key2, operation) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2) || angular.isUndefined(operation))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum += parseFloat(eval(value[key1] + operation + value[key2]));
        });
        return sum;
    }
});

app.filter('Contains', function () {
    return function (string, substring) {
        return (string.indexOf(substring) != -1);
    };
});

app.filter('IndexOf', function () {
    return function (string, substring) {
        if (!string)
            return false;
        else
            return (string.indexOf(substring) != -1);
    };
});

app.filter('numberFixedLen', function () {
    return function (n, len) {
        var num = parseInt(n, 10);
        len = parseInt(len, 10);
        if (isNaN(num) || isNaN(len)) {
            return n;
        }
        num = '' + num;
        while (num.length < len) {
            num = '0' + num;
        }
        return num;
    };
});

/*
 * angular-loading-bar
 *
 * intercepts XHR requests and creates a loading bar.
 * Based on the excellent nprogress work by rstacruz (more info in readme)
 *
 * (c) 2013 Wes Cruver
 * License: MIT
 */


(function () {

    'use strict';

    // Alias the loading bar for various backwards compatibilities since the project has matured:
    angular.module('angular-loading-bar', ['cfp.loadingBarInterceptor']);
    angular.module('chieffancypants.loadingBar', ['cfp.loadingBarInterceptor']);


    /**
     * loadingBarInterceptor service
     *
     * Registers itself as an Angular interceptor and listens for XHR requests.
     */
    angular.module('cfp.loadingBarInterceptor', ['cfp.loadingBar'])
        .config(['$httpProvider', function ($httpProvider) {

            var interceptor = ['$q', '$cacheFactory', '$timeout', '$rootScope', 'cfpLoadingBar', function ($q, $cacheFactory, $timeout, $rootScope, cfpLoadingBar) {

                /**
                 * The total number of requests made
                 */
                var reqsTotal = 0;

                /**
                 * The number of requests completed (either successfully or not)
                 */
                var reqsCompleted = 0;

                /**
                 * The amount of time spent fetching before showing the loading bar
                 */
                var latencyThreshold = cfpLoadingBar.latencyThreshold;

                /**
                 * $timeout handle for latencyThreshold
                 */
                var startTimeout;


                /**
                 * calls cfpLoadingBar.complete() which removes the
                 * loading bar from the DOM.
                 */
                function setComplete() {
                    $timeout.cancel(startTimeout);
                    cfpLoadingBar.complete();
                    reqsCompleted = 0;
                    reqsTotal = 0;
                }

                /**
                 * Determine if the response has already been cached
                 * @param  {Object}  config the config option from the request
                 * @return {Boolean} retrns true if cached, otherwise false
                 */
                function isCached(config) {
                    var cache;
                    var defaults = $httpProvider.defaults;

                    if (config.method !== 'GET' || config.cache === false) {
                        config.cached = false;
                        return false;
                    }

                    if (config.cache === true && defaults.cache === undefined) {
                        cache = $cacheFactory.get('$http');
                    } else if (defaults.cache !== undefined) {
                        cache = defaults.cache;
                    } else {
                        cache = config.cache;
                    }

                    var cached = cache !== undefined ?
                        cache.get(config.url) !== undefined : false;

                    if (config.cached !== undefined && cached !== config.cached) {
                        return config.cached;
                    }
                    config.cached = cached;
                    return cached;
                }


                return {
                    'request': function (config) {
                        // Check to make sure this request hasn't already been cached and that
                        // the requester didn't explicitly ask us to ignore this request:
                        if (!config.ignoreLoadingBar && !isCached(config)) {
                            $rootScope.$broadcast('cfpLoadingBar:loading', { url: config.url });
                            if (reqsTotal === 0) {
                                startTimeout = $timeout(function () {
                                    cfpLoadingBar.start();
                                }, latencyThreshold);
                            }
                            reqsTotal++;
                            cfpLoadingBar.set(reqsCompleted / reqsTotal);
                        }
                        return config;
                    },

                    'response': function (response) {
                        if (!response.config.ignoreLoadingBar && !isCached(response.config)) {
                            reqsCompleted++;
                            $rootScope.$broadcast('cfpLoadingBar:loaded', { url: response.config.url });
                            if (reqsCompleted >= reqsTotal) {
                                setComplete();
                            } else {
                                cfpLoadingBar.set(reqsCompleted / reqsTotal);
                            }
                        }
                        return response;
                    },

                    'responseError': function (rejection) {
                        if (!rejection.config.ignoreLoadingBar && !isCached(rejection.config)) {
                            reqsCompleted++;
                            $rootScope.$broadcast('cfpLoadingBar:loaded', { url: rejection.config.url });
                            if (reqsCompleted >= reqsTotal) {
                                setComplete();
                            } else {
                                cfpLoadingBar.set(reqsCompleted / reqsTotal);
                            }
                        }
                        return $q.reject(rejection);
                    }
                };
            }];

            $httpProvider.interceptors.push(interceptor);
        }]);


    /**
     * Loading Bar
     *
     * This service handles adding and removing the actual element in the DOM.
     * Generally, best practices for DOM manipulation is to take place in a
     * directive, but because the element itself is injected in the DOM only upon
     * XHR requests, and it's likely needed on every view, the best option is to
     * use a service.
     */
    angular.module('cfp.loadingBar', [])
        .provider('cfpLoadingBar', function () {

            this.includeSpinner = true;
            this.includeBar = true;
            this.latencyThreshold = 100;
            this.startSize = 0.02;
            this.parentSelector = 'body';
            this.spinnerTemplate = '<div id="loading-bar-spinner"><div class="spinner-icon"></div></div>';

            this.$get = ['$document', '$timeout', '$animate', '$rootScope', function ($document, $timeout, $animate, $rootScope) {

                var $parentSelector = this.parentSelector,
                    loadingBarContainer = angular.element('<div id="loading-bar"><div class="bar"><div class="peg"></div></div></div>'),
                    loadingBar = loadingBarContainer.find('div').eq(0),
                    spinner = angular.element(this.spinnerTemplate);

                var incTimeout,
                    completeTimeout,
                    started = false,
                    status = 0;

                var includeSpinner = this.includeSpinner;
                var includeBar = this.includeBar;
                var startSize = this.startSize;

                /**
                 * Inserts the loading bar element into the dom, and sets it to 2%
                 */
                function _start() {
                    var $parent = $document.find($parentSelector);
                    $timeout.cancel(completeTimeout);

                    // do not continually broadcast the started event:
                    if (started) {
                        return;
                    }

                    $rootScope.$broadcast('cfpLoadingBar:started');
                    started = true;

                    if (includeBar) {
                        $animate.enter(loadingBarContainer, $parent);
                    }

                    if (includeSpinner) {
                        $animate.enter(spinner, $parent);
                    }

                    _set(startSize);
                }

                /**
                 * Set the loading bar's width to a certain percent.
                 *
                 * @param n any value between 0 and 1
                 */
                function _set(n) {
                    if (!started) {
                        return;
                    }
                    var pct = (n * 100) + '%';
                    loadingBar.css('width', pct);
                    status = n;

                    // increment loadingbar to give the illusion that there is always
                    // progress but make sure to cancel the previous timeouts so we don't
                    // have multiple incs running at the same time.
                    $timeout.cancel(incTimeout);
                    incTimeout = $timeout(function () {
                        _inc();
                    }, 250);
                }

                /**
                 * Increments the loading bar by a random amount
                 * but slows down as it progresses
                 */
                function _inc() {
                    if (_status() >= 1) {
                        return;
                    }

                    var rnd = 0;

                    // TODO: do this mathmatically instead of through conditions

                    var stat = _status();
                    if (stat >= 0 && stat < 0.25) {
                        // Start out between 3 - 6% increments
                        rnd = (Math.random() * (5 - 3 + 1) + 3) / 100;
                    } else if (stat >= 0.25 && stat < 0.65) {
                        // increment between 0 - 3%
                        rnd = (Math.random() * 3) / 100;
                    } else if (stat >= 0.65 && stat < 0.9) {
                        // increment between 0 - 2%
                        rnd = (Math.random() * 2) / 100;
                    } else if (stat >= 0.9 && stat < 0.99) {
                        // finally, increment it .5 %
                        rnd = 0.005;
                    } else {
                        // after 99%, don't increment:
                        rnd = 0;
                    }

                    var pct = _status() + rnd;
                    _set(pct);
                }

                function _status() {
                    return status;
                }

                function _complete() {
                    $rootScope.$broadcast('cfpLoadingBar:completed');
                    _set(1);

                    $timeout.cancel(completeTimeout);

                    // Attempt to aggregate any start/complete calls within 500ms:
                    completeTimeout = $timeout(function () {
                        $animate.leave(loadingBarContainer, function () {
                            status = 0;
                            started = false;
                        });
                        $animate.leave(spinner);
                    }, 500);
                }

                return {
                    start: _start,
                    set: _set,
                    status: _status,
                    inc: _inc,
                    complete: _complete,
                    includeSpinner: this.includeSpinner,
                    latencyThreshold: this.latencyThreshold,
                    parentSelector: this.parentSelector,
                    startSize: this.startSize
                };


            }];     //
        });       // wtf javascript. srsly
})();       //