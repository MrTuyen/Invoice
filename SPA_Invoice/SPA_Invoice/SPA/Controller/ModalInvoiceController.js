app.controller('ModalInvoiceController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$http', '$location', 'permissions', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $http, $location, permissions) {
    var url = '/Invoice/';
    //type: 1: tạo mới, 2: xem chi tiết, 3: hủy bỏ, 4: chuyển đổi, 5: điều chỉnh, 6: thay thế, 7: dải chờ

    var msgFromDateToDatePaymentTerm = "Từ ngày của kỳ thanh toán không được lớn hơn đến ngày."; // Hóa đơn điện, nước
    var msgImportDateExportDate = "Ngày nhập kho không nhỏ hơn ngày xuất kho"; // Phiếu xuất kho

    $rootScope.ModalInvoice = function (item, type, formcode, symbolcode, isCopy) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetPaymentMethod();
        //$rootScope.GetQuantityUnit();

        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = [];
        $scope.Invoice.INVOICETYPE = type;
        $scope.IsCopy = false;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
        $scope.Invoice.CURRENCY = "VND";
        $scope.Invoice.EXCHANGERATE = 1;
        $scope.GLOBALTAXRATEWATER = 0;
        $scope.Invoice.SERVICEFEETAXRATE = $scope.TaxRateList[0].value;

        $scope.Invoice.CUSTOMFIELDEXCHANGERATE = 0;
        $scope.Invoice.CUSTOMFIELDEXCHANGE = 0;

        $scope.LoadCurrencyExchangeRate();
        $scope.TempListFormCode = $rootScope.ListFormCode;

        $('.modal-invoice').modal('show');
        if (type == 1) {
            $scope.Invoice.ID = 0;
            $scope.Invoice.CUSPAYMENTMETHOD = "TM/CK";
            $scope.Invoice.FORMCODE = $rootScope.ListFormCode[0].FORMCODE + '-' + $rootScope.ListFormCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = $rootScope.ListFormCode[0].SYMBOLCODE;
            $scope.TAXRATE = $rootScope.ListFormCode[0].TAXRATE;
            $scope.Invoice.DISCOUNTTYPE = "KHONG_CO_CHIET_KHAU";
            if (item) {
                angular.copy(item, $scope.Invoice);
                if (isCopy) {
                    var formCode = $rootScope.ListFormCode.filter(function (x) {
                        return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
                    })
                    $scope.TAXRATE = formCode[0].TAXRATE;
                    $scope.Invoice.FORMCODE = item.FORMCODE + '-' + item.SYMBOLCODE;
                    $scope.Invoice.INVOICESTATUS = 1;
                    $scope.Invoice.NUMBER = 0;
                    $scope.Invoice.ID = 0;
                    $scope.Invoice.INVOICETYPE = type;
                    $scope.Invoice.REFERENCE = 0;
                    $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $scope.IsCopy = isCopy;
                    if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                        $scope.GetMeterByInvoiceId(item.ID);
                    }
                    $scope.GetInvoiceDetail(item.ID)
                }
                else {
                    $scope.MeterList = null;
                }
            }
            else {
                $scope.Invoice.NUMBER = 0;
                if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONGTGT || $rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG || $rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
                    $timeout(function () {
                        $scope.AddRow();
                    }, 100);
                }
            }
            //Reset danh sách công tơ trong danh sách sản phẩm đối với hóa đơn điện
            $scope.MeterList = null;
            $scope.Invoice.METER = {
                METERNAME: "[Số công tơ]",
                CODE: "[Số công tơ]"
            };

            // check waiting invoice
            if (formcode && symbolcode) {
                $scope.Invoice.FORMCODE = formcode;
                $scope.Invoice.SYMBOLCODE = symbolcode;
                $scope.Invoice.NUMBER = 0;
                $scope.Invoice.INVOICESTATUS = 1;
                $scope.Invoice.INVOICETYPE = 1;
                $scope.Invoice.ISINVOICEWAITING = true;
            }
        } else if (type == 2) {
            item.EXCHANGERATE = item.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            if (item.INVOICETYPE == 5 || item.INVOICETYPE == 3 || item.INVOICETYPE == 6) {
                $scope.GetInvoiceById(item.REFERENCE);
            }
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByInvoiceId($scope.Invoice.ID);
            }

            $scope.GetInvoiceDetail(item.ID);
            setTimeout(function () {
                if ($scope.RefInvoice) {
                    $scope.Invoice.NUMBERTEMP = $scope.RefInvoice.NUMBER;
                    $scope.Invoice.SIGNEDTIME = $scope.RefInvoice.SIGNEDTIME;
                }
            }, 2000)
        } else if (type == 5) {
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByCustaxcode($scope.Invoice.CUSTAXCODE);
            }

            $scope.Invoice.LISTPRODUCT = [];
            //$timeout(function () {
            //    $scope.Invoice.LISTPRODUCT.push({ QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: -1, RETAILPRICE: 0 });
            //}, 100);
            $scope.GetInvoiceDetail(item.ID);

        } else if (type == 6) {
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByCustaxcode($scope.Invoice.CUSTAXCODE);
            }

            $scope.GetInvoiceDetail(item.ID)
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;
        }

        $scope.Invoice.STRDUEDATE = $filter('dateFormat')($scope.Invoice.DUEDATE, 'dd/MM/yyyy');
        $scope.Invoice.STRINVOICEWAITINGTIME = $filter('dateFormat')($scope.Invoice.INVOICEWAITINGTIME, 'dd/MM/yyyy');

        $scope.Invoice.FROMDATE = $filter('dateFormat')($scope.Invoice.FROMDATE, 'dd/MM/yyyy');
        $scope.Invoice.TODATE = $filter('dateFormat')($scope.Invoice.TODATE, 'dd/MM/yyyy');
        $scope.Invoice.DELIVERYORDERDATE = $filter('dateFormat')($scope.Invoice.DELIVERYORDERDATE, 'dd/MM/yyyy');
        $scope.Invoice.FROMDATESTR = $scope.Invoice.FROMDATE;
        $scope.Invoice.TODATESTR = $scope.Invoice.TODATE;
        $scope.Invoice.DELIVERYORDERDATESTR = $scope.Invoice.DELIVERYORDERDATE;
        $(".datepicker").datepicker({
            buttonText: "Chọn ngày tháng",
            dateFormat: "dd/mm/yy"
        });
    }

    $scope.LoadCookie_Invoice_CustomField = function () {
        var check = getCookie('Novaon_Invoice_CustomField');
        if (check) {
            $scope.cookie_customField = JSON.parse(check);
        }
        else {
            $scope.cookie_customField = {
                FieldConsignment: false,
                FieldProductService: true,
                FieldUnit: true,
                FieldQuantity: true,
                FieldPrice: true,
                FieldMoney: true,
                FieldExchangeRateAdd: false,
                FieldExchangeAdd: false,
                FieldOtherTaxFee: false,
                FieldRefundFee: false,
                FieldServiceFee: false
            }
            setCookie('Novaon_Invoice_CustomField', JSON.stringify($scope.cookie_customField), 30);
        }
    }

    $scope.Check = function (status, field) {
        $scope.cookie_customField[field] = !status;
        setCookie('Novaon_Invoice_CustomField', JSON.stringify($scope.cookie_customField), 30);
    }

    $scope.GetInvoiceDetail = function (invoiceid) {
        var action = url + 'GetInvoiceDetail';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (data) {
            if (data) {
                if (data.rs) {
                    $scope.Invoice.LISTPRODUCT = data.result;
                    if ($scope.Invoice.LISTPRODUCT.length == 0) {
                        $timeout(function () {
                            $scope.AddRow();
                        });
                    }
                    if ($scope.TAXRATE === 1) {
                        if ($scope.Invoice.LISTPRODUCT.length > 0) {
                            $scope.ONLYTAXRATE = $scope.Invoice.LISTPRODUCT[0].TAXRATE;
                            $scope.GLOBALTAXRATE = $scope.Invoice.LISTPRODUCT[0].TAXRATE;
                        }
                    }

                    $timeout(function () {
                        if ($scope.Invoice.LISTPRODUCT && Object.keys($scope.Invoice.LISTPRODUCT).length > 0) {
                            $scope.GLOBALTAXRATEWATER = $scope.Invoice.LISTPRODUCT[0].TAXRATEWATER;//Lấy ra thông tin phí bảo vệ môi trường đối với nước
                            setSelectedValueDropdown($rootScope.Enterprise.USINGINVOICETYPE, $scope.Invoice.LISTPRODUCT[0].METERCODE);
                        }
                    }, 100);
                } else {
                    $scope.ErrorMessage = data.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceDetail');
            }
            LoadingHide();
        });
    }

    $scope.AddInvoice = function (type, isSaveAndRelease) {
        var oReturn = {
            messErrorForCustomer: '',
            messErrorForCoder: '',
            result: true
        };
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            return false;
        }

        var formCodeSymbodeCode = $scope.Invoice.FORMCODE.split('-');
        $scope.Invoice.FORMCODE = formCodeSymbodeCode[0];
        $scope.Invoice.SYMBOLCODE = formCodeSymbodeCode[1];

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn ký hiệu hóa đơn');
            return false;
        }
        //Nếu là hóa đơn tiền điện thì kiểm tra thêm từ ngày, đến ngày của kỳ thanh toán
        if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Kỳ thanh toán từ ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.TODATE, "Kỳ thanh toán đến ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }

            oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgFromDateToDatePaymentTerm);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        //Nếu là phiếu xuất kho thì kiểm tra thêm ngày điều động, ngày xuất, ngày nhập
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            if (!$scope.Invoice.CUSNAME) {
                alert('Của: không được để trống');
                return false;
            }

            oReturn = CheckDate($scope.Invoice.DELIVERYORDERDATE, "Ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Ngày xuất kho", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        if (!$scope.Invoice.CUSNAME) {
            if (!$scope.Invoice.CUSBUYER) {
                alert('Bạn phải nhập vào thông tin <strong>Tên đơn vị</strong> hoặc thông tin <b>Người mua hàng</b>');
                return false;
            }
        }
        if (!$scope.Invoice.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }

        //if ($scope.Invoice.CUSTAXCODE && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    var res = $scope.Invoice.CUSTAXCODE.split('-');
        //    if (res.length == 1) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    } else if (res.length == 2) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //        if (res[1].length != 3 || !validation.isNumber(res[1])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    }
        //}

        if ($scope.Invoice.INVOICETYPE === 5 && $scope.Invoice.INVOICEMETHOD === 0) {
            alert('Vui chọn loại hóa đơn điều chỉnh.');
            return false;
        }

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email.');
                return;
            }
        }

        if (!$scope.Invoice.CHANGEREASON && $scope.Invoice.INVOICETYPE == 5) {
            alert('Vui lòng nhập lý do điều chỉnh.');
            return false;
        }

        if ($scope.Invoice.INVOICETYPE != 5) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                if (!obj.PRODUCTNAME && $scope.Invoice.INVOICETYPE != 5) {
                    alert('Tên sản phẩm không được để trống, vui lòng kiểm tra lại dữ liệu dòng thứ ' + (index + 1));
                }
                //obj.QUANTITY = obj.QUANTITY.toString().replace(/\./g, ",");
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                obj.QUANTITY = parseFloat(obj.QUANTITY.toString());
            });

            var find = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                return !s.PRODUCTNAME;
            });

            if (find.length > 0 && $scope.Invoice.INVOICETYPE != 5) {
                return false;
            }
        }

        // for only tax rate
        if ($scope.TAXRATE == 1) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                obj.TAXRATE = $scope.GLOBALTAXRATE;
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                // hóa đơn bán hàng không có thuế. Gán mặc định = -1
                //if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                //    obj.TAXRATE = -1;
                //}
            });
        }

        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '').replace(/\./g, '');
        //$scope.Invoice.CUSTOMFIELDEXCHANGE = $scope.Invoice.CUSTOMFIELDEXCHANGE.replace(/\,/g, '');
        //Tính lại giá tiền
        $scope.CalMoneyAfterChangeValue();

        // Map lại thông tin khi số lượng, đơn giá không điền. Để lưu thông tin vào database
        $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
            obj.TOTALMONEY = obj.ITEMTOTALMONEY;
        });
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENTRAW;
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEYRAW;
        $scope.Invoice.TAXMONEY = $scope.Invoice.TAXMONEYRAW;
        LoadingShow();
        var action = url + 'AddInvoice';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (isSaveAndRelease) {
                        var invoiceId = response.result;
                        Sign(invoiceId).then(function () {
                            toastr.success("Thêm mới thành công!", "Thành công", { timeOut: 4000 })
                            $('.modal-invoice').modal('hide');
                            if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                                $rootScope.ReloadReceipt();
                            }
                            else if ($location.path().toString().includes('/quan-ly-hoa-don')) {
                                $rootScope.ReloadInvoice();
                            }
                        });
                    }
                    else {
                        toastr.success("Thêm mới thành công!", "Thành công", { timeOut: 4000 })
                        $('.modal-invoice').modal('hide');
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ReloadReceipt();
                        }
                        else if ($location.path().toString().includes('/quan-ly-hoa-don')) {
                            $rootScope.ReloadInvoice();
                        }
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.UpdateInvoice = function (type, isSaveAndRelease) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }

        var formCodeSymbodeCode = $scope.Invoice.FORMCODE.split('-');
        $scope.Invoice.FORMCODE = formCodeSymbodeCode[0];
        $scope.Invoice.SYMBOLCODE = formCodeSymbodeCode[1];

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#symbol-code').focus();
            return false;
        }

        //Nếu là hóa đơn tiền điện thì kiểm tra thêm từ ngày, đến ngày của kỳ thanh toán
        if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
            var oReturn = {
                messErrorForCustomer: '',
                messErrorForCoder: '',
                result: true
            };
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Kỳ thanh toán từ ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.TODATE, "Kỳ thanh toán đến ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }

            oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgFromDateToDatePaymentTerm);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        //Nếu là phiếu xuất kho thì kiểm tra thêm ngày điều động, ngày xuất, ngày nhập
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            oReturn = CheckDate($scope.Invoice.DELIVERYORDERDATE, "Ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Ngày xuất kho", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            //oReturn = CheckDate($scope.Invoice.TODATE, "Ngày nhập kho", 10, true);
            //if (oReturn.result === false) {
            //    alert(oReturn.messErrorForCustomer);
            //    return false;
            //}
            //oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgImportDateExportDate);
            //if (oReturn.result === false) {
            //    alert(oReturn.messErrorForCustomer);
            //    return false;
            //}
        }

        //if ($scope.Invoice.CUSTAXCODE && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    var res = $scope.Invoice.CUSTAXCODE.split('-');
        //    if (res.length == 1) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    } else if (res.length == 2) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //        if (res[1].length != 3 || !validation.isNumber(res[1])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    }
        //}

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email');
                return;
            }
        }

        if (!$scope.Invoice.CHANGEREASON && $scope.Invoice.INVOICETYPE == 5) {
            alert('Vui lòng nhập lý do điểu chỉnh');
            return false;
        }

        if ($scope.Invoice.INVOICETYPE != 5) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                if (!obj.PRODUCTNAME && $scope.Invoice.INVOICETYPE != 5) {
                    alert('Tên sản phẩm không được để trống, vui lòng kiểm tra lại dữ liệu dòng thứ ' + (index + 1));
                }
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                //obj.QUANTITY = obj.QUANTITY.toString().replace(/\./g, ",");
                obj.QUANTITY = parseFloat(obj.QUANTITY.toString());
            });

            var find = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                return !s.PRODUCTNAME;
            });

            if (find.length > 0 && $scope.Invoice.INVOICETYPE != 5) {
                return false;
            }
        }

        // for only tax rate
        if ($scope.TAXRATE == 1) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                obj.TAXRATE = $scope.GLOBALTAXRATE;
                // hóa đơn bán hàng không có thuế. Gán mặc định = -1
                //if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                //    obj.TAXRATE = -1;
                //}
            });
        }
        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '');/*.replace(/\./g, '')*/;
        //$scope.Invoice.CUSTOMFIELDEXCHANGE = $scope.Invoice.CUSTOMFIELDEXCHANGE.replace(/\,/g, '');
        //$scope.CalMoneyAfterChangeValue();
        // Map lại thông tin khi số lượng, đơn giá không điền. Để lưu thông tin vào database
        $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
            obj.TOTALMONEY = obj.ITEMTOTALMONEY;
        });
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENTRAW;
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEYRAW;
        $scope.Invoice.TAXMONEY = $scope.Invoice.TAXMONEYRAW;
        LoadingShow();
        var action = url + 'UpdateInvoice';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (isSaveAndRelease) {
                        var invoiceId = response.result;
                        Sign(invoiceId).then(function () {
                            toastr.success("Cập nhật thành công!", "Thành công", { timeOut: 4000 })
                            $('.modal-invoice').modal('hide');
                            if ($location.path().toString().includes('/dai-hoa-don-cho')) {
                                $rootScope.ReloadWaitingInvoice($scope.Invoice.FORMCODE, $scope.Invoice.SYMBOLCODE);
                            }
                            else {
                                $rootScope.ReloadInvoice();
                            }
                        });           
                    }
                    else {
                        toastr.success("Cập nhật thành công!", "Thành công", { timeOut: 2000 })
                        $('.modal-invoice').modal('hide');
                        if ($location.path().toString().includes('/dai-hoa-don-cho')) {
                            $rootScope.ReloadWaitingInvoice($scope.Invoice.FORMCODE, $scope.Invoice.SYMBOLCODE);
                        }
                        else {
                            $rootScope.ReloadInvoice();
                        }
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    //Thêm dòng sản phẩm
    $scope.AddRow = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, INQUANTITY: 1, TAXRATE: 0, RETAILPRICE: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
        }
        else {
            //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, RETAILPRICE: 0, OTHERTAXFEE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
            $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, RETAILPRICE: 0, OTHERTAXFEE: 0, REFUNDFEE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
        }
    }

    $scope.AddRowHeader = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: 0, OTHERTAXFEE: 0, REFUNDFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, ITEMMONEY: 0, ITEMTOTALMONEY: 0, GROUPID: 1, SORTORDER: 0, DESCRIPTION: null });
        //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: 0, OTHERTAXFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, ITEMMONEY: 0, ITEMTOTALMONEY: 0, GROUPID: 1, SORTORDER: 0, DESCRIPTION: null });

        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, OTHERTAXFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, DESCRIPTION: null });
        $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, OTHERTAXFEE: 0, REFUNDFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, DESCRIPTION: null });
    }

    //Xóa dòng sản phẩm
    $scope.RemoveRow = function (item) {
        if (item.PRODUCTNAME) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                        return s != item;
                    });
                    if ($scope.Invoice.LISTPRODUCT.length == 0)
                        $scope.AddRow();
                });
                $scope.CalMoneyAfterChangeValue();
            };
            confirm("Bạn chắc chắn muốn xóa sản phẩm \"" + item.PRODUCTNAME + "\"?", "Xóa sản phẩm", "Không", "Có", confirmContinue)
        }
        else {
            $timeout(function () {
                $scope.Invoice.LISTPRODUCT = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                    return s != item;
                });
                if ($scope.Invoice.LISTPRODUCT.length == 0)
                    $scope.AddRow();
            });
            $scope.CalMoneyAfterChangeValue();
        }
    }

    //Xử lý nhảy dòng
    $scope.onKeypress = function (index) {
        $timeout(function () {
            var keyCode = $scope.Invoice.LISTPRODUCT[index].keyCode;

            if (keyCode == 38) {
                //Up arrow
                if (index > 0) {
                    $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                        obj.isFocus = false;
                    });
                    $timeout(function () {
                        $scope.Invoice.LISTPRODUCT[index - 1].isFocus = true;
                    });
                }
            } else if ((keyCode == 40 || keyCode == 13) && $scope.Invoice.LISTPRODUCT.length - 1 == index) {
                //Nếu là dòng cuối thì thêm dòng mới
                $scope.AddRow();

            } else {
                //Nếu chưa phải dòng cuối thì focus dòng bên dưới
                $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                    obj.isFocus = false;
                });
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT[index + 1].isFocus = true;
                });
            }
        });
    }

    $scope.CreateRowDescription = function (index) {
        var $item_p = ('#Item_p_' + index);
        $($item_p).removeClass('ng-hide');
        $($item_p).addClass('ng-show');
    }

    $scope.CloseRowDescription = function (index) {
        var $item_p = ('#Item_p_' + index);
        $($item_p).hide();
    }

    //Xử lý nhảy dòng
    $scope.onKeypress = function (index) {
        $timeout(function () {
            var keyCode = $scope.Invoice.LISTPRODUCT[index].keyCode;
            if (keyCode == 38) {
                //Up arrow
                if (index > 0) {
                    $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                        obj.isFocus = false;
                    });
                    $timeout(function () {
                        $scope.Invoice.LISTPRODUCT[index - 1].isFocus = true;
                    });
                }
            } else if ((keyCode == 40 || keyCode == 13) && $scope.Invoice.LISTPRODUCT.length - 1 == index) {
                //Nếu là dòng cuối thì thêm dòng mới
                $scope.AddRow();

            } else {
                //Nếu chưa phải dòng cuối thì focus dòng bên dưới
                $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                    obj.isFocus = false;
                });
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT[index + 1].isFocus = true;
                });
            }
        });
    }

    $scope.ChangeRetailPrice = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        var vPrice = item.PRICE;
        if (item.PRICE != null && !isNaN(parseFloat(item.PRICE))) {
            vPrice = GetNumber(item.PRICE);
        }
        item.RETAILPRICE = vPrice;
        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            item.ITEMTOTALMONEY = item.TOTALMONEY;
            item.ITEMMONEY = item.TOTALMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }

    $scope.ChangeTotalPrice = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        var itemTotalPrice = item.ITEMMONEY;
        if (item.ITEMMONEY != null && !isNaN(parseFloat(item.ITEMMONEY))) {
            itemTotalPrice = GetNumber(item.ITEMMONEY);
        }
        item.ITEMTOTALMONEY = itemTotalPrice;
        if (item.QUANTITY !== 0 && item.QUANTITY !== "0" && item.QUANTITY !== "") {
            item.PRICE = (itemTotalPrice / item.QUANTITY).toFixed(3).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            item.RETAILPRICE = (itemTotalPrice / item.QUANTITY);
        }
        if (item.QUANTITY === "0") {
            item.RETAILPRICE = 0;
            item.PRICE = 0;
        }
        if (item.QUANTITY === "") {
            item.RETAILPRICE = null;
            item.PRICE = null;
        }

        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            $scope.CalTotalMoney();
            $scope.CalTaxMoney();
            $scope.CalDiscountMoney();
            $scope.CalTotalPayment();
        }
    }

    $scope.QuantityChange = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        item.QUANTITY = item.QUANTITY.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            item.ITEMTOTALMONEY = item.TOTALMONEY;
            item.ITEMMONEY = item.TOTALMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }

    $scope.DiscountChange = function (item, isDiscountRateChange) {
        if ($scope.Invoice.DISCOUNTTYPE == 'CHIET_KHAU_THEO_HANG_HOA') {
            //item.QUANTITY = item.QUANTITY.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
            //if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            //    item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            //    item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //    $scope.CalMoneyAfterChangeValue();
            //}
            //else {
            //    item.ITEMTOTALMONEY = item.ITEMMONEY;
            //    item.ITEMMONEY = item.ITEMMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //}

            //var itemTotalMoney = Number(item.ITEMTOTALMONEY.replace(/[^0-9.-]+/g, ""));
            if (!isDiscountRateChange) {
                item.DISCOUNTRATE = item.TOTALDISCOUNT / item.ITEMTOTALMONEY * 100;
            }
            else {
                item.TOTALDISCOUNT = item.DISCOUNTRATE * item.ITEMTOTALMONEY / 100;
            }
            $scope.CalMoneyAfterChangeValue();
        }
    }

    $scope.SuggestProduct = function (item) {
        if (item) {
            var obj = $rootScope.selectedResultobj;
            item.RETAILPRICE = obj.PRICE;
            item.PRODUCTID = obj.PRODUCTID;
            item.PRODUCTNAME = obj.PRODUCTNAME;
            item.QUANTITYUNIT = obj.QUANTITYUNIT;
            item.QUANTITY = item.QUANTITY > 0 ? item.QUANTITY : 1;
            item.TAXRATE = item.TAXRATE > 0 ? item.TAXRATE : 0;
            item.PRICE = obj.PRICE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
    }

    $scope.QuantityUnitChange = function (item) {
        var listUnit = [];
        angular.forEach($rootScope.ListQuantityUnit, function (key) {
            listUnit.push(key.QUANTITYUNIT);
        });

        $('#quantityunit').autoComplete({
            minChars: 1,
            source: function (xxx, suggest) {
                var choices = JSON.parse(JSON.stringify(listUnit));
                var suggestions = [];
                for (i = 0; i < choices.length; i++) {
                    if (~($rootScope.cleanAccents(choices[i]).toLowerCase()).indexOf($rootScope.cleanAccents(item.QUANTITYUNIT.toLowerCase())))
                        suggestions.push(choices[i]);
                    suggest(suggestions);
                }
            }
        });
    }

    $scope.CalTotalMoney = function () {
        $scope.CalDiscountMoney();
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.TOTALMONEY = 0;
        $scope.Invoice.TOTALMONEYRAW = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    let total = element.QUANTITY * element.RETAILPRICE;
                    $scope.Invoice.TOTALMONEYRAW += total;
                    $scope.Invoice.TOTALMONEY += total * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }
                    $scope.Invoice.TOTALMONEYRAW += element.ITEMTOTALMONEY;
                    $scope.Invoice.TOTALMONEY += element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                }
            }
        });
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEY - $scope.Invoice.DISCOUNTMONEY;
        $scope.Invoice.TOTALMONEYRAW = $scope.Invoice.TOTALMONEYRAW - $scope.Invoice.DISCOUNTMONEYRAW;

        // CURRENCY != VND
        $scope.Invoice.TOTALMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.TOTALMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.TOTALMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.TOTALMONEY;
    }

    $scope.CalTaxMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;

        $scope.Invoice.TAXWATERMONEY = 0;
        $scope.Invoice.TAXMONEY = 0;
        $scope.Invoice.TAXMONEYRAW = 0;

        if ($scope.TAXRATE == 1) {
            $scope.Invoice.TAXMONEY = $scope.Invoice.TOTALMONEY * ($scope.GLOBALTAXRATE == -1 ? 0 : $scope.GLOBALTAXRATE) / 100;
            $scope.Invoice.TAXMONEYRAW = $scope.Invoice.TOTALMONEYRAW * ($scope.GLOBALTAXRATE == -1 ? 0 : $scope.GLOBALTAXRATE) / 100;
        }
        else {
            $scope.Invoice.LISTPRODUCT.forEach(function (element) {
                var totalMoney = 0;
                var discountMoney = 0;
                var totalMoneyRaw = 0;
                var discountMoneyRaw = 0;

                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    totalMoney = element.QUANTITY * element.RETAILPRICE * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                    discountMoney = element.DISCOUNTRATE * totalMoney / 100;

                    totalMoneyRaw = element.QUANTITY * element.RETAILPRICE;
                    discountMoneyRaw = element.DISCOUNTRATE * totalMoneyRaw / 100;
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }
                    totalMoney = element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                    discountMoney = element.DISCOUNTRATE * totalMoney / 100;

                    totalMoneyRaw = element.ITEMTOTALMONEY;
                    discountMoneyRaw = element.DISCOUNTRATE * totalMoneyRaw / 100;
                }
                

                if (!element.ISPROMOTION) {
                    $scope.Invoice.TAXMONEY += (totalMoney - discountMoney) * (element.TAXRATE == -1 ? 0 : element.TAXRATE) / 100;
                    $scope.Invoice.TAXMONEYRAW += (totalMoneyRaw - discountMoneyRaw) * (element.TAXRATE == -1 ? 0 : element.TAXRATE) / 100;
                }
            });
        }              

        //Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        if ($rootScope.Enterprise.USINGINVOICETYPE === 4) {
            $scope.Invoice.TAXWATERMONEY += ($scope.Invoice.TOTALMONEY * $scope.GLOBALTAXRATEWATER) / 100;
            $scope.Invoice.TAXWATERMONEYRAW += $scope.Invoice.TOTALMONEY * ($scope.GLOBALTAXRATEWATER / 100);
        }

        // CURRENCY != VND
        $scope.Invoice.TAXMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.TAXMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.TAXMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.TAXMONEY;
    }

    $scope.CalOtherTaxFeeMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.OTHERTAXFEE = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    if (element.OTHERTAXFEE === "")
                        element.OTHERTAXFEE = 0;
                    $scope.Invoice.OTHERTAXFEE += parseFloat(element.OTHERTAXFEE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.OTHERTAXFEE === "")
                        element.OTHERTAXFEE = 0;
                    $scope.Invoice.OTHERTAXFEE += parseFloat(element.OTHERTAXFEE.toString().replace(/\,/g, ''));
                }
            }
        });
        return $scope.Invoice.OTHERTAXFEE;
    }

    $scope.CalRefundFee = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.REFUNDFEE = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    if (element.REFUNDFEE === "")
                        element.REFUNDFEE = 0;
                    $scope.Invoice.REFUNDFEE += parseFloat(element.REFUNDFEE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.REFUNDFEE === "")
                        element.REFUNDFEE = 0;
                    $scope.Invoice.REFUNDFEE += parseFloat(element.REFUNDFEE.toString().replace(/\,/g, ''));
                }
            }
        });
        return $scope.Invoice.REFUNDFEE;
    }

    $scope.CalDiscountMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.DISCOUNTMONEY = 0;
        $scope.Invoice.DISCOUNTMONEYRAW = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    let total = element.QUANTITY * element.RETAILPRICE;
                    $scope.Invoice.DISCOUNTMONEY += element.DISCOUNTRATE * (total * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''))) / 100;
                    $scope.Invoice.DISCOUNTMONEYRAW += element.DISCOUNTRATE * total / 100;
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }

                    $scope.Invoice.DISCOUNTMONEY += element.DISCOUNTRATE * (element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''))) / 100;
                    $scope.Invoice.DISCOUNTMONEYRAW += element.DISCOUNTRATE * (element.ITEMTOTALMONEY) / 100;
                }
            }
        });

        // CURRENCY != VND
        $scope.Invoice.DISCOUNTMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.DISCOUNTMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.DISCOUNTMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.DISCOUNTMONEY;
    }

    $scope.CalServiceFeeTax = function () {
        if ($scope.Invoice.SERVICEFEETAXRATE === 0 || $scope.Invoice.SERVICEFEETAXRATE === -1) {
            $scope.Invoice.SERVICEFEETAX = 0;
        }
        else {
            $scope.Invoice.SERVICEFEETAX = $scope.Invoice.SERVICEFEE * $scope.Invoice.SERVICEFEETAXRATE / 100;
        }
        $scope.CalTotalServiceFee();
        return $scope.Invoice.SERVICEFEETAX;
    }

    $scope.CalTotalServiceFee = function () {
        $scope.Invoice.TOTALSERVICEFEE = parseFloat($scope.Invoice.SERVICEFEE) + parseFloat($scope.Invoice.SERVICEFEETAX);
        $scope.CalMoneyAfterChangeValue();
        return $scope.Invoice.TOTALSERVICEFEE;
    }

    $scope.CalTotalPayment = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        if (!$scope.Invoice.TOTALMONEY) {
            $scope.Invoice.TOTALMONEY = 0;
        }
        if (!$scope.Invoice.DISCOUNTMONEY) {
            $scope.Invoice.DISCOUNTMONEY = 0;
        }
        if (!$scope.Invoice.TAXMONEY) {
            $scope.Invoice.TAXMONEY = 0;
        }
        if (!$scope.Invoice.TAXWATERMONEY) {
            $scope.Invoice.TAXWATERMONEY = 0;
        }
        if (!$scope.Invoice.OTHERTAXFEE) {
            $scope.Invoice.OTHERTAXFEE = 0;
        }
        if (!$scope.Invoice.REFUNDFEE) {
            $scope.Invoice.REFUNDFEE = 0;
        }
        if (!$scope.Invoice.TOTALSERVICEFEE) {
            $scope.Invoice.TOTALSERVICEFEE = 0;
        }

        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEYRAW
        }
        else if ($rootScope.Enterprise.USINGINVOICETYPE === 4)//Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXWATERMONEY + $scope.Invoice.TAXMONEY;
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXWATERMONEY + $scope.Invoice.TAXMONEY;
        }
        else {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXMONEY;
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEYRAW + $scope.Invoice.TAXMONEYRAW;
        }
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENT + $scope.Invoice.OTHERTAXFEE - $scope.Invoice.REFUNDFEE + $scope.Invoice.TOTALSERVICEFEE;
        $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALPAYMENTRAW + $scope.Invoice.OTHERTAXFEE - $scope.Invoice.REFUNDFEE + $scope.Invoice.TOTALSERVICEFEE;
        return $scope.Invoice.TOTALPAYMENT;
    }

    $scope.CalMoneyAfterChangeValue = function () {
        if ($scope.TAXRATE == 1) {
            $scope.GLOBALTAXRATE = parseInt($('#idOnlyTaxRate').val());
        }
        //Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        if ($rootScope.Enterprise.USINGINVOICETYPE === 4) {
            $scope.GLOBALTAXRATEWATER = parseInt($('#txtTaxwater').val());
        }
        $timeout(function () {
            $scope.ChangeDiscountType();
            $scope.CalDiscountMoney();
            $scope.CalTotalMoney();
            $scope.CalTaxMoney();
            $scope.CalOtherTaxFeeMoney();
            $scope.CalRefundFee();
            $scope.CalTotalPayment();
            $scope.CalExChangeStr();
            $scope.ReadNumberToCurrencyWords();
        }, 100);
    }

    //Tihns quy đôiỉ
    $scope.CalExChangeStr = function () {
        //var cur = $scope.Invoice.TOTALPAYMENT / $scope.Invoice.CUSTOMFIELDEXCHANGERATE;
        var cur = $scope.Invoice.TOTALPAYMENT / $scope.Invoice.EXCHANGERATE;
        $scope.Invoice.CUSTOMFIELDEXCHANGE = (cur).formatMoney(2, ".", ",");
    }

    //$scope.ReadNumberToCurrencyWords = function () {
    //    var money = 0;
    //    // Kiểm tra là ngoại tệ hay Việt Nam Đồng
    //    if ($scope.Invoice.TOTALPAYMENTRAW && $scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
    //        money = $scope.Invoice.TOTALPAYMENTRAW;
    //    }
    //    else {
    //        money = $scope.Invoice.TOTALPAYMENT;
    //    }
    //    // End
    //    var action = url + 'ReadNumberToWords';
    //    var datasend = JSON.stringify({
    //        currency: $scope.Invoice.CURRENCY,
    //        number: money,
    //    });
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $scope.NumberToCurrencyWords = response.data;
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ReadNumberToCurrencyWords');
    //        }
    //    });
    //}

    $scope.ReadNumberToCurrencyWords = function () {
        var money = 0;
        // Kiểm tra là ngoại tệ hay Việt Nam Đồng
        if ($scope.Invoice.TOTALPAYMENTRAW && $scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
            money = $scope.Invoice.TOTALPAYMENTRAW;
        }
        else {
            money = $scope.Invoice.TOTALPAYMENT;
        }
        // End
        var action = url + 'ReadNumberToWords';
        var datasend = JSON.stringify({
            currency: $scope.Invoice.CURRENCY,
            number: money,
            numberPlace: $rootScope.Enterprise.MONEYPLACE
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.NumberToCurrencyWords = response.data;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ReadNumberToCurrencyWords');
            }
        });
    }

    $scope.SuggestCustomer = function () {
        var obj = $rootScope.selectedCustomer;
        if (obj) {
            $scope.Invoice.CUSID = obj.CUSID;
            $scope.Invoice.CUSNAME = obj.CUSNAME;
            $scope.Invoice.CUSADDRESS = obj.CUSADDRESS;
            $scope.Invoice.CUSTAXCODE = obj.CUSTAXCODE;
            $scope.Invoice.CUSEMAIL = obj.CUSEMAIL;
            $scope.Invoice.CUSPHONENUMBER = obj.CUSPHONENUMBER;
            $scope.Invoice.CUSBUYER = obj.CUSBUYER;
            $scope.Invoice.CUSTOMERCODE = obj.CUSID;
            $scope.Invoice.CUSACCOUNTNUMBER = obj.CUSACCOUNTNUMBER;
            $scope.GetMeterByCustaxcode(obj.CUSID);
        }
    }

    $scope.GetInvoice = function () {
        LoadingShow();
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    $scope.GetInvoiceById = function (id) {
        var action = url + 'GetInvoiceById';
        var datasend = JSON.stringify({
            id: id
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.RefInvoice = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceById');
            }
        });
    }

    $scope.SelectAll = function () {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListInvoice.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.LoadInfoByTaxcode2 = function () {
        if (!$scope.Invoice.CUSTAXCODE) {
            alert('Bạn cần nhập MST');
            return false;
        }
        LoadingShow();
        if ($scope.Invoice.CUSTAXCODE && $scope.Invoice.CUSTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/other/getcompanyinfobytaxcode?taxcode=' + $scope.Invoice.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                $timeout(function () {
                    var rawObj = JSON.parse(response);
                    var obj = new Object();
                    if (rawObj.Data != "") {
                        obj = JSON.parse(rawObj.Data);
                    }
                    if (obj.companyName) {
                        $scope.Invoice.CUSADDRESS = obj.address;
                        $scope.Invoice.CUSNAME = obj.companyName;
                    } else {
                        toastr.warning("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Invoice.CUSNAME = null;
                        $scope.Invoice.CUSADDRESS = null;
                    }
                    LoadingHide();
                }, 2000);
            });
        }
    }

    $scope.LoadInfoByTaxcode = function () {
        //LoadingShow();
        //var action = url + 'LoadInfoByTaxcode';
        //var datasend = JSON.stringify({
        //    taxcode: $scope.Invoice.CUSTAXCODE
        //});
        //CommonFactory.PostDataAjax(action, datasend, function (response) {
        //    if (response) {
        //        if (response.rs) {
        //            if (response.data) {
        //                $scope.Invoice.CUSADDRESS = response.data.Ho_Address;
        //                $scope.Invoice.CUSNAME = response.data.Name;
        //            }
        //            else {
        //                $scope.Invoice.CUSNAME = null;
        //                $scope.Invoice.CUSADDRESS = null;
        //                $scope.LoadInfoByTaxcode2();
        //            }
        //        }
        //        else {
        //            $scope.LoadInfoByTaxcode2();
        //        }
        //    } else {
        //        //alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LoadInfoByTaxcode');
        //        $scope.LoadInfoByTaxcode2();
        //    }
        //    LoadingHide();
        //}, 5000);


        $scope.LoadInfoByTaxcode2();
    }

    $scope.LoadCurrencyExchangeRate = function () { 
        var action = url + 'LoadCurrency';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                var lstCurrency = [];
                lstCurrency = response.items === undefined ? [] : response.items;
                lstCurrency.push({ type: 'VND', imageurl: '', muatienmat: 1, muack: 1, bantienmat: 1, banck: 1 });
                lstCurrency = lstCurrency.filter(function (x) {
                    return (x.type != "XAU");
                }).filter(function (y) {
                    return y.type != "PNJ_DAB";
                });
                lstCurrency.forEach(function (z) {
                    if (z.type == "JPY") {
                        z.bantienmat = z.bantienmat.split('.')[0];
                    }
                })
                $scope.CurrencyList = lstCurrency;
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LoadCurrencyExchangeRate');
            }
        });
    }

    $scope.ChangeCurrencyType = function (currencyType) {
        var exchangeRateObj = $scope.CurrencyList.filter(function (x) {
            return x.type === currencyType;
        })
        $scope.Invoice.EXCHANGERATE = exchangeRateObj[0].bantienmat.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $scope.CalMoneyAfterChangeValue();
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $scope.DiscountType = [
        {
            value: "KHONG_CO_CHIET_KHAU", name: "Không có chiết khấu"
        },
        {
            value: "CHIET_KHAU_THEO_HANG_HOA", name: "Chiết khấu theo hàng hóa"
        }
    ];


    $scope.TaxRateList = [
        {
            value: -1, name: "Không chịu thuế"
        },
        {
            value: 0, name: "0%"
        },
        {
            value: 5, name: "5%"
        },
        {
            value: 10, name: "10%"
        }
    ];

    $scope.Hoa_Don_Ban_Hang_TaxRateList = [
        {
            value: -1, name: "Hàng hóa, dịch vụ không chịu thuế GTGT hoặc thuế GTGT 0%"
        },
        {
            value: 1, name: "Phân phối, cung cấp hàng hoá (1%)"
        },
        {
            value: 5, name: "Dịch vụ, xây dựng không bao thầu nguyên vật liệu (5%)"
        },
        {
            value: 3, name: "Sản xuất, vận tải, dịch vụ có gắn với hàng hoá, xây dựng có bao thầu nguyên vật liệu (3%)"
        },
        {
            value: 2, name: "Hoạt động kinh doanh khác (2%)"
        }
    ];

    $scope.ViewTaxcode = function (taxcode) { 
        if (!taxcode)
            return;
        var number_regex = new RegExp(/\d/, "gi");
        var symbol_regex = new RegExp(/\S/, "gi");
        taxcode = taxcode.replace(symbol_regex, function (matched) {
            if (matched.match(number_regex))
                return "<span class=\"" + "taxcode-symbol" + "\">" + matched + "</span>";
            else
                return "<span class=\"" + "taxcode-space" + "\">" + matched + "</span>";
        });

        return $sce.trustAsHtml(taxcode);
    }

    $scope.ChangeDiscountType = function () {
        if ($scope.Invoice.DISCOUNTTYPE == 'KHONG_CO_CHIET_KHAU') {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                obj.DISCOUNTRATE = 0;
            });
        }
        else {

        }
    }

    $scope.ModalInvoiceNote = function () {
        $('#modal_invoice_note').dialog({
            modal: true,
            width: '35%'
        });

        if ($scope.Invoice.ID == 0) {
            $scope.Invoice.NOTE = '';
        }
    }

    $scope.GetNumber = function () {
        $scope.FormNumber = new Object();

        $scope.FormNumber.FORMCODE = $scope.Number.FORMCODE;
        $scope.FormNumber.SYMBOLCODE = $scope.Number.SYMBOLCODE;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.result[0];
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER + 1;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumberaa');
            }
            LoadingHide();
        });
    }

    $scope.ExchangeRateFormat = function () {
        $("#exchangeRateInput").on('keyup', function () {
            if ($(this).val() != 'NaN') {
                var n = parseInt($(this).val().replace(/\D/g, ''), 10);
                $(this).val(n.toLocaleString().replace('.', ','));
            }
            else {
                $(this).val(1);
            }
        })
    }

    $scope.FormCodeChange = function (formcode) {
        $rootScope.ListFormCode = $scope.TempListFormCode;
        $timeout(function () {
            if ($rootScope.ListFormCode !== undefined) {
                let tmp = formcode.split('-');
                let formCodeSymbolCode = $rootScope.ListFormCode.filter(function (x) {
                    return x.FORMCODE === tmp[0] && x.SYMBOLCODE === tmp[1];
                });

                $scope.GLOBALTAXRATE = 0;
                $scope.TAXRATE = formCodeSymbolCode[0].TAXRATE;
                $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
            }
        }, 100)
    }

    /**
     * truongnv 16.3.2020
     * Kiểm tra loại hóa đơn điều chỉnh để cho phép người dùng sửa thông tin
     * @param {any} value
     */
    $scope.ChangeInvoiceMethod = function (value) {
        var $divContainer = $('#product-container');
        //if ($divContainer && value === 3)
        //    $divContainer.attr('style', 'pointer-events:none;');
        //else
        $divContainer.attr('style', 'pointer-events:auto;');
    }

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $scope.GetMeterByCustaxcode = function (custaxcode) {
        if (custaxcode === undefined) return;
        LoadingShow();
        var action = '/Meter/GetMeterListByCustaxcode';
        var datasend = JSON.stringify({
            custaxcode: custaxcode
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.MeterList = response.msg;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetMeterByCustaxcode');
            }
        });
        LoadingHide();
    }

    $scope.GetMeterByInvoiceId = function (invoiceid) {
        var action = '/Meter/GetListMeterCodeByInvoiceID';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs && response.msg.length > 0) {
                    $scope.MeterList = response.msg;
                    setSelectedValueDropdown($rootScope.Enterprise.USINGINVOICETYPE, $scope.MeterList[0].METERCODE);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetMeterByInvoiceId');
            }
        });
    }

    /**
     * truongnv 20200318
     * Thêm dòng dữ liệu khi chọn khu vực
     * @param {any} value
     */
    $scope.ChooseMeter = function () {
        //// lấy thông tin hàng hóa, dịch vụ
        //var $div = $('#customer_name');
        ////var meterCode = item.METERNAME;
        ////if (index === 4)
        ////    meterCode = $('#ddlMeter2').val();

        //var isCreateRow = true;
        ////Kiểm tra xem đã tồn tại Bộ công tơ chưa
        //$('.item_customer_name .innertext-cusname').each(function (index, item) {
        //    var code = item.getAttribute('rel');
        //    if (code === meterCode) {
        //        isCreateRow = false;
        //        return isCreateRow;
        //    }
        //});

        //if ($scope.MeterList != null) {
        //    var objM = $scope.MeterList.filter(x => x.CODE === meterCode);
        //    if (objM && Object.keys(objM).length > 0)
        //        metername = objM[0].METERNAME;

        //    if (isCreateRow) {
        //        if ($div.text() === '[Số công tơ]') {
        //            $div.attr('rel', meterCode);
        //            $div.text(metername);
        //        }
        //        else {
        //            $('#ITEM_HOADONTIENDIEN tbody tr:last').after("<tr style='background-color: #dee2e6;'><td class='item_customer_name'><strong id='customer_name' class='innertext-cusname' rel='" + meterCode + "'>" + metername + "</strong></td><td colspan='7'></td></tr >");
        //        }

        var action = '/Product/GetProductListByMeterCode';
        var datasend = JSON.stringify({
            meterCode: $scope.Invoice.METER.CODE,
            custaxcode: $scope.Invoice.CUSID
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {

                    for (var i = 0; i < response.msg.length; i++) {
                        $scope.AddDataRow(response.msg[i]);
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddDataRow');
            }
        });
        //    }
        //}
    }

    /**
     * truongnv 20200318
     * Tạo thêm dòng dữ liệu
     * @param {any} data
     */
    $scope.AddDataRow = function (data) {
        data["PRICE"] = data["PRICE"] === undefined ? 0 : data["PRICE"];
        data["RETAILPRICE"] = data["RETAILPRICE"] === undefined ? 0 : data["RETAILPRICE"];
        data["ITEMMONEY"] = data["ITEMMONEY"] === undefined ? 0 : data["ITEMMONEY"];
        data["QUANTITY"] = data["QUANTITY"] === undefined ? 0 : data["QUANTITY"];

        data["OLDNO"] = data["OLDNO"] === undefined ? 0 : data["OLDNO"];
        data["NEWNO"] = data["NEWNO"] === undefined ? 0 : data["NEWNO"];
        data["FACTOR"] = data["FACTOR"] === undefined ? 0 : data["FACTOR"];

        var obj = { METERCODE: data["METERCODE"], PRODUCTNAME: data["PRODUCTNAME"], OLDNO: data["OLDNO"], NEWNO: data["NEWNO"], FACTOR: data["FACTOR"], QUANTITY: 1, TAXRATE: -1, RETAILPRICE: data["PRICE"], METERNAME: data["METERNAME"] }
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push(obj);
    }

    /**
     * Tính Định mức tiêu thụ điện và thành tiền
     * @param {any} item
     */
    $scope.ElectricInvoiceQuantityChange = function (item) {
        item.OLDNO = item.OLDNO.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        item.NEWNO = item.NEWNO.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        // tính định mức tiêu thụ
        var quantity = (item.NEWNO - item.OLDNO) * item.FACTOR;
        item.QUANTITY = quantity.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        //tính thành tiền
        item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $scope.CalMoneyAfterChangeValue();
    }

    $scope.SetDatepikerDate = function () {
        var toDate = $scope.Invoice.TODATE;
        $scope.Invoice.TODATESTR = toDate;

        var fromDate = $scope.Invoice.FROMDATE;
        $scope.Invoice.FROMDATESTR = fromDate;

        var deliveryOrderDate = $scope.Invoice.DELIVERYORDERDATE;
        $scope.Invoice.DELIVERYORDERDATESTR = deliveryOrderDate;
    }
}]);

function setSelectedValueDropdown(type, value) {
    if (type === 2) {
        var $div = $("#ddlMeter option:first");
        if ($div && $div.length > 0) {
            $div.html(value);
            $div.attr('value', value);
        }
    } else if (type === 4) {
        var $div = $("#ddlMeter2 option:first");
        if ($div && $div.length > 0) {
            $div.html(value);
            $div.attr('value', value);
        }
    }
}

function xoa_dau(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    return str;
}


function CheckDate(strCheck, field, fieldLength, isNotBlank) {
    var oReturn = {
        messErrorForCustomer: '',
        messErrorForCoder: '',
        result: true
    };
    if (strCheck === undefined || strCheck.length == 0)//Có được phép để trống trường này hay không
    {
        if (isNotBlank)//Có được phép để trống trường này hay không
        {
            oReturn.result = false;
            oReturn.messErrorForCustomer = field + " không được để trống";
            oReturn.messErrorForCoder = field + " không được để trống";
        }
    }
    else {
        if (strCheck.indexOf("-") != -1)
            strCheck.replace("-", "/");
        var date = strCheck.split('/');
        if (parseInt(date[0]) > 31 || parseInt(date[0]) < 0 || parseInt(date[1]) > 12 || parseInt(date[1]) < 0 || (date[1]).length > 2 || (date[0]).length > 2 || (date[2]).length > 4 || (date[1]).length < 0 || (date[0]).length < 0 || (date[2]).length < 0) {
            oReturn.result = false;
            oReturn.messErrorForCustomer = "Ngày không đúng định dạng dd/mm/yyyy";
            oReturn.messErrorForCoder = "Ngày không đúng định dạng dd/mm/yyyy";
        }
    }

    return oReturn;
}

function compareDates(valueTuThangNam, valueDenThangNam, messageName) {
    var oReturn = {
        messErrorForCustomer: '',
        messErrorForCoder: '',
        result: true
    };

    var arrDenThangNam = valueDenThangNam.split('/');
    var arrTuThangNam = valueTuThangNam.split('/');
    var DenThangNam = new Date(arrDenThangNam[2] + '-' + arrDenThangNam[1] + '-' + arrDenThangNam[0]);
    var TuThangNam = new Date(arrTuThangNam[2] + '-' + arrTuThangNam[1] + '-' + arrTuThangNam[0]);
    var dateTime1 = new Date(TuThangNam).getTime(),
        dateTime2 = new Date(DenThangNam).getTime();
    var diff = dateTime2 - dateTime1;
    if (diff < 0) {
        oReturn.result = false;
        oReturn.messErrorForCustomer = messageName;
        oReturn.messErrorForCoder = messageName;
    }
    return oReturn;
}

function FormatDateTimeFromAjax(dateAjax, strFormat) {
    var date = new Date(parseInt(dateAjax.replace(/(^.*\()|([+-].*$)/g, '')));
    return FormatDateTime(date, strFormat);
}
function FormatDateTime(date, strFormat) {
    var dd = date.getDate();
    var MM = date.getMonth() + 1; //January is 0!
    var HH = date.getHours();
    var mm = date.getMinutes();
    var ss = date.getSeconds();

    return strFormat.replace("dd", dd < 10 ? '0' + dd : dd)
        .replace("MM", MM < 10 ? '0' + MM : MM)
        .replace("yyyy", date.getFullYear())
        .replace("HH", HH < 10 ? '0' + HH : HH)
        .replace("mm", mm < 10 ? '0' + mm : mm)
        .replace("ss", ss < 10 ? '0' + ss : ss);
}

function ConvertDateShowToDate(strDate) {
    var arrDate = strDate.split('/');
    if (arrDate.length == 3) {
        var year = Number(arrDate[2]);
        var month = Number(arrDate[1]);
        var day = Number(arrDate[0]);
        return year + "-" + month + "-" + day;
    }
}

function AddDaysToDate(date, days) {
    var result = new Date(date).getTime() + days * 24 * 3600000;
    return new Date(result);
}

function formatMoney(number, decPlaces, decSep, thouSep) {
    decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
        decSep = typeof decSep === "undefined" ? "." : decSep;
    thouSep = typeof thouSep === "undefined" ? "," : thouSep;
    var sign = number < 0 ? "-" : "";
    var i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(decPlaces)));
    var j = (j = i.length) > 3 ? j % 3 : 0;

    return sign +
        (j ? i.substr(0, j) + thouSep : "") +
        i.substr(j).replace(/(\decSep{3})(?=\decSep)/g, "$1" + thouSep) +
        (decPlaces ? decSep + Math.abs(number - i).toFixed(decPlaces).slice(2) : "");
}

//custom for ES6
function formatMoney(amount, decimalCount = 2, decimal = ".", thousands = ",") {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;

        return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
        console.log(e)
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
    LoadingShow();
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

$('.modal-invoice').on('hidden.bs.modal', function (e) {
    // do something...
    alert("logs");
})