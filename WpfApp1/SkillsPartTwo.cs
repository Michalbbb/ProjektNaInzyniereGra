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
    internal class HolyGrenade : Skills
    {
        double timeBetween;
        Canvas canvas;
        ImageBrush grenadeSprite; // NEED GRAPHIC
        int sequence;

        System.Windows.Shapes.Rectangle grenadeHitBox;
        public HolyGrenade(Canvas canv)
        {
            name = "Holy grenade";
            canvas = canv;
            baseMinDamage = 60;
            baseMaxDamage = 150;
            minDamage = 60;
            maxDamage = 150;
            baseCooldown = 14; // 14Seconds
            Type = "Offensive";
            isUsingSkill = false;
            canCrit = false;

            grenadeSprite = new ImageBrush();
            grenadeHitBox = new System.Windows.Shapes.Rectangle();
            grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade1.png", UriKind.Absolute)); ;
            grenadeHitBox.Fill = grenadeSprite;
            grenadeHitBox.Width = 60;
            grenadeHitBox.Height = 90;

            sequence = 0;
        }
        public override ImageBrush getMiniature()
        {
            return new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade1.png", UriKind.Absolute)));
        }
        public override string returnDescription()
        {
            string info = $"Spawns a grenade under you that \nexplodes after some time and \ndeals damage to all enemies.\nDeals {minDamage} to {maxDamage} damage.\nCooldown:{Math.Round(cooldown, 2)} seconds";
            return info;
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


                if (sequence == 0 && timeBetween <= 0)
                {

                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade0.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    sequence++;
                    timeBetween = 1.38;

                }
                if (sequence == 1 && timeBetween <= 0)
                {

                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade2.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    sequence++;
                    timeBetween = 0.08;

                }
                if (sequence == 2 && timeBetween <= 0)
                {

                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade3.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    sequence++;
                    timeBetween = 0.08;
                }
                if (sequence == 3 && timeBetween <= 0)
                {

                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade4.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    sequence++;
                    timeBetween = 0.08;



                }
                if (sequence == 4 && timeBetween <= 0)
                {
                    grenadeHitBox.Width = 250;
                    grenadeHitBox.Height = 250;
                    Canvas.SetTop(grenadeHitBox, Canvas.GetTop(grenadeHitBox) - 70);
                    Canvas.SetLeft(grenadeHitBox, Canvas.GetLeft(grenadeHitBox) - 95);
                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade5.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    sequence++;
                    timeBetween = 0.08;
                }
                if (sequence == 5 && timeBetween <= 0)
                {
                    grenadeHitBox.Width = 250;
                    grenadeHitBox.Height = 250;
                    //Canvas.SetTop(grenadeHitBox, Canvas.GetTop(grenadeHitBox) - 70);
                    //Canvas.SetLeft(grenadeHitBox, Canvas.GetLeft(grenadeHitBox) - 95);
                    grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade5.png", UriKind.Absolute)); ;
                    grenadeHitBox.Fill = grenadeSprite;
                    int damageDealt = Skills.rnd.Next(minDamage, maxDamage);

                    tryDamaging(grenadeHitBox, damageDealt, statusEffects, canCrit);

                    sequence++;
                    timeBetween = 0.25;

                }
                if (sequence == 6 && timeBetween <= 0)
                {
                    currentCooldown = cooldown;
                    sequence = 0;
                    isUsingSkill = false;
                    canvas.Children.Remove(grenadeHitBox);
                    return;
                }






            }


        }

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









                grenadeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade1.png", UriKind.Absolute)); ;
                grenadeHitBox.Fill = grenadeSprite;
                grenadeHitBox.Width = 60;
                grenadeHitBox.Height = 90;
                Canvas.SetTop(grenadeHitBox, playerPosition.Y);
                Canvas.SetLeft(grenadeHitBox, playerPosition.X);

                canvas.Children.Add(grenadeHitBox);







            }
        }

    }
    internal class StunShock : Skills
    {

        int shockChance = 100;
        int stunChance = 100;
        int stage = 0;

        double timeBetween;
        ImageBrush StunShockSprite; // NEED GRAPHIC

        System.Windows.Shapes.Rectangle StunShockHitBox;
        Canvas canvas;

        public StunShock(Canvas canv)
        {
            name = "Earthquake";
            canvas = canv;
            baseMinDamage = 0; // Duration of 2 seconds with hit every 100ms yield about 60-140 dmg at max and on averge 10 ignites that deal 4dmg each
            baseMaxDamage = 0;
            minDamage = 0;
            maxDamage = 0;
            baseCooldown = 10; // 10Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Support";
            isUsingSkill = false;
            statusEffects[SHOCK_CHANCE] = shockChance;
            statusEffects[STUN_CHANCE] = stunChance;
            canCrit = false;

            StunShockSprite = new ImageBrush();
            StunShockHitBox = new System.Windows.Shapes.Rectangle();
            Canvas.SetZIndex(StunShockHitBox, 20);
            StunShockHitBox.Fill = Brushes.Black;
            StunShockHitBox.Opacity = 0.1;
            StunShockHitBox.Width = 1200;
            StunShockHitBox.Height = 600;


        }
        public override ImageBrush getMiniature()
        {
            return new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/grenade5.png", UriKind.Absolute)));
        }
        public override string returnDescription()
        {
            string info = $"You are intiating earthquake\nstunning and shocking all enemy.\nDeals {minDamage} to {maxDamage} damage.\nCooldown:{Math.Round(cooldown, 2)} seconds";
            return info;
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
                if (stage == 0)
                {
                    timeBetween = 0.15;
                    stage++;

                }
                if (stage == 1 && timeBetween <= 0)
                {
                    tryDamaging.Invoke(StunShockHitBox, 0, statusEffects, canCrit);
                    isUsingSkill = false;
                    canvas.Children.Remove(StunShockHitBox);
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
            if (!isUsingSkill)
            {
                isUsingSkill = true;
                stage = 0;




                timeBetween = 0;
                StunShockHitBox.Width = 1200;
                Canvas.SetTop(StunShockHitBox, 0);
                Canvas.SetLeft(StunShockHitBox, 0);
                StunShockHitBox.Height = 600;

                canvas.Children.Add(StunShockHitBox);







            }
        }
    }

    internal class HolyHands : Skills
    {



        Canvas canvas;
        public HolyHands(Canvas canv)
        {
            name = "Rejuvenation";
            canvas = canv;
            baseMinDamage = 40;
            baseMaxDamage = 80;
            minDamage = 40;
            maxDamage = 80;
            baseCooldown = 15; // 15Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Support";
            isUsingSkill = false;

            canCrit = false;



        }
        public override ImageBrush getMiniature()
        {
            return new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/passive10.png", UriKind.Absolute)));
        }
        public override string returnDescription()
        {
            string info = $"You are healed by power of nature.\nHeals you by {minDamage} to {maxDamage} health.\nCooldown:{Math.Round(cooldown, 2)} seconds";
            return info;
        }
        public override void updateState(double delta, List<Monster> monsters)
        {
            if (currentCooldown > 0)
            {

                currentCooldown -= delta;
            }
            else if (isUsingSkill)
            {
                isUsingSkill = false;
                currentCooldown = cooldown;
                int dealDamage = Skills.rnd.Next(minDamage, maxDamage);

                Monster.damageOverTime.Add(new Tuple<int, double, string>(dealDamage, 1000, "Heal"));

            }


        }

        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;



        }
        public override void useSkill(System.Windows.Point mousePosition, System.Windows.Point playerPosition)
        {
            if (currentCooldown > 0) return;
            if (!isUsingSkill)
            {
                isUsingSkill = true;








            }
        }

    }

    internal class LightningStrike : Skills
    {
        ImageBrush LightningStrikeSprite; // Currently sprite doesn't exist.
        System.Windows.Shapes.Rectangle LightningStrikeHitbox;
        int shockChance = 100;
        Canvas canvas;
        int sequence;
        double timeBetween;
        string direction;
        public LightningStrike(Canvas canv)
        {
            name = "Lightning Strike";
            canvas = canv;
            baseMinDamage = 1;
            baseMaxDamage = 99;
            minDamage = baseMinDamage;
            maxDamage = baseMaxDamage;
            baseCooldown = 15; // 15Seconds
            cooldown = baseCooldown;
            currentCooldown = 0;
            Type = "Offensive";
            isUsingSkill = false;
            statusEffects[SHOCK_CHANCE] = shockChance;
            canCrit = true;

            LightningStrikeSprite = new ImageBrush();
            LightningStrikeHitbox = new System.Windows.Shapes.Rectangle();
            LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/fireball.png", UriKind.Absolute)); ;
            LightningStrikeHitbox.Fill = LightningStrikeSprite;
            LightningStrikeHitbox.Width = 50;
            LightningStrikeHitbox.Height = 50;

        }
        public override ImageBrush getMiniature()
        {
            return new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/passive12.png", UriKind.Absolute)));
        }
        public override string returnDescription()
        {
            string info = $"Spawns a wall of lightning in direction\nyou are facing that deals damage to every\nhit enemy, shocking them.\nDeals {minDamage} to {maxDamage} damage.\nCooldown:{Math.Round(cooldown, 2)} seconds";
            return info;
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
                    canvas.Children.Add(LightningStrikeHitbox);
                    int damageDealt = Skills.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(LightningStrikeHitbox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;

                }
                if (sequence == 1 && timeBetween <= 0)
                {

                    LightningStrikeHitbox.Width = 120;
                    LightningStrikeHitbox.Height = 75;
                    if (direction == "left")
                    {
                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst2l.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(LightningStrikeHitbox, Canvas.GetLeft(LightningStrikeHitbox) - 45);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;


                    }
                    else
                    {
                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst2.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;
                    }

                    int damageDealt = Skills.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;
                }
                if (sequence == 2 && timeBetween <= 0)
                {

                    LightningStrikeHitbox.Width = 171;
                    LightningStrikeHitbox.Height = 75;
                    if (direction == "left")
                    {
                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst3l.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(LightningStrikeHitbox, Canvas.GetLeft(LightningStrikeHitbox) - 51);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width - 120;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;

                    }
                    else
                    {

                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst3.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox) + 120);
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width - 120;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;
                    }
                    int damageDealt = Skills.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.1;
                    sequence++;
                }
                if (sequence == 3 && timeBetween <= 0)
                {

                    Canvas.SetTop(LightningStrikeHitbox, Canvas.GetTop(LightningStrikeHitbox) - 31);
                    LightningStrikeHitbox.Width = 256;
                    LightningStrikeHitbox.Height = 106;
                    if (direction == "left")
                    {
                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst4l.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(LightningStrikeHitbox, Canvas.GetLeft(LightningStrikeHitbox) - 85);
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox));
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width - 171;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;
                    }
                    else
                    {
                        LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst4.png", UriKind.Absolute)); ;
                        LightningStrikeHitbox.Fill = LightningStrikeSprite;
                        Canvas.SetLeft(modifiedHitBox, Canvas.GetLeft(LightningStrikeHitbox) + 171);
                        Canvas.SetTop(modifiedHitBox, Canvas.GetTop(LightningStrikeHitbox));
                        modifiedHitBox.Width = LightningStrikeHitbox.Width - 171;
                        modifiedHitBox.Height = LightningStrikeHitbox.Height;
                    }
                    int damageDealt = Skills.rnd.Next(minDamage, maxDamage);
                    tryDamaging.Invoke(modifiedHitBox, damageDealt, statusEffects, canCrit);
                    timeBetween = 0.05;
                    sequence++;

                }



                if (sequence == 4 && timeBetween <= 0)
                {

                    isUsingSkill = false;
                    canvas.Children.Remove(LightningStrikeHitbox);
                    currentCooldown = cooldown;
                }
                timeBetween -= delta;

            }


        }

        // Call below function every time any stats get updated
        public override void recalculateStats(List<double> increasedDamageList, double cooldownReduction)
        {
            cooldown = baseCooldown * cooldownReduction;
            double increasedDamage = increasedDamageList[DAMAGE] + increasedDamageList[LIGHTNING_DAMAGE];
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


                Canvas.SetLeft(LightningStrikeHitbox, playerPosition.X);
                Canvas.SetTop(LightningStrikeHitbox, playerPosition.Y);
                //MessageBox.Show(moveByY + "<y x>" + moveByX);

                if (mousePosition.X > playerPosition.X) direction = "right";
                else direction = "left";

                LightningStrikeHitbox.Width = 75;
                Canvas.SetTop(LightningStrikeHitbox, playerPosition.Y - 25);
                LightningStrikeHitbox.Height = 75;
                if (direction == "left")
                {
                    Canvas.SetLeft(LightningStrikeHitbox, playerPosition.X - 75);
                    LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst1l.png", UriKind.Absolute)); ;
                    LightningStrikeHitbox.Fill = LightningStrikeSprite;

                }
                else
                {
                    LightningStrikeSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/ActiveSkills/iceBurst1.png", UriKind.Absolute)); ;
                    LightningStrikeHitbox.Fill = LightningStrikeSprite;
                }







            }
        }
    }
}
