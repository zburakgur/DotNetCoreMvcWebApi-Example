using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DotNetCoreExample.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreExample.Services
{
    [Route("api/LoginController/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        /**
         * Returns:
         *  OK: Logined successfully
         *  INVUSER: User is invalid.
         *  ERR: An unexpected error  occured on server
         **/
        // POST api/<controller>/Login/<userName>/<password>
        [HttpPost("{userName}/{password}")]
        public string Login(string userName, string password)
        {
            string result = "ERR";
            SqlConnection connection = new SqlConnection(ProjectVariables.dbConnectionString);

            try
            {
                Functions.EnsureNotNullCredentials(userName, password);

                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                bool flag = DBFunctions.checkLogin(command, userName, password);
                if (flag)
                {
                    Functions.setSession(HttpContext.Session, "userName", userName);
                    Functions.setSession(HttpContext.Session, "password", password);

                    result = "OK";
                }
                else result = "INVUSER";

                connection.Close();
            }
            catch(Exception ex)
            {
                Functions.LogWebMethodError(this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return result;
        }

        /**
         * Returns:
         *  OK: Signed up successfully
         *  INVUSER: There is already a user same named.
         *  ERR: An unexpected error  occured on server 
         **/
        [HttpPost("{userName}/{password}")]
        public string SignUp(string userName, string password)
        {
            string result = "ERR";
            SqlConnection connection = new SqlConnection(ProjectVariables.dbConnectionString);

            try
            {
                Functions.EnsureNotNullCredentials(userName, password);

                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                bool flag = DBFunctions.isLoginNameUsed(command, userName);
                if (!flag)
                {
                    command.Parameters.Clear();
                    command.CommandText = "INSERT INTO LOGINS VALUES(@LOGINNAME, @PASSWORD) ";
                    command.Parameters.Add("@LOGINNAME", SqlDbType.VarChar, 10);
                    command.Parameters.Add("@PASSWORD", SqlDbType.Char, 5);
                    command.Prepare();
                    command.Parameters["@LOGINNAME"].Value = userName;
                    command.Parameters["@PASSWORD"].Value = password;
                    command.ExecuteNonQuery();

                    Functions.setSession(HttpContext.Session, "userName", userName);
                    Functions.setSession(HttpContext.Session, "password", password);

                    result = "OK";
                }
                else result = "INVUSER";

                connection.Close();
            }
            catch (Exception ex)
            {
                Functions.LogWebMethodError(this.GetType().Name,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }

            return result;
        }
        
        [HttpGet]
        public void LogOut()
        {
            HttpContext.Session.Clear();
        }
    }
}