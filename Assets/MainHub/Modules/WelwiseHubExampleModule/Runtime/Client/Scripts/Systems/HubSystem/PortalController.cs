using System;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseSharedModule.Runtime.Client.Scripts.Tools;
using Vector2 = UnityEngine.Vector2;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.HubSystem
{
    public class PortalController
    {
        public event Action<int> OwnerEnteredToPortal;

        private int _gameId = -1;

        private readonly PortalSerializableComponents _portalSerializableComponents;
        private readonly PortalsConfig _portalsConfig;
        private readonly string _portalMaterialsUrl;
        private readonly float _timeBeforeSetActiveVideoSideImages = 0.5f;

        public PortalController(PortalSerializableComponents portalSerializableComponents, int portalId,
            PortalsConfig portalsConfig, IPlatformNavigation platformNavigation, Action<string> gotErrorOnGoingToGame)
        {
            _portalSerializableComponents = portalSerializableComponents;
            _portalsConfig = portalsConfig;

            _portalMaterialsUrl = portalsConfig.MaterialsUrl + "/" + portalId + "/";

            SetActiveVideoSideImages(false);

            LoadAndAppointGameIdAsync();

            LoadAndAppointImageSpriteIfIsntNullAsync(portalSerializableComponents.CoverImage,
                _portalMaterialsUrl + portalsConfig.CoverFileName);
            LoadAndAppointImageSpriteIfIsntNullAsync(portalSerializableComponents.IconImage,
                _portalMaterialsUrl + portalsConfig.IconFileName);

            portalSerializableComponents.ShowingVideoZoneColliderObserver.Entered +=
                collider => TrySettingVideoStateAsync(collider, true);
            portalSerializableComponents.ShowingVideoZoneColliderObserver.Exited +=
                collider => TrySettingVideoStateAsync(collider, false);

            portalSerializableComponents.EnteringGameZoneColliderObserver.Entered += collider
                =>
            {
                _portalSerializableComponents.MusicAudioSource.SetPitchAndPlayOneShot(
                    portalsConfig.EnterPortalAudioClip);
                
                if (!collider.gameObject.TryGetComponent<NetworkObject>(out var networkObject) || !networkObject.IsOwner)
                    return;
                
                OwnerEnteredToPortal?.Invoke(_gameId);
                platformNavigation.GoToGame(_gameId, gotErrorOnGoingToGame);
            };

            _portalSerializableComponents.VideoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleteAsync;
        }

        private async void OnVideoPlayerPrepareCompleteAsync(VideoPlayer player)
        {
            var texture = new RenderTexture((int)player.width, (int)player.height,
                (int)GraphicsFormat.D32_SFloat_S8_UInt);

            _portalSerializableComponents.VideoPlayer.targetTexture = texture;
            _portalSerializableComponents.VideoRawImage.texture = texture;

            player.Play();

            await UniTask.Delay(TimeSpan.FromSeconds(_timeBeforeSetActiveVideoSideImages));

            SetActiveVideoSideImages(true);
        }

        private async void LoadAndAppointGameIdAsync()
        {
            var url = _portalMaterialsUrl + _portalsConfig.GameIdFileName;

            var webRequest = UnityWebRequest.Get(_portalMaterialsUrl + _portalsConfig.GameIdFileName);

            try
            {
                await webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                    _gameId = Convert.ToInt32(webRequest.downloadHandler.text);
                // else
                //     Debug.LogError(url + ": Exception: " + webRequest.error + ", Request Error: " +
                //                    webRequest.downloadHandler.error);
            }
            catch (Exception exception)
            {
                //Debug.LogError(url + ": Exception: " + exception + ", Request Error: " +
                //webRequest.downloadHandler.error;
            }
        }

        private async void LoadAndAppointImageSpriteIfIsntNullAsync(Image image, string url)
        {
            var sprite = await GetDownloadedSpriteAsync(url);

            if (sprite)
                image.sprite = sprite;
        }

        private async UniTask<Sprite> GetDownloadedSpriteAsync(string url)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);

            try
            {
                await webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    // Debug.LogError(url + ": Exception: " + webRequest.error + ", Request Error: " +
                    //                webRequest.downloadHandler.error);
                    return null;
                }

                var texture = DownloadHandlerTexture.GetContent(webRequest);

                if (texture)
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));

                // Debug.LogError(url + ": Texture is null!");
                return null;
            }
            catch (Exception exception)
            {
                // Debug.LogError(url + ": Exception: " + exception + ", Request Error: " +
                //                webRequest.downloadHandler.error);
                return null;
            }
        }

        private async void TrySettingVideoStateAsync(Collider collider, bool didEnterPortalCollider)
        {
            if (!collider.TryGetComponent<NetworkObject>(out var networkObject) || !networkObject.IsOwner) return;

            switch (didEnterPortalCollider)
            {
                case true when !_portalSerializableComponents.VideoPlayer.isPrepared:
                    _portalSerializableComponents.VideoPlayer.source = VideoSource.Url;
                    _portalSerializableComponents.VideoPlayer.url = _portalMaterialsUrl + _portalsConfig.VideoFileName;
                    _portalSerializableComponents.VideoPlayer.Prepare();
                    return;
                case true:
                    _portalSerializableComponents.VideoPlayer.Play();
                    break;
                default:
                    _portalSerializableComponents.VideoPlayer.Stop();
                    break;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_timeBeforeSetActiveVideoSideImages));

            SetActiveVideoSideImages(didEnterPortalCollider);
        }

        private void SetActiveVideoSideImages(bool isStartedVideo)
        {
            _portalSerializableComponents.CoverImage.gameObject.SetActive(!isStartedVideo);
            _portalSerializableComponents.VideoRawImage.gameObject.SetActive(isStartedVideo);
        }
    }
}