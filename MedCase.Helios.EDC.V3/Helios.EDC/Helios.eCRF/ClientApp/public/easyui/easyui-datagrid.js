(function ($) {
    function pagerFilter(data) {
        if ($.isArray(data)) {    // is array
            data = {
                total: data.length,
                rows: data
            }
        }
        var target = this;
        var dg = $(target);
        var state = dg.data('datagrid');
        var opts = dg.datagrid('options');
        if (!state.allRows) {
            state.allRows = (data.rows);
        }
        if (!opts.remoteSort && opts.sortName) {
            var names = opts.sortName.split(',');
            var orders = opts.sortOrder.split(',');
            state.allRows.sort(function (r1, r2) {
                var r = 0;
                for (var i = 0; i < names.length; i++) {
                    var sn = names[i];
                    var so = orders[i];
                    var col = $(target).datagrid('getColumnOption', sn);
                    var sortFunc = col.sorter || function (a, b) {
                        return a == b ? 0 : (a > b ? 1 : -1);
                    };
                    r = sortFunc(r1[sn], r2[sn]) * (so == 'asc' ? 1 : -1);
                    if (r != 0) {
                        return r;
                    }
                }
                return r;
            });
        }
        var start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
        var end = start + parseInt(opts.pageSize);
        data.rows = state.allRows.slice(start, end);
        return data;
    }

    var deleteRowMethod = $.fn.datagrid.methods.deleteRow;
    $.extend($.fn.datagrid.methods, {
        clientPaging: function (jq) {
            return jq.each(function () {
                var dg = $(this);
                var state = dg.data('datagrid');
                var opts = state.options;
                opts.loadFilter = pagerFilter;
                var onBeforeLoad = opts.onBeforeLoad;
                opts.onBeforeLoad = function (param) {
                    state.allRows = null;
                    return onBeforeLoad.call(this, param);
                }
                var pager = dg.datagrid('getPager');
                pager.pagination({
                    onSelectPage: function (pageNum, pageSize) {
                        opts.pageNumber = pageNum;
                        opts.pageSize = pageSize;
                        pager.pagination('refresh', {
                            pageNumber: pageNum,
                            pageSize: pageSize
                        });
                        dg.datagrid('loadData', state.allRows);
                    }
                });
                $(this).datagrid('loadData', state.data);
                if (opts.url) {
                    $(this).datagrid('reload');
                }
            });
        },
     
     
        getAllRows: function (jq) {
            return jq.data('datagrid').allRows;
        }
    })
})(jQuery);