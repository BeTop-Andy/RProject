using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;

namespace RProject
{
    class RCommand
    {
        /// <summary>
        /// 读数据到R中
        /// </summary>
        /// <param name="l">List&lt;int&gt;数据</param>
        /// <returns>R里的变量名</returns>
        public static string LoadingDataToR(List<int> l)
        {
            REngine re = REngine.GetInstanceFromID("R");

            string rComm = null;
            foreach (int i in l) {
                rComm += i + ",";
            }
            rComm = rComm.Substring(0, rComm.Length - 1);

            re.Evaluate("Index <- c(" + rComm + ")");
            return "Index";
        }

        /// <summary>
        /// 用R求平均值
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>平均值(2位小数)</returns>
        public static double Avg(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Math.Round(re.Evaluate("mean(" + varName + ")").AsNumeric()[0], 2);

        }

        /// <summary>
        /// 用R求方差
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>方差(2位小数)</returns>
        public static double Var(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Math.Round(re.Evaluate("var(" + varName + ")").AsNumeric()[0], 2);
        }

        /// <summary>
        /// 用R求最大值
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>最大值</returns>
        public static int Max(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("max(" + varName + ")").AsInteger()[0];
        }

        /// <summary>
        /// 用R求最小值
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>最小值</returns>
        public static int Min(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("min(" + varName + ")").AsInteger()[0];
        }

        /// <summary>
        /// 用R求中位数
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>中位数</returns>
        public static int Median(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("median(" + varName + ")").AsInteger()[0];
        }

        /// <summary>
        /// 用R求众数
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <returns>众数</returns>
        public static int Mode(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Convert.ToInt32(re.Evaluate("names(which.max(table(" + varName + ")))").AsCharacter()[0]);
        }

        public static void LoadingSmoothFunToR()
        {
            REngine re = REngine.GetInstanceFromID("R");
            /*R语言
                my_func_smooth <- function(data, smooth_length){
                    #初始化变量,增加第一个非平滑值保持长度。
                    my_smooth_data <- NULL;
                    for(i in 1:length(data)){
                        #处理大于平滑长度的数据
                        my_data_start <- (i-(smooth_length)+1);
                        #处理小于平滑长度的数据
                        if(i < smooth_length) my_data_start <- 1;
                        my_smooth_data <- c(my_smooth_data, mean(data[my_data_start:i]));
                    }
                    #返回平滑值
                    return (my_smooth_data);
                }*/
            string RSmoothStr =
                @"my_func_smooth <- function(data, smooth_length){
                      my_smooth_data <- NULL;
                      for(i in 1:length(data)){
                        my_data_start <- (i-(smooth_length)+1);
                        if(i < smooth_length) my_data_start <- 1;
                        my_smooth_data <- c(my_smooth_data, mean(data[my_data_start:i]));
                      }
                      return (my_smooth_data);
                    }";
            re.Evaluate(RSmoothStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        public static void SmoothByR(string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");
            re.Evaluate("Index <- my_func_smooth("+varName+",8)");
        }



        /// <summary>
        /// 用R画图
        /// </summary>
        /// <param name="type">类型
        /// 0：条形图
        /// 1：折线图
        /// 2：饼图
        /// 3：分布图
        /// 4：散点图
        /// </param>
        /// <param name="varName">R里的变量名</param>
        /// <param name="startIndex">可视范围开始</param>
        /// <param name="endIndex">可视范围结束</param>
        public static void DrawByR(int type, string varName, int startIndex, int endIndex)
        {
            REngine re = REngine.GetInstanceFromID("R");

            switch (Enum.GetName(typeof(typeEnum), type)) {
                case "柱形图":
                    re.Evaluate(string.Format("plot("+varName+",type=\"h\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
                case "折线图":
                    re.Evaluate(string.Format("plot("+varName+",type=\"l\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
                case "饼图":
                    re.Evaluate(string.Format("pie("+varName+")"));
                    break;
                case "分布图":
                    re.Evaluate(string.Format("plot(table("+varName+"[{0}:{1}]),ylab=\"value\")", startIndex, endIndex));
                    break;
                case "散点图":
                    re.Evaluate(string.Format("plot("+varName+",type=\"p\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
            }
        }



    }

    enum typeEnum
    {
        柱形图 = 0,
        折线图 = 1,
        饼图 = 2,
        分布图 = 3,
        散点图 = 4,
    };
}
