using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace ChartLib
{
  public abstract class Axis
  {
    /// <summary>
    /// Blend will give shaded gradient effect for tick marks merging into grid lines
    /// </summary>
    private Blend m_blend;

    protected Axis()
    {
      SurfaceGrid = new SurfaceGrid();
    }

    #region IDrawable
    internal virtual void Draw(Graphics g, RectangleF physicalBounds, RectangleF logicalBounds)
    {
      if (Hidden) return;
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="bMajorGrid"></param>
    protected void DrawTickMark(Graphics g, Color color, PointF startPoint, PointF endPoint)
    {
      if (g == null)
      {
        throw new ArgumentNullException("g");
      }

      using (LinearGradientBrush brush = new LinearGradientBrush(startPoint, endPoint, color, Color.Gray))
      {
        using (Pen pen = new Pen(color))
        {
          pen.Brush = brush;
          g.DrawLine(pen, startPoint, endPoint);
        }
      }
    }

    /// <summary>
    /// Select blend pattern to identify ticks on Major/Minor grid lines
    /// </summary>
    /// <returns>blended pattern</returns>
    protected Blend Blend
    {
      get
      {
        if (m_blend == null)
        {
          m_blend = new Blend();

          // Set the values in the Factors array to be black fading into gray and then go to black
          // this is useful in showing identifying ticks on grid lines

          // percentage of color factor to be used. 0.4 indicates that at the specified position, the blended color 
          // is composed of 40 percent of the ending gradient color and 60 percent of the starting gradient color
          m_blend.Factors = new float[]
                               {
                                  0.0f, 0.10f, 0.20f, 0.30f, 0.40f, 0.50f, 0.60f, 0.65f, 0.70f, 0.75f, 0.80f, 0.90f, 0.95f , 1.0f
                               };

          // Set the positions.
          //0.2f specifies that this point is 20 percent of the total distance from the starting point.
          // The elements in this array are represented by float values between 0.0f and 1.0f.
          // The first element of the array must be 0.0f, and the last element must be 1.0f
          m_blend.Positions = new float[]
                                 {
                                    0.0f, 0.10f, 0.20f, 0.30f, 0.40f, 0.50f, 0.60f, 0.65f, 0.70f, 0.75f, 0.80f, 0.90f, 0.95f, 1.0f
                                 };
        }
        return m_blend;
      }
    }

    /// <summary>
    /// vertical grid lines on x axis
    /// </summary>
    protected SurfaceGrid SurfaceGrid { get; private set; }

    protected void Initialize()
    {
      TickTextFont = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
      Color = Color.Black;
      TickTextColor = Color.Black;
    }

    #region Axis

    /// <summary>
    /// Property exposing the Axis Label
    /// </summary>
    public Label Label { get; set; }

    /// <summary>
    /// If set to true, the axis is hidden. i.e the axis line, ticks, tick 
    /// labels and axis label will not be drawn. 
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// If this property is set to true, no text will be drawn next to any axis tick marks.
    /// </summary>
    public bool HideTickText { get; set; }

    /// <summary>
    /// This property exposes the font to be used for the drawing of text next 
    /// to the axis tick marks.
    /// </summary>
    public Font TickTextFont { get; set; }

    private string m_numberFormat = "F";
    /// <summary>
    /// This property can be used to specify the format specifier for string 
    /// </summary>
    public string NumberFormat
    {
      get { return m_numberFormat; }
      set { m_numberFormat = value; }
    }

    /// <summary>
    /// The color of the brush used to draw the axis tick labels.
    /// </summary>
    public Color TickTextColor { get; set; }

    /// <summary>
    /// Property exposed for the color of the pen used to draw the ticks and the axis line.
    /// </summary>
    public Color Color { get; set; }

    public AxisScale AxisScale { get; protected set; }

    #endregion
  }
}
