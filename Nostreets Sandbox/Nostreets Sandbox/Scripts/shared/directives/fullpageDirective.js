(function () {
    'use strict';

    angular
        .module('fullPage.js', [])
        .directive('fullPage', fullPageDirective);

    fullPageDirective.$inject = ['$timeout'];

    function fullPageDirective($timeout) {

        var directive = {
            restrict: 'A',
            scope: {
                options: '='
            },
            link: function (scope, element, attr) {

                _render();

                function _render() {
                    _setUp();
                    _handlers();
                }

                function _setUp() {

                    this.pageIndex;
                    this.slideIndex;
                    this.afterRender;
                    this.onLeave;
                    this.onSlideLeave;

                    $(element).attr();


                    if (typeof scope.options === 'object') {
                        if (scope.options.afterRender)
                            this.afterRender = scope.options.afterRender;

                        if (scope.options.this.onLeave)
                            this.onLeave = scope.options.onLeave;

                        if (scope.options.this.onSlideLeave)
                            this.onSlideLeave = scope.options.onSlideLeave;
                    }
                    else if (typeof options === 'undefined')
                        scope.options = {};
                }

                function _handlers() {
                    scope.$watch(_watchNodes, rebuild);
                    scope.$watch('options', _rebuild, true);
                    element.on('$destroy', _destroyFullPage);
                }

                function _rebuild() {
                    destroyFullPage();

                    $(element).fullpage(_sanatizeOptions(scope.options));

                    if (typeof this.afterRender === 'function') {
                        this.afterRender();
                    }
                }

                function _destroyFullPage() {
                    if ($.fn.fullpage.destroy) {
                        $.fn.fullpage.destroy('all');
                    }
                }

                function _sanatizeOptions(options) {
                    options.afterRender = afterAngularRender;
                    options.onLeave = onAngularLeave;
                    options.onSlideLeave = onAngularSlideLeave;

                    function afterAngularRender() {
                        //We want to remove the HREF targets for navigation because they use hashbang
                        //They still work without the hash though, so its all good.
                        if (options && options.navigation) {
                            $('#fp-nav').find('a').removeAttr('href');
                        }

                        if (this.pageIndex) {
                            $timeout(function () {
                                $.fn.fullpage.silentMoveTo(this.pageIndex, this.slideIndex);
                            });
                        }
                    }

                    function onAngularLeave(page, next, direction) {

                        if (typeof this.onLeave === 'function' && this.onLeave(page, next, direction) === false) {
                            return false;
                        }
                        this.pageIndex = next;

                    }

                    function onAngularSlideLeave(anchorLink, page, slide, direction, next) {

                        if (typeof this.onSlideLeave === 'function' && this.onSlideLeave(anchorLink, page, slide, direction, next) === false) {
                            return false;
                        }
                        this.pageIndex = page;
                        this.slideIndex = next;

                    }

                    //if we are using a ui-router, we need to be able to handle anchor clicks without 'href="#thing"'
                    $(document).on('click', '[data-menuanchor]', function () {
                        $.fn.fullpage.moveTo($(this).attr('data-menuanchor'));
                    });

                    return options;
                }

                function _watchNodes() {
                    return element[0].getElementsByTagName('*').length;
                }

                function _removeSlide(sectionIndex, slideIndex) {

                    var section = $(element).find('.section').eq(sectionIndex);
                    $(section).find('.slide').eq(slideIndex).remove();

                    //remembering the active section / slide
                    var activeSectionIndex = $('.fp-section.active').index();
                    var activeSlideIndex = $('.fp-section.active').find('.slide.active').index();


                    _destroyFullPage();


                    //setting the active section as before
                    $('.section').eq(activeSectionIndex).addClass('active');

                    //were we in a slide? Adding the active state again
                    if (activeSlideIndex > -1)
                        $('.section.active').find('.slide').eq(activeSlideIndex || 0).addClass('active');
                }

                function _addSlide(sectionIndex, html) {

                    var section = $(element).find('.section').eq(sectionIndex);
                    $(section).append('<div class="slide">' + html + '</div>');

                    //remembering the active section / slide
                    var activeSectionIndex = $('.fp-section.active').index();
                    var activeSlideIndex = $('.fp-section.active').find('.slide.active').index();


                    _destroyFullPage();


                    //setting the active section as before
                    $('.section').eq(activeSectionIndex).addClass('active');

                    //were we in a slide? Adding the active state again
                    if (activeSlideIndex > -1)
                        $('.section.active').find('.slide').eq(activeSlideIndex || 0).addClass('active');

                }

                function _removeSection(index) {

                    $(element).find('.section').eq(index).remove();

                    //remembering the active section / slide
                    var activeSectionIndex = $('.fp-section.active').index();
                    var activeSlideIndex = $('.fp-section.active').find('.slide.active').index();


                    _destroyFullPage();


                    //setting the active section as before
                    $('.section').eq(activeSectionIndex).addClass('active');

                    //were we in a slide? Adding the active state again
                    if (activeSlideIndex > -1)
                        $('.section.active').find('.slide').eq(activeSlideIndex || 0).addClass('active');
                }

                function _addSection(html) {

                    $(element).append('<div class="section">' + html + '</div>');

                    //remembering the active section / slide
                    var activeSectionIndex = $('.fp-section.active').index();
                    var activeSlideIndex = $('.fp-section.active').find('.slide.active').index();


                    _destroyFullPage();


                    //setting the active section as before
                    $('.section').eq(activeSectionIndex).addClass('active');

                    //were we in a slide? Adding the active state again
                    if (activeSlideIndex > -1)
                        $('.section.active').find('.slide').eq(activeSlideIndex || 0).addClass('active');


                }
            }
        };


        return directive;
    }

})();
