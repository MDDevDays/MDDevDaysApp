using MDDevDaysApp.DomainModel;
using MDDevDaysApp.Infrastructure;
using MDDevDaysApp.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;

namespace MDDevDaysApp
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("MainNavigationPage/MainTabbedPage");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainNavigationPage>();
            Container.RegisterTypeForNavigation<MainTabbedPage>();
            Container.RegisterTypeForNavigation<InfoPage>();
            Container.RegisterTypeForNavigation<SpeakerPage>();
            Container.RegisterTypeForNavigation<ProgramPage>();

            Container.RegisterType(typeof(ISpeakers), typeof(Speakers), string.Empty, new ContainerControlledLifetimeManager());
        }
    }
}
