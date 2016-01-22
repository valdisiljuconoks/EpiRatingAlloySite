require(['dojo'], function (dojo) {

    var ajaxOverlay = {
        element: dojo.byId('ajaxOverlay'),
        show: function () {
            dojo.style(this.element, 'display', 'block');
            dojo.fadeIn({ node: this.element }).play();
        },
        hide: function () {
            dojo.fadeOut({ node: this.element }).play();
            dojo.style(this.element, 'display', 'none');
        }
    };

    // Constructor
    function CacheManager() {
        this.system = {
            machineName: dojo.byId('cm-machine-name'),
            systemMemory: {
                element: dojo.query('.memory-usage-bar', 'cm-system-memory')[0],
                label: dojo.query('.memory-usage-text', 'cm-system-memory')[0],
                valueBar: dojo.query('.memory-usage-bar-inner', 'cm-system-memory')[0]
            },
            appPath: dojo.byId('cm-app-path'),
            appCacheCount: dojo.byId('cm-app-cache-count')
        };
        this.cache = {
            selectAll: dojo.query('thead .select-all', 'tblCacheList')[0],
            cacheList: dojo.query('tbody', 'tblCacheList')[0],
            filterInput: dojo.byId('tblFilter')
        };
        this.pager = {
            nextPageBtn: dojo.query('.next-page', 'cacheListPager')[0],
            prevPageBtn: dojo.query('.prev-page', 'cacheListPager')[0],
            firstPageBtn: dojo.query('.first-page', 'cacheListPager')[0],
            lastPageBtn: dojo.query('.last-page', 'cacheListPager')[0],
            pageNumber: dojo.query('.page-number', 'cacheListPager')[0],
            totalPages: dojo.query('.total-pages', 'cacheListPager')[0],
            pageSize: dojo.query('.page-size', 'cacheListPager')[0]
        };
        this.ajaxData = {
            sortBy: 'Size',
            ascending: false,
            pageNumber: 1,
            pageSize: 10
        };
        this.init();
    }

    // Methods
    CacheManager.prototype = {
        init: function () {
            var self = this;

            // indexOf implementation for < IE9
            if (!Array.prototype.indexOf) {
                Array.prototype.indexOf = function (elt /*, from*/) {
                    var len = this.length >>> 0;
                    var from = Number(arguments[1]) || 0;
                    from = (from < 0) ? Math.ceil(from) : Math.floor(from);
                    if (from < 0) from += len;

                    for (; from < len; from++) {
                        if (from in this && this[from] === elt) return from;
                    }
                    return -1;
                };
            }

            // Set up event handlers
            dojo.connect(dojo.byId('btnRefresh'), 'onclick', function () {
                self.ajaxData.pageNumber = 1;
                self.update();
            });

            dojo.connect(dojo.byId('btnClearCache'), 'onclick', function () {
                self.deleteAllCache();
            });

            dojo.connect(self.cache.selectAll, 'onclick', function () {
                var checkboxes = dojo.query('tbody input[type="checkbox"]', 'tblCacheList');
                for (var i = 0, i_len = checkboxes.length; i < i_len; i++) {
                    checkboxes[i].checked = this.checked;
                }
            });

            dojo.connect(dojo.byId('btnDeleteSelected'), 'onclick', function () {
                self.deleteSelectedCache();
            });

            var t;
            dojo.connect(self.cache.filterInput, 'keypress', function () {
                clearTimeout(t);
                t = setTimeout(function () {
                    self.filterTable(self.cache.filterInput.value);
                }, 300);
            });

            var sortKeys = dojo.query('thead .sortKey', 'tblCacheList');
            sortKeys.connect('onclick', function () {
                var sortKey = dojo.attr(this, 'data-sortKey');
                if (!sortKey || sortKey.length == 0) {
                    return;
                }

                var isActive = dojo.hasClass(this, 'active');
                if (isActive) {
                    if (dojo.hasClass(this, 'asc')) {
                        dojo.removeClass(this, 'asc');
                    } else {
                        dojo.addClass(this, 'asc');
                    }
                } else {
                    dojo.forEach(sortKeys, function (th) {
                        dojo.removeClass(th, ['active', 'asc']);
                    });
                    dojo.addClass(this, 'active');
                }

                self.ajaxData.sortBy = sortKey;
                self.ajaxData.ascending = dojo.hasClass(this, 'asc');

                self.updateCacheList();
            });

            var pager = dojo.query('input[type="button"]', 'cacheListPager');
            pager.connect('onclick', function (e) {
                dojo.stopEvent(e);

                var pageNumber = parseInt(self.pager.pageNumber.value, 10),
                    totalPages = parseInt(self.pager.totalPages.innerHTML, 10);

                if (dojo.hasClass(this, 'prev-page')) {
                    if (parseInt(self.pager.pageNumber.value, 10) <= 1) {
                        return;
                    }
                    self.ajaxData.pageNumber = pageNumber - 1;
                } else if (dojo.hasClass(this, 'next-page')) {
                    if (self.ajaxData.pageNumber >= totalPages) {
                        return;
                    }
                    self.ajaxData.pageNumber = pageNumber + 1;
                } else if (dojo.hasClass(this, 'first-page')) {
                    self.ajaxData.pageNumber = 1;
                } else if (dojo.hasClass(this, 'last-page')) {
                    self.ajaxData.pageNumber = totalPages;
                }

                self.updateCacheList();
            });

            dojo.connect(self.pager.pageNumber, 'keypress', function (e) {
                if (e.keyCode != dojo.keys.ENTER) {
                    return;
                }

                var pageNumber = parseInt(self.pager.pageNumber.value, 10),
                    totalPages = parseInt(self.pager.totalPages.innerHTML, 10);

                if (pageNumber < 1) {
                    self.ajaxData.pageNumber = 1;
                } else if (pageNumber > totalPages) {
                    self.ajaxData.pageNumber = totalPages;
                } else {
                    self.ajaxData.pageNumber = pageNumber;
                }

                self.updateCacheList();
            });

            dojo.connect(self.pager.pageSize, 'change', function (e) {
                self.ajaxData.pageSize = parseInt(self.pager.pageSize.value, 10);
                self.ajaxData.pageNumber = 1;
                self.updateCacheList();
            });

            ajaxOverlay.hide();
        },

        update: function () {
            this.updateSystemStats();
            this.updateCacheList();
        },

        updateSystemStats: function () {
            var self = this;

            ajaxOverlay.show();

            dojo.xhrGet({
                url: 'SystemStats/Get',
                handleAs: 'json',
                load: function (response) {
                    if (response && response.status) {
                        if (response.status == 'success') {
                            var data = response.data;

                            self.system.machineName.innerHTML = data.MachineName;
                            self.system.appPath.innerHTML = data.ApplicationPath;
                            self.system.appCacheCount.innerHTML = data.AppCacheCount;

                            self.system.systemMemory.label.innerHTML = data.SystemMemory.UsedFormatted + ' of ' + data.SystemMemory.TotalFormatted;
                            dojo.style(self.system.systemMemory.valueBar, 'width', data.SystemMemory.UsedPercentage + '%');
                            dojo.removeClass(self.system.systemMemory.element, ['critical', 'warning']);
                            if (data.SystemMemory.UsedPercentage > 90) {
                                dojo.addClass(self.system.systemMemory.element, 'critical');
                            } else if (data.SystemMemory.UsedPercentage > 75) {
                                dojo.addClass(self.system.systemMemory.element, 'warning');
                            }
                        }
                    } else {
                        self.errorHandler(true, response);
                    }
                },
                error: function (response, ioArgs) {
                    self.errorHandler(true, response, ioArgs);
                },
                handle: function () {
                    ajaxOverlay.hide();
                }
            });
        },

        updateCacheList: function () {
            var self = this;

            ajaxOverlay.show();

            self.cache.selectAll.checked = false;

            dojo.xhrGet({
                url: 'Cache/Get',
                handleAs: 'json',
                content: self.ajaxData,
                load: function (response) {
                    if (response && response.status) {
                        if (response.status == 'success') {
                            var cacheListData = response.data.CacheList,
                                result = '';

                            for (var i = 0, i_len = cacheListData.length; i < i_len; i++) {
                                var item = cacheListData[i];
                                result += '<tr>\
                                    <td><input type="checkbox" value="' + item.Key + '"/></td>\
                                    <td>' + item.Key + '</td>\
                                    <td>' + item.Value + '</td>\
                                    <td>' + item.EntryType + '</td>\
                                    <td>' + item.SizeFormatted + '</td>\
                                    <td>' + item.CreatedFormatted + '</td>\
                                    <td>' + item.ExpiresFormatted + '</td>\
                                    </tr>';
                            }

                            self.cache.cacheList.innerHTML = result;
                            self.cache.filterInput.value = '';
                            self.updatePager(response.data.PageNumber, response.data.TotalPages);
                        } else {
                            self.cache.cacheList.innerHTML = '<tr><td colspan="7">Error</td></tr>';
                        }
                    } else {
                        self.errorHandler(true, response);
                    }
                },
                error: function (response, ioArgs) {
                    self.errorHandler(true, response, ioArgs);
                },
                handle: function () {
                    ajaxOverlay.hide();
                }
            });
        },

        updatePager: function (pageNumber, totalPages) {
            this.pager.pageNumber.value = pageNumber;
            this.pager.totalPages.innerHTML = totalPages;
        },

        filterTable: function (filterTerm) {
            var rows = dojo.query('tbody tr', 'tblCacheList');

            for (var i = 0, i_len = rows.length; i < i_len; i++) {
                var current = rows[i],
                    text = current.innerText || current.textContent;

                if (text.toLowerCase().indexOf(filterTerm) > -1 || filterTerm.length == 0) {
                    current.style.display = '';
                } else {
                    current.style.display = 'none';
                }
            }
        },

        deleteAllCache: function () {
            var self = this;

            dojo.xhrGet({
                url: 'Cache/Clear',
                handleAs: 'json',
                load: function (response) {
                    if (response && response.status) {
                        if (response.status == 'success') {
                            self.ajaxData.pageNumber = 1;
                            self.update();
                        }
                    } else {
                        self.errorHandler(true, response);
                    }
                },
                error: function (response, ioArgs) {
                    self.errorHandler(true, response, ioArgs);
                }
            });
        },

        deleteSelectedCache: function () {
            var self = this;

            var checked = dojo.query('tbody input[type="checkbox"]:checked', 'tblCacheList');
            if (checked.length == 0) {
                return;
            }

            var items = [];
            for (var i = 0, i_len = checked.length; i < i_len; i++) {
                items.push(checked[i].value);
            }

            if (items.length == 0) {
                return;
            }

            dojo.xhrPost({
                url: 'Cache/DeleteSelected',
                postData: dojo.toJson({
                    list: items
                }),
                handleAs: 'json',
                headers: { 'Content-Type': 'application/json' },
                load: function (response) {
                    if (response && response.status) {
                        if (response.status == 'success') {
                            self.ajaxData.pageNumber = 1;
                            self.update();
                        }
                    } else {
                        self.errorHandler(true, response);
                    }
                },
                error: function (response, ioArgs) {
                    self.errorHandler(true, response, ioArgs);
                }
            });
        },

        errorHandler: function (reload, response, ioArgs) {
            alert('An error has occurred. If this problem persists, please contact your system administrator.');
            if (reload) {
                location.reload(true);
            }
        }
    };

    dojo.ready(function () {
        return new CacheManager();
    });
});