using IczpNet.AbpCommons;
using IczpNet.AbpTrees;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Connections;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.TextTemplates;
using IczpNet.Pusher;
using IczpNet.Pusher.ShortIds;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SimpleStateChecking;

namespace IczpNet.Chat;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ChatDomainSharedModule),
    typeof(AbpAutoMapperModule)
)]

[DependsOn(typeof(AbpCommonsDomainModule))]
[DependsOn(typeof(AbpTreesDomainModule))]
//[DependsOn(typeof(AbpIdentityDomainModule))]
[DependsOn(typeof(AbpPermissionManagementDomainIdentityModule))]
[DependsOn(typeof(PusherDomainModule))]
public class ChatDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<ChatDomainModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChatDomainModule>(validate: true);
        });

        Configure<ShortIdOptions>(options =>
        {
            options.Length = 16;
            options.UseNumbers = false;
            options.UseSpecialCharacters = false;
        });

        Configure<AbpSimpleStateCheckerOptions<ChatObject>>(options =>
        {
            options.GlobalStateCheckers.Add<ChatObjectStateChecker>();
        });
        Configure<AbpSimpleStateCheckerOptions<SessionUnit>>(options =>
        {
            options.GlobalStateCheckers.Add<SessionUnitStateChecker>();
        });
    }

    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        //await context.AddBackgroundWorkerAsync<ConnectionWorker>();
        //await context.AddBackgroundWorkerAsync<SendMessageWorker>();
        //await context.AddBackgroundWorkerAsync<SendToRoomWorker>();
        await base.OnPostApplicationInitializationAsync(context);
    }
}
