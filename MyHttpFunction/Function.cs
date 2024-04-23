using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using System;

namespace MyHttpFunction;

public class Function : IHttpFunction
{


    public Function()
    {
        //only to test it locally on your laptops
        System.Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
             "msd63b2024-e89abcf33d84.json");
    }

    /// <summary>
    /// This function will take backup of a pdf file associated with a blog id
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {
        string sourceBucket = "msd63b2024ra_fg";
        string destinationBucket = "msd63b_backup_ra";

        string blogId = context.Request.Query["blogId"];
        //string uniqueFilename = Guid.NewGuid().ToString();
        CopyFile(sourceBucket, blogId+".pdf",  destinationBucket, blogId+"_backup.pdf");

        await context.Response.WriteAsync("Function complete!");
    }

    public void CopyFile(
        string sourceBucketName = "source-bucket-name",
        string sourceObjectName = "source-file",
        string destBucketName = "destination-bucket-name",
        string destObjectName = "destination-file-name")
    {
        var storage = StorageClient.Create();
        storage.CopyObject(sourceBucketName, sourceObjectName, destBucketName, destObjectName);

        Console.WriteLine($"Copied {sourceBucketName}/{sourceObjectName} to " + $"{destBucketName}/{destObjectName}.");
    }

}
