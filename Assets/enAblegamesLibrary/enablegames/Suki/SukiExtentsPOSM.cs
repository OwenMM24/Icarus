using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Enablegames;
using Enablegames.Suki;
namespace Enablegames
{
namespace Suki
{
    public class SukiExtentsPOSM: MonoBehaviour
    {
        SukiPOSMExtents extentsProto;
        [SerializeField] private float initialRange = 10f;
        [SerializeField] private bool timeBased = true;
        [SerializeField]
        private float tolarence = 0.1f;
        [Range(0,1)]
        public float adjustmentParameterEasier = 0.4f;
        [Range(0,1)]
        public float adjustmentParameterHarder = 0.4f;

        void Awake()
        {
            Debug.Log("SEWA:Awake");
            extentsProto = new SukiPOSMExtents();
            extentsProto.InitialPrameters(initialRange, timeBased, tolarence, adjustmentParameterEasier, adjustmentParameterHarder);
            SukiSchemaExtents.SetExtentsHandler(extentsProto);           
        }

        private void Update()
        {
            if (timeBased)
            {
                extentsProto.UpdateTime();
            }
        }
    }

    public class POSMExtentsValue
    { 
        private float tolarence = 0.1f;
        public float adjustmentParameterEasier = 0.4f;
        public float adjustmentParameterHarder = 0.4f;
        public bool timeBase = false;

        private Dictionary<float, float> _limits = new Dictionary<float, float>();
        private float suggestLevel;
        private float rangeAdjustTime = 5f;
        private int rangeAdjustInputNum = 10;
        private string _id;
        private float range = 10f;
        private float lastInRange = 0;
        private int inRangeCounter = 0;
        private int outRangeCounter = 0;
        private float lastChangeTime;
        private float rangeAdjustVolume = 2f;
        private float closestValue = 0;
        private float furthestValue = float.PositiveInfinity;

        private bool extentsQueried = false;

        public void UpdateTime()
        {
            if (Time.time > lastInRange + rangeAdjustTime)
            {
                AdjustRangeOutward();
            }

            if (Time.time > lastChangeTime + rangeAdjustTime)
            {
                AdjustRangeInward();
            }
        }

        public void SetInitialValue(float value)
        {
            _limits.Add(value, 1f);
        }

        public float SuggestLevel
        {
            get { return suggestLevel; }
        }

        //get new difficulty level suggested by POSM 
        private float UpdateSuggestLevel()
        {
            float newLevel = suggestLevel;
            float newLevelVector = 0;
            foreach (KeyValuePair<float,float> limit in _limits)
            {
                float a = 0;
                float b = 0;
                foreach (KeyValuePair<float,float> l in _limits)
                {
                    if (l.Key.Equals(limit.Key))
                    {
                        a += l.Value;
                        b += l.Value;
                    }
                    else if (l.Key > limit.Key)
                    {
                        a += l.Value;
                    }
                    else if (l.Key < limit.Key)
                    {
                        b += l.Value;
                    }
                }

                if (newLevelVector < Math.Min(a, b))
                {
                    newLevel = limit.Key;
                    newLevelVector = Math.Min(a, b);
                }
            }

            suggestLevel = newLevel;
            foreach (float key in _limits.Keys.ToList())
            {
                if (key > suggestLevel + range || key < suggestLevel - range)
                {
                    _limits.Remove(key);
                }
            }
            return suggestLevel;
        }

        //update belief vector base on observation
        public void UpdateBeliefVector(float observation)
        {
            Debug.Log("Observation: " + observation);
            if(observation > suggestLevel - range || observation < suggestLevel + range)
            {
                if (Math.Abs(suggestLevel - observation) > Math.Abs(suggestLevel - furthestValue))
                {
                    furthestValue = observation;
                }
                if (timeBase)
                {
                    lastInRange = Time.time;
                    rangeAdjustTime = 2f;
                }
                else
                {
                    outRangeCounter = 0;
                    inRangeCounter += 1;
                    if (inRangeCounter > rangeAdjustInputNum)
                    {
                        AdjustRangeInward();
                    }
                }
                if (observation > suggestLevel + tolarence)
                {
                    float initialVector = _limits[suggestLevel];
                    for (float l = suggestLevel + 2 * tolarence; l <= suggestLevel + range; l += 2 * tolarence)
                    {
                        if (l + tolarence <= observation)
                        {
                            if (!_limits.ContainsKey(l))
                            {
                                _limits.Add(l, initialVector);
                            }
                            else
                            {
                                initialVector = _limits[l];
                            }
                        }

                    }

                    foreach (float key in _limits.Keys.ToList())
                    {
                        if (observation > key + tolarence)
                        {
                            _limits[key] *= adjustmentParameterHarder;
                        }
                    }
                }

                if (observation < suggestLevel - tolarence)
                {
                    float initialVector = _limits[suggestLevel];
                    for (float l = suggestLevel - 2 * tolarence; l >= suggestLevel - range; l -= 2 * tolarence)
                    {
                        if (l - tolarence >= observation)
                        {
                            if (!_limits.ContainsKey(l))
                            {
                                _limits.Add(l, initialVector);
                            }

                            else
                            {
                                initialVector = _limits[l];
                            }
                        }
                    }

                    foreach (float key in _limits.Keys.ToList())
                    {
                        if (observation < key - tolarence)
                        {
                            _limits[key] *= adjustmentParameterEasier;
                        }
                    }
                }
            }
            else
            {
                if (Math.Abs(suggestLevel - observation) < Math.Abs(suggestLevel - closestValue) )
                {
                    closestValue = observation;
                }
                if(!timeBase)
                {
                    outRangeCounter += 1;
                }

                if (outRangeCounter > rangeAdjustInputNum)
                {
                    AdjustRangeOutward();
                    
                }
            }

            UpdateSuggestLevel();
        }

        private void AdjustRangeOutward()
        {
            range = Math.Abs(suggestLevel - closestValue);
            ResetCounterOrTimer();
        }

        private void AdjustRangeInward()
        {
            range = Math.Abs(suggestLevel - furthestValue);
            ResetCounterOrTimer();
        }

        private void ResetCounterOrTimer()
        {
            if (timeBase)
            {
                lastInRange = Time.time;
                lastChangeTime = Time.time;
                rangeAdjustTime = 2f;
            }
            else
            {
                outRangeCounter = 0;
                inRangeCounter = 0;
            }
        }

        public void InitialParameters(float r, bool mode, float t, float ParameterE, float ParameterH)
        {
            range = r;
            timeBase = mode;
            tolarence = t;
            adjustmentParameterEasier = ParameterE;
            adjustmentParameterHarder = ParameterH;
        }
        
    }

    public class SukiPOSMExtents: SukiExtents
    {
        public SukiPOSMExtents()
        {
        }
        public override SukiExtents Create()
        {
            Debug.Log("SEPOSM:Create");
            return new SukiPOSMExtents();
        }

        private POSMExtentsValue _minExtentsXValue = new POSMExtentsValue();
        private POSMExtentsValue _minExtentsYValue = new POSMExtentsValue();
        private POSMExtentsValue _minExtentsZValue = new POSMExtentsValue();
        private POSMExtentsValue _maxExtentsXValue = new POSMExtentsValue();
        private POSMExtentsValue _maxExtentsYValue = new POSMExtentsValue();
        private POSMExtentsValue _maxExtentsZValue = new POSMExtentsValue();
        private float _lastInputFloat;
        private Vector2 _lastInputVector2;
        private Vector3 _lastInputVector3;
        private float _lastChangeX;
        private float _lastChangeY;
        private float _lastChangeZ;
        public float changeTolarence = 0.02f;

        public void InitialPrameters(float r, bool mode, float t, float ParameterE, float ParameterH)
        {
            _minExtentsXValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
            _maxExtentsXValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
            _minExtentsYValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
            _maxExtentsYValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
            _minExtentsZValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
            _maxExtentsZValue.InitialParameters(r, mode, t, ParameterE, ParameterH);
        }

        public void UpdateTime()
        {
            _minExtentsXValue.UpdateTime();
            _maxExtentsXValue.UpdateTime();
            _minExtentsYValue.UpdateTime();
            _maxExtentsYValue.UpdateTime();
            _minExtentsZValue.UpdateTime();
            _maxExtentsZValue.UpdateTime();
        }


        // updates the min/max extents if the value lies outside the previously observed extents
        public override void UpdateExtents(float value)
        {
            Debug.Log("SEPOSM:UpdateExtents: " + value);
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
                _minExtentsXValue.SetInitialValue(minExtent);
                _maxExtentsXValue.SetInitialValue(maxExtent);
                extentsSet = true;
                updated = true;
            }
            else
            {
                float newChange = value - _lastInputFloat;
                if(_lastChangeX <= -changeTolarence && newChange > -changeTolarence )
                {
                    _minExtentsXValue.UpdateBeliefVector(value);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeX > changeTolarence && newChange <= changeTolarence )
                {
                    _minExtentsYValue.UpdateBeliefVector(value);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }

                _lastChangeX = newChange;
                _lastInputFloat = value;

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
            // do nothing if we have not yet received a response from server
            if (!extentsQueried)
            {
                return;
            }

            bool updated = false;
            if (!extentsSet)    //treat first value as special
            {
                //interpolate first value with schema default value

                Debug.Log("Initial value for blend is " + value);
                Debug.Log("Extents for blend are " + minExtent + " and " + maxExtent);
                min2DExtent.x = value.x * (1f - defaultExtentPercent) + min2DExtent.x * defaultExtentPercent;
                max2DExtent.x = value.x * (1f - defaultExtentPercent) + max2DExtent.x * defaultExtentPercent;
                min2DExtent.y = value.y * (1f - defaultExtentPercent) + min2DExtent.y * defaultExtentPercent;
                max2DExtent.y = value.y * (1f - defaultExtentPercent) + max2DExtent.y * defaultExtentPercent;
                Debug.Log("Extents set for first time to " + minExtent + " and " + maxExtent);
                _minExtentsXValue.SetInitialValue(minExtent);
                _maxExtentsXValue.SetInitialValue(maxExtent);
                _minExtentsYValue.SetInitialValue(minExtent);
                _maxExtentsYValue.SetInitialValue(maxExtent);
                extentsSet = true;
                updated = true;
            }
            else
            {
                float newChangeX = value.x - _lastInputVector2.x;
                float newChangeY = value.y - _lastInputVector2.y;
                if(_lastChangeX <= -changeTolarence && newChangeX > -changeTolarence )
                {
                    _minExtentsXValue.UpdateBeliefVector(value.x);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeX > changeTolarence && newChangeX <= changeTolarence )
                {
                    _minExtentsXValue.UpdateBeliefVector(value.x);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }
                if(_lastChangeY <= -changeTolarence && newChangeY > -changeTolarence )
                {
                    _minExtentsYValue.UpdateBeliefVector(value.y);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeY > changeTolarence && newChangeY <= changeTolarence )
                {
                    _minExtentsYValue.UpdateBeliefVector(value.y);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }

                _lastChangeX = newChangeX;
                _lastChangeY = newChangeY;
                _lastInputVector2 = value;
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
                return;
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
                _minExtentsXValue.SetInitialValue(min3DExtent.x);
                _maxExtentsXValue.SetInitialValue(max3DExtent.x);
                _minExtentsYValue.SetInitialValue(min3DExtent.y);
                _maxExtentsYValue.SetInitialValue(max3DExtent.y);
                _minExtentsZValue.SetInitialValue(min3DExtent.z);
                _maxExtentsZValue.SetInitialValue(max3DExtent.z);
                extentsSet = true;
                updated = true;
            }
            else
            {
                float newChangeX = value.x - _lastInputVector3.x;
                float newChangeY = value.y - _lastInputVector3.y;
                float newChangeZ = value.z - _lastInputVector3.z;
                if(_lastChangeX <= -changeTolarence && newChangeX > -changeTolarence )
                {
                    _minExtentsXValue.UpdateBeliefVector(value.x);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeX > changeTolarence && newChangeX <= changeTolarence )
                {
                    _minExtentsXValue.UpdateBeliefVector(value.x);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }
                if(_lastChangeY <= -changeTolarence && newChangeY > -changeTolarence )
                {
                    _minExtentsYValue.UpdateBeliefVector(value.y);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeY > changeTolarence && newChangeY <= changeTolarence )
                {
                    _minExtentsYValue.UpdateBeliefVector(value.y);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }
                if(_lastChangeZ <= -changeTolarence && newChangeZ > -changeTolarence )
                {
                    _minExtentsZValue.UpdateBeliefVector(value.z);
                    minExtent = _minExtentsXValue.SuggestLevel;
                }
                else if(_lastChangeZ > changeTolarence && newChangeZ <= changeTolarence )
                {
                    _minExtentsZValue.UpdateBeliefVector(value.z);
                    maxExtent = _maxExtentsXValue.SuggestLevel;
                }

                _lastChangeX = newChangeX;
                _lastChangeY = newChangeY;
                _lastChangeZ = newChangeZ;
                _lastInputVector3 = value;
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