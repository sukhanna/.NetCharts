using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using ChartLib;
using System.Drawing;

namespace GraphicsDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      RenderView view = (RenderView)windowsHost.Child;
      view.BackColor = System.Drawing.Color.Black;
      LineChart chart = new LineChart();
      chart.Color = System.Drawing.Color.Red;
      chart.OneDimFloatData = new OneDimFloatData();
      float[] data = new float[10000];
      ChartUtility.GenerateSinWave(data, 15, 1000);
      chart.OneDimFloatData.SetData(data);
      chart.XAxis = new XAxis<LinearScale>();
      chart.YAxis = new YAxis<LinearScale>();
      chart.XAxis.Label = new ChartLib.Label("X Axis");
      chart.YAxis.Label = new ChartLib.Label("Y Axis");
      chart.XAxis.Color = System.Drawing.Color.Gray;
      chart.YAxis.Color = System.Drawing.Color.Gray;
      chart.XAxis.Label.Color = System.Drawing.Color.Gray;
      chart.YAxis.Label.Color = System.Drawing.Color.Gray;
      chart.XAxis.TickTextColor = System.Drawing.Color.Gray;
      chart.YAxis.TickTextColor = System.Drawing.Color.Gray;
      view.Add(chart);      
    }
  }
}
