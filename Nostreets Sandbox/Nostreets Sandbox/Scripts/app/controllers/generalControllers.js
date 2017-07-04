(function () {

    angular.module(page.APPNAME).controller("homeController", homeController)
    .controller("applicationsInProgressController", applicationsInProgressController)
    .controller("pastProjectsController", pastProjectsController)
    .controller("contactUsController", contactUsController)
    .controller("aboutController", aboutController);


    homeController.$inject = ["$scope", "$baseController", '$location'];
    applicationsInProgressController.$inject = ["$scope", "$baseController"];
    pastProjectsController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];


    function homeController($scope, $baseController, $location) {

        var vm = this;

        vm.location = $location;
        _render();

        function _render() { }

        function _setUp() { }

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
        }

        function _setUp() { }
    }

    function contactUsController($scope, $baseController, $http) {
        var vm = this;
        vm.sendEmail = _sendEmail;
        _render();

        function _render()
        {
            _setUp();
        }

        function _sendEmail(model) {
            $http({
                method: "POST",
                url: "/api/send/email",
                data: model
            }).then(_emailSuccessResponse, _consoleResponse);}

        function _setUp()
        {
            vm.hasSent = false;
        }

        function _emailSuccessResponse(data)
        {
            vm.hasSent = true;
        }

        function _consoleResponse(data)
        {
            console.log(data);
        }
    }
})();