app.controller('SettingsController', ['$scope', '$timeout', 'CommonFactory', '$rootScope', function ($scope, $timeout, CommonFactory, $rootScope) {
    var url = '/Settings/';
    LoadingShow();

    $scope.Tab = {
        Parameter: true,
        Email: false,
        Zalo: false,
    };

    $scope.TabEmail = {
        Account: true,
        Template: false,
    };

    $scope.TabClick = function (tabId) {
        $scope.Tab.Parameter = false;
        $scope.Tab.Email = false;
        $scope.Tab.Zalo = false;
        switch (tabId) {
            case 1:
                $scope.Tab.Parameter = true;
                break;
            case 2:
                $scope.Tab.Email = true;
                $scope.Enterprise.MAILSERVICEID = $scope.Enterprise.MAILSERVICEID.toString();
                $scope.LoadEmailTemplate();
                break;
            case 3:
                $scope.Tab.Zalo = true;
                break;
            default:
                $scope.Tab.Parameter = true;
                break;
        }
    }

    $scope.TabEmailClick = function (tabId) {
        $scope.TabEmail.Account = false;
        $scope.TabEmail.Template = false;
        switch (tabId) {
            case 1:
                $scope.TabEmail.Account = true;
                $scope.Enterprise.MAILSERVICEID = $scope.Enterprise.MAILSERVICEID.toString();
                $timeout(function () {
                    $('select.cb-select-mail').selectpicker();
                })
                break;
            case 2:
                $scope.TabEmail.Template = true;
                $scope.Enterprise.EMAILTEMPLATEID = '1';
                $timeout(function () {
                    $('.cb-select-template').selectpicker();
                })
                break;
        }
    }

    //Biến ví dụ thay đổi tham số làm tròn
    $scope.ExValue = 10000;
    //$scope.EmailType = [
    //    { id: 1, value: '1', text: 'Dịch vụ email NOVAON' },
    //    { id: 2, value: '2', text: 'Gmail' }
    //];

    $scope.IsDisable = false; //Nhập đầy đủ thông tin mới cho lưu cấu hình email
    angular.element(function () {
        LoadingHide();
        $scope.EnableDisableSaveButton();
        $scope.Enterprise.MAILSERVICEID = '2';
        $('select.cb-select-mail').selectpicker('refresh');    
        $('select.cb-select-template').selectpicker('refresh');      
    });

    $scope.EnableDisableSaveButton = function () {
        if ($scope.Enterprise.MAILSERVICEID !== '1') {
            $scope.IsDisable = true;
        }
        else {
            $scope.IsDisable = false;
        }
    }
   
    $scope.GetUserInfo = function () {
        var action = url + 'GetUserInfo';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.User = response.objUser;
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
        });
    }
    $scope.SaveInfoUser = function () {
        var action = url + 'UpdateUser';
        var datasend = JSON.stringify({
            account: $scope.User
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.GetUserInfo();
                    alert("Cập nhật thành công.");
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
            LoadingHide();
        });
    }

    $scope.UserUpdatePassword = function () {
        var action = url + 'UserUpdatePassword';
        var datasend = JSON.stringify({
            account: $scope.User
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Câp nhật thành công.');
                    $scope.User.PASSWORDHIDDEN = '';
                    $scope.User.NEWPASSWORD = '';
                    $scope.User.RENEWPASSWORD = '';
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
            LoadingHide();
        });
    }

    $scope.SaveInfoEnterprise = function () {

        if (!$scope.Enterprise.COMTAXCODE) {
            alert("Vui lòng cập nhật mã số thuế.");
            return false;
        }
        var action = url + 'SaveInfoEnterprise';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Cập nhật thành công!');
                    //$scope.IsEnterpriseNameUpdate = false;
                    //$scope.IsEnterpriseTypeUpdate = false;
                    //$scope.IsEnterpriseInfoUpdate = false;
                    //$scope.IsGeneralInfoUpate = false;
                    //$scope.IsSignatureUpdate = false;
                    location.reload(true);

                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveEnterprise');
            }
            LoadingHide();
        });
    }

    $scope.SaveEmailConfig = function () {
        if (!$scope.Enterprise.MAILSERVICEID) {
            alert("Vui lòng chọn Dịch vụ Email trước khi lưu.");
            return false;
        }
        if (parseInt($scope.Enterprise.MAILSERVICEID) === EMAIL_SERVICE_TYPE.GMAIL) {
            if (!$scope.Enterprise.MAILSERVICEACCOUNT) {
                alert("Vui lòng khai báo tên đăng nhập trước khi lưu.");
                return false;
            }
            if (!validation.isEmailAddress($scope.Enterprise.MAILSERVICEACCOUNT)) {
                alert('Vui lòng nhập đúng định dạng email');
                return false;
            }
            if (!$scope.Enterprise.MAILSERVICEPASSWORD) {
                alert("Vui lòng khai báo mật khẩu trước khi lưu.");
                return false;
            }
        }

        var action = url + 'SaveEmailConfig';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!')
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveEnterprise');
            }
            LoadingHide();
        });
    }

    $scope.CheckSetupEmailConfig = function () {
        var action = url + 'CheckSetupEmailConfig';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                    $scope.IsDisable = false;
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                    $scope.IsDisable = true;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - CheckSetupEmailConfig');
                $scope.IsDisable = true;
            }
            LoadingHide();
        });
    }

    $scope.SaveZaloAccessToken = function () {
        if (!$scope.Enterprise.ZALOACCESSTOKEN) {
            alert("Vui lòng nhập vào access token ứng dụng của bạn.");
            return false;
        }
        var action = url + 'SaveZaloOAAccessToken';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg, "Thành công");
                } else {
                    toastr.error(response.msg, "Thất bại");
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
        });
    }

    $scope.SaveEmailTemplate = function () {
        var action = url + 'SaveEmailTemplate';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg, "Thành công");
                } else {
                    toastr.error(response.msg, "Thất bại");
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
        });
    }

    $scope.LoadEmailTemplate = function () {
        var action = url + 'LoadEmailTemplate';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Enterprise.EMAILTEMPLATECONTENT = response.msg;
                } else {

                }
            } else {

            }
        });
    }

    $scope.RestoreDefault = function () {
        var action = url + 'RestoreDefault';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Enterprise.EMAILTEMPLATECONTENT = response.msg;
                } else {

                }
            } else {

            }
        });
    }

    $scope.ExportExcel = function () {
        LoadingShow();
        if ($scope.IsCategory == false) {
            var action = url + 'ExportExcelIsCategory';
            var datasend = {
                hasProductData: $scope.hasProductData,
                hasCustomerData: $scope.hasCustomerData
            };
        }

        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend,
            preparingMessageHtml: 'Đang tải file vui lòng đợi...',
            failMessageHtml: 'Có lỗi trong khi tải file excel.'
        });
        LoadingHide();
    }

    $scope.uploadFile = function (event) {
        LoadingShow();
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $scope.Enterprise.COMLOGO = event.target.result;
                    LoadingHide();
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $timeout(function () {
        $('select.cb-select-mail').selectpicker();
        $('select.cb-select-template').selectpicker();
    });

    $scope.SaveParameter = function (type) {
        //if (parseInt($scope.Enterprise.QUANTITYPLACE) < 0 || parseInt($scope.Enterprise.PRICEPLACE) < 0 || parseInt($scope.Enterprise.MONEYPLACE) < 0) {
        //    toastr.warning("Tham số làm tròn nằm trong khoảng 0-8.");
        //    return false;
        //}
        //if (parseInt($scope.Enterprise.QUANTITYPLACE) > 8 || parseInt($scope.Enterprise.PRICEPLACE) > 8 || parseInt($scope.Enterprise.MONEYPLACE) > 8) {
        //    toastr.warning("Tham số làm tròn nằm trong khoảng 0-8.");
        //    return false;
        //}
        var msg = '';
        //Khôi phục mặc định
        if (type === 1) {
            $scope.Enterprise.QUANTITYPLACE = 0;
            $scope.Enterprise.PRICEPLACE = 0;
            $scope.Enterprise.MONEYPLACE = 0;
            msg = 'Khôi phục thành công!';
        }
        //cập nhật tham số
        if (type === 2) {
            if ($scope.Enterprise.QUANTITYPLACE === undefined || $scope.Enterprise.PRICEPLACE === undefined || $scope.Enterprise.MONEYPLACE === undefined) {
                toastr.warning("Tham số làm tròn nằm trong khoảng 0-8.");
                return false;
            }
            if ($scope.Enterprise.QUANTITYPLACE === null || $scope.Enterprise.PRICEPLACE === null || $scope.Enterprise.MONEYPLACE === null) {
                toastr.warning("Tham số làm tròn nằm trong khoảng 0-8.");
                return false;
            }
            msg = 'Thay đổi thành công!';
        }
        var action = url + 'SaveParameter';
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveParameter');
            }
            LoadingHide();
        });
    }

    $scope.ShowPass = 0;
    $scope.OpenHidePassword = function () {
        if ($scope.ShowPass == 0) {
            $('.password').attr('type', 'text');
            $('.open-hide-pass').removeClass('fa-eye');
            $('.open-hide-pass').addClass('fa-eye-slash');
            $scope.ShowPass = 1;
        }
        else {
            $('.password').attr('type', 'password');
            $('.open-hide-pass').addClass('fa-eye');
            $('.open-hide-pass').removeClass('fa-eye-slash');
            $scope.ShowPass = 0;
        }
    }
}]);
