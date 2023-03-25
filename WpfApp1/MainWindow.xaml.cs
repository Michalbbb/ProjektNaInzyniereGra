using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
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
        
        
        private int ticksRemaining=0;
        private int enemies = 0;
        private bool UpKey, DownKey, LeftKey, RightKey, rightD, returnUp, returnDown, returnLeft, returnRight, blockAttack;
        private TextBox[] boxes;
        TextBox playerDmg;
        private const float Friction = 0.65f;
        private double SpeedX, SpeedY, Speed = 100, baseSpeed = 100;
        private int minDmg = 10;
        private int maxDmg = 15;   
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush weaponSprite = new ImageBrush();
        private const int animations = 6;
        BitmapImage[] rightRun = new BitmapImage[animations];
        BitmapImage[] leftRun = new BitmapImage[animations];
        BitmapImage[] attackAnimationsU = new BitmapImage[4];
        BitmapImage[] attackAnimationsD = new BitmapImage[4];
        BitmapImage[] attackAnimationsL = new BitmapImage[4];
        BitmapImage[] attackAnimationsR = new BitmapImage[4];
        Goblin [] goblins=new Goblin[2];
        private int intervalForAttackAnimations = 30;
        private double ticksDone = 0;
        private int currentMovementAnimation = 0;
        private int attackRange = 100, attackDirection;
        private double unlockAttack = 0;
        Random getRand = new Random();
        bool leftDoorExist,rightDoorExist,upDoorExist,downDoorExist;
       




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
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            Layout mapa = new Layout();
            mapa.makeBackground(GameScreen,true, true, true, true, 1,ref leftDoorExist,ref rightDoorExist,ref upDoorExist,ref downDoorExist);
            goblins[0] = new Goblin(GameScreen, 200, 200);
            goblins[1] = new Goblin(GameScreen, 300, 700);
            GameScreen.Focus();
            generateTB("enemy");
            

            for (int i = 0; i < animations; i++)                                                                                                                                                                                                                         
            {
                leftRun[i] = new BitmapImage();                                                                                                                                                                                                                         
                leftRun[i].BeginInit();                                                                                                                                                                                                                                
                leftRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}l.png", UriKind.Absolute);                                                                                                                     
                leftRun[i].EndInit();                                                                                                                                                                                                                                 
                rightRun[i] = new BitmapImage();
                rightRun[i].BeginInit();
                rightRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}.png", UriKind.Absolute);
                rightRun[i].EndInit();

            }
            initializeAnimationsForAttack();
            
            playerSprite.ImageSource = rightRun[0];
            rightD = true;
            Player.Fill = playerSprite;
            CompositionTarget.Rendering += CompositionTarget_Rendering; 

        }
        private DateTime _lastRenderTime = DateTime.MinValue;
        double deltaTime;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;
            if (Speed != 0) Speed = baseSpeed * deltaTime;
            ticksDone += baseSpeed/2 * deltaTime;
            
            if (blockAttack)
            {
                  
                unlockAttack += Convert.ToDouble(1000 * deltaTime);

                if (unlockAttack / (intervalForAttackAnimations * (6 - ticksRemaining)) >= 1&&ticksRemaining>0)
                {
                    
                    attackOmni(sender, e);
                }
                if (unlockAttack > 400) { blockAttack = false; unlockAttack = 0;ticksRemaining = 0; }

            }
            
            gameTick(sender, e);
            checkOpacity("enemy");
            
            for (int i = 0; i < 2; i++)
                goblins[i].moveToTarget(Player, deltaTime, Friction,playerDmg);

        }
        private void checkOpacity(string tag)
        {
            int i = 0;
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == tag)
                {
                        
                        if (boxes[i].Opacity > 0) boxes[i].Opacity -= Speed / 2;
                        else boxes[i].Text = "0"; 
                        Canvas.SetLeft(boxes[i], Canvas.GetLeft(x) + (x.ActualWidth / 2) - (boxes[i].Width / 2));
                        Canvas.SetTop(boxes[i], (Canvas.GetTop(x) - (x.Height - x.ActualHeight)) - boxes[i].Height);
                        i++;


                }


            }
            if (playerDmg.Opacity > 0) playerDmg.Opacity -= Speed / 2;
            else playerDmg.Text = "0";
            Canvas.SetLeft(playerDmg, Canvas.GetLeft(Player) + (Player.ActualWidth / 2) - (playerDmg.Width / 2));
            Canvas.SetTop(playerDmg, (Canvas.GetTop(Player) - (Player.Height - Player.ActualHeight)) - playerDmg.Height);
            i++;

        }
        private void generateTB(string tag) // Text Boxes
        {
            int i = 0;
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == tag)
                {
                    i++;

                }
                    
            }
            boxes = new TextBox[i];
            enemies = i;
            for(int j=0;j<i; j++)
            {
                boxes[j] = new TextBox();
                boxes[j].Width = 30;  
                boxes[j].Height = 25;
                boxes[j].FontSize = 20;
            
                boxes[j].Text = "0";
                boxes[j].Opacity = 0;
                boxes[j].Foreground = Brushes.BlanchedAlmond;
                boxes[j].Background = Brushes.Transparent;
                boxes[j].BorderBrush= Brushes.Transparent;
               
                boxes[j].IsEnabled = false;
                GameScreen.Children.Add(boxes[j]);
                Canvas.SetZIndex(boxes[j], 999);

            }
            
            playerDmg = new TextBox();
            playerDmg.Width = 25;
            playerDmg.Height = 35;
            playerDmg.FontSize = 30;
            playerDmg.Text = "0";
            playerDmg.Opacity = 0;
            playerDmg.Foreground = Brushes.Red;
            
            playerDmg.Background = Brushes.Black;
            playerDmg.BorderBrush = Brushes.Transparent;

            playerDmg.IsEnabled = false;
            GameScreen.Children.Add(playerDmg);
            Canvas.SetZIndex(playerDmg, 999);

        }
        private bool determinateCollision(Rect player, Rect obj)
        {
            if (obj.X < (player.X + player.Width) && (obj.X + obj.Width) > player.X)
            {

                if (obj.Y < (player.Y + player.Height) && (obj.Y + obj.Height) > player.Y) return true;
                else return false;
            }
            else return false;
        }
        private void checkCollision(object sender, EventArgs e)
        {
            if (Speed == 0) return;

            if (UpKey)
            {
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 1000))
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
                if (((Canvas.GetLeft(Player) > 960) && (Canvas.GetTop(Player) < 170)) && (Canvas.GetLeft(Player) - Canvas.GetTop(Player) > 1000))
                {


                }
                else
                {
                    if (!rightD)
                    {

                        rightD = true;
                        currentMovementAnimation = 0;
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

                    rightD = false;
                    currentMovementAnimation = 0;
                    playerSprite.ImageSource = leftRun[0];
                }

            }
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == "collision") // Zaawansowana kolizja
                {

                    Rect playerHitBoxU = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) - Speed * 3, Player.Width, Player.Height);
                    Rect playerHitBoxD = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player) + Speed * 3, Player.Width, Player.Height);
                    Rect playerHitBoxR = new Rect(Canvas.GetLeft(Player) + Speed * 3, Canvas.GetTop(Player), Player.Width, Player.Height);
                    Rect playerHitBoxL = new Rect(Canvas.GetLeft(Player) - Speed * 3, Canvas.GetTop(Player), Player.Width, Player.Height);
                    Rect playerHitBoxUltimate = new Rect(Canvas.GetLeft(Player) + SpeedX * 3, Canvas.GetTop(Player) + SpeedY * 3, Player.Width, Player.Height);

                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (!determinateCollision(playerHitBoxUltimate, collisionChecker)) continue;

                    if (UpKey) if (determinateCollision(playerHitBoxU, collisionChecker)) { SpeedY = 0; }
                    if (DownKey) if (determinateCollision(playerHitBoxD, collisionChecker)) { SpeedY = 0; }
                    if (LeftKey) if (determinateCollision(playerHitBoxL, collisionChecker)) { SpeedX = 0; }
                    if (RightKey) if (determinateCollision(playerHitBoxR, collisionChecker)) { SpeedX = 0; }

                }


            }


        }
        private void checkCollision(string tag)
        {
            int i = 0;
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == tag) 
                {

                   
                    Rect hitbox=new Rect(Canvas.GetLeft(Weapon),Canvas.GetTop(Weapon),Weapon.ActualWidth, Weapon.ActualHeight);    
                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.ActualWidth, x.ActualHeight);
                    if (determinateCollision(hitbox, collisionChecker))
                    {
                        int obecnyDmg = Convert.ToInt32(boxes[i].Text);
                        obecnyDmg += getRand.Next(minDmg,maxDmg+1);
                        boxes[i].Text = obecnyDmg.ToString();              
                        boxes[i].Width = Convert.ToInt16(boxes[i].Text.Length)*15;
                        boxes[i].Opacity = 100;
                        Canvas.SetLeft(boxes[i], Canvas.GetLeft(x) + (x.ActualWidth / 2) - (boxes[i].Width/2));
                        Canvas.SetTop(boxes[i], (Canvas.GetTop(x)-(x.Height-x.ActualHeight))-boxes[i].Height);

                    }
                    

                    i++;


                }


            }
        }
        private void gameTick(object sender, EventArgs e)
        {


            Canvas.SetZIndex(Player, Convert.ToInt32((Canvas.GetTop(Player) + Player.Height) / 100));
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
            checkCollision(sender, e);
            if (UpKey || DownKey || RightKey || LeftKey)
            {
                if (ticksDone >= 10 / Speed)
                {
                    if (rightD)
                    {
                        currentMovementAnimation++;
                        if (currentMovementAnimation == animations) currentMovementAnimation = 0;
                        playerSprite.ImageSource = rightRun[currentMovementAnimation];
                    }
                    else
                    {
                        currentMovementAnimation++;
                        if (currentMovementAnimation == animations) currentMovementAnimation = 0;
                        playerSprite.ImageSource = leftRun[currentMovementAnimation];

                    }
                    ticksDone -= 10 / Speed;
                    if (ticksDone < 0) ticksDone = 0;
                    if (ticksDone >= 10/ Speed) ticksDone = 0;
                }

            }
            else
            {
                currentMovementAnimation = 0;
                if (rightD) playerSprite.ImageSource = rightRun[0];
                else playerSprite.ImageSource = leftRun[0];
            }


            if (Canvas.GetTop(Player) < 10)
            {
                Canvas.SetTop(Player, 10);
                returnUp = true;
            }
            if (Canvas.GetLeft(Player) < -11)
            {
                Canvas.SetLeft(Player, -11);
                returnLeft = true;
            }
            if (Canvas.GetLeft(Player) > 1100)
            {
                Canvas.SetLeft(Player, 1100);
                returnRight = true;
            }
            if (Canvas.GetTop(Player) > 488)
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
                if (SpeedY > 0)
                    SpeedY = 0;
                returnDown = false;
            }
            if (returnRight || Canvas.GetLeft(Player) == 1120)
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


            SpeedX = SpeedX * Friction;
            SpeedY = SpeedY * Friction;
            if (Speed != 0) Canvas.SetLeft(Player, Canvas.GetLeft(Player) + SpeedX);
            if (Speed != 0) Canvas.SetTop(Player, Canvas.GetTop(Player) + SpeedY);


        }
        double abs(double x)
        {
            if (x < 0) return -x;
            else return x;

        }
        private void attackOmni(object sender, EventArgs e)
        {
            if (ticksRemaining == 1)
            {
                checkCollision("enemy");

                ticksRemaining--;
                Weapon.Fill = new SolidColorBrush(Colors.Transparent);
                Speed = baseSpeed*deltaTime;
                return;

            }
            if (attackDirection == 1)
            {
                weaponSprite.ImageSource = attackAnimationsR[5-ticksRemaining];
                Weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 2)
            {
                weaponSprite.ImageSource = attackAnimationsL[5 - ticksRemaining];
                Weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 3)
            {
                weaponSprite.ImageSource = attackAnimationsD[5 - ticksRemaining];
                Weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 4)
            {
                weaponSprite.ImageSource = attackAnimationsU[5 - ticksRemaining];
                Weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
        }


        private void animateAttack(System.Windows.Point mousePosition)
        {

            GameScreen.Focus();
            double CenterXPlayer = Canvas.GetLeft(Player) + Player.Width / 2;
            double CenterYPlayer = Canvas.GetTop(Player) + Player.Height / 2;
            double DeltaX = CenterXPlayer - mousePosition.X;
            double DeltaY = CenterYPlayer - mousePosition.Y;
            ticksRemaining=5;
         
            if (abs(DeltaX) - abs(DeltaY) >= 0)
            {
                if (DeltaX < 0) // right
                    {
                        Canvas.SetLeft(Weapon, CenterXPlayer);
                        Canvas.SetTop(Weapon, CenterYPlayer - (Player.Height * 1.6) / 2);
                        Weapon.Width = attackRange;
                        Weapon.Height = Player.Height * 1.6;
                    attackDirection = 1;

                    }
                else // left
                {
                    Canvas.SetLeft(Weapon, CenterXPlayer - attackRange);
                    Canvas.SetTop(Weapon, CenterYPlayer - (Player.Height * 1.6) / 2);
                    Weapon.Width = attackRange;
                    Weapon.Height = Player.Height * 1.6;
                    attackDirection = 2;
                }
            }
            else
            {
                if (DeltaY < 0) // down
                {
                    Canvas.SetLeft(Weapon, CenterXPlayer - (Player.Height * 1.6) / 2);
                    Canvas.SetTop(Weapon, CenterYPlayer);
                    Weapon.Width = Player.Height * 1.6;
                    Weapon.Height = attackRange;
                    attackDirection = 3;

                }
                else // up
                {
                    Canvas.SetLeft(Weapon, CenterXPlayer - (Player.Height * 1.6) / 2);
                    Canvas.SetTop(Weapon, CenterYPlayer - attackRange);
                    Weapon.Width = Player.Height * 1.6;
                    Weapon.Height = attackRange;
                    attackDirection = 4;
                }
            }
            
           
            
        
        }





        private void RightClick(object sender, MouseButtonEventArgs e) // CHWILOWE PRZYPISANE DO OBYDWU KLIKNIEC ( PRAWO, LEWO )
        {
            if (blockAttack)
            {
                e.Handled = true;
                return;
            }
            blockAttack = true;
            Speed = 0;
            System.Windows.Point mousePosition = e.GetPosition(sender as IInputElement);
            animateAttack(mousePosition);

            e.Handled = true;
        }
        private void initializeAnimationsForAttack()
        {
            // Ładowanie klatek do animacji ataku
            
            for (int i = 0; i < 4; i++)
            {
                attackAnimationsU[i] = new BitmapImage();                                                                                                                                                                                                                 
                attackAnimationsU[i].BeginInit();                                                                                                                                                                                                  
                attackAnimationsU[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}u.png", UriKind.Absolute);                                                                                       
                attackAnimationsU[i].EndInit();
                attackAnimationsD[i] = new BitmapImage();                                                                                                                                                                                                                 
                attackAnimationsD[i].BeginInit();                                                                                                                                                                                                  
                attackAnimationsD[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}d.png", UriKind.Absolute);                                                                                 
                attackAnimationsD[i].EndInit();
                attackAnimationsL[i] = new BitmapImage();                                                                                                                                                                                                                 
                attackAnimationsL[i].BeginInit();                                                                                                                                                                                                                                
                attackAnimationsL[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}l.png", UriKind.Absolute);                                                                                                                     
                attackAnimationsL[i].EndInit();
                attackAnimationsR[i] = new BitmapImage();                                                                                                                                                                                                                       
                attackAnimationsR[i].BeginInit();                                                                                                                                                                                                                                
                attackAnimationsR[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}p.png", UriKind.Absolute);                                                                                                                     
                attackAnimationsR[i].EndInit();

               



            }
        }
    }
}
