using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AutoTestController.Client
{
    [ToolboxBitmap(typeof(ProgressBar))]
    class ProgressBarEx: ProgressBar
    {
        int x = 2;
        int y = 1;
        //Color myColor;
        public ProgressBarEx()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush brush = null;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            if (ProgressBarRenderer.IsSupported) ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rect);
            Pen pen = new Pen(this.BackColor, 1);
            //绘制边框
            e.Graphics.DrawRectangle(pen, rect);
            //绘制背景
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 2, 2, rect.Width - 4, rect.Height - 4);
            rect.Height -= 4;
            rect.Width = (int)(rect.Width * ((double)Value / Maximum)) - 4;
            brush = new SolidBrush(this.ForeColor);
            //绘制当前进度
            //myColor = this.ForeColor;
            //e.Graphics.FillRectangle(new SolidBrush(myColor), x, y, rect.Width - 1, rect.Height);
            e.Graphics.FillRectangle(brush, x, y, rect.Width - 1, rect.Height);
            string text = string.Format("{0}% Complete...", Value * 100 / Maximum);
            using (var font = new Font(FontFamily.GenericSansSerif, 10))
            {
                Graphics g = e.Graphics;
                SizeF sz = g.MeasureString(text, font);
                var location = new PointF(this.Width / 2 - 15, 1);
                g.DrawString(text, font, Brushes.SteelBlue, location);
            }
        }
    }
}
