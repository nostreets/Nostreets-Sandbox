(function () {

    angular.module(page.APPNAME).controller("cardBuilderController", cardController);

    cardController.$inject = ['$scope', '$baseController', '$uibModal', '$oblApiService'];

    function cardController($scope, $baseController, $uibModal, $oblApiService) {

        var vm = this;
        vm.submitCard = _btnSumbit;
        vm.buildCard = _buildCard;
        vm.populateCard = _populateCard;
        vm.loadPreBuiltCard = _loadPreBuiltCard;
        vm.validateForm = _validateForm;
        vm.viewCode = _viewCode;
        vm.selectElement = _selectElement;
        vm.activateMode = _activateMode;
        vm.closeBuilder = _closeBuilder;



        _render();

        function _render() {
            _setUp();
            _handlers();
            _getUserData();
        }

        function _setUp() {
            vm.headerType = null;
            vm.mainType = null;
            vm.footerType = null;
            vm.headerAlignment = null;
            vm.mainAlignment = null;
            vm.footerAlignment = null;
            vm.headerContent = null;
            vm.mainContent = null;
            vm.footerContent = null;
            vm.isLoggedIn = page.isLoggedIn;


            if (!vm.cards) {
                vm.cards = [];
            }
        }

        function _handlers() {
            $baseController.defaultListeners($scope,
                {
                    "loggedIn": () => {
                        _setUp();
                        _getUserData();
                    }
                }
            );

            //$baseController.event.listen("loggedIn", () => {
            //    _setUp();
            //    _getUserData();
            //});
        }

        function _getUserData() {

            $oblApiService.getAllCardsByUser().then(
                (response) => _cardResponse(response),
                (err) => {
                    $baseController.errorCheck(err,
                        {
                            promiseMethod: () => $oblApiService.getAllCardsByUser(),
                            onSuccess: (response) => _cardResponse(response)
                        }
                    )
                }
            );
        }

        function _cardResponse(data) {
            vm.cards = [];
            if (data.data.items) {
                for (let item of data.data.items) {
                    item.html = $baseController.sce.trustAsHtml(item._HTML);
                    vm.cards.push(item);
                }
            }
        }

        function _viewCode() {
            $baseController.http({
                url: "api/view/code/cardController",
                method: "GET",
                responseType: "JSON"
            }).then(function (data) {
                _openCodeModal(data.data.item);
            });
        }

        function _openCodeModal(code) {
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

        function _activateMode(mode) {
            vm.deleteMode = false;
            vm.updateMode = false;

            if (mode === "delete") {
                vm.deleteMode = true;
            }
            else if (mode === "update") {
                vm.updateMode = true;
            }
        }

        function _chooseCard(index) {
            vm.id = vm.cards[index].id;
            vm.name = vm.cards[index].name;
            vm.size = vm.cards[index].size;
            vm.headerType = vm.cards[index].headerType;
            vm.headerAlignment = vm.cards[index].headerAlignment;
            vm.mainType = vm.cards[index].mainType;
            vm.mainAlignment = vm.cards[index].mainAlignment;
            vm.footerType = vm.cards[index].footerType;
            vm.footerAlignment = vm.cards[index].footerAlignment;

            var html = $($.parseHTML(vm.cards[index]._HTML));
            var h_type = vm.headerType === 1 ? "Text" : vm.headerType === 2 ? "Img" : "Vid";
            var m_type = vm.mainType === 1 ? "Text" : vm.headerType === 2 ? "Img" : "Vid";
            var f_type = vm.footerType === 1 ? "Text" : vm.headerType === 2 ? "Img" : "Vid";

            vm.headerContent = html.find(".header" + h_type).text();
            vm.mainContent = html.find(".content" + m_type).text();
            vm.footerContent = html.find(".footer" + f_type).text();


        }

        function _deleteCard(index) {
            swal({
                title: "Do you want to delete this card?",
                showCancelButton: true
            }).then(
                () => {
                    $oblApiService.deleteCard(vm.cards[index].id).then(
                        () => $oblApiService.getAllCardsByUser().then(
                            (response) => _cardResponse(response),
                            (err) => {
                                $baseController.errorCheck(err,
                                    {
                                        promiseMethod: () => $oblApiService.getAllCardsByUser(),
                                        onSuccess: (response) => _cardResponse(response)
                                    }
                                )
                            }
                        ),
                        (err) => {
                            $baseController.errorCheck(err,
                                {
                                    promiseMethod: () => $oblApiService.deleteCard(vm.cards[index].id),
                                    onSuccess: () => $oblApiService.getAllCardsByUser().then(
                                        (response) => _cardResponse(response),
                                        (err) => {
                                            $baseController.errorCheck(err,
                                                {
                                                    promiseMethod: () => $oblApiService.getAllCardsByUser(),
                                                    onSuccess: (response) => _cardResponse(response)
                                                }
                                            )
                                        }
                                    )
                                }
                            )
                        });

                },
                () => { $scope.$apply(() => { vm.deleteMode = false; }); });
        }

        function _selectElement(index) {

            if (vm.deleteMode) {
                _deleteCard(index);
            }
            else if (vm.updateMode) {
                _chooseCard(index);
                $(".card-builder-formModal").modal("show");
            }
        }

        function _btnSumbit() {
            if (_validateForm("#cardBuilderForm")) {
                swal({
                    title: "Do you want to save this card?",
                    showCancelButton: true
                }).then(
                    () => {
                        var card = vm.loadPreBuiltCard();
                        card = vm.buildCard(card);
                        card = vm.populateCard(card);

                        var lastestCard = {
                            id: vm.id || 0,
                            name: vm.name,
                            size: vm.size,

                            headerType:
                            vm.headerType === "text"
                                ? 1
                                : vm.headerType === "img"
                                    ? 2
                                    : 3,
                            headerAlignment:
                            vm.headerAlignmentId === "right"
                                ? 1
                                : vm.headerAlignmentId === "center"
                                    ? 2
                                    : 3,
                            mainType:
                            vm.mainType === "text"
                                ? 1
                                : vm.mainType === "img"
                                    ? 2
                                    : 3,
                            mainAlignment:
                            vm.mainAlignment === "right"
                                ? 1
                                : vm.mainAlignment === "center"
                                    ? 2
                                    : 3,
                            footerType:
                            vm.footerType === "text"
                                ? 1
                                : vm.footerType === "img"
                                    ? 2
                                    : 3,
                            footerAlignment:
                            vm.footerAlignment === "right"
                                ? 1
                                : vm.footerAlignment === "center"
                                    ? 2
                                    : 3,

                            _HTML: card[0].outerHTML,
                            html: $baseController.sce.trustAsHtml(card[0].outerHTML)
                        };


                        var saveCard =
                            (!vm.isLoggedIn)
                                ? () => {
                                    return new Promise((resolve) => {
                                        vm.cards.push(item);
                                        resolve();
                                    });
                                }
                                : (!vm.updateMode)
                                    ? (model) => { return $oblApiService.insertCard(model).then(getUserCards); }
                                    : (model) => { return $oblApiService.updateCard(model).then(getUserCards); };


                        var getUserCards =
                            () => {
                                return $oblApiService.getAllCardsByUser().then(
                                    (response) => _cardResponse(response),
                                    (err) => {
                                        $baseController.errorCheck(err,
                                            {
                                                promiseMethod: () => $oblApiService.getAllCardsByUser(),
                                                onSuccess: (response) => _cardResponse(response)
                                            }
                                        )
                                    }
                                );
                            }

                        //saveCard(lastestCard).then(
                        //    () => $oblApiService.getAllCardsByUser().then(
                        //        (response) => _cardResponse(response),
                        //        (err) => {
                        //            $baseController.errorCheck(err,
                        //                {
                        //                    promiseMethod: () => $oblApiService.getAllCardsByUser(),
                        //                    onSuccess: (response) => _cardResponse(response)
                        //                }
                        //            )
                        //        }
                        //    ),
                        //    (err) => {
                        //        $baseController.errorCheck(err,
                        //            {
                        //                promiseMethod: () => saveCard(lastestCard),
                        //                onSuccess: () => $oblApiService.getAllCardsByUser.then(
                        //                    (response) => _cardResponse(response),
                        //                    (err) => {
                        //                        $baseController.errorCheck(err,
                        //                            {
                        //                                promiseMethod: () => $oblApiService.getAllCardsByUser(),
                        //                                onSuccess: (response) => _cardResponse(response)
                        //                            }
                        //                        )
                        //                    }
                        //                )
                        //            }
                        //        )
                        //    });

                        _closeBuilder();
                    }
                    );
            }
        }

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

            //Header Content
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



            //Header Alignment
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



            //Body Content
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



            //Body Alignment
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



            //Footer Conetnt
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



            //Footer Alignment
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
            //$.validator.setDefaults({ debug: true });
            //$(selector).validate({
            //    rules: {
            //        "size": {
            //            required: true
            //        },
            //        "headerType": {
            //            required: true
            //        },
            //        "headerContent": {
            //            required: true,
            //            minlength: 10,
            //            maxlength: 2000
            //        },
            //        "mainType": {
            //            required: true
            //        },
            //        "mainContent": {
            //            required: true,
            //            minlength: 10,
            //            maxlength: 2000
            //        },
            //        "footerType": {
            //            required: true
            //        },
            //        "footerContent": {
            //            required: true,
            //            minlength: 10,
            //            maxlength: 2000
            //        },
            //    },
            //    errorPlacement: function (error, element) {
            //        $(element).parent('div').addClass('has-error');
            //    }
            //});
            var result = false;

            //if (!$(selector).valid()) {  result = false; }
            //else {
            if (!vm.headerType || !vm.headerAlignment || !vm.mainType || !vm.mainAlignment || !vm.footerType || !vm.footerAlignment || !vm.name || !vm.size) {
                result = false;
            }
            else {
                if (vm.headerType === 2 || vm.headerType === 3 || vm.headerType === "img" || vm.headerType === "vid")
                    if (/^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i.test($("#headerUrl").val()))
                        result = true;
                    else
                        result = false;


                if (vm.mainType === 2 || vm.mainType === 3 || vm.mainType === "img" || vm.mainType === "vid")
                    if (/^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i.test($("#contentUrl").val()))
                        result = true;
                    else
                        result = false;

                if (vm.footerType === 2 || vm.footerType === 3 || vm.footerType === "img" || vm.footerType === "vid")
                    if (/^(http|https|ftp):\/\/[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/i.test($("#footerUrl").val()))
                        result = true;
                    else
                        result = false;

                if (vm.headerType === 1 || vm.headerType === "text")
                    if ($("#headerText").val() && $("#headerText").val().length > 9 && $("#headerText").val().length < 2000)
                        result = true;
                    else
                        result = false;

                if (vm.mainType === 1 || vm.mainType === "text")
                    if ($("#contentText").val() && $("#contentText").val().length > 9 && $("#contentText").val().length < 2000)
                        result = true;
                    else
                        result = false;

                if (vm.footerType === 1 || vm.footerType === "text")
                    if ($("#footerText").val() && $("#footerText").val().length > 9 && $("#footerText").val().length < 2000)
                        result = true;
                    else
                        result = false;

            }
            //}

            return result;
        }

        function _closeBuilder() {
            $(".card-builder-formModal").modal("hide");
            vm.updateMode = false;
        }
    }

})();
