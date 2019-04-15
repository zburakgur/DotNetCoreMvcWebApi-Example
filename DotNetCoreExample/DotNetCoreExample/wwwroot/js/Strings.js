var Strings = {
    /* A */
    ACTOR: "Actor",
    APPEXIT: "Exit",
    /* D */
    DIRECTOR: "Director", 
    /* E */
    ERROR_SERVEROP: "Error on operation. Please try again later!",
    ERROR_INVUSERPASS: "Invalid username and password. Please check entered values!",
    ERROR_TRYANOTHERUSR: "Entered user name is already used. You should try another one!",
    /* G */
    GENRE: "Genre",
    /* L */
    LOGIN: "Login",
    /* M */
    MOVIENAME: "Movie Title",
    /* O */
    OKEY: "Okey",
    /* P */
    PASSWORD: "Password",
    PERFORMINGLOGIN: "System login in progress..",
    /* R */
    RELEASEDATE: "Release Date",
    /* S */
    SEARCHCRITERIA: "Search Criteria",
    SINGUP: "Sign Up",
    SUMMARY: "Summary",
    /* U */
    USERNAME: "User Name",
    /* W */
    WARNFLDVALINVALID: "You should supply a valid value for this field!",
    WARNINPUTLENGTHMAXVALUE: "Character length of the input should be maximum %s!",
    WARNINPUTLENGTHVALUE: "Character length of the input should be %s!",
    WARNNORECORDSFOUNDDBCRITERIA: "No records found in the server database based on supplied criteria!",
    WELCOME: "Welcome",

    GetMovieGenre: function (genre) {
        genre = parseInt(genre);
        switch (genre) {
            case 0: return "Unknowen";
            case 1: return "Horror";
            case 2: return "Drama";
            case 3: return "Mystery";
            case 4: return "Thriller";
            case 5: return "Action";
            case 6: return "Adventure";
            case 7: return "Sci-Fi";
            case 8: return "Biography";
            case 9: return "History";
            case 10: return "War"; 
            default: return "Unknowen";
        }
    }
}