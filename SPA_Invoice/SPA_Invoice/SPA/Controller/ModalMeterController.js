app.controller('ModalMeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Meter/';
    $rootScope.ModalMeter = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        $scope.TYPECHANGE = type;
        $scope.APARTMENTNO = $scope.ApartmentnoList[0].value;
        $scope.Meter = new Object();
        if (type == 1) {
            //tạo mới
            $scope.Meter = new Object();
            if (typeof item === "string") {
                $scope.Meter.METERNAME = item;
            }
        }
        else if (type == 2) {
            //chỉnh sửa
            angular.copy(item, $scope.Meter);
        } else if (type == 3) {
            //sao chép
            angular.copy(item, $scope.Meter);
            $scope.Meter.ID = 0
        }
    }
    $rootScope.ModalAddMeter = function (item, type) {
        $scope.GetListProducts();
        //tạo mới
        $scope.Meter = new Object();
        if (typeof item === "string") {
            $scope.Meter.METERNAME = item;
        }
        $scope.Meter.CUSTAXCODE = $rootScope.Customercode;
        //$scope.GetListProducts();
    }

    $scope.AddMeter = function (type) {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        } 
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        if (!$scope.Meter.PRODUCTCODELIST) {
            alert('Bạn chưa chọn biểu giá điện.');
            return false;
        }
        $scope.IsLoading = true;
        var action = url + 'AddMeter';
        $scope.Meter.PRODUCTCODELIST = $scope.Meter.PRODUCTCODELIST.join(',');
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (type === 1) {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Thêm mới thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else if (type==3)
                    {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Sao chép thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else {
                        $scope.Meter.METERNAME = null;
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddMeter');
            }
            LoadingHide();
        });
    }

    $scope.GetListProducts = function () {
        LoadingShow();
        var action = '/Product/GetListProductByComtaxCode';
        var datasend = JSON.stringify({
            comtaxcode: $rootScope.Enterprise.COMTAXCODE
        });

        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('.selectpicker').selectpicker({});
                    $scope.ListProducts = response.msg;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 1000);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetListProducts');
            }
        });
        LoadingHide();
    }
    $scope.UpdateMeter = function () {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        }
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        var action = url + 'UpdateMeter';
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-add-meter').modal('hide');
                    $rootScope.ReloadMeter();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateMeter');
            }
            LoadingHide();
        });
    }
    $scope.ApartmentnoList = [
        {
            value: 0, name: "Không chọn"
        },
        {
            value: 1, name: "1"
        },
        {
            value: 2, name: "2"
        }
    ];
}]);