﻿var options = {
    url: {
        get: Language.common.apiurl + "/MenuButton/GetMenuButtonByMenuId",
        edit: "/System/MenuButton/Edit",
        delete: "/MenuButton/DeleteMenuButton"
    },
    dialog: {
        width: 610,
        title: "菜单按钮"
    },
    table: {
        name: "list",
        key: "MenuButtonId"
    }
};

var $grid,
    tableObj,
    $tree,
    $arrowin,
    treeId = null;

$(function () {
    $tree = $('#tree');
    $arrowin = $("i", "#arrowin");
    $tree.height(Language.mainHeight);
    $tree.slimScroll({ height: Language.mainHeight });
    initTree();
    initGird();
    initEvent();
});

//初始化事件
function initEvent() {
    $("#reload").click(function () {
        reload(true);
    });

    $("#search").click(function () {
        getGridData();
    });

    $("#arrowin").click(function () {
        arrowin();
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
            even: true,
            height: Language.mainHeight,
            cols: [[
                { type: "checkbox", fixed: 'left', width: 30 },
                { title: '序号', type: "numbers", fixed: 'left' },
                { field: 'Name', title: '名称', width: '120', sort: true, fixed: 'left' },
                { field: 'MenuNames', title: '归属菜单', width: '30%', sort: true },
                { field: 'Icon', title: '图标', templet: '#templetIcon', align: "center", width: '60' },
                { field: 'Script', title: '事件', sort: true, width: '120' },
                { field: 'IsFreeze', title: '冻结', sort: true, templet: '#templetFreeze', width: '100', align: 'center' },
                { field: 'OrderNo', title: '排序', sort: true, align: "center" },
                { field: 'Remark', title: '备注', sort: true }
            ]],
            where: { id: Language.common.guidempty },
            page: false
        });
    });
}

//初始化组织机构
function initTree() {
    UtilAjaxPostAsync("/Menu/GetHaveMenuButtonPermissionMenu", {}, function (data) {
        $tree.jstree({
            core: {
                data: data,
                strings: {
                    'Loading ...': '正在加载...'
                }
            },
            plugins: ['types'],
            types: {
                default: {
                    icon: 'fa fa-tasks'
                }
            }
        }).bind('activate_node.jstree', function (obj, e) {
            treeId = e.node.id;
            getGridData();
        });
    });
}

//获取表格数据
function getGridData() {
    $grid.reload({
        where: { 
            id: treeId,
            filters: getFilters($("#search-filters"))
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
    }, true);
}

//编辑
function copy() {
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen(options.url.edit + "?id=" + data[options.table.key] + "&iscopy=1", "复制" + options.dialog.title + "-" + data.Name, options.dialog.width);
    }, true);
}

//删除
function del() {
    //查看是否选中
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        DialogConfirm(Language.common.deletemsg, function () {
            UtilAjaxPostWait(
                options.url.delete,
                { id: ids },
                perateStatus
            );
        });
    }, false);
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

//折叠/展开
var expand = true;
function arrowin() {
    if (expand) {
        $tree.jstree().close_all();
        expand = false;
        $arrowin.html("展开").attr("class", "fa fa-folder-open");
    } else {
        $tree.jstree().open_all();
        expand = true;
        $arrowin.html("折叠").attr("class", "fa fa-folder");
    }
}