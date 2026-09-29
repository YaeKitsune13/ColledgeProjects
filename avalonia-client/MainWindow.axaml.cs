using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaClient;

public partial class MainWindow : Window
{
    private readonly ApiClient _api = new();
    private readonly Dictionary<string, TextBox> _inputs = new();
    private List<Dictionary<string, string>> _rows = new();
    private Dictionary<string, string>? _selected;

    private Table Current => (Table)ResourceBox.SelectedItem!;

    public MainWindow()
    {
        InitializeComponent();
        ResourceBox.ItemsSource = Tables.All;
        ResourceBox.SelectedIndex = 0;
    }

    private async void OnResourceChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ResourceBox.SelectedItem is null) return;
        BuildForm();
        await Run(Reload);
    }

    private async void OnRefresh(object? sender, RoutedEventArgs e) => await Run(Reload);

    private void OnRowSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (RowsList.SelectedIndex < 0) return;
        _selected = _rows[RowsList.SelectedIndex];
        FillForm(_selected);
    }

    private void OnNew(object? sender, RoutedEventArgs e) => ClearForm();

    private async void OnSave(object? sender, RoutedEventArgs e) => await Run(async () =>
    {
        var body = ReadForm();
        if (_selected is null) await _api.CreateAsync(Current.Path, body);
        else await _api.UpdateAsync(Current.Path, int.Parse(_selected["id"]), body);

        ClearForm();
        await Reload();
    });

    private async void OnDelete(object? sender, RoutedEventArgs e) => await Run(async () =>
    {
        if (_selected is null) throw new InvalidOperationException("Выберите запись в списке");

        await _api.DeleteAsync(Current.Path, int.Parse(_selected["id"]));
        ClearForm();
        await Reload();
    });

    private async Task Reload()
    {
        _rows = await _api.ListAsync(Current.Path);

        HeaderText.Text = "ID   |   " + string.Join("   |   ", Current.Fields.Select(f => f.Label));
        RowsList.ItemsSource = _rows.Select(FormatRow).ToList();
    }

    private string FormatRow(Dictionary<string, string> row) =>
        row.GetValueOrDefault("id", "") + "   |   " +
        string.Join("   |   ", Current.Fields.Select(f => row.GetValueOrDefault(f.Name, "")));


    private void BuildForm()
    {
        _inputs.Clear();
        FormPanel.Children.Clear();

        foreach (var field in Current.Fields)
        {
            var input = new TextBox { Width = 180, Text = field.Default };
            _inputs[field.Name] = input;

            FormPanel.Children.Add(new StackPanel
            {
                Margin = new Thickness(0, 0, 12, 8),
                Children = { new TextBlock { Text = field.Label }, input },
            });
        }
        ClearForm();
    }

    private void FillForm(Dictionary<string, string> row)
    {
        foreach (var field in Current.Fields)
            _inputs[field.Name].Text = row.GetValueOrDefault(field.Name, "");

        FormTitle.Text = $"Изменить запись #{row["id"]}";
    }

    private void ClearForm()
    {
        _selected = null;
        RowsList.SelectedIndex = -1;
        foreach (var field in Current.Fields)
            _inputs[field.Name].Text = field.Default;

        FormTitle.Text = "Новая запись";
    }

    private Dictionary<string, object?> ReadForm()
    {
        var body = new Dictionary<string, object?>();
        foreach (var field in Current.Fields)
        {
            var text = _inputs[field.Name].Text?.Trim() ?? "";
            body[field.Name] = field.IsNumber
                ? (text == "" ? null : long.Parse(text))
                : text;
        }
        return body;
    }

    private async Task Run(Func<Task> action)
    {
        try
        {
            ErrorText.Text = "";
            await action();
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
        }
    }
}
