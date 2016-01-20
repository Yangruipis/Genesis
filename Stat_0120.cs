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
                    BigNumber upper = (Variance1 / Variance2) * new BigNumber(FCDF(Significance, Convert.ToInt32(len2.ToString()) - 1, Convert.ToInt32(len1.ToString()) - 1).ToString());
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
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(),7,0)).ToString());//取前十位
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
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 7, 0)).ToString());
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
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 7, 0)).ToString());
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
                BigNumber pvalue = new BigNumber((MathV.round(NORMSDIST(t_value).ToString(), 7, 0)).ToString());
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
                BigNumber pvalue = new BigNumber((MathV.round(FdistUa(F_value, Convert.ToInt32(len1.ToString()) - 1, Convert.ToInt32(len2.ToString()) - 1).ToString(), 7, 0)).ToString());
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
            if(MathV.round(Rho.ToString(),10,0) == "-0.0000000000")
            {
                return new BigNumber("0");
            }
            else
            {
                return Rho;
            } 
        }

        //行列式值计算
        public static BigNumber MatValue(BigNumber[,] MatrixList, int Level)  //求得|A| 如果为0 说明不可逆
        { 

            BigNumber[,] dMatrix = new BigNumber[Level, Level];   //定义二维数组，行列数相同

            for (int i = 0; i < Level; i++)

                for (int j = 0; j < Level; j++)

                    dMatrix[i, j] = MatrixList[i, j];     //将参数的值，付给定义的数组


            BigNumber c, x;
            BigNumber k = new BigNumber("1");

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)
            {

                if (CompareNumber.Compare(dMatrix[i, j], new BigNumber("0")) == 0)   //判断对角线上的数据是否为0
                {

                    int m = i;

                    for (; CompareNumber.Compare(dMatrix[m, j], new BigNumber("0")) == 0; m++) ;  //如果对角线上数据为0，从该数据开始依次往后判断是否为0

                    if (m == Level)                      //当该行从对角线开始数据都为0 的时候 返回0

                        return new BigNumber("0");

                    else
                    {

                        // Row change between i-row and m-row

                        for (int n = j; n < Level; n++)
                        {

                            c = dMatrix[i, n];

                            dMatrix[i, n] = dMatrix[m, n];

                            dMatrix[m, n] = c;

                        }

                        // Change value pre-value

                        k = k * new BigNumber("-1");

                    }

                }

                // Set 0 to the current column in the rows after current row

                for (int s = Level - 1; s > i; s--)
                {

                    x = dMatrix[s, j];

                    for (int t = j; t < Level; t++)

                        dMatrix[s, t] -= dMatrix[i, t] * (x / dMatrix[i, j]);

                }

            }

            BigNumber sn = new BigNumber("1");

            for (int i = 0; i < Level; i++)
            {

                if (dMatrix[i, i] != new BigNumber("0"))

                    sn *= dMatrix[i, i];

                else

                    return new BigNumber("0");

            }

            return k * sn;

        }

        public static BigNumber[,] MatInv(BigNumber[,] dMatrix, int Level)

        {

            BigNumber dMatrixValue =MatValue(dMatrix, Level);

            if (CompareNumber.Compare(dMatrixValue, new BigNumber("0")) == 0) return null;       //A为该矩阵 若|A| =0 则该矩阵不可逆 返回空


            BigNumber[,] dReverseMatrix = new BigNumber[Level, 2 * Level];

            BigNumber x, c;

            // Init Reverse matrix

            for (int i = 0; i < Level; i++)     //创建一个矩阵（A|I） 以对其进行初等变换 求得其矩阵的逆

            {

                for (int j = 0; j < 2 * Level; j++)

                {

                   if (j < Level)

                        dReverseMatrix[i, j] = dMatrix[i, j];   //该 （A|I）矩阵前 Level列为矩阵A  后面为数据全部为0

                   else

                        dReverseMatrix[i, j] = new BigNumber("0");

                }

                dReverseMatrix[i, Level + i] = new BigNumber("1");


            }

 

            for (int i = 0, j = 0; i < Level && j < Level; i++, j++)

            {

                if (CompareNumber.Compare(dReverseMatrix[i, j],new BigNumber("0"))== 0)   //判断一行对角线 是否为0

                {

                    int m = i;

                    for (; CompareNumber.Compare(dMatrix[m, j],new BigNumber("0")) == 0; m++) ;

                    if (m == Level)

                        return null;  //某行对角线为0的时候 判断该行该数据所在的列在该数据后 是否为0 都为0 的话不可逆 返回空值

                    else

                   {

                        // Add i-row with m-row

                        for (int n = j; n < 2 * Level; n++)   //如果对角线为0 则该i行加上m行 m行为（初等变换要求对角线为1，0-->1先加上某行，下面在变1）

                            dReverseMatrix[i, n] += dReverseMatrix[m, n];

                    }

                }

                x = dReverseMatrix[i, j];

                if (x != new BigNumber("1"))                  //如果对角线元素不为1  执行以下

               {

                    for (int n = j; n < 2 * Level; n++)

                        if (dReverseMatrix[i, n] != new BigNumber("0"))

                            dReverseMatrix[i, n] /= x;   //相除  使i行第一个数字为1

                }

                // Set 0 to the current column in the rows after current row

                for (int s = Level - 1; s > i; s--)         //该对角线数据为1 时，这一列其他数据 要转换为0

                {

                    x = dReverseMatrix[s, j];

                    for (int t = j; t < 2 * Level; t++)

                        dReverseMatrix[s, t] -= (dReverseMatrix[i, t] * x);
            
                }

            }

            // Format the first matrix into unit-matrix

            for (int i = Level - 2; i >= 0; i--)                    

//处理第一行二列的数据 思路如上 就是把除了对角线外的元素转换为0 

           {

                for (int j = i + 1; j < Level; j++)

                    if (dReverseMatrix[i, j] != new BigNumber("0"))

                   {

                        c = dReverseMatrix[i, j];

                        for (int n = j; n < 2 * Level; n++)

                           dReverseMatrix[i, n] -= (c * dReverseMatrix[j, n]);                       

                    }

            }

            BigNumber[,] dReturn = new BigNumber[Level, Level];

            for (int i = 0; i < Level; i++)

                for (int j = 0; j < Level; j++)

                    dReturn[i, j] = dReverseMatrix[i, j + Level];  

            return dReturn;
          }

        private static BigNumber MatrixValue(BigNumber[,] dMatrix, BigNumber Level)
        {
            throw new NotImplementedException();
        }

       /* static void Main(string[] args)
       {
           BigNumber[,] dMatrix = new BigNumber[2, 2] { { new BigNumber("0"), new BigNumber("1") }, { new BigNumber("2"), new BigNumber("2") } };

           BigNumber[,] dReturn = MatInv(dMatrix,2);                              
           if (dReturn != null)  
             {
               for (int i = 0; i < 2; i++)
               Console.WriteLine(string.Format("{0} {1} ", dReturn[i, 0], dReturn[i, 1]));   //输出
               Console.ReadKey();
             }
        }*/

        public static BigNumber[,] MutiRegB(BigNumber[,] x,BigNumber[,] y)
        { //返回多元回归参数估计值**************************************注意：x第一列为1*******************
            int len11 = x.GetLength(0);//行数
            int len12 = x.GetLength(1);//列数
            BigNumber[,] b1 = MathV.MatTrans(x);
            BigNumber[,] b2 = MathV.MatTimes(b1, x);
            BigNumber[,] b3 = Stat.MatInv(b2, len12);
            BigNumber[,] b4 = MathV.MatTimes(b3,b1);
            BigNumber[,] bhat = MathV.MatTimes(b4, y);
            return bhat;
        }
        public static BigNumber[] MutiRegP(BigNumber[,] x, BigNumber[,] y)
        {    //返回多元回归P值
            int len11 = x.GetLength(0);//行数
            int len12 = x.GetLength(1);//列数
            int len21 = y.GetLength(1);//y列数
            if (len21 != 1)
            {
                return null;
            }
            BigNumber[,] b1 = MathV.MatTrans(x);
            BigNumber[,] b2 = MathV.MatTimes(b1, x);
            BigNumber[,] b3 = Stat.MatInv(b2, len12);
            BigNumber[,] b4 = MathV.MatTimes(b3, b1);
            BigNumber[,] bhat = MathV.MatTimes(b4, y);
            BigNumber[,] b5 = MathV.MatTimes(x, bhat);
            BigNumber[,] epsilon = MathV.MatMinu(y, b5);
            BigNumber[] variance = new BigNumber[len12];
            for (int i = 1; i < len11; i++)
            {
                variance[i] = epsilon[i, 1];
            }
            BigNumber sigma2 = Variance(variance) * new BigNumber(((len11 - 1) / len11).ToString());
            BigNumber[,] b6 = new BigNumber[len12, len12]; //sigma^2*(C^T C)^{-1} 参数方差
            for (int i = 0; i < len12; i++)
            {
                for (int j = 0; j < len12; j++)
                {
                    b6[i, j] = sigma2 * b3[i, j];
                }
            }
            BigNumber[] std_b = new BigNumber[len12];
            for (int i = 0; i < len12; i++)
            {
                std_b[i] = b6[i, i].Power(new BigNumber("0.5"));
            }
            BigNumber[] tvalue_b = new BigNumber[len12];
            for (int i = 0; i < len12; i++)
            {
                tvalue_b[i] = bhat[i, 1] / std_b[i];
            }
            BigNumber[] pvalue_b = new BigNumber[len12];
            for (int i = 0; i < len12; i++)
            {
                pvalue_b[i] = new BigNumber(Stat.NORMSDIST(Convert.ToDouble(tvalue_b[i].ToString())).ToString());
            }
            return pvalue_b;
             }
        public static string MutiRegR(BigNumber[,] x, BigNumber[,] y)
        {    //返回多元回归拟合优度R^2 and adj_R^2
            int len11 = x.GetLength(0);//x行数
            int len12 = x.GetLength(1);//x列数
            int len21 = y.GetLength(1);//y列数
            int len22 = y.GetLength(0);//y列数
            if (len21 != 1)
            {
                return null;
            }
            BigNumber ysum = new BigNumber("0");
            for (int i = 0; i < len11;i++ )
            {
                ysum += y[i,1];
            }
            BigNumber ybar = ysum / (new BigNumber(len11.ToString()));
            BigNumber TSS = new BigNumber("0");
            for (int i = 0; i < len11; i++)
            {
                TSS += (y[i, 1] - ybar).Power(new BigNumber("2"));
            }
            BigNumber[,] b1 = MathV.MatTrans(x);
            BigNumber[,] b2 = MathV.MatTimes(b1, x);
            BigNumber[,] b3 = Stat.MatInv(b2, len12);
            BigNumber[,] b4 = MathV.MatTimes(b3, b1);
            BigNumber[,] bhat = MathV.MatTimes(b4, y);
            BigNumber[,] b5 = MathV.MatTimes(x, bhat);
            BigNumber[,] epsilon = MathV.MatMinu(y, b5);
            BigNumber ESS = new BigNumber("0");
            for (int i = 0; i < len11; i++)
            {
                ESS += (epsilon[i, 1]).Power(new BigNumber("2"));
            }
            BigNumber MSS = TSS - ESS;
            BigNumber Rsquare = MSS / TSS;
            BigNumber Adj_Rsquare = new BigNumber("1") - (new BigNumber("1")- Rsquare)*(new BigNumber(len11.ToString()) -new BigNumber("1") / (new BigNumber(len11.ToString()) -new BigNumber(len12.ToString()) -new BigNumber("1")));
            BigNumber Fvalue = (MSS/new BigNumber(len12.ToString()))/(ESS/( new BigNumber(len11.ToString())- new BigNumber(len12.ToString()) - new BigNumber("1") ));
            return Rsquare.ToString() + "," + Adj_Rsquare.ToString() + ","+ Fvalue.ToString();
     
        }

        public static double[,] EigenValue(double[,] matrix, int nMaxIt, double eps)
        {
             int i, j, p = 0, q = 0, u, w, t, s, l;
             double fm, cn, sn, omega, x, y, d;
             int numColumns = matrix.GetLength(1);
             double[,] mtxEigenVector = new double[numColumns,numColumns];
             double[]  dblEigenValue = new double[numColumns];
              l = 1;
             for (i = 0; i <= numColumns - 1; i++)
            {
                mtxEigenVector[i ,i] = 1.0;//主对角线元素为1
                for (j = 0; j <= numColumns - 1; j++)
                    if (i != j)
                        mtxEigenVector[i , j] = 0.0;//其他元素为0
            }
             while (true)
             {
                 fm = 0.0;
                 for (i = 1; i <= numColumns - 1; i++)
                 {
                     for (j = 0; j <= i - 1; j++)
                     {
                         d = Math.Abs(matrix[i,j]);
                         if ((i != j) && (d > fm))//判断原矩阵左下最大值
                         {
                             fm = d;
                             p = i;
                             q = j;
                         }
                     }
                 }

                 if (fm < eps)
                 {
                     for (i = 0; i < numColumns; ++i)
                         dblEigenValue[i] = matrix[i,i]; //特征向量为原矩阵主对角线GetElement(i, i)
                     return null;
                 }

                 if (l > nMaxIt)
                     return null;

                 l = l + 1;
                 u = p * numColumns + q;//第p+1行第q+1列
                 w = p * numColumns + p;
                 t = q * numColumns + p;
                 s = q * numColumns + q;
                 x = -matrix[p,q];
                 y = (matrix[q,q] - matrix[p,p]) / 2.0;
                 omega = x / Math.Sqrt(x * x + y * y);

                 if (y < 0.0)
                     omega = -omega;

                 sn = 1.0 + Math.Sqrt(1.0 - omega * omega);
                 sn = omega / Math.Sqrt(2.0 * sn);
                 cn = Math.Sqrt(1.0 - sn * sn);
                 fm = matrix[p, p];
                 matrix[p, p] = fm * cn * cn + matrix[q, q] * sn * sn + matrix[p, q] * omega;
                 matrix[q, q] = fm * sn * sn + matrix[q, q] * cn * cn - matrix[p, q] * omega;
                 matrix[p, q] = 0.0;
                 matrix[q, p] = 0.0;
                 for (j = 0; j <= numColumns - 1; j++)
                 {
                     if ((j != p) && (j != q))
                     {
                         u = p * numColumns + j; w = q * numColumns + j;
                         fm = matrix[p, q];
                         matrix[p, q] = fm * cn + matrix[p,p] * sn;
                         matrix[p, p] = -fm * sn + matrix[p, p] * cn;
                     }
                 }

                 for (i = 0; i <= numColumns - 1; i++)
                 {
                     if ((i != p) && (i != q))
                     {
                         u = i * numColumns + p;
                         w = i * numColumns + q;
                         fm = matrix[p, q];
                         matrix[p, q] = fm * cn + matrix[p, p] * sn;
                         matrix[p, p] = -fm * sn + matrix[p, p] * cn;
                     }
                 }

                 for (i = 0; i <= numColumns - 1; i++)
                 {
                     u = i * numColumns + p;
                     w = i * numColumns + q;
                     fm = mtxEigenVector[p,q];
                     mtxEigenVector[p, q] = fm * cn + mtxEigenVector[p, p] * sn;
                     mtxEigenVector[p, p] = -fm * sn + mtxEigenVector[p, p] * cn;
                 }
             }
        }

        public static BigNumber[,] MatCorr(BigNumber[,] matrix)
        {
            int len1 = matrix.GetLength(0);
            int len2 = matrix.GetLength(1);
            BigNumber[] a1 = new BigNumber[len1 * len2];
            for (int j = 0; j < len2; j++)
            {
                for (int i = 0; i < len1; i++)
                {
                    a1[j * len1 + i] = matrix[i, j];
                }
            }
            BigNumber[] a2 = new BigNumber[len1];
            BigNumber[] a3 = new BigNumber[len1];
            BigNumber[,] corr = new BigNumber[len2, len2];
            for (int c = 1; c < len2 ;c++  )
            {
                for (int j = 0; j < len2 -1; j++)
                {
                    for (int i = 0; i < len1; i++)
                    {
                        a2[i] = a1[j * len1 + i];
                    }
                    for (int i = 0; i < len1; i++)
                    {
                        a3[i] = a1[(j +1)* len1 + i];
                    }
                    try
                    {
                        corr[j, j + c] = Corr(a2, a3);
                        corr[j + c, j] = Corr(a2, a3);
                    }
                    catch (Exception ex)
                    {
                        
                    }
              }
            }
            for (int i = 0; i < len2;i ++ )
            {
                corr[i, i] = new BigNumber("1");
            }
                return corr;
          }
         public static BigNumber Double2Big(Double x_bignumber)
        {
            string NumberStr = x_bignumber.ToString().Trim();
            int E_position = -1;
             int IsNegative = 0;
             int ScientificNotation = 0;
             string ScientificNumber ;
             BigNumber result = new BigNumber("0");
             string Scientificupper ;
             //0为正，1为负
             for (int i = 0;i < NumberStr.Length ;i++)
             {
                if (NumberStr[i] == 'E' || NumberStr[i]  == 'e')
                {
                    E_position = i;
                }
            }
             if (E_position != -1)
             {
                 ScientificNotation = NumberStr.Length - E_position - 1 - 1;
                 ScientificNumber = NumberStr.Substring(E_position + 1 + 1, ScientificNotation);
                 Scientificupper = NumberStr.Substring(0,E_position-1);
                 if (NumberStr[E_position + 1] == '-')
                 {
                     IsNegative = 1;
                     result = new BigNumber("-1") * new BigNumber(Scientificupper) * (new BigNumber("10").Power(new BigNumber(ScientificNumber)));
                     return result;
                 }
                 else{
                     result = new BigNumber("1") * new BigNumber(Scientificupper) * (new BigNumber("10").Power(new BigNumber(ScientificNumber)));
                     return result;
                 }
                 
             }
             else
             {
                 result = new BigNumber(NumberStr);
                 return result;
             }
        }

      //***********************    时间序列     ********************************
      //自相关矩阵
        public static BigNumber[,] TSCorr(BigNumber[] timeseries, int lag)
        {  
            int len  = timeseries.Length;
            int len2 = lag +1;//自相关矩阵维度
            BigNumber[,] corr = new BigNumber[len2,len2];
            BigNumber[,] Muti_timeseries = new BigNumber[len - lag, len2];
             for (int j = 0; j < len2; j++){
                  for (int i = 0; i < len - lag; i++)
                  {
                    Muti_timeseries[i,j] = timeseries[i+len2 - j-1];
                  }
             }
            corr = MatCorr(Muti_timeseries);
            return corr;
            }
        //平稳性检验
        public static string TStestQ(BigNumber[] timeseries, int lag)
        { 
            //Q检验，返回Q值和p值,P>0.05，则为白噪声序列
            BigNumber sum = new BigNumber("0");
            int len  = timeseries.Length;
            BigNumber[,] corr = TSCorr(timeseries,lag);
            for(int i = 1; i<= lag;i++)
            {
                sum += corr[1, i];
            }
            BigNumber Qtest = len * sum;
            Double Q_test = Convert.ToDouble(Qtest);
            BigNumber pvalue = Double2Big(chi2(Q_test, len));
            return Qtest.ToString() + "," + pvalue.ToString();
        }
        public static string TStestLB(BigNumber[] timeseries, int lag)
        {
            //LB检验，返回LB值和p值，P>0.05，则为白噪声序列
            BigNumber sum = new BigNumber("0");
            int len = timeseries.Length;
            BigNumber[,] corr = TSCorr(timeseries, lag);
            for (int i = 1; i <= lag; i++)
            {
                sum += corr[1, i].Power(new BigNumber("0.5"))/(new BigNumber((len-i).ToString()));
            }
            BigNumber LBtest = len* (len +2) * sum;
            Double LB_test = Convert.ToDouble(LBtest);
            BigNumber pvalue = Double2Big(chi2(LB_test, len));
            return LBtest.ToString() + "," + pvalue.ToString();
        }

        

          
       



    }
}