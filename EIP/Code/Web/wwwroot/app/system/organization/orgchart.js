$(function () {
    UtilAjaxPostWait("/Organization/GetOrganizationChart", {}, function (data) {
        $('#chart-container').orgchart({
            'data': data,
            'nodeContent': 'title'
        });
    });
});