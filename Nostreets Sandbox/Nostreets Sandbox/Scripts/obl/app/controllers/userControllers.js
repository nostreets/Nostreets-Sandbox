(function () {
    angular.module(page.APPNAME)
        .controller("userDashboardController", userController)


    userController.$inject = ["$scope", "$baseController"];


    function userController($scope, $baseController) {
        var vm = this;

        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {

            vm.sliderOptions = page.sliderOptions;
            vm.windowWidth = page.utilities.getDeviceWidth();
            vm.isLoggedIn = page.isLoggedIn;

        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | User Dashboard");
            page.renderParticles();
            page.fixFooter();
        }

    }

})();