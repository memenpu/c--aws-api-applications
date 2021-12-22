using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    [DynamoDBTable("Bookshelf")]
    public class Book
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("name")]
        public string Name { get; set; }
        [DynamoDBProperty("authors")]
        public string Authors { get; set; }
        [DynamoDBProperty("url")]
        public string Url { get; set; }

        public Bookmark initBookmark() {
            return new Bookmark { Book = this, bookmarked_page = 0 };
        }
    }
}
