using DBCheck.ViewModel;
using Microsoft.Win32;
using OracleEx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OracleGenerate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SchemeContext context;
        CheckViewModel model;
        public MainWindow(SchemeContext context)
        {
            InitializeComponent();
            this.context = context;
            this.DataContext = model = new();
        }

        #region 基表查询语句
        //获取用户创建的表
        //select table_name from user_tables;

        //获取表字段
        //select* from user_tab_columns where Table_Name = '用户表';
        //获取表注释
        //select* from user_tab_comments user_tab_comments：table_name,table_type,comments

        //获取字段注释
        //select * from user_col_comments
        #endregion

        private void ClearMsg()
        {
            txtMsg.Content = "";
        }

        private void SetMsg(string msg)
        {
            txtMsg.Content = msg;
        }


        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            ClearMsg();
            SetMsg("计算中...");

            Task.Run(() =>
            {
                Calc();
            });
        }
        private void btnCheckNotExist_Click(object sender, RoutedEventArgs e)
        {
            ClearMsg();
            SetMsg("计算中...");

            Task.Run(() =>
            {
                Calc(false);
            });
        }

        private async Task Calc(bool exist = true)
        {
            //获取所有表
            List<string> tableNames = new();
            this.Dispatcher.Invoke(() =>
            {
                SetMsg("计算中...-尝试获取相关表");
            });
            if (string.IsNullOrWhiteSpace(model.CheckTableName))
            {
                tableNames = context.UserTables.Select(f => f.TableName).ToList();
            }
            else
            {
                tableNames = context.UserTables.Where(f => f.TableName.Contains(model.CheckTableName.ToUpper()))?.Select(f => f.TableName).ToList();
            }

            if (tableNames == null || tableNames.Count == 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetMsg("计算完成-未命中任何表");
                });
                return;
            }
            this.Dispatcher.Invoke(() =>
            {
                SetMsg("计算中...-成功获取相关表");
            });


            //表字段
            var existTables = context.UserTabColumns.Where(f => tableNames.Contains(f.TableName) && (exist ? f.ColumnName == model.CheckFieldName : f.ColumnName != model.CheckFieldName)).Select(f => f.TableName);

            this.Dispatcher.Invoke(() =>
            {
                model.ResultTableList = new ObservableCollection<string>(existTables);
            });


            this.Dispatcher.Invoke(() =>
            {
                SetMsg("计算完成");
            });
        }

        private void labelTable_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock block)
            {
                try
                {
                    Clipboard.SetText(block.Text);
                    SetMsg($"复制成功-{block.Text}");

                }
                catch (System.Exception)
                {
                    SetMsg("无法获取剪贴板权限");
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!(model.ResultTableList?.Any() ?? false))
            {
                return;
            }
            var dialog = new SaveFileDialog();
            var dialogResult = dialog.ShowDialog();
            if (dialogResult != true)
            {
                return;
            }

            var file = new FileInfo(dialog.FileName);
            if (File.Exists(dialog.FileName))
            {
                File.Delete(file.FullName);
            }
            foreach (var item in model.ResultTableList)
            {
                File.AppendAllText(file.FullName, item + "\r\n");
            }

            SetMsg("导出完成");
        }
    }
}
