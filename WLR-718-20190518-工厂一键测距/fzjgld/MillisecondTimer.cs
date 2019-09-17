using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;


namespace fzjgld
{

    /* 因System.Threading.Timer和Winform中的 System.Windows.Forms.Timer如果精确到20ms或者更小的时候会发生执行次数不准确，执行次数变少了。因此封装了Timer使执行次数准确*/
    public sealed class MillisecondTimer : IComponent, IDisposable
    {

        //*****************************************************  字 段  *******************************************************************
        private static TimerCaps caps;
        private int interval;
        private bool isRunning;
        private int resolution;
        private TimerCallback timerCallback;
        private int timerID;


        //*****************************************************  属 性  *******************************************************************
        /// <summary>
        /// 
        /// </summary>
        public int Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                if ((value < caps.periodMin) || (value > caps.periodMax))
                {
                    throw new Exception("超出计时范围！");
                }
                this.interval = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ISite Site
        {
            set;
            get;
        }



        //*****************************************************  事 件  *******************************************************************

        public event EventHandler Disposed;  // 这个事件实现了IComponet接口
        public event EventHandler Tick;




        //***************************************************  构造函数和释构函数  ******************************************************************

        static MillisecondTimer()
        {
            timeGetDevCaps(ref caps, Marshal.SizeOf(caps));
        }


        public MillisecondTimer()
        {
            this.interval = caps.periodMin;    // 
            this.resolution = caps.periodMin;  //

            this.isRunning = false;
            this.timerCallback = new TimerCallback(this.TimerEventCallback);

        }


        public MillisecondTimer(IContainer container)
            : this()
        {
            container.Add(this);
        }



        ~MillisecondTimer()
        {
            timeKillEvent(this.timerID);
        }



        //*****************************************************  方 法  *******************************************************************

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (!this.isRunning)
            {
                this.timerID = timeSetEvent(this.interval, this.resolution, this.timerCallback, 0, 1); // 间隔性地运行

                if (this.timerID == 0)
                {
                    throw new Exception("无法启动计时器");
                }
                this.isRunning = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            if (this.isRunning)
            {
                timeKillEvent(this.timerID);
                this.isRunning = false;
            }
        }

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            timeKillEvent(this.timerID);
            GC.SuppressFinalize(this);
            EventHandler disposed = this.Disposed;
            if (disposed != null)
            {
                disposed(this, EventArgs.Empty);
            }
        }

        //***************************************************  内部函数  ******************************************************************
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerCallback callback, int user, int mode);


        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);


        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);
        //  The timeGetDevCaps function queries the timer device to determine its resolution. 

        private void TimerEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if (this.Tick != null)
            {
                this.Tick(this, null);  // 引发事件
            }
        }


        //***************************************************  内部类型  ******************************************************************

        private delegate void TimerCallback(int id, int msg, int user, int param1, int param2); // timeSetEvent所对应的回调函数的签名


        /// <summary>
        /// 定时器的分辨率（resolution）。单位是ms，毫秒？
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TimerCaps
        {
            public int periodMin;
            public int periodMax;
        }

    }
}
