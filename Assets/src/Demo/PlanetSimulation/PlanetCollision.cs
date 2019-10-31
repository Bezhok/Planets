using System;
using UnityEngine;

namespace src.Demo.PlanetSimulation
{
    public class PlanetCollision : MonoBehaviour
    {
        private Planet _planet;

        public Planet Planet
        {
            get => _planet;
            set => _planet = value;
        }

        void OnTriggerEnter(Collider other)
        {
            Planet otherPlanet;
            try
            {
                otherPlanet = other.gameObject.GetComponent<PlanetCollision>().Planet;
            }
            catch (NullReferenceException)
            {
                return;
            }

            if (otherPlanet.IsDestroyable && !Planet.IsDestroyable)
            {
                otherPlanet.ShouldDestroy = true;
                Planet.Mass += otherPlanet.Mass;
                Destroy(other.gameObject);
            }
        }
    }
}