/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
* 
*  This software is provided 'as-is', without any express or implied
*  warranty.  In no event will the authors be held liable for any damages
*  arising from the use of this software.
*
*  Permission is granted to anyone to use this software for any purpose,
*  including commercial applications, and to alter it and redistribute it
*  freely, subject to the following restrictions:
*
*  1. The origin of this software must not be misrepresented; you must not
*      claim that you wrote the original software. If you use this software
*      in a product, an acknowledgment in the product documentation would be
*      appreciated but is not required.
*  2. Altered source versions must be plainly marked as such, and must not be
*      misrepresented as being the original software.
*  3. This notice may not be removed or altered from any source distribution. 
*/

#region Using Statements
using System;
using System.Collections.Generic;

using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
using Jitter.Dynamics.Constraints;
using Util.MyMath;
#endregion

namespace Jitter.Dynamics
{

    #region public class ContactSettings
    public class ContactSettings
    {
        public enum MaterialCoefficientMixingType { TakeMaximum, TakeMinimum, UseAverage }

        internal float maximumBias = 10.0f;
        internal float bias = 0.25f;
        internal float minVelocity = 0.001f;
        internal float allowedPenetration = 0.01f;
        internal float breakThreshold = 0.01f;

        internal MaterialCoefficientMixingType materialMode = MaterialCoefficientMixingType.UseAverage;

        public float MaximumBias { get { return maximumBias; } set { maximumBias = value; } }

        public float BiasFactor { get { return bias; } set { bias = value; } }

        public float MinimumVelocity { get { return minVelocity; } set { minVelocity = value; } }

        public float AllowedPenetration { get { return allowedPenetration; } set { allowedPenetration = value; } }

        public float BreakThreshold { get { return breakThreshold; } set { breakThreshold = value; } }

        public MaterialCoefficientMixingType MaterialCoefficientMixing { get { return materialMode; } set { materialMode = value; } }
    }
    #endregion


    /// <summary>
    /// </summary>
    public class Contact : IConstraint
    {
        private ContactSettings settings;

        internal RigidBody body1, body2;

        internal Vector3 normal, tangent;

        internal Vector3 realRelPos1, realRelPos2;
        internal Vector3 relativePos1, relativePos2;
        internal Vector3 p1, p2;

        internal float accumulatedNormalImpulse = 0.0f;
        internal float accumulatedTangentImpulse = 0.0f;

        internal float penetration = 0.0f;
        internal float initialPen = 0.0f;

        private float staticFriction, dynamicFriction, restitution;
        private float friction = 0.0f;

        private float massNormal = 0.0f, massTangent = 0.0f;
        private float restitutionBias = 0.0f;

        private bool newContact = false;

        private bool treatBody1AsStatic = false;
        private bool treatBody2AsStatic = false;


        bool body1IsMassPoint; bool body2IsMassPoint;

        float lostSpeculativeBounce = 0.0f;
        float speculativeVelocity = 0.0f;

        /// <summary>
        /// A contact resource pool.
        /// </summary>
        public static readonly ResourcePool<Contact> Pool =
            new ResourcePool<Contact>();

        private float lastTimeStep = float.PositiveInfinity;

        #region Properties
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }

        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }

        public float DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }

        /// <summary>
        /// The first body involved in the contact.
        /// </summary>
        public RigidBody Body1 { get { return body1; } }

        /// <summary>
        /// The second body involved in the contact.
        /// </summary>
        public RigidBody Body2 { get { return body2; } }

        /// <summary>
        /// The penetration of the contact.
        /// </summary>
        public float Penetration { get { return penetration; } }

        /// <summary>
        /// The collision position in world space of body1.
        /// </summary>
        public Vector3 Position1 { get { return p1; } }

        /// <summary>
        /// The collision position in world space of body2.
        /// </summary>
        public Vector3 Position2 { get { return p2; } }

        /// <summary>
        /// The contact tangent.
        /// </summary>
        public Vector3 Tangent { get { return tangent; } }

        /// <summary>
        /// The contact normal.
        /// </summary>
        public Vector3 Normal { get { return normal; } }
        #endregion

        /// <summary>
        /// Calculates relative velocity of body contact points on the bodies.
        /// </summary>
        /// <param name="relVel">The relative velocity of body contact points on the bodies.</param>
        public Vector3 CalculateRelativeVelocity()
        {
            float x, y, z;

            x = (body2.angularVelocity.y * relativePos2.z) - (body2.angularVelocity.z * relativePos2.y) + body2.linearVelocity.x;
            y = (body2.angularVelocity.z * relativePos2.x) - (body2.angularVelocity.x * relativePos2.z) + body2.linearVelocity.y;
            z = (body2.angularVelocity.x * relativePos2.y) - (body2.angularVelocity.y * relativePos2.x) + body2.linearVelocity.z;

            Vector3 relVel;
            relVel.x = x - (body1.angularVelocity.y * relativePos1.z) + (body1.angularVelocity.z * relativePos1.y) - body1.linearVelocity.x;
            relVel.y = y - (body1.angularVelocity.z * relativePos1.x) + (body1.angularVelocity.x * relativePos1.z) - body1.linearVelocity.y;
            relVel.z = z - (body1.angularVelocity.x * relativePos1.y) + (body1.angularVelocity.y * relativePos1.x) - body1.linearVelocity.z;

            return relVel;
        }

        /// <summary>
        /// Solves the contact iteratively.
        /// </summary>
        public void Iterate()
        {
            //body1.linearVelocity = JVector.Zero;
            //body2.linearVelocity = JVector.Zero;
            //return;

            if (treatBody1AsStatic && treatBody2AsStatic) return;

            float dvx, dvy, dvz;

            dvx = body2.linearVelocity.x - body1.linearVelocity.x;
            dvy = body2.linearVelocity.y - body1.linearVelocity.y;
            dvz = body2.linearVelocity.z - body1.linearVelocity.z;

            if (!body1IsMassPoint)
            {
                dvx = dvx - (body1.angularVelocity.y * relativePos1.z) + (body1.angularVelocity.z * relativePos1.y);
                dvy = dvy - (body1.angularVelocity.z * relativePos1.x) + (body1.angularVelocity.x * relativePos1.z);
                dvz = dvz - (body1.angularVelocity.x * relativePos1.y) + (body1.angularVelocity.y * relativePos1.x);
            }

            if (!body2IsMassPoint)
            {
                dvx = dvx + (body2.angularVelocity.y * relativePos2.z) - (body2.angularVelocity.z * relativePos2.y);
                dvy = dvy + (body2.angularVelocity.z * relativePos2.x) - (body2.angularVelocity.x * relativePos2.z);
                dvz = dvz + (body2.angularVelocity.x * relativePos2.y) - (body2.angularVelocity.y * relativePos2.x);
            }

            // this gets us some performance
            if (dvx * dvx + dvy * dvy + dvz * dvz < settings.minVelocity * settings.minVelocity)
            { return; }

            float vn = normal.x * dvx + normal.y * dvy + normal.z * dvz;
            float normalImpulse = massNormal * (-vn + restitutionBias + speculativeVelocity);

            float oldNormalImpulse = accumulatedNormalImpulse;
            accumulatedNormalImpulse = oldNormalImpulse + normalImpulse;
            if (accumulatedNormalImpulse < 0.0f) accumulatedNormalImpulse = 0.0f;
            normalImpulse = accumulatedNormalImpulse - oldNormalImpulse;

            float vt = dvx * tangent.x + dvy * tangent.y + dvz * tangent.z;
            float maxTangentImpulse = friction * accumulatedNormalImpulse;
            float tangentImpulse = massTangent * (-vt);

            float oldTangentImpulse = accumulatedTangentImpulse;
            accumulatedTangentImpulse = oldTangentImpulse + tangentImpulse;
            if (accumulatedTangentImpulse < -maxTangentImpulse) accumulatedTangentImpulse = -maxTangentImpulse;
            else if (accumulatedTangentImpulse > maxTangentImpulse) accumulatedTangentImpulse = maxTangentImpulse;

            tangentImpulse = accumulatedTangentImpulse - oldTangentImpulse;

            // Apply contact impulse
            Vector3 impulse;
            impulse.x = normal.x * normalImpulse + tangent.x * tangentImpulse;
            impulse.y = normal.y * normalImpulse + tangent.y * tangentImpulse;
            impulse.z = normal.z * normalImpulse + tangent.z * tangentImpulse;

            if (!treatBody1AsStatic)
            {
                body1.linearVelocity.x -= (impulse.x * body1.inverseMass);
                body1.linearVelocity.y -= (impulse.y * body1.inverseMass);
                body1.linearVelocity.z -= (impulse.z * body1.inverseMass);

                if (!body1IsMassPoint)
                {
                    float num0, num1, num2;
                    num0 = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
                    num1 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
                    num2 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;

                    float num3 =
                        (((num0 * body1.invInertiaWorld.M11) +
                        (num1 * body1.invInertiaWorld.M21)) +
                        (num2 * body1.invInertiaWorld.M31));
                    float num4 =
                        (((num0 * body1.invInertiaWorld.M12) +
                        (num1 * body1.invInertiaWorld.M22)) +
                        (num2 * body1.invInertiaWorld.M32));
                    float num5 =
                        (((num0 * body1.invInertiaWorld.M13) +
                        (num1 * body1.invInertiaWorld.M23)) +
                        (num2 * body1.invInertiaWorld.M33));

                    body1.angularVelocity.x -= num3;
                    body1.angularVelocity.y -= num4;
                    body1.angularVelocity.z -= num5;
                }
            }

            if (!treatBody2AsStatic)
            {

                body2.linearVelocity.x += (impulse.x * body2.inverseMass);
                body2.linearVelocity.y += (impulse.y * body2.inverseMass);
                body2.linearVelocity.z += (impulse.z * body2.inverseMass);

                if (!body2IsMassPoint)
                {

                    float num0, num1, num2;
                    num0 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
                    num1 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
                    num2 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;

                    float num3 =
                        (((num0 * body2.invInertiaWorld.M11) +
                        (num1 * body2.invInertiaWorld.M21)) +
                        (num2 * body2.invInertiaWorld.M31));
                    float num4 =
                        (((num0 * body2.invInertiaWorld.M12) +
                        (num1 * body2.invInertiaWorld.M22)) +
                        (num2 * body2.invInertiaWorld.M32));
                    float num5 =
                        (((num0 * body2.invInertiaWorld.M13) +
                        (num1 * body2.invInertiaWorld.M23)) +
                        (num2 * body2.invInertiaWorld.M33));

                    body2.angularVelocity.x += num3;
                    body2.angularVelocity.y += num4;
                    body2.angularVelocity.z += num5;

                }
            }

        }

        public float AppliedNormalImpulse { get { return accumulatedNormalImpulse; } }
        public float AppliedTangentImpulse { get { return accumulatedTangentImpulse; } }

        /// <summary>
        /// The points in wolrd space gets recalculated by transforming the
        /// local coordinates. Also new penetration depth is estimated.
        /// </summary>
        public void UpdatePosition()
        {
            if (body1IsMassPoint)
            {
                Vector3.Add(ref realRelPos1, ref body1.position, out p1);
            }
            else
            {
                Vector3.Transform(ref realRelPos1, ref body1.orientation, out p1);
                Vector3.Add(ref p1, ref body1.position, out p1);
            }

            if (body2IsMassPoint)
            {
                Vector3.Add(ref realRelPos2, ref body2.position, out p2);
            }
            else
            {
                Vector3.Transform(ref realRelPos2, ref body2.orientation, out p2);
                Vector3.Add(ref p2, ref body2.position, out p2);
            }


            Vector3 dist; Vector3.Subtract(ref p1, ref p2, out dist);
            penetration = Vector3.Dot(ref dist, ref normal);
        }

        /// <summary>
        /// An impulse is applied an both contact points.
        /// </summary>
        /// <param name="impulse">The impulse to apply.</param>
        public void ApplyImpulse(ref Vector3 impulse)
        {
            #region INLINE - HighFrequency
            //JVector temp;

            if (!treatBody1AsStatic)
            {
                body1.linearVelocity.x -= (impulse.x * body1.inverseMass);
                body1.linearVelocity.y -= (impulse.y * body1.inverseMass);
                body1.linearVelocity.z -= (impulse.z * body1.inverseMass);

                float num0, num1, num2;
                num0 = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
                num1 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
                num2 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;

                float num3 =
                    (((num0 * body1.invInertiaWorld.M11) +
                    (num1 * body1.invInertiaWorld.M21)) +
                    (num2 * body1.invInertiaWorld.M31));
                float num4 =
                    (((num0 * body1.invInertiaWorld.M12) +
                    (num1 * body1.invInertiaWorld.M22)) +
                    (num2 * body1.invInertiaWorld.M32));
                float num5 =
                    (((num0 * body1.invInertiaWorld.M13) +
                    (num1 * body1.invInertiaWorld.M23)) +
                    (num2 * body1.invInertiaWorld.M33));

                body1.angularVelocity.x -= num3;
                body1.angularVelocity.y -= num4;
                body1.angularVelocity.z -= num5;
            }

            if (!treatBody2AsStatic)
            {

                body2.linearVelocity.x += (impulse.x * body2.inverseMass);
                body2.linearVelocity.y += (impulse.y * body2.inverseMass);
                body2.linearVelocity.z += (impulse.z * body2.inverseMass);

                float num0, num1, num2;
                num0 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
                num1 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
                num2 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;

                float num3 =
                    (((num0 * body2.invInertiaWorld.M11) +
                    (num1 * body2.invInertiaWorld.M21)) +
                    (num2 * body2.invInertiaWorld.M31));
                float num4 =
                    (((num0 * body2.invInertiaWorld.M12) +
                    (num1 * body2.invInertiaWorld.M22)) +
                    (num2 * body2.invInertiaWorld.M32));
                float num5 =
                    (((num0 * body2.invInertiaWorld.M13) +
                    (num1 * body2.invInertiaWorld.M23)) +
                    (num2 * body2.invInertiaWorld.M33));

                body2.angularVelocity.x += num3;
                body2.angularVelocity.y += num4;
                body2.angularVelocity.z += num5;
            }


            #endregion
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            #region INLINE - HighFrequency
            //JVector temp;

            if (!treatBody1AsStatic)
            {
                body1.linearVelocity.x -= (impulse.x * body1.inverseMass);
                body1.linearVelocity.y -= (impulse.y * body1.inverseMass);
                body1.linearVelocity.z -= (impulse.z * body1.inverseMass);

                float num0, num1, num2;
                num0 = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
                num1 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
                num2 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;

                float num3 =
                    (((num0 * body1.invInertiaWorld.M11) +
                    (num1 * body1.invInertiaWorld.M21)) +
                    (num2 * body1.invInertiaWorld.M31));
                float num4 =
                    (((num0 * body1.invInertiaWorld.M12) +
                    (num1 * body1.invInertiaWorld.M22)) +
                    (num2 * body1.invInertiaWorld.M32));
                float num5 =
                    (((num0 * body1.invInertiaWorld.M13) +
                    (num1 * body1.invInertiaWorld.M23)) +
                    (num2 * body1.invInertiaWorld.M33));

                body1.angularVelocity.x -= num3;
                body1.angularVelocity.y -= num4;
                body1.angularVelocity.z -= num5;
            }

            if (!treatBody2AsStatic)
            {

                body2.linearVelocity.x += (impulse.x * body2.inverseMass);
                body2.linearVelocity.y += (impulse.y * body2.inverseMass);
                body2.linearVelocity.z += (impulse.z * body2.inverseMass);

                float num0, num1, num2;
                num0 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
                num1 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
                num2 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;

                float num3 =
                    (((num0 * body2.invInertiaWorld.M11) +
                    (num1 * body2.invInertiaWorld.M21)) +
                    (num2 * body2.invInertiaWorld.M31));
                float num4 =
                    (((num0 * body2.invInertiaWorld.M12) +
                    (num1 * body2.invInertiaWorld.M22)) +
                    (num2 * body2.invInertiaWorld.M32));
                float num5 =
                    (((num0 * body2.invInertiaWorld.M13) +
                    (num1 * body2.invInertiaWorld.M23)) +
                    (num2 * body2.invInertiaWorld.M33));

                body2.angularVelocity.x += num3;
                body2.angularVelocity.y += num4;
                body2.angularVelocity.z += num5;
            }


            #endregion
        }

        /// <summary>
        /// PrepareForIteration has to be called before <see cref="Iterate"/>.
        /// </summary>
        /// <param name="timestep">The timestep of the simulation.</param>
        public void PrepareForIteration(float timestep)
        {
            float dvx, dvy, dvz;

            dvx = (body2.angularVelocity.y * relativePos2.z) - (body2.angularVelocity.z * relativePos2.y) + body2.linearVelocity.x;
            dvy = (body2.angularVelocity.z * relativePos2.x) - (body2.angularVelocity.x * relativePos2.z) + body2.linearVelocity.y;
            dvz = (body2.angularVelocity.x * relativePos2.y) - (body2.angularVelocity.y * relativePos2.x) + body2.linearVelocity.z;

            dvx = dvx - (body1.angularVelocity.y * relativePos1.z) + (body1.angularVelocity.z * relativePos1.y) - body1.linearVelocity.x;
            dvy = dvy - (body1.angularVelocity.z * relativePos1.x) + (body1.angularVelocity.x * relativePos1.z) - body1.linearVelocity.y;
            dvz = dvz - (body1.angularVelocity.x * relativePos1.y) + (body1.angularVelocity.y * relativePos1.x) - body1.linearVelocity.z;

            float kNormal = 0.0f;

            Vector3 rantra = Vector3.zero;
            if (!treatBody1AsStatic)
            {
                kNormal += body1.inverseMass;

                if (!body1IsMassPoint)
                {

                    // JVector.Cross(ref relativePos1, ref normal, out rantra);
                    rantra.x = (relativePos1.y * normal.z) - (relativePos1.z * normal.y);
                    rantra.y = (relativePos1.z * normal.x) - (relativePos1.x * normal.z);
                    rantra.z = (relativePos1.x * normal.y) - (relativePos1.y * normal.x);

                    // JVector.Transform(ref rantra, ref body1.invInertiaWorld, out rantra);
                    float num0 = ((rantra.x * body1.invInertiaWorld.M11) + (rantra.y * body1.invInertiaWorld.M21)) + (rantra.z * body1.invInertiaWorld.M31);
                    float num1 = ((rantra.x * body1.invInertiaWorld.M12) + (rantra.y * body1.invInertiaWorld.M22)) + (rantra.z * body1.invInertiaWorld.M32);
                    float num2 = ((rantra.x * body1.invInertiaWorld.M13) + (rantra.y * body1.invInertiaWorld.M23)) + (rantra.z * body1.invInertiaWorld.M33);

                    rantra.x = num0; rantra.y = num1; rantra.z = num2;

                    //JVector.Cross(ref rantra, ref relativePos1, out rantra);
                    num0 = (rantra.y * relativePos1.z) - (rantra.z * relativePos1.y);
                    num1 = (rantra.z * relativePos1.x) - (rantra.x * relativePos1.z);
                    num2 = (rantra.x * relativePos1.y) - (rantra.y * relativePos1.x);

                    rantra.x = num0; rantra.y = num1; rantra.z = num2;
                }
            }

            Vector3 rbntrb = Vector3.zero;
            if (!treatBody2AsStatic)
            {
                kNormal += body2.inverseMass;

                if (!body2IsMassPoint)
                {

                    // JVector.Cross(ref relativePos1, ref normal, out rantra);
                    rbntrb.x = (relativePos2.y * normal.z) - (relativePos2.z * normal.y);
                    rbntrb.y = (relativePos2.z * normal.x) - (relativePos2.x * normal.z);
                    rbntrb.z = (relativePos2.x * normal.y) - (relativePos2.y * normal.x);

                    // JVector.Transform(ref rantra, ref body1.invInertiaWorld, out rantra);
                    float num0 = ((rbntrb.x * body2.invInertiaWorld.M11) + (rbntrb.y * body2.invInertiaWorld.M21)) + (rbntrb.z * body2.invInertiaWorld.M31);
                    float num1 = ((rbntrb.x * body2.invInertiaWorld.M12) + (rbntrb.y * body2.invInertiaWorld.M22)) + (rbntrb.z * body2.invInertiaWorld.M32);
                    float num2 = ((rbntrb.x * body2.invInertiaWorld.M13) + (rbntrb.y * body2.invInertiaWorld.M23)) + (rbntrb.z * body2.invInertiaWorld.M33);

                    rbntrb.x = num0; rbntrb.y = num1; rbntrb.z = num2;

                    //JVector.Cross(ref rantra, ref relativePos1, out rantra);
                    num0 = (rbntrb.y * relativePos2.z) - (rbntrb.z * relativePos2.y);
                    num1 = (rbntrb.z * relativePos2.x) - (rbntrb.x * relativePos2.z);
                    num2 = (rbntrb.x * relativePos2.y) - (rbntrb.y * relativePos2.x);

                    rbntrb.x = num0; rbntrb.y = num1; rbntrb.z = num2;
                }
            }

            if (!treatBody1AsStatic) kNormal += rantra.x * normal.x + rantra.y * normal.y + rantra.z * normal.z;
            if (!treatBody2AsStatic) kNormal += rbntrb.x * normal.x + rbntrb.y * normal.y + rbntrb.z * normal.z;

            massNormal = 1.0f / kNormal;

            float num = dvx * normal.x + dvy * normal.y + dvz * normal.z;

            tangent.x = dvx - normal.x * num;
            tangent.y = dvy - normal.y * num;
            tangent.z = dvz - normal.z * num;

            num = tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z;

            if (num != 0.0f)
            {
                num = (float)Math.Sqrt(num);
                tangent.x /= num;
                tangent.y /= num;
                tangent.z /= num;
            }

            float kTangent = 0.0f;

            if (treatBody1AsStatic) rantra.MakeZero();
            else
            {
                kTangent += body1.inverseMass;
  
                if (!body1IsMassPoint)
                {
                    // JVector.Cross(ref relativePos1, ref normal, out rantra);
                    rantra.x = (relativePos1.y * tangent.z) - (relativePos1.z * tangent.y);
                    rantra.y = (relativePos1.z * tangent.x) - (relativePos1.x * tangent.z);
                    rantra.z = (relativePos1.x * tangent.y) - (relativePos1.y * tangent.x);

                    // JVector.Transform(ref rantra, ref body1.invInertiaWorld, out rantra);
                    float num0 = ((rantra.x * body1.invInertiaWorld.M11) + (rantra.y * body1.invInertiaWorld.M21)) + (rantra.z * body1.invInertiaWorld.M31);
                    float num1 = ((rantra.x * body1.invInertiaWorld.M12) + (rantra.y * body1.invInertiaWorld.M22)) + (rantra.z * body1.invInertiaWorld.M32);
                    float num2 = ((rantra.x * body1.invInertiaWorld.M13) + (rantra.y * body1.invInertiaWorld.M23)) + (rantra.z * body1.invInertiaWorld.M33);

                    rantra.x = num0; rantra.y = num1; rantra.z = num2;

                    //JVector.Cross(ref rantra, ref relativePos1, out rantra);
                    num0 = (rantra.y * relativePos1.z) - (rantra.z * relativePos1.y);
                    num1 = (rantra.z * relativePos1.x) - (rantra.x * relativePos1.z);
                    num2 = (rantra.x * relativePos1.y) - (rantra.y * relativePos1.x);

                    rantra.x = num0; rantra.y = num1; rantra.z = num2;
                }

            }

            if (treatBody2AsStatic) rbntrb.MakeZero();
            else
            {
                kTangent += body2.inverseMass;

                if (!body2IsMassPoint)
                {
                    // JVector.Cross(ref relativePos1, ref normal, out rantra);
                    rbntrb.x = (relativePos2.y * tangent.z) - (relativePos2.z * tangent.y);
                    rbntrb.y = (relativePos2.z * tangent.x) - (relativePos2.x * tangent.z);
                    rbntrb.z = (relativePos2.x * tangent.y) - (relativePos2.y * tangent.x);

                    // JVector.Transform(ref rantra, ref body1.invInertiaWorld, out rantra);
                    float num0 = ((rbntrb.x * body2.invInertiaWorld.M11) + (rbntrb.y * body2.invInertiaWorld.M21)) + (rbntrb.z * body2.invInertiaWorld.M31);
                    float num1 = ((rbntrb.x * body2.invInertiaWorld.M12) + (rbntrb.y * body2.invInertiaWorld.M22)) + (rbntrb.z * body2.invInertiaWorld.M32);
                    float num2 = ((rbntrb.x * body2.invInertiaWorld.M13) + (rbntrb.y * body2.invInertiaWorld.M23)) + (rbntrb.z * body2.invInertiaWorld.M33);

                    rbntrb.x = num0; rbntrb.y = num1; rbntrb.z = num2;

                    //JVector.Cross(ref rantra, ref relativePos1, out rantra);
                    num0 = (rbntrb.y * relativePos2.z) - (rbntrb.z * relativePos2.y);
                    num1 = (rbntrb.z * relativePos2.x) - (rbntrb.x * relativePos2.z);
                    num2 = (rbntrb.x * relativePos2.y) - (rbntrb.y * relativePos2.x);

                    rbntrb.x = num0; rbntrb.y = num1; rbntrb.z = num2;
                }
            }

            if (!treatBody1AsStatic) kTangent += Vector3.Dot(ref rantra, ref tangent);
            if (!treatBody2AsStatic) kTangent += Vector3.Dot(ref rbntrb, ref tangent);
            massTangent = 1.0f / kTangent;

            restitutionBias = lostSpeculativeBounce;

            speculativeVelocity = 0.0f;

            float relNormalVel = normal.x * dvx + normal.y * dvy + normal.z * dvz; //JVector.Dot(ref normal, ref dv);

            if (Penetration > settings.allowedPenetration)
            {
                restitutionBias = settings.bias * (1.0f / timestep) * Mathf.Max(0.0f, Penetration - settings.allowedPenetration);
                restitutionBias = Mathf.Clamp(restitutionBias, 0.0f, settings.maximumBias);
              //  body1IsMassPoint = body2IsMassPoint = false;
            }
      

            float timeStepRatio = timestep / lastTimeStep;
            accumulatedNormalImpulse *= timeStepRatio;
            accumulatedTangentImpulse *= timeStepRatio;

            {
                // Static/Dynamic friction
                float relTangentVel = -(tangent.x * dvx + tangent.y * dvy + tangent.z * dvz);
                float tangentImpulse = massTangent * relTangentVel;
                float maxTangentImpulse = -staticFriction * accumulatedNormalImpulse;

                if (tangentImpulse < maxTangentImpulse) friction = dynamicFriction;
                else friction = staticFriction;
            }

            Vector3 impulse;

            // Simultaneos solving and restitution is simply not possible
            // so fake it a bit by just applying restitution impulse when there
            // is a new contact.
            if (relNormalVel < -1.0f && newContact)
            {
                restitutionBias = Mathf.Max(-restitution * relNormalVel, restitutionBias);
            }

            // Speculative Contacts!
            // if the penetration is negative (which means the bodies are not already in contact, but they will
            // be in the future) we store the current bounce bias in the variable 'lostSpeculativeBounce'
            // and apply it the next frame, when the speculative contact was already solved.
            if (penetration < -settings.allowedPenetration)
            {
                speculativeVelocity = penetration / timestep;

                lostSpeculativeBounce = restitutionBias;
                restitutionBias = 0.0f;
            }
            else
            {
                lostSpeculativeBounce = 0.0f;
            }

            impulse.x = normal.x * accumulatedNormalImpulse + tangent.x * accumulatedTangentImpulse;
            impulse.y = normal.y * accumulatedNormalImpulse + tangent.y * accumulatedTangentImpulse;
            impulse.z = normal.z * accumulatedNormalImpulse + tangent.z * accumulatedTangentImpulse;

            if (!treatBody1AsStatic)
            {
                body1.linearVelocity.x -= (impulse.x * body1.inverseMass);
                body1.linearVelocity.y -= (impulse.y * body1.inverseMass);
                body1.linearVelocity.z -= (impulse.z * body1.inverseMass);

                if (!body1IsMassPoint)
                {
                    float num0, num1, num2;
                    num0 = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
                    num1 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
                    num2 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;

                    float num3 =
                        (((num0 * body1.invInertiaWorld.M11) +
                        (num1 * body1.invInertiaWorld.M21)) +
                        (num2 * body1.invInertiaWorld.M31));
                    float num4 =
                        (((num0 * body1.invInertiaWorld.M12) +
                        (num1 * body1.invInertiaWorld.M22)) +
                        (num2 * body1.invInertiaWorld.M32));
                    float num5 =
                        (((num0 * body1.invInertiaWorld.M13) +
                        (num1 * body1.invInertiaWorld.M23)) +
                        (num2 * body1.invInertiaWorld.M33));

                    body1.angularVelocity.x -= num3;
                    body1.angularVelocity.y -= num4;
                    body1.angularVelocity.z -= num5;

                }
            }

            if (!treatBody2AsStatic)
            {

                body2.linearVelocity.x += (impulse.x * body2.inverseMass);
                body2.linearVelocity.y += (impulse.y * body2.inverseMass);
                body2.linearVelocity.z += (impulse.z * body2.inverseMass);

                if (!body2IsMassPoint)
                {

                    float num0, num1, num2;
                    num0 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
                    num1 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
                    num2 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;

                    float num3 =
                        (((num0 * body2.invInertiaWorld.M11) +
                        (num1 * body2.invInertiaWorld.M21)) +
                        (num2 * body2.invInertiaWorld.M31));
                    float num4 =
                        (((num0 * body2.invInertiaWorld.M12) +
                        (num1 * body2.invInertiaWorld.M22)) +
                        (num2 * body2.invInertiaWorld.M32));
                    float num5 =
                        (((num0 * body2.invInertiaWorld.M13) +
                        (num1 * body2.invInertiaWorld.M23)) +
                        (num2 * body2.invInertiaWorld.M33));

                    body2.angularVelocity.x += num3;
                    body2.angularVelocity.y += num4;
                    body2.angularVelocity.z += num5;
                }
            }

            lastTimeStep = timestep;

            newContact = false;
        }

        public void TreatBodyAsStatic(RigidBodyIndex index)
        {
            if (index == RigidBodyIndex.RigidBody1) treatBody1AsStatic = true;
            else treatBody2AsStatic = true;
        }


        /// <summary>
        /// Initializes a contact.
        /// </summary>
        /// <param name="body1">The first body.</param>
        /// <param name="body2">The second body.</param>
        /// <param name="point1">The collision point in worldspace</param>
        /// <param name="point2">The collision point in worldspace</param>
        /// <param name="n">The normal pointing to body2.</param>
        /// <param name="penetration">The estimated penetration depth.</param>
        public void Initialize(RigidBody body1, RigidBody body2, ref Vector3 point1, ref Vector3 point2, ref Vector3 n,
            float penetration, bool newContact, ContactSettings settings)
        {
            this.body1 = body1;  this.body2 = body2;
            this.normal = n; normal.Normalize();
            this.p1 = point1; this.p2 = point2;

            this.newContact = newContact;

            Vector3.Subtract(ref p1, ref body1.position, out relativePos1);
            Vector3.Subtract(ref p2, ref body2.position, out relativePos2);
            Vector3.Transform(ref relativePos1, ref body1.invOrientation, out realRelPos1);
            Vector3.Transform(ref relativePos2, ref body2.invOrientation, out realRelPos2);

            this.initialPen = penetration;
            this.penetration = penetration;

            body1IsMassPoint = body1.isParticle;
            body2IsMassPoint = body2.isParticle;

            // Material Properties
            if (newContact)
            {
                treatBody1AsStatic = body1.isStatic;
                treatBody2AsStatic = body2.isStatic;

                accumulatedNormalImpulse = 0.0f;
                accumulatedTangentImpulse = 0.0f;

                lostSpeculativeBounce = 0.0f;

                switch (settings.MaterialCoefficientMixing)
                {
                    case ContactSettings.MaterialCoefficientMixingType.TakeMaximum:
                        staticFriction = Mathf.Max(body1.material.staticFriction, body2.material.staticFriction);
                        dynamicFriction = Mathf.Max(body1.material.kineticFriction, body2.material.kineticFriction);
                        restitution = Mathf.Max(body1.material.restitution, body2.material.restitution);
                        break;
                    case ContactSettings.MaterialCoefficientMixingType.TakeMinimum:
                        staticFriction = Mathf.Min(body1.material.staticFriction, body2.material.staticFriction);
                        dynamicFriction = Mathf.Min(body1.material.kineticFriction, body2.material.kineticFriction);
                        restitution = Mathf.Min(body1.material.restitution, body2.material.restitution);
                        break;
                    case ContactSettings.MaterialCoefficientMixingType.UseAverage:
                        staticFriction = (body1.material.staticFriction + body2.material.staticFriction) / 2.0f;
                        dynamicFriction = (body1.material.kineticFriction + body2.material.kineticFriction) / 2.0f;
                        restitution = (body1.material.restitution + body2.material.restitution) / 2.0f;
                        break;
                }

            }
            this.settings = settings;
        }
    }
}
