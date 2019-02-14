using MovieInterest.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieInterest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchPage : Page
    {
        public LaunchPage()
        {
            this.InitializeComponent();
            

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<ListMediaReference> mp = MediaAccessor.getPopular(1,MediaType.tv);

            foreach(ListMediaReference mm in mp)
            {
                flipGridView.movies.Add(mm);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }    
}
