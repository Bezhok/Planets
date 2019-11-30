using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace src.Demo.PlanetSimulation
{
    public class PlanetSimulation : IDemo
    {
        private readonly Dictionary<string, Planet> _planets = new Dictionary<string, Planet>();
        private int _lastPlanetIdx;

        public void Start()
        {
            _planets["star"] = new Planet(5.9722E+24, 0, 0, Resources.Load<Material>("Materials/Star"));
            _planets["star"].IsDestroyable = false;
            _planets["planet"] = new Planet(7.3477E+22, 0, 384400000, null, true);
            _planets["planet"].Speed += new Vector3(1000f, 0);

            List<string> keys = new List<string>();
            foreach (var e in _planets)
            {
                if (!e.Value.IsInCameraView())
                {
                    keys.Add(e.Key);
                }
            }

            foreach (var key in keys)
            {
                _planets.Remove(key);
            }
        }

        public void Disable()
        {
            foreach (var key in _planets)
            {
                key.Value.Disable();
            }
        }

        public void Enable()
        {
            foreach (var key in _planets)
            {
                key.Value.Enable();
            }
        }

        private void SystemMove(float dtime)
        {
            List<string> keys = new List<string>();
            foreach (var e in _planets)
            {
                e.Value.Update();

                if (e.Key != "star")
                    e.Value.Pos += e.Value.Speed * dtime;

                if (!e.Value.IsInCameraView())
                {
                    keys.Add(e.Key);
                    e.Value.Destroy();
                }
                else if (e.Value.ShouldDestroy)
                {
                    keys.Add(e.Key);
                }
            }

            foreach (var key in keys)
            {
                _planets.Remove(key);
            }
        }

        public void Update(float dtime)
        {
            dtime *= 86400 * 5;
            SystemMove(dtime);

            for (int i = 0; i < _planets.Count; i++)
            {
                var speedSum = Vector3.zero;

                var first = _planets.ElementAt(i).Value;
                for (int j = i + 1; j < _planets.Count; j++)
                {
                    var second = _planets.ElementAt(j).Value;
                    float dist = Vector3.Distance(first.Pos, second.Pos);

                    double fBase = Planet.G / (dist * dist * dist);
                    Vector3 sub = second.Pos - first.Pos;
                    speedSum += (float) (second.Mass * fBase) * sub;

                    second.Speed += dtime * (float) (first.Mass * fBase) * (-sub);
                }

                first.Speed += dtime * speedSum;
            }

            CreatePlanetOnMouse();
        }

        private void CreatePlanetOnMouse()
        {
            var rand = new System.Random();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;
                pz *= Planet.OnePoint;

                ++_lastPlanetIdx;
                int massRand = rand.Next(10, 23);
                string key = "Planet" + _lastPlanetIdx;
                _planets[key] = new Planet(Math.Pow(10, massRand), pz.x, pz.y, null, true);

                var vec = _planets["star"].Pos - pz;
                vec.Normalize();
                int scaller = rand.Next(500, 1000);
                _planets[key].Speed += new Vector3(-vec.y * scaller, vec.x * scaller);
            }
        }
    }
}