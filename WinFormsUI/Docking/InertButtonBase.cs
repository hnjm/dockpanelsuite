using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace WeifenLuo.WinFormsUI.Docking
{
    public abstract class InertButtonBase : Control
    {
        protected InertButtonBase()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
        }

        public abstract Bitmap Image
        {
            get;
        }

        private bool m_isMouseOver = false;
        protected bool IsMouseOver
        {
            get { return m_isMouseOver; }
            private set
            {
                if (m_isMouseOver == value)
                    return;

                m_isMouseOver = value;
                Invalidate();
            }
        }

        private bool m_isMouseDown = false;
        protected bool IsMouseDown
        {
            get { return m_isMouseDown; }
            private set
            {
                if (m_isMouseDown == value)
                    return;

                m_isMouseDown = value;
                Invalidate();
            }
        }

        protected override Size DefaultSize
        {
            get { return Resources.DockPane_Close.Size; }
        }

        protected virtual Color HoverBackColor
        {
            get { return Color.Transparent; }
        }

        protected virtual Color HoverBorderColor
        {
            get { return ForeColor; }
        }

        protected virtual Color HoverForeColor
        {
            get { return ForeColor; }
        }

        protected virtual Color PressedBackColor
        {
            get { return Color.Transparent; }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool over = ClientRectangle.Contains(e.X, e.Y);
            if (IsMouseOver != over)
                IsMouseOver = over;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsMouseOver)
                IsMouseOver = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsMouseOver)
                IsMouseOver = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!IsMouseDown)
                IsMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsMouseDown)
                IsMouseDown = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsMouseOver && Enabled)
            {
                using (Pen pen = new Pen(this.HoverBorderColor)) 
                using (Brush brush = new SolidBrush(IsMouseDown ? PressedBackColor : HoverBackColor))
                {
                    Rectangle rect = ClientRectangle;
                    rect.Width--;
                    rect.Height--;
                    e.Graphics.FillRectangle(brush, rect);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }

            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                Color color = IsMouseOver ? HoverForeColor : ForeColor;
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix40 = color.R / 255f;
                matrix.Matrix41 = color.G / 255f;
                matrix.Matrix42 = color.B / 255f;

                imageAttributes.SetColorMatrix(matrix);

                e.Graphics.DrawImage(
                   Image,
                   new Rectangle(
                       (ClientRectangle.Width - Image.Width) / 2,
                       (ClientRectangle.Height - Image.Height) / 2,
                       Image.Width, Image.Height),
                   0, 0,
                   Image.Width,
                   Image.Height,
                   GraphicsUnit.Pixel,
                   imageAttributes);
            }

            base.OnPaint(e);
        }

        public void RefreshChanges()
        {
            if (IsDisposed)
                return;

            bool mouseOver = ClientRectangle.Contains(PointToClient(Control.MousePosition));
            if (mouseOver != IsMouseOver)
                IsMouseOver = mouseOver;

            OnRefreshChanges();
        }

        protected virtual void OnRefreshChanges()
        {
        }
    }
}
