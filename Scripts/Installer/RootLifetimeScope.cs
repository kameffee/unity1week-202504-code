using Alchemy.Inspector;
using Unity1week202504.Audio;
using Unity1week202504.Audio.Player;
using Unity1week202504.Audio.Settings;
using Unity1week202504.Data;
using Unity1week202504.Data.Memories;
using Unity1week202504.InGame.Memories;
using Unity1week202504.InGame.Photo;
using Unity1week202504.Scenes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.Installer
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private SceneTransitionView _sceneTransitionView;

        [Title("DataSource")]
        [SerializeField]
        private PhotoMasterDataSource _photoMasterDataSource;

        [SerializeField]
        private MemoryConditionMasterDataSource _memoryConditionMasterDataSource;

        [SerializeField]
        private MemoryMasterDataSource _memoryMasterDataSource;

        [SerializeField]
        private EndingMasterDataSource _endingMasterDataSource;

        [SerializeField]
        private MessageBlockMasterDataSource _messageBlockMasterDataSource;

        [Title("Settings")]
        [SerializeField]
        private MemorySettings _memorySettings;
        
        [SerializeField]
        private AudioDatabase _audioDatabase;

        [Title("デバッグ")]
        [SerializeField]
        private DebugSettings _debugSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureSceneLoader(builder);
            ConfigureAudioSystem(builder);

            builder.RegisterInstance(_photoMasterDataSource);
            builder.RegisterInstance(_memoryConditionMasterDataSource);
            builder.RegisterInstance(_memoryMasterDataSource);
            builder.RegisterInstance(_endingMasterDataSource);
            builder.RegisterInstance(_messageBlockMasterDataSource);
            builder.RegisterInstance(_audioDatabase);

            builder.Register<PhotoRepository>(Lifetime.Singleton);
            builder.Register<MemoryRepository>(Lifetime.Singleton);
            builder.Register<MemoryGenerateHistoryRepository>(Lifetime.Singleton);
            builder.Register<GameResetter>(Lifetime.Singleton);

            builder.RegisterInstance(_memorySettings);
            builder.RegisterInstance<DebugSettings>(_debugSettings);

            builder.Register<GameInitializer>(Lifetime.Singleton);
            
            builder.RegisterBuildCallback(resolver =>
            {
                // 初期化処理
                var gameInitializer = resolver.Resolve<GameInitializer>();
                gameInitializer.Initialize();
            });
        }

        private static void ConfigureAudioSystem(IContainerBuilder builder)
        {
            builder.Register<AudioPlayer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponentOnNewGameObject<SePlayer>(Lifetime.Singleton).DontDestroyOnLoad();
            builder.RegisterComponentOnNewGameObject<BgmPlayer>(Lifetime.Singleton).DontDestroyOnLoad();

            // Settings
            builder.Register<AudioSettingsService>(Lifetime.Singleton);
        }

        private void ConfigureSceneLoader(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_sceneTransitionView, Lifetime.Singleton)
                .DontDestroyOnLoad()
                .As<ISceneTransitionView>();
        }
    }
}