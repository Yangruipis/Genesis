 // 获取二维数组中一行的数据
        public static BigNumber[] GetRowByID(BigNumber[,] values, int rowID)
        {
            if (rowID > (values.GetLength(0) - 1))
                throw new Exception("rowID超出最大的行索引号!");
            BigNumber[] row = new BigNumber[values.GetLength(1)];
            for (int i = 0; i < values.GetLength(1); i++)
            {
                row[i] = values[rowID, i];
            }
            return row;
        }
        //获取二维数组中一列的数据
        public static BigNumber[] GetColByID(BigNumber[,] values, int columnID)
        {
            if (columnID > (values.GetLength(1) - 1))
            {
                throw new Exception("columuID 超出列最大值");
            }
            BigNumber[] columu = new BigNumber[values.GetLength(0)];
            for (int i = 0; i < values.GetLength(0); i++)
            {
                columu[i] = values[i, columnID];
            }
            return columu;
        }
        // 复制一行数据到二维数组指定的行上
        public static void CopyToRow(BigNumber[,] values, int rowID,BigNumber[] row)
        {
            if (rowID > (values.GetLength(0) - 1))
                throw new Exception("rowID超出最大的行索引号!");
            if (row.Length > (values.GetLength(1)))
                throw new Exception("row行数据列数超过二维数组的列数!");
            for (int i = 0; i < row.Length; i++)
            {
                values[rowID, i] = row[i];
            }
        }
        // 对二维数组排序 
        // 排序的二维数组
        // 排序根据的列的索引号数组
        // 排序的类型，1代表降序，0代表升序
        // 返回排序后的二维数组
        //orderColumnsIndexs =0 表示按第一列排序
        public static BigNumber[,] ArraySort(BigNumber[,] values, int[] orderColumnsIndexs, int type)
        {
            BigNumber[] temp = new BigNumber[values.GetLength(1)];
            int k;
            int compareResult;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (k = i + 1; k < values.GetLength(0); k++)
                {
                    if (type.Equals(1))
                    {
                        for (int h = 0; h < orderColumnsIndexs.Length; h++)
                        {
                            compareResult = Comparer.Default.Compare(GetRowByID(values, k).GetValue(orderColumnsIndexs[h]), GetRowByID(values, i).GetValue(orderColumnsIndexs[h]));
                            if (compareResult.Equals(1))
                            {
                                temp = GetRowByID(values, i);
                                Array.Copy(values, k * values.GetLength(1), values, i * values.GetLength(1), values.GetLength(1));
                                CopyToRow(values, k, temp);
                            }
                            if (compareResult != 0)
                                break;
                        }
                    }
                    else
                    {
                        for (int h = 0; h < orderColumnsIndexs.Length; h++)
                        {
                            compareResult = Comparer.Default.Compare(GetRowByID(values, k).GetValue(orderColumnsIndexs[h]), GetRowByID(values, i).GetValue(orderColumnsIndexs[h]));
                            if (compareResult.Equals(-1))
                            {
                                temp = GetRowByID(values, i);
                                Array.Copy(values, k * values.GetLength(1), values, i * values.GetLength(1), values.GetLength(1));
                                CopyToRow(values, k, temp);
                            }
                            if (compareResult != 0)
                                break;
                        }
                    }
                }
            }
            return values;
        }
        //显示二维数组
        public static void ArrayPrint(BigNumber[,] values)
        {
            int k;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (k = 0; k < values.GetLength(1); k++)
                {
                    Console.Write(values[i, k]);
                    Console.Write("  ");
                }
                Console.WriteLine(" ");
            }
        }
        public static int IsBalance(BigNumber[,] values, int i, int j)
        {
            BigNumber[,] Sorted = ArraySort(values,new int[]{i,j},0);
            BigNumber[] columu_i = GetColByID(Sorted, i);
            BigNumber[] columu_j = GetColByID(Sorted, j);
            int len = Sorted.GetLength(0);
            string[] string_i = new string[len];
            string[] string_j = new string[len];
            for (int c = 0; c < len; c++)
            {
                string_i[c] = columu_i[c].ToString();
                //Console.WriteLine("{0}", string_i[c]);
                string_j[c] = columu_j[c].ToString();
               // Console.WriteLine("{0}", string_j[c]);
            }
            Console.WriteLine("{0}{1}{2}{3}{4}", string_i[0], string_i[1], string_i[2],string_i[3],string_i[4]);
           // Console.WriteLine("{0}", string_j[2]);
            int num_list = StringNum(string_i);
            string[] string_list = StringList(string_i);

           Console.WriteLine("{0}{1}", string_list[0], string_list[1]);

            int[] count = new int[num_list];
            for (int m = 0; m < num_list; m++)
            {
                count[m] = 0;
                for (int n = 0; n < num_list; n++)
                {
                    if (string_i[(len/num_list) *m + n] == string_list[m])
                    {
                        count[m] = count[m] + 1;
                        Console.WriteLine("{0}{1}{2}", string_i[0], string_i[1], string_i[2]);
                    }
                    else
                    {
                        count[m] = count[m] + 0;
                    }
                }
            }
            Console.WriteLine("{0}{1}", count[0].ToString(),count[1].ToString());
            for (int m = 0; m < num_list; m++)
            {
                if (count[m] != count[0])
                {
                    return 0;
                }
            }
            Console.WriteLine("{0}{1}", count[0].ToString(), count[1].ToString()); 

            for (int m1 = 0; m1 < num_list - 1; m1++)
            {
                for (int n = 0; n < len / num_list; n++)
                {
                    if (string_j[m1 * len / num_list + n] != string_j[(m1 + 1) * len / num_list + n])
                    {
                        Console.WriteLine("{0}{1}",m1.ToString(), n.ToString()); 
                        return 0;
                    }
                    else
                    {

                    }
                }
            }
            return 1;
            
           
        }
