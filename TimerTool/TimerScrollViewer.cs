using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;

namespace TimerTool
{
    public class TimerScrollViewer : HandyControl.Controls.ScrollViewer
    {
        private bool _isRunning;

        private DispatcherTimer Timer = new DispatcherTimer();

        private DispatcherTimer MouseWheelTimer = new DispatcherTimer();

        private double ItemHeight = 0;
        private bool IsDown = false;
        private bool isRun = false;

        public TimerScrollViewer()
        {
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            MouseWheelTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Tick += LeftTimer_Tick;
            MouseWheelTimer.Tick += MouseWheelTimer_Tick;
            this.PreviewMouseWheel += TimerScrollViewer_PreviewMouseWheel;
            this.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;


        }

        public void ScrollToVerticalOffsetWithAnimation(double fromoffset, double offset, double milliseconds = 500)
        {
            var animation = AnimationHelper.CreateAnimation(offset, milliseconds);
            animation.From = fromoffset;
            animation.EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e1) =>
            {
                Timer.Start();
                CurrentVerticalOffset = offset;
                this.PanningMode = PanningMode.VerticalOnly;
                _isRunning = false;
            };

            _isRunning = true;

            BeginAnimation(CurrentVerticalOffsetProperty, animation, HandoffBehavior.Compose);


        }

        public int SelectTime
        {
            get { return (int)GetValue(SelectTimeProperty); }
            set
            {
                SetValue(SelectTimeProperty, value);

            }
        }



        // Using a DependencyProperty as the backing store for SelectTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectTimeProperty =
            DependencyProperty.Register("SelectTime", typeof(int), typeof(TimerScrollViewer), new PropertyMetadata(0, SlideCallBack));
        private static void SlideCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollView = d as TimerScrollViewer;
            scrollView.PanningMode = PanningMode.None;
            scrollView.PanningMode = PanningMode.VerticalOnly;
            scrollView.IsDown = true;
            var itemIndex = Math.Round(scrollView.VerticalOffset / scrollView.ItemHeight);
            //Console.WriteLine(itemIndex + "====" + scrollView.VerticalOffset + "====" + scrollView.ItemHeight + "=====" + e.NewValue);
            if ((int)e.NewValue == itemIndex)
                return;
            scrollView.ScrollToVerticalOffsetWithAnimation(scrollView.VerticalOffset, (int)e.NewValue * scrollView.ItemHeight, 500);

        }



        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        internal static readonly DependencyProperty CurrentVerticalOffsetProperty = DependencyProperty.Register(
            "CurrentVerticalOffset", typeof(double), typeof(TimerScrollViewer), new PropertyMetadata(.0, OnCurrentVerticalOffsetChanged));

        private static void OnCurrentVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HandyControl.Controls.ScrollViewer ctl && e.NewValue is double v)
            {
                ctl.ScrollToVerticalOffset(v);
            }
        }

        public static readonly RoutedEvent SildeCompletedEvent = EventManager.RegisterRoutedEvent("SildeComplete", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(TimerScrollViewer));

        /// <summary>
        /// 处理各种路由事件的方法
        /// </summary>
        public event RoutedEventHandler SildeComplete
        {
            //将路由事件添加路由事件处理程序
            add { AddHandler(SildeCompletedEvent, value); }
            //从路由事件处理程序中移除路由事件
            remove { RemoveHandler(SildeCompletedEvent, value); }
        }

        /// <summary>
        ///     当前垂直滚动偏移
        /// </summary>
        public double CurrentVerticalOffset
        {
            // ReSharper disable once UnusedMember.Local
            get => (double)GetValue(CurrentVerticalOffsetProperty);
            set => SetValue(CurrentVerticalOffsetProperty, value);
        }

        private ObservableCollection<string> dataSource = new ObservableCollection<string>();

        //private ItemsPresenter itemControl;


        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            base.OnScrollChanged(e);
            if (ItemHeight == 0)
            {

                //itemControl = Core.Common.FindVisualChild<ItemsPresenter>(this);
                var scrollpresenter = FindVisualChild<ContentPresenter>(this);
                ItemHeight = FindVisualChild<ContentPresenter>(scrollpresenter).ActualHeight;
                //ItemHeight = itemControl.ActualHeight / itemControl.Items.Count;
                //dataSource = itemControl.ItemsSource as ObservableCollection<string>;
            }

            if (_isRunning)
                return;

            if (Timer.IsEnabled)
            {
                Timer.Stop();
            }
            Timer.Start();

        }


        private static childItem FindVisualChild<childItem>(DependencyObject obj)
         where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            IsDown = true;
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
            IsDown = false;
            if (!isRun)
                Timer.Start();
        }

        private void LeftTimer_Tick(object sender, EventArgs e)
        {
            isRun = true;
            Timer.Stop();
            //手指点着屏幕时候不触发动画
            if (!IsDown)
            {
                if (ItemHeight == double.PositiveInfinity)
                    return;
                AdjustVerticalOffSet(this.VerticalOffset, ItemHeight, this);
            }

            isRun = false; ;
        }



        /// <summary>
        ///需要调整到中间
        /// </summary>
        private void AdjustVerticalOffSet(double verticaloffset, double itemheight, TimerScrollViewer scrollViewer)
        {


            var itemIndex = Math.Round(verticaloffset / itemheight);
            if (double.IsNaN(itemIndex))
                return;

            SelectTime = (int)itemIndex;

            RoutedEventArgs args2 = new RoutedEventArgs(SildeCompletedEvent, this);
            //引用自定义路由事件
            this.RaiseEvent(args2);

            if (verticaloffset == itemIndex * itemheight)
                return;

            scrollViewer.ScrollToVerticalOffsetWithAnimation(verticaloffset, itemIndex * itemheight, 500);// leftScrollView.VerticalOffset

        }

        private void TimerScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ItemHeight == 0)
            {
                //itemControl = Core.Common.FindVisualChild<ItemsPresenter>(this);
                // ItemHeight = itemControl.ActualHeight / itemControl.Items.Count;
                // dataSource = itemControl.ItemsSource as ObservableCollection<string>;
                var scrollpresenter = FindVisualChild<ContentPresenter>(this);
                ItemHeight = FindVisualChild<ContentPresenter>(scrollpresenter).ActualHeight;
            }

            if (MouseWheelTimer.IsEnabled)
                MouseWheelTimer.Stop();
            MouseWheelTimer.Start();
        }
        private void MouseWheelTimer_Tick(object sender, EventArgs e)
        {
            MouseWheelTimer.Stop();
            //手指点着屏幕时候不触发动画

            if (ItemHeight == double.PositiveInfinity)
                return;
            AdjustVerticalOffSet(this.VerticalOffset, ItemHeight, this);

        }
    }
}
