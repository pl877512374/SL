using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace fzjgld
{
    class AngleAndRadius
    {
        private double _ang;
        public double ANG
        {
            get { return _ang; }
            set { _ang = value; }
        }
        private double _rad;
        public double RAD
        {
            get { return _rad; }
            set { _rad = value; }
        }
        public AngleAndRadius(double ang, double rad)
        {
            this.ANG = ang;
            this.RAD = rad;
        }

        #region 颜色：RGB转成16禁止
        public static string ColorRGBtoHEX16(Color color)
        {
            if (color.IsEmpty)
                return "#000000";
            return System.Drawing.ColorTranslator.ToHtml(color);
        }
        #endregion
    }
}
