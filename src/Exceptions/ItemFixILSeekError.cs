using MonoMod.Cil;
using System;

namespace StackFix.Exceptions {
	internal sealed class ILSeekException(ILContext il, string expectedDesc, string methodName)
		: Exception($"Failed to match {expectedDesc} IL in {methodName}") {
		public string ILCode { get; } = il.ToString();

		public override string ToString()
			=> $"{base.ToString()}\n===\n{this.ILCode}\n===";
	}
}
