using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace src.Demo.PlanetSimulation
{
    public class PlanetSimulation : IDemo
    {
        private readonly Dictionary<string, Planet> _planets = new Dictionary<string, Planet>();
        private Camera _camera;
        private int _lastPlanetIdx;

        public void Start()
        {
            _camera = Camera.main;

            _planets["star"] = new Planet(5.9722E+24, 0, 0, Resources.Load<Material>("Materials/Star"));
            _planets["star"].IsDestroyable = false;
            _planets["planet"] = new Planet(7.3477E+22, 0, 384400000, null, true);
            _planets["planet"].Speed += new Vector3(1000f, 0);

            var keys = new List<string>();
            foreach (KeyValuePair<string, Planet> e in _planets)
                if (!e.Value.IsInCameraView())
                    keys.Add(e.Key);

            foreach (string key in keys) _planets.Remove(key);
        }

        public void Disable()
        {
            foreach (KeyValuePair<string, Planet> key in _planets) key.Value.Disable();
        }

        public void Enable()
        {
            foreach (KeyValuePair<string, Planet> key in _planets) key.Value.Enable();
        }

        public void Update(float dTime)
        {
            dTime *= 86400 * 5;
            SystemMove(dTime);

            for (int i = 0; i < _planets.Count; i++)
            {
                Vector3 speedSum = Vector3.zero;

                Planet first = _planets.ElementAt(i).Value;
                for (int j = i + 1; j < _planets.Count; j++)
                {
                    Planet second = _planets.ElementAt(j).Value;
                    float dist = Vector3.Distance(first.Pos, second.Pos);

                    double fBase = Planet.G / (dist * dist * dist);
                    Vector3 sub = second.Pos - first.Pos;
                    speedSum += (float) (second.Mass * fBase) * sub;

                    second.Speed += dTime * (float) (first.Mass * fBase) * -sub;
                }

                first.Speed += dTime * speedSum;
            }

            CreatePlanetOnMouse();
        }

        private void SystemMove(float dTime)
        {
            var keys = new List<string>();
            foreach (KeyValuePair<string, Planet> e in _planets)
            {
                e.Value.Update();

                if (e.Key != "star")
                    e.Value.Pos += e.Value.Speed * dTime;

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

            foreach (string key in keys) _planets.Remove(key);
        }

        private void CreatePlanetOnMouse()
        {
            var rand = new Random();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 pz = _camera.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;
                pz *= Planet.OnePoint;

                ++_lastPlanetIdx;
                int massRand = rand.Next(10, 23);
                string key = "Planet" + _lastPlanetIdx;
                _planets[key] = new Planet(Math.Pow(10, massRand), pz.x, pz.y, null, true);

                Vector3 vec = _planets["star"].Pos - pz;
                vec.Normalize();
                int scale = rand.Next(500, 1000);
                _planets[key].Speed += new Vector3(-vec.y * scale, vec.x * scale);
            }
        }
    }
}