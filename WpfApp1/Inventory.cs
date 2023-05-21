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

        }
        public void showItems()
        {
            int x = 339;
            int y = 149;

            for(int i = 0; i < equipment.Count; i++)
            {
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
        int minDamage=0;
        int maxDamage=0;
        ImageBrush sprite;
        const int WEAPON = 0;
        const int BODYARMOUR = 1;
        const int HELMET = 2;
        const int BOOTS = 3;
        const int JEWELLERY = 4;

        Canvas canv;

        public Equipment(int type,int rarity,Canvas gs)
        {
            
            clickToEquipItem = new Button();
             toolTip = new TextBlock();
            backgroundOfButton = new System.Windows.Shapes.Rectangle();
            canv = gs;
            toolTip.IsEnabled = false;
            toolTip.FontSize = 10.0;
            backgroundOfButton.Width = 48;
            backgroundOfButton.Height = 48;
            Canvas.SetZIndex(backgroundOfButton, 1000);
            Canvas.SetZIndex(clickToEquipItem, 1001);
            Canvas.SetZIndex(toolTip, 1002);
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
                sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/items/Pierscien.png", UriKind.Absolute));
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
                slot = "JewelleryJewellery";
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
            Canvas.SetZIndex(clickToEquipItem, 1001);
            clickToEquipItem.MouseMove += showToolTip;
            clickToEquipItem.MouseLeave += hideToolTip;
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
        public int getMinDmg() { return minDamage; }
        public int getMaxDmg() { return maxDamage; }

    }
}
