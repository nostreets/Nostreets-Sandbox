(function () {
    angular.module(page.APPNAME)
        .directive("carsousel", carouselDirective);

    carouselDirective.$inject = ["$interval"];

    function carouselDirective($interval) {

        return {
            restrict: "E",

            scope: {
                collection: '=',
                isHorizontal: '@',
                width: '@',
                height: '@',
                backgroundColor: '@',
                textColor: '@',
                textFont: '@'
            },

            template: "<div ng-show=\"!isHorizontal\" class=\"col-sm-1\" style=\"padding-left: 25%;\"> <a class=\"carousel_previousBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_upward</i> </a> </div> <div class=\"col-sm-12 carousel1_root\"> <div ng-show=\"isHorizontal\" class=\"col-sm-1\" style=\"padding: 7% 0% 0% 0%; margin-right: 10%;\"> <a class=\"carousel_previousBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_back</i> </a> </div>  <div class=\"carousel1_container col-sm-10\"> <div class=\"carousel1\"> <a class=\"carousel1_cell\" ng-repeat=\" item in collection \" href=\"{{item.link ? item.link : ''}}\" on-repeat-finished > {{ item.label }} </a> </div> </div> <div ng-show=\"isHorizontal\" class=\"col-sm-1\" style=\"padding: 7% 0% 0% 39%;\"> <a class=\"carousel_nextBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_forward</i> </a> </div> </div> <div ng-show=\"!isHorizontal\" class=\"col-sm-1\" style=\"padding-left: 25%;\"> <a class=\"carousel_nextBtn\"> <i class=\"material-icons\" style=\"color: white;\">arrow_downward</i> </a> </div>",

            link: function ($scope, element, attr) {

                $scope.$on('ngRepeatFinished', function (ngRepeatFinishedEvent) {
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
                    }, 10000);
                }

                function _setUp() {

                    $scope.collection = typeof ($scope.collection) == "string" ? JSON.parse($scope.collection) : $scope.collection ? $scope.collection : [];
                    $scope.isHorizontal = $scope.isHorizontal && ($scope.isHorizontal == 'false' || $scope.isHorizontal === '0') ? false : true;
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
                }

                function _handlers() {

                    $('.carousel_nextBtn').on('click', () => {
                        $scope.selectedIndex++;
                        _rotateCarousel();
                    });

                    $('.carousel_previousBtn').on('click', () => {
                        $scope.selectedIndex--;
                        _rotateCarousel();
                    });

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

                ////TOGGLE DIRECTION VERTICAL TO HORIZONATAL
                //var orientationRadios = document.querySelectorAll('input[name="orientation"]');
                //(function () {
                //    for (var i = 0; i < orientationRadios.length; i++) {
                //        var radio = orientationRadios[i];
                //        radio.addEventListener('change', onOrientationChange);
                //    }
                //})();

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
                    var style = ".carousel1_root{margin:40px 0;margin:80px auto}  .carousel1_container{position:relative;width:210px;height:140px;perspective:1000px}  .carousel1{width:100%;height:100%;position:absolute;transform:translateZ(-288px);transform-style:preserve-3d;transition:transform 1s}  .carousel1_cell{background:" + $scope.backgroundColor + ";position:absolute;width:" + $scope.width + ";height:" + $scope.height + ";left:10px;top:10px;border:2px solid black;line-height:50px;font-size:2em;font-weight:bold;color:" + $scope.textColor + ";text-align:center;transition:transform 1s,opacity 1s;align-items:center;display:flex;justify-content:center;font-family: " + $scope.textFont + "}  .carousel1-options{text-align:center;position:relative;z-index:2;background:hsla(0,0%,100%,0.8)}";

                    _writeStyles("carousel_styles", style);
                }

            }
        }


    }

})();