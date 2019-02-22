(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController)
        .controller("aboutController", aboutController)
        .controller("ourTeamController", ourTeamController)
        .controller("musicController", musicController)
        .controller("videoController", videoController)
        .controller("contactUsController", contactUsController);


    homeController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    ourTeamController.$inject = ["$scope", "$baseController"];
    musicController.$inject = ["$scope", "$baseController"];
    videoController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];


    function homeController($scope, $baseController) {
        var vm = this;

        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {
            vm.sliderOptions = page.sliderOptions;
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

        }

    }

    function aboutController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {
            $('title').text("OBL | About");


        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

        }

    }

    function ourTeamController($scope, $baseController) {
        var vm = this;

        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {

            vm.deviceWidth = page.utilities.getDeviceWidth();
            vm.sliderOptions = page.sliderOptions;
            vm.cardParams = {
                nile: {
                    "id": "#nileCard",
                    "color": "Indigo",
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
                    "color": "Red",
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
                    "color": "Light-Orange",
                    "name": "Hovaness G.",
                    "title": "CEO / CDO",
                    "desc": "Hovaness was born and raised in Inglewood, California. Being half Hispanic and half Armenian, he brings two different cultures from across the world into OBL with a mix of west coast vibes. Hovaness brings the artistic and creative expertise to OBL designs, music, and all other ventures.",
                    "imgUrl": "/assets/img/hov.jpg",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.hov/",
                        "youtube": "https://www.youtube.com/"
                    }
                }
            };


            $('title').text("OBL | Our Team");
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

        }
    }

    function musicController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {
            $('title').text("OBL | Music");

            vm.musicParams = {
                songs: [
                    {
                        title: "What You Need (prod. WWRED.)",
                        path: "/assets/mp3/What You Need (prod. WWRED.).mp3"
                    },
                    {
                        title: "T.H.U.G.L.I.F.E. (prod. WWRED.)",
                        path: "/assets/mp3/T.H.U.G.L.I.F.E. (short) (prod. WWRED.).mp3"
                    }
                ]
            };
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

        }

    }

    function videoController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {
            $('title').text("OBL | Video");


        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles({
                "particles": {
                    "opacity": {
                        "value": 0.3,
                        "random": true,
                        "anim": {
                            "enable": false,
                            "speed": 1,
                            "opacity_min": 0.1,
                            "sync": false
                        }
                    }
                }
            });
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

        }
    }

    function contactUsController($scope, $baseController, $http) {
        var vm = this;
        vm.sendEmail = _sendEmail;
        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
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

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);

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