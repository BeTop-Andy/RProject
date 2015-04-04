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

namespace RProject
{
    /// <summary>
    /// SetSliderValue.xaml 的交互逻辑
    /// </summary>
    public partial class SetSliderValue : Window
    {
        public bool isSet = false;
        public int newValue = 0;

        public SetSliderValue(int nowValue)
        {
            InitializeComponent();
            L1.Content += nowValue.ToString();
            TB1.Focus();
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            try {
                newValue = Convert.ToInt32(TB1.Text);
                    isSet = true;
                    this.Close();
            } catch {
                MessageBox.Show("请输入一个整数");
            }
        }
        
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TB1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                OKBtn_Click(null, null);
            }
        } 
    }
}
