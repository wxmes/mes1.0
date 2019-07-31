var options = {
    url: {
        get: Language.common.apiurl + "/Group/GetGroupByOrganizationId",
        edit: "/System/Group/Edit",
        delete: "/Group/DeleteGroup"
    },
    dialog: {
        width: 610,
        title: "组"
    },
    table: {
        name: "list",
        key: "GroupId"
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

    $("#arrowin").click(function () {
        arrowin();
    });

    $("#search").click(function () {
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
            even: true,
            height: Language.mainHeight,
            cols: [[
                { type: "checkbox", fixed: 'left', width: 30 },
                { title: '序号', type: "numbers", fixed: 'left' },
                { field: 'Name', title: '名称', width: '180', sort: true, fixed: 'left' },
                { field: 'OrganizationNames', title: '归属单位', width: '40%', sort: true },
                { title: "创建类型", field: "BelongToName", width: 60, align: "center" },
                { title: "创建人", field: "BelongUserName", width: 80, align: "center" },
                { field: 'IsFreeze', title: '冻结', templet: '#templetFreeze', width: '100', sort: true, align: "center" },
                { field: 'CreateTime', title: "创建时间", width: 120, align: "center" },
                { field: 'CreateUserName', title: "创建人", width: 80 },
                { field: 'UpdateTime', title: "修改时间", width: 120, align: "center" },
                { field: 'UpdateUserName', title: "修改人", width: 80 },
                { field: 'OrderNo', title: '排序', sort: true, align: "center" },
                { field: 'Remark', title: '备注', sort: true }
            ]]
        });
    });

}

//初始化组织机构
function initTree() {
    UtilAjaxPostAsync("/Organization/GetOrganizationTree", {}, function (data) {
        $tree.jstree({
            core: {
                data: data,
                strings: {
                    'Loading ...': '正在加载...'
                }
            },
            plugins: ['types', 'dnd'],
            types: {
                default: {
                    icon: 'fa  fa-file-text-o'
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
            filters: getFilters($("#search-filters")),
            id: treeId
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
        DialogOpen(options.url.edit + "?id=" + data[options.table.key], "编辑" + options.dialog.title + "-" + data.Name, options.dialog.width);
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

//组用户
function groupUser() {
    //查看是否选中
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        var info = GridGetSingSelectDataLayTable(tableObj, options.table.name);
        DialogOpen("/Common/UserControl/ChosenPrivilegeMasterUser?" +
            "privilegeMaster=" + Language.privilegeMaster.group + "&" +
            "privilegeMasterValue=" + info.GroupId, "维护组人员-" + info.Name, 800);
    });
}

//组复制
function copy() {
    //查看是否选中
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        var info = GridGetSingSelectDataLayTable(tableObj, options.table.name);
        var dialog = BootstrapDialog.show({
            size: BootstrapDialog.SIZE_SMALL,
            title: '请输入组名称-' + info.Name,
            message: $('<textarea id="copy-group-name" class="form-control" placeholder="请输入组名称..." value="">' + info.Name + '</textarea>'),
            buttons: [{
                label: '确定',
                icon: 'fa fa-check',
                cssClass: 'eip-btn',
                hotkey: 13,
                action: function () {
                    //判断是否值为空
                    var name = $("#copy-group-name").val();
                    if (name == "") {
                        DialogTipsMsgWarn("组名称不能为空", 2000);
                        return false;
                    }
                    UtilAjaxPostWait("/Group/CopyGroup",
                        {
                            id: info[options.table.key],
                            Name: name
                        },
                        function (data) {
                            DialogAjaxResult(data);
                            if (data.ResultSign === 0) {
                                reload(true);
                                dialog.close();
                            }
                        }
                    );
                }
            }, {
                label: '取消',
                icon: 'fa fa-times',
                cssClass: 'btn-danger btn-flat',
                action: function () {
                    dialog.close();

                }
            }]
        });
    });
}

//模块权限
function menuPermission() {
    //查看是否选中
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        var info = GridGetSingSelectDataLayTable(tableObj, options.table.name);
        DialogOpen("/System/Permission/Menu?privilegeMasterValue=" + info.GroupId + "&privilegeMaster=" + Language.privilegeMaster.group, "模块权限授权-" + info.Name, 380);
    });
}

//模块按钮权限
function functionPermission() {
    //查看是否选中
    GridIsSelectLayTableIds(tableObj, options.table.name, options.table.key, function (ids) {
        var info = GridGetSingSelectDataLayTable(tableObj, options.table.name);
        DialogOpen("/System/Permission/Button?privilegeMasterValue=" + info.GroupId + "&privilegeMaster=" + Language.privilegeMaster.group, "模块按钮权限授权-" + info.Name, 810);
    });
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