app.controller('ModalCancelReportController', ['$scope', '$rootScope', '$timeout', '$location', 'CommonFactory', '$filter', function ($scope, $rootScope, $timeout, $location, CommonFactory, $filter) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = String(now.getDate()).padStart(2, '0');
    $scope.CurrentMonth = String(now.getMonth() + 1).padStart(2, '0'); //January is 0!
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = $scope.CurrentDay + "/" + $scope.CurrentMonth + "/" + $scope.CurrentYear;

    $rootScope.ModalCancelReport = function (item1, item) {
        $scope.Invoice = new Object();
        $scope.Report = new Object();
        $timeout(function () {
            angular.copy(item1, $scope.Report);
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.STRCANCELTIME = $filter('dateFormat')($scope.Invoice.CANCELTIME, 'dd/MM/yyyy');
            if ($scope.Invoice.CANCELTIME === "/Date(-62135596800000)/") {
                $scope.Invoice.STRCANCELTIME = $scope.CurrentDate;
            }
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === REPORT_TYPE.Cancel) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Cancel;
        }

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do hủy!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }

        var action = url + 'AddReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CANCELREASON = $scope.Report.REASON
                    $rootScope.ModalCancelInvoice($scope.Invoice);
                    toastr.success(response.msg);
                    if ($scope.Invoice.INVOICETYPE != 3) {
                        $('.modal-cancel-invoice').modal('show')
                    }
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }

    $scope.UpdateReport = function () {
        $scope.Report.REPORTTYPE = $scope.Invoice.ISEXISTCANCELREPORT;
        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do hủy!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }

        LoadingShow();
        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateCancelReport');
            }
            LoadingHide();
        });
    }

    /*
     * Lưu và ký biên bản hủy hóa đơn 
     * truongnv 20200218
     * @param {any} type: Loại biên bản
     */
    $scope.SaveAndSignDocumentPDF = function (type) {
        var msg = "Vui lòng nhập lý do hủy.";
        if (type === REPORT_TYPE.Cancel) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Cancel;
        }
        else
            msg = "Vui lòng nhập lý do điều chỉnh.";

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert(msg);
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }
        var action = url + 'SaveAndSignDocumentPDF';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CANCELREASON = $scope.Report.REASON
                    $rootScope.ModalCancelInvoice($scope.Invoice);
                    toastr.success(response.msg);
                    if ($scope.Invoice.INVOICETYPE != 3) {
                        $('.modal-cancel-invoice').modal('show')
                    }
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }
}]);