var page = {
    APPNAME: "sandbox",
    ngModules: [
        "ui.bootstrap",
        "ngAnimate",
        "ngRoute",
        "toastr",
        "ngSanitize"
    ],
    user: {
        signedIn: localStorage["nostreetsUsername"] ? true : false,
        username: localStorage["nostreetsUsername"] ? localStorage["nostreetsUsername"] : null
    }
};

(function () {


    angular.module(page.APPNAME, page.ngModules);

    page.baseController = angular.module(page.APPNAME)
        .factory("$baseController", baseController)
        .directive("changeUsername", changeUsernanmeDirective);

    baseController.$inject = ['$document', '$log', '$route', '$routeParams', '$systemEventService', '$alertService', "$window", '$uibModal', '$timeout', '$http', '$sce'];

    function baseController($document, $log, $route, $routeParams, $systemEventService, $alertService, $window, $uibModal, $timeout, $http, $sce) {

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
            sce: $sce
        }

        base.tryAgain = function (numberOfTimesLooped, miliseconds, func, predicate) {
            if (numberOfTimesLooped > 10) { numberOfTimesLooped = 9; }

            this.stop = function () {
                clearInterval(funcIntervalId);
                clearInterval(counterIntervalId);
            }

            var index = 0;

            var funcIntervalId = setInterval(() => {
                func();
                if (numberOfTimesLooped < index) { this.stop(); }
                else if (predicate()) { this.stop(); }
            }, miliseconds);

            var counterIntervalId = setInterval(() => { index++; console.log(index); }, miliseconds);
        }

        return base;
    }

    function changeUsernanmeDirective($baseController) {

        return {
            restrict: "A",
            scope: true,
            link: function ($scope, element, attr) {

                $(document).ready(_startUp);

                function _startUp() {

                    element.on("click", function () {
                        swal({
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
                        }).then(function (input) {
                            $baseController.http({
                                url: "/api/user/" + input,
                                method: "GET",
                                headers: { 'Content-Type': 'application/json' }
                            }).then(function (data) {
                                page.user.signedIn = true;
                                page.user.username = input;
                                localStorage["nostreetsUsername"] = input;
                                $baseController.systemEventService.broadcast("refreshedUsername");
                            });
                        });

                    });

                }



            }
        }
    }


})();