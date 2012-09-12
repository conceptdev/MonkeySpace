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


namespace MonkeySpace {
    public partial class InfoPage : PhoneApplicationPage {
        public InfoPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(InfoPage_Loaded);
        }

        void InfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            //WebView.NavigateToString("");
            WebView.Navigate (new Uri("http://monkeyspace.org"));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            PageTitle.Text = "MonkeySpace info";
        }
    }
}