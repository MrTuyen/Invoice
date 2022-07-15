app.controller('ModalCurrencyUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/CurrencyUnit/';
    $scope.currency = new Object();
    $rootScope.ModalCurrency = function (item) {
        angular.copy(item, $scope.currency);
        if (item === "") {
            $scope.currency.ISACTIVED = true;
        }
    }
    //thêm tiền thanh toán
    $scope.AddCurrencyUnit = function () {
        if (!$scope.currency.CURRENCYUNIT) {
            alert('Vui lòng nhập vào tên tiền thanh toán');
            return false;
            $('#currency').focus();
        }
        if ($scope.currency.CURRENCYUNIT.length >= 512) {
            toastr.warning('Tên tiền thanh toán độ dài không quá 512 ký tự');
            return false;
        }
        $scope.IsLoading = true;
        var action = url + 'SaveCurrencyUnit';
        var datasend = JSON.stringify({
            currency: $scope.currency
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-currencyunit').modal('hide');
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCurrencyUnit');
            }
        });
        $scope.IsLoading = false;
    }
  //cập nhật trạng thái
    $rootScope.UpdateCurencyUnit = function (item) {
        let msg = '';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveCurrencyUnit';
        if (item.ISACTIVED == true) {
            item.ISACTIVED = false;
            msg = 'Ngừng kích hoạt thành công';
        } else {
            item.ISACTIVED = true;
            msg = 'Kích hoạt thành công';
        }
        var datasend = JSON.stringify({
            currency: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                   toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCurrencyUnit');
            }
        });
    }
}]);