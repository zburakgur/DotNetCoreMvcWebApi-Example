using DotNetCoreExample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreExample.Utils
{
    public class DBFunctions
    {
        public static List<MovieClass> getMovieTitles(SqlCommand command, int lastRefreshedId)
        {
            command.Parameters.Clear();
            command.CommandText = "SELECT TOP 50 * FROM MOVIES WHERE ID > @LASTREFRESHEDID ORDER BY ID ASC ";
            command.Parameters.Add("@LASTREFRESHEDID", SqlDbType.Int);
            command.Prepare();
            command.Parameters["@LASTREFRESHEDID"].Value = lastRefreshedId;

            List<MovieClass> list = new List<MovieClass>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                MovieClass movie = new MovieClass();
                movie.i = DBTypesEvaluator.ToInteger(reader["ID"]);
                movie.t = DBTypesEvaluator.ToString(reader["TITLE"]);

                list.Add(movie);
            }

            reader.Close();
            return list;
        }

        public static void updateMovie(SqlCommand command, MovieClass movie)
        {
            command.Parameters.Clear();
            command.CommandText = "UPDATE MOVIES SET TITLE = @TITLE, RELEASEDATE = @RELEASEDATE, " +
                                  "GENRE = @GENRE, SUMMARY = @SUMMARY " +
                                  "WHERE ID = @ID ";
            command.Parameters.Add("@TITLE", SqlDbType.VarChar, 100);
            command.Parameters.Add("@RELEASEDATE", SqlDbType.DateTime);
            command.Parameters.Add("@GENRE", SqlDbType.TinyInt);
            command.Parameters.Add("@SUMMARY", SqlDbType.VarChar, 250);
            command.Parameters.Add("@ID", SqlDbType.Int);
            command.Prepare();
            command.Parameters["@TITLE"].Value = movie.t;
            command.Parameters["@RELEASEDATE"].Value = DBTypesEvaluator.ToDateTime(movie.rd);
            command.Parameters["@GENRE"].Value = movie.g;
            command.Parameters["@SUMMARY"].Value = movie.s;
            command.Parameters["@ID"].Value = movie.i;
            command.ExecuteNonQuery();

            // delete directories
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM MOVIEDIRECTORS WHERE MOVIEID = @MOVIEID ";
            command.Parameters.Add("@MOVIEID", SqlDbType.Int);
            command.Prepare();
            command.Parameters["@MOVIEID"].Value = movie.i;
            command.ExecuteNonQuery();

            // delete actors
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM MOVIEACTORS WHERE MOVIEID = @MOVIEID ";
            command.Parameters.Add("@MOVIEID", SqlDbType.Int);
            command.Prepare();
            command.Parameters["@MOVIEID"].Value = movie.i;
            command.ExecuteNonQuery();

            /* insert directors */
            inserDirectors(command, movie);

            /* insert actors */
            insertActors(command, movie);
        }

        private static void inserDirectors(SqlCommand command, MovieClass movie)
        {
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO MOVIEDIRECTORS VALUES (@MOVIEID, @DIRECTOR) ";
            command.Parameters.Add("@MOVIEID", SqlDbType.Int);
            command.Parameters.Add("@DIRECTOR", SqlDbType.VarChar, 50);
            command.Prepare();

            for (int i = 0; i < movie.md.Count; i++)
            {
                command.Parameters["@MOVIEID"].Value = movie.i;
                command.Parameters["@DIRECTOR"].Value = movie.md[i];
                command.ExecuteNonQuery();
            }
        }

        private static void insertActors(SqlCommand command, MovieClass movie)
        {
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO MOVIEACTORS VALUES (@MOVIEID, @ACTOR) ";
            command.Parameters.Add("@MOVIEID", SqlDbType.Int);
            command.Parameters.Add("@ACTOR", SqlDbType.VarChar, 50);
            command.Prepare();

            for (int i = 0; i < movie.ma.Count; i++)
            {
                command.Parameters["@MOVIEID"].Value = movie.i;
                command.Parameters["@ACTOR"].Value = movie.ma[i];
                command.ExecuteNonQuery();
            }
        }

        public static void insertMovie(SqlCommand command, MovieClass movie)
        {
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO MOVIES (TITLE, RELEASEDATE, GENRE, SUMMARY) " +
                                  "VALUES (@TITLE, @RELEASEDATE, @GENRE, @SUMMARY) " +
                                  "SELECT SCOPE_IDENTITY();";
            command.Parameters.Add("@TITLE", SqlDbType.VarChar, 100);
            command.Parameters.Add("@RELEASEDATE", SqlDbType.DateTime);
            command.Parameters.Add("@GENRE", SqlDbType.TinyInt);
            command.Parameters.Add("@SUMMARY", SqlDbType.VarChar, 250);
            command.Prepare();
            command.Parameters["@TITLE"].Value = movie.t;
            command.Parameters["@RELEASEDATE"].Value = DBTypesEvaluator.ToDateTime(movie.rd);
            command.Parameters["@GENRE"].Value = movie.g;
            command.Parameters["@SUMMARY"].Value = movie.s;

            movie.i = DBTypesEvaluator.ToInteger(command.ExecuteScalar());
            if(movie.i != 0)
            {
                /* insert directors */
                inserDirectors(command, movie);

                /* insert actors */
                insertActors(command, movie);
            }
        }

        public static MovieClass getMovie(SqlCommand command, string title)
        {
            MovieClass result = null;

            command.Parameters.Clear();
            command.CommandText = "SELECT TOP 1 * FROM MOVIES WHERE TITLE LIKE @TITLE ";
            command.Parameters.Add("@TITLE", SqlDbType.VarChar, 100);
            command.Prepare();
            command.Parameters["@TITLE"].Value = "%" + title + "%";

            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                result = new MovieClass();
                result.i = DBTypesEvaluator.ToInteger(reader["ID"]);
                result.t = DBTypesEvaluator.ToString(reader["TITLE"]);
                result.rd = DBTypesEvaluator.ToFullTimeStr(reader["RELEASEDATE"]);
                result.g = DBTypesEvaluator.ToInteger(reader["GENRE"]);
                result.s = DBTypesEvaluator.ToString(reader["SUMMARY"]);
            }
            reader.Close();

            if(result != null)
            {
                /* read movie director names */
                command.Parameters.Clear();
                command.CommandText = "SELECT * FROM MOVIEDIRECTORS WHERE MOVIEID = @MOVIEID ";
                command.Parameters.Add("@MOVIEID", SqlDbType.Int);
                command.Prepare();
                command.Parameters["@MOVIEID"].Value = result.i;

                List<string> directorList = new List<string>();
                reader = command.ExecuteReader();
                while (reader.Read())
                    directorList.Add(DBTypesEvaluator.ToString(reader["DIRECTOR"]));

                reader.Close();
                result.md = directorList;

                /* read movie actor names */
                command.Parameters.Clear();
                command.CommandText = "SELECT * FROM MOVIEACTORS WHERE MOVIEID = @MOVIEID ";
                command.Parameters.Add("@MOVIEID", SqlDbType.Int);
                command.Prepare();
                command.Parameters["@MOVIEID"].Value = result.i;

                List<string> actorList = new List<string>();
                reader = command.ExecuteReader();
                while (reader.Read())
                    actorList.Add(DBTypesEvaluator.ToString(reader["ACTOR"]));

                reader.Close();
                result.ma = actorList;
            }

            return result;
        }

        public static bool checkLogin(SqlCommand command, string userName, string password)
        {
            bool result = false;
            
            command.Parameters.Clear();
            command.CommandText = "SELECT * FROM LOGINS WHERE LOGINNAME = @LOGINNAME AND PASSWORD = @PASSWORD ";
            command.Parameters.Add("@LOGINNAME", SqlDbType.VarChar, 10);
            command.Parameters.Add("@PASSWORD", SqlDbType.Char, 5);
            command.Prepare();
            command.Parameters["@LOGINNAME"].Value = userName;
            command.Parameters["@PASSWORD"].Value = password;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                result = true;

            reader.Close();
            return result;
        }

        public static bool isLoginNameUsed(SqlCommand command, string userName)
        {
            bool result = false;

            command.Parameters.Clear();
            command.CommandText = "SELECT * FROM LOGINS WHERE LOGINNAME = @LOGINNAME ";
            command.Parameters.Add("@LOGINNAME", SqlDbType.VarChar, 10);
            command.Prepare();
            command.Parameters["@LOGINNAME"].Value = userName;

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                result = true;

            reader.Close();
            return result;
        }

        public static void updateLastRefreshedId(SqlCommand command, int lastRefreshedId)
        {
            // delete old one
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM SYSTEMPARAMS WHERE PARAMNAME = 'LASTREFRESHEDMOVIEID' ";
            command.Prepare();
            command.ExecuteNonQuery();

            // insert new one
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO SYSTEMPARAMS (PARAMNAME, PARAMTYPE, PARAMINTEGER) " +
                                  "VALUES ('LASTREFRESHEDMOVIEID', 1, @PARAMINTEGER) ";
            command.Parameters.Add("@PARAMINTEGER", SqlDbType.Int);
            command.Prepare();
            command.Parameters["@PARAMINTEGER"].Value = lastRefreshedId;
            command.ExecuteNonQuery();
        }

        public static int getLastRefreshedId(SqlCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "SELECT * FROM SYSTEMPARAMS WHERE PARAMNAME = 'LASTREFRESHEDMOVIEID' ";
            command.Prepare();

            int lastRefreshedId = 0;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                lastRefreshedId = DBTypesEvaluator.ToInteger(reader["PARAMINTEGER"]);
            }
            reader.Close();

            return lastRefreshedId;
        }
    }

    public class DBTypesEvaluator
    {
        public static string ToString(object value)
        {
            return value == DBNull.Value ? string.Empty :
                value.ToString();
        }

        public static int ToInteger(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        public static DateTime ToDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return DateTime.MinValue;
            else return Convert.ToDateTime(value);
        }

        public static string ToFullTimeStr(object value)
        {
            if (value == DBNull.Value) return "";
            return GetTimeFullString(Convert.ToDateTime(value));
        }

        public static string GetTimeFullString(DateTime time)
        {
            string result = "";
            if (time == DateTime.MinValue) return result;
            if (time < new DateTime(1900, 1, 1)) return result;

            result += String.Format("{0:0000}", time.Year); result += "-";
            result += String.Format("{0:00}", time.Month); result += "-";
            result += String.Format("{0:00}", time.Day);
            return result;
        }
    }
}
