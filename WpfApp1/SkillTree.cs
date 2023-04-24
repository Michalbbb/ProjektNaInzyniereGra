using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace BasicsOfGame
{
    internal class Passive
    {
        // Tag ( fe. elementalDamage,lightningAttack), Type ( flat or percent ), value ( double, because it'll be easier to convert it that way)
        List<Tuple<string, string, double>> propertiesOfNode = new List<Tuple<string, string, double>>();
        string name="";
        string description="";
        TextBlock toolTip;
        Button visualPassive;
        Canvas currentCanvas;
        bool isBeingShown = false;
        bool isAddedToTree = false;
        public Passive(string data,Canvas canvas)
        {
            currentCanvas = canvas;
            const int NAME = 0;
            const int DESCRIPTION = 1;
            const int NAME_OF_PASSIVE = 2;
            const int TAG = 3;
            const int TYPE = 4;
            const int VALUE = 5;
            int segment=0;
            string tag = "";
            string type = "";
            string valueToConvert="";
            string nameOfPassive = "";
            bool sequenceEnded = false;
            for(int i=0; i<data.Length; i++)
            {
                if (data[i] == ';') { segment++; continue; }
                if (segment == NAME) name += data[i];
                else if (segment == DESCRIPTION) description += data[i];
                else if (segment == NAME_OF_PASSIVE) nameOfPassive += data[i];
                else if (segment > 2)
                        {
                            if ((segment % 3) + 3 == TAG)
                            {
                                if (sequenceEnded)
                                {
                                    sequenceEnded = false;
                                    propertiesOfNode.Add(new Tuple<string, string, double>(tag, type, Convert.ToDouble(valueToConvert)));
                                    tag = "";
                                    type = "";
                                    valueToConvert = "";
                                }
                                tag += data[i];
                            }
                            else if ((segment % 3) + 3 == TYPE)
                            {

                                type += data[i];
                            }
                            else if ((segment % 3) + 3 == VALUE)
                            {
                                valueToConvert += data[i];
                                sequenceEnded = true;
                            }
                        }
                
            }
            propertiesOfNode.Add(new Tuple<string, string, double>(tag, type, Convert.ToDouble(valueToConvert)));

            toolTip = new TextBlock();
            toolTip.Text = name+"\n"+description;
            toolTip.IsEnabled = false;
            toolTip.FontSize = 10.0;
            int start = toolTip.Text.IndexOf(name);
            int length = name.Length + 1;
            TextPointer startPtr = toolTip.ContentStart.GetPositionAtOffset(start);
            TextPointer endPtr = startPtr.GetPositionAtOffset(length);
            TextRange boldRange = new TextRange(startPtr, endPtr);
            boldRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            boldRange.ApplyPropertyValue(TextElement.FontSizeProperty, 15.0);
            toolTip.Background = Brushes.OrangeRed;
            toolTip.Foreground = Brushes.AliceBlue;
            toolTip.Padding = new Thickness(10);
            visualPassive = new Button();
            visualPassive.Opacity = 0.5;
            visualPassive.Style = (Style)Application.Current.MainWindow.FindResource("InformButton");
            //visualPassive.Style = (Style)FindResource("InformButton");
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/{nameOfPassive}.png", UriKind.Absolute));
            visualPassive.Background = sprite;
            visualPassive.Foreground = Brushes.Black;


            Canvas.SetLeft(visualPassive, 580);
            Canvas.SetTop(visualPassive, 280);
            Canvas.SetZIndex(visualPassive, 1501);
            Canvas.SetZIndex(toolTip, 1502);
            visualPassive.Width = 40;
            visualPassive.Height = 40;
            visualPassive.MouseMove += showToolTip;
            visualPassive.MouseLeave += hideToolTip;

            visualPassive.Click += tryAllocating;

        }
        public void showPassiveInTree()
        {
            if(!isAddedToTree)currentCanvas.Children.Add(visualPassive);
            isAddedToTree=true;
        }
        public void removeFromTree()
        {
            if (!isAddedToTree) return;
            else
            {
                isAddedToTree= false;
                currentCanvas.Children.Remove(visualPassive);
            }
        }

        private void hideToolTip(object sender, MouseEventArgs e)
        {
            isBeingShown = false;
            currentCanvas.Children.Remove(toolTip);
        }

        private void showToolTip(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(currentCanvas);
            Canvas.SetLeft(toolTip, p.X + 15);
            Canvas.SetTop(toolTip, p.Y + 15);

            if (!isBeingShown) currentCanvas.Children.Add(toolTip);
            isBeingShown = true;
        }

        private void tryAllocating(object sender, RoutedEventArgs e)
        {
            if (visualPassive.Opacity == 1) visualPassive.Opacity = 0.5;
            else visualPassive.Opacity = 1;

        }
    }
    internal class SkillTree
    {
        Canvas currentCanvas;
        Passive demo;
        GroupBox tree;
        TextBox description;
        bool isBeingShown;
        public SkillTree(Canvas canvas)
        {
            currentCanvas = canvas;
            string data= "Might of the bear;+10 to maximum health\n+1 to minimum damage;passive56;maximumHealth;flat;10;minimumDamage;flat;1"; 
            demo = new Passive(data, currentCanvas);
            tree = new GroupBox();
            tree.Width = 720;
            tree.Height = 480;
            description= new TextBox();
            description.Text = " SKILL TREE ";
            description.FontFamily = new FontFamily("Algerian");
            description.FontSize = 50;
            Canvas.SetLeft(tree, (currentCanvas.Width / 2) - (tree.Width / 2));
            Canvas.SetTop(tree, (currentCanvas.Height / 2) - (tree.Height / 2));
            Canvas.SetZIndex(tree, 1500);
            description.Width = 300;
            description.Height = 60;
            description.Opacity = 1;
            description.Background = Brushes.Black;
            description.Foreground = Brushes.White;
            

            Canvas.SetLeft(description, (currentCanvas.Width / 2) - (description.Width/2));
            Canvas.SetTop(description,0);
            Canvas.SetZIndex(description, 1500);
            description.IsEnabled = false;
            tree.Background = Brushes.Black;
            tree.BorderThickness = new Thickness(0);

        }
        public void showSkillTree()
        {
            if (isBeingShown) return;
            else
            {
                isBeingShown = true;
                currentCanvas.Children.Add(tree);
                currentCanvas.Children.Add(description);
                demo.showPassiveInTree();
            }
        }
        public void hideSkillTree()
        {
            if (!isBeingShown) return;
            else
            {
                isBeingShown = false;
                currentCanvas.Children.Remove(tree);
                currentCanvas.Children.Remove(description);
                demo.removeFromTree();
            }
        }
        
    }
}
