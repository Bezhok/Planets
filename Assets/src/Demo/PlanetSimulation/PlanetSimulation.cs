using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace src.Demo.PlanetSimulation
{
    public class PlanetSimulation : IDemo
    {
        private Planet _earth;
        private Planet _moon;
        private Planet _smallMoon;

        private Dictionary<string, Planet> _planets = new Dictionary<string, Planet>();
        private int i = 2;
        public void Start()
        {
            _planets["earth"] = new Planet(5.9722E+24, 0, 0);
            _planets["earth"].IsDestroyable = false;
            _planets["moon"] = new Planet(7.3477E+22, 0, 384400000);
            _planets["moon"].Speed += new Vector3(1000f, 0);
            
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
            foreach (var e in _planets)
            {
                e.Value.Update();
                
                if (e.Key != "earth")
                    e.Value.Pos += e.Value.Speed * dtime;
                
                if (!e.Value.IsInCameraView() )
                {
                    if (!e.Value.IsReturning)
                    {
                        e.Value.IsReturning = true;
                        e.Value.Speed = new Vector3(-e.Value.Speed.y, e.Value.Speed.x)/10;
                    }
                }
                else
                {
                    e.Value.IsReturning = false;
                }
            }
            
            List<string> keys = new List<string>();
            foreach (var e in _planets)
            {
                if (e.Value.ShouldDestroy)
                {
                    keys.Add(e.Key);
                }
            }

            foreach (var key in keys)
            {
                _planets.Remove(key);
            }
        }
        public void Update()
        {
            float dtime = Time.deltaTime*86400*5;
            SystemMove(dtime);
            
            Vector3 speedSum = new Vector3();
            for (int i = 0; i < _planets.Count; i++)
            {
                speedSum = Vector3.zero;
                
                var first = _planets.ElementAt(i).Value;
                for (int j = i+1; j < _planets.Count; j++)
                {
                    var second = _planets.ElementAt(j).Value;
                    float dist = Vector3.Distance(first.Pos, second.Pos);
                    
                    double fBase = Planet.G /(dist * dist * dist);
                    Vector3 sub = second.Pos - first.Pos;
                    speedSum += (float)(second.Mass * fBase)*sub;
                    
                    second.Speed += dtime*(float)(first.Mass * fBase)*(-sub);
                }
                first.Speed += dtime * speedSum;
            }

            var rand = new System.Random();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;
                pz *= Planet.OnePoint;
                
                ++i;
                int massRand = rand.Next(10, 23);
                _planets[i.ToString()] = new Planet(Math.Pow(10, massRand), pz.x, pz.y);

                var vec = _planets["earth"].Pos - pz;
                vec.Normalize();
                int scaller = rand.Next(500, 1000);
                _planets[i.ToString()].Speed += new Vector3(-vec.y*scaller, vec.x*scaller);
            }
        }
    }
}