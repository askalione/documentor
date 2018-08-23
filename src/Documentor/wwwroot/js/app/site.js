(function ($) {

    var api = {
        init: function () {
            var that = this;

            $(document).ready(function () {
                $('.scroller').scrollbar();
                $('body').addClass('animation');

                that._bindEvents();
            });
        },
        _bindEvents: function () {
            var that = this;

            $('.sidebar-toggle').on('click', function (e) {
                $('body').toggleClass('sidebar--expanded');
                e.preventDefault();
            });

            $('.content-overlay').on('click', function (e) {
                $('.sidebar-toggle').click();
            });

            $(window).resize(function (e) {
                $('body').removeClass('sidebar--expanded');
            });

            $(document).keyup(function (e) {
                if (e.keyCode == 27 && $('body').hasClass('sidebar--expanded')) {
                    $('.sidebar-toggle').click();
                }
            });
        }
    };

    api.init();

}(jQuery));