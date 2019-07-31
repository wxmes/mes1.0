var options = {
    url: {
        get: Language.common.apiurl + "/Monitor/GetAllApi"
    },
    table: {
        name: "list"
    }
};

var $grid,
    tableObj;

$(function () {
    initGird();
});

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
                { title: '序号', type: "numbers"},
                { field: 'Controller', title: "控制器", width: 100, sort: true },
                { field: 'Action', title: "方法", width: 240, sort: true },
                { field: 'Description', title: "描述", width: 400, sort: true },
                { field: 'ByDeveloperCode', title: '开发者', width: 100 },
                { field: 'ByDeveloperTime', title: '开发时间', width: 100 }
            ]],
            limit: 50,
            limits: [50, 100, 200, 500],
            even: true,
            height: Language.mainHeight
        });
    });
}

//获取表格数据
function getGridData() {
    $grid.reload({
        where: {
           
        }
    });
}
