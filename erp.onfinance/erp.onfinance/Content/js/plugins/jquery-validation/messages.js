(function( factory ) {
	if ( typeof define === "function" && define.amd ) {
		define( ["jquery", "../jquery.validate"], factory );
	} else if (typeof module === "object" && module.exports) {
		module.exports = factory( require( "jquery" ) );
	} else {
		factory( jQuery );
	}
}(function( $ ) {

$.extend( $.validator.messages, {
	required: "Vui lòng nhập",
	remote: "Hãy sửa cho đúng.",
	email: "Email không hợp lệ",
	url: "Url không hợp lệ",
	date: "ngày không hợp lệ",
	dateISO: "Hãy nhập ngày (ISO).",
	number: "Vui lòng nhập số",
	digits: "Vui lòng nhập chữ số",
	creditcard: "Hãy nhập số thẻ tín dụng.",
	equalTo: "Không trùng khớp",
	extension: "Phần mở rộng không đúng.",
	maxlength: $.validator.format( "Hãy nhập từ {0} kí tự trở xuống." ),
	minlength: $.validator.format( "Hãy nhập từ {0} kí tự trở lên." ),
	rangelength: $.validator.format( "Hãy nhập từ {0} đến {1} kí tự." ),
	range: $.validator.format( "Hãy nhập từ {0} đến {1}." ),
	max: $.validator.format( "Hãy nhập từ {0} trở xuống." ),
	min: $.validator.format("Hãy nhập từ {1} trở lên.")
} );
return $;
}));