using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using Google.Cloud.Storage.V1;
using Google.Cloud.Firestore;
using System.IO;
using System.Collections.Generic;


namespace MyPubSubFunction
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
            public async Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
            {
                var blogId = data.Message?.TextData;
                Console.WriteLine($"Blog id received: {blogId}");

                if (string.IsNullOrEmpty(blogId) == false)
                {
                    //we have blogid with data

                    //putting everything inside a pdf.
                    PdfDocument document = new PdfDocument();
                    Console.WriteLine($"PdfDocument created");
                    // Add a new page to the document
                    PdfPage page = document.AddPage();
                    Console.WriteLine($"PdfPage created");
                    //step 2: getting posts for the blog
                    var myPosts = await GetPosts(blogId); //will get a list of posts pertaining to a blog
                    Console.WriteLine($"Posts read: {myPosts.Count}");
                    //step 3: report generation
                    int yPosition = 10;

                    GlobalFontSettings.FontResolver = new FileFontResolver();
                    Console.WriteLine($"FileFontResolver created");

                    XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
                    Console.WriteLine($"Xfont Verdana created");

                    // Get an XGraphics object for drawing
                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        Console.WriteLine($"XGraphics created");
                        foreach (var post in myPosts)
                        {
                            
                            Console.WriteLine($"Post with {post.Name} is being processed");
                            // Draw the text on the page
                            gfx.DrawString(post.Name, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            // Move to the next line (increase Y-coordinate position)
                            yPosition += font.Height;
                            gfx.DrawString(post.Content, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            // Move to the next line (increase Y-coordinate position)
                            yPosition += font.Height;
                            gfx.DrawString("-----------------------------------------------", font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                            // Move to the next line (increase Y-coordinate position)
                            yPosition += (font.Height * 3);

                        }
                        Console.WriteLine($"All posts written in pdf");
                    }
                    //step 4: saving the pdf on the hard drive
                    string filenamePDF = blogId + ".pdf";
                    Console.WriteLine($"About to write pdf into memory...");
                    MemoryStream msTemp= new MemoryStream();
                    document.Save(msTemp);
                    Console.WriteLine($"Pdf was generated in memory...");
                    msTemp.Position = 0;
                    Console.WriteLine($"About to upload on the bucket msd63b2024ra_fg file with filename {filenamePDF}...");
                    await UploadFileAsync(filenamePDF, msTemp);
                    Console.WriteLine($" file with filename {filenamePDF} was uploaded successfully");

            }
        }

        public async Task<Google.Apis.Storage.v1.Data.Object> UploadFileAsync(string filename, MemoryStream ms)
        {
            var storage = StorageClient.Create();
            return await storage.UploadObjectAsync("msd63b2024ra_fg", filename, "application/octet-stream", ms);
        }

        public async Task<List<Post>> GetPosts(string blogId)
        {
            ///blogs/80f73ae0-4006-4c4e-ab94-27993e8f5d4e/Posts/JkTXHqpmRqahUfKIRCQs
            //blogs/<blog-id>/posts

            List<Post> posts = new List<Post>();
            var db = FirestoreDb.Create("msd63b2024");
            Query postQuery = db.Collection($"blogs/{blogId}/posts");
            QuerySnapshot postQuerySnapshot = await postQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in postQuerySnapshot.Documents)
            {
                Post b = documentSnapshot.ConvertTo<Post>();
                b.Id = documentSnapshot.Id;
                b.BlogId_FK = blogId;

                posts.Add(b);
            }

            return posts;
        }


}
}