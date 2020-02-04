using Prism;
using Prism.Ioc;
using AddShortcuts.ViewModels;
using AddShortcuts.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.AppShortcuts;
using System.Linq;
using System;
using Plugin.AppShortcuts.Icons;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AddShortcuts
{
    public partial class App
    {
        public const string AppShortcutUriBase = "myscheme://xamarinappshortcuts/";
        public const string ShortcutOption1 = "PAGE1";
        public const string ShortcutOption2 = "PAGE2";
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            AddShortcuts();
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<Page1, Page1ViewModel>();
            containerRegistry.RegisterForNavigation<Page2, Page2ViewModel>();
        }
        async void AddShortcuts()
        {
            try
            {
                if (CrossAppShortcuts.IsSupported)
                {
                    var shortCurts = await CrossAppShortcuts.Current.GetShortcuts();
                    //check if shortcut is already added or not 
                    if (shortCurts.FirstOrDefault(prop => prop.Label == ShortcutOption1) == null)
                    {
                        var shortcut = new Shortcut()
                        {
                            Label = ShortcutOption1,
                            Description = "Go to Page 1",
                            Icon = new DefaultIcon(),
                            Uri = $"{AppShortcutUriBase}{ShortcutOption1}"
                        };
                        await CrossAppShortcuts.Current.AddShortcut(shortcut);
                    }

                    if (shortCurts.FirstOrDefault(prop => prop.Label == ShortcutOption2) == null)
                    {
                        var shortcut = new Shortcut()
                        {
                            Label = ShortcutOption2,
                            Description = "Go to Page 2",
                            Icon = new LoveIcon(),
                            Uri = $"{AppShortcutUriBase}{ShortcutOption2}"
                        };
                        await CrossAppShortcuts.Current.AddShortcut(shortcut);
                    }
                }
            }
            catch (NotImplementedException)
            {
                //If you receive this error, then the Plugin has not been added as a nuget to the platform that you’re currently targeting.
            }
            catch (NotSupportedOnDeviceException)
            {
                //App shortcuts are not supported on Android API level 24 or lower, or on iOS 8 and below. 
                //If you try and call this api on an unsupported device, this error is raised.
            }
            catch (Exception )
            {
                //Handle any other exception
            }

        }

        protected override async void OnAppLinkRequestReceived(Uri uri)
        {
            var option = uri.ToString().Replace(AppShortcutUriBase, "");
            if (!string.IsNullOrEmpty(option))
            {
                MainPage = new NavigationPage(new MainPage());
                switch (option)
                {
                    case ShortcutOption1:
                        await NavigationService.NavigateAsync("NavigationPage/Page1");
                        break;
                    case ShortcutOption2:

                        await NavigationService.NavigateAsync("NavigationPage/Page2");
                        break;
                }
            }
            else
                base.OnAppLinkRequestReceived(uri);
        }
    }
}
