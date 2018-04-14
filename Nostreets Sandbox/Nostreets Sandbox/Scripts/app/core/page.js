var page = {
    APPNAME: "sandbox",
    isLoggedIn: false,
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

        length: (obj) => {
            if (Array.isArray(obj))
                return obj.length;
            else if (typeof (obj) === 'object') {
                var length = 0;
                for (var prop in obj)
                    length++;
                return length;
            }
            else
                return null;
        },

        in: (obj, values) => {
            for (var i = 0; i < values.length; i++)
                if (values[i] == obj) return true;
            return false;
        },

        equals: (obj1, obj2) => JSON.stringify(obj1) === JSON.stringify(obj2),

        clone: (obj) => JSON.parse(JSON.stringify(obj)),

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

(() => angular.module(page.APPNAME, page.ngModules))();

