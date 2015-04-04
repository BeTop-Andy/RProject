using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RDotNet;

namespace RProject
{
    public partial class MainWindow : Window
    {
        private Setting setting = new Setting();

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            int rowPerPageTemp = 0;
            try {
                rowPerPageTemp = Convert.ToInt32(RowPerPageTB.Text);
            } catch {
                MessageBox.Show("请输入一个整数");
                RowPerPageTB.Focus();
                return;
            }
            if (rowPerPageTemp < 1) {
                MessageBox.Show("请输入大于0的整数");
                RowPerPageTB.Focus();
                return;
            }
            if (rowPerPage != rowPerPageTemp) {
                if (re != null) {
                    MessageBoxResult result = MessageBox.Show("已读入文件，改变每页的行数要重新读入文件", "注意", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK) {
                        int length = re.Evaluate("length(tb[,1])").AsInteger()[0];

                        maxPage = length / rowPerPage;
                        if (length % rowPerPage != 0) { //有余数
                            maxPage++;
                        }

                        SetSelectPageCbBItemsInvoke("Clear", "");
                        for (int i = 1; i <= maxPage; i++) {
                            SetSelectPageCbBItemsInvoke("Add", i.ToString());
                        }
                        SetDataGrid1ItemSourceInvoke(null);
                        setting.RowPerPage = rowPerPageTemp;
                    } else {
                        return;
                    }
                } else {
                    setting.RowPerPage = rowPerPageTemp;
                }
            }
            if (Set()) {
                MessageBox.Show(setting.ToString(), "设置成功");
            } else {
                MessageBox.Show("设置失败");
            }
        }


        private bool Set()
        {
            try {
                rowPerPage = setting.RowPerPage;

                return true;
            } catch {
                return false;
            }
        }
    }

    class Setting
    {
        private int rowPerPage;

        public int RowPerPage
        {
            get { return rowPerPage; }
            set { rowPerPage = value; }
        }

        public Setting()
        {

        }

        public override string ToString()
        {
            return "每页的行数：" + this.RowPerPage;
        }

    }
}
