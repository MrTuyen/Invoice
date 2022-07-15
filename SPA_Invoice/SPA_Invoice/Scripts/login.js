gapi.load('auth2', function () {
    auth2 = gapi.auth2.init({
        client_id: '958337842953-4aelsfbirqv0e2e2lk1akdboghlpsr7t.apps.googleusercontent.com',
        cookiepolicy: 'none'
    });
    attachSignin(document.getElementById('googleLogin'));
});

// Validate phonenumber
function validatePhone(txtPhone) {
    var filter = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/;
    if (filter.test(txtPhone)) {
        return true;
    }
    else {
        return false;
    }
}

// Validate Email
function validateEmail(email) {
    var emailReg = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return emailReg.test(email);
}

function attachSignin(element) {
    auth2.attachClickHandler(element, {},
        function (googleUser) {
            var profile = googleUser.getBasicProfile();
            var userLogin = {};
            userLogin.ID = profile.getId();
            userLogin.Name = profile.getName();
            userLogin.Avarta = profile.getImageUrl();
            userLogin.Email = profile.getEmail();
            userLogin.utmsource = 'google';
            
            //Hiển thị loading
            showLoading();           
            $.post('/user-account/quick-login', userLogin, function (result) {       
                if (result.User != null) {
                    if (!validatePhone(result.User.PHONENUMBER)) {
                        window.location.href = '/user-account/phonenumber/?email=' + userLogin.Email;
                    } else {
                        toastr.success('Đăng nhập thành công!', { "timeOut": "2000" });
                        sessionStorage.setItem("userNameSS", result.User.USERNAME);
                        window.location.href = '/#!/tong-quan';
                    }   
                }    
                else {
                    window.location.href = '/user-account/phonenumber/?email=' + userLogin.Email;
                }   
                //ẩn loading
                hideLoading();
            });
        },
        function (error) {
            //ẩn loading
            hideLoading();
        }
    );
}

function ResetData() {
    $('input#username', 'input#password').val("");
}

function LoginRequest() {
    //ResetData();
    if (!$('input#username').val()) {
        toastr.warning('Vui lòng nhập vào email hoặc số điện thoại');   
        $('input#username').focus();
        return false;
    }

    if (!$('input#password').val()) {
        toastr.warning('Vui lòng nhập vào mật khẩu');
        $('input#password').focus();
        return false;
    }

    //Loading show
    showLoading();

    var pathname = window.location.pathname; // Returns path only
    $.ajax({
        type: 'POST',
        url: pathname + '/',
        data: { 'username': $('input#username').val(), 'password': $('input#password').val() },
        dataType: 'json',
        success: function (data) {
            if (data.rs) {                
                toastr.options = { "timeOut": "2000" };
                toastr.success('Đăng nhập thành công!');
                sessionStorage.setItem("userNameSS", data.objUser.USERNAME);
                window.location.href = '/#!/tong-quan';
            }
            else {
                toastr.warning(data.msg);

                //ẩn loading
                hideLoading();
            }
        },
        error: function () {
            //ẩn loading
            hideLoading();
        }
    });
};

function RegisterRequest() {
    //Loading
    showLoading();

    var pathname = window.location.pathname; // Returns path only
    $.ajax({
        type: 'POST',
        url: pathname + '/',
        data: { 'phonenumber': $('input#phonenumber').val(), 'email': $('input#email').val(), 'password': $('input#password').val() },
        dataType: 'json',
        success: function (data) {
            if (data.rs)
                window.location.href = '/#!/tong-quan';
            else {
                if (data.msg)
                    toastr.warning(data.msg);
                else
                    toastr.warning('Thông tin đăng nhập không đúng vui lòng kiểm tra lại');
                //ResetData();
                //$('span.focus-input100').show();
                //loading
                hideLoading();
            }
        },
        error: function () {
            //loading
            hideLoading();
        }
    });
};

function ForgotPasswordRequest() {
   
    var pathname = window.location.pathname; // Returns path only
    if ($('input#phone-email').val() == "") {
        toastr.warning('Email hoặc số điện thoại đang để trống!');
    }
    else {
        if (validateEmail($('input#phone-email').val()) || validatePhone($('input#phone-email').val())) {
            showLoading();
            $.ajax({
                type: 'POST',
                url: pathname + '/',
                data: { 'username': $('input#phone-email').val() },
                dataType: 'json',
                success: function (data) {
                    if (data.rs) {                       
                        localStorage.setItem('username', data.data);
                        hideLoading();
                        window.location.href = '/user-account/resend-email';
                    }
                    else {              
                        hideLoading();
                        toastr.info(data.msg);
                    }
                },
                error: function () {
                    
                }
            });
        }
        else {
            toastr.info('Email hoặc số điện thoại không hợp lệ!')
        }
        
    }   
};

$('#phone-email').keypress(function (e) {
    if (e.which == 13) {
        ForgotPasswordRequest();
        event.preventDefault();
    }
});
$('#email-phone').html(localStorage.getItem('username'));

function UpdatePhoneNumber() {
    
    if ($('input#phonenumber').val() == "") {
        toastr.warning("Số điện thoại đang để trống!");
    } else {
        if (validatePhone($('input#phonenumber').val())) {           
            var pathname = window.location.pathname; // Returns path only
            $.ajax({
                type: 'POST',
                url: pathname + '/',
                data: { 'phonenumber': $('input#phonenumber').val(), 'email': $('input#login-email').val() },
                dataType: 'json',
                success: function (data) {
                    if (data.rs)
                        window.location.href = '/#!/tong-quan';
                    else toastr.warning(data.msg);
                },
                error: function () {
                }
            });
        }
        else {
            toastr.warning('Số điện thoại không đúng định dạng!')
        }
    }
   
}

var loadingSvg = $('.btn-register img');
function showLoading() {
    //Hiển thị loading
    loadingSvg.show();
    //Disable button
    $('button').attr('disabled', 'disabled');
}

function hideLoading() {
    //Hiển thị loading
    loadingSvg.hide();
    //Disable button
    $('button').removeAttr('disabled');
}

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

    //$('input#username').click(function () {
    //    $('span.focus-input100').hide();
        
    //});
  

    $("input#username") .on('touchstart click', function (e) {
            e.preventDefault();
            var touch = e.touches[0];
            if (touch) {
                $('span.focus-input100').hide();
            }
            else {
                $('span.focus-input100').show();
            }
    });

    $("input#password").on('touchstart click', function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        if (touch) {
            $('span.focus-input100').hide();
        }
        else {
            $('span.focus-input100').show();
        }
    });

    $('#btn-login').click(function () {        
        LoginRequest();
    });

    $('input#username').keypress(function (e) {
        //$('span.focus-input100').hide();
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

    $('#btn-send-email').click(function () {        
        var count = 30;
        var IPause = true;
        $('#btn-send-email').attr('disabled', 'disabled');
        $('#count').removeClass('display-none');
        var doUpdate = function () {
            if (IPause) {
                if (count!=0) {
                    $('#count').html(count); 
                }                                                               
                if (count == 0) {
                    $('#btn-send-email').removeAttr('disabled');
                    $('#count').addClass('display-none');
                    IPause = false;
                }
                count--;
            }
            
        };

        // Schedule the update to happen once every second
        setInterval(doUpdate, 1000);
    });

    $('#phonenumber').keypress(function (e) {
        if (e.which == 13) {
            UpdatePhoneNumber();
            event.preventDefault();
        }
    });
    setTimeout(function () {
        $('input#username').focus();
    }, 100);
    setTimeout(function () {
        $('input#phone-email').focus();
    }, 100);
})(jQuery);
