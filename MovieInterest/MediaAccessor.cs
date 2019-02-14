using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieInterest
{
    /// <summary>
    /// Gives simple methods to connect to the movie database
    /// </summary>
    abstract class MediaAccessor
    {
        private const string startURL = "https://api.themoviedb.org/3/"; //starting line to any query
        private const string apiKey = "a4e722f09fd2244b040453e17da4700a"; //my specific api key


        public static dynamic getDetails(int id, MediaType mediaType)
        {
            string url = startURL + Enum.GetName(typeof(MediaType), mediaType) + "/" + id + "?api_key=" + apiKey;

            return JsonConvert.DeserializeObject<dynamic>(getHttpResponse(url));

        }

        public static List<ListMediaReference> getPopular(int page, MediaType mediaType)
        {
            string url = startURL + Enum.GetName(typeof(MediaType), mediaType) +  "/popular" + "?api_key=" + apiKey + "&page=" + page;

            List<ListMediaReference> shows = new List<ListMediaReference>();
            dynamic d = JsonConvert.DeserializeObject<dynamic>(getHttpResponse(url));

            foreach(dynamic mov in d.results)
            {
                shows.Add(new ListMediaReference((int)mov.id, (string)mov.original_title, mediaType, (string)mov.poster_path, (string)mov.backdrop_path));
            }

            return shows;
        }

        public static List<ListMediaReference> multiSearch(string query)
        {
            string url = startURL + "search/multi?language=en-US&api_key=" + apiKey + "&query=" + query;
            dynamic d = JsonConvert.DeserializeObject<dynamic>(getHttpResponse(url));
            List<ListMediaReference> medias = new List<ListMediaReference>();
            
            foreach (dynamic info in d.results)
            {
                //Debugger.Break();
                string media_type = (string)info.media_type;
                switch (media_type)
                {
                    case "person":
                        break;
                    case "movie":
                        ListMediaReference m = new ListMediaReference((int)info.id, (string)info.original_title, MediaType.movie, (string)info.poster_path, (string)info.backdrop_path);
                        if (m.poster_path != null || m.backdrop_path != null)
                        {
                            medias.Add(m);
                        }
                        
                        break;
                    case "tv":
                        ListMediaReference s = new ListMediaReference((int)info.id, (string)info.original_name, MediaType.tv, (string)info.poster_path, (string)info.backdrop_path);
                        if (s.poster_path != null || s.backdrop_path != null)
                        {
                            medias.Add(s);
                        }
                        break;
                }
            }
            return medias;
        }

        public static string getHttpResponse(string url)
        {
            HttpWebRequest req;
            HttpWebResponse res = null;

            req = (HttpWebRequest)WebRequest.Create(url);
            res = (HttpWebResponse)req.GetResponseAsync().Result;
            Stream stream = res.GetResponseStream();

            return new StreamReader(stream).ReadToEnd();
        }

        public static string createDiscoverURL(MediaType mediaType = MediaType.movie,
                                        SearchCategory category = SearchCategory.discover,
                                        int page = 1,
                                        string mediaID = "", //is a string as to be able to add to script without long effect
                                        SortingMethod sortMethod = SortingMethod.popularity,
                                        SortingOrder sortOrder = SortingOrder.desc,
                                        string query = "",
                                        bool includeAdult = false,
                                        Genre[] withGenres = null)
        {
            if (withGenres == null)
            {
                withGenres = new Genre[0];
            }
            string genres = string.Join(",", withGenres);
            //returns the api url + category + mediatype
            return startURL + "/" + Enum.GetName(typeof(SearchCategory), category) + "/" + Enum.GetName(typeof(MediaType), mediaType) + mediaID +"?api_key=" + apiKey + "&page=" + page + "&query=" + query + "&sort_by=" + Enum.GetName(typeof(SortingMethod), sortMethod) + "." + Enum.GetName(typeof(SortingOrder), sortOrder) + "&include_adult=" + includeAdult.ToString().ToLower() + "&with_genres=" + genres;
        }
    }

    public enum MediaType
    {
        movie,
        tv,
        person,
    }

    /// <summary>
    /// methods used for result organization as defined by the movie database api
    /// </summary>
    public enum SortingMethod
    {
        popularity,
        release_date,
        revenue,
        primary_release_date,
        original_title,
        vote_average,
        vote_count,
    }
    public enum SortingOrder
    {
        asc,
        desc,
    }
    public enum SearchCategory
    {
        search,
        find,
        discover,
    }
    /// <summary>
    /// genres defined by the movie database api
    /// </summary>
    public enum Genre
    {
        ACTION = 28,
        ADVENTURE = 12,
        ANIMATION = 16,
        COMEDY = 35,
        CRIME = 80,
        DOCUMENTARY = 99,
        DRAMA = 18,
        FAMILY = 10751,
        FANTASY = 14,
        HISTORY = 36,
        HORROR = 27,
        MUSIC = 10402,
        MYSTERY = 9648,
        ROMANCE = 10749,
        SCIFI = 878,
        TVMOVIE = 10770,
        THRILLER = 53,
        WAR = 10572,
        WESTERN = 37,

    }


    /// <summary>
    /// Stores enough data for the movie to be found later
    /// </summary>
    public abstract class MediaReference
    {
        public MediaType mediaType; 
        public int id;
        public string original_title { get; set; }

        public MediaReference(int id, string original_title, MediaType mediaType)
        {
            this.id = id;
            this.original_title = original_title;
            this.mediaType = mediaType;
        }
    }

    /// <summary>
    /// Stores enough data for the media to have its movie and picture shown in a list
    /// </summary>
    public class ListMediaReference : MediaReference
    {
        public string poster_path { get; set; }
        public string backdrop_path { get; set; }

        public ListMediaReference(int id, string original_title, MediaType mediaType, string poster_path, string backdrop_path) : base(id, original_title, mediaType)
        {
            init(id, original_title, mediaType, poster_path, backdrop_path);
        }

        public ListMediaReference(MediaReference media) : base(media.id, media.original_title, media.mediaType)
        {
            dynamic d = MediaAccessor.getDetails(media.id, media.mediaType);

            init(media.id, media.original_title, media.mediaType, (string)d.poster_path, (string)d.backdrop_path);
        }

        public void init(int id, string original_title, MediaType mediaType, string poster_path, string backdrop_path)
        {
            this.poster_path = poster_path;
            this.backdrop_path = backdrop_path;
        }
    }

    /// <summary>
    /// Stores all the different types of reviews the user can give a movie
    /// </summary>
    public class UserReview
    {
        //if any 0 then not reviewed
        public int quality = 0;
        public int enjoyment = 0; //useful for movies so bad their good
        public int acting = 0;
        public int director = 0;

        public bool isDefault()
        {
            if(quality == 0 && enjoyment == 0 && acting == 0 && director == 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Top level object that is saved
    /// </summary>
    public class UserDatabase
    {
        public List<MovieReference> movies = new List<MovieReference>();
        public List<TVReference> shows = new List<TVReference>();
    }

    /// <summary>
    /// All the users data that needs to be saved about a movie
    /// </summary>
    public class MovieReference : MediaReference
    {
        public UserReview review;

        public bool OnWatchlist { get; set; }

        public MovieReference(int id, string original_title, UserReview review) : base(id, original_title, MediaType.movie)
        {
            this.review = review;
        }

        public bool isDefault()
        {
            if (review.isDefault() && OnWatchlist == false)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// All the users data that needs to be saved about a TV show
    /// </summary>
    public class TVReference : MediaReference
    {
        public Dictionary<int, UserReview> reviews { get; set; } = new Dictionary<int, UserReview>(); //tuple 0,0 is review for show || tuple 1, 0 is season review
        public Dictionary<Tuple<int, int>, bool> watched { get; set; } = new Dictionary<Tuple<int, int>, bool>();

        public bool OnWatchlist { get; set; }

        public TVReference(int id, string original_title, Dictionary<int, UserReview> reviews, Dictionary<Tuple<int, int>, bool> watched) : base(id, original_title, MediaType.tv)
        {
            this.reviews = reviews;
            this.watched = watched;
        }
    }

}
