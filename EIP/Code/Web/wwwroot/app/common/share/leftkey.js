//禁止F12
$("*").keydown(function (e) {//判断按键
    e = window.event || e || e.which;
    if (e.keyCode == 123) {
        e.keyCode = 0;
        return false;
    } else {
        return true;
    }
});
//禁止审查元素
$(document).bind("contextmenu", function (e) {
    return false;
});