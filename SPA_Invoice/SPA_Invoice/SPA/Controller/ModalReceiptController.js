app.controller('ModalReceiptController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$http', '$location', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $http, $location) {
    var url = '/Receipt/';
    //type: 1: tạo mới, 2: xem chi tiết, 3: hủy bỏ, 4: chuyển đổi, 5: điều chỉnh, 6: thay thế, 7: dải chờ
    $rootScope.ModalReceipt = function (item, type, formcode, symbolcode, isCopy) {
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetPaymentMethod();

        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = [];
        $scope.Invoice.INVOICETYPE = type;
        $scope.IsCopy = false;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
        $scope.Invoice.CURRENCY = "VND";
        $scope.Invoice.EXCHANGERATE = 1;

        $('.modal-receipt').modal('show');
        if (type == 1) {
            $scope.Invoice.ID = 0;
            $scope.Invoice.CUSPAYMENTMETHOD = "TM/CK";
            $timeout(function () {
                $scope.Invoice.FORMCODE = $rootScope.ListFormCode[0].FORMCODE;
                $scope.Invoice.SYMBOLCODE = $rootScope.ListSymbolCode[0].SYMBOLCODE;
                $scope.TAXRATE = $rootScope.ListSymbolCode[0].TAXRATE;
                $scope.Invoice.DISCOUNTTYPE = "KHONG_CO_CHIET_KHAU";
            });
            if (item) {
                angular.copy(item, $scope.Invoice);
                if (isCopy) {
                    $scope.Invoice.INVOICESTATUS = 1;
                    $scope.Invoice.NUMBER = 0;
                    $scope.Invoice.ID = 0;
                    $scope.Invoice.INVOICETYPE = type;
                    $scope.Invoice.REFERENCE = 0;
                    $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $scope.IsCopy = isCopy;
                }
            }
            else
                $scope.Invoice.NUMBER = 0;

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
            var taxrate = $rootScope.ListSymbolCode.filter(function (x) {
                return x.SYMBOLCODE == item.SYMBOLCODE;
            })
            $scope.TAXRATE = taxrate[0].TAXRATE;
            item.EXCHANGERATE = item.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            if (item.INVOICETYPE == 5 || item.INVOICETYPE == 3 || item.INVOICETYPE == 6) {
                item.SIGNEDTIME = item.SIGNEDTIMETEMP;
            }
            angular.copy(item, $scope.Invoice);

        } else if (type == 5) {
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;

            $scope.Invoice.LISTPRODUCT = [];
        } else if (type == 6) {
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;
        } else if (type == 7) {
        }
        $scope.Invoice.STRDUEDATE = $filter('dateFormat')($scope.Invoice.DUEDATE, 'dd/MM/yyyy');
        $scope.Invoice.STRINVOICEWAITINGTIME = $filter('dateFormat')($scope.Invoice.INVOICEWAITINGTIME, 'dd/MM/yyyy');
        ReadNumber($scope.Invoice.TOTALPAYMENT);
    }

    $scope.AddReceipt = function (type) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Bạn cần chọn mẫu số.');
            return false;
        }

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Bạn cần chọn ký hiệu.');
            return false;
        }
        
        if (!$scope.Invoice.CUSID) {
            alert('Bạn cần nhập vào thông tin <b>Mã học sinh</b>.');
            return false;
        }

        if (!$scope.Invoice.NOTE) {
            alert('Bạn cần nhập vào thông tin <b>Ghi chú</b>.');
            return false;
        }

        if (!$scope.Invoice.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email');
                return;
            }
        }

        LoadingShow();
        var action = url + 'AddReceipt';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-receipt').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
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

    $scope.UpdateReceipt = function (type) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#symbol-code').focus();
            return false;
        }

        if (!$scope.Invoice.CUSID) {
            alert('Bạn cần nhập vào thông tin <b>Mã học sinh</b>.');
            return false;
        }

        if (!$scope.Invoice.NOTE) {
            alert('Bạn cần nhập vào thông tin <b>Ghi chú</b>.');
            return false;
        }

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

        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '');/*.replace(/\./g, '')*/;

        LoadingShow();
        var action = url + 'UpdateReceipt';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-receipt').modal('hide');
                    $rootScope.ReloadReceipt();
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.SuggestCustomer = function () {
        var obj = $rootScope.selectedCustomer;
        if (obj) {
            $scope.Invoice.CUSNAME = obj.CUSNAME;
            $scope.Invoice.CUSADDRESS = obj.CUSADDRESS;
            $scope.Invoice.CUSTAXCODE = obj.CUSTAXCODE;
            $scope.Invoice.CUSID = obj.CUSID;
            $scope.Invoice.CUSEMAIL = obj.CUSEMAIL;
            $scope.Invoice.CUSBUYER = obj.CUSBUYER;
        }
    }

    $scope.ReadNumberToCurrencyWords = function () {
        var moneyQty = 0;
        // Kiểm tra là ngoại tệ hay Việt Nam Đồng
        if ($scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
            moneyQty = $scope.Invoice.TOTALPAYMENTRAW;
        }
        else {
            moneyQty = $scope.Invoice.TOTALPAYMENT;
        }
        // End
        var action = url + 'ReadNumberToWords';
        var datasend = JSON.stringify({
            kindOfMoney: $scope.Invoice.CURRENCY,
            number: moneyQty
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

    $scope.oldTaxcode = null;

    $scope.LoadInfoByTaxcode = function () {

        if ($scope.Invoice.CUSTAXCODE != $scope.oldTaxcode && $scope.Invoice.CUSTAXCODE != null && $scope.Invoice.CUSTAXCODE != "") {
            var url = 'https://k8s.misa.com.vn/org-info/suggest?taxCode=' + $scope.Invoice.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                var obj = JSON.parse(response);
                $timeout(function () {
                    if (obj.data.companyName) {
                        $scope.Invoice.CUSNAME = obj.data.companyName;
                        $scope.Invoice.CUSADDRESS = obj.data.address;
                        //if (!$scope.Invoice.CUSBUYER)
                        //    $scope.Invoice.CUSBUYER = obj.data.owner;
                    } else {
                        alert("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Invoice.CUSNAME = null;
                        $scope.Invoice.CUSADDRESS = null;
                    }
                });
            });

            //var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $scope.Invoice.CUSTAXCODE;
            //$("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
            //    var rawObj = JSON.parse(response);
            //    var obj = new Object();
            //    if (rawObj.Data != "") {
            //        obj = JSON.parse(rawObj.Data);
            //    }
            //    $timeout(function () {
            //        if (obj.companyName) {
            //            let fullAddrress = obj.address;
            //            fullAddrress = (obj.ward != "" ? fullAddrress + ', ' + obj.ward : fullAddrress);
            //            fullAddrress = (obj.district != "" ? fullAddrress + ', ' + obj.district : fullAddrress);
            //            fullAddrress = (obj.province != "" ? fullAddrress + ', ' + obj.province : fullAddrress);

            //            $scope.Invoice.CUSADDRESS = fullAddrress + ', Việt Nam';
            //            $scope.Invoice.CUSNAME = obj.companyName.toUpperCase();
            //        } else {
            //            alert("Không tim thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
            //            $scope.Invoice.CUSNAME = null;
            //            $scope.Invoice.CUSADDRESS = null;
            //        }
            //    });
            //});



            //var action = url + 'LoadInfoByComTaxCode';
            //var datasend = JSON.stringify({
            //    taxcode: $scope.Invoice.CUSTAXCODE
            //});
            //LoadingShow();
            //CommonFactory.PostDataAjax(action, datasend, function (response) {
            //    $timeout(function () {
            //        if (response.success) {
            //            $scope.Invoice.CUSADDRESS = response.data.ComAddress;
            //            $scope.Invoice.CUSNAME = response.data.ComName;
            //        } else {
            //            alert("Không tim thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
            //            $scope.Invoice.CUSNAME = null;
            //            $scope.Invoice.CUSADDRESS = null;
            //        }
            //    })
            //    LoadingHide();
            //});

        } else if ($scope.Invoice.CUSTAXCODE != $scope.oldTaxcode) {
            $scope.Invoice.CUSNAME = null;
            $scope.Invoice.CUSADDRESS = null;
        }
    }

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

    $scope.SymbolCodeChange = function (symbolcode) {
        $scope.GLOBALTAXRATE = 0;
        $scope.TAXRATE = 0;
        $scope.CalMoneyAfterChangeValue();
        var listSymbolCode = $rootScope.ListSymbolCode;
        var symbolObj = listSymbolCode.filter(function (x) {
            return x.SYMBOLCODE === symbolcode;
        });
        $scope.TAXRATE = symbolObj[0].TAXRATE;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
    }

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

}]);

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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Validate Input
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function InputLimiter(e, allow) {
    var AllowableCharacters = '';
    if (allow == 'Letters') { AllowableCharacters = ' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'; }
    else if (allow == 'Numbers') { AllowableCharacters = '1234567890'; }
    else if (allow == 'NameCharacters') { AllowableCharacters = ' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.\''; }
    else if (allow == 'NameCharactersAndNumbers') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-\''; }
    else if (allow == '09az') { AllowableCharacters = '1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'; }
    else AllowableCharacters = allow;

    var k = document.all ? parseInt(e.keyCode) : parseInt(e.which);
    if (k != 13 && k != 8 && k != 0) {
        if ((e.ctrlKey == false) && (e.altKey == false)) {
            var ok = (AllowableCharacters.indexOf(String.fromCharCode(k)) != -1);
            if (!ok) Beep();
            return ok;
        } else {
            return true;
        }
    } else {
        return true;
    }
}

function ReadNumber(value) {
    var action = '/Receipt/ReadNumberToWords';
    var datasend = JSON.stringify({
        number: parseFloat(value),
        kindOfMoney: 'VND'
    });
    $.ajax({
        type: 'POST',
        url: action,
        contentType: "application/json; charset=utf-8;",
        dataType: "json",
        processData: true,
        data: datasend,
        success: function (result) {
            $("#AmountInWords").html(result.data);
        },
        error: function (xhr, status, p3, p4) {
            LoadingHide();
        }
    });
}

function UpdateFormat(id, value) {
    if (value === undefined) {
        value = ParseVietnameseNumber($(id).val());
        if (value === false) value = $(id).val();
    }
    value = FormatVietnameseNumber(value);

    if (value !== false) $(id).val(value);

    ReadNumber(ParseVietnameseNumber($(id).val()));
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
    else return num.formatMoney(0, ',', '.');
}

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

    return value;
}

function Beep() {
    var snd = new Audio("data:audio/wav;base64,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qJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU=");
    snd.play();
}
