﻿// Slightly modified version of
using System;
using UIKit;
using CoreGraphics;


namespace Acr.UserDialogs
{
    public class ModalDateTimePickerViewController : UIViewController
    {
        static readonly nfloat ToolbarHeight = 44F;

        readonly UIViewController parent;
        UILabel headerLabel;
        UIButton doneButton;
        UIButton cancelButton;
        UIView internalView;


        public ModalDateTimePickerViewController(string headerText, UIViewController parent)
        {
            HeaderBackgroundColor = UIColor.White;
            HeaderTextColor = UIColor.Black;
            HeaderText = headerText;

            DoneButtonText = "Done";
            CancelButtonText = "Cancel";

            this.parent = parent;
        }


        public UIColor HeaderBackgroundColor { get; set; }
        public UIColor HeaderTextColor { get; set; }
        public string HeaderText { get; set; }
        public string DoneButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public UIDatePicker DatePicker { get; set; }
        public event EventHandler<bool> Dismissed;


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.internalView = new UIView();

            this.headerLabel = new UILabel(new CGRect((nfloat)0, (nfloat)0, (nfloat)(320 / 2), (nfloat)44))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                BackgroundColor = this.HeaderBackgroundColor,
                TextColor = this.HeaderTextColor,
                Text = this.HeaderText,
                TextAlignment = UITextAlignment.Center
            };
            this.internalView.AddSubview(this.headerLabel);

            if (!String.IsNullOrWhiteSpace(this.CancelButtonText))
            {
                this.cancelButton = UIButton.FromType(UIButtonType.System);
                this.cancelButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
                this.cancelButton.BackgroundColor = UIColor.Clear;
                this.cancelButton.SetTitle(CancelButtonText, UIControlState.Normal);
                this.cancelButton.TouchUpInside += CancelButtonTapped;
                this.internalView.AddSubview(this.cancelButton);
            }

            this.doneButton = UIButton.FromType(UIButtonType.System);
            this.doneButton.SetTitleColor(HeaderTextColor, UIControlState.Normal);
            this.doneButton.BackgroundColor = UIColor.Clear;
            this.doneButton.SetTitle(DoneButtonText, UIControlState.Normal);
            this.doneButton.TouchUpInside += DoneButtonTapped;


            this.internalView.AddSubview(DatePicker);
            this.internalView.BackgroundColor = HeaderBackgroundColor;
            this.internalView.AddSubview(this.doneButton);

            this.View.BackgroundColor = UIColor.Clear;
            this.DatePicker.BackgroundColor = UIColor.White;

            this.Add(this.internalView);
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.Show();
        }


        void Show(bool onRotate = false)
        {
            var buttonSize = new CGSize((nfloat)71, (nfloat)30);
            var internalViewSize = new CGSize(this.parent.View.Frame.Width, (nfloat)(DatePicker.Frame.Height + ToolbarHeight));
            var internalViewFrame = CGRect.Empty;

            if (this.InterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                if (onRotate)
                {
                    internalViewFrame = new CGRect(
                        (nfloat)0,
                        (nfloat)(this.View.Frame.Height - internalViewSize.Height),
                        internalViewSize.Width,
                        internalViewSize.Height
                    );
                }
                else
                {
                    internalViewFrame = new CGRect(
                        0,
                        this.View.Bounds.Height - internalViewSize.Height,
                        internalViewSize.Width,
                        internalViewSize.Height
                    );
                }
            }
            else
            {
                if (onRotate)
                {
                    internalViewFrame = new CGRect(
                        (nfloat)0,
                        (nfloat)(this.View.Bounds.Height - internalViewSize.Height),
                        internalViewSize.Width,
                        internalViewSize.Height
                    );
                }
                else
                {
                    internalViewFrame = new CGRect(
                        (nfloat)0,
                        (nfloat)(this.View.Frame.Height - internalViewSize.Height),
                        internalViewSize.Width,
                        internalViewSize.Height
                    );
                }
            }
            this.internalView.Frame = internalViewFrame;
            this.DatePicker.Frame = new CGRect(
                this.DatePicker.Frame.X,
                (nfloat)ToolbarHeight,
                this.internalView.Frame.Width,
                this.DatePicker.Frame.Height
            );

            this.headerLabel.Frame = new CGRect(
                (nfloat)(20 + buttonSize.Width),
                (nfloat)4,
                (nfloat)(this.parent.View.Frame.Width - (40 + 2 * buttonSize.Width)),
                (nfloat)35
            );
            this.doneButton.Frame = new CGRect(
                (nfloat)(internalViewFrame.Width - buttonSize.Width - 10),
                (nfloat)7,
                buttonSize.Width,
                buttonSize.Height
            );
            this.cancelButton.Frame = new CGRect(
                (nfloat)10,
                (nfloat)7,
                buttonSize.Width,
                buttonSize.Height
            );
        }


        async void DoneButtonTapped(object sender, EventArgs e)
        {
            await this.DismissViewControllerAsync(true);
            this.Dismissed?.Invoke(this, true);
        }


        async void CancelButtonTapped(object sender, EventArgs e)
        {
            await this.DismissViewControllerAsync(true);
            this.Dismissed?.Invoke(this, false);
        }


        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            if (this.InterfaceOrientation == UIInterfaceOrientation.Portrait ||
                this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
                this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                this.Show(true);
                this.View.SetNeedsDisplay();
            }
        }
    }
}