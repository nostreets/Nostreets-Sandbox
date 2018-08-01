// Load the JavaScript SDK asynchronously
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

(function () {

    angular.module(page.APPNAME)
        .directive('facebookSignIn', facebookSignIn);

    function facebookSignIn() {
        return {
            restrict: 'A',
            scope: {
                appId: '=',
                signinSuccess: '&?',
                signinFailure: '&?'
            },
            link: function ($scope, element, attr) {

                _render();

                function _render() {

                    _setUp();
                    _handlers();

                    if ($scope.appId)
                        _fbInit($scope.appId);
                    else
                        throw "Must provide the facebookSignIn directive with a app-id attribute...";
                }

                function _setUp() {
                    $scope.signinSuccess = $scope.signinSuccess || ((data) => { console.log(data); });
                    $scope.signinFailure = $scope.signinFailure || ((err) => { console.log(err); });

                }

                function _handlers() {
                    $(element[0]).on('click', () => {
                        _fbLogin();
                    });
                }

                function _fbInit(appId) {
                    // FB JavaScript SDK configuration and setup
                    FB.init({
                        appId: appId, // FB App ID
                        cookie: true,  // enable cookies to allow the server to access the session
                        xfbml: true,  // parse social plugins on this page
                        version: 'v3.1'
                    });


                }

                function _fbLogin() {

                    // Check whether the user already logged in
                    FB.getLoginStatus(function (response) {
                        if (response.status === 'connected') {
                            _getFbUserData();
                        }
                        else {
                            FB.login((response) => {
                                if (response.authResponse) {
                                    getFbUserData();
                                }
                            }, { scope: 'email' });
                        }
                    });


                }

                function _getFbUserData() {
                    FB.api('/me', { locale: 'en_US', fields: 'id,first_name,last_name,email' }, $scope.signinSuccess);
                }


            }
        }
    }

})();