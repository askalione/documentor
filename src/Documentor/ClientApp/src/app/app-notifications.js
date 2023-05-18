var notifications = (function ($) {

    if (typeof toastr === 'undefined')
        return;

    var _defaults = {
        closeButto: false,
        debug: false,
        newestOnTop: true,
        progressBar: true,
        positionClass: 'toast-bottom-full-width',
        preventDuplicates: false,
        onclick: null,
        showDuration: 300,
        hideDuration: 1000,
        timeOut: 5000,
        extendedTimeOut: 1000,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
        iconClasses: {
            error: 'toast-error',
            info: 'toast-info',
            success: 'toast-success',
            warning: 'toast-warning'
        }
    };

    toastr.options = $.extend(true, toastr.options, _defaults);

    return {
        add: function (type, message, options) {
            options = $.extend(true, toastr.options, options);
            switch (type.toLowerCase()) {
                case 'success':
                    break;
                case 'error':
                case 'info':
                case 'warning':
                    options.timeOut = 0;
                    options.extendedTimeOut = 0;
                    break;
                default:
                    throw new Error("Unknown notification type: " + type);
                    break;
            }
            toastr.options = options;
            toastr[type](message);
        },
        success: function (message, options) {
            this.add('success', message, options);
        },
        error: function (message, options) {
            this.add('error', message, options);
        },
        warning: function (message, options) {
            this.add('warning', message, options);
        },
        info: function (message, options) {
            this.add('info', message, options);
        },
        requestFailed: function () {
            this.add('error', 'Request failed');
        }
    };

}(jQuery));