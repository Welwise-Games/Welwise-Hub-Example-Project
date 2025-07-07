namespace WelwiseChatModule.Runtime.Shared.Scripts.Network
{
    public class ChatMessageData
    {
        public  string Content { get; set; }
        public readonly string AuthorNickname;

        public ChatMessageData() { }
        
        public ChatMessageData(string content, string authorNickname)
        {
            AuthorNickname = authorNickname;
            Content = content;
        }
    }
}