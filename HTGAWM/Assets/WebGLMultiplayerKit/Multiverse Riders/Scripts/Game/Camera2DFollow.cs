using System;
using UnityEngine;

namespace MultiverseRiders
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

		public Vector2 maxXAndY; // The maximum x and y coordinates the camera can have.
		public Vector2 minXAndY; // The minimum x and y coordinates the camera can have.

        // Use this for initialization
        private void Start()
        {
		    if(target!=null)
			{
			
             m_LastTargetPosition = target.position;
             m_OffsetZ = (transform.position - target.position).z;
             transform.parent = null;
			}
        }


		public void SetTarget(Transform _target)
		{
		  target = _target;
		  m_LastTargetPosition = target.position;
          m_OffsetZ = (transform.position - target.position).z;
          transform.parent = null;
		}
        // Update is called once per frame
        private void Update()
        {
		   if(target!=null)
			{
		
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

			// By default the target x and y coordinates of the camera are it's current x and y coordinates.
			float targetX = newPos.x;
			float targetY = newPos.y;
			// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
			targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
			targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

			transform.position = new Vector3 (targetX, targetY, newPos.z);

            m_LastTargetPosition = target.position;
		 }
        }
   
}
}