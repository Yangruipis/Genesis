using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 统计工具测试1
{
    class 假设检验2
    {
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
            return val;
        }
        static string HT1(BigNumber H0, BigNumber len ,BigNumber mean ,BigNumber variance,BigNumber proportion, double significance, int tail)
        {
            BigNumber sd_series = variance.Power(new BigNumber("0.5"), 30);
            if ( CompareNumber.Compare(proportion,new BigNumber("0")) == 0 )
            {
            BigNumber tvalue = (H0-mean)/((variance/len).Power(new BigNumber("0.5"),10));
            Double t_value = Convert.ToDouble(tvalue.ToString());
            BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());

            if(tail == 2){
    
                significance = significance / 2;
                BigNumber lower = mean - new BigNumber(NORMSINV(significance).ToString()) * sd_series / len.Power(new BigNumber("-0.5"), 30);
                BigNumber upper = mean + new BigNumber(NORMSINV(significance).ToString()) * sd_series / len.Power(new BigNumber("-0.5"), 30);
                return "["+ lower.ToString() + "," +upper.ToString() +"]";
                if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                {
                    return "t ="+ tvalue.ToString() +",P = " + pvalue.ToString() + "," + "不拒绝原假设";
                }
                else
                {
                    return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "拒绝原假设";
                }
            }
            else
            {
                significance = significance / 1;
                BigNumber lower = mean - new BigNumber(NORMSINV(significance).ToString()) * sd_series / (len.Power(new BigNumber("-0.5"), 30));
                if (tail == 1)
                {
                    return "["+ lower.ToString() + ",]";
                    if (CompareNumber.Compare(lower, H0) == -1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                        if(tail == -1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                        else{
                            return "请输入正确的值";
                        }
                }
                else
                {
                    BigNumber upper = mean + new BigNumber(NORMSINV(significance).ToString()) * sd_series / (len.Power(new BigNumber("-0.5"), 30));
                    return "[," + upper.ToString()+ "]";
                    if (CompareNumber.Compare(upper, H0) == 1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                    }
                }
            }
            }   
            else{
                BigNumber tvalue = (proportion - H0) / ((H0 * (new BigNumber("1") - H0) / len).Power(new BigNumber("0.5"), 10));
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());
            if (tail == 2)
                {
                    significance = significance / 2;
                    BigNumber lower = proportion - new BigNumber(NORMSINV(significance).ToString()) * ((proportion * (new BigNumber("1") - proportion)) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                    BigNumber upper = proportion + new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString()))).Power(new BigNumber("0.5"));
                    return "["+lower.ToString() + "," + upper.ToString()+"]";
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    significance = significance / 1;
                    if (tail == 1)
                    {

                        BigNumber lower = proportion - new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString())).Power(new BigNumber("0.5")));
                        return "["+lower.ToString() + ",]";
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else{
                        if(tail == -1)
                       {
                        BigNumber upper = proportion + new BigNumber(NORMSINV(significance).ToString()) * (proportion * (new BigNumber("1") - proportion) / (new BigNumber(len.ToString())).Power(new BigNumber("0.5")));
                        return "["+"," + upper.ToString()+ "]";
                        if (CompareNumber.Compare(upper, H0) == 1)
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                      }
                        else{
                            return "请输入正确的值";
                        }
                    }
                }

            }
        }

        static string HT2(BigNumber H0, BigNumber len1, BigNumber len2, BigNumber mean1, BigNumber mean2, BigNumber variance1, BigNumber variance2, BigNumber proportion1, BigNumber proportion2, double significance, int tail)
        {
            BigNumber one = new BigNumber("1");
 
            BigNumber S_p = ((len1 - one) * variance1 + (len2 - one) * variance2) / (len1 + len2 - one - one);
            if (CompareNumber.Compare(proportion1, new BigNumber("0")) == 0 && CompareNumber.Compare(proportion2, new BigNumber("0")) == 0)
            {
                BigNumber tvalue = (H0 - (mean1 - mean2)) / S_p.Power(new BigNumber("0.5"), 30);
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());

                if (tail == 2)
                {
                    significance = significance / 2;
                    BigNumber lower = mean1 - mean2 - new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("-0.5"), 30);
                    BigNumber upper = mean1 - mean2 + new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("-0.5"), 30);
                    return "[" + lower.ToString() + "," + upper.ToString() + "]";
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "拒绝原假设";
                    }

                }
                else
                {
                    significance = significance / 1;

                    if (tail == 1)
                    {
                        BigNumber lower = mean1 - mean2 - new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("-0.5"), 30);
                        return "[" + lower.ToString() + ",]";
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else
                    {
                        if (tail == -1)
                        {
                            BigNumber upper = mean1 - mean2 + new BigNumber(NORMSINV(significance).ToString()) * (S_p * (one / len1 + one / len2)).Power(new BigNumber("-0.5"), 30);
                            return "[," + upper.ToString() + "]";
                            if (CompareNumber.Compare(upper, H0) == 1)
                            {
                                return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                            }
                            else
                            {
                                return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                            }
                        }
                        else
                        {
                            return "请输入正确的值";
                        }
                    }
                }
            }

            else
            {
                BigNumber pbar = (proportion1 * len1 + proportion2 * len2) / (len1 + len2);
                BigNumber tvalue = (proportion1 - proportion2 - H0) / (pbar * (new BigNumber("1") - pbar) * (new BigNumber("1") / len1 + new BigNumber("1") / len2)).Power(new BigNumber("0.5"), 10);
                Double t_value = Convert.ToDouble(tvalue.ToString());
                BigNumber pvalue = new BigNumber(NORMSDIST(t_value).ToString());
                if (tail == 2)
                {
                    significance = significance / 2;
                    BigNumber lower = proportion1 - proportion2 - new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                    BigNumber upper = proportion1 - proportion2 + new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                    return "["+lower.ToString() + "," + upper.ToString()+"]";
                    if (CompareNumber.Compare(lower, H0) == -1 && CompareNumber.Compare(upper, H0) == 1)
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "不拒绝原假设";
                    }
                    else
                    {
                        return "t =" + tvalue.ToString() + ",P = " + pvalue.ToString() + "," + "拒绝原假设";
                    }
                }
                else
                {
                    significance = significance / 1;
                    if (tail == 1)
                    {

                        BigNumber lower = proportion1 - proportion2 - new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                        return "[" + lower.ToString() + ",]";
                        if (CompareNumber.Compare(lower, H0) == -1)
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                    else
                    {
                        BigNumber upper = proportion1 - proportion2 + new BigNumber(NORMSINV(significance).ToString()) * (proportion1 * (new BigNumber("1") - proportion1) / (new BigNumber(len1.ToString())) + proportion2 * (new BigNumber("1") - proportion2) / (new BigNumber(len2.ToString()))).Power(new BigNumber("0.5"));
                        return "[," + upper.ToString()+"]";
                        if (CompareNumber.Compare(upper, H0) == 1)
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "不拒绝原假设";
                        }
                        else
                        {
                            return "t =" + tvalue.ToString() + ",P = " + (new BigNumber("2") * pvalue - new BigNumber("1")).ToString() + "," + "拒绝原假设";
                        }
                    }
                }
            }

        }
    }
}
