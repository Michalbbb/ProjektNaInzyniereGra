using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BasicsOfGame
{
    internal class MapsObjects
    {

        System.Windows.Shapes.Rectangle mapObject = new System.Windows.Shapes.Rectangle();
        System.Windows.Shapes.Rectangle objectCollision = new System.Windows.Shapes.Rectangle();
        ImageBrush objSprite = new ImageBrush();
        public MapsObjects(int number, int x, int y)
        {
            if (number == 0)//Statue
            {
                mapObject.Width = 100;
                mapObject.Height = 150;
                mapObject.Tag = "obstacle";
                objSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/bgdecorations/Statue.png", UriKind.Absolute));
                mapObject.Fill = objSprite;
                objectCollision.Width = 70;
                objectCollision.Height = 3;
                objectCollision.Fill = Brushes.Transparent;
                objectCollision.Tag = "collision";
                Canvas.SetLeft(mapObject, x);
                Canvas.SetTop(mapObject, y);
                Canvas.SetLeft(objectCollision, x + 14);
                Canvas.SetTop(objectCollision, y + 115);
               
            }
            else if (number == 1) //Destroyed Statue
            {
                mapObject.Width = 100;
                mapObject.Height = 140;
                mapObject.Tag = "obstacle";
                objSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/bgdecorations/StatueDestroyed.png", UriKind.Absolute));
                mapObject.Fill = objSprite;
                objectCollision.Width = 70;
                objectCollision.Height = 3;
                objectCollision.Fill = Brushes.Transparent;
                objectCollision.Tag = "collision";
                Canvas.SetLeft(mapObject, x);
                Canvas.SetTop(mapObject, y);
                Canvas.SetLeft(objectCollision, x + 14);
                Canvas.SetTop(objectCollision, y + 105);
              
            }
            else if (number == 2) //Flag
            {
                mapObject.Width = 44;
                mapObject.Height = 98;
                mapObject.Tag = "obstacle";
                objSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/bgdecorations/Flag.png", UriKind.Absolute));
                mapObject.Fill = objSprite;
                objectCollision.Width = 16;
                objectCollision.Height = 3;
                objectCollision.Fill = Brushes.Transparent;
                objectCollision.Tag = "collision";
                Canvas.SetLeft(mapObject, x);
                Canvas.SetTop(mapObject, y);
                Canvas.SetLeft(objectCollision, x + 24);
                Canvas.SetTop(objectCollision, y + 80);
             
            }
            else if (number == 3) //Tarnished Flag 
            {
                mapObject.Width = 51;
                mapObject.Height = 100;
                mapObject.Tag = "obstacle";
                objSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/bgdecorations/TarnishedFlag.png", UriKind.Absolute));
                mapObject.Fill = objSprite;
                objectCollision.Width = 16;
                objectCollision.Height = 3;
                objectCollision.Fill = Brushes.Transparent;
                objectCollision.Tag = "collision";
                Canvas.SetLeft(mapObject, x);
                Canvas.SetTop(mapObject, y);
                Canvas.SetLeft(objectCollision, x + 24);
                Canvas.SetTop(objectCollision, y + 80);
               
            }
            else // Statue
            {
                mapObject.Width = 100;
                mapObject.Height = 150;
                mapObject.Tag = "obstacle";
                objSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/bgdecorations/Statue.png", UriKind.Absolute));
                mapObject.Fill = objSprite;
                objectCollision.Width = 70;
                objectCollision.Height = 3;
                objectCollision.Fill = Brushes.Transparent;
                objectCollision.Tag = "collision";
                Canvas.SetLeft(mapObject, x);
                Canvas.SetTop(mapObject, y);
                Canvas.SetLeft(objectCollision, x + 14);
                Canvas.SetTop(objectCollision, y + 105);
                
            }

        }
        public void Add(Canvas canv)
        {
            canv.Children.Add(mapObject);
            canv.Children.Add(objectCollision);
        }
        
    }
}

