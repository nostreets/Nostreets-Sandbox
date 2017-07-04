let page = {
    APPNAME: "sandbox",
    ngModules: [
        "ui.bootstrap",
        "ngAnimate",
        "ngRoute",
        "toastr"
    ]
};

(function () {


    angular.module(page.APPNAME, page.ngModules);

    angular.module(page.APPNAME).factory("$baseController", baseController);

    baseController.$inject = ['$document', '$log', '$route', '$routeParams', '$systemEventService', '$alertService', "$window"];

    function baseController($document, $log, $route, $routeParams, $systemEventService, $alertService, $window) {

        let base = {
            document: $document,
            log: $log,
            systemEventService: $systemEventService,
            route: $route,
            routeParams: $routeParams,
            alertService: $alertService,
            window: $window
        }


        return base;
    }
})();