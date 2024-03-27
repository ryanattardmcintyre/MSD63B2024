using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class ReportsController : Controller
    {
        private PubSubRepository _pubSubRepository;
        private PostsRepository _postsRepository;
        private BucketsRepository _bucketsRepository;
        public ReportsController(PubSubRepository pubsubRepository,
            PostsRepository postsRepository,
            BucketsRepository bucketsRepository) { 
            
            _pubSubRepository= pubsubRepository;
            _postsRepository= postsRepository;
            _bucketsRepository = bucketsRepository;  
        }

        public async Task<IActionResult> Generate () {
            //step 1: pull from the queue the next blog id to be turned as a pdf
           string blogId= 
                _pubSubRepository.PullMessagesSync("msd63b2024_ra-sub", false); //until i confirm that it is working fine!

            if (string.IsNullOrEmpty(blogId) == false)
            {
                //we have blogid with data

                //putting everything inside a pdf.
                PdfDocument document = new PdfDocument();

                // Add a new page to the document
                PdfPage page = document.AddPage();

                //step 2: getting posts for the blog
                var myPosts = await _postsRepository.GetPosts(blogId); //will get a list of posts pertaining to a blog

                //step 3: report generation
                int yPosition = 10;

                GlobalFontSettings.FontResolver = new FileFontResolver();
                XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
                // Get an XGraphics object for drawing
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    foreach (var post in myPosts)
                    {
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
                }
                //step 4: saving the pdf on the hard drive
                string filenamePDF = blogId + ".pdf";

                MemoryStream msTemp= new MemoryStream();
                document.Save(msTemp);
                msTemp.Position = 0;
                //code to open back the file and upload it.

                //MemoryStream msIn = new MemoryStream(System.IO.File.ReadAllBytes(filenamePDF));
                //msIn.Position = 0;
                await _bucketsRepository.UploadFileAsync(filenamePDF, msTemp);
                //System.IO.File.Delete(filenamePDF);


                //you need to update the status of the conversion to done
                //status >> true

                //grant access to the recipient who is able to download the file

                return Content("pdf generated - done");
            }
            else return Content("error occurred");

        }
    }

    public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Verdana", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Fonts/Verdana-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Fonts/Verdana-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Fonts/Verdana-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Fonts/Verdana.ttf");
                }
            }
            return null;
        }
    }
}
