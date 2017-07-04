(function () {

    angular.module(page.APPNAME).factory("$chartsService", chartsService);

    chartsService.$inject = ["$http"];

    function chartsService($http) {
        return {
            getById: _getById,
            getAll: _getAll,
            insert: _insert,
            update: _update,
            deleteById: _deleteById
        };

        function _getById(id, onSuccess, onError)
        {
            $http({
                method: "GET",
                url: "/api/charts/" + id,
            }).then(onSuccess, onError);
        }

        function _getAll(onSuccess, onError)
        {
            $http({
                method: "GET",
                url: "/api/charts/all"
            }).then(onSuccess, onError);
        }

        function _insert(model, onSuccess, onError)
        {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";
            $http({
                method: "POST",
                url: url,
                data: model
            }).then(onSuccess, onError);
        }

        function _update(model, onSuccess, onError)
        {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";
            $http({
                method: "PUT",
                url: url,
                data: model
            }).then(onSuccess, onError);
        }

        function _deleteById(id, onSuccess, onError)
        {
            $http({
                method: "DELETE",
                url: "/api/charts/delete/" + id,
            }).then(onSuccess, onError);
        }
    }

})();