(function () {

    angular.module(page.APPNAME).controller("homeController", homeController)
        .controller("applicationsInProgressController", applicationsInProgressController)
        .controller("pastProjectsController", pastProjectsController)
        .controller("contactUsController", contactUsController)
        .controller("aboutController", aboutController)
        .controller("modalCodeController", modalCodeController);


    homeController.$inject = ["$scope", "$baseController", '$location'];
    applicationsInProgressController.$inject = ["$scope", "$baseController"];
    pastProjectsController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];
    modalCodeController.$inject = ['$baseController', '$uibModalInstance', 'code'];


    function homeController($scope, $baseController, $location) {

        var vm = this;

        vm.location = $location;
        _render();

        function _render() {
            _setUp();
        }

        function _setUp() {
            vm.homeLinks = [
                  { label: 'Personal Applications', link: '#!applicationsInProgress' }
                , { label: 'Previous Workplaces', link: '#!pastProjects' }
                , { label: 'Contact Nile', link: '#!contact' }
                , { label: 'About Nile', link: '#!about' }
            ];

        }

    }

    function applicationsInProgressController($scope, $baseController) {

        var vm = this;
        _render;

        function _render() { }

        function _setUp() { }
    }

    function pastProjectsController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() { }

        function _setUp() { }
    }

    function aboutController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            _setUp();
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
        }

        function _sendEmail(model) {
            $http({
                method: "POST",
                url: "/api/send/email",
                data: model
            }).then(_emailSuccessResponse, _consoleResponse);
        }

        function _setUp() {
            vm.hasSent = false;
        }

        function _emailSuccessResponse(data) {
            vm.hasSent = true;
        }

        function _consoleResponse(data) {
            console.log(data);
        }
    }

    function modalCodeController($baseController, $uibModalInstance, code) {

        var vm = this;

        _setUp();

        function _setUp() {
            if (code) {
                vm.code = code;
            }
        }
    }

})();