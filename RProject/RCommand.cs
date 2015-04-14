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
        public static double Avg(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Math.Round(re.Evaluate("mean(c(" + varName1 + "," + varName2 + "))").AsNumeric()[0], 2);
        }
        public static double Avg(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Avg(commStr);
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
        public static double Var(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Math.Round(re.Evaluate("var(c(" + varName1 + "," + varName2 + "))").AsNumeric()[0], 2);
        }
        public static double Var(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Var(commStr);
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
        public static int Max(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("max(c(" + varName1 + "," + varName2 + "))").AsInteger()[0];
        }
        public static int Max(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Max(commStr);
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
        public static int Min(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("min(c(" + varName1 + "," + varName2 + "))").AsInteger()[0];
        }
        public static int Min(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Min(commStr);
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
        public static int Median(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return re.Evaluate("median(c(" + varName1 + "," + varName2 + "))").AsInteger()[0];
        }
        public static int Median(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Median(commStr);
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
        public static int Mode(string varName1, string varName2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            return Convert.ToInt32(re.Evaluate("names(which.max(table(c(" + varName1 + "," + varName2 + "))))").AsCharacter()[0]);
        }
        public static int Mode(List<string> varNames)
        {
            REngine re = REngine.GetInstanceFromID("R");
            string commStr = "c(";
            foreach (string i in varNames) {
                commStr += i + ",";
            }
            commStr = commStr.Substring(0, commStr.Length - 1);
            commStr += ")";
            return Mode(commStr);
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

        public static void DrawByR_D(string varName1, string varName2, int startIndex, int endIndex)
        {
            REngine re = REngine.GetInstanceFromID("R");
            NumericVector nv1 = re.Evaluate(varName1).AsNumeric();
            NumericVector nv2 = re.Evaluate(varName2).AsNumeric();
            int ymax = (int) nv1.Max();
            int ymin = (int) nv2.Min();
            if (ymax < nv2.Max()) {
                ymax = (int) nv2.Max() + 1;
            }
            if (ymin > nv2.Min()) {
                ymin = (int) nv2.Min() - 1;
            }


            re.Evaluate(string.Format("plot(" + varName1 + ",type=\"l\",ylab=\"value\",xlim=c({0},{1}),col=1,ylim=c({2},{3}))", startIndex, endIndex, ymin, ymax));
            re.Evaluate("lines(" + varName2 + ",col=2)");
            re.Evaluate("legend(\"topleft\",legend=c(\"" + varName1 + "\",\"" + varName2 + "\"),col=c(1,2),lty=1,lwd=1,cex=1)");
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

            re.Evaluate(string.Format("plot(List[[1]],type=\"l\",ylim=c({0},{1}),xlim=c({2},{3}),ylab=\"value\",col=\"blue\")", yMin - 1, yMax + 1, xMin, xMax));
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

    if (startIndex != 1) {
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
	} else {
		preData[1:count] <- 0;
	}
	result <- NULL;
	for (i in 1:count) {
		if (preData[i] == 0) {
			result[i] <- 0;
		} else {
			result[i] <- (nowData[i]-preData[i])/preData[i];
		}
	}

	return (result);
}";
            re.Evaluate(str);
        }

        public static void HuanBiAndDrawByR(string varName, int startIndex, int endIndex, int xMin, int xMax)
        {
            REngine re = REngine.GetInstanceFromID("R");

            re.Evaluate(string.Format("HuanBiResult <- my_func_huanbi({0},{1},{2})", varName, startIndex, endIndex));

            re.Evaluate(string.Format("plot(HuanBiResult,type=\"l\",ylab=\"value\",xlim=c({0},{1}))", xMin, xMax));
        }

        public static void LoadingExtremumFunToR()
        {
            REngine re = REngine.GetInstanceFromID("R");

            string commStr =
                //RFun.txt      //找极大值下标  
@"my_func_extremum <- function(Data) {
    result <- NULL;
    count <- length(Data);
    for (i in 2:(count-1)) {
        if (Data[i] > Data[i-1] && Data[i] >= Data[i+1]) {
            result <- c(result, i);
        } else if (Data[i] > Data[i+1] && Data[i] >= Data[i-1]) {
            result <- c(result,i);
        }
    }
    return (result);
}";

            re.Evaluate(commStr);
        }

        public static List<int> GetExtremumIndexFromR(string varName)
        {
            List<int> l = new List<int>();
            REngine re = REngine.GetInstanceFromID("R");
            LoadingExtremumFunToR();
            l = re.Evaluate("my_func_extremum(" + varName + ")").AsInteger().ToList<int>();
            return l;
        }

        public static string Align(string varName1, string varName2, int index1, int index2)
        {
            REngine re = REngine.GetInstanceFromID("R");
            int diff = 0;
            string temp = "temp";
            if (index2 > index1) {
                diff = index2 - index1;
                temp = "T" + varName1;
                re.Evaluate(temp + " <- " + varName1);
            } else if (index2 < index1) {
                diff = index1 - index2;
                temp = "T" + varName2;
                re.Evaluate(temp + " <- " + varName2);
            } else {

            }
            for (int i = 0; i < diff; i++) {
                re.Evaluate(temp + " <- c(0," + temp + ")");
            }
            return temp;
        }

        public static void CrossAndDrawByR(string varName1, string varName2, int total_days1, int total_days2, int compare_dates, int xMin, int xMax)
        {
            REngine re = REngine.GetInstanceFromID("R");

            GenericVector gv1 = re.Evaluate(string.Format("List1 <- my_func_cross({0},{1},{2})", varName1, total_days1, compare_dates)).AsList();
            double yMax1 = gv1[0].AsNumeric().Max();
            if (gv1[0].AsNumeric().Max() < gv1[1].AsNumeric().Max()) {
                yMax1 = gv1[1].AsNumeric().Max();
            }
            double yMin1 = gv1[0].AsNumeric().Min();
            if (gv1[0].AsNumeric().Min() > gv1[1].AsNumeric().Min()) {
                yMin1 = gv1[1].AsNumeric().Min();
            }
            GenericVector gv2 = re.Evaluate(string.Format("List2 <- my_func_cross({0},{1},{2})", varName2, total_days2, compare_dates)).AsList();
            double yMax2 = gv1[1].AsNumeric().Max();
            if (gv2[0].AsNumeric().Max() < gv2[1].AsNumeric().Max()) {
                yMax2 = gv1[1].AsNumeric().Max();
            }
            double yMin2 = gv2[0].AsNumeric().Min();
            if (gv2[0].AsNumeric().Min() > gv2[1].AsNumeric().Min()) {
                yMin2 = gv1[1].AsNumeric().Min();
            }
            double yMax = yMax1;
            double yMin = yMin1;
            if (yMax1 < yMax2) {
                yMax = yMax2;
            }
            if (yMin1 > yMin2) {
                yMin = yMin2;
            }

            re.Evaluate(string.Format("plot(List1[[1]],type=\"l\",ylim=c({0},{1}),xlim=c({2},{3}),ylab=\"value\",col=\"blue\",lty=1)", yMin - 1, yMax + 1, xMin, xMax));
            re.Evaluate("lines(List1[[2]],col=\"blue\",lty=2)");
            re.Evaluate("lines(List2[[1]],col=\"red\",lty=1)");
            re.Evaluate("lines(List2[[2]],col=\"red\",lty=2)");
            string legendStr = "legend(\"topleft\",legend=c(\"" + varName1 + " By AVG\",\"" + varName1 + " By SD\",\"" + varName2 + " By AVG\",\"" + varName2 + " By SD\"),col=c(\"blue\",\"blue\",\"red\",\"red\"),lty=c(1,2,1,2),lwd=1,cex=1)";
            re.Evaluate(legendStr);
        }

        public static void HuanBiAndDrawByR(string varName1, string varName2, int startIndex1, int endIndex1, int startIndex2,int endIndex2, int xMin, int xMax)
        {
            REngine re = REngine.GetInstanceFromID("R");

            NumericVector nv1 = re.Evaluate(string.Format("HuanBiResult1 <- my_func_huanbi({0},{1},{2})", varName1, startIndex1, endIndex1)).AsNumeric();

            NumericVector nv2 = re.Evaluate(string.Format("HuanBiResult2 <- my_func_huanbi({0},{1},{2})", varName2, startIndex2, endIndex2)).AsNumeric();
            double ymax = nv1.Max();
            if (ymax < nv2.Max()) {
                ymax = nv2.Max();
            }
            double ymin = nv1.Min();
            if (ymin > nv2.Min()) {
                ymin = nv2.Min();
            }

            re.Evaluate(string.Format("plot(HuanBiResult1,type=\"l\",ylab=\"value\",xlim=c({0},{1}),ylim=c({2},{3}),col=\"blue\")", xMin, xMax,ymin,ymax));
            re.Evaluate("lines(HuanBiResult2,col=\"red\")");
            re.Evaluate("legend(\"topleft\",legend=c(\"" + varName1 + "\",\"" + varName2 + "\"),col=c(\"blue\",\"red\"),lty=1,lwd=1,cex=1)");
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
