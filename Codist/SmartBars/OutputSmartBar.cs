﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Text.Editor;
using R = Codist.Properties.Resources;

namespace Codist.SmartBars
{
	sealed class OutputSmartBar : SmartBar
	{
		public OutputSmartBar(IWpfTextView textView, Microsoft.VisualStudio.Text.Operations.ITextSearchService2 textSearchService) : base(textView, textSearchService) {
		}

		ToolBar MyToolBar => ToolBar;

		protected override void AddCommands(CancellationToken cancellationToken) {
			const int PathIsFile = 1, PathIsDirectory = 2;

			base.AddCommands(cancellationToken);
			int p;
			string t;
			try {
				t = TryGetPathFromView(View, out p);
			}
			catch (Exception ex) {
				CodistPackage.ShowErrorMessageBox(ex.Message, null, true);
				return;
			}
			switch (p) {
				case PathIsFile:
					AddCommand(MyToolBar, IconIds.Open, R.CMD_OpenOrExecuteFile, ctx => {
						TryRun(t);
					});
					AddCommand(MyToolBar, IconIds.OpenFolder, R.CMD_OpenFolder, ctx => {
						Process.Start(new ProcessStartInfo("Explorer.exe", "/select,\"" + t + "\"") { WorkingDirectory = Environment.SystemDirectory });
					});
					switch (Path.GetExtension(t).ToLowerInvariant()) {
						case ".exe":
						case ".bat":
						case ".cmd":
						case ".py":
						case ".js":
						case ".vbs":
						case ".com":
						case ".wsf":
							AddCommand(MyToolBar, IconIds.OpenWithCmd, R.CMD_OpenFileInCmd, ctx => {
								Process.Start(new ProcessStartInfo(Environment.SystemDirectory + "\\cmd.exe", "/K \"" + t + "\"") { WorkingDirectory = Path.GetDirectoryName(t) });
							});
							break;
					}
					if (IsFileTypeRegisteredInVS(t)) {
						AddCommand(MyToolBar, IconIds.OpenWithVisualStudio, R.CMD_OpenWithVS, ctx => {
							CodistPackage.DTE.OpenFile(t, 1, 1);
						});
					}
					return;
				case PathIsDirectory:
					AddCommand(MyToolBar, IconIds.OpenFolder, R.CMD_OpenFolder, ctx => {
						TryRun(t);
					});
					AddCommand(MyToolBar, IconIds.OpenWithCmd, R.CMD_OpenFolderWithCmd, ctx => {
						Process.Start(new ProcessStartInfo(Environment.SystemDirectory + "\\cmd.exe") { WorkingDirectory = t });
					});
					return;
			}

			string TryGetPathFromView(ITextView view, out int pathKind) {
				if (view.TryGetFirstSelectionSpan(out var span) && span.Length < 255) {
					var path = TryGetPath(view.TextSnapshot.GetText(span).TrimStart(), out pathKind);
					if (path != null) {
						return path;
					}

					// assume the selection is the file name, try get its full path
					var ts = view.TextSnapshot;
					var line = ts.GetLineFromPosition(span.Start);
					var eol = line.End.Position;
					var e = span.End < eol ? span.End : eol;
					// find start of path
					int ss = span.Start - line.Start > 255 ? span.Start - 255 : line.Start;
					var text = ts.GetText(ss, eol - ss);
					int s = span.Start - ss;
					s = text.LastIndexOf(Path.VolumeSeparatorChar, s, s);
					if (s < 1
						|| Char.IsLetterOrDigit(text[--s]) == false
						|| s > 1 && Char.IsLetterOrDigit(text[s - 1])) {
						return null;
					}
					s += ss;
					if (s > e) {
						e = eol;
					}
					// detect end of path
					char c;
					int i;
					for (i = e; i < eol; i++) {
						switch (c = ts[i]) {
							case '.':
							case '_':
							case '$':
								continue;
						}
						if (Char.IsLetterOrDigit(c)) {
							continue;
						}
						break;
					}
					e = i;
					return TryGetPath(ts.GetText(s, e - s), out pathKind);
				}
				pathKind = 0;
				return null;
			}

			string TryGetPath(string path, out int pathKind) {
				if (path.Length == 0) {
					pathKind = 0;
					return null;
				}
				path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				try {
					if (Path.IsPathRooted(path)) {
						path = path.Replace(@"\\", @"\");
						if (File.Exists(path)) {
							pathKind = PathIsFile;
							return path;
						}
						if (Directory.Exists(path)) {
							pathKind = PathIsDirectory;
							return path;
						}
					}
				}
				catch (ArgumentException) {
					// ignore
				}
				pathKind = 0;
				return null;
			}
			void TryRun(string path) {
				if (path == null) {
					return;
				}
				try {
					Process.Start(path);
				}
				catch (System.ComponentModel.Win32Exception ex) {
					CodistPackage.ShowErrorMessageBox(ex.Message, null, true);
				}
				catch (FileNotFoundException) {
					// ignore
				}
			}
			bool IsFileTypeRegisteredInVS(string fileName) {
				try {
					return ServicesHelper.Instance.FileExtensionRegistry.GetContentTypeForExtension(Path.GetExtension(fileName)).TypeName != "UNKNOWN";
				}
				catch (ArgumentException) {
					return false;
				}
			}
		}
	}
}
