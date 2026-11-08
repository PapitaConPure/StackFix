using System;
using System.Collections.Generic;
using System.Text;

namespace StackFix.Items {
	internal sealed class BolsteringLantern: IItemFix {
		private const float VANILLA_ATTACK_SPEED_BASE = 10f;
		private const float VANILLA_ATTACK_SPEED_STACK = 3.5f;
		private const int VANILLA_ENEMY_COUNT_BASE = 3;
		private const int VANILLA_ENEMY_COUNT_STACK = 1;

		public bool Enabled => throw new NotImplementedException();

		public void ApplyFix() {
			throw new NotImplementedException();
		}
	}
}
