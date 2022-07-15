/*
 * jQuery File Upload Plugin 5.26
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MIT
 */

/*jslint nomen: true, unparam: true, regexp: true */
/*global define, window, document, File, Blob, FormData, location */

(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        // Register as an anonymous AMD module:
        define([
            'jquery',
            'jquery.ui.widget'
        ], factory);
    } else {
        // Browser globals:
        factory(window.jQuery);
    }
}(function ($) {
    'use strict';

    // The FileReader API is not actually used, but works as feature detection,
    // as e.g. Safari supports XHR file uploads via the FormData API,
    // but not non-multipart XHR file uploads:
    $.support.xhrFileUpload = !!(window.XMLHttpRequestUpload && window.FileReader);
    $.support.xhrFormDataFileUpload = !!window.FormData;

    // The fileupload widget listens for change events on file input fields defined
    // via fileInput setting and paste or drop events of the given dropZone.
    // In addition to the default jQuery Widget methods, the fileupload widget
    // exposes the "add" and "send" methods, to add or directly send files using
    // the fileupload API.
    // By default, files added via file input selection, paste, drag & drop or
    // "add" method are uploaded immediately, but it is possible to override
    // the "add" callback option to queue file uploads.
    $.widget('blueimp.fileupload', {

        options: {
            // The drop target element(s), by the default the complete document.
            // Set to null to disable drag & drop support:
            dropZone: $(document),
            // The paste target element(s), by the default the complete document.
            // Set to null to disable paste support:
            pasteZone: $(document),
            // The file input field(s), that are listened to for change events.
            // If undefined, it is set to the file input fields inside
            // of the widget element on plugin initialization.
            // Set to null to disable the change listener.
            fileInput: undefined,
            // By default, the file input field is replaced with a clone after
            // each input field change event. This is required for iframe transport
            // queues and allows change events to be fired for the same file
            // selection, but can be disabled by setting the following option to false:
            replaceFileInput: true,
            // The parameter name for the file form data (the request argument name).
            // If undefined or empty, the name property of the file input field is
            // used, or "files[]" if the file input name property is also empty,
            // can be a string or an array of strings:
            paramName: undefined,
            // By default, each file of a selection is uploaded using an individual
            // request for XHR type uploads. Set to false to upload file
            // selections in one request each:
            singleFileUploads: true,
            // To limit the number of files uploaded with one XHR request,
            // set the following option to an integer greater than 0:
            limitMultiFileUploads: undefined,
            // Set the following option to true to issue all file upload requests
            // in a sequential order:
            sequentialUploads: false,
            // To limit the number of concurrent uploads,
            // set the following option to an integer greater than 0:
            limitConcurrentUploads: undefined,
            // Set the following option to true to force iframe transport uploads:
            forceIframeTransport: false,
            // Set the following option to the location of a redirect url on the
            // origin server, for cross-domain iframe transport uploads:
            redirect: undefined,
            // The parameter name for the redirect url, sent as part of the form
            // data and set to 'redirect' if this option is empty:
            redirectParamName: undefined,
            // Set the following option to the location of a postMessage window,
            // to enable postMessage transport uploads:
            postMessage: undefined,
            // By default, XHR file uploads are sent as multipart/form-data.
            // The iframe transport is always using multipart/form-data.
            // Set to false to enable non-multipart XHR uploads:
            multipart: true,
            // To upload large files in smaller chunks, set the following option
            // to a preferred maximum chunk size. If set to 0, null or undefined,
            // or the browser does not support the required Blob API, files will
            // be uploaded as a whole.
            maxChunkSize: undefined,
            // When a non-multipart upload or a chunked multipart upload has been
            // aborted, this option can be used to resume the upload by setting
            // it to the size of the already uploaded bytes. This option is most
            // useful when modifying the options object inside of the "add" or
            // "send" callbacks, as the options are cloned for each file upload.
            uploadedBytes: undefined,
            // By default, failed (abort or error) file uploads are removed from the
            // global progress calculation. Set the following option to false to
            // prevent recalculating the global progress data:
            recalculateProgress: true,
            // Interval in milliseconds to calculate and trigger progress events:
            progressInterval: 100,
            // Interval in milliseconds to calculate progress bitrate:
            bitrateInterval: 500,
            // By default, uploads are started automatically when adding files:
            autoUpload: true,

            // Additional form data to be sent along with the file uploads can be set
            // using this option, which accepts an array of objects with name and
            // value properties, a function returning such an array, a FormData
            // object (for XHR file uploads), or a simple object.
            // The form of the first fileInput is given as parameter to the function:
            formData: function (form) {
                return form.serializeArray();
            },

            // The add callback is invoked as soon as files are added to the fileupload
            // widget (via file input selection, drag & drop, paste or add API call).
            // If the singleFileUploads option is enabled, this callback will be
            // called once for each file in the selection for XHR file uplaods, else
            // once for each file selection.
            // The upload starts when the submit method is invoked on the data parameter.
            // The data object contains a files property holding the added files
            // and allows to override plugin options as well as define ajax settings.
            // Listeners for this callback can also be bound the following way:
            // .bind('fileuploadadd', func);
            // data.submit() returns a Promise object and allows to attach additional
            // handlers using jQuery's Deferred callbacks:
            // data.submit().done(func).fail(func).always(func);
            add: function (e, data) {
                if (data.autoUpload || (data.autoUpload !== false &&
                        ($(this).data('blueimp-fileupload') ||
                        $(this).data('fileupload')).options.autoUpload)) {
                    data.submit();
                }
            },

            // Other callbacks:

            // Callback for the submit event of each file upload:
            // submit: function (e, data) {}, // .bind('fileuploadsubmit', func);

            // Callback for the start of each file upload request:
            // send: function (e, data) {}, // .bind('fileuploadsend', func);

            // Callback for successful uploads:
            // done: function (e, data) {}, // .bind('fileuploaddone', func);

            // Callback for failed (abort or error) uploads:
            // fail: function (e, data) {}, // .bind('fileuploadfail', func);

            // Callback for completed (success, abort or error) requests:
            // always: function (e, data) {}, // .bind('fileuploadalways', func);

            // Callback for upload progress events:
            // progress: function (e, data) {}, // .bind('fileuploadprogress', func);

            // Callback for global upload progress events:
            // progressall: function (e, data) {}, // .bind('fileuploadprogressall', func);

            // Callback for uploads start, equivalent to the global ajaxStart event:
            // start: function (e) {}, // .bind('fileuploadstart', func);

            // Callback for uploads stop, equivalent to the global ajaxStop event:
            // stop: function (e) {}, // .bind('fileuploadstop', func);

            // Callback for change events of the fileInput(s):
            // change: function (e, data) {}, // .bind('fileuploadchange', func);

            // Callback for paste events to the pasteZone(s):
            // paste: function (e, data) {}, // .bind('fileuploadpaste', func);

            // Callback for drop events of the dropZone(s):
            // drop: function (e, data) {}, // .bind('fileuploaddrop', func);

            // Callback for dragover events of the dropZone(s):
            // dragover: function (e) {}, // .bind('fileuploaddragover', func);

            // Callback for the start of each chunk upload request:
            // chunksend: function (e, data) {}, // .bind('fileuploadchunksend', func);

            // Callback for successful chunk uploads:
            // chunkdone: function (e, data) {}, // .bind('fileuploadchunkdone', func);

            // Callback for failed (abort or error) chunk uploads:
            // chunkfail: function (e, data) {}, // .bind('fileuploadchunkfail', func);

            // Callback for completed (success, abort or error) chunk upload requests:
            // chunkalways: function (e, data) {}, // .bind('fileuploadchunkalways', func);

            // The plugin options are used as settings object for the ajax calls.
            // The following are jQuery ajax settings required for the file uploads:
            processData: false,
            contentType: false,
            cache: false
        },

        // A list of options that require a refresh after assigning a new value:
        _refreshOptionsList: [
            'fileInput',
            'dropZone',
            'pasteZone',
            'multipart',
            'forceIframeTransport'
        ],

        _BitrateTimer: function () {
            this.timestamp = +(new Date());
            this.loaded = 0;
            this.bitrate = 0;
            this.getBitrate = function (now, loaded, interval) {
                var timeDiff = now - this.timestamp;
                if (!this.bitrate || !interval || timeDiff > interval) {
                    this.bitrate = (loaded - this.loaded) * (1000 / timeDiff) * 8;
                    this.loaded = loaded;
                    this.timestamp = now;
                }
                return this.bitrate;
            };
        },

        _isXHRUpload: function (options) {
            return !options.forceIframeTransport &&
                ((!options.multipart && $.support.xhrFileUpload) ||
                $.support.xhrFormDataFileUpload);
        },

        _getFormData: function (options) {
            var formData;
            if (typeof options.formData === 'function') {
                return options.formData(options.form);
            }
            if ($.isArray(options.formData)) {
                return options.formData;
            }
            if (options.formData) {
                formData = [];
                $.each(options.formData, function (name, value) {
                    formData.push({name: name, value: value});
                });
                return formData;
            }
            return [];
        },

        _getTotal: function (files) {
            var total = 0;
            $.each(files, function (index, file) {
                total += file.size || 1;
            });
            return total;
        },

        _initProgressObject: function (obj) {
            obj._progress = {
                loaded: 0,
                total: 0,
                bitrate: 0
            };
        },

        _onProgress: function (e, data) {
            if (e.lengthComputable) {
                var now = +(new Date()),
                    loaded;
                if (data._time && data.progressInterval &&
                        (now - data._time < data.progressInterval) &&
                        e.loaded !== e.total) {
                    return;
                }
                data._time = now;
                loaded = Math.floor(
                    e.loaded / e.total * (data.chunkSize || data._progress.total)
                ) + (data.uploadedBytes || 0);
                // Add the difference from the previously loaded state
                // to the global loaded counter:
                this._progress.loaded += (loaded - data._progress.loaded);
                this._progress.bitrate = this._bitrateTimer.getBitrate(
                    now,
                    this._progress.loaded,
                    data.bitrateInterval
                );
                data._progress.loaded = data.loaded = loaded;
                data._progress.bitrate = data.bitrate = data._bitrateTimer.getBitrate(
                    now,
                    loaded,
                    data.bitrateInterval
                );
                // Trigger a custom progress event with a total data property set
                // to the file size(s) of the current upload and a loaded data
                // property calculated accordingly:
                this._trigger('progress', e, data);
                // Trigger a global progress event for all current file uploads,
                // including ajax calls queued for sequential file uploads:
                this._trigger('progressall', e, this._progress);
            }
        },

        _initProgressListener: function (options) {
            var that = this,
                xhr = options.xhr ? options.xhr() : $.ajaxSettings.xhr();
            // Accesss to the native XHR object is required to add event listeners
            // for the upload progress event:
            if (xhr.upload) {
                $(xhr.upload).bind('progress', function (e) {
                    var oe = e.originalEvent;
                    // Make sure the progress event properties get copied over:
                    e.lengthComputable = oe.lengthComputable;
                    e.loaded = oe.loaded;
                    e.total = oe.total;
                    that._onProgress(e, options);
                });
                options.xhr = function () {
                    return xhr;
                };
            }
        },

        _initXHRData: function (options) {
            var formData,
                file = options.files[0],
                // Ignore non-multipart setting if not supported:
                multipart = options.multipart || !$.support.xhrFileUpload,
                paramName = options.paramName[0];
            options.headers = options.headers || {};
            if (options.contentRange) {
                options.headers['Content-Range'] = options.contentRange;
            }
            if (!multipart) {
                options.headers['Content-Disposition'] = 'attachment; filename="' +
                    encodeURI(file.name) + '"';
                options.contentType = file.type;
                options.data = options.blob || file;
            } else if ($.support.xhrFormDataFileUpload) {
                if (options.postMessage) {
                    // window.postMessage does not allow sending FormData
                    // objects, so we just add the File/Blob objects to
                    // the formData array and let the postMessage window
                    // create the FormData object out of this array:
                    formData = this._getFormData(options);
                    if (options.blob) {
                        formData.push({
                            name: paramName,
                            value: options.blob
                        });
                    } else {
                        $.each(options.files, function (index, file) {
                            formData.push({
                                name: options.paramName[index] || paramName,
                                value: file
                            });
                        });
                    }
                } else {
                    if (options.formData instanceof FormData) {
                        formData = options.formData;
                    } else {
                        formData = new FormData();
                        $.each(this._getFormData(options), function (index, field) {
                            formData.append(field.name, field.value);
                        });
                    }
                    if (options.blob) {
                        options.headers['Content-Disposition'] = 'attachment; filename="' +
                            encodeURI(file.name) + '"';
                        formData.append(paramName, options.blob, file.name);
                    } else {
                        $.each(options.files, function (index, file) {
                            // Files are also Blob instances, but some browsers
                            // (Firefox 3.6) support the File API but not Blobs.
                            // This check allows the tests to run with
                            // dummy objects:
                            if ((window.Blob && file instanceof Blob) ||
                                    (window.File && file instanceof File)) {
                                formData.append(
                                    options.paramName[index] || paramName,
                                    file,
                                    file.name
                                );
                            }
                        });
                    }
                }
                options.data = formData;
            }
            // Blob reference is not needed anymore, free memory:
            options.blob = null;
        },

        _initIframeSettings: function (options) {
            // Setting the dataType to iframe enables the iframe transport:
            options.dataType = 'iframe ' + (options.dataType || '');
            // The iframe transport accepts a serialized array as form data:
            options.formData = this._getFormData(options);
            // Add redirect url to form data on cross-domain uploads:
            if (options.redirect && $('<a></a>').prop('href', options.url)
                    .prop('host') !== location.host) {
                options.formData.push({
                    name: options.redirectParamName || 'redirect',
                    value: options.redirect
                });
            }
        },

        _initDataSettings: function (options) {
            if (this._isXHRUpload(options)) {
                if (!this._chunkedUpload(options, true)) {
                    if (!options.data) {
                        this._initXHRData(options);
                    }
                    this._initProgressListener(options);
                }
                if (options.postMessage) {
                    // Setting the dataType to postmessage enables the
                    // postMessage transport:
                    options.dataType = 'postmessage ' + (options.dataType || '');
                }
            } else {
                this._initIframeSettings(options, 'iframe');
            }
        },

        _getParamName: function (options) {
            var fileInput = $(options.fileInput),
                paramName = options.paramName;
            if (!paramName) {
                paramName = [];
                fileInput.each(function () {
                    var input = $(this),
                        name = input.prop('name') || 'files[]',
                        i = (input.prop('files') || [1]).length;
                    while (i) {
                        paramName.push(name);
                        i -= 1;
                    }
                });
                if (!paramName.length) {
                    paramName = [fileInput.prop('name') || 'files[]'];
                }
            } else if (!$.isArray(paramName)) {
                paramName = [paramName];
            }
            return paramName;
        },

        _initFormSettings: function (options) {
            // Retrieve missing options from the input field and the
            // associated form, if available:
            if (!options.form || !options.form.length) {
                options.form = $(options.fileInput.prop('form'));
                // If the given file input doesn't have an associated form,
                // use the default widget file input's form:
                if (!options.form.length) {
                    options.form = $(this.options.fileInput.prop('form'));
                }
            }
            options.paramName = this._getParamName(options);
            if (!options.url) {
                options.url = options.form.prop('action') || location.href;
            }
            // The HTTP request method must be "POST" or "PUT":
            options.type = (options.type || options.form.prop('method') || '')
                .toUpperCase();
            if (options.type !== 'POST' && options.type !== 'PUT' &&
                    options.type !== 'PATCH') {
                options.type = 'POST';
            }
            if (!options.formAcceptCharset) {
                options.formAcceptCharset = options.form.attr('accept-charset');
            }
        },

        _getAJAXSettings: function (data) {
            var options = $.extend({}, this.options, data);
            this._initFormSettings(options);
            this._initDataSettings(options);
            return options;
        },

        // jQuery 1.6 doesn't provide .state(),
        // while jQuery 1.8+ removed .isRejected() and .isResolved():
        _getDeferredState: function (deferred) {
            if (deferred.state) {
                return deferred.state();
            }
            if (deferred.isResolved()) {
                return 'resolved';
            }
            if (deferred.isRejected()) {
                return 'rejected';
            }
            return 'pending';
        },

        // Maps jqXHR callbacks to the equivalent
        // methods of the given Promise object:
        _enhancePromise: function (promise) {
            promise.success = promise.done;
            promise.error = promise.fail;
            promise.complete = promise.always;
            return promise;
        },

        // Creates and returns a Promise object enhanced with
        // the jqXHR methods abort, success, error and complete:
        _getXHRPromise: function (resolveOrReject, context, args) {
            var dfd = $.Deferred(),
                promise = dfd.promise();
            context = context || this.options.context || promise;
            if (resolveOrReject === true) {
                dfd.resolveWith(context, args);
            } else if (resolveOrReject === false) {
                dfd.rejectWith(context, args);
            }
            promise.abort = dfd.promise;
            return this._enhancePromise(promise);
        },

        // Adds convenience methods to the callback arguments:
        _addConvenienceMethods: function (e, data) {
            var that = this;
            data.submit = function () {
                if (this.state() !== 'pending') {
                    data.jqXHR = this.jqXHR =
                        (that._trigger('submit', e, this) !== false) &&
                        that._onSend(e, this);
                }
                return this.jqXHR || that._getXHRPromise();
            };
            data.abort = function () {
                if (this.jqXHR) {
                    return this.jqXHR.abort();
                }
                return this._getXHRPromise();
            };
            data.state = function () {
                if (this.jqXHR) {
                    return that._getDeferredState(this.jqXHR);
                }
            };
            data.progress = function () {
                return this._progress;
            };
        },

        // Parses the Range header from the server response
        // and returns the uploaded bytes:
        _getUploadedBytes: function (jqXHR) {
            var range = jqXHR.getResponseHeader('Range'),
                parts = range && range.split('-'),
                upperBytesPos = parts && parts.length > 1 &&
                    parseInt(parts[1], 10);
            return upperBytesPos && upperBytesPos + 1;
        },

        // Uploads a file in multiple, sequential requests
        // by splitting the file up in multiple blob chunks.
        // If the second parameter is true, only tests if the file
        // should be uploaded in chunks, but does not invoke any
        // upload requests:
        _chunkedUpload: function (options, testOnly) {
            var that = this,
                file = options.files[0],
                fs = file.size,
                ub = options.uploadedBytes = options.uploadedBytes || 0,
                mcs = options.maxChunkSize || fs,
                slice = file.slice || file.webkitSlice || file.mozSlice,
                dfd = $.Deferred(),
                promise = dfd.promise(),
                jqXHR,
                upload;
            if (!(this._isXHRUpload(options) && slice && (ub || mcs < fs)) ||
                    options.data) {
                return false;
            }
            if (testOnly) {
                return true;
            }
            if (ub >= fs) {
                file.error = 'Uploaded bytes exceed file size';
                return this._getXHRPromise(
                    false,
                    options.context,
                    [null, 'error', file.error]
                );
            }
            // The chunk upload method:
            upload = function () {
                // Clone the options object for each chunk upload:
                var o = $.extend({}, options),
                    currentLoaded = o._progress.loaded;
                o.blob = slice.call(
                    file,
                    ub,
                    ub + mcs,
                    file.type
                );
                // Store the current chunk size, as the blob itself
                // will be dereferenced after data processing:
                o.chunkSize = o.blob.size;
                // Expose the chunk bytes position range:
                o.contentRange = 'bytes ' + ub + '-' +
                    (ub + o.chunkSize - 1) + '/' + fs;
                // Process the upload data (the blob and potential form data):
                that._initXHRData(o);
                // Add progress listeners for this chunk upload:
                that._initProgressListener(o);
                jqXHR = ((that._trigger('chunksend', null, o) !== false && $.ajax(o)) ||
                        that._getXHRPromise(false, o.context))
                    .done(function (result, textStatus, jqXHR) {
                        ub = that._getUploadedBytes(jqXHR) ||
                            (ub + o.chunkSize);
                        // Create a progress event if no final progress event
                        // with loaded equaling total has been triggered
                        // for this chunk:
                        if (o._progress.loaded === currentLoaded) {
                            that._onProgress($.Event('progress', {
                                lengthComputable: true,
                                loaded: ub - o.uploadedBytes,
                                total: ub - o.uploadedBytes
                            }), o);
                        }
                        options.uploadedBytes = o.uploadedBytes = ub;
                        o.result = result;
                        o.textStatus = textStatus;
                        o.jqXHR = jqXHR;
                        that._trigger('chunkdone', null, o);
                        that._trigger('chunkalways', null, o);
                        if (ub < fs) {
                            // File upload not yet complete,
                            // continue with the next chunk:
                            upload();
                        } else {
                            dfd.resolveWith(
                                o.context,
                                [result, textStatus, jqXHR]
                            );
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        o.jqXHR = jqXHR;
                        o.textStatus = textStatus;
                        o.errorThrown = errorThrown;
                        that._trigger('chunkfail', null, o);
                        that._trigger('chunkalways', null, o);
                        dfd.rejectWith(
                            o.context,
                            [jqXHR, textStatus, errorThrown]
                        );
                    });
            };
            this._enhancePromise(promise);
            promise.abort = function () {
                return jqXHR.abort();
            };
            upload();
            return promise;
        },

        _beforeSend: function (e, data) {
            if (this._active === 0) {
                // the start callback is triggered when an upload starts
                // and no other uploads are currently running,
                // equivalent to the global ajaxStart event:
                this._trigger('start');
                // Set timer for global bitrate progress calculation:
                this._bitrateTimer = new this._BitrateTimer();
                // Reset the global progress values:
                this._progress.loaded = this._progress.total = 0;
                this._progress.bitrate = 0;
            }
            if (!data._progress) {
                data._progress = {};
            }
            data._progress.loaded = data.loaded = data.uploadedBytes || 0;
            data._progress.total = data.total = this._getTotal(data.files) || 1;
            data._progress.bitrate = data.bitrate = 0;
            this._active += 1;
            // Initialize the global progress values:
            this._progress.loaded += data.loaded;
            this._progress.total += data.total;
        },

        _onDone: function (result, textStatus, jqXHR, options) {
            var total = options._progress.total;
            if (options._progress.loaded < total) {
                // Create a progress event if no final progress event
                // with loaded equaling total has been triggered:
                this._onProgress($.Event('progress', {
                    lengthComputable: true,
                    loaded: total,
                    total: total
                }), options);
            }
            options.result = result;
            options.textStatus = textStatus;
            options.jqXHR = jqXHR;
            this._trigger('done', null, options);
        },

        _onFail: function (jqXHR, textStatus, errorThrown, options) {
            options.jqXHR = jqXHR;
            options.textStatus = textStatus;
            options.errorThrown = errorThrown;
            this._trigger('fail', null, options);
            if (options.recalculateProgress) {
                // Remove the failed (error or abort) file upload from
                // the global progress calculation:
                this._progress.loaded -= options._progress.loaded;
                this._progress.total -= options._progress.total;
            }
        },

        _onAlways: function (jqXHRorResult, textStatus, jqXHRorError, options) {
            // jqXHRorResult, textStatus and jqXHRorError are added to the
            // options object via done and fail callbacks
            this._active -= 1;
            this._trigger('always', null, options);
            if (this._active === 0) {
                // The stop callback is triggered when all uploads have
                // been completed, equivalent to the global ajaxStop event:
                this._trigger('stop');
            }
        },

        _onSend: function (e, data) {
            if (!data.submit) {
                this._addConvenienceMethods(e, data);
            }
            var that = this,
                jqXHR,
                aborted,
                slot,
                pipe,
                options = that._getAJAXSettings(data),
                send = function () {
                    that._sending += 1;
                    // Set timer for bitrate progress calculation:
                    options._bitrateTimer = new that._BitrateTimer();
                    jqXHR = jqXHR || (
                        ((aborted || that._trigger('send', e, options) === false) &&
                        that._getXHRPromise(false, options.context, aborted)) ||
                        that._chunkedUpload(options) || $.ajax(options)
                    ).done(function (result, textStatus, jqXHR) {
                        that._onDone(result, textStatus, jqXHR, options);
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        that._onFail(jqXHR, textStatus, errorThrown, options);
                    }).always(function (jqXHRorResult, textStatus, jqXHRorError) {
                        that._sending -= 1;
                        that._onAlways(
                            jqXHRorResult,
                            textStatus,
                            jqXHRorError,
                            options
                        );
                        if (options.limitConcurrentUploads &&
                                options.limitConcurrentUploads > that._sending) {
                            // Start the next queued upload,
                            // that has not been aborted:
                            var nextSlot = that._slots.shift();
                            while (nextSlot) {
                                if (that._getDeferredState(nextSlot) === 'pending') {
                                    nextSlot.resolve();
                                    break;
                                }
                                nextSlot = that._slots.shift();
                            }
                        }
                    });
                    return jqXHR;
                };
            this._beforeSend(e, options);
            if (this.options.sequentialUploads ||
                    (this.options.limitConcurrentUploads &&
                    this.options.limitConcurrentUploads <= this._sending)) {
                if (this.options.limitConcurrentUploads > 1) {
                    slot = $.Deferred();
                    this._slots.push(slot);
                    pipe = slot.pipe(send);
                } else {
                    pipe = (this._sequence = this._sequence.pipe(send, send));
                }
                // Return the piped Promise object, enhanced with an abort method,
                // which is delegated to the jqXHR object of the current upload,
                // and jqXHR callbacks mapped to the equivalent Promise methods:
                pipe.abort = function () {
                    aborted = [undefined, 'abort', 'abort'];
                    if (!jqXHR) {
                        if (slot) {
                            slot.rejectWith(options.context, aborted);
                        }
                        return send();
                    }
                    return jqXHR.abort();
                };
                return this._enhancePromise(pipe);
            }
            return send();
        },

        _onAdd: function (e, data) {
            var that = this,
                result = true,
                options = $.extend({}, this.options, data),
                limit = options.limitMultiFileUploads,
                paramName = this._getParamName(options),
                paramNameSet,
                paramNameSlice,
                fileSet,
                i;
            if (!(options.singleFileUploads || limit) ||
                    !this._isXHRUpload(options)) {
                fileSet = [data.files];
                paramNameSet = [paramName];
            } else if (!options.singleFileUploads && limit) {
                fileSet = [];
                paramNameSet = [];
                for (i = 0; i < data.files.length; i += limit) {
                    fileSet.push(data.files.slice(i, i + limit));
                    paramNameSlice = paramName.slice(i, i + limit);
                    if (!paramNameSlice.length) {
                        paramNameSlice = paramName;
                    }
                    paramNameSet.push(paramNameSlice);
                }
            } else {
                paramNameSet = paramName;
            }
            data.originalFiles = data.files;
            $.each(fileSet || data.files, function (index, element) {
                var newData = $.extend({}, data);
                newData.files = fileSet ? element : [element];
                newData.paramName = paramNameSet[index];
                that._initProgressObject(newData);
                that._addConvenienceMethods(e, newData);
                result = that._trigger('add', e, newData);
                return result;
            });
            return result;
        },

        _replaceFileInput: function (input) {
            var inputClone = input.clone(true);
            $('<form></form>').append(inputClone)[0].reset();
            // Detaching allows to insert the fileInput on another form
            // without loosing the file input value:
            input.after(inputClone).detach();
            // Avoid memory leaks with the detached file input:
            $.cleanData(input.unbind('remove'));
            // Replace the original file input element in the fileInput
            // elements set with the clone, which has been copied including
            // event handlers:
            this.options.fileInput = this.options.fileInput.map(function (i, el) {
                if (el === input[0]) {
                    return inputClone[0];
                }
                return el;
            });
            // If the widget has been initialized on the file input itself,
            // override this.element with the file input clone:
            if (input[0] === this.element[0]) {
                this.element = inputClone;
            }
        },

        _handleFileTreeEntry: function (entry, path) {
            var that = this,
                dfd = $.Deferred(),
                errorHandler = function (e) {
                    if (e && !e.entry) {
                        e.entry = entry;
                    }
                    // Since $.when returns immediately if one
                    // Deferred is rejected, we use resolve instead.
                    // This allows valid files and invalid items
                    // to be returned together in one set:
                    dfd.resolve([e]);
                },
                dirReader;
            path = path || '';
            if (entry.isFile) {
                if (entry._file) {
                    // Workaround for Chrome bug #149735
                    entry._file.relativePath = path;
                    dfd.resolve(entry._file);
                } else {
                    entry.file(function (file) {
                        file.relativePath = path;
                        dfd.resolve(file);
                    }, errorHandler);
                }
            } else if (entry.isDirectory) {
                dirReader = entry.createReader();
                dirReader.readEntries(function (entries) {
                    that._handleFileTreeEntries(
                        entries,
                        path + entry.name + '/'
                    ).done(function (files) {
                        dfd.resolve(files);
                    }).fail(errorHandler);
                }, errorHandler);
            } else {
                // Return an empy list for file system items
                // other than files or directories:
                dfd.resolve([]);
            }
            return dfd.promise();
        },

        _handleFileTreeEntries: function (entries, path) {
            var that = this;
            return $.when.apply(
                $,
                $.map(entries, function (entry) {
                    return that._handleFileTreeEntry(entry, path);
                })
            ).pipe(function () {
                return Array.prototype.concat.apply(
                    [],
                    arguments
                );
            });
        },

        _getDroppedFiles: function (dataTransfer) {
            dataTransfer = dataTransfer || {};
            var items = dataTransfer.items;
            if (items && items.length && (items[0].webkitGetAsEntry ||
                    items[0].getAsEntry)) {
                return this._handleFileTreeEntries(
                    $.map(items, function (item) {
                        var entry;
                        if (item.webkitGetAsEntry) {
                            entry = item.webkitGetAsEntry();
                            if (entry) {
                                // Workaround for Chrome bug #149735:
                                entry._file = item.getAsFile();
                            }
                            return entry;
                        }
                        return item.getAsEntry();
                    })
                );
            }
            return $.Deferred().resolve(
                $.makeArray(dataTransfer.files)
            ).promise();
        },

        _getSingleFileInputFiles: function (fileInput) {
            fileInput = $(fileInput);
            var entries = fileInput.prop('webkitEntries') ||
                    fileInput.prop('entries'),
                files,
                value;
            if (entries && entries.length) {
                return this._handleFileTreeEntries(entries);
            }
            files = $.makeArray(fileInput.prop('files'));
            if (!files.length) {
                value = fileInput.prop('value');
                if (!value) {
                    return $.Deferred().resolve([]).promise();
                }
                // If the files property is not available, the browser does not
                // support the File API and we add a pseudo File object with
                // the input value as name with path information removed:
                files = [{name: value.replace(/^.*\\/, '')}];
            } else if (files[0].name === undefined && files[0].fileName) {
                // File normalization for Safari 4 and Firefox 3:
                $.each(files, function (index, file) {
                    file.name = file.fileName;
                    file.size = file.fileSize;
                });
            }
            return $.Deferred().resolve(files).promise();
        },

        _getFileInputFiles: function (fileInput) {
            if (!(fileInput instanceof $) || fileInput.length === 1) {
                return this._getSingleFileInputFiles(fileInput);
            }
            return $.when.apply(
                $,
                $.map(fileInput, this._getSingleFileInputFiles)
            ).pipe(function () {
                return Array.prototype.concat.apply(
                    [],
                    arguments
                );
            });
        },

        _onChange: function (e) {
            var that = this,
                data = {
                    fileInput: $(e.target),
                    form: $(e.target.form)
                };
            this._getFileInputFiles(data.fileInput).always(function (files) {
                data.files = files;
                if (that.options.replaceFileInput) {
                    that._replaceFileInput(data.fileInput);
                }
                if (that._trigger('change', e, data) !== false) {
                    that._onAdd(e, data);
                }
            });
        },

        _onPaste: function (e) {
            var cbd = e.originalEvent.clipboardData,
                items = (cbd && cbd.items) || [],
                data = {files: []};
            $.each(items, function (index, item) {
                var file = item.getAsFile && item.getAsFile();
                if (file) {
                    data.files.push(file);
                }
            });
            if (this._trigger('paste', e, data) === false ||
                    this._onAdd(e, data) === false) {
                return false;
            }
        },

        _onDrop: function (e) {
            var that = this,
                dataTransfer = e.dataTransfer = e.originalEvent.dataTransfer,
                data = {};
            if (dataTransfer && dataTransfer.files && dataTransfer.files.length) {
                e.preventDefault();
            }
            this._getDroppedFiles(dataTransfer).always(function (files) {
                data.files = files;
                if (that._trigger('drop', e, data) !== false) {
                    that._onAdd(e, data);
                }
            });
        },

        _onDragOver: function (e) {
            var dataTransfer = e.dataTransfer = e.originalEvent.dataTransfer;
            if (this._trigger('dragover', e) === false) {
                return false;
            }
            if (dataTransfer && $.inArray('Files', dataTransfer.types) !== -1) {
                dataTransfer.dropEffect = 'copy';
                e.preventDefault();
            }
        },

        _initEventHandlers: function () {
            if (this._isXHRUpload(this.options)) {
                this._on(this.options.dropZone, {
                    dragover: this._onDragOver,
                    drop: this._onDrop
                });
                this._on(this.options.pasteZone, {
                    paste: this._onPaste
                });
            }
            this._on(this.options.fileInput, {
                change: this._onChange
            });
        },

        _destroyEventHandlers: function () {
            this._off(this.options.dropZone, 'dragover drop');
            this._off(this.options.pasteZone, 'paste');
            this._off(this.options.fileInput, 'change');
        },

        _setOption: function (key, value) {
            var refresh = $.inArray(key, this._refreshOptionsList) !== -1;
            if (refresh) {
                this._destroyEventHandlers();
            }
            this._super(key, value);
            if (refresh) {
                this._initSpecialOptions();
                this._initEventHandlers();
            }
        },

        _initSpecialOptions: function () {
            var options = this.options;
            if (options.fileInput === undefined) {
                options.fileInput = this.element.is('input[type="file"]') ?
                        this.element : this.element.find('input[type="file"]');
            } else if (!(options.fileInput instanceof $)) {
                options.fileInput = $(options.fileInput);
            }
            if (!(options.dropZone instanceof $)) {
                options.dropZone = $(options.dropZone);
            }
            if (!(options.pasteZone instanceof $)) {
                options.pasteZone = $(options.pasteZone);
            }
        },

        _create: function () {
            var options = this.options;
            // Initialize options set via HTML5 data-attributes:
            $.extend(options, $(this.element[0].cloneNode(false)).data());
            this._initSpecialOptions();
            this._slots = [];
            this._sequence = this._getXHRPromise(true);
            this._sending = this._active = 0;
            this._initProgressObject(this);
            this._initEventHandlers();
        },

        // This method is exposed to the widget API and allows to query
        // the widget upload progress.
        // It returns an object with loaded, total and bitrate properties
        // for the running uploads:
        progress: function () {
            return this._progress;
        },

        // This method is exposed to the widget API and allows adding files
        // using the fileupload API. The data parameter accepts an object which
        // must have a files property and can contain additional options:
        // .fileupload('add', {files: filesList});
        add: function (data) {
            var that = this;
            if (!data || this.options.disabled) {
                return;
            }
            if (data.fileInput && !data.files) {
                this._getFileInputFiles(data.fileInput).always(function (files) {
                    data.files = files;
                    that._onAdd(null, data);
                });
            } else {
                data.files = $.makeArray(data.files);
                this._onAdd(null, data);
            }
        },

        // This method is exposed to the widget API and allows sending files
        // using the fileupload API. The data parameter accepts an object which
        // must have a files or fileInput property and can contain additional options:
        // .fileupload('send', {files: filesList});
        // The method returns a Promise object for the file upload call.
        send: function (data) {
            if (data && !this.options.disabled) {
                if (data.fileInput && !data.files) {
                    var that = this,
                        dfd = $.Deferred(),
                        promise = dfd.promise(),
                        jqXHR,
                        aborted;
                    promise.abort = function () {
                        aborted = true;
                        if (jqXHR) {
                            return jqXHR.abort();
                        }
                        dfd.reject(null, 'abort', 'abort');
                        return promise;
                    };
                    this._getFileInputFiles(data.fileInput).always(
                        function (files) {
                            if (aborted) {
                                return;
                            }
                            data.files = files;
                            jqXHR = that._onSend(null, data).then(
                                function (result, textStatus, jqXHR) {
                                    dfd.resolve(result, textStatus, jqXHR);
                                },
                                function (jqXHR, textStatus, errorThrown) {
                                    dfd.reject(jqXHR, textStatus, errorThrown);
                                }
                            );
                        }
                    );
                    return this._enhancePromise(promise);
                }
                data.files = $.makeArray(data.files);
                if (data.files.length) {
                    return this._onSend(null, data);
                }
            }
            return this._getXHRPromise(false, data && data.context);
        }

    });

}));

/*! AdminLTE app.js
* ================
* Main JS application file for AdminLTE v2. This file
* should be included in all pages. It controls some layout
* options and implements exclusive AdminLTE plugins.
*
* @Author  Almsaeed Studio
* @Support <https://www.almsaeedstudio.com>
* @Email   <abdullah@almsaeedstudio.com>
* @version 2.4.3
* @repository git://github.com/almasaeed2010/AdminLTE.git
* @license MIT <http://opensource.org/licenses/MIT>
*/

// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('AdminLTE requires jQuery')
}

/* BoxRefresh()
 * =========
 * Adds AJAX content control to a box.
 *
 * @Usage: $('#my-box').boxRefresh(options)
 *         or add [data-widget="box-refresh"] to the box element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.boxrefresh';

    var Default = {
        source: '',
        params: {},
        trigger: '.refresh-btn',
        content: '.box-body',
        loadInContent: true,
        responseType: '',
        overlayTemplate: '<div class="overlay"><div class="fa fa-refresh fa-spin"></div></div>',
        onLoadStart: function () {
        },
        onLoadDone: function (response) {
            return response;
        }
    };

    var Selector = {
        data: '[data-widget="box-refresh"]'
    };

    // BoxRefresh Class Definition
    // =========================
    var BoxRefresh = function (element, options) {
        this.element = element;
        this.options = options;
        this.$overlay = $(options.overlay);

        if (options.source === '') {
            throw new Error('Source url was not defined. Please specify a url in your BoxRefresh source option.');
        }

        this._setUpListeners();
        this.load();
    };

    BoxRefresh.prototype.load = function () {
        this._addOverlay();
        this.options.onLoadStart.call($(this));

        $.get(this.options.source, this.options.params, function (response) {
            if (this.options.loadInContent) {
                $(this.options.content).html(response);
            }
            this.options.onLoadDone.call($(this), response);
            this._removeOverlay();
        }.bind(this), this.options.responseType !== '' && this.options.responseType);
    };

    // Private

    BoxRefresh.prototype._setUpListeners = function () {
        $(this.element).on('click', Selector.trigger, function (event) {
            if (event) event.preventDefault();
            this.load();
        }.bind(this));
    };

    BoxRefresh.prototype._addOverlay = function () {
        $(this.element).append(this.$overlay);
    };

    BoxRefresh.prototype._removeOverlay = function () {
        $(this.element).remove(this.$overlay);
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new BoxRefresh($this, options)));
            }

            if (typeof data == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.boxRefresh;

    $.fn.boxRefresh = Plugin;
    $.fn.boxRefresh.Constructor = BoxRefresh;

    // No Conflict Mode
    // ================
    $.fn.boxRefresh.noConflict = function () {
        $.fn.boxRefresh = old;
        return this;
    };

    // BoxRefresh Data API
    // =================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);


/* BoxWidget()
 * ======
 * Adds box widget functions to boxes.
 *
 * @Usage: $('.my-box').boxWidget(options)
 *         This plugin auto activates on any element using the `.box` class
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.boxwidget';

    var Default = {
        animationSpeed: 500,
        collapseTrigger: '[data-widget="collapse"]',
        removeTrigger: '[data-widget="remove"]',
        collapseIcon: 'fa-minus',
        expandIcon: 'fa-plus',
        removeIcon: 'fa-times'
    };

    var Selector = {
        data: '.box',
        collapsed: '.collapsed-box',
        header: '.box-header',
        body: '.box-body',
        footer: '.box-footer',
        tools: '.box-tools'
    };

    var ClassName = {
        collapsed: 'collapsed-box'
    };

    var Event = {
        collapsed: 'collapsed.boxwidget',
        expanded: 'expanded.boxwidget',
        removed: 'removed.boxwidget'
    };

    // BoxWidget Class Definition
    // =====================
    var BoxWidget = function (element, options) {
        this.element = element;
        this.options = options;

        this._setUpListeners();
    };

    BoxWidget.prototype.toggle = function () {
        var isOpen = !$(this.element).is(Selector.collapsed);

        if (isOpen) {
            this.collapse();
        } else {
            this.expand();
        }
    };

    BoxWidget.prototype.expand = function () {
        var expandedEvent = $.Event(Event.expanded);
        var collapseIcon = this.options.collapseIcon;
        var expandIcon = this.options.expandIcon;

        $(this.element).removeClass(ClassName.collapsed);

        $(this.element)
            .children(Selector.header + ', ' + Selector.body + ', ' + Selector.footer)
            .children(Selector.tools)
            .find('.' + expandIcon)
            .removeClass(expandIcon)
            .addClass(collapseIcon);

        $(this.element).children(Selector.body + ', ' + Selector.footer)
            .slideDown(this.options.animationSpeed, function () {
                $(this.element).trigger(expandedEvent);
            }.bind(this));
    };

    BoxWidget.prototype.collapse = function () {
        var collapsedEvent = $.Event(Event.collapsed);
        var collapseIcon = this.options.collapseIcon;
        var expandIcon = this.options.expandIcon;

        $(this.element)
            .children(Selector.header + ', ' + Selector.body + ', ' + Selector.footer)
            .children(Selector.tools)
            .find('.' + collapseIcon)
            .removeClass(collapseIcon)
            .addClass(expandIcon);

        $(this.element).children(Selector.body + ', ' + Selector.footer)
            .slideUp(this.options.animationSpeed, function () {
                $(this.element).addClass(ClassName.collapsed);
                $(this.element).trigger(collapsedEvent);
            }.bind(this));
    };

    BoxWidget.prototype.remove = function () {
        var removedEvent = $.Event(Event.removed);

        $(this.element).slideUp(this.options.animationSpeed, function () {
            $(this.element).trigger(removedEvent);
            $(this.element).remove();
        }.bind(this));
    };

    // Private

    BoxWidget.prototype._setUpListeners = function () {
        var that = this;

        $(this.element).on('click', this.options.collapseTrigger, function (event) {
            if (event) event.preventDefault();
            that.toggle($(this));
            return false;
        });

        $(this.element).on('click', this.options.removeTrigger, function (event) {
            if (event) event.preventDefault();
            that.remove($(this));
            return false;
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new BoxWidget($this, options)));
            }

            if (typeof option == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.boxWidget;

    $.fn.boxWidget = Plugin;
    $.fn.boxWidget.Constructor = BoxWidget;

    // No Conflict Mode
    // ================
    $.fn.boxWidget.noConflict = function () {
        $.fn.boxWidget = old;
        return this;
    };

    // BoxWidget Data API
    // ==================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });
}(jQuery);


/* ControlSidebar()
 * ===============
 * Toggles the state of the control sidebar
 *
 * @Usage: $('#control-sidebar-trigger').controlSidebar(options)
 *         or add [data-toggle="control-sidebar"] to the trigger
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.controlsidebar';

    var Default = {
        slide: true
    };

    var Selector = {
        sidebar: '.control-sidebar',
        data: '[data-toggle="control-sidebar"]',
        open: '.control-sidebar-open',
        bg: '.control-sidebar-bg',
        wrapper: '.wrapper',
        content: '.content-wrapper',
        boxed: '.layout-boxed'
    };

    var ClassName = {
        open: 'control-sidebar-open',
        fixed: 'fixed'
    };

    var Event = {
        collapsed: 'collapsed.controlsidebar',
        expanded: 'expanded.controlsidebar'
    };

    // ControlSidebar Class Definition
    // ===============================
    var ControlSidebar = function (element, options) {
        this.element = element;
        this.options = options;
        this.hasBindedResize = false;

        this.init();
    };

    ControlSidebar.prototype.init = function () {
        // Add click listener if the element hasn't been
        // initialized using the data API
        if (!$(this.element).is(Selector.data)) {
            $(this).on('click', this.toggle);
        }

        this.fix();
        $(window).resize(function () {
            this.fix();
        }.bind(this));
    };

    ControlSidebar.prototype.toggle = function (event) {
        if (event) event.preventDefault();

        this.fix();

        if (!$(Selector.sidebar).is(Selector.open) && !$('body').is(Selector.open)) {
            this.expand();
        } else {
            this.collapse();
        }
    };

    ControlSidebar.prototype.expand = function () {
        if (!this.options.slide) {
            $('body').addClass(ClassName.open);
        } else {
            $(Selector.sidebar).addClass(ClassName.open);
        }

        $(this.element).trigger($.Event(Event.expanded));
    };

    ControlSidebar.prototype.collapse = function () {
        $('body, ' + Selector.sidebar).removeClass(ClassName.open);
        $(this.element).trigger($.Event(Event.collapsed));
    };

    ControlSidebar.prototype.fix = function () {
        if ($('body').is(Selector.boxed)) {
            this._fixForBoxed($(Selector.bg));
        }
    };

    // Private

    ControlSidebar.prototype._fixForBoxed = function (bg) {
        bg.css({
            position: 'absolute',
            height: $(Selector.wrapper).height()
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new ControlSidebar($this, options)));
            }

            if (typeof option == 'string') data.toggle();
        });
    }

    var old = $.fn.controlSidebar;

    $.fn.controlSidebar = Plugin;
    $.fn.controlSidebar.Constructor = ControlSidebar;

    // No Conflict Mode
    // ================
    $.fn.controlSidebar.noConflict = function () {
        $.fn.controlSidebar = old;
        return this;
    };

    // ControlSidebar Data API
    // =======================
    $(document).on('click', Selector.data, function (event) {
        if (event) event.preventDefault();
        Plugin.call($(this), 'toggle');
    });

}(jQuery);


/* DirectChat()
 * ===============
 * Toggles the state of the control sidebar
 *
 * @Usage: $('#my-chat-box').directChat()
 *         or add [data-widget="direct-chat"] to the trigger
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.directchat';

    var Selector = {
        data: '[data-widget="chat-pane-toggle"]',
        box: '.direct-chat'
    };

    var ClassName = {
        open: 'direct-chat-contacts-open'
    };

    // DirectChat Class Definition
    // ===========================
    var DirectChat = function (element) {
        this.element = element;
    };

    DirectChat.prototype.toggle = function ($trigger) {
        $trigger.parents(Selector.box).first().toggleClass(ClassName.open);
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                $this.data(DataKey, (data = new DirectChat($this)));
            }

            if (typeof option == 'string') data.toggle($this);
        });
    }

    var old = $.fn.directChat;

    $.fn.directChat = Plugin;
    $.fn.directChat.Constructor = DirectChat;

    // No Conflict Mode
    // ================
    $.fn.directChat.noConflict = function () {
        $.fn.directChat = old;
        return this;
    };

    // DirectChat Data API
    // ===================
    $(document).on('click', Selector.data, function (event) {
        if (event) event.preventDefault();
        Plugin.call($(this), 'toggle');
    });

}(jQuery);


/* Layout()
 * ========
 * Implements AdminLTE layout.
 * Fixes the layout height in case min-height fails.
 *
 * @usage activated automatically upon window load.
 *        Configure any options by passing data-option="value"
 *        to the body tag.
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.layout';

    var Default = {
        slimscroll: true,
        resetHeight: true
    };

    var Selector = {
        wrapper: '.wrapper',
        contentWrapper: '.content-wrapper',
        layoutBoxed: '.layout-boxed',
        mainFooter: '.main-footer',
        mainHeader: '.main-header',
        sidebar: '.sidebar',
        controlSidebar: '.control-sidebar',
        fixed: '.fixed',
        sidebarMenu: '.sidebar-menu',
        logo: '.main-header .logo'
    };

    var ClassName = {
        fixed: 'fixed',
        holdTransition: 'hold-transition'
    };

    var Layout = function (options) {
        this.options = options;
        this.bindedResize = false;
        this.activate();
    };

    Layout.prototype.activate = function () {
        this.fix();
        this.fixSidebar();

        $('body').removeClass(ClassName.holdTransition);

        if (this.options.resetHeight) {
            $('body, html, ' + Selector.wrapper).css({
                'height': 'auto',
                'min-height': '100%'
            });
        }

        if (!this.bindedResize) {
            $(window).resize(function () {
                this.fix();
                this.fixSidebar();

                $(Selector.logo + ', ' + Selector.sidebar).one('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend', function () {
                    this.fix();
                    this.fixSidebar();
                }.bind(this));
            }.bind(this));

            this.bindedResize = true;
        }

        $(Selector.sidebarMenu).on('expanded.tree', function () {
            this.fix();
            this.fixSidebar();
        }.bind(this));

        $(Selector.sidebarMenu).on('collapsed.tree', function () {
            this.fix();
            this.fixSidebar();
        }.bind(this));
    };

    Layout.prototype.fix = function () {
        // Remove overflow from .wrapper if layout-boxed exists
        $(Selector.layoutBoxed + ' > ' + Selector.wrapper).css('overflow', 'hidden');

        // Get window height and the wrapper height
        var footerHeight = $(Selector.mainFooter).outerHeight() || 0;
        var neg = $(Selector.mainHeader).outerHeight() + footerHeight;
        var windowHeight = $(window).height();
        var sidebarHeight = $(Selector.sidebar).height() || 0;

        // Set the min-height of the content and sidebar based on
        // the height of the document.
        if ($('body').hasClass(ClassName.fixed)) {
            $(Selector.contentWrapper).css('min-height', windowHeight - footerHeight);
        } else {
            var postSetHeight;

            if (windowHeight >= sidebarHeight) {
                $(Selector.contentWrapper).css('min-height', windowHeight - neg);
                postSetHeight = windowHeight - neg;
            } else {
                $(Selector.contentWrapper).css('min-height', sidebarHeight);
                postSetHeight = sidebarHeight;
            }

            // Fix for the control sidebar height
            var $controlSidebar = $(Selector.controlSidebar);
            if (typeof $controlSidebar !== 'undefined') {
                if ($controlSidebar.height() > postSetHeight)
                    $(Selector.contentWrapper).css('min-height', $controlSidebar.height());
            }
        }
    };

    Layout.prototype.fixSidebar = function () {
        // Make sure the body tag has the .fixed class
        if (!$('body').hasClass(ClassName.fixed)) {
            if (typeof $.fn.slimScroll !== 'undefined') {
                $(Selector.sidebar).slimScroll({ destroy: true }).height('auto');
            }
            return;
        }

        // Enable slimscroll for fixed layout
        if (this.options.slimscroll) {
            if (typeof $.fn.slimScroll !== 'undefined') {
                // Destroy if it exists
                // $(Selector.sidebar).slimScroll({ destroy: true }).height('auto')

                // Add slimscroll
                $(Selector.sidebar).slimScroll({
                    height: ($(window).height() - $(Selector.mainHeader).height()) + 'px'
                });
            }
        }
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option === 'object' && option);
                $this.data(DataKey, (data = new Layout(options)));
            }

            if (typeof option === 'string') {
                if (typeof data[option] === 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.layout;

    $.fn.layout = Plugin;
    $.fn.layout.Constuctor = Layout;

    // No conflict mode
    // ================
    $.fn.layout.noConflict = function () {
        $.fn.layout = old;
        return this;
    };

    // Layout DATA-API
    // ===============
    $(window).on('load', function () {
        Plugin.call($('body'));
    });
}(jQuery);


/* PushMenu()
 * ==========
 * Adds the push menu functionality to the sidebar.
 *
 * @usage: $('.btn').pushMenu(options)
 *          or add [data-toggle="push-menu"] to any button
 *          Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.pushmenu';

    var Default = {
        collapseScreenSize: 767,
        expandOnHover: false,
        expandTransitionDelay: 200
    };

    var Selector = {
        collapsed: '.sidebar-collapse',
        open: '.sidebar-open',
        mainSidebar: '.main-sidebar',
        contentWrapper: '.content-wrapper',
        searchInput: '.sidebar-form .form-control',
        button: '[data-toggle="push-menu"]',
        mini: '.sidebar-mini',
        expanded: '.sidebar-expanded-on-hover',
        layoutFixed: '.fixed'
    };

    var ClassName = {
        collapsed: 'sidebar-collapse',
        open: 'sidebar-open',
        mini: 'sidebar-mini',
        expanded: 'sidebar-expanded-on-hover',
        expandFeature: 'sidebar-mini-expand-feature',
        layoutFixed: 'fixed'
    };

    var Event = {
        expanded: 'expanded.pushMenu',
        collapsed: 'collapsed.pushMenu'
    };

    // PushMenu Class Definition
    // =========================
    var PushMenu = function (options) {
        this.options = options;
        this.init();
    };

    PushMenu.prototype.init = function () {
        if (this.options.expandOnHover
            || ($('body').is(Selector.mini + Selector.layoutFixed))) {
            this.expandOnHover();
            $('body').addClass(ClassName.expandFeature);
        }

        $(Selector.contentWrapper).click(function () {
            // Enable hide menu when clicking on the content-wrapper on small screens
            if ($(window).width() <= this.options.collapseScreenSize && $('body').hasClass(ClassName.open)) {
                this.close();
            }
        }.bind(this));

        // __Fix for android devices
        $(Selector.searchInput).click(function (e) {
            e.stopPropagation();
        });
    };

    PushMenu.prototype.toggle = function () {
        var windowWidth = $(window).width();
        var isOpen = !$('body').hasClass(ClassName.collapsed);

        if (windowWidth <= this.options.collapseScreenSize) {
            isOpen = $('body').hasClass(ClassName.open);
        }

        if (!isOpen) {
            this.open();
        } else {
            this.close();
        }
    };

    PushMenu.prototype.open = function () {
        var windowWidth = $(window).width();

        if (windowWidth > this.options.collapseScreenSize) {
            $('body').removeClass(ClassName.collapsed)
                .trigger($.Event(Event.expanded));
        }
        else {
            $('body').addClass(ClassName.open)
                .trigger($.Event(Event.expanded));
        }
    };

    PushMenu.prototype.close = function () {
        var windowWidth = $(window).width();
        if (windowWidth > this.options.collapseScreenSize) {
            $('body').addClass(ClassName.collapsed)
                .trigger($.Event(Event.collapsed));
        } else {
            $('body').removeClass(ClassName.open + ' ' + ClassName.collapsed)
                .trigger($.Event(Event.collapsed));
        }
    };

    PushMenu.prototype.expandOnHover = function () {
        $(Selector.mainSidebar).hover(function () {
            if ($('body').is(Selector.mini + Selector.collapsed)
                && $(window).width() > this.options.collapseScreenSize) {
                this.expand();
            }
        }.bind(this), function () {
            if ($('body').is(Selector.expanded)) {
                this.collapse();
            }
        }.bind(this));
    };

    PushMenu.prototype.expand = function () {
        setTimeout(function () {
            $('body').removeClass(ClassName.collapsed)
                .addClass(ClassName.expanded);
        }, this.options.expandTransitionDelay);
    };

    PushMenu.prototype.collapse = function () {
        setTimeout(function () {
            $('body').removeClass(ClassName.expanded)
                .addClass(ClassName.collapsed);
        }, this.options.expandTransitionDelay);
    };

    // PushMenu Plugin Definition
    // ==========================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new PushMenu(options)));
            }

            if (option === 'toggle') data.toggle();
        });
    }

    var old = $.fn.pushMenu;

    $.fn.pushMenu = Plugin;
    $.fn.pushMenu.Constructor = PushMenu;

    // No Conflict Mode
    // ================
    $.fn.pushMenu.noConflict = function () {
        $.fn.pushMenu = old;
        return this;
    };

    // Data API
    // ========
    $(document).on('click', Selector.button, function (e) {
        e.preventDefault();
        Plugin.call($(this), 'toggle');
    });
    $(window).on('load', function () {
        Plugin.call($(Selector.button));
    });
}(jQuery);


/* TodoList()
 * =========
 * Converts a list into a todoList.
 *
 * @Usage: $('.my-list').todoList(options)
 *         or add [data-widget="todo-list"] to the ul element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.todolist';

    var Default = {
        onCheck: function (item) {
            return item;
        },
        onUnCheck: function (item) {
            return item;
        }
    };

    var Selector = {
        data: '[data-widget="todo-list"]'
    };

    var ClassName = {
        done: 'done'
    };

    // TodoList Class Definition
    // =========================
    var TodoList = function (element, options) {
        this.element = element;
        this.options = options;

        this._setUpListeners();
    };

    TodoList.prototype.toggle = function (item) {
        item.parents(Selector.li).first().toggleClass(ClassName.done);
        if (!item.prop('checked')) {
            this.unCheck(item);
            return;
        }

        this.check(item);
    };

    TodoList.prototype.check = function (item) {
        this.options.onCheck.call(item);
    };

    TodoList.prototype.unCheck = function (item) {
        this.options.onUnCheck.call(item);
    };

    // Private

    TodoList.prototype._setUpListeners = function () {
        var that = this;
        $(this.element).on('change ifChanged', 'input:checkbox', function () {
            that.toggle($(this));
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new TodoList($this, options)));
            }

            if (typeof data == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.todoList;

    $.fn.todoList = Plugin;
    $.fn.todoList.Constructor = TodoList;

    // No Conflict Mode
    // ================
    $.fn.todoList.noConflict = function () {
        $.fn.todoList = old;
        return this;
    };

    // TodoList Data API
    // =================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);


/* Tree()
 * ======
 * Converts a nested list into a multilevel
 * tree view menu.
 *
 * @Usage: $('.my-menu').tree(options)
 *         or add [data-widget="tree"] to the ul element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.tree';

    var Default = {
        animationSpeed: 500,
        accordion: true,
        followLink: false,
        trigger: '.treeview a'
    };

    var Selector = {
        tree: '.tree',
        treeview: '.treeview',
        treeviewMenu: '.treeview-menu',
        open: '.menu-open, .active',
        li: 'li',
        data: '[data-widget="tree"]',
        active: '.active'
    };

    var ClassName = {
        open: 'menu-open',
        tree: 'tree'
    };

    var Event = {
        collapsed: 'collapsed.tree',
        expanded: 'expanded.tree'
    };

    // Tree Class Definition
    // =====================
    var Tree = function (element, options) {
        this.element = element;
        this.options = options;

        $(this.element).addClass(ClassName.tree);

        $(Selector.treeview + Selector.active, this.element).addClass(ClassName.open);

        this._setUpListeners();
    };

    Tree.prototype.toggle = function (link, event) {
        var treeviewMenu = link.next(Selector.treeviewMenu);
        var parentLi = link.parent();
        var isOpen = parentLi.hasClass(ClassName.open);

        if (!parentLi.is(Selector.treeview)) {
            return;
        }

        if (!this.options.followLink || link.attr('href') === '#') {
            event.preventDefault();
        }

        if (isOpen) {
            this.collapse(treeviewMenu, parentLi);
        } else {
            this.expand(treeviewMenu, parentLi);
        }
    };

    Tree.prototype.expand = function (tree, parent) {
        var expandedEvent = $.Event(Event.expanded);

        if (this.options.accordion) {
            var openMenuLi = parent.siblings(Selector.open);
            var openTree = openMenuLi.children(Selector.treeviewMenu);
            this.collapse(openTree, openMenuLi);
        }

        parent.addClass(ClassName.open);
        tree.slideDown(this.options.animationSpeed, function () {
            $(this.element).trigger(expandedEvent);
        }.bind(this));
    };

    Tree.prototype.collapse = function (tree, parentLi) {
        var collapsedEvent = $.Event(Event.collapsed);

        tree.find(Selector.open).removeClass(ClassName.open);
        parentLi.removeClass(ClassName.open);
        tree.slideUp(this.options.animationSpeed, function () {
            tree.find(Selector.open + ' > ' + Selector.treeview).slideUp();
            $(this.element).trigger(collapsedEvent);
        }.bind(this));
    };

    // Private

    Tree.prototype._setUpListeners = function () {
        var that = this;

        $(this.element).on('click', this.options.trigger, function (event) {
            that.toggle($(this), event);
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, new Tree($this, options));
            }
        });
    }

    var old = $.fn.tree;

    $.fn.tree = Plugin;
    $.fn.tree.Constructor = Tree;

    // No Conflict Mode
    // ================
    $.fn.tree.noConflict = function () {
        $.fn.tree = old;
        return this;
    };

    // Tree Data API
    // =============
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);

"use strict";
var lstDependency = [];
//lstDependency.push("ngRoute");

var app = angular.module("onfinance", lstDependency).run();

app.directive("onlyNumber", function () {
    return {
        restrict: "A",
        link: function (scope, element, attr) {
            element.bind('input', function () {
                var position = this.selectionStart - 1;

                //remove all but number and .
                var fixed = this.value.replace(/[^0-9\.]/g, '');
                if (fixed.charAt(0) === '.')                  //can't start with .
                    fixed = fixed.slice(1);

                var pos = fixed.indexOf(".") + 1;
                if (pos >= 0)               //avoid more than one .
                    fixed = fixed.substr(0, pos) + fixed.slice(pos).replace('.', '');

                if (this.value != fixed) {
                    this.value = fixed;
                    this.selectionStart = position;
                    this.selectionEnd = position;
                }
            });
        }
    };
});

app.directive('ngMin', function ($rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            scope.$watch(attr.ngMin, function () {
                ctrl.$setViewValue(ctrl.$viewValue);
            });
            var minValidator = function (value) {
                var min = scope.$eval(attr.ngMin) || 0;
                if (!$rootScope.isNullOrEmpty(value) && value < min) {
                    ctrl.$setValidity('ngMin', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMin', true);
                    return value;
                }
            };

            ctrl.$parsers.push(minValidator);
            ctrl.$formatters.push(minValidator);
        }
    };
});
app.directive('ngMax', function ($rootScope) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            elem.bind('input', function () {
                var val = this.value.replace(/[^0-9\.]/g, '');
                var max = parseFloat(scope.$eval(attr.ngMax)) || Infinity;

                var intVal = parseFloat(val);

                if (val != null && val != '' && intVal > max) {
                    while (intVal > max) {
                        val = val.slice(0, -1);
                        intVal = parseFloat(val);
                    }
                    this.value = val;
                }
            });
        }
    };
});

app.directive('focusOn', ['$timeout',
    function ($timeout) {
        var checkDirectivePrerequisites = function (attrs) {
            if (!attrs.focusOn && attrs.focusOn != "") {
                throw "FocusOnCondition missing attribute to evaluate";
            }
        }

        return {
            restrict: "A",
            link: function (scope, element, attrs, ctrls) {
                checkDirectivePrerequisites(attrs);

                scope.$watch(attrs.focusOn, function (currentValue, lastValue) {
                    if (currentValue == true) {
                        $timeout(function () {
                            element.focus();
                        });
                    }
                });
            }
        };
    }
]);

app.directive('selectOnClick', ['$window', function ($window) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function () {
                if (!$window.getSelection().toString()) {
                    // Required for mobile Safari
                    this.setSelectionRange(0, this.value.length)
                }
            });
        }
    };
}]);

app.directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return val != null ? parseInt(val, 10) : null;
            });
            ngModel.$formatters.push(function (val) {
                return val != null ? '' + val : null;
            });
        }
    };
});

app.directive('customOnChange', ['$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $timeout(function () {
                    var onChangeFunc = scope.$eval(attrs.customOnChange);
                    element.bind('change', onChangeFunc);
                }, 100);
            }
        };
    }]
);

app.filter('numberFormat', function () {
    return function (n, decPlaces, thouSeparator, decSeparator, trimZero, emptyWhenZero) {
        try {
            if (isNaN(n)) {
                return 0;
            }

            if (n == 0) {
                if (emptyWhenZero == "true") {
                    return "";
                } else if (trimZero == "true") {
                    return 0;
                }
            }

            var decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
                decSeparator = decSeparator == undefined ? "." : decSeparator,
                thouSeparator = thouSeparator == undefined ? "," : thouSeparator,
                sign = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(decPlaces)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return sign + (j ? i.substr(0, j) + thouSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thouSeparator) + (decPlaces ? decSeparator + Math.abs(n - i).toFixed(decPlaces).slice(2) : "");
        } catch (e) {
            return n;
        }
    }
});

app.filter('dateTimeFormat', function ($filter) {
    function getCurrentDate() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = "0" + dd;
        }

        if (mm < 10) {
            mm = "0" + mm;
        }

        today = dd + "/" + mm + "/" + yyyy;
        return today;
    };

    return function (input) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var date;
        var resultDate;

        if (input.indexOf("Date") > -1) {

            resultDate = new Date(+temp);
            date = $filter("date")(resultDate, "dd/MM/yyyy");


            var utc = resultDate.getTime() + (resultDate.getTimezoneOffset() * 60000);

            // create new Date object for different city
            // using supplied offset
            resultDate = new Date(utc + (3600000 * 7));

            if (getCurrentDate() === date) {
                return $filter("date")(resultDate, "HH:mm") + " Hôm nay";
            } else {
                return $filter("date")(resultDate, "HH:mm ") + " " + $filter("date")(resultDate, "dd/MM/yyyy");
            }

        } else {

            date = $filter("date")(new Date(temp), "dd/MM/yyyy");

            if (getCurrentDate() === date) {
                return "Hôm nay";
            } else {
                var utc = date.getTime() + (date.getTimezoneOffset() * 60000);

                // create new Date object for different city
                // using supplied offset
                resultDate = new Date(utc + (3600000 * 7));
                return $filter("date")(resultDate, "dd/MM/yyyy");
            }
        }
    };
});


app.filter('dateFormat', function ($filter) {
    return function (input, format) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var resultDate = new Date(+temp);
        return $filter("date")(resultDate, format);
    };
});

app.filter('sumTotal', function () {
    return function (input, property) {
        var i = input instanceof Array ? input.length : 0;
        if (typeof property === 'undefined' || i === 0) {
            return i;
        } else if (isNaN(input[0][property])) {
            throw 'filter total can count only numeric values';
        } else {
            var total = 0;
            while (i--)
                total += parseFloat(input[i][property]);
            return total;
        }
    };
});

app.filter('totalSumPriceQty', function () {
    return function (data, key1, key2) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum = sum + (parseFloat(value[key1], 10) * parseFloat(value[key2], 10));
        });
        return sum;
    }
});

app.filter('totalWithOperation', function () {
    return function (data, key1, key2, operation) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2) || angular.isUndefined(operation))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum += parseFloat(eval(value[key1] + operation + value[key2]));
        });
        return sum;
    }
});

app.filter('Contains', function () {
    return function (string, substring) {
        return (string.indexOf(substring) != -1);
    };
});

app.filter('IndexOf', function () {
    return function (string, substring) {
        if (!string)
            return false;
        else
            return (string.indexOf(substring) != -1);
    };
});



function SetVietNameInterface(ctr) {
    ctr.datepicker("option", "monthNames", ['Tháng một', 'Tháng hai', 'Tháng ba', 'Tháng bốn', 'Tháng năm', 'Tháng sáu', 'Tháng bảy', 'Tháng tám', 'Tháng chín', 'Tháng mười', 'Tháng mười một', 'Tháng mười hai']);
    ctr.datepicker("option", "monthNamesShort", ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12']);
    ctr.datepicker("option", "dayNamesShort", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNamesMin", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNames", ['Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy']);
}

function SetDateTimePicker(ctr) {
    ctr.datetimepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function SetDatePicker(ctr) {
    ctr.datepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function fdelFromAreaPrice(div) {
    if (!confirm('Xác nhận xóa?')) return false;

    var boxPrice = $("#" + $(div).attr("rel"));
    var idPrice = $(div).attr("data-remove");
    if (idPrice == "") {
        boxPrice.hide("fast", function () {
            boxPrice.remove();
        });
        return;
    }
    var randomUnique = "del" + Math.round(new Date().getTime() + (Math.random() * 100));
    $.ajax({
        url: "/Products/Deletespecial/" + randomUnique,
        type: "POST",
        data: { salepriceid: idPrice },
        success: function (result) {
            boxPrice.hide("fast", function () {
                boxPrice.remove();
            });
        },
        error: function (result) {
            alert("Lỗi???\n" + result.responseText);
        }
    });

    return true;
}

function onsubmitform(div) {
    var $form = $(div);
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = $form.attr("action") + "/" + randomUnique;
    $.ajax({
        url: action,
        type: "POST",
        data: $form.serializeArray(),
        success: function (result) {
            $form.find(".cmddel_special_price").attr("data-remove", result.responseId);
            $form.find('input[name="hSalepriceID"]').val(result.responseId);
            alert(result.responseText);
        },
        error: function (result) {
            alert("Lỗi truyền dữ liệu, thông tin chưa lưu.\n" + result.responseText);
        }
    });
    return false;
}

function autoSalePrice(input) {
    var $input = $(input);
    var percent = $input.val();
    var costprice = $input.parent().parent().find('input[name="COSTPRICE"]').val();
    var saleprice = (percent / 100) * costprice + parseInt(costprice);
    $input.parent().parent().find('input[name="SALEPRICE"]').val(Math.floor(saleprice / 100) * 100);
}

function autoPercentage(input) {
    var $input = $(input);
    var saleprice = parseInt($input.val());
    var costprice = parseInt($input.parent().parent().find('input[name="COSTPRICE"]').val());

    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    if ((saleprice - costprice) <= 0) {
        oPercent.val(0);
    } else {
        oPercent.val(((saleprice - costprice) / costprice) * 100);
    }
}

function autoSalePriceByCost(input) {
    var $input = $(input);
    var costprice = parseInt($input.val());
    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    var oSalePrice = $input.parent().parent().find('input[name="SALEPRICE"]');
    var percent = numeral(oPercent.val()).format('0,0.00');
    if (oPercent.val() == '') {
        oPercent.val(0);
    }
    var saleprice = (percent / 100) * costprice + costprice;
    oSalePrice.val(Math.floor(saleprice / 100) * 100);
}

function uniqueCheckRadio(radio) {
    var $this = $(radio);
    var oPrice = $this.parent().find('input[name="pricespecial"]');
    var valPrice = oPrice.val();
    $this.parent().parent().find('input[name="pricespecial"]').val(0);
    oPrice.val(valPrice);
}

function loadCustomer(input) {
    var cName = $("#CustomerName");
    var cAddr = $("#CustomerAddress");
    var cId = $("#CustomerId");
    if (cName.text() == "" && input.val() != "") {
        var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
        $.post("/SearchInputVoucher/GetCustomer", { custax: input.val(), randomUnique: randomUnique })
            .done(function (result) {
                if (result.responseName != "") {
                    cName.text(result.responseName);
                    cAddr.text(result.responseAddress);
                    cId.val(result.responseId);
                } else {
                    alert("Không thể tìm thấy nhà cung cấp có mã số thuế: " + input.val());
                    input.focus();
                }
            })
            .fail(function (result) {
                alert("Lỗi không thể tìm nhà cung cấp");
            });
    }
    return false;
}

function delTableRow(td) {
    //var productid = td.parent().find("input[name='ProductID']").val();
    if (!confirm("Bạn chắc chắn muốn xóa dòng này?"))
        return false;

    //td.parent().hide();
    td.parent().hide("fast", function () {
        td.parent().remove();
        if ($("#listRowDetail tr:not(:hidden)").length <= 0) {
            $("select[name='VATInput']").val(0);
            $("#VoucherDiscount").val(0);
            $("#totalVoucherMoney").val(0);
            $("#totalVATVoucher").val(0);
            $("#TotalAmountVoucher").val(0);
        }
        updateTotalMoney();
        updateTotalDiscount();
    });
}

var timerId;

function popupSelectOneProduct(jsobj, callback) {
    var obj = JSON.parse(jsobj);
    if (obj.length == 1) {
        callback(obj[0]);
    } else if (obj.length > 1) {
        var div = $('<div class="popupSelectOneProduct">');
        var i = 0;
        obj.forEach(function (o) {
            var guid = "id" + o.ProductID;
            var input = $('<input type="radio"/>').attr('id', guid).attr('name', 'UserSelectedProductID').val(i)
                .keyup(function (e) {
                    if (e.keyCode == 13) {
                        var selectedIndex = $(this).val();
                        callback(obj[selectedIndex]);
                        $('.popupSelectOneProduct').dialog("close");
                    }
                });
            var label = $('<label/>').attr('for', guid).text(" " + o.ProductName + "(" + o.UnitName + ")").css('color', '#188338').prepend(input);
            var p = $('<p/>').append(label);
            div.append(p);
            i++;
        });

        div.dialog({
            title: 'Chọn sản phẩm',
            modal: true,
            resizable: false,
            width: 600,
            height: 250,
            close: function () {
                $('.popupSelectOneProduct').remove();
            },
            buttons: [{
                text: "Ok",
                icons: {
                    primary: "ui-icon-heart"
                },
                click: function () {
                    var selectedIndex = $('input[name="UserSelectedProductID"]:checked').val();
                    callback(obj[selectedIndex]);
                    $(this).dialog("close");
                }
            }]
        });
    }
}

function loadProduct(input) {
    var _tr = input.parent().parent();
    var ProductID = input;
    var ProductName = _tr.find('span[data-field="ProductName"]');
    var UnitID = _tr.find('input[name="QUANTITYUNITID"]');
    var UnitName = _tr.find('span[data-field="UnitName"]');
    var Quantity = _tr.find('input[name="Quantity"]');
    var Price = _tr.find('span[data-field="Price"]');
    var ToMoney = _tr.find('input[name="ToMoney"]');
    var Discount = _tr.find('input[name="Discount"]');

    if (ProductName.text() == "" && input.val() != "") {
        var f = 0;
        $("#listRowDetail tr:not(:hidden) input[name='ProductID']").each(function (index) {
            if ($(this).val() == input.val()) f++;
        });
        if (f > 2) {
            alert("Sản phẩm đã tồn tại, vui lòng kiểm tra lại");
            input.val("");
            input.focus();
        } else {
            var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
            $.post("/SearchInputVoucher/GetProduct", { productid: input.val(), InputTypeInput: $("#InputTypeInput").val(), randomUnique: randomUnique })
                .done(function (result) {
                    if (result.success) {
                        var callback = function (o) {
                            input.val(o.ProductID.trim());
                            ProductName.text(o.ProductName);
                            UnitName.text(o.UnitName);
                            Quantity.val(1);
                            Price.val(0);
                            ToMoney.val(0);
                            Discount.val(0);
                            UnitID.val(o.UnitID);
                        };

                        var o = popupSelectOneProduct(result.jsobj, callback);

                        //$("td.button-new-rowdetail").trigger("click");

                    } else {
                        alert("Không tìm thấy sản phẩm: " + input.val());
                        input.focus();
                    }
                })
                .fail(function (result) {
                    alert("Lỗi không thể tìm sản phẩm");
                });
        }
    }
    return false;
}

function updateTotalMoney(input) {
    if (input != null) {
        var parent = input.parent().parent();
        var newQuantity = parseFloat(parent.find('input[name="Quantity"]').autoNumeric('get'));
        //var quantityunit = parent.find('span[data-field="UnitName"]').text();
        //if (quantityunit == 'Kg') {
        //    var oldQuantity = parseFloat(parent.find('input[name="oldQuantity"]').val());
        //    if (Math.abs(oldQuantity / newQuantity) > 0.5) {
        //        alert("");
        //    }
        //}
        var price = parseInt(parent.find('input[name="ToMoney"]').autoNumeric('get')) / newQuantity;
        parent.find("span[data-field='Price']").text($.number(price, 2));
    }
    var total = 0;
    $("#listRowDetail tr:not(:hidden) input[name='ToMoney']").each(function () {
        if ($(this).val() != '')
            total += parseInt($(this).autoNumeric('get'));
    });
    $("#sumToTalMoneyRow").val(total);
    updateSumMoneyVoucher();
}

function updateSumMoneyVoucher() {



    var sumVATVoucher = 0;
    var sumToTalMoneyRow = parseInt($("#sumToTalMoneyRow").val());
    var sumToTalDiscountRow = parseInt($("#sumToTalDiscountRow").val());
    var voucherDiscount = parseInt($("#VoucherDiscount").autoNumeric('get'));
    var VAT = parseInt($("select[name='VATInput']").val());

    var totalVoucherMoney = $("#totalVoucherMoney");
    var totalVATVoucher = $("#totalVATVoucher");
    var totalMoney = sumToTalMoneyRow - sumToTalDiscountRow - voucherDiscount;
    var totalMoneyProduct = sumToTalMoneyRow - sumToTalDiscountRow;
    $("#listRowDetail tr:not(:hidden)").each(function () {
        var ttsp = parseInt($(this).find("input[name='ToMoney']").autoNumeric('get'));
        var cksp = parseInt($(this).find("input[name='Discount']").autoNumeric('get'));
        var vatsp = parseInt($(this).find("select[name='VAT']").val());
        sumVATVoucher += ((vatsp / 100) * (ttsp - cksp - (voucherDiscount / sumToTalMoneyRow * ttsp)));

        //(1 - voucherDiscount / totalMoneyProduct)
    });
    //console.log(totalMoney);
    totalVoucherMoney.val(totalMoney);

    //var sumVATVoucher = (VAT / 100) * totalMoney;

    totalVATVoucher.val(sumVATVoucher);
    $("#TotalAmountVoucher").val(totalMoney + sumVATVoucher);
}

function updateTotalDiscount() {
    var discount = 0;
    $("tr:not(:hidden) input[name='Discount']").each(function () {
        if ($(this).val() != '') {

            discount += parseInt($(this).autoNumeric('get'));
        }
    });
    $("#sumToTalDiscountRow").val(discount);
    updateSumMoneyVoucher();
}

function checkVoucherDiscount(obj) {
    var vDiscount = parseInt($(obj).autoNumeric('get'));
    var vTotal = parseInt($("#sumToTalMoneyRow").val());
    if (vDiscount > vTotal) {
        alert("Chiết khấu (theo hóa đơn) phải nhỏ hơn hoặc bằng tổng thành tiền: ");
        setTimeout(function () { obj.focus() }, 10);
    }
}

function hiddenRow(btn) {
    if (confirm("Bạn chắc chắn muốn xóa dòng này?")) {
        btn.parent().hide(300, function () {
            $(this).remove();
        });
    }
    return false;
}

function getDecimalSeparator() {
    var n = 3 / 2;
    n = n.toLocaleString().substring(1, 2);
    return n;
}

//var cHamxuly = function () { };
function openPopupSelectProduct(_this, callback, storeid) {

    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    storeid = storeid == null ? 0 : storeid;

    $.post("/Common/SelectProduct", { storeid: storeid })
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn sản phẩm',
                modal: true,
                resizable: false,
                width: 1000,
                height: 650,
                close: function () {
                    btn.removeAttr("disabled");
                    $('.popupselectproduct').remove();
                    setTimeout(function () {
                        callbackExec(null);
                    }, 500);
                }
            });
            modal.html(result);
            //$("select#ddlpSubGroup").multiselect('refresh');
        });
}

var callbackExec = function (str) { };

function openPopupSelectItems(_this, callback) {

    var btn = $(_this);
    btn.attr("disabled", "disabled");
    callbackExec = callback;

    $.post("/Common/SelectItems")
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn Item',
                modal: true,
                resizable: false,
                width: 800,
                height: 670,
                close: function () {
                    btn.removeAttr("disabled");
                    $('.popupselectproduct').remove();
                    //setTimeout(function () {
                    //    callback(null);
                    //}, 500);
                }
            });
            modal.html(result);
        });
}

function openPopupSelectMarket(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectMarket", params)
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn chợ',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

function openPopupSelectCombo(_this, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectCombo")
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn Combo',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

function openPopupSelectStore(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectStore", params)
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn siêu thị',
                modal: true,
                resizable: false,
                width: 1000,
                height: 600,
                close: function () {
                    btn.removeAttr("disabled");

                    $('.popupselectproduct').remove();
                }
            });
            modal.html(result);
        });
}

var $POSloading = {};

function RefreshSession() {
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = "/Account/RefreshSession/" + randomUnique;
    $.post(action, {})
        .done(function (response) {
            if (response) {
                console.log(response.msg);
                //Nếu session null
                if (!response.rs) {
                    if (window.location.href.indexOf('Account/Login') == -1) {
                        window.location.reload();
                    }
                }
            } else {
                console.log("Lỗi không nhận được phản hồi - RefreshSession (js) ");
            }
        })
        .fail(function (result) {
            console.log(response.msg);
        });
}

$(function () {
    //Giữ session
    if (window.location.href.indexOf('Account/Login') == -1) {
        var stopRequest = setInterval(RefreshSession, 300000);
    }

    $('#modelLoading').modal({
        show: false,
        keyboard: false
    });
    $POSloading.show = function () {
        $('#modelLoading').modal('show');
    };
    $POSloading.hide = function () {
        $('#modelLoading').modal('hide');
    };

    //var lH = $("#leftContent").height();
    //$('#mainContent').resize(function () {
    //    var mH = $(this).height();
    //    if (lH < mH) {//$("#leftContent").height(mH);
    //        $("#leftContent").animate({
    //            height: mH
    //        }, 100);
    //    }
    //});
    //$('#mainContent').resize();

    /* DataTables */
    if ($('.dynamicTable').length > 0) {
        $('.dynamicTable').dataTable({
            "sPaginationType": "bootstrap",
            "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
            "oLanguage": {
                "sLengthMenu": "_MENU_ số dòng mỗi trang"
            }
        });
    }

    $("input[name*='typeprice_']").click(function () {
        uniqueCheckRadio(this);
    });

    $('form#frmSearchDoPrice').submit(function () {
        var input = $(this).find(".seachInput");
        if (input.val() == "") {
            alert("Vui lòng nhập mã sản phẩm hoặc tên sản phẩm cần tiềm kiếm");
            input.focus();
            return false;
        }
        return true;
    });

    $('#btnSubmitNewPermission').click(function () {
        var form = $("#frmAddPermission");
        if ($("#idPermission").val() == '') {
            alert("Chưa nhập hoặc nhập sai mã quyền");
            $("#idPermission").focus();
            return false;
        }
        if ($("#namePermission").val() == '') {
            alert("Chưa nhập tên quyền");
            $("#namePermission").focus();
            return false;
        }
        $.post("/Permission/GetPermissionById", form.serializeArray())
            .done(function (result) {
                if (result.perId != "") {
                    alert("Mã quyền đã tồn tại, vui lòng chọn mã quyền khác");
                    $("#idPermission").focus();
                    return false;
                } else
                    form.submit();
            }).fail(function (result) {
                alert("Lỗi không thể lưu thông tin");
                return false;
            });
        return false;
    });

    $(".cmddel_special_price").click(function () {
        return fdelFromAreaPrice(this);
    });

    $("a.cmdadnew_special_price").click(function () {
        var formhtml = $("#templateFrom").html();
        var hProductID = $(this).attr("data-product");
        var hPriceAreaID = $(this).attr("data-pricearea");
        var hrBeforarea = $("#" + $(this).attr("rel"));
        var formID = "frm_" + Math.round(new Date().getTime() + (Math.random() * 100));
        var $form = $("<form action='/Products/Special' method='post'></form>").addClass("frmajaxload").attr({ id: formID, onSubmit: "return onsubmitform(this)" }).append(formhtml);

        hrBeforarea.after($form);

        $("#" + formID + " a.cmddel_special_price").attr({ rel: formID, onclick: "fdelFromAreaPrice(this)" });
        $("#" + formID + " .row2 input[type='radio']").attr("name", "typeprice_" + formID);

        $("#" + formID + " input[data-id='tempradio1']").attr("id", "a_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio1']").attr("for", "a_" + formID);

        $("#" + formID + " input[data-id='tempradio2']").attr("id", "b_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio2']").attr("for", "b_" + formID);

        $("#" + formID + " input[data-id='tempradio3']").attr("id", "c_" + formID).click(function () { uniqueCheckRadio(this); });
        $("#" + formID + " label[data-id='tempradio3']").attr("for", "c_" + formID);

        //$("#" + formID + " input.datepicker_load").datepicker({ dateFormat: 'dd/mm/yy' });
        $("#" + formID + " input.timepicker_load").timepicker({ stepMinute: 5 });

        $("#" + formID + " input.pricespecial_load").number(true, 0);

        $("#" + formID + " input[name='htypeprice']").val("typeprice_" + formID);
        $("#" + formID + " input[name='hProductID']").val(hProductID);
        $("#" + formID + " input[name='hAreaID']").val(hPriceAreaID);


        SetDatePicker($("#" + formID + " input.datepicker_load"));
    });

    $('form.frmajaxload').submit(function () {
        return onsubmitform(this);
    });

    SetDatePicker($(".datepicker"));

    $('.timepicker').timepicker({
        timeFormat: 'HH:mm',
        stepMinute: 5
    });

    $('input[name="PROFITPERCENTAGE"]').number(true, 2);
    $('input.priceinput').number(true, 0);
    //$('input.checkpriceinput').number(true, 0);

    $("#btn_EditInputVoucher").click(function () {
        $("#btn_AcceptInputVoucher").hide();
        $("#btn_SaveInputVoucher").show();
        $("#btn_CacelEditInputVoucher").show();
        $(".hidden-col").show();
        $(".stt-col").hide();
        $(this).remove();

        $(".text_editable").removeAttr('disabled');
        $("#CustomerTax").keyup(function (e) {
            var cName = $("#CustomerName");
            var cAddr = $("#CustomerAddress");
            var cId = $("#CustomerId");
            cName.text("");
            cAddr.text("");
            cId.val("");
            if (e.keyCode == 13) {
                $(this).blur();
            }
        }).focusout(function () {
            loadCustomer($(this));
        });

        return false;
    });
    $("#btn_EditInputAcceptVoucher").click(function () {
        $("#btn_AcceptInputVoucher").hide();
        $("#btn_AcceptStockTransfer").hide();
        $("#btn_SaveInputAcceptVoucher").show();
        $("#btn_CacelEditInputAcceptVoucher").show();
        //$(".hidden-col").show();
        //$(".stt-col").hide();
        $(this).remove();
        $(".text_editaccept").removeAttr('disabled');


        return false;
    });

    $("td.button-new-rowdetail").click(function () {
        var template = $("table tr#rowTemplate:first").clone();
        template.removeAttr("id");
        template.find("td.button-delete-rowdetail").click(function () { delTableRow($(this)); });
        template.find('input[name="ProductID"]').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="Quantity"]').focus();
            } else {
                $(this).parent().parent().find('span[data-field="ProductName"]').text("");
                //$(this).parent().parent().find('input[name="VAT"]').val(0);
                $(this).parent().parent().find('span[data-field="UnitName"]').text("");
                $(this).parent().parent().find('input[name="Quantity"]').val(0);
                $(this).parent().parent().find('span[data-field="Price"]').text("");
                $(this).parent().parent().find('input[name="ToMoney"]').val(0);
                $(this).parent().parent().find('input[name="Discount"]').val(0);
            }
        }).focusout(function () {
            loadProduct($(this));
        });
        template.find('select[name="VAT"]').change(function (e) {
            $('input[name="VAT"]').val($(this).val());
            updateSumMoneyVoucher();
        })
            .val($('select[name="VATInput"]').val());

        template.find('input[name="Quantity"], input[name="InQuantity"], input[name="OutQuantity"]').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="ToMoney"]').focus().select();;
            }
        }).focusout(function () {
            updateTotalMoney($(this));
        }).autoNumeric('init', { mDec: 3, vMin: 0, vMax: 999999 });

        template.find('input[name="ToMoney"]').focusout(function () {
            updateTotalMoney($(this));
        }).keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).parent().parent().find('input[name="Discount"]').focus().select();;
            }
        }).autoNumeric('init', { mDec: 0, vMin: 0, vMax: 9999999999 });
        template.find('input[name="Discount"]').focusout(function () {
            if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
                alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
                $(this).val(0);
                setTimeout(function () { $(this).focus() }, 10);
            }
            updateTotalDiscount();
        }).keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).blur();
            }
        }).autoNumeric('init', { mDec: 0, vMin: 0 });

        $("#listRowDetail").append(template);
    });

    $(".button-delete-rowdetail").click(function () {
        delTableRow($(this));
    });

    var keyold = '';
    $('input[name="ProductID"]').keydown(function (e) {
        keyold = $(this).val();
    });
    $('input[name="ProductID"]').keyup(function (e) {
        var parent = $(this).parent().parent();
        if (e.keyCode == 13) {
            parent.find('input[name="Quantity"]').focus();
        } else if (keyold != $(this).val()) {
            parent.find('span[data-field="ProductName"]').text("");
            //parent.find('input[name="VAT"]').val(0);
            parent.find('span[data-field="UnitName"]').text("");
            parent.find('input[name="Quantity"]').val(0);
            parent.find('span[data-field="Price"]').text("");
            parent.find('input[name="ToMoney"]').val(0);
            parent.find('input[name="Discount"]').val(0);
            parent.find('input[name="action"]').val(2);

            updateTotalMoney();
            updateTotalDiscount();
        }
    }).focusout(function () {
        loadProduct($(this));
    });

    $('input[name="Quantity"]').keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="ToMoney"]').focus().select();
        }
    }).focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    });

    $('input[name="ToMoney"]').focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="Discount"]').focus().select();
        }
    });

    $('input[name="Discount"]').focusout(function () {
        if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
            alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
            $(this).val(0);
            setTimeout(function () { $(this).focus() }, 10);
        }
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalDiscount();
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("#VoucherDiscount").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    }).focusout(function () {
        updateSumMoneyVoucher();
    });

    $("select[name='VATInput']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });

    $("select[name='VAT']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });


});

$(function () {
    //Customize alert
    var ALERT_TITLE = 'Thông báo';
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

        if (d.getElementById('modalContainer'))
            return;

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

        msg.innerHTML = txt;

        if (callback) {
            var div = alertObj.appendChild(d.createElement('div'));
            div.className = 'btnGroup';

            var btn2 = div.appendChild(d.createElement('button'));
            btn2.id = 'cancelBtnAlert';
            btn2.className = 'btn btn-light';
            btn2.appendChild(d.createTextNode(button1));
            btn2.href = '#';
            btn2.onclick = function () {
                removeCustomAlert(); callback(false);
                return false;
            }

            var btn = div.appendChild(d.createElement('button'));
            //btn.id = 'closeBtnAlert';
            btn.className = 'btn btn-success';
            btn.appendChild(d.createTextNode(button2));
            btn.href = '#';
            btn.onclick = function () {
                removeCustomAlert(); callback(true);
                return false;
            }
        } else {
            var btn = alertObj.appendChild(d.createElement('button'));
            btn.id = 'closeBtnAlert';
            btn.className = 'btn btn-success';
            btn.appendChild(d.createTextNode(ALERT_BUTTON_TEXT));
            btn.href = '#';
            btn.onclick = function () {
                removeCustomAlert();
                return false;
            }
        }

        alertObj.style.display = 'block';
    }
});


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

// Control
app.directive('divpage', function ($timeout, $parse) {
    return {
        restrict: 'E',
        scope: {
            pageclick: '&',
            pagemodel: '=',
            totalpage: '@',
            currentpage: '@'
        },
        templateUrl: '/Directives/divPage.html',
        link: function (scope, element, attr) {
            scope.range = new Array(100);

            scope.listPage = [];

            scope.$watch('currentpage', function (newValue, oldValue) {
                rebuildPage(parseInt(newValue), parseInt(scope.totalpage));
            });

            scope.$watch('totalpage', function (newValue, oldValue) {
                rebuildPage(parseInt(scope.currentpage), parseInt(newValue));
            });

            scope.pageOnClick = function (page) {
                scope.pagemodel = page;
                $timeout(scope.pageclick, 50);
            }

            var rebuildPage = function (currentPage, totalPage) {
                var listPage = [];
                var startPage = 1, endPage = totalPage;

                //Hiển thị page 1
                if (currentPage > 3) {
                    startPage = currentPage - 2;
                    var p = {};
                    p.page = 1;
                    p.text = 1;
                    listPage.push(p);
                }

                //Có hiển thị 3 chấm ở đầu hay không?
                if (currentPage > 4) {
                    var p = {};
                    p.page = currentPage - 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //các nút ở giữa
                endPage = Math.min(currentPage + 2, totalPage);
                for (var i = startPage; i <= endPage; i++) {
                    var p = {};
                    p.page = i;
                    p.text = i;
                    listPage.push(p);
                }

                //có hiển thị 3 chấm gần cuối hay không
                if (currentPage + 3 < totalPage) {
                    var p = {};
                    p.page = currentPage + 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //có hiển thị Page cuối hay không?
                if (currentPage + 2 < totalPage) {
                    var p = {};
                    p.page = totalPage;
                    p.text = totalPage;
                    listPage.push(p);
                }

                scope.listPage = listPage;
            }

            var initialise = function () {
            }

            initialise();
        }
    };
});
app.directive('moDateInput', function ($window) {
    return {
        require: '^ngModel',
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            var moment = $window.moment;
            var dateFormat = attrs.moMediumDate;

            attrs.$observe('moDateInput', function (newValue) {
                if (dateFormat == newValue || !ctrl.$modelValue) return;
                dateFormat = newValue;
                ctrl.$modelValue = new Date(ctrl.$setViewValue);
            });

            ctrl.$formatters.unshift(function (modelValue) {
                scope = scope;
                if (!dateFormat || !modelValue) return "";
                var retVal = moment(modelValue).format(dateFormat);
                return retVal;
            });

            ctrl.$parsers.unshift(function (viewValue) {
                scope = scope;
                var date = moment(viewValue, dateFormat);
                return (date && date.isValid() && date.year() > 1950) ? date.toDate() : "";
            });
        }
    };
});