(function () {

    angular.module(APPNAME).controller("loginController", loginController);

    loginController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService'];

    function loginController($scope, $baseController, $uibModalInstance, $sandboxService) {

        var vm = this;

        vm.registerService = registerService;
        vm.sweetAlert = sweetAlert;
        vm.$scope = $scope;
        $baseController.merge(vm, $baseController);

        vm.notify = vm.registerService.getNotifier($scope);


        vm.submitLogin = _submitLogin;


        function _render() {
            _setUp();
        }

        function _setUp() {
            vm.returnUrl = null;
            vm.request = {
                    email: null
                    , password: null
                };
        }

        function _submitLogin() {
            vm.registerService.login(vm.request, _loginRedirect, _loginFailed)
        }

        function _loginRedirect(data) {
             

        }

        function _loginFailed(status, errorThrown, xhr) {
             

        }
    }

})();