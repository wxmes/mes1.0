UE.compule = {
    getLabelHtml: function ($control) {
        var defaultValue = $control.attr("defaultvalue");
        var label = '<label style="';
        if ($control.attr("fontsize")) {
            label += 'font-size:' + $control.attr("fontsize") + ';';
        }
        if ($control.attr("fontcolor")) {
            label += 'color:' + $control.attr("fontcolor") + ';';
        }
        if ("1" == $control.attr("fontbold")) {
            label += 'font-weight:bold;';
        }
        if ("1" == $control.attr("fontstyle")) {
            label += 'font-style:italic;';
        }
        label += '" >';
        label += UE.compule.getDefaultValue(defaultValue);
        label += '</label>';
        $control.after(label);
        $control.remove();
    }
};