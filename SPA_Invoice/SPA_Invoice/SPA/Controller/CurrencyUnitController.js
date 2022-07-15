app.controller('CurrencyUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/CurrencyUnit/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Currency = function () {
        var check = getCookie("Novaon_CurrencyUnitManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCurrencyUnit: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_CurrencyUnitManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CurrencyUnitManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCurrencyUnit($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadCurrencyUnit = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetCurrencyUnit($scope.currentpage);
    }

    $scope.ResetGetCurrencyUnit = function () {
        $scope.LoadCookie_Currency();
        $scope.ListCurrencyUnit = [];
    }
    //lấy ra danh sách tiền thanh toán
    $scope.GetCurrencyUnit = function (intpage) {
        $scope.ResetGetCurrencyUnit();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetCurrencyUnit';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCurrencyUnit = response.result;
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
        var find = $scope.ListCurrencyUnit.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListCurrencyUnit.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCurrencyUnit.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    $scope.RemoveCurrencyUnit = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListCurrencyChecked = [];
            $scope.ListCurrencyActive = [];
            $scope.count = 0;
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được phép xóa khi đang ở trạng thái hoạt động');
                    return false;
                }
                $scope.ListCurrencyChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCurrencyChecked = $scope.ListCurrencyUnit.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCurrencyChecked && listCurrencyChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn tiền thanh toán cần xóa.");
                    return;
                }
                if (listCurrencyChecked && Object.keys(listCurrencyChecked).length > 0) {
                    for (var i = 0; i < listCurrencyChecked.length; i++) {
                        $scope.ListCurrencyChecked.push(listCurrencyChecked[i].ID);
                        $scope.ListCurrencyActive.push(listCurrencyChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListCurrencyActive.length; i++) {
                        if ($scope.ListCurrencyActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstCurrencyid = $scope.ListCurrencyChecked.join(";");
            var action = url + 'RemoveCurrencyUnit';
            var datasend = JSON.stringify({
                id: lstCurrencyid,count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các tiền thanh toán đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
    $scope.ExportExcell = function () {
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