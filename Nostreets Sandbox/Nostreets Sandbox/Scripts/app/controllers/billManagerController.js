(function () {

    angular.module(page.APPNAME)
        .controller("billManagerController", billManagerController)
        .controller("modalMainMenuController", modalMainMenuController)
        .controller("modalInsertController", modalInsertController);

    billManagerController.$inject = ["$scope", "$baseController", '$uibModal', '$sandboxService'];
    modalMainMenuController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$uibModal', 'data'];
    modalInsertController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService', 'model'];


    function billManagerController($scope, $baseController, $uibModal, $sandboxService) {

        var vm = this;
        vm.changeCurrentTab = _changeTab;
        vm.openMainMenuModal = _openMainMenuModal;

        _render();

        function _render() {
            _setUp();
            _getUserData(_changeTab);
        }

        function _setUp() {
            vm.income = [];
            vm.expenses = [];
            vm.charts = [];
            vm.currentTab = 'income';
            vm.renderedChart = null;

        }

        function _getUserData(func) {

            _getAllExpense().then(
                () => _getAllIncome().then(
                    () => _getIncomeChart().then(
                        () => _getExpensesChart().then(
                            () => _getCombinedChart().then(
                                () => func()
                            )))));
        }

        function _arrayOfZeros(lengthOfArr) {
            var arr = [];
            for (var i = 0; i < lengthOfArr; i++) {
                arr.push(0);
            }
            return arr;
        }

        function _getIncomeChart() {
            return $sandboxService.getIncomesChart().then(
                (data) => {
                    var incomeChart = {
                        key: "income",
                        value: data.data.item
                    };
                    vm.charts.push(incomeChart);
                },
                (data) => $baseController.timeout(2000, $sandboxService.getIncomesChart()) //console.log(data)
            );
        }

        function _getExpensesChart() {
            return $sandboxService.getExpensesChart().then(
                (data) => {
                    var expensesChart = {
                        key: "expense",
                        value: data.data.item
                    };
                    vm.charts.push(expensesChart);
                },
                (data) => $baseController.timeout(2000, $sandboxService.getExpensesChart()) //console.log(data)
            );
        }

        function _getCombinedChart() {
            return $sandboxService.getCombinedChart().then(
                (data) => {
                    var combinedChart = {
                        key: "combined",
                        value: data.data.item
                    };
                    vm.charts.push(combinedChart);
                },
                (data) => $baseController.timeout(2000, $sandboxService.getCombinedChart()) //console.log(data)
            );
        }

        function _getAllIncome() {
            return $sandboxService.getAllIncomes().then(
                (data) => vm.income = data.data.items,
                (data) => $baseController.timeout(2000, $sandboxService.getAllIncomes()) //console.log(data)
            );
        }

        function _getAllExpense() {
            return $sandboxService.getAllExpenses().then(
                (data) => vm.expenses = data.data.items,
                (data) => $baseController.timeout(2000, $sandboxService.getAllExpenses()) //console.log(data)
            );
        }

        function _getIncome(id, name, scheduleType, incomeType) {
            $sandboxService.getIncome(id, name, scheduleType, incomeType).then(
                (data) => console.log(data),
                (data) => $baseController.timeout(2000, $sandboxService.getIncome()) //console.log(data)
            );
        }

        function _getExpense(id, name, scheduleType, incomeType) {
            $sandboxService.getExpense(id, name, scheduleType, incomeType).then(
                (data) => console.log(data),
                (data) => $baseController.timeout(2000, $sandboxService.getExpense()) //console.log(data)
            );
        }

        function _insertIncome(model) {

        }

        function _insertExpense(model) {
            $sandboxService.insertExpense().then(
                (data) => console.log(data),
                (data) => console.log(data)
            );
        }

        function _deleteIncome() {
            $sandboxService.deleteIncome().then(
                (data) => console.log(data),
                (data) => console.log(data)
            );
        }

        function _deleteExpense() {
            $sandboxService.deleteExpense().then(
                (data) => console.log(data),
                (data) => console.log(data)
            );
        }

        function _targetGraph(type, elementId) {
            var options = {
                axisX: {
                    labelOffset: {
                        x: 0,
                        y: 0
                    }
                }
            };

            var chart = vm.charts.filter((a) => a.key === type)[0].value;
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

            vm.renderedChart = chart;

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

            switch (typeId) {
                case "income":
                    data.items = vm.income;
                    break;

                case "expense":
                    data.items = vm.expenses;
                    break;

                case "combined":
                    var newArr = vm.expenses.concat(vm.income);
                    data.items = newArr;
            }

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
    }

    function modalMainMenuController($scope, $baseController, $uibModalInstance, $uibModal, data) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.openInsertModal = _openInsertModal;

        _render();

        function _render() {
            _setUp(data);
        }

        function _setUp(data) {
            if (data) {
                vm.type = data.type;
                vm.items = data.items || [];
            }
        }

        function _openInsertModal(data) {

            data = (data) ? data : {};

            var obj = {
                type: vm.type,
                name: (data.name) ? data.name : null,
                price: (data.price) ? data.price : null,
                paySchedule: (data.paySchedule) ? data.paySchedule : null,
                timePaid: (data.timePaid) ? new Date(data.timePaid) : null, //new Date(),
                beginDate: (data.beginDate) ? new Date(data.beginDate) : null,// new Date(),
                endDate: (data.endDate) ? new Date(data.endDate) : null,//new Date(),
                isHiddenOnChart: (typeof (data.isHiddenOnChart) !== "boolean" && data.isHiddenOnChart == true) ? true : false
            };

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
        }

        function _close() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _onSuccess(data) {

        }

        function _consoleResponse(data) {
            console.log(data);
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
            vm.price = data.price || 0;
            vm.paySchedule = data.paySchedule || "1";
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

            var obj = {
                name: vm.name,
                paySchedule: vm.paySchedule,
                timePaid: vm.timePaid,
                beginDate: (vm.beginDate) ? new Date(vm.beginDate) : null,
                endDate: (vm.endDate) ? new Date(vm.endDate) : null,
                isHiddenOnChart: (vm.isHiddenOnChart === "1") ? true : false
            };


            switch (vm.type) {
                case "income":
                    obj.incomeType = parseInt(vm.incomeType);
                    obj.payRate = vm.price;

                    if (vm.id) {
                        obj.id = vm.id
                        $sandboxService.updateIncome(obj).then(
                            () => vm.$uibModalInstance.close(),
                            () => $sandboxService.updateIncome(obj).then(
                                () => vm.$uibModalInstance.close(),
                                (err) => console.log(err)
                            ));
                    }
                    else
                    {
                        $sandboxService.insertIncome(obj).then(
                            () => vm.$uibModalInstance.close(),
                            (err) => $sandboxService.insertIncome(obj).then(
                                () => vm.$uibModalInstance.close(),
                                (err) => console.log(err)
                            ));
                    }
                    break;

                case "expense":
                    obj.expenseType = parseInt(vm.expenseType);
                    obj.price = vm.price;

                    if (vm.id) {
                        obj.id = vm.id
                        $sandboxService.updateExpense(obj).then(
                            () => vm.$uibModalInstance.close(),
                            () => $sandboxService.updateExpense(obj).then(
                                () => vm.$uibModalInstance.close(),
                                (err) => console.log(err)
                            ));
                    }
                    else
                    {
                        $sandboxService.insertExpense(obj).then(
                            () => vm.$uibModalInstance.close(),
                            (err) => $sandboxService.insertExpense(obj).then(
                                () => vm.$uibModalInstance.close(),
                                (err) => console.log(err)
                            ));
                    }
                    break;
            }
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _errorResponse(error) {
            console.log(error);
        }

        function _consoleResponse(data) {
            console.log(data);
        }

    }

})();