app.controller('TempInvoiceController', ['$scope', '$rootScope', '$routeParams', '$timeout', 'CommonFactory', function ($scope, $rootScope, $routeParams, $timeout, CommonFactory) {
    var url = '/TempInvoice/';

    LoadingHide();

    var urlLoadView = '/TempInvoice/ViewTemplate/?id=';
    var iframe = $('#iframe_templateViewInvoice');
    var TEMPLATEPATH = null;
    if ($routeParams.id) {
        //Giải mã url để biết edit template nào
        TEMPLATEPATH = atob($routeParams.id);
        //Lấy thông tin đối tượng cần edit
        //$rootScope.EditingTemplate
    }

    //Hidden left menu bar
    //$('#sidebar').trigger('click');

    //Mặc định nhiều thuế suất
    $scope.InvoiceType = {
        value: "1",    //Default value
        Options: [
            { value: "1", text: 'Mẫu một thuế suất' },
            { value: "2", text: 'Mẫu nhiều thuế suất' },
            { value: "3", text: 'Hóa đơn bán hàng' },
            { value: "4", text: 'Hóa đơn tiền điện' },
            { value: "5", text: 'Hóa đơn tiền nước' },
            { value: "6", text: 'Hóa đơn trường học' },
            { value: "7", text: 'Phiếu xuất kho' }
        ]
    };

    $timeout(function () {
        //khổ giấy
        var vVal = 0;
        if ($rootScope.Enterprise) {
            vVal = $rootScope.Enterprise.USINGINVOICETYPE;
        }
        var defaultVal = "1";
        var optionArr = [
            { value: "1", text: 'Khổ giấy A4' },
            { value: "2", text: 'Khổ giấy A5' }
        ];
        if (vVal === 1 || vVal === 4) {
            defaultVal = "2";
            optionArr = [
                { value: "2", text: 'Khổ giấy A5' }
            ];
        }
        $scope.SizePaper = {
            value: defaultVal,    //Default value
            Options: optionArr
        };
    });

    //Load to complete
    var LoadWhileComplete = function () {
        LoadingShow();
        $timeout(function () {
            var csstext = iframe.contents().find('style').text();
            if (csstext != null && csstext != '') {
                $.parsecss(csstext, function (cssjson) {
                    $timeout(function () {
                        var FORMCODE = iframe.contents().find('#form_invoice_template').text();
                        var SYMBOLCODE = iframe.contents().find('#symbol_invoice_template').text();
                        var f = FORMCODE.split('/');

                        $scope.template = {
                            fontSize: "16px",
                            fontFamily: "Times New Roman",
                            SYMBOLCODE: SYMBOLCODE,
                            FORM: f[0] + "/",
                            CODE: f[1],
                            FORMCODE: FORMCODE,
                            logo: "",
                            bg: "",
                            status: 4,
                            borderRadius: "0px",
                            borderWidth: "0px",
                            style: cssjson
                        };
                        //Nếu chọn khổ giấy là A5 thì thiết lập lại style cho form
                        if ($rootScope.Enterprise) {
                            if ($scope.SizePaper.value === "2" || $rootScope.Enterprise.USINGINVOICETYPE === 1 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                                $timeout(function () {
                                    $('#viewPreviewTemplate')[0].setAttribute('style', 'width: 760px;height: 500px;');
                                    var iFrameContent = $('#iframe_templateViewInvoice').contents();
                                    iFrameContent.find('img.temp')[0].setAttribute('style', 'position: absolute; width: 200px; top: 45%; left: calc(50% - 100px); transform: rotate(-30deg);');
                                    iFrameContent.find('body')[0].setAttribute('style', 'height: 720px;background-size: 70%;');
                                    $('#iframe_templateViewInvoice')[0].setAttribute('style', 'width: 1100px;height: 720px;');
                                });
                            }
                            else {
                                $('#viewPreviewTemplate')[0].removeAttribute('style', 'width: 760px;height: 600px;');
                                var iFrameContent = $('#iframe_templateViewInvoice').contents();
                                iFrameContent.find('img.temp')[0].setAttribute('style', 'position: absolute; width: 200px; top: 50%; left: calc(50% - 100px); transform: rotate(-30deg);');
                                iFrameContent.find('body')[0].removeAttribute('style', 'height: 868px;background-size: 90%;');
                                $('#iframe_templateViewInvoice')[0].removeAttribute('style', 'width: 1100px;height: 868px;');
                            }
                        }
                        LoadingHide();
                    });
                });
            } else {
                //Load repeat
                console.log('Load repeat...');
                LoadWhileComplete();
            }
        }, 300);
    };

    //Nếu là edit
    if (TEMPLATEPATH) {
        //Edit template
        iframe.attr("src", urlLoadView + TEMPLATEPATH);
        LoadWhileComplete();
        $scope.isChonseType = false;
    } else {
        //Show danh sách loại hóa đơn
        $scope.isChonseType = true;

        //UI/UX
        $timeout(function () {
            $('select.cb-select-time').selectpicker();
        });
    }

    $scope.ApplyInvoiceType = function (templateurl) {
        TEMPLATEPATH = templateurl;
        iframe.attr("src", urlLoadView + TEMPLATEPATH);
        LoadWhileComplete();
        $scope.isChonseType = false;
    };

    $scope.SaveTemplate = function (callback) {

        //var href = iframe.contents().find('#mainstyle').attr('href');
        //iframe.contents().find('#mainstyle').attr('href', href + '1');

        //return false;

        LoadingShow();
        //Nếu là edit mẫu thì copy thuộc tính đối tượng cần thiết để đẩy lên server
        var editingTemplate = {};
        if ($rootScope.EditingTemplate) {
            editingTemplate.FORMCODE = $rootScope.EditingTemplate.FORMCODE;
            editingTemplate.SYMBOLCODE = $rootScope.EditingTemplate.SYMBOLCODE;
            editingTemplate.FROMNUMBER = $rootScope.EditingTemplate.FROMNUMBER;
            editingTemplate.TONUMBER = $rootScope.EditingTemplate.TONUMBER;
        }
        if ($scope.SizePaper.value === "2") {
            $scope.template.style.body['background-size'] = "70%";
            $scope.template.style.body['height'] = "720px";
        }
        //ĐỊnh dạng template của teplate hiện tại
        $scope.template.CSS = getCSS($scope.template.style);

        // Gán giá trị mẫu nhiều thuế hay 1 thuế
        $scope.template.TAXRATE = $scope.InvoiceType.value;

        // Cho cập nhật trạng thái thông báo phát hành nếu là tài khoản free trial
        if ($rootScope.Enterprise.ISFREETRIAL === true) {
            $scope.template.STATUS = $("#txtNumberStatus").val();
        }
        var action = url + 'SaveTemplate';
        var datasend = JSON.stringify({
            objNumberBO: $scope.template,
            templateFile: TEMPLATEPATH,
            editingTemplate: editingTemplate
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (typeof callback === "function")
                        callback(response);
                    else
                        alert("Lưu thành công!");
                } else {
                    alert(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
                LoadingHide();
            }
        });
    }

    $scope.SaveExport = function () {
        var pageSize = "A4";
        if ($scope.SizePaper.value === "2")
            pageSize = "A5";
        $scope.SaveTemplate(function (response) {
            var file_path = "/TempInvoice/Downloadfile/?link=" + response.outputTemplate + "&pageSize=" + pageSize;
            var a = document.createElement('a');
            a.href = file_path;
            a.target = "_blank";
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        });
    }

    //var UploadImage = function (imageName, dataImg) {
    //    var action = url + 'UploadImage';
    //    var datasend = JSON.stringify({
    //        formCode: $scope.template.FORMCODE,
    //        symbolCode: $scope.template.SYMBOLCODE,
    //        imageName: imageName,
    //        dataImg: dataImg
    //    });
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (imageName == 'logo') {
    //                $scope.template.style[".logo"]["background-image"] = "url('" + window.location.origin + response.filePath + "')";
    //            } else if (imageName == 'background') {
    //                $scope.template.style["body"]["background-image"] = "url('" + window.location.origin + response.filePath + "')";
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
    //        }
    //    });
    //}

    //Read file logo
    $scope.uploadFile = function (event) {
        $scope.isLoading = true;
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $scope.template.logo = event.target.result;
                    $scope.ChangeLogo($scope.template.logo);
                    //Upload file
                    //UploadImage('logo', $scope.template.logo);
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };

    //Read file background
    $scope.uploadBackground = function (event) {
        $scope.isLoading = true;
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $scope.template.bg = event.target.result;
                    $scope.SetBackground($scope.template.bg, 'auto');
                    //Upload file
                    //UploadImage('background', $scope.template.bg);
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };



    function getCSS(styles) {
        var css = [];
        for (let selector in styles) {
            let style = selector + " {";

            for (let prop in styles[selector]) {
                style += prop + ":" + styles[selector][prop] + ";";
            }
            style += "}";
            css.push(style);
        }
        return css.join("\n");
    }

    //Chuyển json -> css
    $scope.$watch('template.style', function () {
        if ($scope.template) {
            var styleString = getCSS($scope.template.style);
            //console.log(styleString);
            iframe.contents().find('style').text(styleString);
        }
    }, true);

    $scope.ChangeFontSize = function () {
        $scope.template.style["body, table"]["font-size"] = $scope.template.fontSize;
    }

    $scope.ChangeFontFamily = function () {
        $scope.template.style["body, table"]["font-family"] = $scope.template.fontFamily;
    }

    $scope.ChangeColor = function (color) {
        $scope.template.style["body, table"].color = color + " !important";
    }

    $scope.SetBorderColor = function (color) {
        $scope.template.style["#layer1"]["border-color"] = color;

        if ($scope.template.borderWidth === "0px") {
            $scope.template.borderWidth = "3px";
            $scope.template.style["#layer1"]["border-width"] = $scope.template.borderWidth;
        }
    }

    $scope.SetBorderRadius = function () {
        $scope.template.style["#layer1"]["border-radius"] = $scope.template.borderRadius;
    }

    $scope.SetBorderWidth = function () {
        $scope.template.style["#layer1"]["border-width"] = $scope.template.borderWidth;
        if ($scope.template.style["#layer1"]["border-color"] === "#fff") {
            $scope.template.style["#layer1"]["border-color"] = "#000";
        }
    }

    $scope.SetBorderPattern = function (pattern) {
        if (pattern != null && pattern != '')
            $scope.template.style["#layer1"]["background-image"] = "url('" + pattern + "')";
        else
            $scope.template.style["#layer1"]["background-image"] = "none";
    }

    $scope.ChangeSerialNo = function () {
        iframe.contents().find('#symbol_invoice_template').html($scope.template.SYMBOLCODE);
    }

    $scope.ChangeForm = function () {
        if ($scope.template.CODE.length == 1)
            $scope.template.FORMCODE = $scope.template.FORM + "00" + $scope.template.CODE;
        else if ($scope.template.CODE.length == 2)
            $scope.template.FORMCODE = $scope.template.FORM + "0" + $scope.template.CODE;
        else
            $scope.template.FORMCODE = $scope.template.FORM + $scope.template.CODE;

        iframe.contents().find('#form_invoice_template').html($scope.template.FORMCODE);
    }

    $scope.ShowAgainCode = function () {
        if ($scope.template.CODE.length == 1)
            $scope.template.CODE = "00" + $scope.template.CODE;
        else if ($scope.template.CODE.length == 2)
            $scope.template.CODE = "0" + $scope.template.CODE;
    }

    $scope.ChangeLogo = function (data) {
        if (data != null && data != '') {
            $scope.template.style[".logo"]["background-image"] = "url('" + data + "')";
            $scope.template.style[".logo"]["display"] = "block";
        } else {
            $scope.RemoveLogo();
        }
    }
    $scope.RemoveLogo = function () {
        $scope.template.logo = null;
        $scope.template.style[".logo"]["background-image"] = "none";
        $scope.template.style[".logo"]["display"] = "none";
        $('[type="file"]').val('');
    }

    $scope.SetBackground = function (bgImage, bgSize) {
        if (bgImage === '') {
            $scope.template.style["body"]["background-image"] = 'none';
            $scope.template.bg = null;
            $('[type="file"]').val('');
        } else {
            $scope.template.style["body"]["background-image"] = "url('" + bgImage + "')";
        }
        if (bgSize != null) {
            $scope.template.style["body"]["background-size"] = bgSize;
        } else {
            $scope.template.style["body"]["background-size"] = "100% 100%";
        }
    }

    $scope.LoadSizePaper = function () {
        if (parseInt($scope.SizePaper.value) == 1) {
            $('#div-template-a4')[0].setAttribute('style', 'display:block;');
            $('#div-template-a5')[0].setAttribute('style', 'display:none;');
        }
        else {
            $('#div-template-a4')[0].setAttribute('style', 'display:none;');
            $('#div-template-a5')[0].setAttribute('style', 'display:block;');
        }
    }

    //UI/UX
    $('[data-toggle="tooltip"]').tooltip({
        content: function () {
            return $(this).prop('title');
        },
        position: {
            my: "center top+15", at: "left bottom"
        }
    });
}]);