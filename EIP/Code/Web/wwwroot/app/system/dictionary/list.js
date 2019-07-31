var options = {
    url: {
        get: Language.common.apiurl + "/Dictionary/GetDictionariesByParentId",
        edit: "/System/Dictionary/Edit",
        delete: "/Dictionary/DeleteDictionary"
    },
    dialog: {
        width: 610,
        title: "字典"
    },
    table: {
        name: "list",
        key: "DictionaryId"
    }
};

var $grid,
    $tree,
    $arrowin,
    $arrowini,
    $search,
    tableObj,
    $reload,
    $searchFilters,
    $list,
    treeId = null;

$(function () {
    $tree = $('#tree');
    $arrowin = $("#arrowin");
    $arrowini = $("i", $arrowin);
    $reload = $("#reload");
    $search = $("#search");
    $searchFilters = $("#search-filters");
    $tree.height(Language.mainHeight);
    $tree.slimScroll({ height: Language.mainHeight });
    initTree();
    initGird();
    initEvent();
});

//初始化事件
function initEvent() {
    $reload.click(function() {
        reload(true);
    });

    $arrowin.click(function () {
        arrowin();
    }); 

    $search.click(function () {
        getGridData();
    });

   
}

//初始化表格
function initGird() {
    layui.use('table', function () {
        tableObj = layui.table;
        //方法级渲染
        $grid = tableObj.render({
            elem: '#' + options.table.name,
            size: "sm",
            method: 'post',
            url: options.url.get,
            cols: [[
                { type: "checkbox", fixed: 'left', width: 30 },
                { title: '序号', type: "numbers", fixed: 'left' },
                { field: 'Name', title: '名称', width: '200', sort: true, fixed: 'left' },
                { field: 'ParentNames', title: '层级', width: '40%', sort: true },
                { field: 'Value', title: '值', sort: true },
                { field: 'IsFreeze', title: "冻结", width: 80, align: "center", templet: '#templetFreeze' },
                { field: 'OrderNo', title: '排序', sort: true, align: 'center' },
                { field: 'Remark', title: '备注', sort: true }
            ]],
            even: true,
            height: Language.mainHeight,
            page: false
        });

    });
}

//初始化树
function initTree() {
    UtilAjaxPostAsync("/Dictionary/GetDictionaryTree", {}, function (data) {
        $tree.jstree({
            core: {
                data: data,
                strings: {
                    'Loading...': '正在努力加载中...'
                }
            },
            plugins: ['types'],
            types: {
                default: {
                    icon: 'fa fa-file-text-o'
                }
            }
        }).bind('activate_node.jstree', function (obj, e) {
            treeId = e.node.id;
            getGridData();
        }).bind('loaded.jstree', function () {
            if (treeId !== null) {
                $tree.jstree('select_node', treeId, true);
            }
        });
    });
}

//获取表格数据
function getGridData() {
    $grid.reload({
        where: {
            id: treeId,
            filters: getFilters($searchFilters)
        }
    });
}

//新增
function add() {
    DialogOpen(options.url.edit, "新增" + options.dialog.title, options.dialog.width);
}

//编辑
function edit() {
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen(options.url.edit + "?id=" + data[options.table.key] + "&iscopy=0", "编辑" + options.dialog.title + "-" + data.Name, options.dialog.width);
    },true);
}

//删除
function del() {
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        DialogConfirm(Language.common.deletemsg, function () {
            UtilAjaxPostWait(options.url.delete,{ id: ids },
                perateStatus
            );
        });
    },false);
}

//请求完成
function perateStatus(data) {
    DialogAjaxResult(data);
    if (data.ResultSign === 0) {
        reload(false);
    }
}

//刷新
function reload(reset) {
    if (reset) treeId = null;
    $tree.jstree("destroy");
    initTree();
    getGridData();
}

//复制
function copy() {
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen(options.url.edit + "?id=" + data[options.table.key] + "&iscopy=1", "复制" + options.dialog.title + "-" + data.Name, options.dialog.width);
    },true);
}

//折叠/展开
var expand = true;
function arrowin() {
    if (expand) {
        $tree.jstree().close_all();
        expand = false;
        $arrowini.html("展开").attr("class", "fa fa-folder-open");
    } else {
        $tree.jstree().open_all();
        expand = true;
        $arrowini.html("折叠").attr("class", "fa fa-folder");
    }
}