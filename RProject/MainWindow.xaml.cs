using Microsoft.Win32;
using RDotNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.IO;
using MySql.Data.MySqlClient;



namespace RProject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection conn = null;
        private MySqlConnection myConn = null;
        private DataTable csvDt;
        private REngine re;

        private int nowPage = 0;
        private int maxPage = 0;
        private int rowPerPage = 3600;
        private int length = 0;


        public MainWindow()
        {
            InitializeComponent();

            myConn = new MySqlConnection("Database=test;Data Source=127.0.0.1;User Id=root;Password=123456;pooling=false;CharSet=utf8;port=3306");
            try {
                myConn.Open();
            } catch {
                MessageBox.Show("", "数据库连接失败");
            }

            re = REngine.CreateInstance("R");
            re.Initialize();

            MySqlCommand comm = new MySqlCommand("SELECT DISTINCT cowId from `data`;", myConn);
            using (MySqlDataReader dr = comm.ExecuteReader()) {
                while (dr.Read()) {
                    CowIdCbB.Items.Add(dr.GetString(0));
                }
            }
        }

        private void ReadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV|*.csv|SQLServer|*.mdf";

            bool? result = ofd.ShowDialog();

            if (result == true) {
                if (conn != null) {
                    conn.Close();
                    conn = null;
                }
                SetDataGrid1ItemSourceInvoke(null);
                SetLoadingBarVisibilityInvoke(true);
                //Thread loadingThread = new Thread(LoadingFun);
                //loadingThread.Start(ofd.FileName);

                LoadingFun(ofd.FileName);


                ResultTextBox.Text = "已选择文件" + ofd.SafeFileName;
            } else {
                ResultTextBox.Text = "打开文件失败";
            }
        }

        private void LoadingFun(object temp)
        {
            string fileName = temp as string;

            if (fileName.EndsWith(".mdf")) {
                conn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + fileName + ";Integrated Security=True;Connect Timeout=30");
                conn.Open();

                SqlCommand comm = new SqlCommand("SELECT OBJECT_NAME (id) FROM sysobjects WHERE xtype = 'U' AND OBJECTPROPERTY (id, 'IsMSShipped') = 0", conn);
                using (SqlDataReader dr = comm.ExecuteReader()) {
                    SetCbb1ItemsInvoke("Clear", null);
                    while (dr.Read()) {
                        SetCbb1ItemsInvoke("Add", dr.GetString(0));
                    }
                }
            } else if (fileName.EndsWith(".csv")) {
                fileName = fileName.Replace('\\', '/');

                if (re != null) {
                    re = REngine.GetInstanceFromID("csvTest");
                } else {
                    re = REngine.CreateInstance("csvTest");
                }
                if (!re.IsRunning) {
                    re.Initialize();
                }
                re.Evaluate(string.Format("tb <- read.table(\"{0}\",header=TRUE)", fileName));
                length = re.Evaluate("length(tb[,1])").AsInteger()[0];
                maxPage = length / rowPerPage;
                if (length % rowPerPage != 0) { //有余数
                    maxPage++;
                }
                SetSelectPageCbBItemsInvoke("Clear", null);
                for (int i = 1; i <= maxPage; i++) {
                    SetSelectPageCbBItemsInvoke("Add", i.ToString());
                }

            }

            SetLoadingBarVisibilityInvoke(false);
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (conn != null) {
                string name = ComboBox1.SelectedValue.ToString();

                SqlCommand comm = new SqlCommand();
                comm.CommandType = CommandType.Text;
                string sqlText = "select * from " + name;
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(sqlText, conn);
                sda.Fill(dt);
                DataGrid1.ItemsSource = dt.DefaultView;
            } else {
                ComboBoxItem cbi = (ComboBoxItem) ComboBox1.SelectedItem;
                string name = cbi.Content.ToString();
                //MessageBox.Show(name);
                string commandText = "select * from " + name;
                DataTable dt = new DataTable();
                MySqlDataAdapter mySda = new MySqlDataAdapter(commandText, myConn);
                mySda.Fill(dt);
                DataGrid1.ItemsSource = dt.DefaultView;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (conn != null) {
                conn.Close();
            }
            if (myConn != null) {
                myConn.Close();
            }
            Environment.Exit(0);
        }

        private void LastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (re == null) {
                return;
            }

            if (nowPage == 0) {
                MessageBox.Show("请选择第几页");
                return;
            }

            if (nowPage != 1) {
                nowPage--;

                SetSelectPageCbBSelectedIndexInvoke(nowPage - 1);
            } else {
                MessageBox.Show("已经是第一页了");
            }
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (re == null) {
                return;
            }

            if (nowPage == 0) {
                MessageBox.Show("请选择第几页");
                return;
            }

            if (nowPage != maxPage) {
                nowPage++;

                SetSelectPageCbBSelectedIndexInvoke(nowPage - 1);
            } else {
                MessageBox.Show("已经是最后一页了");
            }
        }

        private void SelectPageCbB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nowPage = Convert.ToInt32(SelectPageCbB.SelectedValue);

            ReLoad(nowPage);
        }

        private void ReLoad(object temp)
        {
            //Thread reLoadThread = new Thread(reLoadFun);
            //reLoadThread.Start(nowPage);
            reLoadFun(nowPage);
        }

        private void reLoadFun(object temp)
        {
            SetLoadingBarVisibilityInvoke(true);

            int nowPage = Convert.ToInt32(temp);

            int rowStart = (nowPage - 1) * rowPerPage + 1;
            int rowEnd = nowPage * rowPerPage;
            if (rowEnd > length) {
                rowEnd = length;
            }

            IntegerVector iv;
            try {
                iv = re.Evaluate(string.Format("tb[{0}:{1},1]", rowStart, rowEnd)).AsInteger();
            } catch {
                //MessageBox.Show("Start" + rowStart.ToString(), "End" + rowEnd.ToString());
                MessageBox.Show("请重试");
                return;
            }
            csvDt = new DataTable();
            csvDt.Columns.Add("Line", System.Type.GetType("System.Int32"));
            csvDt.Columns.Add("x", System.Type.GetType("System.Int32"));
            DataRow dr;
            int line = rowStart;
            foreach (int i in iv) {
                dr = csvDt.NewRow();
                dr["Line"] = line;
                dr["x"] = i;
                csvDt.Rows.Add(dr);
                line++;
            }

            SetDataGrid1ItemSourceInvoke(csvDt.DefaultView);
            SetLoadingBarVisibilityInvoke(false);
        }



















    }
}
