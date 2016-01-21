using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 统计图形界面1
{
    public partial class MutiCorr : Form
    {
        public MutiCorr()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int StartCol, EndCol;
            if (start_box.Text.Trim() == "*")
            {
                StartCol = 1;
            }
            else
            {
                StartCol = Convert.ToInt32(start_box.Text);
            }
            if (end_box.Text.Trim() == "*")
            {
                EndCol = Form1.S.dataGridView1.ColumnCount;
            }
            else
            {
                EndCol = Convert.ToInt32(end_box.Text);
            }

            string corr_result = MutiCorr2(StartCol - 1, EndCol - 1);
            textBox_corr.Text = corr_result;
        }
        public string FindNAs(string ID_x, string ID_y)
        {
            int ColNum_x = 0, ColNum_y = 0;
            for (int i = 0; i < Form1.S.dataGridView1.ColumnCount; i++)
            {
                if (Form1.S.dataGridView1.Columns[i].Name == ID_x)
                {
                    ColNum_x = i;
                }
                if (Form1.S.dataGridView1.Columns[i].Name == ID_y)
                {
                    ColNum_y = i;
                }
            }
            int AllRowCounts = Form1.S.dataGridView1.RowCount;
            string WarningNAs = " ";
            int CountNA = 0;
            for (int i = 0; i < AllRowCounts; i++)
            {
                if (Form1.S.dataGridView1.Rows[i].Cells[ColNum_x].Value == null)
                {
                    WarningNAs = WarningNAs + "," + i.ToString();
                    CountNA++;
                }
                else if (Form1.S.dataGridView1.Rows[i].Cells[ColNum_x].Value.ToString().Trim() == "")
                {
                    WarningNAs = WarningNAs + "," + i.ToString();
                    CountNA++;
                }
                else if (Form1.S.dataGridView1.Rows[i].Cells[ColNum_y].Value == null)
                {
                    WarningNAs = WarningNAs + "," + i.ToString();
                    CountNA++;
                }
                else if (Form1.S.dataGridView1.Rows[i].Cells[ColNum_y].Value.ToString().Trim() == "")
                {
                    WarningNAs = WarningNAs + "," + i.ToString();
                    CountNA++;
                }
                else
                {
                    WarningNAs = WarningNAs + ",NOEMPTY";
                }
            }
            return WarningNAs;
        }

        double[] VectorRead(string ID, string WarningNAs)
        {
            //读单列
            int ColNum = 0;
            for (int i = 0; i < Form1.S.dataGridView1.ColumnCount; i++)
            {
                if (Form1.S.dataGridView1.Columns[i].Name == ID)
                {
                    ColNum = i;
                    break;
                }
            }
            int AllRowCounts = Form1.S.dataGridView1.RowCount - 1;
            /*string[] DataReady = new string[AllRowCounts];
            string EmptyRowStr = "";
            int CountEmpty = 0;
            for (int i = 0;i < AllRowCounts ;i++){
                if (Form1.S.dataGridView1.Rows[i].Cells[ColNum].Value == null){
                    EmptyRowStr = EmptyRowStr + "," + i.ToString();
                    CountEmpty ++;
                }
                else if (Form1.S.dataGridView1.Rows[i].Cells[ColNum].Value.ToString().Trim() == ""){
                    EmptyRowStr = EmptyRowStr + "," + i.ToString();
                    CountEmpty ++;
                }
            }*/
            //int CountEmpty = 0;
            char[] separator = { ',' };
            int IsNOTEmpty = 0;
            string Temp;
            foreach (char EachChar in WarningNAs)
            {
                if (EachChar == 'N')
                {
                    IsNOTEmpty++;
                }
            }
            //MessageBox.Show("IsNotEmpty = " + IsNOTEmpty);
            //MessageBox.Show("WarningNAs: " + WarningNAs);
            string[] SkipRow = WarningNAs.Split(separator);
            int DataRows_Count = 0;
            int IsEmpty = 0;
            double[] DataChose = new double[IsNOTEmpty];
            for (int i = 0; i < AllRowCounts; i++)
            {
                IsEmpty = 0;
                foreach (string EachRow in SkipRow)
                {
                    if (EachRow.Trim() == i.ToString())
                    {
                        IsEmpty = 1;
                    }
                }
                if (IsEmpty == 0)
                {
                    Temp = Form1.S.dataGridView1.Rows[i].Cells[ColNum].Value.ToString();
                    //MessageBox.Show("第" + i + "行的变量的值为：" + Temp);
                    DataChose[DataRows_Count] = Convert.ToDouble(Temp.Trim());
                    DataRows_Count++;
                }
            }
            return DataChose;

        }
        public string AdjustStr(string str)
        {
            //将可能包含中文变量的字符串调整成右对齐且长度为12
            byte[] sarr = System.Text.Encoding.Default.GetBytes(str);
            int space = 12 - sarr.Length;
            string strToAdd = "";
            if (space > 0)
            {
                for (int i = 0; i < space; i++)
                {
                    strToAdd = strToAdd + " ";
                }
                str = strToAdd + str;
                return str;
            }
            return str;
        }
        public string MutiCorr2(int StartCol, int EndCol)
        {
            int temp;
            if (StartCol > EndCol)
            {
                temp = StartCol;
                StartCol = EndCol;
                EndCol = temp;
            }
            int len_variable = EndCol - StartCol + 1;
            double[][] variable = new double[len_variable][];
            BigNumber[,] corr = new BigNumber[len_variable, len_variable];
            string[] Col_name = new string[len_variable];
            //int id = this.dataGridView1.SelectedRows[0].Index;

            for (int i = StartCol; i < EndCol+1; i++)
            {
                Col_name[i-StartCol ] = Form1.S.dataGridView1.Columns[i].Name.ToString();
            }
            string BlackList;
            BigNumber[][] Big_variable = new BigNumber[len_variable][];
            int len;
            for (int i = 0; i < len_variable; i++)
            {
                for (int j = i + 1; j < len_variable ; j++)
                {
                    BlackList = FindNAs(Col_name[i], Col_name[j]);
                    variable[i] = VectorRead(Col_name[i], BlackList);
                    variable[j] = VectorRead(Col_name[j], BlackList);
                    len = variable[i].Length;
                    Big_variable[i] = new BigNumber[len];
                    Big_variable[j] = new BigNumber[len];
                    for (int m = 0; m < len; m++)
                    {
                       
                        
                        Big_variable[i][m] = MathV.Double2Big(variable[i][m]);
                        MessageBox.Show(Big_variable[i][m].ToString());
                        Big_variable[j][m] = MathV.Double2Big(variable[j][m]);
                        MessageBox.Show(Big_variable[j][m].ToString());
                    }
                    MessageBox.Show(Big_variable[0][0].ToString());
                    corr[i, j] = Stat.Corr(Big_variable[i],Big_variable[j]);
                    MessageBox.Show(corr[i,j].ToString());
                    corr[j, i] = corr[i, j];
                }
            }
            for (int i = 0; i < len_variable; i++)
            {
                corr[i, i] = new BigNumber("1");
            }

            string result = AdjustStr("变量名") + "\t" + AdjustStr("变量一") + "\t" + AdjustStr("变量二") + "\r\n";
            for (int i = 0; i < len_variable; i++)
            {
                for (int j = 0; j < len_variable; j++)
                {
                    result = result + corr[i, j].ToString() + "\t";
                }
                result = result + "\n";
            }
            return result;

        }

        private void start_box_TextChanged(object sender, EventArgs e)
        {
            int n;
            if (start_box.Text.Trim() == "*")
            {
                n = 1;
            }
            else
            {
                n = Convert.ToInt32(start_box.Text);
            }
        }

        private void end_box_TextChanged(object sender, EventArgs e)
        {
            int n;
            if (end_box.Text.Trim() == "*")
            {
                n = Form1.S.dataGridView1.ColumnCount;
            }
            else
            {
                n = Convert.ToInt32(end_box.Text);
            }
        }


    }
}
