﻿using Accessibility;
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
        Button LevelUp;
        Button SkillsAvailable;
        Button Equipment;
        System.Windows.Shapes.Rectangle BlackScreenOverlay = new System.Windows.Shapes.Rectangle();
        private const float Friction = 0.65f;
        bool tryingAssign = false;
        bool getReadyToDeleteDeadToDot=false;
        public static bool isGameRunning = false;

        Menu gameMenu;
        Grid map;
        //TextBox helper; // Current minimap
        GroupBox miniMapHolder;  // new minimap
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
                map.showMiniMap(miniMapHolder);
                miniMapHolder.Opacity = 0.3;
                
            }
            if(e.Key == Key.P)
            {
                if (tryingAssign)
                {
                    mainCharacter.hideSkillTree();
                    tryingAssign = false;
                }
                else
                {
                    tryingAssign = true;
                    mainCharacter.showSkillTree();
                }
            }
            if (e.Key == Key.Escape)
            {
                if(tryingAssign){
                    mainCharacter.hideSkillTree();
                tryingAssign = false;
                }
                else SwitchState();
                
            }
            if(e.Key == Key.D1)
            {
                
                Jewellery c = new Jewellery(new Random().Next(0,4));
                System.Windows.Point mousePosition = Mouse.GetPosition(GameScreen);
                
                mainCharacter.useFirstSkill(mousePosition,new System.Windows.Point(Canvas.GetLeft(mainCharacter.getBody()) + mainCharacter.getBody().Width/2, Canvas.GetTop(mainCharacter.getBody()) + mainCharacter.getBody().Height / 2));
            }
            if (e.Key == Key.D2)
            {
                System.Windows.Point mousePosition = Mouse.GetPosition(GameScreen);

                mainCharacter.useSecondSkill(mousePosition, new System.Windows.Point(Canvas.GetLeft(mainCharacter.getBody()) + mainCharacter.getBody().Width / 2, Canvas.GetTop(mainCharacter.getBody()) + mainCharacter.getBody().Height / 2));
            }
            if (e.Key == Key.D3)
            {
                System.Windows.Point mousePosition = Mouse.GetPosition(GameScreen);

                mainCharacter.useThirdSkill(mousePosition, new System.Windows.Point(Canvas.GetLeft(mainCharacter.getBody()) + mainCharacter.getBody().Width / 2, Canvas.GetTop(mainCharacter.getBody()) + mainCharacter.getBody().Height / 2));
            }
            if (e.Key == Key.D4)
            {
                System.Windows.Point mousePosition = Mouse.GetPosition(GameScreen);

                mainCharacter.useFourthSkill(mousePosition, new System.Windows.Point(Canvas.GetLeft(mainCharacter.getBody()) + mainCharacter.getBody().Width / 2, Canvas.GetTop(mainCharacter.getBody()) + mainCharacter.getBody().Height / 2));
            }
            if (e.Key == Key.D5)
            {
                System.Windows.Point mousePosition = Mouse.GetPosition(GameScreen);

                mainCharacter.useFifthSkill(mousePosition, new System.Windows.Point(Canvas.GetLeft(mainCharacter.getBody()) + mainCharacter.getBody().Width / 2, Canvas.GetTop(mainCharacter.getBody()) + mainCharacter.getBody().Height / 2));
            }




        }
        private void updateMiniMap()
        {
           
            map.updateMiniMap(miniMapHolder);
        }
        private void setMiniMapPosition()
        {
            //miniMapHolder.Header = "Minimap";
            miniMapHolder.FontSize = 15;
            miniMapHolder.FontFamily = new FontFamily("Algerian");
            miniMapHolder.BorderThickness = new Thickness(0);
            miniMapHolder.Width = 400;
            miniMapHolder.Height = 400;
            Canvas.SetLeft(miniMapHolder, 575 - (miniMapHolder.Width) / 2); //575 is the X value of the center of the screen
            Canvas.SetTop(miniMapHolder, 297 - (miniMapHolder.Height) / 2); //297 is the Y value of the center of the screen
            miniMapHolder.Background = Brushes.Black;
            miniMapHolder.Foreground = Brushes.White;
            Canvas.SetZIndex(miniMapHolder, 998);
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
                miniMapHolder.Opacity = 0;
                map.miniMapClear();
               
            }
            
        }
        
        private void SwitchState()
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
            miniMapHolder = new GroupBox();
            miniMapHolder.Opacity = 0;
            GameScreen.Children.Add(miniMapHolder);
            miniMapHolder.IsEnabled = false;
            setMiniMapPosition();
            
            GameScreen.Focus();
            mainCharacter.startPosition(ref map);
            updateMiniMap();
            mainCharacter.generateTB("enemy", ref boxes);
            LevelUp = new Button();
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/levelUpSprite.png", UriKind.Absolute));
            LevelUp.Width = 50;
            LevelUp.Height = 50;
            LevelUp.Style = (Style)FindResource("IButton");
            LevelUp.Background=sprite;
            Canvas.SetLeft(LevelUp, 50);
            Canvas.SetTop(LevelUp, 50);
            Canvas.SetZIndex(LevelUp, 50);
            LevelUp.IsEnabled = false;
            LevelUp.Visibility = Visibility.Hidden;
            GameScreen.Children.Add(LevelUp);
            LevelUp.Click += assignSkillPoints;
            SkillsAvailable = new Button();
            ImageBrush spriteSkill = new ImageBrush();
            spriteSkill.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/SkillPicker.png", UriKind.Absolute));
            SkillsAvailable.Width = 70;
            SkillsAvailable.Height = 70;
            SkillsAvailable.Style = (Style)FindResource("IButton");
            SkillsAvailable.Background = spriteSkill;
            Canvas.SetLeft(SkillsAvailable, 1130);
            Canvas.SetTop(SkillsAvailable, 530);
            Canvas.SetZIndex(SkillsAvailable, 50);
            SkillsAvailable.IsEnabled = false;
            SkillsAvailable.Visibility = Visibility.Hidden;
            GameScreen.Children.Add(SkillsAvailable);
            SkillsAvailable.Click += allocateActiveSkill;
            Equipment = new Button();
            ImageBrush spriteEq = new ImageBrush();
            spriteEq.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/equipmentClosedSprite.png", UriKind.Absolute));
            Equipment.Width = 50;
            Equipment.Height = 50;
            Equipment.Style = (Style)FindResource("IButton");
            Equipment.Background=spriteEq;
            Canvas.SetLeft(Equipment, 0);
            Canvas.SetTop(Equipment, 50);
            Canvas.SetZIndex(Equipment, 50);
            Equipment.IsEnabled=true;
            Equipment.Click+=showScuffedEquipment;
            Equipment.MouseEnter+=openBackpack;
            Equipment.MouseLeave+=closeBackpack;
            GameScreen.Children.Add(Equipment);
            Monster.deadToDot+=removeDeadToDot;
            isGameRunning = true;
            
        }

        private void allocateActiveSkill(object sender, RoutedEventArgs e)
        {
            mainCharacter.pickSkill();

        }

        private void closeBackpack(object sender, MouseEventArgs e)
        {
            ImageBrush spriteEq = new ImageBrush();
            spriteEq.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/equipmentClosedSprite.png", UriKind.Absolute));
            Equipment.Background=spriteEq;
        }

        private void openBackpack(object sender, MouseEventArgs e)
        {
            ImageBrush spriteEq = new ImageBrush();
            spriteEq.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/equipmentOpenSprite.png", UriKind.Absolute));
            Equipment.Background=spriteEq;
          
        }
        bool statsAreShown=false;
        private void showScuffedEquipment(object sender, RoutedEventArgs e)
        {
           if(!statsAreShown){
            mainCharacter.showStats();
            statsAreShown=true;
           }
           else{
            mainCharacter.hideStats();
            statsAreShown=false;
           }
        }

        private void removeDeadToDot(){
            getReadyToDeleteDeadToDot=true;
        }
        private void assignSkillPoints(object sender, RoutedEventArgs e)
        {
            if (tryingAssign) { mainCharacter.hideSkillTree(); tryingAssign = false; }
            else { mainCharacter.showSkillTree(); tryingAssign = true; }
        }

        public MainWindow()
        {     
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            gameMenu = new Menu(GameScreen,startGame);
            
            this.PreviewKeyDown += MainWindowPreviewKeyDown;
            
            gameMenu.ShowMenu();
            
            


            CompositionTarget.Rendering += CompositionTarget_Rendering;
            
        }
          void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab)
            {
               map.showMiniMap(miniMapHolder);
                miniMapHolder.Opacity = 0.3;
                e.Handled=true;
            }
        }
        private DateTime _lastRenderTime = DateTime.MinValue;
        double deltaTime;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            
            DateTime now = DateTime.Now;
            deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;
            if (!isGameRunning)
            {

                return;
            }

            else GameScreen.Children.Remove(BlackScreenOverlay);

            
            
            gameTick(sender, e);
            checkOpacity("enemy");
            mainCharacter.checkOpacity();
            if (Player.unassignedSkillPoints > 0)
            {
                LevelUp.Content=Player.unassignedSkillPoints.ToString();
                LevelUp.FontSize=30;
                LevelUp.FontFamily=new FontFamily("Algerian");
                LevelUp.Foreground=Brushes.Black;
                LevelUp.BorderBrush=Brushes.Transparent;
                LevelUp.Visibility= Visibility.Visible;
                LevelUp.IsEnabled= true;
            }
            else
            {
                LevelUp.Visibility = Visibility.Hidden;
                LevelUp.IsEnabled = false;
            }
            if (mainCharacter.checkIfCanObtainSkills())
            {
                SkillsAvailable.Content = mainCharacter.returnHowManyActiveSkillsCanBeAllocated();
                SkillsAvailable.FontSize = 30;
                SkillsAvailable.FontFamily = new FontFamily("Algerian");
                SkillsAvailable.Foreground = Brushes.Black;
                SkillsAvailable.BorderBrush = Brushes.Transparent;
                SkillsAvailable.Visibility = Visibility.Visible;
                SkillsAvailable.IsEnabled = true;
            }
            else
            {
                SkillsAvailable.Visibility = Visibility.Hidden;
                SkillsAvailable.IsEnabled = false;
            }
            if (Player.isDead||mainCharacter.getHp()<=0)
            {
                isGameRunning = false;
                Player.isDead = false;
                gameMenu.deathScreen(Player.killedBy, Player.lastDamage);
            }

        }
        private void gameTick(object sender, EventArgs e)
        {
            mainCharacter.gameTick(cam,UpKey,DownKey,RightKey,LeftKey,ref map,deltaTime,Friction,ref boxes, updateMiniMap);
            if(getReadyToDeleteDeadToDot){
                mainCharacter.deleteDeadToDot(ref map,ref boxes);
                getReadyToDeleteDeadToDot=false;
            }
            System.Windows.Point playerCoordinates=mainCharacter.playerCoordinates();
            if(playerCoordinates.X<100&&playerCoordinates.Y<110){
                LevelUp.Opacity=0.5;
                Equipment.Opacity=0.5;
            }
            else{
                LevelUp.Opacity=1;
                Equipment.Opacity=1;
            }
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
                        Canvas.SetTop(boxes[i], (Canvas.GetTop(x) - (x.Height - x.ActualHeight)) - boxes[i].Height - 15);
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
