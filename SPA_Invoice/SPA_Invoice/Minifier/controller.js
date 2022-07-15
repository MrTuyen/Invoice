app.controller('RootController', ['$scope', '$rootScope', '$http', 'CommonFactory', '$interval', '$timeout', '$location', 'permissions', function ($scope, $rootScope, $http, CommonFactory, $interval, $timeout, $location, permissions) {
    var url = '/Root/';

    LoadingShow();
    angular.element(function () {
        $rootScope.GetFormCode();
        $timeout(function () {
            $rootScope.GetEnterpriseInfo();
            $scope.LoadDashboard();
            $rootScope.GetSymbolCode();
            $rootScope.GetPaymentStatus();
            $rootScope.GetInvoiceStatus();
            $rootScope.GetPaymentMethod();
            $rootScope.GetQuantityUnit();
        }, 1500)
    });

    $scope.$on('$routeChangeSuccess', function () {
        gtag('config', 'UA-154152339-1', {
            'page_path': $location.path()
        });
    });

    $rootScope.CurrentURL = $location.absUrl();
    $rootScope.OpenImportInvoiceModal = function () {
        $('.import-invoice').modal('show');
    }
    
    $rootScope.OpenImportProductModal = function () {
        $('.import-product').modal('show');
    }

    $rootScope.OpenImportCustomerModal = function () {
        $('.import-customer').modal('show');
    }

    $rootScope.OpenPreviewInvoiceModal = function () {
        $('.modal-preview-invoice').modal('show');
    }

    $rootScope.OpenImportMeterModal = function () {
        $('.import-meter').modal('show');
    }

    $scope.LoadDashboard = function () {
        var action = '/Invoice/' + 'LoadDashboard';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    //$scope.TotalMoneyNotPay = response.TotalMoneyNotPay;
                    $scope.TotalMoneyPaied = response.TotalMoneyPaied;
                    $scope.TotalMoneyNotApproval = response.TotalMoneyNotApproval;
                    $scope.TotalInvoiceSigned = response.totalInvoiceSigned;

                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetProvince');
            }
        });
    };

    $rootScope.GetCurrencyUnit = function () {
        var action = url + 'GetCurrencyUnit';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListCurrencyUnit = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCurrencyUnit');
            }
        });
    }

    $rootScope.GetCategory = function () {
        var action = url + 'GetCategory';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListCategory = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCategory');
            }
        });
    }

    $rootScope.GetInvoiceStatus = function () {
        if ($rootScope.ListInvoiceStatus)
            return;
        var action = url + 'GetInvoiceStatus';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListInvoiceStatus = response.result;
                    $rootScope.ListReceiptStatus = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceStatus');
            }
        });
    }

    $rootScope.GetInvoiceType = function () {
        if ($rootScope.ListInvoiceType)
            return;
        var action = url + 'GetInvoiceType';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListInvoiceType = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceType');
            }
        });
    }

    $rootScope.GetPaymentMethod = function () {
        var action = url + 'GetPaymentMethod';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListPaymentMethod = response.result;
                    $rootScope.ListPaymentStatus = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetPaymentMethod');
            }
        });
    }

    $rootScope.GetPaymentStatus = function () {
        if ($rootScope.ListPaymentStatus)
            return;
        var action = url + 'GetPaymentStatus';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListPaymentStatus = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetPaymentStatus');
            }
        });
    }

    $rootScope.GetQuantityUnit = function () {
        var action = url + 'GetQuantityUnit';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListQuantityUnit = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetQuantityUnit');
            }
        });
    }

    $rootScope.GetFormCode = function () {
        if ($rootScope.ListFormCode)
            return;
        var action = url + 'GetFormCode';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListFormCode = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetFormCode');
            }
        });
    }

    $rootScope.GetSymbolCode = function () {
        if ($rootScope.ListSymbolCode)
            return;
        var action = url + 'GetSymbolCode';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListSymbolCode = response.result;
                } else {
                    $rootScope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetSymbolCode');
            }
        });
    }
    /*
     * tuyennv-20201014
     * Lấy quyền của user đăng nhập hiện tại
     * */
    $rootScope.GetRoleDetailByCurrentUser = function () {
        var permissionList = [];
        var action = '/Account/GetRoleDetailByCurrentUser';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    response.result.forEach(function (e) {
                        permissionList.push(e.toString());
                    });
                    $rootScope.CURRENT_USER_ROLE = permissionList;
                    permissions.setPermissions(permissionList);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetRoleDetailByCurrentUser');
            }
        });
    }
    $rootScope.GetRoleDetailByCurrentUser();

    /*
     * nvtruong 1.4.2020
     * Sửa lại async: false do khi mở form liên tục bị null value do chạy bất đồng bộ
     */
    $rootScope.GetEnterpriseInfo = function () {
        var action = '/Settings/' + 'GetEnterpriseInfo';
        var datasend = JSON.stringify({
        });

        var timeout = 60000;
        var tokenString = "";
        var uuid = "";
        $.ajax({
            url: action,
            type: "POST",
            headers: {
                "X-Requested-With": "Novaon-DS",
                "Token-String": tokenString,
                "Device-UUID": uuid,
            },
            timeout: timeout,
            cache: true,
            crossDomain: true,
            contentType: "application/json; charset=utf-8;",
            dataType: "json",
            data: datasend,
            processData: true,
            async: false,
            tryCount: 0,
            retryLimit: 3,
            success: function (response) {
                if (response) {
                    if (response.rs) {
                        $rootScope.Enterprise = response.Company;
                        $rootScope.UserInfo = response.User;
                    } else {
                        $scope.ErrorMessage = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
                }
            },
            error: function (error) {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
            }
        });
        //CommonFactory.PostDataAjax(action, datasend, function (response) {
        //    if (response) {
        //        if (response.rs) {
        //            $rootScope.Enterprise = response.Company;
        //        } else {
        //            $scope.ErrorMessage = response.msg;
        //            alert(response.msg);
        //        }
        //    } else {
        //        alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SettingsController - GetUserInfo');
        //    }
        //});
    }

    //Init time
    var now = new Date();
    var firstDay = new Date();
    var lastDay = new Date();
    var currentDay = now.getDay();

    // Sunday - Saturday : 0 - 6
    //This week
    firstDay.setDate(now.getDate() - currentDay);
    lastDay.setDate(firstDay.getDate() + 6);
    var thisWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last week
    firstDay.setDate(firstDay.getDate() - 7);
    lastDay.setDate(lastDay.getDate() - 7);
    var lastWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //This month
    var dayOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    lastDay = new Date(now.getFullYear(), now.getMonth(), dayOfMonth);
    var thisMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last month
    lastDay.setDate(firstDay.getDate() - 1);
    firstDay = new Date(lastDay.getFullYear(), lastDay.getMonth(), 1);
    var lastMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    $scope.Timepickers = {
        value: thisWeek,    //Default value
        Options: [
            { id: 1, value: thisWeek, text: 'Tuần này' },
            { id: 2, value: lastWeek, text: 'Tuần trước' },
            { id: 3, value: thisMonth, text: 'Tháng này' },
            { id: 4, value: lastMonth, text: 'Tháng trước' },
            { id: 5, value: '5', text: 'Tùy chọn' }
        ]
    };
    //End init time

    $scope.clickButton = null;

    $scope.popupAlert = function ($event) {
        var button = $($event.currentTarget);

        var w = button.width();
        var h = button.height();
        var offset = button.offset();
        var popup = $(button.attr('data-popup'));

        var top = offset.top + h + 16;
        var left = offset.left;
        if (popup.hasClass('flyout-right')) {
            left -= (w / 2 - 7);
        } else if (popup.hasClass('flyout-left')) {
            left -= (popup.width() - w - 10);
        }


        popup.css({
            top: top,
            left: left
        });

        var overlay = $('.flyout-overview');

        //Close all others before opening the popup
        $('.flyout-open').removeClass('flyout-open');

        //Open or Close flyout
        if (!button.is($scope.clickButton)) {
            button.addClass('flyout-open');
            popup.addClass('flyout-open');
            overlay.addClass('flyout-open');

            //Sale session click
            $scope.clickButton = button;
        } else {
            //Clear session click
            $scope.clickButton = null;
        }
    }

    //Hide dropdown on page click
    $(document).on('click', function (e) {

        var target = $(e.target);
        var fbutton = target.closest('.flyout-button');
        var fview = target.closest('.flyout-view');

        if (!target.hasClass('flyout-button') && !target.hasClass('flyout-view') && !fbutton.length) {
            //Open or Close flyout
            $('.flyout-open').removeClass('flyout-open');

            //Clear session click
            $scope.clickButton = null;
        } else if (fview.length) {
            setTimeout(function () {
                //Open or Close flyout
                $('.flyout-open').removeClass('flyout-open');

                //Clear session click
                $scope.clickButton = null;
            }, 300);
        }

        ////Close box suggest
        //if (!target.hasClass('box_suggest')) {
        //    //Open or Close flyout
        //    $('.box_suggest').addClass('ng-hide');
        //}
    });


    $rootScope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    $rootScope.SelectAll = function (list, isselect) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == isselect;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !isselect;
            });
        }
    }

    $(function () {
        var ctr = $('.datepicker');
        ctr.datepicker({
            dateFormat: 'dd/mm/yy'
        });
        SetVietNameInterface(ctr);
    });

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    //UI/UX
    $(function () {
        $('.main-sidebar ul.sidebar-menu li').click(function () {
            var _this = $(this);

            $('.main-sidebar ul.sidebar-menu li').removeClass('active');
            if (!_this.hasClass('treeview'))
                $('body').removeClass('sidebar-open');

            _this.addClass('active');
        });

        $('.full-overlay').click(function () {
            $('.sidebar-open').removeClass('sidebar-open');
            setTimeout(function () {
                $('.full-overlay').css({ 'opacity': 0 });
                $('.full-overlay').css('z-index', -1);
            }, 300);
        });

        $(document).on('click', function (e) {
            var target = $(e.target);
            var fbutton = target.closest('.flyout-button');
            var fview = target.closest('.flyout-view');
            if (!target.hasClass('flyout-button') && !target.hasClass('flyout-view') && !fbutton.length) {
                $('.flyout-open').removeClass('flyout-open');
            } else if (fview.length) {
                setTimeout(function () {
                    $('.flyout-open').removeClass('flyout-open');
                }, 300);
            }
        });

        //Customize alert
        var ALERT_TITLE = 'THÔNG BÁO';
        var ALERT_BUTTON_TEXT = 'Đồng ý';
        var CONFIRM_OK_BUTTON_TEXT = 'Đồng ý';
        var CONFIRM_CC_BUTTON_TEXT = 'Bỏ qua';
        var removeCustomAlert = function () {
            document.getElementsByTagName('body')[0].removeChild(document.getElementById('modalContainer'));
        }
        if (document.getElementById) {
            window.alert = function (txt) {
                createCustomAlert(txt, null);
            }
            window.confirm = function (txt, title, button1, button2, callback) {
                createCustomAlert(txt, title, button1, button2, callback);
            }
        }

        function createCustomAlert(txt, title, button1, button2, callback) {
            if (!title)
                title = ALERT_TITLE;
            if (!button1)
                button1 = CONFIRM_CC_BUTTON_TEXT;
            if (!button2)
                button2 = CONFIRM_OK_BUTTON_TEXT;
            var d = document;

            if (d.getElementById('modalContainer')) return;

            var mObj = d.getElementsByTagName('body')[0].appendChild(d.createElement('div'));
            mObj.id = 'modalContainer';
            mObj.style.height = d.documentElement.scrollHeight + 'px';

            var alertObj = mObj.appendChild(d.createElement('div'));
            alertObj.id = 'alertBox';
            if (d.all && !window.opera) alertObj.style.top = document.documentElement.scrollTop + 'px';
            alertObj.style.left = (d.documentElement.scrollWidth - alertObj.offsetWidth) / 2 + 'px';
            alertObj.style.visiblity = 'visible';

            var h1 = alertObj.appendChild(d.createElement('h1'));
            h1.appendChild(d.createTextNode(title));

            var msg = alertObj.appendChild(d.createElement('p'));
            //msg.appendChild(d.createTextNode(txt));
            var find = '\n';
            var re = new RegExp(find, 'g');

            //txt = txt.replace(/(?:\\n)/g, '<br>');

            msg.innerHTML = txt;



            if (callback) {
                var div = alertObj.appendChild(d.createElement('div'));
                div.className = 'btnGroup';

                var btn2 = div.appendChild(d.createElement('button'));
                btn2.id = 'cancelBtnAlert';
                btn2.className = 'btn btn-light';
                btn2.appendChild(d.createTextNode(button1));
                btn2.href = '#';
                btn2.onclick = function () { removeCustomAlert(); callback(false); return false; }

                var btn = div.appendChild(d.createElement('button'));
                //btn.id = 'closeBtnAlert';
                btn.className = 'btn btn-success';
                btn.appendChild(d.createTextNode(button2));
                btn.href = '#';
                btn.onclick = function () { removeCustomAlert(); callback(true); return false; }
            } else {
                var btn = alertObj.appendChild(d.createElement('button'));
                btn.id = 'closeBtnAlert';
                btn.className = 'btn btn-success';
                btn.appendChild(d.createTextNode(ALERT_BUTTON_TEXT));
                btn.href = '#';
                btn.onclick = function () { removeCustomAlert(); return false; }
            }

            alertObj.style.display = 'block';
        }

        //Gán giá trị checked cho dòng được chọn
        $timeout(function () {
            var code = localStorage.getItem("novaon_comcode");
            if (code)
                $('#show-checked_' + code.trim()).addClass('selected');
        });
    });

    function RefreshSession() {
        var action = '/Account/RefreshSession';
        var datasend = JSON.stringify({
        });

        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                //Nếu session null
                if (!response.rs) {
                    $interval.cancel(stopRequest);
                    if (window.location.href.indexOf('Account/Login') === -1) {
                        window.location.href = '/Account/Login';
                    }
                }
            } else {
                alert('Lỗi không nhận được phản hồi - RefreshSession (js) ');
            }
        });
    }

    stopRequest = $interval(RefreshSession, 600000);

    // Phân quyền
    $rootScope.USER_ROLE = {
        // 1. Quản lý hóa đơn
        THEM_MOI_HOA_DON: 1,
        CAP_NHAT_HOA_DON: 2,
        KY: 3,
        XEM_HOA_DON: 4,
        GUI_EMAIL: 5,
        NGHIEP_VU_HOA_DON: 6,
        IMPORT_EXCEL_HOA_DON: 7,

        // 2. Dải hóa đơn chờ
        THEM_DAI_CHO: 8,
        KY_DAI_CHO: 9,

        // 3. Khách hàng
        THEM_MOI_KHACH_HANG: 10,
        CHINH_SUA_KHACH_HANG: 11,
        IMPORT_EXCEL_KHACH_HANG: 12,

        // 4. Hàng hóa, dịch vụ
        THEM_MOI_HANG_HOA: 13,
        CHINH_SUA_HANG_HOA: 14,
        IMPORT_EXCEL_HANG_HOA: 15,

        // 5. Báo cáo, thống kê
        LOC: 16,
        TAI_XUONG: 17,

        // 6. Quản trị người dùng
        CAP_NHAT_THONG_TIN_NGUOI_DUNG: 18,
        PHAN_QUYEN_NGUOI_DUNG: 19,

        // 7. Thông báo phát hành
        THEM_MAU_HOA_DON: 20
    }

    $rootScope.cleanAccents = function (str) {
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
        str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
        str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
        str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
        str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
        str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
        str = str.replace(/Đ/g, "D");
        // Combining Diacritical Marks
        str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // huyền, sắc, hỏi, ngã, nặng 
        str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // mũ â (ê), mũ ă, mũ ơ (ư)

        return str;
    }

    $rootScope.SelectedCompany = function (code) {
        LoadingShow();
        var oldCode = localStorage.getItem("novaon_comcode");
        //if (oldCode !== code) {
            localStorage.setItem("novaon_comcode", code);
            var action = "/AjaxMethod/ResetSessionData";
            var datasend = JSON.stringify({
                comTaxCode: code
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    //Nếu session null
                    if (!response.rs) {
                        alert(result.msg);
                    }
                    window.location.reload();
                } else {
                    alert('Lỗi không nhận được phản hồi - RefreshSession (js) ');
                }
            });
        //}
        LoadingHide();
    }

    $rootScope.ChooseUsingInvoiceType = function (value, typename) {
        LoadingShow();
        var typeid = localStorage.getItem("novaon_comcode_usinginvoicetype");
        //if (parseInt(typeid) !== value) {
            localStorage.setItem("novaon_comcode_usinginvoicetype", value);
            var action = "/AjaxMethod/SaveChangeUsingInvoiceType";
            var datasend = JSON.stringify({
                typeid: value,
                typename: typename
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    //Nếu session null
                    if (!response.rs) {
                        alert(result.msg);
                    }
                    window.location.reload();
                } else {
                    alert('Lỗi không nhận được phản hồi - RefreshSession (js) ');
                }
            });
        //}
        LoadingHide();
    }

}]);

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = 'expires=' + d.toGMTString();
    document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
}

function getCookie(cname) {
    var name = cname + '=';
    var decodedCookie = document.cookie;
    try {
        decodedCookie = decodeURIComponent(document.cookie);
    } catch (error) {
        console.log(error);
    }
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
}

function GetNumber(num) {
    return parseFloat(num.toString().replace(/,/g, ''));
};

app.controller('HomeController', ['$scope', '$timeout', 'CommonFactory', '$rootScope', function ($scope, $timeout, CommonFactory, $rootScope) {
    var url = '/Home/';
    //var chart;
    //var chart2;
    $scope.ActiveTab = 1;
    LoadingShow();
    angular.element(function () {
        LoadingHide();
    });
    //$("#dashboard-drag").sortable({
    //    revert: true
    //});

    $scope.DashboardType = {
        value: "1",    //Default value
        Options: [
            { value: "1", text: 'Dashboard' },
            { value: "2", text: 'Video' }
        ]
    };

    $scope.ChangeDashboard = function (value) {
        $scope.DashboardType.value = value;
    }

    $scope.CheckEnterpriseInfor = function () {
        if (
            ((!$rootScope.UserInfo.USERNAME === "demo" || !$rootScope.UserInfo.EMAIL.includes("demo")) && $rootScope.Enterprise.COMTAXCODE === "0106579683-999")
            || ($rootScope.Enterprise.ISFREETRIAL === true && (getCookie("NovaonFreeTrial") === false || getCookie("NovaonFreeTrial") === undefined))
        )
        {
            $(".modal-onboarding").modal('show');
            $scope.Enterprise.COMTAXCODE = "";
            $scope.Enterprise.COMNAME = "";
            $scope.Enterprise.COMADDRESS = "";
            $scope.Enterprise.COMPHONENUMBER = "";
            $scope.Enterprise.COMEMAIL = "";
            $scope.Enterprise.COMACCOUNTNUMBER = "";
            $scope.Enterprise.COMBANKNAME = "";
            $scope.Enterprise.COMLEGALNAME = "";
            $scope.Enterprise.TAXDEPARTEMENTCODE = "";
            $scope.Enterprise.TAXDEPARTEMENT = "";
        }
    }

    $scope.SetActiveTab = (step) => {
        if (true) {

        }
        $scope.ActiveTab = $scope.ActiveTab + step;
    }

    $scope.SaveEnterpriseInfo = function () {
        if (!$scope.Enterprise.COMTAXCODE) {
            alert('Vui lòng nhập mã số thuế doanh nghiệp.');
            return;
        }
        if (!$scope.Enterprise.COMNAME) {
            alert('Vui lòng nhập tên doanh nghiệp.');
            return;
        }
        if (!$scope.Enterprise.COMADDRESS) {
            alert('Vui lòng nhập địa chỉ doanh nghiệp.');
            return;
        }
        var action = url + "AddEnterpriseInfoFreeVersion";
        var datasend = JSON.stringify({
            enterprise: $scope.Enterprise
        });
        var confirmContinue = function (result) {
            if (!result)
                return false;
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Cập nhật thông tin doanh nghiệp thành công!');
                        $rootScope.Enterprise.ISFREETRIAL = true;
                        setCookie("NovaonFreeTrial", true, 90)
                    } else {
                        alert('Doanh nghiệp với mã số thuế ' + '<b class="text-danger">' + $scope.Enterprise.COMTAXCODE +'</b> đã tồn tại.');
                    }
                } else {
                    alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveEnterpriseInfo");
                }
            });
        };
        confirm("Bạn có thực sự chắc chắn những thông tin trên đúng với doanh nghiệp của bạn? Bạn chỉ có thể cập nhật thông tin này <b class='text-danger'>1 lần duy nhất</b>.", "Thông báo", "Không", "Có", confirmContinue)
    };

    $scope.LoadInfoByTaxcode = function () {
        if ($rootScope.Enterprise.COMTAXCODE != null && $rootScope.Enterprise.COMTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $rootScope.Enterprise.COMTAXCODE;
            $("#loadTaxinfo").load(url, function (response, status, xhr) {
                var rawObj = JSON.parse(response);
                var obj = new Object();
                if (rawObj.Data != "") {
                    obj = JSON.parse(rawObj.Data);
                }
                $timeout(function () {
                    if (obj.companyName) {
                        $rootScope.Enterprise.COMADDRESS = obj.address;
                        $rootScope.Enterprise.COMNAME = obj.companyName.toUpperCase();
                    } else {
                        $rootScope.Enterprise.COMNAME = null;
                        $rootScope.Enterprise.COMADDRESS = null;
                    }
                });
            });
        } else {
            $rootScope.Enterprise.COMNAME = null;
            $rootScope.Enterprise.COMADDRESS = null;
        }
    }

    $scope.ShowDateTimePopup = function () {
        $('#dateTimePopUp').dialog({
            modal: true,
            resizable: false,
            width: 450,
            height: 200,
            open: function () {
                $(this).dialog("option", "title", "Lọc theo thời gian");
            },
            beforeClose: function (e, ui) {
                $scope.Timepickers.value = $scope.Timepickers.Options[0].value;
                $timeout(function () {
                    $("select.cb-select-time").selectpicker('refresh');
                }, 500)
            }
        });
    }

    $scope.LoadDashboard = function () {
        var searchTime = $scope.Timepickers.value;
        if ($scope.Timepickers.value == 5) {
            searchTime = moment($scope.FROMTIME).format('YYYY-MM-DD') + ';' + moment($scope.TOTIME).format('YYYY-MM-DD');
        }
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            dateRange: searchTime
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.TotalMoneyBeforVAT = response.TotalMoneyBeforVAT;
                    $scope.TotalMoneyAfterVAT = response.TotalMoneyAfterVAT;
                    $scope.TotalMoneyVAT = response.TotalMoneyVAT;

                    $timeout(function () {
                        //load chart
                        var lstX = [];
                        var beforvatY = [];
                        var aftervatY = [];
                        var vatY = [];
                        //
                        var userY = [];
                        var cancelY = [];
                        var replaceY = [];

                        if (response.chartData != null) {
                            response.chartData.forEach(function (item) {
                                lstX.push(item.Date);
                                beforvatY.push(item.BeforVAT);
                                aftervatY.push(item.AfterVAT);
                                vatY.push(item.VAT);
                                //
                                userY.push(item.CountUse);
                                cancelY.push(item.CountCancel);
                                replaceY.push(item.CountReplace);
                            });

                            //Chart line
                            chart.updateOptions({
                                xaxis: {
                                    categories: lstX
                                }
                            });

                            chart.updateSeries([{
                                data: beforvatY
                            }, {
                                data: aftervatY
                            }, {
                                data: vatY
                            }], true);


                            //Chart column
                            chart2.updateOptions({
                                xaxis: {
                                    categories: lstX
                                }
                            });

                            chart2.updateSeries([{
                                data: userY
                            }, {
                                data: cancelY
                            }, {
                                data: replaceY
                            }], true);
                        }
                    }, 100)
                }
                $scope.CheckEnterpriseInfor();
            } 
        });
    };

    $scope.LoadData = function () {
        if ($scope.Timepickers.value == 5) {
            $scope.ShowDateTimePopup();
        }
        $scope.LoadDashboard();
    }

    //UI/UX
    $timeout(function () {
        $('select.cb-select-time').selectpicker();
    });


    //Init chart 
    var options = {
        chart: {
            height: 350,
            type: 'area',
            zoom: {
                enabled: false
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val, opts) {
                return val.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
            }
        },
        stroke: {
            curve: 'smooth', //straight
            width: 2,
        },
        series: [{
            name: 'Doanh thu trước thuế',
            data: []
        }, {
            name: 'Doanh thu sau thuế',
            data: []
        }, {
            name: 'Tiền thuế',
            data: []
        }],
        colors: ['#2CA01C', '#3797D3', '#24BAB5'],
        markers: {
            size: 4
        },
        xaxis: {
            type: 'datetime',
            categories: [],
        },
        tooltip: {
            x: {
                format: 'dd/MM/yy'
            },
            y: {
                formatter: function (val) {
                    return val.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,') + " VND"
                }
            }
        },
        legend: {
            horizontalAlign: 'center',
            position: 'top'
        }
    }

    var chart = new ApexCharts(
        document.querySelector("#chart"),
        options
    );

    chart.render();

    var options2 = {
        chart: {
            height: 350,
            type: 'bar',
            stacked: true,
            zoom: {
                enabled: false
            }
        },
        series: [{
            name: 'Số hoá đơn sử dụng',
            data: []
        }, {
            name: 'Số hoá đơn huỷ',
            data: []
        }, {
            name: 'Số hoá đơn thay thế',
            data: []
        }],
        xaxis: {
            type: 'datetime',
            categories: [],
        },
        legend: {
            horizontalAlign: 'center',
            position: 'top'
        },
        fill: {
            opacity: 1
        },
        tooltip: {
            y: {
                formatter: function (val) {
                    return val + " đơn"
                }
            }
        },
    }

    var chart2 = new ApexCharts(
        document.querySelector("#chart2"),
        options2
    );

    chart2.render();

    // custom datepicker for input text
    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime_home").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_home"));
    }

    $scope.SetDatepikerToTime = function () {
        var fromTime = $scope.FROMTIME;
        $("#pk_totime_home").datepicker("option", 'minDate', fromTime);
        $("#pk_totime_home").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(fromTime)
        });
        SetVietNameInterface($("#pk_totime_home"));
    }
    //--------------------------------------
}]);

app.controller('AccountController', ['$scope', '$timeout', 'CommonFactory', '$http', function ($scope, $timeout, CommonFactory, $http) {
    var url = '/Account/';
   

}]);
app.controller('LogoutController', ['$scope', function ($scope) {

    
}]);
app.controller('InvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', '$location', '$window', '$sce', function ($scope, $rootScope, $timeout, CommonFactory, $location, $window, $sce) {
    var url = '/Invoice/';

    angular.element(function () {
        //$scope.LoadDashboard();
        $scope.IsHSM = $window.localStorage.getItem("novaon_kyso_hsm"); // Kiểm tra xem user đăng nhập có sử dụng HSM không? nếu có thì dùng để ẩn/ hiện button ký hàng loạt bằng HSM trên trang chủ QLHD
    });
    $rootScope.ListInvoiceChecked = new Array();
    $scope.Invoice = new Object();
    //========================== Cookie's Own ============================
    $scope.LoadCookie_Invoice = function () {
        var check = getCookie('Novaon_InvoiceManagement');
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldBillDate: true,
                FieldBillType: true,
                FieldCustomer: true,
                FieldFormCode: true,
                FieldSymbolCode: true,
                FieldNumber: true,
                FieldInvoiceStatus: true,
                FieldReferenceCode: true,
                FieldPaymentStatus: false,
                FieldTotalPayment: true,
                FieldCustomerID: true,
                FieldConsignmentID: false,
                FieldExchangeRate: false,
                FieldExchange: false,
                FieldInvoiceNote: false,
                FieldEmailPartner: false,
                RowNum: 10
            }
            setCookie('Novaon_InvoiceManagement', JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie('Novaon_InvoiceManagement', JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        if ($scope.Tab.Wating) {
            $scope.GetInvoiceWait($scope.currentpage);
        } else {
            $scope.GetInvoice($scope.currentpage);
        }
    }
    //==================================== END ================================

    $rootScope.Tab = {
        All: true,
        Cancel: false,
        Tranfer: false,
        Change: false,
        Replace: false,
        ReleaseError: false,
        Delete: false,
        Wating: false,
    };

    $rootScope.InvoiceTabClick = function (xxx) {
        $scope.Tab.All = false;
        $scope.Tab.Cancel = false;
        $scope.Tab.Tranfer = false;
        $scope.Tab.Change = false;
        $scope.Tab.Replace = false;
        $scope.Tab.ReleaseError = false;
        $scope.Tab.Delete = false;
        $scope.Tab.Wating = false;
        $scope.Filter = new Object();
        $scope.ListInvoice = [];
        switch (xxx) {
            case 1:
                $scope.Tab.All = true;
                $scope.Filter = new Object();
                $scope.GetInvoice(1);
                break;
            case 2:
                $scope.Tab.Cancel = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 3:
                $scope.Tab.Tranfer = true;
                $scope.Filter.INVOICETYPE = 4;
                $scope.GetInvoice(1);
                break;
            case 4:
                $scope.Tab.Change = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
                break;
            case 5:
                $scope.Tab.Replace = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 6:
                $scope.Tab.ReleaseError = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceReleaseError(1);
                break;
            case 7:
                $scope.Tab.Delete = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceDelete(1);
                break;
            case 8:
                $scope.Tab.Wating = true;
                $scope.Filter = new Object();
                $scope.GetInvoiceWait(1);
                break;
            default:
                $scope.Tab.All = true;
                break;
        }
        $('.dropdown-menu').removeClass('show');
    }
    $rootScope.ReloadInvoice = function () {
        if ($location.path().toString().includes('/quan-ly-hoa-don')) {

            if ($scope.Tab.Change) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
            } else if ($scope.Tab.Replace) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else if ($scope.Tab.Cancel) {
                $scope.GetInvoiceByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else if ($scope.Tab.Wating) {
                $scope.GetInvoiceWait($scope.currentpage);
            }
            else if ($scope.Tab.Delete) {
                $scope.GetInvoiceDelete($scope.currentpage);
            }
            else if ($scope.Tab.ReleaseError) {
                $scope.GetInvoiceReleaseError($scope.currentpage);
            }
            else {
                $scope.ListInvoice = new Array();
                $scope.GetInvoice($scope.currentpage);
            }
        }
    }
    /**tuyennv - 20200327
    * Declare a function help to call in js file by using $window service
    * Reload invoice index without clearing filter criteria
    */
    $window.ReloadInvoice = function () {
        $rootScope.ReloadInvoice();
    };

    $scope.GetInvoice = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();

        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            if ($scope.FROMTIME > $scope.TOTIME) {
                toastr.warning('Ngày bắt đầu phải nhỏ hơn ngày kết thúc');
                return;
            }
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        if (parseInt($scope.Filter.FROMNUMBER) > parseInt($scope.Filter.TONUMBER)) {
            toastr.warning('Số hóa đơn bắt đầu phải nhỏ hơn số hóa đơn kết thúc');
            return;
        }

        $scope.LoadCookie_Invoice();

        if (!intpage || intpage <= 0) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoice';
        if (!$scope.cookie.RowNum)
            $scope.cookie.RowNum = 10;
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;

        successCallback = function (response) {
            if ($scope.cookie.RowNum >= 500) {
                let data = response.result;
                let firstScreenData = data.splice(0, 50); // Sử dụng splice
                $scope.ListInvoice = firstScreenData;
                $scope.$apply();

                setTimeout(() => {
                    function render(n) {
                        // Mỗi lần hiện thị 50 items
                        let partialData = data.splice(0, 50);
                        $scope.ListInvoice = $scope.ListInvoice.concat(partialData);
                        $scope.$apply();
                        if (n) {
                            setTimeout(() => {
                                render(n - 1);
                            }, 25)
                        }
                    }
                    render(20);// chạy 20 loop mới mỗi loop là 50 items
                }, 450);
            }
            else {
                $timeout(function () {
                    $scope.ListInvoice = response.result;
                }, $scope.cookie.RowNum * 0.5)
            }

            $scope.TotalPages = response.TotalPages;
            $scope.TotalRow = response.TotalRow;
            $scope.FROMTIME = '';
            $scope.TOTIME = '';
        }

        AjaxRequest(action, {
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        }, "POST", true, "json", successCallback);

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUSNAME;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].value;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.ID.toString() === $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    //$scope.GetInvoice = function (intpage) {
    //    if (!$scope.Filter)
    //        $scope.Filter = new Object();

    //    if ($scope.Filter.TIME == '5') {
    //        $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
    //        $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
    //        if ($scope.FROMTIME > $scope.TOTIME) {
    //            toastr.warning('Ngày bắt đầu phải nhỏ hơn ngày kết thúc');
    //            return;
    //        }
    //        $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
    //        if (!$scope.FROMTIME || !$scope.TOTIME) {
    //            $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
    //        }
    //    }
    //    if (parseInt($scope.Filter.FROMNUMBER) > parseInt($scope.Filter.TONUMBER)) {
    //        toastr.warning('Số hóa đơn bắt đầu phải nhỏ hơn số hóa đơn kết thúc');
    //        return;
    //    }
    //    $scope.LoadCookie_Invoice();
    //    if (!intpage || intpage <= 0) {
    //        intpage = 1;
    //    }
    //    if (intpage > $scope.TotalPages) {
    //        intpage = $scope.TotalPages;
    //    }
    //    $scope.currentpage = intpage;
    //    var action = url + 'GetInvoice';
    //    if (!$scope.cookie.RowNum)
    //        $scope.cookie.RowNum = 10;
    //    $scope.ListInvoice = new Array();
    //    $scope.TotalPages = 1;
    //    $scope.TotalRow = 1;

    //    successCallback = function (response) {
    //        $scope.ResultLength = response.result.length;
    //        if ($scope.cookie.RowNum >= 500) {
    //            $scope.ListInvoice = splitArr(response.result, 0);
    //            $timeout(function () {
    //                if ($scope.ListInvoice.length != $scope.ResultLength) {
    //                    let isBigData = true;
    //                    let temp_limit = 50;
    //                    let temp_page = $scope.cookie.RowNum / temp_limit;
    //                    let page = 1;
    //                    while (page <= temp_page) {
    //                        let result = splitArr(response.result, page);
    //                        if (isBigData) {
    //                            $scope.ListInvoice = $scope.ListInvoice.concat(result);
    //                            page = page + 1;
    //                        }
    //                        $scope.$apply();
    //                    }
    //                }
    //            }, 0)
    //        }
    //        else {
    //            $timeout(function () {
    //                $scope.ListInvoice = response.result;
    //            }, $scope.cookie.RowNum * 0.5)
    //        }

    //        $scope.TotalPages = response.TotalPages;
    //        $scope.TotalRow = response.TotalRow;
    //        $scope.FROMTIME = '';
    //        $scope.TOTIME = '';
    //    }

    //    AjaxRequest(action, {
    //        form: $scope.Filter,
    //        currentPage: $scope.currentpage,
    //        itemPerPage: $scope.cookie.RowNum
    //    }, "POST", true, "json", successCallback);

    //    $scope.FilterApply = [];
    //    for (var prop in $scope.Filter) {
    //        if ($scope.Filter[prop] != null) {
    //            var o = {
    //                key: prop,
    //                value: $scope.Filter[prop]
    //            };

    //            if (prop == 'INVOICESTATUS') {
    //                var f = $scope.ListInvoiceStatus.filter(function (item) {
    //                    return item.INVOICESTATUSID == $scope.Filter[prop];
    //                });
    //                o.value = f[0].INVOICESTATUSNAME;
    //            }

    //            if (prop == 'TIME') {
    //                var f = $scope.Timepickers.Options.filter(function (item) {
    //                    return item.value == $scope.Filter[prop];
    //                });
    //                if (f.length > 0) {
    //                    o.value = f[0].value;
    //                }
    //            }

    //            if (prop == 'FORMCODE') {
    //                var f = $scope.ListFormCode.filter(function (item) {
    //                    return item.FORMCODE == $scope.Filter[prop];
    //                });
    //                o.value = f[0].FORMCODE;
    //            }

    //            if (prop == 'PAYMENTSTATUS') {
    //                var f = $scope.ListPaymentStatus.filter(function (item) {
    //                    return item.ID.toString() === $scope.Filter[prop];
    //                });
    //                o.value = f[0].PAYMENTSTATUS;
    //            }

    //            if (prop == 'SYMBOLCODE') {
    //                var f = $scope.ListSymbolCode.filter(function (item) {
    //                    return item.SYMBOLCODE == $scope.Filter[prop];
    //                });
    //                o.value = f[0].SYMBOLCODE;
    //            }

    //            $scope.FilterApply.push(o);
    //        }
    //    }
    //}

    $scope.GetInvoiceWait = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            //if (!$scope.FROMTIME == 'undefined' || $scope.TOTIME == 'undefined') {
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage || intpage <= 0) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceWating';
        if (!$scope.cookie.RowNum)
            $scope.cookie.RowNum = 10;

        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;

        //successCallback = function (response) {
        //    $timeout(function () {
        //        $scope.ListInvoice = response.result;
        //        $scope.TotalPages = response.TotalPages;
        //        $scope.TotalRow = response.TotalRow;
        //        $scope.currentpage = 1;
        //        $scope.FROMTIME = '';
        //        $scope.TOTIME = '';
        //    }, 1000);
        //}

        //successCallback = function (response) {
        //    $scope.ResultLength = response.result.length;
        //    if ($scope.cookie.RowNum >= 500) {
        //        $scope.ListInvoice = splitArr(response.result, 0);
        //        $timeout(function () {
        //            if ($scope.ListInvoice.length != $scope.ResultLength) {
        //                let isBigData = true;
        //                let temp_limit = 50;
        //                let temp_page = $scope.cookie.RowNum / temp_limit;
        //                let page = 1;
        //                while (page <= temp_page) {
        //                    let result = splitArr(response.result, page);
        //                    if (isBigData) {
        //                        $scope.ListInvoice = $scope.ListInvoice.concat(result);
        //                        page = page + 1;
        //                    }
        //                    $scope.$apply();
        //                }
        //            }
        //        }, 0)
        //    }
        //    else {
        //        $timeout(function () {
        //            $scope.ListInvoice = response.result;
        //        }, $scope.cookie.RowNum * 0.5)
        //    }

        //    $scope.TotalPages = response.TotalPages;
        //    $scope.TotalRow = response.TotalRow;
        //    $scope.FROMTIME = '';
        //    $scope.TOTIME = '';
        //}

        successCallback = function (response) {
            if ($scope.cookie.RowNum >= 500) {
                let data = response.result;
                let firstScreenData = data.splice(0, 50); // Sử dụng splice
                $scope.ListInvoice = firstScreenData;
                $scope.$apply();

                setTimeout(() => {
                    function render(n) {
                        // Mỗi lần hiện thị 50 items
                        let partialData = data.splice(0, 50);
                        $scope.ListInvoice = $scope.ListInvoice.concat(partialData);
                        $scope.$apply();
                        if (n) {
                            setTimeout(() => {
                                render(n - 1);
                            }, 25)
                        }
                    }
                    render(20);// chạy 20 loop mới mỗi loop là 50 items
                }, 450);
            }
            else {
                $timeout(function () {
                    $scope.ListInvoice = response.result;
                }, $scope.cookie.RowNum * 0.5)
            }

            $scope.TotalPages = response.TotalPages;
            $scope.TotalRow = response.TotalRow;
            $scope.FROMTIME = '';
            $scope.TOTIME = '';
        }

        AjaxRequest(action, {
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        }, "POST", true, "json", successCallback);

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].value;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.ID.toString() === $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceReleaseError = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceReleaseError';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                    $scope.FROMTIME = '';
                    $scope.TOTIME = '';

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].TIME;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.PAYMENTSTATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceDelete = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceDelete';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum === null ? 100 : $scope.cookie.RowNum
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                    $scope.FROMTIME = '';
                    $scope.TOTIME = '';

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });

        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].TIME;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.PAYMENTSTATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        $rootScope.ReloadInvoice();
    };

    $scope.SeleteRow = function (list, item, isselect, invoiceId) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (item) {
            const index = $scope.ListInvoiceChecked.indexOf(invoiceId);
            if (index > -1) {
                $scope.ListInvoiceChecked.splice(index, 1);
            }
            isselect = false;
            list.forEach(function (item) {
                if (item.ID === invoiceId)
                    item.ISSELECTED = isselect;
            });
        }
        else {
            $scope.ListInvoiceChecked.push(invoiceId);
            if (find.length == list.length - 1) {
                isselect = true;
            }
            else {
                list.forEach(function (item) {
                    if (item.ID === invoiceId)
                        item.ISSELECTED = true;
                });
            }
        }

        $scope.SetCountInvoiceSelected();
    }

    $scope.SelectAllInvoice = function (list, isselect) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == isselect;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !isselect;
            });
        }
        $scope.SetCountInvoiceSelected();
    }

    $scope.SetCountInvoiceSelected = function () {
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length > 0) {
            $('#invoice-seleced-sign').show();
            $('#invoice-seleced-sign').html('(' + listInvoiceChecked.length + ' HĐ đã chọn)');
        }
        else {
            $('#invoice-seleced-sign').html('');
            $('#invoice-seleced-sign').hide();
        }

        if (listInvoiceChecked && listInvoiceChecked.length > 0) {
            $('#invoice-seleced-sign-waiting').show();
            $('#invoice-seleced-sign-waiting').html('(' + listInvoiceChecked.length + ' HĐ đã chọn)');
        }
        else {
            $('#invoice-seleced-sign-waiting').html('');
            $('#invoice-seleced-sign-waiting').hide();
        }
    }

    $scope.Sign = function (idInvoice) {
        LoadingShow();
        Sign(idInvoice);
    }

    $scope.SignMultipleInvoice = function (isSignAll, isUSBToken) {
        $scope.ListInvoiceChecked = [];
        if (!isSignAll) {
            var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS == 2 && obj.ISSELECTED === true; });
            if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                toastr.warning("Hóa đơn với mẫu số <b style='color: #222;'>" + listInvoiceSigned[0].FORMCODE + "/" + listInvoiceSigned[0].SYMBOLCODE + "</b> đã được phát hành.");
                return;
            }
            var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
            if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                toastr.warning("Bạn chưa chọn hóa đơn phát hành.");
                return;
            }
            if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                for (var i = 0; i < listInvoiceChecked.length; i++) {
                    $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                }
                var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
                LoadingShow();
                if (isUSBToken) {
                    SignMultiple(lstInvoiceid, true);
                }
                else {
                    SignMultiple(lstInvoiceid, false);
                }
            }
        }
        else {
            LoadingShow();
            SignMultiple([].join(";"));
        }
    }

    //$scope.BackgroundJobSignInvoice = function () {
    //    var invoiceHub = $.connection.signlRConf;
    //    var userid = sessionStorage.getItem("userNameSS");

    //    $.connection.hub.qs = { 'username': userid };
    //    var invRow = 0;
    //    invoiceHub.client.newMessageReceivedSignInvoice = function (message) {
    //        LoadingHide();
    //        invRow = invRow + 1;
    //        console.log(invRow);
    //        if (invRow === message.totalRow) {
    //            LoadingHide();
    //            // 
    //            var messageSuccess = 'Phát hành thành công <b>' + invRow + '/' + message.totalRow + '</b> hóa đơn.';
    //            $('.invoiceResult').html("Hoàn Thành");
    //            $("#progressTab").hide();
    //            toastr.success(messageSuccess);
    //            invRow = 0;
    //            var confirmContinue = function (result) {
    //                if (result) {
    //                    // Trigger cập nhật số hiện tại và gửi email
    //                    $scope.UpdateAfterSigning();
    //                    return false;
    //                }
    //                else {
    //                    // Trigger cập nhật số hiện tại và gửi email
    //                    $scope.UpdateAfterSigning();
    //                    $rootScope.ReloadInvoice();
    //                }
    //            };
    //            confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Phát hành hóa đơn', 'Có', 'Không', confirmContinue);
    //        }
    //        else {
    //            LoadingShow();
    //            $("#progressTab").show();
    //            var percent = Math.floor((invRow / message.totalRow) * 100);
    //            $('.invoiceResult').html('Đang phát hành hóa đơn: ' + percent + '%');
    //        }
    //    };

    //    $.connection.hub.start().done(function () {
    //        $("#progressTab").hide();
    //    })
    //};

    $scope.BackgroundJobSignInvoice = function () {
        var invoiceHub = $.connection.signlRConf;
        var userid = sessionStorage.getItem("userNameSS");

        $.connection.hub.qs = { 'username': userid };
        var invRow = 0;
        invoiceHub.client.newMessageReceivedSignInvoice = function (message) {
            invRow = invRow + 1;
            console.log(invRow);
            if (message.currentRow === message.totalRow) {
                // 
                var messageSuccess = 'Phát hành thành công <b>' + message.currentRow + '/' + message.totalRow + '</b> hóa đơn.';
                $('.invoiceResult').html("Hoàn Thành");
                $("#progressTab").hide();
                toastr.success(messageSuccess);
                invRow = 0;
                var confirmContinue = function (result) {
                    if (result) {
                        // Trigger cập nhật số hiện tại và gửi email
                        $scope.UpdateAfterSigning();
                        return false;
                    }
                    else {
                        // Trigger cập nhật số hiện tại và gửi email
                        $scope.UpdateAfterSigning();
                        $rootScope.ReloadInvoice();
                    }
                };
                confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Phát hành hóa đơn', 'Có', 'Không', confirmContinue);
            }
            else {
                $("#progressTab").show();
                var percent = Math.floor((message.currentRow / message.totalRow) * 100);
                $('.invoiceResult').html('Đang phát hành hóa đơn: ' + percent + '%');
            }
        };

        $.connection.hub.start().done(function () {
            $("#progressTab").hide();
        })
    };

    $scope.BackgroundJobSignInvoice();

    $scope.UpdateAfterSigning = function () {
        var action = '/Home/UpdateAfterSigning';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    console.log(response.msg);
                } else {
                    console.log(response.msg);
                }
            } else {
                console.log(response.msg);
            }
        });
    }

    $scope.MergingPdfFile = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần in.");
            return;
        }
        var listInvoiceUnSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS == 1 && obj.ISSELECTED === true; });
        if (listInvoiceUnSigned && listInvoiceUnSigned.length > 0) {
            toastr.warning("Hệ thống không hỗ trợ in hàng loạt với hóa đơn chưa phát hành.");
            return;
        }
        if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
            for (var i = 0; i < listInvoiceChecked.length; i++) {
                $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
            }
            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'MergingPdfFile';
            successCallback = function (data) {
                toastr.success(data.msg);
                window.open('Uploads/' + data.msg + "?v=" + new Date().getTime());
            }
            AjaxRequest(action, { idInvoices: lstInvoiceid }, "POST", true, "json", successCallback);
        }
    }

    $scope.ReMergingPdfFile = function () {
        $scope.ListInvoiceChecked = [];
        for (var i = 0; i < $scope.ListInvoice.length; i++) {
            $scope.ListInvoiceChecked.push($scope.ListInvoice[i].ID);
        }
        var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
        var action = url + 'ReMergingPdfFile';
        successCallback = function (data) {
            toastr.success(data.msg);
        }

        AjaxRequest(action, { idInvoices: lstInvoiceid }, "POST", true, "json", successCallback);
    }

    $scope.RemoveInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }

                var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                    toastr.warning("Bạn không được xóa hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                    return;
                }

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RemoveInvoice';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.OpenModalUpdatePaymentStatus = function () {
        $('#modal_choose_paymentstatus').dialog({
            width: '350px',
            modal: true,
            resizable: false,
            show: {
                effect: 'drop',
                duration: 300
            },
            hide: {
                effect: 'drop',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal_choose_paymentstatus').show();
            }
        });
    }

    $scope.UpdatePaymentStatus = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                // Kiểm tra xem đã chọn bản ghi cần cập nhật chưa?
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần cập nhật.");
                    return;
                }

                //var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                //if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                //    toastr.warning("Bạn không được cập nhật hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                //    return;
                //}

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'UpdatePaymentStatus';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid,
                paymentStatus: $scope.PAYMENTSTATUS
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
            $('#modal_choose_paymentstatus').dialog("close");
        };
        confirm("Bạn có thực sự muốn cập nhật các hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.OpenModalUpdatePartner = function () {
        $('#modal_update_partner').dialog({
            width: '550px',
            height: 160,
            modal: true,
            resizable: false,
            show: {
                effect: "drop",
                duration: 500
            },
            hide: {
                effect: "drop",
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_update_partner').show();
            },
            open: function (event, ui) {
                $('.ui-dialog-content').css('overflow', 'visible'); //this line does the actual hiding
            }
        });
    }

    $scope.SuggestUser = function () {
        var obj = $rootScope.SelectedUser;
        if (obj) {
            $scope.User.CUSID = obj.CUSID;
        }
    }

    $scope.UpdatePartner = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                // Kiểm tra xem đã chọn bản ghi cần cập nhật chưa?
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần cập nhật.");
                    return;
                }

                //var listInvoiceSigned = $scope.ListInvoice.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                //if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                //    toastr.warning("Bạn không được cập nhật hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                //    return;
                //}

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'UpdatePartner';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid,
                partnerEmail: $scope.User.EMAIL
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
            $('#modal_update_partner').dialog("close");
        };
        confirm("Bạn có thực sự muốn cập nhật các hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.DownloadInvoiceExcel = function (item) {
        $scope.ListInvoiceChecked = [];
        if (item) {
            $scope.ListInvoiceChecked.push(item);
        }
        else {
            var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
            if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                toastr.warning("Bạn chưa chọn hóa đơn cần export.");
                return;
            }

            if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                for (var i = 0; i < listInvoiceChecked.length; i++) {
                    $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                }
            }
        }

        var lstInvoiceid = $scope.ListInvoiceChecked.join(";");

        var action = url + 'DownloadInvoiceExcel';
        var datasend = {
            idInvoices: lstInvoiceid
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.RecoverInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần phục hồi.");
                    return;
                }
                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RecoverInvoice';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn phục hồi Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }

    $scope.LoadDashboard = function () {
        var action = '/Invoice/' + 'LoadDashboard';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    //$scope.TotalMoneyNotPay = response.TotalMoneyNotPay;
                    $scope.TotalMoneyPaied = response.TotalMoneyPaied;
                    $scope.TotalMoneyNotApproval = response.TotalMoneyNotApproval;
                    $scope.TotalInvoiceSigned = response.totalInvoiceSigned;

                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetProvince');
            }
        });
    };

    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $scope.PreviewReferenceInvoice = function (item) {
        window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
    }

    $scope.PreviewInvoice = function (item, isSignLink) {
        //Nếu chưa có file thì tạo file
        if (isSignLink === false) {
            var action = url + 'CreateFilePdfToView';
            var datasend = JSON.stringify({
                invoiceId: item
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    window.open('NOVAON_FOLDER' + response.msg + "?v=" + new Date().getTime());
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoice');
                }
                LoadingHide();
            });
        }
        else { // ngược lại view file ra
            window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
        }
    }

    $rootScope.InvocieTYPE = 2;
    //TuyenNV - Taỉ hóa đơn trực tiếp
    $scope.DownloadInvoice = function (item) {
        var action = '';
        var datasend = '';
        if ($rootScope.InvocieTYPE == 1) {
            action = url + 'InvoiceConvert';
            datasend = JSON.stringify({
                invoice: item
            });
        } else {
            action = url + 'CreateFilePdfToView';
            datasend = JSON.stringify({
                invoiceID: item.ID
            });
        }
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                var link = document.createElement('a');
                link.href = 'NOVAON_FOLDER' + response.msg + "?v=" + new Date().getTime();
                link.download = 'ONFINANCE_INVOICE_HOA_DON_' + item.COMTAXCODE + '_' + item.FORMCODE + '_' + item.SYMBOLCODE + '_' + item.NUMBER + '.pdf';
                link.dispatchEvent(new MouseEvent('click'));
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DownloadInvoice');
            }
            LoadingHide();
        });
    }


    $scope.PreviewModifiedReport = function (item) {
        window.open('NOVAON_FOLDER' + item.MODIFIEDLINK + "?v=" + new Date().getTime());
    }

    $scope.PreviewCancelReport = function (item) {
        window.open('NOVAON_FOLDER' + item.CANCELLINK + "?v=" + new Date().getTime());
    }

    $scope.ConvertInv = function (item) {
        var result = confirm('Sau khi chuyển sang hóa đơn chuyển đổi. Hóa đơn chỉ được phép in một lần duy nhất. Bạn có thực sự muốn chuyển đổi hóa đơn ?');
        if (result) {
            $scope.IsLoading = true;
            var action = url + 'UpdateConvertInvoice';
            var datasend = JSON.stringify({
                invoice: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Thành công!')
                        $rootScope.ReloadInvoice();
                    } else {
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js)');
                }
            });
            $scope.IsLoading = false;
        }
    }

    /**
     * truongnv 2020-02-14
     * Gán data filter khi người dùng chọn
     * @param {any} type
     * @param {any} intpage
     */
    $scope.GetInvoiceByStatus = function (type, intpage, reportType) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        $scope.LoadCookie_Invoice();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceByStatus';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            type: type,
            intpage: intpage,
            reportType: reportType
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        /*
         *Gán data filter
         * */
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    o.value = f[0].TIME;
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.GetInvoiceWaiting = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceNumerWaiting';
        var datasend = JSON.stringify({
            currentPage: $scope.currentpage,
            itemPerPage: 10
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoiceWaiting = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    $rootScope.ReloadGetNumber = function () {
        $scope.GetNumber($scope.FormNumber.CURRENTPAGE);
    }

    $scope.GetNumber = function (intpage) {
        $scope.FormNumber = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.FormNumber.ITEMPERPAGE = 10;
        $scope.FormNumber.CURRENTPAGE = intpage;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        $scope.ListNumber = new Array();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListNumber = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumber');
            }
            LoadingHide();
        });
    }

    /*
     * Đính kèm file khi gửi mail cho khách hàng
     * @param {any} event
     */
    $scope.uploadFile = function (event) {
        LoadingShow();
        $('#div-file-active').show();
        var files = event.target.files;
        var isPass = CheckFile(files);
        if (isPass) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var reader = new FileReader();
                reader.onloadend = function (event) {
                    $timeout(function () {
                        var base64File = event.target.result.split(',')[1];
                        $('#file-selected').append('<span class="has-tag-attment-file" name = "' + file.name + '" rel= "' + base64File + '" onclick="DeleteFile(this)">' + file.name + '<img src="/Images/close.png"></span>');
                        LoadingHide();
                    }, 100);
                };
                reader.readAsDataURL(file);
            }
        }
        else
            LoadingHide();
    }

    $scope.ConfirmChange = function (item) {
        if (item.IDTEMP > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
            };
            confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
            return false;
        }
        $rootScope.ModalInvoice(item, 2);
        $('.modal-invoice').modal('show');
    }

    $scope.CancelInvoiceConfirm = function (item) {
        if (item.CANCELREASON != null && item.CANCELREASON.trim() != "") {
            $rootScope.ModalCancelInvoice(item);
            $('.modal-cancel-invoice').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
                else {
                    $rootScope.IsUpdateCancelReport = false;
                    $rootScope.ModalCancelReport(null, item);
                    $('.modal-cancel-report').modal('show');
                }
            };
            confirm('Bạn có muốn lập biên bản hủy cho hóa đơn này không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    $scope.ReplaceInvoiceConfirm = function (item) {
        var number = item.NUMBER;
        var formCode = item.FORMCODE;
        var symbolCode = item.SYMBOLCODE;
        if (item.INVOICETYPE == 3) {
            if (item.IDTEMP > 0 && item.IDTEMP > item.ID) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> đã được lập hóa đơn thay thế <strong><' + formCode + ' - ' + symbolCode + ' - ' + 'Chưa cấp số></strong>. Bạn có muốn xem không?', 'Hóa đơn thay thế', 'Không', 'Có', confirmContinue);
            }
            else {
                $rootScope.ModalInvoice(item, 6);
                $('.modal-invoice').modal('show');
            }
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    return false;
                }
                else {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
            };
            confirm('Để lập được hóa đơn thay thế cho hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> bạn cần thực hiện xóa bỏ hóa đơn này trước. Bạn có muốn thực hiện xóa bỏ hóa đơn không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    /* TuyenNV - 20200303
     Kiểm tra xem hóa đơn bị điều chỉnh đã có biên bản/ hóa đơn điểu chỉnh
     + Chưa có biên bản => open modal tạo biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản, có hóa đơn điều chỉnh => view hóa đơn điều chỉnh
     */
    $scope.ModifiedInvoiceConfirm = function (item) {
        if (item.CHANGEREASON != null && item.CHANGEREASON.trim() != "") {
            if (item.IDTEMP > 0) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                return false;
            }
            $rootScope.ModalInvoice(item, 5);
            $('.modal-invoice').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    if (item.IDTEMP > 0) {
                        var confirmContinue = function (result) {
                            if (!result)
                                return false;
                            window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                        };
                        confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                        return false;
                    }
                    $rootScope.ModalInvoice(item, 5);
                    $('.modal-invoice').modal('show');
                }
                else {
                    $rootScope.IsUpdateModifiedReport = false;
                    $rootScope.ModalModifiedReport(null, item);
                    $('.modal-modified-report').modal('show');
                }
            };

            if (item.ISEXISTMODIFIEDREPORT != '1') {
                confirm('Bạn có muốn lập biên bản điều chỉnh cho hóa đơn này không?', 'Hóa đơn điểu chỉnh', 'Không', 'Có', confirmContinue);
            }
            else {
                if (item.IDTEMP > 0) {
                    var confirmContinue = function (result) {
                        if (!result)
                            return false;
                        window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                    };
                    confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                    return false;
                }
                $rootScope.ModalInvoice(item, 5);
                $('.modal-invoice').modal('show');
            }
        }
    }
    // End

    $scope.OpenModalCancelReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTCANCELREPORT == '1') {
            $rootScope.IsUpdateCancelReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTCANCELREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    LoadingHide
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateCancelReport = false;
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
    }

    $scope.OpenModalModifiedReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTMODIFIEDREPORT == '2') {
            $rootScope.IsUpdateModifiedReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTMODIFIEDREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    LoadingHide();
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateModifiedReport = false;
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
    }

    $scope.DownloadReleaseDocument = function (item) {
        var action = url + 'DownloadReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadReleaseDocumentXML = function (item) {
        var action = url + 'DownloadReleaseDocumentXML';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadIssuedDocument = function (item) {
        var action = url + 'DownloadIssuedReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadReleaseInvoiceTemplate = function (item) {
        var action = url + 'DownloadReleaseInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.DownloadConvertibleInvoiceTemplate = function (item) {
        var action = url + 'DownloadConvertibleInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.DownloadDiscountInvoiceTemplate = function (item) {
        var action = url + 'DownloadDiscountInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.PreviewInvoiceTemplate = function (item) {
        $scope.GlobalInvoiceToViewTemplate = item;
        var action = url + 'PreviewInvoiceTemplate';
        var datasend = JSON.stringify({
            numberBO: item,
            isConvert: false
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ShowPopupFileView(response.msg);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoiceTemplate');
            }
            LoadingHide();
        });
    }

    $window.PreviewInvoiceTemplate = function (isConvert) {
        var action = url + 'PreviewInvoiceTemplate';
        var datasend = JSON.stringify({
            numberBO: $scope.GlobalInvoiceToViewTemplate,
            isConvert: isConvert
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('#frViewFileInvoice').attr('src', response.msg);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoiceTemplate');
            }
            LoadingHide();
        });
    }

    $scope.ModalSendEmail = function (item) {
        $('#modal_send_email').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            show: {
                effect: 'drop',
                duration: 500
            },
            hide: {
                effect: 'drop',
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_send_email').show();
            }
        });

        $('#file-selected').html('');

        $scope.GetEmailHistory(item.ID);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        if (!item.CUSEMAIL || item.CUSEMAIL.trim() === "") {
            item.CUSEMAIL = item.CUSEMAILSEND;
        }
        $scope.Invoice.RECIEVEREMAIL = item.CUSEMAIL;
    }

    $scope.SendEmail = function () {
        //if (!$scope.Invoice.RECIEVERNAME) {
        //    alert('Vui nhập vào tên người nhận!');
        //    return false;
        //}
        if (!$scope.Invoice.RECIEVEREMAIL) {
            alert('Vui nhập vào email người nhận!!');
            return false;
        }
        var lstEmail = $scope.Invoice.RECIEVEREMAIL.split(',');
        for (var i = 0; i < lstEmail.length; i++) {
            if (!validation.isEmailAddress(lstEmail[i].trim())) {
                alert('Vui lòng nhập đúng định dạng email ' + (i + 1));
                return false;
            }
        }

        $scope.Invoice.CUSBUYER = $scope.Invoice.RECIEVERNAME;
        $scope.Invoice.CUSEMAIL = $scope.Invoice.RECIEVEREMAIL;

        /*
         * Lấy danh sách file đính kèm của người dùng khi gửi email
         */
        var fileNames = new Array();
        var objFileBase64 = new Array();
        var fFile = $('#file-selected .has-tag-attment-file');
        if (fFile.length > 0) {
            for (var i = 0; i < fFile.length; i++) {
                objFileBase64.push(fFile[i].getAttribute('rel'));
                fileNames.push(fFile[i].getAttribute('name'));
            }
        }
        var action = "";
        if ($scope.Invoice.INVOICETYPE === 3) {
            action = url + 'SendCancelEmail';
        }
        else {
            action = url + 'SendEmail';
        }
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            imgBase64: JSON.stringify(objFileBase64),
            fileNames: fileNames.join(";")
        });

        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_email').dialog("close");
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.error(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
    }

    $scope.OpenSendMultipleEmail = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần gửi email.");
            return;
        }

        var listDeletedInvoice = $scope.ListInvoice.filter(function (obj) { return obj.INVOICETYPE !== 3 && obj.ISSELECTED === true; });
        if (listDeletedInvoice && listDeletedInvoice.length > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                listInvoiceChecked = listInvoiceChecked.filter(function (obj) { return obj.INVOICETYPE !== 3 });
                $scope.INVOICEQUANTITIES = listInvoiceChecked.length;
                if (listInvoiceChecked.length > 50) {
                    toastr.error("Số lượng email tối đa không quá 50.", "Cảnh báo")
                    return false;
                }
                $('#modal_send_multiple_email').dialog({
                    width: '1200px',
                    modal: true,
                    resizable: false,
                    show: {
                        effect: 'drop',
                        duration: 500
                    },
                    hide: {
                        effect: 'drop',
                        duration: 500
                    },
                    create: function (event, ui) {
                        $('#modal_send_multiple_email').show();
                    }
                });
                $timeout(function () {
                    listInvoiceChecked.forEach((item) => {
                        if (!item.CUSEMAIL || item.CUSEMAIL.trim() === "") {
                            item.CUSEMAIL = item.CUSEMAILSEND;
                        }
                    })
                    angular.copy(listInvoiceChecked, $scope.ListInvoiceChecked);
                }, 100)
            };
            confirm('Chương trình chỉ thực hiện gửi các hóa đơn đã phát hành và chưa bị xóa bỏ?', 'Lưu ý gửi mail', 'Không', 'Đồng ý', confirmContinue);
        }
    }

    $scope.SendMultipleEmail = function () {
        var action = url + 'SendMultipleEmail';
        var datasend = JSON.stringify({
            invoices: $scope.ListInvoiceChecked,
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_multiple_email').dialog('close');
                } else {
                    toastr.error(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendMultipleEmail');
            }
        });
    }

    $scope.CurrentItem = new Object();

    $scope.EditTemplate = function (item) {
        $rootScope.EditingTemplate = item;
        //var obj = {
        //    C: item.COMTAXCODE,
        //    F: item.FORMCODE,
        //    S: item.SYMBOLCODE,
        //    R: item.FROMNUMBER,
        //    O: item.TONUMBER,
        //    T: item.TEMPLATEPATH
        //};
        //Mã hóa đối tượng và truyền theo phương thức GET
        document.location.href = "/#/mau-hoa-don/" + btoa(item.TEMPLATEPATH);
    };

    /*
     * Đính kèm file biên bản
     * @param {any} event
     */

    $scope.OpenModalUploadFileReport = function (item) {
        $scope.CurrentInvoiceFileReport = item;
        $('#modal_upload_file_report').dialog({
            width: '550px',
            height: 250,
            modal: true,
            resizable: false,
            show: {
                effect: "drop",
                duration: 300
            },
            hide: {
                effect: "drop",
                duration: 300
            },
            create: function (event, ui) {
                $('#modal_upload_file_report').show();
            },
            open: function (event, ui) {
                $('.ui-dialog-content').css('overflow', 'visible'); //this line does the actual hiding
            }
        });
    }

    $('#frmUploadFileReport').fileupload({
        autoUpload: false,
        add: function (e, data) {
            LoadingShow();
            var fileData = new FormData();
            var data = data.files[0];
            fileData.append('file0', data);
            fileData.append('currentItem', $scope.CurrentInvoiceFileReport.ID);
            fileData.append('comTaxCode', $scope.CurrentInvoiceFileReport.COMTAXCODE);

            var action = url + 'UploadFileReport';
            $.ajax({
                type: 'POST',
                url: action,
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    $timeout(function () {
                        if (result.rs) {
                            $scope.CurrentItem.ATTACHMENTFILELINK = result.id;
                            $rootScope.ReloadInvoice();
                            toastr.success(result.msg);
                            $('#modal_upload_file_report').dialog('close');
                        } else {
                            toastr.error(result.msg);
                        }
                        LoadingHide();
                    }, 10);
                },
                error: function (xhr, status, p3, p4) {
                    alert('Lỗi không thể tải lên file: ' + data.name);
                    LoadingHide();
                }
            });
        }
    });

    $scope.DownloadFileReport = function (item) {
        var action = url + 'DownloadFileReport';
        var datasend = {
            invoiceId: item.ID
        };

        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.RemoveFileReport = function (item) {
        var action = url + 'RemoveFileReport';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.error(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - RemoveFileReport');
            }
            LoadingHide();
        });
    }

    $scope.ExportExcel = function () {
        var action = url + 'ExportExcel';
        var datasend = {
            form: $scope.Filter,
        };
        $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        })
    }

    $scope.GetEmailHistory = function (invoiceId) {
        $scope.ListEmailHistory = new Array();
        var action = url + 'GetEmailHistoryByInvoiceId';
        var datasend = JSON.stringify({
            id: invoiceId
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListEmailHistory = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetEmailHistory');
            }
            LoadingHide();
        });
    }
    LoadingHide();

    /*
     * TuyenNV - 20200602
     * Xem nhanh hóa đơn.
    */
    $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
    if ($scope.IsGridViewDisplay === undefined) {
        setCookie("NovaonIsGridView", 0, 30);
        $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
    } else if ($scope.IsGridViewDisplay === "1" && $("#left-mainnavi").css("width") === "230px") {
        $('#sidebar').trigger('click');
    }
    $scope.CloseSideBar = function () {
        if ($scope.IsGridViewDisplay === undefined) {
            setCookie("NovaonIsGridView", 1, 30);
        }
        if ($scope.IsGridViewDisplay !== undefined) {
            if ($scope.IsGridViewDisplay === "1") {
                setCookie("NovaonIsGridView", 0, 30);
            }
            else {
                setCookie("NovaonIsGridView", 1, 30);
            }
        }
        $scope.IsGridViewDisplay = getCookie("NovaonIsGridView");
        $('#sidebar').trigger('click');
    }
    //HĐ thường 
    $scope.QuickviewInvoice = function (invoice) {
        $rootScope.InvocieTYPE = 2;
        $scope.CurrentActive = invoice.ID;
        $scope.CurrentInvoice = invoice;
        var action = url + 'QuickviewInvoice';
        var datasend = JSON.stringify({
            invoiceId: invoice.ID
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                //$scope.CurrentInvoice.PDF = $sce.trustAsHtml(response.data);
                $('#iframe_templateViewInvoice').attr("src", response.data);
                //$('#iframe_templateViewInvoice').attr("srcdoc", response.data);
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - QuickviewInvoice');
            }
            LoadingHide();
        });
    }
    //HĐ chuyển đổi
    $scope.QuickviewInvoiceConvert = function (invoice) {
        $rootScope.InvocieTYPE = 1;
        $scope.CurrentActive = invoice.ID;
        $scope.CurrentInvoice = invoice;
        var action = url + 'QuickviewInvoiceConvert';
        var datasend = JSON.stringify({
            invoice: invoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response && response.rs) {
                toastr.options = { "timeOut": "2000" };
                toastr.success("Chuyển đổi thành công!");
                //$scope.CurrentInvoice.PDF = $sce.trustAsHtml(response.data[0]);
                $('#iframe_templateViewInvoice').attr("src", response.data);
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - QuickviewInvoiceConvert');
            }
            LoadingHide();
        });
    }
     // End xem nhanh hóa đơn
    // gửi tin zalo
    $rootScope.NOTE = "";
    $rootScope.SendZalo = function (item) {
        if (!item.CUSPHONENUMBER) {
            toastr.warning('Số điện thoại là bắt buộc');
            return;
        }
        var action = url + 'SendZalo';
        var datasend = JSON.stringify({
            invoice: item ,note :$rootScope.NOTE
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success("Gửi thành công tới zalo có số " + item.CUSPHONENUMBER);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendZalo');
            }
            LoadingHide();
        });
    }

    $scope.ModalSendZalo = function (item) {
        $('#modal_send_zalo').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            show: {
                effect: 'drop',
                duration: 500
            },
            hide: {
                effect: 'drop',
                duration: 500
            },
            create: function (event, ui) {
                $('#modal_send_zalo').show();
            }
        });

        $('#file-selected').html('');

        $rootScope.Invoice = new Object();
        angular.copy(item, $rootScope.Invoice);

        var action = url + 'GetHistoryZalo';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $rootScope.ListHistoryZaloUser = response.result;
                } else {
                    toastr.warning(response.msg);
                    $rootScope.ListHistoryZaloUser = null;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetHistoryZalo');
            }
            LoadingHide();
        });
    }
    //format date zalo
    $rootScope.DateFormatZalo = function (dateString) {
        var currentTime = new Date(dateString);
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();
        var hour = currentTime.getHours();
        var minutes = currentTime.getMinutes();
        var seconds = currentTime.getSeconds();
        var date = day + "/" + month + "/" + year + " lúc " + hour + " : " + minutes + " : " + seconds;
        return date;
    }
   
    $rootScope.OpenSendMultipleZalo = function () {
        $scope.ListInvoiceChecked = [];
        var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
        if (listInvoiceChecked && listInvoiceChecked.length === 0) {
            toastr.warning("Bạn chưa chọn hóa đơn cần gửi tin.");
            return;
        }

        var listDeletedInvoice = $scope.ListInvoice.filter(function (obj) { return obj.INVOICETYPE !== 3 && obj.ISSELECTED === true; });
        if (listDeletedInvoice && listDeletedInvoice.length > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                listInvoiceChecked = listInvoiceChecked.filter(function (obj) { return obj.INVOICETYPE !== 3 });
                $rootScope.INVOICEQUANTITIES = listInvoiceChecked.length;
                $('#modal_send_multiple_zalo').dialog({
                    width: '1200px',
                    modal: true,
                    resizable: false,
                    show: {
                        effect: 'drop',
                        duration: 500
                    },
                    hide: {
                        effect: 'drop',
                        duration: 500
                    },
                    create: function (event, ui) {
                        $('#modal_send_multiple_zalo').show();
                    }
                });
                $timeout(function () {
                    listInvoiceChecked.forEach((item) => {
                        if (!item.CUSPHONENUMBER || item.CUSPHONENUMBER.trim() === "") {
                            item.CUSPHONENUMBER = item.CUSPHONENUMBER;
                        }
                    })
                    angular.copy(listInvoiceChecked, $rootScope.ListInvoiceChecked);
                }, 100)
            };
            confirm('Chương trình chỉ thực hiện gửi các hóa đơn đã phát hành và chưa bị xóa bỏ?', 'Lưu ý gửi tin', 'Không', 'Đồng ý', confirmContinue);
        }
    }
    $rootScope.SendMultipleZalo = function () {
        var action = url + 'SendMultipleZalo';
        var datasend = JSON.stringify({
            invoices: $rootScope.ListInvoiceChecked,
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('#modal_send_multiple_zalo').dialog('close');
                } else {
                    toastr.error(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendMultipleZalo');
            }
        });
    }
    //End gửi tin zalo
    $scope.SetdateFromtime = function () {
        moment($("#pk_fromtime").val()).format('yy-mm-dd');
    }
    $scope.SetDatepikerFromTime = function () {
        // All tab
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));

        // Quickview tab
        $("#pk_fromtime_quickview").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_quickview"));
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_quickview"));

        // Waiting tab
        $("#pk_fromtime_wating").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-wating"));
        $("#pk_fromtime_wating").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_wating"));

        //Delete tab
        $("#pk_fromtime_delete").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-delete"));
        $("#pk_fromtime_delete").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_delete"));
        //ReleaseError
        $("#pk_fromtime_error").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime-error"));
        $("#pk_fromtime_error").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime_error"));
    }

    $scope.SetDatepikerToTime = function () {

        // All tab
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));

        // Quickview tab
        var toTimeQuickview = $scope.TOTIME;
        $("#pk_totime_quickview").datepicker("option", 'minDate', toTimeQuickview);
        $("#pk_totime_quickview").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeQuickview)
        });
        SetVietNameInterface($("#pk_totime_quickview"));
        var toTimeQuickview = $scope.TOTIME;
        $("#pk_totime_quickview").datepicker("option", 'minDate', toTimeQuickview);
        $("#pk_totime_quickview").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeQuickview)
        });
        SetVietNameInterface($("#pk_totime_quickview"));

        // Waiting tab
        var toTimeWating = $scope.TOTIME;
        $("#pk_totime_wating").datepicker("option", 'minDate', toTimeWating);
        $("#pk_totime_wating").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeWating)
        });
        SetVietNameInterface($("#pk_totime_wating"));
        var toTimeWating = $scope.TOTIME;
        $("#pk_totime_wating").datepicker("option", 'minDate', toTimeWating);
        $("#pk_totime_wating").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeWating)
        });
        SetVietNameInterface($("#pk_totime_wating"));

        // Delete tab
        var toTimeDelete = $scope.TOTIME;
        $("#pk_totime_delete").datepicker("option", 'minDate', toTimeDelete);
        $("#pk_totime_delete").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeDelete)
        });
        SetVietNameInterface($("#pk_totime_delete"));
        var toTimeDelete = $scope.TOTIME;
        $("#pk_totime_delete").datepicker("option", 'minDate', toTimeDelete);
        $("#pk_totime_delete").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeDelete)
        });
        SetVietNameInterface($("#pk_totime_error"));
        //ReleaseError tab
        var toTimeError = $scope.TOTIME;
        $("#pk_totime_error").datepicker("option", 'minDate', toTimeError);
        $("#pk_totime_error").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTimeError)
        });
        SetVietNameInterface($("#pk_totime_error"));
        var toTimeError = $scope.TOTIME;
        $("#pk_totime_error").datepicker("option", 'minDate', toTimeError);
        $("#pk_totime_error").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTimeError)
        });
        SetVietNameInterface($("#pk_totime_delete"));
    }

    /*
     *truongnv 20200317
     * Sắp sếp dữ liệu
     * */
    // column to sort
    $scope.column = '';

    // sort ordering (Ascending or Descending). Set true for desending
    $scope.reverse = false;

    // called on header click
    $scope.sortColumn = function (col) {
        $scope.column = col;
        if ($scope.reverse) {
            $scope.reverse = false;
            $scope.reverseclass = 'arrow-up_h';
        } else {
            $scope.reverse = true;
            $scope.reverseclass = 'arrow-down_h';
        }
    }

    // remove and change class
    $scope.sortClass = function (col) {
        if ($scope.column == col) {
            if ($scope.reverse) {
                return 'arrow-down_h';
            } else {
                return 'arrow-up_h';
            }
        } else {
            return '';
        }
    }

    //UI/UX
    $timeout(function () {
        $('select.cb-select-time').selectpicker();
        $('.selectpicker').selectpicker();
    });

    $('[data-toggle="tooltip"]').tooltip({
        content: function () {
            return $(this).prop('title');
        },
        position: {
            my: "center top+15", at: "left bottom"
        }
    });

    //xóa hẳn HĐ
    $scope.RemovedInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListInvoice.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }
                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RemovedInvoice';
            var datasend = JSON.stringify({
                ids: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các hóa đơn đã chọn không ?<br/><b class ='text-danger'>Chọn xóa sẽ không thể khôi phục lại được, nên cân nhắc trước khi xóa</b>", "Thông báo", "Không", "Có", confirmContinue)
    }

    //xóa hẳn tất cả HĐ trong tab xóa bỏ
    $scope.RemovedAllInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            var action = url + 'RemovedAllInvoice';
            var datasend = JSON.stringify({
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadInvoice();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa tất cả các hóa đơn không?<br/><b class ='text-danger'>Chọn xóa sẽ không thể khôi phục lại được, nên cân nhắc trước khi xóa</b>", "Thông báo", "Không", "Có", confirmContinue)
    }

    //HĐ chuyển đổi
    $scope.ModalConvertInvoice = function (item) {
        $('#modal_convert_invoice').dialog({
            width: '20%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            show: {
                effect: 'drop',
                direction: 'right',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal_convert_invoice').show();
            }
        });
        angular.copy(item, $scope.Invoice);
    }
    $scope.ConvertInvoices = function () {
        var action = url + "InvoiceConvert";
        var datasend = JSON.stringify({
            invoice: $scope.Invoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Chuyển đổi thành công!");
                    $('#modal_convert_invoice').dialog("close");
                    $scope.PreviewReferenceInvoice(response.data);
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - InvoiceConvert');
            }
            LoadingHide();
        });
    }
    var n = 1;
    $window.GetInvoice = function () {
        if ($scope.cookie.RowNum >= $scope.TotalRow) {
            
        } else {
            $scope.cookie.RowNum = $scope.cookie.RowNum + 10;
            if ($scope.Tab.All === true) {
                $scope.Check($scope.cookie.RowNum, 'RowNum');
            }
            if ($scope.Tab.Wating === true) {
                $scope.Check($scope.cookie.RowNum, 'RowNum');
            }
            if ($scope.Tab.Cancel === true) {
                $scope.GetInvoiceByStatus(3, 1 + n, 1);
            }
            if ($scope.Tab.Change === true) {
                $scope.GetInvoiceByStatus(5, 1 + n, 2);
            }
            if ($scope.Tab.Delete === true) {
                $scope.GetInvoiceDelete(1 + n);
            }
            if ($scope.Tab.Replace === true) {
                $scope.GetInvoiceByStatus(6, 1 + n, 4);
            }
        }
    }
    $scope.ChangeFontSize = function () {
        $scope.template.style["body, table",".bg"]["font-size"] = $scope.template.fontSize;
    }

    $scope.ShowPopupFileView = function (link) {
        $('#popupViewInvoice').dialog({
            modal: true,
            resizable: true,
            width: '900px',

            open: function (data, e, f) {
                $('#frViewFileInvoice').attr('src', link);
                $(this).dialog("option", "title", "");
                //thêm nút thực hiện chức năng chuyển đổi hóa đơn điện tử sang hóa đơn giấy
                $(".function-area").remove();
                var rdInvoice = $('<label class="function-area"><input id = "btnInvoice" checked type="radio" onchange="window.PreviewInvoiceTemplate(' + (false) +')" name="template_check" style="transform: scale(1.7)" /> &nbsp;&nbsp; Hóa đơn mẫu<label/>');
                rdInvoice.appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));
                var rdConvertInvoice = $('<label class="function-area"><input id = "btnConvertInvoice" name="template_check" type="radio" onchange="window.PreviewInvoiceTemplate(' + (true) +')" style="transform: scale(1.7)" />&nbsp;&nbsp; Hóa đơn mẫu chuyển đổi<label/>');
                rdConvertInvoice.appendTo($('[aria-describedby="popupViewInvoice"] .ui-dialog-titlebar'));
            }
        });
    }

}]);
//bắt sự kiện Tự động load trang
var x = 200;
    function LoadPage() {
    var elmnt = document.getElementById("all_invoice");
    var y = elmnt.scrollTop;
    var h = $("#all_invoice").height();
    var h1 = $(document).height();
        var h2 = $("#nofify-footer-bar").height();
        if (y + h >= h1 - h2 + x) {
            x += 200;
            window.GetInvoice();
        }
}
/**
 * Xóa đính kèm đã chọn
 * truongnv 20200219
 * @param {any} index : key
 */
function DeleteFile(index) {
    index.remove();
}

/**
 * Kiểm tra kiểu file và dung lượng file tải lên
 * truongnv 20200219
 * @param {any} files
 */
function CheckFile(files) {
    var arrFiles = new Array();
    var allowFileExtension = ".xls,.xlsx,.doc,.docx,.pdf, .txt, .xml";
    var arrAllowFileExtension = allowFileExtension.split(",");
    if (files.length > 0) {
        var sumSize = 0;
        var sumSizeAllow = 10 * 1024 * 1024;
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
                alert("File <b>" + files[i].name + "</b> có dung lượng <b>" + GetSizeName(fileSize) + "</b>. Bạn không được chọn file có dung lượng lớn hơn <b>10 MB</b>.");
                return false;
            }

            sumSize = sumSize + fileSize;
        }

        for (var i = 0; i < arrFiles.length; i++)
            sumSize = sumSize + arrFiles[i].size;

        if (sumSize > sumSizeAllow) {
            alert("Tổng dung lượng các file bạn đã chọn là <b>" + GetSizeName(sumSize) + "</b>. Bạn không được Attach file có tổng dung lượng lớn hơn <b>10 MB</b>.");
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
    //LoadingShow();
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

            //LoadingHide();
        },
        error: function (xhr, textStatus, errorThrown) {
            //LoadingHide();
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
 * */
function splitArr(arr, i) {
    return arr.slice(i * 50, (i + 1) * 50);
}

//xem thêm,thu gọn 
function See_more_invoice() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_invoice() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}
function See_more_invoice_wating() {
    $('#id3').css('display', 'none');
    $('#id4').css('display', 'block');
}
function Collapse_invoice_wating() {
    $('#id4').css('display', 'none');
    $('#id3').css('display', 'block');
}
function ChangeSizeIframe() {
    frames.$('body').style.fontSize = '11px';
}


app.controller('ProductController', ['$rootScope', '$scope', '$timeout', 'CommonFactory', function ($rootScope, $scope, $timeout, CommonFactory) {
    var url = '/Product/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Product = function () {
        var check = getCookie("Novaon_ProductManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldName: true,
                FieldSKU: true,
                FieldType: true,
                FieldCategory: true,
                FieldDes: true,
                FieldQuantityUnit: true,
                FieldPrice: true,
                RowNum: 10
            }
            setCookie("Novaon_ProductManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_ProductManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetProduct($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }

    $rootScope.ReloadProduct = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetProduct($scope.currentpage);
    }

    $scope.ResetGetFunction = function () {
        $scope.LoadCookie_Product();
        $scope.ListProduct = [];
    }

    $scope.GetProduct = function (intpage) {
        $scope.ResetGetFunction();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetProduct';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListProduct = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            
            if (response) {
                if (response.rs) {
                    $scope.ListProduct = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ProductController - GetProduct');
            }
            LoadingHide();
        });
    }

    $scope.UpdateType = function () {
        if (!$scope.ChangeType)
            return;

        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (find.length == 0) {
            alert('Vui lòng chọn ít nhất 1 dòng');
            $scope.ChangeType = null
            return;
        }

        var result = confirm('Chuyển sang loại ' + $scope.ChangeType);

        if (result) {
            var action = url + 'ChangeProductType';
            var datasend = JSON.stringify({
                products: $scope.ListProduct,
                productType: $scope.ChangeType
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        $rootScope.ReloadProduct();
                    } else {
                        $scope.ChangeType = null
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ProductController - UpdateType');
                }
            });
            LoadingHide();
        } else {
            $scope.ChangeType = null
        }

    }

    $scope.ExportExcel = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            productType: $scope.Filter.PRODUCTTYPE,
            category: $scope.Filter.CATEGORY,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }



    $scope.SelectAll = function () {
        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListProduct.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListProduct.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });
    $scope.RemoveProduct = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListProductChecked = [];
            $scope.ListProductCheckedName = [];
            $scope.ListProductCheckedSku = [];
            if (item) {
                $scope.ListProductChecked.push(item.ID);
                $scope.ListProductCheckedName.push(item.PRODUCTNAME);
                $scope.ListProductCheckedSku.push(item.SKU);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listProductChecked = $scope.ListProduct.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listProductChecked && listProductChecked.length === 0) {
                    alert("Bạn chưa chọn sản phẩm cần xóa.");
                    return;
                }
                if (listProductChecked && Object.keys(listProductChecked).length > 0) {
                    for (var i = 0; i < listProductChecked.length; i++) {
                        $scope.ListProductChecked.push(listProductChecked[i].ID);
                        $scope.ListProductCheckedName.push(listProductChecked[i].PRODUCTNAME);
                        $scope.ListProductCheckedSku.push(listProductChecked[i].SKU);
                    }
                }
            }

            var lstProductid = $scope.ListProductChecked.join(";");
            var lstProductname = $scope.ListProductCheckedName.join(";");
            var lstProductsku = $scope.ListProductCheckedSku.join(";");
            var action = url + 'RemoveProduct';
            var datasend = JSON.stringify({
                idProducts: lstProductid, proname: lstProductname, sku: lstProductsku
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadProduct();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các sản phẩm đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_Pro() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_Pro() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}
app.controller('CustomerController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Customer/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Customer = function () {
        var check = getCookie("Novaon_CustomerManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldIDCustummer: true,
                FieldName: true,
                FieldEnterprise: true,
                FieldPhone: true,
                FieldEmail: true,
                FieldAddress: true,
                FieldStatus: true,
                FieldTotalMoney: true,
                FieldUnpaidMoney: true,
                FieldCustaxcode: true,
                RowNum: 10
            }
            setCookie("Novaon_CustomerManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {        
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CustomerManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCustomer($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '' }

    $rootScope.ReloadCustomer = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetCustomer($scope.currentpage);
    }

    $scope.ResetGetCustomer = function () {
        $scope.LoadCookie_Customer();
        $scope.ListCustomer = [];
    }

    $scope.GetCustomer = function (intpage) {        
        $scope.ResetGetCustomer();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetCustomer';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    $scope.ListCustomer = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.SelectAll = function () {
        var find = $scope.ListCustomer.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {

        var find = $scope.ListCustomer.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCustomer.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    $scope.EditCustomer = function (item) {
        $scope.CurrentCustomer = new Object();
        angular.copy(item, $scope.CurrentCustomer);
    }

    //$scope.EditCustomer = function (item) {
    //    $scope.CurrentCustomer = new Object();
    //    angular.copy(item, $scope.CurrentCustomer);

    //    //$scope.IsLoading = true;
    //    //var action = url + 'GetListInvoiceCustomer';
    //    //var datasend = JSON.stringify({
    //    //    keyword: $scope.Filter.KEYWORD,
    //    //    currentPage: 1
    //    //});
    //    //CommonFactory.PostDataAjax(action, datasend, function (response) {
    //    //    if (response) {
    //    //        if (response.rs) {
    //    //            $scope.ListCustomer = response.result;
    //    //        } else {
    //    //            $scope.ErrorMessage = response.msg;
    //    //        }
    //    //    } else {
    //    //        alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
    //    //    }
    //    //});
    //    //$scope.IsLoading = false;


    //    var action = url + 'GetInvoice';
    //    var datasend = JSON.stringify({
    //        customer: $scope.CurrentCustomer,
    //        currentPage: $scope.currentpage
    //    });
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $scope.ListInvoiceCustomer = response.result;
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
    //        }
    //    });
    //}
    $scope.DeletedCustomer = function (item) {
        item.ISDELETED = true;
        $rootScope.ModalCustomer(item, false);
        $('.modal-customer').modal('show');
    }

    $scope.ExportExcel = function () {
        $scope.IsLoading = true;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            currentPage: 1
        };
        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });

        LoadingHide();
    }

    $scope.DeactiveCustomer = function () {
        var action = url + 'DeactiveCustomer';
        var datasend = JSON.stringify({
            customers: $scope.ListCustomer
        }); LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                } else {
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeactiveCustomer');
            }
            LoadingHide();
        });
    }

    $scope.GetInvoiceByCustomer = function (intpage) {

        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        $scope.IsInfo = false;
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            customer: $scope.CurrentCustomer,
            currentPage: $scope.currentpage
        });
        $scope.ListInvoiceCustomer = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoiceCustomer = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }

    $scope.SendEmail = function () {
        var emailList = [];
        $scope.ListCustomer.forEach(function (obj) {
            if (obj.ISSELECTED == true) {
                if (obj.CUSEMAIL)
                    emailList.push(obj.CUSEMAIL);
            }
        });
        if (emailList.length == 0)
            return false;
        window.open('mailto:' + emailList.join(';'), '_blank');
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });
    $scope.RemoveCustomer = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListCustomerChecked = [];
            $scope.ListCustomerCheckCode = [];
            if (item) {
                $scope.ListCustomerChecked.push(item.ID);
                $scope.ListCustomerCheckCode.push(item.CUSID);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCustomerChecked = $scope.ListCustomer.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCustomerChecked && listCustomerChecked.length === 0) {
                    alert("Bạn chưa chọn khách hàng cần xóa.");
                    return;
                }
                if (listCustomerChecked && Object.keys(listCustomerChecked).length > 0) {
                    for (var i = 0; i < listCustomerChecked.length; i++) {
                        $scope.ListCustomerChecked.push(listCustomerChecked[i].ID);
                        $scope.ListCustomerCheckCode.push(listCustomerChecked[i].CUSID);
                    }
                }
            }

            var lstCustomerid = $scope.ListCustomerChecked.join(";");
            var lstCustomercode = $scope.ListCustomerCheckCode.join(";");
            var action = url + 'RemoveCustomer';
            var datasend = JSON.stringify({
                idCustomers: lstCustomerid, customer: item, codeCus: lstCustomercode
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCustomer();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các khách hàng đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_Cus() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_Cus() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}

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

app.controller('CategoryController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Category/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Category = function () {
        var check = getCookie("Novaon_CategoryManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCategoryName: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_CategoryManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CategoryManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCategory($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadCategory = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetCategory($scope.currentpage);
    }

    $scope.ResetGetCategory = function () {
        $scope.LoadCookie_Category();
        $scope.ListCategory = [];
    }
    //lấy ra danh sách danh mục
    $scope.GetCategory = function (intpage) {
        $scope.ResetGetCategory();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        
        var action = url + 'GetCategory';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCategory = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCategory');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListCategory.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListCategory.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCategory.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    //xóa dịch vụ
    $scope.RemoveCategory = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListCategoryChecked = [];
            $scope.ListCategoryActive = [];
            if (item) {
                if (item.ISACTIVE == true) {
                    toastr.warning('Không được phép xóa khi đang ở trạng thái hoạt động');
                    return false;
                } else {
                    $scope.ListCategoryChecked.push(item.ID);
                    $scope.count = 1;

                }
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCategoryChecked = $scope.ListCategory.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCategoryChecked && listCategoryChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn dịch vụ cần xóa.");
                    return;
                }
                if (listCategoryChecked && Object.keys(listCategoryChecked).length > 0) {
                    for (var i = 0; i < listCategoryChecked.length; i++) {
                        $scope.ListCategoryChecked.push(listCategoryChecked[i].ID);
                        $scope.ListCategoryActive.push(listCategoryChecked[i].ISACTIVE);
                    }
                    for (var i = 0; i < $scope.ListCategoryActive.length; i++) {
                        if ($scope.ListCategoryActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstCategoryid = $scope.ListCategoryChecked.join(";");
            var action = url + 'RemoveCategory';
            var datasend = JSON.stringify({
                id: lstCategoryid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCategory();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các dịch vụ đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
    $scope.ExportExcel = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
}]);
app.controller('StatisticController', ['$scope', '$timeout', 'CommonFactory', '$rootScope', function ($scope, $timeout, CommonFactory, $rootScope) {
    var url = '/Statistic/';

    angular.element(function () {
        LoadingHide();
    });

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;

    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $scope.GetQuarter = function (month) {
        if (month >= 1 && month <= 3) {
            $scope.ReportInvoice.FROMDATE = '01/01/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '31/03/' + $scope.ReportInvoice.YEAR;
            return 1;
        }
        if (month >= 4 && month <= 6) {
            $scope.ReportInvoice.FROMDATE = '01/04/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '30/06/' + $scope.ReportInvoice.YEAR;
            return 2;
        }
        if (month >= 7 && month <= 9) {
            $scope.ReportInvoice.FROMDATE = '01/07/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '30/09/' + $scope.ReportInvoice.YEAR;
            return 3;
        }
        if (month >= 10 && month <= 12) {
            $scope.ReportInvoice.FROMDATE = '01/10/' + $scope.ReportInvoice.YEAR;
            $scope.ReportInvoice.TODATE = '31/12/' + $scope.ReportInvoice.YEAR;
            return 4;
        }
    }

    $scope.InitReportInvoice = function () {
        $scope.ReportInvoice = new Object();
        $scope.ReportInvoice.YEAR = $scope.CurrentYear;
        $scope.ReportInvoice.MONTHQUARTER = "Q" + $scope.GetQuarter($scope.CurrentMonth);
        //$scope.ReportInvoice.MONTHQUARTER = $scope.GetQuarter($scope.CurrentMonth);

        //var date = new Date();
        //var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
        //var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);
        //if (ReportInvoice.MONTHQUARTER.includes('M')) {
        //    $scope.GetQuarter(ReportInvoice.MONTHQUARTER.substring(1, ReportInvoice.MONTHQUARTER.length - 1));
        //}
        //$scope.ReportInvoice.FROMDATE = ("0" + firstDay.getDate()).slice(-2) + "/" + ("0" + firstDay.getMonth()).slice(-2) + "/" + firstDay.getFullYear();
        //$scope.ReportInvoice.TODATE = ("0" + lastDay.getDate()).slice(-2) + "/" + ("0" + lastDay.getMonth()).slice(-2) + "/" + lastDay.getFullYear();
        //$scope.ReportInvoice.FIRSTDAY = ("0" + firstDay.getDate()).slice(-2) + "/" + ("0" + firstDay.getMonth()).slice(-2) + "/" + firstDay.getFullYear();
        //$scope.ReportInvoice.LASTDAY = ("0" + lastDay.getDate()).slice(-2) + "/" + ("0" + lastDay.getMonth()).slice(-2) + "/" + lastDay.getFullYear();
    }
    $scope.InitReportInvoice();

    $scope.GetOutputInvoice = function () {

        var action = url + 'GetOutputInvoice';
        var datasend = JSON.stringify({
            reportUsingInvoice: $scope.ReportInvoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let data = response.result[0];
                    $scope.OutputInvoiceData = data;
                    $scope.TOTALREVENUEPRETAX = parseFloat(parseFloat(data.SUMREVENUENOTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX) + parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUETENPERCENTTAX))
                    $scope.TOTALREVENUEUNDERTAX = parseFloat(parseFloat(data.SUMREVENUETENPERCENTTAX) + parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX))
                    $scope.TOTALTAXMONEY = parseFloat(parseFloat((0.1) * data.SUMREVENUETENPERCENTTAX) + parseFloat((0.05) * data.SUMREVENUEFIVEPERCENTTAX))
                    $scope.ReportTime = response.reportTime;
                    if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                        $scope.TOTALREVENUEUNDERTAX = parseFloat(parseFloat(data.SUMREVENUEFIVEPERCENTTAX) + parseFloat(data.SUMREVENUETHREEPERCENTTAX) + parseFloat(data.SUMREVENUETWOPERCENTTAX) + parseFloat(data.SUMREVENUEONEPERCENTTAX) + parseFloat(data.SUMREVENUEZEROPERCENTTAX));
                        $scope.TOTALTAXMONEY = parseFloat(parseFloat((0.01) * data.SUMREVENUEONEPERCENTTAX) + parseFloat((0.02) * data.SUMREVENUETWOPERCENTTAX) + parseFloat((0.03) * data.SUMREVENUETHREEPERCENTTAX)+ parseFloat((0.05) * data.SUMREVENUEFIVEPERCENTTAX))
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }


    $scope.GetUsingInvoiceStatus = function () {
        //$scope.ReportInvoice.YEAR = $scope.CurrentYear;
        //$scope.ReportInvoice.MONTHQUARTER = $scope.GetQuarter($scope.CurrentMonth);

        var action = url + 'GetUsingInvoice';
        var datasend = JSON.stringify({
            reportUsingInvoice: $scope.ReportInvoice
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.UsingInvoiceList = response.listUsingInvoiceThisTerm;
                    $scope.ReportTime = response.reportTime;
                } else {
                    $scope.ErrorMessage = response.msg;
                    $scope.UsingInvoiceList = response.listUsingInvoiceThisTerm;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
    }
    //
    $scope.GetEmailHistory = function (intpage) {
        $scope.FROMTIME = $('#pk_fromtime').val();
        $scope.TOTIME = $('#pk_totime').val();
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.FROMTIME = moment($scope.FROMTIME).format('YYYY-MM-DD');
            $scope.TOTIME = moment($scope.TOTIME).format('YYYY-MM-DD');
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentRow = 10;
        $scope.currentpage = intpage;
        $scope.ListEmailHistory = new Array();
        var action = url + 'GetEmailHistoryByComtaxcode';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            itemPerPage: $scope.currentRow,
            currentPage: $scope.currentpage
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $timeout(function () {
                        $scope.ListEmailHistory = response.result;
                        $scope.TotalPages = response.TotalPages;
                        $scope.TotalRow = response.TotalRow;
                    }, 200)
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCustomer');
            }
            LoadingHide();
        });
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'MAILTO') {
                    var f = $scope.ListInvoiceStatus.filter(function (item) {
                        return item.MAILTO == $scope.Filter[prop];
                    });
                    o.value = f[0].MAILTO;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    o.value = f[0].TIME;
                }

                if (prop == 'STATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.STATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].STATUS;
                }

                if (prop == 'MAILTYPE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.MAILTYPE == $scope.Filter[prop];
                    });
                    o.value = f[0].MAILTYPE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        //Refresh filter
        $scope.FilterInvoice();
    };
  
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    //Init time
    var now = new Date();
    var firstDay = new Date();
    var lastDay = new Date();
    var currentDay = now.getDay();

    // Sunday - Saturday : 0 - 6
    //This week
    firstDay.setDate(now.getDate() - currentDay);
    lastDay.setDate(firstDay.getDate() + 6);
    var thisWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last week
    firstDay.setDate(firstDay.getDate() - 7);
    lastDay.setDate(lastDay.getDate() - 7);
    var lastWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //This month
    var dayOfMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    lastDay = new Date(now.getFullYear(), now.getMonth(), dayOfMonth);
    var thisMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last month
    lastDay.setDate(firstDay.getDate() - 1);
    firstDay = new Date(lastDay.getFullYear(), lastDay.getMonth(), 1);
    var lastMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //
    $scope.Timepickers = {
        value: thisWeek,    //Default value
        Options: [
            { id: 1,value: thisWeek, text: 'Tuần này' },
            { id: 2,value: lastWeek, text: 'Tuần trước' },
            { id: 3,value: thisMonth, text: 'Tháng này' },
            { id: 4,value: lastMonth, text: 'Tháng trước' },
            { id: 5,value: '5', text: 'Tùy chọn' }
        ]
    }; 
    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
        
        $("#pk_fromtime").datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
    }

    $scope.SetDatepikerToTime = function () {
        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));

        var toTime = $scope.TOTIME;
        $("#pk_totime").datepicker("option", 'minDate', toTime);
        $("#pk_totime").datepicker({
            dateFormat: 'dd/mm/yy',
            minDate: new Date(toTime)
        });
        SetVietNameInterface($("#pk_totime"));
    }
    //End init time

    $scope.LISTMAILTYPE = [
        {
            value: 'Phat-hanh', name: "Thông báo phát hành"
        },
        {
            value: 'Huy', name: "Thông báo hủy"
        }
    ];

    $scope.LISTMAILSTATUS = [
        {
            value: 1, name: "Đã gửi"
        },
        {
            value: 2, name: "Đã xem"
        },
        {
            value: 3, name: "Gửi lỗi"
        }
    ];

}]);
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

app.controller('ModalInvoiceController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$http', '$location', 'permissions', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $http, $location, permissions) {
    var url = '/Invoice/';
    //type: 1: tạo mới, 2: xem chi tiết, 3: hủy bỏ, 4: chuyển đổi, 5: điều chỉnh, 6: thay thế, 7: dải chờ

    var msgFromDateToDatePaymentTerm = "Từ ngày của kỳ thanh toán không được lớn hơn đến ngày."; // Hóa đơn điện, nước
    var msgImportDateExportDate = "Ngày nhập kho không nhỏ hơn ngày xuất kho"; // Phiếu xuất kho

    $rootScope.ModalInvoice = function (item, type, formcode, symbolcode, isCopy) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetPaymentMethod();
        //$rootScope.GetQuantityUnit();

        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = [];
        $scope.Invoice.INVOICETYPE = type;
        $scope.IsCopy = false;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
        $scope.Invoice.CURRENCY = "VND";
        $scope.Invoice.EXCHANGERATE = 1;
        $scope.GLOBALTAXRATEWATER = 0;
        $scope.Invoice.SERVICEFEETAXRATE = $scope.TaxRateList[0].value;

        $scope.Invoice.CUSTOMFIELDEXCHANGERATE = 0;
        $scope.Invoice.CUSTOMFIELDEXCHANGE = 0;

        $scope.LoadCurrencyExchangeRate();
        $scope.TempListFormCode = $rootScope.ListFormCode;

        $('.modal-invoice').modal('show');
        if (type == 1) {
            $scope.Invoice.ID = 0;
            $scope.Invoice.CUSPAYMENTMETHOD = "TM/CK";
            $scope.Invoice.FORMCODE = $rootScope.ListFormCode[0].FORMCODE + '-' + $rootScope.ListFormCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = $rootScope.ListFormCode[0].SYMBOLCODE;
            $scope.TAXRATE = $rootScope.ListFormCode[0].TAXRATE;
            $scope.Invoice.DISCOUNTTYPE = "KHONG_CO_CHIET_KHAU";
            if (item) {
                angular.copy(item, $scope.Invoice);
                if (isCopy) {
                    var formCode = $rootScope.ListFormCode.filter(function (x) {
                        return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
                    })
                    $scope.TAXRATE = formCode[0].TAXRATE;
                    $scope.Invoice.FORMCODE = item.FORMCODE + '-' + item.SYMBOLCODE;
                    $scope.Invoice.INVOICESTATUS = 1;
                    $scope.Invoice.NUMBER = 0;
                    $scope.Invoice.ID = 0;
                    $scope.Invoice.INVOICETYPE = type;
                    $scope.Invoice.REFERENCE = 0;
                    $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $scope.IsCopy = isCopy;
                    if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                        $scope.GetMeterByInvoiceId(item.ID);
                    }
                    $scope.GetInvoiceDetail(item.ID)
                }
                else {
                    $scope.MeterList = null;
                }
            }
            else {
                $scope.Invoice.NUMBER = 0;
                if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONGTGT || $rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG || $rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
                    $timeout(function () {
                        $scope.AddRow();
                    }, 100);
                }
            }
            //Reset danh sách công tơ trong danh sách sản phẩm đối với hóa đơn điện
            $scope.MeterList = null;
            $scope.Invoice.METER = {
                METERNAME: "[Số công tơ]",
                CODE: "[Số công tơ]"
            };

            // check waiting invoice
            if (formcode && symbolcode) {
                $scope.Invoice.FORMCODE = formcode;
                $scope.Invoice.SYMBOLCODE = symbolcode;
                $scope.Invoice.NUMBER = 0;
                $scope.Invoice.INVOICESTATUS = 1;
                $scope.Invoice.INVOICETYPE = 1;
                $scope.Invoice.ISINVOICEWAITING = true;
            }
        } else if (type == 2) {
            item.EXCHANGERATE = item.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            if (item.INVOICETYPE == 5 || item.INVOICETYPE == 3 || item.INVOICETYPE == 6) {
                $scope.GetInvoiceById(item.REFERENCE);
            }
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByInvoiceId($scope.Invoice.ID);
            }

            $scope.GetInvoiceDetail(item.ID);
            setTimeout(function () {
                if ($scope.RefInvoice) {
                    $scope.Invoice.NUMBERTEMP = $scope.RefInvoice.NUMBER;
                    $scope.Invoice.SIGNEDTIME = $scope.RefInvoice.SIGNEDTIME;
                }
            }, 2000)
        } else if (type == 5) {
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByCustaxcode($scope.Invoice.CUSTAXCODE);
            }

            $scope.Invoice.LISTPRODUCT = [];
            //$timeout(function () {
            //    $scope.Invoice.LISTPRODUCT.push({ QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: -1, RETAILPRICE: 0 });
            //}, 100);
            $scope.GetInvoiceDetail(item.ID);

        } else if (type == 6) {
            angular.copy(item, $scope.Invoice);

            var formCode = $rootScope.ListFormCode.filter(function (x) {
                return x.SYMBOLCODE === item.SYMBOLCODE && x.FORMCODE === item.FORMCODE;
            })

            $scope.Invoice.FORMCODE = formCode[0].FORMCODE + '-' + formCode[0].SYMBOLCODE;
            $scope.Invoice.SYMBOLCODE = formCode[0].SYMBOLCODE;
            $scope.TAXRATE = formCode[0].TAXRATE;

            if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
                $scope.GetMeterByCustaxcode($scope.Invoice.CUSTAXCODE);
            }

            $scope.GetInvoiceDetail(item.ID)
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;
        }

        $scope.Invoice.STRDUEDATE = $filter('dateFormat')($scope.Invoice.DUEDATE, 'dd/MM/yyyy');
        $scope.Invoice.STRINVOICEWAITINGTIME = $filter('dateFormat')($scope.Invoice.INVOICEWAITINGTIME, 'dd/MM/yyyy');

        $scope.Invoice.FROMDATE = $filter('dateFormat')($scope.Invoice.FROMDATE, 'dd/MM/yyyy');
        $scope.Invoice.TODATE = $filter('dateFormat')($scope.Invoice.TODATE, 'dd/MM/yyyy');
        $scope.Invoice.DELIVERYORDERDATE = $filter('dateFormat')($scope.Invoice.DELIVERYORDERDATE, 'dd/MM/yyyy');
        $scope.Invoice.FROMDATESTR = $scope.Invoice.FROMDATE;
        $scope.Invoice.TODATESTR = $scope.Invoice.TODATE;
        $scope.Invoice.DELIVERYORDERDATESTR = $scope.Invoice.DELIVERYORDERDATE;
        $(".datepicker").datepicker({
            buttonText: "Chọn ngày tháng",
            dateFormat: "dd/mm/yy"
        });
    }

    $scope.LoadCookie_Invoice_CustomField = function () {
        var check = getCookie('Novaon_Invoice_CustomField');
        if (check) {
            $scope.cookie_customField = JSON.parse(check);
        }
        else {
            $scope.cookie_customField = {
                FieldConsignment: false,
                FieldProductService: true,
                FieldUnit: true,
                FieldQuantity: true,
                FieldPrice: true,
                FieldMoney: true,
                FieldExchangeRateAdd: false,
                FieldExchangeAdd: false,
                FieldOtherTaxFee: false,
                FieldRefundFee: false,
                FieldServiceFee: false
            }
            setCookie('Novaon_Invoice_CustomField', JSON.stringify($scope.cookie_customField), 30);
        }
    }

    $scope.Check = function (status, field) {
        $scope.cookie_customField[field] = !status;
        setCookie('Novaon_Invoice_CustomField', JSON.stringify($scope.cookie_customField), 30);
    }

    $scope.GetInvoiceDetail = function (invoiceid) {
        var action = url + 'GetInvoiceDetail';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (data) {
            if (data) {
                if (data.rs) {
                    $scope.Invoice.LISTPRODUCT = data.result;
                    if ($scope.Invoice.LISTPRODUCT.length == 0) {
                        $timeout(function () {
                            $scope.AddRow();
                        });
                    }
                    if ($scope.TAXRATE === 1) {
                        if ($scope.Invoice.LISTPRODUCT.length > 0) {
                            $scope.ONLYTAXRATE = $scope.Invoice.LISTPRODUCT[0].TAXRATE;
                            $scope.GLOBALTAXRATE = $scope.Invoice.LISTPRODUCT[0].TAXRATE;
                        }
                    }

                    $timeout(function () {
                        if ($scope.Invoice.LISTPRODUCT && Object.keys($scope.Invoice.LISTPRODUCT).length > 0) {
                            $scope.GLOBALTAXRATEWATER = $scope.Invoice.LISTPRODUCT[0].TAXRATEWATER;//Lấy ra thông tin phí bảo vệ môi trường đối với nước
                            setSelectedValueDropdown($rootScope.Enterprise.USINGINVOICETYPE, $scope.Invoice.LISTPRODUCT[0].METERCODE);
                        }
                    }, 100);
                } else {
                    $scope.ErrorMessage = data.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceDetail');
            }
            LoadingHide();
        });
    }

    $scope.AddInvoice = function (type, isSaveAndRelease) {
        var oReturn = {
            messErrorForCustomer: '',
            messErrorForCoder: '',
            result: true
        };
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            return false;
        }

        var formCodeSymbodeCode = $scope.Invoice.FORMCODE.split('-');
        $scope.Invoice.FORMCODE = formCodeSymbodeCode[0];
        $scope.Invoice.SYMBOLCODE = formCodeSymbodeCode[1];

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn ký hiệu hóa đơn');
            return false;
        }
        //Nếu là hóa đơn tiền điện thì kiểm tra thêm từ ngày, đến ngày của kỳ thanh toán
        if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Kỳ thanh toán từ ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.TODATE, "Kỳ thanh toán đến ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }

            oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgFromDateToDatePaymentTerm);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        //Nếu là phiếu xuất kho thì kiểm tra thêm ngày điều động, ngày xuất, ngày nhập
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            if (!$scope.Invoice.CUSNAME) {
                alert('Của: không được để trống');
                return false;
            }

            oReturn = CheckDate($scope.Invoice.DELIVERYORDERDATE, "Ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Ngày xuất kho", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        if (!$scope.Invoice.CUSNAME) {
            if (!$scope.Invoice.CUSBUYER) {
                alert('Bạn phải nhập vào thông tin <strong>Tên đơn vị</strong> hoặc thông tin <b>Người mua hàng</b>');
                return false;
            }
        }
        if (!$scope.Invoice.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }

        //if ($scope.Invoice.CUSTAXCODE && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    var res = $scope.Invoice.CUSTAXCODE.split('-');
        //    if (res.length == 1) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    } else if (res.length == 2) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //        if (res[1].length != 3 || !validation.isNumber(res[1])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    }
        //}

        if ($scope.Invoice.INVOICETYPE === 5 && $scope.Invoice.INVOICEMETHOD === 0) {
            alert('Vui chọn loại hóa đơn điều chỉnh.');
            return false;
        }

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email.');
                return;
            }
        }

        if (!$scope.Invoice.CHANGEREASON && $scope.Invoice.INVOICETYPE == 5) {
            alert('Vui lòng nhập lý do điều chỉnh.');
            return false;
        }

        if ($scope.Invoice.INVOICETYPE != 5) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                if (!obj.PRODUCTNAME && $scope.Invoice.INVOICETYPE != 5) {
                    alert('Tên sản phẩm không được để trống, vui lòng kiểm tra lại dữ liệu dòng thứ ' + (index + 1));
                }
                //obj.QUANTITY = obj.QUANTITY.toString().replace(/\./g, ",");
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                obj.QUANTITY = parseFloat(obj.QUANTITY.toString());
            });

            var find = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                return !s.PRODUCTNAME;
            });

            if (find.length > 0 && $scope.Invoice.INVOICETYPE != 5) {
                return false;
            }
        }

        // for only tax rate
        if ($scope.TAXRATE == 1) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                obj.TAXRATE = $scope.GLOBALTAXRATE;
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                // hóa đơn bán hàng không có thuế. Gán mặc định = -1
                //if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                //    obj.TAXRATE = -1;
                //}
            });
        }

        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '').replace(/\./g, '');
        //$scope.Invoice.CUSTOMFIELDEXCHANGE = $scope.Invoice.CUSTOMFIELDEXCHANGE.replace(/\,/g, '');
        //Tính lại giá tiền
        $scope.CalMoneyAfterChangeValue();

        // Map lại thông tin khi số lượng, đơn giá không điền. Để lưu thông tin vào database
        $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
            obj.TOTALMONEY = obj.ITEMTOTALMONEY;
        });
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENTRAW;
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEYRAW;
        $scope.Invoice.TAXMONEY = $scope.Invoice.TAXMONEYRAW;
        LoadingShow();
        var action = url + 'AddInvoice';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (isSaveAndRelease) {
                        var invoiceId = response.result;
                        Sign(invoiceId).then(function () {
                            toastr.success("Thêm mới thành công!", "Thành công", { timeOut: 4000 })
                            $('.modal-invoice').modal('hide');
                            if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                                $rootScope.ReloadReceipt();
                            }
                            else if ($location.path().toString().includes('/quan-ly-hoa-don')) {
                                $rootScope.ReloadInvoice();
                            }
                        });
                    }
                    else {
                        toastr.success("Thêm mới thành công!", "Thành công", { timeOut: 4000 })
                        $('.modal-invoice').modal('hide');
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ReloadReceipt();
                        }
                        else if ($location.path().toString().includes('/quan-ly-hoa-don')) {
                            $rootScope.ReloadInvoice();
                        }
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.UpdateInvoice = function (type, isSaveAndRelease) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }

        var formCodeSymbodeCode = $scope.Invoice.FORMCODE.split('-');
        $scope.Invoice.FORMCODE = formCodeSymbodeCode[0];
        $scope.Invoice.SYMBOLCODE = formCodeSymbodeCode[1];

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#symbol-code').focus();
            return false;
        }

        //Nếu là hóa đơn tiền điện thì kiểm tra thêm từ ngày, đến ngày của kỳ thanh toán
        if ($rootScope.Enterprise.USINGINVOICETYPE === 2 || $rootScope.Enterprise.USINGINVOICETYPE === 4) {
            var oReturn = {
                messErrorForCustomer: '',
                messErrorForCoder: '',
                result: true
            };
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Kỳ thanh toán từ ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.TODATE, "Kỳ thanh toán đến ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }

            oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgFromDateToDatePaymentTerm);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
        }

        //Nếu là phiếu xuất kho thì kiểm tra thêm ngày điều động, ngày xuất, ngày nhập
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            oReturn = CheckDate($scope.Invoice.DELIVERYORDERDATE, "Ngày", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            oReturn = CheckDate($scope.Invoice.FROMDATE, "Ngày xuất kho", 10, true);
            if (oReturn.result === false) {
                alert(oReturn.messErrorForCustomer);
                return false;
            }
            //oReturn = CheckDate($scope.Invoice.TODATE, "Ngày nhập kho", 10, true);
            //if (oReturn.result === false) {
            //    alert(oReturn.messErrorForCustomer);
            //    return false;
            //}
            //oReturn = compareDates($scope.Invoice.FROMDATE, $scope.Invoice.TODATE, msgImportDateExportDate);
            //if (oReturn.result === false) {
            //    alert(oReturn.messErrorForCustomer);
            //    return false;
            //}
        }

        //if ($scope.Invoice.CUSTAXCODE && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    var res = $scope.Invoice.CUSTAXCODE.split('-');
        //    if (res.length == 1) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    } else if (res.length == 2) {
        //        if (res[0].length != 10 || !validation.isNumber(res[0])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //        if (res[1].length != 3 || !validation.isNumber(res[1])) {
        //            alert('Vui lòng nhập đúng định dạng của mã số thuế');
        //            return false;
        //        }
        //    }
        //}

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email');
                return;
            }
        }

        if (!$scope.Invoice.CHANGEREASON && $scope.Invoice.INVOICETYPE == 5) {
            alert('Vui lòng nhập lý do điểu chỉnh');
            return false;
        }

        if ($scope.Invoice.INVOICETYPE != 5) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                if (!obj.PRODUCTNAME && $scope.Invoice.INVOICETYPE != 5) {
                    alert('Tên sản phẩm không được để trống, vui lòng kiểm tra lại dữ liệu dòng thứ ' + (index + 1));
                }
                obj.TAXRATEWATER = $scope.GLOBALTAXRATEWATER;
                //obj.QUANTITY = obj.QUANTITY.toString().replace(/\./g, ",");
                obj.QUANTITY = parseFloat(obj.QUANTITY.toString());
            });

            var find = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                return !s.PRODUCTNAME;
            });

            if (find.length > 0 && $scope.Invoice.INVOICETYPE != 5) {
                return false;
            }
        }

        // for only tax rate
        if ($scope.TAXRATE == 1) {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
                obj.TAXRATE = $scope.GLOBALTAXRATE;
                // hóa đơn bán hàng không có thuế. Gán mặc định = -1
                //if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
                //    obj.TAXRATE = -1;
                //}
            });
        }
        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '');/*.replace(/\./g, '')*/;
        //$scope.Invoice.CUSTOMFIELDEXCHANGE = $scope.Invoice.CUSTOMFIELDEXCHANGE.replace(/\,/g, '');
        //$scope.CalMoneyAfterChangeValue();
        // Map lại thông tin khi số lượng, đơn giá không điền. Để lưu thông tin vào database
        $scope.Invoice.LISTPRODUCT.forEach(function (obj, index) {
            obj.TOTALMONEY = obj.ITEMTOTALMONEY;
        });
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENTRAW;
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEYRAW;
        $scope.Invoice.TAXMONEY = $scope.Invoice.TAXMONEYRAW;
        LoadingShow();
        var action = url + 'UpdateInvoice';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (isSaveAndRelease) {
                        var invoiceId = response.result;
                        Sign(invoiceId).then(function () {
                            toastr.success("Cập nhật thành công!", "Thành công", { timeOut: 4000 })
                            $('.modal-invoice').modal('hide');
                            if ($location.path().toString().includes('/dai-hoa-don-cho')) {
                                $rootScope.ReloadWaitingInvoice($scope.Invoice.FORMCODE, $scope.Invoice.SYMBOLCODE);
                            }
                            else {
                                $rootScope.ReloadInvoice();
                            }
                        });           
                    }
                    else {
                        toastr.success("Cập nhật thành công!", "Thành công", { timeOut: 2000 })
                        $('.modal-invoice').modal('hide');
                        if ($location.path().toString().includes('/dai-hoa-don-cho')) {
                            $rootScope.ReloadWaitingInvoice($scope.Invoice.FORMCODE, $scope.Invoice.SYMBOLCODE);
                        }
                        else {
                            $rootScope.ReloadInvoice();
                        }
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    //Thêm dòng sản phẩm
    $scope.AddRow = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.PHIEUXUATKHO) {
            $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, INQUANTITY: 1, TAXRATE: 0, RETAILPRICE: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
        }
        else {
            //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, RETAILPRICE: 0, OTHERTAXFEE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
            $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, RETAILPRICE: 0, OTHERTAXFEE: 0, REFUNDFEE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, CONSIGNMENTID: '', DESCRIPTION: null });
        }
    }

    $scope.AddRowHeader = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: 0, OTHERTAXFEE: 0, REFUNDFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, ITEMMONEY: 0, ITEMTOTALMONEY: 0, GROUPID: 1, SORTORDER: 0, DESCRIPTION: null });
        //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 0, TAXRATE: 0, OTHERTAXFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, ITEMMONEY: 0, ITEMTOTALMONEY: 0, GROUPID: 1, SORTORDER: 0, DESCRIPTION: null });

        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        //$scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, OTHERTAXFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, DESCRIPTION: null });
        $scope.Invoice.LISTPRODUCT.push({ PRODUCTNAME: '', QUANTITYUNIT: 'Khác', QUANTITY: 1, TAXRATE: -1, OTHERTAXFEE: 0, REFUNDFEE: 0, RETAILPRICE: 0, DISCOUNTRATE: 0, TOTALDISCOUNT: 0, GROUPID: 0, SORTORDER: 0, DESCRIPTION: null });
    }

    //Xóa dòng sản phẩm
    $scope.RemoveRow = function (item) {
        if (item.PRODUCTNAME) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                        return s != item;
                    });
                    if ($scope.Invoice.LISTPRODUCT.length == 0)
                        $scope.AddRow();
                });
                $scope.CalMoneyAfterChangeValue();
            };
            confirm("Bạn chắc chắn muốn xóa sản phẩm \"" + item.PRODUCTNAME + "\"?", "Xóa sản phẩm", "Không", "Có", confirmContinue)
        }
        else {
            $timeout(function () {
                $scope.Invoice.LISTPRODUCT = $scope.Invoice.LISTPRODUCT.filter(function (s) {
                    return s != item;
                });
                if ($scope.Invoice.LISTPRODUCT.length == 0)
                    $scope.AddRow();
            });
            $scope.CalMoneyAfterChangeValue();
        }
    }

    //Xử lý nhảy dòng
    $scope.onKeypress = function (index) {
        $timeout(function () {
            var keyCode = $scope.Invoice.LISTPRODUCT[index].keyCode;

            if (keyCode == 38) {
                //Up arrow
                if (index > 0) {
                    $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                        obj.isFocus = false;
                    });
                    $timeout(function () {
                        $scope.Invoice.LISTPRODUCT[index - 1].isFocus = true;
                    });
                }
            } else if ((keyCode == 40 || keyCode == 13) && $scope.Invoice.LISTPRODUCT.length - 1 == index) {
                //Nếu là dòng cuối thì thêm dòng mới
                $scope.AddRow();

            } else {
                //Nếu chưa phải dòng cuối thì focus dòng bên dưới
                $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                    obj.isFocus = false;
                });
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT[index + 1].isFocus = true;
                });
            }
        });
    }

    $scope.CreateRowDescription = function (index) {
        var $item_p = ('#Item_p_' + index);
        $($item_p).removeClass('ng-hide');
        $($item_p).addClass('ng-show');
    }

    $scope.CloseRowDescription = function (index) {
        var $item_p = ('#Item_p_' + index);
        $($item_p).hide();
    }

    //Xử lý nhảy dòng
    $scope.onKeypress = function (index) {
        $timeout(function () {
            var keyCode = $scope.Invoice.LISTPRODUCT[index].keyCode;
            if (keyCode == 38) {
                //Up arrow
                if (index > 0) {
                    $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                        obj.isFocus = false;
                    });
                    $timeout(function () {
                        $scope.Invoice.LISTPRODUCT[index - 1].isFocus = true;
                    });
                }
            } else if ((keyCode == 40 || keyCode == 13) && $scope.Invoice.LISTPRODUCT.length - 1 == index) {
                //Nếu là dòng cuối thì thêm dòng mới
                $scope.AddRow();

            } else {
                //Nếu chưa phải dòng cuối thì focus dòng bên dưới
                $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                    obj.isFocus = false;
                });
                $timeout(function () {
                    $scope.Invoice.LISTPRODUCT[index + 1].isFocus = true;
                });
            }
        });
    }

    $scope.ChangeRetailPrice = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        var vPrice = item.PRICE;
        if (item.PRICE != null && !isNaN(parseFloat(item.PRICE))) {
            vPrice = GetNumber(item.PRICE);
        }
        item.RETAILPRICE = vPrice;
        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            item.ITEMTOTALMONEY = item.TOTALMONEY;
            item.ITEMMONEY = item.TOTALMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }

    $scope.ChangeTotalPrice = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        var itemTotalPrice = item.ITEMMONEY;
        if (item.ITEMMONEY != null && !isNaN(parseFloat(item.ITEMMONEY))) {
            itemTotalPrice = GetNumber(item.ITEMMONEY);
        }
        item.ITEMTOTALMONEY = itemTotalPrice;
        if (item.QUANTITY !== 0 && item.QUANTITY !== "0" && item.QUANTITY !== "") {
            item.PRICE = (itemTotalPrice / item.QUANTITY).toFixed(3).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            item.RETAILPRICE = (itemTotalPrice / item.QUANTITY);
        }
        if (item.QUANTITY === "0") {
            item.RETAILPRICE = 0;
            item.PRICE = 0;
        }
        if (item.QUANTITY === "") {
            item.RETAILPRICE = null;
            item.PRICE = null;
        }

        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            $scope.CalTotalMoney();
            $scope.CalTaxMoney();
            $scope.CalDiscountMoney();
            $scope.CalTotalPayment();
        }
    }

    $scope.QuantityChange = function (item) {
        $scope.DiscountChange(item, true); // Nếu có chiết khấu. Thay đổi đơn giá => thay đổi tiền chiết khấu

        item.QUANTITY = item.QUANTITY.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
        else {
            item.ITEMTOTALMONEY = item.TOTALMONEY;
            item.ITEMMONEY = item.TOTALMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        }
    }

    $scope.DiscountChange = function (item, isDiscountRateChange) {
        if ($scope.Invoice.DISCOUNTTYPE == 'CHIET_KHAU_THEO_HANG_HOA') {
            //item.QUANTITY = item.QUANTITY.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
            //if (!((item.QUANTITY === 0 || item.QUANTITY === "0" || item.QUANTITY === "") && item.RETAILPRICE === 0)) {
            //    item.ITEMTOTALMONEY = item.RETAILPRICE * item.QUANTITY;
            //    item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //    $scope.CalMoneyAfterChangeValue();
            //}
            //else {
            //    item.ITEMTOTALMONEY = item.ITEMMONEY;
            //    item.ITEMMONEY = item.ITEMMONEY.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            //}

            //var itemTotalMoney = Number(item.ITEMTOTALMONEY.replace(/[^0-9.-]+/g, ""));
            if (!isDiscountRateChange) {
                item.DISCOUNTRATE = item.TOTALDISCOUNT / item.ITEMTOTALMONEY * 100;
            }
            else {
                item.TOTALDISCOUNT = item.DISCOUNTRATE * item.ITEMTOTALMONEY / 100;
            }
            $scope.CalMoneyAfterChangeValue();
        }
    }

    $scope.SuggestProduct = function (item) {
        if (item) {
            var obj = $rootScope.selectedResultobj;
            item.RETAILPRICE = obj.PRICE;
            item.PRODUCTID = obj.PRODUCTID;
            item.PRODUCTNAME = obj.PRODUCTNAME;
            item.QUANTITYUNIT = obj.QUANTITYUNIT;
            item.QUANTITY = item.QUANTITY > 0 ? item.QUANTITY : 1;
            item.TAXRATE = item.TAXRATE > 0 ? item.TAXRATE : 0;
            item.PRICE = obj.PRICE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $scope.CalMoneyAfterChangeValue();
        }
    }

    $scope.QuantityUnitChange = function (item) {
        var listUnit = [];
        angular.forEach($rootScope.ListQuantityUnit, function (key) {
            listUnit.push(key.QUANTITYUNIT);
        });

        $('#quantityunit').autoComplete({
            minChars: 1,
            source: function (xxx, suggest) {
                var choices = JSON.parse(JSON.stringify(listUnit));
                var suggestions = [];
                for (i = 0; i < choices.length; i++) {
                    if (~($rootScope.cleanAccents(choices[i]).toLowerCase()).indexOf($rootScope.cleanAccents(item.QUANTITYUNIT.toLowerCase())))
                        suggestions.push(choices[i]);
                    suggest(suggestions);
                }
            }
        });
    }

    $scope.CalTotalMoney = function () {
        $scope.CalDiscountMoney();
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.TOTALMONEY = 0;
        $scope.Invoice.TOTALMONEYRAW = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    let total = element.QUANTITY * element.RETAILPRICE;
                    $scope.Invoice.TOTALMONEYRAW += total;
                    $scope.Invoice.TOTALMONEY += total * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }
                    $scope.Invoice.TOTALMONEYRAW += element.ITEMTOTALMONEY;
                    $scope.Invoice.TOTALMONEY += element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                }
            }
        });
        $scope.Invoice.TOTALMONEY = $scope.Invoice.TOTALMONEY - $scope.Invoice.DISCOUNTMONEY;
        $scope.Invoice.TOTALMONEYRAW = $scope.Invoice.TOTALMONEYRAW - $scope.Invoice.DISCOUNTMONEYRAW;

        // CURRENCY != VND
        $scope.Invoice.TOTALMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.TOTALMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.TOTALMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.TOTALMONEY;
    }

    $scope.CalTaxMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;

        $scope.Invoice.TAXWATERMONEY = 0;
        $scope.Invoice.TAXMONEY = 0;
        $scope.Invoice.TAXMONEYRAW = 0;

        if ($scope.TAXRATE == 1) {
            $scope.Invoice.TAXMONEY = $scope.Invoice.TOTALMONEY * ($scope.GLOBALTAXRATE == -1 ? 0 : $scope.GLOBALTAXRATE) / 100;
            $scope.Invoice.TAXMONEYRAW = $scope.Invoice.TOTALMONEYRAW * ($scope.GLOBALTAXRATE == -1 ? 0 : $scope.GLOBALTAXRATE) / 100;
        }
        else {
            $scope.Invoice.LISTPRODUCT.forEach(function (element) {
                var totalMoney = 0;
                var discountMoney = 0;
                var totalMoneyRaw = 0;
                var discountMoneyRaw = 0;

                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    totalMoney = element.QUANTITY * element.RETAILPRICE * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                    discountMoney = element.DISCOUNTRATE * totalMoney / 100;

                    totalMoneyRaw = element.QUANTITY * element.RETAILPRICE;
                    discountMoneyRaw = element.DISCOUNTRATE * totalMoneyRaw / 100;
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }
                    totalMoney = element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));
                    discountMoney = element.DISCOUNTRATE * totalMoney / 100;

                    totalMoneyRaw = element.ITEMTOTALMONEY;
                    discountMoneyRaw = element.DISCOUNTRATE * totalMoneyRaw / 100;
                }
                

                if (!element.ISPROMOTION) {
                    $scope.Invoice.TAXMONEY += (totalMoney - discountMoney) * (element.TAXRATE == -1 ? 0 : element.TAXRATE) / 100;
                    $scope.Invoice.TAXMONEYRAW += (totalMoneyRaw - discountMoneyRaw) * (element.TAXRATE == -1 ? 0 : element.TAXRATE) / 100;
                }
            });
        }              

        //Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        if ($rootScope.Enterprise.USINGINVOICETYPE === 4) {
            $scope.Invoice.TAXWATERMONEY += ($scope.Invoice.TOTALMONEY * $scope.GLOBALTAXRATEWATER) / 100;
            $scope.Invoice.TAXWATERMONEYRAW += $scope.Invoice.TOTALMONEY * ($scope.GLOBALTAXRATEWATER / 100);
        }

        // CURRENCY != VND
        $scope.Invoice.TAXMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.TAXMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.TAXMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.TAXMONEY;
    }

    $scope.CalOtherTaxFeeMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.OTHERTAXFEE = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    if (element.OTHERTAXFEE === "")
                        element.OTHERTAXFEE = 0;
                    $scope.Invoice.OTHERTAXFEE += parseFloat(element.OTHERTAXFEE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.OTHERTAXFEE === "")
                        element.OTHERTAXFEE = 0;
                    $scope.Invoice.OTHERTAXFEE += parseFloat(element.OTHERTAXFEE.toString().replace(/\,/g, ''));
                }
            }
        });
        return $scope.Invoice.OTHERTAXFEE;
    }

    $scope.CalRefundFee = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.REFUNDFEE = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    if (element.REFUNDFEE === "")
                        element.REFUNDFEE = 0;
                    $scope.Invoice.REFUNDFEE += parseFloat(element.REFUNDFEE.toString().replace(/\,/g, ''));
                }
                else {
                    if (element.REFUNDFEE === "")
                        element.REFUNDFEE = 0;
                    $scope.Invoice.REFUNDFEE += parseFloat(element.REFUNDFEE.toString().replace(/\,/g, ''));
                }
            }
        });
        return $scope.Invoice.REFUNDFEE;
    }

    $scope.CalDiscountMoney = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        $scope.Invoice.DISCOUNTMONEY = 0;
        $scope.Invoice.DISCOUNTMONEYRAW = 0;
        $scope.Invoice.LISTPRODUCT.forEach(function (element) {
            if (!element.ISPROMOTION) {
                if (!((element.RETAILPRICE === null || element.RETAILPRICE === NaN || element.RETAILPRICE >= 0) && (element.QUANTITY === "" || element.QUANTITY === "0" || element.QUANTITY === 0))) {
                    let total = element.QUANTITY * element.RETAILPRICE;
                    $scope.Invoice.DISCOUNTMONEY += element.DISCOUNTRATE * (total * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''))) / 100;
                    $scope.Invoice.DISCOUNTMONEYRAW += element.DISCOUNTRATE * total / 100;
                }
                else {
                    if (element.ITEMTOTALMONEY === undefined) {
                        element.ITEMTOTALMONEY = element.TOTALMONEY;
                    }

                    $scope.Invoice.DISCOUNTMONEY += element.DISCOUNTRATE * (element.ITEMTOTALMONEY * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''))) / 100;
                    $scope.Invoice.DISCOUNTMONEYRAW += element.DISCOUNTRATE * (element.ITEMTOTALMONEY) / 100;
                }
            }
        });

        // CURRENCY != VND
        $scope.Invoice.DISCOUNTMONEY = ($scope.Invoice.CURRENCY !== 'VND' ? $scope.Invoice.DISCOUNTMONEYRAW.formatMoneyNumberPlace(2) : $scope.Invoice.DISCOUNTMONEYRAW) * parseInt($scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, ''));

        return $scope.Invoice.DISCOUNTMONEY;
    }

    $scope.CalServiceFeeTax = function () {
        if ($scope.Invoice.SERVICEFEETAXRATE === 0 || $scope.Invoice.SERVICEFEETAXRATE === -1) {
            $scope.Invoice.SERVICEFEETAX = 0;
        }
        else {
            $scope.Invoice.SERVICEFEETAX = $scope.Invoice.SERVICEFEE * $scope.Invoice.SERVICEFEETAXRATE / 100;
        }
        $scope.CalTotalServiceFee();
        return $scope.Invoice.SERVICEFEETAX;
    }

    $scope.CalTotalServiceFee = function () {
        $scope.Invoice.TOTALSERVICEFEE = parseFloat($scope.Invoice.SERVICEFEE) + parseFloat($scope.Invoice.SERVICEFEETAX);
        $scope.CalMoneyAfterChangeValue();
        return $scope.Invoice.TOTALSERVICEFEE;
    }

    $scope.CalTotalPayment = function () {
        if (!$scope.Invoice.LISTPRODUCT)
            return;
        if (!$scope.Invoice.TOTALMONEY) {
            $scope.Invoice.TOTALMONEY = 0;
        }
        if (!$scope.Invoice.DISCOUNTMONEY) {
            $scope.Invoice.DISCOUNTMONEY = 0;
        }
        if (!$scope.Invoice.TAXMONEY) {
            $scope.Invoice.TAXMONEY = 0;
        }
        if (!$scope.Invoice.TAXWATERMONEY) {
            $scope.Invoice.TAXWATERMONEY = 0;
        }
        if (!$scope.Invoice.OTHERTAXFEE) {
            $scope.Invoice.OTHERTAXFEE = 0;
        }
        if (!$scope.Invoice.REFUNDFEE) {
            $scope.Invoice.REFUNDFEE = 0;
        }
        if (!$scope.Invoice.TOTALSERVICEFEE) {
            $scope.Invoice.TOTALSERVICEFEE = 0;
        }

        if ($rootScope.Enterprise.USINGINVOICETYPE === AccountObjectType.HOADONBANHANG) {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEYRAW
        }
        else if ($rootScope.Enterprise.USINGINVOICETYPE === 4)//Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXWATERMONEY + $scope.Invoice.TAXMONEY;
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXWATERMONEY + $scope.Invoice.TAXMONEY;
        }
        else {
            $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALMONEY + $scope.Invoice.TAXMONEY;
            $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALMONEYRAW + $scope.Invoice.TAXMONEYRAW;
        }
        $scope.Invoice.TOTALPAYMENT = $scope.Invoice.TOTALPAYMENT + $scope.Invoice.OTHERTAXFEE - $scope.Invoice.REFUNDFEE + $scope.Invoice.TOTALSERVICEFEE;
        $scope.Invoice.TOTALPAYMENTRAW = $scope.Invoice.TOTALPAYMENTRAW + $scope.Invoice.OTHERTAXFEE - $scope.Invoice.REFUNDFEE + $scope.Invoice.TOTALSERVICEFEE;
        return $scope.Invoice.TOTALPAYMENT;
    }

    $scope.CalMoneyAfterChangeValue = function () {
        if ($scope.TAXRATE == 1) {
            $scope.GLOBALTAXRATE = parseInt($('#idOnlyTaxRate').val());
        }
        //Nếu là hóa đơn tiền nước thì tính tiền phí bảo vệ môi trường
        if ($rootScope.Enterprise.USINGINVOICETYPE === 4) {
            $scope.GLOBALTAXRATEWATER = parseInt($('#txtTaxwater').val());
        }
        $timeout(function () {
            $scope.ChangeDiscountType();
            $scope.CalDiscountMoney();
            $scope.CalTotalMoney();
            $scope.CalTaxMoney();
            $scope.CalOtherTaxFeeMoney();
            $scope.CalRefundFee();
            $scope.CalTotalPayment();
            $scope.CalExChangeStr();
            $scope.ReadNumberToCurrencyWords();
        }, 100);
    }

    //Tihns quy đôiỉ
    $scope.CalExChangeStr = function () {
        //var cur = $scope.Invoice.TOTALPAYMENT / $scope.Invoice.CUSTOMFIELDEXCHANGERATE;
        var cur = $scope.Invoice.TOTALPAYMENT / $scope.Invoice.EXCHANGERATE;
        $scope.Invoice.CUSTOMFIELDEXCHANGE = (cur).formatMoney(2, ".", ",");
    }

    //$scope.ReadNumberToCurrencyWords = function () {
    //    var money = 0;
    //    // Kiểm tra là ngoại tệ hay Việt Nam Đồng
    //    if ($scope.Invoice.TOTALPAYMENTRAW && $scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
    //        money = $scope.Invoice.TOTALPAYMENTRAW;
    //    }
    //    else {
    //        money = $scope.Invoice.TOTALPAYMENT;
    //    }
    //    // End
    //    var action = url + 'ReadNumberToWords';
    //    var datasend = JSON.stringify({
    //        currency: $scope.Invoice.CURRENCY,
    //        number: money,
    //    });
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                $scope.NumberToCurrencyWords = response.data;
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ReadNumberToCurrencyWords');
    //        }
    //    });
    //}

    $scope.ReadNumberToCurrencyWords = function () {
        var money = 0;
        // Kiểm tra là ngoại tệ hay Việt Nam Đồng
        if ($scope.Invoice.TOTALPAYMENTRAW && $scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
            money = $scope.Invoice.TOTALPAYMENTRAW;
        }
        else {
            money = $scope.Invoice.TOTALPAYMENT;
        }
        // End
        var action = url + 'ReadNumberToWords';
        var datasend = JSON.stringify({
            currency: $scope.Invoice.CURRENCY,
            number: money,
            numberPlace: $rootScope.Enterprise.MONEYPLACE
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.NumberToCurrencyWords = response.data;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ReadNumberToCurrencyWords');
            }
        });
    }

    $scope.SuggestCustomer = function () {
        var obj = $rootScope.selectedCustomer;
        if (obj) {
            $scope.Invoice.CUSID = obj.CUSID;
            $scope.Invoice.CUSNAME = obj.CUSNAME;
            $scope.Invoice.CUSADDRESS = obj.CUSADDRESS;
            $scope.Invoice.CUSTAXCODE = obj.CUSTAXCODE;
            $scope.Invoice.CUSEMAIL = obj.CUSEMAIL;
            $scope.Invoice.CUSPHONENUMBER = obj.CUSPHONENUMBER;
            $scope.Invoice.CUSBUYER = obj.CUSBUYER;
            $scope.Invoice.CUSTOMERCODE = obj.CUSID;
            $scope.Invoice.CUSACCOUNTNUMBER = obj.CUSACCOUNTNUMBER;
            $scope.GetMeterByCustaxcode(obj.CUSID);
        }
    }

    $scope.GetInvoice = function () {
        LoadingShow();
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    $scope.GetInvoiceById = function (id) {
        var action = url + 'GetInvoiceById';
        var datasend = JSON.stringify({
            id: id
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.RefInvoice = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceById');
            }
        });
    }

    $scope.SelectAll = function () {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListInvoice.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.LoadInfoByTaxcode2 = function () {
        if (!$scope.Invoice.CUSTAXCODE) {
            alert('Bạn cần nhập MST');
            return false;
        }
        LoadingShow();
        if ($scope.Invoice.CUSTAXCODE && $scope.Invoice.CUSTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/other/getcompanyinfobytaxcode?taxcode=' + $scope.Invoice.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                $timeout(function () {
                    var rawObj = JSON.parse(response);
                    var obj = new Object();
                    if (rawObj.Data != "") {
                        obj = JSON.parse(rawObj.Data);
                    }
                    if (obj.companyName) {
                        $scope.Invoice.CUSADDRESS = obj.address;
                        $scope.Invoice.CUSNAME = obj.companyName;
                    } else {
                        toastr.warning("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Invoice.CUSNAME = null;
                        $scope.Invoice.CUSADDRESS = null;
                    }
                    LoadingHide();
                }, 2000);
            });
        }
    }

    $scope.LoadInfoByTaxcode = function () {
        //LoadingShow();
        //var action = url + 'LoadInfoByTaxcode';
        //var datasend = JSON.stringify({
        //    taxcode: $scope.Invoice.CUSTAXCODE
        //});
        //CommonFactory.PostDataAjax(action, datasend, function (response) {
        //    if (response) {
        //        if (response.rs) {
        //            if (response.data) {
        //                $scope.Invoice.CUSADDRESS = response.data.Ho_Address;
        //                $scope.Invoice.CUSNAME = response.data.Name;
        //            }
        //            else {
        //                $scope.Invoice.CUSNAME = null;
        //                $scope.Invoice.CUSADDRESS = null;
        //                $scope.LoadInfoByTaxcode2();
        //            }
        //        }
        //        else {
        //            $scope.LoadInfoByTaxcode2();
        //        }
        //    } else {
        //        //alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LoadInfoByTaxcode');
        //        $scope.LoadInfoByTaxcode2();
        //    }
        //    LoadingHide();
        //}, 5000);


        $scope.LoadInfoByTaxcode2();
    }

    $scope.LoadCurrencyExchangeRate = function () { 
        var action = url + 'LoadCurrency';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                var lstCurrency = [];
                lstCurrency = response.items === undefined ? [] : response.items;
                lstCurrency.push({ type: 'VND', imageurl: '', muatienmat: 1, muack: 1, bantienmat: 1, banck: 1 });
                lstCurrency = lstCurrency.filter(function (x) {
                    return (x.type != "XAU");
                }).filter(function (y) {
                    return y.type != "PNJ_DAB";
                });
                lstCurrency.forEach(function (z) {
                    if (z.type == "JPY") {
                        z.bantienmat = z.bantienmat.split('.')[0];
                    }
                })
                $scope.CurrencyList = lstCurrency;
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LoadCurrencyExchangeRate');
            }
        });
    }

    $scope.ChangeCurrencyType = function (currencyType) {
        var exchangeRateObj = $scope.CurrencyList.filter(function (x) {
            return x.type === currencyType;
        })
        $scope.Invoice.EXCHANGERATE = exchangeRateObj[0].bantienmat.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $scope.CalMoneyAfterChangeValue();
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $scope.DiscountType = [
        {
            value: "KHONG_CO_CHIET_KHAU", name: "Không có chiết khấu"
        },
        {
            value: "CHIET_KHAU_THEO_HANG_HOA", name: "Chiết khấu theo hàng hóa"
        }
    ];


    $scope.TaxRateList = [
        {
            value: -1, name: "Không chịu thuế"
        },
        {
            value: 0, name: "0%"
        },
        {
            value: 5, name: "5%"
        },
        {
            value: 10, name: "10%"
        }
    ];

    $scope.Hoa_Don_Ban_Hang_TaxRateList = [
        {
            value: -1, name: "Hàng hóa, dịch vụ không chịu thuế GTGT hoặc thuế GTGT 0%"
        },
        {
            value: 1, name: "Phân phối, cung cấp hàng hoá (1%)"
        },
        {
            value: 5, name: "Dịch vụ, xây dựng không bao thầu nguyên vật liệu (5%)"
        },
        {
            value: 3, name: "Sản xuất, vận tải, dịch vụ có gắn với hàng hoá, xây dựng có bao thầu nguyên vật liệu (3%)"
        },
        {
            value: 2, name: "Hoạt động kinh doanh khác (2%)"
        }
    ];

    $scope.ViewTaxcode = function (taxcode) { 
        if (!taxcode)
            return;
        var number_regex = new RegExp(/\d/, "gi");
        var symbol_regex = new RegExp(/\S/, "gi");
        taxcode = taxcode.replace(symbol_regex, function (matched) {
            if (matched.match(number_regex))
                return "<span class=\"" + "taxcode-symbol" + "\">" + matched + "</span>";
            else
                return "<span class=\"" + "taxcode-space" + "\">" + matched + "</span>";
        });

        return $sce.trustAsHtml(taxcode);
    }

    $scope.ChangeDiscountType = function () {
        if ($scope.Invoice.DISCOUNTTYPE == 'KHONG_CO_CHIET_KHAU') {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                obj.DISCOUNTRATE = 0;
            });
        }
        else {

        }
    }

    $scope.ModalInvoiceNote = function () {
        $('#modal_invoice_note').dialog({
            modal: true,
            width: '35%'
        });

        if ($scope.Invoice.ID == 0) {
            $scope.Invoice.NOTE = '';
        }
    }

    $scope.GetNumber = function () {
        $scope.FormNumber = new Object();

        $scope.FormNumber.FORMCODE = $scope.Number.FORMCODE;
        $scope.FormNumber.SYMBOLCODE = $scope.Number.SYMBOLCODE;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.result[0];
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER + 1;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumberaa');
            }
            LoadingHide();
        });
    }

    $scope.ExchangeRateFormat = function () {
        $("#exchangeRateInput").on('keyup', function () {
            if ($(this).val() != 'NaN') {
                var n = parseInt($(this).val().replace(/\D/g, ''), 10);
                $(this).val(n.toLocaleString().replace('.', ','));
            }
            else {
                $(this).val(1);
            }
        })
    }

    $scope.FormCodeChange = function (formcode) {
        $rootScope.ListFormCode = $scope.TempListFormCode;
        $timeout(function () {
            if ($rootScope.ListFormCode !== undefined) {
                let tmp = formcode.split('-');
                let formCodeSymbolCode = $rootScope.ListFormCode.filter(function (x) {
                    return x.FORMCODE === tmp[0] && x.SYMBOLCODE === tmp[1];
                });

                $scope.GLOBALTAXRATE = 0;
                $scope.TAXRATE = formCodeSymbolCode[0].TAXRATE;
                $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
            }
        }, 100)
    }

    /**
     * truongnv 16.3.2020
     * Kiểm tra loại hóa đơn điều chỉnh để cho phép người dùng sửa thông tin
     * @param {any} value
     */
    $scope.ChangeInvoiceMethod = function (value) {
        var $divContainer = $('#product-container');
        //if ($divContainer && value === 3)
        //    $divContainer.attr('style', 'pointer-events:none;');
        //else
        $divContainer.attr('style', 'pointer-events:auto;');
    }

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $scope.GetMeterByCustaxcode = function (custaxcode) {
        if (custaxcode === undefined) return;
        LoadingShow();
        var action = '/Meter/GetMeterListByCustaxcode';
        var datasend = JSON.stringify({
            custaxcode: custaxcode
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.MeterList = response.msg;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetMeterByCustaxcode');
            }
        });
        LoadingHide();
    }

    $scope.GetMeterByInvoiceId = function (invoiceid) {
        var action = '/Meter/GetListMeterCodeByInvoiceID';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs && response.msg.length > 0) {
                    $scope.MeterList = response.msg;
                    setSelectedValueDropdown($rootScope.Enterprise.USINGINVOICETYPE, $scope.MeterList[0].METERCODE);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetMeterByInvoiceId');
            }
        });
    }

    /**
     * truongnv 20200318
     * Thêm dòng dữ liệu khi chọn khu vực
     * @param {any} value
     */
    $scope.ChooseMeter = function () {
        //// lấy thông tin hàng hóa, dịch vụ
        //var $div = $('#customer_name');
        ////var meterCode = item.METERNAME;
        ////if (index === 4)
        ////    meterCode = $('#ddlMeter2').val();

        //var isCreateRow = true;
        ////Kiểm tra xem đã tồn tại Bộ công tơ chưa
        //$('.item_customer_name .innertext-cusname').each(function (index, item) {
        //    var code = item.getAttribute('rel');
        //    if (code === meterCode) {
        //        isCreateRow = false;
        //        return isCreateRow;
        //    }
        //});

        //if ($scope.MeterList != null) {
        //    var objM = $scope.MeterList.filter(x => x.CODE === meterCode);
        //    if (objM && Object.keys(objM).length > 0)
        //        metername = objM[0].METERNAME;

        //    if (isCreateRow) {
        //        if ($div.text() === '[Số công tơ]') {
        //            $div.attr('rel', meterCode);
        //            $div.text(metername);
        //        }
        //        else {
        //            $('#ITEM_HOADONTIENDIEN tbody tr:last').after("<tr style='background-color: #dee2e6;'><td class='item_customer_name'><strong id='customer_name' class='innertext-cusname' rel='" + meterCode + "'>" + metername + "</strong></td><td colspan='7'></td></tr >");
        //        }

        var action = '/Product/GetProductListByMeterCode';
        var datasend = JSON.stringify({
            meterCode: $scope.Invoice.METER.CODE,
            custaxcode: $scope.Invoice.CUSID
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {

                    for (var i = 0; i < response.msg.length; i++) {
                        $scope.AddDataRow(response.msg[i]);
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddDataRow');
            }
        });
        //    }
        //}
    }

    /**
     * truongnv 20200318
     * Tạo thêm dòng dữ liệu
     * @param {any} data
     */
    $scope.AddDataRow = function (data) {
        data["PRICE"] = data["PRICE"] === undefined ? 0 : data["PRICE"];
        data["RETAILPRICE"] = data["RETAILPRICE"] === undefined ? 0 : data["RETAILPRICE"];
        data["ITEMMONEY"] = data["ITEMMONEY"] === undefined ? 0 : data["ITEMMONEY"];
        data["QUANTITY"] = data["QUANTITY"] === undefined ? 0 : data["QUANTITY"];

        data["OLDNO"] = data["OLDNO"] === undefined ? 0 : data["OLDNO"];
        data["NEWNO"] = data["NEWNO"] === undefined ? 0 : data["NEWNO"];
        data["FACTOR"] = data["FACTOR"] === undefined ? 0 : data["FACTOR"];

        var obj = { METERCODE: data["METERCODE"], PRODUCTNAME: data["PRODUCTNAME"], OLDNO: data["OLDNO"], NEWNO: data["NEWNO"], FACTOR: data["FACTOR"], QUANTITY: 1, TAXRATE: -1, RETAILPRICE: data["PRICE"], METERNAME: data["METERNAME"] }
        if (!$scope.Invoice.LISTPRODUCT)
            $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push(obj);
    }

    /**
     * Tính Định mức tiêu thụ điện và thành tiền
     * @param {any} item
     */
    $scope.ElectricInvoiceQuantityChange = function (item) {
        item.OLDNO = item.OLDNO.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        item.NEWNO = item.NEWNO.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        // tính định mức tiêu thụ
        var quantity = (item.NEWNO - item.OLDNO) * item.FACTOR;
        item.QUANTITY = quantity.toString().replace(/\,/g, ".").replace(/[^0-9.]/g, "");
        //tính thành tiền
        item.ITEMMONEY = (item.RETAILPRICE * item.QUANTITY).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $scope.CalMoneyAfterChangeValue();
    }

    $scope.SetDatepikerDate = function () {
        var toDate = $scope.Invoice.TODATE;
        $scope.Invoice.TODATESTR = toDate;

        var fromDate = $scope.Invoice.FROMDATE;
        $scope.Invoice.FROMDATESTR = fromDate;

        var deliveryOrderDate = $scope.Invoice.DELIVERYORDERDATE;
        $scope.Invoice.DELIVERYORDERDATESTR = deliveryOrderDate;
    }
}]);

function setSelectedValueDropdown(type, value) {
    if (type === 2) {
        var $div = $("#ddlMeter option:first");
        if ($div && $div.length > 0) {
            $div.html(value);
            $div.attr('value', value);
        }
    } else if (type === 4) {
        var $div = $("#ddlMeter2 option:first");
        if ($div && $div.length > 0) {
            $div.html(value);
            $div.attr('value', value);
        }
    }
}

function xoa_dau(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    return str;
}


function CheckDate(strCheck, field, fieldLength, isNotBlank) {
    var oReturn = {
        messErrorForCustomer: '',
        messErrorForCoder: '',
        result: true
    };
    if (strCheck === undefined || strCheck.length == 0)//Có được phép để trống trường này hay không
    {
        if (isNotBlank)//Có được phép để trống trường này hay không
        {
            oReturn.result = false;
            oReturn.messErrorForCustomer = field + " không được để trống";
            oReturn.messErrorForCoder = field + " không được để trống";
        }
    }
    else {
        if (strCheck.indexOf("-") != -1)
            strCheck.replace("-", "/");
        var date = strCheck.split('/');
        if (parseInt(date[0]) > 31 || parseInt(date[0]) < 0 || parseInt(date[1]) > 12 || parseInt(date[1]) < 0 || (date[1]).length > 2 || (date[0]).length > 2 || (date[2]).length > 4 || (date[1]).length < 0 || (date[0]).length < 0 || (date[2]).length < 0) {
            oReturn.result = false;
            oReturn.messErrorForCustomer = "Ngày không đúng định dạng dd/mm/yyyy";
            oReturn.messErrorForCoder = "Ngày không đúng định dạng dd/mm/yyyy";
        }
    }

    return oReturn;
}

function compareDates(valueTuThangNam, valueDenThangNam, messageName) {
    var oReturn = {
        messErrorForCustomer: '',
        messErrorForCoder: '',
        result: true
    };

    var arrDenThangNam = valueDenThangNam.split('/');
    var arrTuThangNam = valueTuThangNam.split('/');
    var DenThangNam = new Date(arrDenThangNam[2] + '-' + arrDenThangNam[1] + '-' + arrDenThangNam[0]);
    var TuThangNam = new Date(arrTuThangNam[2] + '-' + arrTuThangNam[1] + '-' + arrTuThangNam[0]);
    var dateTime1 = new Date(TuThangNam).getTime(),
        dateTime2 = new Date(DenThangNam).getTime();
    var diff = dateTime2 - dateTime1;
    if (diff < 0) {
        oReturn.result = false;
        oReturn.messErrorForCustomer = messageName;
        oReturn.messErrorForCoder = messageName;
    }
    return oReturn;
}

function FormatDateTimeFromAjax(dateAjax, strFormat) {
    var date = new Date(parseInt(dateAjax.replace(/(^.*\()|([+-].*$)/g, '')));
    return FormatDateTime(date, strFormat);
}
function FormatDateTime(date, strFormat) {
    var dd = date.getDate();
    var MM = date.getMonth() + 1; //January is 0!
    var HH = date.getHours();
    var mm = date.getMinutes();
    var ss = date.getSeconds();

    return strFormat.replace("dd", dd < 10 ? '0' + dd : dd)
        .replace("MM", MM < 10 ? '0' + MM : MM)
        .replace("yyyy", date.getFullYear())
        .replace("HH", HH < 10 ? '0' + HH : HH)
        .replace("mm", mm < 10 ? '0' + mm : mm)
        .replace("ss", ss < 10 ? '0' + ss : ss);
}

function ConvertDateShowToDate(strDate) {
    var arrDate = strDate.split('/');
    if (arrDate.length == 3) {
        var year = Number(arrDate[2]);
        var month = Number(arrDate[1]);
        var day = Number(arrDate[0]);
        return year + "-" + month + "-" + day;
    }
}

function AddDaysToDate(date, days) {
    var result = new Date(date).getTime() + days * 24 * 3600000;
    return new Date(result);
}

function formatMoney(number, decPlaces, decSep, thouSep) {
    decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
        decSep = typeof decSep === "undefined" ? "." : decSep;
    thouSep = typeof thouSep === "undefined" ? "," : thouSep;
    var sign = number < 0 ? "-" : "";
    var i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(decPlaces)));
    var j = (j = i.length) > 3 ? j % 3 : 0;

    return sign +
        (j ? i.substr(0, j) + thouSep : "") +
        i.substr(j).replace(/(\decSep{3})(?=\decSep)/g, "$1" + thouSep) +
        (decPlaces ? decSep + Math.abs(number - i).toFixed(decPlaces).slice(2) : "");
}

//custom for ES6
function formatMoney(amount, decimalCount = 2, decimal = ".", thousands = ",") {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;

        return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
        console.log(e)
    }
}

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

            //LoadingHide();
        },
        error: function (xhr, textStatus, errorThrown) {
            //LoadingHide();
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

$('.modal-invoice').on('hidden.bs.modal', function (e) {
    // do something...
    alert("logs");
})
app.controller('ModalProductController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Product/';

    $rootScope.ModalProduct = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        $timeout(function () {
            $rootScope.GetCategory();
            $rootScope.GetQuantityUnit();
        }, 200)

        $scope.TYPECHANGE = type;
        $scope.Product = new Object();
        if (type == 1) {
            //tạo mới
            $scope.Product = new Object();
            if (typeof item === "string") {
                $scope.Product.PRODUCTNAME = item;
            }
        }
        else if (type == 2) {
            //chỉnh sửa
            angular.copy(item, $scope.Product);
            $scope.Product.STRPRICE = $scope.Product.PRICE;
        } else if (type == 3) {
            //sao chép
            angular.copy(item, $scope.Product);
            $scope.Product.ID = 0
        }
    }


    $scope.ChangeRetailPrice = function (item) {
        var vPrice = item.STRPRICE;
        if (item.STRPRICE != null && !isNaN(parseFloat(item.STRPRICE))) {
            vPrice = GetNumber(item.STRPRICE);
        }
        item.PRICE = vPrice;
    }

    $scope.uploadFile = function (event) {
        LoadingShow();
        var files = event.target.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var reader = new FileReader();
            reader.onload = function (event) {
                $timeout(function () {
                    $scope.Product.IMAGE = event.target.result;
                    LoadingHide();
                }, 100);
            };
            reader.readAsDataURL(file);
        }
    };


    $scope.AddProduct = function () {
        if (!$scope.Product.PRODUCTTYPE) {
            alert('Vui lòng chọn kiểu sản phẩm');
            return false;
        }
        if (!$scope.Product.PRODUCTNAME) {
            alert('Vui lòng nhập vào tên sản phẩm');
            return false;
        }
        //if (!$scope.Product.CATEGORY && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn danh mục sản phẩm');
        //    return false;
        //}
        //if (!$scope.Product.PRICE) {
        //    alert('Vui lòng nhập vào đơn giá');
        //    return false;
        //}
        //if (!$scope.Product.QUANTITYUNIT && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn đơn vị tính');
        //    return false;
        //}
        $scope.IsLoading = true;
        var action = url + 'AddProduct';

        var datasend = JSON.stringify({
            product: $scope.Product
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-product').modal('hide');
                    $rootScope.ReloadProduct(1);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddProduct');
            }
            LoadingHide();
        });
    }

    $scope.UpdateProduct = function () {
        if (!$scope.Product.PRODUCTTYPE) {
            alert('Vui lòng chọn kiểu sản phẩm');
            $('div#productType').focus();
            return false;
        }
        if (!$scope.Product.PRODUCTNAME) {
            alert('Vui lòng nhập vào tên sản phẩm');
            $('div#idProductName').focus();
            return false;
        }
        //if (!$scope.Product.CATEGORY && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn danh mục sản phẩm');
        //    $('select#idCategory').focus();
        //    return false;
        //}
        //if (!$scope.Product.PRICE) {
        //    alert('Vui lòng nhập vào đơn giá');
        //    $('div#idProductPrice').focus();
        //    return false;
        //}
        //if (!$scope.Product.QUANTITYUNIT && ($rootScope.Enterprise.USINGINVOICETYPE !== 2 && $rootScope.Enterprise.USINGINVOICETYPE !== 4)) {
        //    alert('Vui lòng chọn đơn vị tính');
        //    $('select#idProductQuantityUnit').focus();
        //    return false;
        //}
        var action = url + 'UpdateProduct';
        var datasend = JSON.stringify({
            product: $scope.Product
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-product').modal('hide');
                    $rootScope.ReloadProduct();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateProduct');
            }
            LoadingHide();
        });
    }

    $scope.QuantityUnitChange = function (item) {
        var listUnit = [];
        angular.forEach($rootScope.ListQuantityUnit, function (key) {
            listUnit.push(key.QUANTITYUNIT);
        });

        $('#quantityunitproduct').autoComplete({
            minChars: 1,
            source: function (xxx, suggest) {
                var choices = JSON.parse(JSON.stringify(listUnit));
                var suggestions = [];
                suggestions.length = 0;
                for (i = 0; i < choices.length; i++) {
                    if (~($rootScope.cleanAccents(choices[i]).toLowerCase()).indexOf($rootScope.cleanAccents(item.toLowerCase())))
                        suggestions.push(choices[i]);
                    suggest(suggestions);
                }
            }
        });
    }
}]);
app.controller('ModalCategoryController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Category/';
    $scope.category = new Object();
    $rootScope.ModalCategory = function (item) {
        angular.copy(item, $scope.category);
        if (item === "") {
            $scope.category.ISACTIVE = true;
        }
    }
    //thêm danh mục
    $scope.SaveCategory = function () {
        if (!$scope.category.CATEGORY) {
            alert('Vui lòng nhập tên dịch vụ');
            return;
        }
        if ($scope.category.CATEGORY.length >= 512) {
            toastr.warning('Tên danh mục độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SaveCategory';
        var datasend = JSON.stringify({
            category: $scope.category
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-category').modal('hide');
                    $rootScope.ReloadCategory(1);
                    $rootScope.GetCategory();
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCategory');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdateCategory = function (item) {
        let msg = "";
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveCategory';
        if (item.ISACTIVE == true) {
            item.ISACTIVE = false;
            msg = 'Ngừng kích hoạt thành công';
        } else {
            item.ISACTIVE = true;
            msg = 'Kích hoạt thành công';
        } 
        var datasend = JSON.stringify({
            category: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadCategory(1);
                    $rootScope.GetCategory();
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCategory');
            }
        });
    }
}]);
app.controller('ModalCustomerController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Customer/';
    //var $scope.Customer.CUSTOMERTYPE = 1;
    $rootScope.ModalCustomer = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }
        $rootScope.GetPaymentMethod();
        $rootScope.GetCurrencyUnit();
        $scope.TYPECHANGE = type;
        $scope.selectedValues = [];
        $scope.Customer = new Object();
        if (type == 1) {
            $scope.Customer = new Object();
            $scope.Customer.CUSPAYMENTMETHOD = "TM/CK";
            $scope.Customer.CUSCURRENCYUNIT = "VNĐ (Việt Nam Đồng)";
        } else if (type == 2) {
            angular.copy(item, $scope.Customer);
            if ($rootScope.Enterprise.USINGINVOICETYPE === 2) {
                if ($scope.Customer.METERCODE)
                    $scope.selectedValues = $scope.Customer.METERCODE.split(',');
                $scope.GetListMeters();
            }
        }
        else if (type == 3) {
            angular.copy(item, $scope.Customer);
            $scope.Customer.ID = 0
            if ($rootScope.Enterprise.USINGINVOICETYPE === 2) {
                if ($scope.Customer.METERCODE)
                    $scope.selectedValues = $scope.Customer.METERCODE.split(',');
                $scope.GetListMeters();
            }
        }
        if ($scope.Customer.COMTAXCODE !== null || $scope.Customer.COMTAXCODE !== '') {
            $scope.Customer.CUSTOMERTYPE = 1;
        }
        else {
            $scope.Customer.CUSTOMERTYPE = 2;
        }
        if ($scope.Customer.CUSTOMERTYPE === 1) {
            $('#comtaxt-code-required').html('(*)');
            $('#ctr-address-required').html('(*)');
            $('input:radio[name=DefaultCustomer]').prop('checked', false);
            $('input:radio[name=DefaultPersional]').prop('checked', true);
        }
        else {
            $('input:radio[name=DefaultCustomer]').prop('checked', true);
            $('input:radio[name=DefaultPersional]').prop('checked', false);
            $('#comtaxt-code-required').html('');
            $('#ctr-address-required').html('');
        }
        $rootScope.Customercode = $scope.Customer.CUSTAXCODE;
    }

    /**
     * truongnv 2020030203
     * Kiểm ra bắt buộc nhập MST
     * @param {any} value
     */
    $scope.SetRequiedTaxCode = function (value) {
        $scope.Customer.CUSTOMERTYPE = value;
        if (value === 1) {
            $('#comtaxt-code-required').html('(*)');
            $('#ctr-address-required').html('(*)');
            $('input:radio[name=DefaultCustomer]').prop('checked', false);
            $('input:radio[name=DefaultPersional]').prop('checked', true);
        }
        else {
            $('input:radio[name=DefaultCustomer]').prop('checked', true);
            $('input:radio[name=DefaultPersional]').prop('checked', false);
            $('#comtaxt-code-required').html('');
            $('#ctr-address-required').html('');
        }
    }

    $scope.AddCustomer = function () {
        if ($rootScope.Enterprise.USINGINVOICETYPE === 1) {
            if (!$scope.Customer.CUSTAXCODE) {
                alert('Vui lòng nhập vào nhóm khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSID) {
                alert('Vui lòng nhập vào mã khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSNAME) {
                alert('Vui lòng nhập vào tên khách hàng');
                return false;
            }
            if (!$scope.Customer.CUSADDRESS) {
                alert('Vui lòng nhập vào địa chỉ');
                return false;
            }

        } else {
            if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSTAXCODE) {
                alert('Vui lòng nhập vào mã số thuế doanh nghiệp');
                return false;
            }
            if (!$scope.Customer.CUSNAME) {
                alert('Vui lòng nhập vào tên khách hàng');
                return false;
            }
            //if (!$scope.Customer.CUSID) {
            //    alert('Vui lòng nhập mã khách hàng');
            //    return false;
            //}
            //if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSBUYER) {
            //    alert('Vui lòng nhập vào tên người mua hàng');
            //    return false;
            //}

            if (!$scope.Customer.CUSADDRESS) {
                alert('Vui lòng nhập vào địa chỉ');
                return false;
            }

            if (!$scope.Customer.CUSPAYMENTMETHOD) {
                alert('Vui lòng chọn hình thức thanh toán');
                return false;
            }
        }
        

        var action = url + 'AddCustomer';
        $scope.Customer.METERCODE = $scope.selectedValues.join(',');
        var datasend = JSON.stringify({
            customer: $scope.Customer
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "4000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-customer').modal('hide');
                    $rootScope.ReloadCustomer();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddCustomer');
            }
            LoadingHide();
        });
    }

    $scope.UpdateCustomer = function () {
        if (!$scope.Customer.CUSNAME) {
            alert('Vui lòng nhập vào tên khách hàng');
            return false;
        }

        if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSTAXCODE) {
            alert('Vui lòng nhập vào mã số thuế doanh nghiệp');
            return false;
        }
        //if (!$scope.Customer.CUSID) {
        //    alert('Vui lòng nhập mã khách hàng');
        //    return false;
        //}
        if ($scope.Customer.CUSTOMERTYPE == 1 && !$scope.Customer.CUSBUYER) {
            alert('Vui lòng nhập vào tên người mua hàng');
            return false;
        }

        if (!$scope.Customer.CUSADDRESS) {
            alert('Vui lòng nhập vào địa chỉ');
            return false;
        }

        if (!$scope.Customer.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }
        var action = url + 'UpdateCustomer';
        $scope.Customer.METERCODE = $scope.selectedValues.join(',');
        var datasend = JSON.stringify({
            customer: $scope.Customer
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "4000" };
                    toastr.success("Cập nhật thành công!");
                    $('.modal-customer').modal('hide');
                    $rootScope.ReloadCustomer();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateCustomer');
            }
            LoadingHide();
        });
    }

    $scope.GetListMeters = function () {
        if (!$scope.Customer.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            $scope.ListMeters = [];
            return false;
        }

        $scope.IsLoading = true;
        var action = '/Meter/GetMeterListByCustaxcode';
        var datasend = JSON.stringify({
            custaxcode: $scope.Customer.CUSTAXCODE
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('.selectpicker').selectpicker({});
                    $scope.ListMeters = response.msg;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 1000);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCurrencyUnit');
            }
        });
        $scope.IsLoading = false;
    }

    /**
     * Gán mã số thuế cho form thêm mới công tơ
     * @param {any} customercode
     */
    $scope.FillCustomerCode = function (customercode) {
        $rootScope.Customercode = customercode;
    }
    $scope.LoadInfoByTaxcode = function () {
        if ($scope.Customer.CUSTAXCODE == null) {
            toastr.warning('vui lòng nhập mã số thuế của khách hàng');
        }
        if ($scope.Customer.CUSTAXCODE != $scope.oldTaxcode && $scope.Customer.CUSTAXCODE != null && $scope.Customer.CUSTAXCODE != "") {
            var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $scope.Customer.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                LoadingShow();
                var rawObj = JSON.parse(response);
                var obj = new Object();
                if (rawObj.Data != "") {
                    obj = JSON.parse(rawObj.Data);
                }
                $timeout(function () {
                    if (obj.companyName) {
                        $scope.Customer.CUSID = obj.companyId;
                        $scope.Customer.CUSADDRESS = obj.address;
                        $scope.Customer.CUSNAME = obj.companyName.toUpperCase();
                    } else {
                        alert("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Customer.CUSNAME = null;
                        $scope.Customer.CUSADDRESS = null;
                    }
                    LoadingHide();
                },1000);
            });
        } else if ($scope.Customer.CUSTAXCODE != $scope.oldTaxcode) {
            $scope.Customer.CUSNAME = null;
            $scope.Customer.CUSADDRESS = null;
        }
    }
}]);
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
app.controller('ModalImportCustomerController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Customer/';
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

    $scope.changeDataMapping = function () {
        $scope.listColumnError = [];
    }

    $scope.ImportCustomerFromExcel = function () {
        $('#fileUploadCustomer').trigger('click');
    };

    $('#frmImportCustomer').fileupload({
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
        var action = url + 'PreviewCustomerData';
        successCallback = function (data) {
            $scope.ImportCustomers = data.ListCustomers;
            $timeout(function () {
                $scope.stepThree = true;
                $scope.stepTwo = false;
            }, 100);
        }
        AjaxRequest(action, { listMap: lst, formCode: $scope.Import.FORMCODE, symbolCode: $scope.Import.SYMBOLCODE }, "POST", true, "json", successCallback);
    }

    $scope.count = 1;
    $scope.SaveCustomerList = function () {
        if (!$scope.ImportCustomers) {
            alert("Bạn chưa tải file lên. Vui lòng tải chọn file tải lên.");
        }
        var action = url + 'ImportDataCustomer';
        successCallback = function (data) {
            //if ($scope.count == 1) {
            //    toastr.success(data.msg);
            //    $timeout(function () {
            //        location.reload(true);
            //    }, 4000);
            //}
            //$scope.count++;
            $(".import-customer").modal('hide');
            $scope.ResetAllStep();
        }
        AjaxRequest(action, {}, "POST", true, "json", successCallback);
    }

    //Background service job
    $scope.BackgroundJobServiceCustomer = function () {
        var invoiceHub = $.connection.signlRConf;
        var userid = sessionStorage.getItem("userNameSS");

        $.connection.hub.qs = { 'username': userid };
        var invRow = 0;
        invoiceHub.client.newMessageReceivedCustomer = function (message) {
            invRow = invRow + 1;
            console.log(invRow);
            if (invRow === message.totalRow) {

                var messageSuccess = 'Thêm mới thành công <b>' + invRow + '/' + message.totalRow + '</b> khách hàng.';
                $('.invoiceResult').html("Hoàn Thành");
                $("#progressTab").hide();
                toastr.success(messageSuccess);
                invRow = 0;
                var confirmContinue = function (result) {
                    if (result)
                        return false;
                    $rootScope.ReloadCustomer();
                };
                confirm(messageSuccess + '<br> Bạn có muốn làm mới dữ liệu không?', 'Thêm mới khách hàng', 'Có', 'Không', confirmContinue);
            }
            else {
                $("#progressTab").show();
                var percent = Math.floor((invRow / message.totalRow) * 100);
                $('.invoiceResult').html('Đang tải khách hàng: ' + percent + '%');
            }

        };
        //$.connection.hub.start().done(function () {
        //    $("#progressTab").hide();
        //});
    };
    $scope.BackgroundJobServiceCustomer();

}]);
app.controller('ModalPreviewInvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';

     

}]);
app.controller('ModalCancelInvoiceController', ['$scope', '$rootScope', '$timeout', '$location', 'CommonFactory', function ($scope, $rootScope, $timeout,$location, CommonFactory) {
    var url = '/Invoice/';

    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var today = dd + '/' + mm + '/' + yyyy;

    $rootScope.ModalCancelInvoice = function (item) {
        $scope.CancelInvoice = new Object();
        $timeout(function () {
            angular.copy(item, $scope.CancelInvoice);
            $scope.CancelInvoice.INVOICETYPE = 3;
            $scope.CancelInvoice.STRCANCELTIME = today;
        }, 100);
    }

    $scope.CancelInv = function () {
        var number = $scope.CancelInvoice.NUMBER;
        var formCode = $scope.CancelInvoice.FORMCODE;
        var symbolCode = $scope.CancelInvoice.SYMBOLCODE;
        if ($scope.CancelInvoice.STRCANCELTIME === undefined || $scope.CancelInvoice.STRCANCELTIME === null || $scope.CancelInvoice.STRCANCELTIME.trim() === '') {
            alert("Vui lòng nhập vào ngày hủy.");
            return;
        }
        if (!$scope.CancelInvoice.CANCELREASON || $scope.CancelInvoice.CANCELREASON == " ") {
            alert("Vui lòng nhập lý do hủy.");
            return false;
        }
        if ($scope.CancelInvoice.AttachEmail === true) {
            //if (!$scope.CancelInvoice.RECIEVERNAME) {
            //    alert('Vui nhập vào tên người nhận!');
            //    return false;
            //}
            if (!$scope.CancelInvoice.CUSEMAIL) {
                alert('Vui nhập vào email người nhận!!');
                return false;
            }
            var lstEmail = $scope.CancelInvoice.CUSEMAIL.split(',');
            for (var i = 0; i < lstEmail.length; i++) {
                if (!validation.isEmailAddress(lstEmail[i].trim())) {
                    alert('Vui lòng nhập đúng định dạng email ' + (i + 1));
                    return false;
                }
            }
        }
        var confirmContinue = function (result) {
            if (!result)
                return false;
            var action = url + 'UpdateCancelInvoice';
            var datasend = JSON.stringify({
                invoice: $scope.CancelInvoice,
                type: 0
            }); LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        var confirmContinue = function (result) {
                            if (!result) {
                                return false;
                            }
                            else {
                                if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                                    $rootScope.ModalReceipt($scope.CancelInvoice, 6);
                                    $('.modal-receipt').modal('show');
                                }
                                else {
                                    $rootScope.ModalInvoice($scope.CancelInvoice, 6);
                                    $('.modal-invoice').modal('show');
                                }
                            }
                        };
                        confirm('Xóa bỏ hóa đơn thành công. Bạn có muốn lập hóa đơn thay thế cho hóa đơn này hay không?', 'Lập hóa đơn thay thế', 'Không', 'Có', confirmContinue);


                        $('.modal-cancel-invoice').modal('hide');
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ReloadReceipt();
                        }
                        else {
                            $rootScope.ReloadInvoice();
                        }
                    }
                    else {
                        $scope.ErrorMessage = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - CancelInv');
                }
                LoadingHide();
            });
        };
        confirm('Sau khi hủy bỏ hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> thì hóa đơn này sẽ không còn giá trị sử dụng, bạn không thể khôi phục được trạng thái ban đầu của hóa đơn và bạn phải lập hóa đơn thay thế.<br /> Bạn có thực sự muốn <strong>hủy bỏ</strong> hóa đơn này không?', 'Hủy bỏ hóa đơn', 'Không', 'Đồng ý', confirmContinue);
    }
}]);
app.controller('ModalChangeInvoiceController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';

    $rootScope.ModalChangeInvoice = function (item, type) {
        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = new Array();
        $scope.Invoice.LISTPRODUCT.push(new Object());
        if (type == 5) {
            angular.copy(item, $scope.ChangeInvoice);
            $scope.GetInvoiceDetail(item.ID);
            $scope.ChangeInvoice.ID = 0;
        }
    }

    $scope.GetInvoiceDetail = function (invoiceid) {
        $scope.IsLoading = true;
        var action = url + 'GetInvoiceDetail';
        var datasend = JSON.stringify({
            invoiceid: invoiceid
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.LISTPRODUCT = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoiceDetail');
            }
            LoadingHide();
        });
    }

}]);
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
app.controller('InvoiceWaitController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$location', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $location) {
    var url = '/Invoice/';

    angular.element(function () {
        //$rootScope.GetPaymentStatus();
        //$rootScope.GetInvoiceStatus();
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
    });

    $scope.TabWaiting = {
        TabWaitingNumber: true,
        TabInvoice: false
    };

    $scope.CurrentNumberObject = new Object();
    $rootScope.TabClick = function (xxx, item) {
        if (item) {
            $scope.NumberWaitingId = item.ID;
            $scope.CurrentNumberObject = item;
        }
        var listNumber = [];
        var listUsedNumber = [];
        var xx = [];
        if ($scope.CurrentNumberObject.USEDNUMBER != null) {
            xx = $scope.CurrentNumberObject.USEDNUMBER.split(',');
            for (let i = 0; i < xx.length; i++) {
                listUsedNumber.push(parseInt(xx[i]));
            }
        }
        for (let i = $scope.CurrentNumberObject.FROMNUMBER; i <= $scope.CurrentNumberObject.TONUMBER; i++) {
            listNumber.push(i);
        }
        var filteredArray = listNumber.filter(function (x) {
            return listUsedNumber.indexOf(x) < 0;
        });
        $scope.LISTUSEDNUMBER = listUsedNumber.sort(function (a, b) { return a - b; });
        $scope.LISTNUMBER = filteredArray.sort(function (a, b) { return a - b; });

        if (listUsedNumber.length == listNumber.length) {

        }

        $scope.TabWaiting.TabWaitingNumber = false;
        $scope.TabWaiting.TabInvoice = false;
        $scope.Filter = new Object();
        $scope.ListInvoice = [];
        switch (xxx) {
            case 1:
                $scope.TabWaiting.TabWaitingNumber = true;
                $scope.GetInvoiceWaiting(1);
                break;
            case 2:
                $scope.TabWaiting.TabInvoice = true;
                $scope.Filter = new Object();
                $scope.Filter.INVOICESTATUS = 1;
                $scope.Filter.FORMCODE = item.FORMCODE;
                $scope.Filter.SYMBOLCODE = item.SYMBOLCODE;
                $scope.GetInvoice(1);
                break;
            default:
                $scope.TabWaiting.TabWaitingNumber = true;
                break;
        }
    }

    $rootScope.ReloadWaitingInvoice = function (formcode, symbolcode) {
        if ($location.path().toString().includes('/dai-hoa-don-cho')) {
            $scope.Filter = new Object();
            $scope.ListInvoice = [];
            $scope.Filter.INVOICESTATUS = 1;
            $scope.Filter.FORMCODE = formcode;
            $scope.Filter.SYMBOLCODE = symbolcode;
            $scope.GetInvoice(1);
        }
    }


    $scope.SignWaiting = function (invoiceId, signTime, waitingNumber) {
        var dateReg = /^\d{2}[./-]\d{2}[./-]\d{4}$/

        if (!waitingNumber) {
            alert('Vui lòng nhập vào số muốn ký.');
            return false;
        }
        if (parseInt(waitingNumber) <= 0) {
            alert('Vui lòng nhập số lớn hơn 0.');
            return false;
        }
        if (parseInt(waitingNumber) < parseInt($scope.CurrentNumberObject.FROMNUMBER)) {
            alert('Số hóa đơn không nhỏ hơn số bắt đầu.');
            return false;
        }
        if (parseInt(waitingNumber) > parseInt($scope.CurrentNumberObject.TONUMBER)) {
            alert('Số hóa đơn không lớn hơn số đến.');
            return false;
        }
        if (!signTime) {
            alert('Vui lòng nhập vào ngày muốn ký.');
            return false;
        }
        if (!signTime.match(dateReg)) {
            alert('Vui lòng nhập đúng định dạng ngày dd/mm/yyyy.');
            return false;
        }
        if ($scope.CurrentNumberObject.USEDNUMBER !== null) {
            var find = $scope.CurrentNumberObject.USEDNUMBER.split(',').filter(function (x) {
                return x == waitingNumber;
            })
            if (find.length > 0) {
                alert('Số hóa đơn ' + waitingNumber + ' đã được sử dụng.');
                return false;
            }
        }

        LoadingShow();
        SignWaiting(invoiceId, signTime, $scope.NumberWaitingId, waitingNumber);
    }

    $scope.GetNumber = function (intpage) {
        $scope.FormNumber = new Object();

        $scope.FormNumber.FORMCODE = $scope.Number.FORMCODE;
        $scope.FormNumber.SYMBOLCODE = $scope.Number.SYMBOLCODE;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.data[0];
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.SetDatepiker = function (index) {
        let ctr = $('#pk_' + index);
        ctr.datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: new Date
        });
        SetVietNameInterface(ctr);
    }

    $scope.CheckDateOfPreviousInvoice = function (numberInvoice, item) {
        var ctr = $('#pk_' + item.ID);
        //var dateNow = new Date();
        var dateNow = null;
        var action = url + 'CheckDateOfPreviousInvoice';
        var min_date = "";
        var max_date = "";
        var datasend = JSON.stringify({
            form: item,
            number: parseInt(numberInvoice),
        });
        if (numberInvoice != null && numberInvoice != "") {
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        var minInvoice = response.result.filter((item) => {
                            return item.NUMBER < numberInvoice;
                        });
                        var maxInvoice = response.result.filter((item) => {
                            return item.NUMBER > numberInvoice;
                        });
                        min_date = (minInvoice.length === 0) ? null : new Date(minInvoice[0].SIGNEDTIME.match(/\d+/)[0] * 1);
                        ctr.datepicker("option", 'minDate', min_date);
                        max_date = (maxInvoice.length === 0) ? dateNow : new Date(maxInvoice[0].SIGNEDTIME.match(/\d+/)[0] * 1);
                        ctr.datepicker("option", 'maxDate', max_date);

                        ctr.datepicker({
                            dateFormat: 'dd/mm/yy',
                            mindate: min_date,
                            maxdate: max_date
                        });
                    } else {
                        ctr.datepicker("option", 'maxDate', dateNow);
                        ctr.datepicker({
                            dateFormat: 'dd/mm/yy',
                            maxDate: dateNow
                        });
                        $scope.ErrorMessage = response.msg;
                    }
                    ctr.datepicker("option", 'minDate', min_date);
                    ctr.datepicker("option", 'maxDate', max_date);
                    SetVietNameInterface(ctr);
                    ctr.datepicker('show');
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - CheckDateOfPreviousInvoice');
                }
            });
        }
    }

    $scope.GetInvoice = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }

        $scope.currentpage = intpage;
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: 10
        });
        $scope.ListInvoice = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $rootScope.GetInvoiceWaiting = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentRow = 10;
        $scope.currentpage = intpage;
        var action = url + 'GetInvoiceNumerWaiting';
        var datasend = JSON.stringify({
            itemPerPage: $scope.currentRow,
            currentPage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    response.result.forEach(function (numberItem) {
                        numberItem.NUMBERSTATUS = $scope.ENUM_WAITINGNUMBERSTATUS.CON_SO;
                        let listNumber = [];
                        let listUsedNumber = [];
                        let listAvailableNumber = [];
                        let tempArray = [];
                        if (numberItem.USEDNUMBER != null) {
                            tempArray = numberItem.USEDNUMBER.split(',');
                            for (let i = 0; i < tempArray.length; i++) {
                                listUsedNumber.push(parseInt(tempArray[i]));
                            }
                            numberItem.USEDNUMBER = listUsedNumber.sort(function (a, b) { return a - b; }).join(', ');
                        }
                        for (let i = numberItem.FROMNUMBER; i <= numberItem.TONUMBER; i++) {
                            listNumber.push(i);
                        }

                        if (listUsedNumber.length == listNumber.length) {
                            numberItem.NUMBERSTATUS = $scope.ENUM_WAITINGNUMBERSTATUS.HET_SO;
                        }

                        listAvailableNumber = listNumber.filter(function (e) {
                            return !listUsedNumber.includes(e);
                        })
                        numberItem.AVAILABLENUMBER = listAvailableNumber.join(', ');
                    });

                    $scope.ListInvoiceWaiting = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    //Init time
    var now = new Date();
    var firstDay = new Date();
    var lastDay = new Date();
    var currentDay = now.getDay();

    // Sunday - Saturday : 0 - 6
    //This week
    firstDay.setDate(now.getDate() - currentDay);
    lastDay.setDate(firstDay.getDate() + 6);
    var thisWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last week
    firstDay.setDate(firstDay.getDate() - 7);
    lastDay.setDate(lastDay.getDate() - 7);
    var lastWeek = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //This month
    var dayOfMonth = new Date(now.getFullYear(), now.getMonth(), 0).getDate();
    firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    lastDay = new Date(now.getFullYear(), now.getMonth(), (dayOfMonth - 1));
    var thisMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();

    //Last month
    lastDay.setDate(firstDay.getDate() - 1);
    firstDay = new Date(lastDay.getFullYear(), lastDay.getMonth(), 1);
    var lastMonth = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + firstDay.getDate() + ';' + lastDay.getFullYear() + '-' + (lastDay.getMonth() + 1) + '-' + lastDay.getDate();


    $scope.Timepickers = {
        value: thisWeek,    //Default value
        Options: [
            { value: thisWeek, text: 'Tuần này' },
            { value: lastWeek, text: 'Tuần trước' },
            { value: thisMonth, text: 'Tháng này' },
            { value: lastMonth, text: 'Tháng trước' }
        ]
    };
    //End init time

    $scope.ENUM_WAITINGNUMBERSTATUS = {
        CON_SO: 'Còn số',
        HET_SO: 'Hết số'
    };

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });    

    $(document).on("keyup", ".number-waitting", function (e) {
        if (e.keyCode == 69) {
            $(".number-waitting").val("");
        }
    })

}]);
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
app.controller('ModalCancelReportController', ['$scope', '$rootScope', '$timeout', '$location', 'CommonFactory', '$filter', function ($scope, $rootScope, $timeout, $location, CommonFactory, $filter) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = String(now.getDate()).padStart(2, '0');
    $scope.CurrentMonth = String(now.getMonth() + 1).padStart(2, '0'); //January is 0!
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = $scope.CurrentDay + "/" + $scope.CurrentMonth + "/" + $scope.CurrentYear;

    $rootScope.ModalCancelReport = function (item1, item) {
        $scope.Invoice = new Object();
        $scope.Report = new Object();
        $timeout(function () {
            angular.copy(item1, $scope.Report);
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.STRCANCELTIME = $filter('dateFormat')($scope.Invoice.CANCELTIME, 'dd/MM/yyyy');
            if ($scope.Invoice.CANCELTIME === "/Date(-62135596800000)/") {
                $scope.Invoice.STRCANCELTIME = $scope.CurrentDate;
            }
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === REPORT_TYPE.Cancel) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Cancel;
        }

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do hủy!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }

        var action = url + 'AddReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CANCELREASON = $scope.Report.REASON
                    $rootScope.ModalCancelInvoice($scope.Invoice);
                    toastr.success(response.msg);
                    if ($scope.Invoice.INVOICETYPE != 3) {
                        $('.modal-cancel-invoice').modal('show')
                    }
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }

    $scope.UpdateReport = function () {
        $scope.Report.REPORTTYPE = $scope.Invoice.ISEXISTCANCELREPORT;
        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do hủy!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }

        LoadingShow();
        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateCancelReport');
            }
            LoadingHide();
        });
    }

    /*
     * Lưu và ký biên bản hủy hóa đơn 
     * truongnv 20200218
     * @param {any} type: Loại biên bản
     */
    $scope.SaveAndSignDocumentPDF = function (type) {
        var msg = "Vui lòng nhập lý do hủy.";
        if (type === REPORT_TYPE.Cancel) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Cancel;
        }
        else
            msg = "Vui lòng nhập lý do điều chỉnh.";

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CANCELREASON;
        if (!$scope.Report.REASON) {
            alert(msg);
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Invoice.STRCANCELTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản hủy.");
            return;
        }
        var action = url + 'SaveAndSignDocumentPDF';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CANCELREASON = $scope.Report.REASON
                    $rootScope.ModalCancelInvoice($scope.Invoice);
                    toastr.success(response.msg);
                    if ($scope.Invoice.INVOICETYPE != 3) {
                        $('.modal-cancel-invoice').modal('show')
                    }
                    $('.modal-cancel-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }
}]);
app.controller('ModalModifiedReportController', ['$scope', '$rootScope', '$timeout', "$location", 'CommonFactory', '$filter', function ($scope, $rootScope, $timeout, $location, CommonFactory, $filter) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = String(now.getDate()).padStart(2, '0');
    $scope.CurrentMonth = String(now.getMonth() + 1).padStart(2, '0'); //January is 0!
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = $scope.CurrentDay + "/" + $scope.CurrentMonth + "/" + $scope.CurrentYear;

    $rootScope.ModalModifiedReport = function (item1, item) {
        $scope.Invoice = new Object();
        $scope.Report = new Object();
        $timeout(function () {
            angular.copy(item, $scope.Invoice);
            angular.copy(item1, $scope.Report);
            //if (item.IDTEMP) {
            //    $scope.Invoice.ID = item.IDTEMP;
            //}
            $scope.Report.REPORTTIME = $scope.CurrentDate;
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === REPORT_TYPE.Change) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Change;
        }

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do điều chỉnh!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
            return;
        }

        var action = url + 'AddReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    $('.modal-modified-report').modal('hide');
                    if ($scope.Invoice.INVOICETYPE != 5) {
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ModalReceipt($scope.Invoice, 5);
                        }
                        else {
                            $rootScope.ModalInvoice($scope.Invoice, 5);
                        }
                        
                    }
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }

    $scope.UpdateReport = function () {
        $scope.Report.REPORTTYPE = $scope.Invoice.ISEXISTMODIFIEDREPORT;
        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập lý do điều chỉnh!");
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
            return;
        }

        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(response.msg);
                    $('.modal-modified-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateModifiedReport');
                LoadingHide();
            }
            LoadingHide();
        });
    }

    /*
     * Lưu và ký biên bản hủy hóa đơn 
     * truongnv 20200218
     * @param {any} type: Loại biên bản
     */
    $scope.SaveAndSignDocumentPDF = function (type) {
        var msg = "Vui lòng nhập lý do điều chỉnh.";
        if (type === REPORT_TYPE.Change) {
            $scope.Report.REPORTTYPE = REPORT_TYPE.Change;
        }
        else
            msg = "Vui lòng nhập lý do hủy.";

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert(msg);
            return false;
        }
        $scope.Report.STRREPORTTIME = $scope.Report.REPORTTIME;
        if ($scope.Report.STRREPORTTIME === undefined || $scope.Report.STRREPORTTIME === null || $scope.Report.STRREPORTTIME === '') {
            alert("Vui lòng nhập vào ngày lập biên bản điều chỉnh.");
            return;
        }

        var action = url + 'SaveAndSignDocumentPDF';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    if ($scope.Invoice.INVOICETYPE != 5) {
                        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                            $rootScope.ModalReceipt($scope.Invoice, 5);
                        }
                        else {
                            $rootScope.ModalInvoice($scope.Invoice, 5);
                        }
                    }
                    toastr.success(response.msg);
                    $('.modal-modified-report').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                    else {
                        $rootScope.ReloadInvoice();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }
}]);
app.controller('ModalReleaseDocumentController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Invoice/';

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

    $rootScope.ModalReleaseDocumentReport = function (item) {
        $scope.Invoice = new Object();
        $timeout(function () {
            angular.copy(item, $scope.Invoice);
            angular.copy(item1, $scope.Report);
            //if (item.IDTEMP) {
            //    $scope.Invoice.ID = item.IDTEMP;
            //}
        }, 100);
    }

    $scope.AddReport = function (type) {
        if (type === 2) {
            $scope.Report.REPORTTYPE = '2';
        }

        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập tên cơ quan thuế tiếp nhận thông báo!");
            return false;
        }

        var action = url + 'AddReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.Invoice.CHANGEREASON = $scope.Report.REASON
                    $rootScope.ModalInvoice($scope.Invoice, 5);
                    $('.modal-invoice').modal('show');

                    $('.modal-modified-report').modal('hide');
                    $rootScope.ReloadInvoice();
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddReport');
            }
            LoadingHide();
        });
    }

    $scope.UpdateReport = function () {
        $scope.Report.REPORTTYPE = $scope.Invoice.ISEXISTMODIFIEDREPORT;
        $scope.Report.INVOICEID = $scope.Invoice.ID;
        if (!$scope.Report.COMPHONENUMBER)
            $scope.Report.COMPHONENUMBER = $scope.Enterprise.COMPHONENUMBER;
        if (!$scope.Report.COMLEGALNAME)
            $scope.Report.COMLEGALNAME = $scope.Enterprise.COMLEGALNAME;
        if (!$scope.Report.CUSADDRESS)
            $scope.Report.CUSADDRESS = $scope.Invoice.CUSADDRESS;
        if (!$scope.Report.CUSTAXCODE)
            $scope.Report.CUSTAXCODE = $scope.Invoice.CUSTAXCODE;
        if (!$scope.Report.CUSPHONENUMBER)
            $scope.Report.CUSPHONENUMBER = $scope.Invoice.CUSPHONENUMBER;
        if (!$scope.Report.CUSDELEGATE)
            $scope.Report.CUSDELEGATE = $scope.Invoice.CUSBUYER;
        if (!$scope.Report.REASON)
            $scope.Report.REASON = $scope.Invoice.CHANGEREASON;
        if (!$scope.Report.REASON) {
            alert("Vui lòng nhập tên cơ quan thuế tiếp nhận thông báo!");
            return false;
        }

        var action = url + 'UpdateReport';
        var datasend = JSON.stringify({
            report: $scope.Report
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!')
                    $('.modal-modified-report').modal('hide');
                    $rootScope.ReloadInvoice();
                } else {
                    alert(response.msg);
                }
                LoadingHide();
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateModifiedReport');
                LoadingHide();
            }
            LoadingHide();
        });
    }
}]);
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
app.controller('ReceiptController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', '$location', '$window', function ($scope, $rootScope, $timeout, CommonFactory, $location, $window) {
    var url = '/Receipt/';
    angular.element(function () {
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetInvoiceStatus();
        //$rootScope.GetPaymentStatus();
        $scope.LoadDashboard();
    });
    //========================== Cookie's Own ============================
    $scope.LoadCookie_Receipt = function () {
        var check = getCookie('Novaon_ReceiptManagement');
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldBillDate: true,
                FieldBillType: true,
                FieldCustomer: true,
                FieldFormCode: true,
                FieldSymbolCode: true,
                FieldNumber: true,
                FieldInvoiceStatus: true,
                FieldReferenceCode: true,
                FieldPaymentStatus: false,
                FieldTotalPayment: true,
                FieldCustomerID: true,
                RowNum: 10
            }
            setCookie('Novaon_ReceiptManagement', JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie('Novaon_ReceiptManagement', JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetReceiptPaging($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.Tab = {
        All: true,
        Cancel: false,
        Tranfer: false,
        Change: false,
        Replace: false
    };

    $rootScope.ReceiptTabClick = function (tabIndex) {
        $scope.Tab.All = false;
        $scope.Tab.Cancel = false;
        $scope.Tab.Tranfer = false;
        $scope.Tab.Change = false;
        $scope.Tab.Replace = false;
        $scope.Filter = new Object();
        $scope.ListReceipt = [];
        switch (tabIndex) {
            case 1:
                $scope.Tab.All = true;
                $scope.Filter = new Object();
                $scope.GetReceiptPaging(1);
                break;
            case 2:
                $scope.Tab.Cancel = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            case 3:
                $scope.Tab.Tranfer = true;
                $scope.Filter.INVOICETYPE = 4;
                $scope.GetReceiptPaging(1);
                break;
            case 4:
                $scope.Tab.Change = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Modifield_Invoice, 1, REPORT_TYPE.Change);
                break;
            case 5:
                $scope.Tab.Replace = true;
                $scope.Filter = new Object();
                $scope.GetReceiptByStatus(INVOICE_TYPE.Replace_Invoice, 1, REPORT_TYPE.Cancel);
                break;
            default:
                $scope.Tab.All = true;
                break;
        }
        $('.dropdown-menu').removeClass('show');
    }

    $rootScope.ReloadReceipt = function () {
        if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
            if ($scope.Tab.Cancel) {
                $scope.GetReceiptByStatus(INVOICE_TYPE.Cancel_Invoice, 1, REPORT_TYPE.Cancel);
            }
            else {
                $scope.ListReceipt = new Array();
                $scope.GetReceiptPaging($scope.currentpage);
            }
        }
    }

    $scope.GetReceiptPaging = function (intpage) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        if ($scope.Filter.TIME == '5') {
            $scope.Filter.TIME = $scope.FROMTIME + ';' + $scope.TOTIME;
            if (!$scope.FROMTIME || !$scope.TOTIME) {
                $scope.Filter.TIME = $scope.Timepickers.Options[0].value;
            }
        }
        $scope.LoadCookie_Receipt();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetReceiptPaging';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: $scope.currentpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListReceipt = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListReceipt = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;

                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReceiptPaging');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'INVOICESTATUS') {
                    var f = $scope.ListReceiptStatus.filter(function (item) {
                        return item.INVOICESTATUSID == $scope.Filter[prop];
                    });
                    o.value = f[0].INVOICESTATUS;
                }

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    if (f.length > 0) {
                        o.value = f[0].TIME;
                    }
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'PAYMENTSTATUS') {
                    var f = $scope.ListPaymentStatus.filter(function (item) {
                        return item.PAYMENTSTATUS == $scope.Filter[prop];
                    });
                    o.value = f[0].PAYMENTSTATUS;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    /**
     * truongnv 2020-02-14
     * Gán data filter khi người dùng chọn
     * @param {any} type
     * @param {any} intpage
     */
    $scope.GetReceiptByStatus = function (type, intpage, reportType) {
        if (!$scope.Filter)
            $scope.Filter = new Object();
        $scope.LoadCookie_Receipt();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'GetReceiptByStatus';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            type: type,
            intpage: intpage,
            reportType: reportType
        });
        $scope.ListReceipt = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListReceipt = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
        /*
         *Gán data filter
         * */
        $scope.FilterApply = [];
        for (var prop in $scope.Filter) {
            if ($scope.Filter[prop] != null) {
                var o = {
                    key: prop,
                    value: $scope.Filter[prop]
                };

                if (prop == 'TIME') {
                    var f = $scope.Timepickers.Options.filter(function (item) {
                        return item.value == $scope.Filter[prop];
                    });
                    o.value = f[0].TIME;
                }

                if (prop == 'FORMCODE') {
                    var f = $scope.ListFormCode.filter(function (item) {
                        return item.FORMCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].FORMCODE;
                }

                if (prop == 'SYMBOLCODE') {
                    var f = $scope.ListSymbolCode.filter(function (item) {
                        return item.SYMBOLCODE == $scope.Filter[prop];
                    });
                    o.value = f[0].SYMBOLCODE;
                }

                $scope.FilterApply.push(o);
            }
        }
    }

    $scope.RemoveFilter = function (f) {
        for (var prop in $scope.Filter) {
            if (prop == f.key) {
                $scope.Filter[prop] = null;
                break;
            }
        }

        //Refresh filter
        $rootScope.ReloadReceipt();
    };

    $scope.SeleteRow = function (list, item, isselect) {
        var find = list.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (item)
            isselect = false;
        else {
            if (find.length == list.length - 1) {
                isselect = true;
            }
        }
    }

    $scope.Sign = function (idInvoice) {
        LoadingShow();
        Sign(idInvoice);
    }

    $scope.LoadDashboard = function () {
        var action = url + 'LoadDashboard';
        var datasend = JSON.stringify({
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.TotalMoneyNotPay = response.TotalMoneyNotPay;
                    $scope.TotalMoneyPaied = response.TotalMoneyPaied;
                    $scope.TotalMoneyNotApproval = response.TotalMoneyNotApproval;
                    $scope.TotalInvoiceSigned = response.totalInvoiceSigned;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetProvince');
            }
        });
    };

    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });

    $scope.PreviewReferenceInvoice = function (item) {
        window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
    }

    $rootScope.PreviewInvoice = function (item, isSignLink) {
        //Nếu chưa có file thì tạo file
        if (isSignLink === false) {
            var action = url + 'CreateFilePdfToView';
            var datasend = JSON.stringify({
                invoiceId: item
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    window.open('NOVAON_FOLDER' + response.msg + "?v=" + new Date().getTime());
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - PreviewInvoice');
                }
                LoadingHide();
            });
        }
        else { // ngược lại view file ra
            window.open('NOVAON_FOLDER' + item + "?v=" + new Date().getTime());
        }
    }

    $scope.PreviewModifiedReport = function (item) {
        window.open('NOVAON_FOLDER' + item.MODIFIEDLINK + "?v=" + new Date().getTime());
    }

    $scope.PreviewCancelReport = function (item) {
        window.open('NOVAON_FOLDER' + item.CANCELLINK + "?v=" + new Date().getTime());
    }

    $scope.ConvertInv = function (item) {
        var result = confirm('Sau khi chuyển sang hóa đơn chuyển đổi. Hóa đơn chỉ được phép in một lần duy nhất. Bạn có thực sự muốn chuyển đổi hóa đơn ?');
        if (result) {
            $scope.IsLoading = true;
            var action = url + 'UpdateConvertInvoice';
            var datasend = JSON.stringify({
                invoice: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Thành công!')
                        $rootScope.ReloadReceipt();
                    } else {
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js)');
                }
            });
            $scope.IsLoading = false;
        }
    }

    $rootScope.ReloadGetNumber = function () {
        $scope.GetNumber($scope.FormNumber.CURRENTPAGE);
    }

    $scope.GetNumber = function (intpage) {
        $scope.FormNumber = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.FormNumber.ITEMPERPAGE = 10;
        $scope.FormNumber.CURRENTPAGE = intpage;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        $scope.ListNumber = new Array();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListNumber = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumber');
            }
            LoadingHide();
        });
    }

    $scope.SendEmail = function (item) {
        $scope.IsLoading = true;
        var action = url + 'SendEmail';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    /*
     * Đính kèm file khi gửi mail cho khách hàng
     * @param {any} event
     */
    $scope.uploadFile = function (event) {
        LoadingShow();
        $('#div-file-active').show();
        var files = event.target.files;
        var isPass = CheckFile(files);
        if (isPass) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                var reader = new FileReader();
                reader.onloadend = function (event) {
                    $timeout(function () {
                        var base64File = event.target.result.split(',')[1];
                        $('#file-selected').append('<span class="has-tag-attment-file" name = "' + file.name + '" rel= "' + base64File + '" onclick="DeleteFile(this)">' + file.name + '<img src="/Images/close.png"></span>');
                        LoadingHide();
                    }, 100);
                };
                reader.readAsDataURL(file);
            }
        }
        else
            LoadingHide();
    }

    $scope.ConfirmChange = function (item) {
        if (item.IDTEMP > 0) {
            var confirmContinue = function (result) {
                if (!result)
                    return false;
                window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
            };
            confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
            return false;
        }
        $rootScope.ModalReceipt(item, 2);
        $('.modal-receipt').modal('show');
    }

    $scope.CancelInvoiceConfirm = function (item) {
        if (item.CANCELREASON != null && item.CANCELREASON.trim() != "") {
            $rootScope.ModalCancelInvoice(item);
            $('.modal-cancel-invoice').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
                else {
                    $rootScope.IsUpdateCancelReport = false;
                    $rootScope.ModalCancelReport(null, item);
                    $('.modal-cancel-report').modal('show');
                }
            };
            confirm('Bạn có muốn lập biên bản hủy cho hóa đơn này không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    $scope.ReplaceInvoiceConfirm = function (item) {
        var number = item.NUMBER;
        var formCode = item.FORMCODE;
        var symbolCode = item.SYMBOLCODE;
        if (item.INVOICETYPE == 3) {
            if (item.IDTEMP > 0 && item.IDTEMP > item.ID) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> đã được lập hóa đơn thay thế <strong><' + formCode + ' - ' + symbolCode + ' - ' + 'Chưa cấp số></strong>. Bạn có muốn xem không?', 'Hóa đơn thay thế', 'Không', 'Có', confirmContinue);
            }
            else {
                $rootScope.ModalReceipt(item, 6);
                $('.modal-receipt').modal('show');
            }
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    return false;
                }
                else {
                    $rootScope.ModalCancelInvoice(item);
                    $('.modal-cancel-invoice').modal('show');
                }
            };
            confirm('Để lập được hóa đơn thay thế cho hóa đơn <strong><' + formCode + ' - ' + symbolCode + ' - ' + ('0000000' + number).slice(-7) + '></strong> bạn cần thực hiện xóa bỏ hóa đơn này trước. Bạn có muốn thực hiện xóa bỏ hóa đơn không?', 'Hóa đơn xóa bỏ', 'Không', 'Có', confirmContinue);
        }
    }

    /* TuyenNV - 20200303
     Kiểm tra xem hóa đơn bị điều chỉnh đã có biên bản/ hóa đơn điểu chỉnh
     + Chưa có biên bản => open modal tạo biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản => tạo hóa đơn điều chỉnh
     + Có biên bản, có hóa đơn điều chỉnh => view hóa đơn điều chỉnh
     */
    $scope.ModifiedInvoiceConfirm = function (item) {
        if (item.CHANGEREASON != null && item.CHANGEREASON.trim() != "") {
            if (item.IDTEMP > 0) {
                var confirmContinue = function (result) {
                    if (!result)
                        return false;
                    window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                };
                confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                return false;
            }
            $rootScope.ModalReceipt(item, 5);
            $('.modal-receipt').modal('show');
        }
        else {
            var confirmContinue = function (result) {
                if (!result) {
                    if (item.IDTEMP > 0) {
                        var confirmContinue = function (result) {
                            if (!result)
                                return false;
                            window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                        };
                        confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                        return false;
                    }
                    $rootScope.ModalReceipt(item, 5);
                    $('.modal-receipt').modal('show');
                }
                else {
                    $rootScope.IsUpdateModifiedReport = false;
                    $rootScope.ModalModifiedReport(null, item);
                    $('.modal-modified-report').modal('show');
                }
            };

            if (item.ISEXISTMODIFIEDREPORT != '1') {
                confirm('Bạn có muốn lập biên bản điều chỉnh cho hóa đơn này không?', 'Hóa đơn điểu chỉnh', 'Không', 'Có', confirmContinue);
            }
            else {
                if (item.IDTEMP > 0) {
                    var confirmContinue = function (result) {
                        if (!result)
                            return false;
                        window.open('NOVAON_FOLDER' + item.SIGNLINKTEMP);
                    };
                    confirm('Hóa đơn này đã được lập hóa đơn điều chỉnh. Bạn có muốn xem hóa đơn điều chỉnh đã lập không?', 'Hóa đơn điều chỉnh', 'Không', 'Xem hóa đơn', confirmContinue);
                    return false;
                }
                $rootScope.ModalReceipt(item, 5);
                $('.modal-receipt').modal('show');
            }
        }
    }
    // End

    $scope.OpenModalCancelReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTCANCELREPORT == '1') {
            $rootScope.IsUpdateCancelReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTCANCELREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    LoadingHide
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateCancelReport = false;
            $timeout(function () {
                $rootScope.ModalCancelReport($scope.Report, item);
                $('.modal-cancel-report').modal('show');
            }, 200)
        }
    }

    $scope.OpenModalModifiedReport = function (item) {
        $scope.Report = new Object();
        if (item.ISEXISTMODIFIEDREPORT == '2') {
            $rootScope.IsUpdateModifiedReport = true;
            var action = url + 'GetReportByInvoiceIdReportType';
            var datasend = JSON.stringify({
                invoiceId: item.ID,
                reportType: item.ISEXISTMODIFIEDREPORT
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (data) {
                if (data) {
                    LoadingHide();
                    if (data.rs) {
                        $scope.Report = data.result;
                    } else {
                        alert(data.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetReportByInvoiceIdReportType');
                }
                LoadingHide();
            });
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
        else {
            $rootScope.IsUpdateModifiedReport = false;
            $timeout(function () {
                $rootScope.ModalModifiedReport($scope.Report, item);
                $('.modal-modified-report').modal('show');
            }, 200)
        }
    }

    $scope.DownloadReleaseDocument = function (item) {
        var action = url + 'DownloadReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadIssuedDocument = function (item) {
        var action = url + 'DownloadIssuedReleaseDocument';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.DownloadReleaseInvoiceTemplate = function (item) {
        var action = url + 'DownloadReleaseInvoiceTemplate';
        var datasend = {
            numberBO: item
        };
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            prepareCallback: function () {
                LoadingShow();
            }
        });
        $timeout(function () {
            LoadingHide();
        }, 3000);
    }

    $scope.RemoveInvoice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListInvoiceChecked = [];
            if (item) {
                $scope.ListInvoiceChecked.push(item);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listInvoiceChecked = $scope.ListReceipt.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listInvoiceChecked && listInvoiceChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn hóa đơn cần xóa.");
                    return;
                }

                var listInvoiceSigned = $scope.ListReceipt.filter(function (obj) { return obj.INVOICESTATUS !== 1 && obj.ISSELECTED === true; });
                if (listInvoiceSigned && listInvoiceSigned.length > 0) {
                    toastr.warning("Bạn không được xóa hóa đơn ở trạng thái <b style='color: #222;'>Đã phát hành</b>.");
                    return;
                }

                if (listInvoiceChecked && Object.keys(listInvoiceChecked).length > 0) {
                    for (var i = 0; i < listInvoiceChecked.length; i++) {
                        $scope.ListInvoiceChecked.push(listInvoiceChecked[i].ID);
                    }
                }
            }

            var lstInvoiceid = $scope.ListInvoiceChecked.join(";");
            var action = url + 'RemoveInvoice';
            var datasend = JSON.stringify({
                idInvoices: lstInvoiceid
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadReceipt();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });

        };
        confirm("Bạn có thực sự muốn xóa các Hóa đơn đã chọn không?", "Thông báo", "Không", "Có", confirmContinue)
    }


    $scope.ModalSendEmail = function (item) {
        $('#modal_send_email').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            show: {
                effect: 'drop',
                direction: 'right',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            create: function (event, ui) {
                $('#modal_send_email').show();
            }
        });

        $('#file-selected').html('');

        $scope.GetEmailHistory(item.ID);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        $scope.Invoice = new Object();
        angular.copy(item, $scope.Invoice);

        //open 
        //$('#modal-send-email').dialog('open');
    }

    $scope.SendEmail = function () {
        //if (!$scope.Invoice.RECIEVERNAME) {
        //    alert('Vui nhập vào tên người nhận!');
        //    return false;
        //}
        if (!$scope.Invoice.RECIEVEREMAIL) {
            alert('Vui nhập vào email người nhận!!');
            return false;
        }
        var lstEmail = $scope.Invoice.RECIEVEREMAIL.split(',');
        for (var i = 0; i < lstEmail.length; i++) {
            if (!validation.isEmailAddress(lstEmail[i].trim())) {
                alert('Email bạn nhập không đúng định dạng email: ' + lstEmail[i+1]);
                return false;
            }
        }

        $scope.Invoice.CUSBUYER = $scope.Invoice.RECIEVERNAME;
        $scope.Invoice.CUSEMAIL = $scope.Invoice.RECIEVEREMAIL;

        /*
         * Lấy danh sách file đính kèm của người dùng khi gửi email
         */
        var fileNames = new Array();
        var objFileBase64 = new Array();
        var fFile = $('#file-selected .has-tag-attment-file');
        if (fFile.length > 0) {
            for (var i = 0; i < fFile.length; i++) {
                objFileBase64.push(fFile[i].getAttribute('rel'));
                fileNames.push(fFile[i].getAttribute('name'));
            }
        }
        var action = "";
        if ($scope.Invoice.INVOICETYPE === 3) {
            action = url + 'SendCancelEmail';
        }
        else {
            action = url + 'SendEmail';
        }
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            imgBase64: JSON.stringify(objFileBase64),
            fileNames: fileNames.join(";")

        });

        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                    $('#modal_send_email').dialog("close");
                    $rootScope.ReloadReceipt();
                } else {
                    $scope.ErrorMessage = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }

    $scope.CurrentItem = new Object();

    $scope.EditTemplate = function (item) {
        $rootScope.EditingTemplate = item;
        //var obj = {
        //    C: item.COMTAXCODE,
        //    F: item.FORMCODE,
        //    S: item.SYMBOLCODE,
        //    R: item.FROMNUMBER,
        //    O: item.TONUMBER,
        //    T: item.TEMPLATEPATH
        //};
        //Mã hóa đối tượng và truyền theo phương thức GET
        document.location.href = "/#/mau-hoa-don/" + btoa(item.TEMPLATEPATH);
    };

    $('#frmUploadFileReport').fileupload({
        autoUpload: false,
        add: function (e, data) {
            LoadingShow();
            var fileData = new FormData();
            var data = data.files[0];
            fileData.append('file0', data);
            fileData.append('currentItem', $scope.CurrentItem.ID);
            fileData.append('comTaxCode', $scope.CurrentItem.COMTAXCODE);

            var action = url + 'UploadFileReport';

            $.ajax({
                type: 'POST',
                url: action,
                contentType: false,
                processData: false,
                data: fileData,
                success: function (result) {
                    $timeout(function () {
                        if (result.rs) {
                            $scope.CurrentItem.ATTACHMENTFILELINK = result.id;
                            $rootScope.ReloadReceipt();
                            alert('Thành công');
                        } else {
                            alert(result.msg);
                        }
                        LoadingHide();
                    }, 10);
                },
                error: function (xhr, status, p3, p4) {
                    alert('Lỗi không thể tải lên file: ' + data.name);
                    LoadingHide();
                }
            });
        }
    });

    $scope.DownloadFileReport = function (item) {
        var action = url + 'DownloadFileReport';
        var datasend = {
            invoiceId: item.ID
        };

        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        });
    }

    $scope.RemoveFileReport = function (item) {
        var action = url + 'RemoveFileReport';
        var datasend = JSON.stringify({
            invoice: item
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(response.msg);
                    $rootScope.ReloadReceipt();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SendEmail');
            }
            LoadingHide();
        });
    }

    $scope.ExportExcel = function () {
        var check = getCookie('Novaon_ReceiptManagement');
        var action = url + 'ExportExcell';
        var datasend = {
            form: $scope.Filter,
            currentPage: 1,
            fieldsCookies: check
        };
        $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend
        })
    }

    $scope.GetEmailHistory = function (invoiceId) {
        $scope.ListEmailHistory = new Array();
        var action = url + 'GetEmailHistoryByInvoiceId';
        var datasend = JSON.stringify({
            id: invoiceId
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListEmailHistory = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetEmailHistory');
            }
            LoadingHide();
        });
    }
    LoadingHide();

    $scope.SetDatepikerFromTime = function () {
        $("#pk_fromtime").datepicker({
            dateFormat: 'yy-mm-dd',
            maxDate: new Date
        });
        SetVietNameInterface($("#pk_fromtime"));
    }

    $scope.SetDatepikerToTime = function () {
        var fromTime = $scope.FROMTIME;
        $("#pk_totime").datepicker("option", 'minDate', fromTime);
        $("#pk_totime").datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: new Date(fromTime)
        });
        SetVietNameInterface($("#pk_totime"));
    }

    /*
     *truongnv 20200317
     * Sắp sếp dữ liệu
     * */
    // column to sort
    $scope.column = 'NUMBER';

    // sort ordering (Ascending or Descending). Set true for desending
    $scope.reverse = false;

    // called on header click
    $scope.sortColumn = function (col) {
        $scope.column = col;
        if ($scope.reverse) {
            $scope.reverse = false;
            $scope.reverseclass = 'arrow-up_h';
        } else {
            $scope.reverse = true;
            $scope.reverseclass = 'arrow-down_h';
        }
    }

    // remove and change class
    $scope.sortClass = function (col) {
        if ($scope.column == col) {
            if ($scope.reverse) {
                return 'arrow-down_h';
            } else {
                return 'arrow-up_h';
            }
        } else {
            return '';
        }
    } 

    $window.ReloadInvoice = function () {
        $rootScope.ReloadReceipt();
    };
}]);
/**
 * Xóa đính kèm đã chọn
 * truongnv 20200219
 * @param {any} index : key
 */
function DeleteFile(index) {
    index.remove();
}

/**
 * Kiểm tra kiểu file và dung lượng file tải lên
 * truongnv 20200219
 * @param {any} files
 */
function CheckFile(files) {
    var arrFiles = new Array();
    var allowFileExtension = ".xls,.xlsx,.doc,.docx,.pdf, .txt, .xml";
    var arrAllowFileExtension = allowFileExtension.split(",");
    if (files.length > 0) {
        var sumSize = 0;
        var sumSizeAllow = 10 * 1024 * 1024;
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
                alert("File <b>" + files[i].name + "</b> có dung lượng <b>" + GetSizeName(fileSize) + "</b>. Bạn không được chọn file có dung lượng lớn hơn <b>10 MB</b>.");
                return false;
            }

            sumSize = sumSize + fileSize;
        }

        for (var i = 0; i < arrFiles.length; i++)
            sumSize = sumSize + arrFiles[i].size;

        if (sumSize > sumSizeAllow) {
            alert("Tổng dung lượng các file bạn đã chọn là <b>" + GetSizeName(sumSize) + "</b>. Bạn không được Attach file có tổng dung lượng lớn hơn <b>10 MB</b>.");
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
app.controller('ModalReceiptController', ['$scope', '$rootScope', '$timeout', '$sce', 'CommonFactory', '$filter', '$http', '$location', function ($scope, $rootScope, $timeout, $sce, CommonFactory, $filter, $http, $location) {
    var url = '/Receipt/';
    //type: 1: tạo mới, 2: xem chi tiết, 3: hủy bỏ, 4: chuyển đổi, 5: điều chỉnh, 6: thay thế, 7: dải chờ
    $rootScope.ModalReceipt = function (item, type, formcode, symbolcode, isCopy) {
        //$rootScope.GetFormCode();
        //$rootScope.GetSymbolCode();
        //$rootScope.GetPaymentMethod();

        $scope.TYPECHANGE = type;
        $scope.Invoice = new Object();
        $scope.Invoice.LISTPRODUCT = [];
        $scope.Invoice.INVOICETYPE = type;
        $scope.IsCopy = false;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
        $scope.Invoice.CURRENCY = "VND";
        $scope.Invoice.EXCHANGERATE = 1;

        $('.modal-receipt').modal('show');
        if (type == 1) {
            $scope.Invoice.ID = 0;
            $scope.Invoice.CUSPAYMENTMETHOD = "TM/CK";
            $timeout(function () {
                $scope.Invoice.FORMCODE = $rootScope.ListFormCode[0].FORMCODE;
                $scope.Invoice.SYMBOLCODE = $rootScope.ListSymbolCode[0].SYMBOLCODE;
                $scope.TAXRATE = $rootScope.ListSymbolCode[0].TAXRATE;
                $scope.Invoice.DISCOUNTTYPE = "KHONG_CO_CHIET_KHAU";
            });
            if (item) {
                angular.copy(item, $scope.Invoice);
                if (isCopy) {
                    $scope.Invoice.INVOICESTATUS = 1;
                    $scope.Invoice.NUMBER = 0;
                    $scope.Invoice.ID = 0;
                    $scope.Invoice.INVOICETYPE = type;
                    $scope.Invoice.REFERENCE = 0;
                    $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                    $scope.IsCopy = isCopy;
                }
            }
            else
                $scope.Invoice.NUMBER = 0;

            // check waiting invoice
            if (formcode && symbolcode) {
                $scope.Invoice.FORMCODE = formcode;
                $scope.Invoice.SYMBOLCODE = symbolcode;
                $scope.Invoice.NUMBER = 0;
                $scope.Invoice.INVOICESTATUS = 1;
                $scope.Invoice.INVOICETYPE = 1;
                $scope.Invoice.ISINVOICEWAITING = true;
            }
        } else if (type == 2) {
            var taxrate = $rootScope.ListSymbolCode.filter(function (x) {
                return x.SYMBOLCODE == item.SYMBOLCODE;
            })
            $scope.TAXRATE = taxrate[0].TAXRATE;
            item.EXCHANGERATE = item.EXCHANGERATE.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            if (item.INVOICETYPE == 5 || item.INVOICETYPE == 3 || item.INVOICETYPE == 6) {
                item.SIGNEDTIME = item.SIGNEDTIMETEMP;
            }
            angular.copy(item, $scope.Invoice);

        } else if (type == 5) {
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;

            $scope.Invoice.LISTPRODUCT = [];
        } else if (type == 6) {
            angular.copy(item, $scope.Invoice);
            $scope.Invoice.NUMBER = 0;
            $scope.Invoice.ID = 0;
            $scope.Invoice.INVOICESTATUS = 1;
            $scope.Invoice.INVOICETYPE = type;
            $scope.Invoice.REFERENCE = item.ID;
            $scope.Invoice.NUMBERTEMP = item.NUMBER;
        } else if (type == 7) {
        }
        $scope.Invoice.STRDUEDATE = $filter('dateFormat')($scope.Invoice.DUEDATE, 'dd/MM/yyyy');
        $scope.Invoice.STRINVOICEWAITINGTIME = $filter('dateFormat')($scope.Invoice.INVOICEWAITINGTIME, 'dd/MM/yyyy');
        ReadNumber($scope.Invoice.TOTALPAYMENT);
    }

    $scope.AddReceipt = function (type) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Bạn cần chọn mẫu số.');
            return false;
        }

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Bạn cần chọn ký hiệu.');
            return false;
        }
        
        if (!$scope.Invoice.CUSID) {
            alert('Bạn cần nhập vào thông tin <b>Mã học sinh</b>.');
            return false;
        }

        if (!$scope.Invoice.NOTE) {
            alert('Bạn cần nhập vào thông tin <b>Ghi chú</b>.');
            return false;
        }

        if (!$scope.Invoice.CUSPAYMENTMETHOD) {
            alert('Vui lòng chọn hình thức thanh toán');
            return false;
        }

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email');
                return;
            }
        }

        LoadingShow();
        var action = url + 'AddReceipt';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Thêm mới thành công!")
                    $('.modal-receipt').modal('hide');
                    if ($location.path().toString().includes('/bien-lai-thu-phi-le-phi')) {
                        $rootScope.ReloadReceipt();
                    }
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.UpdateReceipt = function (type) {
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#form-code').focus();
            return false;
        }

        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng chọn mẫu số hóa đơn');
            $('select#symbol-code').focus();
            return false;
        }

        if (!$scope.Invoice.CUSID) {
            alert('Bạn cần nhập vào thông tin <b>Mã học sinh</b>.');
            return false;
        }

        if (!$scope.Invoice.NOTE) {
            alert('Bạn cần nhập vào thông tin <b>Ghi chú</b>.');
            return false;
        }

        if ($scope.Invoice.CUSEMAIL) {
            if (!validation.isEmailAddress($scope.Invoice.CUSEMAIL)) {
                alert('Vui lòng nhập đúng định dạng email');
                return;
            }
        }

        if (!$scope.Invoice.CHANGEREASON && $scope.Invoice.INVOICETYPE == 5) {
            alert('Vui lòng nhập lý do điểu chỉnh');
            return false;
        }

        $scope.Invoice.EXCHANGERATE = $scope.Invoice.EXCHANGERATE.toString().replace(/\,/g, '');/*.replace(/\./g, '')*/;

        LoadingShow();
        var action = url + 'UpdateReceipt';
        var datasend = JSON.stringify({
            invoice: $scope.Invoice,
            type: type
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-receipt').modal('hide');
                    $rootScope.ReloadReceipt();
                } else {
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveAndSend');
            }
            LoadingHide();
        });
    }

    $scope.SuggestCustomer = function () {
        var obj = $rootScope.selectedCustomer;
        if (obj) {
            $scope.Invoice.CUSNAME = obj.CUSNAME;
            $scope.Invoice.CUSADDRESS = obj.CUSADDRESS;
            $scope.Invoice.CUSTAXCODE = obj.CUSTAXCODE;
            $scope.Invoice.CUSID = obj.CUSID;
            $scope.Invoice.CUSEMAIL = obj.CUSEMAIL;
            $scope.Invoice.CUSBUYER = obj.CUSBUYER;
        }
    }

    $scope.ReadNumberToCurrencyWords = function () {
        var moneyQty = 0;
        // Kiểm tra là ngoại tệ hay Việt Nam Đồng
        if ($scope.Invoice.TOTALPAYMENTRAW.toString() != "NaN") {
            moneyQty = $scope.Invoice.TOTALPAYMENTRAW;
        }
        else {
            moneyQty = $scope.Invoice.TOTALPAYMENT;
        }
        // End
        var action = url + 'ReadNumberToWords';
        var datasend = JSON.stringify({
            kindOfMoney: $scope.Invoice.CURRENCY,
            number: moneyQty
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.NumberToCurrencyWords = response.data;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ReadNumberToCurrencyWords');
            }
        });
    }

    $scope.GetInvoice = function () {
        LoadingShow();
        var action = url + 'GetInvoice';
        var datasend = JSON.stringify({
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice');
            }
            LoadingHide();
        });
    }

    $scope.SelectAll = function () {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });
        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListInvoice.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListInvoice.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.oldTaxcode = null;

    $scope.LoadInfoByTaxcode = function () {

        if ($scope.Invoice.CUSTAXCODE != $scope.oldTaxcode && $scope.Invoice.CUSTAXCODE != null && $scope.Invoice.CUSTAXCODE != "") {
            var url = 'https://k8s.misa.com.vn/org-info/suggest?taxCode=' + $scope.Invoice.CUSTAXCODE;
            $("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
                var obj = JSON.parse(response);
                $timeout(function () {
                    if (obj.data.companyName) {
                        $scope.Invoice.CUSNAME = obj.data.companyName;
                        $scope.Invoice.CUSADDRESS = obj.data.address;
                        //if (!$scope.Invoice.CUSBUYER)
                        //    $scope.Invoice.CUSBUYER = obj.data.owner;
                    } else {
                        alert("Không tìm thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
                        $scope.Invoice.CUSNAME = null;
                        $scope.Invoice.CUSADDRESS = null;
                    }
                });
            });

            //var url = 'https://app.meinvoice.vn/Other/GetCompanyInfoByTaxCode?taxCode=' + $scope.Invoice.CUSTAXCODE;
            //$("#tempLoadTaxinfo").load(url, function (response, status, xhr) {
            //    var rawObj = JSON.parse(response);
            //    var obj = new Object();
            //    if (rawObj.Data != "") {
            //        obj = JSON.parse(rawObj.Data);
            //    }
            //    $timeout(function () {
            //        if (obj.companyName) {
            //            let fullAddrress = obj.address;
            //            fullAddrress = (obj.ward != "" ? fullAddrress + ', ' + obj.ward : fullAddrress);
            //            fullAddrress = (obj.district != "" ? fullAddrress + ', ' + obj.district : fullAddrress);
            //            fullAddrress = (obj.province != "" ? fullAddrress + ', ' + obj.province : fullAddrress);

            //            $scope.Invoice.CUSADDRESS = fullAddrress + ', Việt Nam';
            //            $scope.Invoice.CUSNAME = obj.companyName.toUpperCase();
            //        } else {
            //            alert("Không tim thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
            //            $scope.Invoice.CUSNAME = null;
            //            $scope.Invoice.CUSADDRESS = null;
            //        }
            //    });
            //});



            //var action = url + 'LoadInfoByComTaxCode';
            //var datasend = JSON.stringify({
            //    taxcode: $scope.Invoice.CUSTAXCODE
            //});
            //LoadingShow();
            //CommonFactory.PostDataAjax(action, datasend, function (response) {
            //    $timeout(function () {
            //        if (response.success) {
            //            $scope.Invoice.CUSADDRESS = response.data.ComAddress;
            //            $scope.Invoice.CUSNAME = response.data.ComName;
            //        } else {
            //            alert("Không tim thấy thông tin doanh nghiệp. Xin vui lòng kiểm tra lại mã số thuế nhập vào.");
            //            $scope.Invoice.CUSNAME = null;
            //            $scope.Invoice.CUSADDRESS = null;
            //        }
            //    })
            //    LoadingHide();
            //});

        } else if ($scope.Invoice.CUSTAXCODE != $scope.oldTaxcode) {
            $scope.Invoice.CUSNAME = null;
            $scope.Invoice.CUSADDRESS = null;
        }
    }

    $scope.TaxRateList = [
        {
            value: -1, name: "Không chịu thuế"
        },
        {
            value: 0, name: "0%"
        },
        {
            value: 5, name: "5%"
        },
        {
            value: 10, name: "10%"
        }
    ];

    $scope.ViewTaxcode = function (taxcode) {
        if (!taxcode)
            return;
        var number_regex = new RegExp(/\d/, "gi");
        var symbol_regex = new RegExp(/\S/, "gi");
        taxcode = taxcode.replace(symbol_regex, function (matched) {
            if (matched.match(number_regex))
                return "<span class=\"" + "taxcode-symbol" + "\">" + matched + "</span>";
            else
                return "<span class=\"" + "taxcode-space" + "\">" + matched + "</span>";
        });

        return $sce.trustAsHtml(taxcode);
    }

    $scope.ChangeDiscountType = function () {
        if ($scope.Invoice.DISCOUNTTYPE == 'KHONG_CO_CHIET_KHAU') {
            $scope.Invoice.LISTPRODUCT.forEach(function (obj) {
                obj.DISCOUNTRATE = 0;
            });
        }
        else {

        }
    }

    $scope.GetNumber = function () {
        $scope.FormNumber = new Object();

        $scope.FormNumber.FORMCODE = $scope.Number.FORMCODE;
        $scope.FormNumber.SYMBOLCODE = $scope.Number.SYMBOLCODE;
        var action = url + 'GetNumber';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    let result = response.result[0];
                    $scope.Number.CURRENTNUMBER = result.CURRENTNUMBER;
                    $scope.Number.FROMNUMBER = result.CURRENTNUMBER + 1;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetNumberaa');
            }
            LoadingHide();
        });
    }

    $scope.ExchangeRateFormat = function () {
        $("#exchangeRateInput").on('keyup', function () {
            if ($(this).val() != 'NaN') {
                var n = parseInt($(this).val().replace(/\D/g, ''), 10);
                $(this).val(n.toLocaleString().replace('.', ','));
            }
            else {
                $(this).val(1);
            }
        })
    }

    $scope.SymbolCodeChange = function (symbolcode) {
        $scope.GLOBALTAXRATE = 0;
        $scope.TAXRATE = 0;
        $scope.CalMoneyAfterChangeValue();
        var listSymbolCode = $rootScope.ListSymbolCode;
        var symbolObj = listSymbolCode.filter(function (x) {
            return x.SYMBOLCODE === symbolcode;
        });
        $scope.TAXRATE = symbolObj[0].TAXRATE;
        $scope.ONLYTAXRATE = $scope.TaxRateList[0].value;
    }

    var now = new Date();
    $scope.CurrentDay = now.getDate();
    $scope.CurrentMonth = now.getMonth() + 1;
    $scope.CurrentYear = now.getFullYear();
    $scope.CurrentDate = now.getDate() + "/" + (now.getMonth() + 1) + "/" + now.getFullYear();

}]);

function xoa_dau(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    return str;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Validate Input
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function InputLimiter(e, allow) {
    var AllowableCharacters = '';
    if (allow == 'Letters') { AllowableCharacters = ' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'; }
    else if (allow == 'Numbers') { AllowableCharacters = '1234567890'; }
    else if (allow == 'NameCharacters') { AllowableCharacters = ' ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-.\''; }
    else if (allow == 'NameCharactersAndNumbers') { AllowableCharacters = '1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-\''; }
    else if (allow == '09az') { AllowableCharacters = '1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'; }
    else AllowableCharacters = allow;

    var k = document.all ? parseInt(e.keyCode) : parseInt(e.which);
    if (k != 13 && k != 8 && k != 0) {
        if ((e.ctrlKey == false) && (e.altKey == false)) {
            var ok = (AllowableCharacters.indexOf(String.fromCharCode(k)) != -1);
            if (!ok) Beep();
            return ok;
        } else {
            return true;
        }
    } else {
        return true;
    }
}

function ReadNumber(value) {
    var action = '/Receipt/ReadNumberToWords';
    var datasend = JSON.stringify({
        number: parseFloat(value),
        kindOfMoney: 'VND'
    });
    $.ajax({
        type: 'POST',
        url: action,
        contentType: "application/json; charset=utf-8;",
        dataType: "json",
        processData: true,
        data: datasend,
        success: function (result) {
            $("#AmountInWords").html(result.data);
        },
        error: function (xhr, status, p3, p4) {
            LoadingHide();
        }
    });
}

function UpdateFormat(id, value) {
    if (value === undefined) {
        value = ParseVietnameseNumber($(id).val());
        if (value === false) value = $(id).val();
    }
    value = FormatVietnameseNumber(value);

    if (value !== false) $(id).val(value);

    ReadNumber(ParseVietnameseNumber($(id).val()));
}

function ParseVietnameseNumber(value) {
    if (!ValidateVietnameseNumber(value)) return false;

    var valueS = value.toString().replace(/\./g, "").replace(/\,/g, ".");
    value = parseFloat(valueS);
    if (isNaN(value)) return false;

    return value;
}

function ValidateVietnameseNumber(value) {
    if (value === undefined) return false;

    value = value.toString();

    var commaParts = value.split(",");
    if (commaParts.length > 2) return false; // có nhiều hơn 1 dấu phẩy là sai

    if (commaParts.length == 2 && commaParts[1].indexOf('.') >= 0) return false; // có dấu . đằng sau dấu , là sai

    for (i = 0; i < commaParts.length; i++) {
        if (commaParts[i].length == 0) return false; // có dấu phẩy đứng đầu hoặc cuối là sai
    }

    var pointParts = value.split("."); // tách thành các phần phân cách nhau bởi dấu chấm để kiểm tra từng phần

    if (pointParts.length == 1) return !isNaN(pointParts[0].replace(/\,/g, ".")); //không có dấu chấm (có hoặc không có dấu phẩy): trả về đúng nếu là số

    for (i = 0; i < pointParts.length; i++) {
        if (pointParts[i].length == 0) return false; // có dấu chấm đứng đầu hoặc cuối là sai

        var type = 'first';
        if (i > 0 && i < pointParts.length - 1) type = 'middle';
        else if (i > 0 && i == pointParts.length - 1) type = 'last';

        if (!CheckPartVietnameseNumber(type, pointParts[i])) return false;
    }
    return true;
}

function CheckPartVietnameseNumber(type, p) {
    var ic = p.indexOf(',');
    if (type == 'first') {
        if (p.length > 3) return false;
        if (ic >= 0) return false;
    } else if (type == 'middle') {
        if (p.length != 3) return false;
        if (ic >= 0) return false;
    } else if (type == 'last') {
        var commaParts = p.split(",");
        if (commaParts[0].length != 3) return false;
        p = p.replace(",", "."); //chuyển về dạng số mà code hiểu
    } else return false;

    return !isNaN(p); //trả về đúng nếu là số
}

function Comma(Num) {
    Num += '';
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    Num = Num.replace('.', ''); Num = Num.replace('.', ''); Num = Num.replace('.', '');
    x = Num.split(',');
    x1 = x[0];
    x2 = x.length > 1 ? ',' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    return x1 + x2;
}

function replaceAll(varb, replaceThis, replaceBy) {
    var newvarbarray = varb.toString().split(replaceThis);
    var newvarb = newvarbarray.join(replaceBy);
    return newvarb;
}

function formatCurrency(num) {
    var le = num % 1;
    if (le > 0)
        return num.formatMoney(2, ',', '.');
}

function formatCurrency(num, n) {
    var le = num % 1;
    if (le > 0)
        return num.formatMoney(n, ',', '.');
    else return num.formatMoney(0, ',', '.');
}

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
}

function FormatVietnameseNumber(value) { // format value ở dạng số mà code hiểu sang dạng Việt Nam
    if (isNaN(value)) return false;
    var pointParts = value.toString().split(".");

    value = "";
    var first = pointParts[0];
    while (first.length > 0) {
        var i = first.length - 3;
        if (i < 0) i = 0;
        if (value.length > 0) value = "." + value;
        value = first.substring(i) + value;
        first = first.substring(0, i);
    }
    if (pointParts.length > 1) value = value + "," + pointParts[1];

    return value;
}

function Beep() {
    var snd = new Audio("data:audio/wav;base64,//uQRAAAAWMSLwUIYAAsYkXgoQwAEaYLWfkWgAI0wWs/ItAAAGDgYtAgAyN+QWaAAihwMWm4G8QQRDiMcCBcH3Cc+CDv/7xA4Tvh9Rz/y8QADBwMWgQAZG/ILNAARQ4GLTcDeIIIhxGOBAuD7hOfBB3/94gcJ3w+o5/5eIAIAAAVwWgQAVQ2ORaIQwEMAJiDg95G4nQL7mQVWI6GwRcfsZAcsKkJvxgxEjzFUgfHoSQ9Qq7KNwqHwuB13MA4a1q/DmBrHgPcmjiGoh//EwC5nGPEmS4RcfkVKOhJf+WOgoxJclFz3kgn//dBA+ya1GhurNn8zb//9NNutNuhz31f////9vt///z+IdAEAAAK4LQIAKobHItEIYCGAExBwe8jcToF9zIKrEdDYIuP2MgOWFSE34wYiR5iqQPj0JIeoVdlG4VD4XA67mAcNa1fhzA1jwHuTRxDUQ//iYBczjHiTJcIuPyKlHQkv/LHQUYkuSi57yQT//uggfZNajQ3Vmz+Zt//+mm3Wm3Q576v////+32///5/EOgAAADVghQAAAAA//uQZAUAB1WI0PZugAAAAAoQwAAAEk3nRd2qAAAAACiDgAAAAAAABCqEEQRLCgwpBGMlJkIz8jKhGvj4k6jzRnqasNKIeoh5gI7BJaC1A1AoNBjJgbyApVS4IDlZgDU5WUAxEKDNmmALHzZp0Fkz1FMTmGFl1FMEyodIavcCAUHDWrKAIA4aa2oCgILEBupZgHvAhEBcZ6joQBxS76AgccrFlczBvKLC0QI2cBoCFvfTDAo7eoOQInqDPBtvrDEZBNYN5xwNwxQRfw8ZQ5wQVLvO8OYU+mHvFLlDh05Mdg7BT6YrRPpCBznMB2r//xKJjyyOh+cImr2/4doscwD6neZjuZR4AgAABYAAAABy1xcdQtxYBYYZdifkUDgzzXaXn98Z0oi9ILU5mBjFANmRwlVJ3/6jYDAmxaiDG3/6xjQQCCKkRb/6kg/wW+kSJ5//rLobkLSiKmqP/0ikJuDaSaSf/6JiLYLEYnW/+kXg1WRVJL/9EmQ1YZIsv/6Qzwy5qk7/+tEU0nkls3/zIUMPKNX/6yZLf+kFgAfgGyLFAUwY//uQZAUABcd5UiNPVXAAAApAAAAAE0VZQKw9ISAAACgAAAAAVQIygIElVrFkBS+Jhi+EAuu+lKAkYUEIsmEAEoMeDmCETMvfSHTGkF5RWH7kz/ESHWPAq/kcCRhqBtMdokPdM7vil7RG98A2sc7zO6ZvTdM7pmOUAZTnJW+NXxqmd41dqJ6mLTXxrPpnV8avaIf5SvL7pndPvPpndJR9Kuu8fePvuiuhorgWjp7Mf/PRjxcFCPDkW31srioCExivv9lcwKEaHsf/7ow2Fl1T/9RkXgEhYElAoCLFtMArxwivDJJ+bR1HTKJdlEoTELCIqgEwVGSQ+hIm0NbK8WXcTEI0UPoa2NbG4y2K00JEWbZavJXkYaqo9CRHS55FcZTjKEk3NKoCYUnSQ0rWxrZbFKbKIhOKPZe1cJKzZSaQrIyULHDZmV5K4xySsDRKWOruanGtjLJXFEmwaIbDLX0hIPBUQPVFVkQkDoUNfSoDgQGKPekoxeGzA4DUvnn4bxzcZrtJyipKfPNy5w+9lnXwgqsiyHNeSVpemw4bWb9psYeq//uQZBoABQt4yMVxYAIAAAkQoAAAHvYpL5m6AAgAACXDAAAAD59jblTirQe9upFsmZbpMudy7Lz1X1DYsxOOSWpfPqNX2WqktK0DMvuGwlbNj44TleLPQ+Gsfb+GOWOKJoIrWb3cIMeeON6lz2umTqMXV8Mj30yWPpjoSa9ujK8SyeJP5y5mOW1D6hvLepeveEAEDo0mgCRClOEgANv3B9a6fikgUSu/DmAMATrGx7nng5p5iimPNZsfQLYB2sDLIkzRKZOHGAaUyDcpFBSLG9MCQALgAIgQs2YunOszLSAyQYPVC2YdGGeHD2dTdJk1pAHGAWDjnkcLKFymS3RQZTInzySoBwMG0QueC3gMsCEYxUqlrcxK6k1LQQcsmyYeQPdC2YfuGPASCBkcVMQQqpVJshui1tkXQJQV0OXGAZMXSOEEBRirXbVRQW7ugq7IM7rPWSZyDlM3IuNEkxzCOJ0ny2ThNkyRai1b6ev//3dzNGzNb//4uAvHT5sURcZCFcuKLhOFs8mLAAEAt4UWAAIABAAAAAB4qbHo0tIjVkUU//uQZAwABfSFz3ZqQAAAAAngwAAAE1HjMp2qAAAAACZDgAAAD5UkTE1UgZEUExqYynN1qZvqIOREEFmBcJQkwdxiFtw0qEOkGYfRDifBui9MQg4QAHAqWtAWHoCxu1Yf4VfWLPIM2mHDFsbQEVGwyqQoQcwnfHeIkNt9YnkiaS1oizycqJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU=");
    snd.play();
}

app.controller('ModalMeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Meter/';
    $rootScope.ModalMeter = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        $scope.TYPECHANGE = type;
        $scope.APARTMENTNO = $scope.ApartmentnoList[0].value;
        $scope.Meter = new Object();
        if (type == 1) {
            //tạo mới
            $scope.Meter = new Object();
            if (typeof item === "string") {
                $scope.Meter.METERNAME = item;
            }
        }
        else if (type == 2) {
            //chỉnh sửa
            angular.copy(item, $scope.Meter);
        } else if (type == 3) {
            //sao chép
            angular.copy(item, $scope.Meter);
            $scope.Meter.ID = 0
        }
    }
    $rootScope.ModalAddMeter = function (item, type) {
        $scope.GetListProducts();
        //tạo mới
        $scope.Meter = new Object();
        if (typeof item === "string") {
            $scope.Meter.METERNAME = item;
        }
        $scope.Meter.CUSTAXCODE = $rootScope.Customercode;
        //$scope.GetListProducts();
    }

    $scope.AddMeter = function (type) {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        } 
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        if (!$scope.Meter.PRODUCTCODELIST) {
            alert('Bạn chưa chọn biểu giá điện.');
            return false;
        }
        $scope.IsLoading = true;
        var action = url + 'AddMeter';
        $scope.Meter.PRODUCTCODELIST = $scope.Meter.PRODUCTCODELIST.join(',');
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (type === 1) {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Thêm mới thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else if (type==3)
                    {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Sao chép thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else {
                        $scope.Meter.METERNAME = null;
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddMeter');
            }
            LoadingHide();
        });
    }

    $scope.GetListProducts = function () {
        LoadingShow();
        var action = '/Product/GetListProductByComtaxCode';
        var datasend = JSON.stringify({
            comtaxcode: $rootScope.Enterprise.COMTAXCODE
        });

        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('.selectpicker').selectpicker({});
                    $scope.ListProducts = response.msg;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 1000);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetListProducts');
            }
        });
        LoadingHide();
    }
    $scope.UpdateMeter = function () {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        }
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        var action = url + 'UpdateMeter';
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-add-meter').modal('hide');
                    $rootScope.ReloadMeter();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateMeter');
            }
            LoadingHide();
        });
    }
    $scope.ApartmentnoList = [
        {
            value: 0, name: "Không chọn"
        },
        {
            value: 1, name: "1"
        },
        {
            value: 2, name: "2"
        }
    ];
}]);
app.controller('MeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Meter/';

    LoadingShow();

    //========================== Cookie's Own ============================
    $scope.LoadCookie_Meter = function () {
        var check = getCookie("Novaon_MeterManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCode: true,
                FieldMeterName: true,
                FieldCustaxCode: true,
                FieldComtaxCode: true,
                FieldProductCode: true,
                FieldIsactive: true,
                RowNum: 10
            }
            setCookie("Novaon_MeterManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_MeterManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetMeter($scope.currentpage);
    }
    //==================================== END ================================

    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }

    $rootScope.ReloadMeter = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetMeter($scope.currentpage);
    }

    $scope.ResetGetMeter = function () {
        $scope.LoadCookie_Meter();
        $scope.ListMeter = [];
    }

    $scope.GetMeter = function (intpage) {
        $scope.ResetGetMeter();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetMeter';
        var datasend = JSON.stringify({
            form: $scope.Filter,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListMeter = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();

        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    $scope.ListMeter = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - MeterController - GetMeter');
            }
            LoadingHide();
        });
    }

    $scope.UpdateType = function () {
        if (!$scope.ChangeType)
            return;

        var find = $scope.ListMeter.filter(function (obj) {
            return obj.ISSELECTED == true;
        });

        if (find.length == 0) {
            alert('Vui lòng chọn ít nhất 1 dòng');
            $scope.ChangeType = null
            return;
        }

        var result = confirm('Chuyển sang loại ' + $scope.ChangeType);

        if (result) {
            var action = url + 'ChangeProductType';
            var datasend = JSON.stringify({
                meter: $scope.ListMeter,
                meterType: $scope.ChangeType
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        $rootScope.ReloadMeter();
                    } else {
                        $scope.ChangeType = null
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - MeterController - UpdateType');
                }
            });
            LoadingHide();
        } else {
            $scope.ChangeType = null
        }

    }

    $scope.ExportExcel = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            productType: $scope.Filter.PRODUCTTYPE,
            category: $scope.Filter.CATEGORY,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }



    $scope.SelectAll = function () {
        var find = $scope.ListMeter.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListMeter.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListMeter.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }

    //UI/UX
    $('.dropdown-menu').find('form').click(function (e) {
        e.stopPropagation();
    });
    $scope.RemoveMeter = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListMeterChecked = [];
            if (item) {
                $scope.ListMeterChecked.push(item.CODE);
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listMeterChecked = $scope.ListMeter.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listMeterChecked && listMeterChecked.length === 0) {
                    alert("Bạn chưa chọn công tơ  cần xóa.");
                    return;
                }
                if (listMeterChecked && Object.keys(listMeterChecked).length > 0) {
                    for (var i = 0; i < listMeterChecked.length; i++) {
                        $scope.ListMeterChecked.push(listMeterChecked[i].CODE);
                    }
                }
            }
            
            var lstMetertcode = $scope.ListMeterChecked.join(";");
            var action = url + 'RemoveMeter';
            var datasend = JSON.stringify({
                metercode: lstMetertcode
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadMeter();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các công tơ đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
//xem thêm,thu gọn
function See_more_meter() {
    $('#id1').css('display', 'none');
    $('#id2').css('display', 'block');
}
function Collapse_meter() {
    $('#id2').css('display', 'none');
    $('#id1').css('display', 'block');
}


app.controller('ModalMeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Meter/';
    $rootScope.ModalMeter = function (item, type) {
        if (!$rootScope.Enterprise) {
            $rootScope.GetEnterpriseInfo();
        }

        $scope.TYPECHANGE = type;
        $scope.APARTMENTNO = $scope.ApartmentnoList[0].value;
        $scope.Meter = new Object();
        if (type == 1) {
            //tạo mới
            $scope.Meter = new Object();
            if (typeof item === "string") {
                $scope.Meter.METERNAME = item;
            }
        }
        else if (type == 2) {
            //chỉnh sửa
            angular.copy(item, $scope.Meter);
        } else if (type == 3) {
            //sao chép
            angular.copy(item, $scope.Meter);
            $scope.Meter.ID = 0
        }
    }
    $rootScope.ModalAddMeter = function (item, type) {
        $scope.GetListProducts();
        //tạo mới
        $scope.Meter = new Object();
        if (typeof item === "string") {
            $scope.Meter.METERNAME = item;
        }
        $scope.Meter.CUSTAXCODE = $rootScope.Customercode;
        //$scope.GetListProducts();
    }

    $scope.AddMeter = function (type) {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        } 
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        if (!$scope.Meter.PRODUCTCODELIST) {
            alert('Bạn chưa chọn biểu giá điện.');
            return false;
        }
        $scope.IsLoading = true;
        var action = url + 'AddMeter';
        $scope.Meter.PRODUCTCODELIST = $scope.Meter.PRODUCTCODELIST.join(',');
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    if (type === 1) {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Thêm mới thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else if (type==3)
                    {
                        toastr.options = { "timeOut": "2000" };
                        toastr.success("Sao chép thành công!")
                        $('.modal-add-meter').modal('hide');
                    }
                    else {
                        $scope.Meter.METERNAME = null;
                    }
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - AddMeter');
            }
            LoadingHide();
        });
    }

    $scope.GetListProducts = function () {
        LoadingShow();
        var action = '/Product/GetListProductByComtaxCode';
        var datasend = JSON.stringify({
            comtaxcode: $rootScope.Enterprise.COMTAXCODE
        });

        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $('.selectpicker').selectpicker({});
                    $scope.ListProducts = response.msg;
                    $timeout(function () {
                        $('.selectpicker').selectpicker('refresh'); //put it in timeout for run digest cycle
                    }, 1000);
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetListProducts');
            }
        });
        LoadingHide();
    }
    $scope.UpdateMeter = function () {
        if (!$scope.Meter.CUSTAXCODE) {
            alert('Bạn cần nhập MST của khách hàng.');
            return false;
        }
        if (!$scope.Meter.METERNAME) {
            alert('Bạn cần nhập tên công tơ.');
            return false;
        }
        var action = url + 'UpdateMeter';
        var datasend = JSON.stringify({
            meter: $scope.Meter
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.options = { "timeOut": "2000" };
                    toastr.success("Cập nhật thành công!")
                    $('.modal-add-meter').modal('hide');
                    $rootScope.ReloadMeter();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - UpdateMeter');
            }
            LoadingHide();
        });
    }
    $scope.ApartmentnoList = [
        {
            value: 0, name: "Không chọn"
        },
        {
            value: 1, name: "1"
        },
        {
            value: 2, name: "2"
        }
    ];
}]);
app.controller('ModalImportMeterController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/Meter/';

    if (!$rootScope.Enterprise)
        $rootScope.GetEnterpriseInfo();
    angular.element(function () {
        $scope.Import = {};
        $scope.ListMap = {};
        $scope.Import.HeaderRow = 1;
        $scope.Import.ShowSelectSheetAndHeader = false;
    });

    //$scope.ShowStepThree = function () {
    //    $scope.stepOne = false;
    //    $scope.stepTwo = false;
    //    $scope.stepThree = true;
    //}

    //$scope.ResetAllStep = function () {
    //    $scope.stepOne = true;
    //    $scope.stepTwo = false;
    //    $scope.stepThree = false;
    //    $scope.ListData = null;
    //}

    $scope.ShowStepThree = function () {
        $scope.stepOne = false;
        $scope.stepTwo = false;
        $scope.stepThree = true;
    }

    $scope.ResetAllStep = function () {
        $scope.stepOne = true;
        $scope.stepTwo = false;
        $scope.stepThree = false;
        $scope.ListData = null;
        $scope.IsDiffent = true;
    }

    $rootScope.ModalImportMeter = function (item) {
    }

    $scope.ImportMeterFromExcel = function () {
        $('#fileUploadMeter').trigger('click');
    };

    //$('#frmImportMeter').fileupload({
    //    autoUpload: false,
    //    add: function (e, data) {
    //        if (data.files[0].type.match('image.*')) {
    //            return;
    //        }
    //        LoadingShow();
    //        var fileData = new FormData();
    //        fileData.append("file0", data.files[0]);
    //        var action = url + "ImportMeterFromExcel";
    //        $.ajax({
    //            type: "POST",
    //            url: action,
    //            contentType: false,
    //            processData: false,
    //            data: fileData,
    //            success: function (result) {
    //                $timeout(function () {
    //                    if (result.rs) {
    //                        $scope.ListData = result.data;
    //                        $scope.IsDiffent = result.isDiffent;
    //                        $scope.ImportMeters = result.importMeters;
    //                        $scope.stepOne = false;
    //                        $scope.stepTwo = true;
    //                    } else {
    //                        alert(result.msg);
    //                    }
    //                    LoadingHide();
    //                }, 500);
    //            },
    //            error: function (xhr, status, p3, p4) {
    //                alert("Lỗi không thể đọc file excel.");
    //            }
    //        });
    //    }
    //});

    //$scope.SaveMeterList = function () {
    //    if (!$scope.ImportMeters) {
    //        alert("Bạn chưa tải file lên. Vui lòng tải chọn file tải lên.");
    //    }
    //    var action = url + 'SaveMeterList';
    //    var datasend = JSON.stringify({
    //        meters: $scope.ImportMeters
    //    });
    //    LoadingShow();
    //    CommonFactory.PostDataAjax(action, datasend, function (response) {
    //        if (response) {
    //            if (response.rs) {
    //                alert('Thành công!')
    //                location.reload(true);
    //            } else {
    //                $scope.ErrorMessage = response.msg;
    //                alert(response.msg);
    //            }
    //        } else {
    //            alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveMeterList');
    //        }
    //        LoadingHide();
    //    });
    //}

    $('#frmImportMeter').fileupload({
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
        var action = url + 'PreviewMeterData';
        successCallback = function (data) {
            $scope.ImportMeters = data.ListMeters;
            $timeout(function () {
                $scope.stepThree = true;
                $scope.stepTwo = false;
            }, 100);
        }
        AjaxRequest(action, { listMap: lst}, "POST", true, "json", successCallback);
    }
    $scope.count = 1;
    $scope.SaveMeterList = function () {
        if (!$scope.ImportMeters) {
            alert("Bạn chưa tải file lên. Vui lòng tải chọn file tải lên.");
        }
        var action = url + 'ImportDataMeter';
        successCallback = function (data) {
            if ($scope.count == 1) {
                toastr.success(data.msg);
                $timeout(function () {
                    location.reload(true);
                }, 4000);
            }
            $scope.count ++;
        }
        AjaxRequest(action, {}, "POST", true, "json", successCallback);
    }
}]);
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

app.controller('LogController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/Log/';
    LoadingShow();
    $scope.LoadCookie_Log = function () {
        var check = getCookie("Novaon_LogManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldUserName: true,
                FieldActionName: true,
                FieldObjectName: true,
                FieldIpAddress: true,
                FieldDescription: true,
                FieldLogTime: true,
                RowNum: 10
            }
            setCookie("Novaon_LogManagement", JSON.stringify($scope.cookie), 30);
        }
    }
    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_LogManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetLog($scope.currentpage);
    }
    $rootScope.ReloadLog = function (page) {
        if (page == 1)
            $scope.currentpage = page;
        $scope.GetLog($scope.currentpage);
    }
    $scope.ResetGetLog = function () {
        $scope.LoadCookie_Log();
        $scope.ListLog = [];
    }
    $scope.Filter = {KEYWORD:''};
    $scope.GetLog = function (intpage) {
        $scope.ResetGetLog();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        $scope.IsLoading = true;
        var action = url + 'GetLogs';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        });
        $scope.ListLog = new Array();
        $scope.TotalPages = 1;
        $scope.TotalRow = 1;
        LoadingShow();

        CommonFactory.PostDataAjax(action, datasend, function (response) {

            if (response) {
                if (response.rs) {
                    $scope.ListLog = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - LogController - GetLog');
            }
            LoadingHide();
        });
    }
    $scope.SelectAll = function () {
        var find = $scope.ListLog.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }
    $scope.SeleteRow = function (item) {
        var find = $scope.ListLog.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListLog.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }

    $scope.ToggleShowMore = function (_view) {
        $(_view).fadeToggle(300);
    }
    $scope.ExportExcel = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            currentPage: intpage,
            itemPerPage: $scope.cookie.RowNum
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
}]);
app.controller('CurrencyUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/CurrencyUnit/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Currency = function () {
        var check = getCookie("Novaon_CurrencyUnitManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldCurrencyUnit: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_CurrencyUnitManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_CurrencyUnitManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetCurrencyUnit($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadCurrencyUnit = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetCurrencyUnit($scope.currentpage);
    }

    $scope.ResetGetCurrencyUnit = function () {
        $scope.LoadCookie_Currency();
        $scope.ListCurrencyUnit = [];
    }
    //lấy ra danh sách tiền thanh toán
    $scope.GetCurrencyUnit = function (intpage) {
        $scope.ResetGetCurrencyUnit();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetCurrencyUnit';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCurrencyUnit = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCategory');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListCurrencyUnit.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListCurrencyUnit.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListCurrencyUnit.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    $scope.RemoveCurrencyUnit = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;

            $scope.ListCurrencyChecked = [];
            $scope.ListCurrencyActive = [];
            $scope.count = 0;
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được phép xóa khi đang ở trạng thái hoạt động');
                    return false;
                }
                $scope.ListCurrencyChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listCurrencyChecked = $scope.ListCurrencyUnit.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listCurrencyChecked && listCurrencyChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn tiền thanh toán cần xóa.");
                    return;
                }
                if (listCurrencyChecked && Object.keys(listCurrencyChecked).length > 0) {
                    for (var i = 0; i < listCurrencyChecked.length; i++) {
                        $scope.ListCurrencyChecked.push(listCurrencyChecked[i].ID);
                        $scope.ListCurrencyActive.push(listCurrencyChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListCurrencyActive.length; i++) {
                        if ($scope.ListCurrencyActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstCurrencyid = $scope.ListCurrencyChecked.join(";");
            var action = url + 'RemoveCurrencyUnit';
            var datasend = JSON.stringify({
                id: lstCurrencyid,count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các tiền thanh toán đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
    $scope.ExportExcell = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
}]);
app.controller('ModalCurrencyUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/CurrencyUnit/';
    $scope.currency = new Object();
    $rootScope.ModalCurrency = function (item) {
        angular.copy(item, $scope.currency);
        if (item === "") {
            $scope.currency.ISACTIVED = true;
        }
    }
    //thêm tiền thanh toán
    $scope.AddCurrencyUnit = function () {
        if (!$scope.currency.CURRENCYUNIT) {
            alert('Vui lòng nhập vào tên tiền thanh toán');
            return false;
            $('#currency').focus();
        }
        if ($scope.currency.CURRENCYUNIT.length >= 512) {
            toastr.warning('Tên tiền thanh toán độ dài không quá 512 ký tự');
            return false;
        }
        $scope.IsLoading = true;
        var action = url + 'SaveCurrencyUnit';
        var datasend = JSON.stringify({
            currency: $scope.currency
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-currencyunit').modal('hide');
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCurrencyUnit');
            }
        });
        $scope.IsLoading = false;
    }
  //cập nhật trạng thái
    $rootScope.UpdateCurencyUnit = function (item) {
        let msg = '';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveCurrencyUnit';
        if (item.ISACTIVED == true) {
            item.ISACTIVED = false;
            msg = 'Ngừng kích hoạt thành công';
        } else {
            item.ISACTIVED = true;
            msg = 'Kích hoạt thành công';
        }
        var datasend = JSON.stringify({
            currency: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadCurrencyUnit(1);
                } else {
                   toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCurrencyUnit');
            }
        });
    }
}]);
app.controller('PaymentMethodController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/PaymentMethod/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_Payment = function () {
        var check = getCookie("Novaon_PaymentManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldPaymentMethod: true,
                FieldIsActived: true,
                RowNum: 10
            }
            setCookie("Novaon_PaymentManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_PaymentManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetPaymentMethod($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadPayment = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetPaymentMethod($scope.currentpage);
    }

    $scope.ResetGetPayment = function () {
        $scope.LoadCookie_Payment();
        $scope.ListPayment = [];
    }
    // lấy ra danh sách HTTT
    $scope.GetPaymentMethod = function (intpage) {
        $scope.ResetGetPayment();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetPaymentMethod';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListPayment = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetPaymentMethod');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListPayment.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListPayment.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListPayment.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    $scope.ExportExcell = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
    $scope.RemovePaymentMethod = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListPaymentChecked = [];
            $scope.ListPaymentActive = [];
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được xóa khi đang ở trạng thái hoạt động');
                    return false;
                }
                $scope.ListPaymentChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listPaymentChecked = $scope.ListPayment.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listPaymentChecked && listPaymentChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn HTTT cần xóa.");
                    return;
                }
                if (listPaymentChecked && Object.keys(listPaymentChecked).length > 0) {
                    for (var i = 0; i < listPaymentChecked.length; i++) {
                        $scope.ListPaymentChecked.push(listPaymentChecked[i].ID);
                        $scope.ListPaymentActive.push(listPaymentChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListPaymentActive.length; i++) {
                        if ($scope.ListPaymentActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstPaymentid = $scope.ListPaymentChecked.join(";");
            var action = url + 'RemovePaymentMethod';
            var datasend = JSON.stringify({
                id: lstPaymentid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadPayment();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các HTTT đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
}]);
app.controller('ModalPaymentMethodController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/PaymentMethod/';
    $scope.payment = new Object();
    $rootScope.ModalPayment = function (item) {
        angular.copy(item, $scope.payment);
        if (item === "") {
            $scope.payment.ISACTIVED = true;
        }
    }
    //thêm HTTT
    $scope.SaveAndClosePayment = function () {
        if (!$scope.payment.PAYMENTMETHOD) {
            alert('Vui lòng nhập vào tên hình thức thanh toán');
            return false;
            $('#payment').focus();
        }
        if ($scope.payment.PAYMENTMETHOD.length >= 512) {
            toastr.warning('Tên HTTT độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SavePaymentMethod';
        var datasend = JSON.stringify({
            payment: $scope.payment
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-payment').modal('hide');
                    $rootScope.ReloadPayment(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SavePaymentMethod');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdatePaymentMethod = function (item) {
        let msg = '';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        } 
        else {
            var action = url + 'SavePaymentMethod';
            if (item.ISACTIVED == true) {
                item.ISACTIVED = false;
                msg = 'Ngừng kích hoạt thành công';
            } else  {
                item.ISACTIVED = true;
                msg = 'Kích hoạt thành công';
            }
            var datasend = JSON.stringify({
                payment: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        toastr.success(msg);
                        $rootScope.ReloadPayment(1);
                    } else {
                       toastr.warning(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SavePaymentMethod');
                }
            });
        }
    }
}]);
app.controller('QuantityUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', 'persistObject', function ($scope, $rootScope, $timeout, CommonFactory, persistObject) {
    var url = '/QuantityUnit/';
    $scope.Filter = { KEYWORD: '', PRODUCTTYPE: 0, CATEGORY: null }
    $scope.LoadCookie_QuantityUnit = function () {
        var check = getCookie("Novaon_QuantityUnitManagement");
        if (check) {
            $scope.cookie = JSON.parse(check);
        }
        else {
            $scope.cookie = {
                FieldID: true,
                FieldQuantityUnit: true,
                
                RowNum: 10
            }
            setCookie("Novaon_QuantityUnitManagement", JSON.stringify($scope.cookie), 30);
        }
    }

    $scope.Check = function (status, field) {
        if (field == 'RowNum') {
            $scope.cookie[field] = status;
        }
        else
            $scope.cookie[field] = !status;
        setCookie("Novaon_QuantityUnitManagement", JSON.stringify($scope.cookie), 30);
        if (field != 'RowNum')
            return;
        $scope.GetQuantityUnit($scope.currentpage);
    }
    //==================================== END ================================

    $rootScope.ReloadQuantityUnit = function (page) {
        if (page == 1) {
            $scope.currentpage = page;
        }
        $scope.GetQuantityUnit($scope.currentpage);
    }

    $scope.ResetGetQuantityUnit = function () {
        $scope.LoadCookie_QuantityUnit();
        $scope.ListQuantityUnit = [];
    }

    $scope.GetQuantityUnit = function (intpage) {
        $scope.ResetGetQuantityUnit();
        $scope.IsLoading = true;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;

        var action = url + 'GetQuantityUnit';
        var datasend = JSON.stringify({
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        });
        LoadingShow();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListQuantityUnit = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalRow = response.TotalRow;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetQuantityUnit');
            }
            LoadingHide();
        });
        $scope.IsLoading = false;
    }
    $scope.SelectAll = function () {
        var find = $scope.ListQuantityUnit.filter(function (obj) {
            return obj.ISSELECTED == $scope.IsSelectAll;
        });

        if (find.length > 0) {
            find.forEach(function (item) {
                item.ISSELECTED = !$scope.IsSelectAll;
            });
        }
    }

    $scope.SeleteRow = function (item) {
        var find = $scope.ListQuantityUnit.filter(function (obj) {
            return obj.ISSELECTED == true;
        });
        if (item)
            $scope.IsSelectAll = false;
        else {
            if (find.length == $scope.ListQuantityUnit.length - 1) {

                $scope.IsSelectAll = true;
            }
        }
    }
    //xóa đơn vị tính
    $scope.RemoveQuantityUnit = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            $scope.count = 0;
            $scope.ListQuantityChecked = [];
            $scope.ListQuantityActive = [];
            $scope.ListComtaxcode = [];
            if (item) {
                if (item.ISACTIVED == true) {
                    toastr.warning('Không được phép xóa trạng thái đang hoạt động');
                    return;
                }
                $scope.ListQuantityChecked.push(item.ID);
                $scope.count = 1;
            }
            else {
                //kiểm tra xem đã chọn bản ghi xóa chưa
                var listQuantityChecked = $scope.ListQuantityUnit.filter(function (obj) { return obj.ISSELECTED == true; });
                if (listQuantityChecked && listQuantityChecked.length === 0) {
                    toastr.warning("Bạn chưa chọn đơn vị tính cần xóa.");
                    return;
                }
                if (listQuantityChecked && Object.keys(listQuantityChecked).length > 0) {
                    for (var i = 0; i < listQuantityChecked.length; i++) {
                        $scope.ListQuantityChecked.push(listQuantityChecked[i].ID);
                        $scope.ListQuantityActive.push(listQuantityChecked[i].ISACTIVED);
                    }
                    for (var i = 0; i < $scope.ListQuantityActive.length; i++) {
                        if ($scope.ListQuantityActive[i] === false) {
                            $scope.count ++;
                        }
                    }
                }
            }

            var lstQuantityid = $scope.ListQuantityChecked.join(";");
            var action = url + 'RemoveQuantityUnit';
            var datasend = JSON.stringify({
                id: lstQuantityid, count: $scope.count
            });
            LoadingShow();
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response && response.rs) {
                    toastr.success(response.msg);
                    $rootScope.ReloadQuantityUnit();
                } else {
                    toastr.warning(response.msg);
                }
                LoadingHide();
            });
        };
        confirm("Bạn có thực sự muốn xóa các đơn vị tính đã chọn không?", "Thông báo", "Không", "Có", confirmContinue);
    }
    $scope.ExportExcell = function () {
        var intpage;
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentpage = intpage;
        var action = url + 'ExportExcell';
        var datasend = {
            keyword: $scope.Filter.KEYWORD,
            pagesize: $scope.cookie.RowNum,
            currentpage: $scope.currentpage
        };

        LoadingShow();
        var dialog = $.fileDownload(action, {
            httpMethod: 'POST',
            data: datasend
        });
        LoadingHide();
    }
}]);
app.controller('ModalQuantityUnitController', ['$scope', '$rootScope', '$timeout', 'CommonFactory', function ($scope, $rootScope, $timeout, CommonFactory) {
    var url = '/QuantityUnit/';
    $scope.quantityunit = new Object();
    $rootScope.ModalQuantityUnit = function (item) {
        angular.copy(item, $scope.quantityunit);
        if (item === "") {
            $scope.quantityunit.ISACTIVED = true;
        }
    }
    //thêm đơn vị tính
    $scope.AddQuantityUnit = function () {
        if (!$scope.quantityunit.QUANTITYUNIT) {
            alert('Vui lòng nhập vào tên đơn vị tính');
            return false;
            $('#quantityunit').focus();
        }
        if ($scope.quantityunit.QUANTITYUNIT.length >= 512) {
            toastr.warning('Tên đơn vị tính độ dài không quá 512 ký tự');
            return false;
        }
        var action = url + 'SaveQuantityUnit';
        var datasend = JSON.stringify({
            quantityunit: $scope.quantityunit
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success('Cập nhật thành công');
                    $('.modal-quantityunit').modal('hide');
                    $rootScope.ReloadQuantityUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveQuantityUnit');
            }
        });
    }
    //cập nhật trạng thái
    $rootScope.UpdateQuantity = function (item) {
        let msg='';
        if (item.COMTAXCODE == '1') {
            toastr.warning('Bạn không được phép thay đổi trạng thái này');
            return false;
        }
        var action = url + 'SaveQuantityUnit';
            if (item.ISACTIVED == true) {
                item.ISACTIVED = false;
                msg = 'Ngừng kích hoạt thành công';
            } else {
                item.ISACTIVED = true;
                msg = 'Kích hoạt thành công';
            }
        var datasend = JSON.stringify({
            quantityunit: item
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    toastr.success(msg);
                    $rootScope.ReloadQuantityUnit(1);
                } else {
                    toastr.warning(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveQuantityUnit');
            }
        });
    }
}]);