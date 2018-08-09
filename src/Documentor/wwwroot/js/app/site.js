(function ($) {

    var api = {
        init: function () {
            var that = this;

            $(document).ready(function () {
                $('.scroller').scrollbar();

                that._bindEvents();
            });
        },
        _bindEvents: function () {
            var that = this;

            $('.sidebar-toggle').on('click', function (e) {
                $('.sidebar').toggleClass('sidebar--expanded');
                e.preventDefault();
            });

            $('.content-overlay').on('click', function (e) {
                $('.sidebar-toggle').click();
            });

            $(window).resize(function (e) {
                $('.sidebar').removeClass('sidebar--expanded');
            });
        }
    };

    api.init();

}(jQuery));