using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Platform;

namespace AvaloniaInside.Shell.AnimationProviders;

public class NativeAnimationProvider : IAnimationProvider
{
    private IAnimationProvider? _currentNativeAnimationProvider;

    public IPageTransition? Normal { get; init; }
    public IPageTransition? ReplaceRoot { get; init; }
    public IPageTransition? Modal { get; init; }
    public IPageTransition? Replace { get; init; }
    public IPageTransition? Top { get; init; }
    public IPageTransition? Clear { get; init; }
    public IPageTransition? Pop { get; init; }
    public IPageTransition? HostedItemChange { get; init; }

    public IPageTransition? GetAnimationForNavigationType(NavigateType navigateType)
    {
        if (_currentNativeAnimationProvider == null)
            _currentNativeAnimationProvider = CreateNativeAnimationProvider();

        if (_currentNativeAnimationProvider == null)
            return null;

        return navigateType switch
        {
            NavigateType.Normal => _currentNativeAnimationProvider.Normal,
            NavigateType.ReplaceRoot => _currentNativeAnimationProvider.ReplaceRoot,
            NavigateType.Modal => _currentNativeAnimationProvider.Modal,
            NavigateType.Replace => _currentNativeAnimationProvider.Replace,
            NavigateType.Top => _currentNativeAnimationProvider.Top,
            NavigateType.Clear => _currentNativeAnimationProvider.Clear,
            NavigateType.Pop => _currentNativeAnimationProvider.Pop,
            NavigateType.HostedItemChange => _currentNativeAnimationProvider.HostedItemChange,
            _ => throw new ArgumentOutOfRangeException(nameof(navigateType), navigateType, null)
        };
    }

    private IAnimationProvider? CreateNativeAnimationProvider()
    {
        var isMobile = AvaloniaLocator.Current
            .GetService<IRuntimePlatform>()?
            .GetRuntimeInfo()
            .IsMobile ?? false;

        if (isMobile)
        {
            if (OperatingSystem.IsAndroid())
                return new AndroidAnimationProvider();
            if (OperatingSystem.IsIOS())
                return new IosAnimationProvider();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new WindowsAnimationProvider();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new WindowsAnimationProvider();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            return new WindowsAnimationProvider();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return new WindowsAnimationProvider();

        return null;
    }
}