
app.controller('AccountantConfirmController', ['$scope', '$timeout', 'CommonFactory', function ($scope, $timeout, CommonFactory) {
    var url = '/AccountantConfirm/';

    $scope.btnSearch = function () {
        if (!$scope.keyword) {
            if (!$scope.ddlStore) {
                alert('Vui lòng chọn kho nhập.');
                return false;
            }
        }

        if (!$scope.strFromDate) {
            alert('Vui lòng chọn ngày từ.');
            return false;
        }
        if (!$scope.strToDate) {
            alert('Vui lòng chọn ngày đến.');
            return false;
        }
        $scope.isLoading = true;
        var datasend = JSON.stringify({
            key: $scope.key,
            fromDate: $scope.strFromDate,
            toDate: $scope.strToDate,
            storeid: $scope.ddlStore[0]
        });
        var action = url + 'SearchInput';
        CommonFactory.PostDataAjax(action, datasend, function (result) {
            if (!result.success) {
                alert(result.reponseText);
                console.log(result.reponseText);
            } else {
                $scope.ListData = result.listData;
            }
            $scope.isLoading = false;
        });
    };

    $scope.ViewDetail = function (d) {
        $scope.keysearch = null;
        $scope.isLoading = true;
        var datasend = JSON.stringify({
            productdivisionid: d.PRODUCTDIVISIONID,
            instoreid: d.INSTOREID
        });
        var action = url + 'ViewInput';
        CommonFactory.PostDataAjax(action, datasend, function (result) {
            if (!result.success) {
                alert(result.reponseText);
            } else {
                $scope.objDetail = {};
                $scope.objDetail.Master = d;
                $scope.objDetail.lstDetail = result.listDetail;
                console.log($scope.objDetail);
            }
            $scope.isLoading = false;
        });
    }

    $scope.AcceptStoreChange = function (d) {
        var result = confirm("Bạn chắc chắn muốn xác nhận nhập hàng?");
        if (!result) {
            return false;
        }

        $scope.isLoading = true;
        var datasend = JSON.stringify({
            productdivisionid: d.PRODUCTDIVISIONID
        });
        var action = url + 'AcceptStoreChange';
        CommonFactory.PostDataAjax(action, datasend, function (result) {
            if (result) {
                alert(result.reponseText);
                if (result.success) {
                    $scope.objDetail = null;
                    $scope.btnSearch();
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ (js).");
                $scope.objDetail = null;
                $scope.btnSearch();
            }
            $scope.isLoading = false;
        });
    }

    $scope.BackToList = function () {
        $scope.objDetail = null;
    }

    var Init = function () {

        $("select#ddlStore").multiselect({
            multiple: false,
            noneSelectedText: '- Chọn kho nhập -',
        }).multiselectfilter();

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }
        $scope.strToDate = dd + "/" + mm + "/" + yyyy;

        today.setMonth(today.getMonth() - 1);
        dd = today.getDate();
        mm = today.getMonth() + 1; //January is 0!
        yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }
        $scope.strFromDate = dd + "/" + mm + "/" + yyyy;
    };
    $scope.ExcelExport = function () {
        var datasend = JSON.stringify({
            productdivisionid: $scope.objDetail.Master.PRODUCTDIVISIONID,
            storeid: $scope.objDetail.Master.INSTOREID
        });
        $scope.isLoading = true;
        var action = url + "ExcelExport?productdivisionid=" + $scope.objDetail.Master.PRODUCTDIVISIONID +
            "&storeid=" + $scope.objDetail.Master.INSTOREID;
        window.location = action;
        $scope.isLoading = false;
    }

    Init();
}]);



