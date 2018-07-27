using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util.MyMath
{
    public struct Matrix3x3
    {
        public float M11; // 1st row vector
        public float M12;
        public float M13;
        public float M21; // 2nd row vector
        public float M22;
        public float M23;
        public float M31; // 3rd row vector
        public float M32;
        public float M33;

        public static Matrix3x3 InternalIdentity;

        /// <summary>
        /// Identity matrix.
        /// </summary>
        public static readonly Matrix3x3 Identity;
        public static readonly Matrix3x3 Zero;

        static Matrix3x3()
        {
            Zero = new Matrix3x3();

            Identity = new Matrix3x3();
            Identity.M11 = 1.0f;
            Identity.M22 = 1.0f;
            Identity.M33 = 1.0f;

            InternalIdentity = Identity;
        }

        public static Matrix3x3 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Matrix3x3 matrix;
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            CreateFromQuaternion(ref quaternion, out matrix);
            return matrix;
        }

        public static Matrix3x3 CreateRotationX(float radians)
        {
            Matrix3x3 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = num2;
            matrix.M23 = num;
            matrix.M31 = 0f;
            matrix.M32 = -num;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationX(float radians, out Matrix3x3 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M11 = 1f;
            result.M12 = 0f;
            result.M13 = 0f;
            result.M21 = 0f;
            result.M22 = num2;
            result.M23 = num;
            result.M31 = 0f;
            result.M32 = -num;
            result.M33 = num2;
        }

        public static Matrix3x3 CreateRotationY(float radians)
        {
            Matrix3x3 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = num2;
            matrix.M12 = 0f;
            matrix.M13 = -num;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = 0f;
            matrix.M31 = num;
            matrix.M32 = 0f;
            matrix.M33 = num2;
            return matrix;
        }

        public static void CreateRotationY(float radians, out Matrix3x3 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M11 = num2;
            result.M12 = 0f;
            result.M13 = -num;
            result.M21 = 0f;
            result.M22 = 1f;
            result.M23 = 0f;
            result.M31 = num;
            result.M32 = 0f;
            result.M33 = num2;
        }

        public static Matrix3x3 CreateRotationZ(float radians)
        {
            Matrix3x3 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = 0f;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
            return matrix;
        }


        public static void CreateRotationZ(float radians, out Matrix3x3 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M11 = num2;
            result.M12 = num;
            result.M13 = 0f;
            result.M21 = -num;
            result.M22 = num2;
            result.M23 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 1f;
        }


        /// <summary>
        /// Initializes a new instance of the matrix structure.
        /// </summary>
        /// <param name="m11">m11</param>
        /// <param name="m12">m12</param>
        /// <param name="m13">m13</param>
        /// <param name="m21">m21</param>
        /// <param name="m22">m22</param>
        /// <param name="m23">m23</param>
        /// <param name="m31">m31</param>
        /// <param name="m32">m32</param>
        /// <param name="m33">m33</param>
        #region public JMatrix(float m11, float m12, float m13, float m21, float m22, float m23,float m31, float m32, float m33)
        public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23,float m31, float m32, float m33)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }
        #endregion

        /// <summary>
        /// Gets the determinant of the matrix.
        /// </summary>
        /// <returns>The determinant of the matrix.</returns>
        #region public float Determinant()
        //public float Determinant()
        //{
        //    return M11 * M22 * M33 -M11 * M23 * M32 -M12 * M21 * M33 +M12 * M23 * M31 + M13 * M21 * M32 - M13 * M22 * M31;
        //}
        #endregion

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The product of both matrices.</returns>
        #region public static JMatrix Multiply(JMatrix matrix1, JMatrix matrix2)
        public static Matrix3x3 Multiply(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            Matrix3x3 result;
            Matrix3x3.Multiply(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The product of both matrices.</param>
        public static void Multiply(ref Matrix3x3 matrix1, ref Matrix3x3 matrix2, out Matrix3x3 result)
        {
            float num0 = ((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31);
            float num1 = ((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32);
            float num2 = ((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33);
            float num3 = ((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31);
            float num4 = ((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32);
            float num5 = ((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33);
            float num6 = ((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31);
            float num7 = ((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32);
            float num8 = ((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33);

            result.M11 = num0;
            result.M12 = num1;
            result.M13 = num2;
            result.M21 = num3;
            result.M22 = num4;
            result.M23 = num5;
            result.M31 = num6;
            result.M32 = num7;
            result.M33 = num8;
        }
        #endregion

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The sum of both matrices.</returns>
        #region public static JMatrix Add(JMatrix matrix1, JMatrix matrix2)
        public static Matrix3x3 Add(Matrix3x3 matrix1, Matrix3x3 matrix2)
        {
            Matrix3x3 result;
            Matrix3x3.Add(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The sum of both matrices.</param>
        public static void Add(ref Matrix3x3 matrix1, ref Matrix3x3 matrix2, out Matrix3x3 result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }
        #endregion

        /// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverted JMatrix.</returns>
        #region public static JMatrix Inverse(JMatrix matrix)
        public static Matrix3x3 Inverse(Matrix3x3 matrix)
        {
            Matrix3x3 result;
            Matrix3x3.Inverse(ref matrix, out result);
            return result;
        }

        public float Determinant()
        {
            return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
                   M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
        }

        public static void Invert(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            float determinantInverse = 1 / matrix.Determinant();
            float m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
            float m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
            float m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

            float m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
            float m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
            float m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

            float m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
            float m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
            float m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;

            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;

            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
        }

        /// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <param name="result">The inverted JMatrix.</param>
        public static void Inverse(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            float det = matrix.M11 * matrix.M22 * matrix.M33 -
                matrix.M11 * matrix.M23 * matrix.M32 -
                matrix.M12 * matrix.M21 * matrix.M33 +
                matrix.M12 * matrix.M23 * matrix.M31 +
                matrix.M13 * matrix.M21 * matrix.M32 -
                matrix.M13 * matrix.M22 * matrix.M31;

            float num11 = matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32;
            float num12 = matrix.M13 * matrix.M32 - matrix.M12 * matrix.M33;
            float num13 = matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13;

            float num21 = matrix.M23 * matrix.M31 - matrix.M33 * matrix.M21;
            float num22 = matrix.M11 * matrix.M33 - matrix.M31 * matrix.M13;
            float num23 = matrix.M13 * matrix.M21 - matrix.M23 * matrix.M11;

            float num31 = matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22;
            float num32 = matrix.M12 * matrix.M31 - matrix.M32 * matrix.M11;
            float num33 = matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12;

            result.M11 = num11 / det;
            result.M12 = num12 / det;
            result.M13 = num13 / det;
            result.M21 = num21 / det;
            result.M22 = num22 / det;
            result.M23 = num23 / det;
            result.M31 = num31 / det;
            result.M32 = num32 / det;
            result.M33 = num33 / det;
        }
        #endregion

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>A JMatrix multiplied by the scale factor.</returns>
        #region public static JMatrix Multiply(JMatrix matrix1, float scaleFactor)
        public static Matrix3x3 Multiply(Matrix3x3 matrix1, float scaleFactor)
        {
            Matrix3x3 result;
            Matrix3x3.Multiply(ref matrix1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">A JMatrix multiplied by the scale factor.</param>
        public static void Multiply(ref Matrix3x3 matrix1, float scaleFactor, out Matrix3x3 result)
        {
            float num = scaleFactor;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
        }
        #endregion

        /// <summary>
        /// Creates a JMatrix representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <returns>JMatrix representing an orientation.</returns>
        #region public static JMatrix CreateFromQuaternion(JQuaternion quaternion)

        public static Matrix3x3 CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix3x3 result;
            Matrix3x3.CreateFromQuaternion(ref quaternion,out result);
            return result;
        }

        /// <summary>
        /// Creates a JMatrix representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <param name="result">JMatrix representing an orientation.</param>
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix3x3 result)
        {
            float num9 = quaternion.x * quaternion.x;
            float num8 = quaternion.y * quaternion.y;
            float num7 = quaternion.z * quaternion.z;
            float num6 = quaternion.x * quaternion.y;
            float num5 = quaternion.z * quaternion.w;
            float num4 = quaternion.z * quaternion.x;
            float num3 = quaternion.y * quaternion.w;
            float num2 = quaternion.y * quaternion.z;
            float num = quaternion.x * quaternion.w;
            result.M11 = 1f - (2f * (num8 + num7));
            result.M12 = 2f * (num6 + num5);
            result.M13 = 2f * (num4 - num3);
            result.M21 = 2f * (num6 - num5);
            result.M22 = 1f - (2f * (num7 + num9));
            result.M23 = 2f * (num2 + num);
            result.M31 = 2f * (num4 + num3);
            result.M32 = 2f * (num2 - num);
            result.M33 = 1f - (2f * (num8 + num9));
        }
        #endregion

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <returns>The transposed JMatrix.</returns>
        #region public static JMatrix Transpose(JMatrix matrix)
        public static Matrix3x3 Transpose(Matrix3x3 matrix)
        {
            Matrix3x3 result;
            Matrix3x3.Transpose(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <param name="result">The transposed JMatrix.</param>
        public static void Transpose(ref Matrix3x3 matrix, out Matrix3x3 result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }
        #endregion

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The product of both values.</returns>
        #region public static JMatrix operator *(JMatrix value1,JMatrix value2)
        public static Matrix3x3 operator *(Matrix3x3 value1,Matrix3x3 value2)
        {
            Matrix3x3 result; Matrix3x3.Multiply(ref value1, ref value2, out result);
            return result;
        }
        #endregion


        public float Trace()
        {
            return this.M11 + this.M22 + this.M33;
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The sum of both values.</returns>
        #region public static JMatrix operator +(JMatrix value1, JMatrix value2)
        public static Matrix3x3 operator +(Matrix3x3 value1, Matrix3x3 value2)
        {
            Matrix3x3 result; Matrix3x3.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The difference of both values.</returns>
        #region public static JMatrix operator -(JMatrix value1, JMatrix value2)
        public static Matrix3x3 operator -(Matrix3x3 value1, Matrix3x3 value2)
        {
            Matrix3x3 result; Matrix3x3.Multiply(ref value2, -1.0f, out value2);
            Matrix3x3.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion


        /// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="result">The resulting rotation matrix</param>
        #region public static void CreateFromAxisAngle(ref JVector axis, float angle, out JMatrix result)
        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix3x3 result)
        {
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float num2 = (float)Math.Sin((double)angle);
            float num = (float)Math.Cos((double)angle);
            float num11 = x * x;
            float num10 = y * y;
            float num9 = z * z;
            float num8 = x * y;
            float num7 = x * z;
            float num6 = y * z;
            result.M11 = num11 + (num * (1f - num11));
            result.M12 = (num8 - (num * num8)) + (num2 * z);
            result.M13 = (num7 - (num * num7)) - (num2 * y);
            result.M21 = (num8 - (num * num8)) - (num2 * z);
            result.M22 = num10 + (num * (1f - num10));
            result.M23 = (num6 - (num * num6)) + (num2 * x);
            result.M31 = (num7 - (num * num7)) + (num2 * y);
            result.M32 = (num6 - (num * num6)) - (num2 * x);
            result.M33 = num9 + (num * (1f - num9));
        }

        /// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>The resulting rotation matrix</returns>
        public static Matrix3x3 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            Matrix3x3 result; CreateFromAxisAngle(ref axis, angle, out result);
            return result;
        }

        #endregion

    }
}
