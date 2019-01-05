(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController);


    homeController.$inject = ["$scope", "$baseController", '$location'];


    function homeController($scope, $baseController, $location) {
        var vm = this;

        vm.location = $location;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {

            vm.sliderOptions = {
                licenseKey: '6D8C01FB-3C9B4F3A-93CBFC04-52802A55',
                sectionsColor: [
                    'rgba(255, 255, 255, 0.00)'
                    , 'rgba(255, 255, 255, 0.00)'
                    , 'rgba(255, 255, 255, 0.00)'
                    , 'rgba(255, 255, 255, 0.00)'
                ],
                navigation: true,
                navigationPosition: 'right',
                showActiveTooltip: false,
                slidesNavigation: false,
                slidesNavPosition: 'bottom',

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
            _swiperSlider();
        }

        function _swiperSlider() {
            var swiper = new Swiper('.swiper-container', {
                autoplay: 5000,
                pagination: '.swiper-pagination',
                effect: 'coverflow',
                centeredSlides: true,
                loop: true,
                nextButton: '.swiper-button-next',
                prevButton: '.swiper-button-prev',
                slidesPerView: 1,
                autoHeight: true,
                coverflow: {
                    rotate: 50,
                    stretch: 0,
                    depth: 100,
                    modifier: 1,
                    slideShadows: true
                }
            });

            //$(window).resize(function () {
            //    var imgPath = $('.swiper-slide-active')[0].style.backgroundImage != ''
            //        ? window.location.origin + $('.swiper-slide-active')[0].style.backgroundImage.substring(5, $('.swiper-slide-active')[0].style.backgroundImage.length - 2)
            //        : $('.swiper-slide-active')[0].src

            //    page.utilities.getImage(imgPath).then((a) => {
            //        $('.swiper-slide').height(a.height);
            //        $('.swiper-slide').width(a.width);
            //        swiper.update()
            //    }, (err) => console.log(err));

            //});

            //$(window).resize();
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