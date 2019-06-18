
page.APPNAME = 'obl-site';
page.ngModules.push('fullPage.js');
page.ngModules.push('ui.router');

page.sliderOptions = {
    licenseKey: '6D8C01FB-3C9B4F3A-93CBFC04-52802A55',
    sectionsColor: [
        'rgba(255, 255, 255, 0.30)'
        , 'rgba(255, 255, 255, 0.30)'
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
    scrollBar: false,

    //Scrolling
    css3: true,
    scrollingSpeed: 800,
    autoScrolling: true,
    fitToSection: true,
    loopBottom: false,
    loopTop: false,
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
        if (path === '#!/home' || path === '#!/entertainment') {
            $('#normal-footer').hide();
        }
    }
    else if (path === '#!/music') {
        $('#normal-footer').show();
        $('#normal-footer').removeClass('fixed-footer');
        $('#normal-footer').addClass('absolute-footer');
    }
    else {
        $('#normal-footer').show();
        $('#normal-footer').removeClass('absolute-footer');
        $('#normal-footer').addClass('fixed-footer');
    }
}

page.renderParticles = (data) => {
    var particles = {

        "particles": {
            "number": {
                "value": 75,
                "density": {
                    "enable": true,
                    "value_area": 800
                }
            },
            "color": {
                "value": "#BB5D5D"
            },
            "shape": {
                "type": "image", //"circle",
                "stroke": {
                    "width": 0,
                    "color": "#000000"
                },
                "polygon": {
                    "nb_sides": 5
                },
                "image": {
                    "src": "assets/img/obl.2.400.png",
                    "width": 100,
                    "height": 100
                }
            },
            "opacity": {
                "value": 0.5,
                "random": true,
                "anim": {
                    "enable": false,
                    "speed": 1,
                    "opacity_min": 0.1,
                    "sync": false
                }
            },
            "size": {
                "value": 10,
                "random": true,
                "anim": {
                    "enable": false,
                    "speed": 40,
                    "size_min": 0.1,
                    "sync": false
                }
            },
            "line_linked": {
                "enable": false,
                "distance": 500,
                "color": "#ffffff",
                "opacity": 0.4,
                "width": 2
            },
            "move": {
                "enable": true,
                "speed": 6,
                "direction": "bottom",
                "random": false,
                "straight": false,
                "out_mode": "out",
                "bounce": false,
                "attract": {
                    "enable": false,
                    "rotateX": 600,
                    "rotateY": 1200
                }
            }
        },
        "interactivity": {
            "detect_on": "canvas",
            "events": {
                "onhover": {
                    "enable": true,
                    "mode": "bubble"
                },
                "onclick": {
                    "enable": true,
                    "mode": "repulse"
                },
                "resize": true
            },
            "modes": {
                "grab": {
                    "distance": 400,
                    "line_linked": {
                        "opacity": 0.5
                    }
                },
                "bubble": {
                    "distance": 400,
                    "size": 4,
                    "duration": 0.3,
                    "opacity": 1,
                    "speed": 3
                },
                "repulse": {
                    "distance": 200,
                    "duration": 0.4
                },
                "push": {
                    "particles_nb": 4
                },
                "remove": {
                    "particles_nb": 2
                }
            }
        },
        "retina_detect": true
    };

    if (data) {
        for (var prop in data) {
            if (particles[prop]) {
                if (typeof (particles[prop]) === 'object') {
                    for (var p in data[prop])
                        if (particles[prop][p])
                            particles[prop][p] = data[prop][p];
                }

                else
                    particles[prop] = data[prop];
            }
        }
    }

    particlesJS('particles-js', particles);
}

page.animate = (element, animation, callback, speedClass) => {

    var node = null;
    if (typeof (element) === 'object')
        node = element.get(0);
    else
        node = document.querySelector(element);

    node.classList.add('animated', animation);
    if (speedClass)
        node.classList.add(speedClass);

    function handleAnimationEnd() {
        node.classList.remove('animated', animation);
        node.removeEventListener('animationend', handleAnimationEnd);

        if (callback && typeof callback === 'function')
            callback();
    }

    node.addEventListener('animationend', handleAnimationEnd);
}

page.startSite(() => {
    angular.element(window).bind('resize', page.fixFooter);
    page.utilities.setUpJQSwipeHandlers();

});