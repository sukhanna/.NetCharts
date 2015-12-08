using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  public abstract class OneDimData<T>
  {
    T[] m_data;

    // Should have used Property instead of Get and Set methods.
    // But Property gives warning CA1819 which states that 
    // Arrays returned by properties are not write-protected, 
    // even if the property is read-only. To keep the array tamper-proof,
    // the property must return a copy of the array.
    public T[] GetData()
    {
      return m_data;
    }

    public void SetData(T[] data)
    {
      m_data = data;
      CalculateRange();
    }

    public DataRange<T> DataRange { get; protected set; }

    protected abstract void CalculateRange();
  }
}
