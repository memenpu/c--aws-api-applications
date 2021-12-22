using Amazon.DynamoDBv2.DataModel;
using Lab3.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3.Models
{
    [DynamoDBTable("movies")]
    public class Movie
    {
        [DynamoDBHashKey]
        [DisplayName("uuid")]
        public string uuid { get; set; }
        [DisplayName("Title")]
        public string title { get; set; }
        [DisplayName("Release Date")]
        public DateTime released_data { get; set; } = new DateTime();
        [DisplayName("File Name")]
        public string s3_filename { get; set; }
        [DynamoDBIgnore]
        public IFormFile FormFile { get; set; }
        [DynamoDBIgnore]
        public User user { get => HomeController.loginUser; }
        [DynamoDBIgnore]
        private List<Comment> _comments;
        public List<Comment> comments {
            get { if (_comments == null)
                    _comments = new();
                return _comments; }
            set { _comments = value; } 
        }
    
// key is user email
// value is user rating
public Dictionary<string, int> ratings { get; set; }
        [DynamoDBIgnore]
        public int UserRating { get {
                if (ratings == null)
                    ratings = new();
                if (!ratings.ContainsKey(user.username)) {
                    ratings[user.username]=10;
                }
                return ratings[user.username]; 
            }
        }
        [DisplayName("Average Rating")]
        [DynamoDBIgnore]
        public double AverageRating { get => ratings?.Values?.Average()??10; }
    }

    public class Comment
    {
        public string uuid { get; set; }
        public string comment { get; set; }
        public string useremail { get; set; }
        public DateTime modified_dealline { get; set; }
        public bool canEdit { get => DateTime.Now.AddDays(-2) <= modified_dealline && useremail==HomeController.loginUser.username; }
    }
    public class Rating
    {
        public string useremail { get; set; }
        public int rating { get; set; }
    }
}
