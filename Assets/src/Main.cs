using System.Collections.Generic;
using src.Demo;
using src.Demo.BallSimulation;
using src.Demo.PlanetSimulation;
using UnityEngine;

namespace src
{
    public class Main : MonoBehaviour
    {
        private DemoType _currDemo = DemoType.Planets;
        private Dictionary<DemoType, IDemo> _demos;

        private void Start()
        {
            _demos = new Dictionary<DemoType, IDemo>();

            _demos[DemoType.Ball] = new BallSimulation();
            _demos[DemoType.Ball].Start();
            _demos[DemoType.Ball].Disable();

            _demos[DemoType.Planets] = new PlanetSimulation();
            _demos[DemoType.Planets].Start();
        }

        private void Update()
        {
            SwitchDemo();
        }

        private void FixedUpdate()
        {
            _demos[_currDemo].Update(Time.fixedDeltaTime);
        }

        private void SwitchDemo()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _demos[_currDemo].Disable();
                _currDemo = _currDemo == DemoType.Ball ? DemoType.Planets : DemoType.Ball;
                _demos[_currDemo].Enable();
            }
        }

        private enum DemoType
        {
            Planets,
            Ball
        }
    }
}