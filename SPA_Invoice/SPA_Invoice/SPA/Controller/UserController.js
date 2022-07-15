app.controller('UserController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', 'permissions', function ($scope, $rootScope, $timeout, CommonFactory, persistObject, permissions) {
    var url = '/User/';

    /**
        Variables
     */
    $scope.ActiveTab = 1;
    $scope.ListRoleDetail = new Array();
    $scope.ListRoleDetailChecked = new Array();
    $scope.User = null;
    $scope.KEYWORD = '';


    /**
        Method
     */

    $rootScope.ReloadUser = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetUser($scope.currentpage);
    }

    $scope.GetUser = function (page) {
        if (!page) {
            page = 1;
        }
        if (page > $scope.TotalPages) {
            page = $scope.TotalPages;
        }
        $scope.currentpage = page;
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        var action = url + 'GetAllUserByComtaxCode';
        var datasend = JSON.stringify({
            keyword: $scope.KEYWORD,
            page: page
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListUser = response.result;
                    $scope.TotalPages = response.totalPages;
                    $scope.TotalRow = response.totalRow;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetUser');
            }
            LoadingHide();
        });
    }

    $scope.GetRole = function () {
        var action = url + 'GetRole';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListRole = response.result;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetRole');
            }
        });
    }
    $scope.GetRole();

    $scope.GetRoleDetail = function () {
        var action = url + 'GetRoleDetail';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListRoleDetail = response.result;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetRole');
            }
        });
    }
    $scope.GetRoleDetail();

    $scope.GetRoleDetailByUserId = function (email) {
        var action = url + 'GetRoleDetailByUserId';
        var datasend = JSON.stringify({
            email: email
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListRoleDetailChecked = response.result;
                    $scope.ListRoleDetailChecked.forEach((item) => {
                        $scope.ListRoleDetail.filter((item1) => {
                            if (item1.ID === item.ROLEDETAILID) {
                                item1.ISSELECTED = true;
                            }
                        })
                    })
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetRoleDetailByUserId');
            }
            LoadingHide();
        });
    }

    $scope.UpdateUserRole = function (email) {
        if (!$scope.ISEDIT && !$scope.User.EMAIL) {
            toastr.warning('Không thể phân quyền cho người người dùng chưa tạo.', "Cảnh báo", { timeOut: 2000 })
            return;
        }
        $scope.ListRoleDetailChecked.forEach(function (ele) {
            ele.USERNAME = $scope.User.EMAIL;
        })
        var action = url + 'UpdateUserRole';
        var datasend = JSON.stringify({
            email: $scope.User.EMAIL,
            userRoleDetail: $scope.ListRoleDetailChecked
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg, "Thành công", { timeOut: 2000 })
                } else {
                    toastr.error(response.msg, "Thất bại", { timeOut: 2000 })
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetRoleDetailByUserId');
            }
            LoadingHide();
        });
    }

    $scope.AddUser = function () {
        if (!$scope.User.EMAIL) {
            toastr.warning('Vui lòng nhập vào email.', "Cảnh báo", { timeOut: 2000 })
            return;
        }
        if (!$scope.ISEDIT && !$scope.User.PASSWORD) {
            toastr.warning('Vui lòng nhập vào mật khẩu.', "Cảnh báo", { timeOut: 2000 })
            return;
        }
        var action = url + 'AddUser';
        if (!$scope.ISEDIT)
            action = url + 'AddUser';
        else
            action = url + 'UpdateUser';
        var datasend = JSON.stringify({
            registerForm: $scope.User
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg, "Thành công", { timeOut: 2000 })
                    $scope.GetUser(1);
                } else {
                    toastr.error(response.msg, "Thất bại", { timeOut: 2000 })
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddUser');
            }
            LoadingHide();
        });
    }

    $scope.DeleteUser = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            else {
                var action = url + 'DeleteUser';
                var datasend = JSON.stringify({
                    account: item
                });
                LoadingShow();
                CommonFactory.PostDataAjax(action, datasend, function (response) {
                    if (response) {
                        if (response.rs) {
                            toastr.success(response.msg, "Thành công", { timeOut: 2000 })
                            $scope.GetUser(1);
                        } else {
                            toastr.error(response.msg, "Thất bại", { timeOut: 2000 })
                        }
                    } else {
                        alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeleteUser');
                    }
                    LoadingHide();
                });
            }
        };
        confirm("Bạn chắc chắn muốn xóa người dùng này không " + item.EMAIL + "?", "Xóa người dùng", "Không", "Có", confirmContinue)      
    }

    $scope.ShowActiveTab = function (itemId) {
        $scope.ActiveTab = itemId;
    }

    $scope.SelectCheckbox = function (isSelect, roleDetailId) {
        var roleDetail =
        {
            "USERNAME": $scope.User.EMAIL,
            "ROLEDETAILID": roleDetailId
        };
        if (isSelect) {
            const index = $scope.ListRoleDetailChecked.findIndex(x => x.ROLEDETAILID === roleDetailId);
            if (index > -1) {
                $scope.ListRoleDetailChecked.splice(index, 1);
            }
        }
        else {
            $scope.ListRoleDetailChecked.push(roleDetail);
        }
    }

    $scope.ModalAddEditUser = function (item, mode) {
        $scope.ListRoleDetail.filter((item) => {
            item.ISSELECTED = false;
        })
        if (mode === 2) {
            $scope.GetRoleDetailByUserId(item.EMAIL);
        }
        $scope.ISEDIT = false;
        $scope.FormTitle = "Thêm người dùng";
        if (mode == 2) {
            $scope.FormTitle = "Cập nhật thông tin người dùng";
            $scope.ISEDIT = true;
            item.PASSWORD = '';
        }
        $('#modal-addedit-user').dialog({
            title: $scope.FormTitle,
            width: '85%',
            //position: { my: 'center', at: 'top' },
            modal: true,
            resizable: false,
            scope: $scope,
            permissions: permissions,
            show: {
                effect: 'fade',
                direction: 'top',
                duration: 100
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal-addedit-user').show();
            },
            close: function (event, ui) {
                $scope.ListRoleDetailChecked = [];
            }
        });

        $scope.User = new Object();
        angular.copy(item, $scope.User);
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

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

}]);
