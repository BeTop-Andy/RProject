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
        private void SelectTimeB_Click(object sender, RoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            ContentPresenter cp = btn.TemplatedParent as ContentPresenter;
            CowListItem cli = cp.Content as CowListItem;
            //MessageBox.Show(cli.CowId.ToString());
        }
    }
}
