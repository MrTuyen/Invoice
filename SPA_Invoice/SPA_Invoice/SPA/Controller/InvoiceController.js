app.controller('InvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', '$location', '$window', '$sce', function ($scope, $rootScope, $timeout, CommonFactory, $location, $window, $sce) {
    var url = '/Invoice/';

    angular.element(function () {
        //$scope.LoadDashboard();
        $scope.IsHSM = $window.localStorage.getItem("novaon_kyso_hsm"); // Kiểm tra xem user đăng nhập có sử dụng HSM không? nếu có thì dùng để ẩn/ hiện button ký hàng loạt bằng HSM trên trang chủ QLHD
    });
    $rootScope.ListInvoiceChecked = new Array();
    $scope.Invoice = new Object();
    //========================== Cookie's Own ============================
    $scope.LoadCookie_Invoice = function () {
        var check = getCookie('Novaon_InvoiceManagement');
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldBillDate: true,
                FieldBillType: true,
                FieldCustomer: true,
                FieldFormCode: true,
                FieldSymbolCode: true,
                FieldNumber: true,
                FieldInvoiceStatus: true,
                FieldReferenceCode: true,
                FieldPaymentStatus: false,
                FieldTotalPayment: true,
                FieldCustomerID: true,
                FieldConsignmentID: false,
                FieldExchangeRate: false,
                FieldExchange: false,
                FieldInvoiceNote: false,
                FieldEmailPartner: false,
                RowNum: 10
            }
            setCookie('Novaon_InvoiceManagement', JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie('Novaon_InvoiceManagement', JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        if ($scope.Tab.Wating) {
            $scope.GetInvoiceWait($scope.currentpage);
        } else {
            $scope.GetInvoice($scope.currentpage);
        }
    }
    //==================================== END ================================

    $rootScope.Tab = {
        All: true,
        Cancel: false,
        Tranfer: false,
        Change: false,
        Replace: false,
        ReleaseError: false,
        Delete: false,
        Wating: false,
    };

    $rootScope.InvoiceTabClick = function (xxx) {
        $scope.Tab.All = false;
        $scope.Tab.Cancel = false;
        $scope.Tab.Tranfer = false;
        $scope.Tab.Change = false;
        $scope.Tab.Replace = false;
        $scope.Tab.ReleaseError = false;
        $scope.Tab.Delete = false;
        $scope.Tab.Wating = false;
        $scope.Filter = new Object();
        $scope.ListInvoice = [];
        switch (xxx) {
            case 1:
                $scope.Tab.All = true;
                $scope.Filter = new Object();
                $scope.GetInvoice(1);
                break;
            case 2:
                $scope.Tab.Cancel = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 3:
                $scope.Tab.Tranfer = true;
                $scope.Filter.INVOICETYPE = 4;
                $scope.GetInvoice(1);
                break;
            case 4:
                $scope.Tab.Change = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
                break;
            case 5:
                $scope.Tab.Replace = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 6:
                $scope.Tab.ReleaseError = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceReleaseError(1);
                break;
            case 7:
                $scope.Tab.Delete = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceDelete(1);
                break;
            case 8:
                $scope.Tab.Wating = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceWait(1);
                break;
            default:
                $scope.Tab.All = true;
                break;
        }
        $('.dropdown-menu').removeClass('show');
    }
    $rootScope.ReloadInvoice = function () {
        if ($location.path().toString().includes('/quan-ly-hoa-don')) {

            if ($scope.Tab.Change) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
            } else if ($scope.Tab.Replace) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else if ($scope.Tab.Cancel) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else if ($scope.Tab.Wating) {
                $scope.GetInvoiceWait($scope.currentpage);
            }
            else if ($scope.Tab.Delete) {
                $scope.GetInvoiceDelete($scope.currentpage);
            }
            else if ($scope.Tab.ReleaseError) {
                $scope.GetInvoiceReleaseError($scope.currentpage);
            }
            else {
                $scope.ListInvoice = new Array();
                $scope.GetInvoice($scope.currentpage);
            }
        }
    }
    /**tuyennv - 20200327
    * Declare a function help to call in js file by using $window service
    * Reload invoice index without clearing filter criteria
    */
    $window.ReloadInvoice = function () {
        $rootScope.ReloadInvoice();
    };

    $scope.GetInvoice = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();

        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            if ($scope.FROMTIME > $scope.TOTIME) {
                toastr.warning('Ngày bắt đầu phải nhỏ hơn ngày kết thúc');
                return;
            }
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        if (parseInt($scope.Filter.FROMNUMBER) > parseInt($scope.Filter.TONUMBER)) {
            toastr.warning('Số hóa đơn bắt đầu phải nhỏ hơn số hóa đơn kết thúc');
            return;
        }

        $scope.LoadCookie_Invoice();

        if (!intpage || intpage <= 0) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoice';
        if (!$scope.cookie.RowNum)
            $scope.cookie.RowNum = 10;
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;

        successCallback = function (response) {
            if ($scope.cookie.RowNum >= 500) {
                let data = response.result;
                let firstScreenData = data.splice(0, 50); // Sử dụng splice
                $scope.ListInvoice = firstScreenData;
                $scope.$apply();

                setTimeout(() => {
                    function render(n) {
                        // Mỗi lần hiện thị 50 items
                        let partialData = data.splice(0, 50);
                        $scope.ListInvoice = $scope.ListInvoice.concat(partialData);
                        $scope.$apply();
                        if (n) {
                            setTimeout(() => {
                                render(n - 1);
                            }, 25)
                        }
                    }
                    render(20);// chạy 20 loop mới mỗi loop là 50 items
                }, 450);
            }
            else {
                $timeout(function () {
                    $scope.ListInvoice = response.result;
                }, $scope.cookie.RowNum * 0.5)
            }

            $scope.TotalPages = response.TotalPages;
            $scope.TotalRow = response.TotalRow;
            $scope.FROMTIME = '';
            $scope.TOTIME = '';
        }

        AjaxRequest(action, {
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        }, "POST", true, "json", successCallback);

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUSNAME;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].value;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.ID.toString() === $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    //$scope.GetInvoice = function (intpage) {
    //    if (!$scope.Filter)
    //        $scope.Filter = new Object();

    //    if ($scope.Filter.TIME == '5') {
    //        $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
    //        $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
    //        if ($scope.FROMTIME > $scope.TOTIME) {
    //            toastr.warning('Ngày bắt đầu phải nhỏ hơn ngày kết thúc');
    //            return;
    //        }
    //        $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
    //        if (!$scope.FROMTIME || !$scope.TOTIME) {
    //            $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
    //        }
    //    }
    //    if (parseInt($scope.Filter.FROMNUMBER) > parseInt($scope.Filter.TONUMBER)) {
    //        toastr.warning('Số hóa đơn bắt đầu phải nhỏ hơn số hóa đơn kết thúc');
    //        return;
    //    }
    //    $scope.LoadCookie_Invoice();
    //    if (!intpage || intpage <= 0) {
    //        intpage = 1;
    //    }
    //    if (intpage > $scope.TotalPages) {
    //        intpage = $scope.TotalPages;
    //    }
    //    $scope.currentpage = intpage;
    //    var action = url + 'GetInvoice';
    //    if (!$scope.cookie.RowNum)
    //        $scope.cookie.RowNum = 10;
    //    $scope.ListInvoice = new Array();
    //    $scope.TotalPages = 1;
    //    $scope.TotalRow = 1;

    //    successCallback = function (response) {
    //        $scope.ResultLength = response.result.length;
    //        if ($scope.cookie.RowNum >= 500) {
    //            $scope.ListInvoice = splitArr(response.result, 0);
    //            $timeout(function () {
    //                if ($scope.ListInvoice.length != $scope.ResultLength) {
    //                    let isBigData = true;
    //                    let temp_limit = 50;
    //                    let temp_page = $scope.cookie.RowNum / temp_limit;
    //                    let page = 1;
    //                    while (page <= temp_page) {
    //                        let result = splitArr(response.result, page);
    //                        if (isBigData) {
    //                            $scope.ListInvoice = $scope.ListInvoice.concat(result);
    //                            page = page + 1;
    //                        }
    //                        $scope.$apply();
    //                    }
    //                }
    //            }, 0)
    //        }
    //        else {
    //            $timeout(function () {
    //                $scope.ListInvoice = response.result;
    //            }, $scope.cookie.RowNum * 0.5)
    //        }

    //        $scope.TotalPages = response.TotalPages;
    //        $scope.TotalRow = response.TotalRow;
    //        $scope.FROMTIME = '';
    //        $scope.TOTIME = '';
    //    }

    //    AjaxRequest(action, {
    //        form: $scope.Filter,
    //        currentPage: $scope.currentpage,
    //        itemPerPage: $scope.cookie.RowNum
    //    }, "POST", true, "json", successCallback);

    //    $scope.FilterApply = [];
    //    for (var prop in $scope.Filter) {
    //        if ($scope.Filter[prop] != null) {
    //            var o = {
    //                key: prop,
    //                value: $scope.Filter[prop]
    //            };

    //            if (prop == 'INVOICESTATUS') {
    //                var f = $scope.ListInvoiceStatus.filter(function (item) {
    //                    return item.INVOICESTATUSID == $scope.Filter[prop];
    //                });
    //                o.value = f[0].INVOICESTATUSNAME;
    //            }

    //            if (prop == 'TIME') {
    //                var f = $scope.Timepickers.Options.filter(function (item) {
    //                    return item.value == $scope.Filter[prop];
    //                });
    //                if (f.length > 0) {
    //                    o.value = f[0].value;
    //                }
    //            }

    //            if (prop == 'FORMCODE') {
    //                var f = $scope.ListFormCode.filter(function (item) {
    //                    return item.FORMCODE == $scope.Filter[prop];
    //                });
    //                o.value = f[0].FORMCODE;
    //            }

    //            if (prop == 'PAYMENTSTATUS') {
    //                var f = $scope.ListPaymentStatus.filter(function (item) {
    //                    return item.ID.toString() === $scope.Filter[prop];
    //                });
    //                o.value = f[0].PAYMENTSTATUS;
    //            }

    //            if (prop == 'SYMBOLCODE') {
    //                var f = $scope.ListSymbolCode.filter(function (item) {
    //                    return item.SYMBOLCODE == $scope.Filter[prop];
    //                });
    //                o.value = f[0].SYMBOLCODE;
    //            }

    //            $scope.FilterApply.push(o);
    //        }
    //    }
    //}

    $scope.GetInvoiceWait = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            //if (!$scope.FROMTIME == 'undefined' || $scope.TOTIME == 'undefined') {
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage || intpage <= 0) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceWating';
        if (!$scope.cookie.RowNum)
            $scope.cookie.RowNum = 10;

        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;

        //successCallback = function (response) {
        //    $timeout(function () {
        //        $scope.ListInvoice = response.result;
        //        $scope.TotalPages = response.TotalPages;
        //        $scope.TotalRow = response.TotalRow;
        //        $scope.currentpage = 1;
        //        $scope.FROMTIME = '';
        //        $scope.TOTIME = '';
        //    }, 1000);
        //}

        //successCallback = function (response) {
        //    $scope.ResultLength = response.result.length;
        //    if ($scope.cookie.RowNum >= 500) {
        //        $scope.ListInvoice = splitArr(response.result, 0);
        //        $timeout(function () {
        //            if ($scope.ListInvoice.length != $scope.ResultLength) {
        //                let isBigData = true;
        //                let temp_limit = 50;
        //                let temp_page = $scope.cookie.RowNum / temp_limit;
        //                let page = 1;
        //                while (page <= temp_page) {
        //                    let result = splitArr(response.result, page);
        //                    if (isBigData) {
        //                        $scope.ListInvoice = $scope.ListInvoice.concat(result);
        //                        page = page + 1;
        //                    }
        //                    $scope.$apply();
        //                }
        //            }
        //        }, 0)
        //    }
        //    else {
        //        $timeout(function () {
        //            $scope.ListInvoice = response.result;
        //        }, $scope.cookie.RowNum * 0.5)
        //    }

        //    $scope.TotalPages = response.TotalPages;
        //    $scope.TotalRow = response.TotalRow;
        //    $scope.FROMTIME = '';
        //    $scope.TOTIME = '';
        //}

        successCallback = function (response) {
            if ($scope.cookie.RowNum >= 500) {
                let data = response.result;
                let firstScreenData = data.splice(0, 50); // Sử dụng splice
                $scope.ListInvoice = firstScreenData;
                $scope.$apply();

                setTimeout(() => {
                    function render(n) {
                        // Mỗi lần hiện thị 50 items
                        let partialData = data.splice(0, 50);
                        $scope.ListInvoice = $scope.ListInvoice.concat(partialData);
                        $scope.$apply();
                        if (n) {
                            setTimeout(() => {
                                render(n - 1);
                            }, 25)
                        }
                    }
                    render(20);// chạy 20 loop mới mỗi loop là 50 items
                }, 450);
            }
            else {
                $timeout(function () {
                    $scope.ListInvoice = response.result;
                }, $scope.cookie.RowNum * 0.5)
            }

            $scope.TotalPages = response.TotalPages;
            $scope.TotalRow = response.TotalRow;
            $scope.FROMTIME = '';
            $scope.TOTIME = '';
        }

        AjaxRequest(action, {
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        }, "POST", true, "json", successCallback);

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].value;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.ID.toString() === $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceReleaseError = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceReleaseError';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                    $scope.FROMTIME = '';
                    $scope.TOTIME = '';

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].TIME;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.PAYMENTSTATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceDelete = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceDelete';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum === null ? 100 : $scope.cookie.RowNum
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                    $scope.FROMTIME = '';
                    $scope.TOTIME = '';

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].TIME;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.PAYMENTSTATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        $rootScope.ReloadInvoice();
    };

    $scope.SeleteRow = function (list, item, isselect, invoiceId) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (item) {
            const index = $scope.ListInvoiceChecked.indexOf(invoiceId);
            if (index > -1) {
                $scope.ListInvoiceChecked.splice(index, 1);
            }
            isselect = false;
            list.forEach(function (item) {
                if (item.ID === invoiceId)
                    item.ISSELECTED = isselect;
            });
        }
        else {
            $scope.ListInvoiceChecked.push(invoiceId);
            if (find.length == list.length - 1) {
                isselect = true;
            }
            else {
                list.forEach(function (item) {
                    if (item.ID === invoiceId)
                        item.ISSELECTED = true;
                });
            }
        }

        $scope.SetCountInvoiceSelected();
    }

    $scope.SelectAllInvoice = function (list, isselect) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == isselect;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !isselect;
            });
        }
        $scope.SetCountInvoiceSelected();
    }

    $scope.SetCountInvoiceSelected = function () {
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length > 0) {
            $('#invoice-seleced-sign').show();
            $('#invoice-seleced-sign').html('(' + listInvoiceChecked.length + ' HĐ đã chọn)');
        }
        else {
            $('#invoice-seleced-sign').html('');
            $('#invoice-seleced-sign').hide();
        }

        if (listInvoiceChecked && listInvoiceChecked.length > 0) {
            $('#invoice-seleced-sign-waiting').show();
            $('#invoice-seleced-sign-waiting').html('(' + listInvoiceChecked.length + ' HĐ đã chọn)');
        }
        else {
            $('#invoice-seleced-sign-waiting').html('');
            $('#invoice-seleced-sign-waiting').hide();
        }
    }

    $scope.Sign = function (idInvoice) {
        LoadingShow();
        Sign(idInvoice);
    }

    $scope.SignMultipleInvoice = function (isSignAll, isUSBToken) {
        $scope.ListInvoiceChecked = [];
        if (!isSignAll) {
            var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS == 2 && obj.ISSELECTED === true; });
            if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                toastr.warning("Hóa đơn với mẫu số <b style='color: #222;'>" + listInvoiceSigned[0].FORMCODE + "/" + listInvoiceSigned[0].SYMBOLCODE + "</b> đã được phát hành.");
                return;
            }
            var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
            if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                toastr.warning("Bạn chưa chọn hóa đơn phát hành.");
                return;
            }
            if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                for (var i = 0; i < listInvoiceChecked.length; i++) {
                    $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                }
                var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
                LoadingShow();
                if (isUSBToken) {
                    SignMultiple(lstInvoiceid, true);
                }
                else {
                    SignMultiple(lstInvoiceid, false);
                }
            }
        }
        else {
            LoadingShow();
            SignMultiple([].join(";"));
        }
    }

    //$scope.BackgroundJobSignInvoice = function () {
    //    var invoiceHub = $.connection.signlRConf;
    //    var userid = sessionStorage.getItem("userNameSS");

    //    $.connection.hub.qs = { 'username': userid };
    //    var invRow = 0;
    //    invoiceHub.client.newMessageReceivedSignInvoice = function (message) {
    //        LoadingHide();
    //        invRow = invRow + 1;
    //        console.log(invRow);
    //        if (invRow === message.totalRow) {
    //            LoadingHide();
    //            // 
    //            var messageSuccess = 'Phát hành thành công <b>' + invRow + '/' + message.totalRow + '</b> hóa đơn.';
    //            $('.invoiceResult').html("Hoàn Thành");
    //            $("#progressTab").hide();
    //            toastr.success(messageSuccess);
    //            invRow = 0;
    //            var confirmContinue = function (result) {
    //                if (result) {
    //                    // Trigger cập nhật số hiện tại và gửi email
    //                    $scope.UpdateAfterSigning();
    //                    return false;
    //                }
    //                else {
    //                    // Trigger cập nhật số hiện tại và gửi email
    //                    $scope.UpdateAfterSigning();
    //                    $rootScope.ReloadInvoice();
    //                }
    //            };
    //            confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Phát hành hóa đơn', 'Có', 'Không', confirmContinue);
    //        }
    //        else {
    //            LoadingShow();
    //            $("#progressTab").show();
    //            var percent = Math.floor((invRow / message.totalRow) * 100);
    //            $('.invoiceResult').html('Đang phát hành hóa đơn: ' + percent + '%');
    //        }
    //    };

    //    $.connection.hub.start().done(function () {
    //        $("#progressTab").hide();
    //    })
    //};

    $scope.BackgroundJobSignInvoice = function () {
        var invoiceHub = $.connection.signlRConf;
        var userid = sessionStorage.getItem("userNameSS");

        $.connection.hub.qs = { 'username': userid };
        var invRow = 0;
        invoiceHub.client.newMessageReceivedSignInvoice = function (message) {
            invRow = invRow + 1;
            console.log(invRow);
            if (message.currentRow === message.totalRow) {
                // 
                var messageSuccess = 'Phát hành thành công <b>' + message.currentRow + '/' + message.totalRow + '</b> hóa đơn.';
                $('.invoiceResult').html("Hoàn Thành");
                $("#progressTab").hide();
                toastr.success(messageSuccess);
                invRow = 0;
                var confirmContinue = function (result) {
                    if (result) {
                        // Trigger cập nhật số hiện tại và gửi email
                        $scope.UpdateAfterSigning();
                        return false;
                    }
                    else {
                        // Trigger cập nhật số hiện tại và gửi email
                        $scope.UpdateAfterSigning();
                        $rootScope.ReloadInvoice();
                    }
                };
                confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Phát hành hóa đơn', 'Có', 'Không', confirmContinue);
            }
            else {
                $("#progressTab").show();
                var percent = Math.floor((message.currentRow / message.totalRow) * 100);
                $('.invoiceResult').html('Đang phát hành hóa đơn: ' + percent + '%');
            }
        };

        $.connection.hub.start().done(function () {
            $("#progressTab").hide();
        })
    };

    $scope.BackgroundJobSignInvoice();

    $scope.UpdateAfterSigning = function () {
        var action = '/Home/UpdateAfterSigning';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    console.log(response.msg);
                } else {
                    console.log(response.msg);
                }
            } else {
                console.log(response.msg);
            }
        });
    }

    $scope.MergingPdfFile = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần in.");
            return;
        }
        var listInvoiceUnSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS == 1 && obj.ISSELECTED === true; });
        if (listInvoiceUnSigned && listInvoiceUnSigned.length > 0) {
            toastr.warning("Hệ thống không hỗ trợ in hàng loạt với hóa đơn chưa phát hành.");
            return;
        }
        if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
            for (var i = 0; i < listInvoiceChecked.length; i++) {
                $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
            }
            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'MergingPdfFile';
            successCallback = function (data) {
                toastr.success(data.msg);
                window.open('Uploads/' + data.msg + "?v=" + new Date().getTime());
            }
            AjaxRequest(action, { idInvoices: lstInvoiceid }, "POST", true, "json", successCallback);
        }
    }

    $scope.ReMergingPdfFile = function () {
        $scope.ListInvoiceChecked = [];
        for (var i = 0; i < $scope.ListInvoice.length; i++) {
            $scope.ListInvoiceChecked.push($scope.ListInvoice[i].ID);
        }
        var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
        var action = url + 'ReMergingPdfFile';
        successCallback = function (data) {
            toastr.success(data.msg);
        }

        AjaxRequest(action, { idInvoices: lstInvoiceid }, "POST", true, "json", successCallback);
    }

    $scope.RemoveInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }

                var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                    toastr.warning("Bạn không được xóa hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                    return;
                }

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RemoveInvoice';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.OpenModalUpdatePaymentStatus = function () {
        $('#modal_choose_paymentstatus').dialog({
            width: '350px',
            modal: true,
            resizable: false,
            show: {
                effect: 'drop',
                duration: 300
            },
            hide: {
                effect: 'drop',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal_choose_paymentstatus').show();
            }
        });
    }

    $scope.UpdatePaymentStatus = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                // Kiểm tra xem đã chọn bản ghi cần cập nhật chưa?
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần cập nhật.");
                    return;
                }

                //var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                //if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                //    toastr.warning("Bạn không được cập nhật hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                //    return;
                //}

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'UpdatePaymentStatus';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid,
                paymentStatus: $scope.PAYMENTSTATUS
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
            $('#modal_choose_paymentstatus').dialog("close");
        };
        confirm("Bạn có thực sự muốn cập nhật các hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.OpenModalUpdatePartner = function () {
        $('#modal_update_partner').dialog({
            width: '550px',
            height: 160,
            modal: true,
            resizable: false,
            show: {
                effect: "drop",
                duration: 500
            },
            hide: {
                effect: "drop",
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_update_partner').show();
            },
            open: function (event, ui) {
                $('.ui-dialog-content').css('overflow', 'visible'); //this line does the actual hiding
            }
        });
    }

    $scope.SuggestUser = function () {
        var obj = $rootScope.SelectedUser;
        if (obj) {
            $scope.User.CUSID = obj.CUSID;
        }
    }

    $scope.UpdatePartner = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                // Kiểm tra xem đã chọn bản ghi cần cập nhật chưa?
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần cập nhật.");
                    return;
                }

                //var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                //if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                //    toastr.warning("Bạn không được cập nhật hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                //    return;
                //}

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'UpdatePartner';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid,
                partnerEmail: $scope.User.EMAIL
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
            $('#modal_update_partner').dialog("close");
        };
        confirm("Bạn có thực sự muốn cập nhật các hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.DownloadInvoiceExcel = function (item) {
        $scope.ListInvoiceChecked = [];
        if (item) {
            $scope.ListInvoiceChecked.push(item);
        }
        else {
            var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
            if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                toastr.warning("Bạn chưa chọn hóa đơn cần export.");
                return;
            }

            if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                for (var i = 0; i < listInvoiceChecked.length; i++) {
                    $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                }
            }
        }

        var lstInvoiceid = $scope.ListInvoiceChecked.join(";");

        var action = url + 'DownloadInvoiceExcel';
        var datasend = {
            idInvoices: lstInvoiceid
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.RecoverInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần phục hồi.");
                    return;
                }
                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RecoverInvoice';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn phục hồi Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.LoadDashboard = function () {
        var action = '/Invoice/' + 'LoadDashboard';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    //$scope.TotalMoneyNotPay = response.TotalMoneyNotPay;
                    $scope.TotalMoneyPaied = response.TotalMoneyPaied;
                    $scope.TotalMoneyNotApproval = response.TotalMoneyNotApproval;
                    $scope.TotalInvoiceSigned = response.totalInvoiceSigned;

                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetProvince');
            }
        });
    };

    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $scope.PreviewReferenceInvoice = function (item) {
        window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
    }

    $scope.PreviewInvoice = function (item, isSignLink) {
        //Nếu chưa có file thì tạo file
        if (isSignLink === false) {
            var action = url + 'CreateFilePdfToView';
            var datasend = JSON.stringify({
                invoiceId: item
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    window.open('NOVAON_FOLDER' + response.msg + "?v=" + new Date().getTime());
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoice');
                }
                LoadingHide();
            });
        }
        else { // ngược lại view file ra
            window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
        }
    }

    $rootScope.InvocieTYPE = 2;
    //TuyenNV - Taỉ hóa đơn trực tiếp
    $scope.DownloadInvoice = function (item) {
        var action = '';
        var datasend = '';
        if ($rootScope.InvocieTYPE == 1) {
            action = url + 'InvoiceConvert';
            datasend = JSON.stringify({
                invoice: item
            });
        } else {
            action = url + 'CreateFilePdfToView';
            datasend = JSON.stringify({
                invoiceID: item.ID
            });
        }
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                var link = document.createElement('a');
                link.href = 'NOVAON_FOLDER' + response.msg + "?v=" + new Date().getTime();
                link.download = 'ONFINANCE_INVOICE_HOA_DON_' + item.COMTAXCODE + '_' + item.FORMCODE + '_' + item.SYMBOLCODE + '_' + item.NUMBER + '.pdf';
                link.dispatchEvent(new MouseEvent('click'));
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DownloadInvoice');
            }
            LoadingHide();
        });
    }


    $scope.PreviewModifiedReport = function (item) {
        window.open('NOVAON_FOLDER' + item.MODIFIEDLINK + "?v=" + new Date().getTime());
    }

    $scope.PreviewCancelReport = function (item) {
        window.open('NOVAON_FOLDER' + item.CANCELLINK + "?v=" + new Date().getTime());
    }

    $scope.ConvertInv = function (item) {
        var result = confirm('Sau khi chuyển sang hóa đơn chuyển đổi. Hóa đơn chỉ được phép in một lần duy nhất. Bạn có thực sự muốn chuyển đổi hóa đơn ?');
        if (result) {
            $scope.IsLoading = true;
            var action = url + 'UpdateConvertInvoice';
            var datasend = JSON.stringify({
                invoice: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Thành công!')
                        $rootScope.ReloadInvoice();
                    } else {
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js)');
                }
            });
            $scope.IsLoading = false;
        }
    }

    /**
     * truongnv 2020-02-14
     * Gán data filter khi người dùng chọn
     * @param {any} type
     * @param {any} intpage
     */
    $scope.GetInvoiceByStatus = function (type, intpage, reportType) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceByStatus';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            type: type,
            intpage: intpage,
            reportType: reportType
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        /*
         *Gán data filter
         * */
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    o.value = f[0].TIME;
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceWaiting = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceNumerWaiting';
        var datasend = JSON.stringify({
            currentPage: $scope.currentpage,
            itemPerPage: 10
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoiceWaiting = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    $rootScope.ReloadGetNumber = function () {
        $scope.GetNumber($scope.FormNumber.CURRENTPAGE);
    }

    $scope.GetNumber = function (intpage) {
        $scope.FormNumber = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.FormNumber.ITEMPERPAGE = 10;
        $scope.FormNumber.CURRENTPAGE = intpage;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        $scope.ListNumber = new Array();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListNumber = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumber');
            }
            LoadingHide();
        });
    }

    /*
     * Đính kèm file khi gửi mail cho khách hàng
     * @param {any} event
     */
    $scope.uploadFile = function (event) {
        LoadingShow();
        $('#div-file-active').show();
        var files = event.target.files;
        var isPass = CheckFile(files);
        if (isPass) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var reader = new FileReader();
                reader.onloadend = function (event) {
                    $timeout(function () {
                        var base64File = event.target.result.split(',')[1];
                        $('#file-selected').append('<span class="has-tag-attment-file" name = "' + file.name + '" rel= "' + base64File + '" onclick="DeleteFile(this)">' + file.name + '<img src="/Images/close.png"></span>');
                        LoadingHide();
                    }, 100);
                };
                reader.readAsDataURL(file);
            }
        }
        else
            LoadingHide();
    }

    $scope.ConfirmChange = function (item) {
        if (item.IDTEMP > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
            };
            confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
            return false;
        }
        $rootScope.ModalInvoice(item, 2);
        $('.modal-invoice').modal('show');
    }

    $scope.CancelInvoiceConfirm = function (item) {
        if (item.CANCELREASON != null && item.CANCELREASON.trim() != "") {
            $rootScope.ModalCancelInvoice(item);
            $('.modal-cancel-invoice').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
                else {
                    $rootScope.IsUpdateCancelReport = false;
                    $rootScope.ModalCancelReport(null, item);
                    $('.modal-cancel-report').modal('show');
                }
            };
            confirm('Bạn có muốn lập biên bản hủy cho hóa đơn này không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    $scope.ReplaceInvoiceConfirm = function (item) {
        var number = item.NUMBER;
        var formCode = item.FORMCODE;
        var symbolCode = item.SYMBOLCODE;
        if (item.INVOICETYPE == 3) {
            if (item.IDTEMP > 0 && item.IDTEMP > item.ID) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> đã được lập hóa đơn thay thế <strong><' + formCode + ' - ' + symbolCode + ' - ' + 'Chưa cấp số></strong>. Bạn có muốn xem không?', 'Hóa đơn thay thế', 'Không', 'Có', confirmContinue);
            }
            else {
                $rootScope.ModalInvoice(item, 6);
                $('.modal-invoice').modal('show');
            }
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    return false;
                }
                else {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
            };
            confirm('Để lập được hóa đơn thay thế cho hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> bạn cần thực hiện xóa bỏ hóa đơn này trước. Bạn có muốn thực hiện xóa bỏ hóa đơn không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    /* TuyenNV - 20200303
     Kiểm tra xem hóa đơn bị điều chỉnh đã có biên bản/ hóa đơn điểu chỉnh
     + Chưa có biên bản => open modal tạo biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản, có hóa đơn điều chỉnh => view hóa đơn điều chỉnh
     */
    $scope.ModifiedInvoiceConfirm = function (item) {
        if (item.CHANGEREASON != null && item.CHANGEREASON.trim() != "") {
            if (item.IDTEMP > 0) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                return false;
            }
            $rootScope.ModalInvoice(item, 5);
            $('.modal-invoice').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    if (item.IDTEMP > 0) {
                        var confirmContinue = function (result) {
                            if (!result)
                                return false;
                            window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                        };
                        confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                        return false;
                    }
                    $rootScope.ModalInvoice(item, 5);
                    $('.modal-invoice').modal('show');
                }
                else {
                    $rootScope.IsUpdateModifiedReport = false;
                    $rootScope.ModalModifiedReport(null, item);
                    $('.modal-modified-report').modal('show');
                }
            };

            if (item.ISEXISTMODIFIEDREPORT != '1') {
                confirm('Bạn có muốn lập biên bản điều chỉnh cho hóa đơn này không?', 'Hóa đơn điểu chỉnh', 'Không', 'Có', confirmContinue);
            }
            else {
                if (item.IDTEMP > 0) {
                    var confirmContinue = function (result) {
                        if (!result)
                            return false;
                        window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                    };
                    confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                    return false;
                }
                $rootScope.ModalInvoice(item, 5);
                $('.modal-invoice').modal('show');
            }
        }
    }
    // End

    $scope.OpenModalCancelReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTCANCELREPORT == '1') {
            $rootScope.IsUpdateCancelReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTCANCELREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    LoadingHide
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateCancelReport = false;
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
    }

    $scope.OpenModalModifiedReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTMODIFIEDREPORT == '2') {
            $rootScope.IsUpdateModifiedReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTMODIFIEDREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    LoadingHide();
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateModifiedReport = false;
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
    }

    $scope.DownloadReleaseDocument = function (item) {
        var action = url + 'DownloadReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadReleaseDocumentXML = function (item) {
        var action = url + 'DownloadReleaseDocumentXML';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadIssuedDocument = function (item) {
        var action = url + 'DownloadIssuedReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadReleaseInvoiceTemplate = function (item) {
        var action = url + 'DownloadReleaseInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.DownloadConvertibleInvoiceTemplate = function (item) {
        var action = url + 'DownloadConvertibleInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.DownloadDiscountInvoiceTemplate = function (item) {
        var action = url + 'DownloadDiscountInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.PreviewInvoiceTemplate = function (item) {
        $scope.GlobalInvoiceToViewTemplate = item;
        var action = url + 'PreviewInvoiceTemplate';
        var datasend = JSON.stringify({
            numberBO: item,
            isConvert: false
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ShowPopupFileView(response.msg);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoiceTemplate');
            }
            LoadingHide();
        });
    }

    $window.PreviewInvoiceTemplate = function (isConvert) {
        var action = url + 'PreviewInvoiceTemplate';
        var datasend = JSON.stringify({
            numberBO: $scope.GlobalInvoiceToViewTemplate,
            isConvert: isConvert
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('#frViewFileInvoice').attr('src', response.msg);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoiceTemplate');
            }
            LoadingHide();
        });
    }

    $scope.ModalSendEmail = function (item) {
        $('#modal_send_email').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            show: {
                effect: 'drop',
                duration: 500
            },
            hide: {
                effect: 'drop',
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_send_email').show();
            }
        });

        $('#file-selected').html('');

        $scope.GetEmailHistory(item.ID);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        if (!item.CUSEMAIL || item.CUSEMAIL.trim() === "") {
            item.CUSEMAIL = item.CUSEMAILSEND;
        }
        $scope.Invoice.RECIEVEREMAIL = item.CUSEMAIL;
    }

    $scope.SendEmail = function () {
        //if (!$scope.Invoice.RECIEVERNAME) {
        //    alert('Vui nhập vào tên người nhận!');
        //    return false;
        //}
        if (!$scope.Invoice.RECIEVEREMAIL) {
            alert('Vui nhập vào email người nhận!!');
            return false;
        }
        var lstEmail = $scope.Invoice.RECIEVEREMAIL.split(',');
        for (var i = 0; i < lstEmail.length; i++) {
            if (!validation.isEmailAddress(lstEmail[i].trim())) {
                alert('Vui lòng nhập đúng định dạng email ' + (i + 1));
                return false;
            }
        }

        $scope.Invoice.CUSBUYER = $scope.Invoice.RECIEVERNAME;
        $scope.Invoice.CUSEMAIL = $scope.Invoice.RECIEVEREMAIL;

        /*
         * Lấy danh sách file đính kèm của người dùng khi gửi email
         */
        var fileNames = new Array();
        var objFileBase64 = new Array();
        var fFile = $('#file-selected .has-tag-attment-file');
        if (fFile.length > 0) {
            for (var i = 0; i < fFile.length; i++) {
                objFileBase64.push(fFile[i].getAttribute('rel'));
                fileNames.push(fFile[i].getAttribute('name'));
            }
        }
        var action = "";
        if ($scope.Invoice.INVOICETYPE === 3) {
            action = url + 'SendCancelEmail';
        }
        else {
            action = url + 'SendEmail';
        }
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            imgBase64: JSON.stringify(objFileBase64),
            fileNames: fileNames.join(";")
        });

        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_email').dialog("close");
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.error(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
    }

    $scope.OpenSendMultipleEmail = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần gửi email.");
            return;
        }

        var listDeletedInvoice = $scope.ListInvoice.filter(function (obj) { return obj.INVOICETYPE !== 3 && obj.ISSELECTED === true; });
        if (listDeletedInvoice && listDeletedInvoice.length > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                listInvoiceChecked = listInvoiceChecked.filter(function (obj) { return obj.INVOICETYPE !== 3 });
                $scope.INVOICEQUANTITIES = listInvoiceChecked.length;
                if (listInvoiceChecked.length > 50) {
                    toastr.error("Số lượng email tối đa không quá 50.", "Cảnh báo")
                    return false;
                }
                $('#modal_send_multiple_email').dialog({
                    width: '1200px',
                    modal: true,
                    resizable: false,
                    show: {
                        effect: 'drop',
                        duration: 500
                    },
                    hide: {
                        effect: 'drop',
                        duration: 500
                    },
                    create: function (event, ui) {
                        $('#modal_send_multiple_email').show();
                    }
                });
                $timeout(function () {
                    listInvoiceChecked.forEach((item) => {
                        if (!item.CUSEMAIL || item.CUSEMAIL.trim() === "") {
                            item.CUSEMAIL = item.CUSEMAILSEND;
                        }
                    })
                    angular.copy(listInvoiceChecked, $scope.ListInvoiceChecked);
                }, 100)
            };
            confirm('Chương trình chỉ thực hiện gửi các hóa đơn đã phát hành và chưa bị xóa bỏ?', 'Lưu ý gửi mail', 'Không', 'Đồng ý', confirmContinue);
        }
    }

    $scope.SendMultipleEmail = function () {
        var action = url + 'SendMultipleEmail';
        var datasend = JSON.stringify({
            invoices: $scope.ListInvoiceChecked,
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_multiple_email').dialog('close');
                } else {
                    toastr.error(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendMultipleEmail');
            }
        });
    }

    $scope.CurrentItem = new Object();

    $scope.EditTemplate = function (item) {
        $rootScope.EditingTemplate = item;
        //var obj = {
        //    C: item.COMTAXCODE,
        //    F: item.FORMCODE,
        //    S: item.SYMBOLCODE,
        //    R: item.FROMNUMBER,
        //    O: item.TONUMBER,
        //    T: item.TEMPLATEPATH
        //};
        //Mã hóa đối tượng và truyền theo phương thức GET
        document.location.href = "/#/mau-hoa-don/" + btoa(item.TEMPLATEPATH);
    };

    /*
     * Đính kèm file biên bản
     * @param {any} event
     */

    $scope.OpenModalUploadFileReport = function (item) {
        $scope.CurrentInvoiceFileReport = item;
        $('#modal_upload_file_report').dialog({
            width: '550px',
            height: 250,
            modal: true,
            resizable: false,
            show: {
                effect: "drop",
                duration: 300
            },
            hide: {
                effect: "drop",
                duration: 300
            },
            create: function (event, ui) {
                $('#modal_upload_file_report').show();
            },
            open: function (event, ui) {
                $('.ui-dialog-content').css('overflow', 'visible'); //this line does the actual hiding
            }
        });
    }

    $('#frmUploadFileReport').fileupload({
        autoUpload: false,
        add: function (e, data) {
            LoadingShow();
            var fileData = new FormData();
            var data = data.files[0];
            fileData.append('file0', data);
            fileData.append('currentItem', $scope.CurrentInvoiceFileReport.ID);
            fileData.append('comTaxCode', $scope.CurrentInvoiceFileReport.COMTAXCODE);

            var action = url + 'UploadFileReport';
            $.ajax({
                type: 'POST',
                url: action,
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    $timeout(function () {
                        if (result.rs) {
                            $scope.CurrentItem.ATTACHMENTFILELINK = result.id;
                            $rootScope.ReloadInvoice();
                            toastr.success(result.msg);
                            $('#modal_upload_file_report').dialog('close');
                        } else {
                            toastr.error(result.msg);
                        }
                        LoadingHide();
                    }, 10);
                },
                error: function (xhr, status, p3, p4) {
                    alert('Lỗi không thể tải lên file: ' + data.name);
                    LoadingHide();
                }
            });
        }
    });

    $scope.DownloadFileReport = function (item) {
        var action = url + 'DownloadFileReport';
        var datasend = {
            invoiceId: item.ID
        };

        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.RemoveFileReport = function (item) {
        var action = url + 'RemoveFileReport';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.error(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - RemoveFileReport');
            }
            LoadingHide();
        });
    }

    $scope.ExportExcel = function () {
        var action = url + 'ExportExcel';
        var datasend = {
            form: $scope.Filter,
        };
        $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        })
    }

    $scope.GetEmailHistory = function (invoiceId) {
        $scope.ListEmailHistory = new Array();
        var action = url + 'GetEmailHistoryByInvoiceId';
        var datasend = JSON.stringify({
            id: invoiceId
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListEmailHistory = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetEmailHistory');
            }
            LoadingHide();
        });
    }
    LoadingHide();

    /*
     * TuyenNV - 20200602
     * Xem nhanh hóa đơn.
    */
    $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
    if ($scope.IsGridViewDisplay === undefined) {
        setCookie("NovaonIsGridView", 0, 30);
        $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
    } else if ($scope.IsGridViewDisplay === "1" && $("#left-mainnavi").css("width") === "230px") {
        $('#sidebar').trigger('click');
    }
    $scope.CloseSideBar = function () {
        if ($scope.IsGridViewDisplay === undefined) {
            setCookie("NovaonIsGridView", 1, 30);
        }
        if ($scope.IsGridViewDisplay !== undefined) {
            if ($scope.IsGridViewDisplay === "1") {
                setCookie("NovaonIsGridView", 0, 30);
            }
            else {
                setCookie("NovaonIsGridView", 1, 30);
            }
        }
        $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
        $('#sidebar').trigger('click');
    }
    //HĐ thường 
    $scope.QuickviewInvoice = function (invoice) {
        $rootScope.InvocieTYPE = 2;
        $scope.CurrentActive = invoice.ID;
        $scope.CurrentInvoice = invoice;
        var action = url + 'QuickviewInvoice';
        var datasend = JSON.stringify({
            invoiceId: invoice.ID
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                //$scope.CurrentInvoice.PDF = $sce.trustAsHtml(response.data);
                $('#iframe_templateViewInvoice').attr("src", response.data);
                //$('#iframe_templateViewInvoice').attr("srcdoc", response.data);
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - QuickviewInvoice');
            }
            LoadingHide();
        });
    }
    //HĐ chuyển đổi
    $scope.QuickviewInvoiceConvert = function (invoice) {
        $rootScope.InvocieTYPE = 1;
        $scope.CurrentActive = invoice.ID;
        $scope.CurrentInvoice = invoice;
        var action = url + 'QuickviewInvoiceConvert';
        var datasend = JSON.stringify({
            invoice: invoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                toastr.options = { "timeOut": "2000" };
                toastr.success("Chuyển đổi thành công!");
                //$scope.CurrentInvoice.PDF = $sce.trustAsHtml(response.data[0]);
                $('#iframe_templateViewInvoice').attr("src", response.data);
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - QuickviewInvoiceConvert');
            }
            LoadingHide();
        });
    }
     // End xem nhanh hóa đơn
    // gửi tin zalo
    $rootScope.NOTE = "";
    $rootScope.SendZalo = function (item) {
        if (!item.CUSPHONENUMBER) {
            toastr.warning('Số điện thoại là bắt buộc');
            return;
        }
        var action = url + 'SendZalo';
        var datasend = JSON.stringify({
            invoice: item ,note :$rootScope.NOTE
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success("Gửi thành công tới zalo có số " + item.CUSPHONENUMBER);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendZalo');
            }
            LoadingHide();
        });
    }

    $scope.ModalSendZalo = function (item) {
        $('#modal_send_zalo').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            show: {
                effect: 'drop',
                duration: 500
            },
            hide: {
                effect: 'drop',
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_send_zalo').show();
            }
        });

        $('#file-selected').html('');

        $rootScope.Invoice = new Object();
        angular.copy(item, $rootScope.Invoice);

        var action = url + 'GetHistoryZalo';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListHistoryZaloUser = response.result;
                } else {
                    toastr.warning(response.msg);
                    $rootScope.ListHistoryZaloUser = null;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetHistoryZalo');
            }
            LoadingHide();
        });
    }
    //format date zalo
    $rootScope.DateFormatZalo = function (dateString) {
        var currentTime = new Date(dateString);
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var hour = currentTime.getHours();
        var minutes = currentTime.getMinutes();
        var seconds = currentTime.getSeconds();
        var date = day + "/" + month + "/" + year + " lúc " + hour + " : " + minutes + " : " + seconds;
        return date;
    }
   
    $rootScope.OpenSendMultipleZalo = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần gửi tin.");
            return;
        }

        var listDeletedInvoice = $scope.ListInvoice.filter(function (obj) { return obj.INVOICETYPE !== 3 && obj.ISSELECTED === true; });
        if (listDeletedInvoice && listDeletedInvoice.length > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                listInvoiceChecked = listInvoiceChecked.filter(function (obj) { return obj.INVOICETYPE !== 3 });
                $rootScope.INVOICEQUANTITIES = listInvoiceChecked.length;
                $('#modal_send_multiple_zalo').dialog({
                    width: '1200px',
                    modal: true,
                    resizable: false,
                    show: {
                        effect: 'drop',
                        duration: 500
                    },
                    hide: {
                        effect: 'drop',
                        duration: 500
                    },
                    create: function (event, ui) {
                        $('#modal_send_multiple_zalo').show();
                    }
                });
                $timeout(function () {
                    listInvoiceChecked.forEach((item) => {
                        if (!item.CUSPHONENUMBER || item.CUSPHONENUMBER.trim() === "") {
                            item.CUSPHONENUMBER = item.CUSPHONENUMBER;
                        }
                    })
                    angular.copy(listInvoiceChecked, $rootScope.ListInvoiceChecked);
                }, 100)
            };
            confirm('Chương trình chỉ thực hiện gửi các hóa đơn đã phát hành và chưa bị xóa bỏ?', 'Lưu ý gửi tin', 'Không', 'Đồng ý', confirmContinue);
        }
    }
    $rootScope.SendMultipleZalo = function () {
        var action = url + 'SendMultipleZalo';
        var datasend = JSON.stringify({
            invoices: $rootScope.ListInvoiceChecked,
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_multiple_zalo').dialog('close');
                } else {
                    toastr.error(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendMultipleZalo');
            }
        });
    }
    //End gửi tin zalo
    $scope.SetdateFromtime = function () {
        moment($("#pk_fromtime").val()).format('yy-mm-dd');
    }
    $scope.SetDatepikerFromTime = function () {
        // All tab
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));

        // Quickview tab
        $("#pk_fromtime_quickview").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_quickview"));
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_quickview"));

        // Waiting tab
        $("#pk_fromtime_wating").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-wating"));
        $("#pk_fromtime_wating").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_wating"));

        //Delete tab
        $("#pk_fromtime_delete").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-delete"));
        $("#pk_fromtime_delete").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_delete"));
        //ReleaseError
        $("#pk_fromtime_error").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-error"));
        $("#pk_fromtime_error").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_error"));
    }

    $scope.SetDatepikerToTime = function () {

        // All tab
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));

        // Quickview tab
        var toTimeQuickview = $scope.TOTIME;
        $("#pk_totime_quickview").datepicker("option", 'minDate', toTimeQuickview);
        $("#pk_totime_quickview").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeQuickview)
        });
        SetVietNameInterface($("#pk_totime_quickview"));
        var toTimeQuickview = $scope.TOTIME;
        $("#pk_totime_quickview").datepicker("option", 'minDate', toTimeQuickview);
        $("#pk_totime_quickview").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeQuickview)
        });
        SetVietNameInterface($("#pk_totime_quickview"));

        // Waiting tab
        var toTimeWating = $scope.TOTIME;
        $("#pk_totime_wating").datepicker("option", 'minDate', toTimeWating);
        $("#pk_totime_wating").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeWating)
        });
        SetVietNameInterface($("#pk_totime_wating"));
        var toTimeWating = $scope.TOTIME;
        $("#pk_totime_wating").datepicker("option", 'minDate', toTimeWating);
        $("#pk_totime_wating").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeWating)
        });
        SetVietNameInterface($("#pk_totime_wating"));

        // Delete tab
        var toTimeDelete = $scope.TOTIME;
        $("#pk_totime_delete").datepicker("option", 'minDate', toTimeDelete);
        $("#pk_totime_delete").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeDelete)
        });
        SetVietNameInterface($("#pk_totime_delete"));
        var toTimeDelete = $scope.TOTIME;
        $("#pk_totime_delete").datepicker("option", 'minDate', toTimeDelete);
        $("#pk_totime_delete").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeDelete)
        });
        SetVietNameInterface($("#pk_totime_error"));
        //ReleaseError tab
        var toTimeError = $scope.TOTIME;
        $("#pk_totime_error").datepicker("option", 'minDate', toTimeError);
        $("#pk_totime_error").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeError)
        });
        SetVietNameInterface($("#pk_totime_error"));
        var toTimeError = $scope.TOTIME;
        $("#pk_totime_error").datepicker("option", 'minDate', toTimeError);
        $("#pk_totime_error").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeError)
        });
        SetVietNameInterface($("#pk_totime_delete"));
    }

    /*
     *truongnv 20200317
     * Sắp sếp dữ liệu
     * */
    // column to sort
    $scope.column = '';

    // sort ordering (Ascending or Descending). Set true for desending
    $scope.reverse = false;

    // called on header click
    $scope.sortColumn = function (col) {
        $scope.column = col;
        if ($scope.reverse) {
            $scope.reverse = false;
            $scope.reverseclass = 'arrow-up_h';
        } else {
            $scope.reverse = true;
            $scope.reverseclass = 'arrow-down_h';
        }
    }

    // remove and change class
    $scope.sortClass = function (col) {
        if ($scope.column == col) {
            if ($scope.reverse) {
                return 'arrow-down_h';
            } else {
                return 'arrow-up_h';
            }
        } else {
            return '';
        }
    }

    //UI/UX
    $timeout(function () {
        $('select.cb-select-time').selectpicker();
        $('.selectpicker').selectpicker();
    });

    $('[data-toggle="tooltip"]').tooltip({
        content: function () {
            return $(this).prop('title');
        },
        position: {
            my: "center top+15", at: "left bottom"
        }
    });

    //xóa hẳn HĐ
    $scope.RemovedInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }
                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RemovedInvoice';
            var datasend = JSON.stringify({
                ids: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các hóa đơn đã chọn không ?<br/><b class ='text-danger'>Chọn xóa sẽ không thể khôi phục lại được, nên cân nhắc trước khi xóa</b>", "Thông báo", "Không", "Có", confirmContinue)
    }

    //xóa hẳn tất cả HĐ trong tab xóa bỏ
    $scope.RemovedAllInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            var action = url + 'RemovedAllInvoice';
            var datasend = JSON.stringify({
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa tất cả các hóa đơn không?<br/><b class ='text-danger'>Chọn xóa sẽ không thể khôi phục lại được, nên cân nhắc trước khi xóa</b>", "Thông báo", "Không", "Có", confirmContinue)
    }

    //HĐ chuyển đổi
    $scope.ModalConvertInvoice = function (item) {
        $('#modal_convert_invoice').dialog({
            width: '20%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            show: {
                effect: 'drop',
                direction: 'right',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal_convert_invoice').show();
            }
        });
        angular.copy(item, $scope.Invoice);
    }
    $scope.ConvertInvoices = function () {
        var action = url + "InvoiceConvert";
        var datasend = JSON.stringify({
            invoice: $scope.Invoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Chuyển đổi thành công!");
                    $('#modal_convert_invoice').dialog("close");
                    $scope.PreviewReferenceInvoice(response.data);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - InvoiceConvert');
            }
            LoadingHide();
        });
    }
    var n = 1;
    $window.GetInvoice = function () {
        if ($scope.cookie.RowNum >= $scope.TotalRow) {
            
        } else {
            $scope.cookie.RowNum = $scope.cookie.RowNum + 10;
            if ($scope.Tab.All === true) {
                $scope.Check($scope.cookie.RowNum, 'RowNum');
            }
            if ($scope.Tab.Wating === true) {
                $scope.Check($scope.cookie.RowNum, 'RowNum');
            }
            if ($scope.Tab.Cancel === true) {
                $scope.GetInvoiceByStatus(3, 1 + n, 1);
            }
            if ($scope.Tab.Change === true) {
                $scope.GetInvoiceByStatus(5, 1 + n, 2);
            }
            if ($scope.Tab.Delete === true) {
                $scope.GetInvoiceDelete(1 + n);
            }
            if ($scope.Tab.Replace === true) {
                $scope.GetInvoiceByStatus(6, 1 + n, 4);
            }
        }
    }
    $scope.ChangeFontSize = function () {
        $scope.template.style["body, table",".bg"]["font-size"] = $scope.template.fontSize;
    }

    $scope.ShowPopupFileView = function (link) {
        $('#popupViewInvoice').dialog({
            modal: true,
            resizable: true,
            width: '900px',

            open: function (data, e, f) {
                $('#frViewFileInvoice').attr('src', link);
                $(this).dialog("option", "title", "");
                //thêm nút thực hiện chức năng chuyển đổi hóa đơn điện tử sang hóa đơn giấy
                $(".function-area").remove();
                var rdInvoice = $('<label class="function-area"><input id = "btnInvoice" checked type="radio" onchange="window.PreviewInvoiceTemplate(' + (false) +')" name="template_check" style="transform: scale(1.7)" /> &nbsp;&nbsp; Hóa đơn mẫu<label/>');
                rdInvoice.appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));
                var rdConvertInvoice = $('<label class="function-area"><input id = "btnConvertInvoice" name="template_check" type="radio" onchange="window.PreviewInvoiceTemplate(' + (true) +')" style="transform: scale(1.7)" />&nbsp;&nbsp; Hóa đơn mẫu chuyển đổi<label/>');
                rdConvertInvoice.appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));
            }
        });
    }

}]);
//bắt sự kiện Tự động load trang
var x = 200;
    function LoadPage() {
    var elmnt = document.getElementById("all_invoice");
    var y = elmnt.scrollTop;
    var h = $("#all_invoice").height();
    var h1 = $(document).height();
        var h2 = $("#nofify-footer-bar").height();
        if (y + h >= h1 - h2 + x) {
            x += 200;
            window.GetInvoice();
        }
}
/**
 * Xóa đính kèm đã chọn
 * truongnv 20200219
 * @param {any} index : key
 */
function DeleteFile(index) {
    index.remove();
}

/**
 * Kiểm tra kiểu file và dung lượng file tải lên
 * truongnv 20200219
 * @param {any} files
 */
function CheckFile(files) {
    var arrFiles = new Array();
    var allowFileExtension = ".xls,.xlsx,.doc,.docx,.pdf, .txt, .xml";
    var arrAllowFileExtension = allowFileExtension.split(",");
    if (files.length > 0) {
        var sumSize = 0;
        var sumSizeAllow = 10 * 1024 * 1024;
        for (var i = 0; i < files.length; i++) {
            var fileName = files[i].name;
            var fileExtension = fileName.substr(fileName.lastIndexOf('.'));
            var isAllowFileExtension = false;
            for (var j = 0; j < arrAllowFileExtension.length; j++) {
                var fileExtensionAllow = arrAllowFileExtension[j];
                if (fileExtension.toLowerCase() == fileExtensionAllow) {
                    isAllowFileExtension = true;
                    break;
                }
            }

            if (!isAllowFileExtension) {
                alert("Bạn không được chọn file định dạng " + fileExtension);
                return false;
            }

            var fileSize = files[i].size;
            if (fileSize > sumSizeAllow) {
                alert("File <b>" + files[i].name + "</b> có dung lượng <b>" + GetSizeName(fileSize) + "</b>. Bạn không được chọn file có dung lượng lớn hơn <b>10 MB</b>.");
                return false;
            }

            sumSize = sumSize + fileSize;
        }

        for (var i = 0; i < arrFiles.length; i++)
            sumSize = sumSize + arrFiles[i].size;

        if (sumSize > sumSizeAllow) {
            alert("Tổng dung lượng các file bạn đã chọn là <b>" + GetSizeName(sumSize) + "</b>. Bạn không được Attach file có tổng dung lượng lớn hơn <b>10 MB</b>.");
            return false;
        }

        for (var i = 0; i < files.length; i++)
            arrFiles.push(files[i]);

        return true;
    }

    /**
     * Lấy độ lớn của file
     * @param {any} size
     */
    function GetSizeName(size) {
        if (size < 1024 * 1024)
            return Math.round(size / 1024) + " KB";
        else
            return Math.round(size / (1024 * 1024)) + " MB";
    }
}

/**
 * truongnv 20200404
 * @param {any} url
 * @param {any} parameters
 * @param {any} type 
 * @param {any} async 
 * @param {any} dataType: 
 * @param {any} successCallback
 * @param {any} errorCallback
 */
function AjaxRequest(url, parameters, type, async, dataType, successCallback, errorCallback) {
    //LoadingShow();
    $.ajax({
        url: url == undefined ? "" : url,
        data: parameters == undefined ? "" : JSON.stringify(parameters),
        type: type == undefined ? 'POST' : type,
        async: async == undefined ? true : async,
        contentType: 'application/json; charset=utf-8',
        dataType: (dataType == undefined ? 'json' : dataType),
        success: function (data) {
            if (data.hasOwnProperty("rs") && data.hasOwnProperty("msg")) { // trả về object Result
                if (!data.rs) {
                    if (errorCallback !== undefined && typeof errorCallback === "function") {
                        errorCallback(data);
                        return;
                    }
                    alert(data.msg);
                }
                else if (successCallback !== undefined && typeof successCallback === "function") successCallback(data);
            } else
                if (successCallback !== undefined && typeof successCallback === "function") successCallback(data);

            //LoadingHide();
        },
        error: function (xhr, textStatus, errorThrown) {
            //LoadingHide();
            if (errorCallback !== undefined && typeof errorCallback === "function") errorCallback();

            if (xhr.status == 403 || xhr.status == 500)//Hết session khi gọi ws
            {
                alert('Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập.');
                return;
            }
            console.log(xhr);
            var message = xhr.responseText != undefined ? xhr.responseText : textStatus;
            alert('Có lỗi xảy ra. Xin vui lòng thử lại sau hoặc thông báo với quản trị (Error: ' + message + ').');
        }
    });
}

/**
 * */
function splitArr(arr, i) {
    return arr.slice(i * 50, (i + 1) * 50);
}

//xem thêm,thu gọn 
function See_more_invoice() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_invoice() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}
function See_more_invoice_wating() {
    $('#id3').css('display', 'none');
    $('#id4').css('display', 'block');
}
function Collapse_invoice_wating() {
    $('#id4').css('display', 'none');
    $('#id3').css('display', 'block');
}
function ChangeSizeIframe() {
    frames.$('body').style.fontSize = '11px';
}

