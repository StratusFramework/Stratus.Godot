using Godot;

using Stratus.Interpolation;
using System.Reflection;

namespace Stratus.Godot.Extensions
{
    public static class ColorExtensions
    {
        public static Color Alpha(this Color color, float alpha)
        {
            return new Color(color, alpha);
        }

    }

	public class ActionPropertyColor : ActionProperty<Color>
	{
		public ActionPropertyColor(object target, MemberInfo member, Color endValue, float duration, Ease ease) : base(target, member, endValue, duration, ease)
		{
		}

		public override void ComputeDifference()
		{
			this.difference = this.endValue - this.initialValue;
		}

		public override Color ComputeCurrentValue(float easeVal)
		{
			return this.initialValue + this.difference * easeVal;
		}
	}
}
