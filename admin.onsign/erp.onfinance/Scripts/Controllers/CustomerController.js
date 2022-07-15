
app.controller('CustomerController', ['$scope', '$timeout', '$filter', 'CommonFactory', function ($scope, $timeout, $filter, CommonFactory) {
    var url = '/Customer/';
    $scope.isLoading = false;

    $scope.SearchCustomer = function (intpage) {
        var typecustomer = $('#ddlCustomerType').val();
        var txtmonth = $('#idCbMonth').val();

        var action = url + 'GetCustomer';


        if ($scope.searchPointType == undefined || $scope.searchPointType == null)
            $scope.searchPointType = '';

        var datasend = JSON.stringify({
            customerKeywords: $scope.customerKeywords,
            searchpointtype: $scope.searchPointType + '',
            toPointKeywords: $scope.toPointKeywords,
            fromPointKeywords: $scope.fromPointKeywords,
            type: typecustomer,
            month: txtmonth,
            page: intpage,
            startDate: $scope.startDate,
            endDate: $scope.endDate,
            storeIds: $scope.storeOpenCard,
            provinces: $scope.modelProvince,
            districts: $scope.modelDistrict
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCustomer = response.msg;
                    $scope.TotalPages = response.TotalPages;
                    $scope.currentPage = response.page;
                } else {
                    alert(response.msg);
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
            $scope.isLoading = false;
        });
    };

    // Purchase History
    $scope.Search = function (intpage) {
        if ($scope.keywords == '' || $scope.keywords == null || $scope.keywords == undefined) {
            alert("Thông tin của khách hàng không được trống , vui lòng nhập vào : Mã khác hàng , Tên khách hàng , hoặc SĐT để tiếp tục");
            return false;
        }

        $scope.isLoading = true;

        $scope.ListReport = null;
        //$scope.TotalBefor = 0;
        //$scope.TotalAfter = 0;
        var datasend = JSON.stringify({
            keyword: $scope.keywords,
            lstStoreIDs: $('#ddlStore').val(),
            TxtFromDate: $scope.txtFromDate,
            TxtToDate: $scope.txtToDate,
            page: intpage
        });

        var action = url + 'SearchByKeyword';
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListOutput = response.msg;
                    $scope.TotalPages = response.TotalPages;
                    $scope.currentPage = response.page;
                    $scope.d = $scope.keywords; // giữ lại các giá trị cũ tránh bị đổi trên view
                    $scope.fd = $scope.txtFromDate;
                    $scope.td = $scope.TxtToDate;
                    $scope.lst = $('#ddlStore').val();

                } else {
                    alert(response.msg);
                }
            }
            else
            {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }

            $scope.isLoading = false;
        });
    };

    $scope.showCustomer = function (v) {
        $scope.CustomerEdit = JSON.parse(JSON.stringify(v));
        $scope.CustomerEdit.BIRTHDAY = $filter('dateFormat')($scope.CustomerEdit.BIRTHDAY, 'dd/MM/yyyy');
        $('#divEditCustomer').modal('show');

    };

    $scope.EditCustomer = function () {
        if ($scope.CustomerEdit.CUSTOMERNAME == null || $scope.CustomerEdit.CUSTOMERNAME == '') {
            alert('Vui lòng nhập tên khách hàng.');
            return false;
        }

        $scope.isEditing = true;
        var datasend = JSON.stringify({
            customer: $scope.CustomerEdit,
            strBirthDay: $scope.CustomerEdit.BIRTHDAY //Tách ngày sinh ra để lấy ở dạng string
        });
        var action = url + 'EditCustomer';
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                alert(response.msg);

                if (response.rs) {
                    for (var i = 0; i < $scope.ListCustomer.length; i++) {
                        if ($scope.ListCustomer[i].CUSTOMERID == $scope.CustomerEdit.CUSTOMERID) {
                            $scope.ListCustomer[i] = response.obj;
                            break;
                        }
                    }
                    $scope.BackListCustomer();
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
            $scope.isEditing = false;
        });
    };

    $scope.AddCustomerPoint = function (v) {
        $scope.CustomerEdit = JSON.parse(JSON.stringify(v));
        $scope.CustomerEdit.BIRTHDAY = $filter('dateFormat')($scope.CustomerEdit.BIRTHDAY, 'dd/MM/yyyy');
        $('#addmemberpoint').modal('show');

    };

    $scope.SearchBillCode = function () {
        if ($scope.CustomerEdit.OUTPUTVOUCHERID == null || $scope.CustomerEdit.OUTPUTVOUCHERID == "") {
            alert("Vui lòng nhập vào mã đơn hàng")
            return false;
        }
        var datasend = JSON.stringify({
            outputvoucherid: $scope.CustomerEdit.OUTPUTVOUCHERID
        });
        var action = url + "SearchBillCode";
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.objVoucher = response.msg;
                }
                else {
                    alert(response.msg);
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
            $scope.isEditing = false;
        });
           
    };

    $scope.AddPoint = function () {
        if (!confirm("Bạn có chắc chắn muốn sử dụng đơn hàng này để tích điểm bù?"))
            return false;
        var datasend = JSON.stringify({
            CustomerID : $scope.CustomerEdit.CUSTOMERID,
            BillCode : $scope.objVoucher.OUTPUTVOUCHERID
        });
        var action = url + "AddCustomerPoint";
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.BackListCustomerFromAddCustomerPoint();
                    $scope.SearchCustomer(1);
                }
                   alert(response.msg);
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
            $scope.isEditing = false;
        });
    };

    $scope.ExportExcel = function () {

        var action = url + "ExportExcel";

        var typecustomer = $('#ddlCustomerType').val();
        var txtmonth = $('#idCbMonth').val();

        var datasend = {
            customerKeywords: $scope.customerKeywords,
            searchpointtype: $scope.searchPointType + '',
            toPointKeywords: $scope.toPointKeywords,
            fromPointKeywords: $scope.fromPointKeywords,
            type: typecustomer,
            month: txtmonth,
            startDate: $scope.startDate,
            endDate: $scope.endDate,
            storeIds: $scope.storeOpenCard,
            provinces: $scope.modelProvince,
            districts: $scope.modelDistrict
        };


        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            datatype: JSON,
            preparingMessageHtml: "Đang tải file vui lòng đợi...",
            failMessageHtml: "Có lỗi trong khi tải file excel."
        });
    };

    $scope.ChangeProvince = function () {
        console.log($scope.modelProvince);
        console.log($scope.modelDistrict);
        var datasend = JSON.stringify({
            provinceids: $scope.modelProvince
        });
        var action = '/SellOrderOnline/GetDistrictByProvinceId'
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Districts = response.districts;
                    $timeout(function () {
                        $('#idDistrict').multiselect('refresh');
                    }, 100);
                } else {
                    alert(response.msg);
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
        });
    }

    // Export Customer purchase history
    $scope.ExportHistoryToExcel = function () {
        var datasend = {
            keyword: $scope.keywords,
            lstStoreIDs: $('#ddlStore').val(),
            TxtFromDate: $scope.txtFromDate,
            TxtToDate: $scope.txtToDate
        };

        var action = url + "ExportHistoryToExcel";
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            preparingMessageHtml: "Đang tải file vui lòng đợi...",
            failMessageHtml: "Có lỗi trong khi tải file excel."
        });
    };

    //Go back button
    $scope.BackListCustomer = function () {
        $scope.CustomerEdit = null;
        $('#divEditCustomer').modal('hide');
    };
    //Another go back button
    $scope.BackListCustomerFromAddCustomerPoint = function () {
        $scope.CustomerEdit = null;
        $scope.objVoucher = null;
        $('#addmemberpoint').modal('hide');
    };
    $scope.htmlAdText = function (text) { return $sce.trustAsHtml(text); };


    //$scope.ChangeProvince = function () {
    //    var datasend = JSON.stringify({
    //        provinceids: $scope.Master.DELIVERYPROVINCEID
    //    });
    //    var action = url + 'GetDistrictByProvinceId'
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $scope.Districts = response.districts;
    //                $timeout(function () {
    //                    $('#idDistrict').multiselect('refresh');
    //                }, 100);
    //            } else {
    //                alert(response.msg);
    //            }
    //        } else {
    //            alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
    //        }
    //    });
    //}

    // Multi select
    var Init = function () {
        $("select#ddlCustomerType").multiselect({
            noneSelectedText: '- Vui lòng chọn -',
            selectedList: 0,
            selectedText: 'Đã chọn # / # loại '
        });
        
        $("#storeOpenCard").multiselect({
            noneSelectedText: '- Chọn cửa hàng -',
            selectedList: 0,
            selectedText: 'Đã chọn # / # cửa hàng '
        }).multiselectfilter();

        $("select#ddlStore").multiselect({
            noneSelectedText: '- Vui lòng chọn kho -',
            selectedList: 0,
            selectedText: 'Đã chọn # / # cửa hàng '
        });

        $("select#idCbMonth").multiselect({
            noneSelectedText: '- Vui lòng chọn -',
            multiple: false
        });

        $("select#searchPointType").multiselect({
            noneSelectedText: '- Lọc theo điều kiện -',
            multiple: false
        });

        $("#idProvince").multiselect({
            noneSelectedText: '- Chọn Tỉnh/Thành phố -',
            multiple: false
        }).multiselectfilter();

        $("#idDistrict").multiselect({
            noneSelectedText: 'Chọn Quận/Huyện',
            multiple: true,
            selectedText: 'Đã chọn # / #'
        }).multiselectfilter();

        $scope.listtype = [
            { "VALUE": "SAB-FREE", "NAME": "SAB-FREE" },
            { "VALUE": "VIP-15", "NAME": "VIP-15" },
            { "VALUE": "VIP-10", "NAME": "VIP-10" },
            { "VALUE": "SAB-MEMBER", "NAME": "SAB-MEMBER" }
        ];

        $scope.listmonth = [
            { "VALUE": null, "NAME": "- Không quan tâm -" },
            { "VALUE": "1", "NAME": "Tháng 1" },
            { "VALUE": "2", "NAME": "Tháng 2" },
            { "VALUE": "3", "NAME": "Tháng 3" },
            { "VALUE": "4", "NAME": "Tháng 4" },
            { "VALUE": "5", "NAME": "Tháng 5" },
            { "VALUE": "6", "NAME": "Tháng 6" },
            { "VALUE": "7", "NAME": "Tháng 7" },
            { "VALUE": "8", "NAME": "Tháng 8" },
            { "VALUE": "9", "NAME": "Tháng 9" },
            { "VALUE": "10", "NAME": "Tháng 10" },
            { "VALUE": "11", "NAME": "Tháng 11" },
            { "VALUE": "12", "NAME": "Tháng 12" }
        ];

        $timeout(function () {
            $("select#ddlCustomerType").multiselect("refresh");
            $("select#searchPointType").multiselect("refresh");
            $("select#idCbMonth").multiselect("refresh");
        }, 500);
    };

    Init();
}]);