using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;

namespace Hierarchie22
{
    public class MenuItem: TreeViewItemBase
	{
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string MTitle { get; set; }
		public string Klass;
        public ObservableCollection<MenuItem> Items { get; set; }  
    }

	public class Person : TreeViewItemBase
	{
		public Person()
		{
			this.Children = new ObservableCollection<Person>();
		}

		public string Name { get; set; }

		public int Age { get; set; }

		public ObservableCollection<Person> Children { get; set; }
	}

	public class TreeViewItemBase : INotifyPropertyChanged
	{
		private bool isSelected;
		public bool IsSelected
		{
			get { return this.isSelected; }
			set
			{
				if (value != this.isSelected)
				{
					this.isSelected = value;
					NotifyPropertyChanged("IsSelected");
				}
			}
		}

		private bool isExpanded;
		public bool IsExpanded
		{
			get { return this.isExpanded; }
			set
			{
				if (value != this.isExpanded)
				{
					this.isExpanded = value;
					NotifyPropertyChanged("IsExpanded");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}
