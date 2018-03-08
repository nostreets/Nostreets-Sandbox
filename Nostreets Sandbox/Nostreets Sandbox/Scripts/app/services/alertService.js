(function () {
    "use strict";

    angular.module(page.APPNAME)
        .factory('$alertService', ServiceFactory);

    ServiceFactory.$inject = ['toastr'];

    function ServiceFactory(toastr) {
        var svc = this;

        svc.toastr = toastr;

        svc.success = _success;
        svc.info = _info;
        svc.error = _error;
        svc.warning = _warning;
        
        function _success(msg, header) {
            svc.toastr.success(msg, header || "Success");
        }

        function _info(msg, header) {
            svc.toastr.info(msg, header || "Info");
        }

        function _error(msg, header) {
            svc.toastr.error(msg, header || "Error");
        }

        function _warning(msg, header) {
            svc.toastr.warning(msg, header || "Warning");
        }

        return svc;
    }

})();