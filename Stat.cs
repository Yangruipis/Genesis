using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 统计工具测试1
{
    public class Stat
    {
        //对BigNumber数组求均值。将所有数字加起来除以n。
        //调用时参数为BigNumber数组，返回为BigNumber值。
        public static BigNumber Mean(BigNumber[] NumberSeries)
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
        //输入为BigNumber数组，返回为BigNumber值
        //n - 1个自由度
        public static BigNumber Variance(BigNumber[] NumberSeries)
        {
            BigNumber sum = new BigNumber("0");
            int len = NumberSeries.Length;
            BigNumber mean_series = Mean(NumberSeries);
            foreach (BigNumber SingleNumber in NumberSeries)
            {
                sum += (SingleNumber - mean_series).Power(new BigNumber("2"), 30);
            }
            return sum / new BigNumber((len - 1).ToString());
        }
        //sort功能可以对BigNumber数组进行排序。没有返回值。
        //如果有需要之后可以修改这个环节，使之返回新数组。
        public static void Sort(int n, BigNumber[] NumberSeries)
        {
            BigNumber temp = new BigNumber("0");
            if (n <= 1)
            {
                return;
            }
            for (int i = 0; i < n - 1; i++)
            {
                if (CompareNumber.Compare(NumberSeries[i], NumberSeries[i + 1]) == 1)
                {
                    temp = NumberSeries[i + 1];
                    NumberSeries[i + 1] = NumberSeries[i];
                    NumberSeries[i] = temp;
                }
                Sort(n - 1, NumberSeries);
            }
        }
        //Quantile用来求分位数
        //输入BigNumber数组，以及所需分位数的位置
        //位置为0～1之间的小数
        public static BigNumber Quantile(BigNumber[] NumberSeries, double quan)
        {
            int len = NumberSeries.Length;
            double position = quan * (double)len;
            position = Convert.ToDouble(MathV.round(position.ToString(), 0, 0));
            return NumberSeries[Convert.ToInt32(position - 1)];
        }
        //仅限于寻找有序数列中的众数
        //多个众数时返回最小的众数
        public static string Mode(BigNumber[] NumberSeries)
        {

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
        //下面是分位数计算
        //分位数计算统一用double
        //Beta的累积密度函数，a，b为自由度
        //x在0～1之间
        public static double BetaCDF(double x, double a, double b)
        {
            int m, n;
            double I = 0, U = 0;
            double ta = 0, tb = 0;
            m = (int)(2 * a);
            n = (int)(2 * b);
            if (m % 2 == 1 && n % 2 == 1)
            {
                ta = 0.5;
                tb = 0.5;
                U = Math.Sqrt(x * (1.0 - x)) / Math.PI;
                I = 1.0 - 2.0 / Math.PI * Math.Atan(Math.Sqrt((1.0 - x) / x));
            }
            else if (m % 2 == 1 && n % 2 == 0)
            {
                ta = 0.5;
                tb = 0.1;
                U = 0.5 * Math.Sqrt(x) * (1.0 - x);
                I = Math.Sqrt(x);
            }
            else if (m % 2 == 0 && n % 2 == 1)
            {
                ta = 1;
                tb = 0.5;
                U = 0.5 * x * Math.Sqrt(1.0 - x);
                I = 1.0 - Math.Sqrt(1.0 - x);
            }
            else if (m % 2 == 0 && n % 2 == 0)
            {
                ta = 1;
                tb = 1;
                U = x * (1.0 - x);
                I = x;
            }
            while (ta < a)
            {
                I = I - U / ta;
                U = (ta + tb) / ta * x * U;
                ta++;
            }
            while (tb < b)
            {
                I = I + U / tb;
                U = (ta + tb) / tb * (1.0 - x) * U;
                tb++;
            }
            return I;
        }
        //计算t分布的累积密度函数
        //v为自由度
        public static double TDIST(double x, int v)
        {
            double t, prob;
            t = v / (v + x * x);
            if (x > 0)
            {
                prob = 1 - 0.5 * BetaCDF(t, v / 2.0, 0.5);
            }
            else
            {
                prob = 0.5 * BetaCDF(t, v / 2.0, 0.5);
            }
            return prob;
        }
        //计算F的累积密度函数
        //m，n为两个自由度
        public static double FCDF(double x, int m, int n)
        {
            double y, prob;
            if (x <= 0)
            {
                return 0;
            }
            y = m * x / (n + m * x);
            prob = BetaCDF(y, m / 2.0, n / 2.0);
            return prob;
        }
        //二项分布的累积密度函数
        //事件发生的概率为p
        public static double BinomialCDF(double x, double p, int n)
        {
            double prob = 0.0;
            if (x < 0)
            {
                prob = 0.0;
                return prob;
            }
            else if (x >= n)
            {
                prob = 1.0;
                return prob;
            }
            else
            {
                prob = BetaCDF(1.0 - p, n - (int)x, (int)x + 1);
                return prob;
            }

        }
        //Beta函数的分位数
        //af为概率
        //a，b为自由度
        //返回分位数
        public static double BetaUa(double af, double a, double b)
        {
            int MaxTime = 500;
            int times = 0;
            double x1, x2, xn = 0.0;
            double f1, f2, fn, ua;
            double eps = 1.0e-10;
            x1 = 0.0;
            x2 = 1.0;
            f1 = BetaCDF(x1, a, b) - (1.0 - af);
            f2 = BetaCDF(x2, a, b) - (1.0 - af);
            while (Math.Abs((x2 - x1) / 2.0) > eps)
            {
                xn = (x1 + x2) / 2.0;
                fn = BetaCDF(xn, a, b) - (1.0 - af);
                if (f1 * fn < 0)
                {
                    x2 = xn;
                }
                else if (fn * f2 < 0)
                {
                    x1 = xn;
                }
                f1 = BetaCDF(x1, a, b) - (1.0 - af);
                f2 = BetaCDF(x2, a, b) - (1.0 - af);
                times++;
                if (times > MaxTime)
                {
                    break;
                }
            }
            ua = xn;
            return ua;
        }
        //T分布的分位数
        //af为概率
        public static double TINV(double af, int v)
        {
            double ua = 0.0, tbp, bf;
            bf = 1 - af;
            if (af <= 0.5)
            {
                tbp = BetaUa(1 - 2 * af, v / 2.0, 0.5);
                ua = Math.Sqrt(v / tbp - v);
            }
            else if (af > 0.5)
            {
                tbp = BetaUa(1 - 2 * (1.0 - af), v / 2.0, 0.5);
                ua = -Math.Sqrt(v / tbp - v);
            }
            return ua;
        }
        //F分布的分位数
        //上侧概率分位数
        public static double FdistUa(double af, int m, int n)
        {
            double ua, tbp, bf;
            bf = 1 - af;
            tbp = BetaUa(af, m / 2.0, n / 2.0);
            ua = n * tbp / (m * (1.0 - tbp));
            return ua;
        }
        //计算卡方分布累积密度函数
        public static double chi2(double x, int Freedom)
        {
            int k, n;
            double f, h, prob;
            k = Freedom % 2;
            if (k == 1)
            {
                f = Math.Exp(-x / 2.0) / Math.Sqrt(2 * Math.PI * x);
                h = 2.0 * GaossFx1(Math.Sqrt(x)) - 1.0;
                n = 1;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            else
            {
                f = Math.Exp(-x / 2.0) / 2.0;
                h = 1.0 - Math.Exp(-x / 2.0);
                n = 2;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            prob = h;
            return prob;
        }
        //这个函数一般无需调用
        public static double chi21(double x, int Freedom)
        {
            int k, n;
            double f, h, prob;
            k = Freedom % 2;
            if (k == 1)
            {
                f = Math.Exp(-x / 2.0) / Math.Sqrt(2 * Math.PI * x);
                h = 2.0 * GaossFx1(Math.Sqrt(x)) - 1.0;
                n = 1;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            else
            {
                f = Math.Exp(-x / 2.0) / 2.0;
                h = 1.0 - Math.Exp(-x / 2.0);
                n = 2;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            prob = h;
            return prob;
        }
        //Possion分布的累积密度函数
        public static double PossionCDF(double x, double p)
        {
            double prob = 0.0;
            prob = 1.0 - chi21(2 * p, 2 * ((int)x) + 1);
            return prob;
        }
        //卡方分布的上侧分位数的计算  
        public static double chi2Ua(double af, int Freedom)
        {
            int times;
            int MaxTime = 500;
            double eps = 1.0e-10;
            double ua, x0, xn, bf;
            bf = 1 - af;
            x0 = chi2Ua0(af, Freedom);
            if (Freedom == 1 || Freedom == 2)
            {
                ua = x0;
            }
            else
            {
                times = 1;
                xn = x0 - (chi21(x0, Freedom) - 1 + af) / chi2Px(x0, Freedom);
                while (Math.Abs(xn - x0) > eps)
                {
                    x0 = xn;
                    xn = x0 - (chi21(x0, Freedom) - 1 + af) / chi2Px(x0, Freedom);
                    times++;
                    if (times > MaxTime) break;
                }
                ua = xn;
            }
            return ua;
        }
        //这个函数一般无需调用
        public static double chi2Ua0(double af, int Freedom)
        {
            double ua, p, temp;
            if (Freedom == 1)
            {
                p = 1.0 - (1.0 - af + 1.0) / 2.0;
                temp = NORMSINV(p);
                ua = temp * temp;
            }
            else if (Freedom == 2)
            {
                ua = -2.0 * Math.Log(af);
            }
            else
            {
                temp = 1.0 - 2.0 / (9.0 * Freedom) + Math.Sqrt(2.0 / (9.0 * Freedom)) * NORMSINV(af);
                ua = Freedom * (temp * temp * temp);
            }
            return ua;
        }
        //卡方分布的密度函数  
        public static double chi2Px(double x, int Freedom)
        {
            double p, g;
            if (x <= 0) return 0.0;
            g = Gama(Freedom);
            p = 1.0 / Math.Pow(2.0, Freedom / 2.0) / g * Math.Exp(-x / 2.0) * Math.Pow(x, Freedom / 2.0 - 1.0);
            return p;
        }
        public static double Gama(int n)//伽马分布函数Gama(n/2)  
        {
            double g;
            int i, k;
            k = n / 2; if (n % 2 == 1)
            {
                g = Math.Sqrt(Math.PI) * 0.5;
                for (i = 1; i < k; i++)
                    g *= (i + 0.5);
            }
            else
            {
                g = 1.0;
                for (i = 1; i < k; i++)
                    g *= i;
            }
            return g;
        }
        //高斯函数
        public static double GaossFx1(double x)
        {
            double prob = 0, t, temp;
            int i, n, symbol;
            temp = x;
            if (x < 0)
                x = -x;
            n = 28;
            if (x >= 0 && x <= 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                {
                    if (i % 2 == 1) symbol = -1;
                    else symbol = 1;
                    t = symbol * i * x * x / (2.0 * i + 1.0 + t);
                }
                prob = 0.5 + GaossPx(x) * x / (1.0 + t);
            }
            else if (x > 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                    t = 1.0 * i / (x + t);
                prob = 1 - GaossPx(x) / (x + t);
            }
            x = temp;
            if (x < 0)
                prob = 1.0 - prob; return prob;
        }
        public static double GaossFx(double x)//正态分布函数的计算  
        {
            double prob = 0, t, temp;
            int i, n, symbol;
            temp = x;
            if (x < 0)
                x = -x;
            n = 28;//连分式展开的阶数  
            if (x >= 0 && x <= 3.0)//连分式展开方法  
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                {
                    if (i % 2 == 1) symbol = -1;
                    else symbol = 1;
                    t = symbol * i * x * x / (2.0 * i + 1.0 + t);
                }
                prob = 0.5 + GaossPx(x) * x / (1.0 + t);
            }
            else if (x > 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                    t = 1.0 * i / (x + t);
                prob = 1 - GaossPx(x) / (x + t);
            }
            x = temp;
            if (x < 0)
                prob = 1.0 - prob;
            return prob;
        }
        public static double GaossPx(double x)//正态分布的密度函数  
        {
            double f;
            f = 1.0 / Math.Sqrt(2.0 * Math.PI) * Math.Exp(-x * x / 2.0);
            return f;
        }
        //计算正态分布的分位数
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
            Console.WriteLine("NORMDIST测试:{0}", val);
            return val;
        }
        //CI = Confidence Interval
        //置信区间的计算，返回string
        //CI1为单样本估计
        //len 为样本数
        //Tail = "less"为左单尾检验，Tail = "greater"为右单尾。Tail = "two" 为双尾检验
        //对于无需使用的值赋为-1即可，比如均值估计时无需使用比例，则赋值为-1。
        //type = "Mean.Esti"为均值估计
        //type = "Proportion.Esti"为比例估计
        //返回为字符串，如： 3.5,7.6  以逗号分隔
        //如果没有输入正确的type则返回NA
        public static string CI1(BigNumber Mean, BigNumber Variance, BigNumber Proportion,BigNumber len, double Significance, string Tail, string type)
        {
            Tail = Tail.ToLower();
            if (type == "Mean.Esti")
            {
                //均值估计
                BigNumber std = Variance.Power(new BigNumber("0.5"), 30);
                if (Tail == "two") { 
                Significance = Significance / 2;
                BigNumber lower = Mean - new BigNumber(NORMSINV(Significance).ToString()) * std / len.Power(new BigNumber("0.5"), 30);
                BigNumber upper = Mean + new BigNumber(NORMSINV(Significance).ToString()) * std / len.Power(new BigNumber("0.5"), 30);
                return lower.ToString()+ ","+upper.ToString();
                }
                else
                {
                    if (Tail == "greater") //H0:mu>0
                    {
                        Significance = Significance / 1;
                        BigNumber lower = Mean - new BigNumber(NORMSINV(Significance).ToString()) * std / len.Power(new BigNumber("0.5"), 30);
                        return lower.ToString() + ",";
                    }
                    else
                    {
                        if (Tail == "less") //H0:mu>0
                        {
                            Significance = Significance / 1;
                            BigNumber upper = Mean + new BigNumber(NORMSINV(Significance).ToString()) * std / len.Power(new BigNumber("0.5"), 30);
                            return "," + upper.ToString();
                        }
                        else
                        {
                            return "NA";
                        }
                    }
                    
                }
                
              }
           
            else if (type == "Proportion.Esti")
            {
                BigNumber std = Variance.Power(new BigNumber("0.5"), 30);
                if (Tail == "two")
                {
                    Significance = Significance / 2;
                    BigNumber lower = Proportion - new BigNumber(NORMSINV(Significance).ToString()) * ((Proportion * (new BigNumber("1") - Proportion)) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                    BigNumber upper = Proportion + new BigNumber(NORMSINV(Significance).ToString()) * (Proportion * (new BigNumber("1") - Proportion) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                    return lower.ToString() + "," + upper.ToString();
                }
                else
                {
                    if (Tail == "greater") //H0:mu>0
                    {
                        Significance = Significance / 1;
                        BigNumber lower = Proportion - new BigNumber(NORMSINV(Significance).ToString()) * ((Proportion * (new BigNumber("1") - Proportion)) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                        return lower.ToString() + ",";
                    }
                    else
                    {
                        if (Tail == "less") //H0:mu>0
                        {
                            Significance = Significance / 1;
                            BigNumber upper = Proportion + new BigNumber(NORMSINV(Significance).ToString()) * ((Proportion * (new BigNumber("1") - Proportion)) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                            return "," + upper.ToString();
                        }
                        else
                        {
                            return "NA";
                        }
                    }
                    
                }
                
            }
            else
            {
                return "NA";
            }
        }
        //CI = Confidence Interval
        //置信区间的计算，返回string
        //CI2为双样本估计
        //len1,len2为两样本长度
        //Tail = "less"为左单尾估计，Tail = "greater"为右单尾。Tail = "two" 为双尾估计
        //对于无需使用的值赋为-1即可，比如均值估计时无需使用比例，则赋值为-1。
        //type = "Mean.Esti"为均值估计
        //type = "Proportion.Esti"为比例估计
        //type = "Variance.Esti"为方差比估计
        //返回为字符串，如： 3.5,7.6  以逗号分隔
        //如果没有输入正确的type则返回NA
        public static string CI2(BigNumber Mean1, BigNumber Mean2, BigNumber Variance1, BigNumber Variance2, BigNumber Proportion1, BigNumber Proportion2,BigNumber len1,BigNumber len2, double Significance, string Tail, string type)
        {
            BigNumber one = new BigNumber("1");
            BigNumber S_p = ((len1 - one) * Variance1 + (len2 - one) * Variance2) / (len1 + len2 - one - one);
            Tail = Tail.ToLower();
            if (type == "Mean.Esti")
            {   //均值估计
                if(Tail == "two"){
                    Significance = Significance / 2;
                    BigNumber lower = Mean1 - Mean2 - new BigNumber(NORMSINV(Significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("0.5"), 30);
                    BigNumber upper = Mean1 - Mean2 + new BigNumber(NORMSINV(Significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("0.5"), 30);
                    return lower.ToString() + "," + upper.ToString();
                }
                else
                {
                    if(Tail == "greater"){
                        Significance = Significance / 1;
                        BigNumber lower = Mean1 - Mean2 - new BigNumber(NORMSINV(Significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("0.5"), 30);
                        return lower.ToString() + ",";
                    }
                    else
                    {
                        if(Tail == "less"){
                            Significance = Significance / 1;
                            BigNumber upper = Mean1 - Mean2 + new BigNumber(NORMSINV(Significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("0.5"), 30);
                            return  "," + upper.ToString();
                        }
                        else
                        {
                            return "NA";
                        }
                    }
                }
             
            }
            else if (type == "Proportion.Esti")
            {
                //比例估计
                if (Tail == "two")
                {
                    Significance = Significance / 2;
                    BigNumber lower = Proportion1 - Proportion2 - new BigNumber(NORMSINV(Significance).ToString()) * (Proportion1 * ((new BigNumber("1") - Proportion1) / len1 + Proportion2 * (new BigNumber("1") - Proportion2) / len2)).Power(new BigNumber("0.5"));
                    BigNumber upper = Proportion1 - Proportion2 + new BigNumber(NORMSINV(Significance).ToString()) * (Proportion1 * ((new BigNumber("1") - Proportion1) / len1 + Proportion2 * (new BigNumber("1") - Proportion2) / len2)).Power(new BigNumber("0.5"));
                    return lower.ToString() + "," + upper.ToString();
                }
                else
                {
                    if(Tail == "greater"){
                        Significance = Significance / 1;
                        BigNumber lower = Proportion1 - Proportion2 - new BigNumber(NORMSINV(Significance).ToString()) * (Proportion1 * ((new BigNumber("1") - Proportion1) / len1 + Proportion2 * (new BigNumber("1") - Proportion2) / len2)).Power(new BigNumber("0.5"));
                        return lower.ToString() + ",";
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                        Significance = Significance / 1;
                        BigNumber upper = Proportion1 - Proportion2 + new BigNumber(NORMSINV(Significance).ToString()) * (Proportion1 * ((new BigNumber("1") - Proportion1) / len1 + Proportion2 * (new BigNumber("1") - Proportion2) / len2)).Power(new BigNumber("0.5"));
                        return "," + upper.ToString();
                         }
                        else
                        {
                            return "NA";
                        }
                    }
                }
             
            }
            else if (type == "Variance.Esti")
            {
                //方差估计
                if (Tail == "two")
                {
                    Significance =Significance /2;
                    BigNumber lower = (Variance1 / Variance2) / new BigNumber(FCDF(Significance, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString());
                    BigNumber upper = (Variance1 / Variance2) * new BigNumber(FCDF(Significance, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString());
                    return lower.ToString() + "," + upper.ToString();
                }
                else
                {
                    if (Tail == "greater")
                    {
                        Significance = Significance / 1;
                        BigNumber lower = (Variance1 / Variance2) / new BigNumber(FCDF(Significance, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString());
                        return lower.ToString() + ",";
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            Significance = Significance / 1;
                            BigNumber upper = (Variance1 / Variance2) * new BigNumber(FCDF(Significance, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString());
                            return "," + upper.ToString();
                        }
                        else
                        {
                            return "NA";
                        }
                    }
                }
            }
            else
            {
                return "NA";
            }
        }
        //HT = Hypothesis Testing
        //假设检验，返回string
        //HT1为单样本检验
        //Tail = "less"为左单尾检验，Tail = "greater"为右单尾。Tail = "two" 为双尾检验
        //对于无需使用的值赋为-1即可，比如均值检验时无需使用比例，则赋值为-1。
        //type = "Mean.Test"为均值检验
        //type = "Proportion.Test"为比例检验
        //返回为字符串，如： 3.5,7.6  以逗号分隔
        //如果没有输入正确的type则返回NA
        public static string HT1(BigNumber H0, BigNumber Mean, BigNumber Variance, BigNumber Proportion,BigNumber len, double Significance, string Tail, string type)
        {
            Tail = Tail.ToLower();
            if (type == "Mean.Test")
            {
                //均值检验
                BigNumber tvalue = (H0 - Mean) / ((Variance / len).Power(new BigNumber("0.5"), 10));
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(),10,0)).ToString());
                string result = CI1(Mean,Variance,Proportion,len,Significance,Tail,type);
                char[] separator = { ',' };
                string[] intervals = result.Split(separator);
                if (Tail == "two")
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return tvalue.ToString()+ "," + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    if (Tail == "greater")
                    {
                        BigNumber lower = new BigNumber(intervals[0]);
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            BigNumber upper = new BigNumber(intervals[1]);
                            if (CompareNumber.Compare(upper, H0) == 1)
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }
                        }
                        else
                        {
                            return "null";
                        }
                    }
                }
            }
            else if (type == "Proportion.Test")
            {
                //比例检验
                string result = CI1(Mean, Variance, Proportion, len, Significance, Tail, type);
                char[] separator = { ',' };
                string[] intervals = result.Split(separator);
                BigNumber tvalue = (Proportion - H0) / ((H0 * (new BigNumber("1") - H0) / len).Power(new BigNumber("0.5"), 10));
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 10, 0)).ToString());
                if (Tail == "two")
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    if (Tail == "greater")
                    {
                        BigNumber lower = new BigNumber(intervals[0]);
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            BigNumber upper = new BigNumber(intervals[1]);
                            if (CompareNumber.Compare(upper, H0) == 1)
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }
                        }
                        else
                        {
                            return "null";
                        }
                    }
                }
            }
            else
            {
                return "NA";
            }
        }
        //HT = Hypothesis Testing
        //假设检验，返回string
        //HT2为双样本检验
        //Tail = "less"为左单尾检验，Tail = "greater"为右单尾。Tail = "two" 为双尾检验
        //对于无需使用的值赋为-1即可，比如均值检验时无需使用比例，则赋值为-1。
        //type = "Mean.Test"为均值检验
        //type = "Proportion.Test"为比例检验
        //返回为字符串，如： 3.5,7.6  以逗号分隔
        //如果没有输入正确的type则返回NA
        public static string HT2(BigNumber H0, BigNumber Mean1, BigNumber Mean2, BigNumber Variance1, BigNumber Variance2, BigNumber Proportion1, BigNumber Proportion2,BigNumber len1,BigNumber len2, double Significance, string Tail, string type)
        {
            Tail = Tail.ToLower();
            if (type == "Mean.Test")
            {
                //均值检验
                BigNumber one = new BigNumber("1");
                BigNumber S_p = ((len1 - one) * Variance1 + (len2 - one) * Variance2) / (len1 + len2 - one - one);
                BigNumber tvalue = (H0 - (Mean1 - Mean2)) / S_p.Power(new BigNumber("0.5"), 30);
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 10, 0)).ToString());
                string result = CI2(Mean1,Mean2, Variance1,Variance2, Proportion1,Proportion2, len1,len2, Significance, Tail, type);
                char[] separator = { ',' };
                string[] intervals = result.Split(separator);
                if (Tail == "two")
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    if (Tail == "greater")
                    {
                         BigNumber lower = new BigNumber(intervals[0]);
                         if (CompareNumber.Compare(lower, H0) == -1)
                         {
                           return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                         }
                         else
                         {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                         }
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            BigNumber upper = new BigNumber(intervals[1]);
                            if (CompareNumber.Compare(upper, H0) == -1)
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }
                        }
                        else
                        {
                            return "null";
                        }
                    }
                }
            }
            else if (type == "Proportion.Test")
            {
                //比例检验
                BigNumber pbar = (len1*Proportion1 + len2* Proportion2) / (len1 + len2);
                BigNumber tvalue = (Proportion1 - Proportion2 - H0) / (pbar * (new BigNumber("1") - pbar) * (new BigNumber("1") / len1 + new BigNumber("1") / len2)).Power(new BigNumber("0.5"), 10);
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 10, 0)).ToString());
                string result = CI2(Mean1, Mean2, Variance1, Variance2, Proportion1, Proportion2, len1, len2, Significance, Tail, type);
                char[] separator = { ',' };
                string[] intervals = result.Split(separator);
                if (Tail == "two")
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return tvalue.ToString() + "," + pvalue.ToString() + "," + "拒绝原假设";
                    }

                }
                else
                {
                    if (Tail == "greater")
                    {
                        BigNumber lower = new BigNumber(intervals[0]);
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }

                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            BigNumber upper = new BigNumber(intervals[1]);
                            if (CompareNumber.Compare(upper, H0) == -1)
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return tvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }

                        }
                        else
                        {
                            return "null";
                        }
                    }
                }
            }
            else if (type == "Variance.Test")
            {
                //方差比检验
                BigNumber Fvalue = Variance1 / Variance2;
                Double F_value = Convert.ToDouble(Fvalue.ToString());
                BigNumber pvalue = new BigNumber((MathV.round(FdistUa(F_value, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString(), 10, 0)).ToString());
                string result = CI2(Mean1, Mean2, Variance1, Variance2, Proportion1, Proportion2, len1, len2, Significance, Tail, type);
                char[] separator = { ',' };
                string[] intervals = result.Split(separator);
                if (Tail == "two")
                {
                    BigNumber lower = new BigNumber(intervals[0]);
                    BigNumber upper = new BigNumber(intervals[1]);
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return Fvalue.ToString() + "," + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return Fvalue.ToString() + "," + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    if (Tail == "greater")
                    {
                        BigNumber lower = new BigNumber(intervals[0]);
                        if (CompareNumber.Compare(lower, H0) == -1 )
                        {
                            return Fvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return Fvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else
                    {
                        if (Tail == "less")
                        {
                            BigNumber upper = new BigNumber(intervals[0]);
                            if (CompareNumber.Compare(upper, H0) == -1)
                            {
                                return Fvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return Fvalue.ToString() + "," + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }
                        }
                        else
                        {
                            return "null";
                        }
                    }
                }
            }
            else
            {
                return "NA";
            }
        }

        public static string Chisquare(BigNumber[] NumberSeries)
        {
            //正态分布卡方检验
            BigNumber mean = Mean(NumberSeries);
            BigNumber std = Variance(NumberSeries).Power(new BigNumber("0.5"), 30);
            int len = NumberSeries.Length;
            BigNumber[] StdNumber = new BigNumber[len];
            BigNumber[] normal = new BigNumber[21];
            BigNumber[] count = new BigNumber[21];
            for (int i = 0; i < len; i++)
            {
                StdNumber[i] = (NumberSeries[i] - mean) / std;
            }
            int sum = new int(); sum = 0;
            BigNumber chisquare = new BigNumber("0");
            for (Double i = -2.1; i < 2.1; i = i + 0.2)
            {
                sum = sum + 1;
                for (int j = 0; j < len; j++)
                {
                    if (CompareNumber.Compare(StdNumber[j], new BigNumber(i.ToString())) == 1 && CompareNumber.Compare(StdNumber[j], new BigNumber((i + 0.2).ToString())) == -1)
                    {
                        count[sum] = count[sum] + new BigNumber("1");
                    }
                }
            }

            for (int i = 1; i <= 21; i++)
            {
                normal[i] = len * TaylorFunction.Exp(new BigNumber(((i - 11) / 10).ToString()).Power(new BigNumber("2"), 30) / new BigNumber("2"), 30) / ((new BigNumber("2") * new BigNumber("3.1415926535897032384626")).Power(new BigNumber("0.5"), 30));
                chisquare += (count[i] - normal[i]).Power(new BigNumber("2"), 30) / normal[i];
            }
            return chisquare.ToString();

        }

        public static string OneWayANOVA(BigNumber[,] Numberseies)
        {   // 单因素方差分析
            int weidu = Numberseies.Rank;
            int x = Numberseies.GetLength(0);
            int y = Numberseies.GetLength(1);
            int z = Numberseies.Length;
            BigNumber SST = new BigNumber("0");
            BigNumber SSA = new BigNumber("0");
            BigNumber SSW = new BigNumber("0");
            BigNumber sum1 = new BigNumber("0");
            BigNumber[] sum2 = new BigNumber[y];
            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    sum1 += Numberseies[i, j];
                    sum2[j] += Numberseies[i, j];
                }
            }
            BigNumber Txbar = sum1 / new BigNumber(z.ToString());
            BigNumber[] Axbar = new BigNumber[y];
            for (int j = 0; j < y; j++)
            {
                Axbar[j] = sum2[j] / new BigNumber(y.ToString());
            }

            for (int j = 0; j < y; j++)
            {
                SSA += new BigNumber(y.ToString()) * (Axbar[j] - Txbar).Power(new BigNumber("0.5"), 30);
            }

            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    SST += (Numberseies[i, j] - Txbar).Power(new BigNumber("0.5"), 30);
                    SSW += (Numberseies[i, j] - Axbar[j]).Power(new BigNumber("0.5"), 30);
                }
            }
            BigNumber MSA = SSA / (new BigNumber(y.ToString()) - new BigNumber("1"));
            BigNumber MSW = SSW / (new BigNumber(z.ToString()) - new BigNumber(y.ToString()));
            BigNumber MST = SST / (new BigNumber(z.ToString()) - new BigNumber("1"));
            BigNumber Fvalue = MSA / MSW;
            return Fvalue.ToString();

        }
        public  static BigNumber Covariance(BigNumber[] NumberSeries1, BigNumber[] NumberSeries2)
        {
            //协方差计算
            BigNumber sum = new BigNumber("0");
            int len1 = NumberSeries1.Length;
            int len2 = NumberSeries2.Length;
            BigNumber mean_series1 = Mean(NumberSeries1);
            BigNumber mean_series2 = Mean(NumberSeries2);
            BigNumber E_xy = new BigNumber("0");

            for (int i = 0; i < len1; i++)
            {
                E_xy += NumberSeries1[i] * NumberSeries2[i];
            }
            BigNumber cov = E_xy / (new BigNumber(len1.ToString())) - mean_series1 * mean_series2;
            return cov;
        }

        public static BigNumber Corr(BigNumber[] NumberSeries1, BigNumber[] NumberSeries2)
        {
            //相关系数计算
            BigNumber cov = Covariance(NumberSeries1, NumberSeries2);
            BigNumber len1 = new BigNumber(NumberSeries1.Length.ToString());
            BigNumber var1 = Variance(NumberSeries1) * (len1 - new BigNumber("1")) / len1;
            BigNumber var2 = Variance(NumberSeries2) * (len1 - new BigNumber("1")) / len1;
            BigNumber Rho = cov / ((var1 * var2).Power(new BigNumber("0.5")));

            return Rho;
        }
    }
}