using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RProject
{
    public partial class MainWindow : Window
    {
        //group_statistics
        private void GSBtn_Click(object sender, RoutedEventArgs e)
        {
            List<string> varNames = new List<string>();
            foreach (CowListItem i in Cows) {
                if (i.IsChecked == true) {
                    if (!ht.ContainsKey(i.CowId) || !((SelectTime) ht[i.CowId]).isOK) {
                        MessageBox.Show("请选择时段并且确认按下了确定按钮", i.CowId.ToString());
                        return;
                    }
                    if (i.Threshold == -1) {
                        MessageBox.Show("请选择阈值", i.CowId.ToString());
                        return;
                    }
                    varNames.Add("GCow" + i.CowId);
                }
            }
            foreach (string i in varNames) {
                ReadDataToR_D1(Convert.ToInt32( i.Substring(4)), i);
            }
            StatisticsByR_G(varNames);
        }

        private void StatisticsByR_G(List<string> varNames)
        {
            double avg = 0;
            double sd2 = 0;     //方差
            int max = 0;
            int min = 0;
            int median = 0;     //中位数
            int mode = 0;       //众数

            avg = RCommand.Avg(varNames);
            GAVGL.Content = avg.ToString();

            sd2 = RCommand.Var(varNames);
            GSD2L.Content = sd2.ToString();

            max = RCommand.Max(varNames);
            GMaxL.Content = max.ToString();

            min = RCommand.Min(varNames);
            GMinL.Content = min.ToString();

            median = RCommand.Median(varNames);
            GMedianL.Content = median.ToString();

            mode = RCommand.Mode(varNames);
            GModeL.Content = mode.ToString();
        }
    }
}
