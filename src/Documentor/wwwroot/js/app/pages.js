(function ($) {

    var api = {
        init: function () {
            var that = this;

            that.$tree = $('.tree__items');
            that.$addBtn = $('[data-action="add"]');
            that.$modifyBtn = $('[data-action="modify"]');
            that.$removeBtn = $('[data-action="remove"]');

            that.options = {
                addUrl: that.$addBtn.data('url'),
                modifyUrl: that.$modifyBtn.data('url'),
                removeUrl: that.$removeBtn.data('url')
            };

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
        },
        _bindEvents: function () {
            var that = this;

            $(document).on('click', function (e) {
                var tree = that.$tree.jstree(true);

                if (!$(e.target).closest('.jstree').length) {
                    tree.deselect_all();
                }
                var hasSelected = tree.get_selected().length > 0;

                that.$modifyBtn.prop('disabled', !hasSelected);
                that.$removeBtn.prop('disabled', !hasSelected);
            });

            that.$addBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full'),
                    parentVirtualPath = '';

                if (selectedNodes.length > 0)
                    parentVirtualPath = selectedNodes[0].data.virtualPath;

                that._add(parentVirtualPath);
            });

            that.$modifyBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full');

                if (selectedNodes.length > 0) {
                    that._modify(selectedNodes[0].data.virtualPath);
                }
            });

            that.$removeBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full');

                if (selectedNodes.length > 0) {
                    that._remove(selectedNodes[0].data.virtualPath);
                }
            });
        },
        _add: function (parentVirtualPath) {
            var that = this;
            window.location.href = that.options.addUrl + (parentVirtualPath.length ? '?p=' + parentVirtualPath : '');
        },
        _modify: function (virtualPath) {
            var that = this;
            window.location.href = that.options.modifyUrl + '?p=' + virtualPath;
        },
        _remove: function (virtualPath) {
            var that = this;

            if (confirm('A you sure you want to remove selected page?')) {
                $.post(that.options.removeUrl, { virtualPath: virtualPath })
                    .done(function (res) {
                        if (res.success) {
                            window.location.reload(true);
                        } else {
                            notifications.error(res.errorMessage);
                        }
                    })
                    .fail(function () {
                        notifications.requestFailed();
                    });      
            }            
        }
    };

    api.init();

}(jQuery));