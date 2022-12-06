﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.MessageSections.Messages.Dtos;
using IczpNet.Chat.MessageSections.Templates;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.MessageServices
{
    public class MessageAppService
        : CrudChatAppService<
        Message,
        MessageDetailDto,
        MessageDto,
        Guid,
        MessageGetListInput,
        MessageCreateInput,
        MessageUpdateInput>,
        IMessageAppService
    {

        protected IMessageManager MessageManager { get; set; }

        public MessageAppService(
            IRepository<Message, Guid> repository,
            IMessageManager messageManager) : base(repository)
        {
            MessageManager = messageManager;
        }

        public Task<MessageInfo<TextContentInfo>> SendTextMessageAsync(MessageInput<TextContentInfo> input)
        {
            return MessageManager.SendTextMessageAsync(input);
        }
    }
}
