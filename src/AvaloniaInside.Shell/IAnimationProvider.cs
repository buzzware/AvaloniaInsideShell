using System;
using Avalonia.Animation;

namespace AvaloniaInside.Shell;

public interface IAnimationProvider
{
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
        return navigateType switch
        {
            NavigateType.Normal => Normal,
            NavigateType.ReplaceRoot => ReplaceRoot,
            NavigateType.Modal => Modal,
            NavigateType.Replace => Replace,
            NavigateType.Top => Top,
            NavigateType.Clear => Clear,
            NavigateType.Pop => Pop,
            NavigateType.HostedItemChange => HostedItemChange,
            _ => throw new ArgumentOutOfRangeException(nameof(navigateType), navigateType, null)
        };
    }
}