using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace BasicsOfGame
{
    internal class Layout
    {
        public Layout() { 
        
        }
        public void makeBackground(Canvas GameScreen, bool leftDoor, bool rightDoor, bool upDoor, bool downDoor, int tlo,ref bool leftDoorExist,ref bool rightDoorExist,ref bool upDoorExist,ref bool downDoorExist, int doorDirection)
        {
            ImageBrush noweTlo = new ImageBrush();
            noweTlo.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/backgrounds/bg{tlo}.png", UriKind.Absolute));
            GameScreen.Background = noweTlo;

            leftDoorExist = false; // Reset usable doors 
            rightDoorExist = false;
            upDoorExist = false;
            downDoorExist = false;
            foreach (System.Windows.Shapes.Rectangle x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>()) // removing remains of doors
            {
                if((string)x.Name == "Player")
                {
                    switch (doorDirection)  //we substract 20 from the position of the door next to which we spawn the player
                    {
                        case 0: //when player enters top door spawn him at bottom door in next room
                            Canvas.SetLeft(x, 540);  
                            Canvas.SetTop(x, 594 - 109);
                            break;
                        case 1: //when player enters right door spawn him at left door in next room
                            Canvas.SetLeft(x, 0 + 20);
                            Canvas.SetTop(x, 288);
                            break;
                        case 2: //when player enters bottom door spawn him at top door in next room
                            Canvas.SetLeft(x, 540);   
                            Canvas.SetTop(x, 0 + 20);
                            break;
                        case 3: //when player enters left door spawn him at right door in next room
                            Canvas.SetLeft(x, 1150 - 20);
                            Canvas.SetTop(x, 288);
                            break;
                        case 4: //special case for spawning player at the beginning of the game
                            Canvas.SetLeft(x, 540);   
                            Canvas.SetTop(x, 288);
                            break;
                    }
                }
                /*if ((string)x.Tag == "door" || (string)x.Tag == "obstacle")
                {
                    GameScreen.Children.Remove(x);
                }*/
            }
            if (leftDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 8;
                door.Height = 130;
                Canvas.SetLeft(door, 0);
                Canvas.SetTop(door, 260);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/leftDoor.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                leftDoorExist = true;
            }
            if (rightDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 50;
                door.Height = 150;
                Canvas.SetLeft(door, 1150);
                Canvas.SetTop(door, 250);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/rightDoor.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                rightDoorExist = true;
            }
            if (upDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 125;
                door.Height = 94;
                Canvas.SetLeft(door, 507);
                Canvas.SetTop(door, 0);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/upDoor.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                upDoorExist = true;
            }
            if (downDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 130;
                door.Height = 6;
                Canvas.SetLeft(door, 509);
                Canvas.SetTop(door, 594);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/downDoor.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                downDoorExist = true;
            }

        }
    }
}
