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
        int rotation;
        int speed;
        ImageBrush fireballSprite; // NEED GRAPHIC
        System.Windows.Shapes.Rectangle fireballImg;
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
            fireballImg = new System.Windows.Shapes.Rectangle();
            fireballImg.Fill = Brushes.Black;
            fireballImg.Width = 50;
            fireballImg.Height = 50;

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
                if(distanceToTravel<=0) {; currentCooldown = cooldown; canvas.Children.Remove(fireballImg); isUsingSkill = false; }
                Canvas.SetLeft(fireballImg,Canvas.GetLeft(fireballImg)+speed*moveByX*delta);
                Canvas.SetTop(fireballImg, Canvas.GetTop(fireballImg) + speed * moveByY * delta);
                
                int damageDealt=Skill.rnd.Next(minDamage,maxDamage);
                tryDamaging.Invoke(fireballImg, damageDealt, statusEffects, canCrit);
                if (Skill.hitsToDisappear == 0)
                {
                    isUsingSkill = false;
                    canvas.Children.Remove(fireballImg);
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
                moveByX=(mousePosition.X-playerPosition.X)/speed;
                moveByY=(mousePosition.Y-playerPosition.Y)/speed;
                Canvas.SetLeft(fireballImg, playerPosition.X);
                Canvas.SetTop(fireballImg, playerPosition.Y);
                canvas.Children.Add(fireballImg);
                Skill.hitsToDisappear = 1;

                

                
            }
        }
    }
}
