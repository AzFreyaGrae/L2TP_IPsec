using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace L2TP_IPsec
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            progressBar1.Minimum = 0; //设置进度条最小值
            progressBar1.Maximum = 4; //设置进度条最大值
            progressBar1.Value = 0; //初始值0
            button1.Enabled = true;
        }

        private delegate void UpdateLabelDelegate(string text);

        private void UpdateLabel(string text)
        {
            if (label6.InvokeRequired)
            {
                label6.Invoke(new UpdateLabelDelegate(UpdateLabel), text);
            }
            else
            {
                label6.Text = text;
            }
        }

        // 导入注册表 RasMan
        private void RasMan()
        {
            string regContent = Properties.Resources.RasMan; // "RasMan" 是嵌入资源的名称

            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, regContent);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"/s \"{tempFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process regeditProcess = Process.Start(startInfo))
            {
                regeditProcess.WaitForExit();

                if (regeditProcess.ExitCode == 0)
                {
                    UpdateLabel("注册表 RasMan 内容已成功导入。");
                }
                else
                {
                    UpdateLabel("导入注册表 RasMan 时出现错误。");
                }
            }

            File.Delete(tempFilePath);
        }

        private delegate void UpdateLabelDelegate1(string text);

        private void UpdateLabel1(string text)
        {
            if (label5.InvokeRequired)
            {
                label5.Invoke(new UpdateLabelDelegate1(UpdateLabel1), text);
            }
            else
            {
                label5.Text = text;
            }
        }

        // 导入注册表 PolicyAgent
        private void PolicyAgent()
        {
            string regContent = Properties.Resources.PolicyAgent; // "PolicyAgent" 是嵌入资源的名称

            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, regContent);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"/s \"{tempFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (Process regeditProcess = Process.Start(startInfo))
            {
                regeditProcess.WaitForExit();

                if (regeditProcess.ExitCode == 0)
                {
                    UpdateLabel1("注册表 PolicyAgent 内容已成功导入。");
                }
                else
                {
                    UpdateLabel1("导入注册表 PolicyAgent 时出现错误。");
                }
            }

            File.Delete(tempFilePath);
        }

        // 重置网络
        private void Cmd_Code()
        {
            string[] commands = new string[]
            {
                "netsh winsock reset",
                "netsh winhttp reset proxy",
                "sc config \"RemoteAccess\" start= AUTO",
                "net start \"RemoteAccess\""
            };

            foreach (string command in commands)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine($"命令 '{command}' 执行成功。");
                    }
                    else
                    {
                        Console.WriteLine($"命令 '{command}' 执行失败。");
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            // 创建一个 BackgroundWorker 实例
            BackgroundWorker bw = new BackgroundWorker
            {
                // 设置 BackgroundWorker 可以报告进度和支持异步取消操作
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            // 添加 DoWork 事件处理程序，其中包含要在后台执行的操作
            bw.DoWork += (sender1, e1) =>
            {
                RasMan();  // 修复注册表
                bw.ReportProgress(1, "修复 RasMan 注册表项...");
                System.Threading.Thread.Sleep(1000); // 延时1秒

                PolicyAgent(); // 修复注册表项
                bw.ReportProgress(2, "修复 PolicyAgent 注册表项...");
                System.Threading.Thread.Sleep(1000); // 延时1秒

                Cmd_Code(); // 清空WinSock / 重置代理 / 启动RemoteAccess服务
                bw.ReportProgress(3, "清空WinSock...");
                System.Threading.Thread.Sleep(1000); // 延时1秒
                
                bw.ReportProgress(4, "已修复完成，请重启电脑后连接...");
                System.Threading.Thread.Sleep(1000); // 延时1秒
                MessageBox.Show(" - 请重新启动电脑使其设置生效。 \n - Ae (FreyaGrace)", "修复完成");
            };

            // 添加 ProgressChanged 事件处理程序，其中包含要在报告进度时执行的操作
            bw.ProgressChanged += (sender2, e2) =>
            {
                progressBar1.Value = e2.ProgressPercentage;
                label3.Text = e2.UserState.ToString();
            };

            // 启动 BackgroundWorker
            bw.RunWorkerAsync();
        }

        private void 使用说明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("L2TP连接尝试失败，因为安全层在初始化与远程计算机的协商时遇到一个处理错误。", "连接到xxxx错误", MessageBoxButtons.YesNo);

            // 根据用户的响应做出相应的处理
            if (result == DialogResult.Yes)
            {
                // 用户选择了“是”
            }
            else
            {
                // 用户选择了“否”
            }
        }

        private void 广告位招租ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("都说了广告位招租!你要租吗？？？","！！！广告位招租！！！");
        }

        private void 软件信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(" - L2TP连接尝试失败，因为安全层在初始化与远程计算机的协商时遇到一个处理错误。 \n - 点击修复，重启电脑即可。 \n - Ae 20240109 (FreyaGrace)", "处理 L2TP/IPsec 报错");
        }
    }
}
