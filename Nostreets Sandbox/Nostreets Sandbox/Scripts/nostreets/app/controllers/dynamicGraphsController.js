
(function () {

    angular.module(page.APPNAME).controller("dynamicGraphsController", pageController)
        .controller("modalGraphController", modalController);

    pageController.$inject = ["$scope", "$baseController", "$uibModal", '$rootScope', "$oblApiService"];
    modalController.$inject = ["$scope", "$timeout", "$uibModalInstance", "graph", "$rootScope"];


    function pageController($scope, $baseController, $uibModal, $rootScope, $oblApiService) {

        //View Model and Mapping
        var vm = this;
        vm.$scope = $scope;
        vm.openModal = _openModal;
        vm.renderGraph = _renderMainGraph;
        vm.saveGraph = _insertGraph;
        vm.delete = _delete;
        vm.viewControllerCode = _viewControllerCode;
        vm.viewDirectiveCode = _viewDirectiveCode;





        _render();

        //Controller Functions
        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {
            vm.chart = null;
            vm.charts = [];
            vm.legend = [];
            vm.saved = false;
            vm.rendered = false;
            vm.legend = [];
            vm.isLoggedIn = page.isLoggedIn;

            $oblApiService.getAllChartsByUser().then(_chartsResponse, _consoleResponse);
        };

        function _handlers() {
            //$rootScope.$on('logGraph', _logMainGraph);
            //$baseController.event.listen('refreshCharts', _refreshResponse);
            //$baseController.event.listen("loggedIn", _setUp);


            $baseController.defaultListeners($scope,
                {
                    'logGraph': _logMainGraph,
                    'refreshCharts': _refreshResponse,
                    "loggedIn": _setUp
                });
        }

        function _viewDirectiveCode() {
            $baseController.http({
                url: "api/view/code/dymanicGraphsDirective",
                method: "GET",
                responseType: "JSON"
            }).then(function (data) {
                _openCodeModal(data.data.item);
            });
        }

        function _viewControllerCode() {
            $baseController.http({
                url: "api/view/code/dymanicGraphsController",
                method: "GET",
                responseType: "JSON"
            }).then(function (data) {
                _openCodeModal(data.data.item);
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

        function _openModal() {
            var obj = {
                chartId: (vm.chart && vm.chart.chartId) ? vm.chart.chartId : 0,
                typeId: (vm.chart && vm.chart.typeId) ? vm.chart.typeId : 0,
                name: vm.graphName || null,
                labels: vm.chart == null ? [] : vm.chart.labels,
                lines: (vm.chart && vm.chart.typeId) === 3 ? vm.chart.series : !vm.chart ? [] : null
            };
            if (!obj.lines) {
                obj.lines = [];
                for (var i = 0; i < vm.chart.series.length; i++) {
                    obj.lines.push({
                        key: vm.legend[i],
                        points: vm.chart.series[i]
                    });
                }
            }


            var modalInstance = $uibModal.open({
                animation: true
                , templateUrl: "modalGraphBuilder.html"
                , controller: "modalGraphController as mc"
                , size: "lg"
                , resolve: {
                    graph: function () {
                        return obj;
                    }
                }
            });
        }

        function _logMainGraph(scope, item) {
            if (item[1] && item[1].chartId) {
                item = item[1];
            }
            vm.chart = {
                chartId: item.chartId || 0,
                labels: (item.labels[0].label) ? [] : item.labels,
                series: (!item.series) ? [] : item.series,
                typeId: typeof (item.typeId) === "string" ? parseInt(item.typeId) : item.typeId
            };
            vm.graphName = item.name;

            if (item.legend) { vm.legend = item.legend; }

            if (item.labels[0].label) {
                for (var i of item.labels) {
                    vm.chart.labels.push(i.label);
                }
            }

            if (item.lines) {
                for (var i of item.lines) {
                    if (!vm.legend[0] || vm.legend.find(a => { return a !== i.key; })) {
                        vm.legend.push(i.key);
                    }
                    var arr = [];
                    for (var p of i.points) {
                        arr.push(p.point);
                    }
                    vm.chart.series.push(arr);
                }
            }


            _renderMainGraph(vm.chart);
        }

        function _renderMainGraph(chart) {
            vm.saved = false;
            var id = typeof (chart.typeId) === "string" ? parseInt(chart.typeId) : chart.typeId;

            switch (id) {
                case 1:
                    var options = {
                        axisX: {
                            labelOffset: {
                                x: 0,
                                y: 0
                            }
                        },
                        fullWidth: true
                    };
                    vm.renderedChart = new Chartist.Line("#graphMainDiv", chart, options);
                    break;
                case 2:
                    var options = {
                        axisX: {
                            labelOffset: {
                                x: 0,
                                y: 0
                            }
                        },
                        fullWidth: true
                    };
                    vm.renderedChart = new Chartist.Bar("#graphMainDiv", chart, options);
                    break;
                case 3:
                    var options = {
                        donut: true,
                        showLabel: true,
                        donutWidth: 100,
                        labelDirection: "explode"
                    };
                    var oneArr = typeof (chart.series[0]) !== 'number' ? (function () {
                        var slice = [];
                        for (let arr of chart.series) {
                            for (let num of arr) {
                                slice.push(num);
                            }
                        }
                        return slice;
                    })() : chart.series;
                    chart.series = oneArr;
                    vm.renderedChart = new Chartist.Pie("#graphMainDiv", chart, options);
                    break;
            }
            vm.rendered = true;
            angular.element("#graphMainDiv").removeClass('hidden');
            _animateGraph(vm.renderedChart);
        }

        function _buildLineGraph() {
            var chart = {
                labels: vm.chart.labels,
                series: vm.chart.series
            }

            for (var line of vm.lines) {
                chart.series.push(line.points);
            }

            return chart;
        }

        function _getLabels(arr) {
            if (arr == null) {
                return ['N/A', 'N/A', 'N/A', 'N/A', 'N/A', "N/A", "N/A"];
            }
            else if (typeof (arr) === "object" && arr[0]) {
                return arr;
            }
            else if (arr === "days") {
                return ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', "Saterday", "Sunday"];
            }
            else if (arr === "months") {
                return ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
            }
        }

        function _animateGraph(chart) {
            // sequence number aside so we can use it in the event callbacks
            var seq = 0,
                delays = 80,
                durations = 400;

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

        function _insertGraph() {
            if (vm.chart) {
                var model = {
                    name: vm.graphName,
                    typeId: vm.chart.typeId,
                    legend: vm.legend,
                    labels: vm.chart.labels,
                    series: vm.chart.series
                }
                if (!vm.chart.chartId) {
                    $oblApiService.insertChart(model).then(_idResponse, _consoleResponse);
                }
                else {
                    model.id = vm.chart.chartId;
                    $oblApiService.updateChart(model).then(_consoleResponse, _consoleResponse);
                }
                vm.saved = true;
                $oblApiService.getAllChartsByUser.then(_chartsResponse, _consoleResponse);
            }
        }

        function _delete(chart) {
            if (chart && chart.chartId) {
                $oblApiService.deleteChartById(chart.chartId).then(_refreshResponse, _consoleResponse);
            }
        }

        function _chartsResponse(data) {
            if (data.data.items) {
                vm.charts = data.data.items;
            }
        }

        function _idResponse(data) {
            if (data.data.item) {
                var chart = {
                    name: vm.graphName,
                    typeId: 1,
                    legend: vm.legend,
                    id: data.data.item,
                    labels: vm.chart.labels,
                    series: vm.chart.series
                };
                vm.charts.push(chart);
            }
            _refreshResponse();
        }

        function _consoleResponse(data) {
            console.log(data);
        }

        function _refreshResponse() {
            $oblApiService.getAllChartsByUser.then(_chartsResponse, _consoleResponse);
        }
    }

    function modalController($scope, $timeout, $uibModalInstance, graph, $rootScope) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.createLine = _buildEmptyLine;
        vm.submit = _submit;
        vm.validateData = _validateData;
        vm.reset = _setUp;

        _startUp();

        function _startUp() {
            _setUp(graph);
        }

        function _setUp(item) {
            if (typeof (item) !== 'object') { item = {}; }

            vm.chartId = item.chartId || 0;
            vm.typeId = item.typeId || 0;
            vm.points = item.labels ? item.labels.length : 0,
                vm.name = item.name || null;
            vm.items = (function () {
                var arr = [];
                if (item.lines && item.lines[0] && item.lines[0].key) {
                    for (var i of item.lines) {
                        var pointsArr = [];
                        for (var p of i.points) {
                            pointsArr.push({ point: p });
                        }
                        arr.push({ key: i.key, points: pointsArr });
                    }
                }
                else {
                    if (item.lines && item.lines[0]) {
                        var pointsArr = [];
                        for (var i of item.lines) {
                            pointsArr.push({ point: i });
                        }
                        arr.push({ key: 'Chart', points: pointsArr });
                    }
                }
                return arr;
            })();
            vm.labels = (function () {
                var arr = [];
                if (item.labels) {
                    for (var i of item.labels) {
                        arr.push({ label: i });
                    }
                }
                return arr;
            })();
        }

        function _validateData() {

            for (let i of vm.items) {
                if (!i.key) {
                    return false;
                }
            }
            return true;
        }

        function _buildEmptyLine(number) {
            var line = {
                key: vm.typeId === '3' ? "Chart" : "",
                points: []
            };
            for (var i = 0; i < number; i++) {
                var point = Math.round(Math.random() * 20);
                if (Math.random() < 0.5 && vm.typeId !== '3') { point = -point; }
                line.points.push({ point: point });
                if (!vm.labels[i]) {
                    vm.labels.push({ label: "Label " + (i + 1) });
                }
            }
            vm.items.push(line);

        }

        function _submit() {
            var chart = {
                chartId: vm.chartId,
                labels: vm.labels,
                lines: vm.items,
                name: vm.name,
                typeId: vm.typeId
            };
            $rootScope.$broadcast('logGraph', chart);
            vm.$uibModalInstance.close();
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _onSuccess(data) {

        }

        function _errorResponse(error) {
            console.log(error);
        }

        function _consoleResponse(data) {
            console.log(data);
        }

    }
})();