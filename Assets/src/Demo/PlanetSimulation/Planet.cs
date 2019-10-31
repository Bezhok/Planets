using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace src.Demo.PlanetSimulation
{
    public class Planet
    {
        private GameObject _sphere;
        public static readonly double G = 6.67E-11;
        public static readonly float OnePoint = 84400000;
        private bool _shouldDrawLine;
        private float _radius;

        public double Mass { get; set; }
        public Vector3 Speed { get; set; }
        public Vector3 Pos { get; set; }
        public GameObject gameObject;

        public bool ShouldDestroy { get; set; } = false;
        public bool IsDestroyable { get; set; } = true;
        public bool IsReturning { get; set; } = false;

        public Planet(double mass, float x, float y, bool shouldDrawLine = false)
        {
            _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _sphere.GetComponent<SphereCollider>().isTrigger = true;
            _sphere.AddComponent<PlanetCollision>();
            _sphere.AddComponent<Rigidbody>();
            _sphere.GetComponent<Rigidbody>().useGravity = false;
            _sphere.GetComponent<PlanetCollision>().Planet = this;
            
            _shouldDrawLine = shouldDrawLine;
            Mass = mass;

            UpdateRadius();
            
            Pos = new Vector3(x, y);
            _sphere.transform.position = Pos/OnePoint;
        }

        
        public void UpdateRadius()
        {
            int massPower = 0;
            double tempMass = Mass;
            while (tempMass > 1)
            {
                massPower++;
                tempMass /= 10;
            }
            
            _radius = 173700*massPower;
            
            //fake size
            _sphere.transform.localScale = new Vector3(_radius, _radius, _radius)/OnePoint*30;
        }

        public bool IsInCameraView()
        {
            if (_sphere.gameObject == null)
            {
                return true;
            }
            
            var position = _sphere.transform.position;

            var camPos = Camera.main.transform.position;
            var vertExtent = Camera.main.orthographicSize;    
            var horzExtent = vertExtent * Screen.width / Screen.height;
            return (Mathf.Abs(camPos.y - position.y) < vertExtent && Mathf.Abs(camPos.x - position.x) < horzExtent);
        }
        
        private Planet() { }

        private List<Vector3> _positions;
        private LineRenderer _lineRenderer;

        public void Update()
        {
            if (_sphere.gameObject != null)
            {
                _sphere.transform.position = Pos / OnePoint;

                if (_shouldDrawLine)
                {
                    if (gameObject == null)
                    {
                        gameObject = new GameObject();
                        _lineRenderer = gameObject.AddComponent<LineRenderer>();
                        _positions = new List<Vector3>();
                        _lineRenderer.endColor = Color.red;
                        _lineRenderer.startColor = new Color(1, 0, 0, 0);
                        _lineRenderer.endWidth = _lineRenderer.startWidth = 0.2f;
                    }

                    _positions.Add(_sphere.transform.position);

                    _lineRenderer.positionCount = _positions.Count;
                    for (int i = 0; i < _positions.Count; i++)
                    {
                        _lineRenderer.SetPosition(i, _positions[i]);
                    }
                }
            }
        }
        
        public void Disable()
        {
            _sphere.SetActive(false);
        }
        
        public void Enable()
        {
            _sphere.SetActive(true);
        }
    }
}
