var options = {
    url: {
        get: Language.common.apiurl + "/Post/GetPostByOrganizationId",
        edit: "/System/Post/Edit",
        delete: "/Post/DeletePost"
    },
    dialog: {
        width: 610,
        title: "岗位"
    },
    table: {
        name: "list",
        key: "PostId"
    }
};

var $grid,
    tableObj,
    $tree,
    $arrowin,
    treeId = null;

$(function () {
    $tree = $('#tree');
    $arrowin = $("i","#arrowin");
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

//初始化岗位织机构
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

//岗位用户
function postUser() {
    //查看是否选中
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen("/Common/UserControl/ChosenPrivilegeMasterUser?" +
            "privilegeMaster=" + Language.privilegeMaster.post + "&" +
            "privilegeMasterValue=" + data.PostId, "维护岗位人员-" + data.Name, 800);
    }, true);
}

//岗位复制
function copy() {
    //查看是否选中
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        var dialog = BootstrapDialog.show({
            size: BootstrapDialog.SIZE_SMALL,
            title: '请输入岗位名称-' + data.Name,
            message: $('<textarea id="copy-Post-name" class="form-control" placeholder="请输入岗位名称..." value="">' + data.Name + '</textarea>'),
            buttons: [{
                label: '确定',
                icon: 'fa fa-check',
                cssClass: 'eip-btn',
                hotkey: 13,
                action: function () {
                    //判断是否值为空
                    var name = $("#copy-Post-name").val();
                    if (name == "") {
                        DialogTipsMsgWarn("岗位名称不能为空", 2000);
                        return false;
                    }
                    UtilAjaxPostWait("/Post/CopyPost",
                        {
                            id: data[options.table.key],
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
    }, true);
}

//模块权限
function menuPermission() {
    //查看是否选中
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen("/System/Permission/Menu?privilegeMasterValue=" + data.PostId + "&privilegeMaster=" + Language.privilegeMaster.post, "模块权限授权-" + data.Name, 380);
    }, true);
}

//模块按钮权限
function functionPermission() {
    //查看是否选中
    GridIsSelectLayTable(tableObj, options.table.name, function (data) {
        DialogOpen("/System/Permission/Button?privilegeMasterValue=" + data.PostId + "&privilegeMaster=" + Language.privilegeMaster.post, "模块按钮权限授权-" + data.Name, 810);
    }, true);
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