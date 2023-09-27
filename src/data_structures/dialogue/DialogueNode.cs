using System;
using System.Linq;

public abstract class DialogueNode
{
    public DialogueNode[] Next { get; set; }
    public string[] Texts { get; set; }

    protected DialogueNode()
    {
        Next = Array.Empty<DialogueNode>();
        Texts = Array.Empty<string>();
    }
    protected DialogueNode(DialogueNode[] next) : this()
    {
        Next = Next.Concat(next).ToArray();
        Texts = Array.Empty<string>();
    }
}