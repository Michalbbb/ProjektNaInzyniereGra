using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace BasicsOfGame
{
    internal class Layout
    {
        public Layout() { 
        
        }
        public void makeBackground(Canvas GameScreen, bool leftDoor, bool rightDoor, bool upDoor, bool downDoor, int tlo,ref bool leftDoorExist,ref bool rightDoorExist,ref bool upDoorExist,ref bool downDoorExist, int doorDirection)
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
                if((string)x.Name == "Player")
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

            for (int i=j; i>-1; --i)
            {
                GameScreen.Children.Remove(toRemove[i]);    //deleting objects when changing rooms
                toRemove.RemoveAt(i);
            }
        }
    }

    internal class Pokoj
    {
        int type, objectCount, enemyCount;
        Random rnd = new Random();
        bool left=false, up=false, right=false, down=false, seal=false;

        public Pokoj(int t)
        {
            type = t;                       //type determines background image for that room
            objectCount = rnd.Next(0, 4);   //from 0 up to 3 objects
            enemyCount = rnd.Next(2, 6);    //from 2 up to 5 enemies
        }

        public void setType(int t)
        {
            type = t;
        }

        public int getType()
        {
            return type;
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
        Grid()
        {
            grid = new Pokoj[gridSize, gridSize] ; //if 0 then no room and 1,2,3,etc. mean different types of rooms
            for(int i=0; i<gridSize; i++)
            {
                for(int j=0; j<gridSize; ++i)
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
                                grid[currX, currY + 1].setType(type);
                                roomCount--;
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
                                grid[currX, currY + 1].setType(type);
                                roomCount--;
                            }
                            else
                                currY -= 1;
                        }
                        break;
                    case 2:
                        if (currX < gridSize)
                        {
                            if (!CheckRoom(currX + 1, currY))//check to right
                            {
                                grid[currX, currY + 1].setType(type);
                                roomCount--;
                            }
                            else
                                currX += 1;
                        }
                        break;
                    case 3:
                        if (currY < gridSize)
                        {
                            if (!CheckRoom(currX, currY + 1))//check to down
                            {
                                grid[currX, currY + 1].setType(type);
                                roomCount--;
                            }
                            else
                                currY += 1;
                        }
                        break;
                }
            }
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

