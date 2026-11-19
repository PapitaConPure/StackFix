using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StackFix.Utils {
	internal record MatchResult(bool Success, int Index, int Length);

	internal interface IMatchablePattern {
		public MatchResult Match(string input, int startIndex = 0);
	}

	internal record StringPattern(string MatchString): IMatchablePattern {
		public MatchResult Match(string input, int startIndex) {
			int index = input.IndexOf(this.MatchString, startIndex);

			if(index < 0)
				return new(false, -1, 0);

			int length = this.MatchString.Length;

			return new(true, index, length);
		}

		public override string ToString() => $"[Pattern: \"{this.MatchString}\"]";
	}

	internal record RegexPattern(Regex Matcher): IMatchablePattern {
		public MatchResult Match(string input, int startIndex) {
			Match match = this.Matcher.Match(input, startIndex);
			return new(match.Success, match.Index, match.Length);
		}

		public override string ToString() => $"[Pattern: /{this.Matcher}/]";
	}

	internal record ReplacementPattern(IMatchablePattern Pattern, string ReplacementFormat, params IEnumerable<object> ReplacementArgs) {
		public ReplacementPattern(string Pattern, string ReplacementFormat, params IEnumerable<object> ReplacementArgs)
			: this(new StringPattern(Pattern), ReplacementFormat, ReplacementArgs) { }

		public ReplacementPattern(Regex Pattern, string ReplacementFormat, params IEnumerable<object> ReplacementArgs)
			: this(new RegexPattern(Pattern), ReplacementFormat, ReplacementArgs) { }
	}
}
