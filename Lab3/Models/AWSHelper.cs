using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3.Models
{
    interface AWSHelper
    {
        public BasicAWSCredentials GetCredentials(string key, string secret)
        {
            return new BasicAWSCredentials(key, secret);
        }
    }
}
