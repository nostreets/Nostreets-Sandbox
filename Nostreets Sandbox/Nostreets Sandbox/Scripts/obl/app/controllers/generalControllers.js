(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController)
        .controller("aboutController", aboutController)
        .controller("ourTeamController", ourTeamController)
        .controller("musicController", musicController)
        .controller("contactUsController", contactUsController);


    homeController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    ourTeamController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];


    function homeController($scope, $baseController) {
        var vm = this;
        vm.getDeviceWidth = page.utilities.getDeviceWidth;

        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {
            vm.sliderOptions = page.sliderOptions;


        }

    }

    function aboutController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {
            $('title').text("OBL | About");


        }

        function _handlers() {
            $baseController.defaultListeners($scope);

        }
    }

    function ourTeamController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {
            vm.sliderOptions = page.sliderOptions;
            $('title').text("OBL | Our Team");

        }

        function _handlers() {
             $baseController.defaultListeners($scope,
                {
                    'fp-onLeave': (event, data) =>
                    {

                        console.log('left slide...');

                        //if(data.index % 2 === 0)

                    }
                }
            );
        }

    }

    function musicController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {
            $('title').text("OBL | Music");


        }

    }

    function contactUsController($scope, $baseController, $http) {
        var vm = this;
        vm.sendEmail = _sendEmail;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {
            vm.hasSent = false;
            vm.subjects = [
                'Mix And Master Services',
                'Beat Production Services',
                'Video Production Services',
                'OBL Music Inquiry',
                'OBL Apperal Inquiry',
                'OBL Entertainment Inquiry',
                'OBL Techincal Inquiry',
                'Report a Bug',
                'Other'
            ];
        }

        function _sendEmail(model) {
            $http({
                method: "POST",
                url: "/api/send/email",
                data: model
            }).then(_emailSuccessResponse, _consoleResponse);
        }

        function _emailSuccessResponse(data) {
            vm.hasSent = true;
        }

        function _consoleResponse(data) {
            console.log(data);
        }
    }

})();