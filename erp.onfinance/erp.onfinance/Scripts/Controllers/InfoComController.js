app.controller('InfoComController', ['$scope', '$timeout', '$filter', 'CommonFactory', function ($scope, $timeout, $filter, CommonFactory) {
    var url = '/InfoCom/';

    $scope.currentPage = 1;

    $scope.Init = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }

        var action = url + "Init";
        var datasend = JSON.stringify({
            keyword: null,
            itemPerPage: 20,
            currentPage: intpage
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCompany = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCompany");
            }
        });
    };

    $scope.BackToListCompany = function () {
        $scope.Company = new Object();
        $scope.Company.ISEDIT = false;
    };

    $scope.ReloadDataPage = function () {
        $scope.GetAllReleaseNotice($scope.currentPage);
        $scope.GetAllUserByComtaxCode($scope.currentPage);
    }


    $scope.EditCompany = function (item) {
        $scope.Company = new Object();
        $scope.Company.ISNEW = true;
        if (item) angular.copy(item, $scope.Company);
        $timeout(function () {
            $scope.Company.ISEDIT = true;
            $scope.ReloadDataPage();
        }, 100);
    };

    $scope.SaveCompany = function () {
        debugger;
        if (!$scope.Company) {
            alert('Vui lòng nhập vào thông tin doanh nghiệp.');
            return;
        }
        if (!$scope.Company.COMNAME) {
            alert('Vui lòng nhập tên doanh nghiệp.');
            return;
        }
        //if (!$scope.Company.EMAIL) {
        //    alert('Vui lòng nhập địa chỉ email.');
        //    return;
        //}
        if (!$scope.Company.COMTAXCODE) {
            alert('Vui lòng nhập mã số thuế doanh nghiệp.');
            return;
        }
        if (!$scope.Company.COMADDRESS) {
            alert('Vui lòng nhập địa chỉ doanh nghiệp.');
            return;
        }
        //if (!$scope.Company.COMPHONENUMBER) {
        //    alert('Vui lòng nhập số điện thoại doanh nghiệp.');
        //    return;
        //}

        var action = url + "UpdateEnterpriseInfo";
        var datasend = JSON.stringify({
            enterprise: $scope.Company
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!');
                    $scope.GetCompany(1);

                    //Tạo user mặc định
                    if ($scope.Company.ISNEW) {
                        $scope.User = {};
                        $scope.User.EMAIL = $scope.Company.EMAIL;
                        $scope.User.USERNAME = $scope.Company.EMAIL;
                        $scope.User.PHONENUMBER = $scope.Company.COMPHONENUMBER;
                        $scope.User.COMTAXCODE = $scope.Company.COMTAXCODE;
                        $scope.User.PASSWORD = $scope.Company.COMTAXCODE;
                        $scope.SaveUser("auto");
                    }

                    //Clear form & back về list
                    $scope.Company = new Object();
                    $scope.Company.ISEDIT = false;
                } else {
                    alert('Thất bại, vui lòng thử lại!');
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCompany");
            }
        });
    };
    //$scope.GetListProducts = function () {
    //    $scope.IsLoading = true;
    //    var action = '/Product/GetListProductByComtaxCode';
    //    var datasend = JSON.stringify({
    //        comtaxcode: $rootScope.Enterprise.COMTAXCODE
    //    });

    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $('.selectpicker').selectpicker({});
    //                $scope.ListProducts = response.msg;
    //                $timeout(function () {
    //                    $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
    //                }, 1000);
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetListProducts');
    //        }
    //    });
    //    $scope.IsLoading = false;
    //}
    $scope.GetAllUserByComtaxCode = function (currentPage) {
        var action = url + "GetAllUserByComtaxCode";
        var datasend = JSON.stringify({
            comtaxcode: $scope.Company.COMTAXCODE
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListUser = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetAllUserByComtaxCode");
            }
        });
    };

    $scope.GetAllReleaseNotice = function (intpage) {
        $scope.FormNumber = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.FormNumber.COMTAXCODE = $scope.Company.COMTAXCODE;
        $scope.FormNumber.ITEMPERPAGE = 10;
        $scope.FormNumber.CURRENTPAGE = intpage;
        var action = url + 'GetAllReleaseNotice';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        $scope.ListNumber = new Array();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListNumber = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetAllReleaseNotice');
            }
        });
    };

    $scope.EditUser = function (item) {
        $scope.User = new Object();
        $scope.User.ISNEW = true;
        $scope.User.COMTAXCODE = $scope.Company.COMTAXCODE;
        if (item) angular.copy(item, $scope.User);
        $scope.User.PASSWORD = '';
        $scope.User.ISEDIT = true;

        $('#modal_user').dialog({
            width: '25%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            //show: {
            //    effect: 'drop',
            //    direction: 'right',
            //    duration: 300
            //},
            //hide: {
            //    effect: 'fade',
            //    duration: 200
            //},
            create: function (event, ui) {
                $('#modal_user').show();
            }
        });
    };

    $scope.EditReleaseNotice = function (item) {
        $scope.ReleaseNotice = new Object();
        $scope.ReleaseNotice.COMTAXCODE = $scope.Company.COMTAXCODE;
        $scope.ReleaseNotice.STATUS = 4;
        if (item) angular.copy(item, $scope.ReleaseNotice);
        $scope.ReleaseNotice.STRFROMTIME = $filter('dateFormat')($scope.ReleaseNotice.FROMTIME, 'dd/MM/yyyy');


        $('#modal_release_notice').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            //show: {
            //    effect: 'drop',
            //    direction: 'right',
            //    duration: 300
            //},
            //hide: {
            //    effect: 'fade',
            //    duration: 200
            //},
            create: function (event, ui) {
                $('#modal_release_notice').show();
            }
        });
    };

    $scope.CloseModalDialog = function () {
        $scope.User = new Object();
        $scope.ReleaseNotice = new Object();

        $('#modal_user').dialog('close');
        $('#modal_release_notice').dialog('close');
    };

    $scope.SaveReleaseNotice = function (item) {
        var action = url + 'AddReleaseNotice';
        var datasend = JSON.stringify({
            invoiceNumber: $scope.ReleaseNotice
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công');
                    $('#modal_release_notice').dialog('close');
                    $scope.ReloadDataPage();
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveReleaseNotice');
            }
        });
    };

    $scope.SaveUser = function (user) {
        var action = url + 'AddUser';
        var datasend = JSON.stringify({
            account: user
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (typeof item !== "string") {
                if (response) {
                    if (response.rs) {
                        alert('Thành công');
                        $('#modal_user').dialog('close');
                        $scope.ReloadDataPage();
                    } else {
                        $scope.ErrorMessge = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveUser');
                }
            }
        });
    };

    $scope.DeleteUser = function (user) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
        };
        confirm('Sau khi xóa user khỏi doanh nghiệp <b>' + user.COMTAXCODE + '</b> sẽ trở về mặc định thuộc mã số thuế <b>0106579683-999</b>. Bấm <b>Đồng ý</b> để chuyển sang mặc định, bấm <b>Bỏ qua</b> để giữ nguyên trạng thái', 'Set User Default', 'Bỏ qua', 'Đồng ý', confirmContinue);


        user.COMTAXCODE = '0106579683-999';
        var action = url + 'AddUser';
        var datasend = JSON.stringify({
            account: user
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công');
                    $scope.ReloadDataPage();
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeleteUser');
            }
        });
    }
}]);