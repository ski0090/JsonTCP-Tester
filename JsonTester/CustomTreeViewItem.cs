using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace JsonTester
{
    abstract class TreeJsonItem : TreeViewItem
    {
        protected String jsonTag = null;
        protected String MakeJasonName(string _name, char chValue)
        {
            String retString = "\"" + _name + "\":" + chValue;
            return retString;
        }
        public abstract string MakeJson();
    }
    class JsonObjsectItem : TreeJsonItem
    {
        TextBlock strSub = new TextBlock();
        public JsonObjsectItem()
        {
            IsExpanded = true;
            Name = "Object";
            jsonTag += '{';
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Text = Name;
            strSub.Foreground = Brushes.Green;
            // Add
            stack.Children.Add(strSub);

            // assign stack to header
            Header = stack;

        }
        public JsonObjsectItem(int index)
        {
            IsExpanded = true;
            Name = "Object";
            jsonTag += '{';
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Text = index.ToString() + "(index)";
            strSub.Foreground = Brushes.Green;
            // Add
            stack.Children.Add(strSub);

            // assign stack to header
            Header = stack;

        }

        public JsonObjsectItem(string _strSubject)
        {
            IsExpanded = true;
            Name = "Object";
            jsonTag = MakeJasonName(_strSubject, '{');

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Text = _strSubject;
            strSub.Foreground = Brushes.Green;
            // Add
            stack.Children.Add(strSub);

            // assign stack to header
            Header = stack;
        }
        public override string MakeJson()
        {
            String retJson = null;

            retJson += jsonTag;
            foreach (TreeJsonItem value in this.Items)
            {
                retJson += value.MakeJson() + ",";
            }
            retJson = retJson.Remove(retJson.Length - 1);
            retJson += "}";
            return retJson;

        }

    }

    class JsonArrayItem : TreeJsonItem
    {
        TextBlock strSub = new TextBlock();
        string Subject { get { return strSub.Text; } }
        public JsonArrayItem(string _strSubject)
        {
            Name = "Array";
            IsExpanded = true;
            jsonTag = MakeJasonName(_strSubject, '[');
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Text = _strSubject + "(Array)";
            strSub.Foreground = Brushes.Gray;
            // Add
            stack.Children.Add(strSub);

            // assign stack to header
            Header = stack;
        }
        public override string MakeJson()
        {
            String retJson = null;
            retJson += jsonTag;
            foreach (TreeJsonItem value in this.Items)
            {
                retJson += value.MakeJson() + ",";
            }
            retJson = retJson.Remove(retJson.Length - 1);
            retJson += "]";
            return retJson;
        }
    }
    class JsonDataItem : TreeJsonItem
    {
        TextBlock strSub = new TextBlock();
        TextBox strEle = null;
        Image binImage=null;
        string Elements { get { return strEle.Text; } }
        string Subject { get { return strSub.Text; } }

        public JsonDataItem(string _strSubject, string _element)
        {
            IsExpanded = true;
            Name = _strSubject;
            jsonTag = "\"" + _strSubject + "\":";
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Foreground = Brushes.Yellow;
            
            stack.Children.Add(strSub);

            // 내용
            //이미지가 있을 경우
            if (_strSubject == "imgdata")
            {
                binImage=new Image();
                strSub.Text = _strSubject + "(bin): ";
                try
                {

                    byte[] binaryData = Convert.FromBase64String(_element);
                    binImage.Source = BitmapSource.Create(304, 344, 96, 96, PixelFormats.Gray8, null, binaryData, 304);

                    stack.Children.Add(binImage);

                }
                catch (Exception e)
                {
                    strEle = new TextBox();
                    strEle.Text = _element;
                    strEle.Background = Brushes.DarkSlateGray;
                    strEle.Foreground = Brushes.LightGray;
                    stack.Children.Add(strEle);

                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                strSub.Text = _strSubject + "(string): ";
                strEle = new TextBox();
                strEle.Text = _element;
                strEle.Background = Brushes.DarkSlateGray;
                strEle.Foreground = Brushes.LightGray;
                stack.Children.Add(strEle);
            }
            // Add into stack
            Header = stack;
        }
        public JsonDataItem(string _strSubject, int _element)
        {
            IsExpanded = true;
            Name = _strSubject;
            jsonTag = "\"" + _strSubject + "\":";
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Foreground = Brushes.Yellow;
            strSub.Text = _strSubject + "(Int): ";

            // 내용
            strEle.Text = _element.ToString();
            strEle.Background = Brushes.DarkSlateGray;
            strEle.Foreground = Brushes.LightGray;

            // Add into stack
            stack.Children.Add(strSub);
            stack.Children.Add(strEle);
            Header = stack;
        }
        public JsonDataItem(string _strSubject, float _element)
        {
            IsExpanded = true;
            Name = _strSubject;
            jsonTag = "\"" + _strSubject + "\":";
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create 제목
            strSub.Foreground = Brushes.Yellow;
            strSub.Text = _strSubject + "(float): ";

            // 내용
            strEle.Text = _element.ToString();
            strEle.Background = Brushes.DarkSlateGray;
            strEle.Foreground = Brushes.LightGray;

            // Add into stack
            stack.Children.Add(strSub);
            stack.Children.Add(strEle);
            Header = stack;
        }
        public override string MakeJson()
        {
            return jsonTag + "\"" + Elements + "\"";
        }

        private void FromBase64String(string strSrc, int m, byte[] dst)
        {
            int[] i64 = {    0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                             0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                             0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                             0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  0,  0,  0,  0,  0,  0,
                             0, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
                            25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35,  0,  0,  0,  0,  0,
                             0, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                            51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62,  0, 63,  0,  0};
            int i, ii, j, k, kk, l;
            j = 0;
            for (i = 0; i < m; i += 4)
            {
                k = i64[strSrc[i + 3]];
                for (ii = 0; ii < 3; ii++)
                {
                    l = i64[strSrc[i + ii]];
                    kk = k % 4;
                    k /= 4;
                    dst[j++] = (byte)(l + (64 * kk));
                }
            }
            return;

        }
    }
}
