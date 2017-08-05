(function () {

    angular.module(page.APPNAME).factory("$sandboxService", sandboxService);

    sandboxService.$inject = ["$http"];

    function sandboxService($http) {
        return {
            loginUser: _loginUser,
            getChartById: _getChartById,
            getAllCharts: _getAllCharts,
            getAllChartsByUser: _getAllChartsByUser,
            insertChart: _insertChart,
            updateChart: _updateChart,
            deleteChartById: _deleteChartById,
            insertCard: _insertCard,
            getAllCards: _getAllCards,
            getAllCardsByUser: _getAllCardsByUser,
            deleteCard: _deleteCard,
            updateCard: _updateCard
        };

        function _loginUser(username) {
            return $http({
                url: "/api/user/" + username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getChartById(id, onSuccess, onError) {
            $http({
                method: "GET",
                url: "/api/charts/" + id,
            }).then(onSuccess, onError);
        }

        function _getAllCharts(onSuccess, onError) {
            $http({
                method: "GET",
                url: "/api/charts/all"
            }).then(onSuccess, onError);
        }

        function _getAllChartsByUser(onSuccess, OnError) {
            $http({
                url: "/api/charts/user/" + page.user.username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(onSuccess, OnError);
        }

        function _insertChart(model, onSuccess, onError) {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";

            $http({
                method: "POST",
                url: url += "?username=" + page.user.username,
                data: model
            }).then(onSuccess, onError);
        }

        function _updateChart(model, onSuccess, onError) {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";
            $http({
                method: "PUT",
                url: url += "?username=" + page.user.username,
                data: model
            }).then(onSuccess, onError);
        }

        function _deleteChartById(id, onSuccess, onError) {
            $http({
                method: "DELETE",
                url: "/api/charts/delete/" + id,
            }).then(onSuccess, onError);
        }

        function _insertCard(model) {
            return $http({
                url: "/api/cards?username=" + page.user.username,
                method: "POST",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getAllCards(onSuccess, OnError) {
            $http({
                url: "/api/cards/all",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(onSuccess, OnError);
        }

        function _getAllCardsByUser(onSuccess, OnError) {
            $http({
                url: "/api/cards/user/" + page.user.username,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            }).then(onSuccess, OnError);
        }

        function _deleteCard(id, onSuccess, OnError) {
            $http({
                url: "/api/cards/delete/" + id,
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            }).then(onSuccess, OnError);
        }

        function _updateCard(model, onSuccess, OnError) {
            $http({
                url: "/api/cards?username=" + page.user.username,
                method: "PUT",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            }).then(onSuccess, OnError);
        }
    }

})();