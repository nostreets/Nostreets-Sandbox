(function () {
    angular.module(page.APPNAME)
        .directive("carsousel", carouselDirective);

    carouselDirective.$inject = ["$baseController", "$serverModel"];


    function carouselDirective($baseController, $serverModel) {

        return {
            restrict: "E",
            scope: {
                collection: '='
            },
            template: "<div> <div class=\"row\"> <div class=\"col-sm-01\"> <button class=\"carousel_previousBtn\"> Previous </button> </div> <div class=\"col-sm-10 carousel1\"> <a class=\"carousel1_cell\" ng-repeat=\" item in collection \" href=\"{{item.link ? item.link : ''}}\"> {{ item.label }} </a> </div> <div class=\"col-sm-01\"> <button class=\"carousel_nextBtn\"> Next </button> </div> </div>",
            link: function ($scope, element, attr) {

                $(document).ready(_render);

                function _render() {

                    _setUp();
                    _handlers();
                }

                function _setUp() {
                    $scope.collection = typeof ($scope.collection) == "string" ? JSON.parse($scope.collection) : $scope.collection ? $scope.collection : [];
                    $scope.carousel = document.querySelector('.carousel1');
                    $scope.cells = $scope.carousel.querySelectorAll('.carousel1_cell');
                    $scope.cellCount = $scope.collection.length || '';
                    $scope.selectedIndex = 0;
                    $scope.cellWidth = $scope.carousel.offsetWidth;
                    $scope.cellHeight = $scope.carousel.offsetHeight;
                    $scope.isHorizontal = true;
                    $scope.rotateFn = $scope.isHorizontal ? 'rotateY' : 'rotateX';
                    $scope.radius = null;
                    $scope.theta = null;
                }

                function _handlers() {

                    $('.carousel_previousBtn').on('click', () => {
                        $scope.selectedIndex--;
                        rotateCarousel();
                    });

                    $('.carousel_nextBtn').on('click', () => {
                        $scope.selectedIndex++;
                        rotateCarousel();
                    });

                    //VALINLA JAVASCRIPT WAY
                    //$scope.prevButton = document.querySelector('.carousel_previousBtn');
                    //$scope.prevButton.addEventListener('click', function () {
                    //    $scope.selectedIndex--;
                    //    rotateCarousel();
                    //});

                    //$scope.nextButton = document.querySelector('.carousel_nextBtn');
                    //$scope.nextButton.addEventListener('click', function () {
                    //    $scope.selectedIndex++;
                    //    rotateCarousel();
                    //});

                    ////CHANGE NUMBER OF CELLS
                    //var cellsRange = document.querySelector('.cells-range');
                    //cellsRange.addEventListener('change', changeCarousel);
                    //cellsRange.addEventListener('input', changeCarousel);
                }


                function rotateCarousel() {
                    var angle = $scope.theta * $scope.selectedIndex * -1;
                    carousel.style.transform = 'translateZ(' + -$scope.radius + 'px) ' +
                        $scope.rotateFn + '(' + angle + 'deg)';
                }


                function changeCarousel() {
                    $scope.cellCount = cellsRange.value;
                    $scope.theta = 360 / $scope.cellCount;
                    var cellSize = isHorizontal ? cellWidth : cellHeight;
                    $scope.radius = Math.round((cellSize / 2) / Math.tan(Math.PI / $scope.cellCount));
                    for (var i = 0; i < cells.length; i++) {
                        var cell = cells[i];
                        if (i < $scope.cellCount) {
                            // visible cell
                            cell.style.opacity = 1;
                            var cellAngle = $scope.theta * i;
                            cell.style.transform = $scope.rotateFn + '(' + cellAngle + 'deg) translateZ(' + $scope.radius + 'px)';
                        } else {
                            // hidden cell
                            cell.style.opacity = 0;
                            cell.style.transform = 'none';
                        }
                    }

                    rotateCarousel();
                }

                ////TOGGLE DIRECTION VERTICAL TO HORIZONATAL
                //var orientationRadios = document.querySelectorAll('input[name="orientation"]');
                //(function () {
                //    for (var i = 0; i < orientationRadios.length; i++) {
                //        var radio = orientationRadios[i];
                //        radio.addEventListener('change', onOrientationChange);
                //    }
                //})();

                //function onOrientationChange() {
                //    var checkedRadio = document.querySelector('input[name="orientation"]:checked');
                //    isHorizontal = checkedRadio.value == 'horizontal';
                //    rotateFn = isHorizontal ? 'rotateY' : 'rotateX';
                //    changeCarousel();
                //}
                // SET INITIALS
                //onOrientationChange();

            }
        }


    }

})();