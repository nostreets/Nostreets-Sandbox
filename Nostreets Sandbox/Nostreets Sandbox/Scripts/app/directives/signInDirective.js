(function () {
    angular.module(page.APPNAME)
        .directive("signIn", signInDirective)
        .controller("modalRegisterController", modalRegisterController)
        .controller("modalLogInController", modalLogInController);

    signInDirective.$inject = ["$baseController"];
    signInDirective.$inject = ["$scope", "$baseController", "$uibModalInstance"];
    signInDirective.$inject = ["$scope", "$baseController", "$uibModalInstance"];

    function signInDirective($baseController) {

        return {
            restrict: "A",
            scope: true,
            template: (typeof (page.user.data) === "undefined") ? "<i class=\"material-icons\">folder_shared</i>" : "<i class=\"material-icons\">person</i>",
            link: function ($scope, element, attr) {

                //$(document).ready(_startUp);

                _render();

                function _render() {
                    element.on("click", () => {
                        if (typeof (page.user.data) == "undefined")
                            _openRegisterModal();
                        else
                            _openLoginModal();
                        //$baseController.loginPopup();
                    });



                }

                function _openRegisterModal() {

                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "~/template/registerForm.html"
                        , controller: "modalRegisterController as regVm"
                        , size: "lg"
                    });

                }


                function _openLoginModal() {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "~/template/loginForm.html"
                        , controller: "modalLogInController as logVm"
                        , size: "lg"
                    });

                }



            }
        }
    }

    function modalRegisterController($scope, $baseController, $uibModalInstance) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.submit = _submit;
        vm.reset = _setUp;
        vm.cancel = _cancel;


        _render();

        function _render() {
            _setUp();
        }

        function _setUp() {
            vm.username = null;
            vm.password = null;
            vm.rePassword = null;
            vm.email = null;
            vm.reEmail = null;
            vm.firstName = null;
            vm.lastName = null;
            vm.validatorRegEx = /<[a-z][\s\S]*>/;
            vm.emailExists = false;
            vm.usernameExists = false;
        }

        function _submit() {

            var obj = {
                username: vm.username,
                password: vm.password,
                contact: {
                    primaryEmail: vm.email,
                    firstName: vm.firstName,
                    lastName: vm.lastName
                }
            };

            return $baseController.http({
                url: "/api/register",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            });


        }

        function _validateForm() {
            var result = true;
            var values = [vm.firstName, vm.lastName, vm.email, vm.password]


            for (var val in values)
                if (vm.validatorRegEx.test(val))
                    result = false;

            $baseController.timeout(_checkUsername(vm.email).then(
                a =>  {
                    result = (a.data.item) ? false : true;
                    vm.usernameExists = a.data.item;
                }
            ));

            $baseController.timeout(_checkEmail(vm.email).then(
                a =>  {
                    result = (a.data.item) ? false : true;
                    vm.emailExists = a.data.item;
                }
            ));

            return result;
        }

        function _checkEmail(email) {

            return $http({
                url: "/api/checkEmail" + "?email=" + email,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });

        }

        function _checkUsername(username) {

            return $http({
                url: "/api/checkUsername" + "?username=" + username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(vm.$uibModalInstance.close, err => $baseController.alert.error(err));

        }


        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }
    }

    function modalLogInController($scope, $baseController, $uibModalInstance) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.submit = _login;
        vm.reset = _setUp;
        vm.cancel = _cancel;

        _render();

        function _render() {
            _setUp(model);
        }

        function _setUp(data) {

            vm.username = null;
            vm.password = null;
        }


        function _login() {

            return $http({
                url: "/api/user" + "?username=" + vm.username + "&password=" + vm.password,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(vm.$uibModalInstance.close, err => $baseController.alert.error(err));

        }


        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }


    }

})();