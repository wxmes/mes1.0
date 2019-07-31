var $selectObj = null, json = "", oNode = null,
    thePlugins = 'formorganization',
    attJSON = parent.formattributeJSON;

//初始化下拉
function initSelect() {
    UtilAjaxPost("/Organization/GetOrganizationTree", {}, function (data) {
        $selectObj.jstree({
            core: {
                data: data,
                check_callback: true,
                multiple: true,
                strings: {
                    'Loading ...': '正在加载...'
                }
            },
            plugins: ['types', 'dnd', 'search', "checkbox"],
            types: {
                default: {
                    icon: 'fa  fa-file-text-o'
                }
            }
        }).bind("click.jstree", function (e, data) {
            //获取所有选择项
            var nodes = $selectObj.jstree().get_checked(true);
            json = "";
            $.each(nodes, function (i, item) {
                json += item.id + ",";
            });
            if (json != "")
                json = json.substring(0, json.length - 1);

        }).on("loaded.jstree", function (event, data) {
            $selectObj.jstree("deselect_all", true);
            if (UE.plugins[thePlugins].editdom) {
                oNode = UE.plugins[thePlugins].editdom;
            }
            if (oNode) {
                var $obj = $(oNode);
                $.each($obj.attr("val"), function (i, item) {
                    json += item.id + ",";
                    $selectObj.jstree('select_node', item.id, true);
                });
                if (json != "")
                    json = json.substring(0, json.length - 1);

            }
        });
    });
}

//初始化事件
function initEvent() {
    $("#search").click(function () {
        $selectObj.jstree(true).search($("#keywords").val(), true, true);
    });
}

//加载完毕
$(document).ready(function () {
    $selectObj = $('#select-tree');
    initSelect();
    initEvent();
});

//关闭
dialog.oncancel = function () {
    if (UE.plugins[thePlugins].editdom) {
        delete UE.plugins[thePlugins].editdom;
    }
};

//确定
dialog.onok = function () {
    if (json.length === 0) {
        DialogTipsMsgWarn("请选择需要操作的数据", 1000);
        return false;
    }
    //获取有多少个{指定组织}标签
    var $controls = $("[type1^='flow_organization']", editor.document);
    var size = ($controls.size() + 1);
    var html = '<button type="button" type1="flow_organization" value="{指定组织' + size + '}" style="border:0px;background-color:transparent;width:84px" readonly val="' + json + '">';
    html += "{指定组织" + size + "}";
    html += '</button>';
    if (oNode) {
        $(oNode).after(html);
        domUtils.remove(oNode, false);
    } else {
        editor.execCommand('insertHtml', html);
    }
    delete UE.plugins[thePlugins].editdom;
}
