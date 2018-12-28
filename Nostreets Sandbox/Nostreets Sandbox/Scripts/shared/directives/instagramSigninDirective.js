(function () {


    angular.module(page.APPNAME)
        .directive('instagramSignIn', instagramSignIn);

    function instagramSignIn() {
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
                    _handlers();

                    if (!$scope.clientId)
                        throw "Must provide the googleSignIn directive with a client-id attribute...";
                }

                function _setUp() {

                    $scope.accessToken = null;

                    $scope.signinSuccess = $scope.signinSuccess || ((data) => { console.log(data); });
                    $scope.signinFailure = $scope.signinFailure || ((err) => { console.log(err); });

                }

                function _handlers() {
                    $(element[0]).on('click', () => {
                        _instaPopup($scope.clientId);
                    });
                }


                function _instaPopup(instagramClientId) {

                    // Pop-up window size, change if you want
                    var popupWidth = 700,
                        popupHeight = 500,
                        popupLeft = (window.screen.width - popupWidth) / 2,
                        popupTop = (window.screen.height - popupHeight) / 2;

                    // Url needs to point to instagram_auth.php
                    var popup = window.open(
                        'https://instagram.com/oauth/authorize/?client_id=' + instagramClientId + '&redirect_uri=' + window.location.origin + '&response_type=token'
                        , ''
                        , 'width=' + popupWidth + ',height=' + popupHeight + ',left=' + popupLeft + ',top=' + popupTop + '');


                    popup.onload = function () {

                        var interval = setInterval(() => {
                            try {

                                // Check if hash exists
                                if (popup.location.hash.length) {

                                    clearInterval(interval);
                                    $scope.accessToken = popup.location.hash.slice(17); //slice #access_token= from string
                                    popup.close();

                                    $.ajax({
                                        type: "GET",
                                        dataType: "jsonp",
                                        url: "https://api.instagram.com/v1/users/self/?access_token=" + $scope.accessToken,
                                        success: (response) => {

                                            if (response.meta && response.meta.code === 200)
                                                $scope.signinSuccess(response.data);
                                            else
                                                $scope.signinFailure(response);

                                        }
                                    });

                                }
                            }
                            catch (evt) {
                                // Permission denied
                                $scope.signinFailure(evt);
                            }
                        }, 100);
                    };


                }


            }
        }
    }

})();