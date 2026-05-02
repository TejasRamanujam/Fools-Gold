using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fool_s_Gold
{
    //Helpful class for working with rectangles
    public static class RectangleExtensions
    {
        //Calculates the signed depth of intersection between rectangles.
        //Returns the amount of overlap between 2 intersecting rectangles. 
        //These depth values can be negative depending on which side the 
        //rectangles intersect. This allows callers 
        //to determine the correct direction to push objects in order to resolve collisions. 
        //If the rectangles are not intersection, Vector2.Zero is returned.
        public static Vector2 GetIntersectionDepth(this Rectangle _rectA, Rectangle _rectB)
        {
            //Calculate half size.
            float halfWidthA = _rectA.Width / 2.0f;
            float halfHeightA = _rectA.Height / 2.0f;
            float halfWidthB = _rectB.Width / 2.0f;
            float halfHeightB = _rectB.Height / 2.0f;

            //Calculate centers
            Vector2 centerA = new Vector2(_rectA.Left + halfWidthA, _rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(_rectB.Left + halfWidthB, _rectB.Top + halfHeightB);

            //Calculate centers and minimum-non-intersecting distance between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            //If not intersecting at all, return (0,0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            //Calculate and return intersection depths.
            float depthX = 0.0f;
            float depthY = 0.0f;
            if (distanceX > 0)
            {
                depthX = minDistanceX - distanceX;
            }
            else
            {
                depthX = -minDistanceX - distanceX;
            }
            if (distanceY > 0)
            {
                depthY = minDistanceY - distanceY;
            }
            else
            {
                depthY = -minDistanceY - distanceY;
            }
            return new Vector2(depthX, depthY);
        }
    }

}
