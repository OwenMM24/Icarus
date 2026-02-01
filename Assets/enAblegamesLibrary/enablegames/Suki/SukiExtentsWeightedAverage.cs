using System;
using System.Collections.Generic;
using UnityEngine;
using Enablegames;
using Enablegames.Suki;
namespace Enablegames
{
namespace Suki
{
    public class SukiExtentsWeightedAverage: MonoBehaviour
    {
        SukiExtents extentsProto;

        void Awake()
        {
            Debug.Log("SEWA:Awake");
            extentsProto = new SukiExtentsWeightedAvg();
            SukiSchemaExtents.SetExtentsHandler(extentsProto);            
        }

    }

    public class SukiExtentsWeightedAvg: SukiExtents
    {
        public SukiExtentsWeightedAvg()
        {
        }
        public override SukiExtents Create()
        {
            Debug.Log("SEWA:Create");
            return new SukiExtentsWeightedAvg();
        }


        // updates the min/max extents if the value lies outside the previously observed extents
        public override void UpdateExtents(float value)
        {
            Debug.Log("SEWA:UpdateExtents " + value);
            // do nothing if we have not yet received a response from server
            if (!extentsQueried)
            {
                //return;
            }

            bool updated = false;
            if (!extentsSet)    //treat first value as special
            {
                //interpolate first value with schema default value

                Debug.Log("Initial value for blend is " + value);
                Debug.Log("Extents for blend are " + minExtent + " and " + maxExtent);
                minExtent = value * (1f - defaultExtentPercent) + minExtent * defaultExtentPercent;
                maxExtent = value * (1f - defaultExtentPercent) + maxExtent * defaultExtentPercent;
                Debug.Log("Extents set for first time to " + minExtent + " and " + maxExtent);
                extentsSet = true;
                updated = true;
            }
            else
            {
                if (value < minExtent)
                {
                    if (minExtent < float.MaxValue) //not still initialized value
                        minExtent = (minExtent * (minmaxHistory - 1) + value) / minmaxHistory;
                    //Obsolete now, because first value is determined by extentsSet bool above
                    //Left in for now for testing...
                    else  //first time being set  
                        minExtent = value;
                    updated = true;
                }
                if (value > maxExtent)
                {
                    if (maxExtent > float.MinValue)
                        maxExtent = (maxExtent * (minmaxHistory - 1) + value) / minmaxHistory;
                    //Obsolete now, because first value is determined by extentsSet bool above
                    else
                        maxExtent = value;
                    updated = true;
                }
            }
			/*
            if (value < minExtent)
            {
                minExtent = value;
                updated = true;
            }
            if (value > maxExtent)
            {
                maxExtent = value;
                updated = true;
            }*/
            if (updated)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("min", (object)minExtent);
                values.Add("max", (object)maxExtent);
//                if (schema!=null)
                    EnableAPI.Instance.SetSchemaExtents(_id, values);
            }
        }

		float minmaxHistory = 100f;
        // updates the min/max extents if the value lies outside the previously observed extents
        public override void UpdateExtents(Vector2 value)
        {
            Debug.Log("SEWA:UpdateExtents 2D" + value);
            // do nothing if we have not yet received a response from server
            if (!extentsQueried)
            {
                //return;
            }

            bool updated = false;
            if (!extentsSet)    //treat first value as special
            {
                //interpolate first value with schema default value

                Debug.Log("Initial value for blend is " + value);
                Debug.Log("Extents for blend are " + min2DExtent + " and " + max2DExtent);
                min2DExtent.x = value.x * (1f - defaultExtentPercent) + min2DExtent.x * defaultExtentPercent;
                max2DExtent.x = value.x * (1f - defaultExtentPercent) + max2DExtent.x * defaultExtentPercent;
                min2DExtent.y = value.y * (1f - defaultExtentPercent) + min2DExtent.y * defaultExtentPercent;
                max2DExtent.y = value.y * (1f - defaultExtentPercent) + max2DExtent.y * defaultExtentPercent;
                Debug.Log("Extents set for first time to " + min2DExtent + " and " + max2DExtent);
                extentsSet = true;
                updated = true;
            }
            else
            {
                if (value.x < min2DExtent.x)
                {
                    if (min2DExtent.x < float.MaxValue)
                        min2DExtent.x = (min2DExtent.x * (minmaxHistory - 1) + value.x) / minmaxHistory;
                    else
                        min2DExtent.x = value.x;
                    updated = true;
                }
                if (value.x > max2DExtent.x)
                {
                    //Debug.Log ("maxx=" + max2DExtent + ", floatmin=" + float.MinValue + " greater than? " + (max2DExtent.x > float.MinValue));
                    if (max2DExtent.x > float.MinValue)
                        max2DExtent.x = (max2DExtent.x * (minmaxHistory - 1) + value.x) / minmaxHistory;
                    else
                        max2DExtent.x = value.x;
                    updated = true;
                }
                if (value.y < min2DExtent.y)
                {
                    if (min2DExtent.y < float.MaxValue)
                        min2DExtent.y = (min2DExtent.y * (minmaxHistory - 1) + value.y) / minmaxHistory;
                    else
                        min2DExtent.y = value.y;
                    updated = true;
                }
                if (value.y > max2DExtent.y)
                {
                    if (max2DExtent.y > float.MinValue)
                        max2DExtent.y = (max2DExtent.y * (minmaxHistory - 1) + value.y) / minmaxHistory;
                    else
                        max2DExtent.y = value.y;
                    updated = true;
                }
            }
            if (updated)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("xMin", (object)min2DExtent.x);
                values.Add("xMax", (object)max2DExtent.x);
                values.Add("yMin", (object)min2DExtent.y);
                values.Add("yMax", (object)max2DExtent.y);
//                if (schema!=null)
                    EnableAPI.Instance.SetSchemaExtents(_id, values);
            }
        }

        // updates the min/max extents if the value lies outside the previously observed extents
        public override void UpdateExtents(Vector3 value)
        {
            // do nothing if we have not yet received a response from server
            if (!extentsQueried)
            {
                //return;
            }

            bool updated = false;
            if (!extentsSet)    //treat first value as special
            {
                //interpolate first value with schema default value

                Debug.Log("Initial value for blend is " + value);
                Debug.Log("Extents for blend are " + minExtent + " and " + maxExtent);
                min3DExtent.x = value.x * (1f - defaultExtentPercent) + min3DExtent.x * defaultExtentPercent;
                max3DExtent.x = value.x * (1f - defaultExtentPercent) + max3DExtent.x * defaultExtentPercent;
                min3DExtent.y = value.y * (1f - defaultExtentPercent) + min3DExtent.y * defaultExtentPercent;
                max3DExtent.y = value.y * (1f - defaultExtentPercent) + max3DExtent.y * defaultExtentPercent;
                min3DExtent.z = value.z * (1f - defaultExtentPercent) + min3DExtent.z * defaultExtentPercent;
                max3DExtent.z = value.z * (1f - defaultExtentPercent) + max3DExtent.z * defaultExtentPercent;
                Debug.Log("Extents set for first time to " + minExtent + " and " + maxExtent);
                extentsSet = true;
                updated = true;
            }
            else
            {
                if (value.x < min3DExtent.x)
                {
                    if (min3DExtent.x < float.MaxValue)
                        min3DExtent.x = (min3DExtent.x * (minmaxHistory - 1) + value.x) / minmaxHistory;
                    else
                        min3DExtent.x = value.x;
                    updated = true;
                }
                if (value.x > max3DExtent.x)
                {
                    //Debug.Log ("maxx=" + max3DExtent + ", floatmin=" + float.MinValue + " greater than? " + (max3DExtent.x > float.MinValue));
                    if (max3DExtent.x > float.MinValue)
                        max3DExtent.x = (max3DExtent.x * (minmaxHistory - 1) + value.x) / minmaxHistory;
                    else
                        max3DExtent.x = value.x;
                    updated = true;
                }
                if (value.y < min3DExtent.y)
                {
                    if (min3DExtent.y < float.MaxValue)
                        min3DExtent.y = (min3DExtent.y * (minmaxHistory - 1) + value.y) / minmaxHistory;
                    else
                        min3DExtent.y = value.y;
                    updated = true;
                }
                if (value.y > max3DExtent.y)
                {
                    if (max3DExtent.y > float.MinValue)
                        max3DExtent.y = (max3DExtent.y * (minmaxHistory - 1) + value.y) / minmaxHistory;
                    else
                        max3DExtent.y = value.y;
                    updated = true;
                }
                if (value.z < min3DExtent.z)
                {
                    if (min3DExtent.z < float.MaxValue)
                        min3DExtent.z = (min3DExtent.z * (minmaxHistory - 1) + value.z) / minmaxHistory;
                    else
                        min3DExtent.z = value.z;
                    updated = true;
                }
                if (value.z > max3DExtent.z)
                {
                    if (max3DExtent.z > float.MinValue)
                        max3DExtent.z = (max3DExtent.z * (minmaxHistory - 1) + value.z) / minmaxHistory;
                    else
                        max3DExtent.z = value.z;
                    updated = true;
                }
            }
            if (updated)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("xMin", (object)min3DExtent.x);
                values.Add("xMax", (object)max3DExtent.x);
                values.Add("yMin", (object)min3DExtent.y);
                values.Add("yMax", (object)max3DExtent.y);
                values.Add("yMin", (object)min3DExtent.z);
                values.Add("yMax", (object)max3DExtent.z);
 //               if (schema!=null)
                    EnableAPI.Instance.SetSchemaExtents(_id, values);
            }
        }

    }

}
}