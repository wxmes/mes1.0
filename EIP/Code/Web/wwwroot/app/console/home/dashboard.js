
//分析
var loginlogechart,
    analysis = {
        option: {
            tooltip: {
                trigger: 'axis'
            },
            legend: {
                data: ['登录次数']
            },
            grid: {
                x: 50,
                y: 30,
                x2: 50,
                y2: 64
            },
            toolbox: {
                show: true,
                feature: {
                    mark: { show: true },
                    dataView: { show: true, readOnly: false },
                    magicType: { show: true, type: ['line', 'bar'] },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            dataZoom: [{
                type: 'slider',
                show: true,
                xAxisIndex: [0],

                start: 0,
                end: 9999 //初始化滚动条
            }],
            xAxis: {
                type: 'category',
                boundaryGap: false,
                data: []
            },
            yAxis: {
                type: 'value'
            },
            series: {
                data: [],
                name: '登录次数',
                type: 'line',
                areaStyle: {}
            }
        }
    }

$(function () {
    initEvent();
    initSortable();
    initAnalysis();
    initCarousel();
});

//初始化分析信息
function initAnalysis() {
    loginlogechart = echarts.init(document.getElementById('login-analysis'));
    loginlogechart.setOption(analysis.option);
    UtilAjaxPostAsync("/Log/GetLoginLogAnalysis", {}, function (res) {
        analysis.option.xAxis.data = res.analysis.xdata;
        analysis.option.series.data = res.analysis.ydata;
        loginlogechart.setOption(analysis.option);
    });
}

//初始化拖拉
function initSortable() {
    $('.connectedSortable').sortable({
        placeholder: 'sort-highlight',
        connectWith: '.connectedSortable',
        handle: '.box-header, .layui-tab-title',
        forcePlaceholderSize: true,
        zIndex: 999999
    });
    $('.connectedSortable .box-header, .connectedSortable .nav-tabs-custom').css('cursor', 'move');
}

//初始化事件
function initEvent() {
    $(".layui-timeline").slimScroll({ height: 280 });
    layui.use('element', function () { });
}

function initCarousel() {
    layui.use(['carousel', 'form'], function () {
        var carousel = layui.carousel;
        
        //图片轮播
        carousel.render({
            elem: '#jxb'
            , width: '660px'
            , height: '270px'
            , interval: 5000
        });
        carousel.render({
            elem: '#dgy'
            , width: '660px'
            , height: '270px'
            , interval: 5000
        });
        carousel.render({
            elem: '#fiber'
            , width: '660px'
            , height: '270px'
            , interval: 5000
        });
        carousel.render({
            elem: '#echarts'
            , width: '660px'
            , height: '270px'
            , interval: 5000
        });
        carousel.render({
            elem: '#qt'
            , width: '660px'
            , height: '270px'
            , interval: 5000
        });
    });
}