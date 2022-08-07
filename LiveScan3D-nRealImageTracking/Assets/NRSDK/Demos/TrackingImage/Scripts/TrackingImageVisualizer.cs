/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NRExamples
{
    using UnityEngine;

    /// <summary> Uses 4 frame corner objects to visualize an TrackingImage. </summary>
    public class TrackingImageVisualizer : MonoBehaviour
    {
        /// <summary> The TrackingImage to visualize. </summary>
        public NRTrackableImage Image;

/*
        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected. </summary>
        public GameObject FrameLowerLeft;

        /// <summary>
        /// A model for the lower right corner of the frame to place when an image is detected. </summary>
        public GameObject FrameLowerRight;

        /// <summary>
        /// A model for the upper left corner of the frame to place when an image is detected. </summary>
        public GameObject FrameUpperLeft;

        /// <summary>
        /// A model for the upper right corner of the frame to place when an image is detected. </summary>
        public GameObject FrameUpperRight;

        /// <summary> The axis. </summary>
        public GameObject Axis;

        /// <summary> Updates this object. </summary>
        public void Update()
        {
            if (Image == null || Image.GetTrackingState() != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                FrameLowerRight.SetActive(false);
                FrameUpperLeft.SetActive(false);
                FrameUpperRight.SetActive(false);
                Axis.SetActive(false);
                return;
            }

            float halfWidth = Image.ExtentX / 2;
            float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
            FrameLowerRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
            FrameUpperLeft.transform.localPosition = (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
            FrameUpperRight.transform.localPosition = (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);

            var center = Image.GetCenterPose();
            transform.position = center.position;
            transform.rotation = center.rotation;

            FrameLowerLeft.SetActive(true);
            FrameLowerRight.SetActive(true);
            FrameUpperLeft.SetActive(true);
            FrameUpperRight.SetActive(true);
            Axis.SetActive(true);
        }
*/

        // MY mod
        public GameObject Cube;

        
// NOTE: Now want this Visualizer to immediately propagate elements & destroy self
//       FOR ALL PLATFORMS
//#if UNITY_EDITOR
        public void Start()
        {
            // Add to root of scene instead of setting as child
            Instantiate(Cube, Vector3.zero, Quaternion.identity);
            Destroy(gameObject);
            //Cube.SetActive(true);
            Debug.Log("Cube Set Active");
        }
//#else  // Image is not accessible in UNITY_EDITOR, should NOT exec code below
//        public void Update()
//        {
//            // Want the imperfect image tracking here
//            if (Image == null /*|| Image.GetTrackingState() != TrackingState.Tracking*/)
//            {
//                // Cube.SetActive(false);
//                return;
//            }
//            else
//            {
//                var imageCenter = Image.GetCenterPose();
//                // same as Start()
//                Instantiate(Cube, imageCenter.position, imageCenter.rotation);
//                Destroy(gameObject);
//                //transform.position = imageCenter.position + new Vector3(0, 0, 2f);
//                //transform.rotation = imageCenter.rotation;
//                //Cube.SetActive(true);
//                return;
//            }
//        }
//#endif
    }
}
