using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BasicsOfGame
{
    internal class Goblin
    {
        Rectangle body = new Rectangle();
        int attackRange = 100;
        int Speed = 130;
       
        public Goblin(Canvas a, int x, int y)
        {
            body.Height = 70;
            body.Width = 40;
            body.Fill = Brushes.Blue;
            body.Tag = "enemy";
            
            
            a.Children.Add(body);

            body.SetValue(Canvas.TopProperty, (double)x);
            body.SetValue(Canvas.LeftProperty, (double)y);


        }
        public void moveToTarget(Rectangle name, double delta,double friction,TextBox w)
        {
            if (delta > 1) return;
            double x=0, y=0;
            Point p = new Point(Canvas.GetLeft(name)+(name.Width/2),Canvas.GetTop(name)+(name.Height/2));
            
            w.Text= Canvas.GetLeft(body).ToString()+" "+Canvas.GetTop(body).ToString();
            if (p.X >Canvas.GetLeft(body)+body.Width+attackRange/2)
            {
                x = Speed * delta* friction;
            }
            if (p.X < Canvas.GetLeft(body)-attackRange/2)
            {
                x = -Speed * delta* friction;
            }
            if (p.Y < Canvas.GetTop(body)+body.Height/2)
            {
                y = -Speed * delta* friction;
            }
            if (p.Y > Canvas.GetTop(body) + body.Height / 2)
            {
                y = Speed * delta* friction;
            }
          
            Canvas.SetLeft(body, Canvas.GetLeft(body)+x);
            Canvas.SetTop(body, Canvas.GetTop(body)+y);
        }


       
    }
}


