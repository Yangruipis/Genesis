using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 统计工具测试1
{
    class Program
    {
        static BigNumber Mean(BigNumber[] NumberSeries)
        {
            BigNumber sum = new BigNumber("0");
            foreach (BigNumber SingleNumber in NumberSeries)
            {
                sum += SingleNumber;
            }
            int len = NumberSeries.Length;
            BigNumber len_bignumber = new BigNumber(len.ToString());
            return sum / len_bignumber;
        }
        static string round(string number, int digits, int type)
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
        static void Sort(int n, BigNumber[] NumberSeries)
        {
            BigNumber temp = new BigNumber("0");
            if (n <= 1){
                return;
            }
            for (int i = 0;i < n - 1;i++){
                if (CompareNumber.Compare(NumberSeries[i] , NumberSeries[i+1]) == 1){
                    temp = NumberSeries[i +1];
                    NumberSeries[i + 1]= NumberSeries[i];
                    NumberSeries[i]=temp;
                }
                	Sort(n - 1, NumberSeries);
            }
        }
        static BigNumber Quantile(BigNumber[] NumberSeries, double quan)
        {
            int len = NumberSeries.Length;
            double position = quan * (double)(len + 1);
            if ((position - Convert.ToDouble((int)position)) != 0)
            {
                int position_low = Convert.ToInt32(round(position.ToString(), 0, 1));
                BigNumber multi_1 = new BigNumber((position - Convert.ToDouble(position_low)).ToString());
                BigNumber multi_2 = new BigNumber((1 - (position - Convert.ToDouble(position_low))).ToString());
                BigNumber quan_num = new BigNumber("0");
                quan_num = multi_1 * NumberSeries[position_low - 1] + multi_2 * NumberSeries[position_low];
                return quan_num;
            }
            else
            {
                BigNumber quan_num = NumberSeries[(int)position - 1];
                return quan_num;
            }
        }
        static string Mode(BigNumber[] NumberSeries)
        {
            //仅限于寻找有序数列中的众数
            //多个众数时返回最小的众数
            double MaxCount = 0;
            double CurrentCount = 0;
            BigNumber MaxNumber = new BigNumber("0");
            BigNumber CurrentNumber = new BigNumber("0");
            int len = NumberSeries.Length;
            for (int i = 1; i < len; i++)
            {
                if (CompareNumber.Compare(NumberSeries[i - 1], NumberSeries[i]) == 0)
                {
                    CurrentCount++;
                    if (CurrentCount > MaxCount)
                    {
                        MaxCount = CurrentCount;
                        MaxNumber = NumberSeries[i];
                    }
                }
                else
                {
                    CurrentCount = 0;
                }
            }
            if (MaxCount == 0)
            {
                return "NA";
            }
            else
            {
                return MaxNumber.ToString();
            }
        }
        static BigNumber Variance(BigNumber[] NumberSeries)
        {
            //n - 1个自由度
            BigNumber sum = new BigNumber("0");
            int len = NumberSeries.Length;
            BigNumber mean_series = Mean(NumberSeries);
            foreach (BigNumber SingleNumber in NumberSeries)
            {
                sum += (SingleNumber - mean_series).Power(new BigNumber("2"), 30);
            }
            return sum / new BigNumber((len-1).ToString());
        }
        public static double NORMSDIST(double a)
        {
            //近似计算正态分布
            double p = 0.2316419;
            double b1 = 0.31938153;
            double b2 = -0.356563782;
            double b3 = 1.781477937;
            double b4 = -1.821255978;
            double b5 = 1.330274429;
            double x = Math.Abs(a);
            double t = 1 / (1 + p * x);
            double val = 1 - (1 / (Math.Sqrt(2 * Math.PI)) * Math.Exp(-1 * Math.Pow(a, 2) / 2)) * (b1 * t + b2 * Math.Pow(t, 2) + b3 * Math.Pow(t, 3) + b4 * Math.Pow(t, 4) + b5 * Math.Pow(t, 5));
            if (a < 0)
            {
                val = 1 - val;
            }
            return val;
        }
        public static double NORMSINV(double alpha)
        {
            if (0.5 < alpha && alpha < 1)
            {
                alpha = 1 - alpha;
            }
            double[] b = new double[11];
            b[0] = 0.1570796288 * 10;
            b[1] = 0.3706987906 * 0.1;
            b[2] = -0.8364353589 * 0.001;
            b[3] = -0.2250947176 * 0.001;
            b[4] = 0.6841218299 * 0.00001;
            b[5] = 0.5824238515 * 0.00001;
            b[6] = -0.1045274970 * 0.00001;
            b[7] = 0.8360937017 * 0.0000001;
            b[8] = -0.3231081277 * 0.00000001;
            b[9] = 0.3657763036 * 0.0000000001;
            b[10] = 0.6657763036 * 0.000000000001;
            double sum = 0;
            double y = -Math.Log(4 * alpha * (1 - alpha));
            for (int i = 0; i < 11; i++)
            {
                sum += b[i] * Math.Pow(y, i);
            }
            return Math.Pow(sum * y, 0.5);
        }
       

        static string ParaEasti(BigNumber[] NumberSeries, double significance, int tail)
        {
            //单样本总体均值的区间估计
            //tail == 1右单尾，H0：x>= x_0,tail == -1 左单尾H0：x<= x_0,，为2则双尾
            BigNumber mean_series = Mean(NumberSeries);
            BigNumber var_series = Variance(NumberSeries);
            BigNumber sd_series = var_series.Power(new BigNumber("0.5"), 30);
            BigNumber n = new BigNumber(NumberSeries.Length.ToString());
            if (tail == 2)
            {
                significance = significance / 2;
                BigNumber lower = mean_series - new BigNumber(NORMSINV(significance).ToString()) * sd_series /n.Power(new BigNumber("-0.5"), 30);
                BigNumber upper = mean_series + new BigNumber(NORMSINV(significance).ToString()) * sd_series / n.Power(new BigNumber("-0.5"), 30);
                return   lower.ToString() + "," + upper.ToString()  ;
            }
            else
            {
                significance = significance / 1;
                BigNumber lower = mean_series - new BigNumber(NORMSINV(significance).ToString()) * sd_series /( n.Power(new BigNumber("-0.5"), 30));
                if (tail == 1)
                {
                    return lower.ToString() + ",";
                }
                else
                {
                    BigNumber upper = mean_series + new BigNumber(NORMSINV(significance).ToString()) * sd_series / (n.Power(new BigNumber("-0.5"), 30));
                    return "," + upper.ToString();
                }
            }
            
            
            

        }
        static string HT(BigNumber H0, BigNumber[] NumberSeries, double significance, int tail)
        {
            //单样本总体均值假设检验
            string result = ParaEasti(NumberSeries, significance, tail);
            char[] separator = { ',' };
            string[] intervals = result.Split(separator);
           
            BigNumber mean_series = Mean(NumberSeries);
            BigNumber var_series = Variance(NumberSeries);
            BigNumber n = new BigNumber(NumberSeries.Length.ToString());
            BigNumber tvalue = (H0-mean_series)/((var_series/n).Power(new BigNumber("0.5"),10));
            
            Double t_value = Convert.ToDouble(tvalue.ToString());
            BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());
            //tail == 1右单尾，H0：x>= x_0,tail == -1 左单尾H0：x<= x_0,，为2则双尾
            if (tail == 2)
            {
                BigNumber lower = new BigNumber(intervals[0]);
                BigNumber upper = new BigNumber(intervals[1]);
                if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                {
                    return "P = "+ pvalue.ToString() +","+"不拒绝原假设";
                }
                else
                {
                    return "P = "+ pvalue.ToString() +","+"拒绝原假设";
                }
            }
            else
            {
                if (tail == 1)
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    if (CompareNumber.Compare(lower, H0) == -1 )
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    BigNumber lower = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == 1)
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = "+ (new BigNumber("2")*pvalue- new BigNumber("1")).ToString()+ "," + "拒绝原假设";
                    }
                }
            }
        }

        static string ParaEasti2(BigNumber[] NumberSeries1, BigNumber[] NumberSeries2, double significance, int tail)
        {
            //双样本总体均值区间估计
            BigNumber one = new BigNumber("1");
            BigNumber mean1 = Mean(NumberSeries1);
            BigNumber mean2 = Mean(NumberSeries2);
            BigNumber var1 = Variance(NumberSeries1);
            BigNumber var2 = Variance(NumberSeries2);
            BigNumber n1 = new BigNumber(NumberSeries1.Length.ToString());
            BigNumber n2 = new BigNumber(NumberSeries2.Length.ToString());
            BigNumber S_p= ((n1-one)*var1 + (n2-one)*var2)/(n1+n2-one- one );
           
            if (tail == 2)
            {
                significance = significance / 2;
                BigNumber lower = mean1- mean2 - new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one/n1 + one / n2)).Power(new BigNumber("-0.5"), 30);
                BigNumber upper = mean1 - mean2 + new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / n1 + one / n2)).Power(new BigNumber("-0.5"), 30);
                return lower.ToString() + "," + upper.ToString();
            }
            else
            {   significance = significance / 1;
                
                if(tail == 1){
                 BigNumber lower = mean1 - mean2 - new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / n1 + one / n2)).Power(new BigNumber("-0.5"), 30);
                
                return lower.ToString() + ",";
            }
            else{
                if (tail == -1) { 
                BigNumber upper = mean1 - mean2 + new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / n1 + one / n2)).Power(new BigNumber("-0.5"), 30);
                return "," + upper.ToString();
                }
                else
                {
                    return "请输入正确的值";
                }
            }
            }
        }

        static string HT2(BigNumber H0, BigNumber[] NumberSeries1,BigNumber[] NumberSeries2, double significance, int tail)
        {
            //双样本总体均值假设检验
            string result = ParaEasti2(NumberSeries1,NumberSeries2, significance, tail);
            char[] separator = { ',' };
            string[] intervals = result.Split(separator);

            BigNumber one = new BigNumber("1");
            BigNumber mean1 = Mean(NumberSeries1);
            BigNumber mean2 = Mean(NumberSeries2);
            BigNumber var1 = Variance(NumberSeries1);
            BigNumber var2 = Variance(NumberSeries2);
            BigNumber n1 = new BigNumber(NumberSeries1.Length.ToString());
            BigNumber n2 = new BigNumber(NumberSeries2.Length.ToString());
            BigNumber S_p= ((n1-one)*var1 + (n2-one)*var2)/(n1+n2-one- one );
            BigNumber tvalue = (H0-(mean1-mean2))/S_p.Power(new BigNumber("0.5"),30);
            Double t_value = Convert.ToDouble(tvalue.ToString());
            BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());

            if (tail == 2) //H0 : mu1=mu2
            {
                BigNumber lower = new BigNumber(intervals[0]);
                BigNumber upper = new BigNumber(intervals[1]);
                if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                {
                    return "P =" + pvalue.ToString() + "," + "不拒绝原假设";
                }
                else
                {
                    return "P =" + pvalue.ToString() + "," + "拒绝原假设";
                }
            }
            else
            {
                if (tail == 1)//H0:mu1>=mu2
                {   BigNumber lower = new BigNumber(intervals[0]);
                    if (CompareNumber.Compare(lower, H0) == -1 )
                    {
                        return "P =" + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P =" + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
                else//H0:mu1<=mu2
                {
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(upper, H0) == 1)
                    {
                        return "P =" + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P =" + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }

            }
        }


        static BigNumber Covariance(BigNumber[] NumberSeries1, BigNumber[] NumberSeries2)
        {
            //协方差计算
            BigNumber sum = new BigNumber("0");
            int len1 = NumberSeries1.Length;
            int len2 = NumberSeries2.Length;
            BigNumber mean_series1 = Mean(NumberSeries1);
            BigNumber mean_series2 = Mean(NumberSeries2);
            BigNumber E_xy = new BigNumber("0");
           
                for(int i = 0 ;i<len1; i ++ ){
                    E_xy += NumberSeries1[i] * NumberSeries2[i];
                }
                BigNumber cov = E_xy / (new BigNumber(len1.ToString())) - mean_series1 * mean_series2;
                return cov;
        }

        static BigNumber Corr(BigNumber[] NumberSeries1, BigNumber[] NumberSeries2)
        {
            //相关系数计算
            BigNumber cov = Covariance(NumberSeries1, NumberSeries2);
            BigNumber len1 = new BigNumber(NumberSeries1.Length.ToString());
            BigNumber var1 = Variance(NumberSeries1) * (len1 -new BigNumber("1") )/ len1;
            BigNumber var2 = Variance(NumberSeries2) * (len1 - new BigNumber("1")) / len1;
            BigNumber Rho = cov / ((var1 * var2).Power(new BigNumber("0.5")));

            return Rho;
        }

        static string ProportionEasti(BigNumber[] NumberSeries, BigNumber charater, double significance, int tail)
        {
            //单样本总体比例的区间估计
            BigNumber count = new BigNumber("0");
            int len = NumberSeries.Length;
            for (int i=0;i < len ;i++)
            {if (CompareNumber.Compare(NumberSeries[i],charater) == 0)
              {
                count = count + new BigNumber("1");

              }
            else{
                count = count + new BigNumber("0");
            }
            }
            BigNumber proportion = count / new BigNumber(len.ToString());
            Console.WriteLine("计数是{0}", count.ToString());
            Console.WriteLine("比例是{0}", proportion.ToString());
            if (tail == 2)
            {
                significance = significance / 2;
                BigNumber lower = proportion - new BigNumber(NORMSINV(significance).ToString()) * ((proportion * (new BigNumber("1") - proportion)) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                BigNumber upper = proportion + new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                return lower.ToString() + "," + upper.ToString();
            }
            else
            {     significance = significance / 1;
                if (tail == 1)
                {
                    
                    BigNumber lower = proportion - new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString())).Power(new BigNumber("0.5")));
                    return lower.ToString() + ",";
                }
                else
                {
                    BigNumber upper = proportion + new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString())).Power(new BigNumber("0.5")));
                    return "," + upper.ToString();
                }
            }
        }
        static string ProportionHT(BigNumber H0, BigNumber[] NumberSeries,BigNumber charater, double significance, int tail)
        {
            //样本比例假设检验
            string result = ProportionEasti(NumberSeries,charater, significance, tail);
            char[] separator = { ',' };
            string[] intervals = result.Split(separator);
            BigNumber count = new BigNumber("0");
            int len = NumberSeries.Length;
            for (int i = 0; i < len; i++)
            {
                if (CompareNumber.Compare(NumberSeries[i], charater) == 0)
                {
                    count = count + new BigNumber("1");

                }
                else
                {
                    count = count + new BigNumber("0");
                }
            }
            BigNumber proportion = count / new BigNumber(len.ToString());
            BigNumber var_series = Variance(NumberSeries);
            BigNumber n = new BigNumber(NumberSeries.Length.ToString());
            BigNumber tvalue = (proportion - H0) / ((H0 * (new BigNumber("1") - H0) / n).Power(new BigNumber("0.5"), 10));
            Double t_value = Convert.ToDouble(tvalue.ToString());
            BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());
            //tail == 1右单尾，H0：x>= x_0,tail == -1 左单尾H0：x<= x_0,，为2则双尾
            if (tail == 2)
            {
                BigNumber lower = new BigNumber(intervals[0]);
                BigNumber upper = new BigNumber(intervals[1]);
                if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                {
                    return "P = " + pvalue.ToString() + "," + "不拒绝原假设";
                }
                else
                {
                    return "P = " + pvalue.ToString() + "," + "拒绝原假设";
                }
            }
            else
            {
                if (tail == 1)
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    if (CompareNumber.Compare(lower, H0) == -1)
                    {
                        return "P = " + (new BigNumber("2") * pvalue- new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    BigNumber lower = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == 1)
                    {
                        return "P = "+ (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
            }
        }

        static string ProportionEasti2(BigNumber[] NumberSeries1,BigNumber[] NumberSeries2, BigNumber charater, double significance, int tail)
        {
            BigNumber count1 = new BigNumber("0");
            int len1 = NumberSeries1.Length;
            for (int i = 0; i < len1; i++)
            {
                if (CompareNumber.Compare(NumberSeries1[i], charater) == 0)
                {
                    count1 = count1 + new BigNumber("1");

                }
                else
                {
                    count1 = count1 + new BigNumber("0");
                }
            }
            BigNumber proportion1 = count1 / new BigNumber(len1.ToString());

            BigNumber count2 = new BigNumber("0");
            int len2 = NumberSeries2.Length;
            for (int i = 0; i < len2; i++)
            {
                if (CompareNumber.Compare(NumberSeries2[i], charater) == 0)
                {
                    count2 = count2 + new BigNumber("1");

                }
                else
                {
                    count2 = count2 + new BigNumber("0");
                }
            }
            BigNumber proportion2 = count2 / new BigNumber(len2.ToString());

            if (tail == 2)
            {
                significance = significance / 2;
                BigNumber lower = proportion1 - proportion2 - new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                BigNumber upper = proportion1 - proportion2 + new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                return lower.ToString() + "," + upper.ToString();
            }
            else
            {
                significance = significance / 1;
                if (tail == 1)
                {

                    BigNumber lower = proportion1 - proportion2 - new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                    return lower.ToString() + ",";
                }
                else
                {
                    BigNumber upper = proportion1 - proportion2 + new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                    return "," + upper.ToString();
                }
            }
        }

        static string ProportionHT2(BigNumber H0, BigNumber[] NumberSeries1, BigNumber[] NumberSeries2, BigNumber charater, double significance, int tail)
        {
            //样本比例假设检验
            string result = ProportionEasti2(NumberSeries1,NumberSeries2, charater, significance, tail);
            char[] separator = { ',' };
            string[] intervals = result.Split(separator);
            BigNumber count1 = new BigNumber("0");
            int len1 = NumberSeries1.Length;
            for (int i = 0; i < len1; i++)
            {
                if (CompareNumber.Compare(NumberSeries1[i], charater) == 0)
                {
                    count1 = count1 + new BigNumber("1");

                }
                else
                {
                    count1 = count1 + new BigNumber("0");
                }
            }
            BigNumber proportion1 = count1 / new BigNumber(len1.ToString());
            BigNumber count2 = new BigNumber("0");
            int len2 = NumberSeries2.Length;
            for (int i = 0; i < len2; i++)
            {
                if (CompareNumber.Compare(NumberSeries2[i], charater) == 0)
                {
                    count2 = count2 + new BigNumber("1");

                }
                else
                {
                    count2 = count2 + new BigNumber("0");
                }
            }
            BigNumber proportion2 = count2 / new BigNumber(len2.ToString());
            BigNumber n1 = new BigNumber(NumberSeries1.Length.ToString());
            BigNumber n2 = new BigNumber(NumberSeries2.Length.ToString());
            BigNumber pbar = (count1 + count2) / (n1 + n2);
            BigNumber tvalue = (proportion1-proportion2 - H0) / (pbar*(new BigNumber("1")-pbar)*(new BigNumber("1")/n1 + new BigNumber("1")/n2 )).Power(new BigNumber("0.5"), 10);
            Double t_value = Convert.ToDouble(tvalue.ToString());
            BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());
            //tail == 1右单尾，H0：x>= x_0,tail == -1 左单尾H0：x<= x_0,，为2则双尾
            if (tail == 2)
            {
                BigNumber lower = new BigNumber(intervals[0]);
                BigNumber upper = new BigNumber(intervals[1]);
                if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                {
                    return "P = " + pvalue.ToString() + "," + "不拒绝原假设";
                }
                else
                {
                    return "P = " + pvalue.ToString() + "," + "拒绝原假设";
                }
            }
            else
            {
                if (tail == 1)
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    if (CompareNumber.Compare(lower, H0) == -1)
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    BigNumber lower = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == 1)
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            /*Console.WriteLine("请输入你要输入第一组数字个数：");
          int n1 = Convert.ToInt32(Console.ReadLine());
          Console.WriteLine("请输入第一组数，用逗号分隔：");
          string number_series1 = Console.ReadLine();
          Console.WriteLine("请输入你要输入第二组数字个数：");
          int n2= Convert.ToInt32(Console.ReadLine());
          Console.WriteLine("请输入第二组数，用逗号分隔：");
          string number_series2 = Console.ReadLine();
          char[] separator = { ',' };
          string[] numbers1 = number_series1.Split(separator);
          string[] numbers2 = number_series2.Split(separator);
          BigNumber[] x1 = new BigNumber[n1];
          BigNumber[] x2 = new BigNumber[n2];
          for (int i = 0; i < n1; i++)
          {
              x1[i] = new BigNumber(numbers1[i]);
          }
          for (int j = 0; j < n2; j++)
          {
              x2[j] = new BigNumber(numbers2[j]);
          }
         BigNumber mean = Mean(x);
          BigNumber variance = Variance(x);
          BigNumber std = variance.Power(new BigNumber("0.5"), 30);
          BigNumber m = new BigNumber(x.Length.ToString());
          Console.WriteLine("均值区间估计为：{0}", ParaEasti(mean,std, m,0.05, 2));
           Console.WriteLine("假设检验H0 = 20\n{0}", HT(new BigNumber("0.5"),mean, std, m, 0.05, 2));
           Console.WriteLine("假设检验H0 = 20\n{0}", HT(new BigNumber("20"), x, 0.05, 1));
          Console.WriteLine("双样本区间估计{0}", ParaEasti2( x1,x1,0.05, 1));
          Console.WriteLine("双样本假设检验 {0}", HT2(new BigNumber("-10"),x1, x1, 0.05, 1));
          Console.WriteLine("协方差= {0}",Corr(x1,x2));
          Console.WriteLine("比例区间估计{0}", ProportionEasti(x1, new BigNumber("3"), 0.05, 2));

          Console.WriteLine("比例假设检验{0}", ProportionHT2(new BigNumber("0"), x1,x2, new BigNumber("2"), 0.05, 2));*/
            Console.WriteLine("{0}", NORMSDIST(1.96));
            Console.ReadKey();
        }
    }
}
