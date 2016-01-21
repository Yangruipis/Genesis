using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 绘图1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1")
            {
                chart1.Series.Clear();
                Series series = new Series("随便画的函数图");
                series.ChartType = SeriesChartType.StackedColumn;
                chart1.ChartAreas[0].AxisX.Minimum = 0.4; //坐标最小值 
                chart1.ChartAreas[0].AxisX.Maximum = 8;//坐标最大值
                chart1.ChartAreas[0].AxisX.Interval = 0.7;//坐标大刻度间隔
                series.Color = Color.Brown;
               // chart1.ChartAreas[0].AxisX.ScaleView.Zoom(2, 3);
               chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
               chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                double[] x = new double[] { 2.1, 1.4, 1.9, 1, 5.2, 5, 6, 5.4, 4, 3.1, 2.8, 1.1 };
                double[] y = new double[7];
                for (int i = 0; i < 7; i++)
                {
                    y[i] = 1.1 + 0.7 / 2 + i * 0.7;
                }
                //12个数，分成7份，最大值为6，最小值为1.1，极差为4.9，4.9/7=0.7，步长为0.7
                int[] count = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    count[i] = 0;
                    for (int j = 0; j < 12; j++)
                    {
                        if (x[j] >= 1.1 + 0.7 * i && x[j] < 1.1 + 0.7 * (i + 1))
                        {
                            count[i] = count[i] + 1;
                        }
                    }
                }

                series.BorderWidth = 20;
                series.MarkerSize = 10;
                for (int i = 0; i < 7; i++)
                {
                    series.Points.AddXY(y[i], count[i]);
                }


                /* double y;
                 for (double i = -5; i < 5;i = i + 0.1 ){
                     y = (1/Math.Sqrt(2*Math.PI)) *Math.Exp(-Math.Pow(i,2)/2);
                     series.Points.AddY(y);
                 }*/

                chart1.Series.Add(series);
            }
            if (textBox1.Text == "2")
            {
                this.chart1.Series.Clear();
                object[,] o1 = new object[6, 3] { { 1, 2, 2.5 }, { 2, 1, 3 }, { 3, 1, 8 }, { 1, 1, 6 }, { 2, 2, 4 }, { 3, 2, 5 } };
                object[,] o = ArraySort3.Arraysort(o1, new int[] { 0, 1 }, 0);
                int len_x = o.GetLength(0);
                object[] ox = ArraySort3.GetColByID(o, 1);
                object[] oy = ArraySort3.GetColByID(o, 2);
                object[] oclass1 = ArraySort3.GetColByID(o, 0);
                string[] x_string = new string[len_x];
                string[] class1_string = new string[len_x];
                double[] x = new double[len_x];
                double[] y = new double[len_x];
                for (int i = 0; i < len_x; i++)
                {
                    x[i] = Convert.ToDouble(ox[i]);
                    x_string[i] = ox[i].ToString();
                    class1_string[i] = oclass1[i].ToString();
                    y[i] = Convert.ToDouble(oy[i]);
                }
                
                //double[] x = new double[] { 1, 2, 3, 4,1,2,3, 4, 1, 2,3,4 }; //再按照这个排序
                //double[] y = new double[] { 1.1, 2.2, 3.1,1,2, 4.3, 5, 6 ,2,3,2,3};
               // double[] class1 = new double[] {2,2,2,2,3,3,3,3,4,4,4,4};//先按照这个排序
                
                string[] class1_type = MathV.StringList(class1_string);
                int count_class1 = MathV.StringNum(class1_string);
                int count_x = MathV.StringNum(x_string);

                for (int i = 0; i < count_class1; i++)
                 {
                     Series series = new Series();
                     series.Name = class1_type[i];
                     series.ChartType=SeriesChartType.StackedColumn;
                     this.chart1.Series.Add(series);
                 }
                //this.chart1.Series[0].Color = Color.LightSeaGreen;
                //this.chart1.Series[1].Color = Color.Tomato;
               // this.chart1.Series[2].Color = Color.SpringGreen;
                //**********指定第二个柱状图为绿色：
                //this.chart1.Series[0].Points[1].Color = Color.Green

                //Series series0 = new Series("class = 1");
                // Series series1 = new Series("class = 2");
                 //Series series2= new Series("随便画的函数图3");
                // series0.ChartType = SeriesChartType.StackedColumn;
                // series1.ChartType = SeriesChartType.StackedColumn;
                // series2.ChartType = SeriesChartType.StackedColumn;
                  //   this.chart1.Series.Add(series0);
                  //   this.chart1.Series.Add(series1);
                   // this.chart1.Series.Add(series2);
                     for (int i = 0; i < count_class1; i++)  //根据class逐个填写series
                    {
                        double[] x1 = new double[count_x] ;
                        double[] y1 = new double[count_x];
                        for (int j = 0; j < count_x ; j++)//同一series逐个填写x
                        {
                            x1[j] = x[i * count_x + j];
                            y1[j] = y[i * count_x + j];
                            this.chart1.Series[i].Points.AddXY(x1[j], y1[j]);  //添加对应数据点
                        }

                    }



            }







        }
    }
}
    

