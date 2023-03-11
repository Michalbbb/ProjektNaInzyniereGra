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

        private DispatcherTimer attackTimer = new DispatcherTimer();

        private bool UpKey, DownKey, LeftKey, RightKey, rightD, returnUp, returnDown, returnLeft, returnRight, blockAttack;

        private const float Friction = 0.65f;
        private double SpeedX, SpeedY, Speed = 2, baseSpeed = 2;
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush weaponSprite = new ImageBrush();
        private const int animations = 6;
        BitmapImage[] rightRun = new BitmapImage[animations];
        BitmapImage[] leftRun = new BitmapImage[animations];
        BitmapImage[] attackAnimationsU = new BitmapImage[4];
        BitmapImage[] attackAnimationsD = new BitmapImage[4];
        BitmapImage[] attackAnimationsL = new BitmapImage[4];
        BitmapImage[] attackAnimationsR = new BitmapImage[4];
        private int intervalForAttackAnimations = 30;
        private double ticksDone = 0;
        private int currentMovementAnimation = 0;
        private int attackRange = 100, attackDirection, attackTicks = 0;
        private double unlockAttack = 0;




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
            attackTimer.Interval = TimeSpan.FromMilliseconds(intervalForAttackAnimations);
            attackTimer.Tick += attackOmni;
            for (int i = 0; i < animations; i++)                                                                                                                                                                                                                         //ta pentla tylko ładuje obrazki 
            {
                leftRun[i] = new BitmapImage();                                                                                                                                                                                                                         //coś jak wskaźnik na tablicę animacji (typ obrazek)
                leftRun[i].BeginInit();                                                                                                                                                                                                                                 //zaczynasz se inicjalizacje 
                leftRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}l.png", UriKind.Absolute);                                                                                                                      //podajesz sciezke do obrazka
                leftRun[i].EndInit();                                                                                                                                                                                                                                   //konczysz inicjalizacje 
                rightRun[i] = new BitmapImage();
                rightRun[i].BeginInit();
                rightRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}.png", UriKind.Absolute);
                rightRun[i].EndInit();

            }
            initAttack();

            playerSprite.ImageSource = rightRun[0];
            rightD = true;
            Player.Fill = playerSprite;
            CompositionTarget.Rendering += CompositionTarget_Rendering; //funkcja wbudowana odpala się przy nowym renderingu 

        }
        private DateTime _lastRenderTime = DateTime.MinValue;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;
            if (Speed != 0) Speed = baseSpeed * 30 * deltaTime;
            ticksDone += 30 * deltaTime;
            if (blockAttack)
            {
                unlockAttack += Convert.ToDouble(1000 * deltaTime);
                if (unlockAttack > 500) { blockAttack = false; unlockAttack = 0; }

            }
            gameTick(sender, e);

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
                if (Canvas.GetTop(Player) + Canvas.GetLeft(Player) > 1520) { }
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
                if (ticksDone >= 5 / Speed)
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
                    ticksDone -= 5 / Speed;
                    if (ticksDone < 0) ticksDone = 0;
                    if (ticksDone >= 5 / Speed) ticksDone = 0;
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
            if (attackTicks == 4)
            {
                attackTicks = 0;
                attackTimer.Stop();
                Weapon.Fill = new SolidColorBrush(Colors.Transparent);
                Speed = baseSpeed;
                return;

            }
            if (attackDirection == 1)
            {
                weaponSprite.ImageSource = attackAnimationsR[attackTicks];
                Weapon.Fill = weaponSprite;
                attackTicks++;
            }
            else if (attackDirection == 2)
            {
                weaponSprite.ImageSource = attackAnimationsL[attackTicks];
                Weapon.Fill = weaponSprite;
                attackTicks++;
            }
            else if (attackDirection == 3)
            {
                weaponSprite.ImageSource = attackAnimationsD[attackTicks];
                Weapon.Fill = weaponSprite;
                attackTicks++;
            }
            else if (attackDirection == 4)
            {
                weaponSprite.ImageSource = attackAnimationsU[attackTicks];
                Weapon.Fill = weaponSprite;
                attackTicks++;
            }
        }


        private void animateAttack(System.Windows.Point mousePosition)
        {


            double CenterXPlayer = Canvas.GetLeft(Player) + Player.Width / 2;
            double CenterYPlayer = Canvas.GetTop(Player) + Player.Height / 2;
            double DeltaX = CenterXPlayer - mousePosition.X;
            double DeltaY = CenterYPlayer - mousePosition.Y;
            int direction;
            if (abs(DeltaX) - abs(DeltaY) >= 0)
            {
                if (DeltaX < 0) direction = 1;
                else direction = 2;
            }
            else
            {
                if (DeltaY < 0) direction = 3;
                else direction = 4;
            }
            if (direction == 1) //prawo
            {
                Canvas.SetLeft(Weapon, CenterXPlayer);
                Canvas.SetTop(Weapon, CenterYPlayer - (Player.Height * 1.6) / 2);
                Weapon.Width = attackRange;
                Weapon.Height = Player.Height * 1.6;
                attackDirection = 1;
                attackTimer.Start();


            }
            else if (direction == 2) //lewo
            {
                Canvas.SetLeft(Weapon, CenterXPlayer - attackRange);
                Canvas.SetTop(Weapon, CenterYPlayer - (Player.Height * 1.6) / 2);
                Weapon.Width = attackRange;
                Weapon.Height = Player.Height * 1.6;
                attackDirection = 2;
                attackTimer.Start();
            }
            else if (direction == 3) //dol
            {
                Canvas.SetLeft(Weapon, CenterXPlayer - (Player.Height * 1.6) / 2);
                Canvas.SetTop(Weapon, CenterYPlayer);
                Weapon.Width = Player.Height * 1.6;
                Weapon.Height = attackRange;
                attackDirection = 3;
                attackTimer.Start();
            }
            else if (direction == 4) // gora
            {
                Canvas.SetLeft(Weapon, CenterXPlayer - (Player.Height * 1.6) / 2);
                Canvas.SetTop(Weapon, CenterYPlayer - attackRange);
                Weapon.Width = Player.Height * 1.6;
                Weapon.Height = attackRange;
                attackDirection = 4;
                attackTimer.Start();
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
        private void initAttack()
        {
            //ta pentla tylko ładuje obrazki 
            for (int i = 0; i < 4; i++)
            {
                attackAnimationsU[i] = new BitmapImage();                                                                                                                                                                                                                         //coś jak wskaźnik na tablicę animacji (typ obrazek)
                attackAnimationsU[i].BeginInit();                                                                                                                                                                                                                                 //zaczynasz se inicjalizacje 
                attackAnimationsU[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}u.png", UriKind.Absolute);                                                                                                                      //podajesz sciezke do obrazka
                attackAnimationsU[i].EndInit();
                attackAnimationsD[i] = new BitmapImage();                                                                                                                                                                                                                         //coś jak wskaźnik na tablicę animacji (typ obrazek)
                attackAnimationsD[i].BeginInit();                                                                                                                                                                                                                                 //zaczynasz se inicjalizacje 
                attackAnimationsD[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}d.png", UriKind.Absolute);                                                                                                                      //podajesz sciezke do obrazka
                attackAnimationsD[i].EndInit();
                attackAnimationsL[i] = new BitmapImage();                                                                                                                                                                                                                         //coś jak wskaźnik na tablicę animacji (typ obrazek)
                attackAnimationsL[i].BeginInit();                                                                                                                                                                                                                                 //zaczynasz se inicjalizacje 
                attackAnimationsL[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}l.png", UriKind.Absolute);                                                                                                                      //podajesz sciezke do obrazka
                attackAnimationsL[i].EndInit();
                attackAnimationsR[i] = new BitmapImage();                                                                                                                                                                                                                         //coś jak wskaźnik na tablicę animacji (typ obrazek)
                attackAnimationsR[i].BeginInit();                                                                                                                                                                                                                                 //zaczynasz se inicjalizacje 
                attackAnimationsR[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}p.png", UriKind.Absolute);                                                                                                                      //podajesz sciezke do obrazka
                attackAnimationsR[i].EndInit();

                //konczysz inicjalizacje 



            }
        }
    }
}
