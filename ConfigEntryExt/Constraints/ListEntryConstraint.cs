using BepInEx.Configuration;
using ConfigEntryExt.Patches;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigEntryExt.Constraints {
	public sealed record ListEntryConstraint<TValue>(params IReadOnlyList<AcceptableValueEnum<TValue>.NamedAcceptableValue> Values): IEntryConstraint<TValue> where TValue : struct, Enum {
		private ListEntryConstraint(params IReadOnlyList<TValue> Values)
			: this([..Values.Select(v => new AcceptableValueEnum<TValue>.NamedAcceptableValue(v, null))]) { }

		public ListEntryConstraint(Type ValuesEnum): this(Enum.GetValues(ValuesEnum) as IReadOnlyList<TValue>) {}

		public AcceptableValueBase AcceptableValue { get; } = new AcceptableValueEnum<TValue>(Values);

		public TValue Clamp(TValue value) => (TValue)this.AcceptableValue.Clamp(value);

		public bool IsValid(object value) => this.AcceptableValue.IsValid(value);
	}
}
