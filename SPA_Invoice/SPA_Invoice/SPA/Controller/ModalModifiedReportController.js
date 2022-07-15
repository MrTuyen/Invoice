app.controller('ModalModifiedReportController', ['$scope', '$rootScope', '$timeout', "$location", 'CommonFactory', '$filter', function ($scope, $rootScope, $timeout, $location, CommonFactory, $filter) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = String(now.getDate()).padStart(2, '0');
    $scope.CurrentMonth = String(now.getMonth() + 1).padStart(2, '0'); //January is 0!
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = $scope.CurrentDay + "/" + $scope.CurrentMonth + "/" + $scope.CurrentYear;

    $rootScope.ModalModifiedReport = function (item1, item) {
        $scope.Invoice = new Object();
        $scope.Report = new Object();
        $timeout(function () {
            angular.copy(item, $scope.Invoice);
            angular.copy(item1, $scope.Report);
            //if (item.IDTEMP) {
            //    $scope.Invoice.ID = item.IDTEMP;
            //}
            $scope.Report.REPORTTIME = $scope.CurrentDate;
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === REPORT_TYPE.Change) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Change;
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
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do điều chỉnh!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
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
                    toastr.success(response.msg);
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    $('.modal-modified-report').modal('hide');
                    if ($scope.Invoice.INVOICETYPE != 5) {
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ModalReceipt($scope.Invoice, 5);
                        }
                        else {
                            $rootScope.ModalInvoice($scope.Invoice, 5);
                        }
                        
                    }
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
        $scope.Report.REPORTTYPE = $scope.Invoice.ISEXISTMODIFIEDREPORT;
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
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do điều chỉnh!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
            return;
        }

        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('.modal-modified-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateModifiedReport');
                LoadingHide();
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
        var msg = "Vui lòng nhập lý do điều chỉnh.";
        if (type === REPORT_TYPE.Change) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Change;
        }
        else
            msg = "Vui lòng nhập lý do hủy.";

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
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert(msg);
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
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
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    if ($scope.Invoice.INVOICETYPE != 5) {
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ModalReceipt($scope.Invoice, 5);
                        }
                        else {
                            $rootScope.ModalInvoice($scope.Invoice, 5);
                        }
                    }
                    toastr.success(response.msg);
                    $('.modal-modified-report').modal('hide');
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