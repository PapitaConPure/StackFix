namespace StackFix {
	internal interface IItemFix {
		public bool Enabled { get; }
		public void ApplyFix();
	}
}
