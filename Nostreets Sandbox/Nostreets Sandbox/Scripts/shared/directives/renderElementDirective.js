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
                    js: '@',
                    passElement: "@",
                    params: '@'
                },
                link: function (scope, element, attr) {


                    $(document).ready(_render);

                    function _render() {

                        element.append($.parseHTML(_getElement()));

                        _getAndRunJs();
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

                    function _getAndRunJs() {
                        var js = '',
                            passEle = (scope.passElement === "true" || scope.passElement === "1") ? true : false,
                            jsUrl = (attr.js[0] === '~') ? attr.js.replace('~', window.location.origin) : (attr.js[0] === '/') ? window.location.origin + attr.js : attr.js,
                            params =
                                passEle && scope.params ? [element, scope.params] :
                                    passEle && !scope.params ? [element] :
                                        !passEle && scope.params ? [scope.params] : null;

                        if (_isValidURL(jsUrl)) {
                            _loadFromSource(jsUrl,
                                (data) => {
                                    js = data;
                                    //eval('(' + js + ')');
                                    _runJS(js);

                                });
                        }


                    }

                }
            };
        }

        function _runJS(js, params) {
            var func = null;
            params = Array.isArray(params) ? params : params ? [params] : null;

            if (js.slice(0, 7) === "function")
                func = new Function(() => "{ return " + js + " }");

            else if (js.slice(0, 5) === "return")
                func = new Function(() => "{ " + js + " }");

            else
                func = new Function(js);



            func.call(null, params);
        }

        function _isValidURL(url) {

            return /^(f|ht)tps?:\/\//i.test(url);

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

            if (!$('link').filter((i, ele) => ele.href === url)[0]) {
                var head = document.getElementsByTagName('head')[0];
                var link = document.createElement('link');
                link.id = 'renderedStyle_' + _randomString(10);
                link.rel = 'stylesheet';
                link.type = 'text/css';
                link.href = url;
                link.media = 'all';
                head.appendChild(link);
            }
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