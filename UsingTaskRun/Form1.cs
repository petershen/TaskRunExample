using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsingTaskRun
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cts;

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Column 1";
            dataGridView1.Columns[1].Name = "Column 2";

            btnCancel.Enabled = false;
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();

            IProgress<int> progressBarHandler = new Progress<int>(value => progressBar1.Value = value);
            IProgress<string[]> gridRowHandler = new Progress<string[]>(row => dataGridView1.Rows.Add(row));

            progressBarHandler.Report(0);
            dataGridView1.Rows.Clear();

            btnRun.Enabled = false;
            btnCancel.Enabled = true;

            await Task.Run(() => RunTask(progressBarHandler, gridRowHandler));

            btnCancel.Enabled = false;
            btnRun.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                btnRun.Enabled = true;
            }
        }

        private void RunTask(IProgress<int> progressBarHandler, IProgress<string[]> gridRowHandler)
        {
            CancellationToken token = _cts.Token;

            try
            {
                for (int i = 1; i < 101; i++)
                {
                    progressBarHandler.Report(i);
                    gridRowHandler.Report(new string[] { i.ToString(), i.ToString() });
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException)
            {
                _cts.Dispose();
                return;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                _cts.Dispose();
            }
            Application.Exit();
        }
    }
}
