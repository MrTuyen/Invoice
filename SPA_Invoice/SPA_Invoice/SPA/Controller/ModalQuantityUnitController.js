app.controller('ModalQuantityUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/QuantityUnit/';
    $scope.quantityunit = new Object();
    $rootScope.ModalQuantityUnit = function (item) {
        angular.copy(item, $scope.quantityunit);
        if (item === "") {
            $scope.quantityunit.ISACTIVED = true;
        }
    }
    //thêm đơn vị tính
    $scope.AddQuantityUnit = function () {
        if (!$scope.quantityunit.QUANTITYUNIT) {
            alert('Vui lòng nhập vào tên đơn vị tính');
            return false;
            $('#quantityunit').focus();
        }
        if ($scope.quantityunit.QUANTITYUNIT.length >= 512) {
            toastr.warning('Tên đơn vị tính độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SaveQuantityUnit';
        var datasend = JSON.stringify({
            quantityunit: $scope.quantityunit
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-quantityunit').modal('hide');
                    $rootScope.ReloadQuantityUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveQuantityUnit');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdateQuantity = function (item) {
        let msg='';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveQuantityUnit';
            if (item.ISACTIVED == true) {
                item.ISACTIVED = false;
                msg = 'Ngừng kích hoạt thành công';
            } else {
                item.ISACTIVED = true;
                msg = 'Kích hoạt thành công';
            }
        var datasend = JSON.stringify({
            quantityunit: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadQuantityUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveQuantityUnit');
            }
        });
    }
}]);