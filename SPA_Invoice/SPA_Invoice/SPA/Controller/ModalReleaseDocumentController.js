app.controller('ModalReleaseDocumentController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $rootScope.ModalReleaseDocumentReport = function (item) {
        $scope.Invoice = new Object();
        $timeout(function () {
            angular.copy(item, $scope.Invoice);
            angular.copy(item1, $scope.Report);
            //if (item.IDTEMP) {
            //    $scope.Invoice.ID = item.IDTEMP;
            //}
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === 2) {
            $scope.Report.REPORTTYPE = '2';
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
            alert("Vui lòng nhập tên cơ quan thuế tiếp nhận thông báo!");
            return false;
        }

        var action = url + 'AddReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    $rootScope.ModalInvoice($scope.Invoice, 5);
                    $('.modal-invoice').modal('show');

                    $('.modal-modified-report').modal('hide');
                    $rootScope.ReloadInvoice();
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
            alert("Vui lòng nhập tên cơ quan thuế tiếp nhận thông báo!");
            return false;
        }

        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!')
                    $('.modal-modified-report').modal('hide');
                    $rootScope.ReloadInvoice();
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
}]);