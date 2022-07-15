app.controller('ModalProductController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Product/';

    $rootScope.ModalProduct = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        $timeout(function () {
            $rootScope.GetCategory();
            $rootScope.GetQuantityUnit();
        }, 200)

        $scope.TYPECHANGE = type;
        $scope.Product = new Object();
        if (type == 1) {
            //tạo mới
            $scope.Product = new Object();
            if (typeof item === "string") {
                $scope.Product.PRODUCTNAME = item;
            }
        }
        else if (type == 2) {
            //chỉnh sửa
            angular.copy(item, $scope.Product);
            $scope.Product.STRPRICE = $scope.Product.PRICE;
        } else if (type == 3) {
            //sao chép
            angular.copy(item, $scope.Product);
            $scope.Product.ID = 0
        }
    }


    $scope.ChangeRetailPrice = function (item) {
        var vPrice = item.STRPRICE;
        if (item.STRPRICE != null && !isNaN(parseFloat(item.STRPRICE))) {
            vPrice = GetNumber(item.STRPRICE);
        }
        item.PRICE = vPrice;
    }

    $scope.uploadFile = function (event) {
        LoadingShow();
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $scope.Product.IMAGE = event.target.result;
                    LoadingHide();
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };


    $scope.AddProduct = function () {
        if (!$scope.Product.PRODUCTTYPE) {
            alert('Vui lòng chọn kiểu sản phẩm');
            return false;
        }
        if (!$scope.Product.PRODUCTNAME) {
            alert('Vui lòng nhập vào tên sản phẩm');
            return false;
        }
        //if (!$scope.Product.CATEGORY && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn danh mục sản phẩm');
        //    return false;
        //}
        //if (!$scope.Product.PRICE) {
        //    alert('Vui lòng nhập vào đơn giá');
        //    return false;
        //}
        //if (!$scope.Product.QUANTITYUNIT && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn đơn vị tính');
        //    return false;
        //}
        $scope.IsLoading = true;
        var action = url + 'AddProduct';

        var datasend = JSON.stringify({
            product: $scope.Product
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-product').modal('hide');
                    $rootScope.ReloadProduct(1);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddProduct');
            }
            LoadingHide();
        });
    }

    $scope.UpdateProduct = function () {
        if (!$scope.Product.PRODUCTTYPE) {
            alert('Vui lòng chọn kiểu sản phẩm');
            $('div#productType').focus();
            return false;
        }
        if (!$scope.Product.PRODUCTNAME) {
            alert('Vui lòng nhập vào tên sản phẩm');
            $('div#idProductName').focus();
            return false;
        }
        //if (!$scope.Product.CATEGORY && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn danh mục sản phẩm');
        //    $('select#idCategory').focus();
        //    return false;
        //}
        //if (!$scope.Product.PRICE) {
        //    alert('Vui lòng nhập vào đơn giá');
        //    $('div#idProductPrice').focus();
        //    return false;
        //}
        //if (!$scope.Product.QUANTITYUNIT && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn đơn vị tính');
        //    $('select#idProductQuantityUnit').focus();
        //    return false;
        //}
        var action = url + 'UpdateProduct';
        var datasend = JSON.stringify({
            product: $scope.Product
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-product').modal('hide');
                    $rootScope.ReloadProduct();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateProduct');
            }
            LoadingHide();
        });
    }

    $scope.QuantityUnitChange = function (item) {
        var listUnit = [];
        angular.forEach($rootScope.ListQuantityUnit, function (key) {
            listUnit.push(key.QUANTITYUNIT);
        });

        $('#quantityunitproduct').autoComplete({
            minChars: 1,
            source: function (xxx, suggest) {
                var choices = JSON.parse(JSON.stringify(listUnit));
                var suggestions = [];
                suggestions.length = 0;
                for (i = 0; i < choices.length; i++) {
                    if (~($rootScope.cleanAccents(choices[i]).toLowerCase()).indexOf($rootScope.cleanAccents(item.toLowerCase())))
                        suggestions.push(choices[i]);
                    suggest(suggestions);
                }
            }
        });
    }
}]);