using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BasicsOfGame
{
    internal class Inventory
    {
       List<Equipment> equipment;
        public Inventory() {
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

    }
    internal class Equipment
    {
        List<Tuple<string, string, double>> addedSkills;
        string slot;
        string itemRarity;
        string desc;
        int minDamage=0;
        int maxDamage=0;
        ImageBrush sprite;
        const int WEAPON = 0;
        const int BODYARMOUR = 1;
        const int HELMET = 2;
        const int BOOTS = 3;
        const int JEWELLERY = 4;

        public Equipment(int type,int rarity)
        {
            if(type == WEAPON) 
            {
                slot = "Weapon";
                Weapon temp = new Weapon(rarity);
                minDamage=temp.getMinDamage();
                maxDamage=temp.getMaxDamage();
                addedSkills = temp.getStats();
                desc = temp.description();
                sprite=new ImageBrush(); // here should be absolute path for sprite
                itemRarity=temp.returnRarity();
            }
            if(type == BODYARMOUR) 
            {
                slot = "BodyArmour";
                BodyArmour temp = new BodyArmour(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                sprite = new ImageBrush(); // here should be absolute path for sprite
                itemRarity = temp.returnRarity();
            }
            if(type == HELMET) 
            {
                slot = "Helmet";
                Helmet temp = new Helmet(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                sprite = new ImageBrush(); // here should be absolute path for sprite
                itemRarity = temp.returnRarity();

            }
            if (type == BOOTS) 
            {
                slot = "Inwork";
                addedSkills = new List<Tuple<string, string, double>>();
                desc = "Still in work";
                sprite = new ImageBrush(); // here should be absolute path for sprite
            }
            else // JEWELLERY 
            {
                slot = "Boots";
                Helmet temp = new Helmet(rarity);
                addedSkills = temp.getStats();
                desc = temp.description();
                sprite = new ImageBrush(); // here should be absolute path for sprite
                itemRarity = temp.returnRarity();

            }
        }
        public string getDesc()
        {
            return desc;
        }
        public int getMinDmg() { return minDamage; }
        public int getMaxDmg() { return maxDamage; }

    }
}
