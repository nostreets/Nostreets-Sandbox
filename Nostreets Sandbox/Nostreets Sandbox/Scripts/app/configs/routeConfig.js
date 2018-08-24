(function () {

    angular.module(page.APPNAME)
        .config(["$routeProvider", "$locationProvider",
            function ($routeProvider, $locationProvider) {

                $routeProvider.caseInsensitiveMatch = true;

                $locationProvider.hashPrefix('!');

                $routeProvider.when('/', {
                    templateUrl: '/Scripts/app/templates/home.html',
                    controller: 'homeController',
                    controllerAs: 'pg'
                }).when('/skills', {
                    templateUrl: '/Scripts/app/templates/skills.html',
                    controller: 'skillsController',
                    controllerAs: 'pg'
                }).when('/pastProjects', {
                    templateUrl: '/Scripts/app/templates/pastProjects.html',
                    controller: 'pastEmployersController',
                    controllerAs: 'pg'
                }).when('/contact', {
                    templateUrl: '/Scripts/app/templates/contact.html',
                    controller: 'contactUsController',
                    controllerAs: 'pg'
                }).when('/about', {
                    templateUrl: '/Scripts/app/templates/about.html',
                    controller: 'aboutController',
                    controllerAs: 'pg'
                }).when('/applicationsInProgress', {
                    templateUrl: '/Scripts/app/templates/applicationsInProgress.html',
                    controller: 'personalProjectsController',
                    controllerAs: 'pg'
                }).when('/dynamicGraphs', {
                    templateUrl: '/Scripts/app/templates/dynamicGraphs.html',
                    controller: 'dynamicGraphsController',
                    controllerAs: 'pg'
                }).when('/cardBuilder', {
                    templateUrl: '/Scripts/app/templates/bootstrapCardBuilder.html',
                    controller: 'cardBuilderController',
                    controllerAs: 'pg'
                }).when('/billManager', {
                    templateUrl: '/Scripts/app/templates/billManager.html',
                    controller: 'billManagerController',
                    controllerAs: 'pg'
                });

                $locationProvider.html5Mode({
                    enabled: false,
                    requireBase: false
                });
            }]);
})();