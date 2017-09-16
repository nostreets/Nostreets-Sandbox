(function () {

    angular.module(page.APPNAME).controller("billManagerController");

    billManagerController.$inject = ["$scope", "$baseController", '$uibModal', '$sandboxService'];

    function billManagerController($scope, $baseController, $uibModal, $sandboxService) {

        var vm = this;

        _render();

        function _render() {
            _setUp();
        }

        function _setUp() {
            vm.income = [];
            vm.expenses = [];
            vm.charts = [];
        }

        function _getUserData() {

        }

        function _getIncomeChart() {
            $sandboxService.getIncomeChart().then(
                (data) => {
                    var incomeChart = {
                        type: "income",
                        data: data.item
                    };
                    vm.charts.push(incomeChart);
                },
                (data) => console.log(data)
            );
        }

        function _getExpensesChart() {
            $sandboxService.getExpensesChart().then(
                (data) => {
                    var expensesChart = {
                        type: "expense",
                        data: data.item
                    };
                    vm.charts.push(expensesChart);
                },
                (data) => console.log(data)
            );
        }

        function _getCombinedChart() {
            $sandboxService.getCombinedChart().then(
                (data) => {
                    var combinedChart = {
                        type: "combined",
                        data: data.item
                    };
                    vm.charts.push(combinedChart);
                },
                (data) => console.log(data)
            );
        }

        function _getAllIncome() {
             $sandboxService.getAllIncome().then(
                (data) => vm.income = data.items,
                (data) => console.log(data)
            );
        }

        function _getAllExpense() {
            $sandboxService.getAllExpenses().then(
                (data) => vm.expenses = data.items,
                (data) => console.log(data)
            );
        }

        function _getIncome() {

        }

        function _getExpense() {

        }

        function _insertIncome() {

        }

        function _insertExpense() {

        }

        function _deleteExpense() {

        }

        function _deleteExpense() {

        }

    }

})();