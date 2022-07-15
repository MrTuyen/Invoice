/*! AdminLTE app.js
* ================
* Main JS application file for AdminLTE v2. This file
* should be included in all pages. It controls some layout
* options and implements exclusive AdminLTE plugins.
*
* @Author  Almsaeed Studio
* @Support <https://www.almsaeedstudio.com>
* @Email   <abdullah@almsaeedstudio.com>
* @version 2.4.3
* @repository git://github.com/almasaeed2010/AdminLTE.git
* @license MIT <http://opensource.org/licenses/MIT>
*/

// Make sure jQuery has been loaded
if (typeof jQuery === 'undefined') {
    throw new Error('AdminLTE requires jQuery')
}

/* BoxRefresh()
 * =========
 * Adds AJAX content control to a box.
 *
 * @Usage: $('#my-box').boxRefresh(options)
 *         or add [data-widget="box-refresh"] to the box element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.boxrefresh';

    var Default = {
        source: '',
        params: {},
        trigger: '.refresh-btn',
        content: '.box-body',
        loadInContent: true,
        responseType: '',
        overlayTemplate: '<div class="overlay"><div class="fa fa-refresh fa-spin"></div></div>',
        onLoadStart: function () {
        },
        onLoadDone: function (response) {
            return response;
        }
    };

    var Selector = {
        data: '[data-widget="box-refresh"]'
    };

    // BoxRefresh Class Definition
    // =========================
    var BoxRefresh = function (element, options) {
        this.element = element;
        this.options = options;
        this.$overlay = $(options.overlay);

        if (options.source === '') {
            throw new Error('Source url was not defined. Please specify a url in your BoxRefresh source option.');
        }

        this._setUpListeners();
        this.load();
    };

    BoxRefresh.prototype.load = function () {
        this._addOverlay();
        this.options.onLoadStart.call($(this));

        $.get(this.options.source, this.options.params, function (response) {
            if (this.options.loadInContent) {
                $(this.options.content).html(response);
            }
            this.options.onLoadDone.call($(this), response);
            this._removeOverlay();
        }.bind(this), this.options.responseType !== '' && this.options.responseType);
    };

    // Private

    BoxRefresh.prototype._setUpListeners = function () {
        $(this.element).on('click', Selector.trigger, function (event) {
            if (event) event.preventDefault();
            this.load();
        }.bind(this));
    };

    BoxRefresh.prototype._addOverlay = function () {
        $(this.element).append(this.$overlay);
    };

    BoxRefresh.prototype._removeOverlay = function () {
        $(this.element).remove(this.$overlay);
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new BoxRefresh($this, options)));
            }

            if (typeof data == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.boxRefresh;

    $.fn.boxRefresh = Plugin;
    $.fn.boxRefresh.Constructor = BoxRefresh;

    // No Conflict Mode
    // ================
    $.fn.boxRefresh.noConflict = function () {
        $.fn.boxRefresh = old;
        return this;
    };

    // BoxRefresh Data API
    // =================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);


/* BoxWidget()
 * ======
 * Adds box widget functions to boxes.
 *
 * @Usage: $('.my-box').boxWidget(options)
 *         This plugin auto activates on any element using the `.box` class
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.boxwidget';

    var Default = {
        animationSpeed: 500,
        collapseTrigger: '[data-widget="collapse"]',
        removeTrigger: '[data-widget="remove"]',
        collapseIcon: 'fa-minus',
        expandIcon: 'fa-plus',
        removeIcon: 'fa-times'
    };

    var Selector = {
        data: '.box',
        collapsed: '.collapsed-box',
        header: '.box-header',
        body: '.box-body',
        footer: '.box-footer',
        tools: '.box-tools'
    };

    var ClassName = {
        collapsed: 'collapsed-box'
    };

    var Event = {
        collapsed: 'collapsed.boxwidget',
        expanded: 'expanded.boxwidget',
        removed: 'removed.boxwidget'
    };

    // BoxWidget Class Definition
    // =====================
    var BoxWidget = function (element, options) {
        this.element = element;
        this.options = options;

        this._setUpListeners();
    };

    BoxWidget.prototype.toggle = function () {
        var isOpen = !$(this.element).is(Selector.collapsed);

        if (isOpen) {
            this.collapse();
        } else {
            this.expand();
        }
    };

    BoxWidget.prototype.expand = function () {
        var expandedEvent = $.Event(Event.expanded);
        var collapseIcon = this.options.collapseIcon;
        var expandIcon = this.options.expandIcon;

        $(this.element).removeClass(ClassName.collapsed);

        $(this.element)
            .children(Selector.header + ', ' + Selector.body + ', ' + Selector.footer)
            .children(Selector.tools)
            .find('.' + expandIcon)
            .removeClass(expandIcon)
            .addClass(collapseIcon);

        $(this.element).children(Selector.body + ', ' + Selector.footer)
            .slideDown(this.options.animationSpeed, function () {
                $(this.element).trigger(expandedEvent);
            }.bind(this));
    };

    BoxWidget.prototype.collapse = function () {
        var collapsedEvent = $.Event(Event.collapsed);
        var collapseIcon = this.options.collapseIcon;
        var expandIcon = this.options.expandIcon;

        $(this.element)
            .children(Selector.header + ', ' + Selector.body + ', ' + Selector.footer)
            .children(Selector.tools)
            .find('.' + collapseIcon)
            .removeClass(collapseIcon)
            .addClass(expandIcon);

        $(this.element).children(Selector.body + ', ' + Selector.footer)
            .slideUp(this.options.animationSpeed, function () {
                $(this.element).addClass(ClassName.collapsed);
                $(this.element).trigger(collapsedEvent);
            }.bind(this));
    };

    BoxWidget.prototype.remove = function () {
        var removedEvent = $.Event(Event.removed);

        $(this.element).slideUp(this.options.animationSpeed, function () {
            $(this.element).trigger(removedEvent);
            $(this.element).remove();
        }.bind(this));
    };

    // Private

    BoxWidget.prototype._setUpListeners = function () {
        var that = this;

        $(this.element).on('click', this.options.collapseTrigger, function (event) {
            if (event) event.preventDefault();
            that.toggle($(this));
            return false;
        });

        $(this.element).on('click', this.options.removeTrigger, function (event) {
            if (event) event.preventDefault();
            that.remove($(this));
            return false;
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new BoxWidget($this, options)));
            }

            if (typeof option == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.boxWidget;

    $.fn.boxWidget = Plugin;
    $.fn.boxWidget.Constructor = BoxWidget;

    // No Conflict Mode
    // ================
    $.fn.boxWidget.noConflict = function () {
        $.fn.boxWidget = old;
        return this;
    };

    // BoxWidget Data API
    // ==================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });
}(jQuery);


/* ControlSidebar()
 * ===============
 * Toggles the state of the control sidebar
 *
 * @Usage: $('#control-sidebar-trigger').controlSidebar(options)
 *         or add [data-toggle="control-sidebar"] to the trigger
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.controlsidebar';

    var Default = {
        slide: true
    };

    var Selector = {
        sidebar: '.control-sidebar',
        data: '[data-toggle="control-sidebar"]',
        open: '.control-sidebar-open',
        bg: '.control-sidebar-bg',
        wrapper: '.wrapper',
        content: '.content-wrapper',
        boxed: '.layout-boxed'
    };

    var ClassName = {
        open: 'control-sidebar-open',
        fixed: 'fixed'
    };

    var Event = {
        collapsed: 'collapsed.controlsidebar',
        expanded: 'expanded.controlsidebar'
    };

    // ControlSidebar Class Definition
    // ===============================
    var ControlSidebar = function (element, options) {
        this.element = element;
        this.options = options;
        this.hasBindedResize = false;

        this.init();
    };

    ControlSidebar.prototype.init = function () {
        // Add click listener if the element hasn't been
        // initialized using the data API
        if (!$(this.element).is(Selector.data)) {
            $(this).on('click', this.toggle);
        }

        this.fix();
        $(window).resize(function () {
            this.fix();
        }.bind(this));
    };

    ControlSidebar.prototype.toggle = function (event) {
        if (event) event.preventDefault();

        this.fix();

        if (!$(Selector.sidebar).is(Selector.open) && !$('body').is(Selector.open)) {
            this.expand();
        } else {
            this.collapse();
        }
    };

    ControlSidebar.prototype.expand = function () {
        if (!this.options.slide) {
            $('body').addClass(ClassName.open);
        } else {
            $(Selector.sidebar).addClass(ClassName.open);
        }

        $(this.element).trigger($.Event(Event.expanded));
    };

    ControlSidebar.prototype.collapse = function () {
        $('body, ' + Selector.sidebar).removeClass(ClassName.open);
        $(this.element).trigger($.Event(Event.collapsed));
    };

    ControlSidebar.prototype.fix = function () {
        if ($('body').is(Selector.boxed)) {
            this._fixForBoxed($(Selector.bg));
        }
    };

    // Private

    ControlSidebar.prototype._fixForBoxed = function (bg) {
        bg.css({
            position: 'absolute',
            height: $(Selector.wrapper).height()
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new ControlSidebar($this, options)));
            }

            if (typeof option == 'string') data.toggle();
        });
    }

    var old = $.fn.controlSidebar;

    $.fn.controlSidebar = Plugin;
    $.fn.controlSidebar.Constructor = ControlSidebar;

    // No Conflict Mode
    // ================
    $.fn.controlSidebar.noConflict = function () {
        $.fn.controlSidebar = old;
        return this;
    };

    // ControlSidebar Data API
    // =======================
    $(document).on('click', Selector.data, function (event) {
        if (event) event.preventDefault();
        Plugin.call($(this), 'toggle');
    });

}(jQuery);


/* DirectChat()
 * ===============
 * Toggles the state of the control sidebar
 *
 * @Usage: $('#my-chat-box').directChat()
 *         or add [data-widget="direct-chat"] to the trigger
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.directchat';

    var Selector = {
        data: '[data-widget="chat-pane-toggle"]',
        box: '.direct-chat'
    };

    var ClassName = {
        open: 'direct-chat-contacts-open'
    };

    // DirectChat Class Definition
    // ===========================
    var DirectChat = function (element) {
        this.element = element;
    };

    DirectChat.prototype.toggle = function ($trigger) {
        $trigger.parents(Selector.box).first().toggleClass(ClassName.open);
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                $this.data(DataKey, (data = new DirectChat($this)));
            }

            if (typeof option == 'string') data.toggle($this);
        });
    }

    var old = $.fn.directChat;

    $.fn.directChat = Plugin;
    $.fn.directChat.Constructor = DirectChat;

    // No Conflict Mode
    // ================
    $.fn.directChat.noConflict = function () {
        $.fn.directChat = old;
        return this;
    };

    // DirectChat Data API
    // ===================
    $(document).on('click', Selector.data, function (event) {
        if (event) event.preventDefault();
        Plugin.call($(this), 'toggle');
    });

}(jQuery);


/* Layout()
 * ========
 * Implements AdminLTE layout.
 * Fixes the layout height in case min-height fails.
 *
 * @usage activated automatically upon window load.
 *        Configure any options by passing data-option="value"
 *        to the body tag.
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.layout';

    var Default = {
        slimscroll: true,
        resetHeight: true
    };

    var Selector = {
        wrapper: '.wrapper',
        contentWrapper: '.content-wrapper',
        layoutBoxed: '.layout-boxed',
        mainFooter: '.main-footer',
        mainHeader: '.main-header',
        sidebar: '.sidebar',
        controlSidebar: '.control-sidebar',
        fixed: '.fixed',
        sidebarMenu: '.sidebar-menu',
        logo: '.main-header .logo'
    };

    var ClassName = {
        fixed: 'fixed',
        holdTransition: 'hold-transition'
    };

    var Layout = function (options) {
        this.options = options;
        this.bindedResize = false;
        this.activate();
    };

    Layout.prototype.activate = function () {
        this.fix();
        this.fixSidebar();

        $('body').removeClass(ClassName.holdTransition);

        if (this.options.resetHeight) {
            $('body, html, ' + Selector.wrapper).css({
                'height': 'auto',
                'min-height': '100%'
            });
        }

        if (!this.bindedResize) {
            $(window).resize(function () {
                this.fix();
                this.fixSidebar();

                $(Selector.logo + ', ' + Selector.sidebar).one('webkitTransitionEnd otransitionend oTransitionEnd msTransitionEnd transitionend', function () {
                    this.fix();
                    this.fixSidebar();
                }.bind(this));
            }.bind(this));

            this.bindedResize = true;
        }

        $(Selector.sidebarMenu).on('expanded.tree', function () {
            this.fix();
            this.fixSidebar();
        }.bind(this));

        $(Selector.sidebarMenu).on('collapsed.tree', function () {
            this.fix();
            this.fixSidebar();
        }.bind(this));
    };

    Layout.prototype.fix = function () {
        // Remove overflow from .wrapper if layout-boxed exists
        $(Selector.layoutBoxed + ' > ' + Selector.wrapper).css('overflow', 'hidden');

        // Get window height and the wrapper height
        var footerHeight = $(Selector.mainFooter).outerHeight() || 0;
        var neg = $(Selector.mainHeader).outerHeight() + footerHeight;
        var windowHeight = $(window).height();
        var sidebarHeight = $(Selector.sidebar).height() || 0;

        // Set the min-height of the content and sidebar based on
        // the height of the document.
        if ($('body').hasClass(ClassName.fixed)) {
            $(Selector.contentWrapper).css('min-height', windowHeight - footerHeight);
        } else {
            var postSetHeight;

            if (windowHeight >= sidebarHeight) {
                $(Selector.contentWrapper).css('min-height', windowHeight - neg);
                postSetHeight = windowHeight - neg;
            } else {
                $(Selector.contentWrapper).css('min-height', sidebarHeight);
                postSetHeight = sidebarHeight;
            }

            // Fix for the control sidebar height
            var $controlSidebar = $(Selector.controlSidebar);
            if (typeof $controlSidebar !== 'undefined') {
                if ($controlSidebar.height() > postSetHeight)
                    $(Selector.contentWrapper).css('min-height', $controlSidebar.height());
            }
        }
    };

    Layout.prototype.fixSidebar = function () {
        // Make sure the body tag has the .fixed class
        if (!$('body').hasClass(ClassName.fixed)) {
            if (typeof $.fn.slimScroll !== 'undefined') {
                $(Selector.sidebar).slimScroll({ destroy: true }).height('auto');
            }
            return;
        }

        // Enable slimscroll for fixed layout
        if (this.options.slimscroll) {
            if (typeof $.fn.slimScroll !== 'undefined') {
                // Destroy if it exists
                // $(Selector.sidebar).slimScroll({ destroy: true }).height('auto')

                // Add slimscroll
                $(Selector.sidebar).slimScroll({
                    height: ($(window).height() - $(Selector.mainHeader).height()) + 'px'
                });
            }
        }
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option === 'object' && option);
                $this.data(DataKey, (data = new Layout(options)));
            }

            if (typeof option === 'string') {
                if (typeof data[option] === 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.layout;

    $.fn.layout = Plugin;
    $.fn.layout.Constuctor = Layout;

    // No conflict mode
    // ================
    $.fn.layout.noConflict = function () {
        $.fn.layout = old;
        return this;
    };

    // Layout DATA-API
    // ===============
    $(window).on('load', function () {
        Plugin.call($('body'));
    });
}(jQuery);


/* PushMenu()
 * ==========
 * Adds the push menu functionality to the sidebar.
 *
 * @usage: $('.btn').pushMenu(options)
 *          or add [data-toggle="push-menu"] to any button
 *          Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.pushmenu';

    var Default = {
        collapseScreenSize: 767,
        expandOnHover: false,
        expandTransitionDelay: 200
    };

    var Selector = {
        collapsed: '.sidebar-collapse',
        open: '.sidebar-open',
        mainSidebar: '.main-sidebar',
        contentWrapper: '.content-wrapper',
        searchInput: '.sidebar-form .form-control',
        button: '[data-toggle="push-menu"]',
        mini: '.sidebar-mini',
        expanded: '.sidebar-expanded-on-hover',
        layoutFixed: '.fixed'
    };

    var ClassName = {
        collapsed: 'sidebar-collapse',
        open: 'sidebar-open',
        mini: 'sidebar-mini',
        expanded: 'sidebar-expanded-on-hover',
        expandFeature: 'sidebar-mini-expand-feature',
        layoutFixed: 'fixed'
    };

    var Event = {
        expanded: 'expanded.pushMenu',
        collapsed: 'collapsed.pushMenu'
    };

    // PushMenu Class Definition
    // =========================
    var PushMenu = function (options) {
        this.options = options;
        this.init();
    };

    PushMenu.prototype.init = function () {
        if (this.options.expandOnHover
            || ($('body').is(Selector.mini + Selector.layoutFixed))) {
            this.expandOnHover();
            $('body').addClass(ClassName.expandFeature);
        }

        $(Selector.contentWrapper).click(function () {
            // Enable hide menu when clicking on the content-wrapper on small screens
            if ($(window).width() <= this.options.collapseScreenSize && $('body').hasClass(ClassName.open)) {
                this.close();
            }
        }.bind(this));

        // __Fix for android devices
        $(Selector.searchInput).click(function (e) {
            e.stopPropagation();
        });
    };

    PushMenu.prototype.toggle = function () {
        var windowWidth = $(window).width();
        var isOpen = !$('body').hasClass(ClassName.collapsed);

        if (windowWidth <= this.options.collapseScreenSize) {
            isOpen = $('body').hasClass(ClassName.open);
        }

        if (!isOpen) {
            this.open();
        } else {
            this.close();
        }
    };

    PushMenu.prototype.open = function () {
        var windowWidth = $(window).width();

        if (windowWidth > this.options.collapseScreenSize) {
            $('body').removeClass(ClassName.collapsed)
                .trigger($.Event(Event.expanded));
        }
        else {
            $('body').addClass(ClassName.open)
                .trigger($.Event(Event.expanded));
        }
    };

    PushMenu.prototype.close = function () {
        var windowWidth = $(window).width();
        if (windowWidth > this.options.collapseScreenSize) {
            $('body').addClass(ClassName.collapsed)
                .trigger($.Event(Event.collapsed));
        } else {
            $('body').removeClass(ClassName.open + ' ' + ClassName.collapsed)
                .trigger($.Event(Event.collapsed));
        }
    };

    PushMenu.prototype.expandOnHover = function () {
        $(Selector.mainSidebar).hover(function () {
            if ($('body').is(Selector.mini + Selector.collapsed)
                && $(window).width() > this.options.collapseScreenSize) {
                this.expand();
            }
        }.bind(this), function () {
            if ($('body').is(Selector.expanded)) {
                this.collapse();
            }
        }.bind(this));
    };

    PushMenu.prototype.expand = function () {
        setTimeout(function () {
            $('body').removeClass(ClassName.collapsed)
                .addClass(ClassName.expanded);
        }, this.options.expandTransitionDelay);
    };

    PushMenu.prototype.collapse = function () {
        setTimeout(function () {
            $('body').removeClass(ClassName.expanded)
                .addClass(ClassName.collapsed);
        }, this.options.expandTransitionDelay);
    };

    // PushMenu Plugin Definition
    // ==========================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new PushMenu(options)));
            }

            if (option === 'toggle') data.toggle();
        });
    }

    var old = $.fn.pushMenu;

    $.fn.pushMenu = Plugin;
    $.fn.pushMenu.Constructor = PushMenu;

    // No Conflict Mode
    // ================
    $.fn.pushMenu.noConflict = function () {
        $.fn.pushMenu = old;
        return this;
    };

    // Data API
    // ========
    $(document).on('click', Selector.button, function (e) {
        e.preventDefault();
        Plugin.call($(this), 'toggle');
    });
    $(window).on('load', function () {
        Plugin.call($(Selector.button));
    });
}(jQuery);


/* TodoList()
 * =========
 * Converts a list into a todoList.
 *
 * @Usage: $('.my-list').todoList(options)
 *         or add [data-widget="todo-list"] to the ul element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.todolist';

    var Default = {
        onCheck: function (item) {
            return item;
        },
        onUnCheck: function (item) {
            return item;
        }
    };

    var Selector = {
        data: '[data-widget="todo-list"]'
    };

    var ClassName = {
        done: 'done'
    };

    // TodoList Class Definition
    // =========================
    var TodoList = function (element, options) {
        this.element = element;
        this.options = options;

        this._setUpListeners();
    };

    TodoList.prototype.toggle = function (item) {
        item.parents(Selector.li).first().toggleClass(ClassName.done);
        if (!item.prop('checked')) {
            this.unCheck(item);
            return;
        }

        this.check(item);
    };

    TodoList.prototype.check = function (item) {
        this.options.onCheck.call(item);
    };

    TodoList.prototype.unCheck = function (item) {
        this.options.onUnCheck.call(item);
    };

    // Private

    TodoList.prototype._setUpListeners = function () {
        var that = this;
        $(this.element).on('change ifChanged', 'input:checkbox', function () {
            that.toggle($(this));
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, (data = new TodoList($this, options)));
            }

            if (typeof data == 'string') {
                if (typeof data[option] == 'undefined') {
                    throw new Error('No method named ' + option);
                }
                data[option]();
            }
        });
    }

    var old = $.fn.todoList;

    $.fn.todoList = Plugin;
    $.fn.todoList.Constructor = TodoList;

    // No Conflict Mode
    // ================
    $.fn.todoList.noConflict = function () {
        $.fn.todoList = old;
        return this;
    };

    // TodoList Data API
    // =================
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);


/* Tree()
 * ======
 * Converts a nested list into a multilevel
 * tree view menu.
 *
 * @Usage: $('.my-menu').tree(options)
 *         or add [data-widget="tree"] to the ul element
 *         Pass any option as data-option="value"
 */
+function ($) {
    'use strict';

    var DataKey = 'lte.tree';

    var Default = {
        animationSpeed: 500,
        accordion: true,
        followLink: false,
        trigger: '.treeview a'
    };

    var Selector = {
        tree: '.tree',
        treeview: '.treeview',
        treeviewMenu: '.treeview-menu',
        open: '.menu-open, .active',
        li: 'li',
        data: '[data-widget="tree"]',
        active: '.active'
    };

    var ClassName = {
        open: 'menu-open',
        tree: 'tree'
    };

    var Event = {
        collapsed: 'collapsed.tree',
        expanded: 'expanded.tree'
    };

    // Tree Class Definition
    // =====================
    var Tree = function (element, options) {
        this.element = element;
        this.options = options;

        $(this.element).addClass(ClassName.tree);

        $(Selector.treeview + Selector.active, this.element).addClass(ClassName.open);

        this._setUpListeners();
    };

    Tree.prototype.toggle = function (link, event) {
        var treeviewMenu = link.next(Selector.treeviewMenu);
        var parentLi = link.parent();
        var isOpen = parentLi.hasClass(ClassName.open);

        if (!parentLi.is(Selector.treeview)) {
            return;
        }

        if (!this.options.followLink || link.attr('href') === '#') {
            event.preventDefault();
        }

        if (isOpen) {
            this.collapse(treeviewMenu, parentLi);
        } else {
            this.expand(treeviewMenu, parentLi);
        }
    };

    Tree.prototype.expand = function (tree, parent) {
        var expandedEvent = $.Event(Event.expanded);

        if (this.options.accordion) {
            var openMenuLi = parent.siblings(Selector.open);
            var openTree = openMenuLi.children(Selector.treeviewMenu);
            this.collapse(openTree, openMenuLi);
        }

        parent.addClass(ClassName.open);
        tree.slideDown(this.options.animationSpeed, function () {
            $(this.element).trigger(expandedEvent);
        }.bind(this));
    };

    Tree.prototype.collapse = function (tree, parentLi) {
        var collapsedEvent = $.Event(Event.collapsed);

        tree.find(Selector.open).removeClass(ClassName.open);
        parentLi.removeClass(ClassName.open);
        tree.slideUp(this.options.animationSpeed, function () {
            tree.find(Selector.open + ' > ' + Selector.treeview).slideUp();
            $(this.element).trigger(collapsedEvent);
        }.bind(this));
    };

    // Private

    Tree.prototype._setUpListeners = function () {
        var that = this;

        $(this.element).on('click', this.options.trigger, function (event) {
            that.toggle($(this), event);
        });
    };

    // Plugin Definition
    // =================
    function Plugin(option) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data(DataKey);

            if (!data) {
                var options = $.extend({}, Default, $this.data(), typeof option == 'object' && option);
                $this.data(DataKey, new Tree($this, options));
            }
        });
    }

    var old = $.fn.tree;

    $.fn.tree = Plugin;
    $.fn.tree.Constructor = Tree;

    // No Conflict Mode
    // ================
    $.fn.tree.noConflict = function () {
        $.fn.tree = old;
        return this;
    };

    // Tree Data API
    // =============
    $(window).on('load', function () {
        $(Selector.data).each(function () {
            Plugin.call($(this));
        });
    });

}(jQuery);

"use strict";
var lstDependency = [];
//lstDependency.push("ngRoute");

var app = angular.module("onfinance", lstDependency).run();

app.directive("onlyNumber", function () {
    return {
        restrict: "A",
        link: function (scope, element, attr) {
            element.bind('input', function () {
                var position = this.selectionStart - 1;

                //remove all but number and .
                var fixed = this.value.replace(/[^0-9\.]/g, '');
                if (fixed.charAt(0) === '.')                  //can't start with .
                    fixed = fixed.slice(1);

                var pos = fixed.indexOf(".") + 1;
                if (pos >= 0)               //avoid more than one .
                    fixed = fixed.substr(0, pos) + fixed.slice(pos).replace('.', '');

                if (this.value != fixed) {
                    this.value = fixed;
                    this.selectionStart = position;
                    this.selectionEnd = position;
                }
            });
        }
    };
});

app.directive('ngMin', function ($rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            scope.$watch(attr.ngMin, function () {
                ctrl.$setViewValue(ctrl.$viewValue);
            });
            var minValidator = function (value) {
                var min = scope.$eval(attr.ngMin) || 0;
                if (!$rootScope.isNullOrEmpty(value) && value < min) {
                    ctrl.$setValidity('ngMin', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMin', true);
                    return value;
                }
            };

            ctrl.$parsers.push(minValidator);
            ctrl.$formatters.push(minValidator);
        }
    };
});
app.directive('ngMax', function ($rootScope) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            elem.bind('input', function () {
                var val = this.value.replace(/[^0-9\.]/g, '');
                var max = parseFloat(scope.$eval(attr.ngMax)) || Infinity;

                var intVal = parseFloat(val);

                if (val != null && val != '' && intVal > max) {
                    while (intVal > max) {
                        val = val.slice(0, -1);
                        intVal = parseFloat(val);
                    }
                    this.value = val;
                }
            });
        }
    };
});

app.directive('focusOn', ['$timeout',
    function ($timeout) {
        var checkDirectivePrerequisites = function (attrs) {
            if (!attrs.focusOn && attrs.focusOn != "") {
                throw "FocusOnCondition missing attribute to evaluate";
            }
        }

        return {
            restrict: "A",
            link: function (scope, element, attrs, ctrls) {
                checkDirectivePrerequisites(attrs);

                scope.$watch(attrs.focusOn, function (currentValue, lastValue) {
                    if (currentValue == true) {
                        $timeout(function () {
                            element.focus();
                        });
                    }
                });
            }
        };
    }
]);

app.directive('selectOnClick', ['$window', function ($window) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function () {
                if (!$window.getSelection().toString()) {
                    // Required for mobile Safari
                    this.setSelectionRange(0, this.value.length)
                }
            });
        }
    };
}]);

app.directive('convertToNumber', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (val) {
                return parseInt(val, 10);
            });
            ngModel.$formatters.push(function (val) {
                return '' + val;
            });
        }
    }
});

app.directive('customOnChange', ['$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $timeout(function () {
                    var onChangeFunc = scope.$eval(attrs.customOnChange);
                    element.bind('change', onChangeFunc);
                }, 100);
            }
        };
    }]
);

app.filter('numberFormat', function () {
    return function (n, decPlaces, thouSeparator, decSeparator, trimZero, emptyWhenZero) {
        try {
            if (isNaN(n)) {
                return 0;
            }

            if (n == 0) {
                if (emptyWhenZero == "true") {
                    return "";
                } else if (trimZero == "true") {
                    return 0;
                }
            }

            var decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
                decSeparator = decSeparator == undefined ? "." : decSeparator,
                thouSeparator = thouSeparator == undefined ? "," : thouSeparator,
                sign = n < 0 ? "-" : "",
                i = parseInt(n = Math.abs(+n || 0).toFixed(decPlaces)) + "",
                j = (j = i.length) > 3 ? j % 3 : 0;
            return sign + (j ? i.substr(0, j) + thouSeparator : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thouSeparator) + (decPlaces ? decSeparator + Math.abs(n - i).toFixed(decPlaces).slice(2) : "");
        } catch (e) {
            return n;
        }
    }
});

app.filter('dateTimeFormat', function ($filter) {
    function getCurrentDate() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = "0" + dd;
        }

        if (mm < 10) {
            mm = "0" + mm;
        }

        today = dd + "/" + mm + "/" + yyyy;
        return today;
    };

    return function (input) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var date;
        var resultDate;

        if (input.indexOf("Date") > -1) {

            resultDate = new Date(+temp);
            date = $filter("date")(resultDate, "dd/MM/yyyy");


            var utc = resultDate.getTime() + (resultDate.getTimezoneOffset() * 60000);

            // create new Date object for different city
            // using supplied offset
            resultDate = new Date(utc + (3600000 * 7));

            if (getCurrentDate() === date) {
                return $filter("date")(resultDate, "HH:mm") + " Hôm nay";
            } else {
                return $filter("date")(resultDate, "HH:mm ") + " " + $filter("date")(resultDate, "dd/MM/yyyy");
            }

        } else {

            date = $filter("date")(new Date(temp), "dd/MM/yyyy");

            if (getCurrentDate() === date) {
                return "Hôm nay";
            } else {
                var utc = date.getTime() + (date.getTimezoneOffset() * 60000);

                // create new Date object for different city
                // using supplied offset
                resultDate = new Date(utc + (3600000 * 7));
                return $filter("date")(resultDate, "dd/MM/yyyy");
            }
        }
    };
});


app.filter('dateFormat', function ($filter) {
    return function (input, format) {
        if (!input) { return ""; }
        var temp = input.replace(/\//g, "").replace("(", "").replace(")", "").replace("Date", "").replace("+0700", "").replace("-0000", "");

        var resultDate = new Date(+temp);
        return $filter("date")(resultDate, format);
    };
});

app.filter('sumTotal', function () {
    return function (input, property) {
        var i = input instanceof Array ? input.length : 0;
        if (typeof property === 'undefined' || i === 0) {
            return i;
        } else if (isNaN(input[0][property])) {
            throw 'filter total can count only numeric values';
        } else {
            var total = 0;
            while (i--)
                total += parseFloat(input[i][property]);
            return total;
        }
    };
});

app.filter('totalSumPriceQty', function () {
    return function (data, key1, key2) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum = sum + (parseFloat(value[key1], 10) * parseFloat(value[key2], 10));
        });
        return sum;
    }
});

app.filter('totalWithOperation', function () {
    return function (data, key1, key2, operation) {
        if (angular.isUndefined(data) || angular.isUndefined(key1) || angular.isUndefined(key2) || angular.isUndefined(operation))
            return 0;
        var sum = 0;
        angular.forEach(data, function (value) {
            sum += parseFloat(eval(value[key1] + operation + value[key2]));
        });
        return sum;
    }
});

app.filter('Contains', function () {
    return function (string, substring) {
        return (string.indexOf(substring) != -1);
    };
});

app.filter('IndexOf', function () {
    return function (string, substring) {
        if (!string)
            return false;
        else
            return (string.indexOf(substring) != -1);
    };
});



function SetVietNameInterface(ctr) {
    ctr.datepicker("option", "monthNames", ['Tháng một', 'Tháng hai', 'Tháng ba', 'Tháng bốn', 'Tháng năm', 'Tháng sáu', 'Tháng bảy', 'Tháng tám', 'Tháng chín', 'Tháng mười', 'Tháng mười một', 'Tháng mười hai']);
    ctr.datepicker("option", "monthNamesShort", ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12']);
    ctr.datepicker("option", "dayNamesShort", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNamesMin", ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']);
    ctr.datepicker("option", "dayNames", ['Chủ nhật', 'Thứ hai', 'Thứ ba', 'Thứ tư', 'Thứ năm', 'Thứ sáu', 'Thứ bảy']);
}

function SetDateTimePicker(ctr) {
    ctr.datetimepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function SetDatePicker(ctr) {
    ctr.datepicker({ maxDate: null, dateFormat: "dd/mm/yy" })
    SetVietNameInterface(ctr);
}

function fdelFromAreaPrice(div) {
    if (!confirm('Xác nhận xóa?')) return false;

    var boxPrice = $("#" + $(div).attr("rel"));
    var idPrice = $(div).attr("data-remove");
    if (idPrice == "") {
        boxPrice.hide("fast", function () {
            boxPrice.remove();
        });
        return;
    }
    var randomUnique = "del" + Math.round(new Date().getTime() + (Math.random() * 100));
    $.ajax({
        url: "/Products/Deletespecial/" + randomUnique,
        type: "POST",
        data: { salepriceid: idPrice },
        success: function (result) {
            boxPrice.hide("fast", function () {
                boxPrice.remove();
            });
        },
        error: function (result) {
            alert("Lỗi???\n" + result.responseText);
        }
    });

    return true;
}

function onsubmitform(div) {
    var $form = $(div);
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = $form.attr("action") + "/" + randomUnique;
    $.ajax({
        url: action,
        type: "POST",
        data: $form.serializeArray(),
        success: function (result) {
            $form.find(".cmddel_special_price").attr("data-remove", result.responseId);
            $form.find('input[name="hSalepriceID"]').val(result.responseId);
            alert(result.responseText);
        },
        error: function (result) {
            alert("Lỗi truyền dữ liệu, thông tin chưa lưu.\n" + result.responseText);
        }
    });
    return false;
}

function autoSalePrice(input) {
    var $input = $(input);
    var percent = $input.val();
    var costprice = $input.parent().parent().find('input[name="COSTPRICE"]').val();
    var saleprice = (percent / 100) * costprice + parseInt(costprice);
    $input.parent().parent().find('input[name="SALEPRICE"]').val(Math.floor(saleprice / 100) * 100);
}

function autoPercentage(input) {
    var $input = $(input);
    var saleprice = parseInt($input.val());
    var costprice = parseInt($input.parent().parent().find('input[name="COSTPRICE"]').val());

    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    if ((saleprice - costprice) <= 0) {
        oPercent.val(0);
    } else {
        oPercent.val(((saleprice - costprice) / costprice) * 100);
    }
}

function autoSalePriceByCost(input) {
    var $input = $(input);
    var costprice = parseInt($input.val());
    var oPercent = $input.parent().parent().find('input[name="PROFITPERCENTAGE"]');
    var oSalePrice = $input.parent().parent().find('input[name="SALEPRICE"]');
    var percent = numeral(oPercent.val()).format('0,0.00');
    if (oPercent.val() == '') {
        oPercent.val(0);
    }
    var saleprice = (percent / 100) * costprice + costprice;
    oSalePrice.val(Math.floor(saleprice / 100) * 100);
}

function uniqueCheckRadio(radio) {
    var $this = $(radio);
    var oPrice = $this.parent().find('input[name="pricespecial"]');
    var valPrice = oPrice.val();
    $this.parent().parent().find('input[name="pricespecial"]').val(0);
    oPrice.val(valPrice);
}

function loadCustomer(input) {
    var cName = $("#CustomerName");
    var cAddr = $("#CustomerAddress");
    var cId = $("#CustomerId");
    if (cName.text() == "" && input.val() != "") {
        var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
        $.post("/SearchInputVoucher/GetCustomer", { custax: input.val(), randomUnique: randomUnique })
        .done(function (result) {            
            if (result.responseName != "") {
                cName.text(result.responseName);
                cAddr.text(result.responseAddress);
                cId.val(result.responseId);
            } else {
                alert("Không thể tìm thấy nhà cung cấp có mã số thuế: " + input.val());
                input.focus();
            }
        })
        .fail(function (result) {
            alert("Lỗi không thể tìm nhà cung cấp");
        });
    }
    return false;
}

function delTableRow(td) {
    //var productid = td.parent().find("input[name='ProductID']").val();
    if (!confirm("Bạn chắc chắn muốn xóa dòng này?"))
        return false;
    
    //td.parent().hide();
    td.parent().hide("fast", function () {
        td.parent().remove();
        if ($("#listRowDetail tr:not(:hidden)").length <= 0) {
            $("select[name='VATInput']").val(0);
            $("#VoucherDiscount").val(0);
            $("#totalVoucherMoney").val(0);
            $("#totalVATVoucher").val(0);
            $("#TotalAmountVoucher").val(0);
        }
        updateTotalMoney();
        updateTotalDiscount();
    });    
}

var timerId;

function popupSelectOneProduct(jsobj, callback) {
    var obj = JSON.parse(jsobj);
    if (obj.length == 1) {
        callback(obj[0]);
    } else if (obj.length > 1) {
        var div = $('<div class="popupSelectOneProduct">');
        var i = 0;
        obj.forEach(function (o) {
            var guid = "id" + o.ProductID;
            var input = $('<input type="radio"/>').attr('id', guid).attr('name', 'UserSelectedProductID').val(i)
                .keyup(function (e) {
                    if (e.keyCode == 13) {
                        var selectedIndex = $(this).val();
                        callback(obj[selectedIndex]);
                        $('.popupSelectOneProduct').dialog("close");
                    }
                });
            var label = $('<label/>').attr('for', guid).text(" " + o.ProductName + "(" + o.UnitName + ")").css('color', '#188338').prepend(input);
            var p = $('<p/>').append(label);
            div.append(p);
            i++;
        });

        div.dialog({
            title: 'Chọn sản phẩm',
            modal: true,
            resizable: false,
            width: 600,
            height: 250,
            close: function () {
                $('.popupSelectOneProduct').remove();
            },
            buttons: [{
                text: "Ok",
                icons: {
                    primary: "ui-icon-heart"
                },
                click: function () {
                    var selectedIndex = $('input[name="UserSelectedProductID"]:checked').val();
                    callback(obj[selectedIndex]);
                    $(this).dialog("close");
                }
            }]
        });
    }
}

function loadProduct(input) {
    var _tr = input.parent().parent();
    var ProductID   = input;
    var ProductName = _tr.find('span[data-field="ProductName"]');
    var UnitID = _tr.find('input[name="QUANTITYUNITID"]');
    var UnitName = _tr.find('span[data-field="UnitName"]');
    var Quantity    = _tr.find('input[name="Quantity"]');
    var Price       = _tr.find('span[data-field="Price"]');
    var ToMoney     = _tr.find('input[name="ToMoney"]');
    var Discount = _tr.find('input[name="Discount"]');

    if (ProductName.text() == "" && input.val() != "") {
        var f = 0;
        $("#listRowDetail tr:not(:hidden) input[name='ProductID']").each(function (index) {
            if ($(this).val() == input.val()) f++;                                
        });
        if (f > 2) {
            alert("Sản phẩm đã tồn tại, vui lòng kiểm tra lại");
            input.val("");
            input.focus();
        } else {
            var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
            $.post("/SearchInputVoucher/GetProduct", { productid: input.val(), InputTypeInput: $("#InputTypeInput").val(), randomUnique: randomUnique })
            .done(function (result) {
                if (result.success) {
                    var callback = function (o) {
                        input.val(o.ProductID.trim());
                        ProductName.text(o.ProductName);
                        UnitName.text(o.UnitName);
                        Quantity.val(1);
                        Price.val(0);
                        ToMoney.val(0);
                        Discount.val(0);
                        UnitID.val(o.UnitID);
                    };

                    var o = popupSelectOneProduct(result.jsobj, callback);

                    //$("td.button-new-rowdetail").trigger("click");

                } else {
                    alert("Không tìm thấy sản phẩm: " + input.val());
                    input.focus();
                }
            })
            .fail(function (result) {
                alert("Lỗi không thể tìm sản phẩm");
            });
        }
    }
    return false;
}


function updateTotalMoney(input) {
    if (input != null) {
        var parent = input.parent().parent();
        var newQuantity = parseFloat(parent.find('input[name="Quantity"]').autoNumeric('get'));
        //var quantityunit = parent.find('span[data-field="UnitName"]').text();
        //if (quantityunit == 'Kg') {
        //    var oldQuantity = parseFloat(parent.find('input[name="oldQuantity"]').val());
        //    if (Math.abs(oldQuantity / newQuantity) > 0.5) {
        //        alert("");
        //    }
        //}
        var price = parseInt(parent.find('input[name="ToMoney"]').autoNumeric('get')) / newQuantity;
        parent.find("span[data-field='Price']").text($.number(price, 2));
    }
    var total = 0;
    $("#listRowDetail tr:not(:hidden) input[name='ToMoney']").each(function () {
        if ($(this).val() != '')
            total += parseInt($(this).autoNumeric('get'));
    });
    $("#sumToTalMoneyRow").val(total);
    updateSumMoneyVoucher();
}

function updateSumMoneyVoucher() {

   

    var sumVATVoucher = 0;
    var sumToTalMoneyRow = parseInt($("#sumToTalMoneyRow").val());
    var sumToTalDiscountRow = parseInt($("#sumToTalDiscountRow").val());
    var voucherDiscount = parseInt($("#VoucherDiscount").autoNumeric('get'));
    var VAT = parseInt($("select[name='VATInput']").val());

    var totalVoucherMoney = $("#totalVoucherMoney");
    var totalVATVoucher = $("#totalVATVoucher");
    var totalMoney = sumToTalMoneyRow - sumToTalDiscountRow - voucherDiscount;    
    var totalMoneyProduct = sumToTalMoneyRow - sumToTalDiscountRow;
    $("#listRowDetail tr:not(:hidden)").each(function () {
        var ttsp = parseInt($(this).find("input[name='ToMoney']").autoNumeric('get'));
        var cksp = parseInt($(this).find("input[name='Discount']").autoNumeric('get'));
        var vatsp = parseInt($(this).find("select[name='VAT']").val());
        sumVATVoucher += ((vatsp / 100) * (ttsp - cksp - (voucherDiscount / sumToTalMoneyRow * ttsp)));
        
        //(1 - voucherDiscount / totalMoneyProduct)
    });
    //console.log(totalMoney);
    totalVoucherMoney.val(totalMoney);

    //var sumVATVoucher = (VAT / 100) * totalMoney;
    
    totalVATVoucher.val(sumVATVoucher);
    $("#TotalAmountVoucher").val(totalMoney + sumVATVoucher);
}

function updateTotalDiscount() {
    var discount = 0;
    $("tr:not(:hidden) input[name='Discount']").each(function () {
        if ($(this).val() != '') {
            
            discount += parseInt($(this).autoNumeric('get'));
        }
    });
    $("#sumToTalDiscountRow").val(discount);
    updateSumMoneyVoucher();
}

function checkVoucherDiscount(obj) {
    var vDiscount = parseInt($(obj).autoNumeric('get'));
    var vTotal = parseInt($("#sumToTalMoneyRow").val());
    if (vDiscount > vTotal) {
        alert("Chiết khấu (theo hóa đơn) phải nhỏ hơn hoặc bằng tổng thành tiền: ");
        setTimeout(function () { obj.focus() }, 10);
    }
}

function hiddenRow(btn) {
    if (confirm("Bạn chắc chắn muốn xóa dòng này?")) {
        btn.parent().hide(300, function () {
            $(this).remove();
        });
    }
    return false;
}

function getDecimalSeparator() {
    var n = 3/2;
    n = n.toLocaleString().substring(1, 2);
    return n;
}

//var cHamxuly = function () { };
function openPopupSelectProduct(_this, callback, storeid) {
    
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    storeid = storeid == null ? 0 : storeid;

    $.post("/Common/SelectProduct", { storeid: storeid })
    .done(function (result) {
        var modal = $('<div class="popupselectproduct"/>').dialog({
            title: 'Chọn sản phẩm',
            modal: true,
            resizable: false,
            width: 1000,
            height: 650,
            close: function () {
                btn.removeAttr("disabled");
                $('.popupselectproduct').remove();
                setTimeout(function () {
                    callbackExec(null);
                }, 500);
            }
        });
        modal.html(result);
        //$("select#ddlpSubGroup").multiselect('refresh');
    });
}

var callbackExec = function (str) { };

function openPopupSelectItems(_this, callback) {

    var btn = $(_this);
    btn.attr("disabled", "disabled");
    callbackExec = callback;

    $.post("/Common/SelectItems")
        .done(function (result) {
            var modal = $('<div class="popupselectproduct"/>').dialog({
                title: 'Chọn Item',
                modal: true,
                resizable: false,
                width: 800,
                height: 670,
                close: function () {
                    btn.removeAttr("disabled");
                    $('.popupselectproduct').remove();
                    //setTimeout(function () {
                    //    callback(null);
                    //}, 500);
                }
            });
            modal.html(result);
        });
}

function openPopupSelectMarket(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectMarket", params)
    .done(function (result) {
        var modal = $('<div class="popupselectproduct"/>').dialog({
            title: 'Chọn chợ',
            modal: true,
            resizable: false,
            width: 1000,
            height: 600,
            close: function () {
                btn.removeAttr("disabled");

                $('.popupselectproduct').remove();
            }
        });
        modal.html(result);
    });
}

function openPopupSelectCombo(_this, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectCombo")
    .done(function (result) {
        var modal = $('<div class="popupselectproduct"/>').dialog({
            title: 'Chọn Combo',
            modal: true,
            resizable: false,
            width: 1000,
            height: 600,
            close: function () {
                btn.removeAttr("disabled");

                $('.popupselectproduct').remove();
            }
        });
        modal.html(result);
    });
}

function openPopupSelectStore(_this, params, callback) {
    var btn = $(_this);
    btn.attr("disabled", "disabled");

    callbackExec = callback;

    $.post("/Common/SelectStore", params)
    .done(function (result) {
        var modal = $('<div class="popupselectproduct"/>').dialog({
            title: 'Chọn siêu thị',
            modal: true,
            resizable: false,
            width: 1000,
            height: 600,
            close: function () {
                btn.removeAttr("disabled");

                $('.popupselectproduct').remove();
            }
        });
        modal.html(result);
    });
}

var $POSloading = {};

function RefreshSession() {
    var randomUnique = "var" + Math.round(new Date().getTime() + (Math.random() * 100));
    var action = "/Account/RefreshSession/" + randomUnique;
    $.post(action, {})
        .done(function (response) {
            if (response) {
                console.log(response.msg);
                //Nếu session null
                if (!response.rs) {
                    if (window.location.href.indexOf('Account/Login') == -1) {
                        window.location.reload();
                    }
                }
            } else {
                console.log("Lỗi không nhận được phản hồi - RefreshSession (js) ");
            }
        })
        .fail(function (result) {
            console.log(response.msg);
        });
}


$(function ()
{
    //Giữ session
    if (window.location.href.indexOf('Account/Login') == -1) {
        var stopRequest = setInterval(RefreshSession, 300000);
    }

    $('#modelLoading').modal({
        show: false,
        keyboard: false
    });
    $POSloading.show = function () {
        $('#modelLoading').modal('show');
    };
    $POSloading.hide = function () {
        $('#modelLoading').modal('hide');
    };

    //var lH = $("#leftContent").height();
    //$('#mainContent').resize(function () {
    //    var mH = $(this).height();
    //    if (lH < mH) {//$("#leftContent").height(mH);
    //        $("#leftContent").animate({
    //            height: mH
    //        }, 100);
    //    }
    //});
    //$('#mainContent').resize();

    /* DataTables */
    if ($('.dynamicTable').length > 0)
	{
		$('.dynamicTable').dataTable({
			"sPaginationType": "bootstrap",
			"sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
			"oLanguage": {
				"sLengthMenu": "_MENU_ số dòng mỗi trang"
			}
		});
	}

	$("input[name*='typeprice_']").click(function () {
	    uniqueCheckRadio(this);
	});

	$('form#frmSearchDoPrice').submit(function () {
	    var input = $(this).find(".seachInput");
	    if (input.val() == "") {
	        alert("Vui lòng nhập mã sản phẩm hoặc tên sản phẩm cần tiềm kiếm");
	        input.focus();
	        return false;
	    }
	    return true;            
	});

	$('#btnSubmitNewPermission').click(function () {
	    var form = $("#frmAddPermission");
	    if ($("#idPermission").val() == '') {
	        alert("Chưa nhập hoặc nhập sai mã quyền");
	        $("#idPermission").focus();
	        return false;
	    }
	    if ($("#namePermission").val() == '') {
	        alert("Chưa nhập tên quyền");
	        $("#namePermission").focus();
	        return false;
	    }
	    $.post("/Permission/GetPermissionById", form.serializeArray())
        .done(function (result) {
            if (result.perId != "") {
                alert("Mã quyền đã tồn tại, vui lòng chọn mã quyền khác");
                $("#idPermission").focus();
                return false;
            } else
                form.submit();
        }).fail(function (result) {
            alert("Lỗi không thể lưu thông tin");
            return false;
        });
	    return false;
	});

	$(".cmddel_special_price").click(function () {
	    return fdelFromAreaPrice(this);
	});
	
	$("a.cmdadnew_special_price").click(function () {
	    var formhtml = $("#templateFrom").html();
	    var hProductID = $(this).attr("data-product");
	    var hPriceAreaID = $(this).attr("data-pricearea");
	    var hrBeforarea = $("#" + $(this).attr("rel"));
	    var formID = "frm_" + Math.round(new Date().getTime() + (Math.random() * 100));
	    var $form = $("<form action='/Products/Special' method='post'></form>").addClass("frmajaxload").attr({ id: formID, onSubmit: "return onsubmitform(this)" }).append(formhtml);

	    hrBeforarea.after($form);

	    $("#" + formID + " a.cmddel_special_price").attr({ rel: formID, onclick: "fdelFromAreaPrice(this)" });
	    $("#" + formID + " .row2 input[type='radio']").attr("name", "typeprice_" + formID);

	    $("#" + formID + " input[data-id='tempradio1']").attr("id", "a_" + formID).click(function () { uniqueCheckRadio(this); });
	    $("#" + formID + " label[data-id='tempradio1']").attr("for", "a_" + formID);

	    $("#" + formID + " input[data-id='tempradio2']").attr("id", "b_" + formID).click(function () { uniqueCheckRadio(this); });
	    $("#" + formID + " label[data-id='tempradio2']").attr("for", "b_" + formID);

	    $("#" + formID + " input[data-id='tempradio3']").attr("id", "c_" + formID).click(function () { uniqueCheckRadio(this); });
	    $("#" + formID + " label[data-id='tempradio3']").attr("for", "c_" + formID);

	    //$("#" + formID + " input.datepicker_load").datepicker({ dateFormat: 'dd/mm/yy' });
	    $("#" + formID + " input.timepicker_load").timepicker({ stepMinute: 5 });

	    $("#" + formID + " input.pricespecial_load").number(true, 0);

	    $("#" + formID + " input[name='htypeprice']").val("typeprice_" + formID);
	    $("#" + formID + " input[name='hProductID']").val(hProductID);
	    $("#" + formID + " input[name='hAreaID']").val(hPriceAreaID);


	    SetDatePicker($("#" + formID + " input.datepicker_load"));
	});

	$('form.frmajaxload').submit(function () {
	    return onsubmitform(this);
	});	

	SetDatePicker($(".datepicker"));	

	$('.timepicker').timepicker({
	    timeFormat: 'HH:mm',
	    stepMinute: 5
	});

	$('input[name="PROFITPERCENTAGE"]').number(true, 2);
	$('input.priceinput').number(true, 0);
	//$('input.checkpriceinput').number(true, 0);

	$("#btn_EditInputVoucher").click(function () {
	    $("#btn_AcceptInputVoucher").hide();
	    $("#btn_SaveInputVoucher").show();
	    $("#btn_CacelEditInputVoucher").show();
	    $(".hidden-col").show();
	    $(".stt-col").hide();
	    $(this).remove();

	    $(".text_editable").removeAttr('disabled');
	    $("#CustomerTax").keyup(function (e) {
	        var cName = $("#CustomerName");
	        var cAddr = $("#CustomerAddress");
	        var cId = $("#CustomerId");
	        cName.text("");
	        cAddr.text("");
	        cId.val("");
	        if (e.keyCode == 13) {
	            $(this).blur();
	        }
	    }).focusout(function () {
	        loadCustomer($(this));
	    });

	    return false;
	});
    $("#btn_EditInputAcceptVoucher").click(function() {
        $("#btn_AcceptInputVoucher").hide();
        $("#btn_AcceptStockTransfer").hide();
        $("#btn_SaveInputAcceptVoucher").show();
        $("#btn_CacelEditInputAcceptVoucher").show();
        //$(".hidden-col").show();
        //$(".stt-col").hide();
        $(this).remove();
        $(".text_editaccept").removeAttr('disabled');


         return false;
    });

	$("td.button-new-rowdetail").click(function () {
	    var template = $("table tr#rowTemplate:first").clone();
	    template.removeAttr("id");
	    template.find("td.button-delete-rowdetail").click(function () { delTableRow($(this)); });
	    template.find('input[name="ProductID"]').keyup(function (e) {
	        if (e.keyCode == 13) {
	            $(this).parent().parent().find('input[name="Quantity"]').focus();
	        } else {
	            $(this).parent().parent().find('span[data-field="ProductName"]').text("");
	            //$(this).parent().parent().find('input[name="VAT"]').val(0);
	            $(this).parent().parent().find('span[data-field="UnitName"]').text("");
	            $(this).parent().parent().find('input[name="Quantity"]').val(0);
	            $(this).parent().parent().find('span[data-field="Price"]').text("");
	            $(this).parent().parent().find('input[name="ToMoney"]').val(0);
	            $(this).parent().parent().find('input[name="Discount"]').val(0);	            
	        }
	    }).focusout(function () {
	        loadProduct($(this));
	    });
	    template.find('select[name="VAT"]').change(function (e) {
	        $('input[name="VAT"]').val($(this).val());
	        updateSumMoneyVoucher();
	    })
	    .val($('select[name="VATInput"]').val());

	    template.find('input[name="Quantity"], input[name="InQuantity"], input[name="OutQuantity"]').keyup(function (e) {
	        if (e.keyCode == 13) {
	            $(this).parent().parent().find('input[name="ToMoney"]').focus().select();;
	        }
	    }).focusout(function () {
	        updateTotalMoney($(this));
	    }).autoNumeric('init', { mDec: 3, vMin: 0, vMax: 999999 });

	    template.find('input[name="ToMoney"]').focusout(function () {
	        updateTotalMoney($(this));
	    }).keyup(function (e) {
	        if (e.keyCode == 13) {
	            $(this).parent().parent().find('input[name="Discount"]').focus().select();;
	        }
	    }).autoNumeric('init', { mDec: 0, vMin: 0, vMax: 9999999999 });
	    template.find('input[name="Discount"]').focusout(function () {
	        if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
	            alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
	            $(this).val(0);
	            setTimeout(function () { $(this).focus() }, 10);
	        }
	        updateTotalDiscount();
	    }).keyup(function (e) {
	        if (e.keyCode == 13) {
	            $(this).blur();
	        }
	    }).autoNumeric('init', { mDec: 0, vMin: 0 });

	    $("#listRowDetail").append(template);
	});
	
	$(".button-delete-rowdetail").click(function () {
	    delTableRow($(this));
	});

	var keyold = '';
	$('input[name="ProductID"]').keydown(function (e) {
	    keyold = $(this).val();
	});
    $('input[name="ProductID"]').keyup(function (e) {
        var parent = $(this).parent().parent();
        if (e.keyCode == 13) {
            parent.find('input[name="Quantity"]').focus();
        } else if (keyold != $(this).val()) {
            parent.find('span[data-field="ProductName"]').text("");
            //parent.find('input[name="VAT"]').val(0);
            parent.find('span[data-field="UnitName"]').text("");
            parent.find('input[name="Quantity"]').val(0);
            parent.find('span[data-field="Price"]').text("");
            parent.find('input[name="ToMoney"]').val(0);
            parent.find('input[name="Discount"]').val(0);
            parent.find('input[name="action"]').val(2);

            updateTotalMoney();
            updateTotalDiscount();
        }
	}).focusout(function () {
	    loadProduct($(this));
	});

    $('input[name="Quantity"]').keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="ToMoney"]').focus().select();
        }
    }).focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    });

    $('input[name="ToMoney"]').focusout(function () {
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalMoney($(this));
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).parent().parent().find('input[name="Discount"]').focus().select();
        }
    });

    $('input[name="Discount"]').focusout(function () {
        if (parseInt($(this).autoNumeric('get')) > parseInt($(this).parent().parent().find('input[name="ToMoney"]').autoNumeric('get'))) {
            alert("Tiền chiết khấu theo sản phẩm phải nhỏ hơn hoặc bằng thành tiền sản phẩm.");
            $(this).val(0);
            setTimeout(function () { $(this).focus() }, 10);
        }
        $(this).parent().parent().find('input[name="action"]').val(2);
        updateTotalDiscount();
    }).keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("#VoucherDiscount").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    }).focusout(function () {
        updateSumMoneyVoucher();
    });

    $("select[name='VATInput']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });

    $("select[name='VAT']").change(function (e) {
        $('input[name="VAT"]').val($(this).val());
        updateSumMoneyVoucher();
    });


});

app.directive('clearabletextbox', function ($timeout) {
    return {
        restrict: "E",

        scope: {
            iconposition: "@",
            textboxtype: "@",
            textboxstyle: "@",
            textboxplaceholder: "@",
            //textboxmodel: "=",
            valuemodel: "=",
            textboxmaxlength: "@",
            resultchange: "&",
            resultchangedelay: "@",
            resultchangeminlength: "@",
            submitonenter: "&",
            cleartext: "&",
            blur: "&",
            disable: "=",
            textalignment: "@",
            isnumberformat: "=",
            islargetext: "=",
            isclickclear: "=",
            disablenumber: "=",
            allowonlynumber: "=",
            nfocus: "=",
            isinteger: "=",
            focustextbox: "="
        },

        transclude: true,
        templateUrl: "/Directives/ClearableTextBox.html?v=2",

        link: function (scope, element, attrs) {

            //#region Variable

            var submissionHandler = null;

            //#endregion

            //#region Function

            //#region Support

            var formatNumber = function () {
                scope.textboxmodel = scope.textboxmodel.trim();

                if (scope.textboxmodel.length > 1) {
                    while (scope.textboxmodel.charAt(0) == "0" && scope.textboxmodel.charAt(1) != ".") {
                        scope.textboxmodel = scope.textboxmodel.substr(1);
                    }

                    var field = scope.textboxmodel.replace(/[^\d.\','] /g, "");
                    var point = field.indexOf(".");

                    if (point >= 0) {
                        field = field.slice(0, point + 3);
                    }

                    var decimalSplit = field.split(".");
                    var intPart = decimalSplit[0];
                    var decPart = decimalSplit[1];

                    intPart = intPart.replace(/[^\d]/g, "");

                    if (intPart.length > 3) {
                        var intDiv = Math.floor(intPart.length / 3);

                        while (intDiv > 0) {
                            var lastComma = intPart.indexOf(",");

                            if (lastComma < 0) {
                                lastComma = intPart.length;
                            }

                            if (lastComma - 3 > 0) {
                                intPart = intPart.substr(0, lastComma - 3) + "," + intPart.substr(lastComma - 3);//intPart.splice(lastComma - 3, 0, ",");
                            }

                            --intDiv;
                        }
                    }

                    if (decPart === undefined) {
                        decPart = "";
                    } else {
                        decPart = "." + decPart;
                    }

                    var res = intPart + decPart;

                    scope.textboxmodel = res;
                    scope.modelnotcomma = 1000;
                    try {
                        var inputelement = element[0].children[0].children[0];
                        // Cache references
                        var $el = $(inputelement),
                            el = inputelement;

                        // Only focus if input isn't already
                        if (!$el.is(":focus")) {
                            $el.focus();
                        }

                        // If this function exists... (IE 9+)
                        if (el.setSelectionRange) {

                            // Double the length because Opera is inconsistent about whether a carriage return is one character or two.
                            var len = $el.val().length * 2;

                            // Timeout seems to be required for Blink
                            setTimeout(function () {
                                el.setSelectionRange(len, len);
                            }, 1);

                        } else {
                            // As a fallback, replace the contents with itself
                            // Doesn't work in Chrome, but Chrome supports setSelectionRange
                            $el.val($el.val());

                        }

                        // Scroll to the bottom, in case we're in a tall textarea
                        // (Necessary for Firefox and Chrome)
                        this.scrollTop = 999999;


                    } catch (e) {

                    }

                }
            }

            var submit = function () {
                if (submissionHandler) {
                    $timeout.cancel(submissionHandler);
                }

                if (!scope.resultchangeminlength || (scope.textboxmodel && scope.textboxmodel.trim().length >= scope.resultchangeminlength)) {
                    submissionHandler = $timeout(function () {
                        scope.resultchange();
                    }, scope.resultchangedelay);
                }
            };

            //#endregion

            //#region Verify

            //#endregion

            //#region Logic

            scope.Change = function () {
                if (scope.textboxmodel == null) {
                    scope.textboxmodel = "";
                }

                if (scope.isnumberformat) {
                    if (isNaN(parseFloat(scope.textboxmodel))) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }

                if (scope.isinteger) {
                    var transformedInput = scope.textboxmodel.replace(/[^0-9]/g, '');
                    scope.textboxmodel = scope.textboxmodel.replace(',', '');
                    // transformedInput.replace(' ', '');
                    if (transformedInput != scope.textboxmodel) {
                        scope.textboxmodel = "";
                    }
                    else {
                        formatNumber();
                    }
                }

                if (scope.resultchangedelay) {
                    submit();
                } else if (scope.resultchange) {
                    $timeout(scope.resultchange);
                }

                //Gán giá trị không có định dạng
                var NoneDim = scope.textboxmodel.replace(/[^\d]/g, "");
                if (isNaN(NoneDim))
                    scope.valuemodel = 0;
                else
                    scope.valuemodel = parseFloat(NoneDim);
            }

            scope.KeyPress = function (event) {
                if (scope.submitonenter && (event.keyCode == 13 || event.which == 13)) {
                    $timeout(scope.submitonenter);
                }
            }

            scope.KeyDown = function (event) {
                if (scope.isnumberformat || scope.textboxtype == "tel") {
                    if ((!event.ctrlKey && (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105) &&
                        event.keyCode != 46 && event.keyCode != 13 && event.keyCode != 8 &&
                        event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 46 && event.keyCode != 8 &&
                        (event.keyCode != 190 || scope.textboxmodel.indexOf(".") > -1)) ||
                        event.shiftKey) {
                        event.preventDefault();
                    }
                }

                if (scope.disablenumber && event.keyCode > 48 && event.keyCode < 57) {
                    event.preventDefault();
                }

                if (scope.textboxmodel && scope.textboxmodel.length == scope.textboxmaxlength && event.keyCode != 46 && event.keyCode != 8) {
                    event.preventDefault();
                }
            }

            scope.Clear = function () {
                scope.textboxmodel = "";

                if (scope.cleartext) {
                    $timeout(scope.cleartext);
                }
            }

            scope.ClickClear = function () {
                if (scope.isclickclear) {
                    scope.textboxmodel = "";
                    scope.valuemodel = "";
                }
            }

            scope.Blur = function () {
                if (scope.blur) {
                    $timeout(scope.blur);
                }

            }

            //#endregion

            //#endregion

            //#region Init

            var initialise = function () {
                if (scope.isnumberformat && scope.textboxtype != 'number') {
                    scope.type = "tel";
                } else {
                    scope.type = scope.textboxtype;
                }

                scope.textStyle = scope.textboxstyle + ";padding-";
                scope.iconStyle = "position:absolute; font-size:20px; line-height:35px; height:40px; width:34px; color:black;";

                if (scope.iconposition == "left") {
                    scope.textStyle = scope.textStyle + "left:40px;text-align:right";
                    scope.iconStyle = scope.iconStyle + "left";
                } else {
                    scope.textStyle = scope.textStyle + "right:40px;text-align:left";
                    scope.iconStyle = scope.iconStyle + "right";
                }

                scope.iconStyle = scope.iconStyle + ":0";

                if (scope.islargetext) {
                    scope.textStyle = scope.textStyle + ";font-size:20px";
                }

                scope.maxLength = 100;
                if (scope.nfocus) {
                    scope.focus = scope.nfocus;
                }
                if (scope.textboxmaxlength) {
                    scope.maxLength = scope.textboxmaxlength;
                }

                if (scope.resultchangedelay) {
                    if (scope.resultchangeminlength) {
                        scope.resultchangeminlength = parseInt(scope.resultchangeminlength);
                    } else {
                        scope.resultchangeminlength = 0;
                    }
                }

                if (!scope.iconposition) {
                    scope.iconposition = "right";
                }
            }

            initialise();

            //#endregion
        },
    }
});

// Control
app.directive('divpage', function ($timeout, $parse) {
    return {
        restrict: 'E',
        scope: {
            pageclick: '&',
            pagemodel: '=',
            totalpage: '@',
            currentpage: '@'
        },
        templateUrl: '/Directives/divPage.html',
        link: function (scope, element, attr) {
            scope.range = new Array(100);

            scope.listPage = [];

            scope.$watch('currentpage', function (newValue, oldValue) {
                rebuildPage(parseInt(newValue), parseInt(scope.totalpage));
            });

            scope.$watch('totalpage', function (newValue, oldValue) {
                rebuildPage(parseInt(scope.currentpage), parseInt(newValue));
            });

            scope.pageOnClick = function (page) {
                scope.pagemodel = page;
                $timeout(scope.pageclick, 50);
            }

            var rebuildPage = function (currentPage, totalPage) {
                var listPage = [];
                var startPage = 1, endPage = totalPage;

                //Hiển thị page 1
                if (currentPage > 3) {
                    startPage = currentPage - 2;
                    var p = {};
                    p.page = 1;
                    p.text = 1;
                    listPage.push(p);
                }

                //Có hiển thị 3 chấm ở đầu hay không?
                if (currentPage > 4) {
                    var p = {};
                    p.page = currentPage - 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //các nút ở giữa
                endPage = Math.min(currentPage + 2, totalPage);
                for (var i = startPage; i <= endPage; i++) {
                    var p = {};
                    p.page = i;
                    p.text = i;
                    listPage.push(p);
                }

                //có hiển thị 3 chấm gần cuối hay không
                if (currentPage + 3 < totalPage) {
                    var p = {};
                    p.page = currentPage + 3;
                    p.text = '...';
                    listPage.push(p);
                }

                //có hiển thị Page cuối hay không?
                if (currentPage + 2 < totalPage) {
                    var p = {};
                    p.page = totalPage;
                    p.text = totalPage;
                    listPage.push(p);
                }

                scope.listPage = listPage;
            }

            var initialise = function () {
            }

            initialise();
        }
    };
});
app.directive('moDateInput', function ($window) {
    return {
        require: '^ngModel',
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            var moment = $window.moment;
            var dateFormat = attrs.moMediumDate;

            attrs.$observe('moDateInput', function (newValue) {
                if (dateFormat == newValue || !ctrl.$modelValue) return;
                dateFormat = newValue;
                ctrl.$modelValue = new Date(ctrl.$setViewValue);
            });

            ctrl.$formatters.unshift(function (modelValue) {
                scope = scope;
                if (!dateFormat || !modelValue) return "";
                var retVal = moment(modelValue).format(dateFormat);
                return retVal;
            });

            ctrl.$parsers.unshift(function (viewValue) {
                scope = scope;
                var date = moment(viewValue, dateFormat);
                return (date && date.isValid() && date.year() > 1950) ? date.toDate() : "";
            });
        }
    };
});