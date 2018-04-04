(function () {
    angular.module(page.APPNAME)
        .controller("modalRegisterController", modalRegisterController)
        .controller("modalLogInController", modalLogInController)
        .controller("modalUserController", modalUserController)
        .directive("signIn", signInDirective);

    signInDirective.$inject = ["$baseController", "$serverModel"];
    modalLogInController.$inject = ["$scope", "$baseController", "$uibModalInstance", "links"];
    modalRegisterController.$inject = ["$scope", "$baseController", "$uibModalInstance", "links"];
    modalUserController.$inject = ["$scope", "$baseController", "$uibModalInstance", "user"];

    function signInDirective($baseController, $serverModel) {

        return {
            restrict: "A",
            scope: true,
            template: () => ($serverModel.user === null) ? "<i class=\"material-icons\">folder_shared</i>" : "<i class=\"material-icons\">person</i>",
            link: function ($scope, element, attr) {

                $(document).ready(_render);

                function _render() {
                    if ($serverModel.token !== null)
                        swal({
                            title: $serverModel.tokenOutcome,
                            type: "success",
                            showCancelButton: false,
                            showConfirmButton: false,
                            allowOutsideClick: true
                        }).then(() => _openUserModal($serverModel.user), () => _openUserModal($serverModel.user))


                    element.on("click", () => {
                        if ($serverModel.user !== null)
                            _openUserModal($serverModel.user);

                        else if ($serverModel.hasVisited === true)
                            _openLoginModal();

                        else
                            _openRegisterModal();
                    });
                }

                function _openRegisterModal() {

                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/app/templates/registerForm.html"
                        , controller: "modalRegisterController as regVm"
                        , size: "lg"
                        , resolve: {
                            links: () => { return { loginModal: _openLoginModal } }
                        }
                    });

                }

                function _openLoginModal() {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/app/templates/loginForm.html"
                        , controller: "modalLogInController as logVm"
                        , size: "lg"
                        , resolve: {
                            links: () => {
                                return {
                                    registerModal: _openRegisterModal,
                                    userModal: _openUserModal
                                }
                            }
                        }
                    });

                }

                function _openUserModal(user) {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/app/templates/userDashboard.html"
                        , controller: "modalUserController as pg"
                        , size: "lg"
                        , resolve: {
                            user: () => user || $serverModel.user
                        }
                    });

                }
            }
        }
    }

    function modalRegisterController($scope, $baseController, $uibModalInstance, links) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.submit = _submit;
        vm.reset = _setUp;
        vm.cancel = _cancel;
        vm.validateForm = _validateForm;
        vm.checkUsername = _checkUsername;
        vm.checkEmail = _checkEmail;
        vm.login = _openLoginModal;


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

        function _openLoginModal() {
            links.loginModal();
            $uibModalInstance.close();
        }
    }

    function modalLogInController($scope, $baseController, $uibModalInstance, links) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.login = _login;
        vm.reset = _setUp;
        vm.cancel = _cancel;
        vm.signUp = _openRegisterModal;

        _render();

        function _render() {
            _setUp();
        }

        function _setUp() {

            vm.username = null;
            vm.password = null;
            vm.reason = null;
        }

        function _login() {

            if (vm.username && vm.password) {
                return $baseController.http({
                    url: "/api/login",
                    method: "POST",
                    data: {
                        username: vm.username,
                        password: vm.password
                    },
                    headers: { 'Content-Type': 'application/json' }
                }).then(a => _openUserModal(a.data.item),
                    err => { $baseController.alert.error(err.data.errors.message[0]); vm.reason = err.data.errors.message[0]; });

            }


        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _openRegisterModal() {
            links.registerModal();
            $uibModalInstance.close();

        }

        function _openUserModal(user) {
            links.userModal(user);
            $uibModalInstance.close();

        }

        function _forgotPassword() {
            return
            swal({
                title: "Enter your username or email...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: true,
                inputPlaceholder: "Type in your username!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input || input.length < 12)
                            reject("Password is invalid...");
                        else {
                            _validatePassword(input)
                                .then(resolve, () => reject("Password is invalid..."));
                        }
                    });
                }
            }).then(
                $baseController.http({
                    url: "/api/forgotPassword",
                    method: "POST",
                    data: vm.username,
                    headers: { 'Content-Type': 'application/json' }
                }).then(a => _openUserModal(a.data.item),
                    err => { $baseController.alert.error(err.data.errors.message[0]); vm.reason = err.data.errors.message[0]; }));
        }

    }

    function modalUserController($scope, $baseController, $uibModalInstance, user) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.logout = _logout;
        vm.cancel = _cancel;
        vm.toggleEditMode = _toggleEditMode;
        vm.saveChanges = _saveChanges;
        vm.hasUserChanged = _hasUserChanged;

        _render();

        function _render() {

            if (user === null)
                _getUserSession().then(a => _setUp(a.data.item));
            else
                _setUp(user);

        }

        function _setUp(data) {
            vm.user = data;
            vm.userSnap = page.utilities.clone(data);
            vm.isUserChanged = false;
            vm.editMode = false;
        }

        function _hasUserChanged() {
            return !page.utilities.equals(vm.user, vm.userSnap);
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

        function _getUserSession() {
            return $baseController.http({
                url: "/api/user/session",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            })
        }

        function _toggleEditMode() {
            if (!vm.editMode)
                vm.editMode = true;
            else {
                _saveChanges();
            }
        }

        function _passwordLock() {
            return swal({
                title: "Enter your password...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: true,
                inputPlaceholder: "Type in your username!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input || input.length < 12)
                            reject("Password is invalid...");
                        else {
                            _validatePassword(input)
                                .then(resolve, () => reject("Password is invalid..."));
                        }
                    });
                }
            });
        }

        function _updateUser() {
            return $baseController.http({
                url: "/api/user",
                data: vm.user,
                method: "PUT",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _validatePassword(password) {
            return $baseController.http({
                url: "/api/user/validatePassword",
                data: password,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _saveChanges() {
            _passwordLock().then(
                () => {
                    if (vm.user.newPassword && vm.user.newPassword.length > 12)
                        vm.user.password = newPassword;

                    _updateUser();
                    vm.editMode = false;
                });
        }
    }


})();