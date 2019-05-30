(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController)
        .controller("aboutController", aboutController)
        .controller("ourTeamController", ourTeamController)
        .controller("musicController", musicController)
        .controller("entertainmentController", entertainmentController)
        .controller("contactUsController", contactUsController);


    homeController.$inject = ["$scope", "$baseController", "$state"];
    aboutController.$inject = ["$scope", "$baseController"];
    ourTeamController.$inject = ["$scope", "$baseController"];
    musicController.$inject = ["$scope", "$baseController"];
    entertainmentController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];


    function homeController($scope, $baseController, $state) {
        var vm = this;

        _render();

        function _render() {
            _setUp();
            _handlers();
            _pageFixes();
        }

        function _setUp() {
            vm.sliderOptions = page.sliderOptions;
            vm.windowWidth = page.utilities.getDeviceWidth();
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | Home");
            page.renderParticles();
            page.fixFooter();

            if (page.previousView === 'music')
                $baseController.window.location.reload();
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

            if (page.previousView === 'music')
                $baseController.window.location.reload();
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
                    "desc": "Rediet Teferi, also known by his stage name WWRED., is an Ethiopian-American producer, recording artist, and singer/songwriter from Los Angeles, California. With his transcendental and neo-modern mindset, he ushers positive vibrations and an inquisitive amalgam of the old ways of living and new insights into all the enterprises of OBL. Rediet is one-third of OBL as the CEO, CCO, and the President of Music. ",
                    "imgUrl": "/assets/img/rediet.3.jpg",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.wwred/",
                        "youtube": "https://www.youtube.com/channel/UCzF_BoRKI48fynEu5QaJ7SA"
                    }
                },
                hovaness: {
                    "id": "#hovanessCard",
                    "color": "Light-Orange",
                    "name": "Hovaness Gadzhyan",
                    "title": "CEO / CDO / Director Of Written Content",
                    "desc": "Hovaness was born and raised in Inglewood, California. Being half Hispanic and half Armenian, he brings two different cultures from across the world into OBL with a mix of west coast vibes. Hovaness brings the artistic and creative expertise to OBL designs, music, and all other ventures. Hovaness is one-third of OBL, as the CEO, CDO, and Director Of Written Content.",
                    "imgUrl": "/assets/img/hov.1.png",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.hovlito/",
                        "youtube": "https://www.youtube.com/"
                    }
                },
                kandon: {
                    "id": "#kandonCard",
                    "color": "Orange",
                    "name": "Kandon",
                    "title": "CEO",
                    "desc": "...",
                    "imgUrl": "/assets/img/kandon.1.jpg",
                    "socials": {
                        "instagram": "https://www.instagram.com/obl.melodx/",
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

            if (page.previousView === 'music')
                $baseController.window.location.reload();
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
                        musicPath: "/assets/audio/What You Need (prod. WWRED.).mp3",
                        imgPath: "/assets/img/wwred.1.jpg",
                        spotify: "https://open.spotify.com/artist/0FLx6NTV9fZcOWK7UOrv2V",
                        soundcloud: "https://soundcloud.com/wwred/what-you-need-prod-wwred",
                        youtube: "https://www.youtube.com/watch?v=J9S2L4kRBbM&list=OLAK5uy_lo0_pqWHQHK9OjBfOaQ-fhus8HI3_9okQ"
                    },
                    {
                        title: "WWRED. - T.H.U.G.L.I.F.E.",
                        musicPath: "/assets/audio/T.H.U.G.L.I.F.E. (short) (prod. WWRED.).mp3",
                        imgPath: "/assets/img/wwred.2.jpg",
                        spotify: "https://open.spotify.com/artist/0FLx6NTV9fZcOWK7UOrv2V",
                        soundcloud: "https://soundcloud.com/wwred/thuglife-prod-wwred",
                        youtube: "https://www.youtube.com/watch?v=DqJr7eypBs0&list=OLAK5uy_n3rPTZ61Nnz3PtoB62v7HrtCzwjm0i9Sc"
                    },
                    {
                        title: "WWRED. - Løve!",
                        musicPath: "/assets/audio/Løve! (prod. WWRED.).wav",
                        imgPath: "/assets/img/wwred.3.jpg",
                        spotify: "https://open.spotify.com/artist/0FLx6NTV9fZcOWK7UOrv2V",
                        soundcloud: "https://soundcloud.com/wwred/love-prod-wwred",
                        youtube: "https://www.youtube.com/watch?v=A6JucX95DSQ"
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

            if (page.previousView === 'music')
                $baseController.window.location.reload();
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
            vm.windowWidth = page.utilities.getDeviceWidth();
        }

        function _handlers() {
            $baseController.defaultListeners($scope);
        }

        function _pageFixes() {
            $('title').text("OBL | Video");
            page.renderParticles();
            page.fixFooter();

            if (page.previousView === 'music')
                $baseController.window.location.reload();

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
                'Music Distribution Services',
                'Video Production Services',
                'Photography Services',
                'Web Site Development Services',
                'Mobile App Development Services',
                'OBL Records Inquiry',
                'OBL Apperal Inquiry',
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

            if (page.previousView === 'music')
                $baseController.window.location.reload();
        }

        function _sendEmail(model) {
            model.messageText = model.messageText.replace(/</g, "&lt;").replace(/>/g, "&gt;");

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