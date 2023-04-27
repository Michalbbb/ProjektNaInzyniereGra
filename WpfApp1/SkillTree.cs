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
using System.IO;

namespace BasicsOfGame
{
    internal class Passive
    {
        // Tag ( fe. elementalDamage,lightningAttack), Type ( flat or percent ), value ( double, because it'll be easier to convert it that way)
        List<Tuple<string, string, double>> propertiesOfNode = new List<Tuple<string, string, double>>();
        string name="";
        string description="";
        int stages=0;
        int currentStage = 0;
        TextBlock toolTip;
        Button visualPassive;
        Canvas currentCanvas;
        bool isBeingShown = false;
        bool isAddedToTree = false;
        Action up;
        public Passive(string data,Canvas canvas,Action update)
        {
            up = update;
            currentCanvas = canvas;
            const int NAME = 0;
            const int DESCRIPTION = 1;
            const int NAME_OF_PASSIVE_IMAGE = 2;
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
                else if (segment == NAME_OF_PASSIVE_IMAGE) nameOfPassive += data[i];
                else if (segment > 2)
                        {
                            if ((segment % 3) + 3 == TAG)
                            {
                                if (sequenceEnded)
                                {
                                    sequenceEnded = false;
                                    stages++;
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
            stages++;
            propertiesOfNode.Add(new Tuple<string, string, double>(tag, type, Convert.ToDouble(valueToConvert)));

            toolTip = new TextBlock();
            
            toolTip.IsEnabled = false;
            toolTip.FontSize = 10.0;
            toolTip.Background = Brushes.OrangeRed;
            toolTip.Foreground = Brushes.AliceBlue;
            toolTip.Padding = new Thickness(10);
            visualPassive = new Button();
            visualPassive.Opacity = 0.75;
            visualPassive.Style = (Style)Application.Current.MainWindow.FindResource("InformButton");
           
            ImageBrush sprite = new ImageBrush();
            sprite.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/passives/{nameOfPassive}.png", UriKind.Absolute));
            visualPassive.Background = sprite;
            visualPassive.Foreground = Brushes.Black;


            Canvas.SetLeft(visualPassive, 120);
            Canvas.SetTop(visualPassive, 120);
            Canvas.SetZIndex(visualPassive, 1501);
            Canvas.SetZIndex(toolTip, 1502);
            visualPassive.Width = 60;
            visualPassive.Height = 60;
            visualPassive.MouseMove += showToolTip;
            visualPassive.MouseLeave += hideToolTip;

            visualPassive.Click += tryAllocating;
            visualPassive.MouseRightButtonDown += cancelOneStageOfAllocating;

        }

        private void cancelOneStageOfAllocating(object sender, MouseButtonEventArgs e)
        {
            if (currentStage < 1) return;
            currentStage--; updateToolTip();
            Player.unassignedSkillPoints++; Player.assignedSkillPoints--; SkillTree.updateAssignedSkillPoints(); up();
            if(currentStage==0) visualPassive.Opacity = 0.75;
        }

        private void tryAllocating(object sender, RoutedEventArgs e)
        {


            if (Player.unassignedSkillPoints < 1) return;
            else
            {
                if (currentStage < stages)
                {
                    Player.unassignedSkillPoints--; Player.assignedSkillPoints++; SkillTree.updateAssignedSkillPoints(); up();
                currentStage++; updateToolTip(); 
                visualPassive.Opacity = 1;
                }
            }
           
        }

        public void setPos(int x, int y)
        {
            Canvas.SetLeft(visualPassive, x);
            Canvas.SetTop(visualPassive, y);

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
        private void updateToolTip()
        {
            toolTip.Text = name;
           
            toolTip.Text += "\nCurrent effect: ";
            if (currentStage == 0) { toolTip.Text += "none"; }
            else
            {
                toolTip.Text += description + " " + propertiesOfNode[currentStage - 1].Item3.ToString();
                if (propertiesOfNode[currentStage - 1].Item2 == "percent") toolTip.Text += "%";


            }

            if (currentStage < stages)
            {
                toolTip.Text += "\n";
                toolTip.Text += "Next level: ";
                toolTip.Text += description + " " + propertiesOfNode[currentStage].Item3.ToString();
                if (propertiesOfNode[currentStage].Item2 == "percent") toolTip.Text += "%";
            }
            int start = toolTip.Text.IndexOf(name);
            int length = name.Length + 1;

            TextPointer startPtr = toolTip.ContentStart.GetPositionAtOffset(start);
            TextPointer endPtr = startPtr.GetPositionAtOffset(length);
            TextRange boldRange = new TextRange(startPtr, endPtr);
            boldRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            boldRange.ApplyPropertyValue(TextElement.FontSizeProperty, 15.0);
        }
        private void showToolTip(object sender, MouseEventArgs e)
        {
            
            toolTip.Text = name;
            
            toolTip.Text += "\nCurrent effect: ";
            if(currentStage==0) { toolTip.Text += "none"; }
            else
            {
                toolTip.Text += description+" "+propertiesOfNode[currentStage-1].Item3.ToString();
                if (propertiesOfNode[currentStage-1].Item2 == "percent") toolTip.Text += "%";


            }
            
            if (currentStage < stages)
            {
                toolTip.Text += "\n";
                toolTip.Text += "Next level: ";
                toolTip.Text += description + " " + propertiesOfNode[currentStage].Item3.ToString();
                if (propertiesOfNode[currentStage].Item2 == "percent") toolTip.Text += "%";
            }
            int start = toolTip.Text.IndexOf(name);
            int length = name.Length + 1;
            TextPointer startPtr = toolTip.ContentStart.GetPositionAtOffset(start);
            TextPointer endPtr = startPtr.GetPositionAtOffset(length);
            TextRange boldRange = new TextRange(startPtr, endPtr);
            boldRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            boldRange.ApplyPropertyValue(TextElement.FontSizeProperty, 15.0);
            Point p = Mouse.GetPosition(currentCanvas);
            if (p.X>1000)
            Canvas.SetLeft(toolTip, p.X - toolTip.ActualWidth);
            else Canvas.SetLeft(toolTip, p.X + 15);
            if(p.Y>520) Canvas.SetTop(toolTip, p.Y + 15 - toolTip.ActualHeight);
            else Canvas.SetTop(toolTip, p.Y + 15);
            if (!isBeingShown) currentCanvas.Children.Add(toolTip);
            isBeingShown = true;
        }

       
    }
    internal class SkillTree
    {
        Canvas currentCanvas;
        private Passive[] skills=new Passive[5];
        GroupBox tree;
        Button closeTree;
        Button acceptChanges;
        bool isBeingShown;
        static TextBox assignedCurrently;
        private int skillPointsAtTimeOfClick;
        public SkillTree(Canvas canvas)
        {
            currentCanvas = canvas;
            string solutionDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..","..","..", "images");
            string filePath = Path.Combine(solutionDir, "passives", "passivesInfo.txt");
            StreamReader passives = new StreamReader(filePath);
            string data;
            for(int i = 0; i < 5; i++)
            {
                data=passives.ReadLine();
                skills[i] = new Passive(data, currentCanvas,updateAcceptChanges);
            }
            int x; 
            int y = 120;
            //for(int i = 0; i < 5; i++)
            //{
                x = 300;
                for(int j = 0; j < 5; j++)
                {
                    skills[j].setPos(x, y); //i*5
                    x += 200;
                }
                y += 100;
            //}
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
        public static void updateAssignedSkillPoints()
        {
            assignedCurrently.Text = "Remaining skill points: " + Player.unassignedSkillPoints + "\nAssigned skill points: " + Player.assignedSkillPoints;
            
        }
        private void updateAcceptChanges()
        {
            if(Player.unassignedSkillPoints<skillPointsAtTimeOfClick)
            {
                acceptChanges.Opacity = 1;
                acceptChanges.IsEnabled = true;
                acceptChanges.MouseEnter += changeColorAC;
                acceptChanges.MouseLeave += changeColorAC;
            }
        }

        private void changeColorAC(object sender, MouseEventArgs e)
        {
            if (acceptChanges.Foreground == Brushes.Black) acceptChanges.Foreground = Brushes.LightGreen;
            else if (acceptChanges.Foreground == Brushes.LightGreen) acceptChanges.Foreground = Brushes.Black;
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
               
                skillPointsAtTimeOfClick = Player.unassignedSkillPoints;
                isBeingShown = true;
                assignedCurrently.Text = "Remaining skill points: "+Player.unassignedSkillPoints+"\nAssigned skill points: "+Player.assignedSkillPoints;
                currentCanvas.Children.Add(tree);
                currentCanvas.Children.Add(closeTree);
                currentCanvas.Children.Add(assignedCurrently);
                currentCanvas.Children.Add(acceptChanges);
                foreach (Passive pas in skills) pas.showPassiveInTree();
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
                foreach (Passive pas in skills) pas.removeFromTree();
                currentCanvas.Focus();
            }
        }
        
    }
}
