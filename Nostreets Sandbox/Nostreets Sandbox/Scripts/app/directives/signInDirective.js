(function () {
    angular.module(page.APPNAME).directive("signIn", signInDirective);
    function signInDirective($baseController) {

        return {
            restrict: "A",
            scope: true,
            link: function ($scope, element, attr) {

                $(document).ready(_startUp);
                function _startUp() {
                    element.on("click", function () {
                        $baseController.loginPopup();
                    });

                }



            }
        }
    }
})();