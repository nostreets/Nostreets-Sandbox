(function () {
    angular.module(page.APPNAME)
        .controller("modalRegisterController", modalRegisterController)
        .controller("modalLogInController", modalLogInController)
        .directive("signIn", signInDirective);

    signInDirective.$inject = ["$baseController"];
    modalLogInController.$inject = ["$scope", "$baseController", "$uibModalInstance"];
    modalRegisterController.$inject = ["$scope", "$baseController", "$uibModalInstance"];

    function signInDirective($baseController) {

        return {
            restrict: "A",
            scope: true,
            template: (typeof (page.user.data) === "undefined") ? "<i class=\"material-icons\">folder_shared</i>" : "<i class=\"material-icons\">person</i>",
            link: function ($scope, element, attr) {

                $(document).ready(_render);


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
                        , templateUrl: "Scripts/app/templates/registerForm.html"
                        , controller: "modalRegisterController as regVm"
                        , size: "lg"
                    });

                }


                function _openLoginModal() {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/app/templates/loginForm.html"
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
        vm.validateForm = _validateForm;
        vm.checkUsername = _checkUsername;
        vm.checkEmail = _checkEmail;


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
            var result = _checkPassword;
            var values = [vm.firstName, vm.lastName, vm.email, vm.password]

            for (var val in values)
                if (vm.validatorRegEx.test(val))
                    return false;

            return (vm.emailExists || vm.usernameExists) ? false : result;
        }

        function _checkEmail() {

            $baseController.timeout(
                $baseController.http({
                    url: "/api/checkEmail" + "?email=" + vm.email,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                }).then(a => vm.emailExists = a.data.item)
            );
        }

        function _checkUsername() {

            $baseController.timeout(
                $baseController.http({
                    url: "/api/checkUsername" + "?username=" + vm.username,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                }).then(a => vm.usernameExists = a.data.item)
            );

        }

        function _checkPassword() {

            var result = true;
            if (vm.password !== vm.rePassword)
                result = false;
            return result;
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

            return $baseController.http({
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