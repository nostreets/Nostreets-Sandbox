(function () {

    angular.module(page.APPNAME).controller("homeController", homeController)
        .controller("applicationsInProgressController", applicationsInProgressController)
        .controller("pastProjectsController", pastProjectsController)
        .controller("contactUsController", contactUsController)
        .controller("aboutController", aboutController)
        .controller("cardBuilderController", cardController)
        .controller("modalCodeController", modalCodeController);


    homeController.$inject = ["$scope", "$baseController", '$location'];
    applicationsInProgressController.$inject = ["$scope", "$baseController"];
    pastProjectsController.$inject = ["$scope", "$baseController"];
    aboutController.$inject = ["$scope", "$baseController"];
    contactUsController.$inject = ["$scope", "$baseController", "$http"];
    cardController.$inject = ['$baseController', '$uibModal'];


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

    function cardController($baseController, $uibModal) {

        var vm = this;
        vm.submitCard = _btnSumbit;
        vm.buildCard = _buildCard;
        vm.populateCard = _populateCard;
        vm.loadPreBuiltCard = _loadPreBuiltCard;
        vm.validateForm = _validateForm;
        vm.viewCode = _viewCode;

        _setUp();

        function _setUp(cards) {
            //vm.elementsLoaded = _loopTillTrue(null, () => { angular.element(".card-builder-formModal").on("load", () => { return true; })});
            vm.headerType = null;
            vm.mainType = null;
            vm.footerType = null;
            vm.headerAlignment = null;
            vm.mainAlignment = null;
            vm.footerAlignment = null;

            if (!cards) {
                vm.cards = [];
            }
            else {
                vm.cards.concat(vm.cards, cards.filter((a) => !vm.cards.indexOf(a)));
            }
        }

        function _loopTillTrue(input, predicate) {
            var isTrue = predicate(input);
            if (!isTrue) {
                $baseController.timeout(5000, predicate(input));
                //_loopTillTrue(input, $baseController.timeout(5000, predicate(input)))
            }
            else {
                return true;
            }
        }

        function _viewCode() {
            $baseController.http({
                url: "api/view/code/cardController",
                method: "GET",
                responseType: "JSON"
            }).then(function (data) {
                _openModal(data.data.item);
            });
        }

        function _openModal(code) {
            var modalInstance = $uibModal.open({
                animation: true
                , templateUrl: "codeModal.html"
                , controller: "modalCodeController as mc"
                , size: "lg"
                , resolve: {
                    code: function () {
                        return code;
                    }
                }
            });
        }


        function _btnSumbit() {
            if (_validateForm("#cardBuilderForm")) {
                swal({
                    title: "Do you want to save this card?",
                    showCancelButton: true
                }).then(function () {
                    var card = vm.loadPreBuiltCard();
                    card = vm.buildCard(card);
                    card = vm.populateCard(card);
                    console.log(card);
                    vm.cards.push(card);
                    $(".card-builder-formModal").modal("hide");
                });
            }
        }

        //Client Side Functions
        function _loadPreBuiltCard() {
            if ($("#formSize-smCheckbox").is(":checked")) {
                var card = $($("#card").html());
                card.attr("class", "col-md-4 col-sm-6 col-xs-12");
                return card;
            }
            else if ($("#formSize-mdCheckbox").is(":checked")) {
                var card = $($("#card").html());
                card.attr("class", "col-md-8");
                return card;
            }
            else if ($("#formSize-lgCheckbox").is(":checked")) {
                var card = $($("#card").html());
                card.attr("class", "col-md-12");
                return card;
            }
        }

        function _buildCard(template) {
            if ($("#formHeader-textCheckbox").is(":checked")) {
                template.find(".card-header").empty();
                var element = $($("#small-Temp").html());
                element.addClass("headerText");
                template.find(".card-header").append(element);
            }
            else if ($("#formHeader-imageCheckbox").is(":checked")) {
                template.find(".card-header").empty();
                var element = $($("#image-Temp").html());
                element.addClass("headerImg");
                template.find(".card-header").append(element);
            }
            else if ($("#formHeader-videoCheckbox").is(":checked")) {
                template.find(".card-header").empty();
                var element = $($("#video-Temp").html());
                element.addClass("headerVid");
                template.find(".card-header").append(element);
            }


            if ($("#formHeader-leftCheckbox").is(":checked")) {
                template.find(".card-header").removeClass("text-center");
                template.find(".card-header").removeClass("pull-right");
                template.find(".card-header").addClass("pull-left");
            }
            else if ($("#formHeader-middleCheckbox").is(":checked")) {
                template.find(".card-header").addClass("text-center");
                template.find(".card-header").removeClass("pull-right");
                template.find(".card-header").removeClass("pull-left");
            }
            else if ($("#formHeader-rightCheckbox").is(":checked")) {
                template.find(".card-header").removeClass("text-center");
                template.find(".card-header").addClass("pull-right");
                template.find(".card-header").removeClass("pull-left");
            }


            if ($("#formContent-textCheckbox").is(":checked")) {
                template.find(".card-content").empty();
                var element = $($("#small-Temp").html());
                element.addClass("contentText");
                template.find(".card-content").append(element);
            }
            else if ($("#formContent-imageCheckbox").is(":checked")) {
                template.find(".card-content").empty();
                var element = $($("#image-Temp").html());
                element.addClass("contentImg");
                template.find(".card-content").append(element);
            }
            else if ($("#formContent-videoCheckbox").is(":checked")) {
                template.find(".card-content").empty();
                var element = $($("#video-Temp").html());
                element.addClass("contentVid");
                template.find(".card-content").append(element);
            }


            if ($("#formContent-leftCheckbox").is(":checked")) {
                template.find(".card-content").removeClass("text-center");
                template.find(".card-content").removeClass("pull-right");
                template.find(".card-content").addClass("pull-left");
            }
            else if ($("#formContent-middleCheckbox").is(":checked")) {
                template.find(".card-content").addClass("text-center");
                template.find(".card-content").removeClass("pull-right");
                template.find(".card-content").removeClass("pull-left");
            }
            else if ($("#formContent-rightCheckbox").is(":checked")) {
                template.find(".card-content").removeClass("text-center");
                template.find(".card-content").addClass("pull-right");
                template.find(".card-content").removeClass("pull-left");
            }


            if ($("#formFooter-textCheckbox").is(":checked")) {
                template.find(".card-footer").empty();
                var element = $($("#small-Temp").html());
                element.addClass("footerText");
                template.find(".card-footer").append(element);
            }
            else if ($("#formFooter-imageCheckbox").is(":checked")) {
                template.find(".card-footer").empty();
                var element = $($("#image-Temp").html());
                element.addClass("footerImg");
                template.find(".card-footer").append(element);
            }
            else if ($("#formFooter-videoCheckbox").is(":checked")) {
                template.find(".card-footer").empty();
                var element = $($("#video-Temp").html());
                element.addClass("footerVid");
                template.find(".card-footer").append(element);
            }


            if ($("#formFooter-leftCheckbox").is(":checked")) {
                template.find(".card-footer").removeClass("text-center");
                template.find(".card-footer").removeClass("pull-right");
                template.find(".card-footer").addClass("pull-left");
            }
            else if ($("#formFooter-middleCheckbox").is(":checked")) {
                template.find(".card-footer").addClass("text-center");
                template.find(".card-footer").removeClass("pull-right");
                template.find(".card-footer").removeClass("pull-left");
            }
            else if ($("#formFooter-rightCheckbox").is(":checked")) {
                template.find(".card-footer").removeClass("text-center");
                template.find(".card-footer").addClass("pull-right");
                template.find(".card-footer").removeClass("pull-left");
            }

            return template;
        }

        function _populateCard(builtTemplate) {
            if ($("#formHeader-textCheckbox").is(":checked")) {
                builtTemplate.find(".headerText").text($("#headerText").val());
            }
            else if ($("#formHeader-imageCheckbox").is(":checked")) {
                builtTemplate.find(".headerImg").attr("src", $("#headerUrl").val());
            }
            else if ($("#formHeader-videoCheckbox").is(":checked")) {
                builtTemplate.find(".headerVid").attr("src", $("#headerUrl").val());
            }


            if ($("#formContent-textCheckbox").is(":checked")) {
                builtTemplate.find(".contentText").text($("#contentText").val());
            }
            else if ($("#formContent-imageCheckbox").is(":checked")) {
                builtTemplate.find(".contentImg").text($("#contentUrl").val());
            }
            else if ($("#formContent-videoCheckbox").is(":checked")) {
                builtTemplate.find(".contentVid").text($("#contentUrl").val());
            }


            if ($("#formFooter-textCheckbox").is(":checked")) {
                builtTemplate.find(".footerText").text($("#footerText").val());
            }
            else if ($("#formFooter-imageCheckbox").is(":checked")) {
                builtTemplate.find(".footerImg").text($("#footerUrl").val());
            }
            else if ($("#formFooter-videoCheckbox").is(":checked")) {
                builtTemplate.find(".footerVid").text($("#footerUrl").val());
            }

            return builtTemplate;
        }

        function _validateForm(selector) {
            $.validator.setDefaults({ debug: true });
            $(selector).validate({
                rules: {
                    "size": {
                        required: true
                    },
                    "headerType": {
                        required: true
                    },
                    "headerContent": {
                        required: true,
                        minlength: 10,
                        maxlength: 2000
                    },
                    "mainType": {
                        required: true
                    },
                    "mainContent": {
                        required: true,
                        minlength: 10,
                        maxlength: 2000
                    }
                },
                errorPlacement: function (error, element) {
                    $(element).parent('div').addClass('has-error');
                }
            });
            if (!$(selector).valid()) { return false; }
            else {
                if (!vm.headerType || !vm.headerAlignment || !vm.mainType || !vm.mainAlignment || !vm.footerType || !vm.footerAlignment || !vm.name || !vm.size) {
                    return false;
                }
                else {
                    return true;
                }
            }
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