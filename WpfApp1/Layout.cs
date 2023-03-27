using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace BasicsOfGame
{

    internal class Pokoj
    {
        int type, objectCount, enemyCount;
        Random rnd = new Random();
        bool left = false, up = false, right = false, down = false;
        MapsObjects [] roomContent;

        public Pokoj(int t)
        {
            type = t;                       //type determines background image for that room
            objectCount = rnd.Next(0, 4);   //from 0 up to 3 objects
            enemyCount = rnd.Next(2, 6);    //from 2 up to 5 enemies
            
            //below we generate an array of objects and then to the array we randomize the exact object
            int temp;
            roomContent = new MapsObjects[objectCount];
            for (int i = 0; i < objectCount; ++i)
            {
                temp = rnd.Next(0, 3);
                roomContent[i] = temp;
            }
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
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/leftDoor.png", UriKind.Absolute));
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
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/rightDoor.png", UriKind.Absolute));
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
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/upDoor.png", UriKind.Absolute));
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
                temporaryHolder.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/BasicsOfGame;component/images/doors/downDoor.png", UriKind.Absolute));
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


            int newCoord;
            int[] restrictedX = new int[objectCount] { 0 };
            int[] restrictedY = new int[objectCount] { 0 };
            bool pass;
            foreach (MapsObjects obj in roomContent)
            {
                pass = true;
                do
                {
                    newCoord = rnd.Next(100, 1151);
                    for (int i = 0; i < objectCount; ++i)
                    {
                        if (newCoord < restrictedX[i] + 50 || newCoord > restrictedX[i] - 50)
                        {
                            pass = false;
                        }
                    }
                } while (!pass);
                Canvas.SetLeft(obj, newCoord);
                pass = true;
                do
                {
                    newCoord = rnd.Next(100, 495);
                    for (int i = 0; i < objectCount; ++i)
                    {
                        if (obj < restrictedY[i] + 50 || obj > restrictedY[i] - 50)
                        {
                            pass = false;
                        }
                    }
                } while (!pass);
                Canvas.SetTop(obj, newCoord);
            }
        }
    }

    internal class Grid
    {
        public Pokoj[,] grid;
        int roomCount = 15;
        static int gridSize = 9;
        int currX, currY;
        static int gridMid = gridSize / 2;
        Random rnd = new Random();
        int direction;
        int firstDoor = -1, lastDoor;
        public Grid()
        {
            grid = new Pokoj[gridSize, gridSize]; //if 0 then no room and 1,2,3,etc. mean different types of rooms
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    grid[i, j] = new Pokoj(0);
                }
            }
            grid[gridMid, gridMid].setType(1);
            roomCount--;
            currX = gridMid;
            currY = gridMid;
            int type;
            while (roomCount > 0)
            {
                direction = rnd.Next(0, 4);
                type = rnd.Next(1, 2);
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
                            if (!CheckRoom(currX, currY - 1))//check) to up
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
            generateDoors();
        }
        public void ShowMap(TextBox c)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    c.Text += grid[i, j].getType().ToString();
                }
                c.Text += "\n";
            }
            
        }
        public void goTo(Canvas GameScreen, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
            currX = firstDoor / 10;
            currY = firstDoor % 10;
            grid[currX,currY].makeMap(GameScreen, ref leftDoorExist, ref rightDoorExist,ref upDoorExist, ref downDoorExist, doorDirection);
        }
        public void goTo(int xAxis,int yAxis, Canvas GameScreen, ref bool leftDoorExist, ref bool rightDoorExist, ref bool upDoorExist, ref bool downDoorExist, int doorDirection)
        {
            if(xAxis==1 || xAxis == -1) { currX += xAxis; }
            else if(yAxis==1 || yAxis == -1) { currY += yAxis; }
            grid[currX, currY].makeMap(GameScreen, ref leftDoorExist, ref rightDoorExist, ref upDoorExist, ref downDoorExist, doorDirection);
        }
        private void generateDoors()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (firstDoor == -1 && grid[i, j].getType() != 0)
                    {
                        firstDoor = i * 10 + j;
                        grid[i, j].setType(2);

                    }
                    if (grid[i, j].getType() != 0)
                    {
                        bool left = false, right = false, down = false, up = false;
                        lastDoor = i * 10 + j;
                        if (i > 0) { if (grid[i - 1, j].getType() != 0) up = true; } // up
                        if (j > 0) { if (grid[i, j - 1].getType() != 0) left = true; } // left
                        if (i < gridSize - 1) { if (grid[i + 1, j].getType() != 0) down = true; } // down
                        if (j < gridSize - 1) { if (grid[i, j + 1].getType() != 0) right = true; ; } // right
                        grid[i, j].setDoors(up, left, down, right);
                    }

                }
            }
            grid[lastDoor/10,lastDoor%10].setType(2);
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


// 0 0 0 0 2 0 0 0 0
// 0 0 0 0 1 0 0 0 0
// 1 0 0 0 1 0 0 0 0
// 2 0 0 0 1 0 0 2 0
// 2 1 1 2 1 2 1 1 0
// 0 0 0 0 1 0 0 1 0
// 0 0 0 2 1 0 0 0 0
// 0 0 0 1 0 0 0 0 0
// 0 0 0 0 0 0 0 0 0

//tablica z gridem na którym będą pomieszczenia 9x9
//np lista
//random(2)+5+level*2.6

//pokój startowy
//  badamy sąsiadów
//  jeżeli już jest pokój to spadówa
//  jeżeli sąsiad ma więcej niż 1 sąsiada zajętego to spadówa
//  jeżeli mamy wystarczająco pokojów to spadówa
//  w innym wypadku zaznaczamy sąsiada jako pokój i dodajemy go do kolejki

