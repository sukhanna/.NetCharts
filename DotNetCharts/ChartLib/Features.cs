using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChartLib
{
  internal class Features
  {
    private readonly Stack<float> m_zoomStack = new Stack<float>();
    private RectangleF m_logicalBounds;
    private List<float> m_zoomLevels = new List<float>() { 100, 125, 150, 200, 400, 800, 1600, 3200, 6400 };

    Features() : this(RectangleF.Empty) { }

    Features(RectangleF logicalBounds)
    {
      m_logicalBounds = logicalBounds;
    }

    /// <summary>
    /// current measurement data bounds after zoom
    /// </summary>
    public RectangleF ZoomBounds
    {
      get
      {
        return RectangleF.Empty;
      }
    }

    public void Push(float percentageZoomLevel)
    {
      m_zoomStack.Push(percentageZoomLevel);
    }

    public float Pop(float percentageZoomLevel)
    {
      return m_zoomStack.Pop();
    }

    /// <summary>
    /// get index property for zoom levels
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float this[int index]
    {
      get
      {
        float zoomLevel = 100;
        if (m_zoomStack.Count > 0)
          zoomLevel = m_zoomStack.ElementAt(index);
        return zoomLevel;
      }
    }

    /// <summary>
    /// count of zoom bounds in zoom stack
    /// </summary>
    public int Count
    {
      get { return m_zoomStack.Count; }
    }

    /// <summary>
    /// clear zoom stack and reset current bounds
    /// </summary>
    public void Clear()
    {
      m_zoomStack.Clear();
    }

    public bool EnablePan { get; set; }

    public bool CanPan { get; }

    public bool EnableZoom { get; set;}

    public bool CanZoom { get; }

    //private RectangleF ScaleBounds()
    //{
    //  float scaleFactor = GetScaleFactor();
    //}

    /// <summary>
    /// zoom in by a scale factor of percent / oneLevelDown Percent
    /// i.e. to zoom from 100% - 125% scale factor would be 1.25
    /// </summary>
    /// <returns></returns>
    private float GetScaleFactor()
    {
      int zoomIndex = m_zoomStack.Count - 1;
      if(m_zoomStack.Count == 1)
      {
        return this[zoomIndex];
      }
      else
      {
        return this[zoomIndex - 1] / this[zoomIndex];
      }
    }
  }
}
