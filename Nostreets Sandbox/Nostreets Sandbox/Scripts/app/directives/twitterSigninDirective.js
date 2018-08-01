(function () {


    angular.module(page.APPNAME)
        .directive('twitterSignIn', twitterSignIn);

    function twitterSignIn($timeout, $parse) {
        return {
            restrict: 'A',
            scope: {
                clientId: '=',
                signinSuccess: '&?',
                signinFailure: '&?'
            },
            link: function ($scope, element, attr) {

                _render();

                function _render() {
                    _setUp();

                    if ($scope.clientId)
                        _twitterInit($scope.clientId);
                    else
                        throw "Must provide the twitterSignIn directive with a client-id attribute...";
                }

                function _setUp() {

                    $scope.request = null;
                    $scope.signinSuccess = $scope.signinSuccess || ((data) => { console.log(data); });
                    $scope.signinFailure = $scope.signinFailure || ((err) => { console.log(err); });

                }

                function _handlers()
                {
                    $(element[0]).on('click', () => {
                        _twitterSigninPopup();
                    });
                }

                function _twitterInit(clientId) {
                    twttr.init({
                        api_key: clientId,
                        request_url: window.location.origin
                    });
                }

                function _twitterSigninPopup() {

                    twttr.connect((response) => {
                        if (response.success) {
                            $scope.request = response;

                            $.ajax({
                                url: 'https://api.twitter.com/account/verify_credentials',
                                contentType: 'application/json',
                                data: response,
                                method: 'POST',
                                success: $scope.signinSuccess,
                                error: $scope.signinFailure
                            });

                        }
                        else {
                            console.log("Twitter Login Error");
                            console.log(response);
                        }

                    });
                }

            }
        }
    }

})();