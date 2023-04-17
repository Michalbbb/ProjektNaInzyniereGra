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
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Controls.Primitives;

namespace BasicsOfGame
{
    public class Monster
    {
        protected int expGiven;
        protected System.Windows.Shapes.Rectangle body = new System.Windows.Shapes.Rectangle();
        protected System.Windows.Shapes.Rectangle weapon = new System.Windows.Shapes.Rectangle();
        protected int savedDirectionX = 0;
        protected int savedDirectionY = 0;
        protected int attackRange ;
        protected double Speed;
        protected double baseSpeed;
        protected bool inCollision = false;
        protected double timer = 0;
        protected bool gettingOut = false;
        protected bool prepareToAttack = false;
        protected int attackTicks;
        protected double attackTimer = 0;
        protected Canvas BelongTO;
        protected double ticks = 0;
        protected System.Windows.Shapes.Rectangle monsterHpBar;
        protected BitmapImage[] monsterMovementRight;
        protected BitmapImage[] monsterMovementLeft;
        protected BitmapImage[] monsterAttackRight;
        protected BitmapImage[] monsterAttackLeft;
        protected BitmapImage[] attackHitBoxLeft;
        protected BitmapImage[] attackHitBoxRight;
        public bool moveInRightDirection = true;
        protected ImageBrush monsterSprite = new ImageBrush();
        protected ImageBrush weaponSprite = new ImageBrush();
        protected Random rnd = new Random();
        protected int minDmg ;
        protected int maxDmg ;
        protected int animations;
        protected int currentAnimation ;
        protected double diffMulti = 1.0;
        protected double healthPoints;
        protected double maxHealthPoints;
        protected bool dead = false;
        protected static List<Tuple<int,Double>> damageOverTime=new List<Tuple<int,Double>>();
        public static string killedBy="Damage over time";
        public static bool isDead=false;
        public static int lastDamage = 1;

        public static void update(List<Tuple<int, Double>> listOfDots)
        {
             foreach(var x in damageOverTime)
            {
                listOfDots.Add(x);
            }
            damageOverTime.Clear();
        }
        public bool determinateCollision(Rect player, Rect obj)
        {
            if (obj.X < (player.X + player.Width) && (obj.X + obj.Width) > player.X)
            {

                if (obj.Y < (player.Y + player.Height) && (obj.Y + obj.Height) > player.Y) return true;
                else return false;
            }
            else return false;
        }
        public void checkCollisions(ref double coordinateX, ref double coordinateY, double friction, System.Windows.Point playerCenter, double delta)
        {
            bool left = false, right = false, down = false, up = false, axisX = false, axisY = false;
            if (coordinateX > 0) right = true;
            if (coordinateY > 0) down = true;
            if (coordinateY < 0) up = true;
            if (coordinateX < 0) left = true;
            if (left || right) axisX = true;
            if (down || up) axisY = true;
            Rect tryGoingUp = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) - 2 * Speed, body.Width, body.Height);
            Rect tryGoingDown = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body) + 2 * Speed, body.Width, body.Height);
            Rect tryGoingLeft = new Rect(Canvas.GetLeft(body) - 2 * Speed, Canvas.GetTop(body), body.Width, body.Height);
            Rect tryGoingRight = new Rect(Canvas.GetLeft(body) + 2 * Speed, Canvas.GetTop(body), body.Width, body.Height);
            bool rCol = false;
            bool lCol = false;
            bool uCol = false;
            bool dCol = false;
            int collisionsDetected = 0;
            int collisionsWithKin = 0;
            foreach (var x in BelongTO.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if (x == body) continue;
                if ((string)x.Tag != "enemy" && (string)x.Tag != "collision") continue;

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
                    uCol = true;
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
                    Rect myBody = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body), body.Width, body.Height);
                    foreach (Goblin g in BelongTO.Children.OfType<Goblin>())
                    {
                        if (body == g.body) continue;
                        else
                        {
                            Rect gBody = new Rect(Canvas.GetLeft(g.body), Canvas.GetTop(g.body), g.body.Width, g.body.Height);
                            if (determinateCollision(myBody, gBody))
                            {

                                if (Canvas.GetLeft(body) > Canvas.GetLeft(g.body))
                                {
                                    Canvas.SetLeft(body, Canvas.GetLeft(body) + Speed);
                                    Canvas.SetLeft(g.body, Canvas.GetLeft(g.body) - Speed);
                                }
                                else
                                {
                                    Canvas.SetLeft(body, Canvas.GetLeft(body) - Speed);
                                    Canvas.SetLeft(g.body, Canvas.GetLeft(g.body) + Speed);
                                }
                                coordinateX = 0;
                                coordinateY = 0;
                                return;
                            }

                        }
                    }

                }

                coordinateY = savedDirectionY * Speed * friction;
                coordinateX = savedDirectionX * Speed * friction;
            }

            else if (axisX || axisY) // 1 side collision ( not stucked )
            {
                if (collisionsDetected == collisionsWithKin && collisionsDetected > 0)
                {


                    Rect myBody = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body), body.Width, body.Height);
                    foreach (Goblin g in BelongTO.Children.OfType<Goblin>())
                    {
                        if (body == g.body) continue;
                        else
                        {
                            Rect gBody = new Rect(Canvas.GetLeft(g.body), Canvas.GetTop(g.body), g.body.Width, g.body.Height);
                            if (determinateCollision(myBody, gBody))
                            {

                                if (Canvas.GetLeft(body) > Canvas.GetLeft(g.body))
                                {
                                    Canvas.SetLeft(body, Canvas.GetLeft(body) + Speed);
                                    Canvas.SetLeft(g.body, Canvas.GetLeft(g.body) - Speed);
                                }
                                else
                                {
                                    Canvas.SetLeft(body, Canvas.GetLeft(body) - Speed);
                                    Canvas.SetLeft(g.body, Canvas.GetLeft(g.body) + Speed);
                                }
                                coordinateX = 0;
                                coordinateY = 0;
                                return;
                            }

                        }
                    }

                }
                if (!axisX && (left || right)) coordinateX = 0;
                if (!axisY && (up || down)) coordinateY = 0;
                return;
            }
            else // 2 side collision or 1 side while walking in 1 direction collision ( stucked )
            {

                System.Windows.Point bodyCenter = new System.Windows.Point(Canvas.GetLeft(body) - body.Width / 2, Canvas.GetTop(body) - body.Height / 2);
                if (right || left && (!uCol || !dCol))
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
                else if (down || up && (!lCol || !rCol))
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
                if (inCollision && !gettingOut)
                {
                    timer = 300;
                    gettingOut = true;
                }
                else if (gettingOut)
                {
                    timer -= delta * 1000;
                    if (timer <= 0)
                    {
                        savedDirectionY = 0;
                        savedDirectionX = 0;
                        gettingOut = false;
                        inCollision = false;
                    }
                }
            }
           


        }
        public void setCanvas(Canvas Canv)
        {
            BelongTO = Canv;
        }
        public virtual void loadImages() { }
        public virtual void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization) { }
        protected void setRelativeVisibility()
        {
            Canvas.SetZIndex(body, Convert.ToInt32((Canvas.GetTop(body) + body.Height) / 100) - 1);

        }

        protected void NormalizeSpeed(double delta)
        {
            ticks += baseSpeed / 2 * delta;
            Speed = Convert.ToInt32(baseSpeed * delta);
        }
        public void add()
        {
            BelongTO.Children.Add(weapon);
            BelongTO.Children.Add(body);
            BelongTO.Children.Add(monsterHpBar);
        }
        public void remove()
        {
            BelongTO.Children.Remove(weapon);
            BelongTO.Children.Remove(body);
            BelongTO.Children.Remove(monsterHpBar);
        }
        public void setDiff(double plus)
        {
            diffMulti += plus;
            minDmg = Convert.ToInt32(minDmg * diffMulti);
            maxDmg = Convert.ToInt32(maxDmg * diffMulti);
            healthPoints = healthPoints * diffMulti;
            maxHealthPoints=maxHealthPoints * diffMulti;
        }
        protected void hpBar()
        {
            monsterHpBar = new System.Windows.Shapes.Rectangle();
            monsterHpBar.Width = (body.Width*4)/5;
            monsterHpBar.Height = 8;
            monsterHpBar.Fill = Brushes.Red;
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body)+(body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar,Canvas.GetTop(body)-2);
            
        }
        public void damageTaken(int dmg)
        {
            healthPoints -= dmg;
            if(healthPoints>0)
            {
                double width = (healthPoints / maxHealthPoints) * (body.Width * 4) / 5;
                monsterHpBar.Width = width;
            }
            else
            {
                dead = true;
                monsterHpBar.Width = 0;
            }
        }
        public bool IsDead()
        {
            return dead;
        }
        public System.Windows.Shapes.Rectangle getBody()
        {
            return body;
        }
        public int expOnDeath()
        {
            return expGiven;
        }
            
            
    }
    internal class Golem : Monster
    {
        int hitboxTicks=0;
        public Golem(Canvas canv, int x, int y)
        {
            expGiven = 500;
            attackTicks = 0;
            animations = 7;
            currentAnimation = 0;
            attackRange = 100;
            Speed = 80;
            baseSpeed = 80;
            healthPoints = 130 * diffMulti;
            maxHealthPoints = healthPoints;
            body.Height = 144;
            body.Width = 128;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            minDmg = Convert.ToInt32(18 * diffMulti);
            maxDmg = Convert.ToInt32(36 * diffMulti);
            weapon.Height = 40;
            weapon.Width = 40;
            weapon.Fill = Brushes.Transparent;
            Canvas.SetZIndex(weapon, 0);
            BelongTO = canv;
            body.SetValue(Canvas.TopProperty, (double)y);
            body.SetValue(Canvas.LeftProperty, (double)x);
            loadImages();
            monsterSprite.ImageSource = monsterMovementRight[0];
            body.Fill = monsterSprite;
            hpBar();



        }
        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[7];
            monsterMovementLeft = new BitmapImage[7];
            monsterAttackRight = new BitmapImage[7];
            monsterAttackLeft = new BitmapImage[7];
            attackHitBoxLeft = new BitmapImage[3];
            attackHitBoxRight = new BitmapImage[3];
            BitmapImage golemSpriteAttack = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/golemAttack.png", UriKind.Absolute));
            BitmapImage golemSpriteMovement = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/golemMovement.png", UriKind.Absolute));
            int spriteWidth = 64;
            int spriteHeight = 72;
            

            for (int i = 0; i <= golemSpriteAttack.Height; i += spriteHeight )
            {

                int animation = 0;
                for (int j = 0; j <= golemSpriteAttack.Width; j += spriteWidth)
                {


                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(golemSpriteAttack, spriteRect);
                    CroppedBitmap croppedBitmapM = new CroppedBitmap(golemSpriteMovement, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);
                    MemoryStream stream2 = new MemoryStream();
                    PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                    encoder2.Frames.Add(BitmapFrame.Create(croppedBitmapM));
                    encoder2.Save(stream2);
                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();
                    BitmapImage sprite2 = new BitmapImage();
                    sprite2.BeginInit();
                    sprite2.CacheOption = BitmapCacheOption.OnLoad;
                    sprite2.StreamSource = stream2;
                    sprite2.EndInit();


                    if (i > 0)
                        {
                            monsterMovementRight[animation] = sprite2;
                        }
                        else
                        {
                            monsterMovementLeft[animation] = sprite2;
                        }
                    
                    
                        if (i > 0)
                        {
                            monsterAttackRight[animation] = sprite;
                        }
                        else
                        {
                            monsterAttackLeft[animation] = sprite;
                        }
                    animation++;
                   
                }
            }

            // Ładowanie klatek do animacji ataku

            for (int i = 0; i < 3; i++)
            {
                attackHitBoxLeft[i] = new BitmapImage();
                attackHitBoxLeft[i].BeginInit();
                attackHitBoxLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/golAtt{1 + i}.png", UriKind.Absolute);
                attackHitBoxLeft[i].EndInit();
                attackHitBoxRight[i] = new BitmapImage();
                attackHitBoxRight[i].BeginInit();
                attackHitBoxRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/golAtt{1 + i}.png", UriKind.Absolute);
                attackHitBoxRight[i].EndInit();




            }

        }
        private void Smash(System.Windows.Shapes.Rectangle player, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization,int stage)
        {
            int min;
            int max;
            if(stage==1)
            {
                min = minDmg;
                max = maxDmg;
            }
            else if (stage == 2)
            {
                min = Convert.ToInt16(minDmg*2/3);
                max = Convert.ToInt16(maxDmg * 2/3);
            }
            else if (stage == 3)
            {
                min = Convert.ToInt16(minDmg * 1 / 3);
                max = Convert.ToInt16(maxDmg * 1 / 3);
            }
            else
            {
                min = minDmg;
                max = maxDmg;
            }
            
            Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
            Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
            {

                int obecnyDmg = Convert.ToInt32(dmg.Text);
                int dealtDamage = rnd.Next(min, max + 1);
                obecnyDmg += dealtDamage;
                dmg.Text = obecnyDmg.ToString();
                dmg.Width = Convert.ToInt16(dmg.Text.Length) * 20;
                dmg.Opacity = 100;
                Canvas.SetLeft(dmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (dmg.Width / 2));
                Canvas.SetTop(dmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - dmg.Height);
                hp -= dealtDamage;
                if (hp <= 0)
                {
                    isDead = true;
                    killedBy = "Golem";
                    lastDamage = dealtDamage;
                }
                hpVisualization.Text = hp + "/" + maxHp;
                double w = Convert.ToDouble(hp) / Convert.ToDouble(maxHp) * 200;
                if (w < 0) w = 0;
                hpBar.Width = Convert.ToInt32(w);


            }
        }
        private void attack(System.Windows.Shapes.Rectangle player, double delta, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (attackTicks == 7&&attackTimer/200>1)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                hitboxTicks = 0;
                weapon.Fill = Brushes.Transparent;
                return;
            }
            attackTimer += delta * 1000;
            if (attackTicks == 0 && attackTimer / 150 > 1)
            {

                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                   
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    
                    


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 1 && attackTimer / 150 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 2 && attackTimer / 150 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 3 && attackTimer / 150 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    

                }
                body.Fill = monsterSprite;
                attackTicks++;
                
                attackTimer = 0;
                return;
            }
            if (attackTicks == 4 && attackTimer / 300 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 50;
                    weapon.Width = 80;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4);
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[hitboxTicks];
                    weapon.Fill = weaponSprite;
                    
                }
                else
                {
                    weapon.Height = 50;
                    weapon.Width = 80;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4);
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[hitboxTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                hitboxTicks++;
                attackTimer = 0;
                Smash(player, dmg, hpBar, ref hp, ref maxHp, hpVisualization,1);
                return;
            }
            if (attackTicks == 5 && attackTimer / 300 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 100;
                    weapon.Width = 160;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 - 25);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 25);
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[hitboxTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    weapon.Height = 100;
                    weapon.Width = 160;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 25);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 25);
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[hitboxTicks];
                    weapon.Fill = weaponSprite;

                }
               
                body.Fill = monsterSprite;
                attackTicks++;
                hitboxTicks++;
                attackTimer = 0;
                Smash(player, dmg, hpBar, ref hp, ref maxHp, hpVisualization,2);
                return;
            }
            if(attackTicks==6 &&attackTimer/ 300 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 150;
                    weapon.Width = 240;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 - 50);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*3/4  - 50);
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[hitboxTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    weapon.Height = 150;
                    weapon.Width = 240;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 50);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 50);
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[hitboxTicks];
                    weapon.Fill = weaponSprite;

                }

                body.Fill = monsterSprite;
                attackTicks++;
                hitboxTicks++;
                attackTimer = 0;
                Smash(player, dmg, hpBar, ref hp, ref maxHp, hpVisualization,3);
                
                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name)+(name.Width/2) , Canvas.GetTop(name));
            if (prepareToAttack)
            {
                attack(name, delta, dmg, hpBar, ref hp, ref maxHp, hpVisualization);
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
            if (playerCenter.Y > Canvas.GetTop(body) + body.Height)
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
                    monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                    body.Fill = monsterSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                    body.Fill = monsterSprite;
                }
            }

            if (!tryAttack)
                checkCollisions(ref moveMonsterByX, ref moveMonsterByY, friction, playerCenter, delta);
            else
            {
                attackTicks = 0;
                attackTimer = 0;
                prepareToAttack = true;
                return;
            }
            if ((moveMonsterByY != 0 || moveMonsterByX != 0) && ticks >= 10 / Speed)
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
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX < 0)
                {
                    if (!moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX == 0)
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }

            }


            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            if (Canvas.GetTop(body) >= 600 - body.Height) Canvas.SetTop(body, 600 - body.Height);
            if (Canvas.GetTop(body) <= 93 - (body.Height * 3 / 4)) Canvas.SetTop(body, 93 - (body.Height * 3 / 4));
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body) + (body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar, Canvas.GetTop(body) - 15);



        }


    }
    internal class Goblin:Monster
    {

        
        public Goblin(Canvas canv,int x, int y)
        {
            expGiven = 150;
            attackTicks = 0;
            animations = 5;
            currentAnimation = 0;
            attackRange = 50;
            Speed = 130;
            baseSpeed = 130;
            healthPoints = 100*diffMulti;
            maxHealthPoints = healthPoints;
            body.Height = 64;
            body.Width = 64;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            minDmg = Convert.ToInt32(5 * diffMulti);
            maxDmg = Convert.ToInt32(15 * diffMulti);
            weapon.Height = 30;
            weapon.Width = 50;
            weapon.Fill = Brushes.Transparent;
            Canvas.SetZIndex(weapon, 15);
            BelongTO = canv;
            body.SetValue(Canvas.TopProperty, (double)y);
            body.SetValue(Canvas.LeftProperty, (double)x);
            loadImages();
            monsterSprite.ImageSource = monsterMovementRight[0];
            body.Fill = monsterSprite;
            hpBar();



        }
      
        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[5];
            monsterMovementLeft = new BitmapImage[5];
            monsterAttackRight = new BitmapImage[6];
            monsterAttackLeft = new BitmapImage[6];
            attackHitBoxLeft = new BitmapImage[6];
            attackHitBoxRight = new BitmapImage[6];
            BitmapImage goblinSprits = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/goblinsword.png", UriKind.Absolute));
            int spriteWidth = 64;
            int spriteHeight = 64;
            int howManyAnimations;
            
            for (int i = spriteHeight; i < goblinSprits.Height; i += spriteHeight * 2)
            {
                howManyAnimations = 0;
                
                for (int j = 0; j < goblinSprits.Width; j += spriteWidth)
                {
                    
                    
                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(goblinSprits, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);
                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();
                    if (howManyAnimations < 5) // 5 for movement 
                    {
                        if (i <= spriteWidth)
                        {
                            monsterMovementRight[howManyAnimations] = sprite;
                        }
                        else
                        {
                            monsterMovementLeft[howManyAnimations] = sprite;
                        }
                    }
                    else // 6 for attack
                    {
                        if (i <= spriteWidth)
                        {
                            monsterAttackRight[howManyAnimations - 5] = sprite;
                        }
                        else
                        {
                            monsterAttackLeft[howManyAnimations - 5] = sprite;
                        }

                    }
                    howManyAnimations++;
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



        
        

        private void attack(System.Windows.Shapes.Rectangle player, double delta,TextBox dmg,System.Windows.Shapes.Rectangle hpBar,ref int hp,ref int maxHp,TextBox hpVisualization)
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
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body)+body.Width*2/3);
                    Canvas.SetTop(weapon, Canvas.GetTop(body)+body.Height/3);
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body)-body.Width/4);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3);
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 1 && attackTimer / 100 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 2&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 3&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 4&&attackTimer/100>1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[3];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 5 && attackTimer / 100 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {
                    
                        int obecnyDmg = Convert.ToInt32(dmg.Text);
                        int dealtDamage= rnd.Next(minDmg, maxDmg + 1);
                        obecnyDmg += dealtDamage;
                        dmg.Text = obecnyDmg.ToString();
                        dmg.Width = Convert.ToInt16(dmg.Text.Length) * 20;
                        dmg.Opacity = 100;
                        Canvas.SetLeft(dmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (dmg.Width / 2));
                        Canvas.SetTop(dmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - dmg.Height);
                         hp -= dealtDamage;
                            if (hp <= 0)
                        {
                        isDead = true;
                        killedBy = "Goblin";
                        lastDamage = dealtDamage;
                    }
                    hpVisualization.Text = hp + "/" + maxHp;
                        double w = Convert.ToDouble(hp) / Convert.ToDouble(maxHp) * 200;
                        if (w < 0) w = 0;
                        hpBar.Width = Convert.ToInt32(w);
                   
                    
                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction,TextBox dmg, System.Windows.Shapes.Rectangle hpBar,ref int hp,ref int maxHp,TextBox hpVisualization)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            
            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta,dmg,hpBar,ref hp,ref maxHp, hpVisualization);
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
                    monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                    body.Fill = monsterSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                    body.Fill = monsterSprite;
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
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX < 0)
                {
                    if (!moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX == 0 )
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
               
            }
            
            
            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            if (Canvas.GetTop(body) >= 600 - body.Height) Canvas.SetTop(body, 600 - body.Height);
            if (Canvas.GetTop(body) <= 93-(body.Height*3/4))Canvas.SetTop(body, 93 - (body.Height * 3 / 4));
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body) + (body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar, Canvas.GetTop(body) - 15);



        }
        
        
       
    }
    internal class Imp : Monster
    {
        int howManyTimesDidTryToAttack = 0;
       
        public Imp(Canvas canv, int x, int y)
        {
            expGiven = 300;
            attackTicks = 0;
            animations = 4;
            currentAnimation = 0;
            attackRange = 100;
            Speed = 100;
            baseSpeed = 100;
            healthPoints = 120 * diffMulti;
            maxHealthPoints = healthPoints;
            body.Height = 72;
            body.Width = 80;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            minDmg = Convert.ToInt32(3 * diffMulti);
            maxDmg = Convert.ToInt32(6 * diffMulti);
            weapon.Height = 20;
            weapon.Width = 54;
            weapon.Fill = Brushes.Transparent;
            Canvas.SetZIndex(weapon, 15);
            BelongTO = canv;
            body.SetValue(Canvas.TopProperty, (double)y);
            body.SetValue(Canvas.LeftProperty, (double)x);
            loadImages();
            monsterSprite.ImageSource = monsterMovementRight[0];
            body.Fill = monsterSprite;
            hpBar();



        }

        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[4];
            monsterMovementLeft = new BitmapImage[4];
            monsterAttackRight = new BitmapImage[4];
            monsterAttackLeft = new BitmapImage[4];
            attackHitBoxLeft = new BitmapImage[6]; 
            attackHitBoxRight = new BitmapImage[6];
            BitmapImage impSprites = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/impMovement.png", UriKind.Absolute));
            BitmapImage impSpritesA = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/impAttack.png", UriKind.Absolute));
            int spriteWidth = 80;
            int spriteHeight = 68;

            int animation;
            for (int i = 0; i < impSprites.Height; i += spriteHeight )
            {

                animation = 0;
                for (int j = 0; j < impSprites.Width; j += spriteWidth)
                {


                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(impSprites, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    MemoryStream stream2 = new MemoryStream();
                    PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);
                    BitmapImage sprite = new BitmapImage();
                    
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();
                    croppedBitmap = new CroppedBitmap(impSpritesA, spriteRect);
                    encoder2.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder2.Save(stream2);
                    BitmapImage sprite2 = new BitmapImage();
                    sprite2.BeginInit();
                    sprite2.CacheOption = BitmapCacheOption.OnLoad;
                    sprite2.StreamSource = stream2;
                    sprite2.EndInit();

                    if (i==0)
                        {
                            monsterMovementRight[animation] = sprite;
                            monsterAttackRight[animation] = sprite2;
                        }
                        else
                        {
                            monsterMovementLeft[animation] = sprite;
                                 monsterAttackLeft[animation] = sprite2;
                          }


                    animation++;
                    
                }
            }

            // Ładowanie klatek do animacji ataku

            for (int i = 0; i < 6; i++)
            {
                attackHitBoxLeft[i] = new BitmapImage();
                attackHitBoxLeft[i].BeginInit();
                attackHitBoxLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/impAtt{1 + i}l.png", UriKind.Absolute);
                attackHitBoxLeft[i].EndInit();
                attackHitBoxRight[i] = new BitmapImage();
                attackHitBoxRight[i].BeginInit();
                attackHitBoxRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/impAtt{1 + i}.png", UriKind.Absolute);
                attackHitBoxRight[i].EndInit();




            }

        }






        private void attack(System.Windows.Shapes.Rectangle player, double delta, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (attackTicks == 4&&attackTimer/200>1)
            {
                weapon.Height = 20;
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                weapon.Fill = Brushes.Transparent;
                howManyTimesDidTryToAttack = 0;
                return;
            }
            attackTimer += delta * 1000;
            if (attackTicks == 0 && attackTimer / 100 > 1)
            {

                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                    
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    
                    

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 1 && attackTimer / 100 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                   

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 2 && attackTimer / 100 > 1)
            {
                if (howManyTimesDidTryToAttack == 2) weapon.Height = 8;
                if (moveInRightDirection)
                {
                    if (howManyTimesDidTryToAttack == 0)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 6 / 10);
                    }
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[howManyTimesDidTryToAttack];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    if (howManyTimesDidTryToAttack == 0)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*3 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*6/10);
                    }
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[howManyTimesDidTryToAttack];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 3 && attackTimer / 100 > 1)
            {
                
                if (moveInRightDirection)
                {
                    if (howManyTimesDidTryToAttack == 0)
                    { 
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*3/5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2/5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3+10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*6/10);
                    }
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[howManyTimesDidTryToAttack+3];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    if (howManyTimesDidTryToAttack == 0)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*3 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height*6/10);
                    }
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[howManyTimesDidTryToAttack+3];
                    weapon.Fill = weaponSprite;

                }
                Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {

                    int obecnyDmg = Convert.ToInt32(dmg.Text);
                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    obecnyDmg += dealtDamage;
                    dmg.Text = obecnyDmg.ToString();
                    dmg.Width = Convert.ToInt16(dmg.Text.Length) * 20;
                    dmg.Opacity = 100;
                    Canvas.SetLeft(dmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (dmg.Width / 2));
                    Canvas.SetTop(dmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - dmg.Height);
                    hp -= dealtDamage;
                    if (hp <= 0)
                    {
                        isDead = true;
                        killedBy = "Imp";
                        lastDamage = dealtDamage;
                    }
                    hpVisualization.Text = hp + "/" + maxHp;
                    double w = Convert.ToDouble(hp) / Convert.ToDouble(maxHp) * 200;
                    if (w < 0) w = 0;
                    hpBar.Width = Convert.ToInt32(w);


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                howManyTimesDidTryToAttack++;
                
                if (howManyTimesDidTryToAttack < 3)
                {
                    attackTicks = 2;
                    return;
                }
                return;
            }
            


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta, dmg, hpBar, ref hp, ref maxHp, hpVisualization);
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
            if (playerCenter.Y > Canvas.GetTop(body) + body.Height)
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
                    monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                    body.Fill = monsterSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                    body.Fill = monsterSprite;
                }
            }

            if (!tryAttack)
                checkCollisions(ref moveMonsterByX, ref moveMonsterByY, friction, playerCenter, delta);
            else
            {
                attackTicks = 0;
                attackTimer = 0;
                prepareToAttack = true;
                return;
            }
            if ((moveMonsterByY != 0 || moveMonsterByX != 0) && ticks >= 10 / Speed)
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
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX < 0)
                {
                    if (!moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX == 0)
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }

            }


            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            if (Canvas.GetTop(body) >= 600 - body.Height) Canvas.SetTop(body, 600 - body.Height);
            if (Canvas.GetTop(body) <= 93 - (body.Height * 3 / 4)) Canvas.SetTop(body, 93 - (body.Height * 3 / 4));
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body) + (body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar, Canvas.GetTop(body) - 15);



        }
    }
    internal class Spider:Monster
    {
        private int poisonDot;
        private double dotDuration;
        public Spider(Canvas canv, int x, int y)
        {
            expGiven = 300;
            attackTicks = 0;
            animations = 6;
            currentAnimation = 0;
            attackRange = 30;
            Speed = 300;
            baseSpeed = 300;
            healthPoints = 115 * diffMulti;
            maxHealthPoints = healthPoints;
            body.Height = 50;
            body.Width = 64;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            minDmg = Convert.ToInt32(3 * diffMulti);
            maxDmg = Convert.ToInt32(7 * diffMulti);
            poisonDot= Convert.ToInt32(5 * diffMulti);     
            dotDuration = 4000; // 4 sec <- 10dmg per sec
            weapon.Height = 20;
            weapon.Width = 30;
            weapon.Fill = Brushes.Transparent;
            Canvas.SetZIndex(weapon, 15);
            BelongTO = canv;
            body.SetValue(Canvas.TopProperty, (double)y);
            body.SetValue(Canvas.LeftProperty, (double)x);
            loadImages();
            monsterSprite.ImageSource = monsterMovementRight[0];
            body.Fill = monsterSprite;
            hpBar();
        }
        new public void setDiff(double plus)
        {
            diffMulti += plus;
            minDmg = Convert.ToInt32(minDmg * diffMulti);
            maxDmg = Convert.ToInt32(maxDmg * diffMulti);
            poisonDot = Convert.ToInt32(poisonDot * diffMulti);
            healthPoints = healthPoints * diffMulti;
            maxHealthPoints = maxHealthPoints * diffMulti;
        }
        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[6];
            monsterMovementLeft = new BitmapImage[6];
            monsterAttackRight = new BitmapImage[4];
            monsterAttackLeft = new BitmapImage[4];
            attackHitBoxLeft = new BitmapImage[4];
            attackHitBoxRight = new BitmapImage[4];
            BitmapImage spiderSprite = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/MonsterSprites/spiderFullAnimation.png", UriKind.Absolute));
            // 1-4 attack 5-10 movement
            int spriteWidth = 64;
            int spriteHeight = 50;

            int breakPoint = 4;
            for (int i = 0; i < spiderSprite.Height; i += spriteHeight)
            {

                int animation = 0;
                for (int j = 0; j < spiderSprite.Width; j += spriteWidth)
                {


                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(spiderSprite, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);
                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();

                    



                    if (i > 0)
                    {
                        if (animation < 4)monsterAttackRight[animation] = sprite;
                        else monsterMovementRight[animation-breakPoint] = sprite;
                    }
                    else
                    {
                        if(animation<4) monsterAttackLeft[animation] = sprite;
                        else monsterMovementLeft[animation- breakPoint] = sprite;
                    }


                    
                    animation++;

                }
            }

            // Ładowanie klatek do animacji ataku

            for (int i = 0; i < 4; i++)
            {
                attackHitBoxLeft[i] = new BitmapImage();
                attackHitBoxLeft[i].BeginInit();
                attackHitBoxLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/spiAtt{1 + i}l.png", UriKind.Absolute);
                attackHitBoxLeft[i].EndInit();
                attackHitBoxRight[i] = new BitmapImage();
                attackHitBoxRight[i].BeginInit();
                attackHitBoxRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/attAnimations/spiAtt{1 + i}.png", UriKind.Absolute);
                attackHitBoxRight[i].EndInit();




            }

        }
        private void attack(System.Windows.Shapes.Rectangle player, double delta, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (attackTicks == 4&&attackTimer/20>1)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                weapon.Fill = Brushes.Transparent;
                return;
            }
            attackTimer += delta * 1000;
            if (attackTicks == 0 && attackTimer / 400 > 1)
            {

                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3+10);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3 - 7);
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4+10);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3);
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 1 && attackTimer / 40 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 2 && attackTimer / 40 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }
            if (attackTicks == 3 && attackTimer / 40 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[attackTicks];
                    weapon.Fill = weaponSprite;

                }
                Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {

                    int obecnyDmg = Convert.ToInt32(dmg.Text);
                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    obecnyDmg += dealtDamage;
                    dmg.Text = obecnyDmg.ToString();
                    dmg.Width = Convert.ToInt16(dmg.Text.Length) * 20;
                    dmg.Opacity = 100;
                    Canvas.SetLeft(dmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (dmg.Width / 2));
                    Canvas.SetTop(dmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - dmg.Height);
                    hp -= dealtDamage;
                    if (hp <= 0)
                    {
                        isDead = true;
                        killedBy = "Spider";
                        lastDamage = dealtDamage;
                    }
                    hpVisualization.Text = hp + "/" + maxHp;
                    double w = Convert.ToDouble(hp) / Convert.ToDouble(maxHp) * 200;
                    if (w < 0) w = 0;
                    hpBar.Width = Convert.ToInt32(w);
                    Monster.damageOverTime.Add(new Tuple<int,double>(poisonDot, dotDuration));


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, TextBox dmg, System.Windows.Shapes.Rectangle hpBar, ref int hp, ref int maxHp, TextBox hpVisualization)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta, dmg, hpBar, ref hp, ref maxHp, hpVisualization);
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
            if (playerCenter.Y > Canvas.GetTop(body) + body.Height)
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
                    monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                    body.Fill = monsterSprite;
                }
                if (playerCenter.X <= Canvas.GetLeft(body) + body.Width / 2)
                {
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                    body.Fill = monsterSprite;
                }
            }

            if (!tryAttack)
                checkCollisions(ref moveMonsterByX, ref moveMonsterByY, friction, playerCenter, delta);
            else
            {
                attackTicks = 0;
                attackTimer = 0;
                prepareToAttack = true;
                return;
            }
            if ((moveMonsterByY != 0 || moveMonsterByX != 0) && ticks >= 10 / Speed)
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
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX < 0)
                {
                    if (!moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }
                else if (moveMonsterByX == 0)
                {
                    if (moveInRightDirection)
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        currentAnimation++;
                        if (currentAnimation == animations) currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                }

            }


            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            if (Canvas.GetTop(body) >= 600 - body.Height) Canvas.SetTop(body, 600 - body.Height);
            if (Canvas.GetTop(body) <= 93 - (body.Height * 3 / 4)) Canvas.SetTop(body, 93 - (body.Height * 3 / 4));
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body) + (body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar, Canvas.GetTop(body) - 15);



        }
    }
}


