using System;
using System.Collections.Generic;
using Avalonia.Animation;

namespace AvaloniaInside.Shell.AnimationProviders;

public sealed class IosAnimationProvider : IAnimationProvider
{
    public IosAnimationProvider()
    {
        Normal = new CompositePageTransition
        {
            PageTransitions = new List<IPageTransition>
            {
                new CrossFade { Duration = TimeSpan.FromMilliseconds(125) },
                new CustomPageSlide
                {
                    Orientation = CustomPageSlide.SlideAxis.Horizontal,
                    Direction = CustomPageSlide.SlideDirection.RightToLeft,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            }
        };
        ReplaceRoot = new CrossFade { Duration = TimeSpan.FromMilliseconds(250) };
        Modal = new CrossFade { Duration = TimeSpan.FromMilliseconds(250) };
        Replace = new CrossFade { Duration = TimeSpan.FromMilliseconds(250) };
        Pop = new CompositePageTransition
        {
            PageTransitions = new List<IPageTransition>
            {
                new CrossFade { Duration = TimeSpan.FromMilliseconds(125) },
                new CustomPageSlide
                {
                    Orientation = CustomPageSlide.SlideAxis.Horizontal,
                    Direction = CustomPageSlide.SlideDirection.LeftToRight,
                    Duration = TimeSpan.FromMilliseconds(125)
                }
            }
        };
    }

    public IPageTransition? Normal { get; init; }
    public IPageTransition? ReplaceRoot { get; init; }
    public IPageTransition? Modal { get; init; }
    public IPageTransition? Replace { get; init; }
    public IPageTransition? Top { get; init; }
    public IPageTransition? Clear { get; init; }
    public IPageTransition? Pop { get; init; }
    public IPageTransition? HostedItemChange { get; init; }
}