using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Data;

namespace RProject
{
    public partial class MainWindow : Window
    {
        private void SetSelectPageCbBSelectedIndexInvoke(int index)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate { SetSelectPageCbBSelectedIndex(index); });
        }
        private void SetSelectPageCbBSelectedIndex(int index)
        {
            SelectPageCbB.SelectedIndex = index;
        }

        private void SetLoadingBarVisibilityInvoke(bool temp)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate()
            {
                SetLoadingBarVisibility(temp);
            });
        }
        private void SetLoadingBarVisibility(bool temp)
        {
            if (temp) {
                LoadingBar.Visibility = System.Windows.Visibility.Visible;
            } else {
                LoadingBar.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void SetDataGrid1ItemSourceInvoke(DataView dv)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate()
            {
                SetDataGrid1ItemSource(dv);
            });
        }
        private void SetDataGrid1ItemSource(DataView dv)
        {
            DataGrid1.ItemsSource = dv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">Clear,Add</param>
        /// <param name="value"></param>
        private void SetCbb1ItemsInvoke(string op, string value)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate() { SetCbb1Items(op, value); });
        }
        private void SetCbb1Items(string op, string value)
        {
            switch (op) {
                case "Clear":
                    ComboBox1.Items.Clear();
                    break;
                case "Add":
                    ComboBox1.Items.Add(value);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">Clear,Add</param>
        /// <param name="value"></param>
        private void SetSelectPageCbBItemsInvoke(string op, string value)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate() { SetSelectPageCbBItems(op, value); });
        }
        private void SetSelectPageCbBItems(string op, string value)
        {
            switch (op) {
                case "Clear":
                    SelectPageCbB.Items.Clear();
                    break;
                case "Add":
                    SelectPageCbB.Items.Add(value);
                    break;
            }
        }

    }
}
