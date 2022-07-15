app.controller('ModalReleaseNoticeController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', function ($scope, $rootScope, $timeout, $sce, CommonFactory) {
    var url = '/Invoice/';

    $rootScope.ModalNumber = function () {
        $scope.Number = new Object();
    }

    $scope.AddReleaseNotice = function () {
        if (!$scope.Number) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }
        if (!$scope.Number.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }
        if (!$scope.Number.SYMBOLCODE) {
            alert('Vui lòng nhập đúng ký hiệu theo quy định');
            $('select#symbol-code').focus();
            return false;
        } else {
            if ($scope.Number.SYMBOLCODE.length != 6) {
                alert('Ký hiệu bao gồm 6 ký hiệu, vui lòng kiểm tra lại!');
                $('select#symbol-code').focus();
                return false;
            } else {
                var res = $scope.Number.SYMBOLCODE.split('/');
                if (res[0].length != 2 || !$scope.Number.SYMBOLCODE.endsWith('E')) {
                    alert('Vui lòng nhập đúng ký hiệu theo quy định');
                    $('select#symbol-code').focus();
                    return false;
                }
                if (res.length == 2) {
                    if (res[1].length != 3 || !validation.isNumber(res[1].substring(0, 2))) {
                        alert('Vui lòng nhập đúng ký hiệu theo quy định');
                        $('select#symbol-code').focus();
                        return false;
                    }
                }
            }
        }

        if (!$scope.Number.STRFROMTIME) {
            alert('Vui lòng chọn Ngày bắt đầu sử dụng');
            return false;
        }

        if (!$scope.Number.TONUMBER > $scope.Number.FROMNUMBER) {
            alert('Dải số đến không được nhỏ hơn dải số bắt đầu, vui lòng kiểm tra lại.');
            return false;
        }


        $scope.IsLoading = true;
        var action = url + 'AddReleaseNotice';
        var datasend = JSON.stringify({
            numberWaiting: $scope.Number
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!')
                    $('.modal-number-waiting').modal('hide');
                    location.reload(true);
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.AcceptReleaseNotice = function (id) {
        $scope.IsLoading = true;
        var action = url + 'AcceptReleaseNotice';
        var datasend = JSON.stringify({
            releaseNoticeId: id
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!')
                    location.reload(true);
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

}]);