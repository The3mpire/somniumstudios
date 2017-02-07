using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Somnium
{

    /// <summary>
    /// Class containing static methods to assist with movement and movement direction calculations.
    /// </summary>
    public static class MovementUtils
    {

        /// <summary>
        /// Returns a random normalized direction.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomDirection()
        {
            return new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0).normalized;
        }

        /// <summary>
        /// Returns a Vector3 that represents a position vector in the specified direction 
        /// with the specified distance away from the start position vector.
        /// If the new position is through or on a wall, returns the position of the collision point adjusted by the buffer amount.
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="direction">The direction vector</param>
        /// <param name="buffer">Buffer for object collision point</param>
        /// <param name="distance">Distance the new position should be if possible</param>
        /// <returns></returns>
        public static Vector3 GetNewPositionInDirection(Vector3 start, Vector3 direction, float buffer, float distance, LayerMask raycastLayers)
        {
            Vector3 pos;
            direction = direction.normalized;
            RaycastHit2D hit = Physics2D.Raycast(start, direction, distance, raycastLayers);
            if (hit)
            {
                pos = (new Vector3(hit.point.x, hit.point.y, 0f) + (-direction * buffer));
            }
            else
            {
                pos = (start + (direction * distance) + (-direction * buffer));
            }

            return pos;
        }

        /// <summary>
        /// Returns a queue of random position vectors in the specified amount.
        /// Also takes in a min and max distance for the new positions, a collision buffer, and whether or not to chain the positions together.
        /// </summary>
        /// <param name="start">The start position</param>
        /// <param name="amount">Amount of new positions to get</param>
        /// <param name="minDistance">Minimum distance to get a new position for</param>
        /// <param name="maxDistance">Maximum distance to get a new position for</param>
        /// <param name="buffer">Collision buffer</param>
        /// <param name="chain">True if positions should be in succession of eachother (new position uses last position as the origin), \
        /// false if all new positions should use the start position as the origin</param>
        /// <returns></returns>
        public static Queue<Vector3> GetRandomPositions(Vector3 start, int amount, int minDistance, int maxDistance, int buffer, bool chain, LayerMask raycastLayers)
        {
            Queue<Vector3> q = new Queue<Vector3>();
            List<Vector3> dirs = new List<Vector3>();

            Vector3 currentPos = start;

            for (int i = 0; i < amount; i++)
            {
                dirs = dirs.Count == 0 ? GetAllDirections() : dirs;
                int index = Random.Range(0, dirs.Count);
                Vector3 dir = dirs[index];
                dirs.RemoveAt(index);
                Vector3 newPos = GetNewPositionInDirection(currentPos, dir, buffer, Random.Range(minDistance, maxDistance), raycastLayers);
                q.Enqueue(newPos);
                currentPos = chain ? newPos : currentPos;
            }

            return q;
        }

        /// <summary>
        /// Returns a list of vectors representing the 8 travel directions possible.
        /// </summary>
        /// <returns></returns>
        private static List<Vector3> GetAllDirections()
        {
            List<Vector3> list = new List<Vector3>();
            Vector3[] directionVectorArray = new Vector3[] { Vector3.up, new Vector3(1, 1), Vector3.right, new Vector3(1, -1), Vector3.down, new Vector3(-1, -1), Vector3.left, new Vector3(-1, 1) };
            list.AddRange(directionVectorArray);
            return list;
        }

        /// <summary>
        /// Calculates and returns a collision avoidance vector to be added to the objects normal vector.
        /// That resultant vector is the direction the moving character should travel in order to avoid a collision with
        /// the object impeding the moving character's path on the given avoid layer.
        /// </summary>
        /// <param name="origin">The position of the moving character.</param>
        /// <param name="direction">The direction the moving character is travelling in. 
        /// This parameter is normalized for calculation inside this method.</param>
        /// <param name="distance">The minimum avoidance distance.
        /// When the distance between the front of the moving character and the collider reaches this distance,
        /// collision avoidance will execute.</param>
        /// <param name="avoid">What layers contain objects that should be avoided.</param>
        /// <returns></returns>
        private static Vector3 AvoidCollision(Vector3 origin, Vector3 direction, float distance, LayerMask avoid)
        {
            //Normalize direction.
            direction = direction.normalized;

            //Raycast and draw the lookAhead.
            RaycastHit2D lookAheadHit = Physics2D.BoxCast(origin, new Vector2(1, 1), 0f, direction, distance, avoid);
            Debug.DrawLine(origin, origin + direction * distance, Color.red);

            //Raycast and draw the half avoidance distance vector;
            RaycastHit2D minAvoidanceHit = Physics2D.Raycast(origin, direction, distance, avoid);
            Debug.DrawLine(origin, origin + direction * (distance * .5f), Color.blue);

            //If either raycast has a hit, calculate avoidance vector.
            if (lookAheadHit || minAvoidanceHit)
            {
                //Get the raycast hit that isn't null, prioritizing the lookahead hit.
                RaycastHit2D hit = lookAheadHit ? lookAheadHit : minAvoidanceHit;

                //Get the position of the collider being hit.
                Vector3 objCenter = ConvertVector2(hit.collider.transform.position);

                Vector3 endPoint = hit.point;

                //Calculate the avoidance direction.
                Vector3 pushDirection = (endPoint - objCenter).normalized;
                //Vector3 pushDirection = (origin - endPoint).normalized;

                //Draw the avoidance direction relative to the moving object
                Debug.DrawLine(origin, origin + pushDirection, Color.green);

                //Draw the avoidance direction relative to the object being avoided.
                Debug.DrawLine(hit.collider.gameObject.transform.position, hit.collider.gameObject.transform.position + pushDirection, Color.blue);

                Debug.DrawLine(origin, origin + (pushDirection + direction));

                return pushDirection;
            }
            return direction;
        }

        /// <summary>
        /// Collision avoidance algorithm that performs post-calculation check on the collision avoidance direction.
        /// If the resultant of the avoidance direction and input direction still hits a collider, sets direction to the avoidDirection
        /// otherwise, uses the resultant vector.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="avoid"></param>
        public static void AvoidCollision(Vector3 origin, ref Vector3 direction, float distance, LayerMask avoid)
        {
            Vector3 avoidDirection = AvoidCollision(origin, direction, distance, avoid);
            RaycastHit2D check = Physics2D.BoxCast(origin, new Vector2(1, 1), 0f, direction + avoidDirection, distance, avoid);
            RaycastHit2D doubleCheck = Physics2D.BoxCast(origin, new Vector2(1, 1), 0f, avoidDirection, distance, avoid);
            if (check)
            {
                if (doubleCheck)
                {
                    direction = origin - ConvertVector2(doubleCheck.point);
                }
                else
                {
                    direction = avoidDirection;
                }
            }
            else
            {
                direction += avoidDirection;
            }
        }

        /// <summary>
        /// Converts a vector2 to a vector3 by zero-filling the z component.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private static Vector3 ConvertVector2(Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        /// <summary>
        /// Avoids an obstacle by rotating around it.
        /// Similar to Collision avoidance, but instead of calculating a finite escape vector, 
        /// this method will always return a vector perpendicular to the given direction.
        /// </summary>
        public static Vector3 AvoidObstacle(Vector3 origin, Vector3 direction, float distance, LayerMask onLayer)
        {
            RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(1, 1), 0f, direction, distance, onLayer);
            if (hit)
            {
                Vector3 check = Quaternion.Euler(0, 0, -45) * direction;
                RaycastHit2D second = Physics2D.BoxCast(origin, new Vector2(1, 1), 0f, check, distance, onLayer);

                Debug.DrawLine(origin, check.normalized * distance, Color.green);
                Debug.DrawLine(origin, direction.normalized * distance);
                Debug.DrawLine(origin, (Quaternion.Euler(0, 0, 45) * direction).normalized * distance);

                if (!second)
                {
                    return PerpendicularCounterClockwise(direction.normalized);
                }
                return PerpendicularClockwise(direction.normalized);
            }
            else
            {
                return direction;
            }
        }

        /// <summary>
        /// Returns a vector that is perpendicular in a clockwise direction to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private static Vector3 PerpendicularClockwise(Vector3 vector)
        {
            return new Vector3(-vector.y, vector.x);
        }

        /// <summary>
        /// Returns a vector that is perpendicular in a counter-clockwise direction to the given vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private static Vector3 PerpendicularCounterClockwise(Vector3 vector)
        {
            return new Vector3(vector.y, -vector.x);
        }
    }

}