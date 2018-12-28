/*
    DEPENDECIES
        -JQUERY
        -ANGULARJS
*/

(function () {
    angular.module(page.APPNAME)
        .directive("carsousel", carouselDirective);

    carouselDirective.$inject = ["$interval"];

    function carouselDirective($interval) {

        return {
            restrict: "E",

            scope: {
                collection: '=',
                debugMode: '@',
                isHorizontal: '@',
                width: '@',
                height: '@',
                backgroundColor: '@',
                showArrows: '@',
                textColor: '@',
                textFont: '@',
                scrollControl: '@',
                swipeControl: '@',
                interval: '@'
            },

            template: "<div ng-show=\"!isHorizontal && showArrows\" class=\"col-sm-1 carouselVertictalBtn\"> <a class=\"carousel_previousBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_upward</i> </a> </div> <div class=\"container carousel1_root\"> <div ng-show=\"isHorizontal && showArrows\" class=\"col-sm-1\" style=\"padding: 7% 0% 0% 0%; margin-right: 10%;\"> <a class=\"carousel_previousBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_back</i> </a> </div>     <div class=\"carousel1_container col-sm-10\"> <div class=\"carousel1\"> <a class=\"carousel1_cell\" ng-repeat=\" item in collection \" href=\"{{item.link ? item.link : ''}}\" on-repeat-finished> {{ item.label }} </a> </div> </div>      <div ng-show=\"isHorizontal && showArrows\" class=\"col-sm-1\" style=\"padding: 7% 0% 0% 39%;\"> <a class=\"carousel_nextBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_forward</i> </a> </div> </div> <div ng-show=\"!isHorizontal && showArrows\" class=\"col-sm-1 carouselVertictalBtn\"> <a class=\"carousel_nextBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_downward</i> </a> </div>",

            link: function ($scope, element, attr) {

                $scope.$on('ngRepeatFinished', (ngRepeatFinishedEvent) => {
                    _render();
                });


                function _render() {

                    _setUp();
                    _setDirectiveStyle();
                    _handlers();
                    _onOrientationChange();

                    $interval(() => {
                        $scope.selectedIndex++;
                        _rotateCarousel();
                    }, $scope.interval);
                }

                function _setUp() {

                    $scope.collection = typeof ($scope.collection) == "string" ? JSON.parse($scope.collection) : $scope.collection ? $scope.collection : [];
                    $scope.isHorizontal = $scope.isHorizontal && ($scope.isHorizontal == 'false' || $scope.isHorizontal === '0') ? false : true;
                    $scope.showArrows = $scope.showArrows && ($scope.showArrows == 'false' || $scope.showArrows === '0') ? false : true;
                    $scope.scrollControl = $scope.scrollControl && ($scope.scrollControl == 'false' || $scope.scrollControl === '0') ? false : true;
                    $scope.swipeControl = $scope.swipeControl && ($scope.swipeControl == 'false' || $scope.swipeControl === '0') ? false : true;
                    $scope.debugMode = $scope.debugMode && ($scope.debugMode == 'false' || $scope.debugMode === '0') ? false : true;
                    $scope.interval = $scope.interval && (parseInt($scope.interval) !== NaN) ? parseInt($scope.interval) : 10000;
                    $scope.width = $scope.width || '200px';
                    $scope.height = $scope.height || '120px';
                    $scope.textColor = $scope.textColor || 'white';
                    $scope.textFont = $scope.textFont || 'Trebuchet MS';
                    $scope.backgroundColor = $scope.backgroundColor || 'hsla(245, 18%, 48%, 0.80)';

                    $scope.carousel = document.querySelector('.carousel1');
                    $scope.cells = $scope.carousel.querySelectorAll('.carousel1_cell');
                    $scope.cellCount = $scope.collection.length || 0;
                    $scope.selectedIndex = 0;
                    $scope.cellWidth = parseInt($scope.width);
                    $scope.cellHeight = parseInt($scope.height);
                    $scope.rotateFn = $scope.isHorizontal ? 'rotateY' : 'rotateX';
                    $scope.radius = null;
                    $scope.theta = null;
                    $scope.lastScrollPosition = 0;
                }

                function _handlers() {

                    var down = () => {
                        $scope.selectedIndex++;
                        _rotateCarousel();
                    },
                        up = () => {
                            $scope.selectedIndex--;
                            _rotateCarousel();
                        };

                    $('.carousel_nextBtn').on('click', down);

                    $('.carousel_previousBtn').on('click', up);

                    if ($scope.scrollControl)
                        _onScroll(down, up);

                    if ($scope.swipeControl) {

                        if (!page.utilities.checkForJQEvent(".carousel1_cell, .carouselVertictalBtn", 'swipeup')) {
                            console.log("Setting Up Swipe Events...");
                            _setUpSwipeHandlers();
                            _onSwipe(down, up);

                        }
                        else
                            _onSwipe(down, up);
                    }


                    /* VALINLA JAVASCRIPT WAY
                    $scope.prevButton = document.querySelector('.carousel_previousBtn');
                    $scope.prevButton.addEventListener('click', function () {
                        $scope.selectedIndex--;
                        rotateCarousel();
                    });
        
                    $scope.nextButton = document.querySelector('.carousel_nextBtn');
                    $scope.nextButton.addEventListener('click', function () {
                        $scope.selectedIndex++;
                        rotateCarousel();
                    });
        
                    //CHANGE NUMBER OF CELLS
                    var cellsRange = document.querySelector('.cells-range');
                    cellsRange.addEventListener('change', changeCarousel);
                    cellsRange.addEventListener('input', changeCarousel);
                    */
                }

                function _rotateCarousel() {
                    var angle = $scope.theta * $scope.selectedIndex * -1;
                    $scope.carousel.style.transform = 'translateZ(' + -$scope.radius + 'px) ' + $scope.rotateFn + '(' + angle + 'deg)';
                }

                function _changeCarousel() {

                    //$scope.cellCount = cellsRange.value;
                    $scope.theta = 360 / $scope.cellCount;

                    var cellSize = $scope.isHorizontal ? $scope.cellWidth : $scope.cellHeight;
                    $scope.radius = Math.round((cellSize / 2) / Math.tan(Math.PI / $scope.cellCount));

                    for (var i = 0; i < $scope.cells.length; i++) {

                        var cell = $scope.cells[i];

                        if (i < $scope.cellCount) {
                            // visible cell
                            cell.style.opacity = 1;
                            var cellAngle = $scope.theta * i;
                            cell.style.transform = $scope.rotateFn + '(' + cellAngle + 'deg) translateZ(' + $scope.radius + 'px)';
                        }
                        else {
                            // hidden cell
                            cell.style.opacity = 0;
                            cell.style.transform = 'none';
                        }
                    }

                    _rotateCarousel();
                }

                function _onOrientationChange() {
                    //var checkedRadio = document.querySelector('input[name="orientation"]:checked');
                    //$scope.isHorizontal = checkedRadio.value == 'horizontal';
                    $scope.rotateFn = $scope.isHorizontal ? 'rotateY' : 'rotateX';
                    _changeCarousel();
                }

                function _writeStyles(styleName, cssRules) {
                    var styleElement = document.getElementById(styleName);
                    var pastCssRules = (styleElement && styleElement.textContent) ? styleElement.textContent : null;


                    if (styleElement) {
                        document.getElementsByTagName('head')[0].removeChild(
                            styleElement);
                    }

                    styleElement = document.createElement('style');
                    styleElement.type = 'text/css';
                    styleElement.id = styleName;


                    if (cssRules.length && typeof (cssRules) !== 'string') {
                        for (var css of cssRules) {
                            styleElement.appendChild(document.createTextNode(css));
                        }
                    }
                    else {
                        styleElement.innerHTML = cssRules;
                    }

                    document.getElementsByTagName('head')[0].appendChild(styleElement);
                }

                function _setDirectiveStyle() {
                    var style = ".carousel1_root{margin: 20% 0% 15% 0%;width:" + $scope.width + ";}  .carousel1_container{position:relative;width:210px;height:140px;perspective:1000px}  .carousel1{width:100%;height:100%;position:absolute;transform:translateZ(-288px);transform-style:preserve-3d;transition:transform 1s}  .carousel1_cell{background:" + $scope.backgroundColor + ";position:absolute;width:" + $scope.width + ";height:" + $scope.height + ";left:10px;top:10px;border:2px solid black;line-height:50px;font-size:2em;font-weight:bold;color:" + $scope.textColor + ";text-align:center;transition:transform 1s,opacity 1s;align-items:center;display:flex;justify-content:center;font-family: " + $scope.textFont + "}  .carousel1-options{text-align:center;position:relative;z-index:2;background:hsla(0,0%,100%,0.8)}    @media only screen and (max-width: 480px) {   carsousel{margin-right:20%}  .carouselVertictalBtn{padding-left: 16em;}   }    @media only screen and (min-width: 480px) {   carsousel{margin-right:10%}   .carouselVertictalBtn{padding-left: 11em;}   } ";

                    _writeStyles("carousel_styles", style);
                }

                function _onScroll(onDown, onUp) {

                    if (onUp && onDown && typeof (onUp) === 'function' && typeof (onDown) === 'function')
                        $('.carousel1_cell, .carouselVertictalBtn').on(
                            'mousewheel',
                            (scrollEvent) => {

                                scrollEvent.preventDefault();

                                if (scrollEvent.originalEvent.wheelDelta >= 0) {
                                    if ($scope.debugMode)
                                        console.log('Scroll up');
                                    onUp();
                                }
                                else {
                                    if ($scope.debugMode)
                                        console.log('Scroll down');
                                    onDown();
                                }
                            });
                }

                function _onSwipe(onDown, onUp) {

                    if (onUp && typeof (onUp) === 'function')
                        $('.carousel1_cell, .carouselVertictalBtn').on(
                            'swipeup',
                            (swipeUpEvent) => {

                                swipeUpEvent.preventDefault();

                                if ($scope.debugMode)
                                    console.log('Scroll up');
                                onUp();
                            }
                        );

                    if (onDown && typeof (onDown) === 'function')
                        $('.carousel1_cell, .carouselVertictalBtn').on(
                            'swipedown',
                            (swipeDownEvent) => {

                                swipeDownEvent.preventDefault();

                                if ($scope.debugMode)
                                    console.log('Scroll down');
                                onDown();
                            }
                        );
                }

                function _setUpSwipeHandlers() {

                    var supportTouch = $.support.touch,
                        scrollEvent = "touchmove scroll",
                        touchStartEvent = supportTouch ? "touchstart" : "mousedown",
                        touchStopEvent = supportTouch ? "touchend" : "mouseup",
                        touchMoveEvent = supportTouch ? "touchmove" : "mousemove";

                    $.event.special.swipeupdown = {

                        setup: function () {

                            var thisObject = this;

                            var $this = $(thisObject);

                            $this.bind(touchStartEvent, (event) => {

                                var data = event.originalEvent.touches ? event.originalEvent.touches[0] : event,
                                    start = {
                                        time: (new Date).getTime(),
                                        coords: [data.pageX, data.pageY],
                                        origin: $(event.target)
                                    },
                                    stop;

                                var moveHandler = (event) => {
                                    if (!start)
                                        return;

                                    var data = event.originalEvent.touches ?
                                        event.originalEvent.touches[0] :
                                        event;

                                    stop = {
                                        time: (new Date).getTime(),
                                        coords: [data.pageX, data.pageY]
                                    };

                                    // prevent scrolling
                                    if (Math.abs(start.coords[1] - stop.coords[1]) > 10)
                                        event.preventDefault();

                                };

                                $this.bind(touchMoveEvent, moveHandler).one(touchStopEvent,
                                    (event) => {
                                        $this.unbind(touchMoveEvent, moveHandler);
                                        if (start && stop) {
                                            if (stop.time - start.time < 1000 &&
                                                Math.abs(start.coords[1] - stop.coords[1]) > 30 &&
                                                Math.abs(start.coords[0] - stop.coords[0]) < 75) {
                                                start.origin
                                                    .trigger("swipeupdown")
                                                    .trigger(start.coords[1] > stop.coords[1] ? "swipeup" : "swipedown");
                                            }
                                        }
                                        start = stop = undefined;
                                    });
                            });
                        }

                    };


                    $.each({ swipedown: "swipeupdown", swipeup: "swipeupdown" },
                        (event, sourceEvent) => {
                            $.event.special[event] = {
                                setup: () => {
                                    $(this).bind(sourceEvent, $.noop);
                                }
                            };
                        });


                }

                function _checkForEvent(element, event) {


                }

            }
        }


    }

})();