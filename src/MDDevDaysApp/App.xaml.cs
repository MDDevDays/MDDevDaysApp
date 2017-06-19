using System.Threading.Tasks;
using MDDevDaysApp.DomainModel;
using MDDevDaysApp.Infrastructure;
using MDDevDaysApp.Views;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
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

        protected override async void OnStart()
        {
            MobileCenter.Start(
                "ios=db0d11d6-b519-413e-8d16-a35d483bbbcd;android=24b1330b-8cb9-412a-9f28-7b296891a680;uwp=089515cd-4389-40e8-9e49-9ba0ff033c52", 
                typeof(Analytics), 
                typeof(Crashes));

            await Container.Resolve<ITimeslots>().EnsureTimeslotsAreLoaded();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainNavigationPage>();
            Container.RegisterTypeForNavigation<MainTabbedPage>();
            Container.RegisterTypeForNavigation<SpeakerPage>();
            Container.RegisterTypeForNavigation<Views.TimeslotPage>();

            Container.RegisterType<InfoPage>();
            Container.RegisterType<SpeakersPage>();
            Container.RegisterType<TimeslotsPage>();

            Container.RegisterType<ISpeakers, Speakers>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITimeslots, Timeslots>(new ContainerControlledLifetimeManager());
        }
    }
}
