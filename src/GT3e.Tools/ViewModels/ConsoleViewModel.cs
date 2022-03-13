using System;
using System.Collections.Generic;
using System.Windows;
using GT3e.Tools.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace GT3e.Tools.ViewModels;

public class ConsoleViewModel : ObservableObject
{
    private readonly IList<string> messages = new List<string>();
    private string consoleContent;
    private Visibility consoleVisibility;

    public ConsoleViewModel()
    {
        ConsoleLog.Entries.Subscribe(this.HandleNewEntry);
    }

    public string ConsoleContent
    {
        get => this.consoleContent;
        set => this.SetProperty(ref this.consoleContent, value);
    }

    public Visibility ConsoleVisibility
    {
        get => this.consoleVisibility;
        set => this.SetProperty(ref this.consoleVisibility, value);
    }

    private void HandleNewEntry(string entry)
    {
        this.messages.Add(entry);
        if(this.messages.Count > 50)
        {
            this.messages.RemoveAt(0);
        }

        this.consoleContent = string.Join(Environment.NewLine, this.messages);
    }
}