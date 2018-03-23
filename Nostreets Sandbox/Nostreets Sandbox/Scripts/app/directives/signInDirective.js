(function () {
    angular.module(page.APPNAME)
        .controller("modalRegisterController", modalRegisterController)
        .controller("modalLogInController", modalLogInController)
        .directive("signIn", signInDirective);

    signInDirective.$inject = ["$baseController, $serverModel"];
    modalLogInController.$inject = ["$scope", "$baseController", "$uibModalInstance"];
    modalRegisterController.$inject = ["$scope", "$baseController", "$uibModalInstance"];

    function signInDirective($baseController, $serverModel) {

        return {
            restrict: "A",
            scope: true,
            template: (typeof (page.user.data) === "undefined") ? "<i class=\"material-icons\">folder_shared</i>" : "<i class=\"material-icons\">person</i>",
            link: function ($scope, element, attr) {

                $(document).ready(_render);


                function _render() {
                    element.on("click", () => {
                        if (typeof (page.user.data) === "undefined")
                            _openRegisterModal();
                        else
                            if (page.user.loggedIn)
                                _openUserModal(page.user.data);
                            else
                                _openLoginModal();
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

                function _openUserModal() {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/app/templates/userDashboard.html"
                        , controller: "modalUserController as pg"
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
            vm.validatorRegEx = /<[a-zA-Z][\s\S]*>/;
            vm.emailExists = false;
            vm.usernameExists = false;
            vm.usernameExists = false;
            vm.requestSuccessful = false;
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

            $baseController.http({
                url: "/api/register",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                (a) => { vm.requestSuccessful = true; }
                );


        }

        function _validateForm() {
            var values = [vm.firstName, vm.lastName, vm.email, vm.password, vm.username]

            for (var val of values)
                if (vm.validatorRegEx.test(val))
                    return false;

            return (vm.emailExists || vm.usernameExists) ? false : _checkPassword();
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
            return (!vm.password || !vm.rePassword) ? false : (vm.password === vm.rePassword) ? true : false;
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
                url: "/api/login" + "?username=" + vm.username + "&password=" + vm.password,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(vm.$uibModalInstance.close, err => $baseController.alert.error(err));

        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }


    }

    function modalUserController($scope, $baseController, $uibModalInstance, user) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.logout = _logout;
        vm.cancel = _cancel;

        _render();

        function _render() {
            _getUserData().then(a => _setUp(a.data.item));
        }

        function _setUp(data) {

            vm.username = null;
            vm.password = null;
        }

        function _logout() {

            return $baseController.http({
                url: "/api/logout",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(vm.$uibModalInstance.close, err => $baseController.alert.error(err));

        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _getUserData() {
            return $baseController.http({
                url: "/api/user/session",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            }).then(a => a.data.item);
        }


    }


})();