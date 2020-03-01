using UnityEngine;

namespace src.Demo.BallSimulation
{
    public class BallSimulation : IDemo
    {
        private readonly GameObject _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        private Ball _ball;
        private GameObject _prefabPhysicBall;

        public void Start()
        {
            _floor.transform.position = new Vector3(0, -6);
            _ball = new Ball(new Vector2(0, 2), new Vector2(0, 5));

            _prefabPhysicBall =
                Object.Instantiate(Resources.Load("Prefabs/PlatformSimulation", typeof(GameObject))) as GameObject;
        }

        public void Update(float dTime)
        {
            var acceleration = new Vector3(0, -9.81f);
            _ball.Pos += dTime * _ball.Speed;
            _ball.Speed += dTime * acceleration;

            if (_ball.Pos.y < -5)
            {
                _ball.Speed = new Vector3(_ball.Speed.x, -_ball.Speed.y);

                _ball.Pos = new Vector3(_ball.Pos.x, -4.99f);
            }

            _ball.Update();
        }

        public void Disable()
        {
            _ball.Disable();
            _floor.SetActive(false);

            _prefabPhysicBall.SetActive(false);
        }

        public void Enable()
        {
            _ball.Enable();
            _floor.SetActive(true);

            _prefabPhysicBall.SetActive(true);
        }
    }
}