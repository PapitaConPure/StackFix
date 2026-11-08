using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ConfigEntryExt.Patches {
	//why the fuck is AcceptableValueList constrained to IEquatable<T> im going to ggghghtrhrgAERHAWWWGAERATHRYSHGJN
	public sealed class AcceptableValueEnum<TValue>: AcceptableValueBase where TValue : IComparable {
		public record NamedAcceptableValue(TValue Value, string Name = null);

		public IReadOnlyList<NamedAcceptableValue> AcceptableValues { get; }

		public AcceptableValueEnum(IReadOnlyList<NamedAcceptableValue> acceptableValues) : base(typeof(TValue)) {
			if(acceptableValues == null)
				throw new ArgumentNullException(nameof(acceptableValues));

			if(acceptableValues.Count == 0)
				throw new ArgumentException("At least one acceptable value is needed.", nameof(acceptableValues));

			this.AcceptableValues = acceptableValues;
		}

		public override object Clamp(object value)
			=> this.IsValid(value) ? value : this.AcceptableValues[0];

		public override bool IsValid(object value)
			=> value is TValue v1 && this.AcceptableValues.Any(v2 => v1.Equals(v2.Value));

		public override string ToDescriptionString()
			=> "# Acceptable values: " + string.Join(", ", [.. this.AcceptableValues.Select(GetValueDescription)]);

		private static string GetValueDescription(NamedAcceptableValue namedValue) {
			if(!string.IsNullOrWhiteSpace(namedValue.Name))
				return namedValue.Name;

			string name = namedValue.Value.ToString();

			return typeof(TValue).GetField(name)
				?.GetCustomAttribute<DescriptionAttribute>()
				?.Description ?? name;
		}
	}
}
