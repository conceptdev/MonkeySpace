using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace MonkeySpace {
    public partial class AboutPage : PhoneApplicationPage {
        public AboutPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AboutPage_Loaded);
        }

        void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            VersionText.Text = VersionInfo.AssemblyVersionString;

            CurrentTextBox.Text = App.ViewModel.ConfItem.DisplayName + Environment.NewLine
                                + App.ViewModel.ConfItem.StartDate.ToString("dd MMM yyyy") + Environment.NewLine
                                + App.ViewModel.ConfItem.EndDate.ToString("dd MMM yyyy") + Environment.NewLine
                                + Environment.NewLine
                                ;

        }
    }
}