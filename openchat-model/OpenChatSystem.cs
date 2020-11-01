namespace OpenChat.Model
{
    public class OpenChatSystem
    {
        public User RegisterUser(string userName, string password, string about = "")
        {
            return User.Create(userName, about);
        }
    }
}