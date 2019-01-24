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
        vm.reRenderCardsOn = _reRenderCardsOn;

        _render();

        function _render() {
            _setUp();
            _handlers();
        }

        function _setUp() {

            vm.deviceWidth = page.utilities.getDeviceWidth();
            vm.sliderOptions = page.sliderOptions;
            vm.cardParams = {
                nile: {
                    "id": "#nileCard",
                    "color": "Red",
                    "name": "Nile Overstreet",
                    "title": "CEO / CIO",
                    "desc": "Always trying expand and improve my knowledge and understanding of different technologies like in this vast world of computer science...",
                    "imgUrl": "/assets/img/nile5.jpg",
                    "socials": {
                        "github": "https://github.com/nostreets",
                        "instagram": "https://www.instagram.com/obl.nostreets/",
                        "linkedin": "https://www.linkedin.com/in/nile-overstreet/",
                        "youtube": "https://www.youtube.com/"
                    }
                },
                rediet: {
                    "id": "#redietCard",
                    "color": "Indigo",
                    "name": "Rediet Teferi",
                    "title": "CEO / CPO",
                    "desc": "...",
                    "imgUrl": "/assets/img/rediet2.jpg",
                    "socials": {
                        "instagram": "http://obl.network",
                        "youtube": "https://www.youtube.com/"
                    }
                },
                hovaness: {
                    "id": "#hovanessCard",
                    "color": "Blue",
                    "name": "Hovaness G.",
                    "title": "CEO / CDO",
                    "desc": "...",
                    "imgUrl": "/assets/img/hov.jpg",
                    "socials": {
                        "instagram": "http://obl.network",
                        "youtube": "https://www.youtube.com/"
                    }
                }
            };


            $('title').text("OBL | Our Team");
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _reRenderCardsOn() {
            var result = false;
            if (vm.deviceWidth === page.getDeviceWidth())
                result = true;
            else
                vm.deviceWidth = page.getDeviceWidth();

            return result;
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