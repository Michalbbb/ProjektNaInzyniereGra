using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace BasicsOfGame
{
    internal partial class Player
    {
        double damageIncreasedPerDebuff;
        double itemQuantity;
        double itemQuality;
        double lifeGainOnHit;
        double overflowingHealing;
        double healthRecoveryRate;
        int criticalHitChance;
        double criticalHitDamage;

        int chanceToInflictBleed;
        double increasedDamage;
        double increasedDamageDueToDebuffs;
        double increasedFireDamage;
        double increasedIceDamage;
        double increasedLightningDamage;
        double increasedNonElementalDotDamage;
        double damageTakenReduction;
        int armour;
        // cooldown base time is result of equation { 100/(100+(cooldown reduction in percent) , 1 by default and never <= 0 }
        double cooldownBaseTime;
        bool isShieldActive;
        bool isImmunityActive;
        double shieldCooldown;
        double immunityCooldown;
        double timeUntilShieldAvailable;
        double timeUntilImmunityAvailable;
        double igniteResistance;
        double shockResistance;
        private int minDmg;
        private int maxDmg;

        double stunResistance;
        double nonElementalDotResistance;
        SkillTree playerPassives;
        private Tuple<int, int> healthPointsToDistribute;
        private Tuple<int, double, double> healthPointsCalculations;
        private Tuple<double, double, double> healthRecoveryRateCalculations;
        private Tuple<int, int, double, double> damageCalculations;
        private Tuple<double, double> iceDamageCalculations;
        private Tuple<double, double> fireDamageCalculations;
        private Tuple<double, double> lightningDamageCalculations;
        private Tuple<double, double, double> criticalDamageCalculations;
        private Tuple<int, double, double> criticalHitChanceCalculations;
        private Tuple<double, double> attackSpeedCalculations;
        private Tuple<double, double> attackCooldownCalculations;
        private Tuple<int, double> chanceToBleedCalculations;
        private Tuple<double, double> increasedNonElementalDotDamageCalculations;
        private Tuple<int, double, double> armourCalculations;
        private Tuple<double, double, double> decreasedDamageTakenCalculations;
        private Tuple<bool, double> shield;
        private Tuple<bool, double> immunity;
        private Tuple<double, double> cooldownTimeForActiveSkillsCalculations;
        private Tuple<double, double> movementSpeedCalculations;
        private Tuple<double, double> itemQuantityCalculations;
        private Tuple<double, double> itemQualityCalculations;
        private Tuple<double, double, double> lifeGainOnHitCalculations;
        private Tuple<double, double> igniteResistanceCalculations;
        private Tuple<double, double> shockResistanceCalculations;
        private Tuple<double, double> nonElementalDotResistanceCalculations;
        private Tuple<double, double> stunResitanceCalculations;
        private Tuple<double, double> damageIncreasedPerDebuffCalculations;
        private void recalculateStats(List<Tuple<string, string, double>> listOfSkills)
        {

            healthPointsCalculations = new Tuple<int, double, double>(200, 0, 0);
            healthRecoveryRateCalculations = new Tuple<double, double, double>(1, 0, 0);
            damageCalculations = new Tuple<int, int, double, double>(minDmg, maxDmg, 0, 0);
            iceDamageCalculations = new Tuple<double, double>(0, 0);
            fireDamageCalculations = new Tuple<double, double>(0, 0);
            lightningDamageCalculations = new Tuple<double, double>(0, 0);
            criticalDamageCalculations = new Tuple<double, double, double>(1.5, 0, 1);
            criticalHitChanceCalculations = new Tuple<int, double, double>(0, 0, 1);
            attackSpeedCalculations = new Tuple<double, double>(30, 0);
            attackCooldownCalculations = new Tuple<double, double>(400, 0);
            chanceToBleedCalculations = new Tuple<int, double>(0, 0);
            increasedNonElementalDotDamageCalculations = new Tuple<double, double>(0, 0);
            armourCalculations = new Tuple<int, double, double>(0, 0, 0);
            decreasedDamageTakenCalculations = new Tuple<double, double, double>(0, 0, 0);
            shield = new Tuple<bool, double>(false, 30);
            immunity = new Tuple<bool, double>(false, 18);
            cooldownTimeForActiveSkillsCalculations = new Tuple<double, double>(1, 0);
            movementSpeedCalculations = new Tuple<double, double>(100, 0);
            itemQuantityCalculations = new Tuple<double, double>(1, 0);
            itemQualityCalculations = new Tuple<double, double>(1, 0);
            lifeGainOnHitCalculations = new Tuple<double, double, double>(0, 0, 1);
            igniteResistanceCalculations = new Tuple<double, double>(1, 0);
            shockResistanceCalculations = new Tuple<double, double>(1, 0);
            nonElementalDotResistanceCalculations = new Tuple<double, double>(1, 0);
            stunResitanceCalculations = new Tuple<double, double>(1, 0);
            damageIncreasedPerDebuffCalculations = new Tuple<double, double>(0, 0);
            foreach (var skills in listOfSkills)
            {
                if (skills.Item1 == "absoluteCriticalHitChance") { criticalHitChanceCalculations = new Tuple<int, double, double>(criticalHitChanceCalculations.Item1, criticalHitChanceCalculations.Item2 + skills.Item3, criticalHitChanceCalculations.Item3); }
                if (skills.Item1 == "armourPerEq") { armourCalculations = new Tuple<int, double, double>(armourCalculations.Item1, armourCalculations.Item2 + playerInventory.howManyEquipped() * skills.Item3, armourCalculations.Item3); }
                if (skills.Item1 == "armour") { armourCalculations = new Tuple<int, double, double>(armourCalculations.Item1, armourCalculations.Item2 + skills.Item3, armourCalculations.Item3); }
                if (skills.Item1 == "attackSpeed") { attackSpeedCalculations = new Tuple<double, double>(attackSpeedCalculations.Item1, attackSpeedCalculations.Item2 + skills.Item3); attackCooldownCalculations = new Tuple<double, double>(attackCooldownCalculations.Item1, attackCooldownCalculations.Item2 + skills.Item3); }
                if (skills.Item1 == "bleedingChance") { chanceToBleedCalculations = new Tuple<int, double>(chanceToBleedCalculations.Item1, chanceToBleedCalculations.Item2 + skills.Item3); }
                if (skills.Item1 == "cooldownReduced") { cooldownTimeForActiveSkillsCalculations = new Tuple<double, double>(cooldownTimeForActiveSkillsCalculations.Item1, cooldownTimeForActiveSkillsCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "criticalDamage") { criticalDamageCalculations = new Tuple<double, double, double>(criticalDamageCalculations.Item1, criticalDamageCalculations.Item2 + skills.Item3, criticalDamageCalculations.Item3); }
                if (skills.Item1 == "damage") { damageCalculations = new Tuple<int, int, double, double>(damageCalculations.Item1, damageCalculations.Item2, damageCalculations.Item3, damageCalculations.Item4 + skills.Item3); }
                if (skills.Item1 == "damagePerDebuff") { damageIncreasedPerDebuffCalculations = new Tuple<double, double>(damageIncreasedPerDebuffCalculations.Item1, damageIncreasedPerDebuffCalculations.Item2 + skills.Item3 / 100); }
                if (skills.Item1 == "decreaseDamageTaken") { decreasedDamageTakenCalculations = new Tuple<double, double, double>(decreasedDamageTakenCalculations.Item1, decreasedDamageTakenCalculations.Item2 + skills.Item3 / 100, decreasedDamageTakenCalculations.Item3); }
                if (skills.Item1 == "fireDamage") { fireDamageCalculations = new Tuple<double, double>(fireDamageCalculations.Item1, fireDamageCalculations.Item2 + skills.Item3 / 100); }
                if (skills.Item1 == "healthRecoveryRate") { healthRecoveryRateCalculations = new Tuple<double, double, double>(healthRecoveryRateCalculations.Item1, healthRecoveryRateCalculations.Item2, healthRecoveryRateCalculations.Item3 + skills.Item3); }
                if (skills.Item1 == "iceDamage") { iceDamageCalculations = new Tuple<double, double>(iceDamageCalculations.Item1, iceDamageCalculations.Item2 + skills.Item3 / 100); }
                if (skills.Item1 == "immunityStack") { immunity = new Tuple<bool, double>(true, skills.Item3 * 1000); } // ms
                if (skills.Item1 == "itemQuality") { itemQualityCalculations = new Tuple<double, double>(itemQualityCalculations.Item1, itemQualityCalculations.Item2 + skills.Item3); }
                if (skills.Item1 == "itemQuantity") { itemQuantityCalculations = new Tuple<double, double>(itemQuantityCalculations.Item1, itemQuantityCalculations.Item2 + skills.Item3); }
                if (skills.Item1 == "lifeGainOnHit") { lifeGainOnHitCalculations = new Tuple<double, double, double>(lifeGainOnHitCalculations.Item1, lifeGainOnHitCalculations.Item2 + skills.Item3, lifeGainOnHitCalculations.Item3); }
                if (skills.Item1 == "lightningDamage") { lightningDamageCalculations = new Tuple<double, double>(lightningDamageCalculations.Item1, lightningDamageCalculations.Item2 + skills.Item3 / 100); }
                if (skills.Item1 == "maximumHealth") { healthPointsCalculations = new Tuple<int, double, double>(healthPointsCalculations.Item1, healthPointsCalculations.Item2 + skills.Item3, healthPointsCalculations.Item3); }
                if (skills.Item1 == "movementSpeed") { movementSpeedCalculations = new Tuple<double, double>(movementSpeedCalculations.Item1, movementSpeedCalculations.Item2 + skills.Item3 / 100); }
                if (skills.Item1 == "nonElementalDotDamage") { increasedNonElementalDotDamageCalculations = new Tuple<double, double>(increasedNonElementalDotDamageCalculations.Item1, increasedNonElementalDotDamageCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "selfIgniteEffect") { igniteResistanceCalculations = new Tuple<double, double>(igniteResistanceCalculations.Item1, igniteResistanceCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "selfNonElementalDotDamageEffect") { nonElementalDotResistanceCalculations = new Tuple<double, double>(nonElementalDotResistanceCalculations.Item1, nonElementalDotResistanceCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "selfShockEffect") { shockResistanceCalculations = new Tuple<double, double>(shockResistanceCalculations.Item1, shockResistanceCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "selfStunEffect") { stunResitanceCalculations = new Tuple<double, double>(stunResitanceCalculations.Item1, stunResitanceCalculations.Item2 + (skills.Item3 / 100)); }
                if (skills.Item1 == "shieldStack") { shield = new Tuple<bool, double>(true, skills.Item3 * 1000); } //ms


            }
            healthPointsToDistribute = new Tuple<int, int>(Convert.ToInt16(healthPointsCalculations.Item2 * (1 + healthPointsCalculations.Item3 / 100)), Convert.ToInt16(healthPointsToDistribute.Item2));
            maxHealthPoints = Convert.ToInt16(Convert.ToDouble(healthPointsCalculations.Item1 + healthPointsCalculations.Item2) * Convert.ToDouble(1 + healthPointsCalculations.Item3 / 100));

            int healthBoost = healthPointsToDistribute.Item1 - healthPointsToDistribute.Item2;
            if (healthBoost <= 0) healthBoost = 0;
            else healthPointsToDistribute = new Tuple<int, int>(healthPointsToDistribute.Item1, healthPointsToDistribute.Item2 + healthBoost);
            healthPoints += healthBoost;
            updateHpBar();
            itemQuality = itemQualityCalculations.Item1 + itemQualityCalculations.Item1 * (itemQualityCalculations.Item2 / 100);
            itemQuantity = itemQuantityCalculations.Item1 + itemQuantityCalculations.Item1 * (itemQuantityCalculations.Item2 / 100);

            minDmg = Convert.ToInt16(Convert.ToInt16(damageCalculations.Item1 + damageCalculations.Item3) * Convert.ToDouble(1 + (damageCalculations.Item4 / 100) + increasedDamageDueToDebuffs));
            maxDmg = Convert.ToInt16(Convert.ToInt16(damageCalculations.Item2 + damageCalculations.Item3) * Convert.ToDouble(1 + (damageCalculations.Item4 / 100) + increasedDamageDueToDebuffs));

            increasedDamage = 1 + damageCalculations.Item4 / 100 + increasedDamageDueToDebuffs;
            healthRecoveryRate = (healthRecoveryRateCalculations.Item1 + healthRecoveryRateCalculations.Item2) * (1 + healthRecoveryRateCalculations.Item3 / 100);
            shockResistance = shockResistanceCalculations.Item1 - shockResistanceCalculations.Item2;
            nonElementalDotResistance = nonElementalDotResistanceCalculations.Item1 - nonElementalDotResistanceCalculations.Item2;
            igniteResistance = igniteResistanceCalculations.Item1 - igniteResistanceCalculations.Item2;
            stunResistance = stunResitanceCalculations.Item1 - stunResitanceCalculations.Item2;
            if (shockResistance < 0) shockResistance = 0;
            if (nonElementalDotResistance < 0) nonElementalDotResistance = 0;
            if (igniteResistance < 0) igniteResistance = 0;

            if (stunResistance < 0) stunResistance = 0;
            cooldownBaseTime = cooldownTimeForActiveSkillsCalculations.Item1 / (cooldownTimeForActiveSkillsCalculations.Item1 + cooldownTimeForActiveSkillsCalculations.Item2);
            increasedFireDamage = fireDamageCalculations.Item1 + fireDamageCalculations.Item2;
            increasedIceDamage = iceDamageCalculations.Item1 + iceDamageCalculations.Item2;
            increasedLightningDamage = lightningDamageCalculations.Item1 + lightningDamageCalculations.Item2;
            increasedNonElementalDotDamage = increasedNonElementalDotDamageCalculations.Item1 + increasedNonElementalDotDamageCalculations.Item2;
            criticalHitChance = Convert.ToInt16((criticalHitChanceCalculations.Item1 + criticalHitChanceCalculations.Item2) * criticalHitChanceCalculations.Item3);
            criticalHitDamage = (criticalDamageCalculations.Item1 + criticalDamageCalculations.Item2 / 100) * criticalDamageCalculations.Item3;
            lifeGainOnHit = (lifeGainOnHitCalculations.Item1 + lifeGainOnHitCalculations.Item2) * lifeGainOnHitCalculations.Item3;
            armour = Convert.ToInt16((armourCalculations.Item1 + armourCalculations.Item2) * (1 + armourCalculations.Item3 / 100));
            baseSpeed = movementSpeedCalculations.Item1 * (1 + movementSpeedCalculations.Item2);
            damageTakenReduction = (decreasedDamageTakenCalculations.Item1 + decreasedDamageTakenCalculations.Item2) * (1 + decreasedDamageTakenCalculations.Item3);
            if (damageTakenReduction > 1) damageTakenReduction = 1;
            isImmunityActive = immunity.Item1;
            immunityCooldown = immunity.Item2;
            isShieldActive = shield.Item1;
            shieldCooldown = shield.Item2;
            damageIncreasedPerDebuff = damageIncreasedPerDebuffCalculations.Item1 + damageIncreasedPerDebuffCalculations.Item2;
            intervalForAttackAnimations = attackSpeedCalculations.Item1 * (attackSpeedCalculations.Item1 / (attackSpeedCalculations.Item1 + (attackSpeedCalculations.Item1 * attackSpeedCalculations.Item2 / 100)));
            attackCooldown = attackCooldownCalculations.Item1 * (attackCooldownCalculations.Item1 / (attackCooldownCalculations.Item1 + (attackCooldownCalculations.Item1 * attackCooldownCalculations.Item2 / 100))); ;
            chanceToInflictBleed = Convert.ToInt16(chanceToBleedCalculations.Item1 + chanceToBleedCalculations.Item2);

            if (isShieldActive)
            {
                if (!isShieldAdded)
                {
                    isShieldAdded = true;

                    shieldPassiveVisual.Width = 40;
                    shieldPassiveVisual.Height = 40;

                    Canvas.SetTop(shieldPassiveVisual, 0);
                    Canvas.SetLeft(shieldPassiveVisual, 1160 - 40 * addedSpecialBuffs);
                    visualForShieldCooldown.Text = "0.0";
                    visualForShieldCooldown.FontSize = 10;
                    visualForShieldCooldown.Height = 15;
                    visualForShieldCooldown.Width = 30;
                    visualForShieldCooldown.Foreground = Brushes.Black;
                    visualForShieldCooldown.IsHitTestVisible = false;
                    visualForShieldCooldown.Background = Brushes.White;
                    visualForShieldCooldown.Opacity = 1;
                    visualForShieldCooldown.TextAlignment = TextAlignment.Center;
                    Canvas.SetZIndex(shieldPassiveVisual, 49);
                    Canvas.SetZIndex(visualForShieldCooldown, 50);
                    Canvas.SetTop(visualForShieldCooldown, 25);
                    Canvas.SetLeft(visualForShieldCooldown, 1165 - 40 * addedSpecialBuffs);
                    GameScreen.Children.Add(shieldPassiveVisual);
                    GameScreen.Children.Add(visualForShieldCooldown);
                    addedSpecialBuffs++;
                }
            }
            if (isImmunityActive)
            {
                if (!isImmunityAdded)
                {
                    isImmunityAdded = true;

                    immunityPassiveVisual.Width = 40;
                    immunityPassiveVisual.Height = 40;

                    Canvas.SetTop(immunityPassiveVisual, 0);
                    Canvas.SetLeft(immunityPassiveVisual, 1160 - 40 * addedSpecialBuffs);

                    visualForImmunityCooldown.Text = "0.0";
                    visualForImmunityCooldown.TextAlignment = TextAlignment.Center;
                    visualForImmunityCooldown.FontSize = 10;
                    visualForImmunityCooldown.Height = 15;
                    visualForImmunityCooldown.Width = 30;
                    visualForImmunityCooldown.Foreground = Brushes.Black;
                    visualForImmunityCooldown.IsHitTestVisible = false;
                    visualForImmunityCooldown.Background = Brushes.White;
                    visualForImmunityCooldown.Opacity = 1;
                    Canvas.SetZIndex(immunityPassiveVisual, 49);
                    Canvas.SetZIndex(visualForImmunityCooldown, 50);
                    Canvas.SetTop(visualForImmunityCooldown, 25);
                    Canvas.SetLeft(visualForImmunityCooldown, 1165 - 40 * addedSpecialBuffs);
                    GameScreen.Children.Add(immunityPassiveVisual);
                    GameScreen.Children.Add(visualForImmunityCooldown);
                    addedSpecialBuffs++;
                }
            }
            List<double> dmgIncreased = new List<double>();
            dmgIncreased.Add(increasedDamage);
            dmgIncreased.Add(increasedFireDamage);
            dmgIncreased.Add(increasedIceDamage);
            dmgIncreased.Add(increasedLightningDamage);
            dmgIncreased.Add(increasedNonElementalDotDamage);

            // HERE IMPLEMENT FOREACH SKILLS LOOP IN FUTURE
            foreach (Skills skill in obtainedSkills)
                skill.recalculateStats(dmgIncreased, cooldownBaseTime);
            foreach (Skills skill in skillsThatCanBeObtained)
                skill.recalculateStats(dmgIncreased, cooldownBaseTime);
            if (showingStats) showEquipment();
        }
        private void updateStats()
        {
            List<Tuple<string, string, double>> temp = playerInventory.getStats();
            List<Tuple<string, string, double>> addMe = playerPassives.getStats();
            temp.AddRange(addMe);
            minDmg = playerInventory.getMinDmg();
            maxDmg = playerInventory.getMaxDmg();
            recalculateStats(temp);
        }
    }
}
