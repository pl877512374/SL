using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace fzjgld
{
    class DrawCoordinate
    {
        /********************************  GDI+自定义坐标系  ****************************************************
         * 
         * 屏幕坐标指的是自定义的坐标系即本程序中呈现在眼前的直角坐标系：坐标系原点坐标（0,0）自定义，每格宽度自定义
         *    
         *    
         * 像素点坐标即默认的坐标系：左上角的坐标为（0,0）
         *
         * 注意：保存作图记录时应记录屏幕点而不是像素点，然后将屏幕点转换为像素点再进行重绘
         * 
         * Author: 张士英
         * Date:   2017-11-7
         * ModifyDate: 2019-01-23
         * *******************************************************************************************************/
        
        #region 参数定义

        public Bitmap bitmap;
        public Graphics graphics;
        public System.Windows.Forms.PictureBox PicBox;
        public float g_n32Scale = 0.0f;           //放大倍数
        public Point g_epCanvasOrigin;            //坐标原点(像素点)
        private float g_f32FillWidth = 3;         //绘制鼠标点时正方形的宽度
        private float g_f32FillWidth2 = 7;
        //lsp---listscreenpoint            //屏幕点（直角坐标系点）集合
        //lpp---listpolarpoint             //极坐标系点            集合
        //lep---listPixelpoint             //像素点                集合
        public bool g_bpicturerefresh = true;
        #endregion

        #region 绘制直角坐标系
        public void DrawRectangularCoordinate(List<Point> p_lsppath3, List<Point> p_lsppath2, List<Point> p_lsppath1,int p_n32bankno)
        {
            graphics.Clear(SystemColors.Control);
            FillTransparentShadow(6000);                                  //安全区域标记
            //FillBankPath(p_alsppathW, p_alsppathW, p_alsppathS);          //先画区域再画坐标系，免得坐标系被覆盖
            DrawBankField(p_lsppath3,p_lsppath2,p_lsppath1,p_n32bankno);
            //绘制网格线
            float nIncrement = (50 * g_n32Scale);                         //网格间的间隔 根据比例绘制
            int l_n32coordinatestr = 500;
            if (g_n32Scale <= 0.7)
            {
                nIncrement = (50 * g_n32Scale);                           //网格间的间隔 根据比例绘制
            }
            else if (g_n32Scale > 0.7 && g_n32Scale <= 2)
            {
                nIncrement = (25 * g_n32Scale);                           //网格间的间隔 根据比例绘制
                l_n32coordinatestr = 250;
            }
            else if (g_n32Scale > 2)
            {
                nIncrement = (float)(12.5 * g_n32Scale);                  //网格间的间隔 根据比例绘制
                l_n32coordinatestr = 125;
            }

            Font font = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
            Brush stringbrush = new SolidBrush(Color.FromArgb(255, Color.Blue));
            //绘制竖向线
            int l_n32lenx1 = (int)(Math.Abs(g_epCanvasOrigin.X / nIncrement));
            int l_n32lenx2 = (int)(Math.Abs((PicBox.Width - g_epCanvasOrigin.X) / nIncrement));
            string[] l_sArrayx1 = new string[l_n32lenx1 + 1];
            string[] l_sArrayx2 = new string[l_n32lenx2 + 1];

            for (int i = 0; i <= l_n32lenx1; i++)
            {
                l_sArrayx1[i] = (-i * l_n32coordinatestr).ToString();
            }
            for (int i = 0; i <= l_n32lenx2; i++)
            {
                l_sArrayx2[i] = (i * l_n32coordinatestr).ToString();
            }
            int jx1 = 0;
            for (float x = g_epCanvasOrigin.X; x >= 0; x -= nIncrement)
            {
                graphics.DrawLine(Pens.Gray, x, 0, x, PicBox.Height);

                if (jx1 <= l_n32lenx1 && jx1 % 2 == 0 && l_sArrayx1[jx1] != "0")
                {
                    graphics.DrawString(l_sArrayx1[jx1], font, stringbrush, x, g_epCanvasOrigin.Y);
                }
                jx1++;
            }
            int jx2 = 0;
            for (float x = g_epCanvasOrigin.X; x <= PicBox.Width; x += nIncrement)
            {
                graphics.DrawLine(Pens.Gray, x, 0, x, PicBox.Height);
                if (jx2 <= l_n32lenx2 && jx2 % 2 == 0 && l_sArrayx2[jx2] != "-0")
                {
                    graphics.DrawString(l_sArrayx2[jx2], font, stringbrush, x, g_epCanvasOrigin.Y);
                }
                jx2++;

            }

            //绘制横向线
            int l_n32leny1 = (int)(Math.Abs(g_epCanvasOrigin.Y / nIncrement));
            int l_n32leny2 = (int)(Math.Abs((PicBox.Width - g_epCanvasOrigin.Y) / nIncrement));
            string[] l_sArrayy1 = new string[l_n32leny1 + 1];
            string[] l_sArrayy2 = new string[l_n32leny2 + 1];

            for (int i = 0; i <= l_n32leny1; i++)
            {
                l_sArrayy1[i] = (i * l_n32coordinatestr).ToString();

            }
            for (int i = 0; i <= l_n32leny2; i++)
            {
                l_sArrayy2[i] = (-i * l_n32coordinatestr).ToString();
            }
            int j = 0;
            for (float y = g_epCanvasOrigin.Y; y >= 0; y -= nIncrement)
            {
                graphics.DrawLine(Pens.Gray, 0, y, PicBox.Width, y);
                if (j <= l_n32leny1 && j % 2 == 0)
                {
                    graphics.DrawString(l_sArrayy1[j], font, Brushes.Blue, g_epCanvasOrigin.X, y);
                }
                j++;
            }
            int j1 = 0;
            for (float y = g_epCanvasOrigin.Y; y <= PicBox.Width; y += nIncrement)
            {
                graphics.DrawLine(Pens.Gray, 0, y, PicBox.Width, y);
                if (j1 <= l_n32leny2 && j1 % 2 == 0 && l_sArrayy2[j1] != "-0")
                {
                    graphics.DrawString(l_sArrayy2[j1], font, Brushes.Blue, g_epCanvasOrigin.X, y);
                }
                j1++;
            }
            //坐标系
            Pen p = new Pen(Color.Blue, 1);
            graphics.DrawLine(p, 0, g_epCanvasOrigin.Y, PicBox.Width, g_epCanvasOrigin.Y);
            graphics.DrawLine(p, g_epCanvasOrigin.X, 0, g_epCanvasOrigin.X, PicBox.Height);
            p.Dispose();
            
        }

        #endregion

        #region 屏幕坐标转像素点坐标
        /// <summary>
        /// 单个点的转换：
        /// sPoint: ScreenPoint即屏幕点， 
        /// ePoint: 像素点,像素点最初由鼠标绘图得到即e.X,e.Y,因此用ePoint表示屏幕点
        /// </summary>
        /// <param name="sPoint"></param>
        /// <returns ePoint></returns>
        public Point ScreenPointtoPixelPoint(Point p_spoint) 
        {
            Point l_epoint = new Point();
            SizeF ef = (Size)g_epCanvasOrigin;
            l_epoint.X = (int)(p_spoint.X / 10 * g_n32Scale + ef.Width);     //屏幕点转换为坐标点的方法
            l_epoint.Y = (int)(p_spoint.Y / (-10) * g_n32Scale + ef.Height);
            return l_epoint;
        }
        /// <summary>
        /// 一组点的转换：
        /// listsp: 屏幕点的集合，将一组点放入List<Point>类中，sp:ScreenPoint
        /// listep: 像素点的集合，将一组点放入List<Point>类中，ep:ePoint
        /// </summary>
        /// <param name="p_lspoint"></param>
        /// <returns  listep></returns>
        public List<Point> ScreenPointtoPixelPoint(List<Point> p_lspoint)
        {
            List<Point> l_lepoint = new List<Point>();
            int len = p_lspoint.Count;
            for (int i = 0; i < len; i++)
            {
                l_lepoint.Add(ScreenPointtoPixelPoint(p_lspoint[i]));
            }
            return l_lepoint;
        }
        #endregion

        #region 像素点转屏幕坐标
        public Point PixelPointtoScreenPoint(Point p_epoint)
        {
            Point l_spoint = new Point();
            SizeF ef = (Size)g_epCanvasOrigin;
            l_spoint.X = (int)((p_epoint.X - ef.Width) * 10 / g_n32Scale);     
            l_spoint.Y = (int)((p_epoint.Y - ef.Height) * (-10) / g_n32Scale);
            return l_spoint;
        }

        public List<Point> PixelPointtoScreenPoint(List<Point> p_lepoint)
        {
            List<Point> l_lspoint = new List<Point>();
            int len = p_lepoint.Count;
            for (int i = 0; i < len; i++)
            {
                l_lspoint.Add(PixelPointtoScreenPoint(p_lepoint[i]));
            }
            return l_lspoint;
        }
        #endregion

        #region 将直角坐标系点转换为Polar坐标系下的点

        #region 求点的Polar坐标(极坐标)长度
        /// <summary>
        /// 求点的Polar坐标长度
        /// sPoint: 屏幕点坐标
        /// </summary>
        /// <param name="sPoint"></param>
        /// <returns PointRadius ></returns>
        public double PointToRadius(Point sPoint)
        {
            double length = Math.Pow(sPoint.X, 2) + Math.Pow(sPoint.Y, 2);
            double PointRadius = Math.Sqrt(length);
            return PointRadius;
        }
        #endregion

        #region 求点的极坐标角度
        /// <summary>
        /// 求点的极坐标角度
        /// sPoint: 屏幕点坐标
        /// </summary>
        /// <param name="sPoint"></param>
        /// <returns></returns>
        public double PointToAngle(Point sPoint)
        {
            int px = sPoint.X;
            int py = sPoint.Y;
            double PointRatio = (double)Math.Abs(py) / (double)Math.Abs(px);
            double PointAngle = Math.Atan(PointRatio) / Math.PI * 180;

            if (px > 0 && py == 0)
            {
                PointAngle = 0+360;
            }
            else if (px == 0 && py > 0)
            {
                PointAngle = 90+360;
            }
            else if (px < 0 && py == 0)
            {
                PointAngle = 180+360 ;
            }
            else if (px == 0 && py < 0)
            {
                PointAngle = 270+360;
            }
            else if (px > 0 && py > 0)
            {
                PointAngle = PointAngle + 360;
            }
            else if (px < 0 && py > 0)
            {
                PointAngle = 180 - PointAngle + 360;
            }
            else if (px < 0 && py < 0)
            {
                PointAngle = PointAngle + 180 + 360;
            }
            else if (px > 0 && py < 0)
            {
                PointAngle = 360 - PointAngle;
            }
            return PointAngle;
        }
        #endregion

        #region 将一组直角坐标系点（屏幕点）转换为极坐标点
        /// <summary>
        /// 将一组屏幕点转换为极坐标点
        /// listsp: 屏幕点的集合
        /// ListAngleAndRadius：极坐标点的集合
        /// </summary>
        /// <param name="listsp"></param>
        /// <returns ListAngleAndRadius></returns>
        public List<AngleAndRadius> ScreenPointListToPolarPointList(List<Point> listsp)
        {
            List<AngleAndRadius> ListAngleAndRadius = new List<AngleAndRadius>();
            int listcout = listsp.Count;
            for (int i = 0; i < listcout; i++)
            {
                double Radius = PointToRadius(listsp[i]);
                double Angle = PointToAngle(listsp[i]);
                ListAngleAndRadius.Add(new AngleAndRadius(Angle, Radius));
            }
            return ListAngleAndRadius;
        }

        public AngleAndRadius ScreenPoint2PolarPoint(Point p_screenpoint)
        {
            double Radius = PointToRadius(p_screenpoint);
            double Angle = PointToAngle(p_screenpoint);
            return new AngleAndRadius(Angle,Radius);
        }

        #endregion

        #endregion

        #region 将极坐标系点转换为直角坐标系点
        /// <summary>
        /// 将Polar点（距离角度）转换为屏幕点（直角坐标系点）
        /// listpp： 极坐标点的集合 pp:PolarPoint
        /// listsp:  直角坐标系点的集合sp:screenpoint
        /// </summary>
        /// <param name="listpp"></param>
        /// <returns  listsp></returns>
        public List<Point> PolarPointToScreenPoint(List<AngleAndRadius> p_lppoint)
        {
            List<Point> l_lspoint = new List<Point>();
            int len = p_lppoint.Count;
            for (int i = 0; i < len; i++)
            {
                double radius = p_lppoint[i].RAD;
                double angle = p_lppoint[i].ANG / 180 * Math.PI;
                double px = radius * Math.Cos(angle);
                double py = radius * Math.Sin(angle);
                l_lspoint.Add(new Point((int)px, (int)py));
            }
            return l_lspoint;
        }

        public Point PolarPointToScreenPoint(AngleAndRadius listpp)
        {
            double radius = listpp.RAD;
            double angle = listpp.ANG / 180 * Math.PI;
            double px = radius * Math.Cos(angle);
            double py = radius * Math.Sin(angle);
            Point sPoint = new Point((int)px, (int)py);
            return sPoint;
            //return listsp;
        }
        #endregion

        #region 绘制路径和顶点
        public void DrawPathAndVertex(List<Point> p_lsppath,int p_n32fieldindex)
        {
            int len = p_lsppath.Count;
            float l_n32width = g_f32FillWidth2 * g_n32Scale;
            Point[] l_appoint = new Point[len];
            for (int i = 0; i < len; i++)  //描点画线
            {
                l_appoint[i] = ScreenPointtoPixelPoint(p_lsppath[i]);
                if (p_n32fieldindex == 3 && (i < len - 1))
                {
                    DrawVertex(l_appoint[i], Color.Green, l_n32width);
                }
                if (i > 0)
                {
                    graphics.DrawLine(new Pen(Color.Black, 1), l_appoint[i - 1].X, l_appoint[i - 1].Y, l_appoint[i].X, l_appoint[i].Y);
                }
            }
            graphics.DrawLine(new Pen(Color.Black, 1), l_appoint[0].X, l_appoint[0].Y,    //第一个点与最后一个点画线构成闭合图形
                             l_appoint[len - 1].X, l_appoint[len - 1].Y);  
        }

        public void DrawPiePathAndVertex(List<Point> p_lsppath, int p_n32fieldindex)
        {
            int len = p_lsppath.Count;
            float l_n32width = g_f32FillWidth2 * g_n32Scale;
            Point[] l_appoint = new Point[len];
            for (int i = 0; i < len; i++)  //描点画线
            {
                l_appoint[i] = ScreenPointtoPixelPoint(p_lsppath[i]);

                if (p_n32fieldindex < 3)
                {
                    if (i > 0 && i < len - 1)
                    {
                        DrawVertex(l_appoint[i], Color.Green, l_n32width);
                    }
                }
                else
                {
                    DrawVertex(l_appoint[i], Color.Green, l_n32width);
                }
            }
        }
        public void DrawBank9_16PathAndVertex(List<Point> p_lsppath, List<Point> p_lspvertex)
        {
            int len = p_lsppath.Count;
            Point[] l_appoint = new Point[len];
            for (int i = 0; i < len; i++)  //画线
            {
                l_appoint[i] = ScreenPointtoPixelPoint(p_lsppath[i]);
                if (i > 0)
                {
                    graphics.DrawLine(new Pen(Color.Black, 1), l_appoint[i - 1].X, l_appoint[i - 1].Y, l_appoint[i].X, l_appoint[i].Y);
                }
 
            }
            graphics.DrawLine(new Pen(Color.Black, 1), l_appoint[0].X, l_appoint[0].Y,    //第一个点与最后一个点画线构成闭合图形
                             l_appoint[len - 1].X, l_appoint[len - 1].Y);

            //描点
            len = p_lspvertex.Count;
            float l_n32width = g_f32FillWidth2 * g_n32Scale;
            Point[] l_apvertex = new Point[len];
            for (int i = 0; i < len; i++)
            {
                l_apvertex[i] = ScreenPointtoPixelPoint(p_lspvertex[i]);
                DrawVertex(l_apvertex[i], Color.Green, l_n32width);
            }
        }
        //画顶点
        public void DrawVertex(Point p_epoint, Color p_color,float p_fwidth)
        {
            graphics.FillRectangle(new SolidBrush(p_color), p_epoint.X - p_fwidth / 2,             //描点
                                                            p_epoint.Y - p_fwidth / 2, p_fwidth, p_fwidth);
        }
        #endregion

        #region 绘制一个Bank中的多个路径和顶点
        //重新绘制区域的点
        /// <summary>
        /// 将每次鼠标作图时得到的原始数据点作为一个元素存放到ArrayListPointHistoryData
        /// ArrayListPointHistoryData：存放鼠标绘图的到点
        /// l_listsp:  局部变量：临时存放ArrayListRegionHistoryData每一个元素中的屏幕点
        /// </summary>
        /// <param name="ArrayListPointHistoryData"></param>
        public void DrawMultiPathAndVertex(ArrayList p_alsppath, int p_n32fieldindex)
        {
            int count = p_alsppath.Count;
            List<Point> l_lsppath = new List<Point>();
            for (int i = 0; i < count; i++)
            {
                l_lsppath.AddRange((List<Point>)(p_alsppath[i]));
                if (l_lsppath.Count > 0)
                {
                    DrawPathAndVertex(l_lsppath, p_n32fieldindex);
                    l_lsppath.Clear();
                }
            }
        }
        #endregion

        #region 将路径填充颜色

        #region 填充单个路径
        //保证重合区域不显示为多层颜色)
        public void FillPathCompositingMode(List<Point> p_lsppath, Color p_color)
        {
            Brush brush;
            Graphics tempgra = Graphics.FromImage(bitmap);
            tempgra.CompositingMode = CompositingMode.SourceCopy;  //保证重合区域不显示为多层颜色
            tempgra.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            tempgra.PixelOffsetMode = PixelOffsetMode.HighQuality;

            List<Point> l_lepoint = ScreenPointtoPixelPoint(p_lsppath);   //像素点集合（作图时使用）
            GraphicsPath gpp = new GraphicsPath();
            gpp.AddCurve(l_lepoint.ToArray(), 0);

            brush = new SolidBrush(Color.FromArgb(100, p_color));
            //tempgra.FillClosedCurve(brush, listep.ToArray(), FillMode.Winding, 0.5f);
            tempgra.FillPath(brush, gpp);

            tempgra.Dispose();
        }
        //普通模式
        public void FillPathColor(List<Point> p_lsppath, Color p_color)              // 修改点时绘制阴影
        {
            List<Point> l_leppath = ScreenPointtoPixelPoint(p_lsppath);              //像素点集合（作图时使用）
            l_leppath.Add(g_epCanvasOrigin);
            GraphicsPath gpp = new GraphicsPath();
            gpp.AddCurve(l_leppath.ToArray(), 0);
            Brush brush = new SolidBrush(Color.FromArgb(170, p_color));
            graphics.FillPath(brush, gpp);
        }

        #endregion

        #region 填充多个路径
        /// <summary>
        /// 重新绘制鼠标绘图得到的区域形状，每一次绘制得到的形状经过计算每零点1度得到一个长度（polar坐标系）
        /// 作为一个元素存放到ArrayListRegionHistoryData中
        /// ArrayListRegionHistoryData：存放鼠标绘图的到的区域形状
        /// l_listsp:  局部变量：临时存放ArrayListRegionHistoryData每一个元素中的屏幕点
        /// </summary>
        /// <param name="ArrayListRegionHistoryData"></param>
        public void FillMultiPathColor(ArrayList p_alsppath, Color p_color)
        {
            int count = p_alsppath.Count;
            List<Point> lsppath = new List<Point>();
            for (int i = 0; i < count; i++)
            {
                lsppath.AddRange((List<Point>)(p_alsppath[i]));
                if (lsppath.Count > 0)
                {
                    FillPathCompositingMode(lsppath, p_color);
                    lsppath.Clear();
                }
            }
        }

        #endregion

        #endregion

        #region 将一个bank中的不同field的路径进行填充

        private void FillBankPath(ArrayList p_alsppathW, ArrayList p_alsppathW2, ArrayList p_alsppathS)
        {

            if (p_alsppathW2.Count > 0)
            {
                FillMultiPathColor(p_alsppathW2, Color.Yellow);      //历史数据区域
            }
            if (p_alsppathW.Count > 0)
            {
                FillMultiPathColor(p_alsppathW, Color.Orange);      //历史数据区域
            }
            if (p_alsppathS.Count > 0)
            {
                FillMultiPathColor(p_alsppathS, Color.Red);      //历史数据区域
            }
        }
        #endregion

        #region 添加90度不绘图区域(填充银色区域)
        public void FillSilverShadow(double p_n64radius)
        {
            List<AngleAndRadius> l_lpppath = new List<AngleAndRadius>();              //极坐标系点
            l_lpppath.Add(new AngleAndRadius(225.0, p_n64radius));
            l_lpppath.Add(new AngleAndRadius(315.0, p_n64radius));
            List<Point> l_lsppath = PolarPointToScreenPoint(l_lpppath);               //屏幕点
            List<Point> l_leppath = ScreenPointtoPixelPoint(l_lsppath);               //像素点集合（作图时使用）
            l_leppath.Add(g_epCanvasOrigin);                                          //添加零点坐标

            GraphicsPath gpp = new GraphicsPath();
            gpp.AddCurve(l_leppath.ToArray(), 0);
            Brush brush = new SolidBrush(Color.FromArgb(100, Color.Silver));
            graphics.FillPath(brush, gpp);
        }
        #endregion

        #region 绘制透明圆形区域（扫描范围）
        public void FillTransparentShadow(double p_n64radius)
        {
            List<AngleAndRadius> l_lpppath = new List<AngleAndRadius>();
            for (double angle = 315; angle < 361.0; angle++)
            {
                l_lpppath.Add(new AngleAndRadius(angle, p_n64radius));
            }
            for (double angle = 0.0; angle < 226.0; angle++)
            {
                l_lpppath.Add(new AngleAndRadius(angle, p_n64radius));
            }
            List<Point> l_lsppath = PolarPointToScreenPoint(l_lpppath);
            List<Point> l_leppath = ScreenPointtoPixelPoint(l_lsppath);               //像素点集合（作图时使用）
            l_leppath.Add(g_epCanvasOrigin);

            GraphicsPath gpp = new GraphicsPath();
            gpp.AddCurve(l_leppath.ToArray(), 0);
            Brush brush = new SolidBrush(Color.FromArgb(255, Color.Transparent));
            graphics.FillPath(brush, gpp);

        }
        #endregion 

        #region 绘制数据实时波形

        public void DrawRealTimeWaveform(double[] p_an64RegionZhiX, double[] p_an64RegionZhiY, int p_n32drawstyle)
        {
            int len = p_an64RegionZhiX.Length;
            Point[] l_curvepoint = new Point[len];    //坐标点
            if (len > 0)
            {
                if (p_n32drawstyle == 1)              
                {
                    for (int i = len - 1; i >= 0; i--)
                    {
                        l_curvepoint[i] = ScreenPointtoPixelPoint(new Point((int)p_an64RegionZhiX[i], (int)p_an64RegionZhiY[i]));
                    }
                    graphics.DrawCurve(new Pen(Color.Red, 1), l_curvepoint, 0.0f);                        //画曲线，第三个参数可以直接影响作图效果
                }
                else if (p_n32drawstyle == 2)
                {
                    for (int i = len - 1; i >= 0; i--)
                    {
                        l_curvepoint[i] = ScreenPointtoPixelPoint(new Point((int)p_an64RegionZhiX[i], (int)p_an64RegionZhiY[i]));
                        graphics.FillRectangle(new SolidBrush(Color.Black), l_curvepoint[i].X - g_f32FillWidth / 2,  //描点
                                               l_curvepoint[i].Y - g_f32FillWidth / 2, g_f32FillWidth, g_f32FillWidth);
                    }
                }
            }
        }
        #endregion

        #region 刷新picbox

        public void PictureBox_Refresh(List<Point> p_lsppath3, List<Point> p_lsppath2,List<Point> p_lsppath1,int p_n32bankno,
                                       double[] p_an64RegionZhiX, double[] p_an64RegionZhiY,int p_n32drawstyle)
        {
            //绘制坐标系
            DrawRectangularCoordinate(p_lsppath3, p_lsppath2, p_lsppath1,p_n32bankno);
            //画波形
            DrawRealTimeWaveform(p_an64RegionZhiX, p_an64RegionZhiY, p_n32drawstyle);

             //225°~315°不做图的区域
            FillSilverShadow(100000);  
                               
        }
        #endregion

        #region 冒泡法排序(分别按polar图的角度和半径进行排序)
        public List<AngleAndRadius> AngleMinus360AndSortS2B(List<AngleAndRadius> p_lpppoint)  //大于360度的角度值减去360
        {
            for (int i = 0; i < p_lpppoint.Count; i++)
            { 
                if (p_lpppoint[i].ANG >=360)
                {
                    p_lpppoint[i].ANG -= 360;
                }
            }
            SortPointListAsAngleS2B(p_lpppoint);
            return p_lpppoint;

        }
        public List<AngleAndRadius> SortPointListAsAngleS2B(List<AngleAndRadius> p_lpppoint)  //按角度由小到大排序
        {
            var temp = p_lpppoint[0];
            for (int i = 0; i < p_lpppoint.Count - 1; i++)  //冒泡法排序(小到大)
            {
                for (int j = 0; j < p_lpppoint.Count - 1 - i; j++)
                {
                    if (p_lpppoint[j].ANG > p_lpppoint[j + 1].ANG)
                    {
                        temp = p_lpppoint[j];
                        p_lpppoint[j] = p_lpppoint[j + 1];
                        p_lpppoint[j + 1] = temp;
                    }
                }
            }
            return p_lpppoint;
        }
        public List<AngleAndRadius> SortPointListAsAngleB2S(List<AngleAndRadius> p_lpppoint)  //按角度由大到小排序
        {
            var temp = p_lpppoint[0];
            for (int i = 0; i < p_lpppoint.Count - 1; i++)  //冒泡法排序(大到小)
            {
                for (int j = 0; j < p_lpppoint.Count - 1 - i; j++)
                {
                    if (p_lpppoint[j].ANG < p_lpppoint[j + 1].ANG)
                    {
                        temp = p_lpppoint[j];
                        p_lpppoint[j] = p_lpppoint[j + 1];
                        p_lpppoint[j + 1] = temp;
                    }
                }
            }
            return p_lpppoint;
        }
        public List<AngleAndRadius> SortPointListAsRadiusS2B(List<AngleAndRadius> p_lpppoint)  //按半径由小到大排序
        {
            var temp = p_lpppoint[0];
            for (int i = 0; i < p_lpppoint.Count - 1; i++)  //冒泡法排序
            {
                for (int j = 0; j < p_lpppoint.Count - 1 - i; j++)
                {
                    if (p_lpppoint[j].RAD > p_lpppoint[j + 1].RAD)
                    {
                        temp = p_lpppoint[j];
                        p_lpppoint[j] = p_lpppoint[j + 1];
                        p_lpppoint[j + 1] = temp;
                    }
                }
            }
            return p_lpppoint;
        }

        #endregion

        #region 余弦定理
        /// <summary>
        /// 已知边长b、c和夹角a求边长a
        /// anglea:夹角a
        /// radiusb：边长b
        /// radiusc: 边长c
        /// </summary>
        /// <param name="anglea"></param>
        /// <param name="radiusb"></param>
        /// <param name="radiusc"></param>
        /// <returns radiusa></returns>
        public double CosineTheorem(double anglea, double radiusb, double radiusc)
        {
            double cosa = Math.Cos(Math.PI * anglea / 180);
            double pow2radiusa = Math.Pow(radiusb, 2) + Math.Pow(radiusc, 2) - 2 * radiusc * radiusb * cosa; //a^2
            double radiusa = Math.Sqrt(pow2radiusa);
            return radiusa;
        }
        #endregion

        #region 删除集合中角度大于某个角度的所有元素
        public List<AngleAndRadius> DeleteSpecifiedAngle(double p_n64angle, List<AngleAndRadius> p_lpppoint)
        {
            for (int i = 0; i < p_lpppoint.Count; i++)
            {
                if (p_lpppoint[i].ANG > p_n64angle)
                {
                    p_lpppoint.RemoveAt(i);
                    if (i > 0)
                        i--;
                }

            }
            return p_lpppoint;
        }
        #endregion

        #region 删除集合中角度相同半径较小的元素
        public List<AngleAndRadius> DeleteSameAngleSmallerRadius(List<AngleAndRadius> p_lpppoint)
        {
            for (int i = 0; i < p_lpppoint.Count - 1; i++)
            {
                for (int j = 0; j < p_lpppoint.Count; j++)
                {
                    if (p_lpppoint[i].ANG == p_lpppoint[j].ANG)
                    {
                        if (i != j)
                        {
                            if (p_lpppoint[i].RAD <= p_lpppoint[j].RAD)
                            {
                                var temp = p_lpppoint[i];
                                p_lpppoint[i] = p_lpppoint[j];
                                p_lpppoint[j] = temp;
                            }
                            p_lpppoint.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            return p_lpppoint;
        }
        #endregion

        #region 删除路径数据中角度相同距离较小的点
        public ArrayList DeleteSmallerRadiusWithSameAngle(ArrayList p_alsppath)
        {
            List<Point> l_latestlist = new List<Point>();
            ArrayList LatestListRegion = new ArrayList();
            l_latestlist.AddRange((List<Point>)p_alsppath[p_alsppath.Count - 1]);
            l_latestlist.RemoveAt(l_latestlist.Count - 1);//删除原点坐标
            List<AngleAndRadius> l_latestPolarList = ScreenPointListToPolarPointList(l_latestlist);
            l_latestPolarList = SortPointListAsAngleS2B(l_latestPolarList);
            double latestMinAngle = l_latestPolarList[0].ANG;
            double latestMaxAngle = l_latestPolarList[l_latestPolarList.Count - 1].ANG;
            for (int i = 0; i < p_alsppath.Count - 1; i++)
            {
                List<Point> l_lsppath = (List<Point>)p_alsppath[i];
                l_lsppath.RemoveAt(l_lsppath.Count - 1);//删除原点坐标
                List<AngleAndRadius> l_lpppath = ScreenPointListToPolarPointList(l_lsppath);
                l_lpppath = SortPointListAsAngleS2B(l_lpppath);
                double l_minAngle = l_lpppath[0].ANG;
                double l_maxAngle = l_lpppath[l_lpppath.Count - 1].ANG;
                if ((l_minAngle < latestMinAngle && latestMinAngle < l_maxAngle) ||
                    (l_minAngle < latestMaxAngle && latestMaxAngle < l_maxAngle))
                {
                    for (int j = 0; j < l_latestPolarList.Count; j++)
                    {
                        for (int k = 0; k < l_lpppath.Count; k++)
                        {
                            double angle1 = Math.Round(l_latestPolarList[j].ANG, 1);
                            double angle2 = Math.Round(l_lpppath[k].ANG, 1);
                            if (Math.Round(l_latestPolarList[j].ANG, 1) == Math.Round(l_lpppath[k].ANG, 1))
                            {
                                if (l_latestPolarList[j].RAD < l_lpppath[k].RAD)
                                {
                                    l_latestPolarList.RemoveAt(j);
                                    j--;
                                    //break;   //对应for (int k = 0; k < l_polarList.Count; k++)
                                }
                                else
                                {
                                    l_lpppath.RemoveAt(k);
                                    k--;
                                }
                            }  //end of if (l_latestPolarList[j].ANG == l_polarList[k].ANG)
                        }  //end of for (int k = 0; k < l_polarList.Count; k++)
                    }// end of for (int j = 0; j < l_latestPolarList.Count; j++)
                }
                List<Point> l_listscreenPoint = PolarPointToScreenPoint(l_lpppath);
                l_listscreenPoint.Add(new Point(0, 0));//添加原点坐标
                LatestListRegion.Add(l_listscreenPoint);
            }
            List<Point> l_listLastestScreenPoint = PolarPointToScreenPoint(l_latestPolarList);
            l_listLastestScreenPoint.Add(new Point(0, 0));//添加原点坐标
            LatestListRegion.Add(l_listLastestScreenPoint);
            return LatestListRegion;
        }
        #endregion

        #region 删除指定角度的数据

        public List<AngleAndRadius> DeleteSelectedAngle(double p_staang, double p_endang, List<AngleAndRadius> p_ARList)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < p_ARList.Count; i++)
                {
                    if (p_ARList[i].ANG > p_staang && p_ARList[i].ANG < p_endang)
                    {
                        p_ARList.RemoveAt(i);
                        i = 0;
                    }
                }
            }
            return p_ARList;
        }
        #endregion

        # region 计算任意形状、矩形或多边形(每零点一度求一个对应的距离值)
        /// <summary>
        /// 根据相邻两个已知点根据余弦定理计算出三角形的第三条边的长度，然后再根据余弦定理计算三角形的其余两个内角的大小；
        /// 即，已知一边长radiusb、另一边长radiusc和bc的夹角angle即可计算出angleb、anglec和radiusa（边长a）
        /// 然后根据边长b和夹角c计算与边长b的夹角从0.1度依次递增到angle时临边radiusbx的长度
        /// listpp： 鼠标作图时得到点转换为极坐标后的点的集合
        /// listRectangleOrPolygon：返回值：每0.1度对应的长度
        /// </summary>
        /// <param name="listpp"></param>
        /// <returns  listRectangleOrPolygon></returns>
        public List<AngleAndRadius> DrawArbitaryRectangleOrPloygon(List<AngleAndRadius> p_lpppoint)
        {
            List<AngleAndRadius> listRectangleOrPolygon = new List<AngleAndRadius>();  //缓存矩形或多边形每0.1度得到的距离值
            SortPointListAsAngleS2B(p_lpppoint);
            DeleteSameAngleSmallerRadius(p_lpppoint);
            listRectangleOrPolygon.Add(new AngleAndRadius(p_lpppoint[0].ANG, p_lpppoint[0].RAD));
            for (int i = 0; i < p_lpppoint.Count - 1; i++)
            {
                double radiusb = p_lpppoint[i].RAD;
                double radiusc = p_lpppoint[i + 1].RAD;
                double angle = p_lpppoint[i + 1].ANG - p_lpppoint[i].ANG;
                if (angle > 180)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        p_lpppoint[j].ANG += 360;
                    }
                    i = -1;
                    listRectangleOrPolygon.Clear();
                    SortPointListAsAngleS2B(p_lpppoint);
                    listRectangleOrPolygon.Add(new AngleAndRadius(p_lpppoint[0].ANG, p_lpppoint[0].RAD));
                    continue;
                }
                else
                {
                    double radiusa = CosineTheorem(angle, radiusb, radiusc);
                    //计算角度时只能用余弦值，正弦值不能区分是锐角还是钝角
                    double cosb = (Math.Pow(radiusa, 2) + Math.Pow(radiusc, 2) - Math.Pow(radiusb, 2)) / (2 * radiusa * radiusc);
                    double angleb = 180 * (Math.Acos(cosb)) / Math.PI;
                    double sinb = Math.Sin(Math.PI * angleb / 180);
                    double cosc = (Math.Pow(radiusa, 2) + Math.Pow(radiusb, 2) - Math.Pow(radiusc, 2)) / (2 * radiusa * radiusb);
                    double anglec = 180 * (Math.Acos(cosc)) / Math.PI;
                    double sinc = Math.Sin(Math.PI * anglec / 180);

                    for (double j = 0.1; j <= angle; j += 0.1)
                    {
                        double anglecx = 180 - j - anglec;
                        double radiusbx = radiusb * sinc / Math.Sin(Math.PI * anglecx / 180);
                        if (radiusbx > 60000)
                            radiusbx = 60000;
                        if (radiusbx < 0)
                        {
                            radiusbx = 0;
                        }
                        if (double.IsNaN(radiusbx))
                        {
                            radiusbx= 0;
                        }
                        double anglebx = j + p_lpppoint[i].ANG;
                        if (anglebx > 360)
                        {
                            anglebx -= 360;
                        }
                        else if (angle < 0)
                        {
                            angle = angle + 360;
                        }
                        listRectangleOrPolygon.Add(new AngleAndRadius(anglebx, radiusbx));
                    }
                }
            }
            return listRectangleOrPolygon;
        }
        #endregion

        #region 计算圆（每零点一度对应一个距离值）
        /// <summary>
        /// 根据鼠标点得到圆的圆心与半径然后根据余弦定理计算每零点一度坐标原点到圆上的距离
        /// liste: 鼠标作图时得到的原始数据点的集合
        /// ListAngleAndRadius： 计算后得到的极坐标下点的集合（角度和半径）
        /// </summary>
        /// <param name="liste"></param>
        /// <returns ListAngleAndRadius></returns>
        public List<AngleAndRadius> DrawCircle(List<Point> p_leppoint)
        {
            List<AngleAndRadius> l_lpppoint= new List<AngleAndRadius>();
            List<AngleAndRadius> l_circlelistpp = new List<AngleAndRadius>();  //缓存上切线与圆心和原点构成的直线的部分的角度与距离
            int len = p_leppoint.Count - 1;
            int x = p_leppoint[0].X;
            int y = p_leppoint[0].Y;
            int width = p_leppoint[len].X - p_leppoint[0].X;
            int a = x + width / 2;      //圆心横坐标
            int b = y + width / 2;      //圆心纵坐标
            int r = Math.Abs(width / 2);//圆半径
            Point UpContactPoint;       //上切点
            Point DownContactPoint;     //下切点

            Point l_spCircleCenterPoint = PixelPointtoScreenPoint(new Point(a, b));
            double CircleCenterAngle = PointToAngle(l_spCircleCenterPoint);                //圆心与原点连线的角度
            double CircleCenterRadius = PointToRadius(l_spCircleCenterPoint);              //圆心转换为的长度

            //上下切点的计算方法根据鼠标向下拖动还是向上拖动而不同
            if (width > 0)                         //鼠标向下拖动
            {
                UpContactPoint = PixelPointtoScreenPoint(new Point(a, y));
                DownContactPoint = PixelPointtoScreenPoint(new Point(a, y + width));
            }
            else                                   //鼠标向上拖动
            {
                DownContactPoint = PixelPointtoScreenPoint(new Point(a, y));
                UpContactPoint = PixelPointtoScreenPoint(new Point(a, y + width));
            }

            double UpContactPointAngle = PointToAngle(UpContactPoint);                 //上切点
            double UpContactPointRadius = PointToRadius(UpContactPoint);               //上切点
            double UpContactPointHeight = UpContactPointRadius * Math.Sin(Math.PI * UpContactPointAngle / 180); //上切点高度

            double DownContactPointAngle = PointToAngle(DownContactPoint);             //下切点
            double DownContactPointRadius = PointToRadius(DownContactPoint);           //下切点
            double DownContactPointHeight = DownContactPointRadius * Math.Sin(Math.PI * DownContactPointAngle / 180); //下切点高度

            double R = (UpContactPointHeight - DownContactPointHeight) / 2;           //转换后圆的半径

            if (CircleCenterRadius > R) //原点不在圆内
            {
                double angle1 = Math.Asin((double)R / CircleCenterRadius) * 180 / Math.PI;  //切线与圆心和原点构成的直线的夹角
                double angle2 = CircleCenterAngle - angle1;    //切线1与x轴正半轴的夹角
                double angle3 = CircleCenterAngle + angle1;    //切线2与x轴正半轴的夹角

                for (double i = 0; i <= angle1; i += 0.1)
                {
                    double angleA = angle1 - i;       //靠近原点的角度
                    double sinangleB = (CircleCenterRadius * Math.Sin(angleA * Math.PI / 180)) / R; //由正弦定理求出远离原点的角的正弦值 
                    double angleB = Math.Asin(sinangleB) * 180 / Math.PI;   //远离原点的角度
                    double angleC = 180 - angleA - angleB;   //radiusA=R;radiusB=CircleCenterRadius
                    double radiusC = CosineTheorem(angleC, R, CircleCenterRadius);

                    double angle = angle3 - i;
                    if (angle > 360)
                    {
                        angle = angle - 360;
                    }
                    else if (angle < 0)
                    {
                        angle = angle + 360;
                    }
                    if (radiusC > 60000)
                    {
                        radiusC = 60000;
                    }
                    if (radiusC < 0)
                    {
                        radiusC = 0;
                    }
                    if (double.IsNaN(radiusC))
                    {
                        radiusC = 0;
                    }
                    l_circlelistpp.Add(new AngleAndRadius(angle, radiusC));
                    l_lpppoint.Add(new AngleAndRadius(angle, radiusC));
                }
                int j = 0;
                int leng = l_circlelistpp.Count() - 1;
                for (double i = 0; i < angle1; i += 0.1 * 1)
                {
                    double angle = CircleCenterAngle - i;
                    if (angle > 360)
                    {
                        angle = angle - 360;
                    }
                    else if (angle < 0)
                    {
                        angle = angle + 360;
                    }
                    l_lpppoint.Add(new AngleAndRadius(angle, l_circlelistpp[leng - j].RAD));
                    j++;
                }
            }  //end of if (CircleCenterRadius > R) 

            else
            {
                for (double i = 0; i < 360; i += 0.1)
                {
                    double anglea = 360 - CircleCenterAngle + i;
                    double sinangleb = CircleCenterRadius * Math.Sin(Math.PI * anglea / 180) / R;
                    double angleb = Math.Asin(sinangleb) * 180 / Math.PI;
                    double anglec = 180 - anglea - angleb;
                    double radiusc = CosineTheorem(anglec, CircleCenterRadius, R);
                    if (radiusc > 60000)
                    {
                        radiusc = 60000;
                    }
                    if (radiusc < 0)
                    {
                        radiusc = 0;
                    }
                    if (double.IsNaN(radiusc))
                    {
                        radiusc = 0;
                    }
                    l_lpppoint.Add(new AngleAndRadius(i, radiusc));

                }
            }
            return l_lpppoint;
        }
        #endregion

        #region 屏幕点倍乘关系函数
        public List<Point> ListScreenMultiple(List<Point> p_sourceLP, float p_fmultiplier)
        {
            List<Point> l_dstLp = new List<Point>();
            int l_n32count = p_sourceLP.Count();
            for (int i = 0; i < l_n32count; i++)
            {
                Point l_pointtemp = new Point((int)(p_sourceLP[i].X * p_fmultiplier), (int)(p_sourceLP[i].Y * p_fmultiplier));
                l_dstLp.Add(l_pointtemp);
            }
            return l_dstLp;             
        }

        #endregion

        #region 极坐标点倍乘关系函数
        public List<AngleAndRadius> ListPolarMultiple(List<AngleAndRadius> p_sourceLP, float p_fmultiplier)
        {
            List<AngleAndRadius> l_larptemp = new List<AngleAndRadius>();
            int l_n32count = p_sourceLP.Count();
            for (int i = 0; i < l_n32count; i++)
            {
                AngleAndRadius l_pointtemp = new AngleAndRadius((int)(p_sourceLP[i].ANG), (int)(p_sourceLP[i].RAD * p_fmultiplier));
                l_larptemp.Add(l_pointtemp);
            }
            return l_larptemp;
        }

        #endregion

        #region 每隔固定角度计算一个距离值
        public List<AngleAndRadius> CalAngleRadius(List<AngleAndRadius> p_listpp,double p_angle,int p_n32bankno)
        {
            List<AngleAndRadius> l_angleradiustemp = new List<AngleAndRadius>();        //缓存计算得到的距离值和角度值
            SortPointListAsAngleS2B(p_listpp);                                          //按角度值由小到大排序
            DeleteSameAngleSmallerRadius(p_listpp);                                     //删除角度值相同距离较小的点
            l_angleradiustemp.Add(new AngleAndRadius(p_listpp[0].ANG, p_listpp[0].RAD));
            for (int i = 0; i < p_listpp.Count - 1; i++)
            {
                double radiusb = p_listpp[i].RAD;
                double radiusc = p_listpp[i + 1].RAD;
                double angle = p_listpp[i + 1].ANG - p_listpp[i].ANG;

                double radiusa = CosineTheorem(angle, radiusb, radiusc);

                //计算角度时只能用余弦值，正弦值不能区分是锐角还是钝角
                double cosb = (Math.Pow(radiusa, 2) + Math.Pow(radiusc, 2) - Math.Pow(radiusb, 2)) / (2 * radiusa * radiusc);
                double angleb = 180 * (Math.Acos(cosb)) / Math.PI;
                double sinb = Math.Sin(Math.PI * angleb / 180);
                double cosc = (Math.Pow(radiusa, 2) + Math.Pow(radiusb, 2) - Math.Pow(radiusc, 2)) / (2 * radiusa * radiusb);
                double anglec = 180 * (Math.Acos(cosc)) / Math.PI;
                double sinc = Math.Sin(Math.PI * anglec / 180);

                for (double j = p_angle; j <= angle; j += p_angle)
                {
                    double anglecx = 180 - j - anglec;
                    double radiusbx = 0;

                    if (p_n32bankno > 4 && p_n32bankno < 9)
                    {
                        radiusbx = radiusb;      //所有距离值都相等
                    }
                    else
                    {
                        radiusbx = radiusb * sinc / Math.Sin(Math.PI * anglecx / 180);
                    }
                   
                    if (radiusbx > 60000)
                        radiusbx = 60000;
                    if (radiusbx < 0)
                    {
                        radiusbx = 0;
                    }
                    if (double.IsNaN(radiusbx))
                    {
                        radiusbx = 0;
                    }
                    double anglebx = j + p_listpp[i].ANG;
                    if (anglebx > 360)
                    {
                        anglebx -= 360;
                    }
                    else if (angle < 0)
                    {
                        angle = angle + 360;
                    }
                    if (anglebx > 0 && anglebx < 271)
                    {
                        anglebx += 360;
                    }
                    l_angleradiustemp.Add(new AngleAndRadius(anglebx, radiusbx));
                }
            }

            return l_angleradiustemp;
        }
        #endregion

        #region Filed1~4原始区域点
        public void DrawField1to4(List<Point> p_lspshadow, List<Point> p_lsppath)
        {
            Point Screenpointx0y0 = PixelPointtoScreenPoint(g_epCanvasOrigin);     //屏幕零点坐标
            List<Point> l_lsppath2 = new List<Point>();
            List<Point> l_lsppath1 = new List<Point>();

            l_lsppath2 = ListScreenMultiple(p_lsppath, 0.75f);
            l_lsppath1 = ListScreenMultiple(p_lsppath, 0.5f);
            //添加零点
            p_lsppath.Add(Screenpointx0y0);
            l_lsppath2.Add(Screenpointx0y0);
            l_lsppath1.Add(Screenpointx0y0);

            //填充路径
            FillPathColor(p_lsppath, Color.DarkOrange);
            FillPathColor(l_lsppath2, Color.OrangeRed);
            FillPathColor(l_lsppath1, Color.Red);
            //绘制路径
            DrawPathAndVertex(p_lsppath, 3);
            DrawPathAndVertex(l_lsppath2, 2);
            DrawPathAndVertex(l_lsppath1, 1);

            p_lsppath.RemoveAt(p_lsppath.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
        }

        #endregion

        #region 绘制安全区域
        public void DrawBankField(List<Point> p_lsppath3, List<Point> p_lsppath2,List<Point> p_lsppath1,int p_n32bankno)
        {
            if (p_n32bankno < 5)
            {
                Point Screenpointx0y0 = PixelPointtoScreenPoint(g_epCanvasOrigin);     //屏幕零点坐标
                //添加零点
                p_lsppath3.Add(Screenpointx0y0);
                p_lsppath2.Add(Screenpointx0y0);
                p_lsppath1.Add(Screenpointx0y0);

                //填充路径
                FillPathColor(p_lsppath3, Color.DarkOrange);
                FillPathColor(p_lsppath2, Color.OrangeRed);
                FillPathColor(p_lsppath1, Color.Red);
                //绘制路径
                DrawPathAndVertex(p_lsppath3, 3);
                DrawPathAndVertex(p_lsppath2, 2);
                DrawPathAndVertex(p_lsppath1, 1);

                p_lsppath3.RemoveAt(p_lsppath3.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
                p_lsppath2.RemoveAt(p_lsppath2.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
                p_lsppath1.RemoveAt(p_lsppath1.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
            }
            else if (p_n32bankno > 4 && p_n32bankno < 9)
            {
                List<AngleAndRadius> l_larppath = ScreenPointListToPolarPointList(p_lsppath3);  //极坐标点 提取角度和长度
                double l_n64startangle = l_larppath[0].ANG;
                double l_n64endangle = l_larppath[l_larppath.Count - 1].ANG;
                double l_n32sweepangle = l_n64endangle - l_n64startangle;
                if (l_n64endangle >= 360)
                {
                    l_n64endangle -= 360;
                }
                int l_n32radius = (int)l_larppath[1].RAD;
                Point l_spstartpoit1 = new Point(l_n32radius, l_n32radius);
                Point l_spstartpoit2 = new Point(-l_n32radius, l_n32radius);
                Point l_spstartpoit3 = new Point(-l_n32radius, -l_n32radius);
                Point l_spstartpoit4 = new Point(l_n32radius, -l_n32radius);
                List<Point> l_lsppoint = new List<Point>();
                l_lsppoint.Add(l_spstartpoit1);
                l_lsppoint.Add(l_spstartpoit2);
                l_lsppoint.Add(l_spstartpoit3);
                l_lsppoint.Add(l_spstartpoit4);
                List<Point> l_leppath = ScreenPointtoPixelPoint(l_lsppoint);              //像素点集合（作图时使用）
                Rectangle recfield = new Rectangle(l_leppath[1].X, l_leppath[1].Y, l_leppath[0].X - l_leppath[1].X, l_leppath[0].X - l_leppath[1].X);
                Brush brush = new SolidBrush(Color.FromArgb(170, Color.DarkOrange));
                graphics.FillPie(brush, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);
                Pen l_pen = new Pen(Color.Black);
                graphics.DrawPie(l_pen, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);

                //field2
                l_larppath = ScreenPointListToPolarPointList(p_lsppath2);  //极坐标点 提取角度和长度
                l_n32radius = (int)l_larppath[1].RAD;
                l_spstartpoit1 = new Point(l_n32radius, l_n32radius);
                l_spstartpoit2 = new Point(-l_n32radius, l_n32radius);
                l_spstartpoit3 = new Point(-l_n32radius, -l_n32radius);
                l_spstartpoit4 = new Point(l_n32radius, -l_n32radius);
                l_lsppoint.Clear();
                l_lsppoint.Add(l_spstartpoit1);
                l_lsppoint.Add(l_spstartpoit2);
                l_lsppoint.Add(l_spstartpoit3);
                l_lsppoint.Add(l_spstartpoit4);
                l_leppath = ScreenPointtoPixelPoint(l_lsppoint);              //像素点集合（作图时使用）
                recfield = new Rectangle(l_leppath[1].X, l_leppath[1].Y, l_leppath[0].X - l_leppath[1].X, l_leppath[0].X - l_leppath[1].X);
                brush = new SolidBrush(Color.FromArgb(170, Color.OrangeRed));
                graphics.FillPie(brush, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);
                graphics.DrawPie(l_pen, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);

                //field1
                l_larppath = ScreenPointListToPolarPointList(p_lsppath1);  //极坐标点 提取角度和长度
                l_n32radius = (int)l_larppath[1].RAD;
                l_spstartpoit1 = new Point(l_n32radius, l_n32radius);
                l_spstartpoit2 = new Point(-l_n32radius, l_n32radius);
                l_spstartpoit3 = new Point(-l_n32radius, -l_n32radius);
                l_spstartpoit4 = new Point(l_n32radius, -l_n32radius);
                l_lsppoint.Clear();
                l_lsppoint.Add(l_spstartpoit1);
                l_lsppoint.Add(l_spstartpoit2);
                l_lsppoint.Add(l_spstartpoit3);
                l_lsppoint.Add(l_spstartpoit4);
                l_leppath = ScreenPointtoPixelPoint(l_lsppoint);              //像素点集合（作图时使用）
                recfield = new Rectangle(l_leppath[1].X, l_leppath[1].Y, l_leppath[0].X - l_leppath[1].X, l_leppath[0].X - l_leppath[1].X);
                brush = new SolidBrush(Color.FromArgb(170, Color.Red));
                graphics.FillPie(brush, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);
                graphics.DrawPie(l_pen, recfield, 360.0f - (float)l_n64endangle, (float)l_n32sweepangle);

                //绘制路径
                DrawPiePathAndVertex(p_lsppath3, 3);
                DrawPiePathAndVertex(p_lsppath2, 2);
                DrawPiePathAndVertex(p_lsppath1, 1);

            }
            else
            {
                Point Screenpointx0y0 = PixelPointtoScreenPoint(g_epCanvasOrigin);     //屏幕零点坐标
                //添加零点
                p_lsppath3.Add(Screenpointx0y0);
                p_lsppath2.Add(Screenpointx0y0);
                p_lsppath1.Add(Screenpointx0y0);
                //填充路径
                FillPathColor(p_lsppath3, Color.DarkOrange);
                FillPathColor(p_lsppath2, Color.OrangeRed);
                FillPathColor(p_lsppath1, Color.Red);

                List<Point> l_lsppath3 = new List<Point>();
                List<Point> l_lsppath2 = new List<Point>();
                List<Point> l_lsppath1 = new List<Point>();

                l_lsppath3.Add(new Point(p_lsppath3[0].X, p_lsppath3[1].Y/2));
                l_lsppath3.Add(new Point(0, p_lsppath3[1].Y));
                l_lsppath3.Add(new Point(p_lsppath3[2].X, p_lsppath3[1].Y / 2));

                l_lsppath2.Add(new Point(p_lsppath2[0].X, p_lsppath2[1].Y / 2));
                l_lsppath2.Add(new Point(0, p_lsppath2[1].Y));
                l_lsppath2.Add(new Point(p_lsppath2[2].X, p_lsppath2[1].Y / 2));

                l_lsppath1.Add(new Point(p_lsppath1[0].X, p_lsppath1[1].Y / 2));
                l_lsppath1.Add(new Point(0, p_lsppath1[1].Y));
                l_lsppath1.Add(new Point(p_lsppath1[2].X, p_lsppath1[1].Y / 2));
                //绘制路径
                DrawBank9_16PathAndVertex(p_lsppath3, l_lsppath3);
                DrawBank9_16PathAndVertex(p_lsppath2, l_lsppath2);
                DrawBank9_16PathAndVertex(p_lsppath1, l_lsppath1);

                p_lsppath3.RemoveAt(p_lsppath3.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
                p_lsppath2.RemoveAt(p_lsppath2.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
                p_lsppath1.RemoveAt(p_lsppath1.Count - 1);                               //删除屏幕零点坐标防止下次使用时出现（0,0）
            }
        }

        #endregion


    }
}
