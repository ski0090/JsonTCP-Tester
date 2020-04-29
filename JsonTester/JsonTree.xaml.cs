using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Web.Script.Serialization;//안된다면 솔루션 탐색기-> 참조 -> 참조추가 ->using System.Web.Extensions
namespace JsonTester
{

   
    /// <summary>
    /// JsonTree.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JsonTreeView : UserControl
    {
        public JsonTreeView()
        {
            InitializeComponent();
        }
        public object ObjectToVisualize
        {
            get { return (object)GetValue(ObjectToVisualizeProperty); }
            set { SetValue(ObjectToVisualizeProperty, value); }
        }
        public static readonly DependencyProperty ObjectToVisualizeProperty =
            DependencyProperty.Register("ObjectToVisualize", typeof(object), typeof(JsonTreeView), new PropertyMetadata(null, OnObjectChanged));

        private static void OnObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeNode tree = TreeNode.CreateTree(e.NewValue);
            (d as JsonTreeView).TreeNodes = new List<TreeNode>() { tree };
        }

        public List<TreeNode> TreeNodes
        {
            get { return (List<TreeNode>)GetValue(TreeNodesProperty); }
            set { SetValue(TreeNodesProperty, value); }
        }
        public static readonly DependencyProperty TreeNodesProperty =
            DependencyProperty.Register("TreeNodes", typeof(List<TreeNode>), typeof(JsonTreeView), new PropertyMetadata(null));
    }
    public class TreeNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public List<TreeNode> Children  { get; set; }
        public static TreeNode CreateTree(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            Dictionary<string, object> dic = jss.Deserialize<Dictionary<string, object>>(serialized);
            var root = new TreeNode();
            root.Name = "Root";
            BuildTree(dic, root);
            return root;
        }
        public static TreeNode CreateTree(string _json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, object> dic = jss.Deserialize<Dictionary<string, object>>(_json);
            var root = new TreeNode();
            root.Name = "Root";
            BuildTree(dic, root);
            return root;
        }

        private static void BuildTree(object item, TreeNode node)
        {
            if (item is KeyValuePair<string, object>)
            {
                KeyValuePair<string, object> kv = (KeyValuePair<string, object>)item;
                TreeNode keyValueNode = new TreeNode();
                keyValueNode.Name = kv.Key;
                keyValueNode.Value = GetValueAsString(kv.Value);
                if(node.Children==null)
                {
                    node.Children=new List<TreeNode>();
                }
                node.Children.Add(keyValueNode);
                BuildTree(kv.Value, keyValueNode);
            }
            else if (item is ArrayList)
            {
                ArrayList list = (ArrayList)item;
                int index = 0;
                foreach (object value in list)
                {
                    TreeNode arrayItem = new TreeNode();
                    arrayItem.Name = "[{index}]";
                    arrayItem.Value = "";
                    if(node.Children==null)
                    {
                        node.Children=new List<TreeNode>();
                    }
                    node.Children.Add(arrayItem);
                    BuildTree(value, arrayItem);
                    index++;
                }
            }
            else if (item is Dictionary<string, object>)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>)item;
                foreach (KeyValuePair<string, object> d in dictionary)
                {
                    BuildTree(d, node);
                }
            }
        }
        private static string GetValueAsString(object value)
        {
            if (value == null)
                return "null";
            var type = value.GetType();
            if (type.IsArray)
            {
                return "[]";
            }

            if (value is ArrayList)
            {
                var arr = value as ArrayList;
                return "[{arr.Count}]";
            }

            if (type.IsGenericType)
            {
                return "{}";
            }

            return value.ToString();
        }
    }
}
