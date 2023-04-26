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
        Button closeTree;
        Button acceptChanges;
        bool isBeingShown;
        TextBox assignedCurrently;
        public SkillTree(Canvas canvas)
        {
            currentCanvas = canvas;
            string data= "Might of the bear;+10 to maximum health\n+1 to minimum damage;passive56;maximumHealth;flat;10;minimumDamage;flat;1"; 
            demo = new Passive(data, currentCanvas);
            tree = new GroupBox();
            tree.Width = 1200;
            tree.Height = 601;
            ImageBrush bgForST = new ImageBrush();
            bgForST.ImageSource= new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/skillTreeBackground.png", UriKind.Absolute));
            tree.Background =bgForST;
            
            Canvas.SetLeft(tree, 0);
            Canvas.SetTop(tree, -1);
            Canvas.SetZIndex(tree, 1500);
            closeTree = new Button();
            closeTree.Style= (Style)Application.Current.MainWindow.FindResource("InformButton");
            closeTree.Content = "X";
            closeTree.Click += hideST;
            closeTree.MouseEnter += changeColor;
            closeTree.MouseLeave += changeColor;
            Canvas.SetLeft(closeTree, 1110);
            Canvas.SetTop(closeTree, 10);
            Canvas.SetZIndex(closeTree, 1501);
            closeTree.Background = Brushes.White;
            closeTree.Foreground = Brushes.Black;
            closeTree.Width = 50;
            closeTree.Height = 50;
            acceptChanges = new Button();
            acceptChanges.Style = (Style)Application.Current.MainWindow.FindResource("InformButton");
            acceptChanges.Content = "✓";
            acceptChanges.Click += updateTree;
            acceptChanges.IsEnabled = false;
            acceptChanges.Opacity = 0.7;
            Canvas.SetLeft(acceptChanges, 1050);
            Canvas.SetTop(acceptChanges, 10);
            Canvas.SetZIndex(acceptChanges, 1501);
            acceptChanges.Background = Brushes.White;
            acceptChanges.Foreground = Brushes.Black;
            acceptChanges.Width = 50;
            acceptChanges.Height = 50;
            
            assignedCurrently = new TextBox();
            assignedCurrently.FontFamily = new FontFamily("Algerian");
            assignedCurrently.Background = Brushes.Transparent;
            assignedCurrently.BorderBrush = Brushes.Transparent;
            assignedCurrently.Foreground = Brushes.White;
            assignedCurrently.IsEnabled= false;
            assignedCurrently.FontSize = 23;
            Canvas.SetLeft(assignedCurrently, 10);
            Canvas.SetTop(assignedCurrently, 10);
            Canvas.SetZIndex(assignedCurrently, 1501);
            tree.BorderThickness = new Thickness(0);
        }

        private void changeColor(object sender, MouseEventArgs e)
        {
           if(closeTree.Foreground == Brushes.Black)closeTree.Foreground= Brushes.Red;
           else if(closeTree.Foreground == Brushes.Red)closeTree.Foreground= Brushes.Black;

        }

        private void updateTree(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void hideST(object sender, RoutedEventArgs e)
        {
            hideSkillTree();
        }

        public void showSkillTree()
        {
            if (isBeingShown) return;
            else
            {
                isBeingShown = true;
                assignedCurrently.Text = "Remaining skill points: "+Player.unassignedSkillPoints+"\nAssigned skill points: "+Player.assignedSkillPoints;
                currentCanvas.Children.Add(tree);
                currentCanvas.Children.Add(closeTree);
                currentCanvas.Children.Add(assignedCurrently);
                currentCanvas.Children.Add(acceptChanges);
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
                currentCanvas.Children.Remove(closeTree);
                currentCanvas.Children.Remove(assignedCurrently);
                currentCanvas.Children.Remove(acceptChanges);
                demo.removeFromTree();
                currentCanvas.Focus();
            }
        }
        
    }
}
