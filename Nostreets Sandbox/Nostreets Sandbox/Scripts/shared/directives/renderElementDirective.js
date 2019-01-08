(function () {

    angular.module(page.APPNAME)
        .directive("render", renderElementDirective);


    function renderElementDirective() {

        return _returnDirective();

        function _returnDirective() {
            return {
                restrict: "E",
                scope: {
                    html: '@',
                    css: '@',
                    js: '@'
                },
                link: function (scope, element, attr) {


                    $(document).ready(_render);

                    function _render() {

                        element.append($.parseHTML(_getElement()));

                        _runJavascript();
                    }

                    function _getElement() {
                        var result = '',
                            cssUrl = (attr.css[0] === '~') ? attr.css.replace('~', window.location.origin) : (attr.css[0] === '/') ? window.location.origin + attr.css : attr.css,
                            htmlUrl = (attr.html[0] === '~') ? attr.html.replace('~', window.location.origin) : (attr.html[0] === '/') ? window.location.origin + attr.html : attr.html;

                        if (_isValidURL(cssUrl))
                            _getStyleFromUrl(cssUrl);
                        else
                            result = '<style>' + attr.css + '</style>';


                        if (_isValidURL(htmlUrl))
                            _loadFromSource(htmlUrl,
                                (data) => {
                                    result += data;
                                }, 'html');
                        else
                            result += attr.html;


                        return result;
                    }

                    function _runJavascript() {
                        var jsUrl = (attr.js[0] === '~') ? attr.js.replace('~', window.location.origin) : (attr.js[0] === '/') ? window.location.origin + attr.js : attr.js,
                            js = '';
                        if (_isValidURL(jsUrl)) {
                            _loadFromSource(jsUrl,
                                (data) => {
                                    js = data;
                                    eval('(' + js + ')');
                                    //JSON.parse(js, () => eval('(function () {' + js + '})();'));
                                });
                        }
                    }

                }
            };
        }

        function _isValidURL(url) {
            var urlPattern = "(https?|ftp)://(www\\.)?(((([a-zA-Z0-9.-]+\\.){1,}[a-zA-Z]{2,4}|localhost))|((\\d{1,3}\\.){3}(\\d{1,3})))(:(\\d+))?(/([a-zA-Z0-9-._~!$&'()*+,;=:@/]|%[0-9A-F]{2})*)?(\\?([a-zA-Z0-9-._~!$&'()*+,;=:/?@]|%[0-9A-F]{2})*)?(#([a-zA-Z0-9._-]|%[0-9A-F]{2})*)?";

            urlPattern = "^" + urlPattern + "$";
            var regex = new RegExp(urlPattern);

            return regex.test(url);

        }

        function _loadFromSource(url, callback, dataType) {
            $.ajax({
                url: url,
                dataType: dataType,
                success: callback,
                async: false
            });
        }

        function _getStyleFromUrl(url) {

            var head = document.getElementsByTagName('head')[0];
            var link = document.createElement('link');
            link.id = 'renderedStyle_' + _randomString(10);
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = url;
            link.media = 'all';
            head.appendChild(link);

        }

        function _randomString(len) {
            if (typeof (len) !== 'number')
                throw "parameter len has to be a interger...";

            var anysize = len;
            var charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            result = "";
            for (var i = 0; i < anysize; i++)
                result += charset[Math.floor(Math.random() * charset.length)];

            return result;
        }

    }


})();