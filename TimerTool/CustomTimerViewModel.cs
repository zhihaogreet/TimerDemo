using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TimerTool.Helpers;
using System.Windows.Controls.Primitives;

namespace TimerTool
{
    public class CustomTimerViewModel : BindableBase
    {
        public CustomTimerViewModel()
        {
            TimeSystem.Instance.IsOverTimeAction = () =>
            {
                if (Status != TimerStatus.MinWin)
                    Status = TimerStatus.OverTime;
                //铃声开启
                if (IsRing)
                {
                    _player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\Alarm01.wav";
                    _player.PlayLooping();
                }
            };
            TimeSystem.Instance.IsAreadyThreeSecondAction = RedeuceWin;
        }
        #region 事件 
        /// <summary>
        /// 开始计时
        /// </summary>
        public ICommand OnStartTimerCommand => new DelegateCommand(() =>
        {
            DayTime dayTime = new DayTime(SelectHour, SelectMin, SelectSecond);
            TimeSystem.Instance.StartHandle(dayTime);
            if (!IsMinStatus)
                Status = TimerStatus.Nomal;
        });

        /// <summary>
        /// 按钮选择倒计时分钟（控制scrollview滑动）
        /// </summary>
        public ICommand OnSelectTimeCommand => new DelegateCommand<object>(o =>
        {
            var btn = o as ToggleButton;
            var scrollView = btn.Tag as TimerScrollViewer;
            if (scrollView != null)
                scrollView.SelectTime = int.Parse(btn.Content.ToString().Replace("分钟", ""));
        });

        /// <summary>
        /// 重新开始
        /// </summary>
        public ICommand OnStartAgainCommand => new DelegateCommand(() =>
        {
            ReSetValue();
            OnStartTimerCommand.Execute(null);
        });

        /// <summary>
        /// 暂停继续
        /// </summary>
        public ICommand OnPauseOrContinueCommand => new DelegateCommand(() =>
        {
            if (TimeSystem.Instance.CurrentState is StateType.Pause)
                TimeSystem.Instance.UnPauseHandle();
            else
                TimeSystem.Instance.PauseHandle();
        });

        /// <summary>
        /// 重新设置
        /// </summary>
        public ICommand OnSettingAgainCommand => new DelegateCommand(() =>
        {
            Status = 0;
            TimeSystem.Instance.StopHandle();
            //IsPause = false;
            ReSetValue();
        });

        /// <summary>
        /// 按钮选择倒计时时间
        /// </summary>
        public ICommand SelectMinCommand => new DelegateCommand<object>((num) =>
        {
            SelectHour = -1;
            SelectSecond = -1;
            SelectHour = 0;
            SelectSecond = 0;
            SelectMin = int.Parse(num.ToString());
        });

        public ICommand OnCloseCommand => new DelegateCommand(() =>
        {
            //窗口关闭后 线程还存在  如果不终止， 设置铃声后 即使关闭了窗口还会继续响铃声
            TimeSystem.Instance.StopHandle();
            Status = TimerStatus.Nomal; //为了使 openTime模板隐藏 ，隐藏后那些选择时间的按钮（5分钟，10分钟）取消选中状态
            Status = TimerStatus.Setting;
            SelectHour = 0;
            SelectMin = 0;
            SelectSecond = 0;
            ReSetValue();
            return;
        });

        #endregion


        /// <summary>
        /// 重置值
        /// </summary>
        private void ReSetValue()
        {
            _player.Stop();
        }

        private void RedeuceWin()
        {
            //开始计时状态 三秒后开始缩小窗体(第一次才缩小)
            if ((Status == TimerStatus.Nomal || Status == TimerStatus.OverTime) && WinState != WindowState.Maximized)
            {
                IsMinStatus = true;
            }
        }

        #region 缩小窗口后拖动  和  点击事件 (拖动不点击)


        private bool _isDrag;

        public bool IsDrag
        {
            get { return _isDrag; }
            set
            {
                _isDrag = value;
                TimeSystem.Instance.IsDrag = value;
            }
        }


        private Point startPoint;
        public ICommand OnTouchDownCommand => new DelegateCommand<MouseEventArgs>(OnToucheDown);

        private void OnToucheDown(MouseEventArgs e)
        {
            if (Status == TimerStatus.MinWin && e.OriginalSource is System.Windows.Controls.Image)
                return;
            IsDrag = false;
            startPoint = e.GetPosition(null);
        }

        public ICommand OnTouchMoveCommand => new DelegateCommand<MouseEventArgs>(OnTouchMove);

        private void OnTouchMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(null);
                if (Math.Abs(currentPosition.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    IsDrag = true;
                }
            }
        }

        public ICommand OnTouchUpCommand => new DelegateCommand(OnTouchUp);

        private void OnTouchUp()
        {
            //点击
            if (!IsDrag && IsMinStatus)
            {

                if (TimeSystem.Instance.CurrentTime.IsOverTime)
                {
                    Status = TimerStatus.OverTime;
                }
                else
                {
                    Status = TimerStatus.Nomal;
                }

            }

            IsDrag = false;
        }

        #endregion 缩小窗口后拖动  和  点击事件 (拖动不点击)


        #region 属性 
        /// <summary>
        /// 选择小时列表
        /// </summary>
        public ObservableCollection<string> HourList => TimeList();

        /// <summary>
        /// 选择时间分钟列表
        /// </summary>
        public ObservableCollection<string> MinList => TimeList();

        /// <summary>
        /// 选择时间秒列表
        /// </summary>
        public ObservableCollection<string> SenList => TimeList();

        private static ObservableCollection<string> TimeList()
        {
            var list = new ObservableCollection<string>();
            string strValue;
            for (int i = 0; i <= 61; i++)
            {
                if (i == 0)
                    strValue = "";
                else if (i == 61)
                    strValue = "";
                else
                    strValue = (i - 1).ToString().PadLeft(2, '0');

                list.Add(strValue);
            }
            return list;
        }

        private int _selectMin;

        /// <summary>
        /// 选择的分钟
        /// </summary>
        public int SelectMin
        {
            get { return _selectMin; }
            set
            {
                _selectMin = value;
                RaisePropertyChanged(nameof(IsStartEnable));
                RaisePropertyChanged();
            }
        }

        private int _selectSecond;

        /// <summary>
        /// 选择的秒
        /// </summary>
        public int SelectSecond
        {
            get { return _selectSecond; }
            set
            {
                _selectSecond = value;
                RaisePropertyChanged(nameof(IsStartEnable));
                RaisePropertyChanged();
            }
        }

        private int _selectHour;

        public int SelectHour
        {
            get { return _selectHour; }
            set
            {
                _selectHour = value;
                RaisePropertyChanged(nameof(IsStartEnable));
                RaisePropertyChanged();
            }
        }

        public bool IsStartEnable => !(SelectHour == 0 && SelectMin == 0 && SelectSecond == 0);





        private WindowState _winState;

        public WindowState WinState
        {
            get { return _winState; }
            set
            {
                _winState = value;
                RaisePropertyChanged();
            }
        }

        private TimerStatus _status = TimerStatus.Setting;

        /// <summary>
        /// 0-设置时间界面 1-开始计时  2-开始计时缩小窗口  
        /// </summary>
        public TimerStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                if (Status != TimerStatus.MinWin)
                {
                    IsMinStatus = false;
                }
                RaisePropertyChanged();
            }
        }



        private bool _isRing;

        /// <summary>
        /// 是否设置铃声
        /// </summary>
        public bool IsRing
        {
            get { return _isRing; }
            set
            {
                _isRing = value;
                RaisePropertyChanged();
            }
        }

        private int _height = 400;

        public int Height
        {
            get { return _height; }
            private set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        private int _width = 450;

        public int Width
        {
            get { return _width; }
            private set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        private bool _isMinStatus;

        public bool IsMinStatus
        {
            get { return _isMinStatus; }
            set
            {
                _isMinStatus = value;
                if (IsMinStatus)
                {
                    Status = TimerStatus.MinWin;
                }
                RaisePropertyChanged();
            }
        }

        private System.Media.SoundPlayer _player = new System.Media.SoundPlayer();

        #endregion 属性
    }

    public enum TimerStatus
    {
        /// <summary>
        /// 设置界面
        /// </summary>
        Setting,
        /// <summary>
        /// 计时界面
        /// </summary>
        Nomal,
        /// <summary>
        /// 超时界面
        /// </summary>
        OverTime,
        /// <summary>
        /// 小窗口界面
        /// </summary>
        MinWin,
    }
}
