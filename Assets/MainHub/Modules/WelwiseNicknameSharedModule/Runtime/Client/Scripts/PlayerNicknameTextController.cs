using TMPro;

namespace WelwiseNicknameSharedModule.Runtime.Client.Scripts
{
    public class PlayerNicknameTextController
    {
        private readonly TMP_Text _text;

        public PlayerNicknameTextController(TMP_Text text, string nickname)
        {
            _text = text;
            
            SetNickname(nickname);
        }

        public void SetNickname(string nickname) => _text.text = nickname;
    }
}
