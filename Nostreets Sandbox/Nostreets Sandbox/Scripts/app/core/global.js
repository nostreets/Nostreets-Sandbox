var page = {
    APPNAME: "sandbox",
    ngModules: [
        "ui.bootstrap",
        "ngAnimate",
        "ngRoute",
        "toastr",
        "ngSanitize",
        "ngCookies",
        'color.picker'
    ],

    utilities: {

        getStyle: (id) => {
            return document.getElementById(id) ? document.getElementById(id).style : document.querySelector('.ct-series-a').style;
        },

        writeStyles: (styleName, cssRules) => {
            var styleElement = document.getElementById(styleName);
            var pastCssRules = (styleElement && styleElement.textContent) ? styleElement.textContent : null;


            if (styleElement) {
                document.getElementsByTagName('head')[0].removeChild(
                    styleElement);
            }

            styleElement = document.createElement('style');
            styleElement.type = 'text/css';
            styleElement.id = styleName;


            if (cssRules.length) {
                for (var css of cssRules) {
                    styleElement.appendChild(document.createTextNode(css));
                }
            }
            else {
                styleElement.innerHTML = cssRules;
            }

            document.getElementsByTagName('head')[0].appendChild(styleElement);
        },

        doesUrlExist: (method, url) => {

            var xhr = new XMLHttpRequest();
            if ("withCredentials" in xhr) {
                // XHR for Chrome/Firefox/Opera/Safari.
                xhr.open(method, url, true);
            } else if (typeof XDomainRequest != "undefined") {
                // XDomainRequest for IE.
                xhr = new XDomainRequest();
                xhr.open(method, url);
            } else {
                // CORS not supported.
                xhr = null;
            }
            return xhr;


        },

        getRandomColor: () => {
            var letters = '0123456789ABCDEF';
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color;
        },

        getInactiveTime: () => {
            var t;
            window.onload = resetTimer;
            document.onload = resetTimer;
            document.onmousemove = resetTimer;
            document.onmousedown = resetTimer; // touchscreen presses
            document.ontouchstart = resetTimer;
            document.onclick = resetTimer;     // touchpad clicks
            document.onscroll = resetTimer;    // scrolling with arrow keys
            document.onkeypress = resetTimer;

            var logout = () => {
                console.log("Inactive action...")
            }

            var resetTimer = () => {
                clearTimeout(t);
                t = setTimeout(logout, 3000)
            }
        },

        getProviders: () => {
            angular.module(page.APPNAME)['_invokeQueue'].forEach(function (value) {
                console.log(value[1] + ": " + value[2][0]);
            });
        }
    }
};

(function () {


    angular.module(page.APPNAME, page.ngModules);

    page.baseController = angular.module(page.APPNAME)
        .factory("$baseController", baseController);

    baseController.$inject = ['$document', '$systemEventService', '$alertService', "$window", '$uibModal', '$timeout', '$http', '$sce', '$cookies', '$q'];

    function baseController($document, $systemEventService, $alertService, $window, $uibModal, $timeout, $http, $sce, $cookies, $q) {

        //PUBLIC
        var base = {
            document: $document,
            systemEventService: $systemEventService,
            alert: $alertService,
            window: $window,
            modal: $uibModal,
            timeout: $timeout,
            http: $http,
            sce: $sce,
            cookies: $cookies,
            Q: $q
        }

        base.tryAgain = function (maxLoops, miliseconds, promiseMethod, onSuccess) {

            if (onSuccess === null) { onSuccess = (data) => console.log(data); }
            var root = {};

            root.promiseMethod = promiseMethod;
            root.currentIndex = 0;

            _start();

            function _start() {
                var method = () => _repeatUntilSuccessful(root.promiseMethod(), miliseconds, maxLoops, data => onSuccess(data));
                base.timeout(method, miliseconds);
            }

            function _repeatUntilSuccessful(promise, time, maxLoops, callback = null) {

                function delay(time, val) {
                    return new Promise(function (resolve) {
                        setTimeout(function () {
                            resolve(val);
                        }, time);
                    });
                }

                function success(data) {
                    if (callback !== null) {
                        callback(data);
                    }
                    return;
                }

                function error(data) {
                    if (root.currentIndex >= maxLoops) { return; }
                    else {
                        return delay(time).then(
                            () => { root.currentIndex++; _repeatUntilSuccessful(root.promiseMethod(), time, maxLoops, callback); },
                            () => { root.currentIndex++; _repeatUntilSuccessful(root.promiseMethod(), time, maxLoops, callback); }
                        );
                    }
                }

                base.Q.when(promise, success, error);

            }

        }

        base.legacyLoginPopup = function () {

            var alert = (onSuccess, onError) => {
                return swal({
                    title: "Enter your session's username",
                    type: "info",
                    input: "text",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    allowOutsideClick: true,
                    inputPlaceholder: "Type in your username!",
                    preConfirm: function (inputValue) {
                        return new Promise(function (resolve, reject) {
                            if (inputValue === false || inputValue === "") {
                                reject("You need to write something!");
                            }
                            else {
                                resolve();
                            }
                        });
                    }
                }).then(onSuccess, onError);
            };

            var brodcastUpdate = (input, onSuccess, onError) => {
                return login(input).then(
                    (data) => {
                        if (base.cookies.get("loggedIn")) {
                            page.user.loggedIn = true;
                            base.systemEventService.broadcast("refreshedUsername");
                        }
                    }).then(onSuccess, onError);
            };

            var login = (username, password) => {
                if (!username) { username = "GUEST" }
                if (!password) { password = "NOSTREETSSANDBOX" }

                return $http({
                    url: "/api/user" + "?username=" + username + "&password=" + password,
                    method: "GET",
                    headers: { 'Content-Type': 'application/json' }
                });
            };

            alert(

                input => brodcastUpdate(input, null,
                    () => base.tryAgain(2, 3000, () => brodcastUpdate(input, null, null))
                ),


                () => base.tryAgain(4, 5000,
                    () => alert(input => brodcastUpdate(input, null,
                        () => base.tryAgain(2, 3000, () => brodcastUpdate(input, null, null))
                    ))
                )
            );
        }

        base.errorCheck = function (err, tryAgainObj) {

            if (!tryAgainObj)
                tryAgainObj = {};


            if (!tryAgainObj.maxLoops)
                tryAgainObj.maxLoops = 3;


            if (!tryAgainObj.miliseconds)
                tryAgainObj.miliseconds = 1000;


            if (!tryAgainObj.promiseMethod)
                tryAgainObj.promiseMethod = () => {
                    return new Promise((resolve, reject) => {
                        resolve();
                    });
                }


            if (!tryAgainObj.onSuccess) 
                tryAgainObj.onSuccess = (data) => console.log(data);
            

            if (err.data.errors && err.data.errors.length) {
                for (var error of err.data.errors) {
                    switch (error) {
                        //case "User is not logged in...":
                        //    base.loginPopup();
                        //    break;

                        default:
                            if (!tryAgainObj || !tryAgainObj.maxLoops || !tryAgainObj.miliseconds || !tryAgainObj.promiseMethod)
                                return;
                            else
                                base.tryAgain(tryAgainObj.maxLoops
                                    , tryAgainObj.miliseconds
                                    , tryAgainObj.promiseMethod
                                    , tryAgainObj.onSuccess);
                            break;
                    }
                }
            }
            else {
                switch (err.status) {
                    //case 401:
                    //    base.loginPopup();
                    //    break;

                    default:
                        if (!tryAgainObj || !tryAgainObj.maxLoops || !tryAgainObj.miliseconds || !tryAgainObj.promiseMethod) { return; }
                        base.tryAgain(tryAgainObj.maxLoops
                            , tryAgainObj.miliseconds
                            , tryAgainObj.promiseMethod
                            , tryAgainObj.onSuccess);
                        break;
                }
            }
        }

        return base;
    }

})();
