using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Automation.Text;
using System.Windows.Media.Animation;

namespace BasicsOfGame
{
    internal class Goblin
    {
        System.Windows.Shapes.Rectangle body = new System.Windows.Shapes.Rectangle();
        int attackRange = 100;
        int Speed = 130;
        int baseSpeed = 130;
        BitmapImage[] goblinMovementRight=new BitmapImage[5];
        BitmapImage[] goblinMovementLeft = new BitmapImage[5];
        BitmapImage[] goblinAttackRight = new BitmapImage[5];
        BitmapImage[] goblinAttackLeft = new BitmapImage[5];
        int animations = 5;
        int currentAnimation = 0;
        double ticks = 0;
        bool moveInRightDirection = true;
        ImageBrush goblinSprite=new ImageBrush();
        public Goblin(Canvas a, int x, int y)
        {
            body.Height = 64;
            body.Width = 64;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            
            
            a.Children.Add(body);

            body.SetValue(Canvas.TopProperty, (double)x);
            body.SetValue(Canvas.LeftProperty, (double)y);
            loadImages();
            goblinSprite.ImageSource = goblinMovementRight[0];
            body.Fill = goblinSprite;

        }
        public void loadImages()
        {
            BitmapImage goblinSprits = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/goblinsword.png", UriKind.Absolute));
            int spriteWidth = 64;
            int spriteHeight = 64;
            int k;
            int d = 0;
            for(int i = spriteHeight; i < goblinSprits.Height; i += spriteHeight*2)
            {
                k = 0;
                if (d == 20) break;
                for (int j=0; j < goblinSprits.Width; j+=spriteWidth)
                {
                    if (j > spriteWidth * 9) break;
                    d++;
                    if (d == 20) break;
                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(goblinSprits, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);

                    // Create a new BitmapImage object and set its source to the MemoryStream
                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();
                    if (k < 5)
                    {
                        if (i <= spriteWidth)
                        {
                            goblinMovementRight[k] =sprite;
                        }
                        else
                        {
                            goblinMovementLeft[k] = sprite;
                        }
                    }
                    else
                    {
                        if (i <= spriteWidth)
                        {
                            goblinAttackRight[k-5] = sprite;
                        }
                        else
                        {
                            goblinAttackLeft[k-5] = sprite;
                        }

                    }
                    k++;
                }
            }
        }









        public void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction)
        {
            if (delta > 1) return;
            ticks += baseSpeed / 2 * delta;
            Speed = Convert.ToInt32(baseSpeed * delta);
            double x = 0, y = 0;
            System.Windows.Point p = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            // p - center of player

            if (p.X > Canvas.GetLeft(body) + body.Width + attackRange / 2)
            {
                x = Speed* friction;
            }
            if (p.X < Canvas.GetLeft(body) - attackRange / 2)
            {
                x = -Speed* friction;
            }
            if (p.Y < Canvas.GetTop(body) - body.Height / 2)
            {
                y = -Speed* friction;
            }
            if (p.Y > Canvas.GetTop(body) + body.Height *1.5)
            {
                y = Speed* friction;
            }
            if (Speed!=0&&ticks >= 10/Speed)
            {
                
                ticks -= 10 / Speed;
                if (ticks < 0) ticks = 0;
                if (ticks >= 10 / Speed) ticks = 0;
                if (x > 0)
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                    else
                    {
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                }
                if (x < 0)
                {
                    if (!moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementLeft[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                    else
                    {
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                }
                if (x == 0 && (y>1||y<-1))
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        goblinSprite.ImageSource = goblinMovementLeft[currentAnimation];
                        body.Fill = goblinSprite;
                    }
                }
            }
            Canvas.SetLeft(body, Canvas.GetLeft(body)+x);
            Canvas.SetTop(body, Canvas.GetTop(body)+y);
        }


       
    }
}


