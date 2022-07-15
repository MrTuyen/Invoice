app.controller('ProductController', ['$rootScope', '$scope', '$timeout', 'CommonFactory', function ($rootScope, $scope, $timeout, CommonFactory) {
    var url = '/Product/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Product = function () {
        var check = getCookie("Novaon_ProductManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldName: true,
                FieldSKU: true,
                FieldType: true,
                FieldCategory: true,
                FieldDes: true,
                FieldQuantityUnit: true,
                FieldPrice: true,
                RowNum: 10
            }
            setCookie("Novaon_ProductManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_ProductManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetProduct($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }

    $rootScope.ReloadProduct = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetProduct($scope.currentpage);
    }

    $scope.ResetGetFunction = function () {
        $scope.LoadCookie_Product();
        $scope.ListProduct = [];
    }

    $scope.GetProduct = function (intpage) {
        $scope.ResetGetFunction();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetProduct';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListProduct = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            
            if (response) {
                if (response.rs) {
                    $scope.ListProduct = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ProductController - GetProduct');
            }
            LoadingHide();
        });
    }

    $scope.UpdateType = function () {
        if (!$scope.ChangeType)
            return;

        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (find.length == 0) {
            alert('Vui lòng chọn ít nhất 1 dòng');
            $scope.ChangeType = null
            return;
        }

        var result = confirm('Chuyển sang loại ' + $scope.ChangeType);

        if (result) {
            var action = url + 'ChangeProductType';
            var datasend = JSON.stringify({
                products: $scope.ListProduct,
                productType: $scope.ChangeType
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        $rootScope.ReloadProduct();
                    } else {
                        $scope.ChangeType = null
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ProductController - UpdateType');
                }
            });
            LoadingHide();
        } else {
            $scope.ChangeType = null
        }

    }

    $scope.ExportExcel = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            productType: $scope.Filter.PRODUCTTYPE,
            category: $scope.Filter.CATEGORY,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }



    $scope.SelectAll = function () {
        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListProduct.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });
    $scope.RemoveProduct = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListProductChecked = [];
            $scope.ListProductCheckedName = [];
            $scope.ListProductCheckedSku = [];
            if (item) {
                $scope.ListProductChecked.push(item.ID);
                $scope.ListProductCheckedName.push(item.PRODUCTNAME);
                $scope.ListProductCheckedSku.push(item.SKU);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listProductChecked = $scope.ListProduct.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listProductChecked && listProductChecked.length === 0) {
                    alert("Bạn chưa chọn sản phẩm cần xóa.");
                    return;
                }
                if (listProductChecked && Object.keys(listProductChecked).length > 0) {
                    for (var i = 0; i < listProductChecked.length; i++) {
                        $scope.ListProductChecked.push(listProductChecked[i].ID);
                        $scope.ListProductCheckedName.push(listProductChecked[i].PRODUCTNAME);
                        $scope.ListProductCheckedSku.push(listProductChecked[i].SKU);
                    }
                }
            }

            var lstProductid = $scope.ListProductChecked.join(";");
            var lstProductname = $scope.ListProductCheckedName.join(";");
            var lstProductsku = $scope.ListProductCheckedSku.join(";");
            var action = url + 'RemoveProduct';
            var datasend = JSON.stringify({
                idProducts: lstProductid, proname: lstProductname, sku: lstProductsku
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadProduct();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các sản phẩm đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_Pro() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_Pro() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}