﻿using System;
using Prism.Regions;
using Prism.Regions.Adapters;
using Prism.Regions.Behaviors;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Ioc
{
    /// <summary>
    /// Provides registration and configuration helpers for Region Navigation
    /// </summary>
    public static class RegionRegistrationExtensions
    {
        /// <summary>
        /// Registers the default RegionManager
        /// </summary>
        /// <param name="containerRegistry">The current <see cref="IContainerRegistry" /></param>
        /// <param name="configureAdapters">A configuration delegate to Register Adapter Mappings.</param>
        /// <param name="configureBehaviors">A configuration delegate to Add custom Region Behaviors.</param>
        /// <returns>The current <see cref="IContainerRegistry" /></returns>
        public static IContainerRegistry RegisterRegionServices(this IContainerRegistry containerRegistry, Action<RegionAdapterMappings> configureAdapters = null, Action<IRegionBehaviorFactory> configureBehaviors = null)
        {
            containerRegistry.RegisterSingleton<RegionAdapterMappings>(p =>
            {
                var regionAdapterMappings = new RegionAdapterMappings();
                configureAdapters?.Invoke(regionAdapterMappings);

                regionAdapterMappings.RegisterDefaultMapping<CarouselView, CarouselViewRegionAdapter>();
                regionAdapterMappings.RegisterDefaultMapping<CollectionView, CollectionViewRegionAdapter>();
                regionAdapterMappings.RegisterDefaultMapping<Layout<View>, LayoutViewRegionAdapter>();
                regionAdapterMappings.RegisterDefaultMapping<ScrollView, ScrollViewRegionAdapter>();
                regionAdapterMappings.RegisterDefaultMapping<ContentView, ContentViewRegionAdapter>();
                return regionAdapterMappings;
            });

            containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
            containerRegistry.RegisterSingleton<IRegionBehaviorFactory>(p =>
            {
                var regionBehaviors = p.Resolve<RegionBehaviorFactory>();
                regionBehaviors.AddIfMissing<BindRegionContextToVisualElementBehavior>(BindRegionContextToVisualElementBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<RegionActiveAwareBehavior>(RegionActiveAwareBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<SyncRegionContextWithHostBehavior>(SyncRegionContextWithHostBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<RegionManagerRegistrationBehavior>(RegionManagerRegistrationBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<RegionMemberLifetimeBehavior>(RegionMemberLifetimeBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<ClearChildViewsRegionBehavior>(ClearChildViewsRegionBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<AutoPopulateRegionBehavior>(AutoPopulateRegionBehavior.BehaviorKey);
                regionBehaviors.AddIfMissing<DestructibleRegionBehavior>(DestructibleRegionBehavior.BehaviorKey);
                configureBehaviors?.Invoke(regionBehaviors);
                return regionBehaviors;
            });
            containerRegistry.Register<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>();
            containerRegistry.Register<IRegionNavigationJournal, RegionNavigationJournal>();
            containerRegistry.Register<IRegionNavigationService, RegionNavigationService>();
            containerRegistry.RegisterManySingleton<RegionResolverOverrides>(typeof(IResolverOverridesHelper), typeof(IActiveRegionHelper));
            return containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
        }

        /// <summary>
        /// Configures the <see cref="IRegionBehaviorFactory"/>. 
        /// This will be the list of default behaviors that will be added to a region.
        /// </summary>
        public static PrismApplicationBase ConfigureDefaultRegionBehaviors(this PrismApplicationBase app, Action<IRegionBehaviorFactory> configure)
        {
            var regionBehaviors = app.Container.Resolve<IRegionBehaviorFactory>();
            configure.Invoke(regionBehaviors);
            return app;
        }

        /// <summary>
        /// Allows you to provide custom RegionAdapters for your controls or override the default ones from Prism.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        public static PrismApplicationBase ConfigureRegionAdapterMappings(this PrismApplicationBase app, Action<RegionAdapterMappings> configure)
        {
            var container = app.Container;
            var regionAdapterMappings = container.Resolve<RegionAdapterMappings>();
            configure.Invoke(regionAdapterMappings);
            return app;
        }
    }
}
