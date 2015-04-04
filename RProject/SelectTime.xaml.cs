using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace RProject
{
    /// <summary>
    /// SelectTime.xaml 的交互逻辑
    /// </summary>
    public partial class SelectTime : Window
    {
        private DateTime startDate;
        private DateTime endDate;
        private int count = 0;
        private bool isUpdate = false;
        private int updateIndex;

        public int id;
        public DataTable dt = new DataTable();
        public Boolean isOK = false;

        public SelectTime(DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            this.startDate = startDate;
            this.endDate = endDate;

            StartDateDP.DisplayDateStart = startDate;
            StartDateDP.DisplayDateEnd = endDate;
            EndDateDP.DisplayDateStart = startDate;
            EndDateDP.DisplayDateEnd = endDate;

            dt.Columns.Add("ID");
            dt.Columns.Add("开始日期");
            dt.Columns.Add("开始时间");
            dt.Columns.Add("结束日期");
            dt.Columns.Add("结束时间");


            TimeDG.ItemsSource = dt.DefaultView;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StartDateDP.SelectedDate == null) {
                //MessageBox.Show("请选择开始日期");
                StartDateDP.SelectedDate = StartDateDP.DisplayDateStart;
            }
            DateTime sd = (DateTime) StartDateDP.SelectedDate;
            if (EndDateDP.SelectedDate == null) {
                EndDateDP.SelectedDate = EndDateDP.DisplayDateEnd;
            }
            DateTime ed = (DateTime) EndDateDP.SelectedDate;
            if (StartTimeCbB.SelectedIndex == -1) {
                StartTimeCbB.SelectedIndex = 0;
            }
            int st = StartTimeCbB.SelectedIndex;
            if (EndTimeCbB.SelectedIndex == -1) {
                EndTimeCbB.SelectedIndex = EndTimeCbB.Items.Count - 1;
            }
            int et = Convert.ToInt32(EndTimeCbB.SelectedIndex + 1);


            if (sd == ed && st >= et) {
                MessageBox.Show("同一天时，开始时间不能晚于结束时间", "错误");
            } else {
                if (!isUpdate) {
                    DataRow dr = dt.NewRow();
                    count++;
                    dr[0] = count.ToString();
                    dr[1] = sd.ToString("yyyy-M-d");
                    dr[2] = st + ":00";
                    dr[3] = ed.ToString("yyyy-M-d");
                    dr[4] = et + ":00";
                    dt.Rows.Add(dr);
                } else {
                    dt.Rows[updateIndex][1] = sd.ToString("yyyy-M-d");
                    dt.Rows[updateIndex][2] = st + ":00";
                    dt.Rows[updateIndex][3] = ed.ToString("yyyy-M-d");
                    dt.Rows[updateIndex][4] = et + ":00";
                    isUpdate = false;
                    updateIndex = -1;
                }
            }
        }


        private void ClrBtn_Click(object sender, RoutedEventArgs e)
        {
            StartDateDP.SelectedDate = null;
            EndDateDP.SelectedDate = null;
            StartTimeCbB.SelectedIndex = -1;
            EndTimeCbB.SelectedIndex = -1;
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = TimeDG.SelectedIndex;
            //MessageBox.Show(dt.Rows[index][0].ToString(),index.ToString());

            if (index > -1) {
                MessageBoxResult r = MessageBox.Show("将要删除ID-" + index, "警告", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK) {
                    dt.Rows.RemoveAt(index);
                }
            } else {
                MessageBox.Show("请先选择行");
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            int index = TimeDG.SelectedIndex;
            //MessageBox.Show(dt.Rows[index][0].ToString(),index.ToString());

            if (index > -1) {
                StartDateDP.DisplayDate = Convert.ToDateTime(dt.Rows[index][1]);
                string temp = dt.Rows[index][2].ToString();
                temp = temp.Remove(temp.IndexOf(':'));          //剪去"x:00"的":00"
                StartTimeCbB.SelectedIndex = Convert.ToInt32(temp);
                EndDateDP.DisplayDate = Convert.ToDateTime(dt.Rows[index][3]);
                temp = dt.Rows[index][4].ToString();
                temp = temp.Remove(temp.IndexOf(':'));
                EndTimeCbB.SelectedIndex = Convert.ToInt32(temp) - 1;
                isUpdate = true;
                updateIndex = index;
            } else {
                MessageBox.Show("请先选择行");
            }
        }

        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            isOK = true;
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

    }
// 
//     public class Time
//     {
//         private DateTime startDate;
//         public DateTime StartDate
//         {
//             get { return startDate; }
//             set { startDate = value; }
//         }
// 
//         private DateTime endDate;
//         public DateTime EndDate
//         {
//             get { return endDate; }
//             set { endDate = value; }
//         }
// 
//         private int startTime;
//         public int StartTime
//         {
//             get { return startTime; }
//             set { startTime = value; }
//         }
// 
//         private int endTime;
//         public int EndTime
//         {
//             get { return endTime; }
//             set { endTime = value; }
//         }
// 
//         public Time(DateTime startDate, int startTime, DateTime endDate, int endTime)
//         {
//             this.StartDate = startDate;
//             this.StartTime = startTime;
//             this.endDate = endDate;
//             this.endTime = endTime;
//         }
//     }
}
