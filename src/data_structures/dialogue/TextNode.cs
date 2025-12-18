using Godot;

public partial class TextNode : DialogueNode
{
    [Signal] public delegate void TextFinishedEventHandler();

    /// <summary>
    /// Creates a new TextNode with the given text and next nodes.
    /// </summary>
    /// <param name="text">
    ///     The text for the textbox. Texts property will have one element equal to this string
    /// </param>
    /// <param name="next">
    ///     The next textbox. Next property will always have one element equal to this node
    /// </param>
    public TextNode(string text, DialogueNode next)
        : base(new[] { text }, new[] { next })
    {
    }
}
