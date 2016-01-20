using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace 统计工具测试1
{
    public class MathV
    {
        public static string round(string number, int digits, int type)
        {
            //type为0时四舍五入，1为ground，2为ceiling
            int NumOriginLen = number.Length;
            char[] digit_dot = { '.' };
            string[] NumberBroken;
            NumberBroken = number.Split(digit_dot);
            if (digits < 0)
            {
                digits = 0;
            }
            if (NumberBroken[0].Length == NumOriginLen)
            {
                if (digits <= 0)
                {
                    return number;
                }
                else
                {
                    return NumberBroken[0] + ".".PadRight(digits + 1, '0');
                }
            }
            else
            {
                string decimal_part = " ";
                BigNumber zero_point_one = new BigNumber("0.1");
                BigNumber one = new BigNumber("1");
                if (NumberBroken[1].Length > digits)
                {
                    if (type == 1)
                    {
                        decimal_part = NumberBroken[1].Substring(0, digits);
                    }
                    else if (type == 2)
                    {
                        BigNumber carry = new BigNumber(digits.ToString());
                        carry = zero_point_one.Power(carry, 200);
                        BigNumber number_changed = new BigNumber(number);
                        number_changed = number_changed + carry;
                        if (digits <= 0)
                        {
                            return number_changed.ToString().Substring(0, NumberBroken[0].Length);
                        }
                        return number_changed.ToString().Substring(0, NumOriginLen - (NumberBroken[1].Length - digits));
                    }
                    else
                    {
                        if (Convert.ToInt32(NumberBroken[1].Substring(digits, 1)) > 4)
                        {
                            BigNumber carry = new BigNumber(digits.ToString());
                            carry = zero_point_one.Power(carry, 200);
                            BigNumber number_changed = new BigNumber(number);
                            number_changed = number_changed + carry;
                            return number_changed.ToString().Substring(0, NumOriginLen - (NumberBroken[1].Length - digits));
                        }
                        else
                        {
                            decimal_part = NumberBroken[1].Substring(0, digits);
                        }
                    }
                }
                else
                {
                    decimal_part = NumberBroken[1].PadRight(digits, '0');
                }
                if (decimal_part == "")
                {
                    return NumberBroken[0];
                }
                return NumberBroken[0] + '.' + decimal_part;
            }
        }

        public static BigNumber[,] MatPlus(BigNumber[,] mat1, BigNumber[,] mat2)
        {//矩阵加法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);

            if (len11 == len21 && len12 == len22)
            {
                BigNumber[,] a = new BigNumber[len11, len12];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len12; j++)
                    {
                        a[i, j] = mat1[i, j] + mat2[i, j];
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }


        public static BigNumber[,] MatMinu(BigNumber[,] mat1, BigNumber[,] mat2)
        {//矩阵减法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);

            if (len11 == len21 && len12 == len22)
            {
                BigNumber[,] a = new BigNumber[len11, len12];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len12; j++)
                    {
                        a[i, j] = mat1[i, j] - mat2[i, j];
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }

        public static BigNumber[,] MatTimes(BigNumber[,] mat1, BigNumber[,] mat2)
        {    //矩阵乘法
            int len11 = mat1.GetLength(0);
            int len12 = mat1.GetLength(1);
            int len21 = mat2.GetLength(0);
            int len22 = mat2.GetLength(1);
            if (len12 == len21)
            {
                BigNumber[,] a = new BigNumber[len11, len22];
                for (int i = 0; i < len11; i++)
                {
                    for (int j = 0; j < len22; j++)
                    {
                        for (int u = 0; u < len12; u++)
                        {
                            a[i, j] += mat1[i, u] * mat2[u, j];
                        }
                    }
                }
                return a;
            }
            else
            {
                return null;
            }
        }

        public static BigNumber[,] MatTrans(BigNumber[,] mat)
        {
            //矩阵转置
            int len1 = mat.GetLength(0);
            int len2 = mat.GetLength(1);
            BigNumber[,] a = new BigNumber[len2, len1];
            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    a[j, i] = mat[i, j];
                }
            }
            return a;
        }
        public static int StringNum(string[] St)
        {
            int len = St.Length;
            int[] count = new int[len];
            int count2 = 0;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    if (St[i] == St[j])
                    {
                        count[i] = count[i] + 1;
                    }
                    else
                    {
                        count[i] = count[i] + 0;
                    }
                }
            }
            for (int i = 0; i < len; i++)
            {
                if (count[i] == 1)
                {
                    count2 = count2 + 1;
                }
                else
                {
                    count2 = count2 + 0;
                }
            }
            if (len == count2)
            {
                return len;
            }
            int[] sum = new int[len - count2];
            int count3 = 0;

            for (int j = 2; j <= len - count2 + 1; j++)
            {
                for (int i = 0; i < len; i++)
                {
                    if (count[i] == j)
                    {
                        sum[j - 2] = sum[j - 2] + count[i];
                    }
                    else
                    {
                        sum[j - 2] = sum[j - 2] + 0;
                    }

                }
 
                count3 = count3 + sum[j - 2] / Convert.ToInt32(Math.Pow(j, 2.0));
            }
            int Count_single = count2 + count3;
            return Count_single;
        }
        public static string[] StringList(string[] St2)
        {
            int len = St2.Length;
            string[] St = new string[len];
            for (int i = 0; i < len; i++)
            {
                St[i] = St2[i];
            }
            string[] Count_All = new string[StringNum(St)];
            Count_All[0] = St[0];
            for (int i = 0; i < len; i++)
            {
                //int len_ar = len;
                for (int j = i + 1; j < len; j++)
                {

                    if (St[j] == St[i])
                    {
                        St[j] = "Ray";

                    }
                    // len_ar = ar.Count;
                }

            }
           /* for (int i = 0; i < StringNum(St); i++)
            {
                for (int c = i; c < len; c++)
                {
                    if (St[c] != "0")
                    {
                        Count_All[i] = St[c];
                        break;
                    }


                }
            }*/
            ArrayList List = new ArrayList(St);

            for (int i = 0; i < len - StringNum(St)+1; i++)
            {
                List.Remove("Ray");
            }

            string[] arrString = (string[])List.ToArray(typeof(string));
            return arrString;
        }


        // 获取二维数组中一行的数据
        public static object[] GetRowByID(object[,] values, int rowID)
        {
            if (rowID > (values.GetLength(0) - 1))
                throw new Exception("rowID超出最大的行索引号!");
            object[] row = new object[values.GetLength(1)];
            for (int i = 0; i < values.GetLength(1); i++)
            {
                row[i] = values[rowID, i];
            }
            return row;
        }
        //获取二维数组中一列的数据
        public static object[] GetColByID(object[,] values, int columnID)
        {
            if (columnID > (values.GetLength(1) - 1))
            {
                throw new Exception("columuID 超出列最大值");
            }
            object[] columu = new object[values.GetLength(0)];
            for (int i = 0; i < values.GetLength(0); i++)
            {
                columu[i] = values[i, columnID];
            }
            return columu;
        }
        // 复制一行数据到二维数组指定的行上
        public static void CopyToRow(object[,] values, int rowID, object[] row)
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
        public static object[,] ArraySort(object[,] values, int[] orderColumnsIndexs, int type)
        {
            object[] temp = new object[values.GetLength(1)];
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
        public static void ArrayPrint(object[,] values)
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


    }
}