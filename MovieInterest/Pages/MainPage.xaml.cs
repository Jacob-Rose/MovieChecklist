using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace MovieInterest
{
    /// <summary>
    /// Movie navigation page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //private int pageCount = 0;

        List<ListMediaReference> popularMovies = new List<ListMediaReference>();

        public MainPage()
        {
            this.InitializeComponent();
            popularMovies.AddRange(MediaAccessor.getPopular(1, MediaType.movie));
            replaceMovieGrid(popularMovies);

        }

        private void extendMovieGrid(List<ListMediaReference> results)
        {
            foreach (ListMediaReference movie in results)
            {
                mainGrid.Items.Add(movie); //TODO set ItemTemplate
            }
        }

        private void replaceMovieGrid(List<ListMediaReference> results)
        {
            resetMovieGrid();
            extendMovieGrid(results);
        }

        private void resetMovieGrid()
        {
            mainGrid.Items.Clear();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            //TODO: need to check what mode it is in, currently extends popular for everything
            var scrollViewer = (ScrollViewer)sender;
            double offset = scrollViewer.VerticalOffset;
            if (!(scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight))
            {
                return; //TODO: extend movie by checking count of objects and dividing by 20 to get page
            }
            
        }

        private void searchbox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //clearMovieGrid();
            if(sender.Text == "")
            {
                replaceMovieGrid(popularMovies);
            }
            else if (sender.Text.Contains("#"))
            {
                if (sender.Text.Contains("watchlist"))
                {
                    List<ListMediaReference> watchlist = new List<ListMediaReference>();
                    foreach(MovieReference wmr in (App.Current as App).UserDatabase.movies)
                    {
                        if (wmr.OnWatchlist)
                        {
                            watchlist.Add(new ListMediaReference(wmr));
                        }
                    }
                    replaceMovieGrid(watchlist);
                }
            }
            else
            {
                Debug.WriteLine(MediaAccessor.multiSearch(sender.Text).Count);
                replaceMovieGrid(MediaAccessor.multiSearch(sender.Text));
            }
        }

        private void mainGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine((e.ClickedItem as MediaReference).id);
            Frame.Navigate(typeof(MoviePage), e.ClickedItem);
        }
    }

}
