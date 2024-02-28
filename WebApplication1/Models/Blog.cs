using Google.Cloud.Firestore;

namespace WebApplication1.Models
{
    [FirestoreData]
    public class Blog
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Timestamp DateUpdated { get; set; }
        [FirestoreProperty]
        public string Author { get; set; }
        [FirestoreProperty]
        public string PictureUri { get; set; }
    }
}
