app.controller('ModalImportInvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';
    if (!$rootScope.Enterprise)
        $rootScope.GetEnterpriseInfo();
    angular.element(function () {
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        $scope.Import = {};
        $scope.Import.ImportInvoiceType = 0;
        $scope.ListMap = {};
        $scope.Import.HeaderRow = 1;
        $scope.Import.ShowSelectSheetAndHeader = false;
    });

    $scope.listColumnError = [];

    $scope.ResetAllStep = function () {
        $scope.stepOne = true;
        $scope.stepTwo = false;
        $scope.stepThree = false;
        $scope.ListData = null;
        $scope.IsDiffent = true;
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

    $scope.ModalImportInvoice = function (item) {
    }

    $scope.revmoveItem = function (value) {
        var val = value;
        $($scope.ListData).each(function (index, item) {
            if (item.YOURFIELD === val) {
                item.YOURFIELD = null;
            }
        });

        $scope.ListData = $scope.ListData;
    }

    $scope.changeDataMapping = function () {
        $scope.listColumnError = [];
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    $scope.ImportInvoiceFromExcel = function () {
        $('#fileUploadInvoice').trigger('click');
    };

    $('#frmImportInvoice').fileupload({
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
                            $scope.Import.FORMCODE = $rootScope.ListFormCode[0].FORMCODE + '-' + $rootScope.ListFormCode[0].SYMBOLCODE;
                            $scope.Import.SYMBOLCODE = $rootScope.ListFormCode[0].SYMBOLCODE;
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
        if (!$scope.Import.FORMCODE) {
            alert("Bạn chưa chọn mẫu số.");
            return false;
        }
        if (!$scope.Import.SYMBOLCODE) {
            alert("Bạn chưa chọn ký hiệu.");
            return false;
        }
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

        var formCodeSymbodeCode = $scope.Import.FORMCODE.split('-');

        var action = url + 'MappingColumnExcel';
        var datasend = JSON.stringify({
            fromCode: formCodeSymbodeCode[0],
            symbolCode: formCodeSymbodeCode[1],
            selectedSheet: $("#ddlSheet").val(),
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
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ImportExcelMappingData');
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

        var formCodeSymbodeCode = $scope.Import.FORMCODE.split('-');
        var data = JSON.stringify($scope.ClientData);
        var action = url + 'PreviewInvoiceData';

        successCallback = function (data) {
            $scope.ImportInvoices = data.ListInvoices;
            $scope.TotalInvoiceMoneyPayment = data.TotalInvoiceMoney;
            $timeout(function () {
                $scope.stepThree = true;
                $scope.stepTwo = false;
            }, 100);
        }
        AjaxRequest(action, { listMap: lst, formCode: formCodeSymbodeCode[0], symbolCode: formCodeSymbodeCode[1] }, "POST", true, "json", successCallback);
    }


    $scope.SaveInvoiceList = function () {
        if (!$scope.ImportInvoices) {
            alert("Bạn chưa tải file lên. Vui lòng tải chọn file tải lên.");
        }
        $scope.EnableButton(false);
        action = url + 'ImportDataInvoice';
        successCallback = function (data) {
            //toastr.success(data.msg);
            ////$timeout(function () {
            ////    location.reload(true);
            ////}, 2000);
            $('.import-invoice').modal('hide');
            //$rootScope.ReloadInvoice();
            $scope.ResetAllStep();
        }
        AjaxRequest(action, {}, "POST", true, "json", successCallback);

    };
    $scope.EnableButton = function (isEnable) {
        $("#modalFooterInv").find('button').each(function () {
            $(this).prop('disabled', !isEnable);
        });
    };
    //call  backroundjob
    $scope.BackroundJodSinglR = function () {
        var invoiceHub = $.connection.signlRConf;
        var userid = sessionStorage.getItem("userNameSS");

        $.connection.hub.qs = { 'username': userid };
        var invRow = 0;
        invoiceHub.client.newMessageReceived = function (message) {
            invRow = invRow + 1;
            console.log(invRow);
            if (invRow === message.totalRow) {

                var messageSuccess = 'Thêm mới thành công <b>' + invRow + '/' + message.totalRow + '</b> hóa đơn.';
                $('.invoiceResult').html("Hoàn Thành");
                $("#progressTab").hide();
                toastr.success(messageSuccess);
                $scope.EnableButton(true);
                invRow = 0;
                var confirmContinue = function (result) {
                    if (result) {
                        $scope.ActionAfterImport();
                        return false;
                    }
                    else {
                        $scope.ActionAfterImport();
                        $rootScope.ReloadInvoice();
                    }
                };
                confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Thêm mới hóa đơn', 'Có', 'Không', confirmContinue);
            }
            else {
                $("#progressTab").show();
                var percent = Math.floor((invRow / message.totalRow) * 100);
                $('.invoiceResult').html('Đang tải hóa đơn: ' + percent + '%');
            }

        };
        //$.connection.hub.start().done(function () {
        //    $("#progressTab").hide();
        //});
    };
    $scope.BackroundJodSinglR();

    $scope.ActionAfterImport = function () {
        var action = url + 'ActionAfterImport';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (response.msg) {
                        alert('Tải file lỗi <a href="' + response.msg + '" download="OnFinance_error_import_file.txt">tại đây</a>');
                    }
                    console.log(response.msg);
                } else {
                    console.log(response.msg);
                }
            } else {
                console.log(response.msg);
            }
        });
    }
    /**
     * truongnv 2020030203
     * @param {any} value
     */
    $scope.SetRequiedTaxCode = function (value) {
        $scope.Import.ImportInvoiceType = value;
    }

    $scope.Sum = function (items, prop) {
        return items.reduce(function (a, b) {
            return a + b[prop];
        }, 0);
    };
}]);

/**
 * truongnv 20200404
 * @param {any} url
 * @param {any} parameters
 * @param {any} type 
 * @param {any} async 
 * @param {any} dataType: 
 * @param {any} successCallback
 * @param {any} errorCallback
 */
function AjaxRequest(url, parameters, type, async, dataType, successCallback, errorCallback) {
    LoadingShow();
    $.ajax({
        url: url == undefined ? "" : url,
        data: parameters == undefined ? "" : JSON.stringify(parameters),
        type: type == undefined ? 'POST' : type,
        async: async == undefined ? true : async,
        contentType: 'application/json; charset=utf-8',
        dataType: (dataType == undefined ? 'json' : dataType),
        success: function (data) {
            if (data.hasOwnProperty("rs") && data.hasOwnProperty("msg")) { // trả về object Result
                if (!data.rs) {
                    if (errorCallback !== undefined && typeof errorCallback === "function") {
                        errorCallback(data);
                        return;
                    }
                    alert(data.msg);
                }
                else if (successCallback !== undefined && typeof successCallback === "function") successCallback(data);
            } else
                if (successCallback !== undefined && typeof successCallback === "function") successCallback(data);

            LoadingHide();
        },
        error: function (xhr, textStatus, errorThrown) {
            LoadingHide();
            if (errorCallback !== undefined && typeof errorCallback === "function") errorCallback();

            if (xhr.status == 403 || xhr.status == 500)//Hết session khi gọi ws
            {
                alert('Đã hết phiên làm việc. Bạn bấm <b>Đăng nhập lại</b> để trở về trang đăng nhập.');
                return;
            }
            console.log(xhr);
            var message = xhr.responseText != undefined ? xhr.responseText : textStatus;
            alert('Có lỗi xảy ra. Xin vui lòng thử lại sau hoặc thông báo với quản trị (Error: ' + message + ').');
        }
    });
}

/**
 * Kiểm tra kiểu file và dung lượng file tải lên
 * truongnv 20200219
 * @param {any} files
 */
function CheckFile(files) {
    var arrFiles = new Array();
    var allowFileExtension = ".xls,.xlsx";
    var arrAllowFileExtension = allowFileExtension.split(",");
    if (files.length > 0) {
        var sumSize = 0;
        var sumSizeAllow = 5 * 1024 * 1024;
        for (var i = 0; i < files.length; i++) {
            var fileName = files[i].name;
            var fileExtension = fileName.substr(fileName.lastIndexOf('.'));
            var isAllowFileExtension = false;
            for (var j = 0; j < arrAllowFileExtension.length; j++) {
                var fileExtensionAllow = arrAllowFileExtension[j];
                if (fileExtension.toLowerCase() == fileExtensionAllow) {
                    isAllowFileExtension = true;
                    break;
                }
            }

            if (!isAllowFileExtension) {
                alert("Bạn không được chọn file định dạng " + fileExtension);
                return false;
            }

            var fileSize = files[i].size;
            if (fileSize > sumSizeAllow) {
                alert("File <b>" + files[i].name + "</b> có dung lượng <b>" + GetSizeName(fileSize) + "</b>. Bạn không được chọn file có dung lượng lớn hơn <b>5 MB</b>.");
                return false;
            }

            sumSize = sumSize + fileSize;
        }

        for (var i = 0; i < arrFiles.length; i++)
            sumSize = sumSize + arrFiles[i].size;

        if (sumSize > sumSizeAllow) {
            alert("Tổng dung lượng các file bạn đã chọn là <b>" + GetSizeName(sumSize) + "</b>. Bạn không được Attach file có tổng dung lượng lớn hơn <b>5 MB</b>.");
            return false;
        }

        for (var i = 0; i < files.length; i++)
            arrFiles.push(files[i]);

        return true;
    }

    /**
     * Lấy độ lớn của file
     * @param {any} size
     */
    function GetSizeName(size) {
        if (size < 1024 * 1024)
            return Math.round(size / 1024) + " KB";
        else
            return Math.round(size / (1024 * 1024)) + " MB";
    }
}