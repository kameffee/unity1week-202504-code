using R3;
using Unity1week202504.Audio.System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Audio.Settings
{
    public class BgmSettingView : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        public void SetVolume(AudioVolume volume) => _slider.value = volume.Value;

        public Observable<float> OnChangeVolumeAsObservable() => _slider.OnValueChangedAsObservable();
    }
}