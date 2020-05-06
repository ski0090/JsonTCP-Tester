using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Threading;
namespace JsonTester
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Regex regex = new Regex("[^0-9.-]+");
        TcpClient mSocket = new TcpClient();
        Dictionary<string, string> mapJsonPath = new Dictionary<string, string>();
        Encoding eucKr = Encoding.GetEncoding("euc-kr");
        Thread reiciveThread=null;
        public void ErrorMsg(string strMsg)
        {
            ErrText.Text = strMsg;
            ErrMsg.Visibility = Visibility.Visible;
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialize_Tester(object sender, EventArgs e)
        {
            addrIP.Text = Properties.Settings.Default.ipAddress;
            mPacketStart.Text = Properties.Settings.Default.startPacket;
            mPacketEnd.Text = Properties.Settings.Default.endPacket;
            port.Text = Properties.Settings.Default.portNumber;
           
            OnBtGetJson(null, null);
        }
        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }
        private void OnBtConnet(object sender, RoutedEventArgs e)
        {
            Button connectBtn = sender as Button;
            if (mSocket.Client == null || !mSocket.Connected)
            {
                try
                {
                    mSocket = new TcpClient(addrIP.Text, int.Parse(port.Text));
                    reiciveThread = new Thread(RecieveSocket);
                    reiciveThread.Start();

                    connectBtn.Content = "Disconnect";
                    mBtSend.Visibility = Visibility.Visible;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            else
            {
                mSocket.Close();
                connectBtn.Content = "Connect";
                mBtSend.Visibility = Visibility.Hidden;
                reiciveThread.Abort();
            }
        }
        private void OnBtGetJson(object sender, RoutedEventArgs e)
        {
            mapJsonPath.Clear();
            JsonCombo.Items.Clear();
            String currentFolder = Directory.GetCurrentDirectory();

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(currentFolder);

            foreach (System.IO.FileInfo file in di.GetFiles())
            {
                if (file.Extension.ToLower().CompareTo(".json") == 0)
                {
                    String strFileName = file.Name.Substring(0, file.Name.Length - 4);
                    String strPath = file.FullName;
                    mapJsonPath.Add(strFileName, strPath);
                    JsonCombo.Items.Add(strFileName);
                }
            }

        }
        private void JsonCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.Items.Count == 0 ||
                mapJsonPath.Count == 0)
                return;

            string strPath = mapJsonPath[cmb.SelectedItem.ToString()].ToString();
            try
            {
                var strJson = File.ReadAllText(strPath);
                UpdateTreeView(sendTree, strJson);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private TreeViewItem MakeTreeItems(object item,int iIndex=-1)
        {
            TreeViewItem _retNode=null;
            if (item is KeyValuePair<string, object>)
            {
                var KeyItem=(KeyValuePair<string, object>)item;
                if (KeyItem.Value is string)
                {
                    string strValue = KeyItem.Value as string;
                    _retNode = new JsonDataItem(KeyItem.Key, strValue);//Data
                }
                else if(KeyItem.Value is int)
                {
                    int strValue = (int)KeyItem.Value;
                    _retNode = new JsonDataItem(KeyItem.Key, strValue);//Data
                }

                else if (KeyItem.Value is float)
                {
                    float strValue = (float)KeyItem.Value;
                    _retNode = new JsonDataItem(KeyItem.Key, strValue);//Data
                }
                else if (KeyItem.Value is ArrayList)
                {
                    _retNode = new JsonArrayItem(KeyItem.Key);
                    int _iIndex = 0;
                    foreach (object have_name_Object in KeyItem.Value as ArrayList)
                    {
                        TreeViewItem _childNode = MakeTreeItems(have_name_Object, _iIndex);
                        _retNode.Items.Add(_childNode);
                        _iIndex++;
                    }
                }
                else if (KeyItem.Value is Dictionary<string, object>)
                {
                    _retNode = new JsonObjsectItem(KeyItem.Key);
                    foreach (KeyValuePair<string, object> have_name_Object in KeyItem.Value as Dictionary<string, object>)
                    {
                        TreeViewItem _childNode = MakeTreeItems(have_name_Object);
                        _retNode.Items.Add(_childNode);
                    }
                }
            }
            else if (item is Dictionary<string, object>)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
                if(-1 < iIndex)
                    _retNode = new JsonObjsectItem(iIndex);
                else
                    _retNode = new JsonObjsectItem();
                
                foreach (KeyValuePair<string, object> have_name_Object in dictionary)
                {
                    TreeViewItem _childNode = MakeTreeItems(have_name_Object);
                    _retNode.Items.Add(_childNode);
                }
            }
            return _retNode;
        }
        private String MakeJson()
        {
            String retJson=null;
            try
            {
                TreeJsonItem _node = sendTree.Items[0] as TreeJsonItem;
                retJson = _node.MakeJson();
            }
            catch (Exception e)
            {
                ErrText.Text = "Send 할 내용이 없습니다.";
                ErrMsg.Visibility = Visibility.Visible;
            }
            return retJson;
        }
        private void EditJson(object sender, RoutedEventArgs e)
        {
            if (JsonCombo.Items.Count == 0 ||
                JsonCombo.SelectedIndex == -1)
                return;
            String currentFolder = mapJsonPath[JsonCombo.SelectedItem.ToString()].ToString();
            System.Diagnostics.Process.Start(currentFolder);
        }
        private void AddJson(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process ps = new System.Diagnostics.Process();
            try
            {
                ps.StartInfo.FileName = "C:\\Users\\KimPK\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe";
                ps.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                ps.Start();
            }
            catch
            {
                ps.StartInfo.FileName = "notepad";
                ps.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                ps.Start();
            }

            OnBtGetJson(null, null);
        }
        private void OnBtSendJson(object sender, RoutedEventArgs e)
        {
            int iStartPacket = Int32.Parse(mPacketStart.Text, System.Globalization.NumberStyles.HexNumber);
            int iEndPacket = Int32.Parse(mPacketEnd.Text, System.Globalization.NumberStyles.HexNumber);
            Byte startPacket = (Byte)Char.ConvertFromUtf32(iStartPacket)[0];
            Byte endPacket = (Byte)Char.ConvertFromUtf32(iEndPacket)[0];
            //보낼 버퍼
            string strJson = MakeJson();
            if (strJson == null)
                return;
            byte[] jsonBuffer =System.Text.Encoding.Default.GetBytes(strJson);
            byte[] sendBuffer = new Byte[6 + jsonBuffer.Length];
            byte[] jsonLength = BitConverter.GetBytes(jsonBuffer.Length);

            Buffer.SetByte(sendBuffer, 0, startPacket);
            Buffer.BlockCopy(jsonLength,0,sendBuffer, 1, 4);
            Buffer.BlockCopy(jsonBuffer, 0, sendBuffer, 5, jsonBuffer.Length);
            Buffer.SetByte(sendBuffer, 5 + jsonBuffer.Length, endPacket);
            mSocket.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
        }
        void UpdateTreeView(TreeView _treeView,string strJson) 
        {
            try
            {
                _treeView.Items.Clear();
                JavaScriptSerializer js = new JavaScriptSerializer();
                Dictionary<string, object> dic = js.Deserialize<Dictionary<string, object>>(strJson);
                TreeViewItem rootNode = MakeTreeItems(dic);
                if (rootNode == null)
                {
                    throw new Exception("Json 구문 오류..");
                }
                _treeView.Items.Add(rootNode);

            }
           catch(Exception e){
               MessageBox.Show(e.Message);
           }
           return;
             
        }

        private void OnBtClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mSocket.Close();
            if (reiciveThread != null)
            {
                if (reiciveThread.IsAlive)
                    reiciveThread.Abort();
            }
            Properties.Settings.Default.startPacket = mPacketStart.Text;
            Properties.Settings.Default.endPacket = mPacketEnd.Text;
            Properties.Settings.Default.portNumber = port.Text;
            Properties.Settings.Default.ipAddress = addrIP.Text;

            Properties.Settings.Default.Save();
        }
        private void RecieveSocket()
        {
            while (true)
            {
                int recvLen = 0;
                byte[] recvBuffer = new Byte[5];
                int bufferLength = 0;
                if (!mSocket.Connected)
                    return;
                try
                {
                    recvLen = mSocket.GetStream().Read(recvBuffer, 0, 5);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
                if (recvLen == 0)
                {
                    mSocket.Close();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       mBtConnect.Content = "Connect";
                       mBtSend.Visibility = Visibility.Hidden;
                   }));
                    return;
                }
                bufferLength = BitConverter.ToInt32(recvBuffer, 1);

                byte[] recvJsonBuffer = new Byte[bufferLength];
                int offset = 0;
                while (0 != bufferLength)
                {
                    recvLen = mSocket.GetStream().Read(recvJsonBuffer, offset, bufferLength);
                    offset += recvLen;
                    bufferLength -= recvLen;
                }
                string recvJson = eucKr.GetString(recvJsonBuffer);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    mReiciveHistory.SelectedIndex = mReiciveHistory.Items.Add(recvJson);
                }));

                mSocket.GetStream().Read(recvBuffer, 0, 1);
            }
        }

        private void MakeReiciveTreeView(object sender, SelectionChangedEventArgs e)
        {

            string recvJson = mReiciveHistory.Items[mReiciveHistory.SelectedIndex].ToString();
            UpdateTreeView(recvTree, recvJson); 
        }

        private void OnBtErrOk(object sender, RoutedEventArgs e)
        {
            ErrMsg.Visibility = Visibility.Hidden;
        }
    }

}
