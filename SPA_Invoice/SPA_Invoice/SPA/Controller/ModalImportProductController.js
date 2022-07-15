app.controller('ModalImportProductController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Product/';

    if (!$rootScope.Enterprise)
        $rootScope.GetEnterpriseInfo();
    angular.element(function () {
        $scope.Import = {};
        $scope.ListMap = {};
        $scope.Import.HeaderRow = 1;
        $scope.Import.ShowSelectSheetAndHeader = false;
    });

    $scope.ShowStepThree = function () {
        $scope.stepOne = false;
        $scope.stepTwo = false;
        $scope.stepThree = true;
    }

    $scope.ImportBackToStep = function (step) {
        $scope.stepOne = false;
        $scope.stepTwo = false;
        $scope.stepThree = false;
        switch (step) {
            case 1: {
                $scope.stepOne = true;
            } break;
            case 2: {
                $scope.stepTwo = true;
            } break;
        }
    }

    $scope.ResetAllStep = function () {
        $scope.stepOne = true;
        $scope.stepTwo = false;
        $scope.stepThree = false;
        $scope.ListData = null;
        $scope.IsDiffent = true;
    }

    $rootScope.ModalImportProduct = function (item) {
    }

    $scope.ImportProductFromExcel = function () {
        $('#fileUploadProduct').trigger('click');
    };

    $('#frmImportProduct').fileupload({
        autoUpload: false,
        add: function (e, data) {
            if (data.files[0].type.match('image.*')) {
                return;
            }

            //Kiểm tra định dạng và dung lượng file
            var isPass = CheckFile(data.files);
            if (!isPass) return false;

            LoadingShow();
            var fileData = new FormData();
            fileData.append("file0", data.files[0]);
            var action = url + "ReadFileExcel";

            $.ajax({
                type: "POST",
                url: action,
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    $timeout(function () {
                        if (result.rs) {
                            $scope.Import.FileName = result.fileName;
                            $scope.Import.ListSheets = JSON.parse(result.listSheet);
                            $scope.Import.SelectedSheet = [$scope.Import.ListSheets[0]];
                            var $divSheet = $("#ddlSheet");
                            $divSheet.attr("value", $scope.Import.SelectedSheet.Index);
                            $scope.Import.UploadMessage = result.reponseText;
                            $scope.Import.ShowSelectSheetAndHeader = true;
                        } else {
                            alert(result.msg);
                        }
                        LoadingHide();
                    })
                },
                error: function (xhr, status, p3, p4) {
                    alert("Lỗi không thể đọc file excel.");
                    LoadingHide();
                }
            });
        }
    });

    $scope.ImportExcelMappingData = function () {
        $scope.listColumnError = [];
        if (!$scope.Import.FileName) {
            alert("Bạn chưa chọn file.");
            return false;
        }
        if (!$scope.Import.SelectedSheet) {
            alert("Bạn chưa chọn sheet dữ liệu.");
            return false;
        }
        if (!$scope.Import.HeaderRow) {
            alert("Bạn chưa chọn thứ tự dòng tiêu đề.");
            return false;
        }
        var action = url + 'MappingColumnExcel';
        var datasend = JSON.stringify({
            selectedSheet: $scope.Import.SelectedSheet === undefined ? 0 : $scope.Import.SelectedSheet[0].Index,
            headerRow: $scope.Import.HeaderRow
        });

        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListData = response.data;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 100);
                    $scope.stepOne = false;
                    $scope.stepTwo = true;
                    $scope.ClientData = response.clientData;
                    $scope.lstYourField = response.lstYourField;
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveInvoiceList');
            }
            LoadingHide();
        });
    }

    $scope.ImportExcelPreviewData = function () {
        var lst = JSON.stringify($scope.ListMap);

        // Kiểm tra các cột required có được map hay không
        var lstRequired = $scope.ListData.filter(function (obj) {
            return (obj.YOURFIELD === null) && (obj.Required === true);
        });

        if (lstRequired && Object.keys(lstRequired).length > 0) {
            $(lstRequired).each(function (index, item) {
                if (!$scope.ListMap[item.MYFIELD]) {
                    $scope.listColumnError.push(item.MYFIELD);
                }
            });
        }
        if ($scope.listColumnError.length > 0) return false;

        var data = JSON.stringify($scope.ClientData);
        var action = url + 'PreviewProductData';
        successCallback = function (data) {
            $scope.ImportProducts = data.ListProducts;
            $timeout(function () {
                $scope.stepThree = true;
                $scope.stepTwo = false;
            }, 100);
        }
        AjaxRequest(action, { listMap: lst}, "POST", true, "json", successCallback);
    }

    $scope.SaveProductList = function () {
        if (!$scope.ImportProducts) {
            alert("Bạn chưa tải file lên. Vui lòng tải chọn file tải lên.");
        }
        var action = url + 'ImportDataProduct';
        successCallback = function (data) {
            //if ($scope.count === 1) {
            //    toastr.success(data.msg);
            //    $timeout(function () {
            //        location.reload(true);
            //    }, 4000);
            //}
            //$scope.count ++;
            $(".import-product").modal('hide');
            $scope.ResetAllStep();
        }
        AjaxRequest(action, {}, "POST", true, "json", successCallback);
    }

    // Background service job
    $scope.BackgroundJobServiceProduct = function () {
        var invoiceHub = $.connection.signlRConf;
        var userid = sessionStorage.getItem("userNameSS");

        $.connection.hub.qs = { 'username': userid };
        var invRow = 0;
        invoiceHub.client.newMessageReceivedProduct = function (message) {
            invRow = invRow + 1;
            console.log(invRow);
            if (invRow === message.totalRow) {

                var messageSuccess = 'Thêm mới thành công <b>' + invRow + '/' + message.totalRow + '</b> hàng hóa.';
                $('.invoiceResult').html("Hoàn Thành");
                $("#progressTab").hide();
                toastr.success(messageSuccess);
                invRow = 0;
                var confirmContinue = function (result) {
                    if (result)
                        return false;
                    $rootScope.ReloadProduct();
                };
                confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Thêm mới hàng hóa', 'Có', 'Không', confirmContinue);
            }
            else {
                $("#progressTab").show();
                var percent = Math.floor((invRow / message.totalRow) * 100);
                $('.invoiceResult').html('Đang tải hàng hóa: ' + percent + '%');
            }

        };
        //$.connection.hub.start().done(function () {
        //    $("#progressTab").hide();
        //});
    };
    $scope.BackgroundJobServiceProduct();

}]);