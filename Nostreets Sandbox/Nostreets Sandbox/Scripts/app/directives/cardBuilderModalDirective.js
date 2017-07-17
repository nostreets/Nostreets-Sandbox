//(function () {

//    angular.module(page.APPNAME)
//        .directive("cardBuilder", modalDirective)

//    modalDirective.$inject = ['$baseController'];

//    function modalDirective($baseController) {
//        return {
//            restrict: "A",
//            scope: true,
//            link: function ($scope, element, attr) {

//                _exec();

//                function _exec() {

//Globals
var dir = {
    handlers: {}
};

//Startup
$/*baseController.*/(window).on("load", function () {
    dir.cmsTemp = null;
    dir.cards = [];

    $(".card-builder-formSubmit").on("click", dir.handlers.btnSumbit);

    $("#formHeader-textCheckbox").on("click", showHeaderTextInput);
    $("#formHeader-imageCheckbox").on("click", showHeaderUrlInput);
    $("#formHeader-videoCheckbox").on("click", showHeaderUrlInput);

    $("#formContent-textCheckbox").on("click", showContentTextInput);
    $("#formContent-imageCheckbox").on("click", showContentUrlInput);
    $("#formContent-videoCheckbox").on("click", showContentUrlInput);

    $("#formFooter-textCheckbox").on("click", showFooterTextInput);
    $("#formFooter-imageCheckbox").on("click", showFooterUrlInput);
    $("#formFooter-videoCheckbox").on("click", showFooterUrlInput);

    function showHeaderTextInput() { $("#headerText").removeClass("hidden"); $("#headerUrl").addClass("hidden"); }
    function showHeaderUrlInput() { $("#headerUrl").removeClass("hidden"); $("#headerText").addClass("hidden"); }

    function showContentTextInput() { $("#contentText").removeClass("hidden"); $("#contentUrl").addClass("hidden"); }
    function showContentUrlInput() { $("#contentUrl").removeClass("hidden"); $("#contentText").addClass("hidden"); }

    function showFooterTextInput() { $("#footerText").removeClass("hidden"); $("#footerUrl").addClass("hidden"); }
    function showFooterUrlInput() { $("#footerUrl").removeClass("hidden"); $("#footerText").addClass("hidden"); }
});

//Btn Handlers
dir.handlers.btnSumbit = function () {
    swal({
        title: "Do you want to save this card?",
        showCancelButton: true
    }).then(function () {
        var card = dir.loadPreBuiltCard();
        card = dir.buildCard(card);
        card = dir.populateCard(card);
        console.log(card);
        dir.cards.push(card);
        dir.populatePage();
        $(".card-builder-formModal").modal("hide");
    });
}

//Client Side Functions
dir.loadPreBuiltCard = function () {
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

dir.buildCard = function (template) {
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

dir.populateCard = function (builtTemplate) {
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

dir.populatePage = function (selector) {
    if (dir.cards && dir.cards[0]) {
        //for (var card of dir.cards) {
        //    $(selector).append(card);
        //}
        page.baseController.systemEventService.brodcast("builtedCards", div.cards);
    }
}

//                    //Event Relays
//                    $baseController.systemEventService.listen("submitCard", dir.handlers.btnSumbit);

//                }
//            }
//        }
//    }

//})();