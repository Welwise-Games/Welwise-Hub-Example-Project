namespace WelwiseSharedModule.Runtime.Shared.Scripts.Tools
{
    public static class CustomMathTools
    {
        public static bool IsInsideRange(this float value, float minimumValue, float maximumValue) => value >= minimumValue && value <= maximumValue;
    }
}