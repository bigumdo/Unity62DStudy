using UnityEngine;

namespace YUI.Bullets {
    public class BezierBullet : Bullet {
        protected virtual Vector3 FourPointBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) {
            float u = 1 - t;
            return u * u * u * a
                + 3 * u * u * t * b
                + 3 * u * t * t * c
                + t * t * t * d;
        }

        protected virtual float EstimateBezierLength(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int resolution = 20) {
            float length = 0f;
            Vector3 prev = a;
            for (int i = 1; i <= resolution; i++) {
                float t = i / (float)resolution;
                Vector3 point = FourPointBezier(a, b, c, d, t);
                length += Vector3.Distance(prev, point);
                prev = point;
            }
            return length;
        }

        protected virtual  Vector3 GetRandomControlPoint(Vector3 origin, float radiusX, float radiusZ) {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float x = radiusX * Mathf.Cos(angle) + origin.x;
            float z = radiusZ * Mathf.Sin(angle) + origin.z;
            return new Vector3(x, z, origin.y);
        }
    }
}
