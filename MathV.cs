﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}