(function () {

    angular.module(page.APPNAME)
        .controller("billManagerController", billManagerController)
        .controller("modalMainMenuController", modalMainMenuController)
        .controller("modalInsertController", modalInsertController);

    billManagerController.$inject = ["$scope", "$baseController", '$uibModal', '$sandboxService', '$filter'];
    modalMainMenuController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService', '$uibModal', 'data'];
    modalInsertController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService', 'model'];


    function billManagerController($scope, $baseController, $uibModal, $sandboxService, $filter) {

        var vm = this;
        vm.changeCurrentTab = _changeTab;
        vm.openMainMenuModal = _openMainMenuModal;
        vm.openDatePicker = _openDatePicker;
        vm.updateChart = _getUserData;


        $baseController.systemEventService.listen("refreshedUsername", () => { _setUp(); _getUserData(); });


        _render();

        function _render() {
            _setUp();
            _getUserData();
        }

        function _setUp() {
            vm.income = [];
            vm.expenses = [];
            vm.charts = [];
            vm.currentTab = 'income';
            vm.renderedChart = null;
            vm.beginDate = new Date();
            vm.endDate = new Date(new Date().setTime(vm.beginDate.getTime() + 14 * 86400000));
            vm.chartOptions = null;
        }

        function _getUserData() {
            _getIncomeChart().then(
                () => _getExpensesChart().then(
                    () => _getCombinedChart().then(
                        () => _changeTab()
                    )));
        }

        function _openDatePicker(prop) {
            switch (prop) {
                case "start":
                    vm.isStart = true;
                    break;

                case "end":
                    vm.isEndding = true;
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

        function _getDailyDateRange(start, end) {

            if ((!start instanceof Date && !start instanceof Date) || (!typeof (start) === "string" && !typeof (end) === "string")) {
                throw Error("_getDateRange params needs to be a Date object or a valid date string...");
            }

            if (typeof (start) === "string" && typeof (end) === "string") {
                start = new Date(start);
                end = new Date(end);
            }

            var day;
            //var end = new Date();
            var result = [$filter("date")(end, "M/d")];

            while (start < end) {
                day = end.getDate();
                end = new Date(end.setDate(--day));

                formattedDate = $filter("date")(end, "M/d");

                result.unshift(formattedDate);
            }

            return result;
        }

        function _getIncomeChart() {

            return $sandboxService.getIncomesChart(vm.beginDate.toUTCString(), vm.endDate.toUTCString()).then(
                (data) => {
                    var incomeChart = {
                        key: "income",
                        value: data.data.item
                    };
                    vm.charts.add(incomeChart);
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

        function _targetGraph(type, elementId) {

            var chart = vm.charts.filter((a) => a.key === type)[0].value;
            var options = {
                axisX: {
                    labelOffset: {
                        x: 0,
                        y: 0
                    }
                },
                plugins: [
                    chatistScroll()
                ]

            };

            _adjustChartLabels(chart, options);

            if (chart.series.length === 0) {
                chart.series.push(_arrayOfZeros(chart.labels.length));
            }

            if (vm.renderedChart === null) {
                var renderedChart = new Chartist.Line(elementId, chart, options);
                _animateGraph(renderedChart);
            }
            else {
                $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                    //e.target // newly activated tab
                    //e.relatedTarget // previous active tab
                    var renderedChart = new Chartist.Line(elementId, chart, options);
                    _animateGraph(renderedChart);
                });
            }

            page.__chart = renderedChart;
            page.__chartStyle = document.getElementById(page.__chart.container.id).style;

            vm.renderedChart = chart;
            vm.chartOptions = options;

        }

        function _adjustChartLabels(chart, options) {

            //options.width = Math.round(chart.labels.length / 1.5) + "00px"

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
                window.__anim21278907124 = setTimeout(chart.update.bind(chart), 60000);
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
                _targetGraph(vm.currentTab, ((vm.currentTab === "income") ? "#incomeChart" : (vm.currentTab === "expense") ? "#expenseChart" : "#combinedChart"));
            }
        }

        function _openMainMenuModal(typeId) {

            var data = {
                type: typeId
            };

            //switch (typeId) {
            //    case "income":
            //        data.items = vm.income;
            //        break;

            //    case "expense":
            //        data.items = vm.expenses;
            //        break;

            //    case "combined":
            //        var newArr = vm.expenses.concat(vm.income);
            //        data.items = newArr;
            //}

            var modalInstance = $uibModal.open({
                animation: true
                , templateUrl: "modelBillMainMenu.html"
                , controller: "modalMainMenuController as mm"
                , size: "lg"
                , resolve: {
                    data: function () {
                        return data;
                    }
                }
            });
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

        //REMOVE WHEN FUNCTIONAL
        function chatistScroll() {

            return _chartistScoll;

            function _chartistScoll(chart) {

                var options = {
                    width: Math.round(chart.data.labels.length / 1) + "00px"
                };

                options = Chartist.extend({}, options, chart.options);

                var styleTag = document.createElement('style');

                var cssRules = [
                    '#' + chart.container.id + ' { overflow-x: scroll; overflow-y: hidden; }',
                    '#' + chart.container.id + '::-webkit-scrollbar  { width: 0px; }',
                    '#' + chart.container.id + '::-webkit-scrollbar * { background: transparent; }',
                    '#' + chart.container.id + '::-webkit-scrollbar-thumb { background: rgba(0, 0, 0, 0.4) !important; }',
                    '#' + chart.container.id + '::-webkit-scrollbar-track {  -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3); border-radius: 10px; background-color: #F5F5F5; }',
                    '#' + chart.container.id + '::-webkit-scrollbar {  width: 12px; background-color: #F5F5F5; }',
                    '#' + chart.container.id + '::-webkit-scrollbar-thumb {  border-radius: 14px; -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3); background-color: #555; }'
                ];

                for (var css of cssRules) {
                    styleTag.appendChild(document.createTextNode(css));
                }

                chart.container.appendChild(styleTag);

            }

        };
    }

    function modalMainMenuController($scope, $baseController, $uibModalInstance, $sandboxService, $uibModal, data) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.openInsertModal = _openInsertModal;
        vm.deleteAsset = _deleteAsset;

        _render();

        function _render() {
            _setUp(data);
            _refreshData();
            _getBillingEnums([vm.type, "schedule"]);
        }

        function _setUp(data) {
            vm.type = data.type;
            vm.items = [];
            vm.enums = [];
        }

        function _getBillingEnums(billTypes) {
            $sandboxService.getEnums(billTypes).then(
                (obj) => vm.enums = obj.data.items,
                (err) => console.log(err)
            );
        }

        function _openInsertModal(data) {

            data = (data) ? data : {};

            var obj = {
                type: vm.type,
                id: (data.id) ? data.id : 0,
                name: (data.name) ? data.name : null,
                cost: (data.cost) ? data.cost : null,
                paySchedule: (data.paySchedule) ? data.paySchedule : null,
                timePaid: (data.timePaid) ? new Date(data.timePaid) : null,
                beginDate: (data.beginDate) ? new Date(data.beginDate) : null,
                endDate: (data.endDate) ? new Date(data.endDate) : null,
                isHiddenOnChart: (typeof (data.isHiddenOnChart) !== "boolean" && data.isHiddenOnChart === true) ? true : false
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

            modalInstance.closed.then(_refreshData);
        }

        function _close() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _refreshData() {
            return (vm.type === "income") ?

                $sandboxService.getAllIncomes().then(
                    a => vm.items = a.data.items,
                    err => $baseController.errorCheck(err,
                        {
                            maxLoops: 3,
                            miliseconds: 2000,
                            method: () => {
                                $sandboxService.getAllIncomes().then(a => vm.items = a.data.items);
                            }
                        })
                )

                :

                $sandboxService.getAllExpenses().then(
                    a => vm.items = a.data.items,
                    err => $baseController.errorCheck(err,
                        {
                            maxLoops: 3,
                            miliseconds: 2000,
                            method: () => {
                                $sandboxService.getAllExpenses().then(a => vm.items = a.data.items);
                            }
                        })
                )
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

    }

    function modalInsertController($scope, $baseController, $uibModalInstance, $sandboxService, model) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.submit = _submit;
        vm.reset = _setUp;
        vm.cancel = _cancel;
        vm.openDatePicker = _openDatePicker;

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
            vm.isHiddenOnChart = data.isHiddenOnChart || "0";

            vm.isTimePaidOpen = false;
            vm.isBeginDateOpen = false;
            vm.isEndDateOpen = false;

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
                    isHiddenOnChart: (vm.isHiddenOnChart === "1") ? true : false
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

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }


    }

})();