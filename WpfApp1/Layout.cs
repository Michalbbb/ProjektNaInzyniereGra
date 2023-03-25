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
        public void makeBackground(Canvas GameScreen, bool leftDoor, bool rightDoor, bool upDoor, bool downDoor, int tlo,ref bool leftDoorExist,ref bool rightDoorExist,ref bool upDoorExist,ref bool downDoorExist)
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
                if ((string)x.Tag == "door")
                {
                    GameScreen.Children.Remove(x);
                }
            }
            if (leftDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 8;
                door.Height = 75;
                Canvas.SetLeft(door, 0);
                Canvas.SetTop(door, 288);
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
                door.Height = 75;
                Canvas.SetLeft(door, 1150);
                Canvas.SetTop(door, 288);
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
                door.Width = 59;
                door.Height = 94;
                Canvas.SetLeft(door, 540);
                Canvas.SetTop(door, 0);
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
                door.Width = 59;
                door.Height = 6;
                Canvas.SetLeft(door, 540);
                Canvas.SetTop(door, 594);
                door.Tag = "door";
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/downDoor.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                downDoorExist = true;
            }
        }
    }
}
