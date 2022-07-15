app.controller('ModalNumberWaitingController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter) {
    var url = '/Invoice/';


    $rootScope.ModalNumberWaiting = function (item, type) {
        $scope.Number = new Object();
        $scope.TYPECHANGE = type;
        if (type === 2) {
            $timeout(function () {
                angular.copy(item, $scope.Number);
            }, 300)
        }
    }

    $scope.GetNumber = function () {
        if (!$scope.Number.FORMCODE) 
            return;
        let formCodeSymbodeCode = $scope.Number.FORMCODE.split('-');
        $scope.FormNumber = new Object();
        $scope.FormNumber.FORMCODE = formCodeSymbodeCode[0];
        $scope.FormNumber.SYMBOLCODE = formCodeSymbodeCode[1];
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.result[0];
                    $scope.CurrentNumberWaiting = result;
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER + 1;
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert($scope.ErrorMessage);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumberaa');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.AddNumberWaiting = function (type) {   
        if (!$scope.Number.FORMCODE) {
            alert('Vui lòng chọn mẫu số, ký hiệu hóa đơn.');
            return false;
        }
        //if (!$scope.Number.SYMBOLCODE) {
        //    alert('Vui lòng chọn ký hiệu hóa đơn.');
        //    return false;
        //}

        if ($scope.Number.FROMNUMBER < $scope.CurrentNumberWaiting.FROMNUMBER) {
            alert('Số bắt đầu của dải chờ ' + $scope.Number.FROMNUMBER + ' phải lớn hơn hoặc bằng số bắt đầu của dải số ' + $scope.CurrentNumberWaiting.FROMNUMBER);
            return false;
        }

        if ($scope.Number.FROMNUMBER < $scope.Number.CURRENTNUMBER) {
            alert('Số bắt đầu của dải chờ phải lớn hơn hoặc bằng số hiện tại.');
            return false;
        }

        if ($scope.Number.TONUMBER < $scope.Number.FROMNUMBER) {
            alert('Số đến của dải chờ phải lớn hơn hoặc bằng số bắt đầu.');
            return false;
        }

        if ($scope.Number.TONUMBER > $scope.CurrentNumberWaiting.TONUMBER) {
            alert('Số đến của dải chờ ' + $scope.Number.TONUMBER + ' phải nhỏ hơn hoặc bằng số đến của dải số ' + $scope.CurrentNumberWaiting.TONUMBER);
            return false;
        }

        var formCodeSymbodeCode = $scope.Number.FORMCODE.split('-');
        $scope.Number.FORMCODE = formCodeSymbodeCode[0];
        $scope.Number.SYMBOLCODE = formCodeSymbodeCode[1];

        var action = url + 'AddNumberWaiting';
        var datasend = JSON.stringify({
            numberWaiting: $scope.Number
        });
        LoadingShow();
        
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-number-waiting').modal('hide');
                    $rootScope.GetInvoiceWaiting(1);
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
        LoadingHide();
    }

    $scope.UpdateNumberWaiting = function () {
        if (!$scope.Number.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn.');
            $('select#form-code').focus();
            return false;
        }
        if (!$scope.Number.SYMBOLCODE) {
            alert('Vui lòng chọn kí hiệu hóa đơn.');
            $('select#symbol-code').focus();
            return false;
        }

        if ($scope.Number.FROMNUMBER < $scope.Number.CURRENTNUMBER) {
            alert('Số bắt đầu của dải chờ phải lớn hơn hoặc bằng số hiện tại.');
            $('select#symbol-code').focus();
            return false;
        }

        if ($scope.Number.TONUMBER < $scope.Number.FROMNUMBER) {
            alert('Số đến của dải chờ phải lớn hơn hoặc bằng số bắt đầu.');
            $('select#symbol-code').focus();
            return false;
        }

        var action = url + 'UpdateNumberWaiting';
        var datasend = JSON.stringify({
            numberWaiting: $scope.Number
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-number-waiting').modal('hide');
                    $rootScope.GetInvoiceWaiting();
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