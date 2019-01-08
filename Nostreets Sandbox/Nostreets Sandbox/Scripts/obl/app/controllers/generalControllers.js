(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController);


    homeController.$inject = ["$scope", "$baseController"];


    function homeController($scope, $baseController, $location) {
        var vm = this;

        vm.location = $location;
        vm.isViewRendered = false;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope,
                {
                    $viewContentLoaded: () => {
                        $('#fullpage').fullpage(vm.sliderOptions);
                    }
                }
            );

        }

        function _setUp() {

            vm.sliderOptions = {
                licenseKey: '6D8C01FB-3C9B4F3A-93CBFC04-52802A55',
                sectionsColor: [
                    'rgba(255, 255, 255, 0.30)'
                    , 'rgba(255, 255, 255, 0.30)'
                    , 'rgba(255, 255, 255, 0.30)'
                    , 'rgba(255, 255, 255, 0.30)'
                    , 'rgba(255, 255, 255, 0.30)'
                ],
                navigation: true,
                navigationPosition: 'right',
                showActiveTooltip: false,
                slidesNavigation: true,
                slidesNavPosition: 'bottom',
                lockAnchors:true,

                //Scrolling
                css3: true,
                scrollingSpeed: 500,
                autoScrolling: true,
                fitToSection: true,
                scrollBar: false,
                easingcss3: 'ease',
                loopBottom: false,
                loopTop: false,
                scrollHorizontally: false,
                dragAndMove: false,
                fadingEffect: false,
                scrollOverflow: false,
                scrollOverflowReset: false,
                touchSensitivity: 15,
                normalScrollElementTouchThreshold: 5,

                //Accessibility
                keyboardScrolling: true,
                animateAnchor: true,
                recordHistory: true,

                //Design
                controlArrows: true,
                verticalCentered: true,

                //Custom selectors
                sectionSelector: '.section',
                slideSelector: '.slide',

                lazyLoading: true
            };

        }


    }

    function aboutController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {

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
                'Inquiry',
                'Musicial Beat Quote',
                'Web Site Quote',
                'Moblie App Quote',
                'Software Quote',
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