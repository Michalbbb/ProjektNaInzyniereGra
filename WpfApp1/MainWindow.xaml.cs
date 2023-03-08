using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using static System.Net.Mime.MediaTypeNames; // Nie mam pojecia co to robi i jak sie tu znalazlo

namespace BasicsOfGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
       
        //private DispatcherTimer gameTimer = new DispatcherTimer();
        private bool UpKey,DownKey,LeftKey,RightKey,rightD,returnUp,returnDown,returnLeft,returnRight;
        private const float startFriction = 0.58f;
        private const float maxFriction = 0.74f;
        private double SpeedX, SpeedY, Friction=0.55f, Speed=2,baseSpeed=2;
        ImageBrush playerSprite = new ImageBrush();
        private const int animations=2;
        BitmapImage[] rightRun = new BitmapImage[animations];
        BitmapImage[] leftRun= new BitmapImage[animations];
        private double ticksDone = 0;
        private int currentAnimation = 0;
        
       


        private void KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W && e.Key != Key.S)
            {
                UpKey = true;
            }
          
            if (e.Key == Key.S && e.Key != Key.W)
            {
                DownKey = true;
            }
            
            if (e.Key == Key.D && e.Key != Key.A)
            {
                RightKey = true;
            }
            
            if (e.Key == Key.A && e.Key != Key.D)
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
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            //gameTimer.Interval = TimeSpan.FromMilliseconds(16); // 60 fps
            //gameTimer.Tick += gameTick;
            
            //gameTimer.Start();
        }
        private DateTime _lastRenderTime = DateTime.MinValue;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;
            Speed = baseSpeed * 30 * deltaTime;
            ticksDone = ticksDone+1 * 30 * deltaTime;
            gameTick(sender,e);

        }
        private bool determinateCollision(Rect player,Rect obj)
        {
            if (obj.X < (player.X + player.Width) && (obj.X + obj.Width) > player.X)
            {

               if(obj.Y < (player.Y + player.Height) && (obj.Y + obj.Height) > player.Y)return true;
               else return false;
            }
             else return false;
        }
        private void checkCollision(object sender, EventArgs e)
        {
            if (UpKey)
            {
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 1000))
                {


                }
                else SpeedY -= Speed;
            }

            if (DownKey)
            {
                if(Canvas.GetTop(Player) + Canvas.GetLeft(Player) > 1520) { }
                else SpeedY += Speed;
            }

            if (RightKey)
            {
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 1000))
                {


                }
                else if (Canvas.GetTop(Player) + Canvas.GetLeft(Player) > 1520) { }
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
                if (rightD)
                {
                    Friction = startFriction;
                    rightD = false;
                    currentAnimation = 0;
                    playerSprite.ImageSource = leftRun[0];
                }

            }
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == "collision") // Zaawansowana kolizja
                {

                    Rect playerHitBoxU = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) - Speed*3, Player.Width, Player.Height);
                    Rect playerHitBoxD = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) + Speed*3, Player.Width, Player.Height);
                    Rect playerHitBoxR = new Rect(Canvas.GetLeft(Player) + Speed*3, Canvas.GetTop(Player), Player.Width, Player.Height);
                    Rect playerHitBoxL = new Rect(Canvas.GetLeft(Player) - Speed*3, Canvas.GetTop(Player), Player.Width, Player.Height);
                    Rect playerHitBoxUltimate = new Rect(Canvas.GetLeft(Player) + SpeedX * 3, Canvas.GetTop(Player) + SpeedY * 3, Player.Width, Player.Height);

                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (!determinateCollision(playerHitBoxUltimate, collisionChecker)) continue;
                
                    if (UpKey) if (determinateCollision(playerHitBoxU, collisionChecker)) { SpeedY = 0;  }
                    if (DownKey) if (determinateCollision(playerHitBoxD, collisionChecker)) { SpeedY = 0; }
                    if (LeftKey) if (determinateCollision(playerHitBoxL, collisionChecker)) { SpeedX = 0;  }
                    if (RightKey) if (determinateCollision(playerHitBoxR, collisionChecker)) { SpeedX = 0; }

                } 


            }
           

        }
        private void gameTick(object sender, EventArgs e)
        {
            
            
            Canvas.SetZIndex(Player, Convert.ToInt32((Canvas.GetTop(Player)+Player.Height)/ 100));
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
                if (ticksDone >= 10/Speed )
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
                    ticksDone -= 10 / Speed;
                    if (ticksDone < 0) ticksDone = 0;
                    else if (ticksDone >= 10 / Speed) ticksDone = 0;
                }
            }
            else
            {
                currentAnimation = 0;
                if (rightD) playerSprite.ImageSource = rightRun[0];
                else playerSprite.ImageSource = leftRun[0];
            }
           
            
            if(Canvas.GetTop(Player) < 10)
            {
                Canvas.SetTop(Player, 10);
                returnUp = true;
            }
            if(Canvas.GetLeft(Player) < -11)
            {
                Canvas.SetLeft(Player, -11);
                 returnLeft = true;
            }
            if(Canvas.GetLeft(Player) > 1100)
            {
                Canvas.SetLeft(Player, 1100);
                returnRight = true;
            }
            if(Canvas.GetTop(Player) > 488)
            {
                Canvas.SetTop(Player, 488);
                returnDown = true;
            }
  
           
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
        private void RightClick(object sender, MouseButtonEventArgs e) // CHWILOWE PRZYPISANE DO OBYDWU KLIKNIEC ( PRAWO, LEWO )
        {
            System.Windows.Point mousePosition = e.GetPosition(sender as IInputElement);

            Write.Text = mousePosition.X + " " + mousePosition.Y;
            e.Handled = true;
        }
    }
}
