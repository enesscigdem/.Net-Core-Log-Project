using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sql;
using LoginProject.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace LoginProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connectionString = "server=localhost;uid=sa;password=reallyStrongPwd123;database=LoginProject;TrustServerCertificate=true;";
        public IActionResult Index()
        {
            List<LoginViewModel> userList = new List<LoginViewModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Users";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {


                    LoginViewModel user = new LoginViewModel
                    {
                        Name = reader.GetString(0),
                        Surname = reader.GetString(1),
                        Username = reader.GetString(2),
                        Password = reader.IsDBNull(3) ? null : reader.GetString(3),
                    };
                    userList.Add(user);
                }
            }
            return View(userList);
        }

    }
}