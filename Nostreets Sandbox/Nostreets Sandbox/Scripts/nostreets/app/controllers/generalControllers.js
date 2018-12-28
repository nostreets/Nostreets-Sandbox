(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController)
        .controller("personalProjectsController", personalProjectsController)
        .controller("pastEmployersController", pastEmployersController)
        .controller("contactUsController", contactUsController)
        .controller("aboutController", aboutController)
        .controller("skillsController", skillsController)
        .controller("modalCodeController", modalCodeController);

    homeController.$inject = ["$scope", "$baseController", '$location'];
    personalProjectsController.$inject = ["$scope", "$baseController"];
    pastEmployersController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    skillsController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];
    modalCodeController.$inject = ['$baseController', '$uibModalInstance', 'code'];

    function homeController($scope, $baseController, $location) {
        var vm = this;

        vm.location = $location;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {
            vm.homeLinks = [
                { label: 'Personal Projects', link: '#!applicationsInProgress' }
                , { label: 'Previous Employers', link: '#!pastProjects' }
                , { label: 'Contact', link: '#!contact' }
                , { label: 'About', link: '#!about' }
                , { label: 'Skills', link: '#!skills' }
            ];
        }
    }

    function skillsController($scope, $baseController) {
        var vm = this;
        vm.lookup = page.utilities.googleSearch;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {

            vm.skills = {
                'Industry Knowledge': [
                    'Agile Methodologies',
                    'Web Development',
                    'Mobile Development',
                    'Software Development',
                    'Software Development Life Cycle',
                    'Web Design',
                    'Web Services',
                    'Web Applications',
                    'Database Design',
                    'Technology Integration',
                    'Cloud Computing'
                ],

                'Tools & Technologies': [
                    'Microsoft Visual Studio',
                    'Team Foundation Server (TFS)',
                    'SQL Server Management Studio',
                    'Github',
                    '.Net',
                    'C#',
                    'AngularJS',
                    'AJAX',
                    'HTML5',
                    'ASP.NET MVC',
                    'ASP.NET',
                    'ADO.NET',
                    'Bootstrap',
                    'Cascading Style Sheets (CSS)',
                    'HTML',
                    'Java',
                    'Javascript',
                    'jQuery',
                    'SQL Server Integration Services (SSIS)',
                    'SQL Server Reporting Services (SSRS)',
                    'T-SQL',
                    'VBScript',
                    'XML',
                    'JSON',
                    'Amazon Web Services (AWS)',
                    'Google Cloud Platform',
                    'Microsoft Azure',
                    'IBM Bluemix'
                ],

                'API\'s Used': [
                    'Automapper',
                    'Restsharp',
                    'LinqToTwitter',
                    'Mailchimp',
                    'Hangfire',
                    'Chartist.js',
                    'IBM Speech To Text',
                    'Instasharp',
                    'Pinterest',
                    'SendGrid',
                    'Google Maps Geolocation',
                    'Twilio’s Chat',
                    'IBM Text To Speech',
                    'IBM Conversation',
                    'TBDb',
                    'nopCommerce',
                    'UPS',
                    'USPS',
                    'PivotTable.js',
                    'Google Analytics'
                ],

                'Other Skills': [
                    'Microsoft Excel',
                    'Microsoft Office',
                    'AutoCAD'
                ]
            };
            vm.topics = _skillTopics();
        }

        function _skillTopics() {
            var result = [];

            for (var skill in vm.skills)
                result.push(skill);

            return result;
        }
    }

    function personalProjectsController($scope, $baseController) {
        var vm = this;
        _render;

        function _render() {
            $baseController.defaultListeners($scope);
        }

        function _setUp() { }
    }

    function pastEmployersController($scope, $baseController) {
        var vm = this;
        _render();

        function _render() {
            $baseController.defaultListeners($scope);
        }

        function _setUp() { }
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