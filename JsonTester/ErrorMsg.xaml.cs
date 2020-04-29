using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JsonTester
{
    /// <summary>
    /// ErrorMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ErrorMsg : Window
    {
        static ErrorMsg pInstance = null;
        public ErrorMsg()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
        }
        public static void ErrorMessage(string strError)
        {
            if (pInstance == null)
                pInstance = new ErrorMsg();
            pInstance.mErrorMsg.Text = strError;
            pInstance.mErrorMsg.IsReadOnly = true;
            try
            {
                pInstance.Show();
            }
            catch
            {
                pInstance.mErrorMsg.Text = strError;
            }
        }

        private void OnBtOk(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
