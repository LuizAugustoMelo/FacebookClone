using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModel.Account;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FacebookClone.Controllers
{
    public class AccountController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            // Confirm user is not logged in
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return Redirect("~/" + userName);
            }

            //return view
            return View();
        }

        // POST: /Account/CreateAccount
        [HttpPost]
        public ActionResult CreateAccount(UserVM model, HttpPostedFileBase file)
        {
            // Init db
            Db db = new Db();

            // Cheack model state
            if (!ModelState.IsValid)
                return View("Index", model);

            // Make sure username is unique
            if (db.Users.Any(x => x.Username.Equals(model.Username)))
            {
                ModelState.AddModelError("", "That Username: " + model.Username + " is taken");
                model.Username = "";
                return View("Index", model);
            }

            // Create UserDTO
            UserDTO userDTO = new UserDTO()
            {
                FirstName = model.Username,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Username = model.Username,
                Password = model.Password
            };

            // Add to DTO
            db.Users.Add(userDTO);

            // Save
            db.SaveChanges();

            // Get Inserted id
            int userId = userDTO.Id;

            // Login User
            FormsAuthentication.SetAuthCookie(model.Username, false);

            // Set Uploads dir
            var uploadsDir = new DirectoryInfo(string.Format("{0}Uploads", Server.MapPath(@"\")));

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg" && ext != "image/gif" && ext != "image/x-png" && ext != "image/png")
                {
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                    return View("Index", model);
                }

                // Set Image name
                string imageName = userId + ".jpg";

                // Set image Path
                var path = string.Format("{0}\\{1}", uploadsDir, imageName);

                // Save Image
                file.SaveAs(path);
            }

            // Redirect
            return Redirect("~/" + model.Username);
        }

        // Get: /{Username}
        [Authorize]
        public ActionResult Username(string username = "")
        {
            //Init Database
            Db db = new Db();

            //Check if user exists
            if(!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            // ViewBag Username
            ViewBag.Username = username;

            return View();
        }

        // Get: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            // Sign out
            FormsAuthentication.SignOut();

            //redirect
            return Redirect("~/");
        }

        public ActionResult LoginPartial()
        {
            return PartialView();
        }

        // POST: /Account/Login
        [HttpPost]
        public string Login(string username, string password)
        {
            //Init a database
            Db db = new Db();

            // check if user exists
            if (db.Users.Any(x => x.Username.Equals(username) && x.Password.Equals(password)))
            {
                // Login
                FormsAuthentication.SetAuthCookie(username, false);
                return "ok";
            }
            else
            {
                return "error";
            }
        }
    }
}