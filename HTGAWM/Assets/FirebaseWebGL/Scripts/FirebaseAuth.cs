using System.Runtime.InteropServices;

namespace FirebaseWebGL.Scripts.FirebaseBridge
{
    public static class FirebaseAuth
    {
        [DllImport("__Internal")]
        public static extern void CreateUserWithEmailAndPassword(string email, string password, string objectName, string callback, string fallback);

        [DllImport("__Internal")]
        public static extern void SignInWithEmailAndPassword(string email, string password, string objectName, string callback, string fallback);

        [DllImport("__Internal")]
        public static extern void SignInWithGoogle(string objectName, string callback, string fallback);

        [DllImport("__Internal")]
        public static extern void SignInWithFacebook(string objectName, string callback, string fallback);

    }
}