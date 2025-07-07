namespace WelwiseChangingAnimationModule.Runtime.Shared.Scripts
{
    public struct AnimationHashesData
    {
        public readonly int DoesPlayHash, StateHash;

        public AnimationHashesData(int doesPlayHash, int stateHash)
        {
            DoesPlayHash = doesPlayHash;
            StateHash = stateHash;
        }
    }
}