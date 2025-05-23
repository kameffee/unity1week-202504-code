using R3;
using R3.Triggers;
using Unity1week202504.Audio.System;
using UnityEngine;
using UnityEngine.UI;

namespace Unity1week202504.Audio.Settings
{
    public class SeSettingView : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private AudioClip _sampleClip;

        public AudioClip SampleClip => _sampleClip;

        public void SetVolume(AudioVolume volume) => _slider.value = volume.Value;

        public Observable<float> OnChangeVolumeAsObservable() => _slider.OnValueChangedAsObservable();

        public Observable<Unit> OnPointerUpAsObservable() => _slider.OnPointerUpAsObservable().AsUnitObservable();
    }
}