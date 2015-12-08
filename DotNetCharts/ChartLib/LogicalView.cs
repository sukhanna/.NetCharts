using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ChartLib
{
  internal class LogicalView
  {
    internal Bitmap m_bitmap;

    internal LogicalView()
    {
    }

    internal void Draw(Graphics g, RectangleF physicalBounds, IReadOnlyCollection<IDrawable> drawableObjects)
    {
      if (g == null)
        throw new ArgumentNullException("g");

      if (m_bitmap == null)
      {
        m_bitmap = new Bitmap((int)physicalBounds.Width, (int)physicalBounds.Height);

        using (Graphics offscreenGraphics = Graphics.FromImage(m_bitmap))
        {
          //offscreenGraphics.FillRectangle(
          //     new SolidBrush(Color.White), physicalBounds);

          // Fill in the background.
          //if (m_surfaceBackColor != null)
          //{
          //  offscreenGraphics.FillRectangle(
          //     new SolidBrush((Color)m_surfaceBackColor), boundingRect);
          //}
          //else if (m_surfaceBackImage != null)
          //{
          //  offscreenGraphics.DrawImageUnscaled(GraphicsUtils.FitToBounds(m_surfaceBackImage, boundingRect), 0, 0);
          //}

          // draw drawable objects from collection object
          offscreenGraphics.SmoothingMode = SmoothingMode.None;

          // Draw title
          //if (m_labels != null)
          //{
          //  m_labels.Draw(offscreenGraphics, m_screenRectangle);
          //}

          //CalculateLayout(m_screenRectangle);

          // draw all the rendering items depending on their z-Index
          foreach (IDrawable drawable in drawableObjects)
            drawable.Draw(g, physicalBounds);

          // Draw Period Label
          //if (m_periodLabel != null)
          //{
          //  m_periodLabel.Draw(offscreenGraphics, m_screenRectangle);
          //}
        }
      }
      // draw offscreen graphicsto actual device context.
      g.DrawImageUnscaled(m_bitmap, 0, 0);
    }

    /// <summary>
    /// New bitmap should be formed on resize and zoom of SurfacePanel
    /// </summary>
    internal void DisposeBitmap()
    {
      if (m_bitmap != null)
      {
        m_bitmap.Dispose();
        m_bitmap = null;
      }
    }
  }
}
