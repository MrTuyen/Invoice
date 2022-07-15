app.controller('QuantityUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/QuantityUnit/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_QuantityUnit = function () {
        var check = getCookie("Novaon_QuantityUnitManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldQuantityUnit: true,
                
                RowNum: 10
            }
            setCookie("Novaon_QuantityUnitManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_QuantityUnitManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetQuantityUnit($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadQuantityUnit = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetQuantityUnit($scope.currentpage);
    }

    $scope.ResetGetQuantityUnit = function () {
        $scope.LoadCookie_QuantityUnit();
        $scope.ListQuantityUnit = [];
    }

    $scope.GetQuantityUnit = function (intpage) {
        $scope.ResetGetQuantityUnit();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetQuantityUnit';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListQuantityUnit = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetQuantityUnit');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListQuantityUnit.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListQuantityUnit.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListQuantityUnit.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    //xóa đơn vị tính
    $scope.RemoveQuantityUnit = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListQuantityChecked = [];
            $scope.ListQuantityActive = [];
            $scope.ListComtaxcode = [];
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được phép xóa trạng thái đang hoạt động');
                    return;
                }
                $scope.ListQuantityChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listQuantityChecked = $scope.ListQuantityUnit.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listQuantityChecked && listQuantityChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn đơn vị tính cần xóa.");
                    return;
                }
                if (listQuantityChecked && Object.keys(listQuantityChecked).length > 0) {
                    for (var i = 0; i < listQuantityChecked.length; i++) {
                        $scope.ListQuantityChecked.push(listQuantityChecked[i].ID);
                        $scope.ListQuantityActive.push(listQuantityChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListQuantityActive.length; i++) {
                        if ($scope.ListQuantityActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstQuantityid = $scope.ListQuantityChecked.join(";");
            var action = url + 'RemoveQuantityUnit';
            var datasend = JSON.stringify({
                id: lstQuantityid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadQuantityUnit();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các đơn vị tính đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
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