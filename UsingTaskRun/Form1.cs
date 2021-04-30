using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UsingTaskRun
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cts1;
        private CancellationTokenSource _cts2;

        public Form1()
        {
            InitializeComponent();

            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Column 1";
            dataGridView1.Columns[1].Name = "Column 2";

            btnNextRun.Enabled = false;
            btnCancel.Enabled = false;
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            _cts1 = new CancellationTokenSource();

            IProgress<int> progressBarHandler = new Progress<int>(value => progressBar1.Value = value);
            IProgress<string[]> gridRowHandler = new Progress<string[]>(row => dataGridView1.Rows.Add(row));

            progressBarHandler.Report(0);
            dataGridView1.Rows.Clear();

            btnRun.Enabled = false;
            btnCancel.Enabled = true;

            await Task.Run(() => RunTask(progressBarHandler, gridRowHandler));

            btnCancel.Enabled = false;
            if (!_cts1.IsCancellationRequested)
            {
                btnNextRun.Enabled = true;
            }
            else
            {
                btnRun.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts1 != null)
            {
                _cts1.Cancel();
            }
            if (_cts2 != null)
            {
                _cts2.Cancel();
            }
        }

        private void RunTask(IProgress<int> progressBarHandler, IProgress<string[]> gridRowHandler)
        {
            CancellationToken token = _cts1.Token;

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
                return;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (_cts1 != null)
            {
                _cts1.Dispose();
            }
            if (_cts2 != null)
            {
                _cts2.Dispose();
            }
            Application.Exit();
        }

        private async void btnNextRun_Click(object sender, EventArgs e)
        {
            _cts2 = new CancellationTokenSource();

            IProgress<int> progressBarHandler = new Progress<int>(value => progressBar1.Value = value);
            IProgress<string[]> gridRowHandler = new Progress<string[]>(row => dataGridView1.Rows.Add(row));

            progressBarHandler.Report(0);
            dataGridView1.Rows.Clear();

            btnNextRun.Enabled = false;
            btnCancel.Enabled = true;

            await Task.Run(() => RunNextTask(progressBarHandler, gridRowHandler));

            btnCancel.Enabled = false;
            btnRun.Enabled = true;
        }

        private void RunNextTask(IProgress<int> progressBarHandler, IProgress<string[]> gridRowHandler)
        {
            CancellationToken token = _cts2.Token;

            try
            {
                for (int i = 101; i < 201; i++)
                {
                    progressBarHandler.Report(i - 100);
                    gridRowHandler.Report(new string[] { i.ToString(), i.ToString() });
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}
