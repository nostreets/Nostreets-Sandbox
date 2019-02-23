﻿(function () {

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
                    }).state('/ourTeam', {
                        name: 'ourTeam',
                        url: '/ourTeam',
                        templateUrl: '/Scripts/obl/app/templates/ourTeam.html',
                        controller: 'ourTeamController',
                        controllerAs: 'pg'
                    }).state('/entertainment', {
                        name: 'entertainment',
                        url: '/entertainment',
                        templateUrl: '/Scripts/obl/app/templates/entertainment.html',
                        controller: 'entertainmentController',
                        controllerAs: 'pg'
                    }).state('/contact', {
                        name: 'contact',
                        url: '/contact',
                        templateUrl: '/Scripts/obl/app/templates/contact.html',
                        controller: 'contactUsController',
                        controllerAs: 'pg'
                    }).state('/music', {
                        name: 'music',
                        url: '/music',
                        templateUrl: '/Scripts/obl/app/templates/music.html',
                        controller: 'musicController',
                        controllerAs: 'pg'
                    });

                $urlRouterProvider.when('', '/home');

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });
            }]);
})();