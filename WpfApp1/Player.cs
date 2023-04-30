﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using static System.Net.Mime.MediaTypeNames;

namespace BasicsOfGame
{
    internal class Player
    {
        bool godmode = true;
        int level;
        bool ignited;
        bool stunned;
        int poisonStacks;
        bool returnUp, returnDown, returnLeft, returnRight, blockAttack;
        TextBox playerDmg;
        TextBox hpVisualization;
        Random getRand = new Random();
        private int ticksRemaining = 0;
        System.Windows.Shapes.Rectangle player = new System.Windows.Shapes.Rectangle();
        System.Windows.Shapes.Rectangle weapon = new System.Windows.Shapes.Rectangle();
        private double SpeedX, SpeedY, Speed, baseSpeed;
        private int minDmg;
        private int maxDmg;
        public static int unassignedSkillPoints;
        public static int assignedSkillPoints;

        ImageBrush playerSprite = new ImageBrush();
        ImageBrush weaponSprite = new ImageBrush();
        private const int animations = 6;
        BitmapImage[] rightRun = new BitmapImage[animations];
        BitmapImage[] leftRun = new BitmapImage[animations];
        BitmapImage[] attackAnimationsU = new BitmapImage[4];
        BitmapImage[] attackAnimationsD = new BitmapImage[4];
        BitmapImage[] attackAnimationsL = new BitmapImage[4];
        BitmapImage[] attackAnimationsR = new BitmapImage[4];
        int exp;
        private int intervalForAttackAnimations;
        private double ticksDone = 0;
        private int currentMovementAnimation = 0;
        private int attackRange, attackDirection;
        private double unlockAttack = 0;
        private double attackCooldown = 400;
        System.Windows.Shapes.Rectangle hpBar;
        System.Windows.Shapes.Rectangle hpBarWindow;
        System.Windows.Shapes.Rectangle expBar;
        System.Windows.Shapes.Rectangle expBarWindow;
        System.Windows.Shapes.Rectangle DotBar;
        System.Windows.Shapes.Rectangle[] buffsContainer = new System.Windows.Shapes.Rectangle[14];
        TextBox expVisualization;
        int healthPoints;
        int maxHealthPoints;
        bool rightD;
        bool leftDoorExist, rightDoorExist, upDoorExist, downDoorExist;
        const int UPDOOR = 0;
        const int RIGHTDOOR = 1;
        const int DOWNDOOR = 2;
        const int LEFTDOOR = 3;
        int equippedPieces;
        
        double itemQuantity;
        double itemQuality;
        int lifeGainPerHit;
        double healthRecoveryRate;
        int criticalHitChance;
        double criticalHitDamage;

        int chanceToBleed;
        double increasedDamage;
        double increasedFireDamage;
        double increasedIceDamage;
        double increasedLightningDamage;
        double increasedNonElementalDotDamage;
        double damageTakenReduction;
        int armour;
        double cooldownReduction;
        bool isShieldActive;
        bool isImmunityActive;
        double shieldCooldown;
        double immunityCooldown;
        double igniteResistance;
        double shockResistance;
        double stunResistance;
        double nonElementalDotResistance;
        SkillTree playerPassives;
        List<Tuple<double, double, double, double, string>> DamagePerMilliseconds = new List<Tuple<double, double, double, double, string>>();
        // 1. dmg per millisecond 2. accumulated dmg (change everytime dealing dmg from pool ) 3. Time elapsed 4.When remove from list 5.Name
        Canvas GameScreen;
        public static string killedBy;
        public static bool isDead;
        public static int lastDamage;
        // Following tuples have up to 5 arguments : BASE VALUE(Always),FLAT VALUE(optionally),PERCENT VALUE(optionally)
        private Tuple<int,int> healthPointsToDistribute;
        private Tuple<int, double, double> healthPointsCalucalations;
        private Tuple<double, double, double> healthRecoveryRateCalucalations;
        private Tuple<int, int, double, double> damageCalculations;
        private Tuple<double, double> iceDamageCalculations;
        private Tuple<double, double> fireDamageCalculations;
        private Tuple<double, double> lightningDamageCalculations;
        private Tuple<double, double, double> criticalDamageCaluclations;
        private Tuple<int, double, double> crticialHitChanceCaluclations;
        private Tuple<double, double> attackSpeedCalculations;
        private Tuple<double, double> attackCooldownCalculations;
        private Tuple<int, double> chanceToBleedCalculations;
        private Tuple<double, double> increasedNonElementalDotDamageCalculations;
        private Tuple<int, double, double> armourCalculations;
        private Tuple<double,double,double> decreasedDamageTakenCalculations;
        private Tuple<bool, double> shield;
        private Tuple<bool, double> immunity;
        private Tuple<double, double> cooldownTimeForActiveSkillsCalculations;
        private Tuple<int, double> movementSpeedCalculations;
        private Tuple<double, double> itemQuantityCalculations;
        private Tuple<double, double> itemQualityCalculations;
        private Tuple<int, double, double> lifeGainOnHitCalculations;
        private Tuple<double, double> igniteResistanceCalculations;
        private Tuple<double, double> shockResistanceCalculations;
        private Tuple<double, double> nonElementalDotResistanceCalculations;
        private Tuple<double, double> stunResitanceCalculations;






        public Player(Canvas GS)
        {
            chanceToBleed=0;
            healthPointsToDistribute=new Tuple<int, int>(0,0);
            increasedDamage=0;
            increasedFireDamage=0;
            increasedIceDamage=0;
            increasedLightningDamage=0;
            increasedNonElementalDotDamage=0;
            damageTakenReduction=0;
            armour=0;
            cooldownReduction=0;
            isShieldActive=false;
            isImmunityActive=false;
            shieldCooldown = 30;
            immunityCooldown = 18;
            igniteResistance = 1;
            shockResistance = 1;
            stunResistance = 1;
            nonElementalDotResistance = 1;
            equippedPieces = 0;
            itemQuantity = 1;
            itemQuality = 1;
            healthRecoveryRate = 1.0;
            criticalHitDamage = 2.0;
            criticalHitChance = 0;
            lifeGainPerHit = 0;

            healthPointsCalucalations=new Tuple<int,double, double>(200,0,0);
            healthRecoveryRateCalucalations=new Tuple<double, double, double>(1,0,0);
            damageCalculations=new Tuple<int,int,double,double>(10, 15, 0, 0);
            iceDamageCalculations=new Tuple<double, double>(0,0);
            fireDamageCalculations = new Tuple<double, double>(0, 0);
            lightningDamageCalculations = new Tuple<double, double>(0, 0);
            criticalDamageCaluclations=new Tuple<double, double,double>(2,0,0);
            crticialHitChanceCaluclations=new Tuple<int, double, double>(0,0,0);
            attackSpeedCalculations = new Tuple<double, double>(30, 0);
            attackCooldownCalculations = new Tuple<double, double>(400, 0);
            chanceToBleedCalculations=new Tuple<int, double>(0,0);
            increasedNonElementalDotDamageCalculations=new Tuple<double, double>(0,0);
            armourCalculations=new Tuple<int, double, double>(0,0,0);
            decreasedDamageTakenCalculations=new Tuple<double, double, double>(0,0,0);
            shield=new Tuple<bool, double>(false,30);
            immunity=new Tuple<bool, double>(false,18);
            cooldownTimeForActiveSkillsCalculations=new Tuple<double, double>(0,0);
            movementSpeedCalculations=new Tuple<int, double>(100,0);
            itemQuantityCalculations=new Tuple<double, double>(1,0);
            itemQualityCalculations = new Tuple<double, double>(1, 0);
            lifeGainOnHitCalculations = new Tuple<int, double, double>(0, 0, 0);
            igniteResistanceCalculations = new Tuple<double, double>(1, 0);
            shockResistanceCalculations = new Tuple<double, double>(1, 0);
            nonElementalDotResistanceCalculations = new Tuple<double, double>(1, 0);
            stunResitanceCalculations = new Tuple<double, double>(1, 0);

            unassignedSkillPoints = 0;
            assignedSkillPoints = 0;
            killedBy = "Damage over time";
            playerPassives = new SkillTree(GS);
            playerPassives.updateStats += updateStats;
            isDead = false;
            lastDamage = 1;
            level = 1;
            stunned = false;
            poisonStacks = 0;
            GameScreen = GS;
            healthPoints = 200;
            maxHealthPoints = 200;
            attackRange = 100;
            intervalForAttackAnimations = 30;
            exp = 0;
            minDmg = 10;
            maxDmg = 15;
            if (godmode) { minDmg = 1000; maxDmg = 1500; healthPoints = 20000; maxHealthPoints = 20000; unassignedSkillPoints = 10; level = 10; }
            Speed = 100;
            baseSpeed = 100;
            player.Name = "Player";
            player.Width = 88;
            player.Height = 109;
            Canvas.SetLeft(player, 276);
            Canvas.SetTop(player, 170);
            GameScreen.Children.Add(player);
            weapon.Name = "Weapon";
            weapon.Width = 1;
            weapon.Height = 1;
            Canvas.SetLeft(weapon, 0);
            Canvas.SetTop(weapon, 0);
            Canvas.SetZIndex(weapon, 15);
            weapon.Fill = Brushes.Transparent;
            GameScreen.Children.Add(weapon);
            for (int i = 0; i < animations; i++)
            {
                leftRun[i] = new BitmapImage();
                leftRun[i].BeginInit();
                leftRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}l.png", UriKind.Absolute);
                leftRun[i].EndInit();
                rightRun[i] = new BitmapImage();
                rightRun[i].BeginInit();
                rightRun[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/mainCharacter0{1 + i}.png", UriKind.Absolute);
                rightRun[i].EndInit();

            }
            initializeAnimationsForAttack();
            DamagePerMilliseconds = new List<Tuple<double, double, double, double, string>>();
            playerSprite.ImageSource = rightRun[0];
            rightD = true;
            player.Fill = playerSprite;
            makePlayerTB();
            createHpBar();
            createExpBar();
            createDotBar();
            activeBuffs();

        }
        private void recalculateStats(List<Tuple<string, string, double>> listOfSkills)
        {
            healthPointsCalucalations = new Tuple<int, double, double>(200, 0, 0);
            healthRecoveryRateCalucalations = new Tuple<double, double, double>(1, 0, 0);
            damageCalculations = new Tuple<int, int, double, double>(10, 15, 0, 0);
            iceDamageCalculations = new Tuple<double, double>(0, 0);
            fireDamageCalculations = new Tuple<double, double>(0, 0);
            lightningDamageCalculations = new Tuple<double, double>(0, 0);
            criticalDamageCaluclations = new Tuple<double, double, double>(2, 0, 0);
            crticialHitChanceCaluclations = new Tuple<int, double, double>(0, 0, 0);
            attackSpeedCalculations = new Tuple<double, double>(30, 0);
            attackCooldownCalculations = new Tuple<double, double>(400, 0);
            chanceToBleedCalculations = new Tuple<int, double>(0, 0);
            increasedNonElementalDotDamageCalculations = new Tuple<double, double>(0, 0);
            armourCalculations = new Tuple<int, double, double>(0, 0, 0);
            decreasedDamageTakenCalculations = new Tuple<double, double, double>(0, 0, 0);
            shield = new Tuple<bool, double>(false, 30);
            immunity = new Tuple<bool, double>(false, 18);
            cooldownTimeForActiveSkillsCalculations = new Tuple<double, double>(0, 0);
            movementSpeedCalculations = new Tuple<int, double>(100, 0);
            itemQuantityCalculations = new Tuple<double, double>(1, 0);
            itemQualityCalculations = new Tuple<double, double>(1, 0);
            lifeGainOnHitCalculations = new Tuple<int, double, double>(0, 0, 0);
            igniteResistanceCalculations = new Tuple<double, double>(1, 0);
            shockResistanceCalculations = new Tuple<double, double>(1, 0);
            nonElementalDotResistanceCalculations = new Tuple<double, double>(1, 0);
            stunResitanceCalculations = new Tuple<double, double>(1, 0);

            foreach(var skills in listOfSkills)
            {
                if(skills.Item1 == "absoluteCriticalHitChance"){ crticialHitChanceCaluclations= new Tuple<int, double, double>(crticialHitChanceCaluclations.Item1, crticialHitChanceCaluclations.Item2+skills.Item3, crticialHitChanceCaluclations.Item3);}
                if(skills.Item1 == "armourPerEq") { armourCalculations = new Tuple<int, double, double>(armourCalculations.Item1, armourCalculations.Item2+equippedPieces*skills.Item3, armourCalculations.Item3); }
                if(skills.Item1 == "attackSpeed") { attackSpeedCalculations = new Tuple<double, double>(attackSpeedCalculations.Item1, attackSpeedCalculations.Item2+skills.Item3); attackCooldownCalculations = new Tuple<double, double>(attackCooldownCalculations.Item1, attackCooldownCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "bleedingChance") { chanceToBleedCalculations = new Tuple<int, double>(chanceToBleedCalculations.Item1, chanceToBleedCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "cooldownReduced") { cooldownTimeForActiveSkillsCalculations = new Tuple<double, double>(cooldownTimeForActiveSkillsCalculations.Item1, cooldownTimeForActiveSkillsCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "criticalDamage") { criticalDamageCaluclations = new Tuple<double, double, double>(criticalDamageCaluclations.Item1, criticalDamageCaluclations.Item2, criticalDamageCaluclations.Item3+skills.Item3); }
                if(skills.Item1 == "damage") { damageCalculations = new Tuple<int, int, double, double>(damageCalculations.Item1, damageCalculations.Item2, damageCalculations.Item3, damageCalculations.Item4+skills.Item3); }
                if(skills.Item1 == "damagePerDebuff") {} // NOT IMPLEMENTED YET
                if(skills.Item1 == "decreaseDamageTaken") { decreasedDamageTakenCalculations = new Tuple<double, double, double>(decreasedDamageTakenCalculations.Item1, decreasedDamageTakenCalculations.Item2, decreasedDamageTakenCalculations.Item3+skills.Item3); }
                if(skills.Item1 == "fireDamage") { fireDamageCalculations = new Tuple<double, double>(fireDamageCalculations.Item1,fireDamageCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "healthRecoveryRate") { healthRecoveryRateCalucalations = new Tuple<double, double, double>(healthRecoveryRateCalucalations.Item1,healthRecoveryRateCalucalations.Item2,healthRecoveryRateCalucalations.Item3+skills.Item3); }
                if(skills.Item1 == "iceDamage") { iceDamageCalculations = new Tuple<double, double>(iceDamageCalculations.Item1,iceDamageCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "immunityStack") { immunity = new Tuple<bool, double>(true, skills.Item3*1000); } // ms
                if(skills.Item1 == "itemQuality") { itemQualityCalculations = new Tuple<double, double>(itemQualityCalculations.Item1,itemQualityCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "itemQuantity") { itemQuantityCalculations = new Tuple<double, double>(itemQuantityCalculations.Item1,itemQuantityCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "lifeGainOnHit") { lifeGainOnHitCalculations = new Tuple<int, double, double>(lifeGainOnHitCalculations.Item1,lifeGainOnHitCalculations.Item2+skills.Item3,lifeGainOnHitCalculations.Item3); }
                if(skills.Item1 == "lightningDamage") { lightningDamageCalculations = new Tuple<double, double>(lightningDamageCalculations.Item1,lightningDamageCalculations.Item1+skills.Item3); }
                if(skills.Item1 == "maximumHealth") { healthPointsCalucalations = new Tuple<int, double, double>(healthPointsCalucalations.Item1,healthPointsCalucalations.Item2+skills.Item3,healthPointsCalucalations.Item3); }
                if(skills.Item1 == "movementSpeed") { movementSpeedCalculations = new Tuple<int, double>(movementSpeedCalculations.Item1,movementSpeedCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "nonElementalDotDamage") { increasedNonElementalDotDamageCalculations = new Tuple<double, double>(increasedNonElementalDotDamageCalculations.Item1,increasedNonElementalDotDamageCalculations.Item2+skills.Item3); }
                if(skills.Item1 == "selfIgniteEffect") { igniteResistanceCalculations = new Tuple<double, double>(igniteResistanceCalculations.Item1,igniteResistanceCalculations.Item2+(skills.Item3/100)); }
                if(skills.Item1 == "selfNonElementalDotDamageEffect") { nonElementalDotResistanceCalculations = new Tuple<double, double>(nonElementalDotResistanceCalculations.Item1,nonElementalDotResistanceCalculations.Item2+(skills.Item3/100)); }
                if(skills.Item1 == "selfShockEffect") { shockResistanceCalculations = new Tuple<double, double>(shockResistanceCalculations.Item1,shockResistanceCalculations.Item2+(skills.Item3/100)); }
                if(skills.Item1 == "selfStunEffect") { stunResitanceCalculations = new Tuple<double, double>(stunResitanceCalculations.Item1,stunResitanceCalculations.Item2+(skills.Item3/100)); }
                if(skills.Item1 == "shieldStack") { shield = new Tuple<bool, double>(true, skills.Item3); }
                

            }
            healthPointsToDistribute=new Tuple<int, int>(Convert.ToInt32(healthPointsCalucalations.Item2*(1+healthPointsCalucalations.Item3/100)),healthPointsToDistribute.Item2);
            maxHealthPoints=Convert.ToInt32(Convert.ToDouble(healthPointsCalucalations.Item1+healthPointsCalucalations.Item2)*(1+healthPointsCalucalations.Item3/100));
            int healthBoost=healthPointsToDistribute.Item1-healthPointsToDistribute.Item2;
            if(healthBoost<0)healthBoost=0;
            else healthPointsToDistribute=new Tuple<int, int>(healthPointsToDistribute.Item1,healthPointsToDistribute.Item2+healthBoost);
            healthPoints+=healthBoost;
            updateHpBar();
            itemQuality=itemQualityCalculations.Item1+itemQualityCalculations.Item1*(itemQualityCalculations.Item2/100);
            itemQuantity=itemQuantityCalculations.Item1+itemQuantityCalculations.Item1*(itemQuantityCalculations.Item2/100);
            
            minDmg=Convert.ToInt32(Convert.ToInt32(damageCalculations.Item1+damageCalculations.Item3)*Convert.ToDouble(1+damageCalculations.Item4/100));
            maxDmg=Convert.ToInt32(Convert.ToInt32(damageCalculations.Item2+damageCalculations.Item3)*Convert.ToDouble(1+damageCalculations.Item4/100));

            healthRecoveryRate=(healthRecoveryRateCalucalations.Item1+healthRecoveryRateCalucalations.Item2)*(1+healthRecoveryRateCalucalations.Item3/100);
            shockResistance=shockResistanceCalculations.Item1-shockResistanceCalculations.Item2;
            nonElementalDotResistance=nonElementalDotResistanceCalculations.Item1-nonElementalDotResistanceCalculations.Item2;
            igniteResistance=igniteResistanceCalculations.Item1-igniteResistanceCalculations.Item2;
            stunResistance=stunResitanceCalculations.Item1-stunResitanceCalculations.Item2;
            if(shockResistance<0)shockResistance=0;
            if(nonElementalDotResistance<0)nonElementalDotResistance=0;
            if(igniteResistance<0)igniteResistance=0;
            if(shockResistance<0)shockResistance=0;
            if(stunResistance<0)stunResistance=0;
            
        }
        
        private void updateHpBar(){
            if(healthPoints>maxHealthPoints)healthPoints=maxHealthPoints;
            hpVisualization.Text = healthPoints + "/" + maxHealthPoints;
            double w = Convert.ToDouble(healthPoints) / Convert.ToDouble(maxHealthPoints) * 200;
            if (w < 0) w = 0;
            hpBar.Width = Convert.ToInt32(w);
        }
        private void updateStats(List<Tuple<string,string,double>> listOfSkills)
        {
            recalculateStats(listOfSkills);
        }
        private void initializeAnimationsForAttack()
        {
            // Ładowanie klatek do animacji ataku

            for (int i = 0; i < 4; i++)
            {
                attackAnimationsU[i] = new BitmapImage();
                attackAnimationsU[i].BeginInit();
                attackAnimationsU[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}u.png", UriKind.Absolute);
                attackAnimationsU[i].EndInit();
                attackAnimationsD[i] = new BitmapImage();
                attackAnimationsD[i].BeginInit();
                attackAnimationsD[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}d.png", UriKind.Absolute);
                attackAnimationsD[i].EndInit();
                attackAnimationsL[i] = new BitmapImage();
                attackAnimationsL[i].BeginInit();
                attackAnimationsL[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}l.png", UriKind.Absolute);
                attackAnimationsL[i].EndInit();
                attackAnimationsR[i] = new BitmapImage();
                attackAnimationsR[i].BeginInit();
                attackAnimationsR[i].UriSource = new Uri($"pack://application:,,,/BasicsOfGame;component/images/att{1 + i}p.png", UriKind.Absolute);
                attackAnimationsR[i].EndInit();





            }
        }
        public double abs(double x)
        {
            if (x < 0) return -x;
            else return x;

        }
        private void makePlayerTB()
        {
            playerDmg = new TextBox();
            playerDmg.Width = 25;
            playerDmg.Height = 35;
            playerDmg.FontSize = 30;
            playerDmg.Text = "0";
            playerDmg.Opacity = 0;
            playerDmg.Foreground = Brushes.Red;

            playerDmg.Background = Brushes.Black;
            playerDmg.BorderBrush = Brushes.Transparent;

            playerDmg.IsEnabled = false;
            GameScreen.Children.Add(playerDmg);
            Canvas.SetZIndex(playerDmg, 999);
        }
        private void createExpBar()
        {
            expBar = new System.Windows.Shapes.Rectangle();
            expBar.Width = 0;
            expBar.Height = 15;
            expBar.Fill = Brushes.OrangeRed;
            Canvas.SetLeft(expBar, 5);
            Canvas.SetTop(expBar, 30);
            Canvas.SetZIndex(expBar, 910);
            expBarWindow = new System.Windows.Shapes.Rectangle();
            expBarWindow.Width = 210;
            expBarWindow.Height = 25;
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/playerAssets/hpBar.png", UriKind.Absolute));
            expBarWindow.Fill = sprite;
            Canvas.SetLeft(expBarWindow, 0);
            Canvas.SetTop(expBarWindow, 25);
            Canvas.SetZIndex(expBarWindow, 880);
            expVisualization = new TextBox();
            Canvas.SetLeft(expVisualization, 75);
            Canvas.SetTop(expVisualization, 25);
            Canvas.SetZIndex(expVisualization, 990);
            expVisualization.IsEnabled = false;
            expVisualization.Foreground = new SolidColorBrush(Colors.White);
            expVisualization.Background = Brushes.Transparent;
            expVisualization.BorderThickness = new Thickness(0, 0, 0, 0);
            expVisualization.FontSize = 15;
            expVisualization.FontWeight = FontWeights.Bold;
            expVisualization.Opacity = 1;
            expVisualization.Text = "lvl. " + level.ToString();
            GameScreen.Children.Add(expBar);
            GameScreen.Children.Add(expBarWindow);
            GameScreen.Children.Add(expVisualization);
        }
        private void createDotBar()
        {
            /*
             GroupBox DotBar = new GroupBox();
             DotBar.Header = "Buffs & Debuffs";
             DotBar.Foreground= new SolidColorBrush(Colors.Blue);
             //DotBar.Background= new SolidColorBrush(Colors.Black);
             DotBar.BorderBrush= new SolidColorBrush(Colors.Blue);
             */
            DotBar = new System.Windows.Shapes.Rectangle();
            Canvas.SetLeft(DotBar, 210);
            Canvas.SetTop(DotBar, 0);
            Canvas.SetZIndex(DotBar, 880);
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/playerAssets/hpBar.png", UriKind.Absolute));
            DotBar.Width = 210;
            DotBar.Height = 50;
            DotBar.Fill = sprite;
            GameScreen.Children.Add(DotBar);
        }
        private void activeBuffs()//poison
        {


            for (int i = 0; i < 3; i++)
            {
                buffsContainer[i] = new System.Windows.Shapes.Rectangle();
                buffsContainer[i].Width = 30;
                buffsContainer[i].Height = 15;
                Canvas.SetLeft(buffsContainer[i], 210 + i * 30);
                Canvas.SetTop(buffsContainer[i], 5);
                Canvas.SetZIndex(buffsContainer[i], 800);
                ImageBrush sprite = new ImageBrush();
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/Buffs_Debuffs/buff{i + 1}.png", UriKind.Absolute));
                buffsContainer[i].Fill = sprite;
                GameScreen.Children.Add(buffsContainer[i]);

            }


        }

        private void createHpBar()
        {

            hpBar = new System.Windows.Shapes.Rectangle();
            hpBar.Width = 200;
            hpBar.Height = 15;//60%
            hpBar.Fill = Brushes.DarkRed;
            Canvas.SetLeft(hpBar, 5);
            Canvas.SetTop(hpBar, 5);
            Canvas.SetZIndex(hpBar, 910);
            hpBarWindow = new System.Windows.Shapes.Rectangle();
            hpBarWindow.Width = 210;
            hpBarWindow.Height = 25;//hpBarWindow.Height = 35
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/playerAssets/hpBar.png", UriKind.Absolute));
            hpBarWindow.Fill = sprite;
            Canvas.SetLeft(hpBarWindow, 0);
            Canvas.SetTop(hpBarWindow, 0);
            Canvas.SetZIndex(hpBarWindow, 880);
            hpVisualization = new TextBox();
            Canvas.SetLeft(hpVisualization, 65);
            Canvas.SetTop(hpVisualization, 0);
            Canvas.SetZIndex(hpVisualization, 990);
            hpVisualization.IsEnabled = false;
            hpVisualization.Foreground = new SolidColorBrush(Colors.LightBlue);
            hpVisualization.Background = Brushes.Transparent;
            hpVisualization.BorderThickness = new Thickness(0, 0, 0, 0);
            hpVisualization.FontSize = 15;
            hpVisualization.FontWeight = FontWeights.Bold;
            hpVisualization.Opacity = 1;
            hpVisualization.Text = healthPoints + "/" + maxHealthPoints;
            GameScreen.Children.Add(hpVisualization);
            GameScreen.Children.Add(hpBar);
            GameScreen.Children.Add(hpBarWindow);
        }
        public void checkOpacity()
        {
            if (playerDmg.Opacity > 0) { playerDmg.Opacity -= Speed / 200; }
            else playerDmg.Text = "0";
            Canvas.SetLeft(playerDmg, Canvas.GetLeft(player) + (player.ActualWidth / 2) - (playerDmg.Width / 2));
            Canvas.SetTop(playerDmg, (Canvas.GetTop(player) - (player.Height - player.ActualHeight)) - playerDmg.Height);
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
                    dmgPerMs=dmgPerMs*nonElementalDotResistance;
                    DamagePerMilliseconds.Add(new Tuple<double, double, double, double, string>(dmgPerMs, 0, 0, time, dotName));
                    poisonStacks++;
                }
                else if (dotName == "Ignite")
                {
                    dmgPerMs=dmgPerMs*igniteResistance;
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
                        time=time*stunResistance;
                        if(time==0)continue;
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
                        if (poisonStacks == 0) Canvas.SetZIndex(buffsContainer[0], 800);
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




            }


        }
        public void startPosition(ref Grid map)
        {
            map.goTo(GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, RIGHTDOOR);
        }
        private void checkCollision(bool UpKey, bool DownKey, bool RightKey, bool LeftKey)
        {
            if (Speed == 0) return;

            if (UpKey && !stunned)
            {
                if (((Canvas.GetLeft(player) > 960) && (Canvas.GetTop(player) < 170)) && (Canvas.GetLeft(player) - Canvas.GetTop(player) > 1000))
                {


                }
                else SpeedY -= Speed;
            }

            if (DownKey && !stunned)
            {

                SpeedY += Speed;
            }

            if (RightKey && !stunned)
            {
                if (((Canvas.GetLeft(player) > 960) && (Canvas.GetTop(player) < 170)) && (Canvas.GetLeft(player) - Canvas.GetTop(player) > 1000))
                {


                }
                else
                {
                    if (!rightD)
                    {

                        rightD = true;
                        currentMovementAnimation = 0;
                        playerSprite.ImageSource = rightRun[0];
                    }
                    SpeedX += Speed;
                }

            }
            if (LeftKey && !stunned)
            {

                SpeedX -= Speed;
                if (rightD)
                {

                    rightD = false;
                    currentMovementAnimation = 0;
                    playerSprite.ImageSource = leftRun[0];
                }

            }
            foreach (var x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == "collision") // Zaawansowana kolizja
                {

                    Rect playerHitBoxU = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player) - Speed * 3, player.Width, player.Height);
                    Rect playerHitBoxD = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player) + Speed * 3, player.Width, player.Height);
                    Rect playerHitBoxR = new Rect(Canvas.GetLeft(player) + Speed * 3, Canvas.GetTop(player), player.Width, player.Height);
                    Rect playerHitBoxL = new Rect(Canvas.GetLeft(player) - Speed * 3, Canvas.GetTop(player), player.Width, player.Height);
                    Rect playerHitBoxUltimate = new Rect(Canvas.GetLeft(player) + SpeedX * 3, Canvas.GetTop(player) + SpeedY * 3, player.Width, player.Height);

                    Rect collisionChecker = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (!determinateCollision(playerHitBoxUltimate, collisionChecker)) continue;

                    if (UpKey) if (determinateCollision(playerHitBoxU, collisionChecker)) { SpeedY = 0; }
                    if (DownKey) if (determinateCollision(playerHitBoxD, collisionChecker)) { SpeedY = 0; }
                    if (LeftKey) if (determinateCollision(playerHitBoxL, collisionChecker)) { SpeedX = 0; }
                    if (RightKey) if (determinateCollision(playerHitBoxR, collisionChecker)) { SpeedX = 0; }

                }


            }


        }
        public void generateTB(string tag, ref List<TextBox> boxes) // Text Boxes
        {
            List<TextBox> removeTB = new List<TextBox>();
            int howMany = 0;
            foreach (TextBox x in GameScreen.Children.OfType<TextBox>())
            {
                if ((string)x.Tag == "dmgTakenByEnemy") { removeTB.Add(x); howMany++; }
            }
            for (int j = howMany - 1; j >= 0; j--)
            {
                GameScreen.Children.Remove(removeTB[j]);

            }

            int i = 0;
            foreach (System.Windows.Shapes.Rectangle x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>())
            {
                if ((string)x.Tag == tag) i++;
            }
            boxes = new List<TextBox>();


            for (int j = 0; j < i; j++)
            {

                TextBox addMeToList = new TextBox();
                addMeToList.Width = 30;
                addMeToList.Height = 25;
                addMeToList.FontSize = 20;

                addMeToList.Text = "0";
                addMeToList.Opacity = 0;
                addMeToList.Tag = "dmgTakenByEnemy";
                addMeToList.Foreground = Brushes.BlanchedAlmond;
                addMeToList.Background = Brushes.Transparent;
                addMeToList.BorderBrush = Brushes.Transparent;

                addMeToList.IsEnabled = false;
                GameScreen.Children.Add(addMeToList);
                Canvas.SetZIndex(addMeToList, 999);
                boxes.Add(addMeToList);

            }



        }
        public void gameTick(ScrollViewer Camera, bool UpKey, bool DownKey, bool RightKey, bool LeftKey, ref Grid map, double deltaTime, double Friction, ref List<TextBox> boxes, Action updateMiniMap)
        {
            if (Speed != 0) Speed = baseSpeed * deltaTime;
            ticksDone += baseSpeed / 2 * deltaTime;

            if (blockAttack)
            {

                unlockAttack += Convert.ToDouble(1000 * deltaTime);

                if (unlockAttack / (intervalForAttackAnimations * (6 - ticksRemaining)) >= 1 && ticksRemaining > 0)
                {

                    attackOmni(deltaTime, ref map, ref boxes);
                }
                if (unlockAttack > attackCooldown) { blockAttack = false; unlockAttack = 0; ticksRemaining = 0; }

            }

            Canvas.SetZIndex(player, Convert.ToInt32((Canvas.GetTop(player) + player.Height) / 100));
            double rectLeft = Canvas.GetLeft(player);
            double rectTop = Canvas.GetTop(player);
            double rectRight = rectLeft + player.Width;
            double rectBottom = rectTop + player.Height;


            double viewportHeight = Camera.ViewportHeight;
            double viewportWidth = Camera.ViewportWidth;

            double horizontalOffset = (rectLeft + rectRight - viewportWidth) / 2;
            double verticalOffset = (rectTop + rectBottom - viewportHeight) / 2;


            Camera.ScrollToHorizontalOffset(horizontalOffset);
            Camera.ScrollToVerticalOffset(verticalOffset);
            checkCollision(UpKey, DownKey, RightKey, LeftKey);
            if ((UpKey || DownKey || RightKey || LeftKey) && !stunned)
            {
                if (ticksDone >= 10 / Speed)
                {
                    if (rightD)
                    {
                        currentMovementAnimation++;
                        if (currentMovementAnimation == animations) currentMovementAnimation = 0;
                        playerSprite.ImageSource = rightRun[currentMovementAnimation];
                    }
                    else
                    {
                        currentMovementAnimation++;
                        if (currentMovementAnimation == animations) currentMovementAnimation = 0;
                        playerSprite.ImageSource = leftRun[currentMovementAnimation];

                    }
                    ticksDone -= 10 / Speed;
                    if (ticksDone < 0) ticksDone = 0;
                    if (ticksDone >= 10 / Speed) ticksDone = 0;
                }

            }
            else
            {
                currentMovementAnimation = 0;
                if (rightD) playerSprite.ImageSource = rightRun[0];
                else playerSprite.ImageSource = leftRun[0];
            }

            if (map.grid[map.getX(), map.getY()].isCleared() && upDoorExist && Canvas.GetTop(player) < 10 && Canvas.GetLeft(player) > 480 && Canvas.GetLeft(player) < 480 + 100)//info z DoorsInfo
            {
                if (Canvas.GetTop(player) < -20)
                {
                    map.goTo(-1, 0, GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, UPDOOR);
                    updateMiniMap();
                    generateTB("enemy", ref boxes);
                }
            }
            else if (Canvas.GetTop(player) < 10)
            {
                Canvas.SetTop(player, 10);
                returnUp = true;
            }
            if (map.grid[map.getX(), map.getY()].isCleared() && downDoorExist && Canvas.GetTop(player) > 488 && Canvas.GetLeft(player) > 480 && Canvas.GetLeft(player) < 480 + 115)
            {
                if (Canvas.GetTop(player) > 518)
                {
                    map.goTo(1, 0, GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, DOWNDOOR);
                    updateMiniMap();
                    generateTB("enemy", ref boxes);
                }
            }
            else if (Canvas.GetTop(player) > 488)
            {
                Canvas.SetTop(player, 488);
                returnDown = true;
            }
            if (map.grid[map.getX(), map.getY()].isCleared() && leftDoorExist && Canvas.GetLeft(player) < -11 && Canvas.GetTop(player) > 250 - 40 && Canvas.GetTop(player) < 290)
            {
                if (Canvas.GetLeft(player) < -41)
                {
                    map.goTo(0, -1, GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, LEFTDOOR);
                    updateMiniMap();
                    generateTB("enemy", ref boxes);
                }
            }
            else if (Canvas.GetLeft(player) < -11)
            {

                Canvas.SetLeft(player, -11);
                returnLeft = true;
            }
            if (map.grid[map.getX(), map.getY()].isCleared() && rightDoorExist && Canvas.GetLeft(player) > 1100 && Canvas.GetTop(player) > 250 - 40 && Canvas.GetTop(player) < 305)
            {
                if (Canvas.GetLeft(player) > 1130)
                {
                    map.goTo(0, 1, GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, RIGHTDOOR);
                    updateMiniMap();
                    generateTB("enemy", ref boxes);
                }
            }
            else if (Canvas.GetLeft(player) > 1100)
            {
                Canvas.SetLeft(player, 1100);
                returnRight = true;
            }



            if (returnUp || Canvas.GetTop(player) <= -30)
            {
                if (SpeedY < 0) SpeedY = 0;
                returnUp = false;
            }
            if (returnDown || Canvas.GetTop(player) >= 528)
            {
                if (SpeedY > 0)
                    SpeedY = 0;
                returnDown = false;
            }
            if (returnRight || Canvas.GetLeft(player) >= 1140)
            {
                if (SpeedX > 0)
                    SpeedX = 0;
                returnRight = false;
            }
            if (returnLeft || Canvas.GetLeft(player) <= -41)
            {
                if (SpeedX < 0)
                    SpeedX = 0;
                returnLeft = false;
            }


            SpeedX = SpeedX * Friction;
            SpeedY = SpeedY * Friction;
            if (Speed != 0 && !stunned) Canvas.SetLeft(player, Canvas.GetLeft(player) + SpeedX);
            if (Speed != 0 && !stunned) Canvas.SetTop(player, Canvas.GetTop(player) + SpeedY);

            dotUpdate(deltaTime);
            foreach (Monster monster in map.rMon())
                monster.moveToTarget(player, deltaTime, Friction, dealDmgToPlayer);
        }
        public int getHp() { return healthPoints; }
        public bool getBlock() { return blockAttack; }
        public void setBlock(bool block) { blockAttack = block; }
        public double getSpeed() { return Speed; }
        public void  dealDmgToPlayer(int dealtDamage,string monsterName)
        {
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


        private void checkAttackCollision(ref Grid map,ref List<TextBox> boxes)
        {
            int i = 0;
            int minDmgAfterCalc=minDmg;
            int maxDmgAfterCalc=maxDmg;
            if (ignited)
            {
                double igniteEffect=1-(0.2*igniteResistance);
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
                    int obecnyDmg = Convert.ToInt32(boxes[i].Text);
                    obecnyDmg += dealtDmg;
                    x.damageTaken(dealtDmg);
                    boxes[i].Text = obecnyDmg.ToString();
                    boxes[i].Width = Convert.ToInt16(boxes[i].Text.Length) * 15;
                    boxes[i].Opacity = 1;
                    Canvas.SetLeft(boxes[i], Canvas.GetLeft(x.getBody()) + (x.getBody().ActualWidth / 2) - (boxes[i].Width / 2));
                    Canvas.SetTop(boxes[i], (Canvas.GetTop(x.getBody()) - (x.getBody().Height - x.getBody().ActualHeight)) - boxes[i].Height);

                }


                i++;

            }

            for (int j = i - 1; j >= 0; j--)
            {
                if (map.grid[map.getX(), map.getY()].checkIfDead(updateState[j], ref exp))
                {

                    GameScreen.Children.Remove(boxes[j]);
                    boxes.RemoveAt(j);
                    updateExp();
                }
            }
        }
        public void hideSkillTree()
        {
            playerPassives.hideSkillTree();
        }
        public void showSkillTree()
        {
            playerPassives.showSkillTree();
        }
        private void healPlayerBy(double healAmount){
            healthPoints += Convert.ToInt32((healAmount)*(healthRecoveryRate));
                if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
                updateHpBar();
        }
        private void updateExp()
        {
            if (exp > 1000)
            {
                exp -= 1000;
                level++;
                double lifeRestoredDueToLevelUp=maxHealthPoints*0.1;
                healPlayerBy(lifeRestoredDueToLevelUp);
                unassignedSkillPoints++;
                
                
            }
            
            expVisualization.Text = "lvl. "+level.ToString();
            expBar.Width = Convert.ToInt32(Convert.ToDouble(exp)*0.2 );
            
        }

        private void attackOmni(double deltaTime,ref Grid map, ref List<TextBox> boxes)
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
                checkAttackCollision(ref map,ref boxes);
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

        
    }
}
