app.controller('ModalChangeInvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';

    $rootScope.ModalChangeInvoice = function (item, type) {
        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push(new Object());
        if (type == 5) {
            angular.copy(item, $scope.ChangeInvoice);
            $scope.GetInvoiceDetail(item.ID);
            $scope.ChangeInvoice.ID = 0;
        }
    }

    $scope.GetInvoiceDetail = function (invoiceid) {
        $scope.IsLoading = true;
        var action = url + 'GetInvoiceDetail';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.LISTPRODUCT = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceDetail');
            }
            LoadingHide();
        });
    }

}]);