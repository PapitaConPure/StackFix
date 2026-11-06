using System;

namespace StackFix.Exceptions {
	internal class ILSeekException: Exception {
		public ILSeekException(string message) : base(message) {
		}
	}
}
