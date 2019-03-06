(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController)
        .controller("aboutController", aboutController)
        .controller("ourTeamController", ourTeamController)
        .controller("musicController", musicController)
        .controller("entertainmentController", entertainmentController)
        .controller("contactUsController", contactUsController);


    homeController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    ourTeamController.$inject = ["$scope", "$baseController"];
    musicController.$inject = ["$scope", "$baseController"];
    entertainmentController.$inject = ["$scope", "$baseController"];
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
            $('title').text("OBL | Home");
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

        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | About");
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
                    "title": "CEO / CFO / CIO",
                    "desc": "Nile Overstreet was born and raised in Los Angeles, California. Always trying expand and improve his knowledge and understanding of music, buiseness, technology, and more, he brings different innovativations to OBL and the technological expertise to the designs, music, and all other ventures. Nile is one-third of OBL, as the CEO, CIO, CCO, and CFO.",
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
                    "title": "CEO / President Of Music",
                    "desc": "Rediet Teferi, also known by his stage name WWRED. is an American artist from Los Angeles, California. He released his debut single, 'What You Need' in Februbary 2019. Apart from his carrer as an artist he also works as the lead record producer and  of OBL. Rediet is one-third of OBL, as the CEO, CCO, and President Of Music.",
                    "imgUrl": "/assets/img/rediet2.jpg",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.wwred/",
                        "youtube": "https://www.youtube.com/"
                    }
                },
                hovaness: {
                    "id": "#hovanessCard",
                    "color": "Light-Orange",
                    "name": "Hovaness Gadzhyan",
                    "title": "CEO / CDO / Director Of Written Content",
                    "desc": "Hovaness was born and raised in Inglewood, California. Being half Hispanic and half Armenian, he brings two different cultures from across the world into OBL with a mix of west coast vibes. Hovaness brings the artistic and creative expertise to OBL designs, music, and all other ventures. Hovaness is one-third of OBL, as the CEO, CDO, and Director Of Written Content.",
                    "imgUrl": "/assets/img/hov.jpg",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.hovlito/",
                        "youtube": "https://www.youtube.com/"
                    }
                }
            };


        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | Our Team");
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

            vm.musicParams = {
                songs: [
                    {
                        title: "WWRED. - What You Need",
                        path: "/assets/mp3/What You Need (prod. WWRED.).mp3"
                    },
                    {
                        title: "WWRED. - T.H.U.G.L.I.F.E.",
                        path: "/assets/mp3/T.H.U.G.L.I.F.E. (short) (prod. WWRED.).mp3"
                    }
                ]
            };
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | Music");
            page.renderParticles();
            page.fixFooter();
            angular.element($baseController.window).bind('resize', page.fixFooter);
        }

    }

    function entertainmentController($scope, $baseController) {
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
            $('title').text("OBL | Video");
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
            $('title').text("OBL | Contact");
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