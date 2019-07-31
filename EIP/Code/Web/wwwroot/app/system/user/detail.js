var $isFreezeFormatter,
    $isOrganizationLeaderFormatter,
    $userDetail,
    $detailUserId;
$(function () {
    $isFreezeFormatter = $("#IsFreezeFormatter");
    $isOrganizationLeaderFormatter = $("#IsOrganizationLeaderFormatter");
    $userDetail = $("#userdetail");
    $detailUserId = $("#DetailUserId");
    initGird();
});

//初始化表格
function initGird() {
    //异步获取
    UtilAjaxPostWait("/User/GetDetailByUserId", { id: $detailUserId.val() }, function (data) {
        $userDetail.find('td').each(function () {
            var $this = $(this), id = $this.attr('id');
            if (typeof (id) != "undefined") {
                (data[id]) && $this.html(data[id]);
            }
        });
        
        //是否冻结
        $isFreezeFormatter.html(data.IsFreezeFormatter).addClass(data.IsFreeze ? "label label-danger" : "label label-success");
        //是否为部门负责人
        $isOrganizationLeaderFormatter.html(data.IsOrganizationLeaderFormatter).addClass(data.IsOrganizationLeader ? "label label-success" : "label label-danger");

        layui.use('table', function () {
            var tableObj = layui.table;
            tableObj.render({
                elem: '#userrole',
                size: "sm",
                method: 'post',
                data: data.Role,
                cols: [[
                    { title: '序号', type: "numbers" },
                    { field: 'Name', title: '名称', width: 150 },
                    { field: 'Organization', title: '组织机构', width: 350 }
                ]],
                limit: 50,
                limits: [50, 100, 200, 500],
                even: true,
                height: 428,
                page: false
            });

            tableObj.render({
                elem: '#userpost',
                size: "sm",
                method: 'post',
                data: data.Post,
                cols: [[
                    { title: '序号', type: "numbers" },
                    { field: 'Name', title: '名称', width: 150 },
                    { field: 'Organization', title: '组织机构', width: 350 }
                ]],
                limit: 50,
                limits: [50, 100, 200, 500],
                even: true,
                height: 428,
                page: false
            });

            tableObj.render({
                elem: '#usergroup',
                size: "sm",
                method: 'post',
                data: data.Group,
                cols: [[
                    { title: '序号', type: "numbers" },
                    { field: 'Name', title: '名称', width: 150 },
                    { field: 'Organization', title: '组织机构', width: 350 }
                ]],
                limit: 50,
                limits: [50, 100, 200, 500],
                even: true,
                height: 428,
                page: false
            });
        });
    });

}
