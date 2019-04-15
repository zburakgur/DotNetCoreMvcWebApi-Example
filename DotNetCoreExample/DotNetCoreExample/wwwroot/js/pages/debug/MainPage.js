
var movieGrid = {
    view: "datatable",
    id: "gridMovies",
    select: "row",
    editable: false,
    resizeColumn: true,
    resizeRow: false,
    columns: [
        { id: "t", width: 200, header: Strings.MOVIENAME },
        { id: "rd", width: 100, header: Strings.RELEASEDATE },            
        {
            id: "g", width: 100, header: Strings.GENRE,
            template: function (obj) {
                return Strings.GetMovieGenre(obj.g);
            },
        },
        { id: "s", width: 800, header: Strings.SUMMARY }
    ],
    headerRowHeight: 30,
    rowHeight: 25,
    data: null
};

var directorGrid = {
    view: "datatable",
    id: "gridDirectors",
    select: "row",
    editable: false,
    resizeColumn: true,
    resizeRow: false,
    columns: [
        {
            id: "i", width: 200, header: Strings.DIRECTOR,
            template: function (obj) {
                return obj.value;
            }
        }
    ],
    headerRowHeight: 30,
    rowHeight: 25,
    data: null
};

var actorGrid = {
    view: "datatable",
    id: "gridActors",
    select: "row",
    editable: false,
    resizeColumn: true,
    resizeRow: false,
    columns: [
        {
            id: "i", width: 200, header: Strings.ACTOR,
            template: function (obj) {
                return obj.value;
            }
        }
    ],
    headerRowHeight: 30,
    rowHeight: 25,
    data: null
};

var createMainView = function () {
    webix.ui({
        id: "bodyLayout",
        type: "clean",
        margin: 5,
        padding: 5,
        cols: [
            {},
            {
                width: 800,
                rows: [
                    { height: 100 },
                    {
                        view: "toolbar", id: "toolbarMain", css: "toolbarbkgnd", height: 45, padding: 0,
                        elements:
                            [
                                { view: "template", template: Strings.WELCOME + " " + login.userName },
                                {
                                    view: "button", id: "btnExit", type: "image", image: imagesPath + "exit28.png",
                                    width: 40, tooltip: Strings.APPEXIT, click: function () {
                                        logout();
                                    }
                                }
                            ]
                    },
                    {
                        rows: [
                            {
                                view: "search", id: "moviewListfilter", css: "editsearch", height: 30,
                                placeholder: Strings.SEARCHCRITERIA + " ..", keyPressTimeout: 1000,
                            },
                            { height: 7 },
                            movieGrid,
                            {
                                cols: [
                                    directorGrid,
                                    actorGrid
                                ]
                            }
                        ]
                    },
                    { height: 100 },
                ]
            },
            {}
        ]
    });

    $$("moviewListfilter").attachEvent("onKeyPress", function (code, e) {
        if (code == 13) {
            var filterText = $$("moviewListfilter").getValue();
            if (filterText.length < 1) return;
            loadMovieMatches(filterText);
        }
    });
}

var loadMovieMatches = function (filterText) {
    var config = {
        httpMethod: "post",
        returnType: "json"
    }

    var params = {
        userName: login.userName,
        password: login.password,
        title: filterText
    };
    ajaxRequest(config, "/api/MainController/GetMovie", params,
        function (data) {
            if (data == null || data.isErr) {
                showError(Strings.ERROR_SERVEROP);
                return;
            }

            if (data.m == null) {
                showError(Strings.WARNNORECORDSFOUNDDBCRITERIA);
                return;
            }

            $$("gridMovies").clearAll();
            $$("gridMovies").define("data", data.m);
            $$("gridMovies").refresh();

            $$("gridDirectors").clearAll();
            $$("gridDirectors").define("data", data.m.md);
            $$("gridDirectors").refresh();

            $$("gridActors").clearAll();
            $$("gridActors").define("data", data.m.ma);
            $$("gridActors").refresh();
        }
    );
}

var logout = function () {
    var config = {
        httpMethod: "get",
        returnType: "void"
    }
    ajaxRequest(config, "/api/LoginController/LogOut", "",
        function (data) {
            window.setTimeout(function () {
                document.location = "/";
            }, 1000);
        }
    );
}