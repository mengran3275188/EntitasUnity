// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace Util.MyMath
{
    public partial struct Quaternion
    {
        // X component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float x;
        // Y component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float y;
        // Z component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float z;
        // W component of the Quaternion. Don't modify this directly unless you know quaternions inside out.
        public float w;

        // Access the x, y, z, w components using [0], [1], [2], [3] respectively.
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        // Constructs new Quaternion with given x,y,z,w components.
        public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

        // Set x, y, z and w components of an existing Quaternion.
        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);

        // The identity rotation (RO). This quaternion corresponds to "no rotation": the object
        public static Quaternion identity
        {
            get
            {
                return identityQuaternion;
            }
        }

        // Combines rotations /lhs/ and /rhs/.
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        // Rotates the point /point/ with /rotation/.
        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            float x = rotation.x * 2F;
            float y = rotation.y * 2F;
            float z = rotation.z * 2F;
            float xx = rotation.x * x;
            float yy = rotation.y * y;
            float zz = rotation.z * z;
            float xy = rotation.x * y;
            float xz = rotation.x * z;
            float yz = rotation.y * z;
            float wx = rotation.w * x;
            float wy = rotation.w * y;
            float wz = rotation.w * z;

            Vector3 res;
            res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
            res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
            res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
            return res;
        }

        // *undocumented*
        public const float kEpsilon = 0.000001F;
        /// <summary>
        /// Sets the length of the quaternion to one.
        /// </summary>
        #region public void Normalize()
        public void Normalize()
        {
            float num2 = (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z)) + (this.w * this.w);
            float num = 1f / ((float)Math.Sqrt((double)num2));
            this.x *= num;
            this.y *= num;
            this.z *= num;
            this.w *= num;
        }
        #endregion

        // Are two quaternions equal to each other?
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            // Returns false in the presence of NaN values.
            return Dot(lhs, rhs) > 1.0f - kEpsilon;
        }

        // Are two quaternions different from each other?
        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // The dot product between two rotations.
        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        // Returns the angle in degrees between two rotations /a/ and /b/.
        public static float Angle(Quaternion a, Quaternion b)
        {
            float dot = Dot(a, b);
            return Mathf.Acos(Mathf.Min(Mathf.Abs(dot), 1.0F)) * 2.0F * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <returns>The product of both quaternions.</returns>
        #region public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        {
            Quaternion.Multiply(ref quaternion1, ref quaternion2, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Multiply two quaternions.
        /// </summary>
        /// <param name="quaternion1">The first quaternion.</param>
        /// <param name="quaternion2">The second quaternion.</param>
        /// <param name="result">The product of both quaternions.</param>
        public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            float x = quaternion1.x;
            float y = quaternion1.y;
            float z = quaternion1.z;
            float w = quaternion1.w;
            float num4 = quaternion2.x;
            float num3 = quaternion2.y;
            float num2 = quaternion2.z;
            float num = quaternion2.w;
            float num12 = (y * num2) - (z * num3);
            float num11 = (z * num4) - (x * num2);
            float num10 = (x * num3) - (y * num4);
            float num9 = ((x * num4) + (y * num3)) + (z * num2);
            result.x = ((x * num) + (num4 * w)) + num12;
            result.y = ((y * num) + (num3 * w)) + num11;
            result.z = ((z * num) + (num2 * w)) + num10;
            result.w = (w * num) - num9;
        }
        #endregion
        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <returns>JQuaternion representing an orientation.</returns>
        #region public static JQuaternion CreateFromMatrix(JMatrix matrix)
        public static Quaternion CreateFromMatrix(Matrix3x3 matrix)
        {
            Quaternion.CreateFromMatrix(ref matrix, out Quaternion result);
            return result;
        }

        /// <summary>
        /// Creates a quaternion from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix representing an orientation.</param>
        /// <param name="result">JQuaternion representing an orientation.</param>
        public static void CreateFromMatrix(ref Matrix3x3 matrix, out Quaternion result)
        {
            float num8 = (matrix.M11 + matrix.M22) + matrix.M33;
            if (num8 > 0f)
            {
                float num = (float)Math.Sqrt((double)(num8 + 1f));
                result.w = num * 0.5f;
                num = 0.5f / num;
                result.x = (matrix.M23 - matrix.M32) * num;
                result.y = (matrix.M31 - matrix.M13) * num;
                result.z = (matrix.M12 - matrix.M21) * num;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                float num7 = (float)Math.Sqrt((double)(((1f + matrix.M11) - matrix.M22) - matrix.M33));
                float num4 = 0.5f / num7;
                result.x = 0.5f * num7;
                result.y = (matrix.M12 + matrix.M21) * num4;
                result.z = (matrix.M13 + matrix.M31) * num4;
                result.w = (matrix.M23 - matrix.M32) * num4;
            }
            else if (matrix.M22 > matrix.M33)
            {
                float num6 = (float)Math.Sqrt((double)(((1f + matrix.M22) - matrix.M11) - matrix.M33));
                float num3 = 0.5f / num6;
                result.x = (matrix.M21 + matrix.M12) * num3;
                result.y = 0.5f * num6;
                result.z = (matrix.M32 + matrix.M23) * num3;
                result.w = (matrix.M31 - matrix.M13) * num3;
            }
            else
            {
                float num5 = (float)Math.Sqrt((double)(((1f + matrix.M33) - matrix.M11) - matrix.M22));
                float num2 = 0.5f / num5;
                result.x = (matrix.M31 + matrix.M13) * num2;
                result.y = (matrix.M32 + matrix.M23) * num2;
                result.z = 0.5f * num5;
                result.w = (matrix.M12 - matrix.M21) * num2;
            }
        }
        #endregion
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            float num = roll * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float num3 = (float)Math.Cos((double)num);
            float num4 = pitch * 0.5f;
            float num5 = (float)Math.Sin((double)num4);
            float num6 = (float)Math.Cos((double)num4);
            float num7 = yaw * 0.5f;
            float num8 = (float)Math.Sin((double)num7);
            float num9 = (float)Math.Cos((double)num7);
            Quaternion result = default(Quaternion);
            result.x = num9 * num5 * num3 + num8 * num6 * num2;
            result.y = num8 * num6 * num3 - num9 * num5 * num2;
            result.z = num9 * num6 * num2 - num8 * num5 * num3;
            result.w = num9 * num6 * num3 + num8 * num5 * num2;
            return result;
        }
        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Quaternion quaternion)
        {
            float num = roll * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float num3 = (float)Math.Cos((double)num);
            float num4 = pitch * 0.5f;
            float num5 = (float)Math.Sin((double)num4);
            float num6 = (float)Math.Cos((double)num4);
            float num7 = yaw * 0.5f;
            float num8 = (float)Math.Sin((double)num7);
            float num9 = (float)Math.Cos((double)num7);
            quaternion.x = num9 * num5 * num3 + num8 * num6 * num2;
            quaternion.y = num8 * num6 * num3 - num9 * num5 * num2;
            quaternion.z = num9 * num6 * num2 - num8 * num5 * num3;
            quaternion.w = num9 * num6 * num3 + num8 * num5 * num2;
        }
        // Makes euler angles positive 0/360 with 0.0001 hacked to support old behaviour of QuaternionToEuler
        private static Vector3 Internal_MakePositive(Vector3 euler)
        {
            float negativeFlip = -0.0001f * Mathf.Rad2Deg;
            float positiveFlip = 360.0f + negativeFlip;

            if (euler.x < negativeFlip)
                euler.x += 360.0f;
            else if (euler.x > positiveFlip)
                euler.x -= 360.0f;

            if (euler.y < negativeFlip)
                euler.y += 360.0f;
            else if (euler.y > positiveFlip)
                euler.y -= 360.0f;

            if (euler.z < negativeFlip)
                euler.z += 360.0f;
            else if (euler.z > positiveFlip)
                euler.z -= 360.0f;

            return euler;
        }

        // used to allow Quaternions to be used as keys in hash tables
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        // also required for being able to use Quaternions as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is Quaternion)) return false;

            Quaternion rhs = (Quaternion)other;
            return x.Equals(rhs.x) && y.Equals(rhs.y) && z.Equals(rhs.z) && w.Equals(rhs.w);
        }

        public override string ToString()
        {
            return String.Format("({0:F1}, {1:F1}, {2:F1}, {3:F1})", x, y, z, w);
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2}, {3})", x.ToString(format), y.ToString(format), z.ToString(format), w.ToString(format));
        }
    }
} //namespace
