(function () {
    angular.module(page.APPNAME)
        .controller("homeController", homeController);


    homeController.$inject = ["$scope", "$baseController", '$location'];


    function homeController($scope, $baseController, $location) {
        var vm = this;

        vm.location = $location;
        _render();

        function _render() {
            _setUp();
            $baseController.defaultListeners($scope);
        }

        function _setUp() {

            vm.particleConfig = {
                "particles": {
                    "number": {
                        "value": 14,
                        "density": {
                            "enable": false,
                            "value_area": 800
                        }
                    },
                    "color": {
                        "value": "#b11414"
                    },
                    "shape": {
                        "type": "circle",
                        "stroke": {
                            "width": 3,
                            "color": "#000"
                        },
                        "polygon": {
                            "nb_sides": 7
                        },
                        "image": {
                            "src": "img/github.svg",
                            "width": 100,
                            "height": 100
                        }
                    },
                    "opacity": {
                        "value": 0.3,
                        "random": true,
                        "anim": {
                            "enable": false,
                            "speed": 1,
                            "opacity_min": 0.1,
                            "sync": false
                        }
                    },
                    "size": {
                        "value": 82.86050237138863,
                        "random": true,
                        "anim": {
                            "enable": true,
                            "speed": 10,
                            "size_min": 40,
                            "sync": true
                        }
                    },
                    "line_linked": {
                        "enable": false,
                        "distance": 173.61248115909999,
                        "color": "#000000",
                        "opacity": 1,
                        "width": 2
                    },
                    "move": {
                        "enable": true,
                        "speed": 3,
                        "direction": "top",
                        "random": true,
                        "straight": false,
                        "out_mode": "out",
                        "bounce": false,
                        "attract": {
                            "enable": true,
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
                            "enable": false,
                            "mode": "push"
                        },
                        "resize": true
                    },
                    "modes": {
                        "grab": {
                            "distance": 400,
                            "line_linked": {
                                "opacity": 1
                            }
                        },
                        "bubble": {
                            "distance": 400,
                            "size": 95.90409590409591,
                            "duration": 2,
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
        }
    }


})();