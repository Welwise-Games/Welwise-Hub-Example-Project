using UnityEngine;

namespace WelwiseHubExampleModule.Runtime.Client.Scripts.Systems.TrainingSystem
{
    public interface IArrowsPathProvider
    {
        Vector3[] GetPath();
    }
}