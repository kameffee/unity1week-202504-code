using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unity1week202504.InGame.Memories
{
    public static class UserInput
    {
        public static UniTask WaitForAny(CancellationToken cancellationToken = default)
            => UniTask.WaitUntil(IsAny, cancellationToken: cancellationToken);

        public static bool IsAny() => IsAnyKeyDown() || IsMouseButtonDown();
        public static bool IsAnyKeyDown() => Input.anyKeyDown;
        public static bool IsMouseButtonDown() => Input.GetMouseButtonDown(0);
    }
}