(function () {

    angular.module(page.APPNAME)
        .directive("render", renderElementDirective);

    renderElementDirective.$inject = ['$window'];

    function renderElementDirective($window) {

        return _returnDirective();

        function _returnDirective() {
            return {
                restrict: "E",
                scope: {
                    html: '@',
                    css: '@',
                    js: '@',
                    appendScriptToBody: '@',
                    reRenderAtSize: '@',
                    params: '='
                },
                link: function (scope, element, attr) {

                    $(document).ready(_render);

                    function _render() {

                        element.append($.parseHTML(_getElement()));
                        _getAndRunJs();

                        if (scope.reRenderAt) {
                            angular.element($window).bind('resize', function () {
                                scope.width = $window.innerWidth;

                                if (scope.width == scope.reRenderAt)
                                    _reRender();

                                scope.$digest();
                            });
                        }
                    }

                    function _getElement() {
                        var result = '',
                            cssUrl = (attr.css[0] === '~') ? attr.css.replace('~', window.location.origin) : (attr.css[0] === '/') ? window.location.origin + attr.css : attr.css,
                            htmlUrl = (attr.html[0] === '~') ? attr.html.replace('~', window.location.origin) : (attr.html[0] === '/') ? window.location.origin + attr.html : attr.html;

                        if (_isValidURL(cssUrl) && (!window.loadedScripts || !window.loadedScripts.includes(cssUrl))) {
                            _getStyleFromUrl(cssUrl);
                            _addUrlToWindow(cssUrl);
                        }
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
                            jsUrl = (attr.js[0] === '~') ? attr.js.replace('~', window.location.origin) : (attr.js[0] === '/') ? window.location.origin + attr.js : attr.js,
                            params = scope.params || {};


                        if (_isValidURL(jsUrl)) {
                            _loadFromSource(jsUrl,
                                (data) => {
                                    js = data;
                                    _runJS(js, params, scope.appendScriptToBody || false);
                                });
                        }
                    }

                    function _reRender() {
                        $(element.children()[0]).remove();
                        element.append($.parseHTML(_getElement()));
                        _getAndRunJs();
                    }
                }
            };
        }

        function _runJS(js, params, appendToBody) {

            //params = params ? params : null;

            var iifeRegex = /(\(function)\W*(([a-zA-Z])*(,)(\s|\S))*(([a-zA-Z])*\))/i,
                paramsRegex = /(?!\(\)|([a-zA-Z])*\(([a-zA-Z])*\))\((([a-zA-Z])*(,)(\s|\S))*(([a-zA-Z])*\))/i,
                funcRegex = /(\(function)\W*/i;


            if (iifeRegex.test(js)) {

                var match = paramsRegex.exec(js)[0];

                if (match) {
                    var parameters = match.slice(1, match.length - 1).split(", ");

                    for (var key of parameters) {

                        if (Object.keys(params).some(a => a === key)) {
                            var value = params[key];
                            if (typeof (value) === 'string')
                                value = "\"" + value + "\"";
                            else if (typeof (value) === 'object')
                                value = JSON.stringify(value);

                            js = js.replaceAt(js.lastIndexOf(key), value, key.length);

                        }
                        else {
                            var n = "null";
                            for (var i = 0; i < key.length - 4; i++)
                                n += " ";

                            js = js.replaceAt(js.lastIndexOf(key), n, key.length);

                        }


                    }
                }
            }

            else {
                js = '(' + js + ')';

            }



            if (appendToBody) {
                var script = $("<script type='text/javascript'></script>");
                script.text(js);
                $('body').append(script);
            }
            else
                eval(js);
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

        function _addUrlToWindow(url) {

            if (url) {
                if (window.loadedScripts)
                    window.loadedScripts.push(url);
                else
                    window.loadedScripts = [url];
            }

        }

    }

})();