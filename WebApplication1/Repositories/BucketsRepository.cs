using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System.Security.AccessControl;
using System.Text;

namespace WebApplication1.Repositories
{
    public class BucketsRepository
    {

        string _bucketId;
        string _projectId;
        public BucketsRepository(string projectId, string bucketId) {
            _bucketId= bucketId;    
            _projectId= projectId;
        
        }


        public async Task<Google.Apis.Storage.v1.Data.Object> UploadFileAsync(string filename, MemoryStream ms)
        {
            var storage = StorageClient.Create();
            return await storage.UploadObjectAsync(_bucketId, filename, "application/octet-stream", ms);
        }


        public async Task<Google.Apis.Storage.v1.Data.Object> GrantAccess(string filename, string recipient)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(_bucketId, filename, new GetObjectOptions
            {
                Projection = Projection.Full
            });

            storageObject.Acl.Add(new ObjectAccessControl
            {
                Bucket = _bucketId,
                Entity = $"user-{recipient}",
                Role = "READER", 
                //READER permission will be given to the website users
                //SO if the users will access the link to the file directly they can only read it
            });
            return await storage.UpdateObjectAsync(storageObject);
        }
    }
}
