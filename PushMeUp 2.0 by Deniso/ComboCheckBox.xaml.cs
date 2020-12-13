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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for ComboCheckBox.xaml
    /// </summary>
    public partial class ComboCheckBox : UserControl
    {
        public List<CheckBox> checkBoxes = new List<CheckBox>();
        string text = "";

        List<string> comboBoxDummyItems = new List<string>();



        public ComboCheckBox()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public List<string> comboBoxItems
        {
            get { return (List<string>)GetValue(comboBoxItemsProperty); }
            set { SetValue(comboBoxItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for comboBoxItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty comboBoxItemsProperty =
            DependencyProperty.Register("comboBoxItems", typeof(List<string>), typeof(ComboCheckBox), new PropertyMetadata(new List<string>(), OnComboBoxItemsPropertyChanged));
        private static void OnComboBoxItemsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ComboCheckBox)o).OnComboBoxItemsPropertyChanged(e);
        }

        // set up the items on the combobox
        private void OnComboBoxItemsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (string item in comboBoxItems)
            {
                CheckBox check = new CheckBox { Content = item, Style = null, IsChecked = false };
                check.Checked += new RoutedEventHandler(checkBox_CheckedChanged);
                check.Unchecked += new RoutedEventHandler(checkBox_CheckedChanged);
                checkBoxes.Add(check);
                comboBox.Items.Add(check);
            }
        }

        #endregion

        void checkBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            // remove dummy items
            comboBoxDummyItems.ForEach(x => comboBox.Items.Remove(x));
            comboBoxDummyItems.Clear();

            text = "";
            checkBoxes.FindAll(x => x.IsChecked == true).ForEach(x => text += x.Content + ", ");

            if (!comboBox.Items.Contains(text))
            {
                comboBox.Items.Add(text);
                comboBoxDummyItems.Add(text);
            }

            comboBox.Text = text;
        }

        internal List<string> GetCheckedItems()
        {
            List<string> output = new List<string>();
            checkBoxes.FindAll(x => x.IsChecked == true).ForEach(x => output.Add(x.Content.ToString()));
            return output;
        }

        internal void SetCheckedItems(List<string> items)
        {
            foreach (var item in items)
            {
                CheckBox checkBox = checkBoxes.Find(y => y.Content.ToString() == item);
                if(checkBox != null)
                {
                    checkBox.IsChecked = true;
                }
            }
            // this linq statement below doesn't check for null :(
            //items.ForEach(x => checkBoxes.Find(y => y.Content.ToString() == x).IsChecked = true);
        }
    }
}
