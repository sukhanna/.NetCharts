using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace ChartLib
{
  public class RenderView : Panel
  {
    /// <summary>
    /// Screen Rectangle or physical bounds of screen
    /// </summary>
    private RectangleF m_physicalBounds;

    private LogicalView m_logicalView;

    private Bitmap m_bitmap;

    #region private api's

    private void DisposeBitmap()
    {
      if (m_bitmap != null)
      {
        m_bitmap.Dispose();
        m_bitmap = null;
      }
    }

    private void Draw(Graphics g, RectangleF physicalBounds)
    {
      if (m_bitmap == null)
      {
        m_bitmap = new Bitmap((int)physicalBounds.Width, (int)physicalBounds.Height);

        using (Graphics offscreenGraphics = Graphics.FromImage(m_bitmap))
        {
          // Fill in the background.
          if (BackColor != null)
          {
            offscreenGraphics.FillRectangle(
               new SolidBrush((Color)BackColor), physicalBounds);
          }
          else if (BackgroundImage != null)
          {
            offscreenGraphics.DrawImageUnscaled(ChartUtility.FitToBounds(BackgroundImage, physicalBounds), 0, 0);
          }

          // draw drawable objects from collection object
          offscreenGraphics.SmoothingMode = SmoothingMode.None;

          // Draw title
          if (Label != null)
          {
            Label.Draw(offscreenGraphics, physicalBounds);
          }

          //CalculateLayout(m_screenRectangle);

          // draw all the rendering items depending on their z-Index
          foreach (IDrawable drawable in ViewModel.DrawableObjects)
            drawable.Draw(offscreenGraphics, physicalBounds);
        }
      }
      // draw offscreen graphicsto actual device context.
      g.DrawImageUnscaled(m_bitmap, 0, 0);
    }
    #endregion

    #region protected override api's

    /// <summary>
    /// called on size change of surface panel. Dispose bitmap every time
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSizeChanged(EventArgs e)
    {
      // Calculate the main drawing region, axis markers will be drawn
      // outside of this.Reduce the bounds to accommodate the margins
      m_physicalBounds = new RectangleF(Margin.Left,
         Margin.Top,
         Math.Abs(Right - Margin.Right - Margin.Left),
         Math.Abs(Bottom - Margin.Bottom - Margin.Top));

      if (m_bitmap != null)
      {
        DisposeBitmap();
        Invalidate();
      }
      base.OnSizeChanged(e);
    }

    /// <summary>
    /// Calls DoMouseAction
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
    }

    /// <summary>
    /// Calls DoMouseAction
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
    }

    /// <summary>
    /// Calls DoMouseAction
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
    }

    /// <summary>
    /// gets called on Invalidate
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(PaintEventArgs e)
    {
      // Reduce the bounds to accommodate the margins
      // This statement is required here, otherwise setup tool gets screwed up,
      // This should be removed later on will look into it. OnSize is the ideal place
      // for it.
      RectangleF physicalBounds = new RectangleF(
        Margin.Left,
        Margin.Top,
        Math.Abs(Width - Margin.Right - Margin.Left),
        Math.Abs(Height - Margin.Bottom - Margin.Top));

      Draw(e.Graphics, physicalBounds);

      base.OnPaint(e);
    }

    #endregion

    #region public API's

    public RenderView()
    {
      ViewModel = new RenderViewModel();
      m_logicalView = new LogicalView();

      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.ResizeRedraw, true);
      SetStyle(ControlStyles.Opaque, true);
    }

    internal RenderViewModel ViewModel { get;  private set; }

    public void Add(IDrawable drawableObject)
    {
      ViewModel.Add(drawableObject);
    }

    public void Add(IDrawable drawableObject, int zIndex)
    {
      ViewModel.Add(drawableObject, zIndex);
    }

    public void Remove(IDrawable drawableObject)
    {
      ViewModel.Remove(drawableObject);
    }

    public Label Label { get; set; }
    #endregion
  }
}
