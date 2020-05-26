using FacebookClone.Models.Data;
using FacebookClone.Models.ViewModel.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FacebookClone.Controllers
{
    public class ProfileController : Controller
    {
        // GET: /
        public ActionResult Index()
        {
            return View();
        }

        // POST: Profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            // Init Db
            Db db = new Db();

            //Create a list
            List<LiveSearchUserVM> usernames = db.Users.Where(x => x.Username.Contains(searchVal) && x.Username != User.Identity.Name)
                                                        .ToArray().Select(x => new LiveSearchUserVM(x)).ToList();

            //return a JSON
            return Json(usernames);
        }

        // POST: Profile/AddFriend
        [HttpPost]
        public void AddFriend(string friend)
        {
            // Init Db
            Db db = new Db();

            // Get User's Id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();
            int userId = userDTO.Id;

            // Get Friend to be Id
            UserDTO userDTO2 = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();
            int friendId = userDTO2.Id;

            //Add DTO
            FriendsDTO friendsDTO = new FriendsDTO
            {
                User1 = userId,
                User2 = friendId, 
                Active = false
            };

            db.Friends.Add(friendsDTO);

            db.SaveChanges();
        }

        // POST: Profile/DisplayFriendRequests
        [HttpPost]
        public JsonResult DisplayFriendRequests()
        {
            // Init DB
            Db db = new Db();

            // Get User ID
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();

            // Create a List of friends
            List<FriendRequestVM> friendRequestVMs = db.Friends.Where(x => x.User2.Equals(userDTO.Id) && x.Active.Equals(false)).ToArray()
                                                                .Select(x => new FriendRequestVM(x)).ToList();

            // Init list of users
            List<UserDTO> users = new List<UserDTO>();

            foreach( var item in friendRequestVMs)
            {
                var user = db.Users.Where(x => x.Id.Equals(item.User1)).FirstOrDefault();
                users.Add(user);
            }

            // Return JSON
            return Json(users);
        }

        // POST: Profile/AcceptFriendRequests
        [HttpPost]
        public void AcceptFriendRequests(int friendId)
        {
            // Init DB
            Db db = new Db();

            // Get User ID
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(User.Identity.Name)).FirstOrDefault();

            // Make Friends
            FriendsDTO friendsDTO = db.Friends.Where(x => x.User1.Equals(friendId) && x.User2.Equals(userDTO.Id)).FirstOrDefault();
            if (friendsDTO != null)
                friendsDTO.Active = true;

            // Save Db
            db.SaveChanges();
        }
    }
}