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

    page.baseController = angular.module(page.APPNAME).factory("$baseController", baseController);

    baseController.$inject = ['$document', '$log', '$route', '$routeParams', '$systemEventService', '$alertService', "$window", '$uibModal', '$timeout'];

    function baseController($document, $log, $route, $routeParams, $systemEventService, $alertService, $window, $uibModal, $timeout) {

        var base = {
            document: $document,
            log: $log,
            systemEventService: $systemEventService,
            route: $route,
            routeParams: $routeParams,
            alertService: $alertService,
            window: $window,
            modal: $uibModal,
            timeout: $timeout
        }


        return base;
    }

})();