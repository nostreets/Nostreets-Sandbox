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
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'home'; }

                    }).state('/about', {

                        name: 'about',
                        url: '/about',
                        templateUrl: '/Scripts/obl/app/templates/about.html',
                        controller: 'aboutController',
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'about'; }

                    }).state('/ourTeam', {

                        name: 'ourTeam',
                        url: '/ourTeam',
                        templateUrl: '/Scripts/obl/app/templates/ourTeam.html',
                        controller: 'ourTeamController',
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'ourTeam'; }

                    }).state('/entertainment', {

                        name: 'entertainment',
                        url: '/entertainment',
                        templateUrl: '/Scripts/obl/app/templates/entertainment.html',
                        controller: 'entertainmentController',
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'entertainment'; }

                    }).state('/contact', {

                        name: 'contact',
                        url: '/contact',
                        templateUrl: '/Scripts/obl/app/templates/contact.html',
                        controller: 'contactUsController',
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'contact'; }

                    }).state('/music', {

                        name: 'music',
                        url: '/music',
                        templateUrl: '/Scripts/obl/app/templates/music.html',
                        controller: 'musicController',
                        controllerAs: 'pg',
                        onExit: () => { page.previousView = 'music'; }

                    });

                $urlRouterProvider.when('', '/home');

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });

            }]);

})();