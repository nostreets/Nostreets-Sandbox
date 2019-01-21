(function () {

    angular.module(page.APPNAME).factory("$oblApiService", sandboxService);

    sandboxService.$inject = ["$http"];

    function sandboxService($http) {
        return {
            validateUrl: _validateUrl,
            getEnums: _getEnums,

            getChartById: _getChartById,
            getAllChartsByUser: _getAllChartsByUser,
            insertChart: _insertChart,
            updateChart: _updateChart,
            deleteChartById: _deleteChartById,

            insertCard: _insertCard,
            getAllCards: _getAllCards,
            getAllCardsByUser: _getAllCardsByUser,
            deleteCard: _deleteCard,
            updateCard: _updateCard,


            getIncomesChart: _getIncomesChart,
            getExpensesChart: _getExpenseChart,
            getCombinedChart: _getCombinedChart,
            insertIncome: _insertIncome,
            getAllIncomes: _getAllIncomes,
            getIncomeByName: _getIncome,
            deleteIncome: _deleteIncome,
            updateIncome: _updateIncome,
            insertExpense: _insertExpense,
            getAllExpenses: _getAllExpenses,
            getExpenseByName: _getExpense,
            deleteExpense: _deleteExpense,
            updateExpense: _updateExpense
        };

        function _validateUrl(url) {

            return $http({
                url: "/api/config/site?url=" + encodeURIComponent(url),
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getEnums(enumType) {
            var url = "/api/config/enums/";
            url += Array.isArray(enumType) ? enumType.join(",") : enumType;

            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }



        function _getChartById(id) {
            return $http({
                method: "GET",
                url: "/api/charts/" + id,
            });
        }

        function _getAllChartsByUser() {
            return $http({
                url: "/api/charts/user",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _insertChart(model) {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";

            return $http({
                method: "POST",
                url: url += "?username=" + page.user.username,
                data: model
            });
        }

        function _updateChart(model) {
            var url = "/api/charts/";
            model.typeId === 3 ? url += "int" : url += "list/int";
            return $http({
                method: "PUT",
                url: url += "?username=" + page.user.username,
                data: model
            });
        }

        function _deleteChartById(id) {
            return $http({
                method: "DELETE",
                url: "/api/charts/delete/" + id,
            });
        }



        function _insertCard(model) {
            return $http({
                url: "/api/cards?username=" + page.user.username,
                method: "POST",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getAllCards() {
            return $http({
                url: "/api/cards/all",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getAllCardsByUser() {
            return $http({
                url: "/api/cards/user",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _deleteCard(id) {
            return $http({
                url: "/api/cards/delete/" + id,
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _updateCard(model) {
            return $http({
                url: "/api/cards?username=" + page.user.username,
                method: "PUT",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }



        function _getIncomesChart(start, end) {
            var url = "/api/bill/income/chart";
            url += (start && end) ? "?" : "";
            url += (!start) ? "" : (end) ? "startDate=" + start + "&endDate=" + end : "startDate=" + start;

            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getExpenseChart(start, end) {
            var url = "/api/bill/expenses/chart";
            url += (start && end) ? "?" : "";
            url += (!start) ? "" : (end) ? "startDate=" + start + "&endDate=" + end : "startDate=" + start;
            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getCombinedChart(start, end) {
            var url = "/api/bill/combined/chart";
            url += (start && end) ? "?" : "";
            url += (!start) ? "" : (end) ? "startDate=" + start + "&endDate=" + end : "startDate=" + start;
            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _insertIncome(model) {
            return $http({
                url: "/api/bill/income",
                method: "POST",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getAllIncomes() {
            return $http({
                url: "/api/bill/income/all",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getIncome(id, name, scheduleType, incomeType) {

            if (!id && !name && !scheduleType && !incomeType) { return _getAllIncomes(); }

            var url = "/api/bill/income";
            var isFirst = true;
            url += (isFirst && id && id !== null && typeof (id) === "int") ? "?id=" + id : (id && id !== null && typeof (id) === "int") ? "&id=" + id : "";
            url += (isFirst && name && name !== null && typeof (name) === "string") ? "?name=" + name : (name && name != null && typeof (name) === "string") ? "&name=" + name : "";
            url += (isFirst && scheduleType && scheduleType !== null && typeof (scheduleType) === "int") ? "?scheduleType=" + scheduleType : (scheduleType && scheduleType !== null && typeof (scheduleType) === "int") ? "&scheduleType=" + scheduleType : "";
            url += (isFirst && incomeType && incomeType !== null && typeof (incomeType) === "int") ? "?incomeType=" + incomeType : (incomeType && incomeType !== null && typeof (incomeType) === "int") ? "&incomeType=" + incomeType : "";

            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _deleteIncome(id) {
            return $http({
                url: "/api/bill/income/" + id,
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _updateIncome(model) {
            return $http({
                url: "/api/bill/income",
                method: "PUT",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _insertExpense(model) {
            return $http({
                url: "/api/bill/expenses",
                method: "POST",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getAllExpenses() {
            return $http({
                url: "/api/bill/expenses/all",
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _getExpense(id, name, scheduleType, billType) {

            if (!id && !name && !scheduleType && !billType) { return _getAllExpenses(); }

            var url = "/api/bill/expenses";
            var isFirst = true;
            url += (isFirst && id && id !== null && typeof (id) === "int") ? "?id=" + id : (id && id != null && typeof (id) === "int") ? "&id=" + id : "";
            url += (isFirst && name && name !== null && typeof (name) === "string") ? "?name=" + name : (name && name !== null && typeof (name) === "string") ? "&name=" + name : "";
            url += (isFirst && scheduleType && scheduleType !== null && typeof (scheduleType) === "int") ? "?scheduleType=" + scheduleType : (scheduleType && scheduleType !== null && typeof (scheduleType) === "int") ? "&scheduleType=" + scheduleType : "";
            url += (isFirst && billType && billType !== null && typeof (billType) === "int") ? "?billType=" + billType : (billType && billType !== null && typeof (billType) === "int") ? "&billType=" + billType : "";

            return $http({
                url: url,
                method: "GET",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _deleteExpense(id) {
            return $http({
                url: "/api/bill/expenses/" + id,
                method: "DELETE",
                headers: { 'Content-Type': 'application/json' }
            });
        }

        function _updateExpense(model) {
            return $http({
                url: "/api/bill/expenses",
                method: "PUT",
                data: model,
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }

})();