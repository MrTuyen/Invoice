app.controller('CustomerController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Customer/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Customer = function () {
        var check = getCookie("Novaon_CustomerManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldIDCustummer: true,
                FieldName: true,
                FieldEnterprise: true,
                FieldPhone: true,
                FieldEmail: true,
                FieldAddress: true,
                FieldStatus: true,
                FieldTotalMoney: true,
                FieldUnpaidMoney: true,
                FieldCustaxcode: true,
                RowNum: 10
            }
            setCookie("Novaon_CustomerManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {        
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CustomerManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCustomer($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '' }

    $rootScope.ReloadCustomer = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetCustomer($scope.currentpage);
    }

    $scope.ResetGetCustomer = function () {
        $scope.LoadCookie_Customer();
        $scope.ListCustomer = [];
    }

    $scope.GetCustomer = function (intpage) {        
        $scope.ResetGetCustomer();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetCustomer';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    $scope.ListCustomer = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.SelectAll = function () {
        var find = $scope.ListCustomer.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {

        var find = $scope.ListCustomer.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCustomer.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    $scope.EditCustomer = function (item) {
        $scope.CurrentCustomer = new Object();
        angular.copy(item, $scope.CurrentCustomer);
    }

    //$scope.EditCustomer = function (item) {
    //    $scope.CurrentCustomer = new Object();
    //    angular.copy(item, $scope.CurrentCustomer);

    //    //$scope.IsLoading = true;
    //    //var action = url + 'GetListInvoiceCustomer';
    //    //var datasend = JSON.stringify({
    //    //    keyword: $scope.Filter.KEYWORD,
    //    //    currentPage: 1
    //    //});
    //    //CommonFactory.PostDataAjax(action, datasend, function (response) {
    //    //    if (response) {
    //    //        if (response.rs) {
    //    //            $scope.ListCustomer = response.result;
    //    //        } else {
    //    //            $scope.ErrorMessage = response.msg;
    //    //        }
    //    //    } else {
    //    //        alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
    //    //    }
    //    //});
    //    //$scope.IsLoading = false;


    //    var action = url + 'GetInvoice';
    //    var datasend = JSON.stringify({
    //        customer: $scope.CurrentCustomer,
    //        currentPage: $scope.currentpage
    //    });
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $scope.ListInvoiceCustomer = response.result;
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
    //        }
    //    });
    //}
    $scope.DeletedCustomer = function (item) {
        item.ISDELETED = true;
        $rootScope.ModalCustomer(item, false);
        $('.modal-customer').modal('show');
    }

    $scope.ExportExcel = function () {
        $scope.IsLoading = true;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            currentPage: 1
        };
        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });

        LoadingHide();
    }

    $scope.DeactiveCustomer = function () {
        var action = url + 'DeactiveCustomer';
        var datasend = JSON.stringify({
            customers: $scope.ListCustomer
        }); LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                } else {
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeactiveCustomer');
            }
            LoadingHide();
        });
    }

    $scope.GetInvoiceByCustomer = function (intpage) {

        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        $scope.IsInfo = false;
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            customer: $scope.CurrentCustomer,
            currentPage: $scope.currentpage
        });
        $scope.ListInvoiceCustomer = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoiceCustomer = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }

    $scope.SendEmail = function () {
        var emailList = [];
        $scope.ListCustomer.forEach(function (obj) {
            if (obj.ISSELECTED == true) {
                if (obj.CUSEMAIL)
                    emailList.push(obj.CUSEMAIL);
            }
        });
        if (emailList.length == 0)
            return false;
        window.open('mailto:' + emailList.join(';'), '_blank');
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });
    $scope.RemoveCustomer = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListCustomerChecked = [];
            $scope.ListCustomerCheckCode = [];
            if (item) {
                $scope.ListCustomerChecked.push(item.ID);
                $scope.ListCustomerCheckCode.push(item.CUSID);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCustomerChecked = $scope.ListCustomer.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCustomerChecked && listCustomerChecked.length === 0) {
                    alert("Bạn chưa chọn khách hàng cần xóa.");
                    return;
                }
                if (listCustomerChecked && Object.keys(listCustomerChecked).length > 0) {
                    for (var i = 0; i < listCustomerChecked.length; i++) {
                        $scope.ListCustomerChecked.push(listCustomerChecked[i].ID);
                        $scope.ListCustomerCheckCode.push(listCustomerChecked[i].CUSID);
                    }
                }
            }

            var lstCustomerid = $scope.ListCustomerChecked.join(";");
            var lstCustomercode = $scope.ListCustomerCheckCode.join(";");
            var action = url + 'RemoveCustomer';
            var datasend = JSON.stringify({
                idCustomers: lstCustomerid, customer: item, codeCus: lstCustomercode
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCustomer();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các khách hàng đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_Cus() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_Cus() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}
