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
//using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace BasicsOfGame
{
    public class Monster
    {
        protected List<Tuple<double, double, double, double, string>> DamagePerMilliseconds = new List<Tuple<double, double, double, double, string>>();
        protected string nameOfMonster;
        protected int expGiven;
        protected System.Windows.Shapes.Rectangle body = new System.Windows.Shapes.Rectangle();
        protected System.Windows.Shapes.Rectangle weapon = new System.Windows.Shapes.Rectangle();
        protected int savedDirectionX = 0;
        protected int savedDirectionY = 0;
        protected int attackRange;
        protected double Speed;
        protected double baseSpeed;
        protected bool inCollision = false;
        protected double timer = 0;
        protected bool gettingOut = false;
        protected bool prepareToAttack = false;
        protected bool ignited;
        protected bool stunned;
        protected bool chilled;
        protected bool shocked;
        protected int attackTicks;
        protected double attackTimer = 0;
        protected Canvas BelongTO;
        protected double ticks = 0;
        protected double speedMultiplier = 1;
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
        protected int minDmg;
        protected int maxDmg;
        protected int animations;
        protected int currentAnimation;
        protected double diffMulti = 1.0;
        protected double healthPoints;
        protected double maxHealthPoints;
        protected bool dead = false;
        public static List<Tuple<int, Double, string>> damageOverTime = new List<Tuple<int, Double, string>>();
        static public Action deadToDot;

        public static void update(List<Tuple<int, Double, string>> listOfDots)
        {
            foreach (var x in damageOverTime)
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
                    foreach (Monster g in BelongTO.Children.OfType<Monster>())
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
                    foreach (Monster g in BelongTO.Children.OfType<Monster>())
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
                if (!axisX && (left || right)) coordinateX = 0;// nie możesz ruszać się po X 
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
        public virtual void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg) { }
        protected void setRelativeVisibility()
        {
            Canvas.SetZIndex(body, Convert.ToInt32((Canvas.GetTop(body) + body.Height) / 100) - 1);

        }

        protected void NormalizeSpeed(double delta)
        {
            ticks += baseSpeed / 2 * delta;
            Speed = baseSpeed * delta * speedMultiplier;
        }
        public virtual void add()
        {
            BelongTO.Children.Add(weapon);
            BelongTO.Children.Add(body);
            BelongTO.Children.Add(monsterHpBar);
        }
        public virtual void remove()
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
            maxHealthPoints = maxHealthPoints * diffMulti;
        }
        protected virtual void hpBar()
        {
            monsterHpBar = new System.Windows.Shapes.Rectangle();
            monsterHpBar.Width = (body.Width * 4) / 5;
            monsterHpBar.Height = 8;
            monsterHpBar.Fill = Brushes.Red;
            Canvas.SetLeft(monsterHpBar, Canvas.GetLeft(body) + (body.Width * 1) / 10);
            Canvas.SetTop(monsterHpBar, Canvas.GetTop(body) - 2);

        }
        public void addDot(double dmg, double timeInMs, string name)
        {
            double dmgPerMs = dmg / timeInMs;
            if (name == "Ignite")
            {
                if (!ignited)
                {
                    ignited = true;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, timeInMs, name));

                }
                else
                {
                    for (int i = 0; i < DamagePerMilliseconds.Count - 1; i++)
                    {
                        if (DamagePerMilliseconds[i].Item5 == "Ignite")
                        {
                            DamagePerMilliseconds[i] = new Tuple<double, double, double, double, string>(DamagePerMilliseconds[i].Item1, DamagePerMilliseconds[i].Item2, DamagePerMilliseconds[i].Item3, DamagePerMilliseconds[i].Item4 + timeInMs, DamagePerMilliseconds[i].Item5);
                        }
                    }
                }

            }
            else if (name == "Stun")
            {
                if (!stunned)
                {
                    stunned = true;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, timeInMs, name));
                }

            }


            else if (name == "Chill")
            {
                if (!chilled)
                {
                    chilled = true;
                    speedMultiplier = 0.6;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, timeInMs, name));

                }
                else
                {
                    for (int i = 0; i < DamagePerMilliseconds.Count - 1; i++)
                    {
                        if (DamagePerMilliseconds[i].Item5 == "Chill")
                        {
                            DamagePerMilliseconds[i] = new Tuple<double, double, double, double, string>(DamagePerMilliseconds[i].Item1, DamagePerMilliseconds[i].Item2, DamagePerMilliseconds[i].Item3, DamagePerMilliseconds[i].Item4 + timeInMs, DamagePerMilliseconds[i].Item5);
                        }

                    }
                }
            }
            else if (name == "Shock")
            {
                if (!shocked)
                {
                    shocked = true;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, timeInMs, name));

                }
                else
                {
                    for (int i = 0; i < DamagePerMilliseconds.Count - 1; i++)
                    {
                        if (DamagePerMilliseconds[i].Item5 == "Shock")
                        {
                            DamagePerMilliseconds[i] = new Tuple<double, double, double, double, string>(DamagePerMilliseconds[i].Item1, DamagePerMilliseconds[i].Item2, DamagePerMilliseconds[i].Item3, DamagePerMilliseconds[i].Item4 + timeInMs, DamagePerMilliseconds[i].Item5);
                        }

                    }
                }
            }
            else DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, timeInMs, name));
        }

        protected void dotUpdate(double deltaTime)
        {

            if (DamagePerMilliseconds.Count > 0)
            {

                List<Tuple<double, double, double, double, string>> toRemove = new List<Tuple<double, double, double, double, string>>();

                for (int i = 0; i < DamagePerMilliseconds.Count; i++)
                {

                    double currentDmg = DamagePerMilliseconds[i].Item2 + DamagePerMilliseconds[i].Item1 * deltaTime * 1000;
                    if (currentDmg >= 1)
                    {
                        int substractMe = Convert.ToInt32(currentDmg);
                        dotDamageTaken(substractMe);
                        currentDmg -= substractMe;

                    }
                    double dmgPerMs = DamagePerMilliseconds[i].Item1;
                    double timeElapsed = DamagePerMilliseconds[i].Item3 + deltaTime * 1000;
                    double maxTime = DamagePerMilliseconds[i].Item4;
                    string nameOfDot = DamagePerMilliseconds[i].Item5;
                    DamagePerMilliseconds[i] = new Tuple<double, double, double, double, string>(dmgPerMs, currentDmg, timeElapsed, maxTime, nameOfDot);
                    if (maxTime <= timeElapsed) toRemove.Add(DamagePerMilliseconds[i]);

                }

                foreach (var x in toRemove)
                {

                    if (x.Item5 == "Ignite")
                    {
                        ignited = false;

                    }
                    if (x.Item5 == "Stun")
                    {
                        stunned = false;

                    }
                    if (x.Item5 == "Chill")
                    {
                        speedMultiplier = 1;
                        chilled = false;
                    }
                    if (x.Item5 == "Shock") shocked = false;
                    DamagePerMilliseconds.Remove(x);

                }




            }


        }
        public virtual void damageTaken(ref int dmg)
        {
            if (shocked)
            {
                dmg = Convert.ToInt32(dmg * 1.5);

            }
            healthPoints -= dmg;
            if (healthPoints > 0)
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
        public virtual void dotDamageTaken(int dmg)
        {
            if (shocked) dmg = Convert.ToInt32(dmg * 1.5);
            healthPoints -= dmg;
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * (body.Width * 4) / 5;
                monsterHpBar.Width = width;
            }
            else
            {
                deadToDot.Invoke();
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
        int hitboxTicks = 0;
        public Golem(Canvas canv, int x, int y)
        {
            nameOfMonster = "Golem";
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


            for (int i = 0; i <= golemSpriteAttack.Height; i += spriteHeight)
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

        private void Smash(System.Windows.Shapes.Rectangle player, Action<int, string> dealDmg, int stage)
        {
            int min;
            int max;
            double stunDuration = 600;

            if (stage == 1)
            {
                min = minDmg;
                max = maxDmg;

            }
            else if (stage == 2)
            {
                min = Convert.ToInt16(minDmg * 3 / 4);
                max = Convert.ToInt16(maxDmg * 3 / 4);

            }
            else if (stage == 3)
            {
                min = Convert.ToInt16(minDmg * 1 / 2);
                max = Convert.ToInt16(maxDmg * 1 / 2);

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


                int dealtDamage = rnd.Next(min, max + 1);
                if (ignited)
                {
                    dealtDamage = Convert.ToInt32(dealtDamage * 0.8);

                }
                dealDmg(dealtDamage, nameOfMonster);
                string statusEffect = "Stun";
                Monster.damageOverTime.Add(new Tuple<int, double, string>(0, stunDuration, statusEffect));


            }
        }
        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 7 && attackTimer / 200 > 1)
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
                Smash(player, dealDmg, 1);
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
                Smash(player, dealDmg, 2);
                return;
            }
            if (attackTicks == 6 && attackTimer / 300 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 150;
                    weapon.Width = 240;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 - 50);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 50);
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
                Smash(player, dealDmg, 3);

                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            dotUpdate(delta);
            bool tryAttack = true;

            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name));
            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
    internal class Goblin : Monster
    {


        public Goblin(Canvas canv, int x, int y)
        {
            nameOfMonster = "Goblin";
            expGiven = 150;
            attackTicks = 0;
            animations = 5;
            currentAnimation = 0;
            attackRange = 50;
            Speed = 130;
            baseSpeed = 130;
            healthPoints = 100 * diffMulti;
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






        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
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
            if (attackTicks == 0 && attackTimer / 100 > 1)
            {

                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3);
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4);
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
            if (attackTicks == 2 && attackTimer / 100 > 1)
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
            if (attackTicks == 3 && attackTimer / 100 > 1)
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
            if (attackTicks == 4 && attackTimer / 100 > 1)
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


                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    if (ignited)
                    {
                        dealtDamage = Convert.ToInt32(dealtDamage * 0.8);
                    }
                    dealDmg(dealtDamage, nameOfMonster);


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)

        {
            if (delta > 1) return; // Starting delta value is about 3 billions 


            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();
            dotUpdate(delta);
            if (stunned)
            {
                prepareToAttack = false;
                return;

            }
            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
    internal class Imp : Monster
    {
        int howManyTimesDidTryToAttack = 0;
        double dotDuration;
        int igniteDot;
        public Imp(Canvas canv, int x, int y)
        {
            nameOfMonster = "Imp";
            igniteDot = Convert.ToInt32(2 * diffMulti);
            dotDuration = 2000;
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
            for (int i = 0; i < impSprites.Height; i += spriteHeight)
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

                    if (i == 0)
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






        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 4 && attackTimer / 200 > 1)
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
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 6 / 10);
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
                    weaponSprite.ImageSource = attackHitBoxRight[howManyTimesDidTryToAttack + 3];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    if (howManyTimesDidTryToAttack == 0)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 1)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 2 / 5 + 5);
                    }
                    if (howManyTimesDidTryToAttack == 2)
                    {
                        Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 - 10);
                        Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 6 / 10);
                    }
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    weaponSprite.ImageSource = attackHitBoxLeft[howManyTimesDidTryToAttack + 3];
                    weapon.Fill = weaponSprite;

                }
                Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {


                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    if (ignited) dealtDamage = Convert.ToInt32(dealtDamage * 0.8);
                    dealDmg(dealtDamage, nameOfMonster);
                    string dotName = "Ignite";
                    Monster.damageOverTime.Add(new Tuple<int, double, string>(igniteDot, dotDuration, dotName));


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
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();
            dotUpdate(delta);
            if (stunned)
            {
                prepareToAttack = false;
                return;

            }
            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
    internal class Spider : Monster
    {
        private int poisonDot;
        private double dotDuration;
        public Spider(Canvas canv, int x, int y)
        {
            nameOfMonster = "Spider";
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
            poisonDot = Convert.ToInt32(5 * diffMulti);
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
                        if (animation < 4) monsterAttackRight[animation] = sprite;
                        else monsterMovementRight[animation - breakPoint] = sprite;
                    }
                    else
                    {
                        if (animation < 4) monsterAttackLeft[animation] = sprite;
                        else monsterMovementLeft[animation - breakPoint] = sprite;
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
        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 4 && attackTimer / 20 > 1)
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
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 + 10);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height / 3 - 7);
                    weaponSprite.ImageSource = attackHitBoxRight[attackTicks];
                    weapon.Fill = weaponSprite;
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) - body.Width / 4 + 10);
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


                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    if (ignited) dealtDamage = Convert.ToInt32(dealtDamage * 0.8);
                    dealDmg(dealtDamage, nameOfMonster);
                    string dotName = "Poison";
                    Monster.damageOverTime.Add(new Tuple<int, double, string>(poisonDot, dotDuration, dotName));


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)

        {
            if (delta > 1) return; // Starting delta value is about 3 billions 

            NormalizeSpeed(delta);
            bool tryAttack = true;
            setRelativeVisibility();
            dotUpdate(delta);
            if (stunned)
            {
                prepareToAttack = false;
                return;

            }
            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name) + (name.Height / 2));
            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
    internal class oldGreatOne : Monster
    {

        int hitboxTicks = 0;
        double beamCooldown;
        double dashCooldown;
        double tentacleCooldown;
        double currentDashCooldown;
        double currentBeamCooldown;
        double currenctTentacleCooldown;
        TextBox nameHolder;
        System.Windows.Shapes.Rectangle background;
        string currentlyUsing;
        private double gracePeriod = 0.2;
        private bool usingSkill = false;
        private double timerForSkills;
        BitmapImage[] tentacleSprite;
        public oldGreatOne(Canvas canv, int x, int y)
        {
            timerForSkills = 0;
            nameOfMonster = "Sehn, Harbringer \nof Madness";
            beamCooldown = 10;
            dashCooldown = 10;
            tentacleCooldown = 13;
            currentBeamCooldown = 0;
            currentDashCooldown = 0;
            currenctTentacleCooldown = 0;
            expGiven = 2500;
            attackTicks = 0;
            animations = 8;
            currentAnimation = 0;
            attackRange = 100;
            Speed = 200;
            baseSpeed = 200;
            healthPoints = 2000;
            maxHealthPoints = healthPoints;
            body.Height = 132;
            body.Width = 85;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            minDmg = Convert.ToInt32(18);
            maxDmg = Convert.ToInt32(36);
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
        public override void damageTaken(ref int dmg)
        {
            if (shocked)
            {
                dmg = Convert.ToInt32(dmg * 1.5);

            }
            healthPoints -= dmg;
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * 500;
                monsterHpBar.Width = width;
            }
            else
            {
                dead = true;
                monsterHpBar.Width = 0;
            }
        }
        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[8];
            monsterMovementLeft = new BitmapImage[8];
            monsterAttackRight = new BitmapImage[8];
            monsterAttackLeft = new BitmapImage[8];
            attackHitBoxLeft = new BitmapImage[3];
            attackHitBoxRight = new BitmapImage[3];
            tentacleSprite = new BitmapImage[8];
            BitmapImage AbominationSpriteAttack = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Skull.png", UriKind.Absolute));
            BitmapImage AbominationSpriteMovement = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Skull.png", UriKind.Absolute));
            BitmapImage tentacleImages = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/BossAnimations/tentacles.png", UriKind.Absolute));

            int spriteWidth = 57;
            int spriteHeight = 88;

            for (int i = 0; i < 176; i += spriteHeight)
            {

                int animation = 0;
                for (int j = 0; j < 456; j += spriteWidth)
                {


                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(AbominationSpriteAttack, spriteRect);
                    CroppedBitmap croppedBitmapM = new CroppedBitmap(AbominationSpriteMovement, spriteRect);
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
                        monsterMovementLeft[animation] = sprite2;
                    }
                    else
                    {
                        monsterMovementRight[animation] = sprite2;
                    }


                    if (i > 0)
                    {
                        monsterAttackLeft[animation] = sprite;
                    }
                    else
                    {
                        monsterAttackRight[animation] = sprite;
                    }
                    animation++;

                }
            }
            int tenWidth = 25;
            int tenHeight = 90;
            int animationTen = 0;
            for (int i = 0; i < 180; i += tenHeight)
            {


                for (int j = 0; j < 200; j += tenWidth * 2)
                {


                    Int32Rect spriteRect = new Int32Rect(j, i, tenWidth, tenHeight);
                    CroppedBitmap croppedBitmap = new CroppedBitmap(tentacleImages, spriteRect);

                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);

                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();
                    tentacleSprite[animationTen] = sprite;
                    animationTen++;

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

        private void Smash(System.Windows.Shapes.Rectangle player, Action<int, string> dealDmg, int stage)
        {
            int min;
            int max;
            double stunDuration = 600;

            if (stage == 1)
            {
                min = minDmg;
                max = maxDmg;

            }
            else if (stage == 2)
            {
                min = Convert.ToInt16(minDmg * 3 / 4);
                max = Convert.ToInt16(maxDmg * 3 / 4);

            }
            else if (stage == 3)
            {
                min = Convert.ToInt16(minDmg * 1 / 2);
                max = Convert.ToInt16(maxDmg * 1 / 2);

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


                int dealtDamage = rnd.Next(min, max + 1);
                if (ignited)
                {
                    dealtDamage = Convert.ToInt32(dealtDamage * 0.8);

                }
                dealDmg(dealtDamage, nameOfMonster);
                string statusEffect = "Stun";
                Monster.damageOverTime.Add(new Tuple<int, double, string>(0, stunDuration, statusEffect));


            }
        }
        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 7 && attackTimer / 200 > 1)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                hitboxTicks = 0;
                weapon.Fill = Brushes.Transparent;
                return;
            }
            attackTimer += delta * 1000;
            if (attackTicks == 0 && attackTimer / 50 > 1)
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
            if (attackTicks == 1 && attackTimer / 50 > 1)
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
            if (attackTicks == 2 && attackTimer / 50 > 1)
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
            if (attackTicks == 3 && attackTimer / 50 > 1)
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
            if (attackTicks == 4 && attackTimer / 150 > 1)
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
                Smash(player, dealDmg, 1);
                return;
            }
            if (attackTicks == 5 && attackTimer / 150 > 1)
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
                Smash(player, dealDmg, 2);
                return;
            }
            if (attackTicks == 6 && attackTimer / 150 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 150;
                    weapon.Width = 240;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 - 50);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 50);
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
                Smash(player, dealDmg, 3);

                return;
            }


        }
        protected override void hpBar()
        {
            nameHolder = new TextBox();
            nameHolder.Text = "Sehn, Harbringer of Madness";
            nameHolder.FontFamily = new FontFamily("Algerian");
            nameHolder.FontSize = 25;
            nameHolder.TextAlignment = TextAlignment.Center;
            nameHolder.Width = 400;
            nameHolder.Height = 30;
            Canvas.SetLeft(nameHolder, 550);
            Canvas.SetTop(nameHolder, 10);
            Canvas.SetZIndex(nameHolder, 700);

            monsterHpBar = new System.Windows.Shapes.Rectangle();
            background = new System.Windows.Shapes.Rectangle();
            Canvas.SetZIndex(monsterHpBar, 700);
            Canvas.SetZIndex(background, 699);

            background.Width = 500;
            background.Height = 20;
            background.Fill = Brushes.Black;
            Canvas.SetLeft(background, 500);
            Canvas.SetTop(background, 40);
            monsterHpBar.Width = 500;
            monsterHpBar.Height = 20;
            monsterHpBar.Fill = Brushes.Red;
            Canvas.SetLeft(monsterHpBar, 500);
            Canvas.SetTop(monsterHpBar, 40);
        }
        public override void add()
        {
            base.add();
            BelongTO.Children.Add(background);
            BelongTO.Children.Add(nameHolder);
        }
        public override void remove()
        {
            base.remove();
            BelongTO.Children.Remove(background);
            BelongTO.Children.Remove(nameHolder);
        }
        public override void dotDamageTaken(int dmg)
        {
            if (shocked) dmg = Convert.ToInt32(dmg * 1.5);
            healthPoints -= dmg;
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * 500;
                monsterHpBar.Width = width;
            }
            else
            {
                deadToDot.Invoke();
                dead = true;
                monsterHpBar.Width = 0;
            }
        }
        int stage;
        System.Windows.Shapes.Rectangle beamSprite = new System.Windows.Shapes.Rectangle();
        private void checkCollisionForSkill(System.Windows.Shapes.Rectangle player, System.Windows.Shapes.Rectangle hitbox, Action<int, string> dealDmg, int damage)
        {
            Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(hitbox), Canvas.GetTop(hitbox), hitbox.Width, hitbox.Height);
            Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
            {


                int dealtDamage = damage;
                if (ignited)
                {
                    dealtDamage = Convert.ToInt32(dealtDamage * 0.8);

                }
                dealDmg(dealtDamage, nameOfMonster);

            }
        }
        private void useDash(System.Windows.Shapes.Rectangle player, string directionOfAttack, Action<int, string> dealDmg)
        {
            if (stage == 0)
            {
                Canvas.SetLeft(warning, Canvas.GetLeft(body) - 10);
                Canvas.SetTop(warning, Canvas.GetTop(body) - 10);
                warning.Background = Brushes.Red;
                warning.Foreground = Brushes.White;
                warning.Text = "!!!";
                Canvas.SetZIndex(warning, 1000);
                warning.FontSize = 20;
                BelongTO.Children.Add(warning);
                stage++;
                return;
            }

            else if (stage == 1 && timerForSkills < 0.50)
            {
                return;
            }
            else if (stage == 1 && timerForSkills > 0.50)
            {
                BelongTO.Children.Remove(warning);
                System.Windows.Point playerCenter;
                if (directionOfAttack == "Right")
                {
                    double offsetRight = rnd.Next(0, 2) * player.Width;
                    playerCenter = new System.Windows.Point(Canvas.GetLeft(player) + (player.Width / 2) + offsetRight, Canvas.GetTop(player));

                }
                else
                {
                    double offsetLeft = rnd.Next(0, 2) * -1 * player.Width;
                    playerCenter = new System.Windows.Point(Canvas.GetLeft(player) + (player.Width / 2) + offsetLeft, Canvas.GetTop(player));
                }
                targetOfAttack = playerCenter;
                stage++;
            }
            double moveMonsterByX = 0, moveMonsterByY = 0;
            if (targetOfAttack.X > Canvas.GetLeft(body) + body.Width + attackRange / 2)
            {
                moveMonsterByX = Speed * 3;

            }
            if (targetOfAttack.X < Canvas.GetLeft(body) - attackRange / 2)
            {
                moveMonsterByX = -Speed * 3;

            }
            if (targetOfAttack.Y < Canvas.GetTop(body) - body.Height / 6)
            {
                moveMonsterByY = -Speed * 3;

            }
            if (targetOfAttack.Y > Canvas.GetTop(body))
            {
                moveMonsterByY = Speed * 3;

            }
            if (moveMonsterByX == 0 && moveMonsterByY == 0)
            {
                currentDashCooldown = dashCooldown;
                usingSkill = false;
                timerForSkills = 0;
                stage = 0;
                return;
            }
            Canvas.SetLeft(body, Canvas.GetLeft(body) + moveMonsterByX);
            Canvas.SetTop(body, Canvas.GetTop(body) + moveMonsterByY);
            Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body), body.Width, body.Height);

            foreach (System.Windows.Shapes.Rectangle x in BelongTO.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                Rect hitBoxOfElement = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if ((string)x.Tag != "collision" && (string)x.Name != "Player") continue;
                if (determinateCollision(hitBoxOfElement, hitBoxOfAttack))
                {
                    if ((string)x.Tag == "collision")
                    {
                        BelongTO.Children.Remove(x);

                        foreach (System.Windows.Shapes.Rectangle find in BelongTO.Children.OfType<System.Windows.Shapes.Rectangle>())
                        {
                            Rect hitBoxOfObstacle = new Rect(Canvas.GetLeft(find), Canvas.GetTop(find), find.Width, find.Height);
                            if (determinateCollision(hitBoxOfObstacle, hitBoxOfAttack))
                            {
                                if ((string)find.Tag == "obstacle") BelongTO.Children.Remove(find);
                                break;
                            }
                        }
                        currentDashCooldown = dashCooldown;
                        usingSkill = false;
                        timerForSkills = 0;
                        stage = 0;
                        return;
                    }
                    else if ((string)x.Name == "Player")
                    {
                        dealDmg(new Random().Next(70, 140), nameOfMonster);
                        string statusEffect = "Stun";
                        Monster.damageOverTime.Add(new Tuple<int, double, string>(0, 1000, statusEffect));
                        currentDashCooldown = dashCooldown;
                        usingSkill = false;
                        timerForSkills = 0;
                        stage = 0;
                        return;
                    }


                }

            }



        }
        private void dealDmgWithOffset(System.Windows.Shapes.Rectangle player, System.Windows.Shapes.Rectangle damager, Action<int, string> dealDmg, int minDmg, int maxDmg)
        {
            Rect hitBoxPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            Rect hitBoxAttack = new Rect(Canvas.GetLeft(damager), Canvas.GetTop(damager), damager.Width, damager.Height);
            if (determinateCollision(hitBoxPlayer, hitBoxAttack))
            {
                dealDmg(rnd.Next(minDmg, maxDmg + 1), nameOfMonster);
                if ((Canvas.GetLeft(player) + player.Width / 2) > (Canvas.GetLeft(damager) + damager.Width / 2))
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
                }
                else
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);

                }
            }
        }
        private void useAbyss(System.Windows.Shapes.Rectangle player, string directionOfAttack, Action<int, string> dealDmg)
        {
            if (stage == 0)
            {
                Canvas.SetLeft(warning, Canvas.GetLeft(body) - 10);
                Canvas.SetTop(warning, Canvas.GetTop(body) - 10);
                warning.Background = Brushes.Red;
                warning.Foreground = Brushes.White;
                warning.Text = "!";
                Canvas.SetZIndex(warning, 1000);
                warning.FontSize = 20;
                BelongTO.Children.Add(warning);
                stage++;
                return;
            }
            else if (stage == 1 && timerForSkills < 0.15)
            {
                return;
            }
            else if (stage == 1 && timerForSkills > 0.15)
            {
                BelongTO.Children.Remove(warning);

                tentacle = new System.Windows.Shapes.Rectangle();
                tentacleHolder = new System.Windows.Shapes.Rectangle();
                tentacle.Width = 125;
                tentacle.Height = 200;
                tentacleHolder.Width = 125;
                tentacleHolder.Height = 450;
                tentacle.Fill = Brushes.Red;
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[0];
                tentacleHolder.Fill = helper;
                tentacle.Opacity = 0.6;
                Canvas.SetLeft(tentacle, Canvas.GetLeft(player) + player.Width / 2 - tentacle.Width / 2);
                Canvas.SetTop(tentacle, Canvas.GetTop(player) + player.Height / 2 - tentacle.Height / 2);
                Canvas.SetLeft(tentacleHolder, Canvas.GetLeft(tentacle));
                Canvas.SetZIndex(tentacleHolder, 60);
                Canvas.SetTop(tentacleHolder, Canvas.GetTop(tentacle) - 250);

                BelongTO.Children.Add(tentacle);


                stage++;
                timerForSkills = 0;
            }
            else if (stage == 2 && timerForSkills > 0.30)
            {
                tentacle.Fill = Brushes.Transparent;
                BelongTO.Children.Add(tentacleHolder);
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);
            }
            else if (stage == 3 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 4 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 5 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 6 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 7 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }

            else if (stage == 8 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 9 && timerForSkills > 0.09)
            {
                ImageBrush helper = new ImageBrush();
                helper.ImageSource = tentacleSprite[stage - 2];
                tentacleHolder.Fill = helper;
                stage++;
                timerForSkills = 0;
                dealDmgWithOffset(player, tentacle, dealDmg, 10, 20);

            }
            else if (stage == 10 && timerForSkills > 0.09)
            {
                stage = 0;
                currenctTentacleCooldown = tentacleCooldown;
                usingSkill = false;
                timerForSkills = 0;

                BelongTO.Children.Remove(tentacleHolder);
                BelongTO.Children.Remove(tentacle);
            }
        }
        System.Windows.Shapes.Rectangle tentacle;
        System.Windows.Shapes.Rectangle tentacleHolder;
        private void useBeam(System.Windows.Shapes.Rectangle player, string directionOfAttack, Action<int, string> dealDmg)
        {


            beamSprite.Fill = Brushes.Orange;


            if (stage == 0)
            {
                Canvas.SetTop(beamSprite, Canvas.GetTop(body) + body.Height / 3);
                BelongTO.Children.Add(beamSprite);
                if (directionOfAttack == "Left")
                {
                    Canvas.SetLeft(beamSprite, Canvas.GetLeft(body) - 20);
                    beamSprite.Width = 10;
                    beamSprite.Height = 10;
                }
                else
                {
                    Canvas.SetLeft(beamSprite, Canvas.GetLeft(body) + body.Width);
                    beamSprite.Width = 10;
                    beamSprite.Height = 10;
                }
                stage++;
            }
            else if (stage == 1 && timerForSkills > 0.30)
            {

                if (directionOfAttack == "Left")
                {
                    Canvas.SetLeft(beamSprite, 0);
                    beamSprite.Width = Canvas.GetLeft(body) + body.Width / 2;
                    beamSprite.Height = 10;
                }
                else
                {
                    Canvas.SetLeft(beamSprite, Canvas.GetLeft(body) + body.Width / 2);
                    beamSprite.Width = 1200 - Canvas.GetLeft(beamSprite);
                    beamSprite.Height = 10;
                }
                checkCollisionForSkill(player, beamSprite, dealDmg, new Random().Next(10, 15));
                stage++;
                timerForSkills = 0;
            }
            else if (stage == 2 && timerForSkills > 0.09)
            {
                Canvas.SetTop(beamSprite, Canvas.GetTop(body) - 7 + body.Height / 3);
                if (directionOfAttack == "Left")
                {
                    Canvas.SetLeft(beamSprite, 0);
                    beamSprite.Width = Canvas.GetLeft(body) + body.Width / 2;
                    beamSprite.Height = 25;
                }
                else
                {
                    beamSprite.Width = 1200 - Canvas.GetLeft(beamSprite);
                    beamSprite.Height = 25;
                }
                checkCollisionForSkill(player, beamSprite, dealDmg, new Random().Next(20, 25));
                stage++;
                timerForSkills = 0;
            }
            else if (stage == 3 && timerForSkills > 0.09)
            {
                Canvas.SetTop(beamSprite, Canvas.GetTop(body) - 15 + body.Height / 3);

                if (directionOfAttack == "Left")
                {
                    Canvas.SetLeft(beamSprite, 0);
                    beamSprite.Width = Canvas.GetLeft(body) + body.Width / 2;

                    beamSprite.Height = 40;
                }
                else
                {

                    beamSprite.Width = 1200 - Canvas.GetLeft(beamSprite);
                    beamSprite.Height = 40;
                }
                checkCollisionForSkill(player, beamSprite, dealDmg, new Random().Next(30, 35));
                stage++;
                timerForSkills = 0;
            }
            else if (stage == 4 && timerForSkills > 0.09)
            {
                Canvas.SetTop(beamSprite, Canvas.GetTop(body) - 25 + body.Height / 3);

                if (directionOfAttack == "Left")
                {
                    Canvas.SetLeft(beamSprite, 0);
                    beamSprite.Width = Canvas.GetLeft(body) + body.Width / 2;
                    beamSprite.Height = 60;
                }
                else
                {
                    beamSprite.Width = 1200 - Canvas.GetLeft(beamSprite);
                    beamSprite.Height = 60;
                }
                checkCollisionForSkill(player, beamSprite, dealDmg, new Random().Next(40, 45));
                stage++;
            }
            else if (stage == 5 && timerForSkills > 0.09)
            {
                stage = 0;
                timerForSkills = 0;
                currentBeamCooldown = beamCooldown;
                usingSkill = false;
                BelongTO.Children.Remove(beamSprite);
            }
        }


        private void useSkill(double delta, System.Windows.Shapes.Rectangle player, string directionOfAttack, Action<int, string> dealDmg)
        {
            timerForSkills += delta;
            if (currentlyUsing == "beamAttack")
            {
                useBeam(player, directionOfAttack, dealDmg);
            }
            else if (currentlyUsing == "dash")
            {
                useDash(player, directionOfAttack, dealDmg);
            }
            else if (currentlyUsing == "abyss")
            {
                useAbyss(player, directionOfAttack, dealDmg);
            }
            else
            {
                timerForSkills = 0;
                usingSkill = false;
            }
        }
        TextBox warning = new TextBox();
        string directionOfAttack;
        System.Windows.Point targetOfAttack;
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            if (gracePeriod > 0)
            {
                gracePeriod -= delta;
                return;
            }
            if (currentBeamCooldown > 0) currentBeamCooldown -= delta;
            if (currentDashCooldown > 0) currentDashCooldown -= delta;
            if (currenctTentacleCooldown > 0) currenctTentacleCooldown -= delta;
            NormalizeSpeed(delta);
            dotUpdate(delta);
            bool tryAttack = true;

            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name));

            if (usingSkill)
            {
                useSkill(delta, name, directionOfAttack, dealDmg);
                return;
            }
            if (currentDashCooldown <= 0)
            {
                targetOfAttack = playerCenter;
                if (playerCenter.X > (Canvas.GetLeft(body) + body.Width / 2))
                {
                    directionOfAttack = "Right";

                    moveInRightDirection = true;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                    body.Fill = monsterSprite;



                }
                else
                {
                    directionOfAttack = "Left";
                    moveInRightDirection = false;
                    currentAnimation = 0;
                    monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                    body.Fill = monsterSprite;
                }
                stage = 0;
                usingSkill = true;
                currentlyUsing = "dash";
            }
            if (Math.Abs((playerCenter.Y - name.Height / 2) - (Canvas.GetTop(body) - body.Height / 2)) < 60 && !prepareToAttack && !usingSkill && currentBeamCooldown <= 0)
            {
                if (currentBeamCooldown <= 0)
                {
                    stage = 0;
                    timerForSkills = 0;
                    if (playerCenter.X > (Canvas.GetLeft(body) + body.Width / 2))
                    {
                        directionOfAttack = "Right";
                        moveInRightDirection = true;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementRight[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    else
                    {
                        directionOfAttack = "Left";
                        moveInRightDirection = false;
                        currentAnimation = 0;
                        monsterSprite.ImageSource = monsterMovementLeft[currentAnimation];
                        body.Fill = monsterSprite;
                    }
                    usingSkill = true;
                    currentlyUsing = "beamAttack";

                    return;
                }
            }
            if (currenctTentacleCooldown <= 0 && !usingSkill)
            {
                stage = 0;
                timerForSkills = 0;
                usingSkill = true;
                currentlyUsing = "abyss";
                return;

            }
            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
            if (playerCenter.Y < Canvas.GetTop(body) - body.Height / 6)
            {
                moveMonsterByY = -Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.Y > Canvas.GetTop(body))
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




        }



    }
    internal class ghostOfSenjuro : Monster
    {
        int hitboxTicks = 0;
        TextBox nameHolder;
        System.Windows.Shapes.Rectangle background;
        double teleportCooldown;
        double strongAttackCooldown;
        double invisibilityCooldown;
        double currentTeleportCooldown;
        double currentStrongAttackCooldown;
        double currentInvisibilityCooldown;
        string currentlyUsing;
        private double gracePeriod = 0.2;
        private bool usingSkill = false;
        private double timerForSkills;
        public ghostOfSenjuro(Canvas canv, int x, int y)
        {
            timerForSkills = 0;
            teleportCooldown = 30;
            strongAttackCooldown = 20;
            invisibilityCooldown = 15;
            currentTeleportCooldown = 0;
            currentStrongAttackCooldown = 0;
            currentInvisibilityCooldown = 0;

            nameOfMonster = "Senjuro, Ghost Samurai \nof Kanto";

            expGiven = 2000;
            attackTicks = 0;
            attackRange = 100;

            Speed = 200;
            baseSpeed = 200;

            animations = 8;
            currentAnimation = 0;
            body.Height = 240;
            body.Width = 192;
            //body.Height = 240;
            //body.Width = 192;

            healthPoints = 1500;
            maxHealthPoints = healthPoints;

            body.Fill = Brushes.Blue;
            body.Tag = "enemy";

            minDmg = Convert.ToInt32(24);
            maxDmg = Convert.ToInt32(52);

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
        public override void damageTaken(ref int dmg)
        {
            if (shocked)
            {
                dmg = Convert.ToInt32(dmg * 1.5);

            }
            healthPoints -= dmg;
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * 500;
                monsterHpBar.Width = width;
            }
            else
            {
                dead = true;
                monsterHpBar.Width = 0;
            }
        }
        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[8];
            monsterMovementLeft = new BitmapImage[8];
            monsterAttackRight = new BitmapImage[8];
            monsterAttackLeft = new BitmapImage[8];
            attackHitBoxLeft = new BitmapImage[3];
            attackHitBoxRight = new BitmapImage[3];
            BitmapImage SamuraiSpriteAttack = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/SamuraiAttack.png", UriKind.Absolute));
            BitmapImage SamuraiSpriteAttackL = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/SamuraiAttackL.png", UriKind.Absolute));
            BitmapImage SamuraiSpriteMovement = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/SamuraiRun.png", UriKind.Absolute));
            BitmapImage SamuraiSpriteMovementLeft = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/SamuraiRunL.png", UriKind.Absolute));

            int spriteWidth = 232;
            int spriteHeight = 214;

            for (int i = 0; i < 214; i += spriteHeight)
            {

                int animation = 0;
                for (int j = 0; j < (232 * 8); j += spriteWidth)
                {
                    Int32Rect spriteRect = new Int32Rect(j, i, spriteWidth, spriteHeight);
                    CroppedBitmap croppedBitmapM = new CroppedBitmap(SamuraiSpriteMovement, spriteRect);
                    CroppedBitmap croppedBitmapML = new CroppedBitmap(SamuraiSpriteMovementLeft, spriteRect);
                    MemoryStream stream2 = new MemoryStream();
                    PngBitmapEncoder encoder2 = new PngBitmapEncoder();
                    encoder2.Frames.Add(BitmapFrame.Create(croppedBitmapM));
                    encoder2.Save(stream2);
                    MemoryStream stream3 = new MemoryStream();
                    PngBitmapEncoder encoder3 = new PngBitmapEncoder();
                    encoder3.Frames.Add(BitmapFrame.Create(croppedBitmapML));
                    encoder3.Save(stream3);
                    BitmapImage sprite2 = new BitmapImage();
                    sprite2.BeginInit();
                    sprite2.CacheOption = BitmapCacheOption.OnLoad;
                    sprite2.StreamSource = stream2;
                    sprite2.EndInit();
                    BitmapImage sprite3 = new BitmapImage();
                    sprite3.BeginInit();
                    sprite3.CacheOption = BitmapCacheOption.OnLoad;
                    sprite3.StreamSource = stream3;
                    sprite3.EndInit();

                    monsterMovementLeft[animation] = sprite3;
                    monsterMovementRight[animation] = sprite2;
                    animation++;

                }
            }

            int attackspriteHeight = 280;
            int attackspriteWidth = 460;

            for (int i = 0; i < 280; i += attackspriteHeight)
            {

                int animation = 0;
                for (int j = 0; j < (460 * 6); j += attackspriteWidth)
                {
                    Int32Rect spriteRect = new Int32Rect(j, i, attackspriteWidth, attackspriteHeight);

                    CroppedBitmap croppedBitmap = new CroppedBitmap(SamuraiSpriteAttack, spriteRect);
                    MemoryStream stream = new MemoryStream();
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
                    encoder.Save(stream);

                    BitmapImage sprite = new BitmapImage();
                    sprite.BeginInit();
                    sprite.CacheOption = BitmapCacheOption.OnLoad;
                    sprite.StreamSource = stream;
                    sprite.EndInit();

                    CroppedBitmap croppedBitmapL = new CroppedBitmap(SamuraiSpriteAttackL, spriteRect);
                    MemoryStream streamL = new MemoryStream();
                    PngBitmapEncoder encoderL = new PngBitmapEncoder();
                    encoderL.Frames.Add(BitmapFrame.Create(croppedBitmapL));
                    encoderL.Save(streamL);

                    BitmapImage spriteL = new BitmapImage();
                    spriteL.BeginInit();
                    spriteL.CacheOption = BitmapCacheOption.OnLoad;
                    spriteL.StreamSource = streamL;
                    spriteL.EndInit();

                    monsterAttackRight[animation] = sprite;
                    monsterAttackLeft[animation] = spriteL;
                    animation++;

                }
            }

        }

        protected override void hpBar()
        {
            nameHolder = new TextBox();
            nameHolder.Text = "Senjuro, Ghost Samurai \nof Kanto";
            nameHolder.FontFamily = new FontFamily("Algerian");
            nameHolder.FontSize = 25;
            nameHolder.TextAlignment = TextAlignment.Center;
            nameHolder.Width = 400;
            nameHolder.Height = 30;
            Canvas.SetLeft(nameHolder, 550);
            Canvas.SetTop(nameHolder, 10);
            Canvas.SetZIndex(nameHolder, 700);

            monsterHpBar = new System.Windows.Shapes.Rectangle();
            background = new System.Windows.Shapes.Rectangle();
            Canvas.SetZIndex(monsterHpBar, 700);
            Canvas.SetZIndex(background, 699);

            background.Width = 500;
            background.Height = 20;
            background.Fill = Brushes.Black;
            Canvas.SetLeft(background, 500);
            Canvas.SetTop(background, 40);
            monsterHpBar.Width = 500;
            monsterHpBar.Height = 20;
            monsterHpBar.Fill = Brushes.Red;
            Canvas.SetLeft(monsterHpBar, 500);
            Canvas.SetTop(monsterHpBar, 40);
        }

        //string directionOfAttack;
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            if (gracePeriod > 0)
            {
                gracePeriod -= delta;
                return;
            }
            NormalizeSpeed(delta);
            dotUpdate(delta);
            bool tryAttack = true;

            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name));

            if (currentInvisibilityCooldown > 0)
            {
                currentInvisibilityCooldown -= delta;
            }
            if (currentStrongAttackCooldown > 0)
            {
                currentStrongAttackCooldown -= delta;
            }
            if (currentTeleportCooldown > 0)
            {
                currentTeleportCooldown -= delta;
            }

            if (usingSkill)
            {
                useSkill(delta, name, dealDmg);
                return;
            }
            if (currentTeleportCooldown <= 0)
            {
                usingSkill = true;
                currentlyUsing = "Teleport";
            }
            if (prepareToAttack && !usingSkill && currentStrongAttackCooldown <= 0)
            {
                timerForSkills = 0;
                usingSkill = true;
                currentlyUsing = "StrongAttack";
            }
            if (!usingSkill && currentInvisibilityCooldown <= 0)
            {
                timerForSkills = 0;
                usingSkill = true;
                currentlyUsing = "Disappear";
            }


            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
            if (playerCenter.Y < Canvas.GetTop(body) - body.Height / -140)
            {
                moveMonsterByY = -Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.Y > Canvas.GetTop(body) - body.Height / -2)
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
        }

        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 7 && attackTimer / 200 > 1)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
                hitboxTicks = 0;
                weapon.Fill = Brushes.Transparent;
                return;
            }
            attackTimer += delta * 1000;
            if (attackTicks == 0 && attackTimer / 50 > 1)
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
            if (attackTicks == 1 && attackTimer / 50 > 1)
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
            if (attackTicks == 2 && attackTimer / 50 > 1)
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
            if (attackTicks == 3 && attackTimer / 50 > 1)
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
            if (attackTicks == 4 && attackTimer / 150 > 1)
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
            if (attackTicks == 5 && attackTimer / 150 > 1)
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
            if (attackTicks == 6 && attackTimer / 150 > 1)
            {
                if (moveInRightDirection)
                {
                    weapon.Height = 150;
                    weapon.Width = 240;
                    Canvas.SetLeft(weapon, Canvas.GetLeft(body) + body.Width * 2 / 3 - 50);
                    Canvas.SetTop(weapon, Canvas.GetTop(body) + body.Height * 3 / 4 - 50);
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
                Slash(player, dealDmg);

                return;
            }

        }
        public override void add()
        {
            base.add();
            BelongTO.Children.Add(background);
            BelongTO.Children.Add(nameHolder);
        }
        public override void remove()
        {
            base.remove();
            BelongTO.Children.Remove(background);
            BelongTO.Children.Remove(nameHolder);
        }
        public override void dotDamageTaken(int dmg)
        {
            if (shocked) dmg = Convert.ToInt32(dmg * 1.5);
            healthPoints -= dmg;
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * 500;
                monsterHpBar.Width = width;
            }
            else
            {
                deadToDot.Invoke();
                dead = true;
                monsterHpBar.Width = 0;
            }
        }

        private void Slash(System.Windows.Shapes.Rectangle player, Action<int, string> dealDmg)
        {
            int min;
            int max;

            min = minDmg;
            max = maxDmg;

            Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.Width, weapon.Height);
            Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
            {


                int dealtDamage = rnd.Next(min, max + 1);
                if (ignited)
                {
                    dealtDamage = Convert.ToInt32(dealtDamage * 0.8);

                }
                dealDmg(dealtDamage, nameOfMonster);


            }
        }

        //funkcje zapożyczone do skilli
        //int stage;
        System.Windows.Shapes.Rectangle beamSprite = new System.Windows.Shapes.Rectangle();
        private void checkCollisionForSkill(System.Windows.Shapes.Rectangle player, System.Windows.Shapes.Rectangle hitbox, Action<int, string> dealDmg, int damage)
        {
            Rect hitBoxOfAttack = new Rect(Canvas.GetLeft(hitbox), Canvas.GetTop(hitbox), hitbox.Width, hitbox.Height);
            Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
            {


                int dealtDamage = damage;
                if (ignited)
                {
                    dealtDamage = Convert.ToInt32(dealtDamage * 0.8);

                }
                dealDmg(dealtDamage, nameOfMonster);

            }
        }

        private void dealDmgWithOffset(System.Windows.Shapes.Rectangle player, System.Windows.Shapes.Rectangle damager, Action<int, string> dealDmg, int minDmg, int maxDmg)
        {
            Rect hitBoxPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            Rect hitBoxAttack = new Rect(Canvas.GetLeft(damager), Canvas.GetTop(damager), damager.Width, damager.Height);
            if (determinateCollision(hitBoxPlayer, hitBoxAttack))
            {
                dealDmg(rnd.Next(minDmg, maxDmg + 1), nameOfMonster);
                if ((Canvas.GetLeft(player) + player.Width / 2) > (Canvas.GetLeft(damager) + damager.Width / 2))
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + 30);
                }
                else
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - 30);

                }
            }
        }

        private void useSkill(double delta, System.Windows.Shapes.Rectangle player, Action<int, string> dealDmg)
        {
            timerForSkills += delta;
            if (currentlyUsing == "Teleport")
            {
                useTeleport(player);//(player, directionOfAttack, dealDmg);
            }
            else if (currentlyUsing == "StrongAttack")
            {
                useStrongAttack(player, delta, dealDmg);//(player, directionOfAttack, dealDmg);
            }
            else if (currentlyUsing == "Disappear")
            {
                useDisappear();//(player, directionOfAttack, dealDmg);
            }
            else
            {
                timerForSkills = 0;
                usingSkill = false;
            }
        }

        //skill teleportacja
        private void useTeleport(System.Windows.Shapes.Rectangle player)
        {

            if (currentTeleportCooldown <= 0) //&& !usingSkill)
            {
                //MessageBox.Show("Teleport");
                int randX, randY;
                Random randomTeleport = new Random();
                //System.Drawing.Point teleportDestination;

                randX = randomTeleport.Next(40, 1110);
                randY = randomTeleport.Next(40, 550);

                //teleportDestination = new System.Drawing.Point(randX, randY);

                Canvas.SetLeft(body, randX);
                Canvas.SetTop(body, randY);

                timerForSkills = 0;
                currentTeleportCooldown = teleportCooldown;
            }
            usingSkill = false;
        }

        //skill strongAttack
        private void useStrongAttack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {

            if (currentStrongAttackCooldown <= 0)
            {
                TextBox strongAttack = new TextBox();
                strongAttack.Text = "!";
                strongAttack.Foreground = Brushes.Red;
                strongAttack.Visibility = Visibility.Visible;
                //MessageBox.Show("Strong");
                int min = 100, max = 200, damageValue;
                Random strongAttackRand = new Random();
                damageValue = strongAttackRand.Next(min, max + 1);

                checkCollisionForSkill(player, body, dealDmg, damageValue);

                Canvas.SetLeft(strongAttack, Canvas.GetLeft(body) - 10);
                Canvas.SetTop(strongAttack, Canvas.GetTop(body) - 10);
                attack(player, delta, dealDmg);
                currentStrongAttackCooldown = strongAttackCooldown;
            }
            usingSkill = false;
        }


        //skill niewidzialność
        
        private void useDisappear()
        {
            //MessageBox.Show("Disappear");
            if (currentInvisibilityCooldown <= 0)
            {
                
                currentInvisibilityCooldown = invisibilityCooldown;
            }
            usingSkill = false;
        }
    }
    internal class DemonOfBelow : Monster
    {
        int hitboxTicks = 0;
        TextBox nameHolder;
        System.Windows.Shapes.Rectangle background;

        private double gracePeriod = 0.2;
        private bool usingSkill = false;
        private double timerForSkills;
        double teleportCooldown;
        double currentTeleportCooldown;

        public DemonOfBelow(Canvas canv, int x, int y)
        {
            timerForSkills = 0;
            teleportCooldown = 30;
            currentTeleportCooldown = 0;



            nameOfMonster = "Sidragaso son of a Judas,\n Demon from hell";

            expGiven = 2000;
            attackTicks = 0;
            attackRange = 100;

            Speed = 200;
            baseSpeed = 200;

            animations = 6;
            currentAnimation = 0;
            body.Height = 96; // 192
            body.Width = 120; // 240


            healthPoints = 1000;
            maxHealthPoints = healthPoints;

            body.Fill = Brushes.Blue;
            body.Tag = "enemy";

            minDmg = 15;
            maxDmg = 30;

            weapon.Height = 20;
            weapon.Width = 20;
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
        public override void add()
        {
            base.add();
            BelongTO.Children.Add(background);
            BelongTO.Children.Add(nameHolder);
        }
        public override void remove()
        {
            base.remove();
            BelongTO.Children.Remove(background);
            BelongTO.Children.Remove(nameHolder);
        }
        private void attack(System.Windows.Shapes.Rectangle player, double delta, Action<int, string> dealDmg)
        {
            if (attackTicks == 7)
            {
                prepareToAttack = false;
                attackTicks = 0;
                attackTimer = 0;
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
            if (attackTicks == 1 && attackTimer / 50 > 1)
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
            if (attackTicks == 2 && attackTimer / 50 > 1)
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
            if (attackTicks == 3 && attackTimer / 50 > 1)
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
            if (attackTicks == 4 && attackTimer / 50 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[3];
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
            if (attackTicks == 5 && attackTimer / 50 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[3];
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

            if (attackTicks == 6 && attackTimer / 50 > 1)
            {
                if (moveInRightDirection)
                {
                    monsterSprite.ImageSource = monsterAttackRight[attackTicks];
                }
                else
                {
                    monsterSprite.ImageSource = monsterAttackLeft[attackTicks];

                }
                Rect hitBoxOfAttack;
                if (moveInRightDirection) hitBoxOfAttack = new Rect(Canvas.GetLeft(body) + body.Width / 2, Canvas.GetTop(body), body.ActualWidth / 2, body.ActualHeight);
                else hitBoxOfAttack = new Rect(Canvas.GetLeft(body), Canvas.GetTop(body), body.ActualWidth / 2, body.ActualHeight);
                Rect hitBoxOfPlayer = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                if (determinateCollision(hitBoxOfPlayer, hitBoxOfAttack))
                {


                    int dealtDamage = rnd.Next(minDmg, maxDmg + 1);
                    if (ignited)
                    {
                        dealtDamage = Convert.ToInt32(dealtDamage * 0.8);
                    }
                    dealDmg(dealtDamage, nameOfMonster);


                }
                body.Fill = monsterSprite;
                attackTicks++;
                attackTimer = 0;
                return;
            }


        }

        private bool enraged = false;
        public override void damageTaken(ref int dmg)
        {
            if (shocked)
            {
                dmg = Convert.ToInt32(dmg * 1.5);

            }
            healthPoints -= dmg;
            if (healthPoints * 2 < maxHealthPoints && !enraged)
            {
                enraged = true;
                body.Width = 240;
                body.Height = 192;
                minDmg = 30;
                maxDmg = 60;
                Speed = 150;
                baseSpeed = 150;
                weapon.Height = 40;
                weapon.Width = 40;
            }
            if (healthPoints > 0)
            {
                double width = (healthPoints / maxHealthPoints) * 500;
                monsterHpBar.Width = width;
            }
            else
            {
                dead = true;
                monsterHpBar.Width = 0;
            }
        }
        public override void moveToTarget(System.Windows.Shapes.Rectangle name, double delta, double friction, Action<int, string> dealDmg)
        {
            if (delta > 1) return; // Starting delta value is about 3 billions 
            if (gracePeriod > 0)
            {
                gracePeriod -= delta;
                return;
            }
            NormalizeSpeed(delta);
            dotUpdate(delta);
            bool tryAttack = true;

            setRelativeVisibility();

            System.Windows.Point playerCenter = new System.Windows.Point(Canvas.GetLeft(name) + (name.Width / 2), Canvas.GetTop(name));


            if (currentTeleportCooldown > 0)
            {
                currentTeleportCooldown -= delta;
            }




            if (prepareToAttack)
            {
                attack(name, delta, dealDmg);
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
            if (playerCenter.Y < Canvas.GetTop(body) - body.Height / -140)
            {
                moveMonsterByY = -Speed * friction;
                tryAttack = false;
            }
            if (playerCenter.Y > Canvas.GetTop(body) - body.Height / -2)
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
        }

        public override void loadImages()
        {
            monsterMovementRight = new BitmapImage[6];
            monsterMovementLeft = new BitmapImage[6];
            monsterAttackRight = new BitmapImage[7];
            monsterAttackLeft = new BitmapImage[7];
            // 1-7 attack 1-6 movement

            //ruchy
            for (int i = 0; i < 6; i++)
            {
                monsterMovementLeft[i] = new BitmapImage();
                monsterMovementLeft[i].BeginInit();
                monsterMovementLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Demon/demon_walk_L{1 + i}.png", UriKind.Absolute);
                monsterMovementLeft[i].EndInit();
                monsterMovementRight[i] = new BitmapImage();
                monsterMovementRight[i].BeginInit();
                monsterMovementRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Demon/demon_walk_R{1 + i}.png", UriKind.Absolute);
                monsterMovementRight[i].EndInit();

            }
            // Ładowanie klatek do animacji ataku

            for (int i = 0; i < 7; i++)
            {
                monsterAttackLeft[i] = new BitmapImage();
                monsterAttackLeft[i].BeginInit();
                monsterAttackLeft[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Demon/demon_cleave_L{1 + i}.png", UriKind.Absolute);
                monsterAttackLeft[i].EndInit();
                monsterAttackRight[i] = new BitmapImage();
                monsterAttackRight[i].BeginInit();
                monsterAttackRight[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/Bosses/Demon/demon_cleave_R{1 + i}.png", UriKind.Absolute);
                monsterAttackRight[i].EndInit();
            }

        }

        protected override void hpBar()
        {
            nameHolder = new TextBox();
            nameHolder.Text = "Sidragaso son of a Judas \n Demon from HELL";
            nameHolder.FontFamily = new FontFamily("Algerian");
            nameHolder.FontSize = 25;
            nameHolder.TextAlignment = TextAlignment.Center;
            nameHolder.Width = 400;
            nameHolder.Height = 30;
            Canvas.SetLeft(nameHolder, 550);
            Canvas.SetTop(nameHolder, 10);
            Canvas.SetZIndex(nameHolder, 700);

            monsterHpBar = new System.Windows.Shapes.Rectangle();
            background = new System.Windows.Shapes.Rectangle();
            Canvas.SetZIndex(monsterHpBar, 700);
            Canvas.SetZIndex(background, 699);

            background.Width = 500;
            background.Height = 20;
            background.Fill = Brushes.Black;
            Canvas.SetLeft(background, 500);
            Canvas.SetTop(background, 40);
            monsterHpBar.Width = 500;
            monsterHpBar.Height = 20;
            monsterHpBar.Fill = Brushes.Red;
            Canvas.SetLeft(monsterHpBar, 500);
            Canvas.SetTop(monsterHpBar, 40);
        }





    }


}



