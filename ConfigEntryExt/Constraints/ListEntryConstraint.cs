using BepInEx.Configuration;
using System;
using ConfigEntryExt.Patches;


namespace ConfigEntryExt.Constraints {
	public sealed record ListEntryConstraint<TValue>(params TValue[] Values): IEntryConstraint<TValue> where TValue : struct, Enum {
		public ListEntryConstraint(Type ValuesEnum) : this(Enum.GetValues(ValuesEnum) as TValue[]) { }

		public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueEnum<TValue>(Values);

		public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

		public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
	}
}
