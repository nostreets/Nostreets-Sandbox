//# sourceURL=buinessCardConfig.js
(function (id, color, name, title, desc, imgUrl, socials) {

    var vm = {};
    _render();

    function _render() {
        _setUp();
        _handlers();
    }

    function _setUp() {

        var platforms = ["facebook", "linkedin", "google-plus", "twitter", "instagram", "youtube", "github"],
            linksAdded = 0;


        vm.card = $(id + "> .material-card");
        vm.cardFooter = vm.card.find(".mc-footer");


        vm.card.addClass(color);
        vm.card.find(".card-name").text(name);
        vm.card.find(".card-title").text(title);
        vm.card.find(".card-description").text(desc);
        vm.card.find(".card-img").attr("src", imgUrl);



        for (var platform of platforms) {

            var linkTag = $('<a class="fab fa-fw"></a>');
            linkTag.addClass("fa-" + platform);

            if (linksAdded > 5)
                vm.cardFooter.css({ height: "150px" });

            if (socials[platform]) {
                linkTag.attr("href", socials[platform]);
                vm.cardFooter.append(linkTag);
                linksAdded++;
            }

        }
    }

    function _handlers() {
        vm.card.find(".mc-btn-action").click(
            () => {
                console.log('card click!');

                var icon = vm.card.find(".mc-btn-action > i");
                icon.addClass('fa-spin-fast');

                if (vm.card.hasClass('mc-active')) {
                    vm.card.removeClass('mc-active');

                    window.setTimeout(function () {
                        icon
                            .removeClass('fa-arrow-left')
                            .removeClass('fa-spin-fast')
                            .addClass('fa-bars');

                    }, 500);
                }
                else {
                    vm.card.addClass('mc-active');

                    window.setTimeout(function () {
                        icon
                            .removeClass('fa-bars')
                            .removeClass('fa-spin-fast')
                            .addClass('fa-arrow-left');

                    }, 500);
                }
            });
    }

})(id, color, name, title, desc, imgUrl, socials);