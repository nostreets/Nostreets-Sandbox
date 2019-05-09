(function () {

    angular.module(page.APPNAME)
        .controller("modalRegisterController", modalRegisterController)
        .controller("modalLogInController", modalLogInController)
        .controller("modalUserController", modalUserController)
        .controller("modalPrivatePolicyController", modalPrivatePolicyController)
        .controller("formLogInController", formLogInController)
        .directive("signIn", nostreetsSignInDirective);


    nostreetsSignInDirective.$inject = ["$baseController", "$serverModel"];
    modalRegisterController.$inject = ["$scope", "$baseController", "$uibModalInstance", "links"];
    modalLogInController.$inject = ["$scope", "$baseController", "$uibModalInstance", "links"];
    modalUserController.$inject = ["$scope", "$baseController", "$uibModalInstance", "user"];
    modalPrivatePolicyController.$inject = ["$scope", "$uibModalInstance"];
    formLogInController.$inject = ["$scope", "$baseController"];


    function nostreetsSignInDirective($baseController, $serverModel) {

        return {
            restrict: "A",
            scope: {
                elementTag: '@',
                isLoggedIn: '@',
                onLogIn: '&',
                onLogOut: '&'
            },
            templateUrl: "/Scripts/shared/templates/userEntry.html",
            link: function ($scope, element, attr) {

                $(document).ready(_render);

                function _render() {

                    page.isLoggedIn = $serverModel.user && $serverModel.user !== null ? true : false;
                    $scope.isLoggedIn = page.isLoggedIn;

                    if ($serverModel.tokenOutcome && $serverModel.tokenOutcome !== null)
                        swal({
                            title: $serverModel.tokenOutcome,
                            type: $serverModel.state.toLowerCase(),
                            showCancelButton: false,
                            showConfirmButton: false,
                            allowOutsideClick: true
                        });

                    if (page.isLoggedIn)
                        page.siteOptions.billManagerChartType = $serverModel.user.settings.portfolio.chartLibary;


                    _handlers();
                }

                function _openRegisterModal() {

                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/shared/templates/modals/registerForm.html"
                        , controller: "modalRegisterController as regVm"
                        , size: "lg"
                        , resolve: {
                            links: () => { return { loginModal: _openLoginModal }; }
                        }
                    });

                    modalInstance.rendered.then(() => {
                        componentHandler.upgradeAllRegistered();
                    });

                }

                function _openLoginModal() {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/shared/templates/modals/loginForm.html"
                        , controller: "modalLogInController as logVm"
                        , size: "lg"
                        , resolve: {
                            links: () => {
                                return {
                                    registerModal: _openRegisterModal,
                                    userModal: _openUserModal,
                                    onLogIn: $scope.onLogIn
                                };
                            }
                        }
                    });

                    modalInstance.rendered.then(() => {
                        componentHandler.upgradeAllRegistered();
                    });
                }

                function _openUserModal(user) {
                    var modalInstance = $baseController.modal.open({
                        animation: true
                        , templateUrl: "Scripts/shared/templates/modals/userDashboard.html"
                        , controller: "modalUserController as pg"
                        , size: "lg"
                        , resolve: {
                            user: () => user || $serverModel.user
                        }
                    });

                    modalInstance.rendered.then(() => {
                        componentHandler.upgradeAllRegistered();
                    });
                }

                function _handlers() {
                    $baseController.event.listen("loggedOut", () => {
                        page.isLoggedIn = false;
                        $scope.isLoggedIn = false;
                        if ($scope.onLogOut && typeof ($scope.onLogOut) === "function")
                            $scope.onLogOut();
                    });
                    $baseController.event.listen("loggedIn", () => {
                        page.isLoggedIn = true;
                        $scope.isLoggedIn = true;
                        if ($scope.onLogIn && typeof ($scope.onLogIn) === "function")
                            $scope.onLogIn();
                    });


                    if ($scope.elementTag !== "form") {
                        element.on("click", () => {
                            if (page.isLoggedIn)
                                _openUserModal($serverModel.user || null);

                            else if ($serverModel.hasVisited === true)
                                _openLoginModal();

                            else
                                _openRegisterModal();
                        });
                    }

                }

            }
        };
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
        vm.privatePolicy = _openPrivatePolicyModal;


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
            vm.isLoading = false;
            vm.requestSuccessful = null;
        }

        function _submit() {

            vm.isLoading = true;
            var obj = {
                username: vm.username,
                password: vm.password,
                contact: {
                    primaryEmail: vm.email,
                    firstName: vm.firstName,
                    lastName: vm.lastName
                },
                userOrigin: 1
            };


            $baseController.http({
                url: "/api/register",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                (a) => {
                    vm.requestSuccessful = true;
                    vm.isLoading = false;
                },
                (e) => {
                    vm.requestSuccessful = false;
                    vm.isLoading = false;
                });
        }

        function _validateForm() {
            var values = [vm.firstName, vm.lastName, vm.email, vm.password, vm.username];

            for (var val of values)
                if (vm.validatorRegEx.test(val))
                    return false;

            return (vm.emailExists || vm.usernameExists) ? false : _checkPassword();
        }

        function _checkEmail() {

            if (vm.email !== null)
                $baseController.timeout(
                    $baseController.http({
                        url: "/api/checkEmail" + "?email=" + vm.email,
                        method: "GET",
                        headers: { 'Content-Type': 'application/json' }
                    }).then(a => vm.emailExists = a.data.item)
                );
        }

        function _checkUsername() {

            if (vm.username !== null)
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
            if (links && links.loginModal)
                links.loginModal();
            $uibModalInstance.close();
        }

        function _openPrivatePolicyModal() {

            var modalInstance = $baseController.modal.open({
                animation: true
                , templateUrl: "Scripts/app/templates/privatePolicy.html"
                , controller: "modalPrivatePolicyController as ppVm"
                , size: "lg"
            });

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
        vm.forgotPassword = _forgotPassword;
        vm.resendEmailValidation = _resendEmailValidation;

        _render();

        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {

            vm.registered = false;
            vm.username = null;
            vm.password = null;
            vm.reason = null;
            vm.resentEmail = false;
            vm.isLoading = false;
            vm.rememberMe = true;
            vm.googleClientId = "545466753618-ap1q43tsr3omjeh0huefc5qg8l3kgk3o.apps.googleusercontent.com";
            vm.instagramClientId = "fb198f3bbf4744bd89e066fd6eeaeb2e";
            vm.fbAppId = 1903266009978931;

        }

        function _handlers() {

            vm.googleSignInOnSuccess = (user) => {
                vm.email = user.getEmail();
                vm.username = user.getEmail();
                vm.password = user.getId();
                vm.firstName = user.getGivenName();
                vm.lastName = user.getFamilyName();
                vm.userOrigin = 2;

                _externalSignIn();
            };

            vm.facebookSignInOnSuccess = (user) => {
                vm.email = user.email;
                vm.username = user.email;
                vm.password = user.id;
                vm.firstName = user.first_name;
                vm.lastName = user.leat_name;
                vm.userOrigin = 3;

                _externalSignIn();
            };

            vm.instagramSignInOnSuccess = (user) => {
                vm.email = user.email;
                vm.username = user.username;
                vm.password = user.id;
                vm.firstName = user.full_name.split(" ")[0];
                vm.lastName = user.full_name.split(" ").length === 2 ? user.full_name.split(" ")[1] : '';
                vm.userOrigin = 5;

                _externalSignIn();
            };
        }

        function _externalSignIn(user) {

            _checkEmail(vm.email).then((a) => {
                if (a.data.item === false) {
                    var obj = {
                        username: vm.email,
                        password: vm.password,
                        userOrigin: vm.userOrigin,
                        contact: {
                            primaryEmail: vm.email,
                            firstName: vm.firstName,
                            lastName: vm.lastName
                        }
                    };

                    _register(obj);
                }
                else
                    _login();

            });
        }

        function _register(obj) {

            vm.isLoading = true;

            $baseController.http({
                url: "/api/register",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                (a) => {
                    vm.isLoading = false;
                });

            vm.registered = true;

        }

        function _login() {

            if (vm.username && vm.password) {

                vm.reason = null;
                vm.resentEmail = false;
                vm.isLoading = true;

                return $baseController.http({
                    url: "/api/login?rememberDevice=" + (vm.rememberMe ? 'true' : 'false'),
                    method: "POST",
                    data: {
                        username: vm.username,
                        password: vm.password
                    },
                    headers: { 'Content-Type': 'application/json' }
                }).then(
                    a => {

                        if (a.data.item.userName) {
                            vm.isLoading = false;
                            $baseController.event.broadcast('loggedIn');
                            _openUserModal(a.data.item);

                        }
                        else
                            _tfAuthLock(a.data.item).then(
                                b => {
                                    vm.isLoading = false;
                                    $baseController.event.broadcast('loggedIn');
                                    _openUserModal(b.data.item);
                                });

                    },
                    err => {
                        if (err.data.errors) {
                            vm.isLoading = false;
                            vm.reason = err.data.errors.message[0];
                        }
                        else {
                            vm.isLoading = false;
                            vm.reason = err.data.message;
                        }
                    });

            }
        }

        function _tfAuthLock(id) {
            return swal({
                title: "Enter the Validation Code...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: false,
                inputPlaceholder: "Type in the validation code!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input)
                            reject("Code is invalid...");
                        else
                            _valaidateTFAuth(id, input).then(resolve, () => reject("Code is invalid..."));
                    });
                }
            });
        }

        function _valaidateTFAuth(id, input) {
            return $baseController.http({
                url: "/api/user/tfauth?id=" + id + "&code=" + input,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _openRegisterModal() {
            if (links && links.registerModal)
                links.registerModal();
            $uibModalInstance.close();

        }

        function _openUserModal(user) {
            if (links && links.userModal)
                links.userModal(user);
            $uibModalInstance.close();

        }

        function _forgotPassword() {
            return swal({
                title: "Enter your username or email...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: true,
                inputPlaceholder: "Type in your username or email!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input || input.length < 6)
                            reject("Username / Email is invalid...");
                        else {
                            _checkUsername(input).then(
                                a => {
                                    if (a.data.item === false)
                                        _checkEmail(input).then(
                                            b => {
                                                if (b.data.item === false)
                                                    reject("Username / Email does not exist...");
                                                else
                                                    resolve(input);
                                            }
                                        );
                                    else
                                        resolve(input);
                                }
                            );
                        }
                    });
                }
            }).then(
                a => $baseController.http({
                    url: "/api/user/forgotPassword?username=" + a,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                }).then(
                    b => { vm.resentEmail = true; vm.reason = ""; },
                    err => {
                        if (err.data.errors) {
                            vm.reason = err.data.errors.message[0];
                        }
                        else {
                            vm.reason = err.data.message;
                        }
                    })
            );
        }

        function _checkEmail(email) {

            return $baseController.http({
                url: "/api/checkEmail" + "?email=" + email,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            })
        }

        function _checkUsername(username) {

            return $baseController.http({
                url: "/api/checkUsername" + "?username=" + username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _resendEmailValidation() {

            $baseController.http({
                url: "/api/user/resendValidationEmail" + "?username=" + vm.username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(a => { vm.resentEmail = true; vm.reason = ""; });
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
        vm.isLockedOut = _isLockedOut;
        vm.toggleTFAuth = _toggleTFAuth;
        vm.resendEmailValidation = _resendEmailValidation;


        _render();

        function _render() {

            if (user === null)
                _getUserSession().then(a => _setUp(a.data.item));
            else
                _setUp(user);

            _getEnums("chartLibrary");

        }

        function _setUp(data) {

            vm.user = data;
            vm.isUserChanged = false;
            vm.editMode = false;
            vm.isLoading = false;
            vm.user.newPassword = '            ';
            vm.userSnap = page.utilities.clone(vm.user);


            page.siteOptions.billManagerChartType = vm.user.settings.portfolio.chartLibary;

        }

        function _hasUserChanged() {
            return !page.utilities.equals(vm.user, vm.userSnap);
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _toggleEditMode() {
            if (!vm.editMode)
                vm.editMode = true;

            else {
                vm.user = vm.userSnap;
                vm.editMode = false;
            }
        }

        function _passwordLock() {
            return swal({
                title: "Enter your password...",
                type: "info",
                input: "password",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: false,
                inputPlaceholder: "Type in your password!",
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

        function _saveChanges() {

            if (vm.user.userOrigin === 1)
                _passwordLock().then(
                    () => {
                        if (vm.user.newPassword && vm.user.newPassword.length >= 12 && vm.user.newPassword !== vm.userSnap.newPassword) {
                            vm.user.password = vm.user.newPassword;

                            _updateUser().then(_refreshUser);

                            vm.editMode = false;
                        }
                        else {
                            swal({
                                title: "Password is Invalid!",
                                type: "error",
                                showCancelButton: false,
                                showConfirmButton: false,
                                allowOutsideClick: true
                            });
                        }
                    });
            else {
                _updateUser().then(_refreshUser);
                vm.editMode = false;
            }
        }

        function _isLockedOut() {
            var result = null;
            if (!vm.user.settings.hasVaildatedEmail)
                result = "User hasn't validated their email... Maybe it's in the Junk Folder.";
            else if (vm.user.settings.isLockedOut)
                result = "User is locked out...";
            else
                result = "false";

            return result;
        }

        function _toggleTFAuth(type) {
            switch (type) {
                case "email":
                    vm.user.settings.tfAuthByEmail = true;
                    vm.user.settings.tfAuthByPhone = false;
                    break;

                case "phone":
                    if (vm.user.contact.primaryPhone) {
                        vm.user.settings.tfAuthByPhone = true;
                        vm.user.settings.tfAuthByEmail = false;
                    }
                    break;
            }
        }

        function _refreshUser() {
            return _getUserSession().then(a => _setUp(a.data.item));
        }

        //#region API CALLS

        function _getEnums(enumType) {
            return $baseController.http({
                url: "/api/config/enums/" + (Array.isArray(enumType) ? enumType.join(",") : enumType),
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                a => { vm.enums = a.data.items; }
            );
        }

        function _resendEmailValidation() {

            $baseController.http({
                url: "/api/user/resendValidationEmail" + "?username=" + vm.username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(a => { vm.resentEmail = true; vm.reason = ""; });
        }

        function _validatePassword(password) {
            return $baseController.http({
                url: "/api/user/validatePassword?password=" + password,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _updateUser() {

            vm.isLoading = true;

            return $baseController.http({
                url: "/api/user",
                data: vm.user,
                method: "PUT",
                headers: { 'Content-Type': 'application/json' }
            }).then(() => {
                vm.isLoading = false;
                vm.userSnap = page.utilities.clone(vm.user);
            });
        }

        function _getUserSession() {

            vm.isLoading = true;

            return $baseController.http({
                url: "/api/user/session",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(a => { vm.isLoading = false; });
        }

        function _logout() {

            vm.isLoading = true;

            return $baseController.http({
                url: "/api/logout",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                () => {
                    vm.isLoading = false;
                    $baseController.event.broadcast('loggedOut');
                    vm.$uibModalInstance.close();
                },
                err => {
                    vm.isLoading = false;
                    $baseController.alert.error(err);
                }
            );

        }



        //#endregion

    }

    function modalPrivatePolicyController($scope, $uibModalInstance) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.close = $uibModalInstance.close;

        _render();

        function _render() {
            _setUp();
        }

        function _setUp() { }

    }

    function formLogInController($scope, $baseController) {

        var vm = this;
        vm.$scope = $scope;
        vm.login = _login;
        vm.reset = _setUp;
        vm.cancel = _cancel;
        vm.signUp = _openRegisterModal;
        vm.forgotPassword = _forgotPassword;
        vm.resendEmailValidation = _resendEmailValidation;

        _render();

        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {

            vm.registered = false;
            vm.username = null;
            vm.password = null;
            vm.reason = null;
            vm.resentEmail = false;
            vm.isLoading = false;
            vm.rememberMe = true;
            vm.googleClientId = "545466753618-ap1q43tsr3omjeh0huefc5qg8l3kgk3o.apps.googleusercontent.com";
            vm.instagramClientId = "fb198f3bbf4744bd89e066fd6eeaeb2e";
            vm.facebookAppId = 1903266009978931;
            vm.isLoggedIn = $scope.$parent.isLoggedIn || false;
            vm.elementTag = $scope.$parent.elementTag || 'button';
        }

        function _handlers() {

            vm.googleSignInOnSuccess = (user) => {
                vm.email = user.getEmail();
                vm.username = user.getEmail();
                vm.password = user.getId();
                vm.firstName = user.getGivenName();
                vm.lastName = user.getFamilyName();
                vm.userOrigin = 2;

                _externalSignIn();
            };

            vm.facebookSignInOnSuccess = (user) => {
                vm.email = user.email;
                vm.username = user.email;
                vm.password = user.id;
                vm.firstName = user.first_name;
                vm.lastName = user.leat_name;
                vm.userOrigin = 3;

                _externalSignIn();
            };

            vm.instagramSignInOnSuccess = (user) => {
                vm.email = user.email;
                vm.username = user.username;
                vm.password = user.id;
                vm.firstName = user.full_name.split(" ")[0];
                vm.lastName = user.full_name.split(" ").length === 2 ? user.full_name.split(" ")[1] : '';
                vm.userOrigin = 5;

                _externalSignIn();
            };
        }

        function _externalSignIn() {

            _checkEmail(vm.email).then((a) => {
                if (a.data.item === false) {
                    var obj = {
                        username: vm.email,
                        password: vm.password,
                        userOrigin: vm.userOrigin,
                        contact: {
                            primaryEmail: vm.email,
                            firstName: vm.firstName,
                            lastName: vm.lastName
                        }
                    };

                    _register(obj);
                }
                else
                    _login();

            });
        }

        function _register(obj) {

            vm.isLoading = true;

            $baseController.http({
                url: "/api/register",
                data: obj,
                method: "POST",
                headers: { 'Content-Type': 'application/json' }
            }).then(
                (a) => {
                    vm.isLoading = false;
                });

            vm.registered = true;

        }

        function _login() {

            if (vm.username && vm.password) {

                vm.reason = null;
                vm.resentEmail = false;
                vm.isLoading = true;

                return $baseController.http({
                    url: "/api/login?rememberDevice=" + (vm.rememberMe ? 'true' : 'false'),
                    method: "POST",
                    data: {
                        username: vm.username,
                        password: vm.password
                    },
                    headers: { 'Content-Type': 'application/json' }
                }).then(
                    a => {

                        if (a.data.item.userName) {
                            vm.isLoading = false;
                            $baseController.event.broadcast('loggedIn');
                            if (onLogIn)
                                _openUserModal(a.data.item);

                        }
                        else
                            _tfAuthLock(a.data.item).then(
                                b => {
                                    vm.isLoading = false;
                                    $baseController.event.broadcast('loggedIn');
                                    _openUserModal(b.data.item);
                                });

                    },
                    err => {
                        if (err.data.errors) {
                            vm.isLoading = false;
                            vm.reason = err.data.errors.message[0];
                        }
                        else {
                            vm.isLoading = false;
                            vm.reason = err.data.message;
                        }
                    });

            }
        }

        function _tfAuthLock(id) {
            return swal({
                title: "Enter the Validation Code...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: false,
                inputPlaceholder: "Type in the validation code!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input)
                            reject("Code is invalid...");
                        else
                            _valaidateTFAuth(id, input).then(resolve, () => reject("Code is invalid..."));
                    });
                }
            });
        }

        function _valaidateTFAuth(id, input) {
            return $baseController.http({
                url: "/api/user/tfauth?id=" + id + "&code=" + input,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _cancel() {
            vm.$uibModalInstance.dismiss("cancel");
        }

        function _forgotPassword() {
            return swal({
                title: "Enter your username or email...",
                type: "info",
                input: "text",
                showCancelButton: true,
                closeOnConfirm: true,
                allowOutsideClick: true,
                inputPlaceholder: "Type in your username or email!",
                preConfirm: (input) => {
                    return new Promise(function (resolve, reject) {
                        if (!input || input.length < 6)
                            reject("Username / Email is invalid...");
                        else {
                            _checkUsername(input).then(
                                a => {
                                    if (a.data.item === false)
                                        _checkEmail(input).then(
                                            b => {
                                                if (b.data.item === false)
                                                    reject("Username / Email does not exist...");
                                                else
                                                    resolve(input);
                                            }
                                        );
                                    else
                                        resolve(input);
                                }
                            );
                        }
                    });
                }
            }).then(
                a => $baseController.http({
                    url: "/api/user/forgotPassword?username=" + a,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                }).then(
                    b => { vm.resentEmail = true; vm.reason = ""; },
                    err => {
                        if (err.data.errors) {
                            vm.reason = err.data.errors.message[0];
                        }
                        else {
                            vm.reason = err.data.message;
                        }
                    })
            );
        }

        function _checkEmail(email) {

            return $baseController.http({
                url: "/api/checkEmail" + "?email=" + email,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            })
        }

        function _checkUsername(username) {

            return $baseController.http({
                url: "/api/checkUsername" + "?username=" + username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _resendEmailValidation() {

            $baseController.http({
                url: "/api/user/resendValidationEmail" + "?username=" + vm.username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(a => { vm.resentEmail = true; vm.reason = ""; });
        }

        function _openRegisterModal() {

            var modalInstance = $baseController.modal.open({
                animation: true
                , templateUrl: "Scripts/shared/templates/modals/registerForm.html"
                , controller: "modalRegisterController as regVm"
                , size: "lg"
                , resolve: {
                    links: () => { return null; }
                }
            });

            modalInstance.rendered.then(() => {
                componentHandler.upgradeAllRegistered();
            });

        }

        function _openUserModal(user) {
            var modalInstance = $baseController.modal.open({
                animation: true
                , templateUrl: "Scripts/shared/templates/modals/userDashboard.html"
                , controller: "modalUserController as pg"
                , size: "lg"
                , resolve: {
                    links: () => { return null; }
                }

            });

            modalInstance.rendered.then(() => {
                componentHandler.upgradeAllRegistered();
            });


        }
    }

})();