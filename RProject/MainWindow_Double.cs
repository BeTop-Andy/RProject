using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RProject
{
    public partial class MainWindow : Window
    {
        bool DHasStatistics = false;

        private void SelectTimeB_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            ContentPresenter cp = btn.TemplatedParent as ContentPresenter;
            CowListItem cli = cp.Content as CowListItem;
            //MessageBox.Show(cli.CowId.ToString());
            ShowSelectTimeWindow(cli.CowId);
        }

        private void DStatisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            int cowId1 = -1;
            int cowId2 = -1;
            int count = 0;

            foreach (CowListItem i in Cows) {
                if (i.IsChecked == true) {
                    count++;
                    if (count > 2) {
                        MessageBox.Show("请选择2头奶牛");
                        return;
                    }
                    if (cowId1 == -1) {
                        cowId1 = i.CowId;
                    } else {
                        cowId2 = i.CowId;
                    }
                    if (!ht.ContainsKey(i.CowId) || !((SelectTime) ht[i.CowId]).isOK) {
                        MessageBox.Show("请选择时段并且确认按下了确定按钮", i.CowId.ToString());
                        return;
                    }
                    if (i.Threshold == -1) {
                        MessageBox.Show("请选择阈值", i.CowId.ToString());
                        return;
                    }

                }
            }
            if (count != 2) {
                MessageBox.Show("请选择2头奶牛");
                return;
            }

            string varName1 = "Cow" + cowId1;
            string varName2 = "Cow" + cowId2;

            DStartSlider.Maximum = 1;
            DEndSlider.Maximum = 1;




            ReadDataToR_D1(cowId1, varName1);
            ReadDataToR_D1(cowId2, varName2);

            StatisticsByR_D(varName1, varName2);




            DHasStatistics = true;
        }

        private void StatisticsByR_D(string varName1, string varName2)
        {
            double avg = 0;
            double sd2 = 0;     //方差
            int max = 0;
            int min = 0;
            int median = 0;     //中位数
            int mode = 0;       //众数
            try {
                avg = RCommand.Avg(varName1, varName2);
                DAVGL.Content = avg.ToString();

                sd2 = RCommand.Var(varName1, varName2);
                DSD2L.Content = sd2.ToString();

                max = RCommand.Max(varName1, varName2);
                DMaxL.Content = max.ToString();

                min = RCommand.Min(varName1, varName2);
                DMinL.Content = min.ToString();

                median = RCommand.Median(varName1, varName2);
                DMedianL.Content = median.ToString();

                mode = RCommand.Mode(varName1, varName2);
                DModeL.Content = mode.ToString();
            } catch {

            }

        }

        private void ReadDataToR_D1(int cowId, string varName)
        {
            SelectTime stWin = (SelectTime) ht[cowId];
            int threshold = -1;
            foreach (CowListItem cli in Cows) {
                if (cli.CowId == cowId)
                    threshold = cli.Threshold;
            }


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
                        sqlComm = string.Format("select value{0} from `data` where date = date('{1}') and threshold = {2} and cowId = {3}", startIndex, tempDate.ToString("yyyy-M-d"), threshold, cowId);
                        comm = new MySqlCommand(sqlComm, myConn);

                        using (dr = comm.ExecuteReader()) {
                            if (dr.Read()) {
                                l.Add(dr.GetInt32(0));
                            }
                        }
                    }
                }
            }

            DStartSlider.Minimum = 1;
            if (l.Count > DStartSlider.Maximum)
                DStartSlider.Maximum = l.Count;
            DEndSlider.Minimum = 1;
            if (l.Count > DEndSlider.Maximum)
                DEndSlider.Maximum = l.Count;
            DEndSlider.Value = DEndSlider.Maximum;

            RCommand.LoadingDataToR(l, varName);

        }

        private void ThresholdCbB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = e.OriginalSource as ComboBox;
            ContentPresenter cp = cbb.TemplatedParent as ContentPresenter;
            CowListItem cli = cp.Content as CowListItem;

            foreach (CowListItem c in Cows) {
                if (c.CowId == cli.CowId) {
                    c.Threshold = cbb.SelectedIndex;
                }
            }
        }

        private void IsSelectedCB_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.OriginalSource as CheckBox;
            ContentPresenter cp = cb.TemplatedParent as ContentPresenter;
            CowListItem cli = cp.Content as CowListItem;
            foreach (CowListItem c in Cows) {
                if (c.CowId == cli.CowId) {
                    c.IsChecked = cb.IsChecked.Value;
                }
            }
        }

        private void IsSelectedCB_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.OriginalSource as CheckBox;
            ContentPresenter cp = cb.TemplatedParent as ContentPresenter;
            CowListItem cli = cp.Content as CowListItem;
            foreach (CowListItem c in Cows) {
                if (c.CowId == cli.CowId) {
                    c.IsChecked = cb.IsChecked.Value;
                }
            }
        }

        private void DSmoothBtn_Click(object sender, RoutedEventArgs e)
        {
            int cowId1 = -1;
            int cowId2 = -1;
            int count = 0;

            foreach (CowListItem i in Cows) {
                if (i.IsChecked == true) {
                    count++;
                    if (count > 2) {
                        MessageBox.Show("请选择2头奶牛");
                        return;
                    }
                    if (cowId1 == -1) {
                        cowId1 = i.CowId;
                    } else {
                        cowId2 = i.CowId;
                    }
                    if (!ht.ContainsKey(i.CowId) || !((SelectTime) ht[i.CowId]).isOK) {
                        MessageBox.Show("请选择时段并且确认按下了确定按钮", i.CowId.ToString());
                        return;
                    }
                    if (i.Threshold == -1) {
                        MessageBox.Show("请选择阈值", i.CowId.ToString());
                        return;
                    }

                }
            }
            if (count != 2) {
                MessageBox.Show("请选择2头奶牛");
                return;
            }

            string varName1 = "Cow" + cowId1;
            string varName2 = "Cow" + cowId2;

            DStartSlider.Maximum = 1;
            DEndSlider.Maximum = 1;

            ReadDataToR_D1(cowId1, varName1);
            ReadDataToR_D1(cowId2, varName2);
            RCommand.LoadingSmoothFunToR();
            RCommand.SmoothByR(varName1);
            RCommand.SmoothByR(varName2);
            StatisticsByR_D(varName1, varName2);




            DHasStatistics = true;

        }

        private void DDrawBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DHasStatistics) {
                string varName1 = null;
                string varName2 = null;
                foreach (CowListItem i in Cows) {
                    if (i.IsChecked == true) {
                        if (varName1 == null) {
                            varName1 = "Cow" + i.CowId;
                        } else {
                            varName2 = "Cow" + i.CowId;
                        }
                    }
                }
                int startIndex = (int) DStartSlider.Value;
                int endIndex = (int) DEndSlider.Value;
                RCommand.DrawByR_D(varName1, varName2, startIndex, endIndex);

                List<int> l = RCommand.GetExtremumIndexFromR(varName1);
                ExtremumCbB1.Items.Clear();
                foreach (int i in l) {
                    ExtremumCbB1.Items.Add(i);
                }
                Cow1L.Tag = varName1;
                l = RCommand.GetExtremumIndexFromR(varName2);
                ExtremumCbB2.Items.Clear();
                foreach (int i in l) {
                    ExtremumCbB2.Items.Add(i);
                }
                Cow2L.Tag = varName2;
            } else {
                MessageBox.Show("请选进行【统计】操作");
            }
        }

        private void DefaultBtn_Click(object sender, RoutedEventArgs e)
        {
            DDrawBtn_Click(sender, e);
        }

        private void AlignBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ExtremumCbB1.SelectedIndex != -1 && ExtremumCbB2.SelectedIndex != -1) {
                int index1 = Convert.ToInt32(ExtremumCbB1.SelectedValue);
                int index2 = Convert.ToInt32(ExtremumCbB2.SelectedValue);

                string varName1 = Cow1L.Tag.ToString();
                string varName2 = Cow2L.Tag.ToString();

                string temp = RCommand.Align(varName1, varName2, index1, index2);
                if (index1 < index2) {
                    RCommand.DrawByR_D(temp, varName2, (int) DStartSlider.Value, (int) DEndSlider.Value);
                } else if (index2 < index1) {
                    RCommand.DrawByR_D(varName1, temp, (int) DStartSlider.Value, (int) DEndSlider.Value);
                }

            } else {
                MessageBox.Show("请选择下标");
            }
        }

        private void DTongBiBtn_Click(object sender, RoutedEventArgs e)
        {
            string varName1;
            string varName2;

            if (DCow1IDCbB.SelectedIndex < 0 || DCow2IDCbB.SelectedIndex < 0) {
                MessageBox.Show("请选择ID");
                return;
            }
            if (DStartDateDP1.SelectedDate == null || DEndDateDP1.SelectedDate == null || DStartDateDP2.SelectedDate == null || DEndDateDP2.SelectedDate == null) {
                MessageBox.Show("请选择时段");
                return;
            }

            if ((DEndDateDP1.SelectedDate.Value - DStartDateDP1.SelectedDate.Value).Days <= 0 || (DEndDateDP2.SelectedDate.Value - DStartDateDP2.SelectedDate.Value).Days <= 0) {
                MessageBox.Show("结束日期必须在开始日期之后");
                return;
            }

            if (DThresholdCbB.SelectedIndex < 0) {
                MessageBox.Show("请选择阈值");
                return;
            }

            int compareDays;
            if (DCompareDaysCbB.SelectedIndex != -1) {
                compareDays = DCompareDaysCbB.SelectedIndex + 3;//至少为3
            } else {
                MessageBox.Show("请选择比较天数");
                return;
            }

            int cowId1 = Convert.ToInt32(DCow1IDCbB.SelectedValue);
            int cowId2 = Convert.ToInt32(DCow2IDCbB.SelectedValue);
            varName1 = "TongBi" + cowId1;
            varName2 = "TongBi" + cowId2;
            int totalDays1 = (DEndDateDP1.SelectedDate.Value - DStartDateDP1.SelectedDate.Value).Days;
            int totalDays2 = (DEndDateDP2.SelectedDate.Value - DStartDateDP2.SelectedDate.Value).Days;
            int xMin = (int) DStartSlider.Value;
            int xMax = (int) DEndSlider.Value;



            ReadDataToR_D2(varName1, cowId1);
            ReadDataToR_D2(varName2, cowId2);
            RCommand.LoadingSmoothFunToR();
            RCommand.LoadingCrossFunToR();
            RCommand.SmoothByR(varName1);
            RCommand.SmoothByR(varName2);
            RCommand.CrossAndDrawByR(varName1, varName2, totalDays1, totalDays2, compareDays, xMin, xMax);

        }

        private void ReadDataToR_D2(string varName1, int cowId1)
        {
            string commStr;
            MySqlCommand mySqlComm;
            MySqlDataReader dr;
            List<int> l = new List<int>();
            DateTime sd = DStartDateDP1.SelectedDate.Value;
            DateTime ed = DEndDateDP1.SelectedDate.Value;
            int threshold = DThresholdCbB.SelectedIndex;


            for (DateTime temp = sd; temp <= ed; temp = temp.AddDays(1)) {
                for (int i = 1; i <= 24; i++) {
                    commStr = string.Format("select value{0} from `data` where date = date('{1}') and threshold = {2} and cowId = {3}", i.ToString(), temp.ToString("yyyy-M-d"), threshold.ToString(), cowId1);
                    mySqlComm = new MySqlCommand(commStr, myConn);
                    using (dr = mySqlComm.ExecuteReader()) {
                        while (dr.Read()) {
                            l.Add(dr.GetInt32(0));
                        }
                    }
                }
            }
            RCommand.LoadingDataToR(l, varName1);
        }
        private void ReadDataToR_D3(string varName1, int cowId1)
        {
            string commStr;
            MySqlCommand mySqlComm;
            MySqlDataReader dr;
            List<int> l = new List<int>();
            DateTime sd = DStartDateDP1.DisplayDateStart.Value;
            DateTime ed = DEndDateDP1.DisplayDateEnd.Value;
            int threshold = DThresholdCbB.SelectedIndex;


            for (DateTime temp = sd; temp <= ed; temp = temp.AddDays(1)) {
                for (int i = 1; i <= 24; i++) {
                    commStr = string.Format("select value{0} from `data` where date = date('{1}') and threshold = {2} and cowId = {3}", i.ToString(), temp.ToString("yyyy-M-d"), threshold.ToString(), cowId1);
                    mySqlComm = new MySqlCommand(commStr, myConn);
                    using (dr = mySqlComm.ExecuteReader()) {
                        while (dr.Read()) {
                            l.Add(dr.GetInt32(0));
                        }
                    }
                }
            }
            RCommand.LoadingDataToR(l, varName1);
        }

        private void DHuanBiBtn_Click(object sender, RoutedEventArgs e)
        {
            string varName1;
            string varName2;

            if (DCow1IDCbB.SelectedIndex < 0 || DCow2IDCbB.SelectedIndex < 0) {
                MessageBox.Show("请选择ID");
                return;
            }
            if (DStartDateDP1.SelectedDate == null || DEndDateDP1.SelectedDate == null || DStartDateDP2.SelectedDate == null || DEndDateDP2.SelectedDate == null) {
                MessageBox.Show("请选择时段");
                return;
            }

            if ((DEndDateDP1.SelectedDate.Value - DStartDateDP1.SelectedDate.Value).Days <= 0 || (DEndDateDP2.SelectedDate.Value - DStartDateDP2.SelectedDate.Value).Days <= 0) {
                MessageBox.Show("结束日期必须在开始日期之后");
                return;
            }

            if (DThresholdCbB.SelectedIndex < 0) {
                MessageBox.Show("请选择阈值");
                return;
            }


            int cowId1 = Convert.ToInt32(DCow1IDCbB.SelectedValue);
            int cowId2 = Convert.ToInt32(DCow2IDCbB.SelectedValue);
            varName1 = "HuanBi" + cowId1;
            varName2 = "HuanBi" + cowId2;
            DateTime st1 = DStartDateDP1.SelectedDate.Value;
            DateTime ed1 = DEndDateDP1.SelectedDate.Value;
            int startIndex1 = (st1 - DStartDateDP1.DisplayDateStart.Value).Days * 24 + 1;   //*24得到有多少个小时
            int endIndex1 = (ed1 - DStartDateDP1.DisplayDateStart.Value).Days * 24;
            DateTime st2 = DStartDateDP2.SelectedDate.Value;
            DateTime ed2 = DEndDateDP2.SelectedDate.Value;
            int startIndex2 = (st2 - DStartDateDP2.DisplayDateStart.Value).Days * 24 + 1;   //*24得到有多少个小时
            int endIndex2 = (ed2 - DStartDateDP2.DisplayDateStart.Value).Days * 24;


            int xmin = (int) DStartSlider.Value;
            int xmax = (int) DEndSlider.Value;

            ReadDataToR_D3(varName1, cowId1);
            ReadDataToR_D3(varName2, cowId2);
            RCommand.LoadingSmoothFunToR();
            RCommand.LoadingHuanBiFunToR();
            RCommand.SmoothByR(varName1);
            RCommand.SmoothByR(varName2);

            RCommand.HuanBiAndDrawByR(varName1, varName2, startIndex1, endIndex1, startIndex2, endIndex2, xmin, xmax);
        }

        private void DCow1IDCbB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DStartDateDP1.SelectedDate = null;
            DEndDateDP1.SelectedDate = null;

            int cowId = Convert.ToInt32(DCow1IDCbB.SelectedValue);
            string commText = "select min(date),max(date) from `data` where cowId = " + cowId;

            MySqlCommand comm = new MySqlCommand(commText, myConn);
            using (MySqlDataReader dr = comm.ExecuteReader()) {
                if (dr.Read()) {
                    DStartDateDP1.DisplayDateStart = dr.GetDateTime(0);
                    DStartDateDP1.DisplayDateEnd = dr.GetDateTime(1);
                    DEndDateDP1.DisplayDateStart = dr.GetDateTime(0);
                    DEndDateDP1.DisplayDateEnd = dr.GetDateTime(1);
                }
            }
        }

        private void DCow2IDCbB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DStartDateDP2.SelectedDate = null;
            DEndDateDP2.SelectedDate = null;

            int cowId = Convert.ToInt32(DCow2IDCbB.SelectedValue);
            string commText = "select min(date),max(date) from `data` where cowId = " + cowId;

            MySqlCommand comm = new MySqlCommand(commText, myConn);
            using (MySqlDataReader dr = comm.ExecuteReader()) {
                if (dr.Read()) {
                    DStartDateDP2.DisplayDateStart = dr.GetDateTime(0);
                    DStartDateDP2.DisplayDateEnd = dr.GetDateTime(1);
                    DEndDateDP2.DisplayDateStart = dr.GetDateTime(0);
                    DEndDateDP2.DisplayDateEnd = dr.GetDateTime(1);
                }
            }
        }

        private void DStartDateDP1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DStartDateDP1.SelectedDate != null && DEndDateDP1.SelectedDate != null) {
                int preMax = (int) DStartSlider.Maximum;
                int daysLength = (DEndDateDP1.SelectedDate.Value - DStartDateDP1.SelectedDate.Value).Days;
                if (preMax < daysLength*24) {
                    DStartSlider.Maximum = daysLength * 24;
                    DEndSlider.Maximum = daysLength * 24;
                    DEndSlider.Value = DEndSlider.Maximum;
                }
            }
        }

        private void DEndDateDP1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DEndDateDP1.SelectedDate != null && DStartDateDP1.SelectedDate != null) {
                int preMax = (int) DStartSlider.Maximum;
                int daysLength = (DEndDateDP1.SelectedDate.Value - DStartDateDP1.SelectedDate.Value).Days;
                if (preMax < daysLength*24) {
                    DStartSlider.Maximum = daysLength * 24;
                    DEndSlider.Maximum = daysLength * 24;
                    DEndSlider.Value = DEndSlider.Maximum;
                }
            }
        }

        private void DStartDateDP2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DStartDateDP2.SelectedDate != null && DEndDateDP2.SelectedDate != null) {
                int preMax = (int) DStartSlider.Maximum;
                int daysLength = (DEndDateDP2.SelectedDate.Value - DStartDateDP2.SelectedDate.Value).Days;
                if (preMax < daysLength*24) {
                    DStartSlider.Maximum = daysLength * 24;
                    DEndSlider.Maximum = daysLength * 24;
                    DEndSlider.Value = DEndSlider.Maximum;
                }
            }
        }

        private void DEndDateDP2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DEndDateDP2.SelectedDate != null && DStartDateDP2.SelectedDate != null) {
                int preMax = (int) DStartSlider.Maximum;
                int daysLength = (DEndDateDP2.SelectedDate.Value - DStartDateDP2.SelectedDate.Value).Days;
                if (preMax < daysLength*24) {
                    DStartSlider.Maximum = daysLength * 24;
                    DEndSlider.Maximum = daysLength * 24;
                    DEndSlider.Value = DEndSlider.Maximum;
                }
            }
        }
    }
}
