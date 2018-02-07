(function () {
    angular.module(page.APPNAME).directive("tooltip", toolTipDirective);
    function toolTipDirective() {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).hover(function () {
                    // on mouseenter
                    $(element).tooltip('show');
                }, function () {
                    // on mouseleave
                    $(element).tooltip('hide');
                });
            }
        };
    }
})();