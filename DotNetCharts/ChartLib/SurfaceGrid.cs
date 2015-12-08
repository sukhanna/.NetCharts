using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Drawing;


namespace ChartLib
{
  internal struct GridPoints
  {
    public GridPoints(PointF startPoint, PointF endPoint)
    {
      m_startPoint = startPoint;
      m_endPoint = endPoint;
    }

    private readonly PointF m_startPoint;

    public PointF StartPoint
    {
      get { return m_startPoint; }
    }

    private readonly PointF m_endPoint;

    public PointF EndPoint
    {
      get { return m_endPoint; }
    }
  }

  /// <summary>
  /// This class must be used by the GraphSurface object in order to draw the coarse
  /// and fine grids at major and minor divisions, which can be logarithmic or 
  /// linear depending upon the axis scale.
  /// </summary>
  public class SurfaceGrid : IDisposable
  {
    /// <summary>
    /// Track if the Dispose has been called
    /// </summary>
    private bool m_disposed;

    /// <summary>
    /// Enum defining the grid type drawn under graph surface.
    /// </summary>
    public enum GridType
    {
      /// <summary>
      /// No grid.
      /// </summary>
      None = 0,
      /// <summary>
      /// Coarse grid. Lines at large tick positions only.
      /// </summary>
      Coarse = 1,
      /// <summary>
      /// Fine grid. Lines at both large and small tick positions.
      /// </summary>
      Fine = 2,
      /// <summary>
      /// Equi space. Equi space grid lines in x - y direction.
      /// </summary>
      EquiSpace = 3
    }

    private void Dispose(bool disposing)
    {
      // Check to see if Dispose has already been called.
      if (m_disposed) return;
      // If disposing equals true, dispose all managed
      // and unmanaged resources.
      if (disposing)
      {
        // Dispose managed resources.
        m_majorGridPen.Dispose();
        m_minorGridPen.Dispose();
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

    /// <summary>
    /// default constructor
    /// </summary>
    public SurfaceGrid()
    {
      InitSurfaceGrid();
      m_majorGridCollection = new Collection<GridPoints>();
      m_minorGridCollection = new Collection<GridPoints>();
    }

    private void InitSurfaceGrid()
    {
      float[] pattern = { 1.0f, 0.5f };
      m_minorGridPen.DashPattern = pattern;
    }

    private GridType m_gridType = GridType.Coarse;

    /// <summary>
    /// Specifies the horizontal grid type (none, coarse or fine).
    /// </summary>
    public GridType SurfaceGridType
    {
      get { return m_gridType; }
      set { m_gridType = value; }
    }

    /// <summary>
    /// The pen used to draw major (coarse) grid lines.
    /// </summary>
    private Pen m_majorGridPen = new Pen(Color.Gray);

    public Pen MajorGridPen
    {
      get { return m_majorGridPen; }
      set { m_majorGridPen = value; }
    }

    /// <summary>
    /// The pen used to draw minor (fine) grid lines.
    /// </summary>
    private Pen m_minorGridPen = new Pen(Color.Gray);

    public Pen MinorGridPen
    {
      get { return m_minorGridPen; }
      set { m_minorGridPen = value; }
    }

    public Color GridColor
    {
      get { return m_majorGridPen.Color; }
      set
      {
        m_minorGridPen.Color = value;
        m_majorGridPen.Color = value;
      }
    }

    /// <summary>
    /// Draw major/minor grid lines
    /// </summary>
    /// <param name="g">draw on this graphics context</param>
    /// <param name="division">collection of Major/Minor divisions</param>
    /// <param name="p">pen to draw Major/Minor grid lines</param>
    private static void DrawGridLines(Graphics g, Collection<GridPoints> division, Pen p)
    {
      for (int i = 0; i < division.Count; ++i)
      {
        g.DrawLine(p, division[i].StartPoint, division[i].EndPoint);
      }
    }

    /// <summary>
    /// Draws the grid
    /// </summary>
    /// <param name="g">The graphics surface on which to draw</param>
    public void Draw(Graphics g)
    {
      // hide grid lines
      if (!m_bShowGrid)
        return;

      switch (SurfaceGridType)
      {
        case GridType.EquiSpace:
          // Drawing both horizontal and vertical EquiSpace lines
          DrawGridLines(g, m_majorGridCollection, MinorGridPen);
          DrawGridLines(g, m_minorGridCollection, MajorGridPen);
          break;
        case GridType.Coarse:
          DrawGridLines(g, m_majorGridCollection, MajorGridPen);
          break;
        case GridType.Fine:
          DrawGridLines(g, m_majorGridCollection, MajorGridPen);
          DrawGridLines(g, m_minorGridCollection, MinorGridPen);
          break;
        default:
          break;
      }
    }

    private bool m_bShowGrid = true;

    /// <summary>
    /// hide grid lines
    /// </summary>
    public bool ShowGrid
    {
      get { return m_bShowGrid; }
      set { m_bShowGrid = value; }
    }

    private Collection<GridPoints> m_majorGridCollection;

    internal Collection<GridPoints> MajorGridCollection
    {
      get { return m_majorGridCollection; }
    }

    private Collection<GridPoints> m_minorGridCollection;

    internal Collection<GridPoints> MinorGridCollection
    {
      get { return m_minorGridCollection; }
    }

    public void Clear()
    {
      m_minorGridCollection.Clear();
      m_majorGridCollection.Clear();
    }
  }
}
