using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Paper.UI;

// L5 moved to this partial class from main class
public sealed partial class PaperWindow
{
    /// <summary>
    /// Removes any unfilled [form] and [signature] tags from the paper text.
    /// Called when the paper is stamped to finalize the document.
    /// </summary>
    /// <param name="text">The paper text to clean</param>
    /// <returns>Text with unfilled tags removed</returns>
    public static string CleanUnfilledTags(string text)
    {
        return text.Replace("[form]", string.Empty).Replace("[signature]", string.Empty);
    }

    /// <summary>
    /// Opens a modal dialog allowing the user to fill in a specific [form] tag.
    /// Creates a popup with text input, OK/Cancel buttons, and handles form submission.
    /// </summary>
    /// <param name="formIndex">Zero-based index of which [form] tag to replace</param>
    public void OpenFormDialog(int formIndex)
    {
        // Find and highlight the form button
        var formButton = FindFormButton(formIndex);
        if (formButton != null)
            formButton.ModulateSelfOverride = Color.LightBlue;

        // Create the popup dialog structure
        var popup = new Popup();
        var vbox = new BoxContainer { Orientation = BoxContainer.LayoutOrientation.Vertical, Margin = new Thickness(10) };
        var editContainer = new PanelContainer { StyleClasses = { "TransparentBorderedWindowPanel" } };
        var edit = new LineEdit { MinSize = new Vector2(200, 0), Margin = new Thickness(5) };
        var hbox = new BoxContainer { Orientation = BoxContainer.LayoutOrientation.Horizontal };
        var ok = new Button { Text = Loc.GetString("paper-form-dialog-ok") };
        var cancel = new Button { Text = Loc.GetString("paper-form-dialog-cancel") };

        editContainer.AddChild(edit);

        // Handle OK button press - save the form data if text was entered
        ok.OnPressed += _ =>
        {
            if (!string.IsNullOrEmpty(edit.Text))
            {
                var newText = ReplaceNthFormTag(_currentRawText, formIndex, edit.Text);
                OnSaved?.Invoke(newText);
            }
            if (formButton != null)
                formButton.ModulateSelfOverride = null;
            popup.Close();
        };

        // Handle Cancel button - just close without saving
        cancel.OnPressed += _ => {
            if (formButton != null)
                formButton.ModulateSelfOverride = null;
            popup.Close();
        };

        // Handle Enter key in text field - same as OK button
        edit.OnTextEntered += _ =>
        {
            if (!string.IsNullOrEmpty(edit.Text))
            {
                var newText = ReplaceNthFormTag(_currentRawText, formIndex, edit.Text);
                OnSaved?.Invoke(newText);
            }
            popup.Close();
        };

        // Assemble the dialog layout and show it
        hbox.AddChild(ok);
        hbox.AddChild(cancel);
        vbox.AddChild(editContainer);
        vbox.AddChild(hbox);
        popup.AddChild(vbox);
        AddChild(popup);
        popup.Open();
        edit.GrabKeyboardFocus(); // Focus the text input for immediate typing
    }

    /// <summary>
    /// Sends a signature request to the server to handle signature with proper identity system.
    /// </summary>
    /// <param name="signatureIndex">Zero-based index of which [signature] tag to replace</param>
    public void SendSignatureRequest(int signatureIndex)
    {
        OnSignatureRequested?.Invoke(signatureIndex);
    }

    /// <summary>
    /// Finds a form button by index for visual feedback.
    /// </summary>
    private Button? FindFormButton(int formIndex)
    {
        return FindButtonRecursive(WrittenTextLabel, "Fill", formIndex);
    }

    /// <summary>
    /// Finds a check button by index for visual feedback.
    /// </summary>
    private Button? FindCheckButton(int checkIndex)
    {
        return FindCheckButtonRecursive(WrittenTextLabel, checkIndex);
    }

    /// <summary>
    /// Finds check buttons (which can have different text: ☐, ✔, ✖).
    /// </summary>
    private Button? FindCheckButtonRecursive(Control control, int targetIndex)
    {
        var currentIndex = 0;
        return FindCheckButtonRecursiveHelper(control, targetIndex, ref currentIndex);
    }

    private Button? FindCheckButtonRecursiveHelper(Control control, int targetIndex, ref int currentIndex)
    {
        if (control is Button btn && (btn.Text == "☐" || btn.Text == "✔" || btn.Text == "✖"))
        {
            if (currentIndex == targetIndex)
                return btn;
            currentIndex++;
        }

        foreach (Control child in control.Children)
        {
            var result = FindCheckButtonRecursiveHelper(child, targetIndex, ref currentIndex);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Recursively searches for a button with specific text and index.
    /// </summary>
    private Button? FindButtonRecursive(Control control, string buttonText, int targetIndex)
    {
        var currentIndex = 0;
        return FindButtonRecursiveHelper(control, buttonText, targetIndex, ref currentIndex);
    }

    private Button? FindButtonRecursiveHelper(Control control, string buttonText, int targetIndex, ref int currentIndex)
    {
        if (control is Button btn && btn.Text == buttonText)
        {
            if (currentIndex == targetIndex)
                return btn;
            currentIndex++;
        }

        foreach (Control child in control.Children)
        {
            var result = FindButtonRecursiveHelper(child, buttonText, targetIndex, ref currentIndex);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Gets the current text for tag handlers to access.
    /// </summary>
    /// <returns>Current raw text content</returns>
    public string GetCurrentText()
    {
        return _currentRawText;
    }

    /// <summary>
    /// Saves text content for tag handlers to use.
    /// </summary>
    /// <param name="text">Text to save</param>
    public void SaveText(string text)
    {
        OnSaved?.Invoke(text);
    }

    private Popup? _activeCheckPopup;
    private Button? _activeCheckButton;

    /// <summary>
    /// Opens a modal dialog allowing the user to select a check state.
    /// </summary>
    /// <param name="checkIndex">Zero-based index of which [check] tag to replace</param>
    public void OpenCheckDialog(int checkIndex)
    {
        // Close any existing check popup
        if (_activeCheckPopup != null)
        {
            if (_activeCheckButton != null)
                _activeCheckButton.ModulateSelfOverride = null;
            _activeCheckPopup.Close();
        }

        // Find and highlight the check button
        var checkButton = FindCheckButton(checkIndex);
        if (checkButton != null)
            checkButton.ModulateSelfOverride = Color.LightBlue;
        _activeCheckButton = checkButton;

        var popup = new Popup();
        _activeCheckPopup = popup;
        var vbox = new BoxContainer { Orientation = BoxContainer.LayoutOrientation.Vertical, Margin = new Thickness(10) };
        var hbox = new BoxContainer { Orientation = BoxContainer.LayoutOrientation.Horizontal };

        var blankBtn = new Button { Text = "☐ Blank", MinWidth = 80 };
        var checkBtn = new Button { Text = "✔ Check", MinWidth = 80 };
        var crossBtn = new Button { Text = "✖ Cross", MinWidth = 80 };

        blankBtn.OnPressed += _ => {
            var newText = ReplaceNthCheckTag(_currentRawText, checkIndex, "☐");
            OnSaved?.Invoke(newText);
            CloseCheckDialog();
        };

        checkBtn.OnPressed += _ => {
            var newText = ReplaceNthCheckTag(_currentRawText, checkIndex, "✔");
            OnSaved?.Invoke(newText);
            CloseCheckDialog();
        };

        crossBtn.OnPressed += _ => {
            var newText = ReplaceNthCheckTag(_currentRawText, checkIndex, "✖");
            OnSaved?.Invoke(newText);
            CloseCheckDialog();
        };

        hbox.AddChild(blankBtn);
        hbox.AddChild(checkBtn);
        hbox.AddChild(crossBtn);
        vbox.AddChild(hbox);
        popup.AddChild(vbox);
        AddChild(popup);
        popup.Open();
    }

    private void CloseCheckDialog()
    {
        if (_activeCheckButton != null)
            _activeCheckButton.ModulateSelfOverride = null;
        if (_activeCheckPopup != null)
            _activeCheckPopup.Close();
        _activeCheckButton = null;
        _activeCheckPopup = null;
    }

    /// <summary>
    /// Replaces the nth occurrence of [check] tag with replacement symbol.
    /// </summary>
    private static string ReplaceNthCheckTag(string text, int index, string replacement)
    {
        const string checkTag = "[check]";
        var currentIndex = 0;
        var pos = 0;

        while (pos < text.Length)
        {
            var foundPos = text.IndexOf(checkTag, pos, StringComparison.Ordinal);
            if (foundPos == -1)
                break;

            if (currentIndex == index)
            {
                return text.Substring(0, foundPos) + replacement + text.Substring(foundPos + checkTag.Length);
            }

            currentIndex++;
            pos = foundPos + checkTag.Length;
        }

        return text;
    }

    /// <summary>
    /// Replaces the nth occurrence of [form] tag with replacement text.
    /// Uses IndexOf for efficient searching rather than splitting the entire string.
    /// </summary>
    /// <param name="text">The text containing form tags</param>
    /// <param name="index">Zero-based index of which form tag to replace</param>
    /// <param name="replacement">Text to replace the form tag with</param>
    /// <returns>Text with the specified form tag replaced, or original text if index not found</returns>
    private static string ReplaceNthFormTag(string text, int index, string replacement)
    {
        const string formTag = "[form]";
        var currentIndex = 0;
        var pos = 0;

        // Search through the text for form tags
        while (pos < text.Length)
        {
            var foundPos = text.IndexOf(formTag, pos);
            if (foundPos == -1) break; // No more tags found

            // Check if this is the tag we want to replace
            if (currentIndex == index)
            {
                // Replace this specific occurrence: text before + replacement + text after
                return text.Substring(0, foundPos) + replacement + text.Substring(foundPos + formTag.Length);
            }

            // Move to the next tag
            currentIndex++;
            pos = foundPos + formTag.Length;
        }

        // Index not found, return original text unchanged
        return text;
    }

    /// <summary>
    /// Replaces the nth occurrence of [signature] tag with replacement text.
    /// Uses IndexOf for efficient searching rather than splitting the entire string.
    /// </summary>
    /// <param name="text">The text containing signature tags</param>
    /// <param name="index">Zero-based index of which signature tag to replace</param>
    /// <param name="replacement">Text to replace the signature tag with</param>
    /// <returns>Text with the specified signature tag replaced, or original text if index not found</returns>
    private static string ReplaceNthSignatureTag(string text, int index, string replacement)
    {
        const string signatureTag = "[signature]";
        var currentIndex = 0;
        var pos = 0;

        // Search through the text for signature tags
        while (pos < text.Length)
        {
            var foundPos = text.IndexOf(signatureTag, pos);
            if (foundPos == -1) break; // No more tags found

            // Check if this is the tag we want to replace
            if (currentIndex == index)
            {
                // Replace this specific occurrence: text before + replacement + text after
                return text.Substring(0, foundPos) + replacement + text.Substring(foundPos + signatureTag.Length);
            }

            // Move to the next tag
            currentIndex++;
            pos = foundPos + signatureTag.Length;
        }

        // Index not found, return original text unchanged
        return text;
    }

    /// <summary>
    /// Counts the total number of interactive tags that create taller buttons.
    /// </summary>
    private static int CountTags(string text)
    {
        var formCount = CountOccurrences(text, "[form]");
        var signatureCount = CountOccurrences(text, "[signature]");
        var checkCount = CountOccurrences(text, "[check]");
        return formCount + signatureCount + checkCount;
    }

    /// <summary>
    /// Counts occurrences of a substring in text.
    /// </summary>
    private static int CountOccurrences(string text, string substring)
    {
        var count = 0;
        var pos = 0;
        while ((pos = text.IndexOf(substring, pos)) != -1)
        {
            count++;
            pos += substring.Length;
        }
        return count;
    }
}
