(function () {

    angular.module(page.APPNAME)
        .config(["$stateProvider", "$locationProvider", "$urlRouterProvider",
            function ($stateProvider, $locationProvider, $urlRouterProvider) {

                $locationProvider.hashPrefix('!');

                $stateProvider.state('/dashboard', {

                        name: 'dashboard',
                        url: '/dashboard',
                        templateUrl: '/Scripts/obl/app/templates/user.html',
                        controller: 'userDashboardController',
                        controllerAs: 'pg'
                    });

                $urlRouterProvider.when('', '/dashboard');

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });

            }]);

})();