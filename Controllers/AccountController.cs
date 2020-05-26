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
                FirstName = model.FirstName,
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

            #region Upload Image
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
            #endregion

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
            if (!db.Users.Any(x => x.Username.Equals(username)))
            {
                return Redirect("~/");
            }

            // ViewBag Username
            ViewBag.Username = username;

            // Get logged in users username
            string user = User.Identity.Name;

            // ViewBag user´s full name
            UserDTO userDTO1 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
            ViewBag.FullName = userDTO1.FirstName + " " + userDTO1.LastName;

            // Get viewing full name
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();
            ViewBag.ViewingFullName = userDTO2.FirstName + " " + userDTO2.LastName;

            //Get User and Friend ID
            int userId = userDTO1.Id;
            int friendId = userDTO2.Id;

            // Get username's image
            ViewBag.UsernameImage = userDTO2.Id + ".jpg";

            // ViewBag user type
            string userType = "guest";

            if (username.Equals(user))
                userType = "owner";

            ViewBag.UserType = userType;

            //Check if they are friends
            if (userType == "guest")
            {
                //UserDTO userDTO3 = db.Users.Where(x => x.Username.Equals(user)).FirstOrDefault();
                //UserDTO userDTO4 = db.Users.Where(x => x.Username.Equals(username)).FirstOrDefault();


                FriendsDTO friendsDTO1 = db.Friends.Where(x => x.User1.Equals(userId) && x.User2.Equals(friendId)).FirstOrDefault();
                FriendsDTO friendsDTO2 = db.Friends.Where(x => x.User1.Equals(friendId) && x.User2.Equals(userId)).FirstOrDefault();

                if (friendsDTO1 == null && friendsDTO2 == null)
                {
                    ViewBag.NotFriends = "True";
                }
                else if (friendsDTO1 != null)
                {
                    if (!friendsDTO1.Active)
                        ViewBag.NotFriends = "Pending";
                }
                else if (friendsDTO2 != null)
                {
                    if (!friendsDTO2.Active)
                        ViewBag.NotFriends = "Pending";
                }
            }

            // Get friend request count
            ViewBag.FRCount = db.Friends.Count(x => x.User2.Equals(userId) && x.Active.Equals(false));


            //Return
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