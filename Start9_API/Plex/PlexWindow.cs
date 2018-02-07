﻿using Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
//using Start9.Api.Tools.WinApi;
using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;
using Start9.Api.Tools;
using System.Text;

namespace Start9.Api.Plex
{
	[TemplatePart(Name = PartTitlebar, Type = typeof(Grid))]
	[TemplatePart(Name = PartMinimizeButton, Type = typeof(Button))]
	[TemplatePart(Name = PartMaximizeButton, Type = typeof(Button))]
	[TemplatePart(Name = PartRestoreButton, Type = typeof(Button))]
	[TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
	[TemplatePart(Name = PartThumbBottom, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbTop, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbBottomRightCorner, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbTopRightCorner, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbTopLeftCorner, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbBottomLeftCorner, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbRight, Type = typeof(Thumb))]
	[TemplatePart(Name = PartThumbLeft, Type = typeof(Thumb))]
	public class PlexWindow : Window
	{
		const string PartTitlebar = "PART_Titlebar";
		const string PartMinimizeButton = "PART_MinimizeButton";
		const string PartMaximizeButton = "PART_MaximizeButton";
		const string PartRestoreButton = "PART_RestoreButton";
		const string PartCloseButton = "PART_CloseButton";
		const string PartThumbBottom = "PART_ThumbBottom";
		const string PartThumbTop = "PART_ThumbTop";
		const string PartThumbBottomRightCorner = "PART_ThumbBottomRightCorner";
        const string PartResizeGrip = "PART_ResizeGrip";
        const string PartThumbTopRightCorner = "PART_ThumbTopRightCorner";
		const string PartThumbTopLeftCorner = "PART_ThumbTopLeftCorner";
		const string PartThumbBottomLeftCorner = "PART_ThumbBottomLeftCorner";
		const string PartThumbRight = "PART_ThumbRight";
		const string PartThumbLeft = "PART_ThumbLeft";

        public double ShadowOpacity
        {
            get => (double)GetValue(ShadowOpacityProperty);
            set => SetValue(ShadowOpacityProperty, (value * Opacity));
        }

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.RegisterAttached("ShadowOpacity", typeof(double), typeof(PlexWindow), new PropertyMetadata((double)1));

        new public PlexResizeMode ResizeMode
        {
            get => (PlexResizeMode)GetValue(ResizeModeProperty);
            set => SetValue(ResizeModeProperty, value);
        }

        new public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(PlexResizeMode), typeof(PlexWindow), new PropertyMetadata(PlexResizeMode.CanResize, OnResizeModePropertyChangedCallback));

        static void OnResizeModePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int GWL_EXSTYLE = (-20);
            int GWL_STYLE = (-16);
            var window = (PlexWindow)d;
            var hwnd = new WindowInteropHelper(window).Handle;

            if (((window.ResizeMode == PlexResizeMode.Manual) & (!(window.ShowMaxRestButton))) | ((window.ResizeMode == PlexResizeMode.CanResize) | (window.ResizeMode == PlexResizeMode.CanResizeWithGrip)))
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, new IntPtr(0x16CF0000));
            }
            else if (((window.ResizeMode == PlexResizeMode.Manual) & (window.ShowMinButton)) | (window.ResizeMode == PlexResizeMode.CanMinimize))
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, new IntPtr(0x16CA0000));
            }
            else/* if (window.ResizeMode == PlexResizeMode.NoResize)*/
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, new IntPtr(0x16C80000));
            }
            
            /*else if (window.ResizeMode == PlexResizeMode.Manual)
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16C80000);
                if (ShowMaxRestButton)
                {
                    WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16CF0000);
                }
                else if (ShowMinButton)
                {
                    WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16CA0000);
                }
                else
                {
                    WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16C80000);
                }
            }*/

            /*else if (window.ResizeMode == PlexResizeMode.CanResizeWithGrip)
            {
                WinApi.SetWindowLong(hwnd, GWL_STYLE, 0x16CF0000);
            }*/

            //WinApi.SetWindowLong(hwnd, GWL_EXSTYLE, 0x00040100);
        }

        public static readonly DependencyProperty MaximizedProperty =
			DependencyProperty.Register("Maximized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty MinimizedProperty =
			DependencyProperty.Register("Minimized", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty WindowRectProperty = DependencyProperty.Register("WindowRect", typeof(Rect),
			typeof(PlexWindow), new PropertyMetadata(new Rect(), OnWindowRectPropertyChangedCallback));

        static void OnWindowRectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WinApi.MoveWindow(new WindowInteropHelper((PlexWindow)d).Handle, (int)((Rect)e.NewValue).Left, (int)((Rect)e.NewValue).Top, (int)((Rect)e.NewValue).Width, (int)((Rect)e.NewValue).Height, true);
            /*var window = (PlexWindow)d;
            var val = (Rect)e.NewValue;
            window.Left = val.Left;
            window.Top = val.Top;
            window.Width = val.Width;
            window.Height = val.Height;
            window._shadowWindow.Focus();
            window._shadowWindow.Activate();
            window.Focus();
            window.Activate();/
            window.ShiftShadowBehindWindow();*/
        }

        public static readonly DependencyProperty TitleBarContentProperty =
			DependencyProperty.RegisterAttached("TitleBarContent", typeof(object), typeof(PlexWindow),
				new PropertyMetadata(null));

		public static readonly DependencyProperty ToolBarContentProperty =
			DependencyProperty.RegisterAttached("ToolBarContent", typeof(object), typeof(PlexWindow),
				new PropertyMetadata(null));

		public static readonly DependencyProperty FooterContentProperty = DependencyProperty.RegisterAttached("FooterContent",
			typeof(object), typeof(PlexWindow),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register("TitleBarHeight",
			typeof(double), typeof(PlexWindow),
			new FrameworkPropertyMetadata((double) 24, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ToolBarHeightProperty = DependencyProperty.Register("ToolBarHeight",
			typeof(double), typeof(PlexWindow),
			new FrameworkPropertyMetadata((double) 42, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty FooterHeightProperty = DependencyProperty.Register("FooterHeight",
			typeof(double), typeof(PlexWindow),
			new FrameworkPropertyMetadata((double) 36, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTitleBarProperty =
			DependencyProperty.Register("ShowTitleBar", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

		public static readonly DependencyProperty ShowToolBarProperty =
			DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty ShowFooterProperty =
			DependencyProperty.Register("ShowFooter", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));

		public static readonly DependencyProperty BodyBrushProperty = DependencyProperty.Register("BodyBrush", typeof(Brush),
			typeof(PlexWindow), new PropertyMetadata(
				new LinearGradientBrush
				{
					StartPoint = new Point(0, 0),
					EndPoint = new Point(0, 1),
					GradientStops = new GradientStopCollection(new List<GradientStop>
					{
						new GradientStop
						{
							Offset = 0,
							Color = Colors.White
						},
						new GradientStop
						{
							Offset = 1,
							Color = Color.FromArgb(0xFF, 0xC8, 0xD4, 0xE7)
						}
					})
				}
			));

		public static readonly DependencyProperty FooterBrushProperty = DependencyProperty.Register("FooterBrush",
			typeof(Brush), typeof(PlexWindow),
			new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x5E, 0x98, 0xD9))));
        //#FF5E98D9

        /*public bool UseManualWindowManageControls
        {
            get => (bool)GetValue(UseManualWindowManageControlsProperty);
            set => SetValue(UseManualWindowManageControlsProperty, value);
        }

        public static readonly DependencyProperty UseManualWindowManageControlsProperty =
            DependencyProperty.Register("UseManualWindowManageControls", typeof(bool), typeof(PlexWindow), new PropertyMetadata(false));*/

        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register("ShowCloseButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowMaxRestButton
        {
            get => (bool)GetValue(ShowMaxRestButtonProperty);
            set => SetValue(ShowMaxRestButtonProperty, value);
        }

        public static readonly DependencyProperty ShowMaxRestButtonProperty =
            DependencyProperty.Register("ShowMaxRestButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowMinButton
        {
            get => (bool)GetValue(ShowMinButtonProperty);
            set => SetValue(ShowMinButtonProperty, value);
        }

        public static readonly DependencyProperty ShowMinButtonProperty =
            DependencyProperty.Register("ShowMinButton", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowResizeEdges
        {
            get => (bool)GetValue(ShowResizeEdgesProperty);
            set => SetValue(ShowResizeEdgesProperty, value);
        }

        public static readonly DependencyProperty ShowResizeEdgesProperty =
            DependencyProperty.Register("ShowResizeEdges", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        public bool ShowResizeGrip
        {
            get => (bool)GetValue(ShowResizeGripProperty);
            set => SetValue(ShowResizeGripProperty, value);
        }

        public static readonly DependencyProperty ShowResizeGripProperty =
            DependencyProperty.Register("ShowResizeGrip", typeof(bool), typeof(PlexWindow), new PropertyMetadata(true));

        TimeSpan animationDuration = TimeSpan.FromMilliseconds(500);

        /// <summary>
        ///     Interaction logic for PlexWindow.xaml
        /// </summary>
        public PlexWindow()
		{
			_shadowWindow = new ShadowWindow(this);
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
            ScaleTransform ScaleTransform = new ScaleTransform()
            {
                CenterX = (ActualWidth / 2),
                CenterY = (ActualHeight / 2),
                ScaleX = 1,
                ScaleY = 1
            };
            RenderTransform = ScaleTransform;
            Opacity = 0;
            /*_shadowScaleTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    {
                        SyncShadowToWindowScale();
                    }
                }));
            };*/
            Loaded += PlexWindow_Loaded;
            IsVisibleChanged += PlexWindow_IsVisibleChanged;
			Activated += PlexWindow_Activated;
			Deactivated += PlexWindow_Deactivated;
			var restoreMinSettings = new RoutedCommand();
			restoreMinSettings.InputGestures.Add(new KeyGesture(Key.Down, ModifierKeys.Windows));
			CommandBindings.Add(new CommandBinding(restoreMinSettings, RestoreMinimizeWindow));
            Closing += PlexWindow_Closing;
        }

        public bool Maximized
		{
			get => (bool) GetValue(MaximizedProperty);
			set => SetValue(MaximizedProperty, value);
		}

		public bool Minimized
		{
			get => (bool) GetValue(MinimizedProperty);
			set => SetValue(MinimizedProperty, value);
		}

		public Rect WindowRect
		{
			get => (Rect) GetValue(WindowRectProperty);
			set => SetValue(WindowRectProperty, value);
		}

		//private const string PART_Footer = "PART_Footer";

		/*public static void SetTitleBarContent(PlexWindow element, object value)
	    {

	        element.SetValue(TitleBarContentProperty, value);
	    }

	    public static object GetTitleBarContent(PlexWindow element)
	    {
	        return element.GetValue(TitleBarContentProperty);
	    }*/

		public object TitleBarContent
		{
			get => GetValue(TitleBarContentProperty);
			set => SetValue(TitleBarContentProperty, value);
		}

		public object ToolBarContent
		{
			get => GetValue(ToolBarContentProperty);
			set => SetValue(ToolBarContentProperty, value);
		}

		public object FooterContent
		{
			get => GetValue(FooterContentProperty);
			set
			{
				SetCurrentValue(FooterContentProperty, value);
				SetValue(FooterContentProperty, value);
			}
		}

		public double TitleBarHeight
		{
			get => (double) GetValue(TitleBarHeightProperty);
			set => SetValue(TitleBarHeightProperty, value);
		}

		public double ToolBarHeight
		{
			get => (double) GetValue(ToolBarHeightProperty);
			set => SetValue(ToolBarHeightProperty, value);
		}

		public double FooterHeight
		{
			get => (double) GetValue(FooterHeightProperty);
			set => SetValue(FooterHeightProperty, value);
		}
        public bool ShowTitle
        {
            get => (bool)GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, value);
        }

        public bool ShowTitleBar
		{
			get => (bool) GetValue(ShowTitleBarProperty);
			set => SetValue(ShowTitleBarProperty, value);
		}

		public bool ShowToolBar
		{
			get => (bool) GetValue(ShowToolBarProperty);
			set => SetValue(ShowToolBarProperty, value);
		}

		public bool ShowFooter
		{
			get => (bool) GetValue(ShowFooterProperty);
			set => SetValue(ShowFooterProperty, value);
		}

		public Brush BodyBrush
		{
			get => (Brush) GetValue(BodyBrushProperty);
			set => SetValue(BodyBrushProperty, value);
		}
		/*<Border.Background>
	    <LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
	    <LinearGradientBrush.GradientStops>
	    <GradientStop Color="White" Offset="0"/>
	    <GradientStop Color="#FFC8D4E7" Offset="1"/>
	    </LinearGradientBrush.GradientStops>
	    </LinearGradientBrush>
	    </Border.Background>*/

		public Brush FooterBrush
		{
			get => (Brush) GetValue(FooterBrushProperty);
			set => SetValue(FooterBrushProperty, value);
		}

		/*readonly Timer _shadowTimer = new Timer
		{
			Interval = 1
		};*/

        /*readonly Timer _shadowScaleTimer = new Timer
        {
            Interval = 1
        };*/
        /*double StoreMaxWidth = 0;
	    double StoreMaxHeight = 0;*/

        readonly ShadowWindow _shadowWindow;

		LinearGradientBrush _bodyLinearGradientBrush = new LinearGradientBrush
		{
			StartPoint = new Point(0, 0),
			EndPoint = new Point(0, 1),
			GradientStops = new GradientStopCollection(new List<GradientStop>
			{
				new GradientStop
				{
					Offset = 0,
					Color = Colors.White
				},
				new GradientStop
				{
					Offset = 1,
					Color = Color.FromArgb(0xFF, 0xC8, 0xD4, 0xE7)
				}
			})
		};

		Button _closeButton;
		Button _maxButton;
		Button _minButton;
		Button _restButton;
		/*double _restoreHeight;

		double _restoreLeft;
		double _restoreTop;
		double _restoreWidth;*/

		Thickness _shadowOffsetThickness = new Thickness(49, 14, 14, 60);
		Thumb _thumbBottom;
		Thumb _thumbBottomLeftCorner;
		Thumb _thumbBottomRightCorner;
        Thumb _resizeGrip;
		Thumb _thumbLeft;
		Thumb _thumbRight;
		Thumb _thumbTop;
		Thumb _thumbTopLeftCorner;
		Thumb _thumbTopRightCorner;

		Grid _titlebar;

		void RestoreMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
			}
		}

		void PlexWindow_Loaded(object sender, RoutedEventArgs e)
		{
			SyncShadowToWindow();
			SyncShadowToWindowSize();
            _shadowWindow.Focus();
            _shadowWindow.Activate();
            Focus();
            Activate();
            ShiftShadowBehindWindow();
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = (ActualHeight / 2);

            DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
            {
                From  = 0,
                To = 1,
                Duration = animationDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 0.75,
                To = 1,
                Duration = animationDuration,
                EasingFunction = circleEase
            };
            windowOpacityAnimation.Completed += (sendurr, args) =>
            {
                BeginAnimation(Window.OpacityProperty, null);
            };

            BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
        }

        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			{
				_titlebar = GetTemplateChild(PartTitlebar) as Grid;
				_titlebar.MouseLeftButtonDown += Titlebar_MouseLeftButtonDown;
				_titlebar.MouseLeftButtonUp += Titlebar_MouseLeftButtonUp;
				_titlebar.MouseMove += Titlebar_MouseMove;

				_minButton = GetTemplateChild(PartMinimizeButton) as Button;
                //_minButton.Click += delegate { ManageMaximizeRestore(2); };
                _minButton.Click += delegate { WindowState = WindowState.Minimized; };

                _maxButton = GetTemplateChild(PartMaximizeButton) as Button;
                _maxButton.Click += delegate {
                    WindowState = WindowState.Maximized;
                };

                _restButton = GetTemplateChild(PartRestoreButton) as Button;
                _restButton.Click += delegate {
                    WindowState = WindowState.Normal;
                };

                _closeButton = GetTemplateChild(PartCloseButton) as Button;
				_closeButton.Click += delegate
				{
                    Close();
                    //_shadowWindow.Close();
                    //Hide();
                    //_shadowWindow.Visibility = Visibility.Hidden;
                };


				_thumbBottom = GetTemplateChild(PartThumbBottom) as Thumb;
				_thumbBottom.DragDelta += ThumbBottom_DragDelta;


				_thumbTop = GetTemplateChild(PartThumbTop) as Thumb;
				_thumbTop.DragDelta += ThumbTop_DragDelta;


				_thumbBottomRightCorner = GetTemplateChild(PartThumbBottomRightCorner) as Thumb;
				_thumbBottomRightCorner.DragDelta += ThumbBottomRightCorner_DragDelta;


                _resizeGrip = GetTemplateChild(PartResizeGrip) as Thumb;
                _resizeGrip.DragDelta += ThumbBottomRightCorner_DragDelta;


                _thumbTopRightCorner = GetTemplateChild(PartThumbTopRightCorner) as Thumb;
				_thumbTopRightCorner.DragDelta += ThumbTopRightCorner_DragDelta;


				_thumbTopLeftCorner = GetTemplateChild(PartThumbTopLeftCorner) as Thumb;
				_thumbTopLeftCorner.DragDelta += ThumbTopLeftCorner_DragDelta;


				_thumbBottomLeftCorner = GetTemplateChild(PartThumbBottomLeftCorner) as Thumb;
				_thumbBottomLeftCorner.DragDelta += ThumbBottomLeftCorner_DragDelta;


				_thumbRight = GetTemplateChild(PartThumbRight) as Thumb;
				_thumbRight.DragDelta += ThumbRight_DragDelta;


				_thumbLeft = GetTemplateChild(PartThumbLeft) as Thumb;
				_thumbLeft.DragDelta += ThumbLeft_DragDelta;
			}
		}

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            SyncShadowToWindow();
            SyncShadowToWindowSize();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SyncShadowToWindow();
        }

        private void PlexWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsClosingNow)
            {
                _shadowWindow.Visibility = Visibility;
            }

            ShiftShadowBehindWindow();
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = (ActualHeight / 2);

            DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = animationDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 0.75,
                To = 1,
                Duration = animationDuration,
                EasingFunction = circleEase
            };
            windowSizeAnimation.Completed += (sendurr, args) =>
            {
                ShiftShadowBehindWindow();
            };

            BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
        }

        bool IsClosingNow = false;

        private void PlexWindow_Closing(object sender, CancelEventArgs e)
        {
            bool eCancel = e.Cancel;

            if (!eCancel & !IsClosingNow)
            {
                e.Cancel = true;
                IsClosingNow = true;
            }

            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = (ActualHeight / 2);

            DoubleAnimation windowOpacityAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = animationDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0.75,
                Duration = animationDuration,
                EasingFunction = circleEase
            };
            windowOpacityAnimation.Completed += (sendurr, args) =>
            {
                Close();
                _shadowWindow.Close();
            };

            if (!eCancel)
            {
                BeginAnimation(Window.OpacityProperty, windowOpacityAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            }
            /*else
            {
                e.Cancel = eCancel;
                IsClosingNow = false;
            }*/
        }

        /*public void ManageMaximizeRestore(int maxRestMin)
		{
			if (maxRestMin < 2)
			{
				var windowAnimation = new RectAnimation
				{
					EasingFunction = new ExponentialEase
					{
						EasingMode = EasingMode.EaseOut
					},
					Duration = TimeSpan.FromMilliseconds(250),
					From = new Rect(Left, Top, ActualWidth, ActualHeight)
				};

				switch (maxRestMin)
				{
					case 0:
						ManageWindowSize(true);
						var s = Screen.FromHandle(new WindowInteropHelper(this).Handle);
						windowAnimation.To = new Rect(s.WorkingArea.Left, s.WorkingArea.Top, s.WorkingArea.Width, s.WorkingArea.Height);
						Maximized = true;
						_shadowWindow.Hide();
						break;
					case 1:
						windowAnimation.To = new Rect(_restoreLeft, _restoreTop, _restoreWidth, _restoreHeight);

						Maximized = false;
						windowAnimation.Completed += delegate
						{
							_shadowWindow.Visibility = Visibility.Visible;
							Visibility = Visibility.Hidden;
							Visibility = Visibility.Visible;
						};
						break;
				}

				BeginAnimation(WindowRectProperty, windowAnimation);
			}
			else if (maxRestMin == 2)
			{
				ManageWindowSize(true);
				var windowTopAnimation = new DoubleAnimation
				{
					From = Top,
					To = SystemParameters.WorkArea.Bottom,
					Duration = TimeSpan.FromMilliseconds(250)
				};

				var windowLeftAnimation = new DoubleAnimation
				{
					To = _restoreLeft + _restoreWidth,
					Duration = TimeSpan.FromMilliseconds(500)
				};

				var windowScaleAnimation = new DoubleAnimation
				{
					To = 0,
					Duration = TimeSpan.FromMilliseconds(250)
				};


				_shadowWindow.Hide();
				windowLeftAnimation.Completed += delegate
				{
					BeginAnimation(LeftProperty, null);
					Left = _restoreLeft;
				};
				//BeginAnimation(PlexWindow.LeftProperty, WindowLeftAnimation);

				BeginAnimation(WidthProperty, windowScaleAnimation);
				BeginAnimation(HeightProperty, windowScaleAnimation);
				windowScaleAnimation.Completed += delegate
				{
					BeginAnimation(WidthProperty, null);
					BeginAnimation(HeightProperty, null);
					BeginAnimation(OpacityProperty, null);
					Opacity = 1;
					Width = _restoreWidth;
					Height = _restoreHeight;
				};
				BeginAnimation(OpacityProperty, windowScaleAnimation);

				windowTopAnimation.Completed += delegate
				{
					BeginAnimation(TopProperty, null);
					Top = _restoreTop;
					Minimized = true;
					WindowState = WindowState.Minimized;
				};
				BeginAnimation(TopProperty, windowTopAnimation);
			}
		}

		public void ManageWindowSize(bool store)
		{
			if (store) //Store real values to Restore variables
			{
				_restoreLeft = Left;
				_restoreTop = Top;
				_restoreWidth = Width;
				_restoreHeight = Height;
			}
			else //Retrieve real values from Restore variables
			{
				Left = _restoreLeft;
				Top = _restoreTop;
				Width = _restoreWidth;
				Height = _restoreHeight;
			}
		}*/

		protected override void OnStateChanged(EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            Screen s = Screen.FromHandle(hwnd);
            if (WindowState == WindowState.Maximized)
            {
                _shadowWindow.Visibility = Visibility.Hidden;
                Maximized = true;
                _maxButton.Visibility = Visibility.Hidden;
                _restButton.Visibility = Visibility.Visible;
                //double leftPadding = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.X);
                //double topPadding = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.Y);
                //Padding = new Thickness((leftPadding * -1), (topPadding * -1), leftPadding, topPadding);
                MaxWidth = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.Width);
                MaxHeight = Tools.DpiManager.ConvertPixelsToWpfUnits(s.WorkingArea.Height);
                CircleEase circleEase = new CircleEase()
                {
                    EasingMode = EasingMode.EaseOut
                };
                var scaleTransform = (this.RenderTransform as ScaleTransform);
                scaleTransform.CenterX = (ActualWidth / 2);
                scaleTransform.CenterY = (ActualHeight / 2);

                DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                {
                    From = 0.75,
                    To = 1,
                    Duration = animationDuration,
                    EasingFunction = circleEase
                };
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            }
            else
            {
                _maxButton.Visibility = Visibility.Visible;
                _restButton.Visibility = Visibility.Hidden;
                Padding = new Thickness(0, 0, 0, 0);
                MaxWidth = double.PositiveInfinity;
                MaxHeight = double.PositiveInfinity;

                if (WindowState == WindowState.Minimized)
                {
                    if (!Minimized)
                    {
                        if (Maximized)
                        {
                            WindowState = WindowState.Maximized;
                        }
                        else
                        {
                            WindowState = WindowState.Normal;
                        }
                        PlexWindow_AnimateMinimize();
                        //_shadowWindow.Visibility = Visibility.Hidden;
                        Minimized = true;
                    }
                }
                else
                {
                    //_shadowWindow.Visibility = Visibility.Visible;
                    //SyncShadowToWindow();
                    if (Minimized)
                    {
                        /*var scaleTransform = (this.RenderTransform as ScaleTransform);
                        scaleTransform.CenterX = (ActualWidth / 2);
                        scaleTransform.CenterY = ActualHeight;
                        CircleEase circleEase = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
                        };
                        DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };

                        DoubleAnimation windowTopAnimation = new DoubleAnimation()
                        {
                            To = Top,
                            From = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Bottom,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };
                        //scaleTransform.CenterX = (ActualWidth / 2);
                        //scaleTransform.CenterY = (ActualHeight / 2);*/
                        /*Rect windowRect = WindowRect;

                        DoubleAnimation windowTopAnimation = new DoubleAnimation()
                        {
                            To = WindowRect.Top,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };*/

                        /*DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                        {
                            From = 0,
                            To = 1,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };

                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);*/
                        /*windowTopAnimation.Completed += delegate
                        {
                            _shadowScaleTimer.Stop();
                        };
                        _shadowScaleTimer.Start();
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
                        BeginAnimation(PlexWindow.TopProperty, windowTopAnimation);
                        Minimized = false;*/
                        PlexWindow_AnimateRestoreUp();
                    }
                    else if (Maximized)
                    {
                        CircleEase circleEase = new CircleEase()
                        {
                            EasingMode = EasingMode.EaseOut
                        };
                        var scaleTransform = (this.RenderTransform as ScaleTransform);
                        scaleTransform.CenterX = (ActualWidth / 2);
                        scaleTransform.CenterY = (ActualHeight / 2);

                        DoubleAnimation windowSizeAnimation = new DoubleAnimation()
                        {
                            From = 1.333,
                            To = 1,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };
                        DoubleAnimation windowVertSizeAnimation = new DoubleAnimation()
                        {
                            From = 1.333,
                            To = 1,
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };
                        Rect windowRect = WindowRect;

                        /*RectAnimation windowRectAnimation = new RectAnimation()
                        {
                            To = new Rect(Left, Top, Width, Height),
                            From = new Rect(Left - 100, Top - 100, Width + 200, Height + 200),
                            Duration = animationDuration,
                            EasingFunction = circleEase
                        };*/
                        windowVertSizeAnimation.Completed += delegate
                        {
                            WindowRect = new Rect(Left + 100, Top + 100, Width - 200, Height - 200);
                            _shadowWindow.Show();
                            ShiftShadowBehindWindow();
                            //_shadowWindow.Focus();
                            //_shadowWindow.Activate();
                            //Focus();
                            //Activate();
                            //PlexWindow_Activated(null, null);
                            //ShiftShadowBehindWindow();
                            /*Left = Left + 100;
                            Top = Top + 100;
                            Width = Width - 200;
                            Height = Height - 200;*/
                        };


                        WindowRect = new Rect(Left - 100, Top - 100, Width + 200, Height + 200);
                        /*Left = Left - 100;
                        Top = Top - 100;
                        Width = Width + 200;
                        Height = Height + 200;*/
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowVertSizeAnimation);
                        //BeginAnimation(PlexWindow.WindowRectProperty, windowRectAnimation);

                        /*int scaleInterval = 0;

                        System.Windows.Forms.Timer scaleTimer = new System.Windows.Forms.Timer()
                        {
                            Interval = 1
                        };

                        scaleTimer.Tick += delegate
                        {
                            if (scaleInterval < 375)
                            {
                                scaleInterval = scaleInterval + 1;
                            }
                            else
                            {
                                scaleTimer.Stop();
                                scaleInterval = 0;
                                Left = Left + 100;
                                Top = Top + 100;
                                Width = Width - 200;
                                Height = Height - 200;
                            }
                        };*/
                        //scaleTimer.Start();
                        //375
                    }
                }
                Maximized = false;
            }

            if (_shadowWindow.Visibility != Visibility.Hidden)
            {
                ShiftShadowBehindWindow();
            }
        }

        Rect RestoreTo = new Rect(0, 0, 0, 0);

        public void PlexWindow_AnimateRestoreUp()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = ActualHeight;
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut
            };
            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = animationDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowTopAnimation = new DoubleAnimation()
            {
                To = RestoreTo.Y,
                From = Top,
                Duration = animationDuration,
                EasingFunction = circleEase
            };
            windowTopAnimation.Completed += delegate
            {
                //_shadowScaleTimer.Stop();
                Minimized = false;
                PlexWindow_ResetTransformProperties();
            };
            //_shadowScaleTimer.Start();
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            BeginAnimation(PlexWindow.TopProperty, windowTopAnimation);
        }

        public void PlexWindow_AnimateMinimize()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            RestoreTo = new Rect(Left, Top, ActualWidth, ActualHeight);
            var windowRect = WindowRect;
            scaleTransform.CenterX = (ActualWidth / 2);
            scaleTransform.CenterY = ActualHeight;
            CircleEase circleEase = new CircleEase()
            {
                EasingMode = EasingMode.EaseIn
            };

            DoubleAnimation windowSizeAnimation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = animationDuration,
                EasingFunction = circleEase
            };

            DoubleAnimation windowTopAnimation = new DoubleAnimation()
            {
                To = System.Windows.Forms.Screen.FromRectangle(System.Drawing.Rectangle.FromLTRB((int)WindowRect.X, (int)WindowRect.Y, (int)WindowRect.Right, (int)WindowRect.Bottom)).WorkingArea.Bottom,
                From = Top,
                Duration = animationDuration,
                EasingFunction = circleEase
            };
            windowTopAnimation.Completed += delegate
            {
                //_shadowScaleTimer.Stop();
                Minimized = true;
                PlexWindow_ResetTransformProperties();
                WindowRect = windowRect;
                WindowState = WindowState.Minimized;
            };
            //_shadowScaleTimer.Start();
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, windowSizeAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, windowSizeAnimation);
            BeginAnimation(PlexWindow.TopProperty, windowTopAnimation);
        }

        void PlexWindow_ResetTransformProperties()
        {
            var scaleTransform = (this.RenderTransform as ScaleTransform);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            BeginAnimation(PlexWindow.LeftProperty, null);
            BeginAnimation(PlexWindow.TopProperty, null);
            BeginAnimation(PlexWindow.WidthProperty, null);
            BeginAnimation(PlexWindow.HeightProperty, null);
            BeginAnimation(PlexWindow.OpacityProperty, null);
            BeginAnimation(PlexWindow.WindowRectProperty, null);
            Opacity = 1;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            PlexWindow_Activated(this, null);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            PlexWindow_Activated(this, null);
        }

        public void PlexWindow_Activated(object sender, EventArgs e)
        {
            PlexWindow_ResetTransformProperties();

            if (WindowState == WindowState.Normal)
            {
                //_shadowWindow.Visibility = Visibility.Visible;
                //SyncShadowToWindow();
                //_shadowWindow.Opacity = 1;
                //_shadowWindow.Activate();
                /*_shadowWindow.Topmost = true;
                _shadowWindow.Topmost = false;
                Topmost = true;
                Topmost = false;*/
                //Activate();
            }
            /*if (WindowState == WindowState.Normal)
                    {
                        _shadowWindow.Topmost = true;
                        _shadowWindow.Topmost = false;
                    }*/
            //Focus();
            //Activate();
            //Topmost = true;
            //Topmost = false;

            if (!Topmost)
            {
                ShiftShadowBehindWindow();
                //WinApi.SetForegroundWindow(new WindowInteropHelper(this).Handle);
                //_shadowWindow.Topmost = true;
                //_shadowWindow.Topmost = false;
                //_shadowWindow.Activate();
                //Topmost = true;
                //Topmost = false;
                /*else
                {
                    WinApi.SetForegroundWindow(new WindowInteropHelper(_shadowWindow).Handle); 
                }*/
            }
		}

        public void ShiftShadowBehindWindow()
        {
            var thisHandle = new WindowInteropHelper(this).Handle;
            WinApi.SetWindowPos(thisHandle, new IntPtr(0), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010);
            WinApi.SetWindowPos(new WindowInteropHelper(_shadowWindow).Handle, thisHandle, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            _shadowWindow.Topmost = Topmost;
            if ((_shadowWindow.Topmost) & (Mouse.LeftButton != MouseButtonState.Pressed))
            {
                ShiftShadowBehindWindow();
            }
        }


        void PlexWindow_Deactivated(object sender, EventArgs e)
		{
            ShiftShadowBehindWindow();
            //_shadowWindow.Opacity = 0.5;
		}

		public void SyncShadowToWindow()
		{
			_shadowWindow.Left = Left - _shadowOffsetThickness.Left;
			_shadowWindow.Top = Top - _shadowOffsetThickness.Top;
		}

		public void SyncShadowToWindowSize()
		{
			_shadowWindow.Width = Width + _shadowOffsetThickness.Left + _shadowOffsetThickness.Right;
			_shadowWindow.Height = Height + _shadowOffsetThickness.Top + _shadowOffsetThickness.Bottom;
		}

        public void SyncShadowToWindowScale()
        {
            var scaleTransform = RenderTransform as ScaleTransform;
            _shadowWindow.RenderTransform = new ScaleTransform()
            {
                ScaleX = 1,
                ScaleY = 1,
                CenterX = scaleTransform.CenterX,
                CenterY = scaleTransform.CenterY
            };
            (_shadowWindow.RenderTransform as ScaleTransform).ScaleX = scaleTransform.ScaleX;
            (_shadowWindow.RenderTransform as ScaleTransform).ScaleY = scaleTransform.ScaleY;
        }

        void PlexWindow_StateChanged(object sender, EventArgs e)
		{
            if ((WindowState == WindowState.Maximized) | (WindowState == WindowState.Minimized))
                _shadowWindow.Visibility = Visibility.Hidden;
            else
            {
                _shadowWindow.Visibility = Visibility.Visible;
                ShiftShadowBehindWindow();
            }
		}

		/*void PlexWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((Visibility == Visibility.Visible) & (WindowState == WindowState.Normal))
				_shadowWindow.Visibility = Visibility.Visible;
			else
				_shadowWindow.Visibility = Visibility.Hidden;
		}*/

		void Titlebar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
			//SyncShadowToWindow();
			//_shadowTimer.Start();
		}

		void Titlebar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			//_shadowTimer.Stop();
		}

		void Titlebar_MouseMove(object sender, MouseEventArgs e)
		{
			/*if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
			{
			    SyncShadowToWindow();
			}*/
		}

		void ThumbBottomRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Width + e.HorizontalChange > 10)
				Width += e.HorizontalChange;
			if (Height + e.VerticalChange > 10)
				Height += e.VerticalChange;
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbTopRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Width + e.HorizontalChange > 10)
				Width += e.HorizontalChange;
			if (Top + e.VerticalChange > 10)
			{
				Top += e.VerticalChange;
				Height -= e.VerticalChange;
			}
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbTopLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Left + e.HorizontalChange > 10)
			{
				Left += e.HorizontalChange;
				Width -= e.HorizontalChange;
			}
			if (Top + e.VerticalChange > 10)
			{
				Top += e.VerticalChange;
				Height -= e.VerticalChange;
			}
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbBottomLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Left + e.HorizontalChange > 10)
			{
				Left += e.HorizontalChange;
				Width -= e.HorizontalChange;
			}
			if (Height + e.VerticalChange > 10)
				Height += e.VerticalChange;
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbRight_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Width + e.HorizontalChange > 10)
				Width += e.HorizontalChange;
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbLeft_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Left + e.HorizontalChange > 10)
			{
				Left += e.HorizontalChange;
				Width -= e.HorizontalChange;
			}
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbBottom_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Height + e.VerticalChange > 10)
				Height += e.VerticalChange;
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}

		void ThumbTop_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (Top + e.VerticalChange > 10)
			{
				Top += e.VerticalChange;
				Height -= e.VerticalChange;
			}
			SyncShadowToWindow();
			SyncShadowToWindowSize();
		}
	}

    /*[TemplatePart(Name = PartIcon, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_RibbonTitleBar, Type = typeof(RibbonTitleBar))]
    [TemplatePart(Name = PART_WindowCommands, Type = typeof(WindowCommands))]
    public class PlexRibbonWindow : PlexWindowWindow
    {
        const string PartIcon = "PART_Titlebar";
    }*/

    public enum PlexResizeMode
    {
        CanResize,
        CanResizeWithGrip,
        CanMinimize,
        NoResize,
        Manual
    };
}