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
        TextBox descActiveSkills;
        bool showingStats = false;
        List<Skills> obtainedSkills;
        List<Skills> skillsThatCanBeObtained;
        int skillsToPick;
        int pickedSkills;
        TextBox descSkillOne;
        System.Windows.Shapes.Rectangle equipmentBackground;

        TextBox descSkillTwo;
        TextBox[] cdSkills = new TextBox[5];
        public static int unassignedSkillPoints;
        public static int assignedSkillPoints;

        System.Windows.Shapes.Rectangle[] visualsSkills = new System.Windows.Shapes.Rectangle[5];
        private void CloseEquipment_Click(object sender, RoutedEventArgs e)
        {
            closeEquipment();
            GameScreen.Focus();
            e.Handled = true;
        }

        private void updateSkills()
        {
            foreach (System.Windows.Shapes.Rectangle vs in visualsSkills)
            {
                GameScreen.Children.Remove(vs);
            }
            foreach (TextBox cs in cdSkills)
            {
                GameScreen.Children.Remove(cs);
            }
            for (int i = 0; i < obtainedSkills.Count; i++)
            {
                visualsSkills[i].Width = 50;
                visualsSkills[i].Height = 50;
                visualsSkills[i].Fill = obtainedSkills[i].getMiniature();
                Canvas.SetLeft(visualsSkills[i], 0 + i * 50);
                Canvas.SetTop(visualsSkills[i], 550);
                Canvas.SetZIndex(visualsSkills[i], 50);
                GameScreen.Children.Add(visualsSkills[i]);

                cdSkills[i].Width = 40;
                cdSkills[i].Height = 20;
                cdSkills[i].Background = Brushes.White;
                cdSkills[i].Foreground = Brushes.Black;
                cdSkills[i].BorderBrush = Brushes.Black;

                Canvas.SetLeft(cdSkills[i], 5 + i * 50);
                Canvas.SetTop(cdSkills[i], 580);
                Canvas.SetZIndex(cdSkills[i], 51);
                cdSkills[i].IsHitTestVisible = false;
                GameScreen.Children.Add(cdSkills[i]);
                cdSkills[i].TextAlignment = TextAlignment.Center;

            }
        }
        private void updateCd()
        {
            for (int i = 0; i < obtainedSkills.Count; i++)
            {
                double x = obtainedSkills[i].getCooldown();
                if (x != 0)
                {
                    if (Math.Round(x, 1) % 1 == 0) cdSkills[i].Text = Math.Round(x, 1).ToString() + ",0";
                    else cdSkills[i].Text = Math.Round(x, 1).ToString();
                }
                else cdSkills[i].Text = "0,0";
            }
        }
        public int returnHowManyActiveSkillsCanBeAllocated()
        {
            return skillsToPick - pickedSkills;
        }
        public bool checkIfCanObtainSkills()
        {
            if (pickedSkills < skillsToPick) return true;
            return false;
        }
        System.Windows.Shapes.Rectangle currentSkillBody;
        int damageOfCurrentSkill;
        int[] statusEffectsForSkill;
        bool canCurrentSkillCrit;
        bool tryingInvoke;
        private void tryDamagingAnyEnemy(System.Windows.Shapes.Rectangle body, int damage, int[] statusEffects, bool canCrit)
        {
            tryingInvoke = true;
            damageOfCurrentSkill = damage;
            statusEffectsForSkill = statusEffects;
            canCurrentSkillCrit = canCrit;
            currentSkillBody = body;
        }
        int savedFirstSkill;
        int savedSecondSkill;
        bool saveSkillsForLater;
        Button firstChoice;
        Button secondChoice;
        bool pickingActive;
        public void pickSkill()
        {
            if (pickingActive) return;
            if (!saveSkillsForLater)
            {
                saveSkillsForLater = true;
                savedFirstSkill = getRand.Next(0, skillsThatCanBeObtained.Count);
                savedSecondSkill = getRand.Next(0, skillsThatCanBeObtained.Count);
                if (savedFirstSkill == savedSecondSkill)
                {
                    while (savedSecondSkill == savedFirstSkill)
                    {
                        savedSecondSkill = getRand.Next(0, skillsThatCanBeObtained.Count - 1);
                    }
                }
            }


            descSkillOne.Text = skillsThatCanBeObtained[savedFirstSkill].getName() + "\n\n" + skillsThatCanBeObtained[savedFirstSkill].returnDescription();
            descSkillTwo.Text = skillsThatCanBeObtained[savedSecondSkill].getName() + "\n\n" + skillsThatCanBeObtained[savedSecondSkill].returnDescription();
            pickingActive = true;
            firstChoice.Content = skillsThatCanBeObtained[savedFirstSkill].getName();
            firstChoice.Click += pickFirstSkill;
            secondChoice.Content = skillsThatCanBeObtained[savedSecondSkill].getName();
            secondChoice.Click += pickSecondSkill;
            firstChoice.Visibility = Visibility.Visible;
            secondChoice.Visibility = Visibility.Visible;
            firstChoice.IsEnabled = true;
            secondChoice.IsEnabled = true;
            Canvas.SetZIndex(firstChoice, 999);
            Canvas.SetZIndex(secondChoice, 999);
            GameScreen.Children.Add(firstChoice);
            GameScreen.Children.Add(secondChoice);
            GameScreen.Children.Add(blackOverlay);
            GameScreen.Children.Add(descActiveSkills);
            GameScreen.Children.Add(descSkillOne);
            GameScreen.Children.Add(descSkillTwo);

        }
        public void closePicker()
        {
            pickingActive = false;
            updateSkills();
            GameScreen.Focus();
            GameScreen.Children.Remove(firstChoice);
            GameScreen.Children.Remove(secondChoice);
            GameScreen.Children.Remove(blackOverlay);
            GameScreen.Children.Remove(descActiveSkills);
            GameScreen.Children.Remove(descSkillOne);
            GameScreen.Children.Remove(descSkillTwo);
            firstChoice.Visibility = Visibility.Hidden;
            secondChoice.Visibility = Visibility.Hidden;
            firstChoice.IsEnabled = false;
            secondChoice.IsEnabled = false;
        }

        private void pickFirstSkill(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            pickedSkills++;
            skillsThatCanBeObtained[savedFirstSkill].tryDamaging += tryDamagingAnyEnemy;
            obtainedSkills.Add(skillsThatCanBeObtained[savedFirstSkill]);
            skillsThatCanBeObtained.RemoveAt(savedFirstSkill);
            saveSkillsForLater = false;
            closePicker();

        }
        private void pickSecondSkill(object sender, RoutedEventArgs e)
        {
            e.Handled = true;


            pickedSkills++;
            skillsThatCanBeObtained[savedSecondSkill].tryDamaging += tryDamagingAnyEnemy;
            obtainedSkills.Add(skillsThatCanBeObtained[savedSecondSkill]);
            skillsThatCanBeObtained.RemoveAt(savedSecondSkill);
            saveSkillsForLater = false;
            closePicker();

        }
        private void fillSkills(Canvas GS)
        {
            skillsThatCanBeObtained.Add(new Fireball(GS));
            skillsThatCanBeObtained.Add(new IceBurst(GS));
            skillsThatCanBeObtained.Add(new FireStorm(GS));
            skillsThatCanBeObtained.Add(new HammerOfJudgment(GS));
            skillsThatCanBeObtained.Add(new HolyGrenade(GS));
            skillsThatCanBeObtained.Add(new StunShock(GS));
            skillsThatCanBeObtained.Add(new HolyHands(GS));
            skillsThatCanBeObtained.Add(new LightningStrike(GS));

        }
        public void useFirstSkill(System.Windows.Point mouse, System.Windows.Point player)
        {
            if (obtainedSkills.Count >= 1)
                obtainedSkills[0].useSkill(mouse, player);
        }
        public void useSecondSkill(System.Windows.Point mouse, System.Windows.Point player)
        {
            if (obtainedSkills.Count >= 2)
                obtainedSkills[1].useSkill(mouse, player);
        }
        public void useThirdSkill(System.Windows.Point mouse, System.Windows.Point player)
        {
            if (obtainedSkills.Count >= 3)
                obtainedSkills[2].useSkill(mouse, player);
        }
        public void useFourthSkill(System.Windows.Point mouse, System.Windows.Point player)
        {
            if (obtainedSkills.Count >= 4)
                obtainedSkills[3].useSkill(mouse, player);
        }
        public void useFifthSkill(System.Windows.Point mouse, System.Windows.Point player)
        {
            if (obtainedSkills.Count >= 5)
                obtainedSkills[4].useSkill(mouse, player);
        }
        public void showEquipment()
        {

            playerInventory.showItems();
            playerStatsHolder.Text = "";
            playerStatsHolder.Text += "Damage: " + minDmg.ToString() + "-" + maxDmg.ToString();
            playerStatsHolder.Text += "\nIncreased damage: " + Math.Round((increasedDamage - 1) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIncreased fire damage: " + Math.Round((increasedFireDamage) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIncreased lightning damage: " + Math.Round((increasedLightningDamage) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIncreased ice damage: " + Math.Round((increasedIceDamage) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIncreased bleeding and poison damage: " + Math.Round((increasedNonElementalDotDamage) * 100).ToString() + "%";
            playerStatsHolder.Text += "\nChance to inflict bleed on hit: " + (chanceToInflictBleed).ToString() + "%";
            playerStatsHolder.Text += "\nAttack speed: " + Math.Round((100 + attackSpeedCalculations.Item2), 0).ToString() + "% of base";
            playerStatsHolder.Text += "\nCritical hit chance: " + criticalHitChance.ToString() + "%";
            playerStatsHolder.Text += "\nCritical hit damage: " + Math.Round((criticalHitDamage * 100), 0).ToString() + "% of non critical damage";
            playerStatsHolder.Text += "\nArmour:" + armour.ToString();
            playerStatsHolder.Text += "\nMax life: " + maxHealthPoints.ToString();
            playerStatsHolder.Text += "\nDamage Reduction from hits: " + Math.Round((damageTakenReduction * 100), 0).ToString() + "%";
            playerStatsHolder.Text += "\nLife recovery rate: " + Math.Round(healthRecoveryRate * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIgnite effect reduction: " + Math.Round(((1 - igniteResistance) * 100), 0).ToString() + "%";
            playerStatsHolder.Text += "\nShock effect reduction: " + Math.Round(((1 - shockResistance) * 100), 0).ToString() + "%";
            playerStatsHolder.Text += "\nStun duration reduction: " + Math.Round(((1 - stunResistance) * 100), 0).ToString() + "%";
            playerStatsHolder.Text += "\nBleed and poison effect reduction: " + Math.Round(((1 - nonElementalDotResistance) * 100), 0).ToString() + "%";
            playerStatsHolder.Text += "\nMovement speed: " + Math.Round(baseSpeed, 0).ToString() + "%";
            playerStatsHolder.Text += "\nCooldown Reduction: " + Math.Round((cooldownTimeForActiveSkillsCalculations.Item2 * 100), 1).ToString() + "%";
            playerStatsHolder.Text += "\nIncrease of item quality dropped: " + Math.Round((itemQuality - 1) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nIncrease of amount of items dropped: " + Math.Round((itemQuantity - 1) * 100, 0).ToString() + "%";
            playerStatsHolder.Text += "\nLife gain per hit(including life recovery rate): " + Math.Round(lifeGainOnHit * healthRecoveryRate, 1).ToString();
            playerStatsHolder.Text += "\nIncreased dmg per debuff on self: " + Math.Round(damageIncreasedPerDebuff * 100, 0).ToString() + "%";

            if (showingStats)
            {
                return;
            }
            else
            {
                showingStats = true;
                GameScreen.Children.Add(equipmentBackground);
                GameScreen.Children.Add(playerStatsHolder);
                GameScreen.Children.Add(closeEquipmentButton);
            }
        }
        public void closeEquipment()
        {
            playerInventory.hideItems();
            GameScreen.Children.Remove(equipmentBackground);
            GameScreen.Children.Remove(playerStatsHolder);
            GameScreen.Children.Remove(closeEquipmentButton);
            showingStats = false;
        }
        private double mercySystem = 1;
        const int NORMAL = 0;
        const int RARE = 1;
        const int EPIC = 2;
        const int LEGENDARY = 3;
        private void generateItem()
        {
            int legendary = 10;
            int epic = 15;
            int rare = 25;
            legendary = (int)(legendary * itemQuality);
            epic = (int)(epic * itemQuality);
            rare = (int)(rare * itemQuality);
            int baseDropChance = 15; // 15 by default 100 for testing purposes
            baseDropChance = (int)(baseDropChance * itemQuantity * mercySystem);
            int itemDropRequiredChance = getRand.Next(0, 100);
            if (itemDropRequiredChance >= baseDropChance)
            {
                mercySystem *= 1.05;
                return;
            }
            mercySystem = 1; // reset, because we got item
            int quality = getRand.Next(0, 100);
            if (quality < legendary)
            {
                int typeOfEquipment = getRand.Next(0, 5);
                playerInventory.addEquipment(new Equipment(typeOfEquipment, LEGENDARY, GameScreen));
            }
            else if (quality < (epic + legendary))
            {
                int typeOfEquipment = getRand.Next(0, 5);
                playerInventory.addEquipment(new Equipment(typeOfEquipment, EPIC, GameScreen));
            }
            else if (quality < (epic + legendary + rare))
            {
                int typeOfEquipment = getRand.Next(0, 5);
                playerInventory.addEquipment(new Equipment(typeOfEquipment, RARE, GameScreen));
            }
            else
            {

                int typeOfEquipment = getRand.Next(0, 5);
                playerInventory.addEquipment(new Equipment(typeOfEquipment, NORMAL, GameScreen));
            }


        }

    }
}
