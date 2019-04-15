var appDebugVersion = true;
var imagesPath = undefined;

adjustFile = function (file, checkFunc) {
    var appFile = file;
    var fullPath = undefined;

    if (appFile.indexOf("webix.") > -1) {
        var idxSlash = appFile.lastIndexOf("/");
        var idxDot = appFile.lastIndexOf(".");
        var fileName = appFile.substring(idxSlash + 1, idxDot);
        var fileExt = appFile.substring(idxDot + 1, appFile.length);

        if (appDebugVersion) {
            if (fileExt != "css")
                appFile = appFile.replace(fileName + "." + fileExt, fileName + "_debug." + fileExt);
        }

        fullPath = appFile.replace("webix/codebase/", "/webix/v4.0.16/codebase/");
    }
    else {
        var segments = appFile.split("/");
        var folderIncluded = false;
        var folderInclude = appDebugVersion == true ? "debug" : "release";

        if (appFile.indexOf(".js") > -1) {
            fullPath = "/js/";
            
            if (segments.length == 1)
                folderIncluded = true;

            for (var i = 0; i < segments.length; i++) {
                var folderName = checkFunc(segments[i]);
                if (i > 0) fullPath += "/";
                fullPath += folderName;

                if (folderIncluded == false && folderName != "." && folderName != "..") {
                    fullPath += "/" + folderInclude;
                    folderIncluded = true;
                }
            }
        }
        else if (appFile.indexOf(".css") > -1) {
            fullPath = "/css/";

            for (var i = 0; i < segments.length; i++) {
                var folderName = checkFunc(segments[i]);
                if (i > 0) fullPath += "/";
                fullPath += folderName;
            }
        }
    }
    
    if (fullPath == undefined)  return;
    
    appFile = fullPath;
    return appFile;
}

EmbedApplicationScript = function (file, params) {
    var scriptFile = adjustFile(file, function (folder) {
        return folder;
    });

    writeScriptFileToDocument(scriptFile, params);
}

EmbedApplicationStyle = function (file, params) {
    var cssFile = adjustFile(file, function (folder) {
        return folder;
    });

    writeCssFileToDocument(cssFile);
}

writeScriptFileToDocument = function (scriptFile, params) {
    var scriptStr = '<scri' + 'pt src="' + scriptFile;
    /*if (appFile.indexOf("webix") == -1) scriptStr += ("?" + sessionRandValue);*/
    scriptStr += '" type="text/javascript"';

    if (typeof params == "string") scriptStr += ' ' + params;
    scriptStr += '></sc' + 'ript>';

    document.writeln(scriptStr);
}

writeCssFileToDocument = function (cssFile, params) {
    var cssStr = '<li' + 'nk href="' + cssFile;
    /*if (cssFile.indexOf("webix") == -1) cssStr += ("?" + sessionRandValue);*/
    cssStr += '" rel="stylesheet" type="text/css"';
    if (typeof params == "string") cssStr += ' ' + params;
    cssStr += ' />';

    document.writeln(cssStr);
}

getCookie = function (name) {
    var namext = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(namext) == 0) return c.substring(namext.length, c.length);
    }
    return null;
}

setCookie = function (name, value, expires, path, domain, secure) {
    if (expires == null || expires == undefined) {
        var today = new Date();
        expires = new Date(today.getTime() + 365 * 24 * 60 * 60 * 1000);
    }
    document.cookie = name + "=" + escape(value) +
        ((expires) ? "; expires=" + expires.toGMTString() : "") +
        ((path) ? "; path=" + path : "") +
        ((domain) ? "; domain=" + domain : "") +
        ((secure) ? "; secure" : "");
}

systemInit = function () {
    imagesPath = "/images/";

    if (!webix.env.touch && webix.ui.scrollSize)
        webix.CustomScroll.init();
}