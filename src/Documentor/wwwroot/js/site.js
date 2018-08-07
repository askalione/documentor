(function () {

    $(document).ready(function () {
        $('.scroller').scrollbar();

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
    });

}($));