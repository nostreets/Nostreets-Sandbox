(function () {


    angular.module(page.APPNAME)
        .directive('googleSignIn', googleSignIn);

    function googleSignIn() {
        return {
            restrict: 'A',
            scope: {
                clientId: '=',
                signinSuccess: '&?',
                signinFailure: '&?'
            },
            link: function ($scope, element, attr) {

                _render();

                function _render()
                {
                    _setUp();

                    if ($scope.clientId)
                        _googleInit($scope.clientId);
                    else
                        throw "Must provide the googleSignIn directive with a client-id attribute...";
                }

                function _setUp()
                {
                    $scope.signinSuccess = $scope.signinSuccess || ((data) => { console.log(data); });
                    $scope.signinFailure = $scope.signinFailure || ((err) => { console.log(err); });

                }

                function _googleInit(clientId) {
                    gapi.load('auth2', function () {
                        // Retrieve the singleton for the GoogleAuth library and set up the client.
                        auth2 = gapi.auth2.init({
                            client_id: clientId,
                            cookiepolicy: 'single_host_origin',
                            // Request scopes in addition to 'profile' and 'email'
                            //scope: 'additional_scope'
                        });

                        _googleSigninPopup(element[0]);

                    });
                }

                function _googleSigninPopup(element) {

                    auth2.attachClickHandler(
                          element
                        , {}
                        , (data) => { $scope.signinSuccess({ user: data.w3 }) }
                        , $scope.signinFailure
                    );
                }

            }
        }
    }

})();