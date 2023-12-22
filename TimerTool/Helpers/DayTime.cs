 using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TimerTool.Helpers
{
    public class DayTime : BindableBase
    {
        private int _hour;//时
        private int _minute;//分
        private int _second;//秒
        private int _dayToSecond;//把 [时:分:秒] 转化为 [秒]  （这1天一共有多少秒）

        #region 公开属性

        /// <summary>
        /// 时
        /// </summary>
        public int Hour
        {
            get { return _hour; }
            set { AddOrRemoveSeconds((value - _hour) * 60 * 60); }
        }

        /// <summary>
        /// 分
        /// </summary>
        public int Minute
        {
            get { return _minute; }
            set { AddOrRemoveSeconds((value - _minute) * 60); }
        }

        /// <summary>
        /// 秒
        /// </summary>
        public int Second
        {
            get { return _second; }
            set { AddOrRemoveSeconds(value - _second); }
        }

        /// <summary>
        /// 把 [时:分:秒] 转化为 [秒]
        ///（一共有多少秒）
        /// </summary>
        public int DayToSecond
        {
            get { return _dayToSecond; }
            set { SecondToDay(value); }
        }


        /// <summary>
        /// 把时间转换为 [分钟:秒钟]
        /// </summary>
        public List<string> TimerList => new List<string>
                                            {
                                                (Hour/10).ToString(),
                                                (Hour%10).ToString(),
                                                (Minute/10).ToString(),
                                                (Minute%10).ToString(),
                                                (Second/10).ToString(),
                                                (Second%10).ToString(),
                                            };

        private bool _isOverTime = false;
        public bool IsOverTime => _isOverTime;

        #endregion 公开属性

        #region 构造方法

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_hour">时</param>
        /// <param name="_minute">分</param>
        /// <param name="_second">秒</param>
        public DayTime(int hour, int minute, int second)
        {
            _hour = 0;
            _minute = 0;
            _second = 0;

            //计算一共有多少秒
            _dayToSecond = hour * 60 * 60 + minute * 60 + second;

            //把 [秒] 转化为 [时:分:秒]
            SecondToDay(_dayToSecond);
            _isOverTime = false;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dayToSecond">这1天一共有多少秒？</param>
        public DayTime(int dayToSecond)
        {
            _hour = 0;
            _minute = 0;
            _second = 0;
            _dayToSecond = dayToSecond;

            //把 [秒] 转化为 [时:分:秒]
            SecondToDay(dayToSecond);
        }

        public override string ToString()
        {
            return _hour + ":" + _minute + ":" + _second;
        }

        #endregion 构造方法

        #region 公开方法

        /// <summary>
        /// 添加/减少 秒数
        /// </summary>
        /// <param name="changeSeconds">要改变的秒数（+代表增加，-代表减少）</param>
        public void AddOrRemoveSeconds(int changeSeconds)
        { 
            _dayToSecond += changeSeconds;
            SecondToDay(_dayToSecond);
            if (_dayToSecond < 0 && !IsOverTime)
            {
                _isOverTime = true;
                RaisePropertyChanged(nameof(IsOverTime));
            }
            else if (_dayToSecond >= 0 && IsOverTime)
            {
                _isOverTime = false;
                RaisePropertyChanged(nameof(IsOverTime));
            }
        }

        #endregion 公开方法

        #region 私有方法

        /// <summary>
        /// 把 [秒] 转化为 [时:分:秒]
        /// </summary>
        /// <param name="dayToSecond">这一天一共有多少秒?</param>
        private void SecondToDay(int dayToSecond)
        { 
            var absDayToSecond = Math.Abs(dayToSecond);
            _hour = (int)(absDayToSecond / 60.0f / 60.0f);
            _minute = (int)((absDayToSecond - _hour * 60 * 60) / 60.0f);
            _second = absDayToSecond - _hour * 60 * 60 - _minute * 60;

            _dayToSecond = dayToSecond;
             
            RaisePropertyChanged(nameof(TimerList));
        }

        #endregion 私有方法

        #region 静态方法-[DateTime转DayTime]

        /// <summary>
        /// 把DateTime对象 转换为 DayTime对象
        /// </summary>
        /// <param name="dateTime">要转换的DateTime对象</param>
        /// <returns>转换后的DayTime对象</returns>
        public static DayTime DateTimeToDayTime(DateTime dateTime)
        {
            DayTime dayTime = new DayTime(
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second + dateTime.Millisecond / 1000);

            return dayTime;
        }

        /// <summary>
        /// 把DayTime对象 转换为 DateTime对象
        /// </summary>
        /// <param name="dateTime">要转换的DayTime对象</param>
        /// <returns>转换后的DateTime对象</returns>
        public static DateTime DateTimeToDayTime(int year, int month, int day, DayTime dayTime)
        {
            DateTime dateTime = new DateTime(
                year, //年
                month, //月
                day, //日
                dayTime.Hour, //时
                dayTime.Minute, 0); //分

            dateTime.AddSeconds(dayTime.Second); //秒

            return dateTime;
        }

        #endregion 静态方法-[DateTime转DayTime]

        #region 静态方法-[时间转换]

        /// <summary>
        /// 计算当前的这天，一共有多少秒
        /// </summary>
        /// <param name="dateTime">日期数据</param>
        /// <returns>这一天一共有多少秒</returns>
        public static float DateTime_DayToSecond(DateTime dateTime)
        {
            float dayToSecond = dateTime.Hour * 60 * 60 +
                                dateTime.Minute * 60 +
                                dateTime.Second +
                                dateTime.Millisecond / 1000.0f;

            return dayToSecond;
        }

        #endregion 静态方法-[时间转换]

        #region 重写运算符 -[DayTime与DayTime的比较]

        /// <summary>
        /// 重写">"符号的逻辑
        /// </summary>
        public static bool operator >(DayTime d1, DayTime d2)
        {
            bool isD1Max = false;//是否是d1更大？

            /*进行判断*/
            if (d1.DayToSecond > d2.DayToSecond)
            {
                isD1Max = true;
            }

            /*返回值*/
            return isD1Max;
        }

        /// <summary>
        /// 重写"<"符号的逻辑
        /// </summary>
        public static bool operator <(DayTime d1, DayTime d2)
        {
            bool isD1Min = false;//是否是d1更小？

            /*进行判断*/
            if (d1.DayToSecond < d2.DayToSecond)
            {
                isD1Min = true;
            }

            /*返回值*/
            return isD1Min;
        }

        /// <summary>
        /// 重写">="符号的逻辑
        /// </summary>
        public static bool operator >=(DayTime d1, DayTime d2)
        {
            return d1.DayToSecond >= d2.DayToSecond;
        }

        /// <summary>
        /// 重写"<="符号的逻辑
        /// </summary>
        public static bool operator <=(DayTime d1, DayTime d2)
        {
            return d1.DayToSecond <= d2.DayToSecond;
        }

        /// <summary>
        /// 重写"!="符号的逻辑
        /// </summary>
        public static bool operator !=(DayTime d1, DayTime d2)
        {
            return d1.DayToSecond != d2.DayToSecond;
        }

        /// <summary>
        /// 重写"=="符号的逻辑
        /// </summary>
        public static bool operator ==(DayTime d1, DayTime d2)
        {
            return d1.DayToSecond <= d2.DayToSecond;
        }

        #endregion 重写运算符 -[DayTime与DayTime的比较]

        #region 重写运算符 -[DayTime与DateTime的比较]

        /// <summary>
        /// 重写">"符号的逻辑
        /// </summary>
        public static bool operator >(DayTime day, DateTime date)
        {
            float dateDayToSecond = DateTime_DayToSecond(date);

            return day.DayToSecond > dateDayToSecond;
        }

        /// <summary>
        /// 重写"<"符号的逻辑
        /// </summary>
        public static bool operator <(DayTime day, DateTime date)
        {
            float dateDayToSecond = DateTime_DayToSecond(date);

            return day.DayToSecond < dateDayToSecond;
        }

        /// <summary>
        /// 重写">="符号的逻辑
        /// </summary>
        public static bool operator >=(DayTime day, DateTime date)
        {
            float dateDayToSecond = DateTime_DayToSecond(date);

            return day.DayToSecond >= dateDayToSecond;
        }

        /// <summary>
        /// 重写"<="符号的逻辑
        /// </summary>
        public static bool operator <=(DayTime day, DateTime date)
        {
            float dateDayToSecond = DateTime_DayToSecond(date);

            return day.DayToSecond <= dateDayToSecond;
        }

        #endregion 重写运算符 -[DayTime与DateTime的比较]

    }
}
/// <summary>
/// 每天的时间：时、分、秒
/// </summary>
