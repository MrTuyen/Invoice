app.controller('LogController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Log/';
    LoadingShow();
    $scope.LoadCookie_Log = function () {
        var check = getCookie("Novaon_LogManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldUserName: true,
                FieldActionName: true,
                FieldObjectName: true,
                FieldIpAddress: true,
                FieldDescription: true,
                FieldLogTime: true,
                RowNum: 10
            }
            setCookie("Novaon_LogManagement", JSON.stringify($scope.cookie), 30);
        }
    }
    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_LogManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetLog($scope.currentpage);
    }
    $rootScope.ReloadLog = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetLog($scope.currentpage);
    }
    $scope.ResetGetLog = function () {
        $scope.LoadCookie_Log();
        $scope.ListLog = [];
    }
    $scope.Filter = {KEYWORD:''};
    $scope.GetLog = function (intpage) {
        $scope.ResetGetLog();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetLogs';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListLog = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();

        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    $scope.ListLog = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LogController - GetLog');
            }
            LoadingHide();
        });
    }
    $scope.SelectAll = function () {
        var find = $scope.ListLog.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }
    $scope.SeleteRow = function (item) {
        var find = $scope.ListLog.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListLog.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
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
}]);