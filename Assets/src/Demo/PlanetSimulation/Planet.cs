using System.Collections.Generic;
using UnityEngine;

namespace src.Demo.PlanetSimulation
{
    public class Planet
    {
        public static readonly double G = 6.67E-11;
        public static readonly float OnePoint = 84400000;
        private static readonly Material DefaultPlanetMaterial = Resources.Load<Material>("Materials/Planet");
        private static readonly Material DefaultLineMaterial = Resources.Load<Material>("Materials/Line");
        private readonly LineRenderer _lineRenderer;
        private readonly LinkedList<Vector3> _positions;
        private readonly bool _shouldDrawLine;
        private float _radius;

        public Planet(double mass, float x, float y, Material mat = null, bool shouldDrawLine = false)
        {
            if (mat == null) mat = DefaultPlanetMaterial;

            GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.GetComponent<SphereCollider>().isTrigger = true;
            GameObject.AddComponent<PlanetCollision>();
            GameObject.AddComponent<Rigidbody>();
            GameObject.GetComponent<Rigidbody>().useGravity = false;
            GameObject.GetComponent<PlanetCollision>().Planet = this;
            GameObject.GetComponent<MeshRenderer>().material = mat;

            _shouldDrawLine = shouldDrawLine;
            Mass = mass;

            UpdateRadius();

            Pos = new Vector3(x, y);
            GameObject.transform.position = Pos / OnePoint;


            _lineRenderer = GameObject.AddComponent<LineRenderer>();
            _positions = new LinkedList<Vector3>();
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
            Material whiteDiffuseMat = DefaultLineMaterial;
            _lineRenderer.material = whiteDiffuseMat;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.startWidth = 0.05f;
        }


        public GameObject GameObject { get; }

        public double Mass { get; set; }
        public Vector3 Speed { get; set; }
        public Vector3 Pos { get; set; }
        public bool ShouldDestroy { get; set; } = false;
        public bool IsDestroyable { get; set; } = true;

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
            GameObject.transform.localScale = new Vector3(_radius, _radius, _radius) / OnePoint * 30;
        }

        public bool IsInCameraView()
        {
            if (GameObject.gameObject == null) return true;

            Vector3 position = GameObject.transform.position;

            Vector3 camPos = Camera.main.transform.position;
            float vertExtent = Camera.main.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;
            return Mathf.Abs(camPos.y - position.y) < vertExtent && Mathf.Abs(camPos.x - position.x) < horzExtent;
        }

        public void Update()
        {
            if (GameObject.gameObject != null)
            {
                GameObject.transform.position = Pos / OnePoint;

                if (_shouldDrawLine)
                {
                    _positions.AddLast(GameObject.transform.position);

                    if (_positions.Count >= 200) _positions.Remove(_positions.First);

                    _lineRenderer.positionCount = _positions.Count;

                    int i = 0;
                    foreach (Vector3 position in _positions)
                    {
                        _lineRenderer.SetPosition(i, position);
                        ++i;
                    }
                }
            }
        }

        public void Disable()
        {
            GameObject.SetActive(false);
        }

        public void Enable()
        {
            GameObject.SetActive(true);
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }
    }
}