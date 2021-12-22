using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3.Models
{

    [DynamoDBTable("users")]
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }


}
