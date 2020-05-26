using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.Data
{
    [Table("Friends")]
    public class FriendsDTO
    {
        [Key]
        public int Id { get; set; }
        public int User1 { get; set; }
        public int User2 { get; set; }
        public bool Active { get; set; }

        [ForeignKey("User1")]
        public UserDTO Users1 { get; set; }

        [ForeignKey("User1")]
        public UserDTO Users2 { get; set; }
    }
}