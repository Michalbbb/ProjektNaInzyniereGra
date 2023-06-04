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
    public partial class Monster
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
   
    


}



