using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lab4Thumbnail
{
    public class Function
    {
        readonly string SourceFolder = "source";
        readonly string ThumbnailFolder = "thumbnail";
        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="S3event"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event S3event, ILambdaContext context)
        {
            var s3 = S3event.Records?[0].S3;
            if (s3 == null)
            {
                return null;
            }
            try
            {
                Console.WriteLine(s3.Object.Key);
                var rs = await this.S3Client.GetObjectMetadataAsync(
                    s3.Bucket.Name,
                    s3.Object.Key);
                if (rs.Headers.ContentType.StartsWith("image/"))
                {
                    using GetObjectResponse response = await S3Client.GetObjectAsync(
                        s3.Bucket.Name,
                        s3.Object.Key);
                    using Stream responseStream = response.ResponseStream;
                    using var memstream = new MemoryStream();
                    responseStream.CopyTo(memstream);
                    int thumbWidth = 400;
                    int thumbHeight = 200;
                    using GcBitmap origBmp = new GcBitmap();
                    origBmp.Load(memstream.ToArray());
                    Font font;
                    if (FontCollection.SystemFonts.Count == 0)
                    {
                        font = Font.FromFile("Minecraft.ttf");
                    }
                    else
                        font = FontCollection.SystemFonts.DefaultFont;
                    var tf = new TextFormat()
                    {
                        Font = font,
                        FontSize = 15,
                        BackColor = Color.White,
                    };
                    using var newBitmap = new GcBitmap(thumbWidth, thumbHeight, true);
                    using var graphics = newBitmap.CreateGraphics(Color.White);
                    graphics.DrawImage(origBmp, new RectangleF(0, 0, thumbWidth, thumbHeight), null, ImageAlign.ScaleImage);
                    string title = s3.Object.Key.Replace(SourceFolder + "/", "").Split(".")[0];
                    graphics.DrawString(title, tf, new RectangleF(0, 0, thumbWidth, thumbHeight), TextAlignment.Center);
                    using MemoryStream m = new MemoryStream();
                    newBitmap.SaveAsJpeg(m);
                    PutObjectRequest putRequest = new PutObjectRequest()
                    {
                        BucketName = s3.Bucket.Name,
                        Key = s3.Object.Key.Replace(SourceFolder, ThumbnailFolder),
                        ContentType = rs.Headers.ContentType,
                        InputStream = m
                    };
                    await S3Client.PutObjectAsync(putRequest);
                }
                return rs.Headers.ContentType;
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }
    }
}
