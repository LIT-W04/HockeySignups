using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HockeySignups.Data;
using HockeySignups.Web.Models;

namespace HockeySignups.Web.Controllers
{
    public class HockeyController : Controller
    {
        private string _connectionString =
          @"Data Source=.\sqlexpress;Initial Catalog=HockeySignup;Integrated Security=True";

        public ActionResult Index()
        {
            HomePageViewModel vm = new HomePageViewModel();
            #region TempData
            if (TempData["Message"] != null)
            {
                vm.Message = (string)TempData["Message"];
            }
            else if (TempData["Error"] != null)
            {
                vm.ErrorMesage = (string)TempData["Error"];
            }
            #endregion
            return View(vm);
        }

        public ActionResult LatestEvent()
        {
            var db = new HockeySignupsDb(_connectionString);
            Event latestEvent = db.GetLatestEvent();
            EventSignupViewModel vm = new EventSignupViewModel();
            vm.Event = latestEvent;
            vm.EventStatus = db.GetEventStatus(latestEvent);
            vm.Signup = new EventSignup();
            if (Request.Cookies["firstName"] != null)
            {
                vm.Signup.FirstName = Request.Cookies["firstName"].Value;
                vm.Signup.LastName = Request.Cookies["lastName"].Value;
                vm.Signup.Email = Request.Cookies["email"].Value;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EventSignup(string firstName, string lastName, string email, int eventId)
        {
            HockeySignupsDb db = new HockeySignupsDb(_connectionString);
            Event e = db.GetEventById(eventId);
            EventStatus status = db.GetEventStatus(e);
            if (status == EventStatus.InThePast)
            {
                #region TempData
                TempData["Error"] = "You cannot sign up to a game in the past. Jerk.";
                #endregion
                return RedirectToAction("Index");
            }
            if (status == EventStatus.Full)
            {
                #region TempData
                TempData["Error"] = "Nice try sucker....";
                #endregion
                return RedirectToAction("Index");
            }
            EventSignup s = new EventSignup
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EventId = eventId
            };

            HttpCookie firstNameCookie = new HttpCookie("firstName", s.FirstName);
            HttpCookie lastNameCookie = new HttpCookie("lastName", s.LastName);
            HttpCookie emailCookie = new HttpCookie("email", s.Email);

            Response.Cookies.Add(firstNameCookie);
            Response.Cookies.Add(lastNameCookie);
            Response.Cookies.Add(emailCookie);

            db.AddEventSignup(s);
            #region TempData
            TempData["Message"] =
                "You have succesfully signed up for this weeks game, looking forward to checking you into the boards!";
            #endregion

            return RedirectToAction("Index");
        }

        public ActionResult NotificationSignup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NotificationSignup(string firstName, string lastName, string email)
        {
            var db = new HockeySignupsDb(_connectionString);
            var ns = new NotificationSignup
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            db.AddNotificationSignup(ns);
            return View("NotificationSignupConfirmation");
        }
    }
}
