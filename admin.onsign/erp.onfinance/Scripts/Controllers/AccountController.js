app.controller('AccountController', ['$scope', '$timeout', 'CommonFactory', function ($scope, $timeout, CommonFactory) {
    var url = '/Account/';

    $scope.Login = function () {
        if (!$scope.UserName) {
            $scope.ErrorMessge = "Vui lòng cho biết tên đăng nhập của bạn.";
            return false;
        }
        if (!$scope.Password) {
            $scope.ErrorMessge = "Vui lòng nhập mật khẩu của bạn.";
            return false;
        }

        var action = url + "UnauthorisedRequest";
        var datasend = JSON.stringify({
            username: $scope.UserName,
            password: $scope.Password
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    window.location.href = "/";
                } else {
                    $scope.isLoading = false;
                    $scope.ErrorMessge = response.msg;
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
        });
    }

    $scope.PressEnter = function ($event) {
        if ($event.keyCode == 13) {
            $scope.Login();
        }
    }

    $scope.ChangePass = function () {
        if ($scope.password == null || $scope.password == "") {
            alert("Vui lòng nhập mật khẩu hiện tại của bạn.");
            return false;
        }
        if ($scope.newpassword == null || $scope.newpassword == "") {
            alert("Vui lòng nhập mật khẩu mới.");
            return false;
        }
        if ($scope.renewpassword == null || $scope.renewpassword == "") {
            alert("Vui lòng nhập lại mật khẩu mới.");
            return false;
        }

        var action = url + "ChangePassword";
        var datasend = JSON.stringify({
            password: $scope.password,
            newpassword: $scope.newpassword,
            renewpassword: $scope.renewpassword
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                    window.location.href = "/";
                } else {
                    $scope.ErrorMessge = response.msg;
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Login");
            }
            $scope.isLoading = false;
        });
    }
}]);