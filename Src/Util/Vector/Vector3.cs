// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace Util.MyMath
{
    // Representation of 3D vectors and points.
    public partial struct Vector3
    {
        public const float kEpsilon = 0.00001F;
        private const float ZeroEpsilonSq = Mathf.Epsilon * Mathf.Epsilon;
        public static Vector3 InternalZero = zero;

        // X component of the vector.
        public float x;
        // Y component of the vector.
        public float y;
        // Z component of the vector.
        public float z;

        // Linearly interpolates between two vectors.
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
                );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
                );
        }

        // Moves a point /current/ in a straight line towards a /target/ point.
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 toVector = target - current;
            float dist = toVector.magnitude;
            if (dist <= maxDistanceDelta || dist < float.Epsilon)
                return target;
            return current + toVector / dist * maxDistanceDelta;
        }

        // Gradually changes a vector towards a desired goal over time.
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            Vector3 change = current - target;
            Vector3 originalTo = target;

            float maxChange = maxSpeed * smoothTime;
            change = Vector3.ClampMagnitude(change, maxChange);
            target = current - change;

            Vector3 temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            Vector3 output = target + (change + temp) * exp;

            if (Vector3.Dot(originalTo - current, output - originalTo) > 0)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        // Access the x, y, z components using [0], [1], [2] respectively.
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        // Creates a new vector with given x, y, z components.
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        // Creates a new vector with given x, y components and sets /z/ to zero.
        public Vector3(float x, float y) { this.x = x; this.y = y; z = 0F; }
        /// <summary>
        /// Constructor initializing a new instance of the structure
        /// </summary>
        /// <param name="xyz">All components of the vector are set to xyz</param>
        public Vector3(float xyz)
        {
            this.x = xyz;
            this.y = xyz;
            this.z = xyz;
        }
        static Vector3()
        {
            InternalZero = zero;
        }

        // Set x, y and z components of an existing Vector3.
        public void Set(float newX, float newY, float newZ) { x = newX; y = newY; z = newZ; }

        // Multiplies two vectors component-wise.
        public static Vector3 Scale(Vector3 a, Vector3 b) { return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z); }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vector3 scale) { x *= scale.x; y *= scale.y; z *= scale.z; }

        // used to allow Vector3s to be used as keys in hash tables
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }

        // also required for being able to use Vector3s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is Vector3)) return false;

            Vector3 rhs = (Vector3)other;
            return x.Equals(rhs.x) && y.Equals(rhs.y) && z.Equals(rhs.z);
        }

        // Reflects a vector off the plane defined by a normal.
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            return -2F * Dot(inNormal, inDirection) * inNormal + inDirection;
        }

        // *undoc* --- we have normalized property now

        // Makes this vector have a ::ref::magnitude of 1.
        public void Normalize()
        {
            float mag = Magnitude(this);
            if (mag > kEpsilon)
                this = this / mag;
            else
                this = zero;
        }

        // Returns this vector with a ::ref::magnitude of 1 (RO).
        public Vector3 normalized { get { return Vector3.Normalize(this); } }

        // Projects a vector onto another vector.
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < Mathf.Epsilon)
                return zero;
            else
                return onNormal * Dot(vector, onNormal) / sqrMag;
        }

        // Projects a vector onto a plane defined by a normal orthogonal to the plane.
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            return vector - Project(vector, planeNormal);
        }

        // Returns the angle in degrees between /from/ and /to/. This is always the smallest
        public static float Angle(Vector3 from, Vector3 to)
        {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1F, 1F)) * Mathf.Rad2Deg;
        }

        // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            Vector3 fromNorm = from.normalized, toNorm = to.normalized;
            float unsignedAngle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromNorm, toNorm), -1F, 1F)) * Mathf.Rad2Deg;
            float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.Cross(fromNorm, toNorm)));
            return unsignedAngle * sign;
        }

        // Returns the distance between /a/ and /b/.
        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 vec = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        }
        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
        }

        // Returns a copy of /vector/ with its magnitude clamped to /maxLength/.
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
                return vector.normalized * maxLength;
            return vector;
        }

        // *undoc* --- there's a property now
        public static float Magnitude(Vector3 vector) { return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        // Returns the length of this vector (RO).
        public float magnitude { get { return Mathf.Sqrt(x * x + y * y + z * z); } }

        // returns the x-z length of this vector.
        public float xzMagnitude { get { return Mathf.Sqrt(x * x + z * z); } }

        // *undoc* --- there's a property now
        public static float SqrMagnitude(Vector3 vector) { return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z; }

        // Returns the squared length of this vector (RO).
        public float sqrMagnitude { get { return x * x + y * y + z * z; } }

        static readonly Vector3 zeroVector = new Vector3(0F, 0F, 0F);
        static readonly Vector3 oneVector = new Vector3(1F, 1F, 1F);
        static readonly Vector3 upVector = new Vector3(0F, 1F, 0F);
        static readonly Vector3 downVector = new Vector3(0F, -1F, 0F);
        static readonly Vector3 leftVector = new Vector3(-1F, 0F, 0F);
        static readonly Vector3 rightVector = new Vector3(1F, 0F, 0F);
        static readonly Vector3 forwardVector = new Vector3(0F, 0F, 1F);
        static readonly Vector3 backVector = new Vector3(0F, 0F, -1F);
        static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        // Shorthand for writing @@Vector3(0, 0, 0)@@
        public static Vector3 zero { get { return zeroVector; } }
        // Shorthand for writing @@Vector3(1, 1, 1)@@
        public static Vector3 one { get { return oneVector; } }
        // Shorthand for writing @@Vector3(0, 0, 1)@@
        public static Vector3 forward { get { return forwardVector; } }
        public static Vector3 back { get { return backVector; } }
        // Shorthand for writing @@Vector3(0, 1, 0)@@
        public static Vector3 up { get { return upVector; } }
        public static Vector3 down { get { return downVector; } }
        public static Vector3 left { get { return leftVector; } }
        // Shorthand for writing @@Vector3(1, 0, 0)@@
        public static Vector3 right { get { return rightVector; } }
        // Shorthand for writing @@Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity)@@
        public static Vector3 positiveInfinity { get { return positiveInfinityVector; } }
        // Shorthand for writing @@Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity)@@
        public static Vector3 negativeInfinity { get { return negativeInfinityVector; } }

        // Adds two vectors.
        public static Vector3 operator+(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
        // Subtracts one vector from another.
        public static Vector3 operator-(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
        // Negates a vector.
        public static Vector3 operator-(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
        // Multiplies a vector by a number.
        public static Vector3 operator*(Vector3 a, float d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        public static float operator*(Vector3 a, Vector3 b) { return Dot(ref a, ref b); }
        // Multiplies a vector by a number.
        public static Vector3 operator*(float d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }
        // Divides a vector by a number.
        public static Vector3 operator/(Vector3 a, float d) { return new Vector3(a.x / d, a.y / d, a.z / d); }

        // Returns true if the vectors are equal.
        public static bool operator==(Vector3 lhs, Vector3 rhs)
        {
            // Returns false in the presence of NaN values.
            return SqrMagnitude(lhs - rhs) < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator!=(Vector3 lhs, Vector3 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return String.Format("({0:F1}, {1:F1}, {2:F1})", x, y, z);
        }

        public string ToString(string format)
        {
            return String.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));
        }

        [System.Obsolete("Use Vector3.forward instead.")]
        public static Vector3 fwd { get { return new Vector3(0F, 0F, 1F); } }
        [System.Obsolete("Use Vector3.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
        public static float AngleBetween(Vector3 from, Vector3 to) { return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1F, 1F)); }


        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <returns>The transformed vector.</returns>
        #region public static JVector Transform(JVector position, JMatrix matrix)
        public static Vector3 Transform(Vector3 position, Matrix3x3 matrix)
        {
            Vector3.Transform(ref position, ref matrix, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void Transform(ref Vector3 position, ref Matrix3x3 matrix, out Vector3 result)
        {
            float num0 = ((position.x * matrix.M11) + (position.y * matrix.M21)) + (position.z * matrix.M31);
            float num1 = ((position.x * matrix.M12) + (position.y * matrix.M22)) + (position.z * matrix.M32);
            float num2 = ((position.x * matrix.M13) + (position.y * matrix.M23)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }

        /// <summary>
        /// Transforms a vector by the transposed of the given Matrix.
        /// </summary>
        /// <param name="position">The vector to transform.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="result">The transformed vector.</param>
        public static void TransposedTransform(ref Vector3 position, ref Matrix3x3 matrix, out Vector3 result)
        {
            float num0 = ((position.x * matrix.M11) + (position.y * matrix.M12)) + (position.z * matrix.M13);
            float num1 = ((position.x * matrix.M21) + (position.y * matrix.M22)) + (position.z * matrix.M23);
            float num2 = ((position.x * matrix.M31) + (position.y * matrix.M32)) + (position.z * matrix.M33);

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion
        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        #region public static float Dot(Vector3 vector1, Vector3 vector2)
        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return Vector3.Dot(ref vector1, ref vector2);
        }


        /// <summary>
        /// Calculates the dot product of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>Returns the dot product of both vectors.</returns>
        public static float Dot(ref Vector3 vector1, ref Vector3 vector2)
        {
            return ((vector1.x * vector2.x) + (vector1.y * vector2.y)) + (vector1.z * vector2.z);
        }
        #endregion

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The sum of both vectors.</returns>
        #region public static void Add(JVector value1, JVector value2)
        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            Vector3.Add(ref value1, ref value2, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Adds to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The sum of both vectors.</param>
        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num0 = value1.x + value2.x;
            float num1 = value1.y + value2.y;
            float num2 = value1.z + value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The difference of both vectors.</returns>
        #region public static JVector Subtract(JVector value1, JVector value2)
        public static Vector3 Subtract(Vector3 value1, Vector3 value2)
        {
            Subtract(ref value1, ref value2, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Subtracts to vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="result">The difference of both vectors.</param>
        public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            float num0 = value1.x - value2.x;
            float num1 = value1.y - value2.y;
            float num2 = value1.z - value2.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of both vectors.</returns>
        #region public static JVector Cross(JVector vector1, JVector vector2)
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3.Cross(ref vector1, ref vector2, out Vector3 result);
            return result;
        }

        /// <summary>
        /// The cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of both vectors.</param>
        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            float num3 = (vector1.y * vector2.z) - (vector1.z * vector2.y);
            float num2 = (vector1.z * vector2.x) - (vector1.x * vector2.z);
            float num = (vector1.x * vector2.y) - (vector1.y * vector2.x);
            result.x = num3;
            result.y = num2;
            result.z = num;
        }
        #endregion
        public float LengthSquared()
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
        public bool IsNearlyZero()
        {
            return (this.LengthSquared() < ZeroEpsilonSq);
        }
        #region public static JVector Negate(JVector value)
        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <returns>The negated vector.</returns>
        public static Vector3 Negate(Vector3 value)
        {
            Negate(ref value, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Inverses the direction of a vector.
        /// </summary>
        /// <param name="value">The vector to inverse.</param>
        /// <param name="result">The negated vector.</param>
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            float num0 = -value.x;
            float num1 = -value.y;
            float num2 = -value.z;

            result.x = num0;
            result.y = num1;
            result.z = num2;
        }
        #endregion
        #region public static void Swap(ref Vector3 vector1, ref Vector3 vector2)

        /// <summary>
        /// Swaps the components of both vectors.
        /// </summary>
        /// <param name="vector1">The first vector to swap with the second.</param>
        /// <param name="vector2">The second vector to swap with the first.</param>
        public static void Swap(ref Vector3 vector1, ref Vector3 vector2)
        {
            float temp;

            temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;
        }
        #endregion
        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>Returns the multiplied vector.</returns>
        #region public static JVector Multiply(JVector value1, float scaleFactor)
        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            Vector3.Multiply(ref value1, scaleFactor, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Multiply a vector with a factor.
        /// </summary>
        /// <param name="value1">The vector to multiply.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">Returns the multiplied vector.</param>
        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.x = value1.x * scaleFactor;
            result.y = value1.y * scaleFactor;
            result.z = value1.z * scaleFactor;
        }
        #endregion
        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>Returns the cross product of both.</returns>
        #region public static JVector operator %(JVector value1, JVector value2)
        public static Vector3 operator %(Vector3 value1, Vector3 value2)
        {
            Vector3.Cross(ref value1, ref value2, out Vector3 result);
            return result;
        }
        #endregion
        /// <summary>
        /// Gets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>
        #region public static JVector Min(JVector value1, JVector value2)

        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            Vector3.Min(ref value1, ref value2, out Vector3 result);
            return result;
        }

        /// <summary>
        ///  ets a vector with the minimum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = (value1.x < value2.x) ? value1.x : value2.x;
            result.y = (value1.y < value2.y) ? value1.y : value2.y;
            result.z = (value1.z < value2.z) ? value1.z : value2.z;
        }
        #endregion

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>
        #region public static Vector3 Max(Vector3 value1, Vector3 value2)
        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            Vector3.Max(ref value1, ref value2, out Vector3 result);
            return result;
        }

        /// <summary>
        /// Gets a vector with the maximum x,y and z values of both vectors.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.x = (value1.x > value2.x) ? value1.x : value2.x;
            result.y = (value1.y > value2.y) ? value1.y : value2.y;
            result.z = (value1.z > value2.z) ? value1.z : value2.z;
        }
        #endregion
        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <returns>A normalized vector.</returns>
        #region public static JVector Normalize(JVector value)
        public static Vector3 Normalize(Vector3 value)
        {
            Vector3.Normalize(ref value, out Vector3 result);
            return result;
        }
        /// <summary>
        /// Normalizes the given vector.
        /// </summary>
        /// <param name="value">The vector which should be normalized.</param>
        /// <param name="result">A normalized vector.</param>
        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float num2 = ((value.x * value.x) + (value.y * value.y)) + (value.z * value.z);
            float num = 1f / ((float)Math.Sqrt((double)num2));
            result.x = value.x * num;
            result.y = value.y * num;
            result.z = value.z * num;
        }
        #endregion

        /// <summary>
        /// Sets the length of the vector to zero.
        /// </summary>
        #region public void MakeZero()
        public void MakeZero()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        #endregion
    }
}
