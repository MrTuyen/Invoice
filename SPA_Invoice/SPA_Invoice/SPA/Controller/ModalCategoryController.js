app.controller('ModalCategoryController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Category/';
    $scope.category = new Object();
    $rootScope.ModalCategory = function (item) {
        angular.copy(item, $scope.category);
        if (item === "") {
            $scope.category.ISACTIVE = true;
        }
    }
    //thêm danh mục
    $scope.SaveCategory = function () {
        if (!$scope.category.CATEGORY) {
            alert('Vui lòng nhập tên dịch vụ');
            return;
        }
        if ($scope.category.CATEGORY.length >= 512) {
            toastr.warning('Tên danh mục độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SaveCategory';
        var datasend = JSON.stringify({
            category: $scope.category
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-category').modal('hide');
                    $rootScope.ReloadCategory(1);
                    $rootScope.GetCategory();
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCategory');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdateCategory = function (item) {
        let msg = "";
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveCategory';
        if (item.ISACTIVE == true) {
            item.ISACTIVE = false;
            msg = 'Ngừng kích hoạt thành công';
        } else {
            item.ISACTIVE = true;
            msg = 'Kích hoạt thành công';
        } 
        var datasend = JSON.stringify({
            category: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadCategory(1);
                    $rootScope.GetCategory();
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCategory');
            }
        });
    }
}]);