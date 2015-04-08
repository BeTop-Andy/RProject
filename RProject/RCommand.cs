using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDotNet;
using System.Windows;

namespace RProject
{
    class RCommand
    {
        /// <summary>
        /// 装载数据到R中
        /// </summary>
        /// <param name="l">List&lt;int&gt;数据</param>
        /// <param name="varName">R里的变量名</param>
        public static void LoadingDataToR(List<int> l, string varName)
        {
            REngine re = REngine.GetInstanceFromID("R");

            string rComm = null;
            foreach (int i in l) {
                rComm += i + ",";
            }
            rComm = rComm.Substring(0, rComm.Length - 1);

            re.Evaluate(varName + " <- c(" + rComm + ")");
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
            //RFun.txt      //平滑
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
            re.Evaluate(varName + " <- my_func_smooth(" + varName + ",8)");
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
                    re.Evaluate(string.Format("plot(" + varName + ",type=\"h\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
                case "折线图":
                    re.Evaluate(string.Format("plot(" + varName + ",type=\"l\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
                case "饼图":
                    re.Evaluate(string.Format("pie(" + varName + ")"));
                    break;
                case "分布图":
                    re.Evaluate(string.Format("plot(table(" + varName + "[{0}:{1}]),ylab=\"value\")", startIndex, endIndex));
                    break;
                case "散点图":
                    re.Evaluate(string.Format("plot(" + varName + ",type=\"p\",ylab=\"value\",xlim=c({0},{1}))", startIndex, endIndex));
                    break;
            }
        }

        public static void LoadingCrossFunToR()
        {
            REngine re = REngine.GetInstanceFromID("R");

            //RFun.txt      //同比
            string tongbiString = 
@"my_func_cross <- function(my_smooth_data, total_days, compare_dates){
  my_cross_mean <- vector(length = 0);
  my_cross_sd <- vector(length = 0);
  my_average <- NULL; 
  for(i in 4:total_days){
    for(j in 1:24){
      my_cross_seq <- seq(from=(((i-1)-compare_dates)*24+j),length.out=compare_dates, by=24);
      if( i < (compare_dates+1)) my_cross_seq <- seq(from=j, length.out=(i-1), by=24);
      my_cross_data <- my_smooth_data[my_cross_seq];
      my_current_data <- my_smooth_data[(i-1)*24+j];
      
      my_average <- mean(my_cross_data);
      my_sd <- sd(my_cross_data);
      my_cross_mean <- c(my_cross_mean, (my_current_data - my_average)/my_average);
      my_cross_sd <- c(my_cross_sd, (my_current_data - my_average)/my_sd);
    }
  }
  my_cross_mean <- c(rep(0,times=24*3), my_cross_mean);
  my_cross_sd <- c(rep(0,times=24*3), my_cross_sd);
  my_cross_mean[(!is.finite(my_cross_mean))] = 0;
  my_cross_sd[(!is.finite(my_cross_sd))] = 0;
  return (list(my_cross_mean, my_cross_sd));
}";
            re.Evaluate(tongbiString);
        }

        /// <summary>
        /// 用R计算同比，并画图
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        /// <param name="total_days">总天数</param>
        /// <param name="compare_dates">要对比的天数（向前x天）</param>
        /// <param name="xMin">X轴可视范围开始</param>
        /// <param name="xMax">X轴可视范围结束</param>
        public static void CrossAndDrawByR(string varName, int total_days, int compare_dates, int xMin, int xMax)
        {
            REngine re = REngine.GetInstanceFromID("R");

            GenericVector gv = re.Evaluate(string.Format("List <- my_func_cross({0},{1},{2})", varName, total_days, compare_dates)).AsList();

            double yMax = gv[0].AsNumeric().Max();
            if (gv[0].AsNumeric().Max() < gv[1].AsNumeric().Max()) {
                yMax = gv[1].AsNumeric().Max();
            }
            double yMin = gv[0].AsNumeric().Min();
            if (gv[0].AsNumeric().Min() > gv[1].AsNumeric().Min()) {
                yMin = gv[1].AsNumeric().Min();
            }

            re.Evaluate(string.Format("plot(List[[1]],type=\"l\",ylim=c({0},{1}),xlim=c({2},{3}),ylab=\"value\",col=\"blue\")",yMin-1,yMax+1,xMin,xMax));
            re.Evaluate("lines(List[[2]],col=\"red\")");
            string legendStr = "legend(\"topleft\",legend=c(\"By AVG\",\"By SD\"),col=c(\"blue\",\"red\"),lty=1,lwd=1,cex=1)";
            re.Evaluate(legendStr);
        }

        public static void LoadingHuanBiFunToR()
        {
            REngine re = REngine.GetInstanceFromID("R");

            string str = 
                //RFun.txt      //环比
@"my_func_huanbi <- function (smoothData, startIndex, endIndex) {
              nowData <- smoothData[startIndex:endIndex];
              preData <- NULL;
              count <- endIndex - startIndex + 1;
              preStartIndex <- startIndex - count;
              preEndIndex <- startIndex - 1;
              if (preStartIndex < 0) {
                 preData[1:(abs(preStartIndex) + 1)] <- 0;
                 for (i in 1:preEndIndex) {
                     preData[abs(preStartIndex) + 1 + i] <- smoothData[i];
                 }
              } else {
                for (i in 1:count) {
                    preData[i] <- smoothData[preStartIndex+i-1];
                }
              }
              result <- (nowData-preData)/preData;

              return (result);
}";
            re.Evaluate(str);
        }

        public static void HuanBiAndDrawByR(string varName, int startIndex, int endIndex,int xMin, int xMax)
        {
            REngine re = REngine.GetInstanceFromID("R");

            re.Evaluate(string.Format("HuanBiResult <- my_func_huanbi({0},{1},{2})",varName,startIndex,endIndex));

            re.Evaluate(string.Format("plot(HuanBiResult,type=\"l\",ylab=\"value\",xlim=c({0},{1}))",xMin,xMax));
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
