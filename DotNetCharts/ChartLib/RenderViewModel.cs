using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  internal class RenderViewModel
  {
    private SortedDictionary<int, IDrawable> m_drawableCollection;

    private int m_zIndex = 0;

    internal RenderViewModel()
    {
      m_drawableCollection = new SortedDictionary<int, IDrawable>();
    }

    internal void Add(IDrawable drawableObject)
    {
      m_drawableCollection.Add(m_zIndex, drawableObject);
      ++m_zIndex;
    }

    internal void Add(IDrawable drawableObject, int zIndex)
    {
      if (!m_drawableCollection.ContainsKey(zIndex))
        m_drawableCollection.Add(m_zIndex, drawableObject);
    }

    internal void Remove(IDrawable drawableObject)
    {
      if (m_drawableCollection.ContainsValue(drawableObject))
      {
        var value = m_drawableCollection.FirstOrDefault((item) => item.Value.Equals(drawableObject));
        m_drawableCollection.Remove(value.Key);
      }
    }

    internal IReadOnlyCollection<IDrawable> DrawableObjects
    { 
      get { return new List<IDrawable>(m_drawableCollection.Values); } 
    }
  }
}
