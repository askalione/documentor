(function () {

    $(document).ready(function () {
        $('.tree__items').jstree({
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
    });

}($));