app.controller('ModalCancelInvoiceController', ['$scope', '$rootScope', '$timeout', '$location', 'CommonFactory', function ($scope, $rootScope, $timeout,$location, CommonFactory) {
    var url = '/Invoice/';

    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = dd + '/' + mm + '/' + yyyy;

    $rootScope.ModalCancelInvoice = function (item) {
        $scope.CancelInvoice = new Object();
        $timeout(function () {
            angular.copy(item, $scope.CancelInvoice);
            $scope.CancelInvoice.INVOICETYPE = 3;
            $scope.CancelInvoice.STRCANCELTIME = today;
        }, 100);
    }

    $scope.CancelInv = function () {
        var number = $scope.CancelInvoice.NUMBER;
        var formCode = $scope.CancelInvoice.FORMCODE;
        var symbolCode = $scope.CancelInvoice.SYMBOLCODE;
        if ($scope.CancelInvoice.STRCANCELTIME === undefined || $scope.CancelInvoice.STRCANCELTIME === null || $scope.CancelInvoice.STRCANCELTIME.trim() === '') {
            alert("Vui lòng nhập vào ngày hủy.");
            return;
        }
        if (!$scope.CancelInvoice.CANCELREASON || $scope.CancelInvoice.CANCELREASON == " ") {
            alert("Vui lòng nhập lý do hủy.");
            return false;
        }
        if ($scope.CancelInvoice.AttachEmail === true) {
            //if (!$scope.CancelInvoice.RECIEVERNAME) {
            //    alert('Vui nhập vào tên người nhận!');
            //    return false;
            //}
            if (!$scope.CancelInvoice.CUSEMAIL) {
                alert('Vui nhập vào email người nhận!!');
                return false;
            }
            var lstEmail = $scope.CancelInvoice.CUSEMAIL.split(',');
            for (var i = 0; i < lstEmail.length; i++) {
                if (!validation.isEmailAddress(lstEmail[i].trim())) {
                    alert('Vui lòng nhập đúng định dạng email ' + (i + 1));
                    return false;
                }
            }
        }
        var confirmContinue = function (result) {
            if (!result)
                return false;
            var action = url + 'UpdateCancelInvoice';
            var datasend = JSON.stringify({
                invoice: $scope.CancelInvoice,
                type: 0
            }); LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        var confirmContinue = function (result) {
                            if (!result) {
                                return false;
                            }
                            else {
                                if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                                    $rootScope.ModalReceipt($scope.CancelInvoice, 6);
                                    $('.modal-receipt').modal('show');
                                }
                                else {
                                    $rootScope.ModalInvoice($scope.CancelInvoice, 6);
                                    $('.modal-invoice').modal('show');
                                }
                            }
                        };
                        confirm('Xóa bỏ hóa đơn thành công. Bạn có muốn lập hóa đơn thay thế cho hóa đơn này hay không?', 'Lập hóa đơn thay thế', 'Không', 'Có', confirmContinue);


                        $('.modal-cancel-invoice').modal('hide');
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ReloadReceipt();
                        }
                        else {
                            $rootScope.ReloadInvoice();
                        }
                    }
                    else {
                        $scope.ErrorMessage = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - CancelInv');
                }
                LoadingHide();
            });
        };
        confirm('Sau khi hủy bỏ hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> thì hóa đơn này sẽ không còn giá trị sử dụng, bạn không thể khôi phục được trạng thái ban đầu của hóa đơn và bạn phải lập hóa đơn thay thế.<br /> Bạn có thực sự muốn <strong>hủy bỏ</strong> hóa đơn này không?', 'Hủy bỏ hóa đơn', 'Không', 'Đồng ý', confirmContinue);
    }
}]);