/*
 * Copyright (c) 2015, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using NoteSync.Pages;
using NoteSync.ViewModel;

namespace NoteSync.Controls
{
    public sealed partial class EditNoteCard : UserControl
    {
        private NoteObject _note;

        public EditNoteCard()
        {
            InitializeComponent();
        }

        public NoteObject Note
        {
            get { return _note; }
            set
            {
                _note = value;
                SetFields();
            }
        }

        public Flyout AttachedFlyout { get; set; }

        public string Content
        {
            get { return ContentBlock.Text; }
            private set { ContentBlock.Text = value; }
        }

        public string Title
        {
            get { return TitleNameBlock.Text; }
            private set { TitleNameBlock.Text = value; }
        }

        private void SetFields()
        {
            if (_note == null) return;
            Content = _note.Content ?? String.Empty;
            Title = _note.Title ?? String.Empty;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Note.Title = Title;
            Note.Content = Content;
            MainPage.NotesDataModel.SaveNote(Note, String.IsNullOrWhiteSpace(Note.ObjectId));
            HideFlyout();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            HideFlyout();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainPage.NotesDataModel.DeleteObject(Note);
            HideFlyout();
        }

        private async void HideFlyout()
        {
            if (AttachedFlyout != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => AttachedFlyout.Hide());
            }
        }

        public void DeleteVisible(bool visible)
        {
            if (visible)
            {
                DeleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}