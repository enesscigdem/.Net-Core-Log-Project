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
    public class AccountController : Controller
    {
        private readonly string connectionString = "server=localhost;uid=sa;password=reallyStrongPwd123;database=LoginProject;TrustServerCertificate=true;";
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            HttpContext httpContext = HttpContext.Request.HttpContext;
            if (ModelState.IsValid)
            {
                if (IsValidUser(model.Password))
                {
                    if (httpContext != null)
                    {
                        httpContext.Session.SetString("Username", model.Username);
                        httpContext.Session.SetString("Name", model.Name);
                        httpContext.Session.SetString("Surname", model.Surname);
                    }

                    // Kullanıcı doğrulandı, loglama işlemi yap
                    LogUser(model.Username, model.Name, model.Surname);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                }
            }

            return View(model);
        }
        private bool IsValidUser(string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Password", password);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void LogUser(string username, string name, string surname)
        {
            HttpContext httpContext = HttpContext.Request.HttpContext;
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            // Set LoginTime to Session
            HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Log (Username, Name, Surname, IpAddress, LoginTime) VALUES (@Username, @Name, @Surname, @IpAddress, @LoginTime)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@IpAddress", ipAddress);
                cmd.Parameters.AddWithValue("@LoginTime", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }
        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                string username = HttpContext.Session.GetString("Username");
                // Kullanıcının çıkış yaptığı zamanı kaydet
                DateTime loginTime = DateTime.Parse(HttpContext.Session.GetString("LoginTime"));
                DateTime logoutTime = DateTime.Now;
                TimeSpan duration = logoutTime - loginTime;
                double durationInMinutes = Convert.ToDouble(duration.TotalSeconds);

                // LoginTime sütununa ne kadar süre kaldığını kaydet
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Log SET Duration = @Duration WHERE Username = @Username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Duration", durationInMinutes);
                    cmd.ExecuteNonQuery();
                }

                // Kullanıcının oturumunu sonlandır ve giriş sayfasına yönlendir
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login", "Account");

        }
    }
}