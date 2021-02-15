﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Volo.Abp.EntityFrameworkCore
{
    public class AbpDbContextOptions
    {
        internal List<Action<AbpDbContextConfigurationContext>> DefaultPreConfigureActions { get; }

        internal Action<AbpDbContextConfigurationContext> DefaultConfigureAction { get; set; }

        internal Dictionary<Type, List<object>> PreConfigureActions { get; }

        internal Dictionary<Type, object> ConfigureActions { get; }
        
        internal Dictionary<Type, Type> DbContextReplacements { get; }

        public AbpDbContextOptions()
        {
            DefaultPreConfigureActions = new List<Action<AbpDbContextConfigurationContext>>();
            PreConfigureActions = new Dictionary<Type, List<object>>();
            ConfigureActions = new Dictionary<Type, object>();
            DbContextReplacements = new Dictionary<Type, Type>();
        }

        public void PreConfigure([NotNull] Action<AbpDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultPreConfigureActions.Add(action);
        }

        public void Configure([NotNull] Action<AbpDbContextConfigurationContext> action)
        {
            Check.NotNull(action, nameof(action));

            DefaultConfigureAction = action;
        }

        public void PreConfigure<TDbContext>([NotNull] Action<AbpDbContextConfigurationContext<TDbContext>> action)
            where TDbContext : AbpDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            var actions = PreConfigureActions.GetOrDefault(typeof(TDbContext));
            if (actions == null)
            {
                PreConfigureActions[typeof(TDbContext)] = actions = new List<object>();
            }

            actions.Add(action);
        }

        public void Configure<TDbContext>([NotNull] Action<AbpDbContextConfigurationContext<TDbContext>> action) 
            where TDbContext : AbpDbContext<TDbContext>
        {
            Check.NotNull(action, nameof(action));

            ConfigureActions[typeof(TDbContext)] = action;
        }

        internal Type GetReplacedTypeOrSelf(Type dbContextType)
        {
            while (true)
            {
                if (DbContextReplacements.TryGetValue(dbContextType, out var foundType))
                {
                    dbContextType = foundType;
                }
                else
                {
                    return dbContextType;
                }
            }
        }
    }
}