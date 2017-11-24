var page = {
    APPNAME: "sandbox",
    ngModules: [
        "ui.bootstrap",
        "ngAnimate",
        "ngRoute",
        "toastr",
        "ngSanitize",
        "ngCookies"
    ],
    user: {
        loggedIn: false,
    },
    isPoppedUp: {

    },
    utilities: {

        getStyle: (id) => {
            return document.getElementById(id).style;
        },

        writeStyles: (styleName, cssRules) => {
            var styleElement = document.getElementById(styleName);
            var pastCssRules = styleElement.textContent;

            if (styleElement) {
                document.getElementsByTagName('head')[0].removeChild(
                    styleElement);
            }

            styleElement = document.createElement('style');
            styleElement.type = 'text/css';
            styleElement.id = styleName;


            if (cssRules.length) {
                for (var css of cssRules) {
                    styleElement.appendChild(document.createTextNode(css));
                }
            }
            else {
                styleElement.innerHTML = cssText;
            }

            document.getElementsByTagName('head')[0].appendChild(styleElement);
        }

    }
};

(function () {


    angular.module(page.APPNAME, page.ngModules);

    page.baseController = angular.module(page.APPNAME)
        .factory("$baseController", baseController);

    baseController.$inject = ['$document', '$route', '$routeParams', '$systemEventService', '$alertService', "$window", '$uibModal', '$timeout', '$http', '$sce', '$cookies', '$q'];

    function baseController($document, $route, $routeParams, $systemEventService, $alertService, $window, $uibModal, $timeout, $http, $sce, $cookies, $q) {

        var base = {
            document: $document,
            systemEventService: $systemEventService,
            route: $route,
            routeParams: $routeParams,
            alertService: $alertService,
            window: $window,
            modal: $uibModal,
            timeout: $timeout,
            http: $http,
            sce: $sce,
            cookies: $cookies,
            Q: $q
        }

        base.tryAgain = function (maxLoops, miliseconds, promiseMethod, onSuccess) {

            if (onSuccess === null) { onSuccess = (data) => console.log(data); }
            var root = {};

            root.method = promiseMethod;
            root.currentIndex = 0;

            _start();

            function _start() {
                var method = () => _repeatUntilSuccessful(root.method(), miliseconds, maxLoops, data => onSuccess(data));
                base.timeout(method, miliseconds);
            }

            function _repeatUntilSuccessful(promise, time, maxLoops, callback = null) {

                function delay(time, val) {
                    return new Promise(function (resolve) {
                        setTimeout(function () {
                            resolve(val);
                        }, time);
                    });
                }

                function success(data) {
                    if (callback !== null) {
                        callback(data);
                    }
                    return;
                }

                function error(data) {
                    if (root.currentIndex >= maxLoops) { return; }
                    else {
                        return delay(time).then(
                            () => { root.currentIndex++; _repeatUntilSuccessful(root.method(), time, maxLoops, callback); },
                            () => { root.currentIndex++; _repeatUntilSuccessful(root.method(), time, maxLoops, callback); }
                        );
                    }
                }

                base.Q.when(promise, success, error);

            }

        }

        base.login = function (username) {
            if (!username) { username = "GUEST" }

            return $http({
                url: "/api/user?username=" + username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        base.loginPopup = function () {

            var alert = (onSuccess, onError) => {
                return swal({
                    title: "Enter your session's username",
                    type: "info",
                    input: "text",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    allowOutsideClick: true,
                    inputPlaceholder: "Type in your username!",
                    preConfirm: function (inputValue) {
                        return new Promise(function (resolve, reject) {
                            if (inputValue === false || inputValue === "") {
                                reject("You need to write something!");
                            }
                            else {
                                resolve();
                            }
                        });
                    }
                }).then(onSuccess, onError);
            };

            var brodcastUpdate = (input, onSuccess, onError) => {
                return base.login(input).then(
                    (data) => {
                        if (base.cookies.get("loggedIn")) {
                            page.user.loggedIn = true;
                            base.systemEventService.broadcast("refreshedUsername");
                        }
                    }).then(onSuccess, onError);
            };


            if (!base.hasPopup) {

                base.hasPopup = true;
                alert(

                    input => brodcastUpdate(input, null,
                        () => base.tryAgain(2, 3000, () => brodcastUpdate(input, null, null), (i) => { base.hasPopup = false; })
                    ),


                    () => base.tryAgain(4, 5000,
                        () => alert(input => brodcastUpdate(input, null,
                            () => base.tryAgain(2, 3000, () => brodcastUpdate(input, null, null), (i) => { base.hasPopup = false; })
                        ))
                    )
                );
            }
        }

        base.errorCheck = function (err, tryAgainObj) {

            if (!tryAgainObj) {

                tryAgainObj = {};
            }
            if (!tryAgainObj.maxLoops) {
                tryAgainObj.maxLoops = 1;
            }
            if (!tryAgainObj.miliseconds) {
                tryAgainObj.miliseconds = 100;
            }
            if (!tryAgainObj.method) {

                tryAgainObj.method = () => {
                    return new Promise((resolve, reject) => {
                        resolve();
                    });
                }

            }
            if (!tryAgainObj.onSuccess) {
                tryAgainObj.onSuccess = (data) => console.log(data);
            }

            if (err.data.errors && err.data.errors.length) {
                for (var error of err.data.errors) {
                    switch (error) {
                        case "User is not logged in...":
                            base.loginPopup();
                            break;

                        default:
                            if (!tryAgainObj || !tryAgainObj.maxLoops || !tryAgainObj.miliseconds || !tryAgainObj.method) { return; }
                            base.tryAgain(tryAgainObj.maxLoops, tryAgainObj.miliseconds, tryAgainObj.method, tryAgainObj.onSuccess);
                            break;
                    }
                }
            }
            else {
                switch (err.status) {
                    case 401:
                        base.loginPopup();
                        break;

                    default:
                        if (!tryAgainObj || !tryAgainObj.maxLoops || !tryAgainObj.miliseconds || !tryAgainObj.method) { return; }
                        base.tryAgain(tryAgainObj.maxLoops, tryAgainObj.miliseconds, tryAgainObj.method, tryAgainObj.onSuccess);
                        break;
                }
            }
        }

        return base;
    }

})();
