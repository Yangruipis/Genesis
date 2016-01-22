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
                        
                        Big_variable[j][m] = MathV.Double2Big(variable[j][m]);
                        
                    }
                    
                    corr[i, j] = Stat.Corr(Big_variable[i],Big_variable[j]);
                    
                    corr[j, i] = corr[i, j];
                }
            }
            for (int i = 0; i < len_variable; i++)
            {
                corr[i, i] = new BigNumber("1");
            }

            string result = AdjustStr("变量名") + "\t"  + "\r\n";
            for (int i = 0; i < len_variable; i++)
            {
                result = result + Col_name[i] + "\t";
                for (int j = 0; j < len_variable; j++)
                {
                    result = result + MathV.round(corr[i, j].ToString(),4,0) + "\t";
                }
                result = result + "\r\n";
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