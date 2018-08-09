(function ($) {

    var api = {
        init: function () {
            var that = this;

            $(document).ready(function () {
                that.$tree = $('.tree__items');

                that.$tree.jstree({
                    'core': {
                        'check_callback': true
                    },
                    'types': {
                        'default': {
                            'icon': 'la la-folder'
                        }
                    },
                    'plugins': ['types', 'dnd']
                });

                that._bindEvents();
            });
        },
        _bindEvents: function () {
            var that = this;

            $(document).on('click', function (e) {
                if (!$(e.target).closest('.jstree').length) {
                    that.$tree.jstree(true).deselect_all();
                }
            });
        }
    };

    api.init();

}(jQuery));