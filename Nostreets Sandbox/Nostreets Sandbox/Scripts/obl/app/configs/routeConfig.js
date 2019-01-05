(function () {

    angular.module(page.APPNAME)
        .config(["$routeProvider", "$locationProvider",
            function ($routeProvider, $locationProvider) {

                $routeProvider.caseInsensitiveMatch = true;

                $locationProvider.hashPrefix('!');

                $routeProvider.when('/', {
                    templateUrl: '/Scripts/obl/app/templates/home.html',
                    controller: 'homeController',
                    controllerAs: 'pg'
                }).when('/about', {
                    templateUrl: '/Scripts/obl/app/templates/about.html',
                    controller: 'aboutController',
                    controllerAs: 'pg'
                }).when('/contact', {
                    templateUrl: '/Scripts/obl/app/templates/contact.html',
                    controller: 'contactController',
                    controllerAs: 'pg'
                });

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });
            }]);
})();