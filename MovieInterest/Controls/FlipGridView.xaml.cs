using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MovieInterest.Controls
{
    public sealed partial class FlipGridView : UserControl
    {

        public ObservableCollection<ListMediaReference> movies = new ObservableCollection<ListMediaReference>();

        public FlipGridView()
        {
            this.InitializeComponent();
            flipView.ItemsSource = movies;
            gridView.ItemsSource = movies;
            
            

            
        }

        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            flipView.SelectedItem = (e.AddedItems[0] as ListMediaReference);
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(e.AddedItems.Count);
            gridView.SelectedItem = (e.AddedItems[0] as ListMediaReference);
        }

        
    }


    public class PrependStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            return (string)parameter + (string)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // implement for two-way convertion
            throw new NotImplementedException();
        }
    }
}
