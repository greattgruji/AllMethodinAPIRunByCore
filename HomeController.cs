
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

using System.Text;
//using CoreApplicationTest.DataConnection;
using CoreApplicationTest.Models;
using System.Security.Claims;

namespace CoreApplicationTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly ApplicationDbContext _db;

        Uri apiUrl = new Uri("http://localhost:26939/api/Emp/");
        HttpClient Client;

        public StringContent StringContent { get; private set; }

        public HomeController()
        {
           
            Client = new HttpClient();
            Client.BaseAddress = apiUrl;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> mobj = new List<Employee>();
            HttpResponseMessage listmobj = Client.GetAsync(Client.BaseAddress + "Get/Emp").Result;
            if (listmobj.IsSuccessStatusCode)
            {


                string data = listmobj.Content.ReadAsStringAsync().Result;

                var res = JsonConvert.DeserializeObject<List<Employee>>(data);

                foreach (var item in res)
                {
                    mobj.Add(new Employee
                    {
                        id = item.id,
                        name = item.name,
                        sal = item.sal,
                        gmail = item.gmail,
                        password = item.password
                    });
                }

            }
            return View(mobj);
           
        }
        [HttpGet]
        public IActionResult Addemp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Addemp(Employee obj)
        {
            string data = JsonConvert.SerializeObject(obj);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage res = Client.PostAsync(Client.BaseAddress + "Post/Emp/add", content).Result;
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
           

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var res = Client.DeleteAsync(Client.BaseAddress + "Post/Emp/delete" + '?' + "id" + "=" + id.ToString()).Result;
           

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var res = Client.GetAsync(Client.BaseAddress + "Post/Emp/Edit" + '?' + "id" + "=" + id.ToString()).Result;
            string data = res.Content.ReadAsStringAsync().Result;
            var emp = JsonConvert.DeserializeObject<Employee>(data);
            return View("Addemp", emp);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(Login uobj)
        {
            string data = JsonConvert.SerializeObject(uobj);
            StringContent = new StringContent(data,Encoding.UTF8,"application/Json");
            HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            if(res.IsSuccessStatusCode)
            {
                string data1 = res.Content.ReadAsStringAsync().Result;
                var d = JsonConvert.DeserializeObject<Login>(data1);
            }
            if(uobj.gmail == null)
            {
                TempData["gmail"] = "Gmail not found";
            }
            else
            {
                if (uobj.gmail == uobj.gmail && uobj.name == uobj.name)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, uobj.name), new Claim(ClaimTypes.Email, uobj.gmail) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["name"] = "name not found";

                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Sign_Up()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Sign_Up(User_Info obj3)
        //{

        //    _db.User_Infos.Add(obj3);
        //    _db.SaveChanges();
        //    return RedirectToAction("Login");

        //}
        //public IActionResult Logout()
        //{
        //    HttpContext.SignOutAsync(CookieAuthenticationDefaults.LoginPath);

        //    return RedirectToAction("Login");
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
