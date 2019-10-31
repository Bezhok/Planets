using UnityEngine;

namespace src.Demo.BallSimulation
{
    public class Ball
    {
        private GameObject _sphere;
        public Vector3 Speed { get; set; }
        public Vector3 Pos { get; set; }

        public Ball(Vector2 speed, Vector2 pos)
        {
            Speed = new Vector3(speed.x, speed.y, 0);
            Pos = new Vector3(pos.x, pos.y, 0);;
            
            _sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _sphere.GetComponent<SphereCollider>().isTrigger = true;
        }

        public void Update()
        {
            _sphere.transform.position = Pos;
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