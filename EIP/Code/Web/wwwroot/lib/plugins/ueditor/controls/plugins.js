//数据权限操作
UE.plugins['eipdata'] = function () {
    var me = this, thePlugins = 'eipdata';
   
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type1 = el.getAttribute('type1');
       
        //当前用户
        if (/button/ig.test(el.tagName) && type1 == "flow_current_user_id") {
            var html = popup.formatHtml('<nobr>当前用户: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }

        //所有
        if (/button/ig.test(el.tagName) && type1 == "flow_all") {
            var html = popup.formatHtml('<nobr>所有: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }

        //所在组织
        if (/button/ig.test(el.tagName) && type1 == "flow_current_organization_id") {
            var html = popup.formatHtml('<nobr>所在组织: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }

        //所在组织及下级组织
        if (/button/ig.test(el.tagName) && type1 == "flow_current_children_organization_id") {
            var html = popup.formatHtml('<nobr>所在组织及下级组织: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }

      
        //所在岗位
        if (/button/ig.test(el.tagName) && type1 == "flow_current_post_id") {
            var html = popup.formatHtml('<nobr>所在岗位: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }

        //所在工作组
        if (/button/ig.test(el.tagName) && type1 == "flow_current_group_id") {
            var html = popup.formatHtml('<nobr>所在工作组: <span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Label标签
UE.plugins['formlabel'] = function () {
    var me = this, thePlugins = 'formlabel';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/Label',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: 'Label标签',
                cssRules: "width:600px;height:300px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var formtype = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && formtype == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>Label标签: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Radio标签
UE.plugins['formradio'] = function () {
    var me = this, thePlugins = 'formradio';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/Radio',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '单选按钮组',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>单选按钮组: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Checkbox标签
UE.plugins['formcheckbox'] = function () {
    var me = this, thePlugins = 'formcheckbox';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/CheckBox',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '多选按钮组',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>多选按钮组: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//TextArea标签
UE.plugins['formtextarea'] = function () {
    var me = this, thePlugins = 'formtextarea';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/TextArea',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '文本域',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/textarea/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>文本域: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Text标签
UE.plugins['formtext'] = function () {
    var me = this, thePlugins = 'formtext';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/Text',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '文本框',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>文本框: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Html标签
UE.plugins['formhtml'] = function () {
    var me = this, thePlugins = 'formhtml';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/Html',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: 'Html编辑器',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/textarea/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>Html编辑器: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//DateTime标签
UE.plugins['formdatetime'] = function () {
    var me = this, thePlugins = 'formdatetime';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/DateTime',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '日期标签',
                cssRules: "width:720px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>日期标签: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};

//Select下拉标签
UE.plugins['formselect'] = function () {
    var me = this, thePlugins = 'formselect';
    me.commands[thePlugins] = {
        execCommand: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/Form/Control/Select',
                name: thePlugins + '_' + (new Date().valueOf()),
                editor: this,
                title: '下拉列表',
                cssRules: "width:600px;height:360px;",
                buttons: [
                    {
                        className: 'btn btn-success',
                        label: '确定',
                        onclick: function () {
                            dialog.close(true);
                        }
                    },
                    {
                        className: 'btn btn-danger',
                        label: '取消',
                        onclick: function () {
                            dialog.close(false);
                        }
                    }]
            });
            dialog.render();
            dialog.open();
        }
    };
    var popup = new baidu.editor.ui.Popup({
        editor: this,
        content: '',
        className: 'edui-bubble',
        _edittext: function () {
            baidu.editor.plugins[thePlugins].editdom = popup.anchorEl;
            me.execCommand(thePlugins);
            this.hide();
        },
        _delete: function () {
            if (window.confirm('确认删除该控件吗？')) {
                baidu.editor.dom.domUtils.remove(this.anchorEl, false);
            }
            this.hide();
        }
    });
    popup.render();
    me.addListener('mouseover', function (t, evt) {
        evt = evt || window.event;
        var el = evt.target || evt.srcElement;
        var type = el.getAttribute('eip-control-type');
        if (/input/ig.test(el.tagName) && type == "eip-control-" + thePlugins.replace('form', '')) {
            var html = popup.formatHtml('<nobr>下拉列表: <span onclick=$$._edittext() class="edui-clickable">编辑</span>&nbsp;&nbsp;<span onclick=$$._delete() class="edui-clickable">删除</span></nobr>');
            if (html) {
                popup.getDom('content').innerHTML = html;
                popup.anchorEl = el;
                popup.showAnchor(popup.anchorEl);
            } else {
                popup.hide();
            }
        }
    });
};