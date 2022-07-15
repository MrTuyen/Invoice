app.controller('ManagerController', ['$scope', '$timeout', '$filter', 'CommonFactory', function ($scope, $timeout, $filter, CommonFactory) {
    var url = '/Manager/';

    $scope.InvoiceType = [
        {
            Key: 0,
            Value: "Hóa đơn giá trị gia tăng"
        },
        {
            Key: 1,
            Value: "Hóa đơn trường học"
        },
        {
            Key: 2,
            Value: "Hóa đơn tiền điện"
        },
        {
            Key: 3,
            Value: "Hóa đơn bán hàng"
        },
        {
            Key: 4,
            Value: "Hóa đơn tiền nước"
        },
        {
            Key: 5,
            Value: "Phiếu xuất kho kiêm vận chuyển nội bộ"
        }
    ]
    $scope.currentPage = 1;

    $scope.GetCompany = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentPage = intpage;

        var action = url + "GetCompany";
        var datasend = JSON.stringify({
            keyword: $scope.Keywords,
            currentPage: $scope.currentPage
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListCompany = response.result;
                    $scope.TotalPages = response.TotalPages;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetCompany");
            }
        });
    };

    $scope.BackToListCompany = function () {
        $scope.Company = new Object();
        $scope.Company.ISEDIT = false;
    };

    $scope.ReloadDataPage = function () {
        $scope.GetAllReleaseNotice($scope.currentPage);
        $scope.GetAllUserByComtaxCode($scope.currentPage);
    }


    $scope.EditCompany = function (item) {
        let arrInvoiceTypes = [];
        $scope.Company = new Object();
        $scope.Company.ISNEW = true;
        if (item) {
            $scope.SelectedInvoiceType = item.USINGINVOICETYPETMP;
            angular.copy(item, $scope.Company);
            if ($scope.Company.USINGINVOICETYPETMP) {
                let listInvoiceType = $scope.Company.USINGINVOICETYPETMP.split(',').filter(function (invoiceType) {
                    let tmp = $scope.InvoiceType.filter(function (x) {
                        return x.Key.toString() === invoiceType;
                    })[0].Value;

                    arrInvoiceTypes.push(tmp);
                });

                $("#ddlPro").selectpicker({ title: arrInvoiceTypes.join() }).selectpicker('render');
                $("#ddlPro").selectpicker("refresh");
            }
            $scope.Company.ARRUSINGTYPETMP = $scope.Company.USINGINVOICETYPETMP;
        };
        $timeout(function () {
            $scope.Company.ISEDIT = true;
            $scope.ReloadDataPage();
        }, 100);
    };

    $scope.SaveCompany = function () {
        if (!$scope.Company) {
            alert('Vui lòng nhập vào thông tin doanh nghiệp.');
            return;
        }
        if (!$scope.Company.COMNAME) {
            alert('Vui lòng nhập tên doanh nghiệp.');
            return;
        }
        if (!$scope.Company.COMTAXCODE) {
            alert('Vui lòng nhập mã số thuế doanh nghiệp.');
            return;
        }
        if (!$scope.Company.ARRUSINGTYPETMP && !$scope.Company.USINGINVOICETYPETMP) {
            alert('Vui lòng chọn loại hóa đơn.');
            return;
        }

        if (Array.isArray($scope.Company.ARRUSINGTYPETMP)) {
            let tmp = $scope.Company.ARRUSINGTYPETMP.join();
            $scope.Company.ARRUSINGTYPETMP = tmp;
        }

        var action = url + "UpdateEnterpriseInfo";
        var datasend = JSON.stringify({
            enterprise: $scope.Company
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công!');
                    $scope.GetCompany(1);

                    ////Tạo user mặc định
                    //if ($scope.Company.ISNEW) {
                    //    $scope.User = {};
                    //    $scope.User.EMAIL = $scope.Company.EMAIL;
                    //    $scope.User.USERNAME = $scope.Company.EMAIL;
                    //    $scope.User.PHONENUMBER = $scope.Company.COMPHONENUMBER;
                    //    $scope.User.COMTAXCODE = $scope.Company.COMTAXCODE;
                    //    $scope.User.PASSWORD = $scope.Company.COMTAXCODE;
                    //    $scope.SaveUser("auto");
                    //}

                    //Clear form & back về list
                    $scope.Company = new Object();
                    $scope.Company.ISEDIT = false;
                } else {
                    alert('Thất bại, vui lòng thử lại!');
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveCompany");
            }
        });
    };

    $scope.GetAllUserByComtaxCode = function (currentPage) {
        var action = url + "GetAllUserByComtaxCode";
        var datasend = JSON.stringify({
            comtaxcode: $scope.Company.COMTAXCODE
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListUser = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetAllUserByComtaxCode");
            }
        });
    };

    $scope.GetAllReleaseNotice = function (intpage) {
        $scope.FormNumber = new Object();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.FormNumber.COMTAXCODE = $scope.Company.COMTAXCODE;
        $scope.FormNumber.ITEMPERPAGE = 10;
        $scope.FormNumber.CURRENTPAGE = intpage;
        var action = url + 'GetAllReleaseNotice';
        var datasend = JSON.stringify({
            form: $scope.FormNumber
        });
        $scope.ListNumber = new Array();
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListNumber = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetAllReleaseNotice');
            }
        });
    };

    $scope.EditUser = function (item) {
        $scope.User = new Object();
        $scope.User.ISNEW = true;
        $scope.User.COMTAXCODE = $scope.Company.COMTAXCODE;
        if (item) angular.copy(item, $scope.User);
        $scope.User.PASSWORD = '';
        $scope.User.ISEDIT = true;

        $('#modal_user_edit').dialog({
            width: '24%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            //show: {
            //    effect: 'drop',
            //    direction: 'right',
            //    duration: 300
            //},
            //hide: {
            //    effect: 'fade',
            //    duration: 200
            //},
            create: function (event, ui) {
                $('#modal_user_edit').show();
            }
        });
    };

    $scope.EditReleaseNotice = function (item) {
        $scope.ISRELEASENOTICEEDIT = false;
        $scope.ReleaseNotice = new Object();
        $scope.ReleaseNotice.COMTAXCODE = $scope.Company.COMTAXCODE;
        $scope.ReleaseNotice.STATUS = 4;
        if (item) {
            angular.copy(item, $scope.ReleaseNotice);
            $scope.ISRELEASENOTICEEDIT = true;
            $scope.ReleaseNotice.OLDFROMNUMBER = item.FROMNUMBER;
            $scope.ReleaseNotice.OLDTONUMBER = item.TONUMBER;
        }
        $scope.ReleaseNotice.STRFROMTIME = $filter('dateFormat')($scope.ReleaseNotice.FROMTIME, 'dd/MM/yyyy');


        $('#modal_release_notice').dialog({
            width: '45%',
            modal: true,
            resizable: false,
            //autoOpen: false,
            //show: {
            //    effect: 'drop',
            //    direction: 'right',
            //    duration: 300
            //},
            //hide: {
            //    effect: 'fade',
            //    duration: 200
            //},
            create: function (event, ui) {
                $('#modal_release_notice').show();
            }
        });
    };

    $scope.CloseModalDialog = function () {
        $scope.User = new Object();
        $scope.ReleaseNotice = new Object();
        $('#modal_user').closest('.ui-dialog-content').dialog('close');
        $('#modal_release_notice').closest('.ui-dialog-content').dialog('close');
        $('#modal_invoice').closest('.ui-dialog-content').dialog('close');
    };

    $scope.AddReleaseNotice = function (item) {
        var action = url + 'AddReleaseNotice';
        var datasend = JSON.stringify({
            invoiceNumber: $scope.ReleaseNotice
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công');
                    $('#modal_release_notice').dialog('close');
                    $scope.ReloadDataPage();
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveReleaseNotice');
            }
        });
    };

    $scope.SaveReleaseNotice = function (item) {
        var action = url + 'SaveReleaseNotice';
        var datasend = JSON.stringify({
            invoiceNumber: $scope.ReleaseNotice
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert('Thành công');
                    $('#modal_release_notice').dialog('close');
                    $scope.ReloadDataPage();
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveReleaseNotice');
            }
        });
    };

    $scope.SaveUser = function (user, isEdit) {
        if (user.EMAIL === '') {
            alert('Vui lòng nhập địa chỉ email.');
            return;
        }

        if (user.COMTAXCODE === '') {
            alert('Không tồn tại MST doanh nghiệp.');
            return;
        }
        var action = url + 'AddUser';
        if (isEdit)
            action = url + 'UpdateUser';     
        
        var datasend = JSON.stringify({
            account: user
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (typeof item !== "string") {
                if (response) {
                    if (response.rs) {
                        alert('Thành công');
                        $('#modal_user').dialog('close');
                        $scope.ReloadDataPage();
                    } else {
                        $scope.ErrorMessge = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveUser');
                }
            }
        });
    };

    $scope.DeleteUser = function (user) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            var action = url + 'DeleteUser';
            var datasend = JSON.stringify({
                account: user
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Thành công');
                        $scope.ReloadDataPage();
                    } else {
                        $scope.ErrorMessge = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeleteUser');
                }
            });
        };
        confirm('Bạn có chắc không', 'Xóa tài khoản quản lý', 'Bỏ qua', 'Đồng ý', confirmContinue);
    }

    //$scope.DeleteUser = function (user) {
    //    var confirmContinue = function (result) {
    //        if (!result)
    //            return false;
    //        user.COMTAXCODE = '0106579683-999';
    //        var action = url + 'AddUser';
    //        var datasend = JSON.stringify({
    //            account: user
    //        });
    //        CommonFactory.PostDataAjax(action, datasend, function (response) {
    //            if (response) {
    //                if (response.rs) {
    //                    alert('Thành công');
    //                    $scope.ReloadDataPage();
    //                } else {
    //                    $scope.ErrorMessge = response.msg;
    //                    alert(response.msg);
    //                }
    //            } else {
    //                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeleteUser');
    //            }
    //        });
    //    };
    //    confirm('Sau khi xóa user khỏi doanh nghiệp <b>' + user.COMTAXCODE + '</b> sẽ trở về mặc định thuộc mã số thuế <b>0106579683-999</b>.<br/><br/>Bấm <b>Đồng ý</b> để chuyển sang mặc định, bấm <b>Bỏ qua</b> để giữ nguyên trạng thái', 'Xóa tài khoản quản lý', 'Bỏ qua', 'Đồng ý', confirmContinue);
    //}

    $scope.DeleteReleaseNotice = function (item) {
        var confirmContinue = function (result) {
            if (!result)
                return false;
            var action = url + 'DeleteReleaseNotice';
            var datasend = JSON.stringify({
                releaseNotice: item
            });
            CommonFactory.PostDataAjax(action, datasend, function (response) {
                if (response) {
                    if (response.rs) {
                        alert('Cập nhật thành công');
                        $scope.ReloadDataPage();
                    } else {
                        $scope.ErrorMessge = response.msg;
                        alert(response.msg);
                    }
                } else {
                    alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - DeleteUser');
                }
            });
        };
        confirm('Bạn có chắc chắn muốn xóa thông báo phát hành cho mẫu số <b>' + item.FORMCODE + '</b>, ký hiệu <b>' + item.SYMBOLCODE + '</b> từ số <b>' + item.FROMNUMBER + '</b> đến số <b>' + item.TONUMBER + '</b>.<br/><br/>Bấm <b>Đồng ý</b> để xóa, bấm <b>Bỏ qua</b> để giữ nguyên trạng thái', 'Xóa thông báo phát hành', 'Bỏ qua', 'Đồng ý', confirmContinue);
    }
    $scope.GetInvoice = function (intpage) {
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentPage = intpage;

        var action = url + "GetInvoice";
        var datasend = JSON.stringify({
            keyword: $scope.Keywords,
            currentPage: $scope.currentPage
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - GetInvoice");
            }
        });
    };

    $scope.BackToListInvoice = function () {
        $scope.Invoice = new Object();
        //$scope.Invoice.ISEDIT = false;
    };

    $scope.ReloadDataPage = function () {
        $scope.GetAllReleaseNotice($scope.currentPage);
        $scope.GetAllUserByComtaxCode($scope.currentPage);
    }
    
    $scope.EditInvoice = function (item) {
        $scope.Invoice = new Object();
        $scope.Invoice.ISNEW = true;
        $scope.Invoice.COMTAXCODE = $scope.Company.COMTAXCODE;
        if (item) angular.copy(item, $scope.Invoice);
        //$scope.User.PASSWORD = '';
        $scope.Invoice.ISEDIT = true;
        $('#modal_invoice_edit').dialog({
            width: '650px',
            modal: true,
            resizable: false,
            create: function (event, ui) {
                $('#modal_invoice_edit').show();
            }
        });
    };
    $scope.SaveInvoice = function () {
        if (!$scope.Invoice.INVOICEWATING) {
            alert('Vui lòng chọn cập nhật trạng thái hóa đơn hoặc dải chờ.');
            return;
        }
        if (!$scope.Invoice.COMTAXCODE) {
            alert('Vui lòng nhập mã số thuế doanh nghiệp.');
            return;
        }
        if (!$scope.Invoice.NUMBER && $scope.Invoice.INVOICEWATING == 1) {
            alert('Vui lòng nhập số HĐ cần cập nhật.');
            return;
        }
        if (!$scope.Invoice.NUMBER && $scope.Invoice.INVOICEWATING == 2) {
            alert('Vui lòng nhập số HĐ chờ bắt đầu.');
            return;
        }
        if (!$scope.Invoice.NUMBERTEMP && $scope.Invoice.INVOICEWATING == 2) {
            alert('Vui lòng nhập số HĐ chờ kết thúc .');
            return;
        }
        if (!$scope.Invoice.FORMCODE) {
            alert('Vui lòng nhập mẫu số.');
            return;
        }
        if (!$scope.Invoice.SYMBOLCODE) {
            alert('Vui lòng nhập ký hiệu.');
            return;
        }
        if (!$scope.Invoice.INVOICESTATUS && $scope.Invoice.INVOICEWATING==1) {
            alert('Vui chọn trạng thái.');
            return;
        }
        var mesg = 'Cập nhật trạng thái HĐ thành công!';
        if ($scope.Invoice.INVOICEWATING == 2) {
            mesg = 'Thêm mới dải chờ thành công';
        }
        var action = url + "UpdateInvoiceStatus";
        var datasend = JSON.stringify({
            invoice: $scope.Invoice 
        });

        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    alert(mesg);
                    $('#modal_invoice_edit').dialog("close");
                    $scope.GetInvoice(1);
                    $scope.Invoice = new Object();
                    $scope.Invoice.ISEDIT = false;
                } else {
                    alert('Thất bại, vui lòng thử lại!');
                }
            } else {
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - SaveInvoice");
            }
        });
    };
    //báo cáo tình hình sử dụng HD
    $scope.Getforstatistics = function (intpage) {
        LoadingShow();
        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentPage = intpage;
        
        var action = url + "Getforstatistics";
        var datasend = JSON.stringify({
            keyword: $scope.Keywords,
            currentPage: $scope.currentPage,
            statuscus: $scope.Statuscus
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                    $scope.TotalPages = response.TotalPages;
                    $scope.TotalCusAll = response.TotalCusAll;
                    $scope.TotalCusFee = response.TotalCusFee;
                    $scope.TotalCusFree = response.TotalCusFree;
                    $scope.TotalCusActive = response.TotalCusActive;
                    $scope.TotalCusNoActive = response.TotalCusNoActive;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Getforstatistics");
            }
            LoadingHide();
        });
    };


    // Thống kê top 10 khách hàng phát hành hóa đơn nhiều nhất trong tuần và từ trước đén nay
    $scope.GetStatisticTopTen = function (intpage) {
        LoadingShow();
        if ($scope.Statistic.STATUS === "6" && !$scope.Statistic.FROMTIME && !$scope.Statistic.TOTIME) {
            LoadingHide();
            return;
        }

        if (!intpage) {
            intpage = 1;
        }
        if (intpage > $scope.TotalPages) {
            intpage = $scope.TotalPages;
        }
        $scope.currentPage = intpage;

        var action = url + "GetStatisticTopTen";
        var datasend = JSON.stringify({
            statistic: $scope.Statistic,
            currentPage: $scope.currentPage
        });
        $scope.isLoading = true;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.ListInvoice = response.result;
                } else {
                    $scope.ErrorMessge = response.msg;
                    alert(response.msg);
                }
            } else {
                $scope.isLoading = false;
                alert("Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - Getforstatistics");
            }
            LoadingHide();
        });
    };


    //thay đổi trạng thái người dùng
    $scope.ChangStatusUser = function (comtaxcode, email, actived) {
        var msg = 'Đã kích hoạt thành công';
        if (actived == true) {
            msg = 'Đã vô hiệu hóa thành công';
        }
        var action = url + 'ChangStatusUser';
        var datasend = JSON.stringify({
            comtaxcode: comtaxcode, email: email, actived: actived
        });
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    setTimeout: 2000;
                    //toastr.success('cập nhật thành công');
                    alert(msg);
                    $scope.Company.ISEDIT = true;
                    $scope.ReloadDataPage();
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } else {
                alert('Không nhận được phản hồi từ máy chủ, vui lòng thử lại (js) - ChangStatusUser');
            }
        });
    }
    
    var GetSumany = function () {
        var action = url + 'GetSumany';
        var datasend = null;
        CommonFactory.PostDataAjax(action, datasend, function (response) {
            if (response) {
                if (response.rs) {
                    $scope.TotalCusAll = response.result.TOTAL;
                    $scope.TotalCusFee = response.result.TOTAL - response.result.FREETRIAL;
                    $scope.TotalCusActive = response.result.COUNTUSED;
                    $scope.TotalNotYetActive = response.result.TOTAL - response.result.COUNTUSED;
                    $scope.TotalWaitingPublic = response.result.WAITINGPUBLIC;
                } else {
                    $scope.ErrorMessage = response.msg;
                }
            } 
        });
    }

    GetSumany();

}]);