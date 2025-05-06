using Alchemy.Inspector;
using Unity1week202504.Data;
using Unity1week202504.InGame;
using Unity1week202504.InGame.Cameras;
using Unity1week202504.InGame.Focus;
using Unity1week202504.InGame.Memories;
using Unity1week202504.Message;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.Installer
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [AssetsOnly]
        [SerializeField]
        private MemoryFactoryCanvas _memoryFactoryCanvasPrefab;

        [AssetsOnly]
        [SerializeField]
        private MessageWindow _messageWindowPrefab;

        [AssetsOnly]
        [SerializeField]
        private SubTitleMessageWindow _subTitleMessageWindowPrefab;

        [AssetsOnly]
        [SerializeField]
        private MemoryAfterPerformerView _memoryAfterPerformerViewPrefab;

        [AssetsOnly]
        [SerializeField]
        private MemoryViewingCanvas _memoryViewingCanvasPrefab;

        [AssetsOnly]
        [SerializeField]
        private IntroSequence _introSequencePrefab;

        [FormerlySerializedAs("_focusViewPrefab")]
        [SerializeField]
        private FocusFrameView _focusFrameViewPrefab;

        [SerializeField]
        private InGameSettings _inGameSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            // GameLoop
            builder.RegisterEntryPoint<InGameLoop>();
            builder.RegisterComponentInNewPrefab(_introSequencePrefab, Lifetime.Singleton);
            builder.Register<TakePhotoPhase>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<MemoryGeneratePhase>(Lifetime.Singleton).AsSelf();
            builder.Register<MemoryConverter>(Lifetime.Singleton);

            // MemoryFactoryCanvas
            builder.RegisterComponentInNewPrefab<MemoryFactoryCanvas>(_memoryFactoryCanvasPrefab, Lifetime.Singleton);
            builder.Register<MemoryFactoryViewModelFactory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MemoryGenerateCanvasPresenter>();

            // MessageWindow
            builder.RegisterComponentInNewPrefab<MessageWindow>(_messageWindowPrefab, Lifetime.Singleton);
            builder.Register<MessagePlayer>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_subTitleMessageWindowPrefab, Lifetime.Singleton);
            builder.Register<SubTitleMessagePlayer>(Lifetime.Singleton);

            // MemoryAfterPerformer
            builder.Register<MemoryAfterPerformer>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab<MemoryAfterPerformerView>(_memoryAfterPerformerViewPrefab,
                Lifetime.Singleton);

            // MemoryViewingCanvas
            builder.RegisterComponentInNewPrefab<MemoryViewingCanvas>(_memoryViewingCanvasPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<MemoryViewingCanvasPresenter>().AsSelf();

            // SnapTargetObject
            builder.Register<SnapTargetActiveSwitcher>(Lifetime.Singleton);

            // Camera
            builder.RegisterComponentInHierarchy<InGameCamera>();
            builder.RegisterEntryPoint<CameraMoveControlPresenter>();
            builder.RegisterComponentInHierarchy<CameraMoveControlView>();

            // Settings
            builder.RegisterInstance(_inGameSettings);

            builder.RegisterComponentInNewPrefab(_focusFrameViewPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<FocusPresenter>().AsSelf();

            builder.RegisterBuildCallback(resolver =>
            {
                var findObjects = FindObjectsByType<SnapTargetObject>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
                foreach (var snapTargetObject in findObjects)
                {
                    resolver.Inject(snapTargetObject);
                }
            });
        }
    }
}