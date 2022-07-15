app.controller('PaymentMethodController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/PaymentMethod/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Payment = function () {
        var check = getCookie("Novaon_PaymentManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldPaymentMethod: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_PaymentManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_PaymentManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetPaymentMethod($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadPayment = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetPaymentMethod($scope.currentpage);
    }

    $scope.ResetGetPayment = function () {
        $scope.LoadCookie_Payment();
        $scope.ListPayment = [];
    }
    // lấy ra danh sách HTTT
    $scope.GetPaymentMethod = function (intpage) {
        $scope.ResetGetPayment();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetPaymentMethod';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListPayment = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetPaymentMethod');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListPayment.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListPayment.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListPayment.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
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
    $scope.RemovePaymentMethod = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListPaymentChecked = [];
            $scope.ListPaymentActive = [];
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được xóa khi đang ở trạng thái hoạt động');
                    return false;
                }
                $scope.ListPaymentChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listPaymentChecked = $scope.ListPayment.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listPaymentChecked && listPaymentChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn HTTT cần xóa.");
                    return;
                }
                if (listPaymentChecked && Object.keys(listPaymentChecked).length > 0) {
                    for (var i = 0; i < listPaymentChecked.length; i++) {
                        $scope.ListPaymentChecked.push(listPaymentChecked[i].ID);
                        $scope.ListPaymentActive.push(listPaymentChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListPaymentActive.length; i++) {
                        if ($scope.ListPaymentActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstPaymentid = $scope.ListPaymentChecked.join(";");
            var action = url + 'RemovePaymentMethod';
            var datasend = JSON.stringify({
                id: lstPaymentid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadPayment();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các HTTT đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);