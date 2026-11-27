using System;
using System.Collections.Generic;

namespace ConfigEntryExt {
	public static class DescriptionModifiers {
		private sealed class Modifier(Func<string, string> modifyFn): IDisposable {
			Func<string, string> modifyFn = modifyFn;

			public Func<string, string> Modify => modifyFn;

			public void Dispose() {
				this.modifyFn = null;
			}
		}

		private static readonly List<Modifier> Modifiers = [];

		public static void Register(Func<string, string> modifier) {
			if(modifier == null)
				throw new ArgumentNullException(nameof(modifier));

			Modifiers.Add(new(modifier));
		}

		public static void Clear() {
			foreach(Modifier modifier in Modifiers)
				modifier?.Dispose();

			Modifiers.Clear();
		}

		internal static string Apply(string description) {
			foreach(Modifier modifier in Modifiers)
				description = modifier.Modify(description)
					?? throw new InvalidOperationException("A description modifier returned null.");

			return description;
		}
	}
}
