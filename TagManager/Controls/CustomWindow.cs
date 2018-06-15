using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace TagManager.Controls
{
	public class CustomWindow : Window
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		protected IntPtr _windowHandle;
		protected internal const string TOP_LEFT = "TOP_LEFT";
		protected internal const string TOP = "TOP";
		protected internal const string TOP_RIGHT = "TOP_RIGHT";
		protected internal const string LEFT = "LEFT";
		protected internal const string RIGHT = "RIGHT";
		protected internal const string BOTTOM_LEFT = "BOTTOM_LEFT";
		protected internal const string BOTTOM = "BOTTOM";
		protected internal const string BOTTOM_RIGHT = "BOTTOM_RIGHT";

		private bool _restoreIfMove = false;

		private readonly Dictionary<BorderPosition, Cursor> _cursors = new Dictionary<BorderPosition, Cursor>
		{
			{ BorderPosition.Left, Cursors.SizeWE },
			{ BorderPosition.Right, Cursors.SizeWE },
			{ BorderPosition.Top, Cursors.SizeNS },
			{ BorderPosition.Bottom, Cursors.SizeNS },
			{ BorderPosition.BottomLeft, Cursors.SizeNESW },
			{ BorderPosition.TopRight, Cursors.SizeNESW },
			{ BorderPosition.BottomRight, Cursors.SizeNWSE },
			{ BorderPosition.TopLeft, Cursors.SizeNWSE }
		};
		protected internal string PART_MAXIMIZE_RESTORE_BUTTON = "Max";
		protected internal string PART_CLOSE_BUTTON = "Close";
		protected internal string PART_MINIMIZE_BUTTON = "Min";

		public CustomWindow()
		{

		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_windowHandle = new WindowInteropHelper(this).Handle;

			ApplyResizeEvents(GetTemplateChild(TOP_LEFT) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(TOP) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(TOP_RIGHT) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(LEFT) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(RIGHT) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(BOTTOM_LEFT) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(BOTTOM) as Rectangle);
			ApplyResizeEvents(GetTemplateChild(BOTTOM_RIGHT) as Rectangle);

			var closeButton = GetTemplateChild(PART_CLOSE_BUTTON) as Button;
			if (closeButton != null)
			{
				closeButton.Click += PART_CLOSE_Click;
			}

			var maximizeButton = GetTemplateChild(PART_MAXIMIZE_RESTORE_BUTTON) as Button;
			if (maximizeButton != null)
			{
					maximizeButton.Click += PART_MAXIMIZE_RESTORE_Click;
			}

			var minimizeButton = GetTemplateChild(PART_MINIMIZE_BUTTON) as Button;
			if (minimizeButton != null)
			{
					minimizeButton.Click += PART_MINIMIZE_Click;
			}


			var border = GetTemplateChild("TOPGrid") as Grid;

			if (border != null)
			{
				border.MouseLeftButtonDown += PART_TITLEBAR_MouseLeftButtonDown;
				border.MouseMove += PART_TITLEBAR_OnMouseMove;
				border.MouseLeftButtonUp += PART_TITLEBAR_OnMouseLeftButtonUp;
			}
		}

		private void PART_TITLEBAR_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_restoreIfMove = false;
		}

		private void PART_TITLEBAR_OnMouseMove(object sender, MouseEventArgs e)
		{
			 //if (_restoreIfMove)
    //        {
    //            var screen = System.Windows.Forms.
				//_restoreIfMove = false;
    //            var mouseX = System.Windows.Forms.Control.MousePosition.X;
    //            var width = RestoreBounds.Width;
    //            var x = mouseX - width / 2;

    //            if (x + width > screen.WorkingArea.Width)
    //            {
    //                x = screen.WorkingArea.Width - width;
    //            }

    //            WindowState = WindowState.Normal;
    //            Left = x;
    //            Top = 0;

    //            if (Mouse.LeftButton == MouseButtonState.Pressed)
    //                DragMove();
    //        }
		}

		private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
				{
						SwitchState();
				}
			}
			else
			{
				if (WindowState == WindowState.Maximized)
				{
					_restoreIfMove = true;
				}

				if (Mouse.LeftButton == MouseButtonState.Pressed)
					DragMove();

				e.Handled = true;
			}
		}

		private void Topgrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
			}
			else
			{
				WindowState = WindowState.Normal;
			}
		}

		private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ApplyResizeEvents(Rectangle rect)
		{
			if (rect != null)
			{
				rect.PreviewMouseLeftButtonDown += Resize;
				rect.MouseMove += DisplayResizeCursor;
				rect.MouseLeave += ResetCursor;
			}
		}
		private void Resize(object sender, MouseButtonEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element != null && WindowState == WindowState.Normal && e.ButtonState == MouseButtonState.Pressed)
			{
				Cursor = _cursors[(BorderPosition)element.Tag];
				ResizeWindow((BorderPosition)element.Tag);
			}
		}

		private void DisplayResizeCursor(object sender, MouseEventArgs e)
		{
			var element = sender as FrameworkElement;
			if (element != null && WindowState == WindowState.Normal)
			{
				Cursor = _cursors[(BorderPosition)element.Tag];
			}
		}

		private void ResetCursor(object sender, MouseEventArgs e)
		{
			if (Mouse.LeftButton != MouseButtonState.Pressed)
			{
				Cursor = Cursors.Arrow;
			}
		}

		private void ResizeWindow(BorderPosition direction)
		{
			SendMessage(_windowHandle, 0x112, (IntPtr)direction, IntPtr.Zero);
		}
		private void SwitchState()
		{
			switch (WindowState)
			{
				case WindowState.Normal:
					{
						WindowState = WindowState.Maximized;
						break;
					}
				case WindowState.Maximized:
					{
						WindowState = WindowState.Normal;
						break;
					}
			}
		}
	}
	public enum BorderPosition
	{
		Left = 61441,
		Right = 61442,
		Top = 61443,
		TopLeft = 61444,
		TopRight = 61445,
		Bottom = 61446,
		BottomLeft = 61447,
		BottomRight = 61448
	}
}
