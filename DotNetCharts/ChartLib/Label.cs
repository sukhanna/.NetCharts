using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChartLib
{
  /// <summary>
  /// This class is basically meant for drawing the <see cref="RenderView"/> title,
  /// axis labels and labels associated with legends. 
  /// </summary>
  public class Label : IDisposable, ICloneable
  {
    /// <summary>
    /// Track if the Dispose has been called
    /// </summary>
    private bool m_disposed;

    /// <summary>
    /// 
    /// </summary>
    private SizeF m_margin;

    /// <summary>
    /// 
    /// </summary>
    public Label()
    {
      m_labelFormat.Alignment = StringAlignment.Center;
    }

    public Label(string labelText) : 
      this()
    {
      m_labelText = labelText;
    }

    public Label(string labelText, SizeF margin) : 
      this()
    {
      m_labelText = labelText;
      m_margin = margin;
    }

    #region IDispose
    private void Dispose(bool disposing)
    {
      // Check to see if Dispose has already been called.
      if (m_disposed) return;
      // If disposing equals true, dispose all managed
      // and unmanaged resources.
      if (disposing)
      {
        // Dispose managed resources.
        m_labelFormat.Dispose();
        m_labelFont.Dispose();
      }
      // Note disposing has been done.
      m_disposed = true;
    }

    /// <summary>
    /// Implement IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      // This object will be cleaned up by the Dispose method.
      // Therefore, you should call GC.SupressFinalize to
      // take this object off the finalization queue
      // and prevent finalization code for this object
      // from executing a second time.
      GC.SuppressFinalize(this);
    }
    #endregion

    #region IClone
    public object Clone()
    {
      Label label = new Label();
      DoClone(this, label);
      return label;
    }

    protected static void DoClone(Label fromLabel, Label toLabel)
    {
      // value items
      toLabel.m_alignLabel = fromLabel.m_alignLabel;
      toLabel.m_labelOffset = fromLabel.m_labelOffset;

      // refrence items
      toLabel.m_labelFont = (Font)fromLabel.m_labelFont.Clone();
      toLabel.m_labelFormat = (StringFormat)fromLabel.m_labelFormat.Clone();
      toLabel.m_labelText = (string)fromLabel.m_labelText.Clone();
    }
    #endregion

    #region Label

    /// <summary>
    /// Draw the label.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="physicalBounds"></param>
    internal void Draw(Graphics g, RectangleF physicalBounds)
    {
      if (g == null)
        return;

      if (String.IsNullOrEmpty(m_labelText)) return;
      using (SolidBrush labelBrush = new SolidBrush(Color))
      {
        PointF labelPos = new PointF(0, 0);
        // Draw Labels aligned to defined edge or custom align
        switch (m_alignLabel)
        {
          case AlignLabel.TopEdgeCenter:
            labelPos.X = (physicalBounds.Left + physicalBounds.Right) / 2.0f + m_labelOffset.X;
            labelPos.Y = (physicalBounds.Top / 2.0f - m_labelFont.Height / 2) - m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.BottomEdgeCenter:
            labelPos.X = (physicalBounds.Left + physicalBounds.Right) / 2.0f + m_labelOffset.X;
            labelPos.Y = physicalBounds.Bottom + m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.LeftEdgeCenter:
            labelPos = new PointF(0, 0);
            labelPos.X = physicalBounds.Left - m_labelOffset.X;
            labelPos.Y = (physicalBounds.Bottom + physicalBounds.Top) / 2.0f + m_labelOffset.Y;
            g.TranslateTransform(labelPos.X, labelPos.Y);
            g.RotateTransform(-90.0f);
            g.TranslateTransform(-labelPos.X, -labelPos.Y);
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            g.ResetTransform();
            break;
          case AlignLabel.RightEdgeCenter:
            labelPos.X = physicalBounds.Right + m_labelOffset.X;
            labelPos.Y = (physicalBounds.Bottom + physicalBounds.Top) / 2.0f + m_labelOffset.Y;
            g.TranslateTransform(labelPos.X, labelPos.Y);
            g.RotateTransform(-90.0f);
            g.TranslateTransform(-labelPos.X, -labelPos.Y);
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            g.ResetTransform();
            break;
          case AlignLabel.BottomRight:
            labelPos.X = physicalBounds.Right;
            labelPos.Y = physicalBounds.Bottom + m_labelOffset.Y - m_labelFont.Height - 5.0f;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.TopRight:
            labelPos.X = physicalBounds.Right + m_labelOffset.X;
            labelPos.Y = physicalBounds.Top + m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.BottomLeft:
            labelPos.X = physicalBounds.Left + m_labelOffset.X;
            labelPos.Y = physicalBounds.Bottom + m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.TopLeft:
            labelPos.X = physicalBounds.Left + m_labelOffset.X;
            labelPos.Y = physicalBounds.Top - m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          case AlignLabel.None:
            labelPos.X = m_labelOffset.X;
            labelPos.Y = m_labelOffset.Y;
            g.DrawString(m_labelText, m_labelFont, labelBrush, labelPos, m_labelFormat);
            break;
          default:
            break;
        }
      }
    }

    private string m_labelText = "";
    public string LabelText
    {
      get { return m_labelText; }
      set { m_labelText = value; }
    }

    private Font m_labelFont = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
    public Font LabelFont
    {
      get { return m_labelFont; }
      set { m_labelFont = value; }
    }

    private StringFormat m_labelFormat = new StringFormat();
    public StringFormat LabelFormat
    {
      get { return m_labelFormat; }
      set { m_labelFormat = value; }
    }

    private PointF m_labelOffset = new PointF(0, 0);
    public PointF LabelOffset
    {
      get { return m_labelOffset; }
      set { m_labelOffset = value; }
    }

    private AlignLabel m_alignLabel = AlignLabel.TopEdgeCenter;
    public AlignLabel AlignLabels
    {
      get { return m_alignLabel; }
      set { m_alignLabel = value; }
    }

    public Color Color { get; set; }

    private Region m_lastInvalidatedRegion;
    /// <summary>
    /// need to re validate the region which was invalidated before
    /// </summary>
    public Region LastInvalidatedRegion
    {
      get { return m_lastInvalidatedRegion; }
      set
      {
        if (m_lastInvalidatedRegion == null || value == null)
          m_lastInvalidatedRegion = value;
        else
          m_lastInvalidatedRegion.Union(value);
      }
    }

    #endregion
  }
}
