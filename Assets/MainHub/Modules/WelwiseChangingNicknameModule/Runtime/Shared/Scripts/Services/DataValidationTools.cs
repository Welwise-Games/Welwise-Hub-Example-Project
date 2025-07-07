using System.Text.RegularExpressions;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwiseChangingNicknameModule.Runtime.Shared.Scripts.Services
{
    public static class DataValidationTools
    {
        public static bool IsValidNickname(string currentNickname, string newNickname,
            SharedClientsNicknamesConfig clientsConfig) =>
            !newNickname.IsNullOrEmptyOrWhiteSpace() && newNickname != currentNickname &&
            newNickname.Length <= clientsConfig.MaximumNicknameLength &&
            newNickname.Length >= clientsConfig.MinimalNicknameLength && DoesntHaveNickIncorrectSymbols(newNickname);

        private static bool DoesntHaveNickIncorrectSymbols(string nickname) =>
            Regex.IsMatch(nickname, @"^[a-zA-Zа-яА-Я0-9 ]+$");
    }
}