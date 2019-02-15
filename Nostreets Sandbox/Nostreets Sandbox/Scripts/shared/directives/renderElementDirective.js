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
                    params: '=',
                    debug: '@'
                },
                link: function (scope, element, attr) {

                    $(document).ready(_render);

                    function _render() {

                        _getElement((html) => {
                            element.append($.parseHTML(html));
                            _getAndRunJs((code, params) => {
                                js = _evaluateJS(code, params, scope.debug || false);
                                _runJs(js, scope.appendScriptToBody || false);
                            });
                        });

                        if (scope.reRenderAt)
                            angular.element($window).bind('resize', function () {
                                scope.width = $window.innerWidth;

                                if (scope.width == scope.reRenderAt)
                                    _reRender();

                                scope.$digest();
                            });

                    }

                    function _getElement(callback) {

                        var result = '',
                            cssUrl = (attr.css[0] === '~') ? attr.css.replace('~', window.location.origin) : (attr.css[0] === '/') ? window.location.origin + attr.css : (attr.css) ? attr.css : "",
                            htmlUrl = (attr.html[0] === '~') ? attr.html.replace('~', window.location.origin) : (attr.html[0] === '/') ? window.location.origin + attr.html : (attr.html) ? attr.html : "";

                        if (_isValidURL(cssUrl) && (!window.loadedScripts || !window.loadedScripts.includes(cssUrl))) {
                            _getStyleFromUrl(cssUrl);
                            _addUrlToWindow(cssUrl);
                        }
                        else if (attr.css)
                            result = '<style>' + attr.css + '</style>';


                        if (_isValidURL(htmlUrl))
                            _loadFromSource(htmlUrl, callback, 'html');
                        else
                            callback(attr.html);
                    }

                    function _getAndRunJs(callback) {
                        var jsUrl = (attr.js[0] === '~') ? attr.js.replace('~', window.location.origin) : (attr.js[0] === '/') ? window.location.origin + attr.js : (attr.js) ? attr.js : "",
                            params = scope.params || {};


                        if (_isValidURL(jsUrl))
                            _loadFromSource(jsUrl, (code) => callback(code, params));
                        else if (attr.js)
                            callback(attr.js, params);

                    }

                    function _reRender() {
                        _getElement((html) => {
                            $(element.children()[0]).remove();
                            element.append($.parseHTML(html));
                            _getAndRunJs((code, params) => {
                                js = _evaluateJS(code, params, scope.debug || false);
                                _runJs(js, scope.appendScriptToBody || false);
                            });
                        });
                    }
                }
            };
        }

        function _evaluateJS(js, params, debug) {

            params = params ? params : {};

            var iifeRegex = /(\(function)\W*(([a-zA-Z])*(,)(\s|\S))*(([a-zA-Z])*\))/i,
                paramsRegex = /(?!\(\)|([a-zA-Z]))\((([a-zA-Z])*(,)(\s|\S))*(([a-zA-Z])*\))/i,
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

            if (debug === true)
                console.log(js);

            return js;

        }

        function _runJs(js, appendToBody) {
            if (appendToBody === 'true') {
                var script = $("<script type='text/javascript'></script>");
                script.text(js);
                $('body').append(script);
            }
            else {
                eval(js);
            }
        }

        function _isValidURL(url) {

            return /^(f|ht)tps?:\/\//i.test(url);

        }

        function _loadFromSource(url, callback, dataType) {

            $.when($.ajax({
                url: url,
                dataType: dataType
            })).done(callback);


            //.then(, (err) => console.log(err));
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