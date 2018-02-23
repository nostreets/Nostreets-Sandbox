(function () {

    angular.module(page.APPNAME)
        .controller("billManagerController", billManagerController)
        .controller("modalInsertController", modalInsertController);

    billManagerController.$inject = ["$scope", "$baseController", '$uibModal', '$sandboxService', '$filter'];
    modalInsertController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService', 'model'];


    function billManagerController($scope, $baseController, $uibModal, $sandboxService, $filter) {

        var vm = this;
        vm.changeCurrentTab = _changeTab;
        //vm.openMainMenuModal = _openMainMenuModal;
        vm.openDatePicker = _openDatePicker;
        vm.updateChart = _getUserCharts;
        vm.openInsertModal = _openInsertModal;
        vm.deleteAsset = _deleteAsset;
        vm.toggleVisiblity = _toggleVisiblity;
        vm.postChart = _postChart;
        vm.dateEnding = _dateEnding;


        $baseController.systemEventService.listen("refreshedUsername", () => { _setUp(); _getUserCharts(); });


        _render();

        function _render() {
            _setUp();
            _getEnums();
            _getUserCharts();
            //_dateZoomEvent();
        }

        function _setUp() {
            vm.scope = $scope;
            vm.charts = [];
            vm.legend = [];
            vm.currentTab = 'income';
            vm.renderedChart = null;
            vm.beginDate = new Date();
            vm.endDate = new Date(new Date().setTime(vm.beginDate.getTime() + 14 * 86400000));
            vm.chartOptions = null;
            vm.chartType = 'line';
            vm.chartLineStyle = 'none';
            vm.enums = null;
            vm.cpApi = null;
            vm.cpOptions = {
                allowEmpty: false,
                format: 'rgb',
                hue: true,
                saturation: true,
                alpha: false,
                swatch: true,
                lightness: true,
                swatchPos: 'left',
                swatchOnly: true,
                round: true,
            };
            vm.cpEventApi =
                {
                    onChange: function (api, color, $event) { },
                    onBlur: function (api, color, $event) { },
                    onOpen: function (api, color, $event) { },
                    onClose: function (api, color, $event) {
                        _postChart(api.getScope().$parent.item);
                        _getUserCharts();
                    },
                    onClear: function (api, color, $event) { },
                    onReset: function (api, color, $event) { },
                    onDestroy: function (api, color) { }
                };
            vm.wheelUpTick = false;
            vm.wheelDownTick = false;
            vm.dateRangeTick = false;
            vm.resetFunc = null;
        }

        function _getUserCharts() {
            _getIncomeChart().then(
                () => _getExpensesChart().then(
                    () => _getCombinedChart().then(
                        () => _getChartLengend().then(
                            () => _targetGraph(vm.currentTab, ((vm.currentTab === "income") ? "#incomeChart" : (vm.currentTab === "expense") ? "#expenseChart" : "#combinedChart"))
                        ))));
        }

        function _dateEnding(date) {
            date = new Date(date);
            return ((date.getDate() === 1) ? 'st' : (date.getDate() === 2) ? 'nd' : 'th');
        }

        function _dateZoomEvent() {
            angular.element(".ct-golden-section").on("DOMMouseScroll mousewheel onmousewheel", (event) => {

                // cross-browser wheel delta
                var event = window.event || event; // old IE support
                var delta = Math.max(-1, Math.min(1, (event.wheelDelta || -event.detail)));


                if (delta > 0) {
                    $scope.$apply(() => {
                        vm.wheelUpTick = true;
                        vm.wheelDownTick = false;
                        vm.dateRangeTick = (vm.dateRangeTick === false) ? true : false;

                    });

                    // for IE
                    event.returnValue = false;
                    // for Chrome and Firefox
                    if (event.preventDefault) {
                        event.preventDefault();
                    }

                }
                else if (delta < 0) {
                    $scope.$apply(function () {
                        vm.wheelDownTick = true;
                        vm.wheelUpTick = false;
                        vm.dateRangeTick = (vm.dateRangeTick === false) ? true : false;


                    });

                    // for IE
                    event.returnValue = false;
                    // for Chrome and Firefox
                    if (event.preventDefault) {
                        event.preventDefault();
                    }

                }

                if (vm.wheelDownTick) {
                    if (vm.dateRangeTick === true) {
                        vm.beginDate.setTime(vm.beginDate.getTime() - (24 * 60 * 60 * 1000)); //24 hrs in nillseconds
                    }
                    else {
                        vm.endDate.setTime(vm.endDate.getTime() + (24 * 60 * 60 * 1000));
                    }
                }
                else {
                    if (vm.dateRangeTick === true) {
                        vm.beginDate.setTime(vm.beginDate.getTime() + (24 * 60 * 60 * 1000));
                    }
                    else {
                        vm.endDate.setTime(vm.endDate.getTime() - (24 * 60 * 60 * 1000));
                    }
                }

            });
        }

        function _openDatePicker(prop) {
            switch (prop) {
                case "start":
                    vm.isStart = true;
                    break;

                case "end":
                    vm.isEnding = true;
                    break;
            }
        }

        function _arrayOfZeros(lengthOfArr) {
            var arr = [];
            for (var i = 0; i < lengthOfArr; i++) {
                arr.push(0);
            }
            return arr;
        }

        function _toggleVisiblity(chart) {
            if (!chart || typeof (chart.isHiddenOnChart) === 'undefined')
                return;
            else {
                if (chart.isHiddenOnChart === false)
                    chart.isHiddenOnChart = true;
                else
                    chart.isHiddenOnChart = false;

                _postChart(chart);
            }
        }

        function _postChart(chart) {
            if (!chart)
                return;
            else {
                var obj = {
                    name: chart.name,
                    cost: chart.cost,
                    paySchedule: chart.paySchedule,
                    timePaid: chart.timePaid,
                    beginDate: (chart.beginDate) ? new Date(chart.beginDate) : null,
                    endDate: (chart.endDate) ? new Date(chart.endDate) : null,
                    isHiddenOnChart: (chart.isHiddenOnChart) ? true : false,
                    costMultilplier: (chart.costMultilplier) ? chart.costMultilplier : 1,
                    style: (chart.style) ? JSON.stringify(chart.style) : JSON.stringify({ color: page.utilities.getRandomColor() }),
                    rate: (vm.rate) ? vm.rate : 2,
                    rateMultilplier: (vm.rateMultilplier) ? vm.rateMultilplier : 1
                }

                switch (vm.currentTab) {
                    case "income":
                        obj.incomeType = parseInt(chart.incomeType);

                        if (chart.id) {
                            obj.id = chart.id;
                            $sandboxService.updateIncome(obj).then(
                                _getUserCharts,
                                err => $baseController.errorCheck(err,
                                    {
                                        maxLoops: 3,
                                        miliseconds: 2000,
                                        method: () => {
                                            $sandboxService.updateIncome(obj);
                                        }
                                    })
                            );
                        }
                        else {
                            $sandboxService.insertIncome(obj).then(
                                _getUserCharts,
                                err => $baseController.errorCheck(err,
                                    {
                                        maxLoops: 3,
                                        miliseconds: 2000,
                                        method: () => {
                                            $sandboxService.insertIncome(obj);
                                        }
                                    })
                            );
                        }
                        break;

                    case "expense":
                        obj.expenseType = parseInt(chart.expenseType);

                        if (vm.id) {
                            obj.id = chart.id
                            $sandboxService.updateExpense(obj).then(
                                _getUserCharts,
                                err => $baseController.errorCheck(err,
                                    {
                                        maxLoops: 3,
                                        miliseconds: 2000,
                                        method: () => {
                                            $sandboxService.updateExpense(obj);
                                        }
                                    })
                            );
                        }
                        else {
                            $sandboxService.insertExpense(obj).then(
                                _getUserCharts,
                                err => $baseController.errorCheck(err,
                                    {
                                        maxLoops: 3,
                                        miliseconds: 2000,
                                        method: () => {
                                            $sandboxService.insertExpense(obj);
                                        }
                                    })
                            );
                        }
                        break;
                }
            }
        }

        function _getDailyDateRange(start, end) {

            if ((!start instanceof Date && !start instanceof Date) || (!typeof (start) === "string" && !typeof (end) === "string")) {
                throw Error("_getDateRange params needs to be a Date object or a valid date string...");
            }

            if (typeof (start) === "string" && typeof (end) === "string") {
                start = new Date(start);
                end = new Date(end);
            }

            var day,
                utcEnd = end.toUTCString(),
                result = [$filter("date")(end, "M/d")];

            while (start < end) {
                day = end.getDate();
                end = new Date(end.setDate(--day));

                formattedDate = $filter("date")(end, "M/d");

                result.unshift(formattedDate);
            }

            vm.beginDate = new Date(start);
            vm.endDate = new Date(utcEnd);
            return result;
        }

        function _getEnums() {
            $sandboxService.getEnums('income,expense,schedule').then(
                (obj) => vm.enums = obj.data.items,
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getEnums('income,expense,schedule').then((obj) => vm.enums = obj.data.items);
                        }
                    })
            );
        }

        function _getIncomeChart() {

            return $sandboxService.getIncomesChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                (data) => {
                    var incomeChart = {
                        key: "income",
                        value: data.data.item
                    };

                    vm.charts.add(incomeChart);
                    vm.incomeCap = data.data.item.series.length;
                },

                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getIncomesChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                                (data) => {
                                    var chart = {
                                        key: "income",
                                        value: data.data.item
                                    };
                                    vm.charts.add(chart);
                                });
                        }
                    })
            );
        }

        function _getExpensesChart() {
            return $sandboxService.getExpensesChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                (data) => {
                    var expensesChart = {
                        key: "expense",
                        value: data.data.item
                    };
                    vm.charts.add(expensesChart);
                },

                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getExpensesChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                                (data) => {
                                    var chart = {
                                        key: "expense",
                                        value: data.data.item
                                    };
                                    vm.charts.add(chart);
                                });
                        }
                    })
            );
        }

        function _getCombinedChart() {
            return $sandboxService.getCombinedChart(vm.beginDate, vm.endDate).then(
                (data) => {
                    var combinedChart = {
                        key: "combined",
                        value: data.data.item
                    };
                    vm.charts.add(combinedChart);
                },

                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getCombinedChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                                (data) => {
                                    var chart = {
                                        key: "combined",
                                        value: data.data.item
                                    };
                                    vm.charts.add(chart);
                                });
                        }
                    })
            );
        }

        function _getIncomes() {

            return $sandboxService.getAllIncomes().then(
                a => {
                    if (a.data.items)
                        for (var i = 0; i < a.data.items.length; i++)
                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                    vm.legend = a.data.items
                    vm.incomeCap = a.data.items.length;
                },
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getAllIncomes().then(
                                a => {
                                    if (a.data.items)
                                        for (var i = 0; i < a.data.items.length; i++)
                                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                    vm.legend = a.data.items
                                    vm.incomeCap = a.data.items.length;
                                });
                        }
                    })
            );
        }

        function _getExpenses() {

            return $sandboxService.getAllExpenses().then(
                a => {
                    if (a.data.items)
                        for (var i = 0; i < a.data.items.length; i++)
                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                    vm.legend = a.data.items
                },
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getAllExpenses().then(a => {
                                if (a.data.items)
                                    for (var i = 0; i < a.data.items.length; i++)
                                        a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                vm.legend = a.data.items
                            });
                        }
                    })
            );
        }

        function _getCombinedAssets() {
            return $sandboxService.getAllIncomes().then(
                a => {
                    if (a.data.items)
                        for (var i = 0; i < a.data.items.length; i++)
                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                    vm.legend = a.data.items
                },
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getAllIncomes()
                                .then(a => {
                                    if (a.data.items)
                                        for (var i = 0; i < a.data.items.length; i++)
                                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                    vm.legend = a.data.items
                                }).then(
                                () => {
                                    $sandboxService.getAllExpenses().then(
                                        a => {
                                            if (a.data.items)
                                                for (var i = 0; i < a.data.items.length; i++)
                                                    a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                            vm.legend.concat(a.data.items)
                                        },
                                        err => $baseController.errorCheck(err,
                                            {
                                                maxLoops: 3,
                                                miliseconds: 2000,
                                                method: () => {
                                                    $sandboxService.getAllExpenses().then(a => {
                                                        if (a.data.items)
                                                            for (var i = 0; i < a.data.items.length; i++)
                                                                a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                                        vm.legend.concat(a.data.items)
                                                    });
                                                }
                                            })
                                    );
                                });
                        }
                    })
            ).then(
                () => {
                    $sandboxService.getAllExpenses().then(
                        a => {
                            for (var i = 0; i < a.data.items.length; i++)
                                a.data.items[i].style = JSON.parse(a.data.items[i].style);

                            vm.legend.concat(a.data.items)
                        },
                        err => $baseController.errorCheck(err,
                            {
                                maxLoops: 3,
                                miliseconds: 2000,
                                method: () => {
                                    $sandboxService.getAllExpenses().then(a => {
                                        for (var i = 0; i < a.data.items.length; i++)
                                            a.data.items[i].style = JSON.parse(a.data.items[i].style);

                                        vm.legend.concat(a.data.items)
                                    });
                                }
                            })
                    );
                });
        }

        function _getIncome(id, name, scheduleType, incomeType) {
            var isSuccessful = false;
            $sandboxService.getIncome(id, name, scheduleType, incomeType).then(
                (data) => console.log(data),
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getIncome(id, name, scheduleType, incomeType).then((data) => { console.log(data); isSuccessful = true; });
                        }
                    })
            );
        }

        function _getExpense(id, name, scheduleType, billType) {
            $sandboxService.getExpense(id, name, scheduleType, billType).then(
                (data) => console.log(data),
                err => $baseController.errorCheck(err,
                    {
                        maxLoops: 3,
                        miliseconds: 2000,
                        method: () => {
                            $sandboxService.getExpense(id, name, scheduleType, billType).then((data) => { console.log(data); isSuccessful = true; });
                        }
                    })
            );
        }

        function _deleteAsset(obj) {
            var hasError = false;
            if (obj.incomeType) {
                $sandboxService.deleteIncome(obj.id).then(
                    (data) => console.log(data),
                    err => $baseController.errorCheck(err,
                        {
                            maxLoops: 3,
                            miliseconds: 2000,
                            method: () => {
                                $sandboxService.deleteIncome(obj.id).then((data) => console.log(data));
                            }
                        })
                );
            }
            else {
                $sandboxService.deleteExpense(obj.id).then(
                    (data) => console.log(data),
                    err => $baseController.errorCheck(err,
                        {
                            maxLoops: 3,
                            miliseconds: 2000,
                            method: () => {
                                $sandboxService.deleteExpense(obj.id).then((data) => console.log(data));
                            }
                        })
                );
            }

        }

        function _targetGraph(type, elementId) {

            var chart = vm.charts.filter((a) => a.key === type)[0].value;
            var options = _getChartOptions(chart);

            if (chart.series.length === 0)
                chart.series.push(_arrayOfZeros(chart.labels.length));


            _adjustTimeLabels(chart);
            _addTooltipDetials(chart);

            var renderedChart = new Chartist.Line(elementId, chart, options);

            _animateGraph(renderedChart, 200);

            vm.renderedChart = chart;
            vm.chartOptions = options;


        }

        function _getChartLengend() {

            //return (vm.currentTab == 'income') ? _getIncomes() : (vm.currentTab == 'expense') ? _getExpenses() : _getCombinedAssets()
            _getEnums();
            var arr = [];
            var getAssets = () => { return (vm.currentTab == 'income') ? _getIncomes() : (vm.currentTab == 'expense') ? _getExpenses() : _getCombinedAssets(); };

            return getAssets().then(
                () => {
                    var hiddenLines = 0;
                    for (var i = 0; i < vm.legend.length; i++) {
                        var backupColor = page.utilities.getRandomColor(),
                            lineChar = String.fromCharCode(97 + (i - hiddenLines));

                        if (vm.legend[i].isHiddenOnChart) {
                            hiddenLines++;
                            continue;
                        }

                        if (!vm.legend[i].style)
                            vm.legend[i].style = { color: backupColor };

                        arr.push('.ct-series-' + lineChar + ' .ct-point, .ct-series-' + lineChar + ' .ct-line, .ct-series-' + lineChar + ' .ct-bar, .ct-series-' + lineChar + '{ stroke: ' + vm.legend[i].style.color + ' !important }');
                    }
                    page.utilities.writeStyles("_lineStyles", arr);
                    vm.legend.reverse();
                });
        }

        function _chartStyles(index) {
            var result,
                char = String.fromCharCode(97 + index);
            switch (char) {
                case 'a':
                    result = { color: '#00bcd4' };
                    break;

                case 'b':
                    result = { color: '#f44336' };
                    break;

                case 'c':
                    result = { color: '#ff9800' };
                    break;

                case 'd':
                    result = { color: '#9c27b0' };
                    break;

                case 'e':
                    result = { color: '#4caf50' };
                    break;

                case 'f':
                    result = { color: '#9C9B99' };
                    break;

                case 'g':
                    result = '{color: #999999}';
                    break;

                case 'h':
                    result = '{color: #dd4b39}';
                    break;

                case 'i':
                    result = '{color: #35465c}';
                    break;

                case 'j':
                    result = '{color: #e52d27}';
                    break;

                case 'k':
                    result = '{color: #55acee}';
                    break;

                case 'l':
                    result = '{color: #cc2127}';
                    break;

                case 'm':
                    result = '{color: #1769ff}';
                    break;

                case 'n':
                    result = '{color: #6188e2}';
                    break;

                case 'o':
                    result = '{color: #a748ca}';
                    break;
            }
            return result;
        }

        function _getChartOptions(chart) {

            var lineSmooth = false;

            var onZoom = (chart, reset) => {
                vm.resetFunc = reset;
            }
            switch (vm.chartType) {
                case 'line':
                    switch (vm.chartLineStyle) {
                        case 'none':
                            lineSmooth = false;
                            break;

                        case 'smooth':
                            lineSmooth = true;
                            break;

                        case 'step':
                            lineSmooth = Chartist.Interpolation.step();
                            break;
                    }
                    break;
            }

            var options = {
                lineSmooth: lineSmooth,
                width: Math.round(chart.labels.length / 1.5) + "00px",
                axisY: {
                    //type: Chartist.AutoScaleAxis
                },
                axisX: {
                    //type: Chartist.AutoScaleAxis,
                    labelOffset: {
                        x: 0,
                        y: 0
                    }
                },
                plugins: [
                    chartistScroll({ height: "8px" }),
                    Chartist.plugins.tooltip()
                    //Chartist.plugins.zoom({ onZoom: onZoom })
                ]

            };

            return options;
        }

        function _addTooltipDetials(chart) {
            for (var a = 0; a < chart.series.length; a++) {
                var chartObj = vm.legend[a];
                for (var b = 0; b < chart.series[a].length; b++) {
                    var label = chart.labels[b],
                        value = chart.series[a][b],
                        lastValue = (b === 0) ? 0 : chart.series[a][b - 1].value;
                    var meta = '<div> Date ' + label + ' </div>' + '<div> ' + ((chartObj.incomeType) ? '+ ' : '- ') + (value - lastValue) + ' </div>';
                    chart.series[a][b] = {
                        meta: meta,
                        value: value
                    }
                }
            }
        }

        function _adjustTimeLabels(chart) {

            if (angular.element("#assetChartTabContainer")[0].clientWidth < 1150) {

                switch (_findScheduleType(chart.labels)) {

                    case "Daily":
                        var minimizedLabels = _getDailyDateRange(vm.beginDate, vm.endDate);

                        if (chart.labels.length === minimizedLabels.length) {
                            chart.labels = _getDailyDateRange(vm.beginDate, vm.endDate);
                        }
                        break;
                }
            }
        }

        function _findScheduleType(arr) {
            var item = arr[0];
            if (!item)
                return;

            if (
                item === "January" ||
                item === "February" ||
                item === "March" ||
                item === "April" ||
                item === "May" ||
                item === "June" ||
                item === "July" ||
                item === "August" ||
                item === "September" ||
                item === "October" ||
                item === "November" ||
                item === "December") {

                return "Monthly";

            }
            else if (item.substr(item.length - 3, 3) === "day") {

                return "Daily";

            }
            else if (item.includes("AM") || item.includes("PM")) {

                return "Hourly";

            }
            else if (item.includes("Year")) {

                return "Yearly";

            }
            else {
                return "Weekly";
            }

        }

        function _animateGraph(chart, time) {
            // sequence number aside so we can use it in the event callbacks
            var seq = 0,
                delays = 80,
                durations = time || 400;

            // Once the chart is fully created we reset the sequence
            chart.on('created', function () {
                seq = 0;
                if (window.__anim21278907124) {
                    clearTimeout(window.__anim21278907124);
                    window.__anim21278907124 = null;
                }
                window.__anim21278907124 = setTimeout(chart.update.bind(chart), 1800000);
            });

            // On each drawn element by Chartist we use the Chartist.Svg API to trigger SMIL animations
            chart.on('draw', function (data) {
                seq++;

                if (data.type === 'line') {
                    // If the drawn element is a line we do a simple opacity fade in. This could also be achieved using CSS3 animations.
                    data.element.animate({
                        opacity: {
                            // The delay when we like to start the animation
                            begin: seq * delays + 1000,
                            // Duration of the animation
                            dur: durations,
                            // The value where the animation should start
                            from: 0,
                            // The value where it should end
                            to: 1
                        }
                    });
                }
                else if (data.type === 'bar') {
                    data.element.animate({
                        y2: {
                            dur: 1000,
                            from: data.y1,
                            to: data.y2,
                            easing: Chartist.Svg.Easing.easeOutQuint
                        },
                        opacity: {
                            dur: 1000,
                            from: 0,
                            to: 1,
                            easing: Chartist.Svg.Easing.easeOutQuint
                        }
                    });
                }
                else if (data.type === 'slice') {
                    // Get the total path length in order to use for dash array animation
                    var pathLength = data.element._node.getTotalLength();

                    // Set a dasharray that matches the path length as prerequisite to animate dashoffset
                    data.element.attr({
                        'stroke-dasharray': pathLength + 'px ' + pathLength + 'px'
                    });

                    // Create animation definition while also assigning an ID to the animation for later sync usage
                    var animationDefinition = {
                        'stroke-dashoffset': {
                            id: 'anim' + data.index,
                            dur: 1000,
                            from: -pathLength + 'px',
                            to: '0px',
                            easing: Chartist.Svg.Easing.easeOutQuint,
                            // We need to use `fill: 'freeze'` otherwise our animation will fall back to initial (not visible)
                            fill: 'freeze'
                        }
                    };

                    // If this was not the first slice, we need to time the animation so that it uses the end sync event of the previous animation
                    if (data.index !== 0) {
                        animationDefinition['stroke-dashoffset'].begin = 'anim' + (data.index - 1) + '.end';
                    }

                    // We need to set an initial value before the animation starts as we are not in guided mode which would do that for us
                    data.element.attr({
                        'stroke-dashoffset': -pathLength + 'px'
                    });

                    // We can't use guided mode as the animations need to rely on setting begin manually
                    data.element.animate(animationDefinition, false);
                }

                else if (data.type === 'label' && data.axis === 'x') {
                    data.element.animate({
                        y: {
                            begin: seq * delays,
                            dur: durations,
                            from: data.y + 100,
                            to: data.y,
                            // We can specify an easing function from Chartist.Svg.Easing
                            easing: 'easeOutQuart'
                        }
                    });
                }
                else if (data.type === 'label' && data.axis === 'y') {
                    data.element.animate({
                        x: {
                            begin: seq * delays,
                            dur: durations,
                            from: data.x - 100,
                            to: data.x,
                            easing: 'easeOutQuart'
                        }
                    });
                }
                else if (data.type === 'point') {
                    data.element.animate({
                        x1: {
                            begin: seq * delays,
                            dur: durations,
                            from: data.x - 10,
                            to: data.x,
                            easing: 'easeOutQuart'
                        },
                        x2: {
                            begin: seq * delays,
                            dur: durations,
                            from: data.x - 10,
                            to: data.x,
                            easing: 'easeOutQuart'
                        },
                        opacity: {
                            begin: seq * delays,
                            dur: durations,
                            from: 0,
                            to: 1,
                            easing: 'easeOutQuart'
                        }
                    });
                }
                else if (data.type === 'grid') {
                    // Using data.axis we get x or y which we can use to construct our animation definition objects
                    var pos1Animation = {
                        begin: seq * delays,
                        dur: durations,
                        from: data[data.axis.units.pos + '1'] - 30,
                        to: data[data.axis.units.pos + '1'],
                        easing: 'easeOutQuart'
                    };

                    var pos2Animation = {
                        begin: seq * delays,
                        dur: durations,
                        from: data[data.axis.units.pos + '2'] - 100,
                        to: data[data.axis.units.pos + '2'],
                        easing: 'easeOutQuart'
                    };

                    var animations = {};
                    animations[data.axis.units.pos + '1'] = pos1Animation;
                    animations[data.axis.units.pos + '2'] = pos2Animation;
                    animations['opacity'] = {
                        begin: seq * delays,
                        dur: durations,
                        from: 0,
                        to: 1,
                        easing: 'easeOutQuart'
                    };

                    data.element.animate(animations);
                }
            });

        }

        function _changeTab(tab) {
            if (tab) {
                vm.currentTab = tab;
            }

            if (vm.currentTab) {

                $('a[data-toggle="tab"]').on('shown.bs.tab',
                    function (e) {
                        //e.target // newly activated tab
                        //e.relatedTarget // previous active tab

                        _getChartLengend().then(
                            () => _targetGraph(vm.currentTab, ((vm.currentTab === "income") ? "#incomeChart" : (vm.currentTab === "expense") ? "#expenseChart" : "#combinedChart"))
                        );
                    });
            }
        }

        function _openInsertModal(data) {

            data = (data) ? data : {};

            var obj = {
                type: vm.currentTab,
                id: (data.id) ? data.id : 0,
                name: (data.name) ? data.name : null,
                cost: (data.cost) ? data.cost : null,
                paySchedule: (data.paySchedule) ? data.paySchedule : null,
                timePaid: (data.timePaid) ? new Date(data.timePaid) : null,
                beginDate: (data.beginDate) ? new Date(data.beginDate) : null,
                endDate: (data.endDate) ? new Date(data.endDate) : null,
                isHiddenOnChart: (data.isHiddenOnChart === true) ? true : false,
                costMultilplier: (data.costMultilplier) ? data.costMultilplier : 1,
                style: (data.style) ? data.style : null,
                rate: (data.rate) ? data.rate : 2,
                rateMultilplier: (data.rateMultilplier) ? data.rateMultilplier : 1
            };

            if (vm.type === "combined") { obj.type = (data.incomeType) ? "income" : "expense"; }

            var modalInstance = $uibModal.open({
                animation: true
                , templateUrl: "modalExpenseBuilder.html"
                , controller: "modalInsertController as mc"
                , size: "lg"
                , resolve: {
                    model: function () {
                        return obj;
                    }
                }
            });

            modalInstance.closed.then(_getUserCharts);
        }

        function _openCodeModal(code) {
            var modalInstance = $uibModal.open({
                animation: true
                , templateUrl: "codeModal.html"
                , controller: "modalCodeController as mc"
                , size: "lg"
                , resolve: {
                    code: function () {
                        return code;
                    }
                }
            });
        }

        //Change When Controller is Complete
        function _getCode() {
            $baseController.http({
                url: "api/view/code/dymanicGraphsDirective",
                method: "GET",
                responseType: "JSON"
            }).then(function (data) {
                _openCodeModal(data.data.item);
            });
        }

    }

    function modalInsertController($scope, $baseController, $uibModalInstance, $sandboxService, model) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.submit = _submit;
        vm.reset = _setUp;
        vm.cancel = _cancel;
        vm.openDatePicker = _openDatePicker;
        vm.toggleVisiblity = _toggleVisiblity;

        _render();

        function _render() {
            _setUp(model);
        }

        function _setUp(data) {

            if (!data) { data = {}; }

            vm.type = model.type;
            vm.id = data.id || 0;
            vm.name = data.name || null;
            vm.cost = data.cost || 0;
            vm.paySchedule = (data.paySchedule) ? data.paySchedule.toString() : "1";
            vm.timePaid = data.timePaid || null;
            vm.beginDate = data.beginDate || null;
            vm.endDate = data.endDate || null;
            vm.isHiddenOnChart = data.isHiddenOnChart || false;
            vm.costMultilplier = data.costMultilplier || 1;
            vm.style = data.style || { color: page.utilities.getRandomColor() };
            vm.cpOptions = {
                allowEmpty: false,
                format: 'rgb',
                hue: true,
                saturation: true,
                alpha: false,
                swatch: true,
                lightness: true,
                swatchPos: 'left',
                swatchOnly: true,
                round: true,
            };
            vm.isTimePaidOpen = false;
            vm.isBeginDateOpen = false;
            vm.isEndDateOpen = false;
            vm.rate = data.rate + '' || '2';
            vm.rateMultilplier = data.rateMultilplier || 1;

            switch (model.type) {
                case "income":
                    vm.incomeType = data.incomeType || "1";
                    break;

                case "expense":
                    vm.expenseType = data.expenseType || "1";
                    break;
            }

        }

        function _openDatePicker(prop) {
            switch (prop) {
                case "timePaid":
                    vm.isTimePaidOpen = true;
                    break;

                case "beginDate":
                    vm.isBeginDateOpen = true;
                    break;

                case "endDate":
                    vm.isEndDateOpen = true;
                    break;
            }
        }

        function _submit() {

            var hasError = false,
                obj = {
                    name: vm.name,
                    cost: vm.cost,
                    paySchedule: vm.paySchedule,
                    timePaid: vm.timePaid,
                    beginDate: (vm.beginDate) ? new Date(vm.beginDate) : null,
                    endDate: (vm.endDate) ? new Date(vm.endDate) : null,
                    isHiddenOnChart: (vm.isHiddenOnChart) ? true : false,
                    style: (vm.style) ? JSON.stringify(vm.style) : JSON.stringify({ color: page.utilities.getRandomColor() }),
                    rate: (vm.rate) ? vm.rate : 2,
                    rateMultilplier: (vm.rateMultilplier) ? vm.rateMultilplier : 1
                };


            switch (vm.type) {
                case "income":
                    obj.incomeType = parseInt(vm.incomeType);
                    if (vm.id) {
                        obj.id = vm.id;
                        $sandboxService.updateIncome(obj).then(

                            () => vm.$uibModalInstance.close(),

                            err => $baseController.errorCheck(err,
                                {
                                    maxLoops: 3,
                                    miliseconds: 2000,
                                    method: () => {
                                        $sandboxService.updateIncome(obj).then(() => vm.$uibModalInstance.close());
                                    }
                                })
                        );
                    }
                    else {
                        $sandboxService.insertIncome(obj).then(
                            () => vm.$uibModalInstance.close(),
                            err => $baseController.errorCheck(err,
                                {
                                    maxLoops: 3,
                                    miliseconds: 2000,
                                    method: () => {
                                        $sandboxService.insertIncome(obj).then(() => vm.$uibModalInstance.close());
                                    }
                                })
                        );
                    }
                    break;

                case "expense":
                    obj.expenseType = parseInt(vm.expenseType);

                    if (vm.id) {
                        obj.id = vm.id
                        $sandboxService.updateExpense(obj).then(
                            () => vm.$uibModalInstance.close(),
                            err => $baseController.errorCheck(err,
                                {
                                    maxLoops: 3,
                                    miliseconds: 2000,
                                    method: () => {
                                        $sandboxService.updateExpense(obj).then(() => vm.$uibModalInstance.close());
                                    }
                                })
                        );
                    }
                    else {
                        $sandboxService.insertExpense(obj).then(
                            () => vm.$uibModalInstance.close(),
                            err => $baseController.errorCheck(err,
                                {
                                    maxLoops: 3,
                                    miliseconds: 2000,
                                    method: () => {
                                        $sandboxService.insertExpense(obj).then(() => vm.$uibModalInstance.close());
                                    }
                                })
                        );
                    }
                    break;
            }
        }

        function _toggleVisiblity() {
            if (vm.isHiddenOnChart === false)
                vm.isHiddenOnChart = true;
            else
                vm.isHiddenOnChart = false;
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }


    }

})();