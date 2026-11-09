namespace StackFix {
	internal interface IItemFix {
		public bool Enabled { get; }
		public bool Applied { get; }
		public void ApplyFix();
	}
}
