(function ($) {

    var api = {
        init: function () {
            var that = this;
            that.options = {};

            that.options.addUrl = $('[data-action="add"]').data('url');
            that.options.removeUrl = $('.table__users').data('removeUrl');

            that._bindEvents();
        },
        _bindEvents: function () {
            var that = this;

            $(document).on('click', '[data-action="add"]', function (e) {
                var email = prompt('Type email', '');
                that._add(email);
            });

            $(document).on('click', '[data-action="remove"]', function (e) {
                var email = $(this).closest('tr').data('email');
                that._remove(email);
            });
        },
        _add: function (email) {
            var that = this;

            if (email) {
                $.post(that.options.addUrl, { email: email })
                    .done(function (res) {
                        if (res.ok) {
                            setTimeout(function () {
                                window.location.reload(true)
                            }, 1000);                            
                        } else {
                            notifications.error(res.message);
                        }
                    })
                    .fail(function () {
                        notifications.requestFailed();
                    });
            }
        },
        _remove: function (email) {
            var that = this;

            $.post(that.options.removeUrl, { email: email })
                .done(function (res) {
                    if (res.ok) {
                        setTimeout(function () {
                            window.location.reload(true)
                        }, 1000);
                    } else {
                        console.log('error', );
                        notifications.error(res.message);
                    }
                })
                .fail(function () {
                    console.log('failed');
                    notifications.requestFailed();
                });
        }
    };

    api.init();

}(jQuery));