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
        loggedIn: localStorage["nostreetsUsername"] ? true : false,
    }
};

(function () {


    angular.module(page.APPNAME, page.ngModules);

    page.baseController = angular.module(page.APPNAME)
        .factory("$baseController", baseController);

    baseController.$inject = ['$document', '$log', '$route', '$routeParams', '$systemEventService', '$alertService', "$window", '$uibModal', '$timeout', '$http', '$sce', '$cookies'];

    function baseController($document, $log, $route, $routeParams, $systemEventService, $alertService, $window, $uibModal, $timeout, $http, $sce, $cookies) {

        var base = {
            document: $document,
            log: $log,
            systemEventService: $systemEventService,
            route: $route,
            routeParams: $routeParams,
            alertService: $alertService,
            window: $window,
            modal: $uibModal,
            timeout: $timeout,
            http: $http,
            sce: $sce,
            cookies: $cookies
        }

        base.tryAgain = function (maxLoops, miliseconds, promise, method) {

            var rootObj = {};

            rootObj.isSuccessful = false;
            rootObj.method = method;

            _start();

            function _stop() {
                clearInterval(rootObj.funcIntervalId);
                clearInterval(rootObj.counterIntervalId);
            }

            function _start() {
                if (!rootObj.index) { rootObj.index = 0; }
                if (maxLoops > 10) { maxLoops = 9; }

                rootObj.counterIntervalId = (rootObj.counterIntervalId) ? rootObj.counterIntervalId : setInterval(() => { rootObj.index++; }, miliseconds);

                rootObj.funcIntervalId = (rootObj.funcIntervalId) ? rootObj.funcIntervalId :
                    setInterval(() => {
                        //promise.then(() => isSuccessful = true, () => isSuccessful = false);

                        isSuccessful = promise.toObject().isFulfilled();
                        method();

                        if (maxLoops <= rootObj.index) { _stop(); return; }
                        else if (isSuccessful) { _stop(); return; }

                    }, miliseconds);

            }
        }

        base.login = function (username) {
            if (!username) { username = "__refreshUser__" }

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
                return base.login(input).then((data) => {
                    if (base.cookies.get("loggedIn")) {
                        page.loggedIn = true;
                        base.systemEventService.broadcast("refreshedUsername");
                    }
                }).then(onSuccess, onError);
            };

            alert(
                input => brodcastUpdate(input, null,

                    base.tryAgain(2, 3000,
                        brodcastUpdate(input, null, null), brodcastUpdate)),

                () => base.tryAgain(4, 5000,
                    alert(input => brodcastUpdate(input, null,

                        base.tryAgain(2, 3000,
                            brodcastUpdate(input, null, null), brodcastUpdate))
                    ), alert));
        }

        base.errorCheck = function (err, tryAgainObj) {
            if (!tryAgainObj) {
                tryAgainObj = {
                    maxLoops: 0,
                    miliseconds: 10,
                    method: () => { return null; },
                    promise: new Promise((onSuccess, onError) => onSuccess())
                };
            }

            for (var error of err.data.errors) {
                switch (error) {
                    case "User is not logged in...":
                        base.loginPopup();
                        break;

                    default:
                        if (!tryAgainObj || !tryAgainObj.maxLoops || !tryAgainObj.miliseconds || !tryAgainObj.method || !tryAgainObj.predicate) { return; }
                        base.tryAgain(tryAgainObj.maxLoops, tryAgainObj.miliseconds, tryAgainObj.method, tryAgainObj.predicate);
                        break;
                }
            }
        }

        return base;
    }

})();