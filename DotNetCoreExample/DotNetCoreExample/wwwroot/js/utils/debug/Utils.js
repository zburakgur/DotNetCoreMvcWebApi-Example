var prevErrText = "";
var prevErrTime = (new Date()).getTime();

var showError = function (errorText, timeout) {
    var currTime = (new Date()).getTime();
    if (prevErrText == errorText) {
        if (Math.abs(currTime - prevErrTime) < 2000) return;
    }

    prevErrText = errorText;
    prevErrTime = currTime;
    webix.message({ type: "error", text: errorText, expire: timeout });
}

var showInfo = function (message, timeout) {
    webix.message({ type: "info", text: message, expire: timeout });
} 

var activateFieldWarning = function (controlId, textVal, disableIcon) {
    if ($$("fieldValidatorWarning") == null) {
        webix.ui({
            view: "submenu",
            id: "fieldValidatorWarning",
            autowidth: true,
            autoheight: true,
            padding: 0,
            data: null,
            type: {
                height: 40,
                template: function (e) {
                    var s = "";
                    if (e.icon) s += "<img class='warningIcon' src='" + imagesPath + e.icon + ".png' />";
                    s += "<span class='text'>" + e.text + "</span>";
                    return s;
                }
            },
            on: {
                onMenuItemClick: function (id) {
                    $$("fieldValidatorWarning").hide();
                }
            }
        });
    }

    $$("fieldValidatorWarning").clearAll();

    if (disableIcon) {
        $$("fieldValidatorWarning").add({
            id: 1,
            text: textVal
        });
    }
    else {
        $$("fieldValidatorWarning").add({
            id: 1,
            icon: imagesPath + "warning18x18.png",
            text: textVal
        });
    }

    $$("fieldValidatorWarning").show($$(controlId).getNode());
}

/**
 * Converts object to parameterized format
 * /attr1/attr2 ...
 * @param {any} obj
 */
var objectToQueryString = function (obj) {
    var result = "";

    if (obj instanceof Object) {
        for (var property in obj) {
            result += "/" + obj[property];
        }
    }

    return result;
}

/**
 * This fucntion implements ajax requests.
 * 
 * @param {any} config  :It defines a configuration object for request
 *                      { httpMethod : "post", returnType : "json" }
 * @param {any} api     :It defines the api will be called.
 * @param {any} params  :Api's param.
 * @param {any} successcallback
 * @param {any} finishcallback
 */
var ajaxRequest = function (config, api, params,
    successcallback, finishcallback) {

    var promise = eval("webix.ajax().timeout(30000)." + config.httpMethod + "('"+api + objectToQueryString(params) + "', '');");
    promise.then(
        function success(data) {
            if (typeof successcallback == "function") {
                try {
                    var result = undefined;
                    if (config.returnType == "json")
                        result = data.json();

                    else if (config.returnType == "text")
                        result = data.text();

                    successcallback(result);
                }
                catch (err) {
                    webix.message({
                        type: 'error',
                        text: Strings.ERROR_SERVEROP + "<br><br>Web service call:<br>" + api
                    });

                    if (appDebugVersion == true) {
                        if (typeof err == "string") alert(err);
                        if (typeof err.message == "string") alert(err.message);
                    }
                }
            }

            if (typeof finishcallback == "function") {
                finishcallback();
            }
        },
        function error(err) {
            webix.message({
                type: 'error',
                text: Strings.ERROR_SERVEROP + "<br><br>Web service call:<br>" + api
            });

            promise.errorInvoked = true;

            if (typeof finishcallback == "function") {
                finishcallback();
            }
            if (appDebugVersion == true && typeof err.response == "string") alert(err.responseText);
        }
    );

    promise.fail(function (err) {
        if (promise.errorInvoked != true) {
            webix.message({
                type: 'error',
                text: Strings.ERROR_SERVEROP + "<br><br>Web service call:<br>" + api
            });
            if (typeof finishcallback == "function") {
                finishcallback();
            }
            if (appDebugVersion == true && typeof err.response == "string") alert(err.responseText);
        }
    });

}