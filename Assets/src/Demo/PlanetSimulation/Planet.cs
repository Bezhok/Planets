using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace src.Demo.PlanetSimulation
{
    public class Planet
    {
        public static readonly double G = 6.67E-11;
        public static readonly float OnePoint = 84400000;
        private GameObject _gameObject;
        private bool _shouldDrawLine;
        private float _radius;
        private LinkedList<Vector3> _positions;
        private LineRenderer _lineRenderer;
        private static readonly Material DefaultPlanetMaterial = Resources.Load<Material>("Materials/Planet");
        private static readonly Material DefaultLineMaterial = Resources.Load<Material>("Materials/Line");
        
        
        public GameObject GameObject => _gameObject;
        public double Mass { get; set; }
        public Vector3 Speed { get; set; }
        public Vector3 Pos { get; set; }
        public bool ShouldDestroy { get; set; } = false;
        public bool IsDestroyable { get; set; } = true;

        public Planet(double mass, float x, float y, Material mat = null, bool shouldDrawLine = false)
        {
            if (mat == null)
            {
                mat = DefaultPlanetMaterial;
            }
            
            _gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _gameObject.GetComponent<SphereCollider>().isTrigger = true;
            _gameObject.AddComponent<PlanetCollision>();
            _gameObject.AddComponent<Rigidbody>();
            _gameObject.GetComponent<Rigidbody>().useGravity = false;
            _gameObject.GetComponent<PlanetCollision>().Planet = this;
            _gameObject.GetComponent<MeshRenderer>().material = mat;

            _shouldDrawLine = shouldDrawLine;
            Mass = mass;

            UpdateRadius();

            Pos = new Vector3(x, y);
            _gameObject.transform.position = Pos / OnePoint;
            
            
            _lineRenderer = _gameObject.AddComponent<LineRenderer>();
            _positions = new LinkedList<Vector3>();
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
            Material whiteDiffuseMat = DefaultLineMaterial;
            _lineRenderer.material = whiteDiffuseMat;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.startWidth = 0.05f;
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

            _radius = 173700 * massPower;

            //fake size
            _gameObject.transform.localScale = new Vector3(_radius, _radius, _radius) / OnePoint * 30;
        }
        
        public bool IsInCameraView()
        {
            if (_gameObject.gameObject == null)
            {
                return true;
            }

            var position = _gameObject.transform.position;

            var camPos = Camera.main.transform.position;
            var vertExtent = Camera.main.orthographicSize;
            var horzExtent = vertExtent * Screen.width / Screen.height;
            return (Mathf.Abs(camPos.y - position.y) < vertExtent && Mathf.Abs(camPos.x - position.x) < horzExtent);
        }
        
        public void Update()
        {
            if (_gameObject.gameObject != null)
            {
                _gameObject.transform.position = Pos / OnePoint;

                if (_shouldDrawLine)
                {
                    _positions.AddLast(_gameObject.transform.position);

                    if (_positions.Count >= 200)
                    {
                        _positions.Remove(_positions.First);
                    }
                    
                    _lineRenderer.positionCount = _positions.Count;

                    int i = 0;
                    foreach (var position in _positions)
                    {
                        _lineRenderer.SetPosition(i, position);
                        ++i;
                    }
                }
            }
        }

        public void Disable()
        {
            _gameObject.SetActive(false);
        }

        public void Enable()
        {
            _gameObject.SetActive(true);
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(_gameObject);
        }
    }
}