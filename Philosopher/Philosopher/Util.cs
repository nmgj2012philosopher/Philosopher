using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Philosopher
{
    class Util
    {
        public static bool SemiAutoKey(Keys key, KeyboardState prevKeys)
        {
            return Keyboard.GetState().IsKeyDown(key) &&
                !prevKeys.IsKeyDown(key);
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * y1 - y2));
        }

        public static bool CircleLineIntersect(Vector2 la, Vector2 lb, Vector2 cc, float cr)
        {
            double dx = lb.X - la.X;
            double dy = lb.Y - la.Y;
            double a = dx * dx + dy * dy;
            double b = 2 * (dx * (la.X - cc.X) + dy * (la.Y - cc.Y));
            double c = cc.X * cc.X + cc.Y * cc.Y;

            c += la.X * la.X + la.Y * la.Y;
            c -= 2 * (cc.X * la.X + cc.Y * la.Y);
            c -= cr * cr;

            double bb4ac = b * b - 4 * a * c;

            if (bb4ac < 0) return false;

            else
            {
                double mu = (-b + Math.Sqrt(bb4ac)) / 2 * a;
                double ix1 = la.X + mu * dx;
                double iy1 = la.Y + mu * dy;
                mu = (-b - Math.Sqrt(bb4ac)) / (2 * a);
                double ix2 = la.X + mu * dx;
                double iy2 = la.Y + mu * dy;

                double testX, testY;

                if (Distance(la.X, la.Y, cc.X, cc.Y) < Distance(lb.X, lb.Y, cc.X, cc.Y))
                {
                    testX = lb.X;
                    testY = lb.Y;
                }
                else
                {
                    testX = la.X;
                    testY = la.Y;
                }

                if (Distance(testX, testY, ix1, iy1) < Distance(la.X, la.Y, lb.X, lb.Y) ||
                    Distance(testX, testY, ix2, iy2) < Distance(la.X, la.Y, lb.X, lb.Y))
                    return true;
            }
            return false;
        }
    }
}
