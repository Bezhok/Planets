using System;
using System.Collections.Generic;
using src.Demo;
using src.Demo.BallSimulation;
using src.Demo.PlanetSimulation;
using UnityEngine;

namespace src
{
    public class Main : MonoBehaviour
    {
        enum DemoType
        {
            Planets,
            Ball
        }

        private DemoType[] _demoTypeVals;
        private DemoType _demoType = DemoType.Planets;

        private Dictionary<DemoType, IDemo> _demos;
        void Start()
        {
            _demoTypeVals = Enum.GetValues(typeof(DemoType)) as DemoType[];
        
            _demos = new Dictionary<DemoType, IDemo>();
            _demos.Add(DemoType.Planets, new PlanetSimulation());
            _demos[DemoType.Planets].Start();

            _demos[DemoType.Ball] = new BallSimulation();
            _demos[DemoType.Ball].Start();
            _demos[DemoType.Ball].Disable();
        }

        void Update()
        {
            DetDemo();
            _demos[_demoType].Update();
        }

        void DetDemo()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _demos[_demoType].Disable();
                int index = Next(_demoTypeVals.Length);
                _demoType = _demoTypeVals[index];
                _demos[_demoType].Enable();
            }
        }

        private int _curr = 0;
        int Next(int max)
        {
            ++_curr;
            if (_curr == max)
                _curr = 0;
        
            return _curr;
        }
    }
}
