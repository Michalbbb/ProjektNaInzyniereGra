using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicsOfGame
{
    class MapGeneration
    {
        //tablica z gridem na którym będą pomieszczenia 10x10
        //np lista
        //random(2)+5+level*2.6

        //pokój startowy
        //  badamy sąsiadów
        //  jeżeli już jest pokój to spadówa
        //  jeżeli sąsiad ma więcej niż 1 sąsiada zajętego to spadówa
        //  jeżeli mamy wystarczająco pokojów to spadówa
        //  w innym wypadku zaznaczamy sąsiada jako pokój i dodajemy go do kolejki
        Pokoj[,] grid = new Pokoj[10, 10];
    }

    internal class Pokoj
    {
        int x, y;
        Object[] obiekty;
        //Doors[] drzwi;
        //jak gracz wejdzie w górne drzwi -10 jak w dolne +10, jak w lewe to -1 a jak w prawe to +1
    }

}

