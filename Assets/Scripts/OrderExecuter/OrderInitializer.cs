using System.Linq;
using UnityEngine;

namespace OrderExecuter
{
    public class OrderInitializer: MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] startableBehaviours;

        private void Start()
        {
            foreach (var behaviour in startableBehaviours.OfType<IStartable>())
                behaviour.OnStart();
        }
    }
}