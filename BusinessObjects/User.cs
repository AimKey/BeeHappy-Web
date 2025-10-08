using BusinessObjects.Base;
using BusinessObjects.NestedObjects;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class User : MongoEntity
    {
        public string Username { get; set; }
        public string NormalizedName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string GoogleId { get; set; }
        public List<string> Roles { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Profile Profile { get; set; }
        public List<Editor> Editors { get; set; }
        public List<ObjectId> Badges { get; set; }
        public List<UserPaint> Paints { get; set; }
        public bool IsPremium { get; set; }
    }
}