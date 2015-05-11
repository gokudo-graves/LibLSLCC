﻿/* Legal note:
 * This code interfaces the AvalonEdit TextEditor library,
 * which is provided under the terms of the GNU Lesser General Public license (LGPL),
 * available here: http://www.gnu.org/copyleft/lesser.html.
 * If you use this code in your project you might be subject to restrictions
 * imposed by the LGPL.
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using RichTextBox = System.Windows.Controls.RichTextBox;
using TextBox = System.Windows.Controls.TextBox;

namespace FindReplace
{
    /// <summary>
    /// Adapter for Avalonedit TextEditor
    /// </summary>
    public class TextEditorAdapter : IEditor
    {
        public TextEditorAdapter(TextEditor editor) { te = editor; }

        TextEditor te;
        public string Text { get { return te.Text; } }
        public int SelectionStart { get { return te.SelectionStart; } }
        public int SelectionLength { get { return te.SelectionLength; } }
        public void BeginChange() { te.BeginChange(); }
        public void EndChange() { te.EndChange(); }
        public void Select(int start, int length)
        {
            te.Select(start, length);
            TextLocation loc = te.Document.GetLocation(start);
            te.ScrollTo(loc.Line, loc.Column);
        }
        public void Replace(int start, int length, string ReplaceWith) { te.Document.Replace(start, length, ReplaceWith); }

    }

    /// <summary>
    /// Adapter for WPF TextBox.
    /// Note that this is semi-usable since the WPF Textbox does not have a HideSelection property.
    /// </summary>
    public class TextBoxAdapter : IEditor
    {
        public TextBoxAdapter(TextBox editor) { te = editor; }

        TextBox te;
        public string Text { get { return te.Text; } }
        public int SelectionStart { get { return te.SelectionStart; } }
        public int SelectionLength { get { return te.SelectionLength; } }
        public void BeginChange() { te.BeginChange(); }
        public void EndChange() { te.EndChange(); }
        public void Select(int start, int length)
        {
            te.Select(start, length);
        }
        public void Replace(int start, int length, string ReplaceWith) { te.Text = te.Text.Substring(0,start) + ReplaceWith + te.Text.Substring(start+length); }

    }

    /// <summary>
    /// Adapter for WPF RichTextBox.
    /// The WPF RichTextBox does not have a HideSelection property either.
    /// Here the currently selected text is colored yellow, so that it can be seen.
    /// </summary>
    public class RichTextBoxAdapter : IEditor
    {
        public RichTextBoxAdapter(RichTextBox editor) { rtb = editor; }

        RichTextBox rtb;
        TextRange oldsel;
        public string Text { get { return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text; } }
        public int SelectionStart
        {
            get
            {
                return GetPos(rtb.Document.ContentStart, rtb.Selection.Start);
            } 
        }
        public int SelectionLength { get { return rtb.Selection.Text.Length; } }
        public void BeginChange() { rtb.BeginChange(); }
        public void EndChange() { rtb.EndChange(); }
        public void Select(int start, int length)
        {                         
            TextPointer tp = rtb.Document.ContentStart;
            rtb.Selection.Select(GetPoint(tp, start), GetPoint(tp, start + length));
            rtb.ScrollToVerticalOffset(rtb.Selection.Start.GetCharacterRect(LogicalDirection.Forward).Top);
            rtb.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
            oldsel = new TextRange(rtb.Selection.Start, rtb.Selection.End);
            rtb.SelectionChanged += rtb_SelectionChanged;            
        }

        void rtb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            oldsel.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            rtb.SelectionChanged -= rtb_SelectionChanged;
        }

       public void Replace(int start, int length, string ReplaceWith) {
           TextPointer tp = rtb.Document.ContentStart;
           TextRange tr = new TextRange(GetPoint(tp, start), GetPoint(tp, start + length));
           tr.Text = ReplaceWith;
       }

       /*private static TextPointer GetPointOld(TextPointer start, int x)
       {
           var ret = start;
           var i = 0;
           while (i < x && ret != null)
           {
               if (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text || ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
                   i++;              
               if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                   return ret;
               ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
           }
           return ret;
       }*/
       private static TextPointer GetPoint(TextPointer start, int x)
       {                      
           TextPointer ret = start.GetPositionAtOffset(x);           
           while (new TextRange(start, ret).Text.Length < x)
           {
               if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                   return ret;
               ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
           }

           return ret;
       }

       private static int GetPos(TextPointer start, TextPointer p)
       {
           return (new TextRange(start, p)).Text.Length;
       }
    }

    class WFTextBoxAdapter : IEditor
    {
        TextBoxBase tb;
        public WFTextBoxAdapter(TextBoxBase ttb)
        {
            tb = ttb;
        }

        public string Text { get { return tb.Text; } }
        public int SelectionStart { get { return tb.SelectionStart; } }
        public int SelectionLength { get { return tb.SelectionLength; } }
        public void BeginChange() {  }
        public void EndChange() { }
        public void Select(int start, int length)
        {
            tb.Select(start, length);
            tb.ScrollToCaret();
        }
        public void Replace(int start, int length, string ReplaceWith) { 
            tb.Text = tb.Text.Substring(0,start) + ReplaceWith + tb.Text.Substring(start+length); 
        }
    }

    public class IEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TextEditor)
                return new TextEditorAdapter(value as TextEditor);
            if (value is TextBox)
                return new TextBoxAdapter(value as TextBox);
            if (value is RichTextBox)
                return new RichTextBoxAdapter(value as RichTextBox);
            return null;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}