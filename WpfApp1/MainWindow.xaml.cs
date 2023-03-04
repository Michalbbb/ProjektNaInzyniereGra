using System;
using System.Collections.Generic;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
       
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private bool UpKey,DownKey,LeftKey,RightKey,rightD;
        double lastT=300, lastL=300;
        private const float startFriction = 0.58f;
        private const float maxFriction = 0.74f;
        private float SpeedX, SpeedY, Friction=0.55f, Speed=2;
        ImageBrush playerSprite = new ImageBrush();
        BitmapImage[] rightRun = new BitmapImage[8];
        BitmapImage[] leftRun= new BitmapImage[8];
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
            for(int i = 0; i < 8; i++)
            {
                leftRun[i] = new BitmapImage();
                leftRun[i].BeginInit();
                leftRun[i].UriSource= new Uri($"pack://application:,,,/WpfApp1;component/images/newRunner_0{i+1}l.png", UriKind.Absolute);
                leftRun[i].EndInit();
                rightRun[i]=new BitmapImage();
                rightRun[i].BeginInit();
                rightRun[i].UriSource = new Uri($"pack://application:,,,/WpfApp1;component/images/newRunner_0{1+i}.gif", UriKind.Absolute);
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

            foreach (var x in GameScreen.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "collision") // Zaawansowana kolizja
                {

                   
                    Rect playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width,Player.Height);
                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);


                    Write.Text = "Obecna Pozycja gracza : " + Convert.ToInt32(Canvas.GetLeft(Player)).ToString() + ":" + Convert.ToInt32(Canvas.GetTop(Player)).ToString();

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
            checkCollision(sender,e);
            if(UpKey||DownKey||RightKey||LeftKey)
            {
                if (ticksDone % 5 == 0)
                {
                    if (rightD)
                    {
                        currentAnimation++;
                        if (currentAnimation == 8) currentAnimation = 0;
                        playerSprite.ImageSource = rightRun[currentAnimation];
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == 8) currentAnimation = 0;
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
            if (UpKey&&Canvas.GetTop(Player)>50)
            {
                if (((Canvas.GetLeft(Player) > 980) && (Canvas.GetTop(Player) < 130)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 940))
                {
                    

                }
                else SpeedY -= Speed;
            }
            if (DownKey && Canvas.GetTop(Player) < 430) 
            {
               
              SpeedY += Speed;
            }
            if (RightKey && Canvas.GetLeft(Player) < 1060)
            {
                if (((Canvas.GetLeft(Player) > 980) && (Canvas.GetTop(Player) < 130))&& (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 940))
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
            if (LeftKey && Canvas.GetLeft(Player) > 10)
            {
                if (rightD) {
                    Friction = startFriction;
                    rightD = false;
                    currentAnimation = 0;
                    playerSprite.ImageSource = leftRun[0];
                }
                SpeedX -= Speed;
            }
            ticksDone++;
            if (Friction < maxFriction) Friction+=0.01f;
            SpeedX = SpeedX * Friction;
            SpeedY = SpeedY * Friction;
            Canvas.SetLeft(Player,Canvas.GetLeft(Player) + SpeedX);
            Canvas.SetTop(Player,Canvas.GetTop(Player) + SpeedY);
        }
        private void startGame()
        {

        }
     private void run()
        {

        }
    }
}
