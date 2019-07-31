var baseListFunction = {
    //初始化功能项
    initFunction: function () {
        var toolbar = "";
        $toolbar.prepend(toolbar);
        UtilAjaxPostAsync("/Permission/GetFunctionByMenuIdAndUserId",
            { Area: $area.val(), Controller: $controller.val(), Action: $action.val() },
            function (data) {
                $.each(data,
                    function (i, item) {
                        toolbar += ' <button onclick="' +
                            item.Script +
                            '();" title="' +
                            item.Name +
                            '" data-toggle="tooltip"  class="btn  btn-flat btn-sm eip-btn"><i class="fa ' +
                            item.Icon +
                            '">' +
                            item.Name +
                            '</i></button>';
                    });
                $toolbar.prepend(toolbar);
            });
    }
};
var $toolbar,
    $area,
    $controller,
    $action;
$(function () {
    $toolbar = $("#table-toolbar");
    $area = $("#area");
    $controller = $("#controller");
    $action = $("#action");
    baseListFunction.initFunction();
});