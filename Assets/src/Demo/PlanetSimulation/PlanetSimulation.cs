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
            foreach (var e in _planets)
                if (!e.Value.IsInCameraView())
                    keys.Add(e.Key);

            foreach (var key in keys) _planets.Remove(key);
        }

        public void Disable()
        {
            foreach (var key in _planets) key.Value.Disable();
        }

        public void Enable()
        {
            foreach (var key in _planets) key.Value.Enable();
        }

        public void Update(float dTime)
        {
            dTime *= 86400 * 5;
            SystemMove(dTime);

            for (var i = 0; i < _planets.Count; i++)
            {
                var speedSum = Vector3.zero;

                var first = _planets.ElementAt(i).Value;
                for (var j = i + 1; j < _planets.Count; j++)
                {
                    var second = _planets.ElementAt(j).Value;
                    var dist = Vector3.Distance(first.Pos, second.Pos);

                    var fBase = Planet.G / (dist * dist * dist);
                    var sub = second.Pos - first.Pos;
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
            foreach (var e in _planets)
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

            foreach (var key in keys) _planets.Remove(key);
        }

        private void CreatePlanetOnMouse()
        {
            var rand = new Random();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var pz = _camera.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;
                pz *= Planet.OnePoint;

                ++_lastPlanetIdx;
                var massRand = rand.Next(10, 23);
                var key = "Planet" + _lastPlanetIdx;
                _planets[key] = new Planet(Math.Pow(10, massRand), pz.x, pz.y, null, true);

                var vec = _planets["star"].Pos - pz;
                vec.Normalize();
                var scale = rand.Next(500, 1000);
                _planets[key].Speed += new Vector3(-vec.y * scale, vec.x * scale);
            }
        }
    }
}