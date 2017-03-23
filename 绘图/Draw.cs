using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SimuProteus
{
    public class Draw
    {
        private static Color colorFoot = Color.Black;
        private static float widthFoot = 5f, widthLine = 3f;

        private static int pointColorNone = int.Parse(Ini.GetItemValue("colorInfo", "colorNONE"));
        private static int pointColorVCC = int.Parse(Ini.GetItemValue("colorInfo", "colorVCC"));
        private static int pointColorGND = int.Parse(Ini.GetItemValue("colorInfo", "colorGND"));
        private static int pointSize = int.Parse(Ini.GetItemValue("sizeInfo", "pixelNetPoint"));
        private static int pointRadius = pointSize / 2;

        /// <summary>
        /// 绘制元器件的管脚
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void LineFoot(Control board, int x, int y, int ox, int oy)
        {
            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(colorFoot, widthFoot);
            g.DrawLine(pen, new Point(x, y), new Point(ox, oy));
        }

        /// <summary>
        /// 连接两个元器件
        /// </summary>
        /// <param name="board"></param>
        /// <param name="isXFirst"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        public static void LineBetweenElement(Control board, bool isXFirst, int x, int y, int ox, int oy)
        {
            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(Color.Black, widthLine);
            if (isXFirst)
            {
                g.DrawLine(pen, new Point(x, y), new Point(ox, y));
                g.DrawLine(pen, new Point(ox, y), new Point(ox, oy));
            }
            else
            {
                g.DrawLine(pen, new Point(x, y), new Point(x, oy));
                g.DrawLine(pen, new Point(x, oy), new Point(ox, oy));
            }
        }

        public static void DrawArrawLine(Control board, Point start, Point end,Color color)
        {
            Graphics g = board.CreateGraphics();

            Pen pen = new Pen(color, 1);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;//恢复实线  
            pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;//定义线尾的样式为箭头  
            g.DrawLine(pen,start,end);  
        }

        public static void DrawSolidLine(Control board, Point start, Point end, bool isDemo)
        {
            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(Color.Blue, widthLine);

            if (isDemo)
            {
                pen.Color = Color.Black;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            }

            start.X += pointRadius;
            start.Y += pointRadius;
            end.X += pointRadius;
            end.Y += pointRadius;

            g.DrawLine(pen, start, end);
        }

        public static void DrawSolidLine(Control board, Point start, Point end, Color color)
        {
            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(color, widthLine);

            start.X += pointRadius;
            start.Y += pointRadius;
            end.X += pointRadius;
            end.Y += pointRadius;

            g.DrawLine(pen, start, end);
        }

        /// <summary>
        /// 画实心圆
        /// </summary>
        /// <param name="board"></param>
        /// <param name="point"></param>
        /// <param name="originLU"></param>
        public static void DrawSolidCircle(Control board, enumNetPointType point, int originLUX, int originLUY)
        {
            int colorPoint = -1;
            switch (point)
            {
                case enumNetPointType.GND: colorPoint = pointColorGND; break;
                case enumNetPointType.VCC: colorPoint = pointColorVCC; break;
                default: colorPoint = pointColorNone; break;
            }

            DrawSolidCircle(board, colorPoint, new Point(originLUX+pointRadius,originLUY+pointRadius), pointRadius);
        }

        /// <summary>
        /// 画实心圆
        /// </summary>
        /// <param name="board"></param>
        /// <param name="color"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public static void DrawSolidCircle(Control board, int color, Point center, int radius)
        {
            Color colorItem = Color.FromArgb(color);
            Rectangle rect = new Rectangle(center.X - radius, center.Y - radius, radius * 2 , radius * 2);
            
            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(colorItem);
            g.DrawEllipse(pen, rect);
            Brush brush = new SolidBrush(colorItem);
            g.FillEllipse(brush, rect);
        }

        /// <summary>
        /// 画实心矩形
        /// </summary>
        /// <param name="board"></param>
        /// <param name="luCoor"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawSolidRect(Control board, Point luCoor, Size size,Color color)
        {
            Rectangle rect = new Rectangle(luCoor, size);

            Graphics g = board.CreateGraphics();
            Pen pen = new Pen(color);
            g.DrawEllipse(pen, rect);
            Brush brush = new SolidBrush(color);
            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// 写字
        /// </summary>
        /// <param name="board"></param>
        /// <param name="color"></param>
        /// <param name="location"></param>
        /// <param name="words"></param>
        public static void DrawWords(Control board, Color color,PointF location, string words)
        {
            Graphics g = board.CreateGraphics();
            Brush brush = new SolidBrush(color);
            Font font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

            g.DrawString(words, font, brush, location);
        }

        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="newW"></param>
        /// <param name="newH"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image bmp, int newW, int newH)
        {
            try
            {
                Bitmap bap = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(bap);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return bap;
            }
            catch
            {
                return null;
            }
        }
    }
}
