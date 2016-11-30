(function () {
    var DUBUG_MODE = false;

    console.log = function (message) {
        return window.external.notify("console.log: " + message);
    };

    window.alert = function (message) {
        window.external.notify("window.alert: " + message);
    }

    // common functions
    function forEach(list, fn) {
        for (var i = 0; i < list.length; i++) {
            fn(list[i]);
        }
    }

    function getElement(selector) {
        return document.querySelector(selector);
    }

    function getElements(selector) {
        return document.querySelectorAll(selector);
    }

    function waitFor(selector, fn) {
        var element = getElement(selector);
        if (element) {
            fn(element);
            return;
        }

        // set up the mutation observer
        var observer = new MutationObserver(function (mutations, me) {
            // `mutations` is an array of mutations that occurred
            // `me` is the MutationObserver instance
            var element = getElement(selector);

            if (element) {
                fn(element);
                me.disconnect(); // stop observing
                return;
            }
        });

        // start observing
        observer.observe(document, {
            childList: true,
            subtree: true
        });
    }

    function waitForAll(selector, fn) {
        var elements = getElements(selector);
        if (elements.length) {
            forEach(elements, fn);
            return;
        }

        // set up the mutation observer
        var observer = new MutationObserver(function (mutations, me) {
            // `mutations` is an array of mutations that occurred
            // `me` is the MutationObserver instance
            var elements = getElements(selector);

            if (elements.length) {
                forEach(elements, fn);
                me.disconnect(); // stop observing
                return;
            }
        });

        // start observing
        observer.observe(document, {
            childList: true,
            subtree: true
        });
    }

    // DOM manipulations
    function removeElement(selector) {
        waitFor(selector, (elem) => {
            elem.parentNode.removeChild(elem);
        });       
    }

    function removeElements(selector) {
        waitForAll(selector, (elem) => {
            elem.parentNode.removeChild(elem);
        });
    }

    function hideElement(selector) {
        waitFor(selector, (elem) => {
            elem.style.visibility = 'hidden';
        });
    }

    function changeLinkUrl(selector, newUrl) {
        document.querySelector(selector).href = newUrl;
    }

    function setUserAgent(window, userAgent) {
        if (window.navigator.userAgent != userAgent) {
            var userAgentProp = { get: function () { return userAgent; } };
            try {
                Object.defineProperty(window.navigator, 'userAgent', userAgentProp);
            } catch (e) {
                window.navigator = Object.create(navigator, {
                    userAgent: userAgentProp
                });
            }
        }
    }

    //#body
})();