﻿using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using System.Diagnostics;
using Avalonia.Rendering.Composition;
using Avalonia.Layout;
using Avalonia.Rendering.Composition.Animations;
using System.Xml.Linq;

namespace AvaloniaInside.Shell.Platform.Ios;

public class DefaultIosPageSlide : IPageTransition
{
    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.25);

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public Easing SlideInEasing { get; set; } = Easing.Parse("0.42, 0.0, 0.58, 1.0");

    private ImplicitAnimationCollection? _implicitAnimations;

    private void EnsureAnimationCreated(CompositionVisual element)
    {
        if (_implicitAnimations != null) return;

        var compositor = element.Compositor;
        var easing = Easing.Parse("0.42, 0.0, 0.58, 1.0");

        var offsetAnimation = compositor.CreateVector3DKeyFrameAnimation();
        offsetAnimation.Duration = Duration;
        offsetAnimation.Target = "Offset";
        
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easing);

        var animationGroup = compositor.CreateAnimationGroup();
        animationGroup.Add(offsetAnimation);

        _implicitAnimations = compositor.CreateImplicitAnimationCollection();
        _implicitAnimations["Offset"] = animationGroup;
    }

    public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var tasks = new List<Task>();
        var parent = GetVisualParent(from, to);

        var distance = parent.Bounds.Width;

        EnsureAnimationCreated(ElementComposition.GetElementVisual(parent)!);

        if (from != null)
        {
            from.ZIndex = forward ? 0 : 1;
            var fromElement = ElementComposition.GetElementVisual(from)!;

            fromElement.ImplicitAnimations = _implicitAnimations;
            fromElement.Offset = new Vector3D(forward ? -distance / 4d : distance, 0d, 0d);
        }

        if (to != null)
        {
            to.ZIndex = forward ? 1 : 0;
            var toElement = ElementComposition.GetElementVisual(to)!;

            if (forward) {
                to[TranslateTransform.XProperty] = distance;
            }
            toElement.ImplicitAnimations = _implicitAnimations;
            toElement.Offset = new Vector3D(0d, 0d, 0d);
        }

        await Task.Run(() => Task.Delay(Duration, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Gets the common visual parent of the two control.
    /// </summary>
    /// <param name="from">The from control.</param>
    /// <param name="to">The to control.</param>
    /// <returns>The common parent.</returns>
    /// <exception cref="ArgumentException">
    /// The two controls do not share a common parent.
    /// </exception>
    /// <remarks>
    /// Any one of the parameters may be null, but not both.
    /// </remarks>
    protected static Visual GetVisualParent(Visual? from, Visual? to)
    {
        var p1 = (from ?? to)!.GetVisualParent();
        var p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2)
        {
            throw new ArgumentException("Controls for PageSlide must have same parent.");
        }

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}

internal class IosPageSlide : IPageTransition
{
    /// <summary>
    /// The axis on which the PageSlide should occur
    /// </summary>
    public enum SlideAxis
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageSlide"/> class.
    /// </summary>
    public IosPageSlide()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageSlide"/> class.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="orientation">The axis on which the animation should occur</param>
    public IosPageSlide(TimeSpan duration, SlideAxis orientation = SlideAxis.Horizontal)
    {
        Duration = duration;
        Orientation = orientation;
    }

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.25);

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public SlideAxis Orientation { get; set; } = SlideAxis.Horizontal;

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public Easing SlideInEasing { get; set; } = Easing.Parse("0.42, 0.0, 0.58, 1.0");

    /// <summary>
    /// Gets or sets element exit easing.
    /// </summary>
    public Easing SlideOutEasing { get; set; } = Easing.Parse("0.42, 0.0, 0.58, 1.0");

    /// <inheritdoc />
    public virtual async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var tasks = new List<Task>();
        var parent = GetVisualParent(from, to);
        var distance = Orientation == SlideAxis.Horizontal ? parent.Bounds.Width : parent.Bounds.Height;
        var translateProperty = Orientation == SlideAxis.Horizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty;

        if (from != null)
        {
            from.ZIndex = forward ? 0 : 1;
            var animation = new Animation
            {
                FillMode = FillMode.Forward,
                Easing = SlideOutEasing,
                Children =
                    {
                        new KeyFrame
                        {
                            Setters = {
                                new Setter { Property = translateProperty, Value = 0d },
                                new Setter
                                {
                                    Property = Visual.IsVisibleProperty,
                                    Value = true
                                }
                            },
                            Cue = new Cue(0d)
                        },
                        new KeyFrame
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = translateProperty,
                                    Value = forward ? -distance / 4d : distance
                                }
                            },
                            Cue = new Cue(1d)
                        }
                    },
                Duration = Duration
            };
            tasks.Add(animation.RunAsync(from, cancellationToken));
        }

        if (to != null)
        {
            to.ZIndex = forward ? 1 : 0;
            to.IsVisible = true;
            var animation = new Animation
            {
                FillMode = FillMode.Forward,
                Easing = SlideInEasing,
                Children =
                    {
                        new KeyFrame
                        {
                            Setters =
                            {
                                new Setter
                                {
                                    Property = translateProperty,
                                    Value = forward ? distance : -distance / 4d
                                }
                            },
                            Cue = new Cue(0d)
                        },
                        new KeyFrame
                        {
                            Setters = { new Setter { Property = translateProperty, Value = 0d } },
                            Cue = new Cue(1d)
                        }
                    },
                Duration = Duration
            };
            tasks.Add(animation.RunAsync(to, cancellationToken));
        }

        await Task.WhenAll(tasks);
        if (from != null && !cancellationToken.IsCancellationRequested)
        {
            from.IsVisible = false;
        }
    }

    /// <summary>
    /// Gets the common visual parent of the two control.
    /// </summary>
    /// <param name="from">The from control.</param>
    /// <param name="to">The to control.</param>
    /// <returns>The common parent.</returns>
    /// <exception cref="ArgumentException">
    /// The two controls do not share a common parent.
    /// </exception>
    /// <remarks>
    /// Any one of the parameters may be null, but not both.
    /// </remarks>
    protected static Visual GetVisualParent(Visual? from, Visual? to)
    {
        var p1 = (from ?? to)!.GetVisualParent();
        var p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2)
        {
            throw new ArgumentException("Controls for PageSlide must have same parent.");
        }

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}
