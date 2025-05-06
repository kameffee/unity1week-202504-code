using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unity1week202504.License
{
    public class LicenseTextProvider
    {
        private readonly TextAsset _textAsset;

        public LicenseTextProvider(TextAsset textAsset)
        {
            _textAsset = textAsset;
        }

        public UniTask<string> GetAsync()
        {
            return new UniTask<string>(_textAsset.text);
        }
    }
}