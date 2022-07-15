/*
 * TRUONGnv 20200404
 * Đối tượng JSON để xử lý Date + BigNumber
 * required: Decimal.js
 */
var MISAJSON = {
    /*
     * Do 1 số trường hợp kiểm tra kiểu instanceof Decimal đang trả về sai -> tạm fix or thêm DK: typeof obj.lessThan === 'function' && typeof obj.lt === 'function' sẽ xem xét lại sau
     */
    type: (function () {
        var class2type = {};
        var arr = "Boolean Number String Function Array Date RegExp Object".split(" ");
        for (var n = 0; n < arr.length; n++) {
            class2type["[object " + arr[n] + "]"] = arr[n].toLowerCase();
        }
        return function (obj) {
            var res = '';
            if (obj == null) {
                res = String(obj);
            } else if (obj instanceof Decimal || (typeof obj.lessThan === 'function' && typeof obj.lt === 'function')) {
                res = 'decimal';
            } else {
                res = class2type[Object.prototype.toString.call(obj)] || "object";
            }

            return res;
        }
    })(),

    stringify: (function () {
        function formatDate(n) {
            return n < 10 ? '0' + n : n
        }
        var quote = (function () {
            var escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
                meta = {
                    '\b': '\\b',
                    '\t': '\\t',
                    '\n': '\\n',
                    '\f': '\\f',
                    '\r': '\\r',
                    '"': '\\"',
                    '\\': '\\\\'
                };
            return function (string) {
                escapable.lastIndex = 0;
                return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                    var c = meta[a];
                    return typeof c === 'string' ? c : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4)
                }) + '"' : '"' + string + '"'
            }
        })();
        return function (value) {
            var arr = [], strRet, type = this.type(value);
            if (type === "date") {
                if (isFinite(value.valueOf())) {
                    var temp = value.clone();
                    temp.addHours(temp.getTimezoneOffset() / -60);

                    value = temp.toJSON();
                } else {
                    value = null;
                }
            }
            type = this.type(value);
            switch (type) {
                case 'number':
                    strRet = isFinite(value) ? value : String(null);
                    break;
                case 'boolean':
                    strRet = String(value);
                    break;
                case 'string':
                    strRet = quote(value);
                    break;
                case 'undefined':
                case 'null':
                    strRet = 'null';
                    break;
                case 'array':
                    for (var n = 0; n < value.length; n++) {
                        arr.push(this.stringify(value[n]));
                    }
                    strRet = "[" + arr.join(",") + "]";
                    break;
                case 'decimal':
                    strRet = value.toJSON();
                    break;
                default:
                    for (var i in value) {
                        if (Object.prototype.hasOwnProperty.call(value, i)) {
                            if (this.type(value[i]) !== "undefined") {
                                arr.push(quote(i) + ":" + this.stringify(value[i]));
                            }
                        }
                    }
                    strRet = "{" + arr.join(",") + "}";
                    break;
            }

            return strRet;
        }
    })(),

    parse: (function () {
        "use strict";

        // This is a function that can parse a JSON text, producing a JavaScript
        // data structure. It is a simple, recursive descent parser. It does not use
        // eval or regular expressions, so it can be used as a model for implementing
        // a JSON parser in other languages.

        // We are defining the function inside of another function to avoid creating
        // global variables.

        var at,     // The index of the current character
            ch,     // The current character
            escapee = {
                '"': '"',
                '\\': '\\',
                '/': '/',
                b: '\b',
                f: '\f',
                n: '\n',
                r: '\r',
                t: '\t'
            },
            text,

            error = function (m) {

                // Call error when something is wrong.

                throw {
                    name: 'SyntaxError',
                    message: m,
                    at: at,
                    text: text
                };
            },

            next = function (c) {

                // If a c parameter is provided, verify that it matches the current character.

                if (c && c !== ch) {
                    error("Expected '" + c + "' instead of '" + ch + "'");
                }

                // Get the next character. When there are no more characters,
                // return the empty string.

                ch = text.charAt(at);
                at += 1;
                return ch;
            },

            number = function () {
                // Parse a number value.
                var number,
                    string = '';

                if (ch === '-') {
                    string = '-';
                    next('-');
                }
                while (ch >= '0' && ch <= '9') {
                    string += ch;
                    next();
                }
                if (ch === '.') {
                    string += '.';
                    while (next() && ch >= '0' && ch <= '9') {
                        string += ch;
                    }
                }
                if (ch === 'e' || ch === 'E') {
                    string += ch;
                    next();
                    if (ch === '-' || ch === '+') {
                        string += ch;
                        next();
                    }
                    while (ch >= '0' && ch <= '9') {
                        string += ch;
                        next();
                    }
                }

                if (/[0123456789\.\,\-]$/.test(string)) {
                    var idx = string.indexOf('.');
                    if (idx > -1) {
                        var c = 0;
                        for (var i = string.length - 1; i >= idx; i--) {
                            if (string[i] == '0') {
                                c++;
                            } else if (string[i] == '.') {
                                c++;
                                break;
                            } else {
                                break;
                            }
                        }

                        if (c > 0) {
                            string = string.substr(0, string.length - c);
                        }
                    }

                    number = parseFloat(string);
                    if (number !== 0 && String(number) !== string) {
                        number = new Decimal(string);
                    }
                }
                return number;
            },

            string = function () {

                // Parse a string value.

                var hex,
                    i,
                    string = '',
                    uffff;

                // When parsing for string values, we must look for " and \ characters.

                if (ch === '"') {
                    while (next()) {
                        if (ch === '"') {
                            next();
                            /*
                             * nếu string là date.toJSON -> cast về dat
                             * Kiểm tra dateString -> convert
                             * regex
                             * Đã có regex check -> bỏ if kiểm tra JSON
                             * Sửa loại đoạn regular cho đúng
                             * Đáp ứng các định dang sau:
                             * 2015-12-31T10:42:48.996
                             * 2015-12-09T00:00:00
                             * 2015-12-09T00:00:00Z
                             * 2015-12-31T11:49:16.8422164+07:00
                             * 2015-01-01T00:00:00.000Z
                             */
                            var res = string,
                                regular = /^\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(.)?(\d{0,9})?(Z)?(\+)?(\d{2})?(:)?(\d{2})?$/;
                            if (string && regular.test(string)) {
                                res = MISAJSON.parseDateWithProcessTimeZone(string);
                            }

                            return res;
                        }

                        if (ch === '\\') {
                            next();
                            if (ch === 'u') {
                                uffff = 0;
                                for (var i = 0; i < 4; i += 1) {
                                    hex = parseInt(next(), 16);
                                    if (!isFinite(hex)) {
                                        break;
                                    }
                                    uffff = uffff * 16 + hex;
                                }
                                string += String.fromCharCode(uffff);
                            } else if (typeof escapee[ch] === 'string') {
                                string += escapee[ch];
                            } else {
                                break;
                            }
                        } else {
                            string += ch;
                        }
                    }
                }
                error("Bad string");
            },

            white = function () {

                // Skip whitespace.

                while (ch && ch <= ' ') {
                    next();
                }
            },

            word = function () {

                // true, false, or null.

                switch (ch) {
                    case 't':
                        next('t');
                        next('r');
                        next('u');
                        next('e');
                        return true;
                    case 'f':
                        next('f');
                        next('a');
                        next('l');
                        next('s');
                        next('e');
                        return false;
                    case 'n':
                        next('n');
                        next('u');
                        next('l');
                        next('l');
                        return null;
                }
                error("Unexpected '" + ch + "'");
            },

            value,  // Place holder for the value function.

            array = function () {

                // Parse an array value.

                var array = [];

                if (ch === '[') {
                    next('[');
                    white();
                    if (ch === ']') {
                        next(']');
                        return array;   // empty array
                    }
                    while (ch) {
                        array.push(value());
                        white();
                        if (ch === ']') {
                            next(']');
                            return array;
                        }
                        next(',');
                        white();
                    }
                }
                error("Bad array");
            },

            object = function () {

                // Parse an object value.

                var key,
                    object = {};

                if (ch === '{') {
                    next('{');
                    white();
                    if (ch === '}') {
                        next('}');
                        return object;   // empty object
                    }
                    while (ch) {
                        key = string();
                        white();
                        next(':');
                        if (Object.hasOwnProperty.call(object, key)) {
                            error('Duplicate key "' + key + '"');
                        }
                        object[key] = value();
                        white();
                        if (ch === '}') {
                            next('}');
                            return object;
                        }
                        next(',');
                        white();
                    }
                }
                error("Bad object");
            };

        value = function () {

            // Parse a JSON value. It could be an object, an array, a string, a number,
            // or a word.

            white();
            switch (ch) {
                case '{':
                    return object();
                case '[':
                    return array();
                case '"':
                    return string();
                case '-':
                    return number();
                default:
                    return ch >= '0' && ch <= '9'
                        ? number()
                        : word();
            }
        };

        // Return the json_parse function. It will have access to all of the above
        // functions and variables.
        // Thêm case nếu source không xác định -> return source
        return function (source, reviver) {
            if (!source) {
                return source;
            }

            var result;

            text = source;
            at = 0;
            ch = ' ';
            result = value();
            white();
            if (ch) {
                error("Syntax error");
            }

            // If there is a reviver function, we recursively walk the new structure,
            // passing each name/value pair to the reviver function for possible
            // transformation, starting with a temporary root object that holds the result
            // in an empty key. If there is not a reviver function, we simply return the
            // result.

            return typeof reviver === 'function'
                ? (function walk(holder, key) {
                    var k, v, value = holder[key];
                    if (value && typeof value === 'object') {
                        for (k in value) {
                            if (Object.prototype.hasOwnProperty.call(value, k)) {
                                v = walk(value, k);
                                if (v !== undefined) {
                                    value[k] = v;
                                } else {
                                    delete value[k];
                                }
                            }
                        }
                    }
                    return reviver.call(holder, key, value);
                }({ '': result }, ''))
                : result;
        };
    }()),

    /*
     * parse string to date trong đó có xử lý quy hết về timezone chuẩn
     * @param 
     */
    parseDateWithProcessTimeZone: function (value) {
        var result = new Date(value);
        return result;
    },
};