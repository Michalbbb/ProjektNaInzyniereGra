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
        Button help= new Button();
        Button start = new Button();
        Button back = new Button();
        Action act = null;
        ImageBrush noweTlo = new ImageBrush();
        DispatcherTimer timer = new DispatcherTimer();
        Canvas canvas;
        public Menu(Canvas canv,Action x) {
            timer.Interval = TimeSpan.FromMilliseconds(100);
            noweTlo.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/MenuBG.png", UriKind.Absolute));
            canvas = canv;
            act = x;
            generatePauseMenu();
          
        }
        private void setButtons()
        {
            
            exit.Width = 300;
            exit.Height = 65;
            exit.Background = Brushes.LightGray;
            exit.Content = "EXIT";
            exit.FontFamily = new FontFamily("Algerian");
            exit.FontSize = 50;

            authors.Width = 300;
            authors.Height = 65;
            authors.Background = Brushes.LightGray;
            authors.Content = "CREDITS";
            authors.FontFamily = new FontFamily("Algerian");
            authors.FontSize = 50;

            start.Width = 300;
            start.Height = 65;
            start.FontSize = 50;
            start.Background = Brushes.LightGray;
            start.Content = "START";
            start.FontFamily = new FontFamily("Algerian");

            help.Width = 300;
            help.Height = 65;
            help.FontSize = 50;
            help.Background = Brushes.LightGray;
            help.Content = "GUIDE";
            help.FontFamily = new FontFamily("Algerian");

            Canvas.SetLeft(start,510); Canvas.SetLeft(authors,510); Canvas.SetLeft(help, 510); Canvas.SetLeft(exit, 510);
            Canvas.SetTop(start,160); Canvas.SetTop(authors,245); Canvas.SetTop(help, 330); Canvas.SetTop(exit, 415);
            start.Click += setGame;
            exit.Click += exitGame;
            authors.Click += authorsPage;
            help.Click += guidePage;

         





            canvas.Children.Add(exit); canvas.Children.Add(start);canvas.Children.Add(authors); canvas.Children.Add(help);



        }
        public void winnerScreen()
        {
            canvas.Children.Clear();
            ImageBrush deathBG = new ImageBrush();
            deathBG.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/winnerScreen.png", UriKind.Absolute));
            canvas.Background = deathBG;
            
            GroupBox a = new GroupBox();
            a.Header = "Wanna play again?";
            a.Foreground = Brushes.WhiteSmoke;
            a.Background = Brushes.DarkBlue;
            Canvas.SetLeft(a, 415);
            Canvas.SetTop(a, 190);
            Canvas.SetZIndex(a, 999);
            a.Width = 440;
            a.FontFamily = new FontFamily("Algerian");
            a.FontSize = 40;
            StackPanel buttonHolder = new StackPanel();
            Button optionOne = new Button();
            optionOne.Content = "Try again";
            optionOne.FontFamily = new FontFamily("Algerian");
            Button optionTwo = new Button();
            optionTwo.Content = "Exit to menu";
            optionTwo.FontFamily = new FontFamily("Algerian");
            Button optionThree = new Button();
            optionThree.Content = "Exit game";
            optionThree.FontFamily = new FontFamily("Algerian");
            System.Windows.Shapes.Rectangle sep = new System.Windows.Shapes.Rectangle();
            sep.Height = 30;
            sep.Fill = Brushes.Transparent;
            System.Windows.Shapes.Rectangle sep2 = new System.Windows.Shapes.Rectangle();
            sep2.Height = 30;
            sep2.Fill = Brushes.Transparent;
            optionOne.FontSize = 40;
            optionThree.FontSize = 40;
            optionTwo.FontSize = 40;
            optionOne.Background = Brushes.LightCyan;
            optionTwo.Background = Brushes.LightCyan;
            optionThree.Background = Brushes.LightCyan;
            buttonHolder.Children.Add(optionOne);
            buttonHolder.Children.Add(sep);
            buttonHolder.Children.Add(optionTwo);
            buttonHolder.Children.Add(sep2);
            buttonHolder.Children.Add(optionThree);
            optionOne.Click += setGame;
            optionTwo.Click += exMenu;
            optionThree.Click += exitGame;
            a.Content = buttonHolder;
            canvas.Children.Add(a);


        }
        public void deathScreen(string name,int dealtDmg)
        {
            canvas.Children.Clear();
            ImageBrush deathBG = new ImageBrush();
            deathBG.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/Death.png", UriKind.Absolute));
            canvas.Background = deathBG;
            TextBox info = new TextBox();
            TextBox importantInfo = new TextBox();
            if (name.Length > 20)
            {
                info.Text = $"You were killed by\n\n\nLethal hit dealt";
                importantInfo.Text = $"{name}\n\n{dealtDmg} damage.";
            }
            else
            {
                info.Text = $"You were killed by\n\nLethal hit dealt";
                importantInfo.Text = $"{name}\n\n{dealtDmg} damage.";
            }
            info.IsEnabled = false;
            info.FontFamily = new FontFamily("Algerian");
            info.FontSize = 40;
            info.BorderBrush = Brushes.Transparent;
            info.Background = Brushes.Transparent; 
            info.Foreground=Brushes.Crimson;
            importantInfo.IsEnabled = false;
            importantInfo.FontFamily = new FontFamily("Algerian");
            importantInfo.FontSize = 40;
            importantInfo.BorderBrush = Brushes.Transparent;
            importantInfo.Background = Brushes.Transparent;
            importantInfo.Foreground = Brushes.Black;
            importantInfo.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(info, 560);
            Canvas.SetTop(info, 260);
            Canvas.SetLeft(importantInfo, 560);
            Canvas.SetTop(importantInfo, 300);
            canvas.Children.Add(info);
            canvas.Children.Add(importantInfo);
            GroupBox a = new GroupBox();
            a.Header = "What you gonna do ?";
            a.Foreground = Brushes.WhiteSmoke;
            a.Background = Brushes.DarkBlue;
            Canvas.SetLeft(a, 50);
            Canvas.SetTop(a, 190);
            Canvas.SetZIndex(a, 999);
            a.Width = 440;
            a.FontFamily = new FontFamily("Algerian");
            a.FontSize = 40;
            StackPanel buttonHolder = new StackPanel();
            Button optionOne = new Button();
            optionOne.Content = "Try again";
            optionOne.FontFamily = new FontFamily("Algerian");
            Button optionTwo = new Button();
            optionTwo.Content = "Exit to menu";
            optionTwo.FontFamily = new FontFamily("Algerian");
            Button optionThree = new Button();
            optionThree.Content = "Exit game";
            optionThree.FontFamily = new FontFamily("Algerian");
            System.Windows.Shapes.Rectangle sep=new System.Windows.Shapes.Rectangle();
            sep.Height = 30;
            sep.Fill = Brushes.Transparent;
            System.Windows.Shapes.Rectangle sep2 = new System.Windows.Shapes.Rectangle();
            sep2.Height = 30;
            sep2.Fill = Brushes.Transparent;
            optionOne.FontSize = 40;
            optionThree.FontSize = 40;
            optionTwo.FontSize = 40;
            optionOne.Background = Brushes.LightCyan;
            optionTwo.Background = Brushes.LightCyan;
            optionThree.Background = Brushes.LightCyan;
            buttonHolder.Children.Add(optionOne);
            buttonHolder.Children.Add(sep);
            buttonHolder.Children.Add(optionTwo);
            buttonHolder.Children.Add(sep2);
            buttonHolder.Children.Add(optionThree);
            optionOne.Click += setGame;
            optionTwo.Click += exMenu;
            optionThree.Click += exitGame;
             a.Content = buttonHolder;
            canvas.Children.Add(a);
           

        }

        private void authorsPage(object sender, RoutedEventArgs e)
        {
            List<UIElement> toRemove = new List<UIElement>();
            foreach (UIElement x in canvas.Children)
            {
                toRemove.Add(x);
            }
            for (int i = toRemove.Count - 1; i >= 0; i--)
            {
                canvas.Children.Remove(toRemove[i]);
            }
            ImageBrush auBG=new ImageBrush();
            auBG.ImageSource=new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/Credits.png", UriKind.Absolute));
            canvas.Background = auBG;
            back.Width = 170;
            back.Height = 65;
            back.FontSize = 50;
            back.Background = Brushes.LightGray;
            back.Content = "BACK";
            back.FontFamily = new FontFamily("Algerian");
            Canvas.SetLeft(back, 20);
            Canvas.SetTop(back, 20);
            back.Click += goToMenu;
            canvas.Children.Add(back);

        }
        private void guidePage(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            ImageBrush auBG = new ImageBrush();
            auBG.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/guideBG.png", UriKind.Absolute));
            canvas.Background = auBG;
            back.Width = 170;
            back.Height = 65;
            back.FontSize = 50;
            back.Background = Brushes.LightGray;
            back.Content = "BACK";
            back.FontFamily = new FontFamily("Algerian");
            Canvas.SetLeft(back, 20);
            Canvas.SetTop(back, 20);
            back.Click += goToMenu;

         
            TextBox boldInfo= new TextBox();
            boldInfo.Text = "\t\t\t\t\t[ENEMIES]";
            boldInfo.Text += "\nCurrently in game there are 4 types of monsters that player can face:\n1.Goblin - Low hp,low dmg and no special attack makes him easy opponent.";
            boldInfo.Text += "\n2.Imp - Moderate hp,attacks in sequence of 3 weak hits and inflict ignite.";
            boldInfo.Text += "\n3.Spider - Very low hp.Weak hits, but inflicts deadly poison. Extremaly fast.";
            boldInfo.Text += "\n4.Golem - High hp. Slow, but powerful hits in sequence of 3 aoe hits.\n each of his attacks inflicts stun. Slow.";
            boldInfo.Text += "\n\t\t\t\t         [STATUS EFFECTS]";
            boldInfo.Text += "\nThere are 3 types of status effect that player can be inflicted with:";
            boldInfo.Text += "\n1.Poison - stackable debuff that deal high dot.";
            boldInfo.Text += "\n2.Ignite - Deals low dot and causes player to deal 20% less damage.\nEvery new instance of ignite prolongs existing one by its duration.";
            boldInfo.Text += "\n3.Stun - debuff, that prevent player from taking any action.\nNew stuns while stunned do not increase or refresh current stun duration.";
            boldInfo.Text += "\n\n*dot stands for \"damage over time\"\n*aoe stands for \"area of effect\"";
            boldInfo.Width = 890; 
            boldInfo.Height = 450;
            
            boldInfo.IsEnabled = false;
            boldInfo.FontFamily = new FontFamily("Algerian");
            boldInfo.FontSize = 20;
            boldInfo.BorderBrush = Brushes.Transparent;
            boldInfo.Background = Brushes.Yellow;
            boldInfo.Foreground = Brushes.Black;
            boldInfo.FontWeight= FontWeights.Bold;
            Canvas.SetLeft(boldInfo, 200);
            Canvas.SetTop(boldInfo, 150);
            Canvas.SetZIndex(boldInfo, 40000);
            canvas.Children.Add(back);
            
     
            canvas.Children.Add(boldInfo);


        }

        private void goToMenu(object sender, RoutedEventArgs e)
        {
            ShowMenu();
        }

        GroupBox globalBox;
        private void exitGame(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        
        private void generatePauseMenu()
        {
            GroupBox a = new GroupBox();
            a.Header = "Game paused";
            a.Foreground = Brushes.WhiteSmoke;
            a.Background = Brushes.DarkBlue;
            Canvas.SetLeft(a, 440);
            Canvas.SetTop(a, 240);
            Canvas.SetZIndex(a, 999);
            a.Width = 320;
            a.FontFamily = new FontFamily("Algerian");
            a.FontSize = 40;
            StackPanel buttonHolder = new StackPanel();
            Button optionOne = new Button();
            optionOne.Content = "Continue";
            optionOne.FontFamily = new FontFamily("Algerian");
            Button optionTwo = new Button();
            optionTwo.Content = "Exit to menu";
            optionTwo.FontFamily = new FontFamily("Algerian");
            Button optionThree = new Button();
            optionThree.Content = "Exit game";
            optionThree.FontFamily = new FontFamily("Algerian");
            optionOne.FontSize = 30;
            optionThree.FontSize = 30;
            optionTwo.FontSize = 30;
            optionOne.Background = Brushes.LightCyan;
            optionTwo.Background = Brushes.LightCyan;
            optionThree.Background = Brushes.LightCyan;
            buttonHolder.Children.Add(optionOne);
            buttonHolder.Children.Add(optionTwo);
            buttonHolder.Children.Add(optionThree);
            optionOne.Click += cont;
            optionTwo.Click += exMenu;
            optionThree.Click += exitGame;
            a.Content = buttonHolder;
            globalBox = a;
        }

        private void cont(object sender, RoutedEventArgs e)
        {
            MainWindow.isGameRunning = true;
            canvas.Focus();
            unpause();

        }

        private void exMenu(object sender, RoutedEventArgs e)
        {
            ShowMenu();
        }

       

        public void pauseGameMenu()
        {

            globalBox.IsEnabled = true;
            canvas.Children.Add(globalBox);
        }
        public void unpause()
        {
            
            globalBox.IsEnabled = false;
            canvas.Children.Remove(globalBox);
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
