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
