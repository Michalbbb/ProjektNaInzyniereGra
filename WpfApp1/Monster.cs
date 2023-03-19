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
using System.Security.Cryptography.Pkcs;

namespace BasicsOfGame
{
    internal class Goblin
    {
        System.Windows.Shapes.Rectangle body = new System.Windows.Shapes.Rectangle();
        int attackRange = 50;
        int Speed = 130;
        int baseSpeed = 130;
        BitmapImage[] goblinMovementRight = new BitmapImage[5];
        BitmapImage[] goblinMovementLeft = new BitmapImage[5];
        BitmapImage[] goblinAttackRight = new BitmapImage[5];
        BitmapImage[] goblinAttackLeft = new BitmapImage[5];
        int animations = 5;
        int currentAnimation = 0;
        double ticks = 0;
        public bool moveInRightDirection = true;
        ImageBrush goblinSprite = new ImageBrush();
        Canvas BelongTO;
        
        public Goblin(Canvas a, int x, int y)
        {
            body.Height = 64;
            body.Width = 64;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            BelongTO = a;

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
            for (int i = spriteHeight; i < goblinSprits.Height; i += spriteHeight * 2)
            {
                k = 0;
                if (d == 20) break;
                for (int j = 0; j < goblinSprits.Width; j += spriteWidth)
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
                            goblinMovementRight[k] = sprite;
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
                            goblinAttackRight[k - 5] = sprite;
                        }
                        else
                        {
                            goblinAttackLeft[k - 5] = sprite;
                        }

                    }
                    k++;
                }
            }
        }



        private void setRelativeVisibility()
        {
            Canvas.SetZIndex(body, Convert.ToInt32((Canvas.GetTop(body) + body.Height) / 100)-1);
           
        }

        private void NormalizeSpeed(double delta)
        {
            ticks += baseSpeed / 2 * delta;
            Speed = Convert.ToInt32(baseSpeed * delta);
        }
        private Random rnd=new Random();

        
        public void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            
            NormalizeSpeed(delta);

            setRelativeVisibility();

            double moveMonsterByX = 0, moveMonsterByY = 0;

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));

            
            if (playerCenter.X > Canvas.GetLeft(body) + body.Width + attackRange / 2)
            {
                moveMonsterByX = Speed * friction;
            }
            if (playerCenter.X < Canvas.GetLeft(body) - attackRange / 2)
            {
                moveMonsterByX = -Speed * friction;
            }
            if (playerCenter.Y < Canvas.GetTop(body) - body.Height / 2)
            {
                moveMonsterByY = -Speed * friction;
            }
            if (playerCenter.Y > Canvas.GetTop(body) + body.Height * 1.5)
            {
                moveMonsterByY = Speed * friction;
            }
           


            checkCollisions(ref moveMonsterByX, ref moveMonsterByY,friction);
           
            if ((moveMonsterByY != 0|| moveMonsterByX != 0 )&& ticks >= 10 / Speed)
            {

                ticks -= 10 / Speed;
                if (ticks < 0) ticks = 0;
                if (ticks >= 10 / Speed) ticks = 0;
                if (moveMonsterByX > 0)
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
                else if (moveMonsterByX < 0)
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
                else if (moveMonsterByX == 0 )
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
            if(moveMonsterByX==0&&moveMonsterByY==0)
            {
                if (playerCenter.X > Canvas.GetLeft(body) + body.Width / 2)
                {
                    currentAnimation = 0;
                    goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                    body.Fill = goblinSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    currentAnimation = 0;
                    goblinSprite.ImageSource = goblinMovementLeft[currentAnimation];
                    body.Fill = goblinSprite;
                }
            }
            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            if (Canvas.GetTop(body) >= 600 - body.Height) Canvas.SetTop(body, 600 - body.Height);
                if (Canvas.GetTop(body) <= 93-(body.Height*3/4))Canvas.SetTop(body, 93 - (body.Height * 3 / 4));
            



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
        private void checkCollisions(ref double coordinateX,ref double coordinateY,double friction)
        {
            Rect tryGoingUp = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) - 2 * Speed, body.Width, body.Height);
            Rect tryGoingDown = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) + 2 * Speed, body.Width, body.Height);
            Rect tryGoingLeft = new Rect(Canvas.GetLeft(body) - 2 * Speed, Canvas.GetTop(body), body.Width, body.Height);
            Rect tryGoingRight = new Rect(Canvas.GetLeft(body) + 2 * Speed , Canvas.GetTop(body), body.Width, body.Height);
            foreach (var x in BelongTO.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if (x == body) continue;
                if ((string)x.Tag!= "enemy" && (string)x.Tag != "collision") continue;

                Rect hitBoxOfObject;
                if ((string)x.Tag == "enemy") { hitBoxOfObject = new Rect(Canvas.GetLeft(x) + (9 * x.Width / 20), Canvas.GetTop(x) + (9 * x.Height / 20), x.Width / 10, x.Height / 10); }              
                else hitBoxOfObject = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                bool wentThroughY = false;
                if (coordinateY != 0)
                {
                    if (coordinateY > 0)
                    {
                        if (determinateCollision(tryGoingDown, hitBoxOfObject))
                        {
                            coordinateY = 0;
                            if (coordinateX == 0) coordinateX = Speed  * friction;
                            wentThroughY = true;
                        }
                    }
                    else
                    {
                        if (determinateCollision(tryGoingUp, hitBoxOfObject))
                        {
                            coordinateY = 0;
                            if (coordinateX == 0) coordinateX = Speed  * friction;
                            wentThroughY = true;
                        }
                    }
                }
                if (coordinateX != 0)
                {
                    if (coordinateX > 0)
                    {
                        if (determinateCollision(tryGoingRight, hitBoxOfObject))
                        {
                            coordinateX = 0;
                            if (coordinateY == 0 && !wentThroughY) coordinateY = Speed  * friction;
                            

                        }
                    }
                    else
                    {
                        if (determinateCollision(tryGoingLeft, hitBoxOfObject))
                        {
                            coordinateX = 0;
                            if (coordinateY == 0 && !wentThroughY) coordinateY = Speed  * friction;


                        }
                    }
                }
            }



        }
    }
}


