(function () {

    angular.module(page.APPNAME)
        .config(["$stateProvider", "$locationProvider", "$urlRouterProvider",
            function ($stateProvider, $locationProvider, $urlRouterProvider) {

                $locationProvider.hashPrefix('!');

                $stateProvider
                    .state('/home', {
                        name: 'home',
                        url: '/home',
                        templateUrl: '/Scripts/obl/app/templates/home.html',
                        controller: 'homeController',
                        controllerAs: 'pg'
                    }).state('/about', {
                        name: 'about',
                        url: '/about',
                        templateUrl: '/Scripts/obl/app/templates/about.html',
                        controller: 'aboutController',
                        controllerAs: 'pg'
                    }).state('/contact', {
                        name: 'contact',
                        url: '/contact',
                        templateUrl: '/Scripts/obl/app/templates/contact.html',
                        controller: 'contactUsController',
                        controllerAs: 'pg'
                    });

                $urlRouterProvider.when('', '/home');

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });
            }]);
})();