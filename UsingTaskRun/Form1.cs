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
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();

            Progress<int> progressHandler = new Progress<int>(value => progressBar1.Value = value);
            IProgress<int> progress = progressHandler as IProgress<int>;

            progress.Report(0);
            await Task.Run(() => RunTask(progress));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null)
                _cts.Cancel();
        }

        private void RunTask(IProgress<int> progress)
        {
            var token = _cts.Token;

            try
            {
                for (int i = 0; i < 100; i++)
                {
                    progress.Report(i);
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
