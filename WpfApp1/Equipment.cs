using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BasicsOfGame
{
    internal class Weapon 
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
        Random rnd = new Random();
        int minDamage; // 10-14
        int maxDamage; // 15-18
        string desc="";
        const int minDamageMinimum = 10;
        const int minDamageMaximum = 15;
        const int maxDamageMinimum = 15;
        const int maxDamageMaximum = 19;
        List<Tuple<string, string, double>> AdditionalStats;
        public Weapon(int rarity)
        {
               
            AdditionalStats = new List<Tuple<string, string, double>>();
            generateName(rarity); 
            minDamage =rnd.Next(minDamageMinimum,minDamageMaximum);
            maxDamage = rnd.Next(maxDamageMinimum, maxDamageMaximum);
            desc += "Base damage: " + minDamage + " to " + maxDamage+"\nAdditional stats:\n";
            for(int i=0;i<rarity;i++){ AdditionalStats.Add(generateRandomStats()); }

           
            MessageBox.Show(desc);
           
            
        }
        const int DAMAGE=0;
        const int ATTSPD = 1;
        const int CRITDMG = 2;
        const int CRITCHANCE = 3;
        const int LIGHTDAMAGE = 4;
        const int ICEDAMAGE = 5;
        const int FIREDAMAGE = 6;
        private void generateName(int rarity)
        {
            string[] prefix = new string[] { "Malicious", "Weeping", "Rune", "Splendid", "Heroic","Legendary","Stalwart"};
            string[] name = new string[] { "Sword", "Blade", "Axe", "Dagger" };
            string[] suffix = new string[] { "Destruction", "Darkness","Godslayer","Eagle Tribe","Starbreaking"};
            if(rarity == NORMAL)
            {
                desc += "(NORMAL)\n" + name[rnd.Next(0, name.Length - 1)];
            }
            if (rarity == MAGIC)
            {
                desc += "(MAGIC)\n" + prefix[rnd.Next(0,prefix.Length-1)] +" "+ name[rnd.Next(0, name.Length - 1)];

            }
            if (rarity == RARE)
            {
                desc += "(RARE)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)]+" of "+suffix[rnd.Next(0,suffix.Length-1)];
            }
            if (rarity == EPIC)
            {
                desc += "(EPIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            desc += '\n';
        }
        private Tuple<string, string, double> generateRandomStats()
        {

            int x = rnd.Next(0, 8);
            int range;
            Tuple<string, string, double> returnMe;
            if (x == DAMAGE)
            {
                range = rnd.Next(12, 26);
                returnMe = new Tuple<string, string, double>("damage","percent",range);

                desc += "Increases your damage by " + range + "%\n";
            }
            else if (x == ATTSPD)
            { 
                range = rnd.Next(3,6);
                returnMe = new Tuple<string, string, double>("attackSpeed", "percent", range);
                desc += "Increases your attack speed by " + range + "%\n";
            }
            else if(x == CRITDMG)
            {
                range = rnd.Next(25, 51);
                returnMe = new Tuple<string, string, double>("criticalDamage", "percent", range);
                desc += "Increases your critical damage by " + range + "%\n";
            }
            else if(x == CRITCHANCE)
            {
                range = rnd.Next(5, 16);
                returnMe = new Tuple<string, string, double>("absoluteCriticalHitChance", "percent", range);
                desc += "Increases your critical strike chance by " + range + "%\n";
            }
            else if(x ==  LIGHTDAMAGE)
            {
                range = rnd.Next(25, 51);
                returnMe = new Tuple<string, string, double>("lightningDamage", "percent", range);
                desc += "Increases your lightning damage by " + range + "%\n";
            }
            else if(x == ICEDAMAGE)
            {
                range = rnd.Next(25, 51);
                returnMe = new Tuple<string, string, double>("iceDamage", "percent", range);
                desc += "Increases your ice damage by " + range + "%\n";
            }
            else if(x == FIREDAMAGE)
            {
                range = rnd.Next(25, 51);
                returnMe = new Tuple<string, string, double>("fireDamage", "percent", range);
                desc += "Increases your fire damage by " + range + "%\n";
            }
            else
            {
                range = rnd.Next(25, 51);
                returnMe = new Tuple<string, string, double>("nonElementalDotDamage", "percent", range);
                desc += "Increases your poison and bleed damage by " + range + "%\n";
            }
            return returnMe;
        }
        public int getMinDamage() { return minDamage; }
        public int getMaxDamage() { return maxDamage; }
        public List<Tuple<string, string, double>> getStats(){ return AdditionalStats; }
        public string description()
        {

            return desc;
        }
    };
    
    internal class BodyArmour // BASE ARMOUR 
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
    };
    internal class Helmet // BASE STUN RESISTANCE 
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
    };
    internal class Boots // BASE MOVEMENT SPEED
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
    };
    internal class Jewellery // BASE COOLDOWN
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
    };


}
