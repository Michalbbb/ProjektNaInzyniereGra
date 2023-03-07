﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace BasicsOfGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
       
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private bool UpKey,DownKey,LeftKey,RightKey,rightD,returnUp,returnDown,returnLeft,returnRight;
        double lastT=300, lastL=300;
        private const float startFriction = 0.58f;
        private const float maxFriction = 0.74f;
        private float SpeedX, SpeedY, Friction=0.55f, Speed=2;
        ImageBrush playerSprite = new ImageBrush();
        private const int animations=2;
        BitmapImage[] rightRun = new BitmapImage[animations];
        BitmapImage[] leftRun= new BitmapImage[animations];
        private int ticksDone = 0;
        private int currentAnimation = 0;
       


        private void KeyboardDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.W)
            {
                UpKey = true;
            }
            if (e.Key == Key.S)
            {
                DownKey = true;
            }
            if (e.Key == Key.D)
            {
                RightKey = true;
            }
            if (e.Key == Key.A)
            {
                LeftKey = true;
            }
        }

        private void KeyboardUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                UpKey = false;
            }
            if (e.Key == Key.S)
            {
                DownKey = false;
            }
            if (e.Key == Key.D)
            {
                RightKey = false;
            }
            if (e.Key == Key.A)
            {
                LeftKey = false;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            GameScreen.Focus();
            for(int i = 0; i < animations; i++)
            {
                leftRun[i] = new BitmapImage();
                leftRun[i].BeginInit();
                leftRun[i].UriSource= new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1+i}l.png", UriKind.Absolute);
                leftRun[i].EndInit();
                rightRun[i]=new BitmapImage();
                rightRun[i].BeginInit();
                rightRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1+i}.png", UriKind.Absolute);
                rightRun[i].EndInit();

            }
            
            playerSprite.ImageSource = rightRun[0];
            rightD = true;
            Player.Fill=playerSprite;
            gameTimer.Interval = TimeSpan.FromMilliseconds(16); // 60 fps
            gameTimer.Tick += gameTick;
            
            gameTimer.Start();
        }

        private void checkCollision(object sender, EventArgs e)
        {

            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == "collision") // Zaawansowana kolizja
                {

                   
                    Rect playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width,Player.Height);
                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                   // Write.Text = "Obecna Pozycja gracza : " + Convert.ToInt32(Canvas.GetLeft(Player)).ToString() + ":" + Convert.ToInt32(Canvas.GetTop(Player)).ToString();

                    if (playerHitBox.IntersectsWith(collisionChecker))
                    {
                        Rect ifPlayerShorter = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height - 15);
                        Rect ifPlayerPositionLower = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) + 15, Player.Width, Player.Height );
                        Rect ifPlayerSlimer = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width - 15, Player.Height);
                        Rect ifPlayerPositionRight = new Rect(Canvas.GetLeft(Player) + 15, Canvas.GetTop(Player), Player.Width , Player.Height);
                        if (!(ifPlayerShorter.IntersectsWith(collisionChecker)) || !(ifPlayerPositionLower.IntersectsWith(collisionChecker))) { lastL = Canvas.GetLeft(Player);}
                        else Canvas.SetLeft(Player, lastL);
                        if (!(ifPlayerSlimer.IntersectsWith(collisionChecker)) || !(ifPlayerPositionRight.IntersectsWith(collisionChecker))) { lastT = Canvas.GetTop(Player); }
                        else Canvas.SetTop(Player, lastT);
                        return;
                    }

                }


            }
            lastL = Canvas.GetLeft(Player);
            lastT = Canvas.GetTop(Player);

        }
        private void gameTick(object sender, EventArgs e)
        {
            double rectLeft = Canvas.GetLeft(Player);
            double rectTop = Canvas.GetTop(Player);
            double rectRight = rectLeft + Player.Width;
            double rectBottom = rectTop + Player.Height;

           
            double viewportWidth = Camera.ViewportWidth;
            double viewportHeight = Camera.ViewportHeight;

            
            double horizontalOffset = (rectLeft + rectRight - viewportWidth) / 2;
            double verticalOffset = (rectTop + rectBottom - viewportHeight) / 2;
            
            
            Camera.ScrollToHorizontalOffset(horizontalOffset);
            Camera.ScrollToVerticalOffset(verticalOffset);
            checkCollision(sender,e);
            if(UpKey||DownKey||RightKey||LeftKey)
            {
                if (ticksDone % Convert.ToInt32(10/Speed) == 0)
                {
                    if (rightD)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        playerSprite.ImageSource = rightRun[currentAnimation];
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        playerSprite.ImageSource = leftRun[currentAnimation];
                        
                    }
                }
            }
            else
            {
                currentAnimation = 0;
                if (rightD) playerSprite.ImageSource = rightRun[0];
                else playerSprite.ImageSource = leftRun[0];
            }
            //Podstowe kolizje
            if (UpKey)
            {
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 915))
                {


                }
                else SpeedY -= Speed;
            }
            
            if (DownKey) 
            {
               
              SpeedY += Speed;
            }
        
            if (RightKey)
            {
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 915))
                {


                }
                else
                {
                    if (!rightD)
                    {
                        Friction = startFriction;
                        rightD = true;
                        currentAnimation = 0;
                        playerSprite.ImageSource = rightRun[0];
                    }
                    SpeedX += Speed;
                }

            }
            if (LeftKey)
            {
               
                SpeedX -= Speed;
                if (rightD) {
                    Friction = startFriction;
                    rightD = false;
                    currentAnimation = 0;
                    playerSprite.ImageSource = leftRun[0];
                }
                
            }
            if(Canvas.GetTop(Player) < 70)
            {
                Canvas.SetTop(Player, 70);
                returnUp = true;
            }
            if(Canvas.GetLeft(Player) < -11)
            {
                Canvas.SetLeft(Player, -11);
                 returnLeft = true;
            }
            if(Canvas.GetLeft(Player) > 1120)
            {
                Canvas.SetLeft(Player, 1120);
                returnRight = true;
            }
            if(Canvas.GetTop(Player) > 488)
            {
                Canvas.SetTop(Player, 488);
                returnDown = true;
            }
  
            ticksDone++;
            if (returnUp || Canvas.GetTop(Player) == 70)
            {
                if (SpeedY < 0) SpeedY = 0;
                returnUp = false;
            }
            if (returnDown || Canvas.GetTop(Player) == 488)
            {
                if(SpeedY >0)
                    SpeedY = 0;
                returnDown=false;
            }
            if (returnRight||Canvas.GetLeft(Player)==1120)
            {
                if (SpeedX > 0)
                    SpeedX = 0;
                returnRight = false;
            }
            if (returnLeft || Canvas.GetLeft(Player) == -11)
            {
                if (SpeedX < 0)
                    SpeedX = 0;
                returnLeft = false;
            }

            if (Friction < maxFriction) Friction+=0.01f;
            SpeedX = SpeedX * Friction;
            SpeedY = SpeedY * Friction;
            Canvas.SetLeft(Player,Canvas.GetLeft(Player) + SpeedX);
            Canvas.SetTop(Player,Canvas.GetTop(Player) + SpeedY);
        }
        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePosition = e.GetPosition(sender as IInputElement);

            Write.Text = mousePosition.X + " " + mousePosition.Y;
            e.Handled = true;
        }
    }
}
