
return function (id, color, name, title, desc, imgUrl, socials) {


    function _render() {
        _setUp();
        _handlers();
    }

    function _setUp() {

        var card = $("render > .material-card"),
            linksAdded = 0,
            cardFooter = card.find(".mc-footer"),
            platforms = ["facebook", "linkedin", "google-plus", "twitter", "instagram", "youtube", "github"];

        card.addClass(color);
        card.find(".card-name").text(name);
        card.find(".card-title").text(title);
        card.find(".card-description").text(desc);
        card.find(".card-img").attr("src", imgUrl);



        for (var platform of platforms) {

            var linkTag = $('<a class="fa fa-fw"></a>');
            linkTag.addClass(".fa-" + platform);

            if (linksAdded > 5)
                cardFooter.css({ height: "150px" });

            if (socials[platform]) {
                linkTag.attr("href", socials[platform]);
                cardFooter.append(linkTag);
            }

        }
    }

    function _handlers() {
        $('.material-card > .mc-btn-action').click(
            () => {
                var card = $(this).parent('.material-card');
                var icon = $(this).children('i');
                icon.addClass('fa-spin-fast');

                if (card.hasClass('mc-active')) {
                    card.removeClass('mc-active');

                    window.setTimeout(function () {
                        icon
                            .removeClass('fa-arrow-left')
                            .removeClass('fa-spin-fast')
                            .addClass('fa-bars');

                    }, 800);
                } else {
                    card.addClass('mc-active');

                    window.setTimeout(function () {
                        icon
                            .removeClass('fa-bars')
                            .removeClass('fa-spin-fast')
                            .addClass('fa-arrow-left');

                    }, 800);
                }
            });
    }

}