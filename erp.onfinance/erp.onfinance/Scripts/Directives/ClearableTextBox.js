
app.directive('clearabletextbox', function ($timeout) {
    return {
        restrict: "E",

        scope: {
            iconposition: "@",
            textboxtype: "@",
            textboxstyle: "@",
            textboxplaceholder: "@",
            //textboxmodel: "=",
            valuemodel: "=",
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
            islargetext: "=",
            isclickclear: "=",
            disablenumber: "=",
            allowonlynumber: "=",
            nfocus: "=",
            isinteger: "=",
            focustextbox: "="
        },

        transclude: true,
        templateUrl: "/Directives/ClearableTextBox.html?v=2",

        link: function (scope, element, attrs) {

            //#region Variable

            var submissionHandler = null;

            //#endregion

            //#region Function

            //#region Support

            var formatNumber = function () {
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
                    scope.modelnotcomma = 1000;
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

                if (scope.isinteger) {
                    var transformedInput = scope.textboxmodel.replace(/[^0-9]/g, '');
                    scope.textboxmodel = scope.textboxmodel.replace(',', '');
                    // transformedInput.replace(' ', '');
                    if (transformedInput != scope.textboxmodel) {
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

                //Gán giá trị không có định dạng
                var NoneDim = scope.textboxmodel.replace(/[^\d]/g, "");
                if (isNaN(NoneDim))
                    scope.valuemodel = 0;
                else
                    scope.valuemodel = parseFloat(NoneDim);
            }

            scope.KeyPress = function (event) {
                if (scope.submitonenter && (event.keyCode == 13 || event.which == 13)) {
                    $timeout(scope.submitonenter);
                }
            }

            scope.KeyDown = function (event) {
                if (scope.isnumberformat || scope.textboxtype == "tel") {
                    if ((!event.ctrlKey && (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105) &&
                        event.keyCode != 46 && event.keyCode != 13 && event.keyCode != 8 &&
                        event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 46 && event.keyCode != 8 &&
                        (event.keyCode != 190 || scope.textboxmodel.indexOf(".") > -1)) ||
                        event.shiftKey) {
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

            scope.ClickClear = function () {
                if (scope.isclickclear) {
                    scope.textboxmodel = "";
                    scope.valuemodel = "";
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

                scope.textStyle = scope.textboxstyle + ";padding-";
                scope.iconStyle = "position:absolute; font-size:20px; line-height:35px; height:40px; width:34px; color:black;";

                if (scope.iconposition == "left") {
                    scope.textStyle = scope.textStyle + "left:40px;text-align:right";
                    scope.iconStyle = scope.iconStyle + "left";
                } else {
                    scope.textStyle = scope.textStyle + "right:40px;text-align:left";
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