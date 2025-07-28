using System;

namespace WelwiseMiniGamesModule.Runtime.DoodleJump.Scripts.Tools
{
	[Serializable]
	public struct RangedFloat
	{
		public float minValue;
		public float maxValue;

		public float Random()
		{
			return UnityEngine.Random.Range(minValue, maxValue);
		}
	}
}