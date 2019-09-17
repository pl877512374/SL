using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fzjgld
{
    class PointClass
    {
        public double x = 0;
        public double y = 0;
        public PointClass()
        {
            x = 0; y = 0;
        }
        //-------写一个排序函数，使得输入的点按顺序排列，是因为插值算法的要求是，x轴递增有序的---------  
        public static PointClass[] DeSortX(PointClass[] points)
        {
            int length = points.Length;
            double temx, temy;
            for (int i = 0; i < length - 1; i++)
            {
                for (int j = 0; j < length - i - 1; j++)
                    if (points[j].x > points[j + 1].x)
                    {

                        temx = points[j + 1].x;
                        points[j + 1].x = points[j].x;
                        points[j].x = temx;
                        temy = points[j + 1].y;
                        points[j + 1].y = points[j].y;
                        points[j].y = temy;
                    }
            }
            return points;
        }
    }
}
