﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using AppHelpers;

namespace Codist.Controls
{
	// HACK: put the symbol list, smart bar, etc. on top of the WpfTextView
	// don't use AdornmentLayer to do so, otherwise contained objects will go up and down when scrolling code window
	sealed class ExternalAdornment : Canvas
	{
		readonly Microsoft.VisualStudio.Text.Editor.IWpfTextView _View;
		int _LayerZIndex;
		bool _isDragging;
		Point _beginDragPosition;

		public ExternalAdornment(Microsoft.VisualStudio.Text.Editor.IWpfTextView view) {
			UseLayoutRounding = true;
			SnapsToDevicePixels = true;
			Grid.SetColumn(this, 1);
			Grid.SetRow(this, 1);
			Grid.SetIsSharedSizeScope(this, true);
			var grid = view.VisualElement.GetParent<Grid>();
			if (grid != null) {
				grid.Children.Add(this);
			}
			else {
				view.VisualElement.Loaded += VisualElement_Loaded;
			}
			_View = view;
		}

		public void FocusOnTextView() {
			_View.VisualElement.Focus();
		}

		public void ClearUnpinnedChildren() {
			for (int i = Children.Count - 1; i >= 0; i--) {
				var c = Children[i] as SymbolList;
				if (c != null && c.IsPinned) {
					continue;
				}
				Children.RemoveAt(i);
			}
		}

		protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs args) {
			base.OnPreviewMouseRightButtonUp(args);
			// hack: suppress the undesired built-in context menu of tabs in VS 16.5
			if (args.Source is SymbolList symbolList) {
				symbolList.ShowContextMenu(args);
			}
			args.Handled = true;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			var element = visualAdded as UIElement;
			if (element != null) {
				element.MouseLeave -= ReleaseQuickInfo;
				element.MouseLeave += ReleaseQuickInfo;
				element.MouseEnter -= SuppressQuickInfo;
				element.MouseEnter += SuppressQuickInfo;
				element.MouseLeftButtonDown -= BringToFront;
				element.MouseLeftButtonDown += BringToFront;
				SetZIndex(element, ++_LayerZIndex);
			}
			element = visualRemoved as UIElement;
			if (element != null) {
				element.MouseLeave -= ReleaseQuickInfo;
				element.MouseEnter -= SuppressQuickInfo;
				element.MouseLeftButtonDown -= BringToFront;
				_View.Properties.RemoveProperty(nameof(ExternalAdornment));
			}
			if (Children.Count == 0 || Children.Count == 1 && Children[0] is null) {
				FocusOnTextView();
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			foreach (var item in Children) {
				var child = item as FrameworkElement;
				if (child != null) {
					ConstrainChildWindow(child, new Point(GetLeft(child), GetTop(child)));
				}
			}
		}

		void ConstrainChildWindow(FrameworkElement child, Point newPos) {
			const int ConstraintSize = 30;
			// constrain window within editor view
			if (newPos.X + child.ActualWidth < ConstraintSize) {
				newPos.X = ConstraintSize - child.ActualWidth;
			}
			else if (newPos.X > ActualWidth - ConstraintSize) {
				newPos.X = ActualWidth - ConstraintSize;
			}
			if (newPos.Y + child.ActualHeight < ConstraintSize) {
				newPos.Y = ConstraintSize - child.ActualHeight;
			}
			else if (newPos.Y > ActualHeight - ConstraintSize) {
				newPos.Y = ActualHeight - ConstraintSize;
			}
			SetLeft(child, newPos.X);
			SetTop(child, newPos.Y);
		}

		#region Draggable
		public void MakeDraggable(FrameworkElement draggablePart) {
			draggablePart.MouseLeftButtonDown += MenuHeader_MouseDown;
		}

		void MenuHeader_MouseDown(object sender, MouseButtonEventArgs e) {
			var s = e.Source as UIElement;
			s.MouseLeftButtonDown -= MenuHeader_MouseDown;
			s.MouseMove += MenuHeader_DragMove;
		}

		void MenuHeader_DragMove(object sender, MouseEventArgs e) {
			var s = e.Source as FrameworkElement;
			if (e.LeftButton == MouseButtonState.Pressed) {
				if (_isDragging == false && s.CaptureMouse()) {
					_isDragging = true;
					s.Cursor = Cursors.SizeAll;
					_beginDragPosition = e.GetPosition(s);
				}
				else if (_isDragging) {
					var cp = e.GetPosition(this);
					cp.X -= _beginDragPosition.X;
					cp.Y -= _beginDragPosition.Y;
					ConstrainChildWindow(s, cp);
				}
			}
			else {
				if (_isDragging) {
					_isDragging = false;
					s.Cursor = null;
					s.MouseMove -= MenuHeader_DragMove;
					s.ReleaseMouseCapture();
					s.MouseLeftButtonDown += MenuHeader_MouseDown;
				}
			}
		}
		#endregion

		void VisualElement_Loaded(object sender, RoutedEventArgs e) {
			_View.VisualElement.Loaded -= VisualElement_Loaded;
			_View.VisualElement.GetParent<Grid>().Children.Add(this);
		}

		void BringToFront(object sender, MouseButtonEventArgs e) {
			SetZIndex(e.Source as UIElement, ++_LayerZIndex);
		}

		void ReleaseQuickInfo(object sender, MouseEventArgs e) {
			_View.Properties.RemoveProperty(nameof(ExternalAdornment));
		}

		void SuppressQuickInfo(object sender, MouseEventArgs e) {
			_View.Properties[nameof(ExternalAdornment)] = true;
		}
	}
}
