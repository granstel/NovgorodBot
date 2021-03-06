﻿using Autofac;
using NovgorodBot.Services;
using NovgorodBot.Services.Configuration;
using NovgorodBot.Services.Serialization;
using GranSteL.Helpers.Redis;
using StackExchange.Redis;

namespace NovgorodBot.Api.DependencyModules
{
    public class InternalServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConversationService>().As<IConversationService>();
            builder.RegisterType<QnaService>().As<IQnaService>();
            builder.RegisterType<DialogflowService>().As<IDialogflowService>();
            builder.RegisterType<GeolocationService>().As<IGeolocationService>();
            builder.RegisterType<SkillsService>().As<ISkillsService>();
            builder.RegisterType<CustomJsonSerializer>().AsSelf();

            builder.Register(RegisterCacheService).As<IRedisCacheService>().SingleInstance();
        }

        private RedisCacheService RegisterCacheService(IComponentContext context)
        {
            var configuration = context.Resolve<RedisConfiguration>();

            var db = context.Resolve<IDatabase>();

            var service = new RedisCacheService(db, configuration.KeyPrefix);

            return service;
        }
    }
}
