var baseEditFunction = {
    initEvent: function () {
        var $editform = $("#editform");
        $('input[type="checkbox"].minimal, input[type="radio"].minimal', $editform).iCheck({
            checkboxClass: 'icheckbox_minimal-blue',
            radioClass: 'iradio_minimal-blue'
        });

        $.each($("input:text, input:password, " +
            "input[type='email'], input[type='url'], input[type='search'], input[type='tel'], input[type='number'], " +
            "input[type='datetime'],textarea",
            $editform),
            function (index, element) {
                var $element = $(element);
                if (($element.attr("data-pure-clear-button") == undefined ||
                    $element.attr("data-pure-clear-button") === 'false') &&
                    !$element.hasClass("has-pure-clear-button")) {
                    return;
                }
                $element.pureClearButton("create");
            });

        //枚举下拉
        $.each($('select[eip-type="enum"]', $editform),
            function (index, element) {
                var $element = $(element),
                    typevalue = $element.attr("eip-type-value"),
                    needdefault = $element.attr("eip-type-needdefault");
                UtilAjaxPost("/UserControl/GetEnumNameValue", { type: typevalue }, function (data) {
                    if (needdefault == "true") {
                        var option = document.createElement("option");
                        $(option).val("");
                        $(option).text("===请选择===");
                        $element.append(option);
                    }
                    $.each(data,
                        function (index, item) {
                            var option = document.createElement("option");
                            $(option).val(item.Value);
                            $(option).text(item.Name);
                            $element.append(option);
                        });
                });
            });

        //字典下拉
        $.each($('select[eip-type="dictionary"]', $editform),
            function (index, element) {
                var $element = $(element),
                    typevalue = $element.attr("eip-type-value"),
                    needdefault = $element.attr("eip-type-needdefault");
                UtilAjaxPost("/Dictionary/GetDictionarieByParentId", { id: typevalue }, function (data) {
                    if (needdefault == "true") {
                        var option = document.createElement("option");
                        $(option).val("");
                        $(option).text("===请选择===");
                        $element.append(option);
                    }
                    $.each(data,
                        function (index, item) {
                            var option = document.createElement("option");
                            $(option).val(item.DictionaryId);
                            $(option).text(item.Name);
                            $element.append(option);
                        });
                });
            });
    }
}

$(function () {
    baseEditFunction.initEvent();
    ValidformNeed();
});