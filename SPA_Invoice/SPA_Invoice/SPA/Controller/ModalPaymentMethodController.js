app.controller('ModalPaymentMethodController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/PaymentMethod/';
    $scope.payment = new Object();
    $rootScope.ModalPayment = function (item) {
        angular.copy(item, $scope.payment);
        if (item === "") {
            $scope.payment.ISACTIVED = true;
        }
    }
    //thêm HTTT
    $scope.SaveAndClosePayment = function () {
        if (!$scope.payment.PAYMENTMETHOD) {
            alert('Vui lòng nhập vào tên hình thức thanh toán');
            return false;
            $('#payment').focus();
        }
        if ($scope.payment.PAYMENTMETHOD.length >= 512) {
            toastr.warning('Tên HTTT độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SavePaymentMethod';
        var datasend = JSON.stringify({
            payment: $scope.payment
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-payment').modal('hide');
                    $rootScope.ReloadPayment(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SavePaymentMethod');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdatePaymentMethod = function (item) {
        let msg = '';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        } 
        else {
            var action = url + 'SavePaymentMethod';
            if (item.ISACTIVED == true) {
                item.ISACTIVED = false;
                msg = 'Ngừng kích hoạt thành công';
            } else  {
                item.ISACTIVED = true;
                msg = 'Kích hoạt thành công';
            }
            var datasend = JSON.stringify({
                payment: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        toastr.success(msg);
                        $rootScope.ReloadPayment(1);
                    } else {
                       toastr.warning(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SavePaymentMethod');
                }
            });
        }
    }
}]);