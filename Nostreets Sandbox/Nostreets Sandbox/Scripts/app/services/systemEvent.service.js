(function () {
    "use strict";

    angular.module(page.APPNAME)
        .factory('$systemEventService', EventServiceFactory);

    EventServiceFactory.$inject = ['$rootScope'];

    function EventServiceFactory($rootScope) {
        var svc = this;

        svc.$rootScope = $rootScope;

        svc.listen = _listen;
        svc.broadcast = _broadcast;

        svc.$rootScope.$on('$locationChangeStart', svc.broadcast);

        function _listen(event, callback, controllerInstance) {
            if (controllerInstance) {

                controllerInstance.$scope.$on(event, callback);

            }
            else {
                svc.$rootScope.$on(event, callback);
            }

        }

        function _broadcast() {
            svc.$rootScope.$broadcast(arguments[0], arguments);
        }

        return svc;
    }

})();