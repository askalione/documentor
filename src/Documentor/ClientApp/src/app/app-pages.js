(function ($) {

    var api = {
        init: function () {
            var that = this;

            that.$tree = $('.tree__items');
            that.$addBtn = $('[data-action="add"]');
            that.$editBtn = $('[data-action="edit"]');
            that.$modifyBtn = $('[data-action="modify"]');            
            that.$deleteBtn = $('[data-action="delete"]');

            that.options = {
                addUrl: that.$addBtn.data('url'),
                editUrl: that.$editBtn.data('url'),
                modifyUrl: that.$modifyBtn.data('url'),
                deleteUrl: that.$deleteBtn.data('url'),
                moveUrl: $('[data-action="move"]').data('url')
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
                'dnd': {
                    'inside_pos': 'last'
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

                that.$editBtn.prop('disabled', !hasSelected);
                that.$modifyBtn.prop('disabled', !hasSelected);
                that.$deleteBtn.prop('disabled', !hasSelected);
            });

            that.$tree.on('move_node.jstree', function (e, data) {
                var virtualPath = data.node.data.virtualPath,
                    newParentVirtualPath = null,
                    newSequenceNumber = data.position + 1;

                if (data.old_parent != data.parent) {
                    newParentVirtualPath = data.parent == '#' ? '/' : that.$tree.jstree(true).get_node(data.parent).data.virtualPath;                
                }

                that._move(virtualPath, newParentVirtualPath, newSequenceNumber, function (res) {
                    if (!res)
                        data.old_instance.refresh();
                });
            });

            that.$addBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full'),
                    parentVirtualPath = '';

                if (selectedNodes.length > 0)
                    parentVirtualPath = selectedNodes[0].data.virtualPath;

                that._add(parentVirtualPath);
            });

            that.$editBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full');

                if (selectedNodes.length > 0) {
                    that._edit(selectedNodes[0].data.virtualPath);
                }
            });

            that.$modifyBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full');

                if (selectedNodes.length > 0) {
                    that._modify(selectedNodes[0].data.virtualPath);
                }
            });

            that.$deleteBtn.on('click', function (e) {
                var tree = that.$tree.jstree(true),
                    selectedNodes = tree.get_selected('full');

                if (selectedNodes.length > 0) {
                    that._delete(selectedNodes[0].data.virtualPath);
                }
            });
        },
        _add: function (parentVirtualPath) {
            var that = this;
            window.location.href = that.options.addUrl + (parentVirtualPath.length ? '?p=' + parentVirtualPath : '');
        },
        _edit: function (virtualPath) {
            var that = this;
            window.location.href = that.options.editUrl + '/' + virtualPath;
        },
        _modify: function (virtualPath) {
            var that = this;
            window.location.href = that.options.modifyUrl + '?p=' + virtualPath;
        },
        _delete: function (virtualPath) {
            var that = this;

            if (confirm('A you sure you want to remove selected page?')) {
                $.post(that.options.deleteUrl, { virtualPath: virtualPath })
                    .done(function (res) {
                        if (res.ok) {
                            window.location.reload(true);
                        } else {
                            notifications.error(res.message);
                        }
                    })
                    .fail(function () {
                        notifications.requestFailed();
                    });      
            }            
        },
        _move: function (virtualPath, newParentVirtualPath, newSequenceNumber, callback) {
            var that = this;

            console.log('data:', { virtualPath: virtualPath, newParentVirtualPath: newParentVirtualPath, newSequenceNumber: newSequenceNumber });

            $.post(that.options.moveUrl,
                {
                    virtualPath: virtualPath,
                    newParentVirtualPath: newParentVirtualPath,
                    newSequenceNumber: newSequenceNumber
                })
                .done(function (res) {
                    if (res.ok) {
                        window.location.reload(true);
                    } else {
                        notifications.error(res.message);
                    }
                    if (callback) callback(res.ok);
                })
                .fail(function () {
                    notifications.requestFailed();
                    if (callback) callback(false);
                });
        }
    };

    api.init();

}(jQuery));