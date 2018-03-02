(function () {
    "use strict";

    angular.module(APPNAME).controller("regController", registerController);

    registerController.$inject = ["$scope", "$baseController", '$uibModalInstance', '$sandboxService', 'model'];

    function registerController($scope, $baseController, $uibModalInstance, $sandboxService, model) {

        var vm = this;
        vm.$scope = $scope;

        function _render() {
            demo.checkFullPageBackgroundImage();

            setTimeout(function () {
                // after 1000 ms we add the class animated to the login/register card
                vm.$document.find('.card').removeClass('card-hidden');
            }, 700);
        }

        function _setUp() {
            vm.request = {
                userName: null
                , password: ""
                , passwordConfirmation: ""
                , confirmPassword: ""
                , referralCode: ""
            };
            vm.userNameRegex = "[A-Za-z0-9\ \_\']*";
            vm.userNameExists = false;
            vm.emailExists = false;
            vm.$scope.regVm.request.userName = null;
            vm.$scope.regVm.request.email = null;
            vm.user = null;
            vm.formInvalid = true;
        }

    }
})();