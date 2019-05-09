(function () {

    angular.module(page.APPNAME)
        .config(["$stateProvider", "$locationProvider", "$urlRouterProvider",
            function ($stateProvider, $locationProvider, $urlRouterProvider) {

                $locationProvider.hashPrefix('!');

                $stateProvider.state('/user', {

                        name: 'user',
                        url: '/dashboard',
                        templateUrl: '/Scripts/obl/app/templates/user.html',
                        controller: 'userController',
                        controllerAs: 'pg'
                    });

                $urlRouterProvider.when('', '/login');

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });

            }]);

})();