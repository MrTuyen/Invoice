function LoginRequest() {
    if (!$('input#username').val()) {
        alert('Vui lòng nhập vào email hoặc số điện thoại');
        $('input#username').focus();
        return false;
    }

    if (!$('input#password').val()) {
        alert('Vui lòng nhập vào mật khẩu');
        $('input#password').focus();
        return false;
    }

    $.ajax({
        type: 'POST',
        url: '/Account/Login',
        data: { 'username': $('input#username').val(), 'password': $('input#password').val() },
        dataType: 'json',
        success: function (data) {
            if (data.rs)
                window.location.href = '/';
            else
                alert(data.msg);
        },
        error: function (result) {
            console.log(result);
        }
    });
};



(function ($) {
    'use strict';

    /*==================================================================
    [ Focus input ]*/
    $('.input100').each(function () {
        $(this).on('blur', function () {
            if ($(this).val().trim() != '') {
                $(this).addClass('has-val');
            }
            else {
                $(this).removeClass('has-val');
            }
        })
    })


    /*==================================================================
    [ Validate ]*/
    var input = $('.validate-input .input100');

    $('.validate-form').on('onclick', function () {
        var check = true;

        for (var i = 0; i < input.length; i++) {
            if (validate(input[i]) == false) {
                showValidate(input[i]);
                check = false;
            }
        }

        if (!check) {
            $('.border-danger').first().focus();
        }

        return check;
    });


    $('.validate-form .input100').keypress(function () {
        hideValidate(this);
    });

    function validate(input) {
        if ($(input).attr('type') == 'email' || $(input).attr('name') == 'email') {
            if ($(input).val().trim().match(/^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{1,5}|[0-9]{1,3})(\]?)$/) == null) {
                return false;
            }
        } else if ($(input).attr('name') == 'phonenumber') {
            if ($(input).val().trim().match(/^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im) == null) {
                return false;
            }
        }
        else {
            if ($(input).val().trim() == '') {
                return false;
            }
        }
    }

    function showValidate(input) {
        $(input).addClass('border-danger');
    }

    function hideValidate(input) {
        $(input).removeClass('border-danger');
    }

    /*==================================================================
    [ Show pass ]*/
    var showPass = 0;
    $('.btn-show-pass').on('click', function () {
        if (showPass == 0) {
            $(this).next('input').attr('type', 'text');
            $(this).find('i').removeClass('zmdi-eye');
            $(this).find('i').addClass('zmdi-eye-off');
            showPass = 1;
        }
        else {
            $(this).next('input').attr('type', 'password');
            $(this).find('i').addClass('zmdi-eye');
            $(this).find('i').removeClass('zmdi-eye-off');
            showPass = 0;
        }
    });

    $('#btn-login').click(function () {
        LoginRequest();
    });

    $('input#username').keypress(function (e) {
        if (e.which == 13) {
            if ($('input#password').val() != "")
                LoginRequest();
            else
                $('input#password').focus();
        }
    });

    $('input#password').keypress(function (e) {
        if (e.which == 13) {
            LoginRequest();
        }
    });

    setTimeout(function () {
        $('input#username').focus();
    }, 100);

    $('input[name="daterange"]').daterangepicker({
        opens: 'left',
        locale: {
            format: 'DD/MM/YYYY'
        }
    });

    $('input[name="taxnumber"]').blur(function () {
        $('.imgLoading').toggleClass('d-none');

        $.ajax({
            type: 'POST',
            url: '/Home/LoadInfo',
            data: {
                'taxnumber': $('input[name="taxnumber"]').val()
            },
            success: function (data) {
                if (data.obj) {
                    $('[name="companyname"]').val(data.obj.COMNAME);
                } else {
                    $('[name="companyname"]').val('{Không tìm thấy đơn vị}');

                }

                $('.imgLoading').toggleClass('d-none');
            },
            error: function () {
                alert("Lỗi tìm kiếm");
                $('.imgLoading').toggleClass('d-none');

            }
        });
    });

    var searchInvoice = function (page) {
        if (!$('input[name="daterange"]').val()) {
            alert('Vui lòng nhập vào khoảng thời gian cần xem');
            $('input[name="daterange"]').focus();
            return false;
        }

        if (!$('input[name="taxnumber"]').val() && !$('input[name="companyname"]').val()) {
            alert('Vui lòng nhập mã số thuế hoặc tên đơn vị');
            $('input[name="daterange"]').focus();
            return false;
        }

        $('.imgLoading').toggleClass('d-none');

        $.ajax({
            type: 'POST',
            url: '/Home/Search',
            data: {
                'taxnumber': $('input[name="taxnumber"]').val(),
                'companyname': $('input[name="companyname"]').val(),
                'daterange': $('input[name="daterange"]').val(),
                'page': page
            },
            success: function (data) {
                if (data.list) {
                    $('#resultBox').html(data.list);
                    $('#resultPages').html(data.pages);
                } else
                    alert(data.msg);

                $('.imgLoading').toggleClass('d-none');

            },
            error: function () {
                alert("Lỗi tìm kiếm");
                $('.imgLoading').toggleClass('d-none');

            }
        });
    }

    $('#btnSubmit').click(function () {
        searchInvoice(1);
    });

    $(document).on("click", ".page", function () {
        searchInvoice($(this).attr('data-page'));
    });

    $('#btnExport').click(function () {
        if (!$('input[name="daterange"]').val()) {
            alert('Vui lòng nhập vào khoảng thời gian cần xem');
            $('input[name="daterange"]').focus();
            return false;
        }

        if (!$('input[name="taxnumber"]').val() && !$('input[name="companyname"]').val()) {
            alert('Vui lòng nhập mã số thuế hoặc tên đơn vị');
            $('input[name="daterange"]').focus();
            return false;
        }

        var datasend = {
            'taxnumber': $('input[name="taxnumber"]').val(),
            'companyname': $('input[name="companyname"]').val(),
            'daterange': $('input[name="daterange"]').val()
        };

        var action = "/Home/ExportExcel";
        var dialog = $.fileDownload(action, {
            httpMethod: "POST",
            data: datasend,
            preparingMessageHtml: "Đang tải file vui lòng đợi...",
            failMessageHtml: "Có lỗi trong khi tải file excel."
        });
    });

})(jQuery);
