using MySql.Data.MySqlClient;
using RDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;

namespace RProject
{
    public partial class MainWindow : Window
    {
        private Hashtable ht = new Hashtable();
        private bool hasStatic = false;

        private void StartPageCbB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int startIndex = Convert.ToInt32(StartPageCbB1.SelectedValue);
            EndPageCbB1.Items.Clear();
            for (int i = startIndex; i <= maxPage; i++) {
                EndPageCbB1.Items.Add(i);
            }
            StartSlider.Minimum = 1;
            EndSlider.Minimum = 1;
        }

        private void EndPageCbB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int startPage = Convert.ToInt32(StartPageCbB1.SelectedValue);
            int endPage = Convert.ToInt32(EndPageCbB1.SelectedValue);

            StartSlider.Maximum = (endPage - startPage + 1) * rowPerPage;
            EndSlider.Maximum = (endPage - startPage + 1) * rowPerPage;
            EndSlider.Value = EndSlider.Maximum;
        }


        private void StatisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            // 
            //             if (csvDt != null) {
            //                 int startIndex;
            //                 int endIndex;
            // 
            //                 if (StartPageCbB1.SelectedIndex >= 0 && EndPageCbB1.SelectedIndex >= 0) {
            //                     int startPage = Convert.ToInt32(StartPageCbB1.SelectedValue);
            //                     int endPage = Convert.ToInt32(EndPageCbB1.SelectedValue);
            // 
            //                     startIndex = (startPage - 1) * rowPerPage + 1;
            //                     endIndex = endPage * rowPerPage;
            //                     if (endIndex > length) {
            //                         endIndex = length;
            //                     }
            //                 } else {
            //                     startIndex = 1;
            //                     endIndex = length;
            //                 }
            // 
            // 
            //                 try {
            //                     avg = re.Evaluate(string.Format("mean(tb[{0}:{1},1])", startIndex, endIndex)).AsNumeric()[0];
            //                     avg = Math.Round(avg, 2);
            //                     AvgLabel.Content = avg.ToString();
            // 
            //                     sd2 = re.Evaluate(string.Format("var(tb[{0}:{1},1])", startIndex, endIndex)).AsNumeric()[0];
            //                     sd2 = Math.Round(sd2, 2);
            //                     Sd2Label.Content = sd2.ToString();
            // 
            //                     max = re.Evaluate(string.Format("max(tb[{0}:{1},1])", startIndex, endIndex)).AsInteger()[0];
            //                     MaxLabel.Content = max.ToString();
            // 
            //                     min = re.Evaluate(string.Format("min(tb[{0}:{1},1])", startIndex, endIndex)).AsInteger()[0];
            //                     MinLabel.Content = min.ToString();
            // 
            //                     median = re.Evaluate(string.Format("median(tb[{0}:{1},1])", startIndex, endIndex)).AsInteger()[0];
            //                     MedianLabel.Content = median.ToString();
            // 
            //                     mode = Convert.ToInt32(re.Evaluate(string.Format("names(which.max(table(tb[{0}:{1},1])))", startIndex, endIndex)).AsCharacter()[0]);
            //                     ModeLabel.Content = mode.ToString();
            //                 } catch {
            // 
            //                 }
            //             } else 
            string varName = null;
            if (ReadDataToR(ref varName)) {
                StatisticsByR(varName);
            }
        }

        /// <summary>
        /// 读数据到R中
        /// </summary>
        /// <param name="varName">变量名字</param>
        /// <returns>是否成功</returns>
        private bool ReadDataToR(ref string varName)
        {
            int cowId = Convert.ToInt32(Sigle_CowIdCbB.SelectedValue);

            List<bool> condition = new List<bool> { myConn != null, Sigle_CowIdCbB.SelectedIndex != -1, ht.Contains(cowId) && ((SelectTime) ht[cowId]).isOK, Single_ThresholdCbB.SelectedIndex != -1 };

            string[] msg = new string[] { "数据库连接失败", "请选择奶牛ID", "请选择时段并且确认按下了确定按钮", "请选择阈值" };


            int index = condition.FindIndex(x => x == false);       //找不符合的
            if (index != -1) {
                MessageBox.Show(msg[index]);
                return false;
            }


            SelectTime stWin = (SelectTime) ht[cowId];
            List<int> l = new List<int>();
            string startDate;
            int startTime;
            string endDate;
            int endTime;

            DateTime tempDate;

            string sqlComm;
            MySqlCommand comm;
            MySqlDataReader dr;

            l.Clear();
            for (int i = 0; i < stWin.dt.Rows.Count; i++) {
                startDate = stWin.dt.Rows[i][1].ToString();

                string temp = stWin.dt.Rows[i][2].ToString();
                temp = temp.Remove(temp.IndexOf(':'));
                startTime = Convert.ToInt32(temp);

                endDate = stWin.dt.Rows[i][3].ToString();

                temp = stWin.dt.Rows[i][4].ToString();
                temp = temp.Remove(temp.IndexOf(':'));
                endTime = Convert.ToInt32(temp);

                tempDate = Convert.ToDateTime(startDate);

                for (; tempDate <= Convert.ToDateTime(endDate); tempDate = tempDate.AddDays(1)) {
                    int startIndex = 1;
                    int endIndex = 24;

                    if (tempDate == Convert.ToDateTime(startDate)) {
                        startIndex = startTime + 1;      //第一天
                    } else if (tempDate == Convert.ToDateTime(endDate)) {
                        endIndex = endTime;             //最后一天
                    } else {
                        startIndex = 1;
                    }
                    for (; startIndex <= endIndex; startIndex++) {
                        sqlComm = string.Format("select value{0} from `data` where date = date('{1}') and threshold = {2}", startIndex, tempDate.ToString("yyyy-M-d"), Single_ThresholdCbB.SelectedIndex);
                        comm = new MySqlCommand(sqlComm, myConn);

                        using (dr = comm.ExecuteReader()) {
                            if (dr.Read()) {
                                l.Add(dr.GetInt32(0));
                            }
                        }
                    }

                }
            }

            StartSlider.Minimum = 1;
            StartSlider.Maximum = l.Count;
            EndSlider.Minimum = 1;
            EndSlider.Maximum = l.Count;
            EndSlider.Value = EndSlider.Maximum;

            varName = "Index";
            RCommand.LoadingDataToR(l, varName);

            return true;
        }

        /// <summary>
        /// 用R统计平均值等……
        /// </summary>
        /// <param name="varName">R里的变量名</param>
        private void StatisticsByR(string varName)
        {
            double avg = 0;
            double sd2 = 0;     //方差
            int max = 0;
            int min = 0;
            int median = 0;     //中位数
            int mode = 0;       //众数
            try {
                avg = RCommand.Avg(varName);
                AvgLabel.Content = avg.ToString();

                sd2 = RCommand.Var(varName);
                Sd2Label.Content = sd2.ToString();

                max = RCommand.Max(varName);
                MaxLabel.Content = max.ToString();

                min = RCommand.Min(varName);
                MinLabel.Content = min.ToString();

                median = RCommand.Median(varName);
                MedianLabel.Content = median.ToString();

                mode = RCommand.Mode(varName);
                ModeLabel.Content = mode.ToString();
            } catch {

            }

            hasStatic = true;
        }

        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {
            // 
            //             if (StartPageCbB1.SelectedIndex >= 0 && EndPageCbB1.SelectedIndex >= 0) {
            //                 if (PicTypeCbB.SelectedIndex != -1) {
            //                     int startPage = Convert.ToInt32(StartPageCbB1.SelectedValue);
            //                     int endPage = Convert.ToInt32(EndPageCbB1.SelectedValue);
            //                     int minIndex = (startPage - 1) * rowPerPage + 1;
            //                     int maxIndex = endPage * rowPerPage;
            //                     int startIndex = (int) StartSlider.Value;
            //                     int endIndex = (int) EndSlider.Value;
            //                     if (endIndex > length) {
            //                         endIndex = length;
            //                     }
            //                     int type = PicTypeCbB.SelectedIndex;
            // 
            //                     DrawPar o = new DrawPar(type, minIndex, maxIndex, startIndex, endIndex);
            // 
            //                     SetLoadingBarVisibilityInvoke(true);
            //                     DrawPic(o);
            //                 } else {
            //                     MessageBox.Show("请选择图形类别");
            //                 }
            //             } else {
            //                 MessageBox.Show("请选择页数");
            //             }

            if (hasStatic) {
                int type = PicTypeCbB.SelectedIndex;
                string varName = "Index";
                int startIndex = (int) StartSlider.Value;
                int endIndex = (int) EndSlider.Value;
                if (type == -1) {
                    MessageBox.Show("请选择图的类型");
                } else {
                    RCommand.DrawByR(type, varName, startIndex, endIndex);
                }
            } else {
                MessageBox.Show("请选进行【统计】操作");
            }


        }

        private void StartSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (StartSlider.Value > EndSlider.Value) {
                EndSlider.Value = StartSlider.Value + 1;
            }
        }

        private void EndSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (EndSlider.Value < StartSlider.Value) {
                StartSlider.Value = EndSlider.Value - 1;
            }
        }

        //         /// <summary>
        //         /// 
        //         /// </summary>
        //         /// <param name="type">类型：
        //         /// 0：条形图
        //         /// 1：折线图
        //         /// 2：饼图
        //         /// 3：分布图
        //         /// 4：散点图
        //         /// </param>
        //         /// <param name="minIndex">最小值</param>
        //         /// <param name="maxIndex">最大值</param>
        //         /// <param name="startIndex">可视范围开始</param>
        //         /// <param name="endIndex">可视范围结束</param>
        //         private void DrawPic(DrawPar o)
        //         {
        //             int type = o.Type;
        //             int minIndex = o.MinIndex;
        //             int maxIndex = o.MaxIndex;
        //             int startIndex = o.StartIndex;
        //             int endIndex = o.EndIndex;
        // 
        // 
        //             try {
        //                 switch (type) {
        //                     case 0:
        //                         re.Evaluate(string.Format("plot(tb[{0}:{1},1],type=\"h\",xlim=c({2},{3}),ylab=\"value\")", minIndex, maxIndex, startIndex, endIndex));
        //                         break;
        //                     case 1:
        //                         re.Evaluate(string.Format("plot(tb[{0}:{1},1],type=\"l\",xlim=c({2},{3}),ylab=\"value\")", minIndex, maxIndex, startIndex, endIndex));
        //                         break;
        //                     case 2:
        //                         re.Evaluate(string.Format("pie(tb[{0}:{1},1])", startIndex, endIndex));
        //                         break;
        //                     case 3:
        //                         re.Evaluate(string.Format("plot(table(tb[{0}:{1},1]),ylab=\"value\")", startIndex, endIndex));
        //                         break;
        //                     case 4:
        //                         re.Evaluate(string.Format("plot(tb[{0}:{1},1],type=\"p\",xlim=c({2},{3}),ylab=\"value\")", minIndex, maxIndex, startIndex, endIndex));
        //                         break;
        //                 }
        //             } catch (Exception ex) {
        //                 MessageBox.Show(ex.Message);
        //             }
        // 
        //             SetLoadingBarVisibilityInvoke(false);
        //         }

        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int now = (int) StartSlider.Value;
            SetSliderValue win = new SetSliderValue(now);
            win.ShowDialog();
            if (win.isSet) {
                StartSlider.Value = win.newValue;
            }
        }

        private void Label_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int now = (int) EndSlider.Value;
            SetSliderValue win = new SetSliderValue(now);
            win.ShowDialog();
            if (win.isSet) {
                EndSlider.Value = win.newValue;
            }
        }


        private void SelectTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (myConn != null && Sigle_CowIdCbB.SelectedIndex != -1) {
                int id = Convert.ToInt32(Sigle_CowIdCbB.SelectedValue);
                string commText = "select min(date),max(date) from `data` where cowId = " + id;

                MySqlCommand comm = new MySqlCommand(commText, myConn);
                using (MySqlDataReader dr = comm.ExecuteReader()) {
                    DateTime sd;            //StartDate
                    DateTime ed;            //EndDate

                    if (dr.Read()) {
                        sd = dr.GetDateTime(0);
                        ed = dr.GetDateTime(1);

                        SelectTime stWin;

                        if (ht.Contains(id)) {
                            stWin = (SelectTime) ht[id];
                        } else {
                            stWin = new SelectTime(sd, ed);
                            ht.Add(id, stWin);
                        }

                        stWin.isOK = false;
                        stWin.ShowDialog();
                    }
                }
            } else {
                MessageBox.Show("请选择奶牛ID");
            }
        }


        private void SmoothBtn_Click(object sender, RoutedEventArgs e)
        {
            string varName = null;
            if (ReadDataToR(ref varName)) {
                RCommand.LoadingSmoothFunToR();
                RCommand.SmoothByR(varName);

                StatisticsByR(varName);
            }

        }


        private void CowIdCbB2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartDateDP.SelectedDate = null;
            EndDateDP.SelectedDate = null;

            int cowId = Convert.ToInt32(Sigle_CowIdCbB2.SelectedValue);
            string commText = "select min(date),max(date) from `data` where cowId = " + cowId;

            MySqlCommand comm = new MySqlCommand(commText, myConn);
            using (MySqlDataReader dr = comm.ExecuteReader()) {
                if (dr.Read()) {
                    StartDateDP.DisplayDateStart = dr.GetDateTime(0);
                    StartDateDP.DisplayDateEnd = dr.GetDateTime(1);
                    EndDateDP.DisplayDateStart = dr.GetDateTime(0);
                    EndDateDP.DisplayDateEnd = dr.GetDateTime(1);
                }
            }
        }

        private bool ReadDataToR_2(ref string varName)
        {
            List<bool> condition = new List<bool> { myConn != null, Sigle_CowIdCbB2.SelectedIndex != -1, StartDateDP.SelectedDate != null, EndDateDP.SelectedDate != null, Sigle_ThresholdCbB.SelectedIndex != -1 };

            string[] msg = new string[] { "数据库连接失败", "请选择奶牛ID", "请选择开始日期", "请选择结束日期", "请选择阈值" };

            int index = condition.FindIndex(x => x == false);       //找不符合的
            if (index != -1) {
                MessageBox.Show(msg[index]);
                return false;
            }
            if ((EndDateDP.SelectedDate.Value -
                StartDateDP.SelectedDate.Value).Days <= 0) {
                MessageBox.Show("结束日期必须在开始日期之后");
                return false;
            }



            int cowId = Convert.ToInt32(Sigle_CowIdCbB.SelectedValue);
            DateTime sd = StartDateDP.SelectedDate.Value;
            DateTime ed = EndDateDP.SelectedDate.Value;
            int threshold = Sigle_ThresholdCbB.SelectedIndex;

            string commStr;
            MySqlCommand mySqlComm;
            MySqlDataReader dr;
            List<int> l = new List<int>();

            for (DateTime temp = sd; temp <= ed; temp = temp.AddDays(1)) {
                for (int i = 1; i <= 24; i++) {
                    commStr = string.Format("select value{0} from `data` where date = date('{1}') and threshold = {2}", i.ToString(), temp.ToString("yyyy-M-d"), threshold.ToString());
                    mySqlComm = new MySqlCommand(commStr, myConn);
                    using (dr = mySqlComm.ExecuteReader()) {
                        while (dr.Read()) {
                            l.Add(dr.GetInt32(0));
                        }
                    }
                }
            }

            varName = "CrossValue";
            RCommand.LoadingDataToR(l, varName);

            return true;
        }
        private void StartDateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDateDP.SelectedDate != null) {
                int daysLength = (EndDateDP.SelectedDate.Value - StartDateDP.SelectedDate.Value).Days;
                StartSlider.Maximum = daysLength * 24;
                EndSlider.Maximum = daysLength * 24;
                EndSlider.Value = EndSlider.Maximum;
            }
        }

        private void EndDateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDateDP.SelectedDate != null) {
                int daysLength = (EndDateDP.SelectedDate.Value - StartDateDP.SelectedDate.Value).Days;
                StartSlider.Maximum = daysLength * 24;
                EndSlider.Maximum = daysLength * 24;
                EndSlider.Value = EndSlider.Maximum;
            }
        }

        private void TongbiBtn_Click(object sender, RoutedEventArgs e)
        {
            string varName = null;
            int compareDays;

            if (SelectCompareDaysCbB.SelectedIndex != -1) {
                compareDays = SelectCompareDaysCbB.SelectedIndex + 3;//至少为3
            } else {
                MessageBox.Show("请选择比较天数");
                return;
            }

            if (ReadDataToR_2(ref varName)) {
                int totalDays = (EndDateDP.SelectedDate.Value - StartDateDP.SelectedDate.Value).Days;
                int xMin = (int) StartSlider.Value;
                int xMax = (int) EndSlider.Value;
                RCommand.LoadingSmoothFunToR();
                RCommand.LoadingCrossFunToR();
                RCommand.SmoothByR(varName);
                RCommand.CrossAndDrawByR(varName, totalDays, compareDays, xMin, xMax);
            }
        }

        private void HuanbiBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    // 
    //     public class DrawPar
    //     {
    //         private int type;
    // 
    //         public int Type
    //         {
    //             get { return type; }
    //             set { type = value; }
    //         }
    //         private int minIndex;
    // 
    //         public int MinIndex
    //         {
    //             get { return minIndex; }
    //             set { minIndex = value; }
    //         }
    //         private int maxIndex;
    // 
    //         public int MaxIndex
    //         {
    //             get { return maxIndex; }
    //             set { maxIndex = value; }
    //         }
    //         private int startIndex;
    // 
    //         public int StartIndex
    //         {
    //             get { return startIndex; }
    //             set { startIndex = value; }
    //         }
    //         private int endIndex;
    // 
    //         public int EndIndex
    //         {
    //             get { return endIndex; }
    //             set { endIndex = value; }
    //         }
    // 
    // 
    // 
    //         /// <summary>
    //         /// 
    //         /// </summary>
    //         /// <param name="t">type</param>
    //         /// <param name="m">minIndex</param>
    //         /// <param name="max">maxIndex</param>
    //         /// <param name="s">startIndex</param>
    //         /// <param name="e">endIndex</param>
    //         public DrawPar(int t, int m, int max, int s, int e)
    //         {
    //             type = t;
    //             minIndex = m;
    //             maxIndex = max;
    //             startIndex = s;
    //             endIndex = e;
    //         }
    //     }
}
