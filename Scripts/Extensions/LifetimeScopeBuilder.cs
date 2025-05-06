using UnityEngine;
using VContainer;

namespace Unity1week202504.Extensions
{
    public abstract class LifetimeScopeBuilder : MonoBehaviour
    {
        public abstract void Configure(IContainerBuilder builder);
    }
}