using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BasicsOfGame
{
    internal partial class Player
    {
        private void attackOmni(double deltaTime, ref Grid map, ref List<TextBox> boxes)
        {
            if (stunned)
            {
                ticksRemaining = 0;
                weapon.Fill = new SolidColorBrush(Colors.Transparent);
                Speed = baseSpeed * deltaTime;
                return;
            }
            if (ticksRemaining == 1)
            {
                checkAttackCollision(ref map, ref boxes);
                ticksRemaining--;
                weapon.Fill = new SolidColorBrush(Colors.Transparent);
                Speed = baseSpeed * deltaTime;
                return;
            }
            if (attackDirection == 1)
            {
                weaponSprite.ImageSource = attackAnimationsR[5 - ticksRemaining];
                weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 2)
            {
                weaponSprite.ImageSource = attackAnimationsL[5 - ticksRemaining];
                weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 3)
            {
                weaponSprite.ImageSource = attackAnimationsD[5 - ticksRemaining];
                weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
            else if (attackDirection == 4)
            {
                weaponSprite.ImageSource = attackAnimationsU[5 - ticksRemaining];
                weapon.Fill = weaponSprite;
                ticksRemaining--;
            }
        }
        private void updateExp()
        {
            while (exp >= 1000)
            {
                exp -= 1000;
                level++;
                double lifeRestoredDueToLevelUp = maxHealthPoints * 0.1;
                healPlayerBy(lifeRestoredDueToLevelUp);
                unassignedSkillPoints++;
                if (level % 5 == 0 && skillsToPick < 5)
                {
                    skillsToPick++;
                }

            }

            expVisualization.Text = "lvl. " + level.ToString();
            expBar.Width = Convert.ToInt32(Convert.ToDouble(exp) * 0.2);

        }
        public void dealDmgToPlayer(int dealtDamage, string monsterName)
        {
            if (isShieldActive && timeUntilShieldAvailable <= 0)
            {
                timeUntilShieldAvailable = shieldCooldown;
                return;
            }
            dealtDamage = Convert.ToInt16(dealtDamage * (1 - damageTakenReduction));
            dealtDamage = Convert.ToInt16(dealtDamage * Convert.ToDouble(1 - Convert.ToDouble(Convert.ToDouble(armour) / 300))); // /3 and /100  

            int obecnyDmg = Convert.ToInt32(playerDmg.Text);
            obecnyDmg += dealtDamage;
            playerDmg.Text = obecnyDmg.ToString();
            playerDmg.Width = Convert.ToInt16(playerDmg.Text.Length) * 20;
            playerDmg.Opacity = 1;
            Canvas.SetLeft(playerDmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (playerDmg.Width / 2));
            Canvas.SetTop(playerDmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - playerDmg.Height);
            healthPoints -= dealtDamage;
            if (healthPoints <= 0)
            {
                isDead = true;
                killedBy = monsterName;
                lastDamage = dealtDamage;
            }
            hpVisualization.Text = healthPoints + "/" + maxHealthPoints;
            double w = Convert.ToDouble(healthPoints) / Convert.ToDouble(maxHealthPoints) * 200;
            if (w < 0) w = 0;
            hpBar.Width = Convert.ToInt32(w);



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

        public void deleteDeadToDot(ref Grid map, ref List<TextBox> boxes)
        {

            List<Monster> updateState = map.rMon();
            for (int j = map.rMon().Count - 1; j >= 0; j--)
            {
                if (map.grid[map.getX(), map.getY()].checkIfDead(updateState[j], ref exp))
                {
                    generateItem();
                    GameScreen.Children.Remove(boxes[j]);
                    boxes.RemoveAt(j);
                    updateExp();
                }
            }
        }
        const int BLEED_CHANCE = 0;
        const int IGNITE_CHANCE = 1;
        const int CHILL_CHANCE = 2;
        const int SHOCK_CHANCE = 3;
        const int POISON_CHANCE = 4;
        const int STUN_CHANCE = 5;

        private void checkAttackCollision(ref Grid map, ref List<TextBox> boxes, System.Windows.Shapes.Rectangle skillBody, int damage, int[] statusEffects, bool canCrit)
        {

            int i = 0;

            if (ignited)
            {
                double igniteEffect = 1 - (0.2 * igniteResistance);
                damage = Convert.ToInt32(damage * igniteEffect);

            }
            List<Monster> updateState = map.rMon();
            foreach (Monster x in updateState)
            {


                int rand = getRand.Next(0, 100); // ONLY 1 check for better optimization
                Rect hitbox = new Rect(Canvas.GetLeft(skillBody), Canvas.GetTop(skillBody), skillBody.ActualWidth, skillBody.ActualHeight); ;
                Rect collisionChecker = new Rect(Canvas.GetLeft(x.getBody()), Canvas.GetTop(x.getBody()), x.getBody().ActualWidth, x.getBody().ActualHeight);
                if (determinateCollision(hitbox, collisionChecker))
                {
                    if (!Skills.Fireball1hit && skillBody.Width == 50 && skillBody.Height == 50) return;
                    if (rand < statusEffects[BLEED_CHANCE])
                    {
                        double dmg = damage * 1.2 * (increasedDamage + increasedNonElementalDotDamage);
                        x.addDot(dmg, 3000, "Bleed");

                    }
                    if (rand < statusEffects[IGNITE_CHANCE])
                    {
                        double dmg = 4 * (increasedDamage + increasedFireDamage);
                        x.addDot(dmg, 2000, "Ignite");

                    }
                    if (rand < statusEffects[CHILL_CHANCE])
                    {
                        double dmg = 0;
                        x.addDot(dmg, 5000, "Chill");

                    }
                    if (rand < statusEffects[SHOCK_CHANCE])
                    {
                        double dmg = 0;
                        x.addDot(dmg, 4000, "Shock");

                    }
                    if (rand < statusEffects[POISON_CHANCE])
                    {
                        double dmg = damage * 1.8 * (increasedDamage + increasedNonElementalDotDamage);
                        x.addDot(dmg, 4000, "Poison");

                    }
                    if (rand < statusEffects[STUN_CHANCE])
                    {
                        double dmg = 0;
                        x.addDot(dmg, 2400, "Stun");

                    }
                    if (canCrit && rand < criticalHitChance)
                    {
                        damage = Convert.ToInt16(damage * criticalHitDamage);
                        x.damageTaken(ref damage);
                        boxes[i].Text = damage.ToString() + "!";
                    }
                    else
                    {
                        x.damageTaken(ref damage);
                        boxes[i].Text = damage.ToString();
                    }




                    boxes[i].Foreground = Brushes.BlanchedAlmond;
                    boxes[i].Width = Convert.ToInt16(boxes[i].Text.Length) * 14;
                    boxes[i].Opacity = 1;
                    Canvas.SetLeft(boxes[i], Canvas.GetLeft(x.getBody()) + (x.getBody().ActualWidth / 2) - (boxes[i].Width / 2));
                    Canvas.SetTop(boxes[i], (Canvas.GetTop(x.getBody()) - (x.getBody().Height - x.getBody().ActualHeight)) - (boxes[i].Height) - 15);


                    healPlayerBy(lifeGainOnHit);
                    if (Skills.Fireball1hit && skillBody.Width == 50 && skillBody.Height == 50) Skills.Fireball1hit = false;
                }


                i++;

            }

            for (int j = i - 1; j >= 0; j--)
            {
                if (map.grid[map.getX(), map.getY()].checkIfDead(updateState[j], ref exp))
                {
                    generateItem();
                    GameScreen.Children.Remove(boxes[j]);
                    boxes.RemoveAt(j);
                    updateExp();
                }
            }
        }
        private void checkAttackCollision(ref Grid map, ref List<TextBox> boxes)
        {

            int i = 0;
            int minDmgAfterCalc = minDmg;
            int maxDmgAfterCalc = maxDmg;
            if (ignited)
            {
                double igniteEffect = 1 - (0.2 * igniteResistance);
                minDmgAfterCalc = Convert.ToInt32(minDmg * igniteEffect);
                maxDmgAfterCalc = Convert.ToInt32(maxDmg * igniteEffect);
            }
            List<Monster> updateState = map.rMon();
            foreach (Monster x in updateState)
            {



                Rect hitbox = new Rect(Canvas.GetLeft(weapon), Canvas.GetTop(weapon), weapon.ActualWidth, weapon.ActualHeight);
                Rect collisionChecker = new Rect(Canvas.GetLeft(x.getBody()), Canvas.GetTop(x.getBody()), x.getBody().ActualWidth, x.getBody().ActualHeight);
                if (determinateCollision(hitbox, collisionChecker))
                {
                    int dealtDmg = getRand.Next(minDmgAfterCalc, maxDmgAfterCalc + 1);
                    if (getRand.Next(0, 100) < chanceToInflictBleed)
                    {
                        double dmg = dealtDmg * 1.2 * (increasedDamage + increasedNonElementalDotDamage);
                        x.addDot(dmg, 4000, "Bleed");
                        boxes[i].Foreground = Brushes.PaleVioletRed;
                    }
                    else
                    {
                        boxes[i].Foreground = Brushes.BlanchedAlmond;
                    }
                    if (getRand.Next(0, 100) < criticalHitChance)
                    { // 0 - 99 < 1 - 100
                        dealtDmg = Convert.ToInt16(dealtDmg * criticalHitDamage);
                        x.damageTaken(ref dealtDmg);
                        boxes[i].Text = dealtDmg.ToString() + "!";
                    }
                    else
                    {
                        x.damageTaken(ref dealtDmg);
                        boxes[i].Text = dealtDmg.ToString();
                    }





                    boxes[i].Width = Convert.ToInt16(boxes[i].Text.Length) * 14;
                    boxes[i].Opacity = 1;
                    Canvas.SetLeft(boxes[i], Canvas.GetLeft(x.getBody()) + (x.getBody().ActualWidth / 2) - (boxes[i].Width / 2));
                    Canvas.SetTop(boxes[i], (Canvas.GetTop(x.getBody()) - (x.getBody().Height - x.getBody().ActualHeight)) - (boxes[i].Height) - 15);


                    healPlayerBy(lifeGainOnHit);
                }


                i++;

            }

            for (int j = i - 1; j >= 0; j--)
            {
                if (map.grid[map.getX(), map.getY()].checkIfDead(updateState[j], ref exp))
                {
                    generateItem();
                    GameScreen.Children.Remove(boxes[j]);
                    boxes.RemoveAt(j);
                    updateExp();
                }
            }
        }
        private void dotUpdate(double deltaTime)
        {
            List<Tuple<int, double, string>> dotUpdater = new List<Tuple<int, double, string>>();
            Monster.update(dotUpdater);
            double dmg;
            double time;
            string dotName;

            foreach (var x in dotUpdater)
            {
                dmg = x.Item1;
                time = x.Item2;
                double dmgPerMs = dmg / 1000;

                dotName = x.Item3;
                if (dotName == "Poison")
                {
                    dmgPerMs = dmgPerMs * nonElementalDotResistance;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, time, dotName));
                    poisonStacks++;
                }
                else if (dotName == "Heal")
                {
                    healPlayerBy(dmg);

                }
                else if (dotName == "Ignite")
                {
                    dmgPerMs = dmgPerMs * igniteResistance;
                    double currentDmg;
                    double elapsedTime;
                    bool foundIgnite = false;
                    for (int i = 0; i < DamagePerMilliseconds.Count; i++)
                    {

                        if (DamagePerMilliseconds[i].Item5 == "Ignite")
                        {
                            foundIgnite = true;
                            time += DamagePerMilliseconds[i].Item4;
                            currentDmg = DamagePerMilliseconds[i].Item2;
                            elapsedTime = DamagePerMilliseconds[i].Item3;
                            if (DamagePerMilliseconds[i].Item1 < dmgPerMs)
                            {
                                dmgPerMs = DamagePerMilliseconds[i].Item1 + dmgPerMs / 2;
                            }

                            DamagePerMilliseconds[i] = new Tuple<double, double, double, double, string>(dmgPerMs, currentDmg, elapsedTime, time, dotName);

                        }
                    }
                    if (!foundIgnite)
                    {
                        DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, time, dotName));
                        ignited = true;
                    }
                }
                else if (dotName == "Stun")
                {
                    if (!stunned)
                    {
                        time = time * stunResistance;
                        if (time == 0) continue;
                        DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(0, 0, 0, time, dotName));
                        Canvas.SetZIndex(buffsContainer[2], 1000);
                        stunned = true;
                        if (rightD)
                        {
                            currentMovementAnimation = 0;

                            playerSprite.ImageSource = rightRun[currentMovementAnimation];
                        }
                        else
                        {
                            currentMovementAnimation = 0;

                            playerSprite.ImageSource = leftRun[currentMovementAnimation];

                        }
                    }

                }
                else { DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, time, dotName)); }
            }

            if (DamagePerMilliseconds.Count > 0)
            {

                List<Tuple<double, double, double, double, string>> toRemove = new List<Tuple<double, double, double, double, string>>();
                if (isImmunityActive && timeUntilImmunityAvailable == 0)
                {
                    Tuple<double, double, double, double, string> LongestDot;
                    LongestDot = DamagePerMilliseconds[0];
                    for (int i = 1; i < DamagePerMilliseconds.Count; i++)
                    {
                        if ((DamagePerMilliseconds[i].Item4 - DamagePerMilliseconds[i].Item3) > (LongestDot.Item4 - LongestDot.Item3))
                            LongestDot = DamagePerMilliseconds[i];
                    }
                    toRemove.Add(LongestDot);
                    timeUntilImmunityAvailable = immunityCooldown;

                }
                foreach (var x in toRemove)
                {

                    if (x.Item5 == "Poison")
                    {
                        poisonStacks--;
                        if (poisonStacks <= 0)
                        {
                            Canvas.SetZIndex(buffsContainer[0], 800);
                            poisonStacks = 0;
                        }
                    }
                    if (x.Item5 == "Ignite")
                    {
                        ignited = false;
                        Canvas.SetZIndex(buffsContainer[1], 800);
                    }
                    if (x.Item5 == "Stun")
                    {
                        stunned = false;
                        Canvas.SetZIndex(buffsContainer[2], 800);
                    }
                    DamagePerMilliseconds.Remove(x);

                }
                toRemove.Clear();
                for (int i = 0; i < DamagePerMilliseconds.Count; i++)
                {

                    if (DamagePerMilliseconds[i].Item5 == "Poison") Canvas.SetZIndex(buffsContainer[0], 1000);
                    if (DamagePerMilliseconds[i].Item5 == "Ignite") Canvas.SetZIndex(buffsContainer[1], 1000);
                    double currentDmg = DamagePerMilliseconds[i].Item2 + DamagePerMilliseconds[i].Item1 * deltaTime * 1000;
                    if (currentDmg >= 1)
                    {
                        int substractMe = Convert.ToInt32(currentDmg);
                        dealDotDmg(substractMe);
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

                    if (x.Item5 == "Poison")
                    {
                        poisonStacks--;
                        if (poisonStacks <= 0)
                        {
                            Canvas.SetZIndex(buffsContainer[0], 800);
                            poisonStacks = 0;
                        }
                    }
                    if (x.Item5 == "Ignite")
                    {
                        ignited = false;
                        Canvas.SetZIndex(buffsContainer[1], 800);
                    }
                    if (x.Item5 == "Stun")
                    {
                        stunned = false;
                        Canvas.SetZIndex(buffsContainer[2], 800);
                    }
                    DamagePerMilliseconds.Remove(x);

                }

                int currentDebuffs = 0;
                for (int i = 0; i < howManyBuffsAndDebuffsImplemented; i++) // 7 debuffs & 7 buffs
                {
                    if (Canvas.GetZIndex(buffsContainer[i]) == 1000)
                    {

                        currentDebuffs++;
                    }
                }

                increasedDamageDueToDebuffs = currentDebuffs * damageIncreasedPerDebuff;
                minDmg = Convert.ToInt16(Convert.ToInt16(damageCalculations.Item1 + damageCalculations.Item3) * Convert.ToDouble(1 + (damageCalculations.Item4 / 100) + increasedDamageDueToDebuffs));
                maxDmg = Convert.ToInt16(Convert.ToInt16(damageCalculations.Item2 + damageCalculations.Item3) * Convert.ToDouble(1 + (damageCalculations.Item4 / 100) + increasedDamageDueToDebuffs));

                increasedDamage = 1 + damageCalculations.Item4 / 100 + increasedDamageDueToDebuffs;



            }


        }
        public void animateAttack(System.Windows.Point mousePosition)
        {
            Speed = 0;
            GameScreen.Focus();
            double CenterXPlayer = Canvas.GetLeft(player) + player.Width / 2;
            double CenterYPlayer = Canvas.GetTop(player) + player.Height / 2;
            double DeltaX = CenterXPlayer - mousePosition.X;
            double DeltaY = CenterYPlayer - mousePosition.Y;
            ticksRemaining = 5;

            if (abs(DeltaX) - abs(DeltaY) >= 0)
            {
                if (DeltaX < 0) // right
                {
                    Canvas.SetLeft(weapon, CenterXPlayer);
                    Canvas.SetTop(weapon, CenterYPlayer - (player.Height * 1.6) / 2);
                    weapon.Width = attackRange;
                    weapon.Height = player.Height * 1.6;
                    attackDirection = 1;

                }
                else // left
                {
                    Canvas.SetLeft(weapon, CenterXPlayer - attackRange);
                    Canvas.SetTop(weapon, CenterYPlayer - (player.Height * 1.6) / 2);
                    weapon.Width = attackRange;
                    weapon.Height = player.Height * 1.6;
                    attackDirection = 2;
                }
            }
            else
            {
                if (DeltaY < 0) // down
                {
                    Canvas.SetLeft(weapon, CenterXPlayer - (player.Height * 1.6) / 2);
                    Canvas.SetTop(weapon, CenterYPlayer);
                    weapon.Width = player.Height * 1.6;
                    weapon.Height = attackRange;
                    attackDirection = 3;

                }
                else // up
                {
                    Canvas.SetLeft(weapon, CenterXPlayer - (player.Height * 1.6) / 2);
                    Canvas.SetTop(weapon, CenterYPlayer - attackRange);
                    weapon.Width = player.Height * 1.6;
                    weapon.Height = attackRange;
                    attackDirection = 4;
                }
            }




        }
        private void dealDotDmg(int x)
        {
            healthPoints -= x;
            hpVisualization.Text = healthPoints + "/" + maxHealthPoints;
            double w = Convert.ToDouble(healthPoints) / Convert.ToDouble(maxHealthPoints) * 200;
            if (w < 0) w = 0;
            hpBar.Width = Convert.ToInt32(w);
        }
    }

}
