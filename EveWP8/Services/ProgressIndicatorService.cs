using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EveWP8.Controls;

namespace EveWP8.Services
{
    public interface IProgressIndicatorService
    {
        void Show(string message);
        void Hide();
    }

    public class ProgressIndicatorService : IProgressIndicatorService
    {
        private ProgressIndicatorEx _progressIndicatorEx = new ProgressIndicatorEx();


        public void Show(string message)
        {
            _progressIndicatorEx.Text = message;
            _progressIndicatorEx.Show();
        }

        public void Hide()
        {
            _progressIndicatorEx.Hide();
        }
    }
}
