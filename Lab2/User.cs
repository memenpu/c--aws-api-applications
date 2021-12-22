using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{

    [DynamoDBTable("users")]
    public class User
    {
        [DynamoDBHashKey]
        public string username { get; set; }
        public List<Bookmark> bookmarks { get; set; } = new();
        public string password { get; set; }
    }

    public class Bookmark
    {
        public string name { get; set; }
        public string authors { get; set; }
        public int bookmarked_page { get; set; }
        [DynamoDBIgnore]
        private Book book;
        [DynamoDBIgnore]
        public Book Book
        {
            get { return book; }
            set { book = value; name = book.Name; authors = book.Authors; }
        }

    }



}
