using Google.Cloud.Firestore;

namespace WebApplication1.Models
{
    [FirestoreData]
    public class Post
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Timestamp DateUpdated { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }

        public string BlogId_FK { get; set; }

        
    }
}
