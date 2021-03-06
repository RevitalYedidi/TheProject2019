﻿using NEW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NEW.Controllers
{
    public class BaseController : Controller
    {
        public string IsApprove
        {
            get
            {
                var isApprove = Session["IsApprove"];
                return isApprove == null ? "" : isApprove.ToString();
            }
            set
            {
                Session["IsApprove"] = value;
            }
        }
        public string LoginSession
        {
            get
            {
                var login = Session["Login"];
                return login == null ? "" : login.ToString();
            }
            set
            {
                Session["Login"] = value;
            }
        }

    }

    public class HomeController : BaseController
    {
        private ApplicationDbContext _context;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Main()
        {
            return View();
        }
        //public void OnClickNotify(string message, string imageGuest)
        //{
        //    Message( message,  imageGuest);
        //}
        //[HttpPost]
        //public PartialViewResult Message(string message, string imageGuest)
        //{
        //    ViewBag.Message = message;
        //    ViewBag.Image = imageGuest;
        //    return PartialView();
        //}

        public ActionResult Message(int msgId)
        {
            if (LoginSession != "")
            {
                IsApprove = "";
                using(ApplicationDbContext context = new ApplicationDbContext())
                {
                    var Message = context.Notifications.FirstOrDefault(m => m.id == msgId);
                    ViewBag.Message = Message.GuestMessage;
                };
                ViewBag.time = DateTime.Now.ToString("HH:mm");
                Session["MsgID"] = msgId;
                
               // ViewBag.Image = imageGuest;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }

        public JsonResult DeclineAnswer()
        {
            IsApprove = "false";

            return Json(IsApprove);
        }

        public JsonResult AcceptAnswer()
        {

            IsApprove = "true";
            return Json(IsApprove);
        }

        public ActionResult Entrance()
        {
            return View();
        }

        public ActionResult GetSessionForLayout()
        {
            return Content(LoginSession);
        }
        public ActionResult GetUserName()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var User = context.Tenants.FirstOrDefault(t => t.UserEmail == LoginSession);
                return Content(User.FirstName.ToString());
            };
        }



        public ActionResult Login()
        {
            using(ApplicationDbContext context = new ApplicationDbContext())
            {
                var Users = context.Tenants.ToList();
                return View();
            }
            
        }
        public JsonResult CheckLogin(string Email, string Password)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var User = context.Tenants.FirstOrDefault(t => t.UserEmail == Email);

                if (User != null)
                {
                    if (Password == User.UserPassword.ToString())
                    {
                        Session["Login"] = User.UserEmail.ToString();
                        return Json("LogIn");
                    }
                    else
                        return Json("Wrong Password");
                }
                else
                {
                    return Json("User Not Found");
                }
            }
        }
        public ActionResult LogOut()
        {
            Session["Login"] = "";
            return Redirect("Login");
        }
        public JsonResult CloseSession()
        {
            Session["Login"] = "";
            return Json("true");
        }
        /// <summary>
        ///יקבל מזהה של בניין וכך יזהה את הקוד ואת כל מה שצריך
        /// </summary>
        /// <returns></returns>
        public ActionResult Admin()
        {
            if (LoginSession != ""  )
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var Tenant = context.Tenants.FirstOrDefault(t => t.UserEmail == LoginSession);
                    ViewBag.IsAdmin = Tenant.Admin; 
                    var building = context.Buildings.FirstOrDefault(b => b.Id == 1);
                    ViewBag.Building = building;
                    return View(building);
                };
            }
            return RedirectToAction("Login", "Home");
        }

        public JsonResult ChangeCode(string CurrentCode, string NewCode, string Id)
        {
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var building = context.Buildings.FirstOrDefault(b => b.Id.ToString() == Id);

                    if (building.CyferCode.ToString() != CurrentCode)
                    {
                        return Json("Wrong Password!");
                    }
                    else
                    {
                        building.CyferCode = NewCode;

                        context.SaveChanges();
                        return Json("The Code Was Succefully Saved! ");
                    }
                };
            }
            catch(Exception e)
            {
                return Json("Something Went Wrong " + e.Message);
            }
        }

        //בעתיד להוסיף": לשלוח לפונקציה גם מספק בניין לדעת איזה קוד של בניין אנחנו בודקים
        //כרגע זה רק על בניין מספר 1
       
        public JsonResult CheckCode(string InsertedCode)
        {
            string code = InsertedCode.Replace(",", "");

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var TheCode = context.Buildings.FirstOrDefault(b => b.Id == 1).CyferCode;

                if (code == TheCode.ToString())
                {
                    return Json("Confirmed");
                }
                else
                {
                    return Json("Not Confirmed");
                }
            };
        }

    }
}