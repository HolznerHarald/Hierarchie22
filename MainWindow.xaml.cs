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
using System.IO;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Hierarchie22
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> NurName = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();

            List<Person> persons = new List<Person>();
            Person person1 = new Person() { Name = "John Doe", Age = 42 };
            Person person2 = new Person() { Name = "Jane Doe", Age = 39 };
            Person child1 = new Person() { Name = "Sammy Doe", Age = 13 };
            person1.Children.Add(child1);
            person2.Children.Add(child1);
            person2.Children.Add(new Person() { Name = "Jenny Moe", Age = 17 });
            Person person3 = new Person() { Name = "Becky Toe", Age = 25 };
            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);
            person2.IsExpanded = true;
            person2.IsSelected = true;
            trvPersons.ItemsSource = persons;
        }
        
        private void Button_Click  (object sender, RoutedEventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();
            var result = folderBrowser.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var ordnerInfo = new DirectoryInfo(folderBrowser.SelectedPath);
                if (ordnerInfo.Exists)
                {
                    Dateiliste.Items.Clear();
                    NurName.Clear();
                    foreach (var dateiInfo in ordnerInfo.GetFiles())
                    {
                        String hname = dateiInfo.FullName;
                        string hs;
                        hs = hname.Substring(hname.Length - 3);
                        if ((hs == ".cs") && (File.Exists(hname)))
                        {
                            Dateiliste.Items.Add(dateiInfo);
                            NurName.Add(dateiInfo.FullName, dateiInfo.Name);
                        }
                    }
                }
                analyse anal = new analyse();
                anal.analyseMain();                
             }
        }

        private void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (trvMenu.SelectedItem != null)
            {
                var MItem = (trvMenu.SelectedItem as MenuItem);
                Ausgabe5.Text = MItem.MTitle+"  " +  MItem.Klass;
            }
        }
        private void btnSelectNext_Click(object sender, RoutedEventArgs e)
        {
            if (trvPersons.SelectedItem != null)
            {
                var list = (trvPersons.ItemsSource as List<Person>);
                int curIndex = list.IndexOf(trvPersons.SelectedItem as Person);
                if (curIndex >= 0)
                    curIndex++;
                if (curIndex >= list.Count)
                    curIndex = 0;
                if (curIndex >= 0)
                    list[curIndex].IsSelected = true;
            }
        }

        private void btnToggleExpansion_Click(object sender, RoutedEventArgs e)
        {
            if (trvPersons.SelectedItem != null)
                (trvPersons.SelectedItem as Person).IsExpanded = !(trvPersons.SelectedItem as Person).IsExpanded;
        }
    }    
}
