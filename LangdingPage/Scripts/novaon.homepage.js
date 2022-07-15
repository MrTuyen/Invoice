$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $("img.lazyload").lazyload({
        effect: "fadeIn"
    });

    //Keyvisual click
    $('.lstKeyvisual li').click(function () {
        $('.lstKeyvisual li').removeClass('actived');
        $(this).addClass('actived');
        var source = $(this).attr('data-resource');
        $('#viewKeyvisual').attr('src', source);
    });

    // Close menu mobile
    if ($("#navbarMainMenu").length > 0) {
        $("button.navbar-toggler").click(function () {
            $("#overlay").toggleClass("show");
            // $('#navbarMainMenu').addClass('show');
        });

        $("#navbarMainMenu i.close").click(function () {
            $("#overlay").removeClass("show");
            $(this)
                .parent("#navbarMainMenu")
                .removeClass("show");
        });
        $(window).click(function (e) {
            if (
                $(e.target).attr("id") == "overlay" &&
                $("#navbarMainMenu").hasClass("show") &&
                $(e.target).parents("#navbarMainMenu").length == 0
            ) {
                $("#overlay").removeClass("show");
                $("#navbarMainMenu").removeClass("show");
            }
        });
    }


    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        jQuery('.hide-on-mobile').css('display', 'none');
        jQuery('.show-hide-column-table').css('display', 'block');
        jQuery('.show-hide-column-table .default').trigger('click');
    }

    //Inlude file
    $('#header-new').load('/header.html?v=1');
    $('#boxreviewer').load('/review.html');
    $('footer.footer').load('/footer.html');
});