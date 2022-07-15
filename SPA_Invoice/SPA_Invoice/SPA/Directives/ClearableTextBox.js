app.directive('clearabletextbox', function ($timeout) {
    return {
        restrict: "E",

        scope: {
            iconposition: "@",
            textboxtype: "@",
            textboxplaceholder: "@",
            textboxmodel: "=",
            textboxmaxlength: "@",
            resultchange: "&",
            resultchangedelay: "@",
            resultchangeminlength: "@",
            submitonenter: "&",
            cleartext: "&",
            blur: "&",
            disable: "=",
            textalignment: "@",
            isnumberformat: "=",
            isnumberformat3digit: "=",
            islargetext: "=",
            disablenumber: "=",
            allowonlynumber: "=",
            nfocus: "=",
            isinteger: "=",
            focustextbox: "=",
            textview: "=?"
        },

        transclude: true,
        templateUrl: "/Directives/ClearableTextBox.html?v=2",

        link: function (scope, element, attrs) {

            //#region Variable

            var submissionHandler = null;
            scope.textboxmodel = scope.textview + "";   //Parse to string
            
            //#endregion

            //#region Function

            //#region Support

            var formatNumber = function () {
                if ((typeof scope.textboxmodel === "string"))
                    scope.textboxmodel = scope.textboxmodel.trim();

                if (scope.textboxmodel.length > 1) {
                    while (scope.textboxmodel.charAt(0) == "0" && scope.textboxmodel.charAt(1) != ".") {
                        scope.textboxmodel = scope.textboxmodel.substr(1);
                    }

                    var field = scope.textboxmodel.replace(/[^\d.\','] /g, "");
                    var point = field.indexOf(".");

                    if (point >= 0) {
                        field = field.slice(0, point + 3);
                    }

                    var decimalSplit = field.split(".");
                    var intPart = decimalSplit[0];
                    var decPart = decimalSplit[1];

                    intPart = intPart.replace(/[^\d]/g, "");

                    if (intPart.length > 3) {
                        var intDiv = Math.floor(intPart.length / 3);

                        while (intDiv > 0) {
                            var lastComma = intPart.indexOf(",");

                            if (lastComma < 0) {
                                lastComma = intPart.length;
                            }

                            if (lastComma - 3 > 0) {
                                intPart = intPart.substr(0, lastComma - 3) + "," + intPart.substr(lastComma - 3);//intPart.splice(lastComma - 3, 0, ",");
                            }

                            --intDiv;
                        }
                    }

                    if (decPart === undefined) {
                        decPart = "";
                    } else {
                        decPart = "." + decPart;
                    }

                    var res = intPart + decPart;

                    scope.textboxmodel = res;
                    try {
                        var inputelement = element[0].children[0].children[0];
                        // Cache references
                        var $el = $(inputelement),
                            el = inputelement;

                        // Only focus if input isn't already
                        if (!$el.is(":focus")) {
                            $el.focus();
                        }

                        // If this function exists... (IE 9+)
                        if (el.setSelectionRange) {

                            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                            var len = $el.val().length * 2;

                            // Timeout seems to be required for Blink
                            setTimeout(function () {
                                el.setSelectionRange(len, len);
                            }, 1);

                        } else {
                            // As a fallback, replace the contents with itself
                            // Doesn't work in Chrome, but Chrome supports setSelectionRange
                            $el.val($el.val());

                        }

                        // Scroll to the bottom, in case we're in a tall textarea
                        // (Necessary for Firefox and Chrome)
                        this.scrollTop = 999999;


                    } catch (e) {

                    }

                }
            }

            var formatNumber3Digit = function () {
                if ((typeof scope.textboxmodel === "string"))
                    scope.textboxmodel = scope.textboxmodel.trim();

                if (scope.textboxmodel.length > 1) {
                    while (scope.textboxmodel.charAt(0) == "0" && scope.textboxmodel.charAt(1) != ".") {
                        scope.textboxmodel = scope.textboxmodel.substr(1);
                    }

                    var field = scope.textboxmodel.replace(/[^\d.\','] /g, "");
                    var point = field.indexOf(".");

                    if (point >= 0) {
                        // configure the number after the dot
                        //field = field.slice(0, point + 4);
                        field = field.slice(0, point + 10);
                    }

                    var decimalSplit = field.split(".");
                    var intPart = decimalSplit[0];
                    var decPart = decimalSplit[1];

                    intPart = intPart.replace(/[^\d]/g, "");

                    if (intPart.length > 3) {
                        var intDiv = Math.floor(intPart.length / 3);

                        while (intDiv > 0) {
                            var lastComma = intPart.indexOf(",");

                            if (lastComma < 0) {
                                lastComma = intPart.length;
                            }

                            if (lastComma - 3 > 0) {
                                intPart = intPart.substr(0, lastComma - 3) + "," + intPart.substr(lastComma - 3);//intPart.splice(lastComma - 3, 0, ",");
                            }

                            --intDiv;
                        }
                    }

                    if (decPart === undefined) {
                        decPart = "";
                    } else {
                        decPart = "." + decPart;
                    }

                    var res = intPart + decPart;

                    scope.textboxmodel = res;
                    //scope.textview = res;
                    try {
                        var inputelement = element[0].children[0].children[0];
                        // Cache references
                        var $el = $(inputelement),
                            el = inputelement;

                        // Only focus if input isn't already
                        if (!$el.is(":focus")) {
                            $el.focus();
                        }

                        // If this function exists... (IE 9+)
                        if (el.setSelectionRange) {

                            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                            var len = $el.val().length * 2;

                            // Timeout seems to be required for Blink
                            setTimeout(function () {
                                el.setSelectionRange(len, len);
                            }, 1);

                        } else {
                            // As a fallback, replace the contents with itself
                            // Doesn't work in Chrome, but Chrome supports setSelectionRange
                            $el.val($el.val());

                        }

                        // Scroll to the bottom, in case we're in a tall textarea
                        // (Necessary for Firefox and Chrome)
                        this.scrollTop = 999999;


                    } catch (e) {

                    }

                }
            }

            var submit = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                if (!scope.resultchangeminlength || (scope.textboxmodel && scope.textboxmodel.trim().length >= scope.resultchangeminlength)) {
                    submissionHandler = $timeout(function () {
                        scope.resultchange();
                    }, scope.resultchangedelay);
                }
            };

            //#endregion

            //#region Verify

            //#endregion

            //#region Logic

            scope.Change = function () {
                if (scope.textboxmodel == null) {
                    scope.textboxmodel = "";
                }

                if (scope.isnumberformat) {
                    if (isNaN(parseFloat(scope.textboxmodel))) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }
                if (scope.isnumberformat3digit) {
                    if (isNaN(parseFloat(scope.textboxmodel))) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber3Digit();
                    }
                }
                if (scope.isinteger) {
                    var transformedInput = scope.textboxmodel.replace(/[^0-9]/g, '');
                    scope.textboxmodel = scope.textboxmodel.replace(',', '');
                    // transformedInput.replace(' ', '');
                    if (transformedInput !== scope.textboxmodel) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }

                if (scope.resultchangedelay) {
                    submit();
                } else if (scope.resultchange) {
                    $timeout(scope.resultchange);
                }
            }

            //Gọi lại hàm khi cần cho trường hợp xem lại có sẵn dữ liệu
            scope.Change();

            scope.KeyPress = function (event) {
                if (scope.submitonenter && (event.keyCode == 13 || event.which == 13)) {
                    $timeout(scope.submitonenter);
                }
            }

            scope.KeyDown = function (event) {
                if (scope.isnumberformat || scope.isnumberformat3digit || scope.textboxtype == "tel") {
                    //if ((!event.ctrlKey && (event.keyCode < 48 || event.keyCode > 57) &&
                    //    event.keyCode != 46 && event.keyCode != 13 && event.keyCode != 8 &&
                    //    event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 46 && event.keyCode != 8 &&
                    //    (event.keyCode != 190 || scope.textboxmodel.indexOf(".") > -1)) ||
                    //    event.shiftKey) {
                    //    event.preventDefault();
                    //}
                    var allow = ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 39 || event.keyCode == 37
                        || event.keyCode == 35 || event.keyCode == 36 || event.keyCode == 190 || event.keyCode == 110 || event.keyCode == 9 || event.keyCode == 33 || event.keyCode == 34
                        || event.keyCode == 8 || event.keyCode == 46 || event.ctrlKey);

                    if (!allow) {
                        event.preventDefault();
                    }
                }

                if (scope.disablenumber && event.keyCode > 48 && event.keyCode < 57) {
                    event.preventDefault();
                }

                if (scope.textboxmodel && scope.textboxmodel.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8) {
                    event.preventDefault();
                }
            }

            scope.Clear = function () {
                scope.textboxmodel = "";

                if (scope.cleartext) {
                    $timeout(scope.cleartext);
                }
            }

            scope.Blur = function () {
                if (scope.blur) {
                    $timeout(scope.blur);
                }

            }

            //#endregion

            //#endregion

            //#region Init

            var initialise = function () {
                if (scope.isnumberformat && scope.textboxtype != 'number') {
                    scope.type = "tel";
                } else {
                    scope.type = scope.textboxtype;
                }

                scope.textStyle = "padding-";
                scope.iconStyle = "position:absolute;font-size:20px;line-height:40px;height:40px;width:34px;color:black;";

                if (scope.iconposition == "left") {
                    scope.textStyle = scope.textStyle + "left:0px;text-align:right";
                    scope.iconStyle = scope.iconStyle + "left";
                } else {
                    scope.textStyle = scope.textStyle + "right:0px;text-align:left";
                    scope.iconStyle = scope.iconStyle + "right";
                }

                scope.iconStyle = scope.iconStyle + ":0";

                if (scope.islargetext) {
                    scope.textStyle = scope.textStyle + ";font-size:20px";
                }

                scope.maxLength = 100;
                if (scope.nfocus) {
                    scope.focus = scope.nfocus;
                }
                if (scope.textboxmaxlength) {
                    scope.maxLength = scope.textboxmaxlength;
                }

                if (scope.resultchangedelay) {
                    if (scope.resultchangeminlength) {
                        scope.resultchangeminlength = parseInt(scope.resultchangeminlength);
                    } else {
                        scope.resultchangeminlength = 0;
                    }
                }

                if (!scope.iconposition) {
                    scope.iconposition = "right";
                }
            }

            initialise();

            //#endregion
        },
    }
});