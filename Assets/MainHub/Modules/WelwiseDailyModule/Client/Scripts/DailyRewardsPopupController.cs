using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WelwiseSharedModule.Runtime.Shared.Scripts;
using WelwiseSharedModule.Runtime.Shared.Scripts.Loading;
using WelwiseSharedModule.Runtime.Shared.Scripts.Observers;

public class DailyRewardsPopupView : MonoBehaviour
{
    [field: SerializeField] public Button[] GetRewardForDayButtons { get; private set; }
    [field: SerializeField] public Slider TotalDaysSlider { get; private set; }
    //[field: SerializeField] public TextMeshProUGUI TotalDaysText { get; private set; }
}

[CreateAssetMenu(menuName = "WelwiseDailyRewardsModule/RewardsConfig")]
public class DailyRewardsConfig : ScriptableObject
{
    [field: SerializeField] public DailyRewardConfig[] RewardConfigs { get; private set; }
}

public class DailyRewardConfigProviderService
{
    private readonly IAssetLoader _assetLoader;
    private readonly Container _container = new Container();

    private const string DailyRewardsConfigAssetId =
#if ADDRESSABLES
        "DailyRewardsConfig";
#else
    "WelwiseDailyRewardsModule/Shared/Loadable/DailyRewardsConfig";
#endif

    public DailyRewardConfigProviderService(IAssetLoader assetLoader)
    {
        _assetLoader = assetLoader;
    }

    public async UniTask<DailyRewardsConfig> GetDailyRewardsConfigAsync() =>
        await _container.GetOrLoadAndRegisterObjectAsync<DailyRewardsConfig>(DailyRewardsConfigAssetId, _assetLoader);
}

public struct DailyRewardsData
{
    public List<int> GotDailyRewardsIds;
    public long RegistrationDataInTicks;
}

[Serializable]
public class DailyRewardConfig
{
    [field: SerializeField] public int SecondsBeforeGet { get; private set; }
    [field: SerializeField] public int RewardId { get; private set; }
}

public interface IDailyRewardsDataProviderService
{
    DailyRewardsData Data { get; }
    event Action<DailyRewardsData> AddedData;
}

public class DailyRewardsPopupController
{
    public DailyRewardsPopupController(DailyRewardsPopupView popupView)
    {
        
    }
}