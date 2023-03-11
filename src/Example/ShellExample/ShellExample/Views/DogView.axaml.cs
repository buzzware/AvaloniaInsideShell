using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class DogView : UserControl, INavigation
{
	public DogView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-dog";
	public object? Title => "Dog";
	public object? Item { get; }
}

