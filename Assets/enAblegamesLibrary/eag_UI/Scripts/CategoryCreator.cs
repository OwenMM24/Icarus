using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Enablegames
{

    public class CategoryCreator : MonoBehaviour
    {
        public string category;
        
        [SerializeField] private string parameterKey;

        public string ParameterKey
        {
            get { return parameterKey; }
        }

        [SerializeField] private GameObject BoolWidgetPrefab;
        [SerializeField] private GameObject RangeWidgetPrefab;
        [SerializeField] private GameObject StringListPrefab;
        [SerializeField] private GameObject CategoryPrefab;
        [SerializeField] private GameObject scrollViewRoot;
        
        private List<ParameterWidget> Widgets;
        private GameParameters paramInst;
        private bool setUpComplete = false;
        
        public delegate void GameParameterHandler(bool state);
        public static event GameParameterHandler SetUpComplete;

        public void Setup(string name)
        {
            if (setUpComplete == true)
            {
                return;
            }

            if (paramInst == null)
            {
                ParameterHandler.Instance.AddParameters(parameterKey /*, true*/);
                paramInst = ParameterHandler.Instance.GetParameters(parameterKey);
            }

            //Game.ActiveParameters = paramInst;
            Widgets = new List<ParameterWidget>();

            setUpComplete = true;
            RaiseOnSetupComplete(true);
            foreach (GameParameter p in paramInst.Categories[category])
            {
                SetupWidget(p);
            }
        }
        void RaiseOnSetupComplete(bool state)
        {
            if (SetUpComplete != null)
            {
                SetUpComplete(state);
            }
        }

        void SetupWidget(GameParameter parameter)
        {
            ParameterWidget widget = null;
            GameObject toAdd = RangeWidgetPrefab;
            if (parameter.GetType() == typeof(BoolParameter))
            {
                toAdd = BoolWidgetPrefab;
            }

            if (parameter.GetType() == typeof(StringListParameter))
            {
                toAdd = StringListPrefab;
            }

            print("SetupWidget: " + parameter.Name);
            widget = ((GameObject)Instantiate(toAdd, scrollViewRoot.transform)).GetComponent<ParameterWidget>();
            widget.name = parameter.Name + "-Widget";
            Widgets.Add(widget);
            widget.Setup(parameter);
            //PositionWidget(widget);
        }

    }
}
