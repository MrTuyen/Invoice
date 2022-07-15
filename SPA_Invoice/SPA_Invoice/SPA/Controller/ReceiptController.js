app.controller('ReceiptController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', '$location', '$window', function ($scope, $rootScope, $timeout, CommonFactory, $location, $window) {
    var url = '/Receipt/';
    angular.element(function () {
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetInvoiceStatus();
        //$rootScope.GetPaymentStatus();
        $scope.LoadDashboard();
    });
    //========================== Cookie's Own ============================
    $scope.LoadCookie_Receipt = function () {
        var check = getCookie('Novaon_ReceiptManagement');
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
                RowNum: 10
            }
            setCookie('Novaon_ReceiptManagement', JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie('Novaon_ReceiptManagement', JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetReceiptPaging($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.Tab = {
        All: true,
        Cancel: false,
        Tranfer: false,
        Change: false,
        Replace: false
    };

    $rootScope.ReceiptTabClick = function (tabIndex) {
        $scope.Tab.All = false;
        $scope.Tab.Cancel = false;
        $scope.Tab.Tranfer = false;
        $scope.Tab.Change = false;
        $scope.Tab.Replace = false;
        $scope.Filter = new Object();
        $scope.ListReceipt = [];
        switch (tabIndex) {
            case 1:
                $scope.Tab.All = true;
                $scope.Filter = new Object();
                $scope.GetReceiptPaging(1);
                break;
            case 2:
                $scope.Tab.Cancel = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 3:
                $scope.Tab.Tranfer = true;
                $scope.Filter.INVOICETYPE = 4;
                $scope.GetReceiptPaging(1);
                break;
            case 4:
                $scope.Tab.Change = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
                break;
            case 5:
                $scope.Tab.Replace = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            default:
                $scope.Tab.All = true;
                break;
        }
        $('.dropdown-menu').removeClass('show');
    }

    $rootScope.ReloadReceipt = function () {
        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
            if ($scope.Tab.Cancel) {
                $scope.GetReceiptByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else {
                $scope.ListReceipt = new Array();
                $scope.GetReceiptPaging($scope.currentpage);
            }
        }
    }

    $scope.GetReceiptPaging = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Receipt();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetReceiptPaging';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListReceipt = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListReceipt = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReceiptPaging');
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
                    var f = $scope.ListReceiptStatus.filter(function (item) {
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

    /**
     * truongnv 2020-02-14
     * Gán data filter khi người dùng chọn
     * @param {any} type
     * @param {any} intpage
     */
    $scope.GetReceiptByStatus = function (type, intpage, reportType) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        $scope.LoadCookie_Receipt();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetReceiptByStatus';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            type: type,
            intpage: intpage,
            reportType: reportType
        });
        $scope.ListReceipt = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListReceipt = response.result;
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

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        //Refresh filter
        $rootScope.ReloadReceipt();
    };

    $scope.SeleteRow = function (list, item, isselect) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (item)
            isselect = false;
        else {
            if (find.length == list.length - 1) {
                isselect = true;
            }
        }
    }

    $scope.Sign = function (idInvoice) {
        LoadingShow();
        Sign(idInvoice);
    }

    $scope.LoadDashboard = function () {
        var action = url + 'LoadDashboard';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.TotalMoneyNotPay = response.TotalMoneyNotPay;
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

    $rootScope.PreviewInvoice = function (item, isSignLink) {
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
                        $rootScope.ReloadReceipt();
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

    $scope.SendEmail = function (item) {
        $scope.IsLoading = true;
        var action = url + 'SendEmail';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
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
        $rootScope.ModalReceipt(item, 2);
        $('.modal-receipt').modal('show');
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
                $rootScope.ModalReceipt(item, 6);
                $('.modal-receipt').modal('show');
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
            $rootScope.ModalReceipt(item, 5);
            $('.modal-receipt').modal('show');
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
                    $rootScope.ModalReceipt(item, 5);
                    $('.modal-receipt').modal('show');
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
                $rootScope.ModalReceipt(item, 5);
                $('.modal-receipt').modal('show');
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
                var listInvoiceChecked = $scope.ListReceipt.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }

                var listInvoiceSigned = $scope.ListReceipt.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
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
                    $rootScope.ReloadReceipt();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }


    $scope.ModalSendEmail = function (item) {
        $('#modal_send_email').dialog({
            width: '45%',
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
                $('#modal_send_email').show();
            }
        });

        $('#file-selected').html('');

        $scope.GetEmailHistory(item.ID);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        //open 
        //$('#modal-send-email').dialog('open');
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
                alert('Email bạn nhập không đúng định dạng email: ' + lstEmail[i+1]);
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
                    alert(response.msg);
                    $('#modal_send_email').dialog("close");
                    $rootScope.ReloadReceipt();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
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

    $('#frmUploadFileReport').fileupload({
        autoUpload: false,
        add: function (e, data) {
            LoadingShow();
            var fileData = new FormData();
            var data = data.files[0];
            fileData.append('file0', data);
            fileData.append('currentItem', $scope.CurrentItem.ID);
            fileData.append('comTaxCode', $scope.CurrentItem.COMTAXCODE);

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
                            $rootScope.ReloadReceipt();
                            alert('Thành công');
                        } else {
                            alert(result.msg);
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
                    alert(response.msg);
                    $rootScope.ReloadReceipt();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
    }

    $scope.ExportExcel = function () {
        var check = getCookie('Novaon_ReceiptManagement');
        var action = url + 'ExportExcell';
        var datasend = {
            form: $scope.Filter,
            currentPage: 1,
            fieldsCookies: check
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

    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
    }

    $scope.SetDatepikerToTime = function () {
        var fromTime = $scope.FROMTIME;
        $("#pk_totime").datepicker("option", 'minDate', fromTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(fromTime)
        });
        SetVietNameInterface($("#pk_totime"));
    }

    /*
     *truongnv 20200317
     * Sắp sếp dữ liệu
     * */
    // column to sort
    $scope.column = 'NUMBER';

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

    $window.ReloadInvoice = function () {
        $rootScope.ReloadReceipt();
    };
}]);
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