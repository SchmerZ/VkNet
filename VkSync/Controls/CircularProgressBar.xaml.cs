//  --------------------------------
//  Copyright (c) Huy Pham. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.opensource.org/licenses/ms-pl.html
//  ---------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VkSync.Controls
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar
    {
        public static readonly DependencyProperty IsSpinningProperty =
            DependencyProperty.Register("IsSpinning", typeof(bool), typeof(CircularProgressBar), new UIPropertyMetadata(true, OnSpinningChanged));

        #region Fields

        private readonly DispatcherTimer _animationTimer;

        #endregion

        #region Constructors

        public CircularProgressBar()
        {
            InitializeComponent();

            IsVisibleChanged += OnVisibleChanged;

            _animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
            {
                Interval = new TimeSpan(0, 0, 0, 0, 75)
            };
        }

        #endregion

        public bool IsSpinning
        {
            get { return (bool) GetValue(IsSpinningProperty); }
            set { SetValue(IsSpinningProperty, value); }
        }

        private static void SetPosition(DependencyObject ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50 + (Math.Sin(offset + (posOffSet * step)) * 50));
            ellipse.SetValue(Canvas.TopProperty, 50 + (Math.Cos(offset + (posOffSet * step)) * 50));
        }

        public void Start()
        {
            _animationTimer.Tick += OnAnimationTick;
            _animationTimer.Start();
        }

        public void Stop()
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= OnAnimationTick;
        }

        private void OnAnimationTick(object sender, EventArgs e)
        {
            _spinnerRotate.Angle = (_spinnerRotate.Angle + 36) % 360;
        }

        private void OnCanvasLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(_circle0, offset, 0.0, step);
            SetPosition(_circle1, offset, 1.0, step);
            SetPosition(_circle2, offset, 2.0, step);
            SetPosition(_circle3, offset, 3.0, step);
            SetPosition(_circle4, offset, 4.0, step);
            SetPosition(_circle5, offset, 5.0, step);
            SetPosition(_circle6, offset, 6.0, step);
            SetPosition(_circle7, offset, 7.0, step);
            SetPosition(_circle8, offset, 8.0, step);
        }

        private void OnCanvasUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

            if (isVisible && IsSpinning)
                Start();
            else
                Stop();
        }

        private static void OnSpinningChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var isSpinning = (bool)args.NewValue;
            var circularProgressBar = (CircularProgressBar) sender;

            if (isSpinning)
                circularProgressBar.Start();
            else
                circularProgressBar.Stop();
        }
    }
}