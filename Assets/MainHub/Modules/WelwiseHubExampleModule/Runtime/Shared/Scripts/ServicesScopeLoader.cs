using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Shared.Scripts
{
    public class ServicesScopeLoader : MonoBehaviour
    {
        private void Awake()
        {
            new ServicesScopeFactory().CreateServicesScope();
            Destroy(gameObject);
        }
    }
}
