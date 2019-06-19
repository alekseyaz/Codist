﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using AppHelpers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Codist.Classifiers
{
	[Export(typeof(IViewTaggerProvider))]
	[ContentType(Constants.CodeTypes.Code)]
	[TagType(typeof(IClassificationTag))]
	sealed class MarkdownTaggerProvider : IViewTaggerProvider
	{
		static readonly IClassificationType[] _HeaderClassificationTypes = new IClassificationType[7];

		public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
			if (Config.Instance.Features.MatchFlags(Features.SyntaxHighlight) == false) {
				return null;
			}
			if (textView.TextBuffer.LikeContentType(Constants.CodeTypes.Markdown) == false) {
				return null;
			}
			if (_HeaderClassificationTypes[1] == null) {
				InitHeaderClassificationTypes();
			}
			textView.Closed += TextViewClosed;
			return textView.Properties.GetOrCreateSingletonProperty(() => new MarkdownTagger(textView)) as ITagger<T>;
		}

		static void InitHeaderClassificationTypes() {
			var r = ServicesHelper.Instance.ClassificationTypeRegistry;
			_HeaderClassificationTypes[1] = r.GetClassificationType(Constants.MarkdownHeading1);
			_HeaderClassificationTypes[2] = r.GetClassificationType(Constants.MarkdownHeading2);
			_HeaderClassificationTypes[3] = r.GetClassificationType(Constants.MarkdownHeading3);
			_HeaderClassificationTypes[4] = r.GetClassificationType(Constants.MarkdownHeading4);
			_HeaderClassificationTypes[5] = r.GetClassificationType(Constants.MarkdownHeading5);
			_HeaderClassificationTypes[6] = r.GetClassificationType(Constants.MarkdownHeading6);
		}

		void TextViewClosed(object sender, EventArgs args) {
			var textView = sender as ITextView;
			textView.Closed -= TextViewClosed;
		}

		sealed class MarkdownTagger : CachedTaggerBase
		{
			public MarkdownTagger(ITextView textView) : base(textView) {
			}

			protected override TaggedContentSpan Parse(ITextSnapshot textSnapshot, SnapshotSpan span) {
				var t = span.GetText();
				if (t.Length > 0 && t[0] == '#') {
					int c = 1, w = 0;
					for (int i = 1; i < t.Length; i++) {
						switch (t[i]) {
							case '#': if (w == 0) { ++c; } continue;
							case ' ': ++w; continue;
						}
						break;
					}
					w += c;
					return new TaggedContentSpan(textSnapshot, new ClassificationTag(_HeaderClassificationTypes[c]), span.Start, t.Length, w, t.Length - w);
				}
				return null;
			}
		}
	}
}
