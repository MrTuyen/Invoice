app.controller('SettingTemplateController', ['$scope', '$rootScope', '$routeParams', '$timeout', 'CommonFactory', '$sce', function ($scope, $rootScope, $routeParams, $timeout, CommonFactory, $sce) {
    var url = '/SettingTemplate/';

    LoadingShow();
    //Mặc định nhiều thuế suất
    $scope.InvoiceType = {
        value: "1",    //Default value
        Options: [
            { value: "1", text: 'Mẫu một thuế suất' },
            { value: "2", text: 'Mẫu nhiều thuế suất' },
            { value: "3", text: 'Hóa đơn bán hàng' },
            { value: "4", text: 'Hóa đơn tiền điện' },
            { value: "5", text: 'Hóa đơn tiền nước' },
            { value: "6", text: 'Hóa đơn trường học' }
        ]
    };

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

    //Load to complete
    var LoadWhileComplete = function () {
        LoadingShow();
        $timeout(function () {
            var csstext = $('.html-view-content').find('style').text();
            if (csstext != null && csstext != '') {
                $.parsecss(csstext, function (cssjson) {
                    $timeout(function () {
                        var FORMCODE = $('#form_invoice_template').text();
                        var SYMBOLCODE = $('#symbol_invoice_template').text();
                        var f = FORMCODE.split('/');

                        $rootScope.template = {
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

    //Chuyển json -> css
    $scope.$watch('template.style', function () {
        if ($rootScope.template) {
            var styleString = getCSS($rootScope.template.style);
            //console.log(styleString);
            $('.html-view-content').find('style').text(styleString);
        }
    }, true);

    $scope.ApplyInvoiceType = function (tempid) {

        LoadWhileComplete();

        $('.close-details').click();
        var action = url + 'LoadTemplate';
        var datasend = JSON.stringify({
            tempid: tempid
        });

        LoadingShow();
        $('.modal-invoice-template-change').modal('show');
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.htmlResult = $sce.trustAsHtml(response.res);
                    LoadWhileComplete();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert($scope.ErrorMessage);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - MeterController - GetMeter');
            }
            LoadingHide();
        });
    };

    $scope.SaveTemplate = function (tempid) {
        LoadingShow();

        var $divLogo = $('div.logo.highlight-logo');
        var xxx = $divLogo.attr('style');
        var css = parseCSSText(xxx);
        $rootScope.template.style['.fix-logo-pos'] = css.style;
        //Nếu là edit mẫu thì copy thuộc tính đối tượng cần thiết để đẩy lên server
        var editingTemplate = {};
        if ($rootScope.EditingTemplate) {
            editingTemplate.FORMCODE = $rootScope.EditingTemplate.FORMCODE;
            editingTemplate.SYMBOLCODE = $rootScope.EditingTemplate.SYMBOLCODE;
            editingTemplate.FROMNUMBER = $rootScope.EditingTemplate.FROMNUMBER;
            editingTemplate.TONUMBER = $rootScope.EditingTemplate.TONUMBER;
        }
        if ($scope.SizePaper.value === "2") {
            $rootScope.template.style.body['background-size'] = "70%";
            $rootScope.template.style.body['height'] = "720px";
        }
        //ĐỊnh dạng template của teplate hiện tại
        $rootScope.template.CSS = getCSS($rootScope.template.style);

        // Gán giá trị mẫu nhiều thuế hay 1 thuế
        $rootScope.template.TAXRATE = $scope.InvoiceType.value;

        var action = url + 'SaveTemplate';
        var datasend = JSON.stringify({
            objNumberBO: $rootScope.template,
            templateFile: tempid,
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

    //Read file logo
    $scope.uploadFile = function (event) {
        $scope.isLoading = true;
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $rootScope.template.logo = event.target.result;
                    $scope.ChangeLogo($rootScope.template.logo);

                    setResizable();
                    reszieableLogo();
                    //Upload file
                    //UploadImage('logo', $rootScope.template.logo);
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
                    $rootScope.template.bg = event.target.result;
                    $scope.SetBackground($rootScope.template.bg, 'auto');
                    //Upload file
                    //UploadImage('background', $rootScope.template.bg);
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };

    $scope.ChangeFontSize = function () {
        $rootScope.template.style["body, table, .bg"]["font-size"] = $rootScope.template.fontSize;
    }

    $scope.ChangeFontFamily = function () {
        $rootScope.template.style["body, table, .bg"]["font-family"] = $rootScope.template.fontFamily;
    }

    $scope.ChangeColor = function (color) {
        $rootScope.template.style["body, table, .bg"].color = color + " !important";
    }

    $scope.SetBorderColor = function (color) {
        $rootScope.template.style["#layer1"]["border-color"] = color;

        if ($rootScope.template.borderWidth === "0px") {
            $rootScope.template.borderWidth = "3px";
            $rootScope.template.style["#layer1"]["border-width"] = $rootScope.template.borderWidth;
        }
    }

    $scope.SetBorderRadius = function () {
        $rootScope.template.style["#layer1"]["border-radius"] = $rootScope.template.borderRadius;
    }

    $scope.SetBorderWidth = function () {
        $rootScope.template.style["#layer1"]["border-width"] = $rootScope.template.borderWidth;
        if ($rootScope.template.style["#layer1"]["border-color"] === "#fff") {
            $rootScope.template.style["#layer1"]["border-color"] = "#000";
        }
    }

    $scope.SetBorderPattern = function (pattern) {
        if (pattern != null && pattern != '')
            $rootScope.template.style["#layer1"]["background-image"] = "url('" + pattern + "')";
        else
            $rootScope.template.style["#layer1"]["background-image"] = "none";
    }

    $scope.ChangeSerialNo = function () {
        $('#symbol_invoice_template').html($rootScope.template.SYMBOLCODE);
    }

    $scope.ChangeForm = function () {
        if ($rootScope.template.CODE.length == 1)
            $rootScope.template.FORMCODE = $rootScope.template.FORM + "00" + $rootScope.template.CODE;
        else if ($rootScope.template.CODE.length == 2)
            $rootScope.template.FORMCODE = $rootScope.template.FORM + "0" + $rootScope.template.CODE;
        else
            $rootScope.template.FORMCODE = $rootScope.template.FORM + $rootScope.template.CODE;

        $('#form_invoice_template').html($rootScope.template.FORMCODE);
    }

    $scope.ShowAgainCode = function () {
        if ($rootScope.template.CODE.length == 1)
            $rootScope.template.CODE = "00" + $rootScope.template.CODE;
        else if ($rootScope.template.CODE.length == 2)
            $rootScope.template.CODE = "0" + $rootScope.template.CODE;
    }

    $scope.ChangeLogo = function (data) {
        if (data != null && data != '') {
            $rootScope.template.style[".logo"]["background-image"] = "url('" + data + "')";
            $rootScope.template.style[".logo"]["display"] = "block";
            $rootScope.template.style[".logo"]["position"] = "relative";
            $('.logo').addClass('highlight-logo');
        } else {
            $scope.RemoveLogo();
        }
    }
    $scope.RemoveLogo = function () {
        $rootScope.template.logo = null;
        $rootScope.template.style[".logo"]["background-image"] = "none";
        $rootScope.template.style[".logo"]["display"] = "none";
        $('[type="file"]').val('');
        $('.logo').removeClass('highlight-logo');
    }

    $scope.SetBackground = function (bgImage, bgSize) {
        if (bgImage === '') {
            $rootScope.template.style["#layer1"]["background-image"] = 'none';
           // $rootScope.template.style["body, table, .bg"]["background-image"] = 'none';
            $rootScope.template.bg = null;
            $('[type="file"]').val('');
        } else {
            $rootScope.template.style["#layer1"]["background-image"] = "url('" + bgImage + "')";
            //$rootScope.template.style["body, table, .bg"]["background-image"] = "url('" + bgImage + "')";
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

    /*
    * event khi sử dụng QR code
    */
    $scope.ChangeQRCode = function () {
        var qrCode = $(".qrcode");
        qrCode.toggle();
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

    LoadingHide();
}]);

/*
* enum vị trí logo
*/
POSITION_LOGO = {
    Left: 1,
    Right: 2,
}

function parseCSSText(cssText) {
    var cssTxt = cssText.replace(/\/\*(.|\s)*?\*\//g, " ").replace(/\s+/g, " ");
    var style = {}, [, ruleName, rule] = cssTxt.match(/ ?(.*?) ?{([^}]*)}/) || [, , cssTxt];
    var cssToJs = s => s.replace(/\W+\w/g, match => match.slice(-1).toUpperCase());
    var properties = rule.split(";").map(o => o.split(":").map(x => x && x.trim()));
    for (var [property, value] of properties) style[cssToJs(property)] = value;
    return { cssText, ruleName, style };
} 

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

//Sự kiện resize ảnh logo và di chuyển ảnh mẫu
function reszieableLogo() {
    var itemResize = $(".ui-icon-gripsmall-diagonal-se");
    itemResize.css("position", "absolute");
    itemResize.css("width", "8");
    itemResize.css("height", "8");
    itemResize.css("background", "#1492e6");
    itemResize.css("right", "-5px");
    itemResize.css("bottom", "-5px");
    itemResize.css("cursor", "se-resize");
    itemResize.css("border", "1px solid #fff"); 
}

function setResizable() {
    $(".logo").resizable({
        maxWidth: 250,
        start: function (event, ui) {
            $(this).data("dragging", true);
        },
        stop: function (event, ui) {
            setTimeout(function () {
                $(event.target).data("dragging", false);
            }, 1);
        } 
    });
    //$(".logo").draggable();
    $(".logo").draggable({
        start: function (event, ui) {
            $(this).data("dragging", true);
        },
        stop: function (event, ui) {
            var $td = $('td-logo-company');
            $td.width(ui.helper.width() + ui.position.left);
            setTimeout(function () {
                $(event.target).data("dragging", false);
            }, 1);
        },
        drag: function (event, ui) {
            var $target = $(event.target),
                positionLogo = POSITION_LOGO.Left;
            //nếu là góc trái
            if (positionLogo == POSITION_LOGO.Left) {
                if ($target.width() + ui.position.left >= 150) {
                    ui.position.left = 150 - $target.width();
                }
                if (ui.position.left < 0) {
                    ui.position.left = 0;
                }
            } else {
                if (ui.position.left < -30) {
                    ui.position.left = -30;
                }
                if (ui.position.left > 0) {
                    ui.position.left = 0;
                }
            }
            if ((Math.abs(ui.position.top) + ($target.height() / 2)) > 80) {
                if (ui.position.top < 0) {
                    ui.position.top = ($target.height() / 2) - 80;
                } else {
                    ui.position.top = 80 - ($target.height() / 2);
                }
            }
        },
    });
}
