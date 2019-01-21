(function () {

    angular.module(page.APPNAME).directive("chart", directive);

    directive.$inject = ["$systemEventService", '$oblApiService'];

    function directive($systemEventService, $oblApiService) {

        return {
            restrict: "A",
            scope: {
                graph: "=graph"
            },
            link: function ($scope, element, attr) {

                $(document).ready(renderGraph);

                function renderGraph() {

                    var mainElement = attr.mainGraph;
                    var data = $scope.graph;
                    var graph = {
                        labels: data.labels,
                        series: data.series
                    };

                    _startUp();

                    function _startUp() {

                        element.attr("class", "graph" + data.chartId);

                        $(element).append("<input type='button' class='btn btn-xs btn-round pull-center' value='View' id='viewGraph" + data.chartId + "' />");
                        $(element).append("<input type='button' class='btn btn-xs btn-round pull-center' value='X' style='background-color: #e00;' id='deleteGraph" + data.chartId + "' />");

                        $("#viewGraph" + data.chartId).on("click", _sendLogEvent);
                        $("#deleteGraph" + data.chartId).on("click", _deleteById);

                        _targetElementGraph();
                    }

                    function _deleteById()
                    {
                        $oblApiService.deleteChartById(data.chartId, _consoleResponse, _consoleResponse);
                        $systemEventService.broadcast("refreshCharts", true);
                    }

                    function _targetElementGraph() {
                        switch (data.typeId) {
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
                                var renderedChart = new Chartist.Line(".graph" + data.chartId, graph, options);
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
                                var renderedChart = new Chartist.Bar(".graph" + data.chartId, graph, options);
                                break;
                            case 3:
                                var options = {
                                    donut: true,
                                    showLabel: true,
                                    donutWidth: 20,
                                    labelDirection: "explode"
                                };
                                var renderedChart = new Chartist.Pie(".graph" + data.chartId, graph, options);
                                break;
                        }
                        _animateGraph(renderedChart, 100);
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

                    function _sendLogEvent() {
                        $systemEventService.broadcast("logGraph", data);
                        var top = $("#graphMainDiv").offset().top;
                        var selector = window.innerWidth < 1025 ? $("body") : $("#rightSide");
                        selector.animate({ scrollTop: top }, 1000);
                    }

                    function _consoleResponse(data){
                        console.log(data);
                    }
                }
            }
        }
    }

})();
