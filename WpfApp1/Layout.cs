using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Numerics;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace BasicsOfGame
{

    internal class Pokoj
    {
        int type, objectCount, enemyCount;
        Random rnd = new Random();
        bool left = false, up = false, right = false, down = false;
        MapsObjects [] roomContent;
        List<Monster> monsters;
        Canvas BelongTo;
        Point[,] objectGrid;
        bool[,] availableOnGrid;
        bool visited;
        public Action endGame;
        
        public Pokoj(){ // Empty Room
                type=0;
        }
        public Pokoj(Canvas canv,int t,string usageOfRoom, int roomWeight)
        {
            type = t;                       //type determines background image for that room
            objectCount = rnd.Next(0, 4);   //from 0 up to 3 objects
            enemyCount = 0;
            BelongTo = canv;
            visited = false;

            //below we setup a grid for spawning objects
            int gridX = 7, gridY = 3, gridBlockSizeX=162, gridBlockSizeY=166;
            int tempGridX=(gridBlockSizeX/2), tempGridY;

            objectGrid = new Point[gridX, gridY];
            for (int i = 0; i < gridX; ++i)
            {
                tempGridY = (94 + (gridBlockSizeY / 2));
                for (int j = 0; j < gridY; ++j)
                {
                    objectGrid[i, j] = new Point(tempGridX, tempGridY);
                    tempGridY += gridBlockSizeY;
                }
                tempGridX += gridBlockSizeX;
            }

            availableOnGrid = new bool[gridX, gridY]; //defining array of taken slots in object grid
            for (int i = 0; i < gridX; ++i)
            {
                for (int j = 0; j < gridY; ++j)
                {
                    availableOnGrid[i, j] = true;
                    if (i == gridX/2)                   //block of "if" statements below is used to automatically restrict grid spaces near door from being used
                    {
                        if(j == 0 || j == gridY - 1)
                        {
                            availableOnGrid[i, j] = false;
                        }
                    }
                    else if (j == 1)
                    {
                        if(i==0 || i == gridX - 1)
                        {
                            availableOnGrid[i, j] = false;
                        }
                    }
                }
            }

            //below we generate an array of objects and then to the array we randomize the exact object
            roomContent = new MapsObjects[objectCount];
            monsters = new List<Monster>();
            int objectType, objectPlacementX, objectPlacementY;
            bool correctPlacement;

            //here we decide the initial object placement in the room
            for(int i=0; i < objectCount; ++i)
            {
                objectType = rnd.Next(0, 4);
                do
                {
                    objectPlacementX = rnd.Next(0, gridX);
                    objectPlacementY = rnd.Next(0, gridY);

                    if (availableOnGrid[objectPlacementX, objectPlacementY])
                    {
                        correctPlacement = true;
                        availableOnGrid[objectPlacementX, objectPlacementY] = false;
                    }
                    else
                        correctPlacement = false;


                } while (!correctPlacement);
                roomContent[i] = new MapsObjects(objectType, (int)objectGrid[objectPlacementX,objectPlacementY].X, (int)objectGrid[objectPlacementX,objectPlacementY].Y);
            }


            //below we do the same for monsters (placement)
            int x1, y1;
            int currRoomWeight = 0; //Current "Weight" of the room, which we compare to roomWeight to check whether we made the room difficult enough
            const int maxEnemyCount = 7;

            if (usageOfRoom != "startingRoom")
            {
                for (int i = 0; i < maxEnemyCount; i++)
                {
                    Monster addMeToList;
                    int whichOne = rnd.Next(0, 4);

                    switch (whichOne)
                    {
                        case 0:
                        case 1:
                            currRoomWeight += 3;
                            break;
                        case 2:
                            currRoomWeight += 5;
                            break;
                        case 3:
                            currRoomWeight++;
                            break;
                    }

                    if (currRoomWeight > roomWeight) break; //if adding next monster would mean that the room would be "too difficult" then we break;

                    do
                    {
                        objectPlacementX = rnd.Next(0, gridX);
                        objectPlacementY = rnd.Next(0, gridY);

                        if (availableOnGrid[objectPlacementX, objectPlacementY])
                        {
                            correctPlacement = true;
                            availableOnGrid[objectPlacementX, objectPlacementY] = false;
                        }
                        else
                            correctPlacement = false;


                    } while (!correctPlacement);
                    x1 = (int)objectGrid[objectPlacementX, objectPlacementY].X;
                    y1 = (int)objectGrid[objectPlacementX, objectPlacementY].Y;

                    if (whichOne == 0) addMeToList = new Spider(canv, x1, y1);
                    else if (whichOne == 1) addMeToList = new Imp(canv, x1, y1);
                    else if (whichOne == 2) addMeToList = new Golem(canv, x1, y1);
                    else addMeToList = new Goblin(canv, x1, y1);

                    monsters.Add(addMeToList);
                    enemyCount++;
                }
            }
        }
        const int SENJURO=0;
        const int SEHN=1;
        public void changeRoomUsage(string usage){
            if(type==0) return; // In case if something goes wrong
            if(usage=="bossRoom"){
            monsters = new List<Monster>();
            enemyCount=1; // boss
            int gridX = 7, gridY = 3;
            int objectPlacementX, objectPlacementY;
            bool correctPlacement;
            int x1, y1;

            for (int i=0;i<enemyCount;i++)
            {
                Monster addMeToList;
                int whichOne = rnd.Next(0, 4);

                do
                {
                    objectPlacementX = rnd.Next(0, gridX);
                    objectPlacementY = rnd.Next(0, gridY);

                    if (availableOnGrid[objectPlacementX, objectPlacementY])
                    {
                        correctPlacement = true;
                        availableOnGrid[objectPlacementX, objectPlacementY] = false;
                    }
                    else
                        correctPlacement = false;


                } while (!correctPlacement);
                x1 = (int)objectGrid[objectPlacementX, objectPlacementY].X; 
                y1 = (int)objectGrid[objectPlacementX, objectPlacementY].Y;
                    int boss = rnd.Next(0, 3);
                    if(boss==SENJURO)addMeToList = new ghostOfSenjuro(BelongTo, x1, y1);
                    else if(boss==SEHN)addMeToList = new oldGreatOne(BelongTo,x1,y1);
                    else addMeToList = new DemonOfBelow(BelongTo, x1, y1);
                    monsters.Add(addMeToList);
            }
            }
          
        }
        public bool isCleared()
        {
            if (monsters.Count == 0) return true;
            return false;
        }
        public void setDiff(double plusDiff)
        {
            
            for (int i = 0; i < enemyCount; i++)
            {
                monsters[i].setDiff(plusDiff);
            }
        }
        public bool checkIfDead(Monster y,ref int exp)
        {
            if (y.IsDead())
            {
                exp += y.expOnDeath();
                y.remove();
                monsters.Remove(y);
                if (type == 2)
                {
                    endGame?.Invoke();
                    
                }
                if (monsters.Count == 0)
                {
                    ImageBrush tempHolder = new ImageBrush(); 
                    foreach(System.Windows.Shapes.Rectangle rect in BelongTo.Children.OfType<System.Windows.Shapes.Rectangle>())
                    {
                        if ((string)rect.Tag == "door")
                        {
                            if (rect.Width == 8)
                            {
                                tempHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/leftDoor.png", UriKind.Absolute));
                                rect.Fill = tempHolder;

                            }
                            if(rect.Width == 50)
                            {
                                tempHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/rightDoor.png", UriKind.Absolute));
                                rect.Fill = tempHolder;
                            }
                            if (rect.Width == 125)
                            {
                                tempHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/upDoor.png", UriKind.Absolute));
                                rect.Fill = tempHolder;
                            }
                            if (rect.Width == 130)
                            {
                                tempHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/DownDoor.png", UriKind.Absolute));
                                rect.Fill = tempHolder;
                            }
                        }
                    }
                }
                return true;
            }
            return false;

        } 
        public List<Monster> monArr()
        {
            return monsters;
        } 
        private int abs(int x)
        {
            if (x > 0) return x;
            else return -x;
        }
        public void setType(int t)
        {
            type = t;
        }
        public int getType()
        {
            return type;
        }
        public void setDoors(bool upDoors, bool leftDoors, bool downDoors, bool rightDoors)
        {
            left = leftDoors;
            down = downDoors;
            right = rightDoors;
            up = upDoors;
        }
        public void makeMap(Canvas GameScreen, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
           
            makeBackground(GameScreen, left, right, up, down,type, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist,doorDirection);
        }
        
        
        public void makeBackground(Canvas GameScreen, bool leftDoor, bool rightDoor, bool upDoor, bool downDoor, int tlo, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
            ImageBrush noweTlo = new ImageBrush();
            noweTlo.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/backgrounds/bg{tlo}.png", UriKind.Absolute));
            GameScreen.Background = noweTlo;

            leftDoorExist = false; // Reset usable doors 
            rightDoorExist = false;
            upDoorExist = false;
            downDoorExist = false;

            var toRemove = new List<System.Windows.Shapes.Rectangle>();

            foreach (System.Windows.Shapes.Rectangle x in GameScreen.Children.OfType<System.Windows.Shapes.Rectangle>()) // removing remains of doors
            {
                if ((string)x.Name == "Player")
                {
                    switch (doorDirection)  //we substract 20 from the position of the door next to which we spawn the player
                    {
                        case 0: //when player enters top door spawn him at bottom door in next room
                            Canvas.SetLeft(x, 540);
                            Canvas.SetTop(x, 594 - 109);
                            break;
                        case 1: //when player enters right door spawn him at left door in next room
                            Canvas.SetLeft(x, 0 + 20);
                            Canvas.SetTop(x, 288);
                            break;
                        case 2: //when player enters bottom door spawn him at top door in next room
                            Canvas.SetLeft(x, 540);
                            Canvas.SetTop(x, 0 + 20);
                            break;
                        case 3: //when player enters left door spawn him at right door in next room
                            Canvas.SetLeft(x, 1150 - 20);
                            Canvas.SetTop(x, 288);
                            break;
                        case 4: //special case for spawning player at the beginning of the game
                            Canvas.SetLeft(x, 540);
                            Canvas.SetTop(x, 288);
                            break;
                    }
                }
                if ((string)x.Tag == "door" || (string)x.Tag == "obstacle" || (string)x.Tag == "collision")
                {
                    toRemove.Add(x);
                }
            }
           
            if (leftDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 8;
                door.Height = 130;
                Canvas.SetLeft(door, 0);
                Canvas.SetTop(door, 260);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                if(isCleared())temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/leftDoor.png", UriKind.Absolute));
                else temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/leftDoorB.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                leftDoorExist = true;
            }
            if (rightDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 50;
                door.Height = 150;
                Canvas.SetLeft(door, 1150);
                Canvas.SetTop(door, 250);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                if (isCleared()) temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/rightDoor.png", UriKind.Absolute));
                else temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/rightDoorB.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                rightDoorExist = true;
            }
            if (upDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 125;
                door.Height = 94;
                Canvas.SetLeft(door, 507);
                Canvas.SetTop(door, 0);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                if (isCleared()) temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/upDoor.png", UriKind.Absolute));
                else temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/upDoorB.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                upDoorExist = true;
            }
            if (downDoor)
            {
                ImageBrush temporaryHolder = new ImageBrush();
                System.Windows.Shapes.Rectangle door = new System.Windows.Shapes.Rectangle();
                door.Width = 130;
                door.Height = 6;
                Canvas.SetLeft(door, 509);
                Canvas.SetTop(door, 594);
                Canvas.SetZIndex(door, -1);
                door.Tag = "door";
                if (isCleared()) temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/downDoor.png", UriKind.Absolute));
                else temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/downDoorB.png", UriKind.Absolute));
                door.Fill = temporaryHolder;
                GameScreen.Children.Add(door);
                downDoorExist = true;
            }
            int j = toRemove.Count() - 1;

            for (int i = j; i > -1; --i)
            {
                GameScreen.Children.Remove(toRemove[i]);    //deleting objects when changing rooms
                toRemove.RemoveAt(i);
            }
            foreach(MapsObjects obj in roomContent)
            {
                obj.Add(GameScreen);

            }
            foreach(Monster monster in monsters)
            {
                monster.add();
            }


            
        }

        public void setVisited(bool newValue)
        {
            visited = newValue;
        }

        public bool getVisited()
        {
            return visited;
        }
    }

    internal class Grid
    {
        public Pokoj[,] grid;
        int roomCount =16;
        static int gridSize = 9;
        int currX, currY;
        static int gridMid = gridSize / 2;
        Random rnd = new Random();
        int direction;
        int firstDoor = -1, lastDoor;
        List<System.Windows.Shapes.Rectangle> miniMapRectangles = new List<System.Windows.Shapes.Rectangle>();
        List<Label> miniMapQuestionMarks = new List<Label>();

        private int[,] isAssignedAs = new int[gridSize, gridSize];
        private const int NOTHING = 0;
        private const int BASIC_ROOM = 1;
        private const int QUESTION_MARK_ROOM = 2;
        private const int BOSS_ROOM = 3;
        private const int TREASURE_ROOM = 4;
        public Action gameWon;

        Canvas canvas;
        private bool isMiniMapBeingUsed=false;

        public void endGame()
        {
            gameWon?.Invoke();
        }
        public Grid(Canvas canv)
        {
            grid = new Pokoj[gridSize, gridSize]; //if 0 then no room and 1,2,3,etc. mean different types of rooms
            for(int i=0;i<gridSize; i++)
            {
                 for(int j=0;j<gridSize; j++)
                {
                    isAssignedAs[i, j] = NOTHING;
                    
                }
            }
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = new Pokoj();
                    
                    
                }
            }
            canvas = canv;
            grid[gridMid, gridMid].setType(1);
            roomCount--;
            currX = gridMid;
            currY = gridMid;
            int type=1; // from now onwards we generate type of room in generateDoors
            

            while (roomCount > 0)
            {
                direction = rnd.Next(0, 4);
            
                switch (direction)
                {
                    case 0:
                        if (currX > 0)
                        {
                            if (!CheckRoom(currX - 1, currY))//check to left
                            {
                                
                                grid[currX - 1, currY].setType(type);
                                roomCount--;
                                currX -= 1;
                            }
                            else
                                currX -= 1;
                        }
                        break;
                    case 1:
                        if (currY > 0)
                        {
                            if (!CheckRoom(currX, currY - 1))//check to up
                            {
                                grid[currX, currY - 1].setType(type);
                                roomCount--;
                                currY -= 1;
                            }
                            else
                                currY -= 1;
                        }
                        break;
                    case 2:
                        if (currX < gridSize - 1)
                        {
                            if (!CheckRoom(currX + 1, currY))//check to right
                            {
                                grid[currX + 1, currY].setType(type);
                                roomCount--;
                                currX += 1;
                            }
                            else
                                currX += 1;
                        }
                        break;
                    case 3:
                        if (currY < gridSize - 1)
                        {
                            if (!CheckRoom(currX, currY + 1))//check to down
                            {
                                grid[currX, currY + 1].setType(type);
                                roomCount--;
                                currY += 1;
                            }
                            else
                                currY += 1;
                        }
                        break;
                }

            }
            
            generateDoors(canv);
        }
        public void updateMiniMap(GroupBox c)  //minimapa
        {
            if (isMiniMapBeingUsed)
            {
                foreach (Rectangle rectangle in miniMapRectangles) canvas.Children.Remove(rectangle);
                foreach (Label questionMark in miniMapQuestionMarks) canvas.Children.Remove(questionMark);
            }
            
            if (grid[currX, currY].getVisited() && isAssignedAs[currX,currY]!= BASIC_ROOM)
            {
                if (isAssignedAs[currX, currY] == QUESTION_MARK_ROOM)
                {
                    for(int i = miniMapQuestionMarks.Count - 1; i >= 0; i--)
                    {
                        // Question mark room becoming basic room ( delete question mark remains from list of question mark rectangles)
                        if (Canvas.GetLeft(miniMapQuestionMarks[i]) == (Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize))) && Canvas.GetTop(miniMapQuestionMarks[i]) ==(Canvas.GetTop(c) + (currX * ((c.Height) / gridSize))) )
                        {
                            miniMapQuestionMarks.Remove(miniMapQuestionMarks[i]);
                        }
                    }
                }
                isAssignedAs[currX, currY] = BASIC_ROOM;
                        
                Rectangle square = new Rectangle();
                square.Width = (c.Width / gridSize);
                square.Height = (c.Height / gridSize);
                square.Fill = Brushes.White;
                square.Opacity = 0.55;
                Canvas.SetZIndex(square, 1000);
                Canvas.SetLeft(square, Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize)));
                Canvas.SetTop(square, Canvas.GetTop(c) + (currX * ((c.Height) / gridSize)));

                miniMapRectangles.Add(square);
                        //rysujemy kwadracik
        
                if (currX - 1 >= 0 && !grid[currX - 1, currY].getVisited()&& grid[currX-1,currY].getType()!=0 && isAssignedAs[currX - 1, currY] != QUESTION_MARK_ROOM)
                {
                    isAssignedAs[currX-1, currY] = QUESTION_MARK_ROOM;
                    Label question = new Label();
                    question.Width = c.Width / gridSize;
                    question.Height = c.Height / gridSize;
                    if (grid[currX - 1, currY].getType() == 1) question.Content = "  ?";
                    else if (grid[currX - 1, currY].getType() == 2) question.Content = "  B";
                    question.Foreground = Brushes.White;
                    question.BorderBrush = Brushes.White;
                    question.FontSize = 20;
                    question.Opacity = 0.55;
                    Canvas.SetZIndex(question, 1000);
                    Canvas.SetLeft(question, Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize)));
                    Canvas.SetTop(question, Canvas.GetTop(c) + ((currX - 1) * ((c.Height) / gridSize)));

                    miniMapQuestionMarks.Add(question);
                    //grid[currX-1,currY].getType()==0 oznacza, ze pokoj jest nie uzywany, więc !=0 oznacza, że jest używany.
                    //rysujemy znak zapytania

                }
                if (currY - 1 >= 0 && !grid[currX, currY - 1].getVisited() && grid[currX, currY - 1].getType() != 0 && isAssignedAs[currX, currY - 1] != QUESTION_MARK_ROOM)
                {
                    isAssignedAs[currX, currY - 1] = QUESTION_MARK_ROOM;
                    Label question = new Label();
                    question.Width = c.Width / gridSize;
                    question.Height = c.Height / gridSize;
                    if (grid[currX, currY - 1].getType() == 1) question.Content = "  ?";
                    else if (grid[currX, currY - 1].getType() == 2) question.Content = "  B";
                    question.Foreground = Brushes.White;
                    question.BorderBrush = Brushes.White;
                    question.FontSize = 20;
                    question.Opacity = 0.55;
                    Canvas.SetZIndex(question, 1000);
                    Canvas.SetLeft(question, Canvas.GetLeft(c) + ((currY - 1) * ((c.Width) / gridSize)));
                    Canvas.SetTop(question, Canvas.GetTop(c) + ((currX) * ((c.Height) / gridSize)));

                    miniMapQuestionMarks.Add(question);
                    //rysujemy znak zapytania
                }
                if (currX + 1 < gridSize && !grid[currX + 1, currY].getVisited() && grid[currX + 1, currY].getType()!=0 && isAssignedAs[currX + 1, currY] != QUESTION_MARK_ROOM)
                {
                    isAssignedAs[currX + 1, currY] = QUESTION_MARK_ROOM;
                    Label question = new Label();
                    question.Width = c.Width / gridSize;
                    question.Height = c.Height / gridSize;
                    if (grid[currX + 1, currY].getType() == 1) question.Content = "  ?";
                    else if (grid[currX + 1, currY].getType() == 2) question.Content = "  B";
                    question.Foreground = Brushes.White;
                    question.BorderBrush = Brushes.White;
                    question.FontSize = 20;
                    question.Opacity = 0.55;
                    Canvas.SetZIndex(question, 1000);
                    Canvas.SetLeft(question, Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize)));
                    Canvas.SetTop(question, Canvas.GetTop(c) + ((currX + 1) * ((c.Height) / gridSize)));

                    miniMapQuestionMarks.Add(question);
                    //rysujemy znak zapytania
                }
                if (currY + 1 < gridSize && !grid[currX, currY + 1].getVisited() && grid[currX, currY+1].getType() != 0 && isAssignedAs[currX, currY + 1] != QUESTION_MARK_ROOM)
                {
                    isAssignedAs[currX, currY + 1] = QUESTION_MARK_ROOM;
                    Label question = new Label();
                    question.Width = c.Width / gridSize;
                    question.Height = c.Height / gridSize;
                    if(grid[currX, currY + 1].getType()==1)question.Content = "  ?";
                    else if (grid[currX, currY + 1].getType() == 2) question.Content = "  B";
                    question.Foreground = Brushes.White;
                    question.BorderBrush = Brushes.White;
                    question.FontSize = 20;
                    question.Opacity = 0.55;
                    Canvas.SetZIndex(question, 1000);
                    Canvas.SetLeft(question, Canvas.GetLeft(c) + ((currY + 1) * ((c.Width) / gridSize)));
                    Canvas.SetTop(question, Canvas.GetTop(c) + ((currX) * ((c.Height) / gridSize)));

                    miniMapQuestionMarks.Add(question);
                    //rysujemy znak zapytania
                }
                    
                    //c.Text += grid[i, j].getType().ToString();
                
                //c.Text += "\n";
            }

            if (isMiniMapBeingUsed)
            {
                foreach (Rectangle rectangle in miniMapRectangles)
                {
                    if (Canvas.GetLeft(rectangle) == (Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize))) && Canvas.GetTop(rectangle) == (Canvas.GetTop(c) + (currX * ((c.Height) / gridSize))))
                    {
                        rectangle.Opacity = 0.8;
                    }
                    else
                    {
                        rectangle.Opacity = 0.55;// w przyszłości może będzie tu zmienna żeby 2 razy nie pisać 0.55 bo się zepsuje
                    }
                    canvas.Children.Add(rectangle);
                }
                foreach (Label questionMark in miniMapQuestionMarks) canvas.Children.Add(questionMark);
            }

        }
        public void showMiniMap(GroupBox c)
        {
            if (isMiniMapBeingUsed) return;
            isMiniMapBeingUsed = true;
            foreach (Rectangle rectangle in miniMapRectangles)
            {
                if (Canvas.GetLeft(rectangle) == (Canvas.GetLeft(c) + (currY * ((c.Width) / gridSize))) && Canvas.GetTop(rectangle) == (Canvas.GetTop(c) + (currX * ((c.Height) / gridSize))))
                {
                    rectangle.Opacity = 0.8;
                }
                else
                {
                    rectangle.Opacity = 0.55;// w przyszłości może będzie tu zmienna żeby 2 razy nie pisać 0.55 bo się zepsuje
                }
                canvas.Children.Add(rectangle);
            }
            foreach (Label questionMark in miniMapQuestionMarks) canvas.Children.Add(questionMark);
        }
        public void miniMapClear()
        {
            for(int i=0; i<miniMapRectangles.Count; i++)
            {
                canvas.Children.Remove(miniMapRectangles[i]);
            }
            for (int i = 0; i < miniMapQuestionMarks.Count; i++)
            {
                canvas.Children.Remove(miniMapQuestionMarks[i]);
            }
            isMiniMapBeingUsed = false;
        }
        public int getX()
        {
            return currX;
        }
        public int getY()
        {
            return currY;
        }
        public void goTo(Canvas GameScreen, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
            currX = firstDoor / 10;
            currY = firstDoor % 10;
            //if (!grid[currX, currY].getVisited())
            //{
            //}
            
            grid[currX, currY].setVisited(true);
            grid[currX,currY].makeMap(GameScreen, ref leftDoorExist, ref rightDoorExist,ref upDoorExist, ref downDoorExist, doorDirection);
        }
        public List<Monster> rMon()
        {
            return grid[currX, currY].monArr();
        }
        public void goTo(int xAxis,int yAxis, Canvas GameScreen, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
            //if (!grid[currX, currY].getVisited())
            //{
            //}
            foreach (Monster x in grid[currX, currY].monArr())
            {
                x.remove();
            }
            if (xAxis==1 || xAxis == -1) { currX += xAxis; }
            else if(yAxis==1 || yAxis == -1) { currY += yAxis; }
            grid[currX, currY].setVisited(true);
            grid[currX, currY].makeMap(GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, doorDirection);
        }
       
        private void generateDoors(Canvas canv)
        {
            Random generateType=new Random();
            bool unlock = false;
            double diff=0.00;
            int firstRoomWeight=10;
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    
                    if (firstDoor == -1 && grid[i, j].getType() != 0)
                    {
                        unlock = true;
                        firstDoor = i * 10 + j;
                        grid[i,j]=new Pokoj(canv,2,"startingRoom", 0);
                        grid[i, j].endGame += endGame;
                        grid[i, j].setVisited(true);
                        bool left = false, right = false, down = false, up = false;
                        lastDoor = i * 10 + j;
                        if (i > 0) { if (grid[i - 1, j].getType() != 0) up = true; } // up
                        if (j > 0) { if (grid[i, j - 1].getType() != 0) left = true; } // left
                        if (i < gridSize - 1) { if (grid[i + 1, j].getType() != 0) down = true; } // down
                        if (j < gridSize - 1) { if (grid[i, j + 1].getType() != 0) right = true; ; } // right
                        grid[i, j].setDoors(up, left, down, right);

                    }
                    else if (grid[i, j].getType() != 0)
                    {
                        int typeOfRoom=generateType.Next(1,2);
                        grid[i,j]=new Pokoj(canv,typeOfRoom,"basicRoom", firstRoomWeight++);
                        grid[i, j].endGame += endGame;
                        bool left = false, right = false, down = false, up = false;
                        lastDoor = i * 10 + j;
                        if (i > 0) { if (grid[i - 1, j].getType() != 0) up = true; } // up
                        if (j > 0) { if (grid[i, j - 1].getType() != 0) left = true; } // left
                        if (i < gridSize - 1) { if (grid[i + 1, j].getType() != 0) down = true; } // down
                        if (j < gridSize - 1) { if (grid[i, j + 1].getType() != 0) right = true; ; } // right
                        grid[i, j].setDoors(up, left, down, right);
                    }
                    if(unlock)grid[i, j].setDiff(diff);
                }
                if(unlock)diff += 0.05;
            }
            
            grid[lastDoor/10,lastDoor%10].setType(2);
            grid[lastDoor/10,lastDoor%10].changeRoomUsage("bossRoom");
            grid[lastDoor / 10, lastDoor % 10].setDiff(0.20);
            
        }
       

        private bool CheckRoom(int x, int y)
        {
            if (grid[x, y].getType() != 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}



