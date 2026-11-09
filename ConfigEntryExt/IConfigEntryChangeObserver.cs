using System;

namespace ConfigEntryExt {
	public interface IConfigEntryChangeObserver {
		void ObserveConfigEntryChanged(EventHandler handler);
	}
}
