let page = {
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

    page.baseController = angular.module(page.APPNAME).factory("$baseController", baseController);

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


        return base;
    }

})();