﻿using System;

namespace BotFix
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new TelegramController("8042101976:AAGcVCALZxsK0hjzSxuB-yr4gbQoFeMIpFA");
            a.MessageChecker();
            var ev = new EventList(a);
            a.OnEventOccurred += ev.Fuckup;
            a.OnEventOccurred += ev.GetFriend;
            a.OnEventOccurred += ev.GetUserName; // здесь важен порядок объявления. Мы идем с конца в начало.
            a.OnEventOccurred += ev.startE;
            Console.ReadLine();
        }
    }
}