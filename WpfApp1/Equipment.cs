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
            desc += "Base damage: " + minDamage + " to " + maxDamage+"\n";
            if (rarity != NORMAL) desc += "Additional stats:\n";
            for (int i=0;i<rarity;i++){ AdditionalStats.Add(generateRandomStats()); }

           
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
        Random rnd = new Random();
        int minArmour; // 5-25
        string desc = "";
        const int minArmourMinimum = 3;
        const int minArmourMaximum = 12;
        List<Tuple<string, string, double>> AdditionalStats;
        public BodyArmour(int rarity)
        {

            AdditionalStats = new List<Tuple<string, string, double>>();
            generateName(rarity);
            minArmour = rnd.Next(minArmourMinimum, minArmourMaximum);
            desc += "Base Armour: " + minArmour + "\nAdditional stats:\n";
            for (int i = 0; i < rarity; i++) { AdditionalStats.Add(generateRandomStats()); }

            MessageBox.Show(desc);
        }
        private void generateName(int rarity)
        {
            string[] prefix = new string[] { "Obsidian", "Platinum", "Golden", "Iron", "Bronze", "Tungsten" };
            string[] name = new string[] { "Plate Armour", "Chainmail", "Cuirass", "Breastplate" };
            string[] suffix = new string[] { "Salvation", "Protection", "Conquest", "Perserverance" };
            if (rarity == NORMAL)
            {
                desc += "(NORMAL)\n" + name[rnd.Next(0, name.Length - 1)];
            }
            if (rarity == MAGIC)
            {
                desc += "(MAGIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)];

            }
            if (rarity == RARE)
            {
                desc += "(RARE)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            if (rarity == EPIC)
            {
                desc += "(EPIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            desc += '\n';
        }

        const int MAXHEALTH = 0;
        const int HEALTHRECOVERY = 1;
        const int LIFEGAINONHIT = 2;
        const int RESISTANCECHANCE = 3;

        private Tuple<string, string, double> generateRandomStats()
        {

            int x = rnd.Next(0, 4);
            int range;
            Tuple<string, string, double> returnMe;
            if (x == MAXHEALTH)
            {
                range = rnd.Next(20, 50);
                returnMe = new Tuple<string, string, double>("maximumHealth", "percent", range);
                desc += "Increases your maximum health by " + range + " health points\n";
            }
            else if (x == HEALTHRECOVERY)
            {
                range = rnd.Next(3, 15);
                returnMe = new Tuple<string, string, double>("healthRecoveryRate", "percent", range);
                desc += "Increases your health recovery by " + range + " per second\n";
            }
            else if (x == LIFEGAINONHIT)
            {
                range = rnd.Next(5, 25);
                returnMe = new Tuple<string, string, double>("lifeGainOnHit", "percent", range);
                desc += "Increases how much life you gain when you hit an enemy by " + range + "%\n";
            }
            else// (x == RESISTANCECHANCE)
            {
                range = rnd.Next(5, 15);
                returnMe = new Tuple<string, string, double>("debuffResistance", "percent", range);
                desc += "Increases your resistance to all debuffs by " + range + "%\n";
            }
            return returnMe;
        }

        public int getMinArmour() { return minArmour; }
        public List<Tuple<string, string, double>> getStats() { return AdditionalStats; }
        public string description()
        {

            return desc;
        }
    };
    internal class Helmet // BASE STUN RESISTANCE 
    {
        const int NORMAL = 0;
        const int MAGIC = 1;
        const int RARE = 2;
        const int EPIC = 3;
        Random rnd = new Random();
        double minStunResistance; // 5-25
        string desc = "";
        const int minStunResistanceMinimum = 5;
        const int minStunResistanceMaximum = 26;
        List<Tuple<string, string, double>> AdditionalStats;
        public Helmet(int rarity)
        {

            AdditionalStats = new List<Tuple<string, string, double>>();
            generateName(rarity);
            minStunResistance = rnd.Next(minStunResistanceMinimum, minStunResistanceMaximum);
            desc += "Base Armour: " + minStunResistance + "\nAdditional stats:\n";
            for (int i = 0; i < rarity; i++) { AdditionalStats.Add(generateRandomStats()); }

            MessageBox.Show(desc);
        }
        private void generateName(int rarity)
        {
            string[] prefix = new string[] { "Obsidian", "Platinum", "Golden", "Iron", "Bronze", "Horned" };
            string[] name = new string[] { "Helmet", "Basinet", "Kabuto", "Turban" };
            string[] suffix = new string[] { "Wisdom", "Protection", "Conquest", "Divinity" };
            if (rarity == NORMAL)
            {
                desc += "(NORMAL)\n" + name[rnd.Next(0, name.Length - 1)];
            }
            if (rarity == MAGIC)
            {
                desc += "(MAGIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)];

            }
            if (rarity == RARE)
            {
                desc += "(RARE)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            if (rarity == EPIC)
            {
                desc += "(EPIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            desc += '\n';
        }

        const int MAXHEALTH = 0;
        const int HEALTHRECOVERY = 1;
        const int COOLDOWNREDUCED = 2;
        const int DAMAGEPERDEBUFF = 3;
        const int DECREASEDAMAGETAKEN = 4;

        private Tuple<string, string, double> generateRandomStats()
        {

            int x = rnd.Next(0, 4);
            int range;
            Tuple<string, string, double> returnMe;
            if (x == MAXHEALTH)
            {
                range = rnd.Next(10, 40);
                returnMe = new Tuple<string, string, double>("maximumHealth", "percent", range);
                desc += "Increases your maximum health by " + range + " health points\n";
            }
            else if (x == HEALTHRECOVERY)
            {
                range = rnd.Next(3, 15);
                returnMe = new Tuple<string, string, double>("healthRecoveryRate", "percent", range);
                desc += "Increases your health recovery by " + range + " per second\n";
            }
            else if (x == COOLDOWNREDUCED)
            {
                range = rnd.Next(5, 20);
                returnMe = new Tuple<string, string, double>("lifeGainOnHit", "percent", range);
                desc += "Decreases your active skills cooldown time by " + range + "%\n";
            }
            else if (x == DAMAGEPERDEBUFF)
            {
                range = rnd.Next(10, 25);
                returnMe = new Tuple<string, string, double>("lifeGainOnHit", "percent", range);
                desc += "For every type of debuff on you, increase your dmg by " + range + "%\n";
            }
            else// (x == DECREASEDAMAGETAKEN)
            {
                range = rnd.Next(3, 10);
                returnMe = new Tuple<string, string, double>("debuffResistance", "percent", range);
                desc += "Decrease damage taken from hits by " + range + "%\n";
            }
            return returnMe;
        }

        public double getMinStunResistance() { return minStunResistance; }
        public List<Tuple<string, string, double>> getStats() { return AdditionalStats; }
        public string description()
        {

            return desc;
        }
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
        Random rnd = new Random();
        int BaseCd; // 12-20
        string desc = "";
     
        List<Tuple<string, string, double>> AdditionalStats;
        public Jewellery(int rarity)
        {

            AdditionalStats = new List<Tuple<string, string, double>>();
            generateName(rarity);
            BaseCd = rnd.Next(12, 21);
            
            desc += "Cooldown Reduction: " + BaseCd + "%\n";
            if(rarity!=NORMAL) desc += "Additional stats:\n";
                
            AdditionalStats.Add(new Tuple<string, string, double>("cooldownReduced","percent", BaseCd));
            for (int i = 0; i < rarity; i++) { AdditionalStats.Add(generateRandomStats()); }


            MessageBox.Show(desc);


        }
       
        private void generateName(int rarity)
        {
            string[] prefix = new string[] { "Golden", "Shining", "Rune", "Glutonous","Prismatic" };
            string[] name = new string[] { "Ring", "Bracelet", "Necklace" };
            string[] suffix = new string[] { "Hope", "Darkness", "White Moon", "Black Sun", "Foreseeing","Devil" };
            if (rarity == NORMAL)
            {
                desc += "(NORMAL)\n" + name[rnd.Next(0, name.Length - 1)];
            }
            if (rarity == MAGIC)
            {
                desc += "(MAGIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)];

            }
            if (rarity == RARE)
            {
                desc += "(RARE)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            if (rarity == EPIC)
            {
                desc += "(EPIC)\n" + prefix[rnd.Next(0, prefix.Length - 1)] + " " + name[rnd.Next(0, name.Length - 1)] + " of " + suffix[rnd.Next(0, suffix.Length - 1)];
            }
            desc += '\n';
        }
        const int MAXHEALTH = 0;
        const int LIFEGAINONHIT = 1;
        const int CRITDMG = 2;
        const int CRITCHANCE = 3;
        const int ITEMQUANTITY = 4;
        const int ITEMRARITY = 5;
        //const int BLEEDINGCHANCE = 6;
        private Tuple<string, string, double> generateRandomStats()
        {

            int x = rnd.Next(0, 8);
            int range;
            Tuple<string, string, double> returnMe;
            if (x == MAXHEALTH)
            {
                range = rnd.Next(20, 41);
                returnMe = new Tuple<string, string, double>("maximumHealth", "flat", range);

                desc += "Increases your max health by " + range + "\n";
            }
            else if (x == LIFEGAINONHIT)
            {
                range = rnd.Next(50, 101);
                double afterConversion = Convert.ToDouble(range) / 100;
                
                returnMe = new Tuple<string, string, double>("lifeGainOnHit", "flat", afterConversion);
                desc += "Gain " + afterConversion + " life for each enemy hit.\n";
            }
            else if (x == CRITDMG)
            {
                range = rnd.Next(15, 31);
                returnMe = new Tuple<string, string, double>("criticalDamage", "percent", range);
                desc += "Increases your critical damage by " + range + "%\n";
            }
            else if (x == CRITCHANCE)
            {
                range = rnd.Next(5, 10);
                returnMe = new Tuple<string, string, double>("absoluteCriticalHitChance", "percent", range);
                desc += "Increases your critical strike chance by " + range + "%\n";
            }
            else if (x == ITEMQUANTITY)
            {
                range = rnd.Next(10, 21);
                returnMe = new Tuple<string, string, double>("itemQuantity", "percent", range);
                desc += "Increases your chance to drop items " + range + "%\n";
            }
            else if (x == ITEMRARITY)
            {
                range = rnd.Next(20, 41);
                returnMe = new Tuple<string, string, double>("itemQuality", "percent", range);
                desc += "Increases quality of items dropped by " + range + "%\n";
            }
            else
            {
                range = rnd.Next(8, 17);
                returnMe = new Tuple<string, string, double>("bleedingChance", "percent", range);
                desc += "+ " + range + "% chance to inflict bleeding on hit.\n";
            }
            return returnMe;
        }
      
        public List<Tuple<string, string, double>> getStats() { return AdditionalStats; }
        public string description()
        {

            return desc;
        }
    };


}
