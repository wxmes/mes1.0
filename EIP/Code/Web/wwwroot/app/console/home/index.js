var $content = null,
    $sidebarMenu,
    $changePwd,
    $longinOut,
    $refreshMenu,
    $headImage,
    url;
$(function () {
    initData();
    initEvent();
});

//初始化数据
function initData() {
    $content = $('#tab-page-content');
    $sidebarMenu = $(".sidebar-menu");
    $refreshMenu = $("#refresh-menu");
    $changePwd = $("#changePwd");
    $longinOut = $("#longinOut");
    $headImage = $("#headImage");
    $(".logo-lg").html(Language.common.logoLg);
    $(".logo-mini").html(Language.common.logoMini);
    $("#user-name").html(UtilGetLocalStorage("UserName"));
    $("#user-organizationName").html(UtilGetLocalStorage("OrganizationName") + "-" + UtilGetLocalStorage("UserName"));
}

//初始化事件
function initEvent() {

    //修改密码
    $changePwd.click(function () {
        $.get($(this).attr("data-href"), {}, function (res) {
            $('#tab-page-content').html(res);
        });
    });

    //退出
    $longinOut.click(function () {
        BootstrapDialog.show({
            size: BootstrapDialog.SIZE_SMALL,
            message: function () {
                var $message = $('<div style="margin:10px;font-size:16px"> 是否退出系统 ? </div>');
                return $message;
            },
            title: "退出提示",
            buttons: [
                {
                    icon: 'fa fa-check',
                    label: '确定',
                    title: '确定',
                    cssClass: 'btn-success btn-flat',
                    action: function (dialogItself) {
                        dialogItself.close();
                        //清空
                        localStorage.removeItem('Authorization');
                        localStorage.removeItem('UserName');
                        localStorage.removeItem('OrganizationId');
                        localStorage.removeItem('OrganizationName');
                        localStorage.removeItem('Code');
                        localStorage.removeItem('HeadImage');
                        //请求后台并记录日志
                        window.location.href = "/Account/Login";
                    }
                }, {
                    icon: 'fa fa-times',
                    label: '取消',
                    title: '取消',
                    cssClass: 'btn-danger btn-flat',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }
            ]
        });
    });

    //重新加载菜单
    $refreshMenu.click(function () {
        $("li:not(:first)", $sidebarMenu).remove();
        mainSidebarFunction.initMenu();
    });

    $.ajaxSetup({
        cache: true //开启AJAX相应的缓存 
    });

    mainSidebarFunction.initDashboard();
    mainSidebarFunction.initMenu();
    mainSidebarFunction.handleSidebarAjaxContent();
    mainSidebarFunction.initBaseListHeight();
    mainSidebarFunction.setNameAndHeadImage();
    mainSidebarFunction.checkTable();
}

//配置
window.paceOptions = {
    ajax: {
        trackMethods: ['GET', 'POST']
    }
}

//方法
var mainSidebarFunction = {
    //高度
    initBaseListHeight: function () {
        var winW = 0, winH = 0;
        if (window.innerHeight) { // all except IE
            winW = window.innerWidth;
            winH = window.innerHeight;
        } else if (document.documentElement && document.documentElement.clientHeight) { // IE 6 Strict Mode
            winW = document.documentElement.clientWidth;
            winH = document.documentElement.clientHeight;
        } else if (document.body) { // other
            winW = document.body.clientWidth;
            winH = document.body.clientHeight;
        }
        Language.mainHeight = winH - 133;
        Language.mainWidth = winW;
    },

    //初始化面板
    initDashboard: function () {
        $content.load("/Console/Home/Dashboard", function (responseTxt, statusTxt, xhr) {
            if (statusTxt === "success") {
                //alert("外部内容加载成功！");
            }
            if (statusTxt === "error") {
                if (xhr.status === "404") {
                    $sidebarMenu.html("/Common/Error/NotFind");
                }
                alert("Error: " + xhr.status + ": " + xhr.statusText);
            }
        });
    },

    //处理点击菜单事件
    handleSidebarAjaxContent: function () {
        $sidebarMenu.on('click', ' li > a.ajaxify', function (e) {
            e.preventDefault();
            var $this = $(this);
            url = $this.attr("href");
            if (url !== "#") {
                $content.load(url, function (responseTxt, statusTxt, xhr) {
                    if (statusTxt === "success") {
                        $("ul.treeview-menu li").removeClass("active");
                        $this.parent().addClass("active");
                    }
                    if (statusTxt === "error") {
                        if (xhr.status === "404") {
                            $content.load("/Common/Error/NotFind");
                        }
                    }
                });
            }
        });

        //点击修改头像
        $headImage.click(function (e) {
            mainSidebarFunction.headImage();
        });
    },

    //初始化菜单
    initMenu: function () {
        $.ajax({
            url: Language.common.apiurl + "/Home/LoadMenuPermission", // 跳转到地址
            type: "post",
            async: true,
            headers: {
                'Authorization': UtilGetAuthorizationLocalStorage()
            },
            success: function (data) {
                $sidebarMenu.append(data);
            },
            error: function (e) {
                if (e.status == 401) {
                    DialogTipsMsgWarn(e.responseJSON.Message + " 请重新登录授权", 1000);
                    setTimeout("mainSidebarFunction.login()", 1000);
                } else {
                    DialogTipsMsgWarn(e.responseJSON.Message, 1000);
                }
            }
        });
    },

    //去登录
    login: function () {
        window.location.href = "/Account/Login";
    },

    //赋予名字和头像
    setNameAndHeadImage: function () {
        $("#user-name-sidebar").html(/*UtilGetLocalStorage("OrganizationName") + "-" + */UtilGetLocalStorage("UserName"));
        $(".img-circle").attr('src', UtilGetLocalStorage("HeadImage"));
    },

    //修改头像
    headImage: function () {
        DialogOpen("/System/User/HeadImage", "修改头像-" + UtilGetLocalStorage("UserName"), 900);
    },

    //单选表格
    checkTable: function () {
        var tableDiv;
        $(document).on("click", ".layui-table-body table.layui-table tbody tr", function () {
            var index = $(this).attr('data-index');
            var tableBox = $(this).parents('.layui-table-box');
            //存在固定列
            if (tableBox.find(".layui-table-fixed.layui-table-fixed-l").length > 0) {
                tableDiv = tableBox.find(".layui-table-fixed.layui-table-fixed-l");
            } else {
                tableDiv = tableBox.find(".layui-table-body.layui-table-main");
            }
            var checkCell = tableDiv.find("tr[data-index=" + index + "]").find("td div.laytable-cell-checkbox div.layui-form-checkbox I");
            if (checkCell.length > 0) {
                checkCell.click();
            }
        });

        $(document).on("click", "td div.laytable-cell-checkbox div.layui-form-checkbox", function (e) {
            e.stopPropagation();
        });
    }
}
$(window).resize(function () {
    mainSidebarFunction.initBaseListHeight();
});

//加载完毕
Pace.on("done", function () {
    $("#eip-loading").fadeOut();
    $("#pace-style").attr("href", "/lib/plugins/pace/pace.min.css");
});

//全屏事件
$(document).on('click', "[data-toggle='fullscreen']", function () {
    var doc = document.documentElement;
    if ($(document.body).hasClass("full-screen")) {
        $(document.body).removeClass("full-screen");
        document.exitFullscreen ? document.exitFullscreen() : document.mozCancelFullScreen ? document.mozCancelFullScreen() : document.webkitExitFullscreen && document.webkitExitFullscreen();
    } else {
        $(document.body).addClass("full-screen");
        doc.requestFullscreen ? doc.requestFullscreen() : doc.mozRequestFullScreen ? doc.mozRequestFullScreen() : doc.webkitRequestFullscreen ? doc.webkitRequestFullscreen() : doc.msRequestFullscreen && doc.msRequestFullscreen();
    }
});

//天气预报
(function (T, h, i, n, k, P, a, g, e) {
    g = function () {
        P = h.createElement(i);
        a = h.getElementsByTagName(i)[0];
        P.src = k;
        P.charset = "utf-8";
        P.async = 1;
        a.parentNode.insertBefore(P, a);
    };
    T["ThinkPageWeatherWidgetObject"] = n;
    T[n] || (T[n] = function () { (T[n].q = T[n].q || []).push(arguments) });
    T[n].l = +new Date();
    if (T.attachEvent) {
        T.attachEvent("onload", g);
    } else {
        T.addEventListener("load", g, false);
    }
}(window, document, "script", "tpwidget", "//widget.seniverse.com/widget/chameleon.js"));
tpwidget("init", {
    "flavor": "slim",
    "location": "WX4FBXXFKE4F",
    "geolocation": "enabled",
    "language": "auto",
    "unit": "c",
    "theme": "chameleon",
    "container": "tp-weather-widget",
    "bubble": "enabled",
    "alarmType": "circle",
    "uid": "U45310684A",
    "hash": "0672adf7661640ff0b9dbc80b66f22e0"
});
tpwidget("show");
