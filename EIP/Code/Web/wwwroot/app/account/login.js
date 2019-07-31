var currentindex = 1,
    timerID,
    $submitButton = $("#submit"),
    $submitMsg = $("#submitMsg"),
    $code = $("#userCode"),
    $pwd = $("#userPwd");
method = {
    //绑定事件
    bindEvent: function () {
        $submitButton.click(function () {
            method.submit();
        });
        $("#userCode,#userPwd,#verify").bind("keypress", function (event) {
            if (event.keyCode === "13") {
                method.submit();
            }
        });
    },

    //提交
    submit: function () {
        var loading = $submitButton.button('loading');
        var code = $code.val();
        var pwd = $pwd.val();
        if (code === "") {
            $code.focus();
            loading.button('reset');
            loading.dequeue();
            method.formMessage("请输入用户名", 'warning', 'warning');
            return;
        }
        if (pwd === "") {
            $pwd.focus();
            loading.button('reset');
            loading.dequeue();
            method.formMessage("请输入密码", 'warning', 'warning');
            return;
        }
        if (!locked) {
            loading.button('reset');
            loading.dequeue();
            method.formMessage("请按住滑块，拖动到最右边", 'warning', 'warning');
            return;
        }
        method.formMessage("正在登录中,请稍等...", 'success', 'spinner fa-spin');
        $.post(Language.common.apiurl + "/Account/Login",
            {
                code: code,
                pwd: pwd,
                remberme: $("#remember").prop("checked"),
                returnUrl: $("#returnHidden").val()
            },
            function (data) {
                var resultType = data.ResultSign;
                var resultMsg = data.Message;
                $("#btnSubmit").removeAttr("disabled");
                if (resultType === 2) {
                    loading.button('reset');
                    loading.dequeue();
                    method.formMessage(resultMsg, 'danger', "ban");

                } else {
                    method.formMessage('登录验证成功,正在跳转首页', 'success', 'check');
                    setInterval(method.writeAuthorizationBearerLocalStorage(data), 1000);
                }
            }, "json").success(function () {//成功
            }).error(function () {//失败
                loading.button('reset');
                loading.dequeue();
                method.formMessage("登录失败,请稍后重试", 'danger', "ban");

            }).complete(function () {//完成
            });
    },

    //写Bearer
    writeAuthorizationBearerLocalStorage: function (data) {
        var storage = window.localStorage,user=data.Data;
        storage["Authorization"] = "Bearer " + user.Token;
        storage["UserName"] = user.Name;
        storage["OrganizationId"] = user.OrganizationId;
        storage["OrganizationName"] = user.OrganizationName;
        storage["Code"] = user.Code;
        storage["HeadImage"] = (user.HeadImage === "" || user.HeadImage === null) ? "/css/user/avatar.jpg" : user.HeadImage;
        window.location.href = "/";
        return false;
    },

    //提示消息
    formMessage: function (msg, type, icon) {
        $submitMsg.show().html('').attr("class", "callout callout-" + type);
        $submitMsg.append('<h5><i class="icon fa fa-' + icon + '"></i>' + msg + ' </h5>');
    }
}

$(document).ready(function () {
    $code.focus();
    method.bindEvent();
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%'
    });
});

var locked = false;
window.onload = function () {
    slide();
    //禁止F12
    $("*").keydown(function (e) {//判断按键
        e = window.event || e || e.which;
        if (e.keyCode == 123) {
            e.keyCode = 0;
            return false;
        }
    });
    //禁止审查元素
    $(document).bind("contextmenu", function (e) {
        return false;
    });
}

window.onresize = function () {
    if (locked == true) {
        var boxWidth = $('#slide_box').width();
        $('#slide_xbox').width(boxWidth);
    } else {
        slide();
    }
}

//滑动解锁移动
function slide() {
    var slideBox = $('#slide_box')[0];
    var btn = $('#verify_btn')[0];
    var slideBoxWidth = slideBox.offsetWidth;
    var btnWidth = btn.offsetWidth;
    //pc端
    btn.ondragstart = function () {
        return false;
    };
    btn.onselectstart = function () {
        return false;
    };
    btn.onmousedown = function (e) {
        var disX = e.clientX - btn.offsetLeft;
        document.onmousemove = function (e) {
            var objX = e.clientX - disX + btnWidth;
            if (objX < btnWidth) {
                objX = btnWidth;
            }
            if (objX > slideBoxWidth) {
                objX = slideBoxWidth;
            }
            $('#slide_xbox').width(objX + 'px');
        };
        document.onmouseup = function (e) {
            var objX = e.clientX - disX + btnWidth;
            if (objX < slideBoxWidth) {
                objX = btnWidth;
            } else {
                objX = slideBoxWidth;
                locked = true;
                $('#slide_xbox').html('验证通过<div id="verify_btn"><i class="iconfont icon-xuanzhong" style="color: #35b34a;"></i></div>');
            }
            $('#slide_xbox').width(objX + 'px');
            document.onmousemove = null;
            document.onmouseup = null;
        };
    };
    //移动端
    var cont = $("#verify_btn");
    var startX = 0, sX = 0, moveX = 0, leftX = 0;
    cont.on({//绑定事件
        touchstart: function (e) {
            startX = e.originalEvent.targetTouches[0].pageX;//获取点击点的X坐标
            sX = $(this).offset().left;//相对于当前窗口X轴的偏移量
            leftX = startX - sX;//鼠标所能移动的最左端是当前鼠标距div左边距的位置
        },
        touchmove: function (e) {
            e.preventDefault();
            moveX = e.originalEvent.targetTouches[0].pageX;//移动过程中X轴的坐标
            var objX = moveX - leftX + btnWidth;
            if (objX < btnWidth) {
                objX = btnWidth;
            }
            if (objX > slideBoxWidth) {
                objX = slideBoxWidth;
            }
            $('#slide_xbox').width(objX + 'px');
        },
        touchend: function (e) {
            var objX = moveX - leftX + btnWidth;
            if (objX < slideBoxWidth) {
                objX = btnWidth;
            } else {
                objX = slideBoxWidth;
                locked = true;
                $('#slide_xbox').html('验证通过<div id="verify_btn"><i class="iconfont icon-xuanzhong" style="color: #35b34a;"></i></div>');
            }
            $('#slide_xbox').width(objX + 'px');
        }
    });
}