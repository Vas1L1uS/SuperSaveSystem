using System;
using UnityEngine;

namespace AAA_NewSaveSystem.Scripts.SaveSystem.UnityComponentsData
{
    [Serializable]
    public struct RigidbodyData
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public bool isKinematic;
        public bool useGravity;
        public float drag;
        public float mass;
        public bool detectCollisions;
        public float sleepThreshold;
        public int solverIterations;
        public bool automaticInertiaTensor;
        public Vector3 inertiaTensor;
        public Vector3 centerOfMass;
        public bool freezeRotation;
        public float angularDrag;
        public float maxAngularVelocity;
        public float maxDepenetrationVelocity;
        public float maxLinearVelocity;
        public int solverVelocityIterations;
        public Quaternion inertiaTensorRotation;
        public bool automaticCenterOfMass;
    }
}