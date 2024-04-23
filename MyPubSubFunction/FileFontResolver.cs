using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.IO;
using System;
using Google.Cloud.Storage.V1;

 namespace MyPubSubFunction
 {
 
 
 public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
           /* using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }*/

              var storage = StorageClient.Create();
              MemoryStream stream = new MemoryStream();
              storage.DownloadObject("msd63b2024ra_fg", "Verdana.ttf", stream);
              stream.Position = 0;
              return stream.ToArray();

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