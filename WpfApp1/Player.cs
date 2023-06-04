using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
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
 
    internal partial class Player
    {
        bool allocateMode = true;

        TextBox playerStatsHolder;
        TextBox visualForShieldCooldown;
        TextBox visualForImmunityCooldown;
        System.Windows.Shapes.Rectangle immunityPassiveVisual;
        Button closeEquipmentButton;
        System.Windows.Shapes.Rectangle shieldPassiveVisual;
        bool isShieldAdded;
        bool isImmunityAdded;
        int addedSpecialBuffs;
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
        private double intervalForAttackAnimations;
        private double ticksDone = 0;
        private int currentMovementAnimation = 0;
        private int attackRange, attackDirection;
        private double unlockAttack;
        private double attackCooldown;
        System.Windows.Shapes.Rectangle hpBar;
        System.Windows.Shapes.Rectangle hpBarWindow;
        System.Windows.Shapes.Rectangle expBar;
        System.Windows.Shapes.Rectangle expBarWindow;
        System.Windows.Shapes.Rectangle DotBar;
        System.Windows.Shapes.Rectangle[] buffsContainer = new System.Windows.Shapes.Rectangle[14];
        int howManyBuffsAndDebuffsImplemented;
        TextBox expVisualization;
        int healthPoints;
        int maxHealthPoints;
        bool rightD;
        bool leftDoorExist, rightDoorExist, upDoorExist, downDoorExist;
        const int UPDOOR = 0;
        const int RIGHTDOOR = 1;
        const int DOWNDOOR = 2;
        const int LEFTDOOR = 3;
        
       
        List<Tuple<double, double, double, double, string>> DamagePerMilliseconds = new List<Tuple<double, double, double, double, string>>();
        // 1. dmg per millisecond 2. accumulated dmg (change everytime dealing dmg from pool ) 3. Time elapsed 4.When remove from list 5.Name
        Canvas GameScreen;
        public static string killedBy;
        public static bool isDead;
        public static int lastDamage;
        System.Windows.Shapes.Rectangle blackOverlay;
        // Following tuples have up to 5 arguments : BASE VALUE(Always),FLAT VALUE(optionally),PERCENT VALUE(optionally)
        
        
        Inventory playerInventory;

        public Player(Canvas GS)
        {
            
            descActiveSkills=new TextBox();
            descActiveSkills.Text = "PICK ONE SKILL";
            descActiveSkills.FontFamily = new FontFamily("Algerian");
            descActiveSkills.FontSize = 50;
            descActiveSkills.Foreground = Brushes.White;
            descActiveSkills.Background = Brushes.Transparent;
            descActiveSkills.BorderBrush = Brushes.Transparent;
            descActiveSkills.IsHitTestVisible = false;
            closeEquipmentButton = new Button();
            Canvas.SetLeft(closeEquipmentButton, 1140);
            Canvas.SetTop(closeEquipmentButton, 10);
            closeEquipmentButton.Width = 50;
            closeEquipmentButton.Height = 50;
            Canvas.SetZIndex(closeEquipmentButton, 1200);
            closeEquipmentButton.Content = "X";

            closeEquipmentButton.Click += CloseEquipment_Click;

            Canvas.SetLeft(descActiveSkills, 420);
            Canvas.SetTop(descActiveSkills, 140);
            Canvas.SetZIndex(descActiveSkills, 999);

            descSkillOne = new TextBox();
            descSkillOne.Text = "TBD";
            descSkillOne.FontFamily = new FontFamily("Algerian");
            descSkillOne.FontSize = 13;
            descSkillOne.Foreground = Brushes.White;
            descSkillOne.Background = Brushes.Transparent;
            descSkillOne.BorderBrush = Brushes.Transparent;
            descSkillOne.IsHitTestVisible = false;

            equipmentBackground = new System.Windows.Shapes.Rectangle();
            ImageBrush temp = new ImageBrush();
            temp.ImageSource= new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/UI/equipmentUI.png", UriKind.Absolute));
            equipmentBackground.Width = 1200;
            equipmentBackground.Height = 600;
            equipmentBackground.Fill = temp;
            Canvas.SetZIndex(equipmentBackground, 1199);
            Canvas.SetLeft(equipmentBackground, 0);
            Canvas.SetTop(equipmentBackground, 0);
           
            Canvas.SetLeft(descSkillOne, 300);
            Canvas.SetTop(descSkillOne, 200);
            Canvas.SetZIndex(descSkillOne, 999);
            descSkillTwo = new TextBox();
            descSkillTwo.Text = "TBD";
            descSkillTwo.FontFamily = new FontFamily("Algerian");
            descSkillTwo.FontSize = 13;
            descSkillTwo.Foreground = Brushes.White;
            descSkillTwo.Background = Brushes.Transparent;
            descSkillTwo.BorderBrush = Brushes.Transparent;
            descSkillTwo.IsHitTestVisible = false;

            Canvas.SetLeft(descSkillTwo, 610);
            Canvas.SetTop(descSkillTwo, 200);
            Canvas.SetZIndex(descSkillTwo, 999);
            pickedSkills = 0;
            skillsThatCanBeObtained = new List<Skills>();
            obtainedSkills = new List<Skills>();
            fillSkills(GS);
            blackOverlay = new System.Windows.Shapes.Rectangle();
            blackOverlay.Width = 700;
            blackOverlay.Height = 400;
            Canvas.SetLeft(blackOverlay, 250);
            Canvas.SetTop(blackOverlay, 120);
            Canvas.SetZIndex(blackOverlay, 998);
            blackOverlay.Opacity = 0.6;
            blackOverlay.Fill = Brushes.Black;
            firstChoice = new Button();
            secondChoice = new Button();
            secondChoice.Visibility = Visibility.Hidden;
            firstChoice.Visibility = Visibility.Hidden;
            secondChoice.IsEnabled = false;
            firstChoice.IsEnabled = false;
            firstChoice.Width = 100;
            firstChoice.Height = 50;
            Canvas.SetLeft(firstChoice, 360);
            Canvas.SetTop(firstChoice, 450);
            secondChoice.Width = 100;
            secondChoice.Height = 50;
            Canvas.SetLeft(secondChoice, 700);
            Canvas.SetTop(secondChoice, 450);

            skillsToPick = 0;
            playerStatsHolder = new TextBox();
            Canvas.SetLeft(playerStatsHolder, 0);
            Canvas.SetTop(playerStatsHolder, 280);
            playerStatsHolder.FontSize = 9;
           
            Canvas.SetZIndex(playerStatsHolder, 1200);
            
            playerStatsHolder.IsHitTestVisible = false;
            playerStatsHolder.Background = Brushes.Transparent;
            playerStatsHolder.Foreground = Brushes.Black;
            playerStatsHolder.BorderBrush = Brushes.Transparent;
            shieldPassiveVisual = new System.Windows.Shapes.Rectangle();
            immunityPassiveVisual = new System.Windows.Shapes.Rectangle();
            ImageBrush shieldSprite = new ImageBrush();
            shieldSprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/passive9.png", UriKind.Absolute));
            shieldPassiveVisual.Fill = shieldSprite;
            ImageBrush immunitySprite = new ImageBrush();
            immunitySprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/passive19.png", UriKind.Absolute));
            immunityPassiveVisual.Fill = immunitySprite;
            visualForShieldCooldown = new TextBox();
            visualForImmunityCooldown = new TextBox();
            isImmunityAdded = false;
            isShieldAdded = false;
            addedSpecialBuffs = 0;
            attackCooldown = 400;
            unlockAttack = 0;
            chanceToInflictBleed = 0;
            increasedDamage = 1.0;
            increasedFireDamage = 0.0;
            increasedIceDamage = 0.0;
            increasedLightningDamage = 0.0;
            increasedNonElementalDotDamage = 0.0;
            damageTakenReduction = 0;
            armour = 0;
            timeUntilShieldAvailable = 0;
            timeUntilImmunityAvailable = 0;
            cooldownBaseTime = 1.0;
            isShieldActive = false;
            isImmunityActive = false;
            shieldCooldown = 30;
            immunityCooldown = 18;
            igniteResistance = 1;
            shockResistance = 1;
            stunResistance = 1;
            nonElementalDotResistance = 1;
            
            itemQuantity = 1;
            itemQuality = 1;
            healthRecoveryRate = 1.0;
            criticalHitDamage = 1.5;
            criticalHitChance = 0;
            lifeGainOnHit = 0;
            increasedDamageDueToDebuffs = 0;

            healthPointsCalculations = new Tuple<int, double, double>(200, 0, 0);
            healthPointsToDistribute = new Tuple<int, int>(0, 0);
            healthRecoveryRateCalculations = new Tuple<double, double, double>(1, 0, 0);
            damageCalculations = new Tuple<int, int, double, double>(10, 15, 0, 0);
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
            unassignedSkillPoints = 0;
            assignedSkillPoints = 0;
            killedBy = "Damage over time";
            playerInventory = new Inventory(GS);
            playerPassives = new SkillTree(GS);
            playerPassives.requestStatsRecalculation += updateStats;
            playerInventory.requestStatRecalculation += updateStats;
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
            if (allocateMode) { unassignedSkillPoints = 25; level = 25;skillsToPick = 5; }
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
            for(int i = 0; i < 5; i++)
            {
                visualsSkills[i] = new System.Windows.Shapes.Rectangle();
                cdSkills[i] = new TextBox();
            }

        }

        

     

       

        private void updateHpBar()
        {
            if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
            hpVisualization.Text = healthPoints + "/" + maxHealthPoints;
            double w = Convert.ToDouble(healthPoints) / Convert.ToDouble(maxHealthPoints) * 200;
            if (w < 0) w = 0;
            hpBar.Width = Convert.ToInt32(w);
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

            howManyBuffsAndDebuffsImplemented = 3;
            for (int i = 0; i < howManyBuffsAndDebuffsImplemented; i++)
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
        public System.Windows.Shapes.Rectangle getBody()
        {
            return player;
        }
        double waitBeforeUpdating = 0;
        public void gameTick(ScrollViewer Camera, bool UpKey, bool DownKey, bool RightKey, bool LeftKey, ref Grid map, double deltaTime, double Friction, ref List<TextBox> boxes, Action updateMiniMap)
        {
            if (Speed != 0) Speed = baseSpeed * deltaTime;
            ticksDone += baseSpeed / 2 * deltaTime;
            foreach(Skills skill in obtainedSkills)
                skill.updateState(deltaTime, map.rMon());
            waitBeforeUpdating -= deltaTime;
            if (waitBeforeUpdating <= 0)
            {
                updateCd();
                waitBeforeUpdating = 0.1;
            }
            if (tryingInvoke)
            {
                tryingInvoke = false;
                checkAttackCollision(ref map, ref boxes, currentSkillBody, damageOfCurrentSkill, statusEffectsForSkill, canCurrentSkillCrit);
            }
            timeUntilImmunityAvailable -= Convert.ToDouble(1000 * deltaTime);
            if (timeUntilImmunityAvailable < 0) timeUntilImmunityAvailable = 0;
            timeUntilShieldAvailable -= Convert.ToDouble(1000 * deltaTime);
            if (timeUntilShieldAvailable < 0) timeUntilShieldAvailable = 0;
            if (isShieldAdded)
            {
                int cdOnShieldStackS = (int)Math.Floor(timeUntilShieldAvailable / 1000);
                int cdOnShieldStackMs = (int)Math.Floor(timeUntilShieldAvailable / 100) - (cdOnShieldStackS * 10);

                visualForShieldCooldown.Text = cdOnShieldStackS.ToString() + "." + cdOnShieldStackMs.ToString();
            }
            if (isImmunityAdded)
            {
                int cdOnImmunityStackS = (int)Math.Floor(timeUntilImmunityAvailable / 1000);

                int cdOnImmunityStackMs = (int)Math.Floor(timeUntilImmunityAvailable / 100) - (cdOnImmunityStackS * 10);
                visualForImmunityCooldown.Text = cdOnImmunityStackS.ToString() + "." + cdOnImmunityStackMs.ToString();
            }
            if (timeUntilImmunityAvailable > 0) immunityPassiveVisual.Opacity = 0.5;
            else immunityPassiveVisual.Opacity = 1;
            if (timeUntilShieldAvailable > 0) shieldPassiveVisual.Opacity = 0.5;
            else shieldPassiveVisual.Opacity = 1;
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
            if (Canvas.GetLeft(player) < 400 && Canvas.GetTop(player) < 60)
            {
                hpBar.Opacity = 0.5;
                hpBarWindow.Opacity = 0.5;
                hpVisualization.Opacity = 0.5;
                expBar.Opacity = 0.5;
                expBarWindow.Opacity = 0.5;
                expVisualization.Opacity = 0.5;
                DotBar.Opacity = 0.5;
                for (int i = 0; i < howManyBuffsAndDebuffsImplemented; i++)
                {
                    if (Canvas.GetZIndex(buffsContainer[i]) < 950) buffsContainer[i].Visibility = Visibility.Hidden;
                }
            }
            else
            {
                hpBar.Opacity = 1;
                hpBarWindow.Opacity = 1;
                hpVisualization.Opacity = 1;
                expBar.Opacity = 1;
                expBarWindow.Opacity = 1;
                expVisualization.Opacity = 1;
                DotBar.Opacity = 1;
                for (int i = 0; i < howManyBuffsAndDebuffsImplemented; i++)
                    buffsContainer[i].Visibility = Visibility.Visible;

            }

            foreach (Monster monster in map.rMon())
                monster.moveToTarget(player, deltaTime, Friction, dealDmgToPlayer);
            dotUpdate(deltaTime);
        }
        public int getHp() { return healthPoints; }
        public bool getBlock() { return blockAttack; }
        public void setBlock(bool block) { blockAttack = block; }
        public double getSpeed() { return Speed; }
        public System.Windows.Point playerCoordinates() { return new System.Windows.Point(Canvas.GetLeft(player), Canvas.GetTop(player)); }
      
      
        public void hideSkillTree()
        {
            playerPassives.hideSkillTree();
        }
        public void showSkillTree()
        {
            playerPassives.showSkillTree();
        }
        private void healPlayerBy(double healAmount)
        {
            double expectedHealing = healAmount * healthRecoveryRate;
            int healingGiven = Convert.ToInt16(healAmount * healthRecoveryRate);
            overflowingHealing += (expectedHealing - healingGiven);
            if (overflowingHealing > 1)
            {
                int additionalHealingDueToAccumulationOfOverlowingHealing = Convert.ToInt16(overflowingHealing);
                healingGiven += additionalHealingDueToAccumulationOfOverlowingHealing;
                overflowingHealing -= additionalHealingDueToAccumulationOfOverlowingHealing;
            }
            healthPoints += healingGiven;
            if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
            updateHpBar();
        }
       

       


    }
}
