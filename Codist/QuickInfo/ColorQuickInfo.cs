﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using AppHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Codist.QuickInfo
{
	sealed class ColorQuickInfoController : IQuickInfoSource
	{
		/// <summary>
		/// Provides quick info for named colors or #hex colors
		/// </summary>
		[Export(typeof(IQuickInfoSourceProvider))]
		[Name("Color Quick Info Controller")]
		[Order(After = "Default Quick Info Presenter")]
		[ContentType(Constants.CodeTypes.Text)]
		sealed class ColorQuickInfoControllerProvider : IQuickInfoSourceProvider
		{
			[Import]
			internal ITextStructureNavigatorSelectorService _NavigatorService = null;

			public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer) {
				return Config.Instance.Features.MatchFlags(Features.SuperQuickInfo)
					? new ColorQuickInfoController(_NavigatorService, textBuffer.ContentType.IsOfType(Constants.CodeTypes.CSharp))
					: null;
			}
		}

		readonly ITextStructureNavigatorSelectorService _NavigatorService;
		readonly bool _ParseSystemColor;

		public ColorQuickInfoController(ITextStructureNavigatorSelectorService navigatorService, bool parseSystemColor) {
			_NavigatorService = navigatorService;
			//_ParseSystemColor = parseSystemColor;
		}

		public void AugmentQuickInfoSession(IQuickInfoSession session, IList<Object> qiContent, out ITrackingSpan applicableToSpan) {
			if (Config.Instance.QuickInfoOptions.MatchFlags(QuickInfoOptions.Color) == false) {
				goto EXIT;
			}
			var buffer = session.TextView.TextBuffer;
			var snapshot = session.TextView.TextSnapshot;
			var navigator = _NavigatorService.GetTextStructureNavigator(buffer);
			var extent = navigator.GetExtentOfWord(session.GetTriggerPoint(snapshot).GetValueOrDefault()).Span;
			var word = snapshot.GetText(extent);
			var brush = UIHelper.GetBrush(word, _ParseSystemColor);
			if (brush == null) {
				if ((extent.Length == 6 || extent.Length == 8) && extent.Span.Start > 0 && Char.IsPunctuation(snapshot.GetText(extent.Span.Start - 1, 1)[0])) {
					word = "#" + word;
				}
				brush = UIHelper.GetBrush(word, _ParseSystemColor);
			}
			if (brush != null) {
				qiContent.Add(ColorQuickInfo.PreviewColor(brush));
				applicableToSpan = snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeExclusive);
				return;
			}
			EXIT:
			applicableToSpan = null;
		}

		void IDisposable.Dispose() { }
	}

	static class ColorQuickInfo
	{
		public static StackPanel PreviewColorMethodInvocation(SemanticModel semanticModel, SyntaxNode node, IMethodSymbol methodSymbol) {
			StackPanel preview = null;
			switch (methodSymbol.Name) {
				case nameof(Color.FromArgb):
					var args = GetColorMethodArguments(semanticModel, node, 4);
					if (args != null) {
						preview = PreviewColor(new SolidColorBrush(Color.FromArgb(args[0], args[1], args[2], args[3])));
					}
					break;
				case nameof(Color.FromRgb):
					args = GetColorMethodArguments(semanticModel, node, 3);
					if (args != null) {
						preview = PreviewColor(new SolidColorBrush(Color.FromRgb(args[0], args[1], args[2])));
					}
					break;
			}
			return preview;
		}

		public static StackPanel PreviewColor(SolidColorBrush brush) {
			const string SAMPLE = "Hi,WM.";
			var c = brush.Color;
			var v = Microsoft.VisualStudio.Imaging.HslColor.FromColor(c);
			return new StackPanel {
				Children = {
					new ToolTipText().Append(new System.Windows.Shapes.Rectangle { Width = 16, Height = 16, Fill = brush }).Append("Color", true),
					new StackPanel().AddReadOnlyTextBox($"{c.A}, {c.R}, {c.G}, {c.B}").Add(new ToolTipText(" ARGB", true)).MakeHorizontal(),
					new StackPanel().AddReadOnlyTextBox(c.ToHexString()).Add(new ToolTipText(" HEX", true)).MakeHorizontal(),
					new StackPanel().AddReadOnlyTextBox($"{v.Hue.ToString("0.###")}, {v.Saturation.ToString("0.###")}, {v.Luminosity.ToString("0.###")}").Add(new ToolTipText(" HSL", true)).MakeHorizontal(),
					new StackPanel {
						Children = {
							new TextBox { Text = SAMPLE, BorderBrush = Brushes.Black, Foreground = brush, Background = Brushes.Black },
							new TextBox { Text = SAMPLE, BorderBrush = Brushes.Black, Background = brush, Foreground = Brushes.Black },
							new TextBox { Text = SAMPLE, BorderBrush = Brushes.Black, Foreground = brush, Background = Brushes.Gray },
							new TextBox { Text = SAMPLE, BorderBrush = Brushes.Black, Background = brush, Foreground = Brushes.White },
							new TextBox { Text = SAMPLE, BorderBrush = Brushes.Black, Foreground = brush, Background = Brushes.White },
						}
					}.MakeHorizontal(),
				}
			};
		}

		static byte[] GetColorMethodArguments(SemanticModel semanticModel, SyntaxNode node, int length) {
			var invoke = node?.Parent?.Parent as InvocationExpressionSyntax;
			if (invoke == null) {
				return null;
			}
			var args = invoke.ArgumentList.Arguments;
			if (args.Count != length) {
				return null;
			}
			var r = new byte[length];
			for (int i = 0; i < length; i++) {
				var a1 = args[i].Expression;
				if (a1.Kind() == SyntaxKind.NumericLiteralExpression) {
					r[i] = Convert.ToByte((a1 as LiteralExpressionSyntax).Token.Value);
					continue;
				}
				if (a1.Kind() == SyntaxKind.SimpleMemberAccessExpression) {
					var m = a1 as MemberAccessExpressionSyntax;
					var s = semanticModel.GetSymbolInfo(a1).Symbol;
					if (s == null) {
						return null;
					}
					if (s.Kind == SymbolKind.Field) {
						var f = s as IFieldSymbol;
						if (f.HasConstantValue) {
							r[i] = Convert.ToByte(f.ConstantValue);
							continue;
						}
					}
				}
				return null;
			}
			return r;
		}
	}
}
