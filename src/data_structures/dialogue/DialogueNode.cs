using System;

public abstract class DialogueNode
{
    public DialogueNode[] Next { get; set; }
    public string[] Texts { get; set; }

    protected DialogueNode()
    {
        Next = Array.Empty<DialogueNode>();
        Texts = Array.Empty<string>();
    }

    protected DialogueNode(string[] texts, DialogueNode[] next = null) : this()
    {
        Texts = texts ?? Array.Empty<string>();
        Next = next ?? Array.Empty<DialogueNode>();
    }
}
