using Firebase.Database;
using FirebaseRepos.Reposatories;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseRepos.WPFSimple.Models
{
    public static class Context
    {
        //Details for firebase realtime database
        private static string baseURL = "https://occasion-b8c88-default-rtdb.firebaseio.com/";
        private static string apiSecret = "xPn6BUewzcYVisw2bPu0QPILKF5hBvxBF9mblRxO";


        ////Details for firestore
        //private static string projectID = "occasion-b8c88";


        ////Details for Storage
        //private static string apiName = "occasion-b8c88.appspot.com";
        //private static string apiKey = "AIzaSyDn5cUTgR_mcIdjAHKgu80rFM52q9xqdIo";
        //private static string uploadEmail = "y.s.f.me2@gmail.com";
        //private static string uploadEmailPawword = "YSFysf251521191956";

        private static readonly FirebaseClient _firebaseClient = new FirebaseClient(baseURL, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(apiSecret) });
        public static RealTimeRepo<User> Users { get; private set; } = new RealTimeRepo<User>(_firebaseClient.Child("Users"), true);
        //public static Storage Storage { get; } = new Storage(apiName, apiKey, uploadEmail, uploadEmailPawword); 
        //private static readonly FirestoreDb FirestoreDb = FirestoreDb.Create(projectID);
    }
}
