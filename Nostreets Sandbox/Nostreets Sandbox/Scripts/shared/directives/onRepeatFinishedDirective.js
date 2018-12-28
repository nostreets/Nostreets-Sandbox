(function () {


    angular.module(page.APPNAME)
        .directive('onRepeatFinished', onRepeatFinished);

    onRepeatFinished.$inject = ['$timeout', '$parse'];

    function onRepeatFinished($timeout, $parse) {
        return {
            restrict: 'A',
            link: function ($scope, element, attr) {
                if ($scope.$last === true) {
                    $timeout(function () {
                        $scope.$emit('ngRepeatFinished');
                        if (attr.onRepeatFinished) {
                            $parse(attr.onRepeatFinished)($scope);
                        }
                    });
                }
            }
        }
    }

})();