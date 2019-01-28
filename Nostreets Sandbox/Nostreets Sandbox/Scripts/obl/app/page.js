
page.APPNAME = 'obl-site';
page.ngModules.push('fullPage.js');
page.ngModules.push('ui.router');
page.ngModules.push('angularSoundManager');

page.sliderOptions = {
    licenseKey: '6D8C01FB-3C9B4F3A-93CBFC04-52802A55',
    sectionsColor: [
        'rgba(255, 255, 255, 0.30)'
        , 'rgba(255, 255, 255, 0.30)'
        , 'rgba(255, 255, 255, 0.30)'
        , 'rgba(255, 255, 255, 0.30)'
    ],
    navigation: true,
    navigationPosition: 'right',
    showActiveTooltip: false,
    slidesNavigation: true,
    slidesNavPosition: 'bottom',
    lockAnchors: true,
    scrollBar: true,

    //Scrolling
    css3: true,
    scrollingSpeed: 500,
    autoScrolling: true,
    fitToSection: true,
    loopBottom: true,
    loopTop: true,
    touchSensitivity: 15,
    normalScrollElementTouchThreshold: 5,

    //Accessibility
    keyboardScrolling: true,
    animateAnchor: true,
    recordHistory: true,

    //Design
    controlArrows: true,
    verticalCentered: true,

    //Custom selectors
    sectionSelector: '.section',
    slideSelector: '.slide',

    lazyLoading: true
};

page.fixFooter = () => {

    var path = window.location.hash,
        width = page.utilities.getDeviceWidth();

    if (width < 768) {
        if (path === '#!/home' || path === '#!/ourTeam') {
            $('#fullpage-footer').show();
            $('#normal-footer').hide();
        }
        else {
            $('#fullpage-footer').hide();
            $('#normal-footer').show();
            $('#normal-footer').removeClass('fixed-footer');
            $('#normal-footer').addClass('absolute-footer');
        }
    }
    else if (path === '#!/music' || path === '#!/video') {
        $('#fullpage-footer').hide();
        $('#normal-footer').show();
        $('#normal-footer').removeClass('fixed-footer');
        $('#normal-footer').addClass('absolute-footer');
    }
    else {
        $('#fullpage-footer').hide();
        $('#normal-footer').show();
        $('#normal-footer').removeClass('absolute-footer');
        $('#normal-footer').addClass('fixed-footer');
    }
}



page.startSite();