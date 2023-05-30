using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace BasicsOfGame
{
    internal class Inventory
    {
        
       List<Equipment> equipment;
       private Tuple<int, int> helmetCoord=new Tuple<int,int>(128,19);
        private Tuple<int, int> armourCoord = new Tuple<int, int>(128, 89);
        private Tuple<int, int> bootsCoord = new Tuple<int, int>(128, 159);
        private Tuple<int, int> weaponCoord = new Tuple<int, int>(58, 89);
        private Tuple<int, int> jewelleryCoord = new Tuple<int, int>(198, 89);
        bool isHelmetEquipped;
        bool isArmourEquipped;
        bool isBootsEquipped;
        bool isWeaponEquipped;
        bool isJewelleryEquipped;
        int helmetIndex;
        int armourIndex;
        int bootsIndex;
        int jewelleryIndex;
        int weaponIndex;
        public Action requestStatRecalculation;
        Canvas canv;
        public Inventory(Canvas gs) {
            canv = gs;
            equipment = new List<Equipment>();
        }
        public string sum()
        {
            string desc = "";
            foreach(Equipment item in equipment)
            {

                desc += item.getDesc();
                desc += "\n\n";
                

            }
            return desc;
        }
        public void addEquipment(Equipment itemToAdd)
        {

            equipment.Add(itemToAdd);
            equipment[equipment.Count - 1].requestEquipping += equipItem;
        }
        public int getMinDmg()
        {
            if (isWeaponEquipped)
            {
                return equipment[weaponIndex].getMinDmg();
            }
            return 10;
        }
        public int howManyEquipped()
        {
            int equippedPieces = 0;
            if (isArmourEquipped) equippedPieces++;
            if (isHelmetEquipped) equippedPieces++;
            if (isWeaponEquipped) equippedPieces++;
            if (isJewelleryEquipped) equippedPieces++;
            if (isBootsEquipped) equippedPieces++;


            return equippedPieces;
        }
        public int getMaxDmg()
        {
            if (isWeaponEquipped)
            {
                return equipment[weaponIndex].getMaxDmg();
            }
            return 15;
        }
        private void equipItem(string slot,Equipment choosen)
        {
            
            if (slot == "Weapon")
            {
                if (isWeaponEquipped)
                {
                    equipment[weaponIndex].takeOff();
                    
                }
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i] == choosen)
                    {

                        equipment[i].Equip();
                        equipment[i].setButtonPosition(weaponCoord.Item1, weaponCoord.Item2);
                        isWeaponEquipped = true;
                        weaponIndex = i;
                        break;
                    }
                }
                showItems();
            }
            if (slot == "BodyArmour")
            {
                if (isArmourEquipped)
                {
                    equipment[armourIndex].takeOff();

                }
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i] == choosen)
                    {

                        equipment[i].Equip();
                        isArmourEquipped = true;
                        equipment[i].setButtonPosition(armourCoord.Item1, armourCoord.Item2);
                        armourIndex = i;
                        break;
                    }
                }
                showItems();
            }
            if (slot == "Helmet")
            {
                if (isHelmetEquipped)
                {
                    equipment[helmetIndex].takeOff();

                }
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i] == choosen)
                    {

                        equipment[i].Equip();
                        isHelmetEquipped = true;
                        equipment[i].setButtonPosition(helmetCoord.Item1, helmetCoord.Item2);
                        helmetIndex = i;
                        break;
                    }
                }
                showItems();
            }
            if (slot == "Boots")
            {
                if (isBootsEquipped)
                {
                    equipment[bootsIndex].takeOff();

                }
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i] == choosen)
                    {

                        equipment[i].Equip();
                        isBootsEquipped = true;
                        equipment[i].setButtonPosition(bootsCoord.Item1, bootsCoord.Item2);
                        bootsIndex = i;
                        break;
                    }
                }
                showItems();
            }
            if (slot == "Jewellery")
            {
                if (isJewelleryEquipped)
                {
                    equipment[jewelleryIndex].takeOff();

                }
                for (int i = 0; i < equipment.Count; i++)
                {
                    if (equipment[i] == choosen)
                    {

                        equipment[i].Equip();
                        isJewelleryEquipped = true;
                        equipment[i].setButtonPosition(jewelleryCoord.Item1, jewelleryCoord.Item2);
                        jewelleryIndex = i;
                        break;
                    }
                }
                showItems();
            }
            
            requestStatRecalculation?.Invoke();
        }
        public List<Tuple<string, string, double>> getStats()
        {
            List<Tuple<string, string, double>> statsFromItems = new List<Tuple<string, string, double>>();
            foreach (Equipment item in equipment)
            {
                if (item.isEquipped())
                {
                    foreach (Tuple<string, string, double> tuple in item.getStats())
                    {
                        statsFromItems.Add(tuple);
                    }
                }
            }
            return statsFromItems;

        }
        public void showItems()
        {
            int x = 339;
            int y = 149;

            for(int i = 0; i < equipment.Count; i++)
            {
                if (equipment[i].isEquipped())
                {
                    equipment[i].addButton(); continue;
                }
                equipment[i].setButtonPosition(x, y);
                equipment[i].addButton();
                x += 50;
                if (x >= 1189)
                {
                    x = 339;
                    y += 50;
                }
            }

        }
        public void hideItems()
        {
            foreach (Equipment item in equipment) item.hideButton();
        }

    }
    internal class Equipment
    {
        List<Tuple<string, string, double>> addedSkills;
        string slot;
        string itemRarity;
        Button clickToEquipItem;
        System.Windows.Shapes.Rectangle backgroundOfButton;
        TextBlock toolTip;
        string desc;
        int minDamage = 0;
        int maxDamage = 0;
        ImageBrush sprite;
        const int WEAPON = 0;
        const int BODYARMOUR = 1;
        const int HELMET = 2;
        const int BOOTS = 3;
        const int JEWELLERY = 4;
        bool equipped;
        Canvas canv;
        public Action<string, Equipment> requestEquipping;

        public Equipment(int type,int rarity,Canvas gs)
        {
            equipped = false;
            clickToEquipItem = new Button();
             toolTip = new TextBlock();
            backgroundOfButton = new System.Windows.Shapes.Rectangle();
            canv = gs;
            toolTip.IsEnabled = false;
            toolTip.FontSize = 10.0;
            backgroundOfButton.Width = 48;
            backgroundOfButton.Height = 48;
            Canvas.SetZIndex(backgroundOfButton, 1200);
            Canvas.SetZIndex(clickToEquipItem, 1201);
            Canvas.SetZIndex(toolTip, 1202);
            toolTip.Foreground = Brushes.Black;
            toolTip.Padding = new Thickness(10);
            if(type == WEAPON) 
            {
                slot = "Weapon";
                Weapon temp = new Weapon(rarity);
                minDamage=temp.getMinDamage();
                maxDamage=temp.getMaxDamage();
                addedSkills = temp.getStats();
                desc = temp.description();
                clickToEquipItem.Style=(Style)Application.Current.MainWindow.FindResource("IButton");
                sprite=new ImageBrush(); // here should be absolute path for sprite
                
                sprite.ImageSource= new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/sword.png", UriKind.Absolute));
                clickToEquipItem.Background = sprite;
                itemRarity =temp.returnRarity();
            }
            else if(type == BODYARMOUR) 
            {
                slot = "BodyArmour";
                BodyArmour temp = new BodyArmour(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                clickToEquipItem.Style = (Style)Application.Current.MainWindow.FindResource("IButton");
                sprite = new ImageBrush(); // here should be absolute path for sprite
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/Zbroja.png", UriKind.Absolute));
                clickToEquipItem.Background = sprite;
                itemRarity = temp.returnRarity();
            }
            else if(type == HELMET) 
            {
                slot = "Helmet";
                Helmet temp = new Helmet(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                clickToEquipItem.Style = (Style)Application.Current.MainWindow.FindResource("IButton");
                sprite = new ImageBrush(); // here should be absolute path for sprite
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/Helm.png", UriKind.Absolute));
                clickToEquipItem.Background = sprite;
                itemRarity = temp.returnRarity();

            }
            else if (type == BOOTS) 
            {
                slot = "Boots";
                Boots temp = new Boots(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                clickToEquipItem.Style = (Style)Application.Current.MainWindow.FindResource("IButton");
                sprite = new ImageBrush(); // here should be absolute path for sprite
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/Buty.png", UriKind.Absolute));
                clickToEquipItem.Background = sprite;
                itemRarity = temp.returnRarity();
            }
            else // JEWELLERY 
            {
                slot = "Jewellery";
                Jewellery temp = new Jewellery(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                clickToEquipItem.Style = (Style)Application.Current.MainWindow.FindResource("IButton");
                sprite = new ImageBrush(); // here should be absolute path for sprite
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/Pierscien.png", UriKind.Absolute));
                clickToEquipItem.Background = sprite;
                itemRarity = temp.returnRarity();

            }
            if (itemRarity == "Normal")
            {
                toolTip.Background = Brushes.LightGray;
                backgroundOfButton.Fill = Brushes.LightGray;
            }
            if (itemRarity == "Rare")
            {

                
                toolTip.Background = new SolidColorBrush(Color.FromRgb(8, 146, 194));
                backgroundOfButton.Fill = new SolidColorBrush(Color.FromRgb(8, 146, 194));

            }
            if (itemRarity == "Epic")
            {
                toolTip.Background = Brushes.MediumPurple;
                backgroundOfButton.Fill = Brushes.MediumPurple;
               


            }
            if (itemRarity == "Legendary")
            {
                toolTip.Background = Brushes.Goldenrod;
                backgroundOfButton.Fill = Brushes.Goldenrod;


            }
            clickToEquipItem.Width = 50;
            clickToEquipItem.Height = 50;
            Canvas.SetZIndex(clickToEquipItem, 1201);
            clickToEquipItem.MouseMove += showToolTip;
            clickToEquipItem.MouseLeave += hideToolTip;

            clickToEquipItem.MouseRightButtonDown += invokeEquipping;
        }
        public List<Tuple<string, string, double>> getStats(){ return addedSkills;}

        private void invokeEquipping(object sender, MouseButtonEventArgs e)
        {
           if(!equipped) requestEquipping?.Invoke(slot,this);
        }

        bool isBeingShown = false;
        public void setButtonPosition(int x, int y)
        {
            Canvas.SetLeft(clickToEquipItem, x);
            Canvas.SetLeft(backgroundOfButton, x+2);
            Canvas.SetTop(clickToEquipItem, y);
            Canvas.SetTop(backgroundOfButton, y+2);
            


        }
        bool isButtonAdded = false;
        public void addButton()
        {
            if (!isButtonAdded)
            {
                canv.Children.Add(clickToEquipItem);
                canv.Children.Add(backgroundOfButton);
            }
            isButtonAdded = true;  
        }
        public void hideButton()
        {
            isButtonAdded = false;
            canv.Children.Remove(clickToEquipItem);
            canv.Children.Remove(backgroundOfButton);


        }
        private void hideToolTip(object sender, MouseEventArgs e)
        {
            isBeingShown = false;
            canv.Children.Remove(toolTip);
        }

        private void showToolTip(object sender, MouseEventArgs e)
        {
            toolTip.Text = desc;

            
            
            Point p = Mouse.GetPosition(canv);
            if (p.X > 710)
                Canvas.SetLeft(toolTip, p.X - toolTip.ActualWidth - 5);
            else Canvas.SetLeft(toolTip, p.X + 15);
            if (p.Y > 500) Canvas.SetTop(toolTip, p.Y + 15 - toolTip.ActualHeight);
            else Canvas.SetTop(toolTip, p.Y + 15);
            if (!isBeingShown) canv.Children.Add(toolTip);
            isBeingShown = true;
        }

        public string getDesc()
        {
            return desc;
        }
        public bool isEquipped()
        {
            return equipped;
        }
        public void Equip()
        {
            equipped = true;
        }
        public void takeOff()
        {
            equipped = false;
        }
        public int getMinDmg() { return minDamage; }
        public int getMaxDmg() { return maxDamage; }

    }
}
