app.controller('MeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Meter/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Meter = function () {
        var check = getCookie("Novaon_MeterManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCode: true,
                FieldMeterName: true,
                FieldCustaxCode: true,
                FieldComtaxCode: true,
                FieldProductCode: true,
                FieldIsactive: true,
                RowNum: 10
            }
            setCookie("Novaon_MeterManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_MeterManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetMeter($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }

    $rootScope.ReloadMeter = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetMeter($scope.currentpage);
    }

    $scope.ResetGetMeter = function () {
        $scope.LoadCookie_Meter();
        $scope.ListMeter = [];
    }

    $scope.GetMeter = function (intpage) {
        $scope.ResetGetMeter();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetMeter';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListMeter = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();

        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    $scope.ListMeter = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - MeterController - GetMeter');
            }
            LoadingHide();
        });
    }

    $scope.UpdateType = function () {
        if (!$scope.ChangeType)
            return;

        var find = $scope.ListMeter.filter(function (obj) {
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
                meter: $scope.ListMeter,
                meterType: $scope.ChangeType
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        $rootScope.ReloadMeter();
                    } else {
                        $scope.ChangeType = null
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - MeterController - UpdateType');
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
        var find = $scope.ListMeter.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListMeter.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListMeter.length - 1) {

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
    $scope.RemoveMeter = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListMeterChecked = [];
            if (item) {
                $scope.ListMeterChecked.push(item.CODE);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listMeterChecked = $scope.ListMeter.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listMeterChecked && listMeterChecked.length === 0) {
                    alert("Bạn chưa chọn công tơ  cần xóa.");
                    return;
                }
                if (listMeterChecked && Object.keys(listMeterChecked).length > 0) {
                    for (var i = 0; i < listMeterChecked.length; i++) {
                        $scope.ListMeterChecked.push(listMeterChecked[i].CODE);
                    }
                }
            }
            
            var lstMetertcode = $scope.ListMeterChecked.join(";");
            var action = url + 'RemoveMeter';
            var datasend = JSON.stringify({
                metercode: lstMetertcode
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadMeter();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các công tơ đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_meter() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_meter() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}

