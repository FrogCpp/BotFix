using System;

namespace BotFix
{
    public class MainProgram
    {
        static public void Run()
        {
            var a = new TelegramController("8042101976:AAGcVCALZxsK0hjzSxuB-yr4gbQoFeMIpFA");
            a.MessageChecker();
            var ev = new EventList(a);
            a.OnEventOccurred += ev.Fuckup;
            a.OnEventOccurred += ev.GetFriend;
            a.OnEventOccurred += ev.GetUserName; // ����� ����� ������� ����������. �� ���� � ����� � ������.
            a.OnEventOccurred += ev.startE;
            Console.ReadLine();
        }
    }
}