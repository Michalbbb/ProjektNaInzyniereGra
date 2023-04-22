using Accessibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
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
using System.Xml;
using static System.Net.Mime.MediaTypeNames; // Nie mam pojecia co to robi i jak sie tu znalazlo

namespace BasicsOfGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private bool UpKey, DownKey, LeftKey, RightKey;
        private List<TextBox> boxes;
        
        System.Windows.Shapes.Rectangle BlackScreenOverlay = new System.Windows.Shapes.Rectangle();
        private const float Friction = 0.65f;
        
        
        public static bool isGameRunning = false;

        Menu gameMenu;
        Grid map;
        TextBox helper; // Current minimap
        Player mainCharacter;


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
            if (e.Key == Key.Tab)
            {
                helper.Opacity = 1;
                
            }
            if (e.Key == Key.Escape)
            {
                StopGame();
            }
            
            


        }
        private void write()
        {
            map.ShowMap(helper);
            helper.Width = 200;
            helper.Height = 200;
            helper.Background = Brushes.Black;
            helper.Foreground = Brushes.White;
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
            if (e.Key == Key.Tab)
            {
                helper.Opacity = 0;
            }
            
        }
        
        private void StopGame()
        {
            if (isGameRunning)
            {

                isGameRunning = false;
                GameScreen.Children.Remove(BlackScreenOverlay);
                GameScreen.Children.Add(BlackScreenOverlay);
                BlackScreenOverlay.Width = this.Width;
                BlackScreenOverlay.Height = this.Height;
                BlackScreenOverlay.Opacity = 0.3;
                BlackScreenOverlay.Fill = Brushes.Black;
                Canvas.SetLeft(BlackScreenOverlay, 0);
                Canvas.SetTop(BlackScreenOverlay, 0);
                Canvas.SetZIndex(BlackScreenOverlay, 50);
                BlackScreenOverlay.Visibility = Visibility.Visible;
               
                gameMenu.pauseGameMenu();
                

            }
            else
            {
                
                isGameRunning = true;
                gameMenu.unpause();
                
               
            }
        }
        public void startGame()
        {
            
            List<UIElement>toRemove=new List<UIElement>();
            foreach(System.Windows.UIElement x in GameScreen.Children)
            {
                toRemove.Add(x);
            }
            for(int i=toRemove.Count-1; i>=0; i--)
            {
                GameScreen.Children.Remove(toRemove[i]);
            }
            mainCharacter = new Player(GameScreen);
            map = new Grid(GameScreen);
            helper = new TextBox();
            helper.Opacity = 0;
            GameScreen.Children.Add(helper);
            helper.IsEnabled = false;
            write();
            
            GameScreen.Focus();
            mainCharacter.startPosition(ref map);
            mainCharacter.generateTB("enemy", ref boxes);



            isGameRunning = true;
        }
        public MainWindow()
        {     
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            gameMenu = new Menu(GameScreen,startGame);
            
            
            
            gameMenu.ShowMenu();
            
            


            CompositionTarget.Rendering += CompositionTarget_Rendering;
            
        }
        private DateTime _lastRenderTime = DateTime.MinValue;
        double deltaTime;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            
            DateTime now = DateTime.Now;
            deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;
            if (!isGameRunning)  return;
            
            else GameScreen.Children.Remove(BlackScreenOverlay);

            
            
            gameTick(sender, e);
            checkOpacity("enemy");
            mainCharacter.checkOpacity();
          
            if (Player.isDead||mainCharacter.getHp()<=0)
            {
                isGameRunning = false;
                Player.isDead = false;
                gameMenu.deathScreen(Player.killedBy, Player.lastDamage);
            }

        }
        private void gameTick(object sender, EventArgs e)
        {
            mainCharacter.gameTick(cam,UpKey,DownKey,RightKey,LeftKey,ref map,deltaTime,Friction,ref boxes);
        }
        private void checkOpacity(string tag)
        {
            int i = 0;
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == tag)
                {
                        
                       if (boxes[i].Opacity > 0) boxes[i].Opacity -= mainCharacter.getSpeed() / 400;
                        else boxes[i].Text = "0"; 
                        
                        Canvas.SetLeft(boxes[i], Canvas.GetLeft(x) + (x.ActualWidth / 2) - (boxes[i].Width / 2));
                        Canvas.SetTop(boxes[i], (Canvas.GetTop(x) - (x.Height - x.ActualHeight)) - boxes[i].Height);
                        i++;


                }


            }
            mainCharacter.checkOpacity();
           
            

        }
        
        
        
        
       


        private void RightClick(object sender, MouseButtonEventArgs e) // CHWILOWE PRZYPISANE DO OBYDWU KLIKNIEC ( PRAWO, LEWO )
        {
            if (!isGameRunning)
            {
                e.Handled = true;
                return;
            }
            if (mainCharacter.getBlock())
            {
                e.Handled = true;
                return;
            }
            mainCharacter.setBlock(true);
            
            System.Windows.Point mousePosition = e.GetPosition(sender as IInputElement);
            mainCharacter.animateAttack(mousePosition);

            e.Handled = true;
        }
        
    }
}
