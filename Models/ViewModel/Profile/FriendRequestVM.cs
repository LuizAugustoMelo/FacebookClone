using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModel.Profile
{
    public class FriendRequestVM
    {
        public int Id { get; set; }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public bool Active { get; set; }

        public FriendRequestVM()
        {
        }

        public FriendRequestVM(FriendsDTO row)
        {
            Id = row.Id;
            User1 = row.User1;
            User2 = row.User2;
            Active = row.Active;
        }
    }
}