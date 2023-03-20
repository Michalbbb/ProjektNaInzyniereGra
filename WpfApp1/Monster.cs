﻿using System;
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
        System.Windows.Shapes.Rectangle weapon = new System.Windows.Shapes.Rectangle();
        int savedDirectionX=0;
        int savedDirectionY=0;
        int attackRange = 50;
        int Speed = 130;
        int baseSpeed = 130;
        bool inCollision = false;
        Random rnd=new Random();
        int maxDmg = 10;
        int minDmg = 5;
        BitmapImage[] goblinMovementRight = new BitmapImage[5];
        BitmapImage[] goblinMovementLeft = new BitmapImage[5];
        BitmapImage[] goblinAttackRight = new BitmapImage[6];
        BitmapImage[] goblinAttackLeft = new BitmapImage[6];
        BitmapImage[] attackHitBoxLeft = new BitmapImage[6];
        BitmapImage[] attackHitBoxRight = new BitmapImage[6];
        int animations = 5;
        int currentAnimation = 0;
        double ticks = 0;
        public bool moveInRightDirection = true;
        ImageBrush goblinSprite = new ImageBrush();
        ImageBrush weaponSprite = new ImageBrush();
        Canvas BelongTO;
        double timer = 0;
        bool gettingOut=false;
        bool prepareToAttack = false;
        int attackTicks;
        double attackTimer = 0;
       
        
        public Goblin(Canvas a, int x, int y)
        {
            body.Height = 64;
            body.Width = 64;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            BelongTO = a;
            weapon.Height = 30;
            weapon.Width = 50;
            weapon.Fill = Brushes.Transparent;
            Canvas.SetZIndex(weapon, 15);
            a.Children.Add(weapon);

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
                if (d == 23) break;
                for (int j = 0; j < goblinSprits.Width; j += spriteWidth)
                {
                    if (j > spriteWidth * 10) break;
                    d++;
                    if (d == 23) break;
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
           
                // Ładowanie klatek do animacji ataku

                for (int i = 0; i < 6; i++)
                {
                    attackHitBoxLeft[i] = new BitmapImage();
                attackHitBoxLeft[i].BeginInit();
                attackHitBoxLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/gobAtt{1 + i}l.png", UriKind.Absolute);
                attackHitBoxLeft[i].EndInit();
                attackHitBoxRight[i] = new BitmapImage();
                attackHitBoxRight[i].BeginInit();
                attackHitBoxRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/gobAtt{1 + i}.png", UriKind.Absolute);
                attackHitBoxRight[i].EndInit();
               





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
        

        private void attack(System.Windows.Shapes.Rectangle player, double delta,TextBox dmg)
        {
            if (attackTicks == 6)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                weapon.Fill = Brushes.Transparent;
                return;
            }
            attackTimer += delta * 1000;
            if(attackTicks == 0&&attackTimer/100>1)
            {

                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body)+body.Width*2/3);
                    Canvas.SetTop(weapon, Canvas.GetTop(body)+body.Height/3);
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body)-body.Width/4);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3);
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 1 && attackTimer / 100 > 1)
            {
                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 2&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 3&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 4&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[3];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 5 && attackTimer / 100 > 1)
            {
                if (moveInRightDirection)
                {
                    goblinSprite.ImageSource = goblinAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    goblinSprite.ImageSource = goblinAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {
                    
                        int obecnyDmg = Convert.ToInt32(dmg.Text);
                        obecnyDmg += rnd.Next(minDmg, maxDmg + 1);
                        dmg.Text = obecnyDmg.ToString();
                        dmg.Width = Convert.ToInt16(dmg.Text.Length) * 20;
                        dmg.Opacity = 100;
                        Canvas.SetLeft(dmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (dmg.Width / 2));
                        Canvas.SetTop(dmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - dmg.Height);

                    
                }
                body.Fill = goblinSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }
        public void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction,TextBox dmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            
            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta,dmg);
                return;
            }


            double moveMonsterByX = 0, moveMonsterByY = 0;
            if (playerCenter.X > Canvas.GetLeft(body) + body.Width + attackRange / 2)
            {
                moveMonsterByX = Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.X < Canvas.GetLeft(body) - attackRange / 2)
            {
                moveMonsterByX = -Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.Y < Canvas.GetTop(body) - body.Height / 3)
            {
                moveMonsterByY = -Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.Y > Canvas.GetTop(body) + body.Height )
            {
                moveMonsterByY = Speed * friction;
                tryAttack = false;
            }
            if (moveMonsterByX == 0 && moveMonsterByY == 0)
            {
                if (playerCenter.X > Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = true;
                    currentAnimation = 0;
                    goblinSprite.ImageSource = goblinMovementRight[currentAnimation];
                    body.Fill = goblinSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    goblinSprite.ImageSource = goblinMovementLeft[currentAnimation];
                    body.Fill = goblinSprite;
                }
            }

            if (!tryAttack) 
            checkCollisions(ref moveMonsterByX, ref moveMonsterByY,friction,playerCenter,delta);
            else
            {
                attackTicks = 0;
                attackTimer = 0;
                prepareToAttack = true;
                return;
            }
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
        private void checkCollisions(ref double coordinateX,ref double coordinateY,double friction, System.Windows.Point playerCenter,double delta)
        {
            bool left=false, right=false, down=false, up=false,axisX=false,axisY=false;
            if (coordinateX > 0) right = true;
            if (coordinateY > 0) down = true;
            if (coordinateY < 0) up = true;
            if (coordinateX < 0) left = true;
            if (left || right) axisX = true;
            if(down || up)  axisY = true;
            Rect tryGoingUp = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) - 2 * Speed, body.Width, body.Height);
            Rect tryGoingDown = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) + 2 * Speed, body.Width, body.Height);
            Rect tryGoingLeft = new Rect(Canvas.GetLeft(body) - 2 * Speed, Canvas.GetTop(body), body.Width, body.Height);
            Rect tryGoingRight = new Rect(Canvas.GetLeft(body) + 2 * Speed , Canvas.GetTop(body), body.Width, body.Height);
            bool rCol = false;
            bool lCol = false;
            bool uCol= false;
            bool dCol = false;
            int collisionsDetected = 0;
            int collisionsWithKin = 0;
            foreach (var x in BelongTO.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if (x == body) continue;
                if ((string)x.Tag!= "enemy" && (string)x.Tag != "collision") continue;

                Rect hitBoxOfObject;
                if ((string)x.Tag == "enemy") { hitBoxOfObject = new Rect(Canvas.GetLeft(x) + (9 * x.Width / 20), Canvas.GetTop(x) + (9 * x.Height / 20), x.Width / 10, x.Height / 10); }              
                else hitBoxOfObject = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                
                
             
                    
                   if (determinateCollision(tryGoingDown, hitBoxOfObject))
                   {
                             if (down) axisY = false;
                            collisionsDetected++;
                    if ((string)x.Tag == "enemy") collisionsWithKin++;
                    dCol = true;
                    coordinateY = 0;
                   }
                   if (determinateCollision(tryGoingUp, hitBoxOfObject))
                   {
                           if (up) axisY = false;
                            collisionsDetected++;
                    if ((string)x.Tag == "enemy") collisionsWithKin++;
                          uCol =true;
                    coordinateY = 0;
                }
                   if (determinateCollision(tryGoingRight, hitBoxOfObject))
                   {
                        if (right) axisX = false;
                         rCol = true;
                        collisionsDetected++;
                    if ((string)x.Tag == "enemy") collisionsWithKin++;
                    coordinateX = 0;

                }
                    if (determinateCollision(tryGoingLeft, hitBoxOfObject))
                    {
                        if (left) axisX = false;
                        lCol = true;
                        collisionsDetected++;
                    if ((string)x.Tag == "enemy") collisionsWithKin++;
                    coordinateX = 0;

                }
                
                    
                
            }
            if (inCollision)
            {
                if (gettingOut && collisionsDetected > 0)
                {
                    gettingOut = false;
                    inCollision = false;
                }
                if (collisionsDetected == collisionsWithKin)
                {
                    if (rCol)
                    {
                        Canvas.SetLeft(body, Canvas.GetLeft(body) - 10);
                    }
                    if (lCol)
                    {
                        Canvas.SetLeft(body, Canvas.GetLeft(body) + 10);
                    }
                    if (uCol)
                    {
                        Canvas.SetTop(body, Canvas.GetTop(body) + 10);
                    }
                    if (dCol)
                    {
                        Canvas.SetTop(body, Canvas.GetTop(body) - 10);
                    }
                }
                
                coordinateY = savedDirectionY * Speed * friction;
                coordinateX = savedDirectionX * Speed * friction;
            }
            else if (axisX || axisY) // 1 side collision ( not stucked )
            {
                if (!axisX && (left || right)) coordinateX = 0;
                if (!axisY && (up || down)) coordinateY = 0;
                return;
            }
            else // 2 side collision or 1 side while walking in 1 direction collision ( stucked )
            {
               
                System.Windows.Point bodyCenter = new System.Windows.Point(Canvas.GetLeft(body)-body.Width/2, Canvas.GetTop(body)-body.Height/2);
                if (right || left && (!uCol||!dCol))
                {
                    if (bodyCenter.Y > playerCenter.Y)
                    {
                        if (!uCol) coordinateY = -Speed * friction;
                        else coordinateY = Speed * friction;

                    }
                    else
                    {
                        if (!dCol) coordinateY = Speed * friction;
                        else coordinateY = -Speed * friction;
                    }
                    inCollision = true;
                    if (coordinateY < 0) savedDirectionY = -1;
                    else savedDirectionY = 1;
                }
                else if(down||up && (!lCol || !rCol))
                {
                    if (bodyCenter.X > playerCenter.X)
                    {
                        if (!lCol) coordinateX = -Speed * friction;
                        else coordinateX = Speed * friction;

                    }
                    else
                    {
                        if (!rCol) coordinateX = Speed * friction;
                        else coordinateX = -Speed * friction;
                    }
                    inCollision = true;
                    if (coordinateX < 0) savedDirectionX = -1;
                    else savedDirectionX = 1;
                }
            }
            if (collisionsDetected == 0)
            {
                if (inCollision&&!gettingOut)
                {
                    timer = 300;
                    gettingOut = true;
                }
                else if (gettingOut)
                {
                    timer -= delta*1000;
                    if(timer <= 0)
                    {
                        savedDirectionY = 0;
                        savedDirectionX = 0;
                        gettingOut = false;
                        inCollision = false;
                    }
                }
            }


        }
    }
}


