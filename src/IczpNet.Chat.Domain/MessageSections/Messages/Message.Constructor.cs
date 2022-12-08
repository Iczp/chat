﻿using IczpNet.AbpCommons;
using IczpNet.Chat.ChatObjects;
using System;

namespace IczpNet.Chat.MessageSections.Messages;

public partial class Message
{
    public const int QuotePathMaxLength = 1000;
    public const int ForwardPathMaxLength = 1000;
    public const string Delimiter = "/";
    protected Message() { }

    public Message(Guid id, ChatObject sender, ChatObject receiver) : base(id)
    {
        //, IMessageContent messageContent
        Sender = sender;
        SenderType = sender.ObjectType;
        Receiver = receiver;
        ReceiverType = receiver.ObjectType;
        MessageChannel = MessageExtentions.MakeMessageChannel(sender, receiver);
        SessionId = MessageExtentions.MakeSessionId(MessageChannel, sender, receiver);
        //MessageType = messageType;
    }

    internal void SetQuoteMessage(Message forwardMessage)
    {
        QuoteMessage = forwardMessage;
        QuoteDepth = forwardMessage.QuoteDepth + 1;
        QuotePath = forwardMessage.QuotePath + Delimiter + forwardMessage.AutoId;
        Assert.If(QuotePath.Length > QuotePathMaxLength, "Maximum length exceeded in [QuotePath].");
    }

    internal void SetForwardMessage(Message forwardMessage)
    {
        ForwardMessage = forwardMessage;
        ForwardDepth = forwardMessage.QuoteDepth + 1;
        ForwardPath = forwardMessage.QuotePath + Delimiter + forwardMessage.AutoId;
        Assert.If(ForwardPath.Length > ForwardPathMaxLength, "Maximum length exceeded in [ForwardPath].");
    }

    internal virtual void SetKey(string keyName, string keyValue)
    {
        KeyName = keyName;
        KeyValue = keyValue;
    }
}