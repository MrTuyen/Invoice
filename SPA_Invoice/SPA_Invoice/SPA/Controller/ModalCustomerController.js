app.controller('ModalCustomerController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Customer/';
    //var $scope.Customer.CUSTOMERTYPE = 1;
    $rootScope.ModalCustomer = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }
        $rootScope.GetPaymentMethod();
        $rootScope.GetCurrencyUnit();
        $scope.TYPECHANGE = type;
        $scope.selectedValues = [];
        $scope.Customer = new Object();
        if (type == 1) {
            $scope.Customer = new Object();
            $scope.Customer.CUSPAYMENTMETHOD = "TM/CK";
            $scope.Customer.CUSCURRENCYUNIT = "VNĐ (Việt Nam Đồng)";
        } else if (type == 2) {
            angular.copy(item, $scope.Customer);
            if ($rootScope.Enterprise.USINGINVOICETYPE === 2) {
                if ($scope.Customer.METERCODE)
                    $scope.selectedValues = $scope.Customer.METERCODE.split(',');
                $scope.GetListMeters();
            }
        }
        else if (type == 3) {
            angular.copy(item, $scope.Customer);
            $scope.Customer.ID = 0
            if ($rootScope.Enterprise.USINGINVOICETYPE === 2) {
                if ($scope.Customer.METERCODE)
                    $scope.selectedValues = $scope.Customer.METERCODE.split(',');
                $scope.GetListMeters();
            }
        }
        if ($scope.Customer.COMTAXCODE !== null || $scope.Customer.COMTAXCODE !== '') {
            $scope.Customer.CUSTOMERTYPE = 1;
        }
        else {
            $scope.Customer.CUSTOMERTYPE = 2;
        }
        if ($scope.Customer.CUSTOMERTYPE === 1) {
            $('#comtaxt-code-required').html('(*)');
            $('#ctr-address-required').html('(*)');
            $('input:radio[name=DefaultCustomer]').prop('checked', false);
            $('input:radio[name=DefaultPersional]').prop('checked', true);
        }
        else {
            $('input:radio[name=DefaultCustomer]').prop('checked', true);
            $('input:radio[name=DefaultPersional]').prop('checked', false);
            $('#comtaxt-code-required').html('');
            $('#ctr-address-required').html('');
        }
        $rootScope.Customercode = $scope.Customer.CUSTAXCODE;
    }

    /**
     * truongnv 2020030203
     * Kiểm ra bắt buộc nhập MST
     * @param {any} value
     */
    $scope.SetRequiedTaxCode = function (value) {
        $scope.Customer.CUSTOMERTYPE = value;
        if (value === 1) {
            $('#comtaxt-code-required').html('(*)');
            $('#ctr-address-required').html('(*)');
            $('input:radio[name=DefaultCustomer]').prop('checked', false);
            $('input:radio[name=DefaultPersional]').prop('checked', true);
        }
        else {
            $('input:radio[name=DefaultCustomer]').prop('checked', true);
            $('input:radio[name=DefaultPersional]').prop('checked', false);
            $('#comtaxt-code-required').html('');
            $('#ctr-address-required').html('');
        }
    }

    $scope.AddCustomer = function () {
        if ($rootScope.Enterprise.USINGINVOICETYPE === 1) {
            if (!$scope.Customer.CUSTAXCODE) {
                alert('Vui lòng nhập vào nhóm khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSID) {
                alert('Vui lòng nhập vào mã khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSNAME) {
                alert('Vui lòng nhập vào tên khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSADDRESS) {
                alert('Vui lòng nhập vào địa chỉ');
                return false;
            }

        } else {
            if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSTAXCODE) {
                alert('Vui lòng nhập vào mã số thuế doanh nghiệp');
                return false;
            }
            if (!$scope.Customer.CUSNAME) {
                alert('Vui lòng nhập vào tên khách hàng');
                return false;
            }
            //if (!$scope.Customer.CUSID) {
            //    alert('Vui lòng nhập mã khách hàng');
            //    return false;
            //}
            //if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSBUYER) {
            //    alert('Vui lòng nhập vào tên người mua hàng');
            //    return false;
            //}

            if (!$scope.Customer.CUSADDRESS) {
                alert('Vui lòng nhập vào địa chỉ');
                return false;
            }

            if (!$scope.Customer.CUSPAYMENTMETHOD) {
                alert('Vui lòng chọn hình thức thanh toán');
                return false;
            }
        }
        

        var action = url + 'AddCustomer';
        $scope.Customer.METERCODE = $scope.selectedValues.join(',');
        var datasend = JSON.stringify({
            customer: $scope.Customer
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "4000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-customer').modal('hide');
                    $rootScope.ReloadCustomer();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddCustomer');
            }
            LoadingHide();
        });
    }

    $scope.UpdateCustomer = function () {
        if (!$scope.Customer.CUSNAME) {
            alert('Vui lòng nhập vào tên khách hàng');
            return false;
        }

        if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSTAXCODE) {
            alert('Vui lòng nhập vào mã số thuế doanh nghiệp');
            return false;
        }
        //if (!$scope.Customer.CUSID) {
        //    alert('Vui lòng nhập mã khách hàng');
        //    return false;
        //}
        if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSBUYER) {
            alert('Vui lòng nhập vào tên người mua hàng');
            return false;
        }

        if (!$scope.Customer.CUSADDRESS) {
            alert('Vui lòng nhập vào địa chỉ');
            return false;
        }

        if (!$scope.Customer.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }
        var action = url + 'UpdateCustomer';
        $scope.Customer.METERCODE = $scope.selectedValues.join(',');
        var datasend = JSON.stringify({
            customer: $scope.Customer
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "4000" };
                    toastr.success("Cập nhật thành công!");
                    $('.modal-customer').modal('hide');
                    $rootScope.ReloadCustomer();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateCustomer');
            }
            LoadingHide();
        });
    }

    $scope.GetListMeters = function () {
        if (!$scope.Customer.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            $scope.ListMeters = [];
            return false;
        }

        $scope.IsLoading = true;
        var action = '/Meter/GetMeterListByCustaxcode';
        var datasend = JSON.stringify({
            custaxcode: $scope.Customer.CUSTAXCODE
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('.selectpicker').selectpicker({});
                    $scope.ListMeters = response.msg;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 1000);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCurrencyUnit');
            }
        });
        $scope.IsLoading = false;
    }

    /**
     * Gán mã số thuế cho form thêm mới công tơ
     * @param {any} customercode
     */
    $scope.FillCustomerCode = function (customercode) {
        $rootScope.Customercode = customercode;
    }
    $scope.LoadInfoByTaxcode = function () {
        if ($scope.Customer.CUSTAXCODE == null) {
            toastr.warning('vui lòng nhập mã số thuế của khách hàng');
        }
        if ($scope.Customer.CUSTAXCODE != $scope.oldTaxcode && $scope.Customer.CUSTAXCODE != null && $scope.Customer.CUSTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $scope.Customer.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                LoadingShow();
                var rawObj = JSON.parse(response);
                var obj = new Object();
                if (rawObj.Data != "") {
                    obj = JSON.parse(rawObj.Data);
                }
                $timeout(function () {
                    if (obj.companyName) {
                        $scope.Customer.CUSID = obj.companyId;
                        $scope.Customer.CUSADDRESS = obj.address;
                        $scope.Customer.CUSNAME = obj.companyName.toUpperCase();
                    } else {
                        alert("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Customer.CUSNAME = null;
                        $scope.Customer.CUSADDRESS = null;
                    }
                    LoadingHide();
                },1000);
            });
        } else if ($scope.Customer.CUSTAXCODE != $scope.oldTaxcode) {
            $scope.Customer.CUSNAME = null;
            $scope.Customer.CUSADDRESS = null;
        }
    }
}]);