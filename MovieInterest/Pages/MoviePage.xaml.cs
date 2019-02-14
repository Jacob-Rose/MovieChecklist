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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MovieInterest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoviePage : Page
    {
        private MediaReference movieInfo;

        public MoviePage()
        {
            this.InitializeComponent();
            userRating.RatingChanged += UserRating_RatingChanged;
            Loaded += MoviePage_Loaded;
            
        }

        private void MoviePage_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic d = MediaAccessor.getDetails(movieInfo.id, movieInfo.mediaType);
            poster_image.Source = new BitmapImage(new Uri("https://image.tmdb.org/t/p/w1280" + d.poster_path));
            if ((App.Current as App).UserDatabase.movies.Exists(x => (x.id) == movieInfo.id))
            {
                userRating.Score = (App.Current as App).UserDatabase.movies.Find(x => (x.id) == movieInfo.id).review.quality;
            }

            if ((App.Current as App).UserDatabase.movies.Exists(x => x.id == movieInfo.id) && (App.Current as App).UserDatabase.movies.Find(x => x.id == movieInfo.id).OnWatchlist)
            {
                OnWatchlist.IsChecked = true;
            }
            else
            {
                OnWatchlist.IsChecked = false;
            }
        }

        private void UserRating_RatingChanged(object sender, Controls.RatingUpdateEvent r)
        {
            
            if ((App.Current as App).UserDatabase.movies.Exists(x => (x.id) == movieInfo.id))
            {
                (App.Current as App).UserDatabase.movies.Find(x => (x.id) == movieInfo.id).review.quality = r.score;
            }
            else
            {
                MovieReference rmr = new MovieReference( movieInfo.id, movieInfo.original_title, new UserReview());
                rmr.review.quality = r.score;
                (App.Current as App).UserDatabase.movies.Add(rmr);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is MediaReference)
            {
                movieInfo = (MediaReference)e.Parameter;
            }
            else
            {
                Frame.GoBack();
            }
            base.OnNavigatedTo(e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            List<MovieReference> movies = (App.Current as App).UserDatabase.movies;
            if (movies.Exists(x => x.id == movieInfo.id))
            {
                MovieReference movie = movies.Find(x => x.id == movieInfo.id);
                if ((bool)OnWatchlist.IsChecked)
                {
                    movie.OnWatchlist = true;
                }
                else
                {
                    if(movie.isDefault())
                    {
                        movies.Remove(movie);
                    }
                    else
                    {
                        movie.OnWatchlist = false;
                    }
                }
            }
            else
            {
                if((bool)OnWatchlist.IsChecked)
                {
                    (App.Current as App).UserDatabase.movies.Add(new MovieReference(movieInfo.id, movieInfo.original_title, new UserReview()) { OnWatchlist = true });
                }
            }
        }

        private void OnWatchlist_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
