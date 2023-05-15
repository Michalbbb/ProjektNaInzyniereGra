using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BasicsOfGame
{
    internal class Skill
    {
        //RotateTransform rotateByX = new RotateTransform(ANGLE);
        //example.RenderTransform=rotateByX;
        static protected Random rnd=new Random();
        protected const int DAMAGE = 0;
        protected const int FIRE_DAMAGE = 1;
        protected const int ICE_DAMAGE = 2;
        protected const int LIGHTNING_DAMAGE = 3;
        protected const int DOT_DAMAGE = 4;
        protected const int BLEED_CHANCE = 0;
        protected const int IGNITE_CHANCE = 1;
        protected const int CHILL_CHANCE = 2;
        protected const int SHOCK_CHANCE = 3;
        protected const int POISON_CHANCE = 4;
        protected const int STUN_CHANCE = 5;
        protected string Type; // Support/Offensive/Defensive
        protected int baseMinDamage;
        protected int minDamage;
        protected int baseMaxDamage;
        protected int maxDamage;
        protected double baseCooldown;
        protected double cooldown;
        protected double currentCooldown;
        protected bool isUsingSkill;
        protected bool canCrit;
        protected double distanceToTravel;
        protected int[] statusEffects =new int[]{0,0,0,0,0,0};
        public static int hitsToDisappear;
        public Action<System.Windows.Shapes.Rectangle,int, int[], bool> tryDamaging;
        public double getCooldown()
        {
            if (currentCooldown < 0) return 0;
            return currentCooldown;
        }
        protected bool determinateCollision(Rect skill, Rect obj)
        {
            if (obj.X < (skill.X + skill.Width) && (obj.X + obj.Width) > skill.X)
            {

                if (obj.Y < (skill.Y + skill.Height) && (obj.Y + obj.Height) > skill.Y) return true;
                else return false;
            }
            else return false;
        }
        virtual public void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition) { }
        virtual public void updateState(double delta, List<Monster> monsters) { }
        virtual public void recalculateStats(List<double> increasedDamageList,double cooldownReduction) { }
    }
    internal class Fireball : Skill
    {
        
        int stunChance = 100;
        double moveByX;
        double moveByY;
       
        int speed;
        ImageBrush fireballSprite; // NEED GRAPHIC
        
        System.Windows.Shapes.Rectangle fireballHitbox;
        Canvas canvas;
        public Fireball(Canvas canv)
        {
            canvas = canv;
            baseMinDamage = 40;
            baseMaxDamage = 80;
            minDamage = 40;
            maxDamage = 80;
            baseCooldown = 6; // 6Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Offensive";
            isUsingSkill = false;
            statusEffects[STUN_CHANCE] = stunChance;
            canCrit = false;
            
            speed = 700;
            fireballSprite = new ImageBrush();
            fireballHitbox = new System.Windows.Shapes.Rectangle();
            fireballSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireball.png", UriKind.Absolute)); ;
            fireballHitbox.Fill = fireballSprite;
            fireballHitbox.Width = 50;
            fireballHitbox.Height = 50;
            

        }
        public override void updateState(double delta, List<Monster> monsters)
        {
            if (currentCooldown > 0)
            {
               
                currentCooldown -= delta;
            }
            else if (isUsingSkill)
            {
                distanceToTravel -= speed*delta;
                if(distanceToTravel<=0) {; currentCooldown = cooldown; canvas.Children.Remove(fireballHitbox); isUsingSkill = false; }
                Canvas.SetLeft(fireballHitbox,Canvas.GetLeft(fireballHitbox)+(moveByX* delta));
                Canvas.SetTop(fireballHitbox, Canvas.GetTop(fireballHitbox) + (moveByY * delta));
                
                int damageDealt=Skill.rnd.Next(minDamage,maxDamage);
                tryDamaging.Invoke(fireballHitbox, damageDealt, statusEffects, canCrit);
                if (Skill.hitsToDisappear == 0)
                {
                    isUsingSkill = false;
                    canvas.Children.Remove(fireballHitbox);
                    currentCooldown = cooldown;
                }  
                  
             }
                
            
        }
       
        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;
            double increasedDamage = increasedDamageList[DAMAGE] + increasedDamageList[FIRE_DAMAGE];
            minDamage = Convert.ToInt32(increasedDamage * baseMinDamage);
            maxDamage = Convert.ToInt32(increasedDamage * baseMaxDamage);
            
        }
        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {
            if (currentCooldown > 0) return;
            if(!isUsingSkill)
            {
                isUsingSkill = true;
                distanceToTravel = 600;
                double time = (Math.Abs(mousePosition.X - playerPosition.X) + Math.Abs(mousePosition.Y - playerPosition.Y))/speed;
                moveByX =(mousePosition.X-playerPosition.X)/time;
                moveByY=(mousePosition.Y-playerPosition.Y) /time;
                
                Canvas.SetLeft(fireballHitbox, playerPosition.X);
                Canvas.SetTop(fireballHitbox, playerPosition.Y);
                //MessageBox.Show(moveByY + "<y x>" + moveByX);
                RotateTransform rotateByAngle;
                if(Math.Abs(Math.Abs(moveByX) - Math.Abs(moveByY)) < 0.20)
                {
                   
                        if (moveByX > 0) { 
                            if(moveByY > 0) rotateByAngle = new RotateTransform(45);
                            else rotateByAngle = new RotateTransform(315);

                        }
                        else
                        {
                            if (moveByY > 0) rotateByAngle = new RotateTransform(135);
                            else rotateByAngle = new RotateTransform(225);
                        }
                    
                   
                }
                else if (Math.Abs(moveByX) > Math.Abs(moveByY))
                {
                    if(moveByX>0) { rotateByAngle = new RotateTransform(0); }
                    else
                    {
                        rotateByAngle = new RotateTransform(180);
                    }
                }
                else
                {
                    if(moveByY>0) { rotateByAngle = new RotateTransform(90); }
                    else rotateByAngle = new RotateTransform(270);
                }
                
                fireballHitbox.RenderTransform= rotateByAngle;
                canvas.Children.Add(fireballHitbox);
                Skill.hitsToDisappear = 1;

                

                
            }
        }
    }
    internal class IceBurst : Skill
    {

        int chillChance = 100;
        int sequence;
        double timeBetween;
        string direction;
        ImageBrush iceBurstSprite; // NEED GRAPHIC

        System.Windows.Shapes.Rectangle iceBurstHitBox;
        Canvas canvas;
        public IceBurst(Canvas canv)
        {
            canvas = canv;
            baseMinDamage = 15;
            baseMaxDamage = 30;
            minDamage = 15;
            maxDamage = 30;
            baseCooldown = 10; // 10Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Offensive";
            isUsingSkill = false;
            statusEffects[CHILL_CHANCE] = chillChance;
            canCrit = false;


            iceBurstSprite = new ImageBrush();
            iceBurstHitBox = new System.Windows.Shapes.Rectangle();
            iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst1.png", UriKind.Absolute)); ;
            iceBurstHitBox.Fill = iceBurstSprite;
            iceBurstHitBox.Width = 75;
            iceBurstHitBox.Height = 75;


        }
        public override void updateState(double delta, List<Monster> monsters)
        {
            if (currentCooldown > 0)
            {

                currentCooldown -= delta;
            }
            else if (isUsingSkill)
            {

                System.Windows.Shapes.Rectangle modifiedHitBox = new System.Windows.Shapes.Rectangle(); ;

                if (sequence == 0)
                {
                    canvas.Children.Add(iceBurstHitBox);
                      int damageDealt = Skill.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(iceBurstHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;

                }
                if(sequence == 1&&timeBetween<=0) {
                   
                    iceBurstHitBox.Width = 120;
                    iceBurstHitBox.Height = 75;
                    if (direction == "left")
                    {
                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst2l.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(iceBurstHitBox, Canvas.GetLeft(iceBurstHitBox) - 45);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 75;
                        modifiedHitBox.Height = iceBurstHitBox.Height;


                    }
                    else
                    {
                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst2.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox) + 75);
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 75;
                        modifiedHitBox.Height = iceBurstHitBox.Height;
                    }

                    int damageDealt = Skill.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;
                }
                if (sequence == 2 && timeBetween <= 0)
                {
                    
                    iceBurstHitBox.Width = 171;
                    iceBurstHitBox.Height = 75;
                    if (direction == "left")
                    {
                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst3l.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(iceBurstHitBox, Canvas.GetLeft(iceBurstHitBox) - 51);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 120;
                        modifiedHitBox.Height = iceBurstHitBox.Height;

                    }
                    else
                    {

                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst3.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox) + 120);
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 120;
                        modifiedHitBox.Height = iceBurstHitBox.Height;
                    }
                    int damageDealt = Skill.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;
                }
                if (sequence == 3 && timeBetween <= 0)
                {
                    
                    Canvas.SetTop(iceBurstHitBox, Canvas.GetTop(iceBurstHitBox) - 31);
                    iceBurstHitBox.Width = 256;
                    iceBurstHitBox.Height = 106;
                    if (direction == "left")
                    {
                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst4l.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(iceBurstHitBox, Canvas.GetLeft(iceBurstHitBox) - 85);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 171;
                        modifiedHitBox.Height = iceBurstHitBox.Height;
                    }
                    else
                    {
                        iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst4.png", UriKind.Absolute)); ;
                        iceBurstHitBox.Fill = iceBurstSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(iceBurstHitBox) + 171);
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(iceBurstHitBox));
                        modifiedHitBox.Width = iceBurstHitBox.Width - 171;
                        modifiedHitBox.Height = iceBurstHitBox.Height;
                    }
                    int damageDealt = Skill.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.05;
                    sequence++;
                   
                }
              


                if (sequence == 4&&timeBetween<=0)
                {
                   
                    isUsingSkill = false;
                    canvas.Children.Remove(iceBurstHitBox);
                    currentCooldown = cooldown;
                }
                timeBetween -= delta;

            }


        }

        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;
            double increasedDamage = increasedDamageList[DAMAGE] + increasedDamageList[ICE_DAMAGE];
            minDamage = Convert.ToInt32(increasedDamage * baseMinDamage);
            maxDamage = Convert.ToInt32(increasedDamage * baseMaxDamage);

        }
        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {
            if (currentCooldown > 0) return;
            if (!isUsingSkill)
            {
                isUsingSkill = true;
                sequence = 0;
                timeBetween = 0;
               

                Canvas.SetLeft(iceBurstHitBox, playerPosition.X);
                Canvas.SetTop(iceBurstHitBox, playerPosition.Y);
                //MessageBox.Show(moveByY + "<y x>" + moveByX);
               
                if (mousePosition.X > playerPosition.X) direction = "right";
                else direction = "left";
               
                iceBurstHitBox.Width = 75;
                Canvas.SetTop(iceBurstHitBox, playerPosition.Y - 25);
                iceBurstHitBox.Height = 75;
                if (direction == "left")
                {
                    Canvas.SetLeft(iceBurstHitBox, playerPosition.X-75);
                    iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst1l.png", UriKind.Absolute)); ;
                    iceBurstHitBox.Fill = iceBurstSprite;

                }
                else
                {
                    iceBurstSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst1.png", UriKind.Absolute)); ;
                    iceBurstHitBox.Fill = iceBurstSprite;
                }

                
                
                Skill.hitsToDisappear = 999;




            }
        }
    }
    internal class FireStorm: Skill
    {

        int igniteChance = 50;
        int sequence;
        double timeBetween;
        string direction;
        ImageBrush fireStormSprite; // NEED GRAPHIC
        int duration;
        System.Windows.Shapes.Rectangle fireStormHitBox;
        Canvas canvas;
        int speed;
        public FireStorm(Canvas canv)
        {
            canvas = canv;
            baseMinDamage = 3; // Duration of 2 seconds with hit every 100ms yield about 60-140 dmg at max and on averge 10 ignites that deal 4dmg each
            baseMaxDamage = 7;
            minDamage = 3;
            maxDamage = 7;
            baseCooldown = 10; // 10Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Offensive";
            isUsingSkill = false;
            statusEffects[IGNITE_CHANCE] = igniteChance;
            canCrit = false;
            duration = 20;
            speed = 120;
            fireStormSprite = new ImageBrush();
            fireStormHitBox = new System.Windows.Shapes.Rectangle();
            fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm1.png", UriKind.Absolute)); ;
            fireStormHitBox.Fill = fireStormSprite;
            fireStormHitBox.Width = 60;
            fireStormHitBox.Height = 110;


        }
        public override void updateState(double delta, List<Monster> monsters)
        {
            if (currentCooldown > 0)
            {

                currentCooldown -= delta;
            }
            else if (isUsingSkill)
            {
                bool changeAnimation=false;
                timeBetween -= delta;
                if (direction == "left") Canvas.SetLeft(fireStormHitBox, Canvas.GetLeft(fireStormHitBox) - delta * speed);
                else Canvas.SetLeft(fireStormHitBox, Canvas.GetLeft(fireStormHitBox) + delta * speed);
                if (timeBetween <= 0)
                {
                    int damageDealt = Skill.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(fireStormHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.05;
                    duration--;
                    changeAnimation = true;
                    sequence++;
                    if (duration < 0)
                    {
                        isUsingSkill = false;
                        canvas.Children.Remove(fireStormHitBox);
                        currentCooldown = cooldown;

                    }

                }

                if (sequence == 0&& changeAnimation)
                {

                    fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm4.png", UriKind.Absolute)); ;
                    fireStormHitBox.Fill = fireStormSprite;


                }
                if (sequence == 1 && changeAnimation)
                {

                    fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm1.png", UriKind.Absolute)); ;
                    fireStormHitBox.Fill = fireStormSprite;
               
                   
                }
                if (sequence == 2 && changeAnimation)
                {

                    fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm2.png", UriKind.Absolute)); ;
                    fireStormHitBox.Fill = fireStormSprite;
                    
                }
                if (sequence == 3 && changeAnimation)
                {

                    fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm3.png", UriKind.Absolute)); ;
                    fireStormHitBox.Fill = fireStormSprite;

                    
                    sequence = -1;

                }



               
                

            }


        }

        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;
            double increasedDamage = increasedDamageList[DAMAGE] + increasedDamageList[FIRE_DAMAGE];
            minDamage = Convert.ToInt32(increasedDamage * baseMinDamage);
            maxDamage = Convert.ToInt32(increasedDamage * baseMaxDamage);

        }
        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {
            if (currentCooldown > 0) return;
            if (!isUsingSkill)
            {
                isUsingSkill = true;
                sequence = 0;
                timeBetween = 0;
                fireStormSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireStorm1.png", UriKind.Absolute)); ;
                fireStormHitBox.Fill = fireStormSprite;



                //MessageBox.Show(moveByY + "<y x>" + moveByX);

                if (mousePosition.X > playerPosition.X)
                {
                    Canvas.SetLeft(fireStormHitBox, playerPosition.X);
                    Canvas.SetTop(fireStormHitBox, playerPosition.Y);
                    direction = "right";
                }
                else
                {
                    Canvas.SetLeft(fireStormHitBox, playerPosition.X-50);
                    Canvas.SetTop(fireStormHitBox, playerPosition.Y);
                    direction = "left";

                }

                
               
                fireStormHitBox.Width = 75;
                Canvas.SetTop(fireStormHitBox, playerPosition.Y - 30);
                fireStormHitBox.Height = 75;
                duration = 20;
                canvas.Children.Add(fireStormHitBox);



                Skill.hitsToDisappear = 999;




            }
        }
    }
    internal class HammerOfJudgment : Skill
    {

        int stunChance = 70;
        int sequence;
        double timeBetween;
        
        ImageBrush hammerSprite; // NEED GRAPHIC
      
        System.Windows.Shapes.Rectangle hammerHitBox;
        Canvas canvas;
      
        public HammerOfJudgment(Canvas canv)
        {
            canvas = canv;
            baseMinDamage = 60; // Duration of 2 seconds with hit every 100ms yield about 60-140 dmg at max and on averge 10 ignites that deal 4dmg each
            baseMaxDamage = 150;
            minDamage = 60;
            maxDamage = 150;
            baseCooldown = 20; // 20Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Offensive";
            isUsingSkill = false;
            statusEffects[STUN_CHANCE] = stunChance;
            canCrit = true;
           
           
            hammerSprite = new ImageBrush();
            hammerHitBox = new System.Windows.Shapes.Rectangle();
            hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer1.png", UriKind.Absolute)); ;
            hammerHitBox.Fill = hammerSprite;
            hammerHitBox.Width = 200;
            hammerHitBox.Height = 200;


        }
        public override void updateState(double delta, List<Monster> monsters)
        {
            if (currentCooldown > 0)
            {

                currentCooldown -= delta;
            }
            else if (isUsingSkill)
            {
                
                timeBetween -= delta;
               
                
                if (sequence == 0 && timeBetween<=0)
                {

                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer1.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    sequence++;
                    timeBetween = 0.08;

                }
                if (sequence == 1 && timeBetween <= 0)
                {

                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer2.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    sequence++;
                    timeBetween = 0.08;

                }
                if (sequence == 2 && timeBetween <= 0)
                {

                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer3.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    sequence++;
                    timeBetween = 0.08;
                }
                if (sequence == 3 && timeBetween <= 0)
                {

                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer4.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    sequence++;
                    timeBetween = 0.08;

                    

                }
                if(sequence ==4  && timeBetween <= 0)
                {
                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer5.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    sequence++;
                    timeBetween = 0.08;
                }
                if (sequence == 5 && timeBetween <= 0)
                {
                    hammerHitBox.Width = 100;
                    hammerHitBox.Height = 100;
                    Canvas.SetTop(hammerHitBox, Canvas.GetTop(hammerHitBox) + 40);
                    Canvas.SetLeft(hammerHitBox, Canvas.GetLeft(hammerHitBox) + 100);
                    hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer6.png", UriKind.Absolute)); ;
                    hammerHitBox.Fill = hammerSprite;
                    int damageDealt=Skill.rnd.Next(minDamage,maxDamage);
                    
                    tryDamaging(hammerHitBox, damageDealt,statusEffects,canCrit);
                    
                    sequence++;
                    timeBetween = 0.15;
                    
                }
                if (sequence == 6 && timeBetween <= 0)
                {
                    currentCooldown = cooldown;
                    sequence = 0;
                    isUsingSkill = false;
                    canvas.Children.Remove(hammerHitBox);
                    return;
                }






            }


        }

        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;
            double increasedDamage = increasedDamageList[DAMAGE];
            minDamage = Convert.ToInt32(increasedDamage * baseMinDamage);
            maxDamage = Convert.ToInt32(increasedDamage * baseMaxDamage);

        }
        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {
            if (currentCooldown > 0) return;
            if (!isUsingSkill)
            {
                isUsingSkill = true;
                sequence = 0;
                timeBetween = 0;




                //MessageBox.Show(moveByY + "<y x>" + moveByX);




                hammerSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/hammer1.png", UriKind.Absolute)); ;
                hammerHitBox.Fill = hammerSprite;
                hammerHitBox.Width = 200;
                hammerHitBox.Height = 120;
                Canvas.SetTop(hammerHitBox, mousePosition.Y - 90);
                Canvas.SetLeft(hammerHitBox, mousePosition.X - 150);

                canvas.Children.Add(hammerHitBox);



                Skill.hitsToDisappear = 999;




            }
        }
    }


    internal class HolyGrenade : Skill
    {
        double timeBetween;
        Canvas canvas;
        ImageBrush grenadeSprite; // NEED GRAPHIC

        System.Windows.Shapes.Rectangle grenadeHitBox;
        public HolyGrenade(Canvas canv)
        {
            canvas = canv;
            baseMinDamage = 60; 
            baseMaxDamage = 150;
            minDamage = 60;
            maxDamage = 150;
            baseCooldown = 20; // 20Seconds
            Type = "Offensive";
            isUsingSkill = false;
            canCrit = false;

            grenadeSprite = new ImageBrush();
            grenadeHitBox = new System.Windows.Shapes.Rectangle();
            grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade1.png", UriKind.Absolute)); ;
            grenadeHitBox.Fill = grenadeSprite;
            grenadeHitBox.Width = 200;
            grenadeHitBox.Height = 150;

        }
        public override void updateState(double delta, List<Monster> monsters)
        {

        }
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {

        }

        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {

        }

    }
    
}
