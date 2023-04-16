using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Windows.Threading;

namespace BasicsOfGame
{
    internal class ClusterUI
    {
    }
    internal class Menu
    {
        Button exit = new Button();
        Button authors = new Button();
        Button start = new Button();
        Action act = null;
        ImageBrush noweTlo = new ImageBrush();
        DispatcherTimer timer = new DispatcherTimer();
        Canvas canvas;
        public Menu(Canvas canv,Action x) {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            noweTlo.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/MenuBG.png", UriKind.Absolute));
            canvas = canv;
            act = x;
          
        }
        private void setButtons()
        {
            ImageBrush temp = new ImageBrush();
            exit.Width = 300;
            exit.Height = 65;
            temp.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/Exit.png", UriKind.Absolute));
            exit.Background = Brushes.LightGray;
            exit.Content = "EXIT";
            exit.FontFamily = new FontFamily("Algerian");
            exit.FontSize = 50;
            authors.Width = 300;
            authors.Height = 65;
            temp = new ImageBrush();
            temp.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/Authors.png", UriKind.Absolute));
            authors.Background = Brushes.LightGray;
            authors.Content = "AUTHORS";
            authors.FontFamily = new FontFamily("Algerian");
            authors.FontSize = 50;
            start.Width = 300;
            start.Height = 65;
            start.FontSize = 50;
            temp = new ImageBrush();
            temp.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/Start.png", UriKind.Absolute));
            start.Background = Brushes.LightGray;
            start.Content = "START";
            start.FontFamily = new FontFamily("Algerian");
            Canvas.SetLeft(start,510); Canvas.SetLeft(authors,510); Canvas.SetLeft(exit,510);
            Canvas.SetTop(start,160); Canvas.SetTop(authors,275); Canvas.SetTop(exit,390);
            start.Click += setGame;
            exit.Click += exitGame;
            authors.Click += exitGame;
            




            
            canvas.Children.Add(exit); canvas.Children.Add(start);canvas.Children.Add(authors);
           
           

        }
        
        
        private void exitGame(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void ShowMenu()
        {
            List<UIElement> toRemove = new List<UIElement>();
            foreach(UIElement x in  canvas.Children)
            {
                toRemove.Add(x);
            }
            for(int i = toRemove.Count - 1; i >= 0; i--)
            {
                canvas.Children.Remove(toRemove[i]);
            }
            canvas.Background = noweTlo;
            setButtons();
        }
        private void setGame(object sender,EventArgs e)
        {
            
            timer.Start();
            List<UIElement> toRemove = new List<UIElement>();
            foreach (UIElement x in canvas.Children)
            {
                toRemove.Add(x);
            }
            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                canvas.Children.Remove(toRemove[i]);
            }
            ImageBrush loadingBg = new ImageBrush();
            loadingBg.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/loadingBG.png", UriKind.Absolute));
            canvas.Background = loadingBg;
            timer.Tick += getReady;
        }
        private void getReady(object sender,EventArgs e)
        {
            timer.Stop();
            act();
        }
        
    }
}
