using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using UnityEngine;

namespace WelwiseSharedModule.Runtime.Client.Scripts.NetworkModule
{
    public static class SharedNetworkTools
    {
        public static bool IsOwners(this NetworkConnection networkConnection) => OwnerConnection == networkConnection;
        public static NetworkConnection OwnerConnection => InstanceFinder.ClientManager.Connection;
        public static T GetOwners<T>(this IReadOnlyDictionary<NetworkConnection, T> data) => data.GetValueOrDefault(OwnerConnection);

        
        public static void ReappointTransformsAndRebindAnimator(this Transform clientsAnimatorChildrenParent, Animator animator)
        {
            var children = Enumerable.Range(0, clientsAnimatorChildrenParent.childCount)
                .Select(i =>
                    clientsAnimatorChildrenParent.transform.GetChild(i)).ToList();

            var boolParams = new Dictionary<string, bool>();
            var floatParams = new Dictionary<string, float>();
            var intParams = new Dictionary<string, int>();

            foreach (var param in animator.parameters)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        boolParams[param.name] = animator.GetBool(param.name);
                        break;
                    case AnimatorControllerParameterType.Float:
                        floatParams[param.name] = animator.GetFloat(param.name);
                        break;
                    case AnimatorControllerParameterType.Int:
                        intParams[param.name] = animator.GetInteger(param.name);
                        break;
                }
            }
            
            children.ForEach(child => child.SetParent(animator.transform));
            
            animator.Rebind();
            
            foreach (var kvp in boolParams)
                animator.SetBool(kvp.Key, kvp.Value);
            foreach (var kvp in floatParams)
                animator.SetFloat(kvp.Key, kvp.Value);
            foreach (var kvp in intParams)
                animator.SetInteger(kvp.Key, kvp.Value);
        }
    }
}