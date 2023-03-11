using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AvaloniaInside.Shell;

public partial class ShellView
{
	public const double DefaultSideMenuSize = 250;

	private readonly AvaloniaList<SideMenuItem> _sideMenuItems = new();

	#region Properties

	public double SideMenuSize => ScreenSize == ScreenSizeType.Small ? DesiredSize.Width - 35 : DefaultSideMenuSize;

	#region SideMenuPresented

	private bool _sideMenuPresented;
	public static readonly DirectProperty<ShellView, bool> SideMenuPresentedProperty =
		AvaloniaProperty.RegisterDirect<ShellView, bool>(
			nameof(SideMenuPresented),
			o => o.SideMenuPresented,
			(o, v) => o.SideMenuPresented = v);
	public bool SideMenuPresented
	{
		get => _sideMenuPresented;
		set
		{
			if (SetAndRaise(SideMenuPresentedProperty, ref _sideMenuPresented, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region LargeScreenSideMenuBehave

	private SideMenuBehaveType _largeScreenSideMenuBehave = SideMenuBehaveType.Keep;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> LargeScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.LargeScreenSideMenuBehave,
			(o, v) => o.LargeScreenSideMenuBehave = v);
	public SideMenuBehaveType LargeScreenSideMenuBehave
	{
		get => _largeScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(LargeScreenSideMenuBehaveProperty, ref _largeScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region MediumScreenSideMenuBehave

	private SideMenuBehaveType _mediumScreenSideMenuBehave = SideMenuBehaveType.Default;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> MediumScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.MediumScreenSideMenuBehave,
			(o, v) => o.MediumScreenSideMenuBehave = v);
	public SideMenuBehaveType MediumScreenSideMenuBehave
	{
		get => _mediumScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(MediumScreenSideMenuBehaveProperty, ref _mediumScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region SmallScreenSideMenuBehave

	private SideMenuBehaveType _smallScreenSideMenuBehave = SideMenuBehaveType.Default;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> SmallScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.SmallScreenSideMenuBehave,
			(o, v) => o.SmallScreenSideMenuBehave = v);
	public SideMenuBehaveType SmallScreenSideMenuBehave
	{
		get => _smallScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(SmallScreenSideMenuBehaveProperty, ref _smallScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region LargeScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> LargeScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(LargeScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.Inline,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode LargeScreenSideMenuMode
	{
		get => GetValue(LargeScreenSideMenuModeProperty);
		set => SetValue(LargeScreenSideMenuModeProperty, value);
	}

	#endregion

	#region MediumScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> MediumScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(MediumScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.CompactInline,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode MediumScreenSideMenuMode
	{
		get => GetValue(MediumScreenSideMenuModeProperty);
		set => SetValue(MediumScreenSideMenuModeProperty, value);
	}

	#endregion

	#region SmallScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> SmallScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(SmallScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.Overlay,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode SmallScreenSideMenuMode
	{
		get => GetValue(SmallScreenSideMenuModeProperty);
		set => SetValue(SmallScreenSideMenuModeProperty, value);
	}

	#endregion

	#region SideMenuHeader

	public static readonly StyledProperty<object?> SideMenuHeaderProperty =
		AvaloniaProperty.Register<SideMenuView, object?>(
			nameof(SideMenuHeader));

	public object? SideMenuHeader
	{
		get => GetValue(SideMenuHeaderProperty);
		set => SetValue(SideMenuHeaderProperty, value);
	}

	#endregion

	#region SideMenuFooter

	public static readonly StyledProperty<object?> SideMenuFooterProperty =
		AvaloniaProperty.Register<SideMenuView, object?>(
			nameof(SideMenuFooter));

	public object? SideMenuFooter
	{
		get => GetValue(SideMenuFooterProperty);
		set => SetValue(SideMenuFooterProperty, value);
	}

	#endregion

	#region SideMenuContentsTemplate

	public static readonly StyledProperty<IDataTemplate> SideMenuContentsTemplateProperty =
		AvaloniaProperty.Register<SideMenuView, IDataTemplate>(
			nameof(SideMenuContentsTemplate));

	public IDataTemplate SideMenuContentsTemplate
	{
		get => GetValue(SideMenuContentsTemplateProperty);
		set => SetValue(SideMenuContentsTemplateProperty, value);
	}

	#endregion

	#region SideMenuContents

	public static readonly StyledProperty<AvaloniaList<object>> SideMenuContentsProperty =
		AvaloniaProperty.Register<SideMenuView, AvaloniaList<object>>(
			nameof(SideMenuContents),
			defaultValue: new AvaloniaList<object>());

	public AvaloniaList<object> SideMenuContents
	{
		get => GetValue(SideMenuContentsProperty);
		set => SetValue(SideMenuContentsProperty, value);
	}

	#endregion

	#region SideMenuSelectedItem

	private SideMenuItem? _sideMenuSelectedItem;
	public static readonly DirectProperty<ShellView, SideMenuItem?> SideMenuSelectedItemProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuItem?>(
			nameof(SideMenuSelectedItem),
			o => o.SideMenuSelectedItem,
			(o, v) => o.SideMenuSelectedItem = v);
	public SideMenuItem? SideMenuSelectedItem
	{
		get => _sideMenuSelectedItem;
		set
		{
			if (SetAndRaise(SideMenuSelectedItemProperty, ref _sideMenuSelectedItem, value))
				SideMenuItemChanged(value);
		}
	}

	#endregion

	#endregion

	#region Behavior

	protected virtual Task MenuActionAsync(CancellationToken cancellationToken)
	{
		SideMenuPresented = !SideMenuPresented;
		return Task.CompletedTask;
	}

	protected virtual void UpdateSideMenu()
	{
		if (_splitView == null || _navigationView == null) return;

		var behave = ScreenSize switch
		{
			ScreenSizeType.Small => SmallScreenSideMenuBehave,
			ScreenSizeType.Medium => MediumScreenSideMenuBehave,
			ScreenSizeType.Large => LargeScreenSideMenuBehave,
			_ => throw new ArgumentOutOfRangeException()
		};

		if (behave == SideMenuBehaveType.Default)
		{
			_splitView.OpenPaneLength = SideMenuPresented ? SideMenuSize : 0;
			_splitView.IsPaneOpen = SideMenuPresented;
			_navigationView.HasSideMenuOption = true;
		}
		else if (behave == SideMenuBehaveType.Keep)
		{
			_splitView.OpenPaneLength = SideMenuSize;
			_splitView.IsPaneOpen = true;
			_navigationView.HasSideMenuOption = false;
		}
		else if (behave == SideMenuBehaveType.Closed)
		{
			_splitView.OpenPaneLength = 0;
			_splitView.IsPaneOpen = true;
			_navigationView.HasSideMenuOption = true;
		}
	}

	private bool _skipChanges = false;
	protected virtual void SelectSideMenuItem()
	{
		if (_sideMenuView == null) return;
		_skipChanges = true;
		SideMenuSelectedItem = _sideMenuItems
			.FirstOrDefault(f => f.Path == Navigation.CurrentUri.AbsolutePath);
		_skipChanges = false;
	}

	private void SideMenuItemChanged(SideMenuItem item)
	{
		if (_skipChanges) return;
		//_ = Navigation.NavigateAsync(item.Path, NavigateType.Top);
	}

	#endregion

	#region Actions

	public void AddSideMenuItem(string title, string path, string icon)
	{
		_sideMenuItems.Add(new SideMenuItem(title, path, icon));
	}

	public void AddSideMenuItem(string title, string path, IImage icon)
	{
		_sideMenuItems.Add(new SideMenuItem(title, path, icon));
	}

	#endregion
}
