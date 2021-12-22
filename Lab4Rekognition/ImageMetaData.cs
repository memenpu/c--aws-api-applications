using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4Rekognition
{
    [DynamoDBTable("Lab4-Rekognition")]
    public class ImageMetaData
    {
        [DynamoDBHashKey("image_url")]
        public string ImageUrl { get; set; }
        [DynamoDBProperty("file_name")]
        public string FileName { get; set; }
        [DynamoDBProperty("labels")]
        public Dictionary<string, double> Labels { get; set; } = new Dictionary<string, double>();
    }
}
