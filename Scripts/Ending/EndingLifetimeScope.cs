using Alchemy.Inspector;
using Unity1week202504.Message;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Unity1week202504.Ending
{
    public class EndingLifetimeScope : LifetimeScope
    {
        [AssetsOnly]
        [SerializeField]
        private MemorySelectView _memorySelectViewPrefab;

        [SerializeField]
        private MessageWindow _messageWindowPrefab;

        [SerializeField]
        private SubTitleMessageWindow _subTitleMessageWindowPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EndingPresenter>();
            builder.RegisterComponentInHierarchy<EndingView>();
            builder.RegisterEntryPoint<MemorySelectPhase>().AsSelf();
            builder.RegisterComponentInNewPrefab(_memorySelectViewPrefab, Lifetime.Singleton);
            builder.Register<EndingCalculator>(Lifetime.Singleton);

            // Message
            builder.Register<MessagePlayer>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_messageWindowPrefab, Lifetime.Singleton);

            // SubTitle
            builder.Register<SubTitleMessagePlayer>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_subTitleMessageWindowPrefab, Lifetime.Singleton);
        }
    }
}