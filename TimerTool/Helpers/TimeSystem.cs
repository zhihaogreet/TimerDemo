using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;

namespace TimerTool.Helpers
{
    /// <summary>
    /// 时间的系统
    /// </summary>
    public class TimeSystem : BindableBase
    {
        #region 属性

        private int _dcount = 0;

        private Timer _timer;//计时器

        public static readonly TimeSystem Instance = new TimeSystem();

        private StateType _currentState;//当前的状态 
        /// <summary>
        /// 当前的状态
        /// </summary>
        public StateType CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;
                RaisePropertyChanged(nameof(IsPause));
                RaisePropertyChanged();

            }
        }

        private DayTime _currentTime;//当前的时间 

        /// <summary>
        /// 当前的时间
        /// </summary>
        public DayTime CurrentTime
        {
            get { return _currentTime; }
        }
         
        public bool IsPause => CurrentState is StateType.Pause;   

        public Action IsOverTimeAction;

        public Action IsAreadyThreeSecondAction;

        public bool IsDrag = false;

        #endregion
        private TimeSystem()
        {
            //设置定时器
            _timer = new Timer();//定时器：类似于Unity中的SuperInvoke插件
            _timer.Interval = 1000;//定时器的间隔时间（1秒）：每隔多少秒，调用一次代码？
            _timer.Elapsed += TimerOnTick;//要调用的代码
            _currentState = StateType.None;
            _currentTime = new DayTime(0);
        }

        #region 公开方法
        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void StartHandle(DayTime daytime)
        {
            _timer.Stop();
            _currentTime = daytime;
            RaisePropertyChanged(nameof(CurrentTime));
            //修改标识符
            CurrentState = StateType.Run;
            //开始运行计时器
            _timer.Start();
            _dcount = 0;
        }


        /// <summary>
        /// 取消暂停倒计时
        /// </summary>
        public void UnPauseHandle()
        {
            //修改标识符
            CurrentState = StateType.Run;
        }


        /// <summary>
        /// 暂停倒计时
        /// </summary>
        public void PauseHandle()
        {
            //修改标识符
            CurrentState = StateType.Pause;
        }


        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopHandle()
        {
            //修改标识符
            CurrentState = StateType.Stop;

            //停止计时器
            _timer.Stop();
        }
        #endregion


        #region 私有方法 -[定时器的事件] 

        // 定时器的Tick事件：当定时器每次达到间隔时间时，都会触发一次Tick事件
        private void TimerOnTick(object sender, EventArgs e)
        {
            /* 判断是否进行倒计时？ */
            if (CurrentState != StateType.Run)
                return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                CurrentTime.AddOrRemoveSeconds(-1);
            }));

            if (CurrentTime.DayToSecond == -1)
            { 
                IsOverTimeAction?.Invoke();
            }
            if (IsDrag && _dcount <= 2)
            {
                _dcount = 0; 
            }
            if (_dcount++ == 2)
            {
                IsAreadyThreeSecondAction?.Invoke();
            }
        }
        #endregion
    }
    public enum StateType : byte
    {
        None,
        Run,//倒计时
        Pause,//暂停
        Stop,//停止
    }
}
