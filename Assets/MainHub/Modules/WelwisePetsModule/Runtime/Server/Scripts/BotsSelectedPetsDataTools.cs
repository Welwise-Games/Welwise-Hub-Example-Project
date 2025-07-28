using System.Linq;
using WelwisePetsModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Tools;

namespace WelwisePetsModule.Runtime.Server.Scripts
{
    public static class BotsSelectedPetsDataTools
    {
        public static SelectedPetsData GetRandomSelectedPetsData(float setDataChance, PetsConfig petsConfig,
            SelectedPetsData currentData = null)
        {
            var petsIndexes = petsConfig.Configs.Select(config => config.Index).Append(null).ToList();

            return new SelectedPetsData(Enumerable.Range(0, petsConfig.MaxSelectedItemsNumber).Select(i =>
            {
                var petIndex = currentData == null || setDataChance.UseAsChanceAndGetResult()
                    ? petsIndexes.GetRandomOrDefault()
                    : currentData.SelectedItemsData[i].Index;
                var petData = new SelectedPetData(i, petIndex);

                if (petIndex != null)
                    petsIndexes.Remove(petIndex);

                return petData;
            }).ToList(), petsConfig);
        }
    }
}