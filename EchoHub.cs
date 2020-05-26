using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using FacebookClone.Models.Data;
using System.Web.Mvc;
using Microsoft.SqlServer;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message)
        {
            //Clients.All.hello();
            Trace.WriteLine(message);

            // Set Clients
            var clients = Clients.All;

            // Call Js function
            clients.test("This is a test!!!!!");
        }

        public void Notify(string friend)
        {
            //Init Db
            Db db = new Db();

            //Get Friend's Id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(friend)).FirstOrDefault();

            // Get Friend Counts
            var frCount = db.Friends.Count(x => x.User2.Equals(userDTO.Id) && x.Active.Equals(false));

            // Set Clients
            var clients = Clients.Others;

            // Call Js function
            clients.frnotify(friend, frCount);
        }

        public void GetFrcount()
        {
            //Init Db
            Db db = new Db();

            //Get Friend's Id
            UserDTO userDTO = db.Users.Where(x => x.Username.Equals(Context.User.Identity.Name)).FirstOrDefault();

            // Get friend request account
            var friendReqCount = db.Friends.Count(x => x.User2.Equals(userDTO.Id) && x.Active.Equals(false));

            // Set Clients
            var clients = Clients.All;

            // Call Js function
            clients.frcount(Context.User.Identity.Name, friendReqCount);
        }

        public void getFcount(int friendId)
        {
            
        }
    }
}