using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace TimerTool
{
    public class CustomWindow : Window
    {
        public DockPanel DpTitleBar { get; private set; }
        public CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow), new FrameworkPropertyMetadata(typeof(CustomWindow)));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow));

            //删除旧有窗口样式，重新定义
            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(8),
                GlassFrameThickness = new Thickness(-1),
                UseAeroCaptionButtons = false,
                CornerRadius = new CornerRadius(10),

            };

            WindowChrome.SetWindowChrome(this, chrome);

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.Loaded += CustomWindow_Loaded; 
            this.MouseMove += CustomWindow_MouseMove;
        }

        DockPanel _partDockPanel;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _partDockPanel = GetTemplateChild("toptitle") as DockPanel;
        }

        private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Owner = Application.Current.MainWindow;
        }

        private void CustomWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.GetPosition(this).Y <= _partDockPanel.Height * SystemParameters.PrimaryScreenHeight/ 2160)
                {
                    //不这样写会把mouseup时间吞掉
                    this.MouseMove -= CustomWindow_MouseMove;
                    DragMove();
                    this.MouseMove += CustomWindow_MouseMove;
                }


            }
        }


        #region =====依赖属性=====

        /// <summary>
        /// 是否显示放大窗口的按钮
        /// </summary>
        public bool IsShowMaxButton
        {
            get { return (bool)GetValue(IsShowMaxButtonProperty); }
            set { SetValue(IsShowMaxButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowMaxButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowMaxButtonProperty =
            DependencyProperty.Register("IsShowMaxButton", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

        /// <summary>
        /// 是否显示缩小窗口的按钮
        /// </summary>
        public bool IsShowMinButton
        {
            get { return (bool)GetValue(IsShowMinButtonProperty); }
            set { SetValue(IsShowMinButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowMinButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowMinButtonProperty =
            DependencyProperty.Register("IsShowMinButton", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

        public double mActualWidth = 3840;
        public double mActualHeight = 2160;

        /// <summary>
        /// 窗体放大显示状态
        /// </summary>
        public WinMaxStatus WinMaxStatus
        {
            get { return (WinMaxStatus)GetValue(WinMaxStatusProperty); }
            set { SetValue(WinMaxStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WinMaxStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WinMaxStatusProperty =
            DependencyProperty.Register("WinMaxStatus", typeof(WinMaxStatus), typeof(CustomWindow), new PropertyMetadata(WinMaxStatus.MaxHide));

        /// <summary>
        /// 窗体缩小显示状态
        /// </summary>
        public WinMinStauts WinMinStauts
        {
            get { return (WinMinStauts)GetValue(WinMinStautsProperty); }
            set { SetValue(WinMinStautsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WinMinStauts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WinMinStautsProperty =
            DependencyProperty.Register("WinMinStauts", typeof(WinMinStauts), typeof(CustomWindow), new PropertyMetadata(WinMinStauts.MinHide));

        public bool IsShowTitleLine
        {
            get { return (bool)GetValue(IsShowTitleLineProperty); }
            set { SetValue(IsShowTitleLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowTitleLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowTitleLineProperty =
            DependencyProperty.Register("IsShowTitleLine", typeof(bool), typeof(CustomWindow), new PropertyMetadata(true));

        public static double oldTopValue = 0;
        public static double oldLeftValue = 0;

        public WindowPosition WindowPosition
        {
            get { return (WindowPosition)GetValue(WindowPositionProperty); }
            set { SetValue(WindowPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowPositionProperty =
            DependencyProperty.Register("WindowPosition", typeof(WindowPosition), typeof(CustomWindow), new PropertyMetadata(WindowPosition.CenterScreen, PositionCallBack));

        private static void PositionCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as CustomWindow;
            if ((WindowPosition)e.NewValue != WindowPosition.Resrory)
            {
                oldTopValue = window.Top;
                oldLeftValue = window.Left;
            }
            switch (e.NewValue)
            {
                case WindowPosition.CenterScreen:
                    window.Top = (SystemParameters.PrimaryScreenHeight / 2) - (window.Height / 2);
                    window.Left = (SystemParameters.PrimaryScreenWidth / 2) - (window.Width / 2);
                    break;

                case WindowPosition.TopRight:

                    window.Top = 50;
                    window.Left = SystemParameters.PrimaryScreenWidth - window.ActualWidth - 50;
                    break;

                case WindowPosition.Resrory:
                    window.Top = oldTopValue == 0 ? (SystemParameters.PrimaryScreenHeight / 2) - (window.Height / 2) : oldTopValue;
                    window.Left = oldLeftValue == 0 ? (SystemParameters.PrimaryScreenWidth / 2) - (window.Width / 2) : oldLeftValue;
                    break;
            }
        }

        /// <summary>
        /// 窗口高度
        /// </summary>
        public double dHeight
        {
            get { return (double)GetValue(dHeightProperty); }
            set { SetValue(dHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for dHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dHeightProperty =
            DependencyProperty.Register("dHeight", typeof(double), typeof(CustomWindow), new PropertyMetadata(ChangeHeightCallBack));

        private static void ChangeHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CustomWindow).Height = (double)e.NewValue * SystemParameters.PrimaryScreenHeight / (d as CustomWindow).mActualHeight;
        }

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public double dWidth
        {
            get { return (double)GetValue(dWidthProperty); }
            set { SetValue(dWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for dWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty dWidthProperty =
            DependencyProperty.Register("dWidth", typeof(double), typeof(CustomWindow), new PropertyMetadata(ChangeWidthCallBack));

        private static void ChangeWidthCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CustomWindow).Width = (double)e.NewValue * SystemParameters.PrimaryScreenWidth / (d as CustomWindow).mActualWidth;
        }

        /// <summary>
        /// 窗口放大的时候，内容是否跟着变大 ，如果不随变大，放大后将会以4K大小显示
        /// </summary>
        public bool isFollowBigger
        {
            get { return (bool)GetValue(isFollowBiggerProperty); }
            set { SetValue(isFollowBiggerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for isFollowBigger.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty isFollowBiggerProperty =
            DependencyProperty.Register("isFollowBigger", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

        /// <summary>
        /// 窗口是否变小状态
        /// </summary>
        public bool IsMinStauts
        {
            get { return (bool)GetValue(IsMinStautsProperty); }
            set { SetValue(IsMinStautsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMinStauts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinStautsProperty =
            DependencyProperty.Register("IsMinStauts", typeof(bool), typeof(CustomWindow), new PropertyMetadata(false));

        /// <summary>
        /// 是否显示标题栏
        /// </summary>
        public bool IsShowTitleBar
        {
            get { return (bool)GetValue(IsShowTitleBarProperty); }
            set { SetValue(IsShowTitleBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowTitleBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowTitleBarProperty =
            DependencyProperty.Register("IsShowTitleBar", typeof(bool), typeof(CustomWindow), new PropertyMetadata(true));

        private static void OnIsShowTitleBar(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tmpWin = d as CustomWindow;
            if (tmpWin == null)
                return;
            if (tmpWin.DpTitleBar == null)
                return;
            bool tmpIsShow = true;
            bool.TryParse(e.NewValue.ToString(), out tmpIsShow);
            tmpWin.DpTitleBar.Visibility = tmpIsShow ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 可移动-启用
        /// </summary>
        public bool RemovableEnabled
        {
            get { return (bool)GetValue(RemovableEnabledProperty); }
            set { SetValue(RemovableEnabledProperty, value); }
        }

        public static readonly DependencyProperty RemovableEnabledProperty =
            DependencyProperty.Register("RemovableEnabled", typeof(bool), typeof(CustomWindow), new PropertyMetadata(true));

        /// <summary>
        /// 窗口圆角值
        /// </summary>
        public CornerRadius WinCornerRadius
        {
            get { return (CornerRadius)GetValue(WinCornerRadiusProperty); }
            set { SetValue(WinCornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty WinCornerRadiusProperty =
            DependencyProperty.Register("WinCornerRadius", typeof(CornerRadius), typeof(CustomWindow), new PropertyMetadata(new CornerRadius(10)));


        #endregion =====依赖属性===== 
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.OriginalSource is Image img && img.Name == "DevImage")
            {
                e.Handled = true;
            }
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed && RemovableEnabled)
            {
                //Console.WriteLine("dragmove");
            }
        }

        private bool IsSizeToContentWidthAndHeight = false;

        /// <summary>
        /// 内容的Size和Window的Size计算错误 目前解决办法 可能有问题
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
            {
                InvalidateMeasure();
                IsSizeToContentWidthAndHeight = true;
            }
        }

        /// <summary>
        /// 窗体恢复原来样子
        /// </summary>
        /// <param name="e"></param>
        public virtual void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            //chrome.GlassFrameThickness = new Thickness(0);//不设置0的话，不能设置窗口圆角
            SystemCommands.RestoreWindow(this);
            if (IsSizeToContentWidthAndHeight)
            {
                this.SizeToContent = SizeToContent.WidthAndHeight;
            }
            IsMinStauts = false;
        }

        /// <summary>
        /// 放大窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            // chrome.GlassFrameThickness = new Thickness(1);//这个设置不为0 才可以遮住任务栏放大
            SystemCommands.MaximizeWindow(this);
            IsMinStauts = false;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            IsMinStauts = false;
        }

        

        /// <summary>
        /// 窗体缩小更具  内容高度和宽度去缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            IsMinStauts = true;
        }
    }

    public enum WinMaxStatus
    {
        /// <summary>
        /// 放大显示标题框
        /// </summary>
        MaxShow,

        /// <summary>
        /// 放大隐藏标题框
        /// </summary>
        MaxHide,
    }

    public enum WinMinStauts
    {
        /// <summary>
        /// 缩小显示标题框
        /// </summary>
        MinShow,

        /// <summary>
        /// 缩小隐藏标题框
        /// </summary>
        MinHide,
    }

    public enum WindowPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        CenterScreen,
        Resrory

    }
}
