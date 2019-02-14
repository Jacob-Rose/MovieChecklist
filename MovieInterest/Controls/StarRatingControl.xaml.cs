using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    
    public sealed partial class StarRatingControl : UserControl
    {
        List<SymbolIcon> stars = new List<SymbolIcon>();

        public event RatingUpdateHandler RatingChanged;
        public delegate void RatingUpdateHandler(object sender, RatingUpdateEvent r);

        public int Score {
            get { return score; }
            set
            {
                score = value;
                RatingChanged(this, new RatingUpdateEvent(value));
                updateUI();
            }
        }

        private int score = 0;
        public StarRatingControl()
        {
            this.InitializeComponent();

            for(int i = 0; i < 5; i++)
            {
                SymbolIcon s = new SymbolIcon();
                s.Symbol = Symbol.OutlineStar;
                s.SetValue(Grid.ColumnProperty, i);
                s.PointerExited += SymbolIcon_PointerExited;
                s.PointerPressed += SymbolIcon_PointerPressed;
                s.PointerEntered += SymbolIcon_PointerEntered;
                stars.Insert(i, s);
                starGrid.Children.Add(s);
            }
        }

        private void SymbolIcon_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            int starIndex = (int)(sender as SymbolIcon).GetValue(Grid.ColumnProperty);
            foreach(SymbolIcon s in stars)
            {
                if((int)s.GetValue(Grid.ColumnProperty) > starIndex)
                {
                    s.Symbol = Symbol.OutlineStar;
                    s.Foreground = new SolidColorBrush(Colors.Gray);
                }
                else
                {
                    
                    s.Symbol = Symbol.SolidStar;
                    s.Foreground = new SolidColorBrush(Colors.Black);
                }
                
            }
        }

        private void updateUI()
        {
            for (int i = 0; i < 5; i++)
            {
                Color starColor;
                /*
                switch (score)
                {
                    case 1:
                        starColor = Colors.Red;
                        break;
                    case 2:
                        starColor = Colors.Orange;
                        break;
                    case 3:
                        starColor = Colors.Gold;
                        break;
                    case 4:
                        starColor = Colors.LightSeaGreen;
                        break;
                    case 5:
                        starColor = Colors.LimeGreen;
                        break;
                }
                */
                starColor = Colors.Black;
                SymbolIcon star = stars.ElementAt(i);
                if (i <= score - 1)
                {
                    star.Symbol = Symbol.SolidStar;
                    star.Foreground = new SolidColorBrush(starColor);
                }
                else
                {
                    star.Symbol = Symbol.OutlineStar;
                    star.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        private void SymbolIcon_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            updateUI();
        }

        private void SymbolIcon_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Score = (int)(sender as SymbolIcon).GetValue(Grid.ColumnProperty) + 1;
        }
    }

    public class RatingUpdateEvent : EventArgs
    {
        public int score;

        public RatingUpdateEvent(int s)
        {
            score = s;
        }
    }
}
