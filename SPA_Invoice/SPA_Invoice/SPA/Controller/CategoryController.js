app.controller('CategoryController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Category/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Category = function () {
        var check = getCookie("Novaon_CategoryManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCategoryName: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_CategoryManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CategoryManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCategory($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadCategory = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetCategory($scope.currentpage);
    }

    $scope.ResetGetCategory = function () {
        $scope.LoadCookie_Category();
        $scope.ListCategory = [];
    }
    //lấy ra danh sách danh mục
    $scope.GetCategory = function (intpage) {
        $scope.ResetGetCategory();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        
        var action = url + 'GetCategory';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCategory = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCategory');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListCategory.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListCategory.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCategory.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    //xóa dịch vụ
    $scope.RemoveCategory = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListCategoryChecked = [];
            $scope.ListCategoryActive = [];
            if (item) {
                if (item.ISACTIVE == true) {
                    toastr.warning('Không được phép xóa khi đang ở trạng thái hoạt động');
                    return false;
                } else {
                    $scope.ListCategoryChecked.push(item.ID);
                    $scope.count = 1;

                }
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCategoryChecked = $scope.ListCategory.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCategoryChecked && listCategoryChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn dịch vụ cần xóa.");
                    return;
                }
                if (listCategoryChecked && Object.keys(listCategoryChecked).length > 0) {
                    for (var i = 0; i < listCategoryChecked.length; i++) {
                        $scope.ListCategoryChecked.push(listCategoryChecked[i].ID);
                        $scope.ListCategoryActive.push(listCategoryChecked[i].ISACTIVE);
                    }
                    for (var i = 0; i < $scope.ListCategoryActive.length; i++) {
                        if ($scope.ListCategoryActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstCategoryid = $scope.ListCategoryChecked.join(";");
            var action = url + 'RemoveCategory';
            var datasend = JSON.stringify({
                id: lstCategoryid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCategory();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các dịch vụ đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
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
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
}]);