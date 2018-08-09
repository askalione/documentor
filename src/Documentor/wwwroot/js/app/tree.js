(function () {

    $(document).ready(function () {
        var $tree = $('.tree__items');

        $tree.jstree({
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

        $(document).on('click', function (e) {
            if (!$(e.target).closest('.jstree').length) {
                $tree.jstree(true).deselect_all();
            }
        });
    });

}($));