
var createLoginView = function () {
    webix.ui({
        id: "bodyLayout",
        type: "clean",
        margin: 5,
        padding: 5,
        rows: [
            {},
            {
                cols: [
                    {},
                    {
                        view: "form", width: 300, padding: 0, margin: 0,
                        borderless: true,
                        elementsConfig: {
                            labelPosition: 'top'
                        },
                        rows: [
                            { view: "text", id: "prmUserName", label: Strings.USERNAME, value: "" },
                            { view: "text", type: "password", id: "prmPassword", label: Strings.PASSWORD, value: "" },
                            {
                                cols: [
                                    { view: "button", id: "btnSignUp", width: 100, value: Strings.SINGUP, click: function () { signUp(); } },
                                    {},
                                    { view: "button", id: "btnLogin", width: 100, value: Strings.LOGIN, click: function () { login(); }  }
                                ]
                            }
                        ]
                    
                    },
                    {}
                ]
            },
            {}
        ]
    });
}

var checkEmptyness = function (userName, password) {
    if (userName == undefined || userName == "") {
        activateFieldWarning("prmUserName", Strings.WARNFLDVALINVALID);
        return 0;
    }
    
    if (password == undefined || password == "") {
        activateFieldWarning("prmPassword", Strings.WARNFLDVALINVALID);
        return 1;
    }

    return -1;
}

var checkInputValidation = function (userName, password) {
    if (userName.length > 10) {
        activateFieldWarning("prmUserName", Strings.WARNINPUTLENGTHMAXVALUE.replace("%s", "10"));
        return 0;
    }

    if (password.length != 5) {
        activateFieldWarning("prmPassword", Strings.WARNINPUTLENGTHVALUE.replace("%s", "5"));
        return 1;
    }

    return -1;
}

var getInputs = function () {
    var userName = $$("prmUserName").getValue();
    var password = $$("prmPassword").getValue();

    var result = checkEmptyness(userName, password);
    if (result == 0 || result == 1) return null;

    result = checkInputValidation(userName, password);
    if (result == 0 || result == 1) return null;

    var obj = { userName: userName, password: password };
    return obj;
}

var login = function(){
    var inputs = getInputs();
    if (inputs == null)
        return;

    var config = {
        httpMethod: "post",
        returnType: "text"
    }

    var params = {
        userName: inputs.userName,
        password: inputs.password
    };
    ajaxRequest(config,"/api/LoginController/Login", params,
        function (data) {
            if (data == "ERR") {
                showError(Strings.ERROR_SERVEROP);
                return;
            }
            else if (data == "INVUSER"){
                showError(Strings.ERROR_INVUSERPASS);
                return;
            }

            setCookie("lastUser", params.userName);
            showInfo(Strings.PERFORMINGLOGIN);
            
            goToTheAppPage();
        }
    );
}

var signUp = function () {
    var inputs = getInputs();
    if (inputs == null)
        return;

    var config = {
        httpMethod: "post",
        returnType: "text"
    }
    var params = {
        userName: inputs.userName,
        password: inputs.password
    };
    ajaxRequest(config, "/api/LoginController/SignUp", params,
        function (data) {
            if (data == "ERR") {
                showError(Strings.ERROR_SERVEROP);
                return;
            }
            else if (data == "INVUSER") {
                showError(Strings.ERROR_TRYANOTHERUSR);
                return;
            }

            setCookie("lastUser", params.userName);
            showInfo(Strings.PERFORMINGLOGIN);

            goToTheAppPage();   
        }
    );
}

var goToTheAppPage = function () {
    window.setTimeout(function () {
        document.location = "Home/MainPage";
    }, 1000);
}