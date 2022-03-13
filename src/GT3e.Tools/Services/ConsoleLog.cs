using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GT3e.Tools.Services;

internal static class ConsoleLog
{
    private static readonly Subject<string> entriesSubject = new();
    internal static IObservable<string> Entries => entriesSubject.AsObservable();

    internal static void Write(string message)
    {
        entriesSubject.OnNext(message);
    }
}